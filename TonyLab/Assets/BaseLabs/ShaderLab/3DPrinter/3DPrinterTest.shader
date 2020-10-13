Shader "Custom/3DprinterTest"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_MainTex("Adbedo(RGB)", 2D) = "white" {}

		_Metallic("Metallic", Range(0, 1)) = 0.0
		_MetallicTex("Metallic",2D) = "white"{}

		_Glossiness("Smoothness", Range(0, 1)) = 0.5

		[Normal]_NormalTex("Normal Map",2D) = "bump"{}

		_ConstructY("constructY", float) = 0
		_ConstructGap("constructGap", float) = 0
		_ConstructColor("constructColor", Color) = (0.5, 0.5, 0.5, 0.5)
		[Toggle] _useWobbly("_useWobbly", Float) = 1
		 _wobblySize("_wobblySize", Float) = 1
		 _wobblyHeight("_wobblyHeight", Float) = 1	//一厘米为单位
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		Cull Off

		CGPROGRAM

		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0 

		sampler2D _MainTex;


		sampler2D _MetallicTex;
		sampler2D _NormalTex;

		float _ConstructY;
		float _ConstructGap;
		fixed4 _ConstructColor;
		float _useWobbly;
		float _wobblySize;
		float _wobblyHeight;

		float3 viewDir;
		int building;

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_MetallicTex;
			float2 uv_NormalTex;
			float3 worldPos;
			float3 viewDir;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{

			viewDir = IN.viewDir;

			float3 normal = UnpackNormal(tex2D(_NormalTex, IN.uv_NormalTex));

			o.Normal = normal;

			float s ;

			if(_useWobbly)
			{
 				s = +sin((IN.worldPos.x + IN.worldPos.z) * _wobblySize + _Time[3] ) /200*_wobblyHeight;
			}
			else
			{
				s = 0;
			}

			if (IN.worldPos.y < _ConstructY)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				o.Alpha = c.a;
				building = 0;
			}
			else if(IN.worldPos.y < _ConstructY + _ConstructGap +s)
			{
				o.Albedo = _ConstructColor.rgb;
				o.Alpha = _ConstructColor.a;
				building = 1;
			}
			else	
			{
				discard;
			}

			o.Metallic = tex2D(_MetallicTex, IN.uv_MetallicTex).rgb;
			o.Smoothness = _Glossiness;
		}

		inline void LightingCustom_GI(SurfaceOutputStandard s, UnityGIInput data,inout UnityGI gi)
		{
			LightingStandard_GI(s,data,gi);
		}

		inline half4 LightingCustom(SurfaceOutputStandard s, half3 lightDir, UnityGI gi)
		{
			if (building)
				return _ConstructColor;// 不使用光照模型渲染
			if (dot(s.Normal, viewDir) > 0)
				return _ConstructColor; //避免出现模型空洞

			return LightingStandard(s, lightDir, gi);
		}

		ENDCG
	}
	FallBack "Standard"
}
