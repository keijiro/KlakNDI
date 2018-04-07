Shader "Hidden/KlakNDI/Receiver"
{
    Properties
    {
        _MainTex("", 2D) = "" {}
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;

    half3 YUV2RGB(half3 yuv)
    {
        yuv.yz -= 0.5;
        return half3(
            yuv.x + 1.403 * yuv.z,
            yuv.x - 0.344 * yuv.y - 0.714 * yuv.z,
            yuv.x + 1.770 * yuv.y
        );
    }

    half4 Fragment(v2f_img input) : SV_Target
    {
        float2 uv = input.uv;
        uv.y = 1 - uv.y;

        half4 uyvy = tex2D(_MainTex, uv);
        bool second = frac(uv.x * _MainTex_TexelSize.z ) > 0.4;
        half3 yuv = second ? uyvy.wxz : uyvy.yxz;

        return half4(GammaToLinearSpace(YUV2RGB(yuv)), 1);
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
