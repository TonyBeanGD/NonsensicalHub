using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GLTest : MonoBehaviour
{
    public Material material;
    void OnPostRender()
    {
        GL.PushMatrix();
        material.SetPass(0);

        int i = Time.frameCount % 180;

        GL.Begin(GL.LINES);
        GL.Color(new Color(Mathf.Abs((i - 90) / 90f), 1 - Mathf.Abs((i - 90) / 90f), Mathf.Abs(i - 90) / 180f, 1));
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(Mathf.Sin(i / 90f * Mathf.PI), Mathf.Cos(i / 90f * Mathf.PI), 0);
        GL.End();

        GL.PopMatrix();
    }
}
