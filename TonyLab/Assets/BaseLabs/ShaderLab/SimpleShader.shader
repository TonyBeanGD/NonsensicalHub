Shader "TBShader/SimpleShader" {

	SubShader{
		Pass{
			CGPROGRAM

			// 两条非常重要的编译指令
			#pragma vertex vert // 定点着色器
			#pragma fragment frag // 片元着色器

			// 这一步是把顶点坐标从模型空间转换到裁剪空间
			//vert , 顶点着色器代码, 逐顶点执行, 函数的输入 v 包含了这个顶点的位置, 返回值是该顶点在裁剪空间中的位置, POSITION : 把模型的顶点坐标填充到输入参数中, SV_POSITION : 顶点着色器的输出是裁剪空间中的顶点坐标
			float4 vert(float4 v : POSITION) : SV_POSITION {
				return UnityObjectToClipPos(v);
			}

			//frag , SV_Target : 把用户的输出颜色存储到一个渲染目标中, 这里将输出到默认的帧缓存中, 片元着色器输出的颜色的每个分量范围是[0,1].
			fixed4 frag() : SV_Target{
				return fixed4(1.0,1.0,1.0,1.0);
			}

			ENDCG
		}
	}
}