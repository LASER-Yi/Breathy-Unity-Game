﻿Shader "FX/LASER/UiShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Brightness ("Brightness", Range(0, 1)) = 0.5
        _Transparent ("Transparent", Range(0, 1)) = 0.0
    }
    SubShader
    {
        Tags {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _GrabTexture;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Brightness;
            float _Transparent;
            float _TextureMixRate;

            struct appdata_v{
                float4  vertex : POSITION;
                float2  texcoord : TEXCOORD0;
                float4  color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 uvgrab : TEXCOORD1;
                float4 color: COLOR;
            };

            v2f vert(appdata_v v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                // #if UNITY_UV_STARTS_AT_TOP
                //     float scale = -1.0;
                // #else
                //     float scale = 1.0;
                // #endif
                o.uvgrab.xy = (float2(o.pos.x, o.pos.y*1.0) + o.pos.w) * 0.5;
                o.uvgrab.zw = o.pos.zw;
                o.color = v.color;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float4 texColor = tex2D(_MainTex, i.uv);
                texColor.xyz = lerp(texColor.xyz, i.color, i.color.w);

                float4 grabColor = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
                grabColor *= _Brightness;

                float4 color;
                color.xyz = lerp(grabColor, texColor , texColor.w);
                color.w = 1.0 - _Transparent;
                return color;
            }
            ENDCG
        }
    }
}
