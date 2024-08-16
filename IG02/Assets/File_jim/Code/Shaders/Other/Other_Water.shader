
Shader "StarUnion/Other/Water"
{
    Properties
    {
		[Header(Main)][Space(5)]
		_Color ("Color", Color) = (1,1,1,1)
		_Lighting ("Lighting", float) = 1
        _MainTex ("Main Texture", 2D) = "white" {}
		[Header(Fresnel)][Space(5)]
		_FresnelColor ("Fresnel Color", Color) = (0.53, 0.79, 0.82, 1)
		_FresnelPow ("Fresnel Pow", float) = 4
		_NVOffset ("NV Offset", Range(0, 1)) = 0.15
		_Fresnel("Fresnel", 2D) = "white" {}
		[Header(Normal)][Space(5)]
		_Normal("Normal", 2D) = "white"{}
		_NormalUVSpeed("Normal UV Speed (2 waves)", Vector) = (0.31, 0.25, -0.1, -0.12)
		_NormalTiling("Normal Tiling", Vector) = (0.2, 0.38, 0.19, 0.24)
		[Header(Wave)][Space(5)]
		[Toggle(_WAVEON)] _WaveOn("Enable Wave", Float) = 0
		_WaveTexture("Wave Texture", 2D) = "white" {}
		_WaveMask("Wave Mask", 2D) = "white" {}
		_WaveSpeed ("Wave Speed", float) = 2
		_WaveColor ("Wave Color", Color) = (0.66, 0.59, 0.7, 0.4)
		[Header(Specular)][Space(5)]
		_SpecularColor("Specular Color", Color) = (0.91, 0.88, 1, 0.29)
		[Header(Advanced Options)][Space(5)]
		_LightVector("LightVector", Vector) = (-0.25, -0.26, -0.57, 1)
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
        }

		ZWrite Off
		ZTest LEqual
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
        	CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma shader_feature _WAVEON
            #include "UnityCG.cginc"


			float4 _Color;
			float _Lighting;
			sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _FresnelColor;
			float _FresnelPow;
			float _NVOffset;
			sampler2D _Fresnel;
			sampler2D _Normal;
			half4 _NormalUVSpeed;
			half4 _NormalTiling;
			half4 _LightVector;		//xyz: 方向 ， w：强度
			half4 _SpecularColor;	//rgb：颜色 ， a：强度
		#if _WAVEON
			sampler2D _WaveTexture;
			sampler2D _WaveMask;
			float4 _WaveColor;
			float _WaveSpeed;
		#endif

			half3 PerPixelNormal(sampler2D normalTex, half4 normalUV, half normalPower)
			{
				float2 normal = (UnpackNormal(tex2D(normalTex, normalUV.xy)) + UnpackNormal(tex2D(normalTex, normalUV.zw))) * 0.5;
				float3 normalDir;
				normalDir.xz = normal * normalPower;
				normalDir.y = 1;
				return normalDir;
			}

			
			struct a2v
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
			};
			
			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv :	TEXCOORD0;
				float4 uvNormal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
			};

			v2f vert(a2v v)
            {
				v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.uvNormal = (o.worldPos.xzxz + _Time.y * _NormalUVSpeed) * _NormalTiling;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				half3 normalDir = normalize(PerPixelNormal(_Normal, i.uvNormal, _LightVector.w));

				half3 h = normalize(normalize(-_LightVector.xyz) + viewDir);
				fixed nh = saturate(dot(normalDir, h));
				fixed nv = saturate(dot(normalDir, viewDir));
				
				//wave
			#if _WAVEON
				float mask = tex2D(_WaveMask, i.uv).r;
				float rangeX = normalDir.x * _WaveSpeed + 1;
				half3 wave = tex2D(_WaveTexture, float2(1 - min(rangeX, 1 - mask)/rangeX, 1)).rgb; 
				wave *= mask * _WaveColor.rgb * _WaveColor.a;
			#endif

				//albedo
				half4 albedo = tex2D(_MainTex, i.uv);

				//fresnel
				half fresnelLerp = tex2D(_Fresnel, half2(nv - _NVOffset, 0.5)).r;
				fresnelLerp = pow(fresnelLerp, _FresnelPow);
				half3 fresnel = lerp(albedo.rgb * _Color.rgb, _FresnelColor.rgb * _FresnelColor.a, fresnelLerp);

				//specular
				half3 specular = _SpecularColor.rgb * _SpecularColor.a * pow(max(0, nh), 256);

				//final
				fixed4 final;
				final.rgb = specular + fresnel;
			#if _WAVEON
				final.rgb += wave;
			#endif
				final.rgb *= _Lighting;
				final.a = _Color.a;

				return final;
            }
        	ENDCG
        }
    }
}
