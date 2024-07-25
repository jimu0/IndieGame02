

Shader "StarUnion/Unlight/Blend"
{
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Lighting ("Lighting",  float) = 1
		_MainTex ("Main Texture", 2D) = "white" {}
		[Toggle(_RFORALPHA)] _RForAlpha("R For Alpha", Float) = 0
		_RStepMin("R Step Min", Range(0, 1)) = 0
		_RStepMax("R Step Max", Range(0, 1)) = 1
		[Toggle(_MAINUVMOVE)] _MainUVMove("Enable Main UV Move", Float) = 0
		_MainUVSpeedX("Main UV Speed X", Float) = 10
		_MainUVSpeedY("Main UV Speed Y", Float) = 10
		//[Space(5)]
		[Toggle(_NOISEON)] _NoiseOn("Enable Noise", Float) = 0
		_NoiseTex ("Noise Texture", 2D) = "white" {}
		[Toggle(_NOISEUVMOVE)] _NoiseUVMove("Enable Noise UV Move", Float) = 0
		_NoiseUVSpeedX("Noise UV Speed X", Float) = 10
		_NoiseUVSpeedY("Noise UV Speed Y", Float) = 10
		//[Space(5)]
		[Toggle(_MASKON)] _MaskOn("Enable Mask", Float) = 0
		_MaskTex ("Mask Texture", 2D) = "white" {}
		//[Space(5)]
		[Toggle(_DISSOLVEON)] _DissolveOn("Enable Dissolve", Float) = 0
		_DissolveTex ("Dissolve Texture", 2D) = "white" {}
		[Toggle(_UV2DISSOLVE)] _UV2Dissolve("Enable UV2 Dissolve", Float) = 0
		_DissolveValue("Dissolve Value", float) = 1
		//[Space(5)]
		[Toggle(_FOGON)] _FogOn("Enable Fog", Float) = 1
		//[Space(5)]
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4
		[HideInInspector] _Mode ("mode", Float) = 0
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend Mode", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend Mode", Float) = 10
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2
	}


	SubShader
	{
		Tags { 
			"Queue" = "Transparent" 
			"IgnoreProjector" = "True" 
			"RenderType" = "Transparent" 
			"RenderPipeline" = "UniversalPipeline"
		}
	
		Pass
		{
			ZWrite Off
			Blend[_SrcBlend][_DstBlend]
			ZTest[_ZTest]
			Cull[_Cull]

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma shader_feature _FOGON
			#pragma shader_feature _RFORALPHA
			#pragma shader_feature _MAINUVMOVE
			#pragma shader_feature _NOISEON
			#pragma shader_feature _NOISEUVMOVE
			#pragma shader_feature _MASKON
			#pragma shader_feature _DISSOLVEON
			#pragma shader_feature _UV2DISSOLVE
			#pragma multi_compile_fog
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			
			
			sampler2D _MainTex;
		
		#if _NOISEON
			sampler2D _NoiseTex;
		#endif
		#if _MASKON
			sampler2D _MaskTex;
		#endif
		#if _DISSOLVEON
			sampler2D _DissolveTex;
		#endif

			CBUFFER_START(UnityPerMaterial)
			float4 _Color;
			float _Lighting;
			float4 _MainTex_ST;
		#if _MAINUVMOVE
			float _MainUVSpeedX;
			float _MainUVSpeedY;
		#endif
		#if _RFORALPHA
			float _RStepMin;
			float _RStepMax;
		#endif
		#if _NOISEON
			float4 _NoiseTex_ST;
		  #if _NOISEUVMOVE
			float _NoiseUVSpeedX;
			float _NoiseUVSpeedY;
		  #endif
		#endif
		#if _DISSOLVEON
			float4 _DissolveTex_ST;
			float _DissolveValue;
		#endif
			CBUFFER_END

 
			struct a2v {
				float4 vertex : POSITION;
				float2 texcoord0 : TEXCOORD0;
			#if	_UV2DISSOLVE
				float2 texcoord1 : TEXCOORD1;
			#endif
				float4 vertColor : COLOR;
			};

            struct v2f {
                float4 pos : SV_POSITION;
				float4 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
			#if _MASKON
				float2 maskUV : TEXCOORD1;
			#endif
			#if _DISSOLVEON
				float4 dissolveUV : TEXCOORD2;
			#endif
			#if _FOGON
				float fogCoord : TEXCOORD3;
			#endif
            };
      

            v2f vert (a2v v)
            {
                v2f o;        
                o.uv.xy = TRANSFORM_TEX(v.texcoord0, _MainTex);

			#if	_NOISEON
				o.uv.zw = TRANSFORM_TEX(v.texcoord0, _NoiseTex);
			#else
				o.uv.zw = half2(0, 0);
			#endif
			
			#if _MASKON
				o.maskUV = v.texcoord0;
			#endif

			#if _DISSOLVEON
				o.dissolveUV.xy = TRANSFORM_TEX(v.texcoord0, _DissolveTex);
				o.dissolveUV.zw = half2(0, 0);
			  #if _UV2DISSOLVE
			  	o.dissolveUV.zw += v.texcoord1;
			  #endif
			#endif
			
				o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.vertexColor = v.vertColor;
			#if _FOGON
				o.fogCoord = ComputeFogFactor(o.pos.z);
			#endif
                return o;
            }


            float4 frag (v2f i) : COLOR
            {
				//main uv
				half2 mainUV = i.uv.xy;
			#if _MAINUVMOVE
				mainUV += half2(_MainUVSpeedX, _MainUVSpeedY) * _Time.x;
			#endif

				//noise
			#if _NOISEON
				half2 noiseUV = i.uv.zw;
			#if _NOISEUVMOVE
				noiseUV += half2(_NoiseUVSpeedX, _NoiseUVSpeedY) * _Time.x;
			#endif
				float noise = tex2D(_NoiseTex, noiseUV).r;
				mainUV += noise;
			#endif

				//main
                float4 col = tex2D(_MainTex, mainUV);

				//final
				float4 final;
				final.rgb = col.rgb * _Color.rgb * _Lighting * i.vertexColor.rgb;
			#if _RFORALPHA
				final.a = smoothstep(_RStepMin, _RStepMax, col.r);
			#else
				final.a = col.a;
			#endif		
				final.a *= _Color.a * i.vertexColor.a;	

			#if _MASKON
				float mask = tex2D(_MaskTex, i.maskUV).r;
				final.a *= mask;
			#endif

			#if _DISSOLVEON
				float dissolveValue;
			  #if _UV2DISSOLVE
				dissolveValue = i.dissolveUV.z;
			  #else
			  	dissolveValue = _DissolveValue;
			  #endif
				float dissolve = tex2D(_DissolveTex, i.dissolveUV.xy).r;
				final.a = lerp(0, final.a, step(dissolve, dissolveValue));
			#endif

			#if _FOGON
				final.rgb = MixFog(final.rgb, i.fogCoord);
			#endif

				return final;
            }
            ENDHLSL
        }        
	}
	CustomEditor "UnlitBlendShaderGUI"
}