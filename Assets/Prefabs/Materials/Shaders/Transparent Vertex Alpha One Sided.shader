Shader "Custom/Transparent Vertex Alpha One Sided"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Smoothness("Smoothness", Range(0, 1)) = 0
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
            LOD 200

            CGINCLUDE
            struct Input
            {
                float2 uv_MainTex;
                float4 color: Color;

                float4 screenPos;
            };

            sampler2D _MainTex;
            half _Smoothness;

            void surf(Input IN, inout SurfaceOutputStandardSpecular o)
            {
    #ifdef LOD_FADE_CROSSFADE
                float2 vpos = IN.screenPos.xy / IN.screenPos.w * _ScreenParams.xy;
                UnityApplyDitherCrossFade(vpos);
    #endif
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
                o.Albedo = c.rgb * IN.color.rgb;
                o.Alpha = c.a * IN.color.a;
                o.Smoothness = _Smoothness;
                o.Specular = 0;
                o.Emission = 0;
            }
            ENDCG

            Cull Back
            CGPROGRAM

    #pragma multi_compile _ LOD_FADE_CROSSFADE
    #pragma surface surf StandardSpecular alpha
    #pragma target 3.0
            ENDCG
        }
            FallBack "Diffuse"
}
