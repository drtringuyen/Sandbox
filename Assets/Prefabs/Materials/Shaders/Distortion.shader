Shader "Custom/Distortion" {
	Properties{
		_MainTex("Main Tex", 2D) = "white"{}
		_MaskMap("Mask", 2D) = "white"{}
		_BumpMap("Normal Map", 2D) = "bump"{}
		_Cubemap("Environment Cubemap", Cube) = "_Skybox"{}
		_Distortion("Distortion", Range(0, 10000)) = 100
		_RefractAmount("Refraction Amount", Range(0, 1)) = 1
		_ScrollYSpeed("Scroll Speed Y", Range(-10,10)) = -0.1
	}

		SubShader{
			Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }

			GrabPass { "_RefractionTex" }

			pass {
				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				sampler2D _MainTex;
				float4 _MainTex_ST;
				sampler2D _MaskMap;
				float4 _MaskMap_ST;
				sampler2D _BumpMap;
				float4 _BumpMap_ST;
				samplerCUBE _Cubemap;
				float _Distortion;
				fixed _RefractAmount;
				sampler2D _RefractionTex;
				float4 _RefractionTex_TexelSize;
				fixed _ScrollYSpeed;

				struct a2v {
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float4 tangent : TANGENT;
					float4 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 pos : SV_POSITION;
					float4 scrPos : TEXCOORD0;
					float4 uv : TEXCOORD1;
					float4 TtoW0 : TEXCOORD2;
					float4 TtoW1 : TEXCOORD3;
					float4 TtoW2 : TEXCOORD4;
				};

				v2f vert(a2v v) {
					v2f o;

					// Billboard facing.
					v.vertex.xy *= float2(length(unity_ObjectToWorld._m00_m10_m20), length(unity_ObjectToWorld._m01_m11_m21));

					float3 forward = -normalize(UNITY_MATRIX_V._m20_m21_m22);
					float3 up = normalize(UNITY_MATRIX_V._m10_m11_m12);
					float3 right = normalize(UNITY_MATRIX_V._m00_m01_m02);

					float4x4 rotationMatrix = float4x4(right, 0,
						up, 0,
						forward, 0,
						0, 0, 0, 1);
					v.vertex = mul(v.vertex, rotationMatrix);
					v.normal = mul(v.normal, rotationMatrix);

					v.vertex.xyz = mul((float3x3)unity_WorldToObject, v.vertex.xyz);
					v.normal = mul(v.normal, (float3x3)unity_ObjectToWorld);

					// Grab pass.
					o.pos = UnityObjectToClipPos(v.vertex);
					o.scrPos = ComputeGrabScreenPos(o.pos);

					o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);

					float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
					fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
					fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

					o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
					o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
					o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

					return o;
				}

				fixed4 frag(v2f i) : SV_Target {
					float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
					fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));

					half2 dir = half2(0, 0);
					half2 flowDir = half2(0, _ScrollYSpeed);
					half2 uvPhase = i.uv + (flowDir.xy * _Time[1]);
					fixed3 bump = UnpackNormal(tex2D(_BumpMap, uvPhase)) * tex2D(_MaskMap, i.uv.xy).r;

					fixed2 offset = bump * _Distortion * _RefractionTex_TexelSize;
					i.scrPos.xy = (offset + i.scrPos.xy) / i.scrPos.w;
					fixed3 refrColor = tex2D(_RefractionTex, i.scrPos.xy).rgb;

					bump = normalize(half3(dot(i.TtoW0.xyz, bump), dot(i.TtoW1.xyz, bump), dot(i.TtoW2.xyz, bump)));
					fixed3 reflDir = reflect(-worldViewDir, bump);
					fixed3 texColor = tex2D(_MainTex, i.uv.xy).rgb;
					fixed3 reflColor = texCUBE(_Cubemap, reflDir).rgb * texColor;
					fixed3 finalColor = lerp(reflColor, refrColor, _RefractAmount);

					return fixed4(finalColor, 1.0);
				}

			ENDCG
			}
		}

		FallBack "Diffuse"
}
