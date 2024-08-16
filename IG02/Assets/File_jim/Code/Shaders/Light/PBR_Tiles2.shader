
Shader "StarUnion/PBR/Tiles2" {

	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Lighting ("Lighting",  float) = 1
		_Tiling("Tiling", vector) = (1,1,1,1)
		_MainTex ("Main Texture", 2D) = "white" {}
		_Layer ("Layer", 2D) = "white" {}
		_BumpMap ("Normal Map", 2D) = "bump" {}
        _NormalScale("Normal Scale", float) = 1
		_MetallicValue("Metallic Value", Range(0, 1)) = 0
        _Smoothness("Smoothness", Range(0, 1)) = 0.5
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
			TEXTURE2D(_Layer);
            SAMPLER(sampler_Layer);
			TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);
			// TEXTURE2D(_Mask);
            // SAMPLER(sampler_Mask);

			CBUFFER_START(UnityPerMaterial)
			half4 _Color;
			float _Lighting;
			half4 _Tiling;
			float _NormalScale;
			float _MetallicValue;
            float _Smoothness;
			CBUFFER_END

			//NDF，Normal Distribution Function，法线分布函数
            float GetDTerm(float nh, float a2)
            {
                float d = nh * nh * (a2 - 1) + 1;
                return a2 / (3.14 * d * d + 1e-7f);
            }
            //Geometry function，几何函数
            float GetGTerm(float nl, float nv, float roughness)
            {
                float k = (roughness+1) * (roughness+1) / 8;
                float gl = nl / lerp(nl, 1, k);
                float gv = nv / lerp(nv, 1, k);
                return gl * gv;
            }
            //Fresnel，Fresnel equation，菲涅尔方程
            float GetFLerpValue(float cosA)
            {
                return exp2((-5.55473 * cosA - 6.98316) * cosA);  //ue拟合方法节省计算量，unity方法：pow(1 - cosA, 5)
            }
            float3 GetFTerm(float3 f0, float cosA)
            {
                float t = GetFLerpValue(cosA);
                return f0 + (1 - f0) * t;
            }
            float3 GetIndirectFTerm(float3 f0, float cosA, float roughness)
            {
                float t = GetFLerpValue(cosA);
                return f0 + saturate(1 - f0 - roughness) * t;
            }
            float3 GetIndirectSpecFLerp(float3 f0, float3 f90, float cosA)
            {
                float t = GetFLerpValue(cosA);
                return lerp(f0, f90, t);
            }

            half3 LightingPBR(half4 albedo, half3 lightColor, half3 lightDirWS, half lightAtten, half3 normalWS, half3 viewDirWS, half metallic, half roughness, half indirectMask)
            {
                half nl = max(saturate(dot(normalWS, lightDirWS)), 1e-6f);
                half3 h = normalize(lightDirWS + viewDirWS);
                half nh = max(saturate(dot(normalWS, h)), 1e-6f);
                half nv = max(saturate(dot(normalWS, viewDirWS)), 1e-6f);
                half hl = max(saturate(dot(h, lightDirWS)), 1e-6f);

                float a2 = roughness * roughness;
                float a4 = max(a2 * a2, 1e-4f);

                //直接光照
                //F_cook-torrance
                float D = GetDTerm(nh, a4);  //此处根据效果用a4取代a2
                float G = GetGTerm(nl, nv, roughness);
                float3 fresnelColor;
            #ifdef UNITY_COLORSPACE_GAMMA
                fresnelColor = half3(0.22, 0.22, 0.22);
            #else
                fresnelColor = half3(0.04, 0.04, 0.04);
            #endif
                float3 F0 = lerp(fresnelColor, albedo.rgb, metallic);
                float3 F = GetFTerm(F0, hl);
                float3 specular = D * G * F * 0.25 * 3.14 / (nl * nv);  //未乘nl效果是不对的

                //F_lambert
                float3 KS = F;
                float3 KD = (1 - KS) * (1 - metallic);
                float3 diffuse = KD * albedo.rgb;

                

                half3 directColor = (diffuse + specular) * lightColor * lightAtten * nl;

                //间接光照
                //F_lambert Indirect
                float3 sh = SampleSH(normalWS);
                float3 indirectF = GetIndirectFTerm(F0, nv, roughness);
                float3 indirectKS = indirectF;
                float3 indirectKD = (1 - indirectKS) * (1 - metallic);
                float3 indirectDiffuse = sh * indirectKD * albedo.rgb;

                //F_cook-torrance Indirect
                //environment cube
                float3 relUVW = reflect(-viewDirWS, normalWS);
                float roughnessMip = roughness * (1.7 - 0.7 * roughness);  //接近实际值的拟合曲线算法
                float mip = roughnessMip * 6;  //把粗糙度remap到0~6，7个阶级
                half4 rgbm = SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0, samplerunity_SpecCube0, relUVW, mip);
                //environment cube factor
                float surfaceReduction;
			#ifdef UNITY_COLORSPACE_GAMMA
				surfaceReduction = 1 - 0.28 * a2;
			#else
				surfaceReduction = 1 / (a2 + 1);
			#endif
                float reflectivity = max( max(specular.r, specular.g), specular.b );
                float grazingTerm = saturate((1 - roughness) + reflectivity);
                half3 indirectSpecFactor = surfaceReduction * GetIndirectSpecFLerp(F0, grazingTerm, nv);  //曲线拟合，代替采样BRDF LUT
                half3 indirectSpecColor = DecodeHDREnvironment(rgbm, unity_SpecCube0_HDR);  //将颜色从HDR编码下解码
                half3 indirectSpecular = indirectSpecColor * indirectSpecFactor;

                half3 indirectColor = (indirectDiffuse + indirectSpecular) * indirectMask;  // * ao

                return directColor + indirectColor;
            }
            half3 LightingPBR(half4 albedo, Light light, half3 normalWS, half3 viewDirWS, half metallic, half roughness, half indirectMask)
            {
                return LightingPBR(albedo, light.color, light.direction, light.distanceAttenuation * light.shadowAttenuation, normalWS, viewDirWS, metallic, roughness, indirectMask);
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
				float metallic = _MetallicValue;
                float roughness = 1 - _Smoothness;

				//albedo
				//float4 mask = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.uv2.xy);
				float4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv.xy * _Tiling.x);
				float4 layer = SAMPLE_TEXTURE2D(_Layer, sampler_Layer, i.uv2.xy * _Tiling.y);
    			float4 albedo;
				albedo.rgb = lerp(mainTex.rgb, layer.rgb, layer.a);
				albedo.a = mainTex.a;

				//direction
				half3 normTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, i.uv2.xy), _NormalScale);
				half3 normalTS = lerp(float3(0,0,1), normTS, layer.a);
                half3 normalWS = TransformTangentToWorld(normalTS, half3x3(i.tangentWS.xyz, i.bitangentWS.xyz, i.normalWS.xyz));
                half3 viewDirWS = SafeNormalize(half3(i.normalWS.w, i.tangentWS.w, i.bitangentWS.w));

                //main light
                float4 shadowCoord = TransformWorldToShadowCoord(i.positionWS.xyz);  //获取阴影坐标
                Light mainLight = GetMainLight(shadowCoord);

				half4 final;
                final.rgb = LightingPBR(albedo, mainLight, normalWS, viewDirWS, metallic, roughness, 1);
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