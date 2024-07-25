#ifndef STAR_UNION_ROLE_SHADOW_CG_INCLUDE
#define STAR_UNION_ROLE_SHADOW_CG_INCLUDE


    float _ShadowPlanePosY;
    float4 _ShadowColor;
    half _ShadowAtten;
    float _ShadowFade;
    fixed _Alpha;

    float3 ShadowProjectPos(float4 vertPos){
        float3 shadowPos;
        float3 worldPos = mul(unity_ObjectToWorld, vertPos).xyz;
        float3 lightDir = normalize(UnityWorldSpaceLightDir(vertPos));
        shadowPos.y = min(worldPos.y, _ShadowPlanePosY);
        shadowPos.xz = worldPos.xz - lightDir.xz * max(0 , worldPos.y - _ShadowPlanePosY) / lightDir.y;

        return shadowPos;
    }


    struct a2v{
        float4 vertex : POSITION;
    };
    struct v2f{
        float4 pos : SV_POSITION;
        float4 color : COLOR;
    };

    v2f vert(a2v v){
        v2f o;
        float3 shadowPos = ShadowProjectPos(v.vertex);
        o.pos = UnityWorldToClipPos(shadowPos);
        float3 center = float3(unity_ObjectToWorld[0].w, _ShadowPlanePosY, unity_ObjectToWorld[2].w);
        float fade = 1 - saturate(distance(shadowPos, center) * _ShadowFade);

        o.color = _ShadowColor;
        o.color.a = o.color.a * fade + 0.02h;

        //o.color.a = (_ShadowColor.a + 0.02h) * _ShadowAtten;

        // #ifdef _DEAD_ON
        // o.color.a *= _Alpha;
        // #endif

        return o;
    }

    v2f vertSimple(a2v v){
        v2f o;
        float3 shadowPos = ShadowProjectPos(v.vertex);
        o.pos = UnityWorldToClipPos(shadowPos);
        o.color = _ShadowColor;
        return o;
    }

    v2f deadvert(a2v v){
        v2f o;
        float3 shadowPos=ShadowProjectPos(v.vertex);
        o.pos = UnityWorldToClipPos(shadowPos);
        // float3 center=float3(unity_ObjectToWorld[0].w,_shadowPlanePosY,unity_ObjectToWorld[2].w);
        // float fade=1-saturate(distance(shadowPos,center)*_ShadowFade);
        // o.color=_ShadowColor;
        o.color=_ShadowColor;
        // o.color.a=fade * _ShadowColor.a + 0.02h;
        // o.color.a=_ShadowColor.a + 0.02h;
        o.color.a=_ShadowColor.a + 0.02h;
        // #ifdef _DEAD_ON
        o.color.a *= _Alpha;
        // #endif
        return o;
    }



    fixed4 frag(v2f i):SV_TARGET{
        // i.color = _simpleLightDir;
        return i.color;
    }

    // fixed4 deadfrag(v2f i):SV_TARGET{
    //     clip()
    //     return i.color;
    // }
#endif