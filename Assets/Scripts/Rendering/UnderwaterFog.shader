// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Hidden/UnderwaterFog"
{
	Properties
	{
		_intensity("Intensity", Float) = 1.0
		_offset("Offset", Float) = 0.0

		_fogColor("Fog Color", Color) = (0, 0.1, 0.1, 1)
		_disabled("Disabled", Int) = 1
		_waterY("Water Y", Float) = 0
	}

		SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 viewVector: TEXCOORD1;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				float3 viewVector = mul(unity_CameraInvProjection, float4(v.uv * 2 - 1, 0, -1));
				o.viewVector = mul(unity_CameraToWorld, float4(viewVector, 0));
				return o;
			}

			sampler2D _CameraColorTexture;
			sampler2D _CameraDepthTexture;
			float _intensity;
			float _offset;

			fixed4 _fogColor;
			float _waterY;

			fixed4 frag(v2f i) : SV_Target
			{
				float depth = tex2D(_CameraDepthTexture, i.uv).r;
				depth = LinearEyeDepth(depth);

				fixed4 source = tex2D(_CameraColorTexture, i.uv);

				float3 n = float3(0, 1, 0);
				float3 p0 = float3(0, _waterY, 0);
				float3 l0 = _WorldSpaceCameraPos.xyz;
				float3 l = i.viewVector;
				bool aboveWater = l0.y > _waterY;

				float ldotn = dot(l, n);

				if (ldotn == 0)
					return source;

				float waterDistance = dot(p0 - l0, n) / ldotn;
				float waterDepth = 0;

				if (aboveWater)
				{
					if (waterDistance < 0 || depth < waterDistance)
						return source;
					waterDepth = max(0, depth - waterDistance);
				}
				else
				{
					//looking down
					if (waterDistance < 0)
					{
						waterDepth = depth;
					}
					//looking up
					else
					{
						//object in front of water surface when looking up
						if (depth < waterDistance)
						{
							waterDepth = depth;
						}
						//water border in front of overworld object
						else
						{
							waterDepth = waterDistance;
						}
					}
				}

				float ad = (waterDepth + _offset);

				return lerp(source, _fogColor, saturate(sqrt(ad) * _intensity));
			}

			ENDCG
		}
	}
}