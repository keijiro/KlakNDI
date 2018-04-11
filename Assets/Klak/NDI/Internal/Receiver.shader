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

    // 4:2:2 subsampling
    half3 SampleUYVY(float2 uv)
    {
        half4 uyvy = tex2D(_MainTex, uv);
        bool sel = frac(uv.x * _MainTex_TexelSize.z) < 0.5;
        half3 yuv = sel ? uyvy.yxz : uyvy.wxz;
        return GammaToLinearSpace(YUV2RGB(yuv));
    }

    half4 Fragment_UYVY(v2f_img input) : SV_Target
    {
        return half4(SampleUYVY(float2(input.uv.x, 1 - input.uv.y)), 1);
    }

    half4 Fragment_UYVA(v2f_img input) : SV_Target
    {
        half3 rgb = SampleUYVY(float2(input.uv.x, (1 - input.uv.y) * 2 / 3));

        // Alpha data retrieval
        float2 uv = input.uv;
        uv.x = (uv.x + (frac(uv.y * _MainTex_TexelSize.w / 3) < 0.5)) / 2;
        uv.y = 1 - uv.y / 3;
        half4 a4 = tex2D(_MainTex, uv);
        half sel = frac(input.uv.x * _MainTex_TexelSize.z / 2) * 4;
        half a = lerp(a4.x, a4.y, saturate(sel - 0.5));
        a = lerp(a, a4.z, saturate(sel - 1.5));
        a = lerp(a, a4.w, saturate(sel - 2.5));

        return half4(rgb, a);
    }

    ENDCG

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_img
            #pragma fragment Fragment_UYVY
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_img
            #pragma fragment Fragment_UYVA
            ENDCG
        }
    }
}
