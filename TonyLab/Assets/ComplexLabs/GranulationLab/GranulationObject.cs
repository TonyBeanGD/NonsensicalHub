using NonsensicalFrame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class GranulationObject : MonoBehaviour
{
    [SerializeField]
    protected bool DrawMesh = true;

    [SerializeField]
    protected int level = -1;

    public Granulation granulation;

    private Mesh mesh;

    public bool needRefresh;

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        granulation = new Granulation(level, mesh);
        RenderMesh();
    }

    protected virtual void Update()
    {
        if (Time.frameCount%10==0)
        {
            if (needRefresh)
            {
                needRefresh = false;
                RenderMesh();
            }
        }
        
    }

    protected virtual void OnDestroy()
    {

    }

    protected void RenderMesh()
    {
        if (DrawMesh == false)
        {
            return;
        }

        MeshBuffer meshBuffer = new MeshBuffer();
        
        Bool4Array bool6s = new Bool4Array(granulation.length0, granulation.length1, granulation.length2, 6);
        
        for (int i = 0; i < granulation.length0; i++)
        {
            for (int j = 0; j < granulation.length1; j++)
            {
                for (int k = 0; k < granulation.length2; k++)
                {
                    if (granulation.points[i, j, k] == true)
                    {
                        if (i == 0 || (i > 0 && granulation.points[i - 1, j, k] == false))
                        {
                            if (bool6s[i, j, k, 0] == false)
                            {
                                AddFace(meshBuffer, granulation, 1, new Int3(i, j, k), bool6s);
                            }
                        }

                        if (i == granulation.length0 - 1 || (i < granulation.length0 - 1 && granulation.points[i + 1, j, k] == false))
                        {
                            if (bool6s[i, j, k, 1] == false)
                            {
                                AddFace(meshBuffer, granulation, 2, new Int3(i, j, k), bool6s);
                            }
                        }

                        if (j == 0 || (j > 0 && granulation.points[i, j - 1, k] == false))
                        {
                            if (bool6s[i, j, k, 2] == false)
                            {
                                AddFace(meshBuffer, granulation, 3, new Int3(i, j, k), bool6s);
                            }
                        }

                        if (j == granulation.length1 - 1 || (j < granulation.length1 - 1 && granulation.points[i, j + 1, k] == false))
                        {
                            if (bool6s[i, j, k, 3] == false)
                            {
                                AddFace(meshBuffer, granulation, 4, new Int3(i, j, k), bool6s);
                            }
                        }

                        if (k == 0 || (k > 0 && granulation.points[i, j, k - 1] == false))
                        {
                            if (bool6s[i, j, k, 4] == false)
                            {
                                AddFace(meshBuffer, granulation, 5, new Int3(i, j, k), bool6s);
                            }
                        }

                        if (k == granulation.length2 - 1 || (k < granulation.length2 - 1 && granulation.points[i, j, k + 1] == false))
                        {
                            if (bool6s[i, j, k, 5] == false)
                            {
                                AddFace(meshBuffer, granulation, 6, new Int3(i, j, k), bool6s);
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
    private void AddFace(MeshBuffer meshBuffer, Granulation granulation, int dir, Int3 crtPoint, Bool4Array bool6s)
    {
        Vector3 normal;

        Int3 dir1;
        Int3 dir2;

        switch (dir)
        {
            case 1:
                {
                    dir1 = new Int3(0, 1, 0);
                    dir2 = new Int3(0, 0, 1);
                    normal = new Vector3(-1, 0, 0);
                }
                break;
            case 2:
                {
                    dir1 = new Int3(0, 1, 0);
                    dir2 = new Int3(0, 0, 1);
                    normal = new Vector3(1, 0, 0);
                }
                break;
            case 3:
                {
                    dir1 = new Int3(1, 0, 0);
                    dir2 = new Int3(0, 0, 1);
                    normal = new Vector3(0, -1, 0);
                }
                break;
            case 4:
                {
                    dir1 = new Int3(1, 0, 0);
                    dir2 = new Int3(0, 0, 1);
                    normal = new Vector3(0, 1, 0);
                }
                break;
            case 5:
                {
                    dir1 = new Int3(1, 0, 0);
                    dir2 = new Int3(0, 1, 0);
                    normal = new Vector3(0, 0, -1);
                }
                break;
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

        Bool3Array buffer = new Bool3Array(granulation.length0, granulation.length1, granulation.length2); 
        int arrMax1 = granulation.length0 - 1;
        int arrMax2 = granulation.length1 - 1;
        int arrMax3 = granulation.length2 - 1;

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
            buffer[point.i1, point.i2, point.i3] = true;

            Int3 dir1Negative = point + (-dir1);
            Int3 dir1Positive = point + dir1;
            Int3 dir2Negative = point + (-dir2);
            Int3 dir2Positive = point + dir2;
            
            if (dir1Negative.CheckBound(arrMax1, arrMax2, arrMax3) == true)
            {
                if (granulation.points[dir1Negative.i1, dir1Negative.i2, dir1Negative.i3] == true
                 && bool6s[dir1Negative.i1, dir1Negative.i2, dir1Negative.i3, dir - 1] == false)
                {
                    points.Push(dir1Negative);
                }
                else if (dir1Value > minDir1Limit && buffer[dir1Negative.i1, dir1Negative.i2, dir1Negative.i3] == false)
                {
                    minDir1Limit = dir1Value;
                }
               
            }
            else
            {
                if (dir1Value > minDir1Limit)
                {
                    minDir1Limit = dir1Value;
                }
            }

            if (dir1Positive.CheckBound(arrMax1, arrMax2, arrMax3) == true)
            {
                if (granulation.points[dir1Positive.i1, dir1Positive.i2, dir1Positive.i3] == true
                 && bool6s[dir1Positive.i1, dir1Positive.i2, dir1Positive.i3, dir - 1] == false)
                {
                    points.Push(dir1Positive);
                }
                else if (dir1Value < maxDir1Limit && buffer[dir1Positive.i1, dir1Positive.i2, dir1Positive.i3] == false)
                {
                    maxDir1Limit = dir1Value;
                }

            }
            else
            {
                if (dir1Value < maxDir1Limit)
                {
                    maxDir1Limit = dir1Value;
                }
            }

            if (dir2Negative.CheckBound(arrMax1, arrMax2, arrMax3) == true)
            {
                if (granulation.points[dir2Negative.i1, dir2Negative.i2, dir2Negative.i3] == true
                 && bool6s[dir2Negative.i1, dir2Negative.i2, dir2Negative.i3, dir - 1] == false)
                {
                    points.Push(dir2Negative);
                }
                else if (dir2Value > minDir2Limit && buffer[dir2Negative.i1, dir2Negative.i2, dir2Negative.i3] == false)
                {
                    minDir2Limit = dir2Value;
                }

            }
            else
            {
                if (dir2Value > minDir2Limit)
                {
                    minDir2Limit = dir2Value;
                }
            }

            if (dir2Positive.CheckBound(arrMax1, arrMax2, arrMax3) == true)
            {
                if (granulation.points[dir2Positive.i1, dir2Positive.i2, dir2Positive.i3] == true
                 && bool6s[dir2Positive.i1, dir2Positive.i2, dir2Positive.i3, dir - 1] == false)
                {
                    points.Push(dir2Positive);
                }
                else if (dir2Value < maxDir2Limit && buffer[dir2Positive.i1, dir2Positive.i2, dir2Positive.i3] == false)
                {
                    maxDir2Limit = dir2Value;
                }

            }
            else
            {
                if (dir2Value < maxDir2Limit)
                {
                    maxDir2Limit = dir2Value;
                }
            }
        }

        for (int i = 0; i < granulation.length0; i++)
        {
            for (int j = 0; j < granulation.length1; j++)
            {
                for (int k = 0; k < granulation.length2; k++)
                {
                    if (buffer[i,j,k]==true)
                    {
                        Int3 point = new Int3(i, j, k);
                        int dir1Value = point.GetValue(dir1);
                        int dir2Value = point.GetValue(dir2);

                        if (dir1Value < minDir1Limit || dir1Value > maxDir1Limit
                            || dir2Value < minDir2Limit || dir2Value > maxDir2Limit)
                        {
                            bool6s[point.i1, point.i2, point.i3, dir - 1] = false;
                        }
                    }
                }
            }
        }

        Vector3[] point4 = new Vector3[4];
        float step = Mathf.Pow(10, granulation.level);
        float distance = step * 0.5f;
        Vector3 offset = new Vector3((crtPoint.i1 + 0.5f) * step, (crtPoint.i2 + 0.5f) * step, (crtPoint.i3 + 0.5f) * step);
        Vector3 origin = granulation.origin + offset;
        Vector3 faceCenterPoint = origin + normal * distance;

        Vector3 dir1V3 = new Vector3(dir1.i1, dir1.i2, dir1.i3);
        Vector3 dir2V3 = new Vector3(dir2.i1, dir2.i2, dir2.i3);

        Vector3 dir1MinOffset = (minDir1Limit - crtPoint.GetValue(dir1)) * dir1V3 * step;
        Vector3 dir1MaxOffset = (maxDir1Limit - crtPoint.GetValue(dir1)) * dir1V3 * step;
        Vector3 dir2MinOffset = (minDir2Limit - crtPoint.GetValue(dir2)) * dir2V3 * step;
        Vector3 dir2MaxOffset = (maxDir2Limit - crtPoint.GetValue(dir2)) * dir2V3 * step;

        point4[0] = faceCenterPoint + dir1MinOffset + dir2MinOffset + -dir1V3 * distance + -dir2V3 * distance;
        point4[1] = faceCenterPoint + dir1MaxOffset + dir2MinOffset + dir1V3 * distance + -dir2V3 * distance;
        point4[2] = faceCenterPoint + dir1MaxOffset + dir2MaxOffset + dir1V3 * distance + dir2V3 * distance;
        point4[3] = faceCenterPoint + dir1MinOffset + dir2MaxOffset + -dir1V3 * distance + dir2V3 * distance;

        if (dir == 2 || dir == 3 || dir == 6)
        {
            meshBuffer.AddQuad(point4, normal, Vector2.one * 0.5f);
        }
        else
        {
            meshBuffer.AddQuad(new Vector3[] { point4[2], point4[1], point4[0], point4[3] }, normal, Vector2.one * 0.5f);
        }
    }


    public struct Granulation
    {
        public int level;
        public Bool3Array points;
        public int length0;
        public int length1;
        public int length2;
        /// <summary>
        /// (0,0,0)位置的本地坐标
        /// </summary>
        public Vector3 origin;

        /// <summary>
        /// TODO:可以使用DEXEL模型进行初始化的优化(射线组)
        /// </summary>
        public Granulation(int _level, Mesh mesh)
        {
            level = _level;
            float step = Mathf.Pow(10, level);

            Bounds bounds = mesh.bounds;

            origin = NumHelper.GetNearVector3(bounds.center + bounds.min, level);

            int x = ((bounds.max.x - bounds.min.x) / step).RoundingToInt_Add();
            int y = ((bounds.max.y - bounds.min.y) / step).RoundingToInt_Add();
            int z = ((bounds.max.z - bounds.min.z) / step).RoundingToInt_Add();

            points = new Bool3Array(x, y, z);
            length0 = x;
            length1 = y;
            length2 = z;

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

}
