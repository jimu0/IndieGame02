
Shader "StarUnion/Unlight/FrameAnim"
{
	Properties {
		_Color("Color", Color) = (1,1,1,1)
		_Lighting("Lighting",  float) = 1
		_MainTex ("Texture", 2D) = "white" { }
		_FrameVector("FrameVector", vector) = (8, 8, 200, 1)
		//_TimeValue("Time Value", float) = 0
		[Space(20)]
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend Mode", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend Mode", Float) = 10
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2
	}


		SubShader{

			Tags{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
			}


		Pass
		{
			Blend[_SrcBlend][_DstBlend]
			ZWrite Off
			ZTest[_ZTest]
			Cull[_Cull]

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				sampler2D _MainTex;
				half4 _MainTex_ST;
				half4 _Color;
				half _Lighting;
				half4 _FrameVector;
				half4 _RowVector;
				//float _TimeValue;

 
			struct appdata {
				half4 vertex : POSITION;
				half2 texcoord:TEXCOORD0;
			};

            struct v2f {
                half4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;

            };
      

            v2f vert (appdata v)
            {
                v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);

                return o;
            }


            fixed4 frag (v2f i) : COLOR
            {
				//方式1：精度问题导致像素块bug
				//可用方式1的计算，从外部传time，然后间隔重置time解决此bug
				// int speed = floor(_Time.y * _FrameVector.z);  //fmod(_Time.y, 3000000)
				// int y = floor(speed / _FrameVector.x);
				// int x = speed - y * _FrameVector.x;
				//方式2：处理像素块bug
				int speed = fmod(_Time.y * _FrameVector.z, _FrameVector.x * _FrameVector.y);
				int x = fmod(speed, _FrameVector.x);
				int y = fmod(speed / _FrameVector.x, _FrameVector.x);

				float2 mUv = float2(i.uv.x / _FrameVector.x, i.uv.y / _FrameVector.y);
				mUv.x += x / _FrameVector.x;
				mUv.y += -y / _FrameVector.y;

                fixed4 col= tex2D(_MainTex, mUv.xy);
				col.rgb *= _Lighting;
				col.rgb *= _Color.rgb;
				col.a *= _Color.a;

				return col;
            }
            ENDCG
        }
	}

}