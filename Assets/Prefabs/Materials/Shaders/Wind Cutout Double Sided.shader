Shader "Custom/Wind Cutout Double Sided"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Cutoff("Base Alpha Cutoff", Range(0,0.9)) = 0.5
        _Smoothness("Smoothness", Range(0, 1)) = 0

        _wind_dir("Wind Direction", Vector) = (0.5,0.05,0.5,0)
        _wind_size("Wind Wave Size", range(5,50)) = 5

        _tree_sway_stutter_influence("Tree Sway Stutter Influence", range(0,1)) = 0
        _tree_sway_stutter("Tree Sway Stutter", range(0,10)) = 0
        _tree_sway_speed("Tree Sway Speed", range(0,10)) = 1
        _tree_sway_disp("Tree Sway Displacement", range(0,1)) = 0

        _branches_disp("Branches Displacement", range(0,0.5)) = 0.015

        _leaves_wiggle_disp("Leaves Wiggle Displacement", float) = 0.1
        _leaves_wiggle_speed("Leaves Wiggle Speed", float) = 0.01

        _a_influence("Vertex Color Alpha Influence", range(0,1)) = 0.1
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

                float4 screenPos;
            };

            sampler2D _MainTex;
            half _Smoothness;

            float4 _wind_dir;
            float _wind_size;
            float _tree_sway_speed;
            float _tree_sway_disp;
            float _leaves_wiggle_disp;
            float _leaves_wiggle_speed;
            float _branches_disp;
            float _tree_sway_stutter;
            float _tree_sway_stutter_influence;
            float _a_influence;

            void surf(Input IN, inout SurfaceOutputStandardSpecular o)
            {
#ifdef LOD_FADE_CROSSFADE
                float2 vpos = IN.screenPos.xy / IN.screenPos.w * _ScreenParams.xy;
                UnityApplyDitherCrossFade(vpos);
#endif
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
                o.Albedo = c.rgb * IN.color.rgb;
                o.Alpha = c.a;
                o.Smoothness = _Smoothness;
                o.Specular = 0;
                o.Emission = 0;
            }
            ENDCG

            Cull Front
            CGPROGRAM

    #pragma multi_compile _ LOD_FADE_CROSSFADE
    #pragma surface surf StandardSpecular alphatest:_Cutoff vertex:vert vertex:wind
    #pragma target 3.0

            void vert(inout appdata_full v)
            {
                v.normal = -v.normal;
            }

            void wind(inout appdata_full i)
            {
                float3 worldPos = mul(unity_ObjectToWorld, i.vertex).xyz;

                // Tree movement.
                i.vertex.x += (cos(_Time.z * _tree_sway_speed + (worldPos.x / _wind_size) + (sin(_Time.z * _tree_sway_stutter * _tree_sway_speed + (worldPos.x / _wind_size)) * _tree_sway_stutter_influence)) + 1) / 2 * _tree_sway_disp * _wind_dir.x * (i.vertex.y / 10) +
                    cos(_Time.w * i.vertex.x * _leaves_wiggle_speed + (worldPos.x / _wind_size)) * _leaves_wiggle_disp * _wind_dir.x * i.color.a * _a_influence;

                i.vertex.z += (cos(_Time.z * _tree_sway_speed + (worldPos.z / _wind_size) + (sin(_Time.z * _tree_sway_stutter * _tree_sway_speed + (worldPos.z / _wind_size)) * _tree_sway_stutter_influence)) + 1) / 2 * _tree_sway_disp * _wind_dir.z * (i.vertex.y / 10) +
                    cos(_Time.w * i.vertex.z * _leaves_wiggle_speed + (worldPos.x / _wind_size)) * _leaves_wiggle_disp * _wind_dir.z * i.color.a * _a_influence;

                i.vertex.y += cos(_Time.z * _tree_sway_speed + (worldPos.z / _wind_size)) * _tree_sway_disp * _wind_dir.y * (i.vertex.y / 10);

                // Branches movement.
                i.vertex.x += sin(_Time.w * _tree_sway_speed + _wind_dir.x + (worldPos.z / _wind_size)) * _branches_disp * i.color.a * _a_influence;
            }
            ENDCG

            Cull Off
            CGPROGRAM

    #pragma multi_compile _ LOD_FADE_CROSSFADE
    #pragma surface surf StandardSpecular alphatest:_Cutoff vertex:wind
    #pragma target 3.0

            void wind (inout appdata_full i) 
            {
                float3 worldPos = mul (unity_ObjectToWorld, i.vertex).xyz;
                
                // Tree movement.
                i.vertex.x += (cos(_Time.z * _tree_sway_speed + (worldPos.x/_wind_size) + (sin(_Time.z * _tree_sway_stutter * _tree_sway_speed + (worldPos.x/_wind_size)) * _tree_sway_stutter_influence) ) + 1)/2 * _tree_sway_disp * _wind_dir.x * (i.vertex.y / 10) + 
                cos(_Time.w * i.vertex.x * _leaves_wiggle_speed + (worldPos.x/_wind_size)) * _leaves_wiggle_disp * _wind_dir.x * i.color.a * _a_influence;
                
                i.vertex.z += (cos(_Time.z * _tree_sway_speed + (worldPos.z/_wind_size) + (sin(_Time.z * _tree_sway_stutter * _tree_sway_speed + (worldPos.z/_wind_size)) * _tree_sway_stutter_influence) ) + 1)/2 * _tree_sway_disp * _wind_dir.z * (i.vertex.y / 10) + 
                cos(_Time.w * i.vertex.z * _leaves_wiggle_speed + (worldPos.x/_wind_size)) * _leaves_wiggle_disp * _wind_dir.z * i.color.a * _a_influence;
                
                i.vertex.y += cos(_Time.z * _tree_sway_speed + (worldPos.z/_wind_size)) * _tree_sway_disp * _wind_dir.y * (i.vertex.y / 10);
                
                // Branches movement.
                i.vertex.x += sin(_Time.w * _tree_sway_speed + _wind_dir.x + (worldPos.z/_wind_size)) * _branches_disp  * i.color.a * _a_influence;
            }
            ENDCG
        }
            FallBack "Diffuse"
}
