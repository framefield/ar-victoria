Shader "Custom/EdgeHighlight" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_EdgeMap ("Edge Map", 2D) = "black" {}
		_ExhibitionMode ("Exhibition Mode", Range(0,1)) = 0.0
		_EdgeThreshold ("Edge Threshold", Range(0,1)) = 0.1
		_GazeUV ("GazeUV",Vector) = (0.5,0.5,0.0,0.0)
		_Radius ("Radius",Range(0,1)) = 0.1
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _EdgeMap;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		half _ExhibitionMode;
		half _EdgeThreshold;
		half _Radius;
		half4 _GazeUV;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			if (_ExhibitionMode != 0.0f)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
				o.Albedo = c.rgb;
				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;
			}
			else
			{
				fixed4 c = tex2D(_EdgeMap, IN.uv_MainTex);
				if (c.r < _EdgeThreshold||length(IN.uv_MainTex-_GazeUV.xy)>_Radius)
				{
					fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
					o.Albedo = c.rgb;
					discard;
				}
				else
				{
					o.Albedo = float3(1.0f, 0.0f, 0.0f);
				}

				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = 1.0f;
			}
		}
		ENDCG
	}
	FallBack "Diffuse"
}
