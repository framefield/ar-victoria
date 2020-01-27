Shader "Unlit/TestUnlit"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f u) : SV_Target
			{
			//#define t iTime
			//#define r iResolution.xy
				float t = 0.5;
				float3 c;
				float l,z = t;
				for (int i = 0; i < 3; i++) {
					float2 uv,p = u.uv;
					uv = u.uv;
					p -= .5;
					p.x *= 0.5;
					z += .07;
					l = length(p);
					uv += p / l * (sin(z) + 1.)*abs(sin(l*9. - z * 2.));
					c[i] = .01 / length(abs(fmod(uv,1.) - .5));
				}
				return fixed4(c / l,t);
			}
			ENDCG
		}
	}
}
