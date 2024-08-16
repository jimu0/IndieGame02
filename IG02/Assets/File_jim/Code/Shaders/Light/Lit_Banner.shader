
Shader "StarUnion/Light/Banner" {
    Properties {
		_Color("Color", Color) = (0.59, 0.59, 0.59, 1)
		_Lighting("Lighting", float) = 1
		_MainTex("Main Texture", 2D) = "white" {}
		[Toggle(_DIFFUSEON)] _DiffuseOn("Enable Diffuse", Float) = 1
		[Space(5)]
		[Toggle(_SPECULARON)] _SpecularOn("Enable Specular", Float) = 0
		_SpecularColor("Specular Color", Color) = (0.59, 0.59, 0.59, 1)
		_SpecularPower("Specular Power", Range(0, 1)) = 1
		_SpecularRange("Specular Range", Range(0, 1)) = 0.6
		[Space(5)]
		[Toggle(_MATCAPON)] _MatcapOn("Enable Matcap", Float) = 0
		_Matcap("Matcap", 2D) = "white" {}
		_MatcapPower("Matcap Power", float) = 1
		[Space(5)]
		[Toggle(_EMISSIONON)] _EmissionOn("Enable Emission", Float) = 0
		_EmissionPower("Emission Power", float) = 1
		[Space(5)]
		[Toggle(_LIGHTMAPON)]_LightmapOn("Enable Lightmap", float) = 0
		_LightMapColor("LightMap Color", Color) = (0.26,0.26,0.26,1)
		_LightMap("LightMap", 2D) = "white" {}
		[Toggle(_LINEARSPACE)]_LinearSpace("Linear Space", float) = 0
		[Space(5)]
		_VectorTexture ("Vector Texture", 2D) = "white" {}
		_Mask ("Mask", 2D) = "white" {}
        _VectorOffset ("Vector Offset", Vector) = (0,0,0,0)
        _DistortPower ("Distort Power", Range(0, 1)) = 0.5
        _SmallDistortValue ("Small Distort Value", Range(0, 1)) = 0
		[Space(5)]
		[Toggle(_FOGON)] _FogOn("Enable Fog", Float) = 0
		[Space(5)]
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2
		_Cutoff("Alpha Cutoff", Range(0, 1)) = 0.5
    }

	SubShader {
		Tags {
			"RenderType" = "Opaque" 
			"RenderPipeline" = "UniversalPipeline" 
			"UniversalMaterialType" = "Lit" 
			"IgnoreProjector" = "True" 
		}

		Pass {
			Name "BaseForward"
			Tags{ "LightMode" = "UniversalForward" }

			Cull[_Cull]

			HLSLPROGRAM
			#pragma vertex LitPassVertex
			#pragma fragment LitPassFragment
			#pragma shader_feature _FOGON
			#pragma shader_feature _DIFFUSEON
			#pragma shader_feature _SPECULARON
			#pragma shader_feature _MATCAPON
			#pragma shader_feature _EMISSIONON
			#pragma shader_feature _LIGHTMAPON
			#pragma shader_feature _LINEARSPACE
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile_fog
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"


			
			TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
		#if _MATCAPON
			TEXTURE2D(_Matcap);
            SAMPLER(sampler_Matcap);
		#endif
		#if defined (_LIGHTMAPON) && !(LIGHTMAP_ON)
			TEXTURE2D(_LightMap);
            SAMPLER(sampler_LightMap);
		#endif
			TEXTURE2D(_VectorTexture);
            SAMPLER(sampler_VectorTexture);
			TEXTURE2D(_Mask);
            SAMPLER(sampler_Mask);

			//当有不同变体被使用时，CBuffer内去需要掉变体开关才能用SRP Batcher，否则提示CBuffer不一致
			CBUFFER_START(UnityPerMaterial)
			float4 _Color;
			float _Lighting;
			float4 _MainTex_ST;
		//#if _MATCAPON
			float _MatcapPower;
		//#endif
		//#if _SPECULARON
			float _SpecularRange;
			float4 _SpecularColor;
			float _SpecularPower;
		//#endif
		//#if _EMISSIONON
      		float _EmissionPower;
		//#endif
		//#if defined (_LIGHTMAPON)
			half4 _LightMap_ST;
		  //#if !(LIGHTMAP_ON)
			half4 _LightMapColor;
		  //#endif
		//#endif
			float4 _Mask_ST;
			float4 _VectorTexture_ST;
			float4 _VectorOffset;
			float _DistortPower;
			float _SmallDistortValue;
			float _Cutoff;
			CBUFFER_END


			struct Attributes
			{
				float4 positionOS : POSITION;
				float2 texcoord : TEXCOORD0;
				float2 lightmapUV : TEXCOORD1;
				float3 normalOS : NORMAL;
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float4 texcoord : TEXCOORD0;
			#if _LIGHTMAPON
				float2 lightmapUV : TEXCOORD1;
			#endif
			#if defined (_DIFFUSEON) || (_SPECULARON) || (LIGHTMAP_ON)
				float3 normalWS : TEXCOORD2;
			#endif
			#if _SPECULARON
				float3 viewDirWS : TEXCOORD3;
			#endif
			#if _FOGON
				float fogCoord : TEXCOORD4;
			#endif
			};


			Varyings LitPassVertex(Attributes input)
			{
				Varyings output;
				
				//vertex animation
				float4 mask = SAMPLE_TEXTURE2D_LOD(_Mask, sampler_Mask, TRANSFORM_TEX(input.texcoord, _Mask), 0);
				float2 vecUV1 = input.texcoord * 0.3 + _Time.y * float2(-0.07, 0.05);
				float4 vecTex1 = SAMPLE_TEXTURE2D_LOD(_VectorTexture, sampler_VectorTexture, TRANSFORM_TEX(vecUV1, _VectorTexture), 0);
				float2 vecUV2 = input.texcoord + _Time.y * float2(-0.15, 0);
				float4 vecTex2 = SAMPLE_TEXTURE2D_LOD(_VectorTexture, sampler_VectorTexture, TRANSFORM_TEX(vecUV2, _VectorTexture), 0);
				input.positionOS.xyz += mask.rgb * (_VectorOffset.xyz + (_DistortPower*2 * vecTex1.rgb + vecTex2.rgb * _SmallDistortValue));

				output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
				output.texcoord.xy = TRANSFORM_TEX(input.texcoord, _MainTex);
			#if _MATCAPON
				output.texcoord.z = dot(normalize(UNITY_MATRIX_IT_MV[0].xyz), normalize(input.normalOS));
				output.texcoord.w = dot(normalize(UNITY_MATRIX_IT_MV[1].xyz), normalize(input.normalOS));
				output.texcoord.zw = output.texcoord.zw * 0.5 + 0.5;
			#else
				output.texcoord.zw = half2(0, 0);
			#endif
			#if _LIGHTMAPON
			  #if LIGHTMAP_ON
				output.lightmapUV = input.lightmapUV * unity_LightmapST.xy + unity_LightmapST.zw;
			  #else
				output.lightmapUV = TRANSFORM_TEX(input.lightmapUV, _LightMap);
			  #endif
			#endif
			#if defined (_DIFFUSEON) || (_SPECULARON) || (LIGHTMAP_ON)
				output.normalWS = TransformObjectToWorldNormal(input.normalOS);
			#endif
			#if _SPECULARON
				float3 worldPos = TransformObjectToWorld(input.positionOS.xyz);
				output.viewDirWS = TransformWorldToView(worldPos);
			#endif

			#if _FOGON
				output.fogCoord = ComputeFogFactor(output.positionCS.z);
			#endif

				return output;
			}

			float4 LitPassFragment(Varyings input) : SV_Target
			{
				//albedo										 
				float4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.texcoord.xy);

			#if defined (_DIFFUSEON) || (_SPECULARON) || (LIGHTMAP_ON)
				Light mainLight = GetMainLight();
				float3 normalDirection = NormalizeNormalPerPixel(input.normalWS);
				float3 lightDirection = normalize(mainLight.direction);    //light direction
			#endif

			#if _SPECULARON
				float3 viewDirection = SafeNormalize(input.viewDirWS);    //view direction
				float3 h = normalize(lightDirection + viewDirection);
				float nh = saturate(dot(normalDirection, h));
				float specRange = exp2(_SpecularRange * 10.0 + 1.0);
				specRange = pow(max(0, nh), specRange);
				float3 specular = specRange * _SpecularColor.rgb * _SpecularPower*10;
			#endif

			#if _DIFFUSEON
				float nl = saturate(dot(normalDirection, lightDirection));
				float3 lightColor = mainLight.color;
				float3 diffuse = albedo.rgb * nl * lightColor;
			#endif

				//final
				float4 finalColor;
			#if _LIGHTMAPON
				finalColor.rgb = albedo.rgb;
			#else
				finalColor.rgb = albedo.rgb * _GlossyEnvironmentColor.rgb;
			#endif
				//add diffuse
			#if _DIFFUSEON
				finalColor.rgb += diffuse;
			#endif
				//add specular
			#if _SPECULARON
				finalColor.rgb += specular;
			#endif
			#if _MATCAPON
				float3 matcap = SAMPLE_TEXTURE2D(_Matcap, sampler_Matcap, input.texcoord.zw).rgb * _MatcapPower;
				finalColor.rgb *= matcap;
			#endif
				//add emission
			#if _EMISSIONON
				float3 emission = albedo.rgb;
				finalColor.rgb += emission * _EmissionPower;
			#endif
				//add lightmap
				half3 lm;
			#if _LIGHTMAPON
			  #if LIGHTMAP_ON
				lm = SampleLightmap(input.lightmapUV, normalDirection);
				finalColor.rgb *= lm;
			  #else
				lm = SAMPLE_TEXTURE2D(_LightMap, sampler_LightMap, input.lightmapUV).rgb;
				lm = min(lm + _LightMapColor.rgb, 1);
			#if _LINEARSPACE
				lm = pow(abs(lm.rgb), 2.2);
			#endif
				finalColor.rgb *= lm.rgb;
			  #endif
			#endif
				finalColor.rgb *= _Color.rgb * _Lighting;
				finalColor.a = albedo.a;

				clip(finalColor.a - _Cutoff);

			#if _FOGON
				finalColor.rgb = MixFog(finalColor.rgb, input.fogCoord);
			#endif

				return finalColor;
			}
			ENDHLSL
		}
	}
	//CustomEditor "LitbaseShaderGUI"
}
