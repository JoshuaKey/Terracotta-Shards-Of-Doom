Shader "Custom/ColorSwap" {
	Properties{
		_MainTex ("Sprite Texture", 2D) = "white" {}
	    _Color ("Color Replacement", Color) = (1, 1, 1, 1)

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



		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		sampler2D _MainTex;
		float4 _Color;

		half _Glossiness;
		half _Metallic;

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o) {
			float r = tex2D(_MainTex, IN.uv_MainTex).r;
			float g = tex2D(_MainTex, IN.uv_MainTex).g;
			float b = tex2D(_MainTex, IN.uv_MainTex).b;

			float bright = r * 0.3 + g * 0.59 + b * 0.11;

			float3 color = bright * _Color;

			o.Albedo = color;
			o.Alpha = tex2D(_MainTex, IN.uv_MainTex).a;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}

		ENDCG
	}
	FallBack "Diffuse"
}

// Either specific values and Replacement
// Lerp based on color relativity
// Move UVs to more centralized locartion and adjust color from UV