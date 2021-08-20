Shader "Custom/Emission Color Double Sided"
{
    Properties
    {
        _Color("Color", Color) = (0,0,0)
        [HDR]_EmissionColor("Color", Color) = (0,0,0)
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            CGINCLUDE
            struct Input
            {
                float4 color: Color;
            };

            float4 _Color;
            float4 _EmissionColor;

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                o.Albedo = _Color.rgb * IN.color.rgb;
                o.Alpha = _Color.a * IN.color.a;

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
    #pragma target 3.0

            void vert(inout appdata_full v)
            {
                v.normal = -v.normal;
            }
            ENDCG

            Cull Off
            CGPROGRAM
    #pragma surface surf Standard
    #pragma surface surf Lambert alpha:fade nofog
    #pragma target 3.0
            ENDCG
        }
            FallBack "Diffuse"
}
