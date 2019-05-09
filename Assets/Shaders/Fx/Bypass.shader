Shader "Hidden/Bypass"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    CGINCLUDE

    #include "UnityCG.cginc"
    sampler2D _MainTex;
    float4 _MainTex_ST;
    fixed4 frag (v2f_img i) : SV_Target
    {
        // sample the texture
        fixed4 col = tex2D(_MainTex, i.uv);
        return col;
    }
    ENDCG

    SubShader
    {
        Pass
        {
            ZTest Always
			Cull Off
			ZWrite Off
			Fog{ Mode Off }

            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            ENDCG
        }
    }
}
