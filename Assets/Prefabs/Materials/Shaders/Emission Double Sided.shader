Shader "Custom/Emission Double Sided"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        [HDR]_EmissionColor("Color", Color) = (0,0,0)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGINCLUDE
        struct Input
        {
            float2 uv_MainTex;
            float4 color: Color;
        };

        sampler2D _MainTex;
        float4 _EmissionColor;

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb * IN.color.rgb;
            o.Alpha = c.a * IN.color.a;

#ifndef UNITY_COLORSPACE_GAMMA
            o.Emission = LinearToGammaSpace(_EmissionColor.rgb);
#endif
#ifndef UNITY_COLORSPACE_GAMMA
            o.Emission = GammaToLinearSpace(_EmissionColor.rgb);
#endif
        }
        ENDCG

        Cull Front
        CGPROGRAM
#pragma surface surf Standard vertex:vert
#pragma surface surf Lambert alpha:fade nofog
#pragma target 3.0

        void vert(inout appdata_full v)
        {
            v.normal = -v.normal;
        }
        ENDCG

        Cull Off
        CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0
        ENDCG
    }
    FallBack "Diffuse"
}
