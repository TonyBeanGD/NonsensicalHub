using NonsensicalFrame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public abstract class GranulationObject : MonoBehaviour
{
    public struct Granulation
    {
        public int level;
        public bool[,,] points;
        public Vector3 origin;

        /// <summary>
        /// TODO:可以使用DEXEL模型进行初始化的优化(射线组)
        /// </summary>
        public Granulation(int _level, Mesh mesh)
        {
            level = _level;
            float step = Mathf.Pow(10, level);

            Bounds bounds = mesh.bounds;

            origin = NumHelper.GetNearVector3(bounds.center = bounds.min, -1);

            int x = ((bounds.max.x - bounds.min.x) / step).RoundingToInt_Add();
            int y = ((bounds.max.y - bounds.min.y) / step).RoundingToInt_Add();
            int z = ((bounds.max.z - bounds.min.z) / step).RoundingToInt_Add();

            points = new bool[x, y, z];

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    for (int k = 0; k < z; k++)
                    {
                        Vector3 offset = new Vector3((i + 0.5f) * step, (j + 0.5f) * step, (k + 0.5f) * step);
                        points[i, j, k] = mesh.Contain(origin + offset);
                    }
                }
            }
        }
    }

    public Granulation granulation;

    protected Mesh mesh;

    protected virtual void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        granulation = new Granulation(-1, mesh);
        ReRenderMesh();
    }

    protected void ReRenderMesh()
    {
        //生成方块debug
        if (false)
        {
            for (int i = 0; i < granulation.points.GetLength(0); i++)
            {
                for (int j = 0; j < granulation.points.GetLength(1); j++)
                {
                    for (int k = 0; k < granulation.points.GetLength(2); k++)
                    {
                        if (granulation.points[i, j, k] == true)
                        {
                            GameObject newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            newCube.transform.position = new Vector3(i, j, k);
                        }
                    }
                }
            }
            return;
        }


        //MeshBuffer meshBuffer = new MeshBuffer();

        //float step = Mathf.Pow(10, granulation.level);

        //bool[,,,] bools = new bool[granulation.points.GetLength(0), granulation.points.GetLength(1), granulation.points.GetLength(2), 6];

        //for (int i = 0; i < granulation.points.GetLength(0); i++)
        //{
        //    for (int j = 0; j < granulation.points.GetLength(1); j++)
        //    {
        //        for (int k = 0; k < granulation.points.GetLength(2); k++)
        //        {
        //            if (granulation.points[i, j, k] == true)
        //            {
        //                Vector3 offset = new Vector3((i + 0.5f) * step, (j + 0.5f) * step, (k + 0.5f) * step);

        //                if (i == 0 || (i > 0 && granulation.points[i - 1, j, k] == false))
        //                {
        //                    AddFace(meshBuffer, granulation.origin + offset, 1, granulation.level);
        //                }

        //                if (i == granulation.points.GetLength(0) - 1 || (i < granulation.points.GetLength(0) - 1 && granulation.points[i + 1, j, k] == false))
        //                {
        //                    AddFace(meshBuffer, granulation.origin + offset, 2, granulation.level);
        //                }

        //                if (j == 0 || (j > 0 && granulation.points[i, j - 1, k] == false))
        //                {
        //                    AddFace(meshBuffer, granulation.origin + offset, 3, granulation.level);
        //                }

        //                if (j == granulation.points.GetLength(1) - 1 || (j < granulation.points.GetLength(1) - 1 && granulation.points[i, j + 1, k] == false))
        //                {
        //                    AddFace(meshBuffer, granulation.origin + offset, 4, granulation.level);
        //                }

        //                if (k == 0 || (k > 0 && granulation.points[i, j, k - 1] == false))
        //                {
        //                    AddFace(meshBuffer, granulation.origin + offset, 5, granulation.level);
        //                }

        //                if (k == granulation.points.GetLength(2) - 1 || (k < granulation.points.GetLength(2) - 1 && granulation.points[i, j, k + 1] == false))
        //                {
        //                    AddFace(meshBuffer, granulation.origin + offset, 6, granulation.level);
        //                }
        //            }
        //        }
        //    }
        //}
        //meshBuffer.Apply(mesh);

        MeshBuffer meshBuffer = new MeshBuffer();

        bool[,,,] bool6s = new bool[granulation.points.GetLength(0), granulation.points.GetLength(1), granulation.points.GetLength(2), 6];

        for (int i = 0; i < granulation.points.GetLength(0); i++)
        {
            for (int j = 0; j < granulation.points.GetLength(1); j++)
            {
                for (int k = 0; k < granulation.points.GetLength(2); k++)
                {
                    if (granulation.points[i, j, k] == true)
                    {
                        if (i == 0 || (i > 0 && granulation.points[i - 1, j, k] == false))
                        {
                            if (bool6s[i, j, k, 0] == false)
                            {
                                AddLittleFace(meshBuffer, granulation, 1, new Int3(i, j, k), bool6s);
                            }
                        }

                        if (i == granulation.points.GetLength(0) - 1 || (i < granulation.points.GetLength(0) - 1 && granulation.points[i + 1, j, k] == false))
                        {
                            if (bool6s[i, j, k, 1] == false)
                            {
                                AddLittleFace(meshBuffer, granulation, 2, new Int3(i, j, k), bool6s);
                            }
                        }

                        if (j == 0 || (j > 0 && granulation.points[i, j - 1, k] == false))
                        {
                            if (bool6s[i, j, k, 2] == false)
                            {
                                AddLittleFace(meshBuffer, granulation, 3, new Int3(i, j, k), bool6s);
                            }
                        }

                        if (j == granulation.points.GetLength(1) - 1 || (j < granulation.points.GetLength(1) - 1 && granulation.points[i, j + 1, k] == false))
                        {
                            if (bool6s[i, j, k, 3] == false)
                            {
                                AddLittleFace(meshBuffer, granulation, 4, new Int3(i, j, k), bool6s);
                            }
                        }

                        if (k == 0 || (k > 0 && granulation.points[i, j, k - 1] == false))
                        {
                            if (bool6s[i, j, k, 4] == false)
                            {
                                AddLittleFace(meshBuffer, granulation, 5, new Int3(i, j, k), bool6s);
                            }
                        }

                        if (k == granulation.points.GetLength(2) - 1 || (k < granulation.points.GetLength(2) - 1 && granulation.points[i, j, k + 1] == false))
                        {
                            if (bool6s[i, j, k, 5] == false)
                            {
                                AddLittleFace(meshBuffer, granulation, 6, new Int3(i, j, k), bool6s);
                            }
                        }
                    }
                }
            }
        }
        meshBuffer.Apply(mesh);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="meshBuffer"></param>
    /// <param name="granulation"></param>
    /// <param name="dir">1到6分别为x负，x正，y负，y正，z负，z正</param>
    /// <param name="crtPoint"></param>
    /// <param name="bool6s"></param>
    private void AddLittleFace(MeshBuffer meshBuffer, Granulation granulation, int dir, Int3 crtPoint, bool[,,,] bool6s)
    {
        Vector3 normal;

        Int3 dir1;
        Int3 dir2;

        switch (dir)
        {
            case 1:
            case 2:
                {
                    dir1 = new Int3(0, 1, 0);
                    dir2 = new Int3(0, 0, 1);
                    normal = new Vector3(1, 0, 0);
                }
                break;
            case 3:
            case 4:
                {
                    dir1 = new Int3(1, 0, 0);
                    dir2 = new Int3(0, 0, 1);
                    normal = new Vector3(0, 1, 0);
                }
                break;
            case 5:
            case 6:
                {
                    dir1 = new Int3(1, 0, 0);
                    dir2 = new Int3(0, 1, 0);
                    normal = new Vector3(0, 0, 1);
                }
                break;
            default:
                return;
        }

        Stack<Int3> points = new Stack<Int3>();

        int minDir1Limit = -1;
        int maxDir1Limit = 2147483647;
        int minDir2Limit = -1;
        int maxDir2Limit = 2147483647;

        points.Push(crtPoint);

        List<Int3> changes = new List<Int3>();

        while (points.Count > 0)
        {
            Int3 point = points.Pop();

            int dir1Value = point.GetValue(dir1);
            int dir2Value = point.GetValue(dir2);

            if (dir1Value < minDir1Limit || dir1Value > maxDir1Limit
                || dir2Value < minDir2Limit || dir2Value > maxDir2Limit)
            {
                continue;
            }
            bool6s[point.i1, point.i2, point.i3, dir - 1] = true;
            changes.Add(point);

            Int3 dir1Negative = point + (-dir1);
            Int3 dir1Positive = point + dir1;
            Int3 dir2Negative = point + (-dir2);
            Int3 dir2Positive = point + dir2;

            int arrMax1 = granulation.points.GetLength(0) - 1;
            int arrMax2 = granulation.points.GetLength(1) - 1;
            int arrMax3 = granulation.points.GetLength(2) - 1;

            if (dir1Negative.CheckBound(arrMax1, arrMax2, arrMax3) == true
                && granulation.points[dir1Negative.i1, dir1Negative.i2, dir1Negative.i3] == true
                && bool6s[dir1Negative.i1, dir1Negative.i2, dir1Negative.i3, dir - 1] == false)
            {
                points.Push(dir1Negative);
            }
            else if (dir1Value > minDir1Limit)
            {
                if (changes.Contains(dir1Negative) == false)
                {
                    minDir1Limit = dir1Value;
                }
            }

            if (dir1Positive.CheckBound(arrMax1, arrMax2, arrMax3) == true
                && granulation.points[dir1Positive.i1, dir1Positive.i2, dir1Positive.i3] == true
                && bool6s[dir1Positive.i1, dir1Positive.i2, dir1Positive.i3, dir - 1] == false)
            {
                points.Push(dir1Positive);
            }
            else if (dir1Value < maxDir1Limit)
            {
                if (changes.Contains(dir1Positive) == false)
                {
                    maxDir1Limit = dir1Value;
                }
            }

            if (dir2Negative.CheckBound(arrMax1, arrMax2, arrMax3) == true
                && granulation.points[dir2Negative.i1, dir2Negative.i2, dir2Negative.i3] == true
                && bool6s[dir2Negative.i1, dir2Negative.i2, dir2Negative.i3, dir - 1] == false)
            {
                points.Push(dir2Negative);
            }
            else if (dir2Value > minDir2Limit)
            {
                if (changes.Contains(dir2Negative) == false)
                {
                    minDir2Limit = dir2Value;
                }
            }

            if (dir2Positive.CheckBound(arrMax1, arrMax2, arrMax3) == true
                && granulation.points[dir2Positive.i1, dir2Positive.i2, dir2Positive.i3] == true
                && bool6s[dir2Positive.i1, dir2Positive.i2, dir2Positive.i3, dir - 1] == false)
            {
                points.Push(dir2Positive);
            }
            else if (dir2Value < maxDir2Limit)
            {
                if (changes.Contains(dir2Positive) == false)
                {
                    maxDir2Limit = dir2Value;
                }
            }
        }

        foreach (var point in changes)
        {
            int dir1Value = point.GetValue(dir1);
            int dir2Value = point.GetValue(dir2);

            if (dir1Value < minDir1Limit || dir1Value > maxDir1Limit
                || dir2Value < minDir2Limit || dir2Value > maxDir2Limit)
            {
                bool6s[point.i1, point.i2, point.i3, dir - 1] = false;
            }
        }

        Vector3[] point4 = new Vector3[4];
        float step = Mathf.Pow(10, granulation.level);
        float distance = step * 0.5f;

        Vector3 dir1V3 = new Vector3(dir1.i1, dir1.i2, dir1.i3);
        Vector3 dir2V3 = new Vector3(dir2.i1, dir2.i2, dir2.i3);

        Vector3 dir1MinOffset = (minDir1Limit - crtPoint.GetValue(dir1)) * dir1V3 * step;
        Vector3 dir1MaxOffset = (maxDir1Limit - crtPoint.GetValue(dir1)) * dir1V3 * step;
        Vector3 dir2MinOffset = (minDir2Limit - crtPoint.GetValue(dir2)) * dir2V3 * step;
        Vector3 dir2MaxOffset = (maxDir2Limit - crtPoint.GetValue(dir2)) * dir2V3 * step;

        Debugger.Log(dir1MinOffset, dir1MaxOffset, dir2MinOffset, dir2MaxOffset);

        point4[0] = dir1MinOffset + dir2MinOffset + -dir1V3 * distance + -dir2V3 * distance;
        point4[1] = dir1MaxOffset + dir2MinOffset + dir1V3 * distance + -dir2V3 * distance;
        point4[2] = dir1MaxOffset + dir2MaxOffset + dir1V3 * distance + dir2V3 * distance;
        point4[3] = dir1MinOffset + dir2MaxOffset + -dir1V3 * distance + dir2V3 * distance;

        Debugger.Log(crtPoint);
        Debugger.Log(StringHelper.GetSetString(point4));
        int rawLength = meshBuffer.vertices.Count;

        Vector3 offset = new Vector3((crtPoint.i1 + 0.5f) * step, (crtPoint.i2 + 0.5f) * step, (crtPoint.i3 + 0.5f) * step);
        Vector3 origin = granulation.origin + offset;
        Vector3 faceCenterPoint = origin + normal * distance;

        if (dir == 2 || dir == 3 || dir == 6)
        {
            meshBuffer.vertices.Add(faceCenterPoint + point4[0]);
            meshBuffer.vertices.Add(faceCenterPoint + point4[1]);
            meshBuffer.vertices.Add(faceCenterPoint + point4[2]);
            meshBuffer.vertices.Add(faceCenterPoint + point4[3]);
        }
        else
        {
            meshBuffer.vertices.Add(faceCenterPoint + point4[2]);
            meshBuffer.vertices.Add(faceCenterPoint + point4[1]);
            meshBuffer.vertices.Add(faceCenterPoint + point4[0]);
            meshBuffer.vertices.Add(faceCenterPoint + point4[3]);
        }

        meshBuffer.uv.Add(Vector2.one * 0.5f);
        meshBuffer.uv.Add(Vector2.one * 0.5f);
        meshBuffer.uv.Add(Vector2.one * 0.5f);
        meshBuffer.uv.Add(Vector2.one * 0.5f);

        meshBuffer.normals.Add(normal);
        meshBuffer.normals.Add(normal);
        meshBuffer.normals.Add(normal);
        meshBuffer.normals.Add(normal);

        meshBuffer.triangles.Add(rawLength + 0);
        meshBuffer.triangles.Add(rawLength + 1);
        meshBuffer.triangles.Add(rawLength + 3);

        meshBuffer.triangles.Add(rawLength + 1);
        meshBuffer.triangles.Add(rawLength + 2);
        meshBuffer.triangles.Add(rawLength + 3);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="meshBuffer"></param>
    /// <param name="origin"></param>
    /// <param name="distance"></param>
    private void AddFace(MeshBuffer meshBuffer, Vector3 origin, int dir, int level)
    {
        Vector3[] point4 = new Vector3[4];
        float distance = Mathf.Pow(10, level) * 0.5f;

        Vector3 normal;

        switch (dir)
        {
            case 1:
                {
                    point4[0] = new Vector3(0, -distance, distance);
                    point4[1] = new Vector3(0, distance, distance);
                    point4[2] = new Vector3(0, distance, -distance);
                    point4[3] = new Vector3(0, -distance, -distance);

                    normal = new Vector3(-1, 0, 0);
                }
                break;
            case 2:
                {
                    point4[0] = new Vector3(0, -distance, -distance);
                    point4[1] = new Vector3(0, distance, -distance);
                    point4[2] = new Vector3(0, distance, distance);
                    point4[3] = new Vector3(0, -distance, distance);

                    normal = new Vector3(1, 0, 0);
                }
                break;
            case 3:
                {
                    point4[0] = new Vector3(distance, 0, -distance);
                    point4[1] = new Vector3(distance, 0, distance);
                    point4[2] = new Vector3(-distance, 0, distance);
                    point4[3] = new Vector3(-distance, 0, -distance);

                    normal = new Vector3(0, -1, 0);
                }
                break;
            case 4:
                {
                    point4[0] = new Vector3(-distance, 0, -distance);
                    point4[1] = new Vector3(-distance, 0, distance);
                    point4[2] = new Vector3(distance, 0, distance);
                    point4[3] = new Vector3(distance, 0, -distance);

                    normal = new Vector3(0, 1, 0);
                }
                break;
            case 5:
                {
                    point4[0] = new Vector3(-distance, -distance, 0);
                    point4[1] = new Vector3(-distance, distance, 0);
                    point4[2] = new Vector3(distance, distance, 0);
                    point4[3] = new Vector3(distance, -distance, 0);

                    normal = new Vector3(0, 0, -1);
                }
                break;
            case 6:
                {
                    point4[0] = new Vector3(distance, -distance, 0);
                    point4[1] = new Vector3(distance, distance, 0);
                    point4[2] = new Vector3(-distance, distance, 0);
                    point4[3] = new Vector3(-distance, -distance, 0);

                    normal = new Vector3(0, 0, 1);
                }
                break;
            default:
                return;
        }

        int rawLength = meshBuffer.vertices.Count;

        Vector3 faceCenterPoint = origin + normal * distance;

        meshBuffer.vertices.Add(faceCenterPoint + point4[0]);
        meshBuffer.vertices.Add(faceCenterPoint + point4[1]);
        meshBuffer.vertices.Add(faceCenterPoint + point4[2]);
        meshBuffer.vertices.Add(faceCenterPoint + point4[3]);

        meshBuffer.uv.Add(Vector2.one * 0.5f);
        meshBuffer.uv.Add(Vector2.one * 0.5f);
        meshBuffer.uv.Add(Vector2.one * 0.5f);
        meshBuffer.uv.Add(Vector2.one * 0.5f);

        meshBuffer.normals.Add(normal);
        meshBuffer.normals.Add(normal);
        meshBuffer.normals.Add(normal);
        meshBuffer.normals.Add(normal);

        meshBuffer.triangles.Add(rawLength + 0);
        meshBuffer.triangles.Add(rawLength + 1);
        meshBuffer.triangles.Add(rawLength + 3);

        meshBuffer.triangles.Add(rawLength + 1);
        meshBuffer.triangles.Add(rawLength + 2);
        meshBuffer.triangles.Add(rawLength + 3);

    }
}
