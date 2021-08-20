Shader "Custom/Cutout Scroll Double Sided"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        [HDR]_EmissionColor("Color", Color) = (0,0,0)
        _EmissionIntensity("Vertex Color Emission Intensity", Range(0,100.0)) = 1
        _Cutoff("Base Alpha Cutoff", Range(0,0.9)) = 0.5
        _Smoothness("Smoothness", Range(0, 1)) = 0
        _ScrollYSpeed("Scroll Speed Y", Range(-10,10)) = -0.1
    }
        SubShader
        {
            Tags {  "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" "RenderType" = "TransparentCutout" "Queue" = "AlphaTest"}
            LOD 200

            CGINCLUDE
            struct Input
            {
                float2 uv_MainTex;
                float4 color: Color;
            };

            sampler2D _MainTex;
            float4 _EmissionColor;
            float _EmissionIntensity;
            half _Smoothness;

            float3 _Vector;

            fixed _ScrollYSpeed;

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                // Scroll albedo.
                half2 flowDir = half2(0, _ScrollYSpeed);
                half2 uvPhase0 = IN.uv_MainTex + flowDir.xy * _Time[1];
                half4 finalColor = tex2D(_MainTex, uvPhase0);

                // Albedo.
                o.Albedo = finalColor.rgb * IN.color.rgb;
                o.Alpha = finalColor.a;
                o.Smoothness = _Smoothness;

                // Emission.
    #ifndef UNITY_COLORSPACE_GAMMA
                o.Emission = LinearToGammaSpace(_EmissionColor * (IN.color.rgb * _EmissionIntensity));
    #endif
    #ifndef UNITY_COLORSPACE_GAMMA
                o.Emission = GammaToLinearSpace(_EmissionColor * (IN.color.rgb * _EmissionIntensity));
    #endif
            }
            ENDCG

            Cull Front
            CGPROGRAM
    #pragma surface surf Standard alphatest:_Cutoff vertex:vert
    #pragma target 3.0

            void vert(inout appdata_full v)
            {
                v.normal = -v.normal;
            }
            ENDCG

            Cull Off
            CGPROGRAM
    #pragma surface surf Standard alphatest:_Cutoff

    #pragma surface surf Lambert alpha:fade nofog

    #pragma target 3.0
            ENDCG
        }
            FallBack "Diffuse"
}
