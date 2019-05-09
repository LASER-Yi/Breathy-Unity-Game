Shader "FX/LASER/BasicShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TexTint ("Texture Tint", Range(0,1)) = 0
    }
    SubShader
    {
        Tags {
            "RenderType"="Opaque" 
            "Queue"="Transparent"
            }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _GrabTexture;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _TexTint;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 uvgrab : TEXCOORD1;
            };

            v2f vert(appdata_img v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                #if UNITY_UV_STARTS_AT_TOP
                float scale = -1.0;
                #else
                float scale = 1.0;
                #endif
                o.uvgrab.xy = (float2(o.pos.x, o.pos.y*scale) + o.pos.w) * 0.5;
                o.uvgrab.zw = o.pos.zw;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float4 _maintex = tex2D(_MainTex, i.uv);
                float4 _texture = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
                float4 color = lerp(_texture, _maintex, _TexTint);
                return color;
            }
            ENDCG
        }
    }
}
