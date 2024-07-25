
Shader "StarUnion/Light/Gem" {
    Properties {
		[Header(Main)][Space(5)]
		_Color("Color", Color) = (0.59, 0.59, 0.59, 1)
		_AmbientColor("Ambient Color", Color) = (0.59, 0.59, 0.59, 1)
		_MainTex("Main Texture", 2D) = "white" {}
		[Header(Specular)][Space(5)]
		_SpecularColor("Specular Color", Color) = (0.59, 0.59, 0.59, 1)
		_SpecularPower("Specular Power", Range(0, 1)) = 1
		_SpecularRange("Specular Range", Range(0, 1)) = 0.6
		[Header(Matcap)][Space(5)]
		_Matcap("Matcap", 2D) = "white" {}
		_MatcapPower("Matcap Power", float) = 0.2
		[Header(Rim)][Space(5)]
        _RimColor("Rim Color", Color) = (1,1,1,0.3)
		_RimPower("Rim Power", Range(0, 1)) = 1
        _RimRange("Rim Range", Range(1, 8)) = 5
		[Header(Stramer)][Space(5)]
		_StramerColor("Stramer Color", Color) = (1,1,1,1)
		_Streamer("Streamer", 2D) = "white" {}
		_UvVector("UvMove Vector", vector) = (0, 0, 1, 1)
		_UvRotation("Uv Rotation", Range(0, 3.14)) = 0
		[Header(Advanced Options)][Space(5)]
		_LightDirection("Light Direction", vector) = (1, 0, 1, 1)
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2
    }

	SubShader {
		Tags {
			"RenderType" = "Opaque" 
			"RenderPipeline" = "UniversalPipeline" 
			"UniversalMaterialType" = "Lit" 
			"IgnoreProjector" = "True" 
		}

		Pass {
			Tags{ "LightMode" = "UniversalForward" }

			Cull[_Cull]

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
			TEXTURE2D(_Matcap);
            SAMPLER(sampler_Matcap);
			TEXTURE2D(_Streamer);
            SAMPLER(sampler_Streamer);

			CBUFFER_START(UnityPerMaterial)
			float4 _Color;
			float4 _AmbientColor;
			float4 _MainTex_ST;

			float _MatcapPower;

			float _SpecularRange;
			float4 _SpecularColor;
			float _SpecularPower;

			half4 _RimColor;
			float _RimPower;
            float _RimRange;

			half4 _StramerColor;
			half4 _UvVector;
			float _UvRotation;

			half4 _LightDirection;
			CBUFFER_END


			struct a2v
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 uv0 : TEXCOORD0;
				float3 normalDir : TEXCOORD2;
				float3 viewDir : TEXCOORD3;
			};


			v2f vert(a2v v)
			{
				v2f o;

				o.pos = TransformObjectToHClip(v.vertex.xyz);
				o.uv0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);

				o.uv0.z = dot(normalize(UNITY_MATRIX_IT_MV[0].xyz), normalize(v.normal));
				o.uv0.w = dot(normalize(UNITY_MATRIX_IT_MV[1].xyz), normalize(v.normal));
				o.uv0.zw = o.uv0.zw * 0.5 + 0.5;

				o.normalDir = TransformObjectToWorldNormal(v.normal);

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.viewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);

				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				//albedo										 
				float4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv0.xy);

				float3 normalDirection = normalize(i.normalDir);
				float3 lightDirection = normalize(-_LightDirection.xyz);    //light direction
				float3 viewDirection = SafeNormalize(i.viewDir);    //view direction

				float nl = saturate(dot(normalDirection, lightDirection));
				float3 h = normalize(lightDirection + viewDirection);
				float nh = saturate(dot(normalDirection, h));
				half nv = saturate(dot(normalDirection, viewDirection));

				//diffuse
				float3 diffuse = lerp(_AmbientColor.rgb, albedo.rgb, nl);

	       		//specular
				float specRange = exp2(_SpecularRange * 10.0 + 1.0);
				specRange = pow(max(0, nh), specRange);
				float3 specular = specRange * _SpecularColor.rgb * _SpecularPower*10;
				
				//matcap
				float3 matcap = SAMPLE_TEXTURE2D(_Matcap, sampler_Matcap, i.uv0.zw).rgb * _MatcapPower;

				//rim
                half rimRange = pow(1 - nv, _RimRange);
                half3 rim = _RimColor.rgb * rimRange * _RimPower * 10;

				//final
				float4 finalColor;
				finalColor.rgb = (diffuse + specular + rim) * matcap;
				finalColor.rgb *= _Color.rgb * _LightDirection.w;
				finalColor.a = 1;

				//streamer
				half2 streamerUv = i.uv0.xy - half2(0.5, 0.5);
				streamerUv = half2( streamerUv.x*cos(_UvRotation)-streamerUv.y*sin(_UvRotation),streamerUv.y*cos(_UvRotation) + streamerUv.x*sin(_UvRotation) );
				half3 streamer = SAMPLE_TEXTURE2D(_Streamer, sampler_Streamer, streamerUv * _UvVector.zw + _UvVector.xy * _Time.xy).rgb * _StramerColor.rgb;
				finalColor.rgb += streamer;

				return finalColor;
			}
			ENDHLSL
		}
	}
}
