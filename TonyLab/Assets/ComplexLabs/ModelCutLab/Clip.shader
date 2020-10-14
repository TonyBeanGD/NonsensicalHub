Shader "TBShader/Clip" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Metallic ("Metallic", Range(0,1)) = 0.5
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_ClipColor ("ClipColor", Color) = (1,1,0,1)
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		LOD 200

		Cull Back
		CGPROGRAM

		#pragma surface surf Standard noshadow 
		#pragma target 3.0 

		fixed4 _Color;
		sampler2D _MainTex;
		half _Metallic;
		half _Glossiness;
		uniform half4 _Clip;

		struct Input
		{
			float2 uv_MainTex;
			float3 worldPos;
			float3 viewDir;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			half distance = dot(IN.worldPos, _Clip.xyz)+ _Clip.w;
			if (distance<0)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				o.Alpha = c.a;
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
			}
			else
			{
				discard;
			}

		}

		ENDCG
		

		Cull Front
		CGPROGRAM
		
		#pragma surface surf Standard noshadow 
		#pragma target 3.0 
		
		fixed4 _ClipColor; 
		uniform half4 _Clip;

		struct Input
		{
			float3 worldPos;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			half distance = dot(IN.worldPos, _Clip.xyz)+ _Clip.w;
			if (distance<0)
			{
				o.Albedo = _ClipColor.rgb;
				o.Alpha = _ClipColor.a;
			}
			else
			{
				discard;
			}
		}
		ENDCG
	}
}
