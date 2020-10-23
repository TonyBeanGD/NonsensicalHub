using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MeshHelper
{
    /// <summary>
    /// 清除未被使用的顶点
    /// </summary>
    /// <param name="mesh"></param>
    public static void ClearUnuseVertex(Mesh mesh)
    {
        //值类型转引用类型存储
        List<Vector3?> vertexsQuote = new List<Vector3?>();
        List<Vector2> uv = new List<Vector2>(mesh.uv);
        List<Vector3> normals = new List<Vector3>(mesh.normals);

        foreach (var item in mesh.vertices)
        {
            vertexsQuote.Add(item);
        }

        //保存引用类型
        List<TriangleContainer> triangleContainers = new List<TriangleContainer>();

        foreach (var item in mesh.triangles)
        {
            triangleContainers.Add(new TriangleContainer(vertexsQuote[item]));
        }

        //获取未被使用的定点
        bool[] use = Enumerable.Repeat(false, mesh.vertexCount).ToArray();

        foreach (var item in mesh.triangles)
        {
            use[item] = true;
        }

        //从链表中清除未被使用的定点
        for (int i = 0, useCount = 0; i < vertexsQuote.Count; i++, useCount++)
        {
            if (use[useCount] == false)
            {
                vertexsQuote.RemoveAt(i);
                uv.RemoveAt(i);
                normals.RemoveAt(i);
                i--;
            }
        }

        //获取新三角数组
        List<int> triangles = new List<int>();

        foreach (var item in triangleContainers)
        {
            triangles.Add(vertexsQuote.IndexOf(item.Vertice));
        }

        //链表转数组
        List<Vector3> vertices = new List<Vector3>();
        foreach (var item in vertexsQuote)
        {
            vertices.Add((Vector3)item);
        }

        //赋值mesh
        mesh.triangles = triangles.ToArray();
        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.normals = normals.ToArray();
    }

    /// <summary>
    /// 根据顶点数组求出质点并返回回偏移量
    /// </summary>
    /// <param name="mesh">需要求质点的mesh</param>
    /// <returns></returns>
    public static Vector3 AutoCentroidShift(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;

        Vector3 sum = Vector3.zero;

        foreach (var item in vertices)
        {
            sum += item;
        }

        Vector3 offSet = -sum / vertices.Length;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] += offSet;
        }
       
        return offSet;
    }

    private class TriangleContainer
    {
        public Vector3? Vertice;

        public TriangleContainer(Vector3? _vertice)
        {
            Vertice = _vertice;
        }
    }
}
