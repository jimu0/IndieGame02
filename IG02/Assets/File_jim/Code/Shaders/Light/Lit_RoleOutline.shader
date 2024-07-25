
Shader "StarUnion/Light/RoleOutline" {
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
		_MatcapRotate("Matcap Rotate", Range(-3.14, 3.14)) = 0
		//[Space(5)]
		[Toggle(_EMISSIONON)] _EmissionOn("Enable Emission", Float) = 1
		_EmissionPower("Emission Power", float) = 1
		
		_ShadowColor ("Shadow Color", Color) = (0,0,0,0.7)
        _ShadowPlanePosY("Shadow Position Y", float) = 0
		_ShadowFade("Shadow Fade", float) = 1
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Outline("Outline Width", Range(0,0.01)) = 0.001
		//[Space(5)]
		[Toggle(_FOGON)] _FogOn("Enable Fog", Float) = 1
		//[Space(5)]
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2
    }

	SubShader {
		Tags {
			"Queue" = "Geometry"
			"IgnoreProjector" = "True"
			"RenderType" = "Opaque"
		}

		UsePass "StarUnion/Other/Outline/OutlinePass"
		UsePass "StarUnion/Light/Role/LightBaseColor"
		UsePass "StarUnion/Other/PlanarShadow/Shadow"
	}
	CustomEditor "LitRoleShaderGUI"
}
