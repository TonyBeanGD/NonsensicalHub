Shader "Unlit/axis"
{
	Properties
	{
		_Color("Color", color) = (1,1,1,1)
		_Color2("Color2",color) = (1,1,1,1)
		_MainIntensity("Intensity",int) = 1
		_SecondIntensity("Intensity",int) = 0
	}
	SubShader
	{
		Pass
		{
			Tags {"RenderType" = "Opaque" "Queue" = "Transparent" }
			
            Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			#include"Lighting.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal:NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float3 color :COLOR;
				float4 vertex : SV_POSITION;
				float3 normal:NORMAL;
			};

			float4 _Color;
			float4 _Color2;
			int _MainIntensity;
			int _SecondIntensity;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
				float3 worldnormal = normalize(mul(i.normal, (float3x3)unity_WorldToObject));
				float3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
				float3 diffuse = _LightColor0.rgb * 5 * _Color.rgb*saturate(dot(worldnormal, worldLight));

				i.color = (ambient + diffuse + _Color * 0.4)*_MainIntensity + _Color2 * _SecondIntensity;
				return fixed4(i.color,_Color.w);
			}
			ENDCG
		}
	}
}
