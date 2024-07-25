
Shader "StarUnion/Light/Tiles3" {

	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Lighting ("Lighting",  float) = 1
		_Tiling("Tiling", vector) = (1,1,1,1)
		_MainTex ("Main Texture", 2D) = "white" {}
		_Layer1 ("Layer1", 2D) = "white" {}
		_Layer2 ("Layer2", 2D) = "white" {}
		_BumpMap1 ("Normal Map1", 2D) = "bump" {}
		_BumpMap2 ("Normal Map2", 2D) = "bump" {}
        _NormalScale("Normal Scale", float) = 1
		[Header(rgb_Color    a_Intensity)]
		_SpecularColor ("Specular Color", Color) = (1,1,1,0.3)
        _SpecularRange("Specular Range", Range(0, 1)) = 0.2
		//_Mask ("Mask", 2D) = "white" {}
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2
	}

	SubShader {
		Tags { 
			"Queue" = "Geometry"
			"IgnoreProjector" = "True"
			"RenderType" = "Opaque"
			"RenderPipeline" = "UniversalPipeline"
		}

		Pass {
		    Name "BASECOMBIE"
			Tags { "LightMode" = "UniversalForward" }

		   	Cull[_Cull]
 
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"


			TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
			TEXTURE2D(_Layer1);
            SAMPLER(sampler_Layer1);
			TEXTURE2D(_Layer2);
            SAMPLER(sampler_Layer2);
			TEXTURE2D(_BumpMap1);
            SAMPLER(sampler_BumpMap1);
			TEXTURE2D(_BumpMap2);
            SAMPLER(sampler_BumpMap2);
			// TEXTURE2D(_Mask);
            // SAMPLER(sampler_Mask);

			CBUFFER_START(UnityPerMaterial)
			half4 _Color;
			float _Lighting;
			half4 _Tiling;
			float _NormalScale;
			half4 _SpecularColor;  //rgb: color, a: intensity
            float _SpecularRange;
			CBUFFER_END

			//计算漫反射与高光
            half3 LightingBased(half4 albedo, half3 lightColor, half3 lightDirWS, half lightAtten, half3 normalWS, half3 viewDirWS, half2 specRangePower)
            {
                //lambert
                half nl = saturate(dot(normalWS, lightDirWS));
                half3 diffuse = lightColor * (lightAtten * nl);  // * _DiffuseColor.rgb;

                //blinn
                half3 h = normalize(lightDirWS + viewDirWS);
                half nh = saturate(dot(normalWS, h));
                float specularRange = exp2(max(3, specRangePower.x * 8));
                float specularPower = specRangePower.y * 10;
                half3 specular = lightColor * pow(nh, specularRange) * _SpecularColor.rgb * specularPower;

                return (diffuse + specular) * albedo.rgb;
            }
            half3 LightingBased(half4 albedo, Light light, half3 normalWS, half3 viewDirWS, half2 specRangePower)
            {
                return LightingBased(albedo, light.color, light.direction, light.distanceAttenuation * light.shadowAttenuation, normalWS, viewDirWS, specRangePower);
            }


			struct a2v
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float3 normal : NORMAL;
                float4 tangent : TANGENT;
			};

			struct v2f {
				half4 pos : SV_POSITION;
				half3 uv: TEXCOORD0;
				half2 uv2 : TEXCOORD1;
				half4 normalWS : TEXCOORD3;  // xyz: normal, w: viewDir.x
                half4 tangentWS : TEXCOORD4;  // xyz: tangent, w: viewDir.y
                half4 bitangentWS : TEXCOORD5;  // xyz: bitangent, w: viewDir.z
				half3 positionWS : TEXCOORD6;
			};


			v2f vert (a2v v)
			{
				v2f o;
				o.pos = TransformObjectToHClip(v.vertex.xyz);
				o.uv.xy = v.texcoord;
				o.uv.z = ComputeFogFactor(o.pos.z);
				o.uv2 = v.texcoord1;

				o.positionWS = TransformObjectToWorld(v.vertex.xyz);
				half3 viewDirWS = TransformWorldToView(o.positionWS);
				o.normalWS = half4(TransformObjectToWorldNormal(v.normal), viewDirWS.x);
                o.tangentWS = half4(TransformObjectToWorldDir(v.tangent.xyz), viewDirWS.y);
                half3 bitanWS = cross(o.normalWS.xyz, o.tangentWS.xyz) * v.tangent.w * GetOddNegativeScale();
                o.bitangentWS = half4(bitanWS, viewDirWS.z);
				    
				return o;
			}

			float4 frag (v2f i) : COLOR
			{
				//albedo
				//float4 mask = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.uv2.xy);
				float4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv.xy * _Tiling.x);
				float4 layer1 = SAMPLE_TEXTURE2D(_Layer1, sampler_Layer1, i.uv2.xy * _Tiling.y);
				float4 layer2 = SAMPLE_TEXTURE2D(_Layer2, sampler_Layer2, i.uv2.xy * _Tiling.z);
    			float4 albedo;
				albedo.rgb = lerp(mainTex.rgb, layer1.rgb, layer1.a);
				albedo.rgb = lerp(albedo.rgb, layer2.rgb, layer2.a);
				albedo.a = mainTex.a;

				//direction
				half3 normTS1 = UnpackNormalScale(SAMPLE_TEXTURE2D(_BumpMap1, sampler_BumpMap1, i.uv2.xy), _NormalScale);
				half3 normTS2 = UnpackNormalScale(SAMPLE_TEXTURE2D(_BumpMap2, sampler_BumpMap2, i.uv2.xy), _NormalScale);
				half3 normalTS = lerp(float3(0,0,1), normTS1, layer1.a);
				normalTS = lerp(normalTS, normTS2, layer2.a);
                half3 normalWS = TransformTangentToWorld(normalTS, half3x3(i.tangentWS.xyz, i.bitangentWS.xyz, i.normalWS.xyz));
                half3 viewDirWS = SafeNormalize(half3(i.normalWS.w, i.tangentWS.w, i.bitangentWS.w));

				//ambient
                half3 ambient = _GlossyEnvironmentColor.rgb * albedo.rgb;  //Unity内置环境光

                //main light
                float4 shadowCoord = TransformWorldToShadowCoord(i.positionWS.xyz);  //获取阴影坐标
                Light mainLight = GetMainLight(shadowCoord);

				//secular power
                half2 specRangePower;
                specRangePower.x = _SpecularRange;
                specRangePower.y = lerp(0, _SpecularColor.a, layer1.a);

				half4 final;
                final.rgb = LightingBased(albedo, mainLight, normalWS, viewDirWS, specRangePower) + ambient;
                final.a = albedo.a;
				final.rgb *= _Color.rgb * _Lighting;

				final.rgb = MixFog(final.rgb, i.uv.z);

				return final;
			}
			ENDHLSL
		}

		UsePass "StarUnion/Shadow/ShadowCaster/ShadowCaster"
	}
} 