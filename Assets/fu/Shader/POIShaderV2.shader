Shader "Unlit/POIShaderV2"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ActivePOITex("POITexture", 2D) = "white"{}
		_ActivePOIColor("ActiveColorArea", Color) = (0,0,0,0)
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
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _ActivePOITex;
			float4 _ActivePOIColor;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				half3 m = tex2D(_ActivePOITex, i.uv);
				half4 c = tex2D(_MainTex, i.uv);
				half3 res = m;
				if (m.r == _ActivePOIColor.r && m.g == _ActivePOIColor.g && m.b == _ActivePOIColor.b) res = c.rgb;

				//if (m.r == 0  m.g == 0  m.b == 0) res = c.rgb;
				//if (m.r >= .1 || m.g >= .1 || m.b >= .1) res = (_Color1.rgb * m.r) + (_Color2.rgb * m.g) + (_Color3.rgb * m.b);
				//i.Albedo = c.rgb * res;
				
				
				// sample the texture
				fixed4 col = tex2D(_MainTex, res.rgb);
				
				
				return col;
			}
			ENDCG
		}
	}
}
