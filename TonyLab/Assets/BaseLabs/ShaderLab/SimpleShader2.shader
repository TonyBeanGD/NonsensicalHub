Shader "Custom/SimpleShader2" {
	SubShader{
		Pass{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			//a2v 把数据从应用阶段传到顶点着色器
			//在 Unity 中,填充到 POSITION / TANGENT / NORMAL 这些语句中的数据是由使用该材质 Mesh Render 组件提供的, 在每帧调用 Draw Call 的时候, Mesh Render 组件会把它负责渲染的模型数据发送给 Unity Shader.
			//一个模型通常包括了一组三角面片, 每个三角面片由三个顶点组成, 每个顶点又包含了一些数据, 例如顶点位置 / 法线 / 切线 / 纹理坐标 / 顶点颜色等
			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
			};

			// 使用一个结构体来定义顶点着色器的输出
			struct v2f {
				// SV_POSITION 语义告诉 Unity , pos 里包括了顶点在裁剪空间的位置信息
				float4 pos : SV_POSITION;
				// COLOR0 语义告诉 Unity 用于存储颜色信息
				fixed3 color : COLOR0;
			};

			v2f vert(a2v v) {
				// 生命输出结构
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.color = v.normal * 0.5 + fixed3(0.5,0.5,0.5);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target{
				// 将插值后的 i.color 显示到屏幕上
				return fixed4(i.color, 1.0);
			}

			ENDCG
		}
	}
}
