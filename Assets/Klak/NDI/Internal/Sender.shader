Shader "Hidden/KlakNDI/Sender"
{
    Properties
    {
        _MainTex("", 2D) = "" {}
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;

    half3 RGB2YUV(half3 rgb)
    {
        rgb = LinearToGammaSpace(rgb);
        half y = dot(half3(0.299, 0.587, 0.114), rgb);
        half u = (rgb.b - y) * 0.565 + 0.5;
        half v = (rgb.r - y) * 0.713 + 0.5;
        return half3(y, u, v);
    }

    half4 Fragment(v2f_img input) : SV_Target
    {
        float2 uv1 = input.uv;
        uv1.x -= _MainTex_TexelSize.x / 2;
        uv1.y = 1 - uv1.y;

        float2 uv2 = uv1 + float2(_MainTex_TexelSize.x, 0);

        half3 c1 = RGB2YUV(tex2D(_MainTex, uv1).rgb);
        half3 c2 = RGB2YUV(tex2D(_MainTex, uv2).rgb);

        return half4(c1.yx, c2.zx);
    }

    ENDCG

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_img
            #pragma fragment Fragment
            ENDCG
        }
    }
}
