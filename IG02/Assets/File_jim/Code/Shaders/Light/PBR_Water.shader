Shader "StarUnion/PBR/Water"
{
    Properties
    {
        _Metallic ("Metallic", Range(0, 1)) = 0
        _Gloss ("Gloss", Range(0, 1)) = 0.8
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _Normal ("Normal", 2D) = "bump" {}
        _NormalPower ("Normal Power", Range(0, 1)) = 0.5
        _WaveScale ("Wave Scale", Range(0, 1)) = 0.5
        [Enum(Off, 0, On, 1)] _ZWrite("ZWrite", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend Mode", Float) = 0
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2
    }
    SubShader
    {
        Tags 
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Tags
            {
                "LightMode" = "UniversalForward"
            }
            
            ZWrite[_ZWrite]
			Blend[_SrcBlend][_DstBlend]
			Cull[_Cull]
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 texcoord0 : TEXCOORD0;
                // float2 texcoord1 : TEXCOORD1;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv1 : TEXCOORD0;
                // float2 uv2 : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float3 normalWS : TEXCOORD3;
                float3 tangentWS : TEXCOORD4;
                float3 bitangentWS : TEXCOORD5;
                float3 viewDirWS : TEXCOORD6;
            };

            TEXTURE2D(_MainTex);
            TEXTURE2D(_Normal);
            SAMPLER(sampler_MainTex);
            SAMPLER(sampler_Normal);
            
            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST, _Normal_ST;
            float4 _Color;
            float _Metallic, _Gloss, _NormalPower,_WaveScale;
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


            
            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.tangentWS = normalize(TransformObjectToWorldDir(IN.tangentOS.xyz));
                real sign = IN.tangentOS.w * unity_WorldTransformParams.w;
                OUT.bitangentWS = normalize(cross(OUT.normalWS, OUT.tangentWS) * sign);
                OUT.viewDirWS = normalize(_WorldSpaceCameraPos - OUT.positionWS);
                OUT.uv1 = TRANSFORM_TEX(IN.texcoord0, _MainTex);
                return OUT;
            }

            float4 frag (Varyings IN) : SV_Target
            {
                float metallic = _Metallic;
                float roughness = 1 - _Gloss;
                
                //Shadow相关
                #ifdef MAIN_LIGHT_CALCULATE_SHADOWS
                    float4 shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
                #else
                    float4 shadowCoord = float4(0, 0, 0, 0);
                #endif
                Light mainLight = GetMainLight(shadowCoord);
                float shadow = mainLight.shadowAttenuation * mainLight.distanceAttenuation;
                
                IN.normalWS = normalize(IN.normalWS);
                float3x3 tangentTransform = float3x3(IN.tangentWS, IN.bitangentWS, IN.normalWS);

                //水波纹法线贴图计算部分
                float customTime = _Time.y;
                float2 WS_UV = float2(IN.positionWS.r, IN.positionWS.b) * (_WaveScale * 1.9 + 0.1);
                //第一层
                float2 water_uv1 = WS_UV * 0.2 + customTime * float2(0.05, 0.05);
                float3 waterNormal1 = UnpackNormal(SAMPLE_TEXTURE2D(_Normal, sampler_Normal, water_uv1));
                float2 water_uv2 = WS_UV * 0.25 + customTime * float2(-0.04, -0.06);
                float3 waterNormal2 = UnpackNormal(SAMPLE_TEXTURE2D(_Normal, sampler_Normal, water_uv2));
                float3 normalBase1 = waterNormal1 + float3(0, 0, 1);
                float3 normalDetail1 = waterNormal2 * float3(-1, -1, 1);
                float3 finalNormal1 = normalBase1 * dot(normalBase1, normalDetail1) / normalBase1.z - normalDetail1;
                //第二层
                float2 water_uv3 = WS_UV * 0.18 + customTime * float2(0.04, -0.06);
                float3 waterNormal3 = UnpackNormal(SAMPLE_TEXTURE2D(_Normal, sampler_Normal, water_uv3));
                float2 water_uv4 = WS_UV * 0.25 + customTime * float2(-0.06, 0.03);
                float3 waterNormal4 = UnpackNormal(SAMPLE_TEXTURE2D(_Normal, sampler_Normal, water_uv4));
                float3 normalBase2 = waterNormal3 + float3(0, 0, 1);
                float3 normalDetail2 = waterNormal4 * float3(-1, -1, 1);
                float3 finalNormal2 = normalBase2 * dot(normalBase2, normalDetail2) / normalBase2.z - normalDetail2;
                //法线合并
                float3 finalBaseNormal = finalNormal1 + float3(0, 0, 1);
                float3 finalDetailNormal = finalNormal2 * float3(-1, -1, 1);
                float3 normal = finalBaseNormal * dot(finalBaseNormal, finalDetailNormal) / finalBaseNormal.z - finalDetailNormal;
                normal = normalize(lerp(float3(0, 0, 1), normal, _NormalPower));
                normal = normalize(mul(normal, tangentTransform));
                //水波纹法线贴图计算部分结束
                
                float4 BaseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv1);
                float4 diffuseColor = BaseColor * _Color;

                float4 finalColor;
                finalColor.rgb = LightingPBR(diffuseColor, mainLight, normal, IN.viewDirWS, metallic, roughness,1);
                finalColor.a = 1;
                
                return finalColor;
            }
            ENDHLSL
        }
        UsePass "StarUnion/Shadow/ShadowCaster/ShadowCaster"
    }
}
