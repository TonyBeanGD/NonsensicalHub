// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Grid"
{
	Properties
	{
		_gridColor("网格的颜色",Color) = (0.5,0.5,0.5)
		_tickWidth("网格的间距",Range(0.01,1)) = 0.1
		_gridWidth("网格的宽度",Range(0.0001,0.01)) = 0.008
	}
		SubShader
	{
		CGINCLUDE

		ENDCG

		Pass
	{
		CGPROGRAM
#pragma vertex vert  
#pragma fragment frag  

#include "UnityCG.cginc"  

		uniform float4 _backgroundColor;
	uniform float4 _gridColor;
	uniform float _tickWidth;
	uniform float _gridWidth;


	struct appdata
	{
		float4 vertex:POSITION;
		float2 uv:TEXCOORD0;
	};
	struct v2f
	{
		float2 uv:TEXCOORD0;
		float4 vertex:SV_POSITION;
	};
	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		return o;
	}

	float4 frag(v2f i) :COLOR
	{
		//将坐标的中心从左下角移动到网格的中心  
		float2 r = 2.0*i.uv ;
		float aspectRatio = _ScreenParams.x / _ScreenParams.y;

		float4 backgroundColor = _backgroundColor;

		float4 gridColor = _gridColor;

		float4 pixel = backgroundColor;
		float a = 0;

		if (fmod(r.x, _tickWidth) < _gridWidth/2)
		{
			pixel = gridColor;
		}
		if (_tickWidth-fmod(r.x, _tickWidth) < _gridWidth/2)
		{
			pixel = gridColor;
		}

		if (fmod(r.y, _tickWidth) < _gridWidth/2)
		{
			pixel = gridColor;

		}
		if (_tickWidth-fmod(r.y, _tickWidth) < _gridWidth/2)
		{
			pixel = gridColor;
		}

		if (abs(pixel.x) == backgroundColor.x
			&& abs(pixel.y) == backgroundColor.y
			&& abs(pixel.z) == backgroundColor.z
			&& abs(pixel.w) == backgroundColor.w
			)
		{
			discard;
		}

		return pixel;
	}
		ENDCG
	}

	}

}