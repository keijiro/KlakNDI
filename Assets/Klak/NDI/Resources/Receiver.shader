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

    // Adobe-esque HDTV Rec.709 (2.2 gamma, 16-235 limit)
    half3 YUV2RGB(half3 yuv)
    {
        const half K_B = 0.0722;
        const half K_R = 0.2126;

        half y = (yuv.x - 16.0 / 255) * 255 / 219;
        half u = (yuv.y - 128.0 / 255) * 255 / 112;
        half v = (yuv.z - 128.0 / 255) * 255 / 112;

        half r = y + v * (1 - K_R);
        half g = y - v * K_R / (1 - K_R) - u * K_B / (1 - K_B);
        half b = y + u * (1 - K_B);

        return GammaToLinearSpace(half3(r, g, b));
    }

    // 4:2:2 subsampling
    half3 SampleUYVY(float2 uv)
    {
        half4 uyvy = tex2D(_MainTex, uv);
        bool sel = frac(uv.x * _MainTex_TexelSize.z) < 0.5;
        half3 yuv = sel ? uyvy.yxz : uyvy.wxz;
        return YUV2RGB(yuv);
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
