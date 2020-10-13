using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawBoundaryAirWall : MonoBehaviour
{
    public float showRange = 10;

    public float xHighLimit = 50;
    public float xLowLimit = -50;
    public float yHighLimit = 50;
    public float yLowLimit = -50;
    public float zHighLimit = 50;
    public float zLowLimit = -50;

    public Color lineColor = new Color(1, 1, 1, 1);//画出的线的颜色，默认是白色
    public Material lineMaterial;//线的材质

    private Transform mainCamera;//主摄像机对象

    private void Start()
    {
        if (transform.tag == "MainCamera")
        {
            mainCamera = transform;
        }
        else
        {
            mainCamera = Camera.main.transform;
        }
    }
    
    private void OnPostRender()
    {
        CreateLineMaterial();
        lineMaterial.SetPass(0);

        GL.PushMatrix();

        float gridCount = (int)(showRange * 2);

        if (Mathf.Abs(mainCamera.position.x - xHighLimit) < showRange)
        {
            GL.Begin(GL.LINES);
            int middleY = Mathf.RoundToInt(mainCamera.position.y);
            int middleZ = Mathf.RoundToInt(mainCamera.position.z);
            GL.Color(new Color(lineColor.r, lineColor.g, lineColor.b, (showRange - Mathf.Abs(mainCamera.position.x - xHighLimit)) / showRange));
            for (int i = 0; i <= gridCount; i++)
            {
                if (middleY - gridCount / 2 + i >= yLowLimit && middleY - gridCount / 2 + i <= yHighLimit)
                {
                    GL.Vertex3(xHighLimit, middleY - gridCount / 2 + i, SetLimit(middleZ - gridCount / 2, 2));
                    GL.Vertex3(xHighLimit, middleY - gridCount / 2 + i, SetLimit(middleZ + gridCount / 2, 2));
                }


                if (middleZ - gridCount / 2 + i >= zLowLimit && middleZ - gridCount / 2 + i <= zHighLimit)
                {
                    GL.Vertex3(xHighLimit, SetLimit(middleY - gridCount / 2, 1), middleZ - gridCount / 2 + i);
                    GL.Vertex3(xHighLimit, SetLimit(middleY + gridCount / 2, 1), middleZ - gridCount / 2 + i);
                }
            }
            GL.End();
        }

        if (Mathf.Abs(mainCamera.position.x - xLowLimit) < showRange)
        {
            GL.Begin(GL.LINES);
            int middleY = Mathf.RoundToInt(mainCamera.position.y);
            int middleZ = Mathf.RoundToInt(mainCamera.position.z);
            GL.Color(new Color(lineColor.r, lineColor.g, lineColor.b, (showRange - Mathf.Abs(mainCamera.position.x - xLowLimit)) / showRange));
            for (int i = 0; i <= gridCount; i++)
            {
                if (middleY - gridCount / 2 + i >= yLowLimit && middleY - gridCount / 2 + i <= yHighLimit)
                {
                    GL.Vertex3(xLowLimit, middleY - gridCount / 2 + i, SetLimit(middleZ - gridCount / 2, 2));
                    GL.Vertex3(xLowLimit, middleY - gridCount / 2 + i, SetLimit(middleZ + gridCount / 2, 2));
                }
                if (middleZ - gridCount / 2 + i >= zLowLimit && middleZ - gridCount / 2 + i <= zHighLimit)
                {
                    GL.Vertex3(xLowLimit, SetLimit(middleY - gridCount / 2, 1), middleZ - gridCount / 2 + i);
                    GL.Vertex3(xLowLimit, SetLimit(middleY + gridCount / 2, 1), middleZ - gridCount / 2 + i);
                }
            }
            GL.End();
        }

        if (Mathf.Abs(mainCamera.position.y - yHighLimit) < showRange)
        {
            GL.Begin(GL.LINES);
            int middleX = Mathf.RoundToInt(mainCamera.position.x);
            int middleZ = Mathf.RoundToInt(mainCamera.position.z);
            GL.Color(new Color(lineColor.r, lineColor.g, lineColor.b, (showRange - Mathf.Abs(mainCamera.position.y - yHighLimit)) / showRange));
            for (int i = 0; i <= gridCount; i++)
            {
                if (middleX - gridCount / 2 + i >= xLowLimit && middleX - gridCount / 2 + i <= xHighLimit)
                {
                    GL.Vertex3(middleX - gridCount / 2 + i, yHighLimit, SetLimit(middleZ - gridCount / 2, 2));
                    GL.Vertex3(middleX - gridCount / 2 + i, yHighLimit, SetLimit(middleZ + gridCount / 2, 2));
                }
                if (middleZ - gridCount / 2 + i >= zLowLimit && middleZ - gridCount / 2 + i <= zHighLimit)
                {
                    GL.Vertex3(SetLimit(middleX - gridCount / 2, 0), yHighLimit, middleZ - gridCount / 2 + i);
                    GL.Vertex3(SetLimit(middleX + gridCount / 2, 0), yHighLimit, middleZ - gridCount / 2 + i);
                }
            }
            GL.End();
        }

        if (Mathf.Abs(mainCamera.position.y - yLowLimit) < showRange)
        {
            GL.Begin(GL.LINES);
            int middleX = Mathf.RoundToInt(mainCamera.position.x);
            int middleZ = Mathf.RoundToInt(mainCamera.position.z);
            GL.Color(new Color(lineColor.r, lineColor.g, lineColor.b, (showRange - Mathf.Abs(mainCamera.position.y - yLowLimit)) / showRange));
            for (int i = 0; i <= gridCount; i++)
            {
                if (middleX - gridCount / 2 + i >= xLowLimit && middleX - gridCount / 2 + i <= xHighLimit)
                {
                    GL.Vertex3(middleX - gridCount / 2 + i, yLowLimit, SetLimit(middleZ - gridCount / 2, 2));
                    GL.Vertex3(middleX - gridCount / 2 + i, yLowLimit, SetLimit(middleZ + gridCount / 2, 2));
                }
                if (middleZ - gridCount / 2 + i >= zLowLimit && middleZ - gridCount / 2 + i <= zHighLimit)
                {
                    GL.Vertex3(SetLimit(middleX - gridCount / 2, 0), yLowLimit, middleZ - gridCount / 2 + i);
                    GL.Vertex3(SetLimit(middleX + gridCount / 2, 0), yLowLimit, middleZ - gridCount / 2 + i);
                }
            }
            GL.End();
        }

        if (Mathf.Abs(mainCamera.position.z - zHighLimit) < showRange)
        {
            GL.Begin(GL.LINES);
            int middleX = Mathf.RoundToInt(mainCamera.position.x);
            int middleY = Mathf.RoundToInt(mainCamera.position.y);
            GL.Color(new Color(lineColor.r, lineColor.g, lineColor.b, (showRange - Mathf.Abs(mainCamera.position.z - zHighLimit)) / showRange));
            for (int i = 0; i <= gridCount; i++)
            {
                if (middleX - gridCount / 2 + i >= xLowLimit && middleX - gridCount / 2 + i <= xHighLimit)
                {
                    GL.Vertex3(middleX - gridCount / 2 + i, SetLimit(middleY - gridCount / 2, 1), zHighLimit);
                    GL.Vertex3(middleX - gridCount / 2 + i, SetLimit(middleY + gridCount / 2, 1), zHighLimit);
                }
                if (middleY - gridCount / 2 + i >= yLowLimit && middleY - gridCount / 2 + i <= yHighLimit)
                {
                    GL.Vertex3(SetLimit(middleX - gridCount / 2, 0), middleY - gridCount / 2 + i, zHighLimit);
                    GL.Vertex3(SetLimit(middleX + gridCount / 2, 0), middleY - gridCount / 2 + i, zHighLimit);
                }
            }
            GL.End();
        }

        if (Mathf.Abs(mainCamera.position.z - zLowLimit) < showRange)
        {
            GL.Begin(GL.LINES);
            int middleX = Mathf.RoundToInt(mainCamera.position.x);
            int middleY = Mathf.RoundToInt(mainCamera.position.y);
            GL.Color(new Color(lineColor.r, lineColor.g, lineColor.b, (showRange - Mathf.Abs(mainCamera.position.z - zLowLimit)) / showRange));
            for (int i = 0; i <= gridCount; i++)
            {
                if (middleX - gridCount / 2 + i >= xLowLimit && middleX - gridCount / 2 + i <= xHighLimit)
                {
                    GL.Vertex3(middleX - gridCount / 2 + i, SetLimit(middleY - gridCount / 2, 1), zLowLimit);
                    GL.Vertex3(middleX - gridCount / 2 + i, SetLimit(middleY + gridCount / 2, 1), zLowLimit);
                }
                if (middleY - gridCount / 2 + i >= yLowLimit && middleY - gridCount / 2 + i <= yHighLimit)
                {
                    GL.Vertex3(SetLimit(middleX - gridCount / 2, 0), middleY - gridCount / 2 + i, zLowLimit);
                    GL.Vertex3(SetLimit(middleX + gridCount / 2, 0), middleY - gridCount / 2 + i, zLowLimit);
                }
            }
            GL.End();
        }


        GL.PopMatrix();
    }

    private float SetLimit(float rawValue, int axis)
    {
        switch (axis)
        {
            case 0:
                if (rawValue < xLowLimit)
                {
                    rawValue = xLowLimit;
                }
                if (rawValue > xHighLimit)
                {
                    rawValue = xHighLimit;
                }
                return rawValue;
            case 1:
                if (rawValue < yLowLimit)
                {
                    rawValue = yLowLimit;
                }
                if (rawValue > yHighLimit)
                {
                    rawValue = yHighLimit;
                }
                return rawValue;
            case 2:
                if (rawValue < zLowLimit)
                {
                    rawValue = zLowLimit;
                }
                if (rawValue > zHighLimit)
                {
                    rawValue = zHighLimit;
                }
                return rawValue;
            default:
                return 0;
        }
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
}
