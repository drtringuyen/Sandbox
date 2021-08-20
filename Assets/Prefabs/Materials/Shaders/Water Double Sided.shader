Shader "Custom/Water Double Sided"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _BumpMap("Bumpmap", 2D) = "bump" {}
        _NoiseMap("Noise (RGB)", 2D) = "white" {}
        _NormalStrength("Normal", Range(0.0,10.0)) = 0.0
        [HDR]_EmissionColor("Color", Color) = (0,0,0)
        _Smoothness("Smoothness", Range(0, 1)) = 0
        _ScrollYSpeed("Y", Range(-10,10)) = -0.1
        _Gravity("Gravity", Range(1,100)) = 20
        _NoiseStrength("Noise Strength", Range(0,1)) = 0.5
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            CGINCLUDE
            struct Input
            {
                float2 uv_MainTex;
                float2 uv_BumpMap;
                float2 uv_NoiseMap;
                float4 color: Color;

                float3 worldNormal;
                INTERNAL_DATA
            };

            sampler2D _MainTex;
            sampler2D _BumpMap;
            sampler2D _NoiseMap;
            half _NormalStrength;
            float4 _EmissionColor;
            half _Smoothness;

            float3 _Vector;

            fixed _ScrollYSpeed;
            fixed _Gravity;
            fixed _NoiseStrength;

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                // Surface angle.
                _Vector = half3(0, 1, 0);
                float3 worldNormal = WorldNormalVector(IN, o.Normal);
                half w = dot(worldNormal, normalize(_Vector));
                // Y angle in red.
                half3 color2 = lerp(half3(1, 0, 0), half3(0, 1, 0), w);

                // Flow.
                float3 flowDir = color2.r;

                // Steeper angles will cause higher speed.
                flowDir = half3(0, _ScrollYSpeed + ((_Gravity * _ScrollYSpeed) * color2.r), 0);

                // Calculate noise blend.
                half dampByAngle = 1 - (abs(color2.r));
                // Damp noise strength with angle.
                _NoiseStrength = _NoiseStrength * dampByAngle;
                float cycleOffset = tex2D(_NoiseMap, IN.uv_MainTex).r * _NoiseStrength;

                float phase0 = frac(cycleOffset + _Time[1] * 0.5f + 0.5f);
                float phase1 = frac(cycleOffset + _Time[1] * 0.5f + 1.0f);

                // Scroll albedo.
                half2 uvPhase0 = IN.uv_MainTex + flowDir.xy * phase0;
                half2 uvPhase1 = IN.uv_MainTex + flowDir.xy * phase1;

                half3 tex0 = tex2D(_MainTex, uvPhase0);
                half3 tex1 = tex2D(_MainTex, uvPhase1);

                float flowLerp = abs((0.5f - phase0) / 0.5f);
                half3 finalColor = lerp(tex0, tex1, flowLerp);

                // Albedo.
                o.Albedo = finalColor.rgb * IN.color.rgb;
                o.Alpha = IN.color.r;

                // Scroll normal.

                // Damp the normal strength with angle.
                half nStrength = _NormalStrength * dampByAngle;

                tex0 = UnpackScaleNormal(tex2D(_BumpMap, uvPhase0), nStrength);
                tex1 = UnpackScaleNormal(tex2D(_BumpMap, uvPhase1), nStrength);

                finalColor = lerp(tex0, tex1, flowLerp);

                // Normal.
                o.Normal = finalColor;
                o.Smoothness = _Smoothness;

                // Emission.
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
    #pragma target 3.0
            ENDCG
        }
            FallBack "Diffuse"
}
