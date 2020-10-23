using NonsensicalFrame;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 模型切割脚本，仅能在切面图形非凹时正常使用
/// </summary>
public class RealCut : MonoBehaviour
{
    private Vector3 touchBeganPos;

    private Plane clipPlane;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchBeganPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Vector3 touchBeganPoint_screen = new Vector3(touchBeganPos.x, touchBeganPos.y, Camera.main.nearClipPlane);
            Vector3 touchEndPoint_screen = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
            Vector3 normalEndPoint_screen = new Vector3(touchBeganPos.x, touchBeganPos.y, Camera.main.nearClipPlane + 10);

            if (UpdateClipPlane(touchBeganPoint_screen, touchEndPoint_screen, normalEndPoint_screen))
            {
                ClipMesh();
            }
        }
    }

    /// <summary>
    /// 判断鼠标按下后是否进行了移动，在移动的情况下，更新切割面的法线和点
    /// </summary>
    /// <param name="touchBeganPoint_sceen"></param>
    /// <param name="touchEndPoint_sceen"></param>
    /// <param name="normalEndPoint_screen"></param>
    /// <returns></returns>
    private bool UpdateClipPlane(Vector3 touchBeganPoint_sceen, Vector3 touchEndPoint_sceen, Vector3 normalEndPoint_screen)
    {
        if (Vector3.Distance(touchBeganPoint_sceen, touchEndPoint_sceen) == 0.0f)
        {
            return false;
        }

        //屏幕坐标转世界坐标
        Vector3 touchBeganPoint_world = Camera.main.ScreenToWorldPoint(touchBeganPoint_sceen);
        Vector3 touchEndPoint_world = Camera.main.ScreenToWorldPoint(touchEndPoint_sceen);
        Vector3 noramlEndPoint_world = Camera.main.ScreenToWorldPoint(normalEndPoint_screen);

        //世界坐标转本地坐标
        Vector3 touchBeginPoint_local = transform.worldToLocalMatrix.MultiplyPoint(touchBeganPoint_world);
        Vector3 touchEndPoint_local = transform.worldToLocalMatrix.MultiplyPoint(touchEndPoint_world);
        Vector3 normalEngPoint_local = transform.worldToLocalMatrix.MultiplyPoint(noramlEndPoint_world);

        clipPlane = new Plane(Vector3.Cross(normalEngPoint_local - touchBeginPoint_local, touchEndPoint_local - touchBeginPoint_local).normalized, touchEndPoint_local);

        return true;
    }

    private void ClipMesh()
    {
        MeshFilter mf = this.gameObject.GetComponent<MeshFilter>();
        int rawVerticeCount = mf.mesh.vertices.Length;
        List<Vector3> verticeList = new List<Vector3>(mf.mesh.vertices);
        List<int> triangleList = new List<int>(mf.mesh.triangles);
        List<Vector2> uvList = new List<Vector2>(mf.mesh.uv);
        List<Vector3> normalList = new List<Vector3>(mf.mesh.normals);

        bool sideFlag = false;
        for (int i = 0; i < triangleList.Count; i += 3)
        {

            if (clipPlane.GetSide(verticeList[triangleList[i]]) ^ clipPlane.GetSide(verticeList[triangleList[i + 1]]) == true)
            {
                sideFlag = true;
                break;
            }
            else if (clipPlane.GetSide(verticeList[triangleList[i]]) ^ clipPlane.GetSide(verticeList[triangleList[i + 2]]) == true)
            {
                sideFlag = true;
                break;
            }
            else if (clipPlane.GetSide(verticeList[triangleList[i + 1]]) ^ clipPlane.GetSide(verticeList[triangleList[i + 2]]) == true)
            {
                sideFlag = true;
                break;
            }
        }
        if (sideFlag == false)
        {
            //模型的所有点在切割面的一侧,不执行后续步骤
            return;
        }
        int newPointCount = 0;

        //切割所有三角形
        for (int triangleIndex = 0; triangleIndex < triangleList.Count;)
        {
            int trianglePointIndex0 = triangleList[triangleIndex];
            int trianglePointIndex1 = triangleList[triangleIndex + 1];
            int trianglePointIndex2 = triangleList[triangleIndex + 2];

            Vector3 trianglePointCoord0 = verticeList[trianglePointIndex0];
            Vector3 trianglePointCoord1 = verticeList[trianglePointIndex1];
            Vector3 trianglePointCoord2 = verticeList[trianglePointIndex2];

            Vector2 uv0 = uvList[trianglePointIndex0];
            Vector2 uv1 = uvList[trianglePointIndex1];
            Vector2 uv2 = uvList[trianglePointIndex2];

            Vector3 normalX0 = normalList[trianglePointIndex0];
            Vector3 normalX1 = normalList[trianglePointIndex1];
            Vector3 normalX2 = normalList[trianglePointIndex2];

            //0-1，1-2相连线段被切割
            if (clipPlane.GetSide(trianglePointCoord0) ^ clipPlane.GetSide(trianglePointCoord1) == true &&
                clipPlane.GetSide(trianglePointCoord1) ^ clipPlane.GetSide(trianglePointCoord2) == true)
            {
                newPointCount++;
                //求得0-1与切平面的交点
                Vector3 newVertice01 = (Vector3)VectorHelper.GetLinePlaneCrossPoint(trianglePointCoord0, trianglePointCoord1, clipPlane);
                verticeList.Add(newVertice01);
                int index01 = verticeList.Count - 1;
                float k01 = (newVertice01 - trianglePointCoord0).magnitude / (trianglePointCoord1 - trianglePointCoord0).magnitude;
                uvList.Add(Vector2.Lerp(uv0, uv1, k01));
                normalList.Add(Vector3.Lerp(normalX0, normalX1, k01));

                //求得1-2与切平面的交点
                Vector3 newVertice12 = (Vector3)VectorHelper.GetLinePlaneCrossPoint(trianglePointCoord1, trianglePointCoord2, clipPlane);
                verticeList.Add(newVertice12);
                int index12 = verticeList.Count - 1;
                float k12 = (newVertice12 - trianglePointCoord1).magnitude / (trianglePointCoord2 - trianglePointCoord1).magnitude;
                uvList.Add(Vector2.Lerp(uv1, uv2, k12));
                normalList.Add(Vector3.Lerp(normalX1, normalX2, k12));

                //删除原来的顶点索引，插入新的顶点索引，以此构建三个新三角形
                triangleList.RemoveRange(triangleIndex, 3);

                triangleList.Insert(triangleIndex + 0, trianglePointIndex0);
                triangleList.Insert(triangleIndex + 1, index01);
                triangleList.Insert(triangleIndex + 2, index12);

                triangleList.Insert(triangleIndex + 3, index12);
                triangleList.Insert(triangleIndex + 4, index01);
                triangleList.Insert(triangleIndex + 5, trianglePointIndex1);

                triangleList.Insert(triangleIndex + 6, trianglePointIndex0);
                triangleList.Insert(triangleIndex + 7, index12);
                triangleList.Insert(triangleIndex + 8, trianglePointIndex2);

                triangleIndex += 9;
            }
            //1-2，2-0相连线段被切割
            else if (clipPlane.GetSide(trianglePointCoord1) ^ clipPlane.GetSide(trianglePointCoord2) == true &&
                clipPlane.GetSide(trianglePointCoord2) ^ clipPlane.GetSide(trianglePointCoord0) == true)
            {
                newPointCount++;
                //求得1-2与切平面的交点
                Vector3 newVertice12 = (Vector3)VectorHelper.GetLinePlaneCrossPoint(trianglePointCoord1, trianglePointCoord2, clipPlane);
                verticeList.Add(newVertice12);
                int index12 = verticeList.Count - 1;
                float k12 = (newVertice12 - trianglePointCoord1).magnitude / (trianglePointCoord2 - trianglePointCoord1).magnitude;
                uvList.Add(Vector2.Lerp(uv1, uv2, k12));
                normalList.Add(Vector3.Lerp(normalX1, normalX2, k12));

                //求得0-2与切平面的交点
                Vector3 newVertice02 = (Vector3)VectorHelper.GetLinePlaneCrossPoint(trianglePointCoord0, trianglePointCoord2, clipPlane);
                verticeList.Add(newVertice02);
                int index02 = verticeList.Count - 1;
                float k02 = (newVertice02 - trianglePointCoord0).magnitude / (trianglePointCoord2 - trianglePointCoord0).magnitude;
                uvList.Add(Vector2.Lerp(uv0, uv2, k02));
                normalList.Add(Vector3.Lerp(normalX0, normalX2, k02));

                //删除原来的顶点索引，插入新的顶点索引，以此构建三个新三角形
                triangleList.RemoveRange(triangleIndex, 3);
                triangleList.Insert(triangleIndex + 0, trianglePointIndex0);
                triangleList.Insert(triangleIndex + 1, trianglePointIndex1);
                triangleList.Insert(triangleIndex + 2, index12);

                triangleList.Insert(triangleIndex + 3, index02);
                triangleList.Insert(triangleIndex + 4, index12);
                triangleList.Insert(triangleIndex + 5, trianglePointIndex2);

                triangleList.Insert(triangleIndex + 6, index02);
                triangleList.Insert(triangleIndex + 7, trianglePointIndex0);
                triangleList.Insert(triangleIndex + 8, index12);
                triangleIndex += 9;
            }
            //0-1，2-0相连线段被切割
            else if (clipPlane.GetSide(trianglePointCoord0) ^ clipPlane.GetSide(trianglePointCoord1) == true &&
                clipPlane.GetSide(trianglePointCoord2) ^ clipPlane.GetSide(trianglePointCoord0) == true)
            {
                newPointCount++;
                //求得0-1与切平面的交点
                Vector3 newVertice01 = (Vector3)VectorHelper.GetLinePlaneCrossPoint(trianglePointCoord0, trianglePointCoord1, clipPlane);
                verticeList.Add(newVertice01);
                int index01 = verticeList.Count - 1;
                float k01 = (newVertice01 - trianglePointCoord0).magnitude / (trianglePointCoord1 - trianglePointCoord0).magnitude;
                uvList.Add(Vector2.Lerp(uv0, uv1, k01));
                normalList.Add(Vector3.Lerp(normalX0, normalX1, k01));

                //求得0-2与切平面的交点
                Vector3 newVertice02 = (Vector3)VectorHelper.GetLinePlaneCrossPoint(trianglePointCoord0, trianglePointCoord2, clipPlane);
                verticeList.Add(newVertice02);
                int index02 = verticeList.Count - 1;
                float k02 = (newVertice02 - trianglePointCoord0).magnitude / (trianglePointCoord2 - trianglePointCoord0).magnitude;
                uvList.Add(Vector2.Lerp(uv0, uv2, k02));
                normalList.Add(Vector3.Lerp(normalX0, normalX2, k02));

                //删除原来的顶点索引，插入新的顶点索引，以此构建三个新三角形
                triangleList.RemoveRange(triangleIndex, 3);
                triangleList.Insert(triangleIndex + 0, trianglePointIndex0);
                triangleList.Insert(triangleIndex + 1, index01);
                triangleList.Insert(triangleIndex + 2, index02);

                triangleList.Insert(triangleIndex + 3, index01);
                triangleList.Insert(triangleIndex + 4, trianglePointIndex1);
                triangleList.Insert(triangleIndex + 5, trianglePointIndex2);

                triangleList.Insert(triangleIndex + 6, trianglePointIndex2);
                triangleList.Insert(triangleIndex + 7, index02);
                triangleList.Insert(triangleIndex + 8, index01);
                triangleIndex += 9;
            }
            else
            {
                triangleIndex += 3;
            }
        }


        //筛选出切割面两侧的三角形索引
        List<int> triangles1 = new List<int>();
        List<int> triangles2 = new List<int>();
        for (int triangleIndex = 0; triangleIndex < triangleList.Count; triangleIndex += 3)
        {
            int trianglePoint0 = triangleList[triangleIndex];
            int trianglePoint1 = triangleList[triangleIndex + 1];
            int trianglePoint2 = triangleList[triangleIndex + 2];

            Vector3 point0 = verticeList[trianglePoint0];
            Vector3 point1 = verticeList[trianglePoint1];
            Vector3 point2 = verticeList[trianglePoint2];

            float dis0 = clipPlane.GetDistanceToPoint(point0);
            float dis1 = clipPlane.GetDistanceToPoint(point1);
            float dis2 = clipPlane.GetDistanceToPoint(point2);

            if ((dis0 < 0 || IsNear(dis0, 0)) && (dis1 < 0 || IsNear(dis1, 0)) && (dis2 < 0 || IsNear(dis2, 0)))
            {
                triangles1.Add(trianglePoint0);
                triangles1.Add(trianglePoint1);
                triangles1.Add(trianglePoint2);
            }
            else
            {
                triangles2.Add(trianglePoint0);
                triangles2.Add(trianglePoint1);
                triangles2.Add(trianglePoint2);
            }
        }

        List<Vector3> normalList2 = new List<Vector3>(normalList.ToArray());

        int newVerticeCount = verticeList.Count - rawVerticeCount;

        #region 新增uv和法线不同的定点

        for (int newVerticeIndex = 0; newVerticeIndex < newVerticeCount; ++newVerticeIndex)
        {
            Vector3 newVertice = verticeList[rawVerticeCount + newVerticeIndex];
            verticeList.Add(newVertice);

            Vector2 newUv = new Vector2(0.99f, 0.99f);
            uvList.Add(newUv);

            normalList.Add(clipPlane.normal);
            normalList2.Add(-clipPlane.normal);
        }

        rawVerticeCount = rawVerticeCount + newVerticeCount;
        #endregion

        #region 重新排序面上的顶点
        List<SortAngle> SortAngleList = new List<SortAngle>();
        
        for (int verticeIndex = rawVerticeCount + 1; verticeIndex < verticeList.Count; verticeIndex++)
        {
            //计算角度,以0-1为参照，01一定是相邻的两个点（从同一个三角形中切出的点）
            Vector3 line0to1 = verticeList[rawVerticeCount + 1] - verticeList[rawVerticeCount];
            Vector3 line0toi = verticeList[verticeIndex] - verticeList[rawVerticeCount];

            if (IsNear(line0toi.magnitude, 0))
            {
                continue;
            }
            float angle = Vector3.Angle(line0to1, line0toi);

            bool isExistSameAngel = false;
            for (int i = 0; i < SortAngleList.Count; ++i)
            {
                //同样角度，距离近的被剔除
                if (IsNear(Mathf.Abs(SortAngleList[i].Angle - angle), 0) || IsNear(verticeList[SortAngleList[i].Index], verticeList[verticeIndex]))
                {
                    float dis1 = Vector3.Distance(verticeList[SortAngleList[i].Index], verticeList[rawVerticeCount]);
                    float dis2 = Vector3.Distance(verticeList[verticeIndex], verticeList[rawVerticeCount]);
                    if (dis2 >= dis1)
                    {
                        SortAngleList[i].Index = verticeIndex;
                    }
                    isExistSameAngel = true;
                    break;
                }
            }
            if (!isExistSameAngel)
            {
                SortAngle sortAngle = new SortAngle
                {
                    Index = verticeIndex,
                    Angle = angle
                };
                SortAngleList.Add(sortAngle);
            }
        }
        SortAngleList.Sort();
        #endregion

        #region 缝合

        Vector3 line1 = verticeList[rawVerticeCount + 1] - verticeList[rawVerticeCount];
        Vector3 line2 = verticeList[rawVerticeCount + 2] - verticeList[rawVerticeCount];
        Vector3 line3 = verticeList[rawVerticeCount + 3] - verticeList[rawVerticeCount];

        float angle12 = Vector3.Angle(line1, line2);
        float angle13 = Vector3.Angle(line1, line3);

        bool type1 = angle12 < angle13;


        for (int verticeIndex = 0; verticeIndex < SortAngleList.Count ; verticeIndex++)
        {

            if (verticeIndex == 0&& !type1)
            {
                continue;
            }
            if (verticeIndex== SortAngleList.Count-1&& type1)
            {
                continue;
            }
            int next = verticeIndex + 1;
            if (next>= SortAngleList.Count)
            {
                next = 0;
            }
            if (Vector3.SignedAngle(verticeList[SortAngleList[next].Index] - verticeList[rawVerticeCount], verticeList[SortAngleList[verticeIndex].Index] - verticeList[rawVerticeCount], clipPlane.normal) > 0)
            {
                triangles1.Add(SortAngleList[next].Index);
                triangles1.Add(SortAngleList[verticeIndex].Index);
                triangles1.Add(rawVerticeCount);

                triangles2.Add(rawVerticeCount);
                triangles2.Add(SortAngleList[verticeIndex].Index);
                triangles2.Add(SortAngleList[next].Index);
            }
            else
            {

                triangles1.Add(SortAngleList[verticeIndex].Index);
                triangles1.Add(SortAngleList[next].Index);
                triangles1.Add(rawVerticeCount);

                triangles2.Add(rawVerticeCount);
                triangles2.Add(SortAngleList[next].Index);
                triangles2.Add(SortAngleList[verticeIndex].Index);
            }
        }
        #endregion

        mf.mesh.vertices = verticeList.ToArray();
        mf.mesh.uv = uvList.ToArray();
        mf.mesh.normals = normalList.ToArray();
        mf.mesh.triangles = triangles1.ToArray();
        
        GameObject newModel = new GameObject("New Model");
        MeshFilter meshFilter = newModel.AddComponent<MeshFilter>();
        meshFilter.mesh.vertices = mf.mesh.vertices;
        meshFilter.mesh.uv = mf.mesh.uv;
        meshFilter.mesh.normals = normalList2.ToArray();
        meshFilter.mesh.triangles = triangles2.ToArray();
        Renderer newRenderer = newModel.AddComponent<MeshRenderer>();
        newRenderer.material = this.gameObject.GetComponent<MeshRenderer>().material;
        newModel.transform.localPosition = transform.localPosition;
        newModel.AddComponent<RealCut>();

        MeshHelper.ClearUnuseVertex(mf.mesh);
        mf.transform.position+= MeshHelper.AutoCentroidShift(mf.mesh);
        mf.GetComponent<MeshCollider>().sharedMesh = mf.mesh;
        MeshHelper.ClearUnuseVertex(meshFilter.mesh);
        newModel.transform.position += MeshHelper.AutoCentroidShift(meshFilter.mesh);
        newModel.AddComponent<MeshCollider>();
        newModel.GetComponent<MeshCollider>().convex=true;


        newModel.AddComponent<Rigidbody>();
        newModel.GetComponent<Rigidbody>().drag = 1 ;
    }

    /// <summary>
    /// 判断两个float是否非常接近
    /// </summary>
    /// <param name="num1"></param>
    /// <param name="num2"></param>
    /// <returns></returns>
    private bool IsNear(float num1, float num2)
    {
        float absX = Mathf.Abs(num1 - num2);
        return absX < 0.00001f;
    }

    private bool IsNear(Vector3 vec1, Vector3 vec2)
    {
        if (IsNear(Vector3.Distance(vec1, vec2), 0))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public class SortAngle : IComparable<SortAngle>
    {
        public int Index;
        public float Angle;
        public int CompareTo(SortAngle item)
        {
            return item.Angle.CompareTo(Angle);
        }
    }
}
