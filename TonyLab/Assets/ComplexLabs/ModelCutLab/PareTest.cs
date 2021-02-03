using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NonsensicalKit;
using NonsensicalKit.Utility;

public class PareTest : MonoBehaviour
{
    public GameObject cutModel;
    public Mesh cutMesh;
    public Mesh selfMesh;

    void Start()
    {
        cutMesh = cutModel.GetComponent<MeshFilter>().mesh;
        selfMesh = GetComponent<MeshFilter>().mesh;
    }

    void Update()
    {
        Vector3 selfCenter = transform.position;
        Vector3 targetCenter = cutModel.transform.position;

        MeshBuffer selfMeshBuffer = new MeshBuffer(selfMesh);
        MeshBuffer cutMeshBuffer = new MeshBuffer(cutMesh);
        for (int i = 0; i < selfMeshBuffer.triangles.Count; i += 3)
        {

            List<int> contactTriangles = new List<int>();
            List<Vector3> intersections = new List<Vector3>();

            for (int j = 0; j < cutMeshBuffer.triangles.Count; j+=3)
            {
                Vector3? point01 = VectorHelper.GetRayTriangleCrossPoint(cutMeshBuffer.GetVerticeByTrianglesIndex(j + 0), cutMeshBuffer.GetVerticeByTrianglesIndex(j + 1), selfMeshBuffer.GetVerticeByTrianglesIndex(i + 0), selfMeshBuffer.GetVerticeByTrianglesIndex(i + 1), selfMeshBuffer.GetVerticeByTrianglesIndex(i + 2));

                Vector3? point02 = VectorHelper.GetRayTriangleCrossPoint(cutMeshBuffer.GetVerticeByTrianglesIndex(j + 0), cutMeshBuffer.GetVerticeByTrianglesIndex(j + 2), selfMeshBuffer.GetVerticeByTrianglesIndex(i + 0), selfMeshBuffer.GetVerticeByTrianglesIndex(i + 1), selfMeshBuffer.GetVerticeByTrianglesIndex(i + 2));

                Vector3? point12 = VectorHelper.GetRayTriangleCrossPoint(cutMeshBuffer.GetVerticeByTrianglesIndex(j + 1), cutMeshBuffer.GetVerticeByTrianglesIndex(j + 2), selfMeshBuffer.GetVerticeByTrianglesIndex(i + 0), selfMeshBuffer.GetVerticeByTrianglesIndex(i + 1), selfMeshBuffer.GetVerticeByTrianglesIndex(i + 2));

                if (point01!=null&& point02 !=null&& point12!=null)
                {
                    contactTriangles.Add(j);
                    if (point01!=null)
                    {
                        intersections.Add((Vector3)point01);
                    }
                    if (point02 != null)
                    {
                        intersections.Add((Vector3)point02);
                    }
                    if (point12 != null)
                    {
                        intersections.Add((Vector3)point12);
                    }
                }
            }

            if (contactTriangles.Count == 0)
            {
                //没有交集的情况下有一个点在mesh里就代表三个点都在mesh里面
                if (cutMesh.Contain(targetCenter.LocalPosTransform(selfCenter, selfMeshBuffer.GetVerticeByTrianglesIndex(i + 0))))
                {
                    selfMeshBuffer.triangles.RemoveRange(i, 3);
                }

                continue;
            }

            foreach (var item in contactTriangles)
            {
                Vector3? point01 = VectorHelper.GetRayTriangleCrossPoint(selfMeshBuffer.GetVerticeByTrianglesIndex(item + 0), selfMeshBuffer.GetVerticeByTrianglesIndex(item + 1), cutMeshBuffer.GetVerticeByTrianglesIndex(item + 0), cutMeshBuffer.GetVerticeByTrianglesIndex(item + 1), cutMeshBuffer.GetVerticeByTrianglesIndex(item + 2));

                Vector3? point02 = VectorHelper.GetRayTriangleCrossPoint(selfMeshBuffer.GetVerticeByTrianglesIndex(item + 0), selfMeshBuffer.GetVerticeByTrianglesIndex(item + 2), cutMeshBuffer.GetVerticeByTrianglesIndex(item + 0), cutMeshBuffer.GetVerticeByTrianglesIndex(item + 1), cutMeshBuffer.GetVerticeByTrianglesIndex(item + 2));

                Vector3? point12 = VectorHelper.GetRayTriangleCrossPoint(selfMeshBuffer.GetVerticeByTrianglesIndex(item + 1), selfMeshBuffer.GetVerticeByTrianglesIndex(item + 2), cutMeshBuffer.GetVerticeByTrianglesIndex(item + 0), cutMeshBuffer.GetVerticeByTrianglesIndex(item + 1), cutMeshBuffer.GetVerticeByTrianglesIndex(item + 2));

            }
        }
    }
}
