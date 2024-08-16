// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Unlit/Lit_Lambert"
{
    Properties
    {
        _Diffuse ("Diffuse", Color) = (0.5,0.5,0.5,1)
        _Specular ("Specular",Color) = (0.25,0.25,0.25,1)
        _Gloss ("Gloss",Range(8.0,256)) = 20
    }
    SubShader
    {
        Tags 
        {
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline" 
            //"UniversalMaterialType" = "Lit" 
            //"IgnoreProjector" = "True" 
            //"ShaderModel"="4.5"
        }
        LOD 100
        Pass
        {
            Name "Lit_Lambert"
            Tags{"LightMode"="UniversalForward"}
            //Cull Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag        
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            half4 _Diffuse;
            half4 _Specular;
            float _Gloss;
            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };
            v2f vert (a2v v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                //o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);
                o.worldNormal = TransformObjectToWorldNormal(v.normal);
                o.worldPos = TransformObjectToWorld(v.vertex.xyz);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {           
                half3 ambient =  UNITY_LIGHTMODEL_AMBIENT.xyz;
                half3 worldNormal = normalize(i.worldNormal);
                half3 worldLightDir = normalize(_MainLightPosition.xyz);
                //half3 diffuse = _MainLightColor.rgb*_Diffuse.rgb*saturate(dot(worldNormal,worldLightDir)*0.5+0.5);//°ëÀ¼²®ÌØ
                half3 diffuse = _MainLightColor.rgb*_Diffuse.rgb*saturate(dot(worldNormal,worldLightDir));//À¼²®ÌØ
                half3 reflectDir = normalize(reflect(-worldLightDir,worldNormal));
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz-i.worldPos.xyz);
                half3 halfDir = normalize(worldLightDir+viewDir);
                half3 specular = _MainLightColor.rgb*_Specular.rgb*pow(max(0,dot(worldNormal,halfDir)),_Gloss);
                return half4(ambient+diffuse+specular,1.0);
            }
            ENDHLSL
            
            
        }
       
    }
    //FallBack "Specular"
}
