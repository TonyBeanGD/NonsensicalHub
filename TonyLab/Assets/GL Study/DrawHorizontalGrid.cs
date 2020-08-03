using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawHorizontalGrid : MonoBehaviour
{
    public float horizontal = 0;//地平线高度
    public Color lineColor = new Color(1, 1, 1, 1);//画出的线的颜色，默认是白色
    public Material lineMaterial;//线的材质
    public int gridCount = 200;//单边格子个数

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
        float crtHeight = Mathf.Abs(mainCamera.position.y - horizontal);
        int crtLevel = GetLevel(crtHeight);

        CreateLineMaterial();
        lineMaterial.SetPass(0);

        GL.PushMatrix();

        GL.Begin(GL.LINES);

        //渲染当前级
        GL.Color(lineColor);
        float levelValue = Mathf.Pow(10, crtLevel);

        float pointX = GetNearValue(mainCamera.position.x, crtLevel);
        float pointZ = GetNearValue(mainCamera.position.z, crtLevel);
        for (int i = 0; i <= gridCount; i++)
        {
            GL.Vertex3(pointX + -gridCount / 2 * levelValue, horizontal, pointZ + (-gridCount / 2 + i) * levelValue);
            GL.Vertex3(pointX + gridCount / 2 * levelValue, horizontal, pointZ + (-gridCount / 2 + i) * levelValue);

            GL.Vertex3(pointX + (-gridCount / 2 + i) * levelValue, horizontal, pointZ + -gridCount / 2 * levelValue);
            GL.Vertex3(pointX + (-gridCount / 2 + i) * levelValue, horizontal, pointZ + gridCount / 2 * levelValue);
        }

        GL.End();

        if (crtHeight / Mathf.Pow(10, crtLevel) <= 2.5)
        {
            //渲染低一级
            GL.Begin(GL.LINES);

            float lowPointX = GetNearValue(mainCamera.position.x, crtLevel - 1);
            float lowPointZ = GetNearValue(mainCamera.position.z, crtLevel - 1);
            float lowLevelValue = levelValue / 10;
            Color lowLevelColor = lineColor;
            lowLevelColor.a *= (2.5f - crtHeight / levelValue) / 1.5f;
            GL.Color(lowLevelColor);
            for (int i = 0; i <= gridCount * 10; i++)
            {
                GL.Vertex3(lowPointX + (-gridCount * 10 / 2) * lowLevelValue, horizontal, lowPointZ + (-gridCount * 10 / 2 + i) * lowLevelValue);
                GL.Vertex3(lowPointX + (gridCount * 10 / 2) * lowLevelValue, horizontal, lowPointZ + (-gridCount * 10 / 2 + i) * lowLevelValue);

                GL.Vertex3(lowPointX + (-gridCount * 10 / 2 + i) * lowLevelValue, horizontal, lowPointZ + (-gridCount * 10 / 2) * lowLevelValue);
                GL.Vertex3(lowPointX + (-gridCount * 10 / 2 + i) * lowLevelValue, horizontal, lowPointZ + (gridCount * 10 / 2) * lowLevelValue);
            }

            GL.End();
        }
        else
        {
            //渲染高一级
            GL.Begin(GL.LINES);
            float highPointX = GetNearValue(mainCamera.position.x, crtLevel + 1);
            float highPointZ = GetNearValue(mainCamera.position.z, crtLevel + 1);
            float highLevelValue = levelValue * 10;
            Color highLevelColor = lineColor;
            highLevelColor.a *= 1 + (crtHeight / levelValue - 10) / 7.5f;
            GL.Color(highLevelColor);
            for (int i = 0; i <= gridCount / 10; i++)
            {
                GL.Vertex3(highPointX + (-gridCount / 10 / 2) * highLevelValue, horizontal, highPointZ + (-gridCount / 10 / 2 + i) * highLevelValue);
                GL.Vertex3(highPointX + (gridCount / 10 / 2) * highLevelValue, horizontal, highPointZ + (-gridCount / 10 / 2 + i) * highLevelValue);

                GL.Vertex3(highPointX + (-gridCount / 10 / 2 + i) * highLevelValue, horizontal, highPointZ + (-gridCount / 10 / 2) * highLevelValue);
                GL.Vertex3(highPointX + (-gridCount / 10 / 2 + i) * highLevelValue, horizontal, highPointZ + (gridCount / 10 / 2) * highLevelValue);
            }

            GL.End();
        }

        GL.PopMatrix();
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

    /// <summary>
    /// 获取传入数值的级数
    /// </summary>
    /// <param name="rawNum">传入数值</param>
    /// <returns>级数，个位数（如5）是0级，十位数（25）是1级，一位小数（0.5）是-1级</returns>
    private int GetLevel(float rawNum)
    {
        if (rawNum == 0)
        {
            return 0;
        }
        rawNum = Mathf.Abs(rawNum);
        if (rawNum > 1)
        {
            int level = -1;
            float crtNum = rawNum;
            while (crtNum > 1)
            {
                crtNum /= 10;
                level++;
            }
            return level;
        }
        else
        {
            int level = 0;
            float crtNum = rawNum;
            while (crtNum < 1)
            {
                crtNum *= 10;
                level--;
            }
            return level;
        }
    }

    /// <summary>
    /// 根据传入float变量与当前等级求最近的整值float变量
    /// </summary>
    /// <param name="rawFloat">传入的float变量</param>
    /// <param name="level">传入的float变量</param>
    /// <returns>最近的整值float变量</returns>
    private float GetNearValue(float rawFloat, int level)
    {
        float crtFloat = rawFloat / Mathf.Pow(10, level);
        float nearInt = Mathf.Round(crtFloat);
        return nearInt * Mathf.Pow(10, level);
    }

    /// <summary>
    /// 获取摄像机看向地平线的视点
    /// </summary>
    /// <param name="cameraPos">摄像机位置</param>
    /// <param name="cameraForwardPos">摄像机前方位置</param>
    /// <returns>没有看向地平线时返回null,否则返回视点的位置</returns>
    private Vector3? GetViewPoint(Vector3 cameraPos, Vector3 cameraForwardPos)
    {
        if ((cameraPos.y - horizontal) * (cameraForwardPos.y - horizontal) > 0//当摄像机的点和摄像机的前方点没有在地平线两侧时
                   && Mathf.Abs(cameraPos.y) - Mathf.Abs(cameraForwardPos.y) < 0)//且当没有看向地面时
        {
            return null;
        }
        else
        {
            float h1 = Mathf.Abs(cameraPos.y - horizontal);
            float h2 = Mathf.Abs(cameraForwardPos.y - horizontal);
            float l1 = Vector3.Distance(cameraPos, cameraForwardPos);
            float l2 = h1 * l1 / (h1 - h2);
            return cameraForwardPos + (cameraForwardPos - cameraPos).normalized * l2;
        }
    }
}
