using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAxis : MonoBehaviour
{
    public Camera targetCamera;

    public Material lineMaterial = null;

    private int screenWidth;
    private int screenHeight;
    private Vector2 xOffset;
    private Vector2 yOffset;
    private Vector2 zOffset;

    private const int offset = 10;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void Update()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        Vector3 middlePoint = targetCamera.ScreenToWorldPoint(new Vector3(screenWidth / 2, screenHeight / 2, 1));

        Vector3 xDirPoint = targetCamera.WorldToScreenPoint(middlePoint + new Vector3(0.03f, 0, 0));
        Vector3 yDirPoint = targetCamera.WorldToScreenPoint(middlePoint + new Vector3(0, 0.03f, 0));
        Vector3 zDirPoint = targetCamera.WorldToScreenPoint(middlePoint + new Vector3(0, 0, 0.03f));

        xOffset = new Vector2(xDirPoint.x - screenWidth / 2, xDirPoint.y - screenHeight / 2);
        yOffset = new Vector2(yDirPoint.x - screenWidth / 2, yDirPoint.y - screenHeight / 2);
        zOffset = new Vector2(zDirPoint.x - screenWidth / 2, zDirPoint.y - screenHeight / 2);
    }

    private void OnPostRender()
    {
        CreateLineMaterial();
        if (targetCamera.orthographic == true)
        {
            return;
        }
        Vector3 end = Input.mousePosition;//鼠标当前位置
        GL.PushMatrix();//保存摄像机变换矩阵,把投影视图矩阵和模型视图矩阵压入堆栈保存
        lineMaterial.SetPass(0);//为渲染激活给定的pass。
        GL.LoadPixelMatrix();//设置用屏幕坐标绘图

        float proportion = screenWidth / 1000f + screenHeight / 1000f;

        #region x

        TextX tx = new TextX(2, 2, 0, 5, proportion);

        GL.Begin(GL.LINES);

        GL.Color(new Color(1, 0, 0));

        GL.Vertex3(proportion * offset, proportion * offset, 0);
        GL.Vertex3(proportion * offset + xOffset.x, proportion * offset + xOffset.y, 0);

        GL.Vertex3(proportion * offset + xOffset.x + tx.point1X, proportion * offset + xOffset.y + tx.point1Y, 0);
        GL.Vertex3(proportion * offset + xOffset.x + tx.point2X, proportion * offset + xOffset.y + tx.point2Y, 0);

        GL.Vertex3(proportion * offset + xOffset.x + tx.point3X, proportion * offset + xOffset.y + tx.point3Y, 0);
        GL.Vertex3(proportion * offset + xOffset.x + tx.point4X, proportion * offset + xOffset.y + tx.point4Y, 0);
        GL.End();

        #endregion

        #region y

        TextY ty = new TextY(2, 3, 0, 5, proportion);

        GL.Begin(GL.LINES);

        GL.Color(new Color(0, 1, 0));

        GL.Vertex3(proportion * offset, proportion * offset, 0);
        GL.Vertex3(proportion * offset + yOffset.x, proportion * offset + yOffset.y, 0);

        GL.Vertex3(proportion * offset + yOffset.x + ty.point1X, proportion * offset + yOffset.y + ty.point1Y, 0);
        GL.Vertex3(proportion * offset + yOffset.x + ty.point2X, proportion * offset + yOffset.y + ty.point2Y, 0);

        GL.Vertex3(proportion * offset + yOffset.x + ty.point3X, proportion * offset + yOffset.y + ty.point3Y, 0);
        GL.Vertex3(proportion * offset + yOffset.x + ty.point4X, proportion * offset + yOffset.y + ty.point4Y, 0);

        GL.End();

        #endregion

        #region z 

        TextZ tz = new TextZ(2, 2, 0, 5, proportion);

        GL.Begin(GL.LINES);

        GL.Color(new Color(0, 0, 1));

        GL.Vertex3(proportion * offset, proportion * offset, 0);
        GL.Vertex3(proportion * offset + zOffset.x, proportion * offset + zOffset.y, 0);

        GL.Vertex3(proportion * offset + zOffset.x + tz.point1X, proportion * offset + zOffset.y + tz.point1Y, 0);
        GL.Vertex3(proportion * offset + zOffset.x + tz.point2X, proportion * offset + zOffset.y + tz.point2Y, 0);

        GL.Vertex3(proportion * offset + zOffset.x + tz.point2X, proportion * offset + zOffset.y + tz.point2Y, 0);
        GL.Vertex3(proportion * offset + zOffset.x + tz.point3X, proportion * offset + zOffset.y + tz.point3Y, 0);

        GL.Vertex3(proportion * offset + zOffset.x + tz.point3X, proportion * offset + zOffset.y + tz.point3Y, 0);
        GL.Vertex3(proportion * offset + zOffset.x + tz.point4X, proportion * offset + zOffset.y + tz.point4Y, 0);

        GL.End();

        #endregion

        GL.PopMatrix();//恢复摄像机投影矩阵
    }

    private void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    private struct TextX
    {
        public float point1X;
        public float point1Y;
        public float point2X;
        public float point2Y;
        public float point3X;
        public float point3Y;
        public float point4X;
        public float point4Y;

        public TextX(float halfWidth, float halfHeight, float middlePointX, float middlePointY, float proportion)
        {
            point1X = proportion * (middlePointX - halfWidth);
            point1Y = proportion * (middlePointY + halfHeight);
            point2X = proportion * (middlePointX + halfWidth);
            point2Y = proportion * (middlePointY - halfHeight);
            point3X = proportion * (middlePointX + halfWidth);
            point3Y = proportion * (middlePointY + halfHeight);
            point4X = proportion * (middlePointX - halfWidth);
            point4Y = proportion * (middlePointY - halfHeight);
        }
    }

    private struct TextY
    {
        public float point1X;
        public float point1Y;
        public float point2X;
        public float point2Y;
        public float point3X;
        public float point3Y;
        public float point4X;
        public float point4Y;

        public TextY(float halfWidth, float halfHeight, float middlePointX, float middlePointY, float proportion)
        {
            point1X = proportion * (middlePointX - halfWidth);
            point1Y = proportion * (middlePointY + halfHeight);
            point2X = proportion * (middlePointX);
            point2Y = proportion * (middlePointY);
            point3X = proportion * (middlePointX + halfWidth);
            point3Y = proportion * (middlePointY + halfHeight);
            point4X = proportion * (middlePointX - halfWidth);
            point4Y = proportion * (middlePointY - halfHeight);
        }
    }

    private struct TextZ
    {
        public float point1X;
        public float point1Y;
        public float point2X;
        public float point2Y;
        public float point3X;
        public float point3Y;
        public float point4X;
        public float point4Y;

        public TextZ(float halfWidth, float halfHeight, float middlePointX, float middlePointY, float proportion)
        {
            point1X = proportion * (middlePointX - halfWidth);
            point1Y = proportion * (middlePointY + halfHeight);
            point2X = proportion * (middlePointX + halfWidth);
            point2Y = proportion * (middlePointY + halfHeight);
            point3X = proportion * (middlePointX - halfWidth);
            point3Y = proportion * (middlePointY - halfHeight);
            point4X = proportion * (middlePointX + halfWidth);
            point4Y = proportion * (middlePointY - halfHeight);
        }
    }
}
