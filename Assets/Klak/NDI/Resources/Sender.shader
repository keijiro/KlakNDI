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

    // Adobe-esque HDTV Rec.709 (2.2 gamma, 16-235 limit)
    half3 RGB2YUV(half3 rgb)
    {
        const half K_B = 0.0722;
        const half K_R = 0.2126;

        rgb = LinearToGammaSpace(rgb);

        half y = dot(half3(K_R, 1 - K_B - K_R, K_B), rgb);
        half u = ((rgb.b - y) / (1 - K_B) * 112 + 128) / 255;
        half v = ((rgb.r - y) / (1 - K_R) * 112 + 128) / 255;

        y = (y * 219 + 16) / 255;

        return half3(y, u, v);
    }

    half4 Fragment_UYVY(v2f_img input) : SV_Target
    {
        const float3 ts = float3(_MainTex_TexelSize.xy, 0);

        float2 uv = input.uv;
        uv.x -= ts.x * 0.5;
        uv.y = 1 - uv.y;

        half3 yuv1 = RGB2YUV(tex2D(_MainTex, uv        ).rgb);
        half3 yuv2 = RGB2YUV(tex2D(_MainTex, uv + ts.xz).rgb);

        half u = (yuv1.y + yuv2.y) * 0.5;
        half v = (yuv1.z + yuv2.z) * 0.5;

        return half4(u, yuv1.x, v, yuv2.x);
    }

    half4 Fragment_UYVA(v2f_img input) : SV_Target
    {
        const float3 ts = float3(_MainTex_TexelSize.xy, 0);

        float2 uv = input.uv;
        uv.x -= ts.x * 0.5;
        uv.y = 1 - uv.y * 1.5;

        float2 uv_a = input.uv;
        uv_a.x = frac(uv_a.x * 2) - ts.x * 1.5;
        uv_a.y = 3 - uv_a.y * 3;
        uv_a.y += ts.y * (input.uv.x < 0.5 ? 0.5 : -0.5);

        half3 yuv1 = RGB2YUV(tex2D(_MainTex, uv        ).rgb);
        half3 yuv2 = RGB2YUV(tex2D(_MainTex, uv + ts.xz).rgb);

        half u = (yuv1.y + yuv2.y) * 0.5;
        half v = (yuv1.z + yuv2.z) * 0.5;

        half a1 = tex2D(_MainTex, uv_a            ).a;
        half a2 = tex2D(_MainTex, uv_a + ts.xz * 1).a;
        half a3 = tex2D(_MainTex, uv_a + ts.xz * 2).a;
        half a4 = tex2D(_MainTex, uv_a + ts.xz * 3).a;

        return uv_a.y < 1 ? half4(a1, a2, a3, a4) : half4(u, yuv1.x, v, yuv2.x);
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
