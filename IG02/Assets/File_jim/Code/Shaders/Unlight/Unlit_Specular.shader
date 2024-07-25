

Shader "StarUnion/Unlight/Specular"
{
	Properties
	{
		[Header(Main)][Space(5)]
		_Color ("Color", Color) = (1,1,1,1)
		_Lighting ("Lighting",  float) = 1
		_MainTex ("Main Texture", 2D) = "white" {}
		[Header(Specular)][Space(5)]
		_SpecColor("Specular Color", color) = (1, 0.87, 0.68, 0.1)
        _SpecTex("Specular Texture", 2D) = "white" {}
		[Header(Advanced Options)][Space(5)]
		[Header(xyz_Direction     w_Gloss)][Space]
		_LightVector("Light Vector", vector) = (-1.91, -1.95, -2.82, 0.27)
	}

	SubShader
	{
		Tags { 
			"Queue" = "Transparent" 
			"IgnoreProjector" = "True" 
			"RenderType" = "Transparent" 
		}
	
		Pass
		{
			ZWrite Off
			ZTest LEqual
			Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#include "UnityCG.cginc"
			
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float _Lighting;
			half4 _SpecColor;
            sampler2D _SpecTex;
            half4 _SpecTex_ST;
            half4 _LightVector;

 
			struct a2v {
				float4 vertex : POSITION;
				float2 texcoord0 : TEXCOORD0;
				float3 normal : NORMAL;
			};

            struct v2f {
                float4 pos : SV_POSITION;
				float4 uv : TEXCOORD0;
				half3 worldPos : TEXCOORD1;
                half3 normalDir : TEXCOORD2;
            };
      

            v2f vert (a2v v)
            {
                v2f o;        
                o.uv.xy = TRANSFORM_TEX(v.texcoord0, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.texcoord0, _SpecTex);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                return o;
            }


            float4 frag (v2f i) : COLOR
            {
				//main
                float4 col = tex2D(_MainTex, i.uv.xy);

				half3 lightDir = normalize(-_LightVector.xyz);
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
                half3 h = normalize(lightDir + viewDir);
				fixed nh = saturate(dot(normalize(i.normalDir), h));

				//specular
                half3 specTex = tex2D(_SpecTex, i.uv.zw).rgb;
				fixed specRange = exp2(_LightVector.w * 10.0 + 1.0);
				half3 specular = pow(max(0, nh), specRange) * (_SpecColor.a * 5) * _SpecColor.rgb * specTex;

				//final
				float4 final;
				final.rgb = col.rgb + specular;
				final.rgb *= _Color.rgb * _Lighting;
				final.a = col.a * _Color.a;

				return final;
            }
            ENDCG
        }        
	}
}