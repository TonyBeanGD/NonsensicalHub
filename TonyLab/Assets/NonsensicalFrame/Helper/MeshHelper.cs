using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NonsensicalFrame
{
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

        /// <summary>
        /// 网格是否包含坐标
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="pos_Local"></param>
        /// <returns></returns>
        public static bool Contain(this Mesh mesh, Vector3 pos_Local)
        {
            Vector3[] vertices = mesh.vertices;
            int[] triangle = mesh.triangles;

            Vector3 dir = Vector3.up;

            List<Vector3> crossPoint = new List<Vector3>();
            for (int i = 0; i < triangle.Length; i += 3)
            {
                Vector3? cross = VectorHelper.GetRayTriangleCrossPoint(pos_Local, dir, vertices[triangle[i]], vertices[triangle[i + 1]], vertices[triangle[i + 2]]);
                if (cross != null)
                {
                    crossPoint.Add((Vector3)cross);
                }
            }
            VectorHelper.SortList(crossPoint);

            for (int i = 0; i < crossPoint.Count - 1; i++)
            {
                if (VectorHelper.IsNear(crossPoint[i], crossPoint[i + 1]))
                {
                    crossPoint.RemoveAt(i + 1);
                }
            }

            if (crossPoint.Count % 2 == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
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


    public class MeshBuffer
    {
        public List<Vector3> vertices;
        public List<Vector3> normals;
        public List<Vector2> uv;
        public List<int> triangles;
        
        public MeshBuffer(Mesh mesh)
        {
            vertices = new List<Vector3>(mesh.vertices);
            normals = new List<Vector3>(mesh.normals);
            uv = new List<Vector2>(mesh.uv);
            triangles = new List<int>(mesh.triangles);
        }

        public MeshBuffer()
        {
            vertices = new List<Vector3>();
            normals = new List<Vector3>();
            uv = new List<Vector2>();
            triangles = new List<int>();
        }

        public MeshBuffer Clone()
        {
            MeshBuffer temp = new MeshBuffer()
            {
                vertices = new List< Vector3 > (this.vertices.ToArray()),
                normals = new List<Vector3>(this.normals.ToArray()),
                uv = new List<Vector2>(this.uv.ToArray()),
                triangles = new List<int>(this.triangles.ToArray())
            };

            return temp;
        }

    public void Apply(Mesh mesh)
        {
            mesh.Clear();
            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetUVs(0, uv);
            mesh.SetTriangles(triangles, 0);
        }
        
        public Mesh GetMesh()
        {
            Mesh mesh = new Mesh();

            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetUVs(0, uv);
            mesh.SetTriangles(triangles, 0);

            return mesh;
        }

        public void AddQuad(Vector3[] _vertices, Vector3 _normal, Vector2 _uv)
        {
            int rawLength = vertices.Count;

            vertices.Add(_vertices[0]);
            vertices.Add(_vertices[1]);
            vertices.Add(_vertices[2]);
            vertices.Add(_vertices[3]);

            normals.Add(_normal);
            normals.Add(_normal);
            normals.Add(_normal);
            normals.Add(_normal);

            uv.Add(_uv);
            uv.Add(_uv);
            uv.Add(_uv);
            uv.Add(_uv);
            
            triangles.Add(rawLength + 0);
            triangles.Add(rawLength + 1);
            triangles.Add(rawLength + 3);

            triangles.Add(rawLength + 1);
            triangles.Add(rawLength + 2);
            triangles.Add(rawLength + 3);
        }
    }
}
