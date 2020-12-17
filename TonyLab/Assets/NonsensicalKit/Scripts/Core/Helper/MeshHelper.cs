using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.Text;

namespace NonsensicalKit
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
        /// 网格是否包含坐标,性能较低，但可以判断凹体网格或者三角面方向不正确的网格
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
                    i--;
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

        /// <summary>
        /// 网格是否包含坐标,性能较低，但可以判断凹体网格或者三角面方向不正确的网格
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="pos_Local"></param>
        /// <returns></returns>
        public static bool Contain(this MeshBuffer mesh, Vector3 pos_Local)
        {
            Vector3[] vertices = mesh.vertices.ToArray();
            int[] triangle = mesh.triangles.ToArray();

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
                    i--;
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

        /// <summary>
        /// 网格是否包含坐标，性能较高，但要求网格为非凹体且网格三角面方向完全正确
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="pos_Local"></param>
        /// <returns></returns>
        public static bool Contain_2(this Mesh mesh, Vector3 pos_Local)
        {
            Vector3[] vertices = mesh.vertices;
            int[] triangle = mesh.triangles;

            Vector3 dir = Vector3.up;

            for (int i = 0; i < triangle.Length; i += 3)
            {
                Vector3 line1 = vertices[triangle[i + 1]] - vertices[triangle[i]];
                Vector3 line2 = vertices[triangle[i + 2]] - vertices[triangle[i]];

                Vector3 normal = Vector3.Cross(line1, line2);

                Vector3 posDir = pos_Local - vertices[triangle[i + 1]];

                if (Vector3.Dot(posDir, normal) > 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 网格是否包含坐标，性能较高，但要求网格为非凹体且网格三角面方向完全正确
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="pos_Local"></param>
        /// <returns></returns>
        public static bool Contain_2(this MeshBuffer mesh, Vector3 pos_Local)
        {
            Vector3[] vertices = mesh.vertices.ToArray();
            int[] triangle = mesh.triangles.ToArray();

            Vector3 dir = Vector3.up;

            for (int i = 0; i < triangle.Length; i += 3)
            {
                Vector3 line1 = vertices[triangle[i + 1]] - vertices[triangle[i]];
                Vector3 line2 = vertices[triangle[i + 2]] - vertices[triangle[i]];

                Vector3 normal = Vector3.Cross(line1, line2);

                Vector3 posDir = pos_Local - vertices[triangle[i + 1]];

                if (Vector3.Dot(posDir, normal) > 0)
                {
                    return false;
                }
            }

            return true;
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
    

    /// <summary>
    /// 用于缓存mesh数据以进行处理
    /// </summary>
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


        public MeshBuffer(string data)
        {
            vertices = new List<Vector3>();
            normals = new List<Vector3>();
            uv = new List<Vector2>();
            triangles = new List<int>();

            string[] parts = data.Split('&');
            string[] part1 = parts[0].Split('|');
            string[] part2 = parts[1].Split('|');
            string[] part3 = parts[2].Split('|');
            for (int i = 0; i < part1.Length; i++)
            {
                string[] part1Split = part1[i].Split('#');
                string[] part2Split = part2[i].Split('#');
                vertices.Add(new Vector3(int.Parse(part1Split[0]), int.Parse(part1Split[1]), int.Parse(part1Split[2])));
                normals.Add(new Vector2(int.Parse(part2Split[0]), int.Parse(part2Split[1])));
                uv.Add(Vector2.one * 0.5f);
                triangles.Add(int.Parse(part3[i]));
            }
        }


        public MeshBuffer()
        {
            vertices = new List<Vector3>();
            normals = new List<Vector3>();
            uv = new List<Vector2>();
            triangles = new List<int>();
        }

        public Vector3 GetVerticeByTrianglesIndex(int _index)
        {
            return vertices[triangles[_index]];
        }

        public MeshBuffer Clone()
        {
            MeshBuffer temp = new MeshBuffer()
            {
                vertices = new List<Vector3>(this.vertices.ToArray()),
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

        public void AddTriangle(Vector3[] _vertices, Vector3 _normal, Vector2 _uv)
        {
            int rawLength = vertices.Count;

            vertices.Add(_vertices[0]);
            vertices.Add(_vertices[1]);
            vertices.Add(_vertices[2]);

            normals.Add(_normal);
            normals.Add(_normal);
            normals.Add(_normal);

            uv.Add(_uv);
            uv.Add(_uv);
            uv.Add(_uv);

            triangles.Add(rawLength + 0);
            triangles.Add(rawLength + 1);
            triangles.Add(rawLength + 2);
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

        public void AddQuad_2(Vector3[] _vertices, Vector3 _center, Vector2 _uv)
        {
            int rawLength = vertices.Count;

            vertices.Add(_vertices[0]);
            vertices.Add(_vertices[1]);
            vertices.Add(_vertices[2]);
            vertices.Add(_vertices[3]);

            normals.Add(_vertices[0]-_center);
            normals.Add(_vertices[1]-_center);
            normals.Add(_vertices[2]-_center);
            normals.Add(_vertices[3]-_center);

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

        public void AddQuad_3(Vector3[] _vertices, Vector3[] _centers, Vector2 _uv)
        {
            int rawLength = vertices.Count;

            vertices.Add(_vertices[0]);
            vertices.Add(_vertices[1]);
            vertices.Add(_vertices[2]);
            vertices.Add(_vertices[3]);

            normals.Add(_vertices[0] - _centers[0]);
            normals.Add(_vertices[1] - _centers[1]);
            normals.Add(_vertices[2] - _centers[2]);
            normals.Add(_vertices[3] - _centers[3]);

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

        public void AddRound(Vector3 center, float radius, Vector3 dir, int smoothness)
        {
            if (smoothness < 3)
            {
                throw new Exception("点数过少");
            }
            Vector3 dir1 = VectorHelper.GetCommonVerticalLine(dir, dir);
            Vector3 dir2 = VectorHelper.GetCommonVerticalLine(dir, dir1);
            float partAngle = (2 * Mathf.PI) / smoothness;
            Vector3[] pointArray = new Vector3[smoothness];

            for (int i = 0; i < smoothness; i++)
            {
                pointArray[i] = center + radius * dir1 * Mathf.Sin(partAngle * i) + radius * dir2 * Mathf.Cos(partAngle * i);
            }

            for (int i = 0; i < pointArray.Length; i++)
            {
                int next = i + 1;
                if (next >= smoothness)
                {
                    next = 0;
                }
                AddTriangle(new Vector3[] { center, pointArray[i], pointArray[next] }, dir, Vector2.one * 0.5f);
            }
        }

        public void AddRing(Vector3 center, float innerDiameter, float outerDiameter, Vector3 dir, int smoothness)
        {
            if (smoothness < 3)
            {
                throw new Exception("点数过少");
            }
            if (innerDiameter == 0)
            {
                AddRound(center, outerDiameter, dir, smoothness);
            }

            Vector3 dir1 = VectorHelper.GetCommonVerticalLine(dir, dir);
            Vector3 dir2 = VectorHelper.GetCommonVerticalLine(dir, dir1);
            float partAngle = (2 * Mathf.PI) / smoothness;
            Vector3[] pointArray1 = new Vector3[smoothness];
            Vector3[] pointArray2 = new Vector3[smoothness];

            for (int i = 0; i < smoothness; i++)
            {
                pointArray1[i] = center + innerDiameter * dir1 * Mathf.Sin(partAngle * i) + innerDiameter * dir2 * Mathf.Cos(partAngle * i);
                pointArray2[i] = center + outerDiameter * dir1 * Mathf.Sin(partAngle * i) + outerDiameter * dir2 * Mathf.Cos(partAngle * i);
            }

            for (int i = 0; i < smoothness; i++)
            {
                int next = i + 1;
                if (next >= smoothness)
                {
                    next = 0;
                }

                //AddQuad(new Vector3[] { pointArray1[i], pointArray2[i], pointArray2[next], pointArray1[next] }, (pointArray1[i] + pointArray2[i] + pointArray2[next] + pointArray1[next]) * 0.25f, new Vector2(0.5f, 0.5f));
                AddQuad(new Vector3[] { pointArray1[i], pointArray2[i], pointArray2[next], pointArray1[next] },-dir, new Vector2(0.5f, 0.5f));
            }
        }

        public void AddRing3D(Vector3 ringSide1, float ringSide1Radius, Vector3 ringSide2, float ringSide2Radius, Vector3 dir, int smoothness)
        {
            if (smoothness < 3)
            {
                throw new Exception("点数过少");
            }

            Vector3 dir1 = VectorHelper.GetCommonVerticalLine(dir, dir);
            Vector3 dir2 = VectorHelper.GetCommonVerticalLine(dir, dir1);

            float partAngle = (2 * Mathf.PI) / smoothness;
            Vector3[] pointArray1 = new Vector3[smoothness];
            Vector3[] pointArray2 = new Vector3[smoothness];
            for (int i = 0; i < smoothness; i++)
            {
                pointArray1[i] = ringSide1 + ringSide1Radius * dir1 * Mathf.Sin(partAngle * i) + ringSide1Radius * dir2 * Mathf.Cos(partAngle * i);
                pointArray2[i] = ringSide2 + ringSide2Radius * dir1 * Mathf.Sin(partAngle * i) + ringSide2Radius * dir2 * Mathf.Cos(partAngle * i);
            }

            for (int i = 0; i < smoothness; i++)
            {
                int next = i + 1;
                if (next >= smoothness)
                {
                    next = 0;
                }
               // AddQuad(new Vector3[] { pointArray1[i], pointArray2[i], pointArray2[next], pointArray1[next] }, (pointArray1[i] + pointArray2[i] + pointArray2[next] + pointArray1[next]) * 0.25f- (ringSide1+ringSide2)*0.5f, new Vector2(0.5f, 0.5f));
                AddQuad_3(new Vector3[] { pointArray1[i], pointArray2[i], pointArray2[next], pointArray1[next] },new Vector3[] { ringSide1, ringSide2, ringSide2, ringSide1 }, new Vector2(0.5f, 0.5f));
            }
        }

        public string ToData()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append((int)vertices[0].x);
            builder.Append("#");
            builder.Append((int)vertices[0].y);
            builder.Append("#");
            builder.Append((int)vertices[0].z);
            for (int i = 1; i < vertices.Count; i++)
            {
                builder.Append("|");
                builder.Append((int)vertices[i].x);
                builder.Append("#");
                builder.Append((int)vertices[i].y);
                builder.Append("#");
                builder.Append((int)vertices[i].z);
            }
            builder.Append("&");
            builder.Append((int)normals[0].x);
            builder.Append("#");
            builder.Append((int)normals[0].y);
            for (int i = 1; i < normals.Count; i++)
            {
                builder.Append("|");
                builder.Append((int)normals[i].x);
                builder.Append("#");
                builder.Append((int)normals[i].y);
            }
            builder.Append("&");
            builder.Append(triangles[0]);
            for (int i = 1; i < normals.Count; i++)
            {
                builder.Append("|");
                builder.Append(triangles[i]);
            }
            return builder.ToString();
        }
    }
}
