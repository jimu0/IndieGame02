Shader "StarUnion/Other/PlanarShadow"
{
    Properties
    {
        _ShadowColor ("Shadow Color", Color) = (0,0,0,1)
        _ShadowPlanePosY("Shadow Position Y", float) = 0
        _ShadowFade("Shadow Fade", float) = 0.2
    }

    SubShader
    {
        Pass
        {
            Name "PlanarShadow"
            Tags
            { 
                "Queue" = "Transparent"
                //"LightMode" = "SRPDefaultUnlit"  
                "RenderPipeline" = "UniversalPipeline"
            }
            Stencil
            {
                Ref 0
                Comp equal
                Pass incrWrap
                Fail Keep
                ZFail Keep
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Zwrite Off
            Offset -1, 0

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"


            CBUFFER_START(UnityPerMaterial)
            half4 _ShadowColor;
            float _ShadowPlanePosY;
            float _ShadowFade;
            CBUFFER_END

            float3 ShadowProjCoord(float4 positionOS, float3 lightDirection)
            {
                float3 PositionWS = mul(unity_ObjectToWorld, positionOS).xyz;

                //阴影的世界空间坐标（低于地面的部分不做改变）
                float3 shadowCoord;
                shadowCoord.y = min(PositionWS.y, _ShadowPlanePosY);
                shadowCoord.xz = PositionWS.xz - lightDirection.xz * max(0, PositionWS.y - _ShadowPlanePosY) / lightDirection.y; 

                return shadowCoord;
            }


            struct a2v
            {
                float4 positionOS : POSITION;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float4 shadowColor : COLOR;
            };

            v2f vert(a2v v)
            {
                v2f o;

                Light light = GetMainLight();

                float3 shadowCoordWS = ShadowProjCoord(v.positionOS, light.direction);
                o.positionCS = TransformWorldToHClip(shadowCoordWS);

                //得到中心点世界坐标
                float3 centerPositionWS = float3(unity_ObjectToWorld[0].w, _ShadowPlanePosY, unity_ObjectToWorld[2].w);
                //计算阴影衰减              
                float falloff = 1 - saturate(distance(shadowCoordWS , centerPositionWS) * _ShadowFade);

                o.shadowColor = half4(_ShadowColor.rgb, _ShadowColor.a * falloff);

                return o;
            }
            
            half4 frag(v2f i) : SV_Target
            {
                half4 final;
                final = i.shadowColor;

                return final;
            }
            ENDHLSL
        }   
     }
}