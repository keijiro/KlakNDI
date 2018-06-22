Shader "Hidden/TestCards/Overlay"
{
    Properties
    {
        _MainTex("", 2D) = "black"{}
        _Color("", Color) = (0.5, 0.5, 0.5)
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;

    half3 _Color;
    float _Scale;

    // Hue to RGB convertion
    half3 HueToRGB(half h)
    {
        h = saturate(h);
        half r = abs(h * 6 - 3) - 1;
        half g = 2 - abs(h * 6 - 2);
        half b = 2 - abs(h * 6 - 4);
        half3 rgb = saturate(half3(r, g, b));
        return rgb;
    }

    half4 frag_fill(v2f_img i) : SV_Target
    {
        return half4(_Color, 1);
    }

    half4 frag_spectrum(v2f_img i) : SV_Target
    {
        half3 rgb = lerp(HueToRGB(i.uv.x), i.uv.x, i.uv.y > 0.5);
        #if !defined(UNITY_COLORSPACE_GAMMA)
        rgb = GammaToLinearSpace(rgb);
        #endif
        return half4(rgb, 1);
    }

    half4 frag_checker(v2f_img i) : SV_Target
    {
        float2 c = step(0.4999, frac(i.uv.xy * _MainTex_TexelSize.zw * _Scale));
        return abs(c.x - c.y);
    }

    half4 frag_pattern(v2f_img i) : SV_Target
    {
        float scale = 27 * _MainTex_TexelSize.y;          // Grid scale
        float2 p0 = (i.uv - 0.5) * _MainTex_TexelSize.zw; // Position (pixel)
        float2 p1 = p0 * scale;                           // Position (half grid)
        float2 p2 = p1 / 2 - 0.5;                         // Position (grid)

        // Size of inner area
        half aspect = _MainTex_TexelSize.y * _MainTex_TexelSize.z;
        half2 area = half2(floor(6.5 * aspect) * 2 + 1, 13);

        // Crosshair and grid lines
        half2 ch = abs(p0);
        half2 grid = (1 - abs(frac(p2) - 0.5) * 2) / scale;
        half c1 = min(min(ch.x, ch.y), min(grid.x, grid.y)) < 1 ? 1 : 0.5;

        // Outer area checker
        half2 checker = frac(floor(p2) / 2) * 2;
        if (any(abs(p1) > area)) c1 = abs(checker.x - checker.y);

        half corner = sqrt(8) - length(abs(p1) - area + 4); // Corner circles
        half circle = 12 - length(p1);                      // Big center circle
        half mask = saturate(circle / scale);               // Center circls mask

        // Grayscale bars
        half bar1 = saturate(p1.y < 5 ? floor(p1.x / 4 + 3) / 5 : p1.x / 16 + 0.5);
        if (abs(5 - p1.y) < 4 * mask) c1 = bar1;

        // Basic color bars
        half3 bar2 = HueToRGB((p1.y > -5 ? floor(p1.x / 4) / 6 : p1.x / 16) + 0.5);
        float3 rgb = abs(-5 - p1.y) < 4 * mask ? bar2 : saturate(c1);

        // Circle lines
        rgb = lerp(rgb, 1, saturate(1.5 - abs(max(circle, corner)) / scale));

        #if !defined(UNITY_COLORSPACE_GAMMA)
        rgb = GammaToLinearSpace(rgb);
        #endif

        return half4(rgb, 1);
    }

    half4 frag_shutter(v2f_img i) : SV_Target
    {
        const float4 tsize = _MainTex_TexelSize;
        const float radius = 0.45;

        const float time = _Time.y;
        const float deltaTime = unity_DeltaTime.x;

        float2 uv = (i.uv - 0.5) * float2(tsize.y * tsize.z, 1);

        float phi = atan2(-uv.x, -uv.y) / (UNITY_PI * 2) + 0.5;
        half arc = saturate((phi - frac(time)) / fwidth(phi));

        float dist = length(uv);
        arc *= saturate((radius + tsize.y - dist) * tsize.w);

        half circle = saturate(1 - abs(dist - radius) * tsize.w);

        half flash = frac(time) <= frac(time - deltaTime);

        half2 c2 = step(0.4999, frac(uv * 3.5));
        half checker = lerp(0.1, 0.2, abs(c2.x - c2.y));

        half c = max(max(max(arc, circle), flash), checker);

        #if defined(UNITY_COLORSPACE_GAMMA)
        return half4(c, c, c, 1);
        #else
        return half4((half3)GammaToLinearSpace(c), 1);
        #endif
    }

    half4 frag_frequency(v2f_img i) : SV_Target
    {
        float2 uv = i.uv.xy - 0.25;

        half phi = atan2(uv.x * _MainTex_TexelSize.y * _MainTex_TexelSize.z, uv.y);
        half lim = saturate(fwidth(phi) * 20); // frequency limitter
        phi = lerp(sin(phi * 100), 0, lim);

        float2 freq = UNITY_PI * 0.5 / (1 + uv * 4);
        half2 comb = cos(freq * uv * _MainTex_TexelSize.zw);

        half2 sig = uv > 0;
        half c = lerp(phi, lerp(comb.x, comb.y, sig.y), abs(sig.x - sig.y));
        c = c / 2 + 0.5;

        #if defined(UNITY_COLORSPACE_GAMMA)
        return half4(c, c, c, 1);
        #else
        return half4((half3)GammaToLinearSpace(c), 1);
        #endif
    }

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag_fill
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma multi_compile __ UNITY_COLORSPACE_GAMMA
            #pragma vertex vert_img
            #pragma fragment frag_spectrum
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag_checker
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma multi_compile __ UNITY_COLORSPACE_GAMMA
            #pragma vertex vert_img
            #pragma fragment frag_pattern
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma multi_compile __ UNITY_COLORSPACE_GAMMA
            #pragma vertex vert_img
            #pragma fragment frag_shutter
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma multi_compile __ UNITY_COLORSPACE_GAMMA
            #pragma vertex vert_img
            #pragma fragment frag_frequency
            ENDCG
        }
    }
}
