Shader "WZYURP/Unlit/Base"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        [Enum(Off, 0, On, 1)] _ZWrite("ZWrite", Float) = 1
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend Mode", Float) = 0
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2
    }

    SubShader
    {
        Tags { 
            "RenderType"="Opaque" 
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100

        Pass
        {
            ZWrite[_ZWrite]
            ZTest[_ZTest]
			Blend[_SrcBlend][_DstBlend]
            Cull[_Cull]

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            half4 _Color;
            CBUFFER_END


            struct a2v
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
            };

            v2f vert (a2v v)
            {
                v2f o;

                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                col *= _Color;
                clip(col.a - 0.01);//剔除完全透明的部分_Jim
                return col;
            }
            ENDHLSL
        }
    }
}
