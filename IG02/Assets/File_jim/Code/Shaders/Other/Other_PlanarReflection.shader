
Shader "StarUnion/Other/PlanarReflection"
{	
    Properties {
		[Header(Falloff)][Space(5)]
		_Color ("Color", Color) = (1,1,1,1)
		_FalloffTex ("Falloff Texture", 2D) = "white" {}
		_UvRotation("Uv Rotation", Range(-3.14, 3.14)) = 0
    	_UOffset ("_UOffset", Float) = 1
    	_VOffset ("_VOffset", Float) = 1
    	_FalloffPower ("FalloffPower", Float) = 1
		[Header(Advanced Options)][Space(5)]
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend Mode", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend Mode", Float) = 10
    }

	SubShader
	{
		Tags { "RenderType"="Opaque" }
 
		Pass
		{
            Blend[_SrcBlend][_DstBlend]
			ZTest[_ZTest]
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			half4 _Color;
			sampler2D _ReflectionTex;
			sampler2D _FalloffTex;
			float _UvRotation;

			float _UOffset;
			float _VOffset;
			float _FalloffPower;
 
			struct appdata
			{
				float4 vertex : POSITION;
			};
 
			struct v2f
			{
				float4 screenPos : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_ReflectionTex, i.screenPos.xy / i.screenPos.w);  //or  tex2Dproj(_ReflectionTex, i.screenPos)

				half2 falloffUV = (i.screenPos.xy / i.screenPos.w);
				falloffUV -= half2(0.5, 0.5);
				falloffUV = half2( falloffUV.x * cos(_UvRotation) - falloffUV.y * sin(_UvRotation),
								   falloffUV.y * cos(_UvRotation) + falloffUV.x * sin(_UvRotation) );
				falloffUV += half2(_UOffset, _VOffset);
				fixed falloff = tex2D(_FalloffTex, falloffUV).r;
				falloff = pow(falloff, _FalloffPower);
				col.rgb *= _Color.rgb;
				col.a *= _Color.a * falloff;

				return col;
			}
			ENDCG
		}
	}
}