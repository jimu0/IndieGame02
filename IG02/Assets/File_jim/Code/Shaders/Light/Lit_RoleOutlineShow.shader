
Shader "StarUnion/Light/RoleOutlineShow" {
    Properties {
		_Color("Color", Color) = (0.59, 0.59, 0.59, 1)
		_Lighting("Lighting", float) = 1
		_MainTex("Main Texture", 2D) = "white" {}
		[Toggle(_DIFFUSEON)] _DiffuseOn("Enable Diffuse", Float) = 1
		//[Space(5)]
		[Toggle(_SPECULARON)] _SpecularOn("Enable Specular", Float) = 1
		_SpecularColor("Specular Color", Color) = (0.59, 0.59, 0.59, 1)
		_SpecularPower("Specular Power", Range(0, 1)) = 1
		_SpecularRange("Specular Range", Range(0, 1)) = 0.6
		//[Space(5)]
		[Toggle(_MATCAPON)] _MatcapOn("Enable Matcap", Float) = 1
		_Matcap("Matcap", 2D) = "white" {}
		_MatcapPower("Matcap Power", float) = 0.2
    	//[Space(5)]
    	[Toggle(_OUTLINEMATCAPON)] _OutLineMatcapOn("Enable Matcap", Float) = 1
    	_OutLineMatcap("OutLine Matcap", 2D) = "white" {}
    	_OutLineMatcapPower("OutLine Matcap Power", float) = 0.2
		//[Space(5)]
		[Toggle(_EMISSIONON)] _EmissionOn("Enable Emission", Float) = 1
		_EmissionPower("Emission Power", float) = 1
		
		_ShadowColor ("Shadow Color", Color) = (0,0,0,0.7)
        _ShadowPlanePosY("Shadow Position Y", float) = 0
		_ShadowFade("Shadow Fade", float) = 1
		//[Space(5)]
		[Toggle(_FOGON)] _FogOn("Enable Fog", Float) = 1
		//[Space(5)]
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2
    	//[Space(5)]
	    _OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Outline("Outline Width", Range(0,0.01)) = 0.01
    }

	SubShader {
		Tags {
			"Queue" = "Geometry"
			"IgnoreProjector" = "True"
			"RenderType" = "Opaque"
		}
		LOD 1500

		Pass
		{
			Name "OUTLINE"
			Tags {"LightMode" = "Always"}
			Cull Front
			ZWrite On
			ColorMask RGB
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 color : COLOR;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
			};

			fixed _Outline;
			fixed4 _OutlineColor;

			v2f vert (appdata v)
			{
				v2f o;

				// 顶点色控制描边得效果。
				// float4 vPos = float4(UnityObjectToViewPos(v.vertex),1.0f);
				// float cameraDis = length(vPos.xyz);
				// vPos.xyz += normalize(normalize(vPos.xyz)) * v.color.b;
				// float3 vNormal = mul((float3x3)UNITY_MATRIX_IT_MV,v.normal);
				// o.pos = mul(UNITY_MATRIX_P,vPos);
				// float2 offset = TransformViewToProjection(vNormal).xy;
				// offset += offset * cameraDis  * v.color.g;
				// o.pos.xy += offset * _Outline* v.color.a;

				// 顶点色储存了软边模型得法线信息。
				float4 vPos = float4(UnityObjectToViewPos(v.vertex),1.0f);
				float cameraDis = length(vPos.xyz);
				vPos.xyz += normalize(vPos.xyz);
				float3 vertexNormal = v.color.rgb * 2 - 1;
				float3 vNormal = UnityObjectToWorldNormal(vertexNormal);
				o.pos = mul(UNITY_MATRIX_P, vPos);
				float2 offset = TransformViewToProjection(vNormal).xy;
				offset += offset * cameraDis ;
				o.pos.xy += offset * _Outline;

				// float3 vertexNormal = v.color.rgb * 2 - 1;
				// v.vertex.xyz += vertexNormal.xyz * _Outline;
				// o.pos = UnityObjectToClipPos(v.vertex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = _OutlineColor;
				return col;
			}
			ENDCG
		}
		
		Pass {
			Name "LightBaseColor"
			Tags { "LightMode" = "ForwardBase" } 

			Cull[_Cull]

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature _FOGON
			#pragma shader_feature _DIFFUSEON
			#pragma shader_feature _SPECULARON
			#pragma shader_feature _MATCAPON
			#pragma shader_feature _OUTLINEMATCAPON
			#pragma shader_feature _EMISSIONON
			#pragma multi_compile_fog
			#include "UnityCG.cginc"
			#include "Lighting.cginc"


			float4 _Color;
			float _Lighting;
			sampler2D _MainTex;
			float4 _MainTex_ST;
		#if _MATCAPON
			sampler2D _Matcap;
			float _MatcapPower;
		#endif
		#if _OUTLINEMATCAPON
			sampler2D _OutLineMatcap;
			float _OutLineMatcapPower;
		#endif
		#if _EMISSIONON
      		float _EmissionPower;
		#endif
		#if _SPECULARON
			float _SpecularRange;
			float4 _SpecularColor;
			float _SpecularPower;
		#endif


			struct a2v
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 uv0 : TEXCOORD0;
			#if defined (_DIFFUSEON) || (_SPECULARON)
				float3 normalDir : TEXCOORD2;
			#endif
			#if _SPECULARON
				float3 viewDir : TEXCOORD3;
			#endif
			#if _FOGON
				UNITY_FOG_COORDS(4)
			#endif
			};


			v2f vert(a2v v)
			{
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
			#if _MATCAPON
				o.uv0.z = dot(normalize(UNITY_MATRIX_IT_MV[0].xyz), normalize(v.normal));
				o.uv0.w = dot(normalize(UNITY_MATRIX_IT_MV[1].xyz), normalize(v.normal));
				o.uv0.zw = o.uv0.zw * 0.5 + 0.5;
			#else
				o.uv0.zw = half2(0, 0);
			#endif

			#if defined (_DIFFUSEON) || (_SPECULARON)
				o.normalDir = UnityObjectToWorldNormal(v.normal);
			#endif
			#if _SPECULARON
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.viewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);
			#endif
				

			#if _FOGON
				UNITY_TRANSFER_FOG(o, o.pos);
			#endif

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//albedo										 
				float4 albedo = tex2D(_MainTex, i.uv0.xy);

			#if defined (_DIFFUSEON) || (_SPECULARON)
				float3 normalDirection = normalize(i.normalDir);
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);    //light direction
			#endif

			#if _SPECULARON
				float3 viewDirection = (i.viewDir);    //view direction
				float3 h = normalize(lightDirection + viewDirection);
				float nh = saturate(dot(normalDirection, h));
				float specRange = exp2(_SpecularRange * 10.0 + 1.0);
				specRange = pow(max(0, nh), specRange);
				float3 specular = specRange * _SpecularColor.rgb * _SpecularPower*10;
			#endif

			#if _DIFFUSEON
				float nl = saturate(dot(normalDirection, lightDirection));
				float3 lightColor = _LightColor0.rgb;
				float3 diffuse = albedo * nl * lightColor;
			#endif

				//final
				fixed4 finalColor;
			#if _LIGHTMAPON
				finalColor.rgb = albedo.rgb;
			#else
				finalColor.rgb = albedo.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb;
			#endif
				//add diffuse
			#if _DIFFUSEON
				finalColor.rgb += diffuse;
			#endif
				//add specular
			#if _SPECULARON
				finalColor.rgb += specular;
			#endif
			#if _MATCAPON
				float3 matcap = tex2D(_Matcap, i.uv0.zw).rgb * _MatcapPower;
				finalColor.rgb *= matcap;
			#endif
			#if _OUTLINEMATCAPON
				float3 outline_matcap = tex2D(_OutLineMatcap, i.uv0.zw).rgb * _OutLineMatcapPower;
				finalColor.rgb += outline_matcap * finalColor.rgb;
			#endif
				//add emission
			#if _EMISSIONON
				float3 emission = albedo.rgb;
				finalColor.rgb += emission * _EmissionPower;
			#endif

				finalColor.rgb *= _Color.rgb * _Lighting;
				finalColor.a = 1;

			#if _FOGON
				UNITY_APPLY_FOG(i.fogCoord, finalColor);
			#endif

				return finalColor;
			}
			ENDCG
		}

		UsePass "StarUnion/Other/PlanarShadow/Shadow"
	}
    CustomEditor "LitRoleShaderGUI"
}
