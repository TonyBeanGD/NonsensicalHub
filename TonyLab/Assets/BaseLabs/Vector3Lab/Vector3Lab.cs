using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector3Lab : MonoBehaviour
{
    [SerializeField]
    private Transform linePoint1;
    [SerializeField]
    private Transform linePoint2;
    [SerializeField]
    private Transform crossPoint;

    private Plane clipPlane;

    void Update()
    {
        clipPlane = new Plane(transform.up, transform.position);
        Vector3? newPos = GetLinePlaneCrossPoint(linePoint1.position, linePoint2.position, clipPlane);
        if (newPos!=null)
        {
            crossPoint.position = (Vector3)newPos;
        }
        else
        {
            Debug.Log("线面平行");
        }
    }

    private Vector3? GetLinePlaneCrossPoint(Vector3 linePoint1, Vector3 linePoint2, Plane plane)
    {
        Vector3 l = linePoint2 - linePoint1;
        Vector3 p0 = -plane.normal * plane.distance;
        Vector3 l0 = linePoint1;
        Vector3 n = plane.normal;

        //直线向量和法线向量垂直时（即直线和面平行）
        if (Vector3.Dot(l, n) == 0)
        {
            //直线与平面重合时
            if (Vector3.Dot(p0 - l0, n) == 0)
            {
                return linePoint1;
            }
            else
            {
                return null;
            }
        }

        float d = Vector3.Dot((p0 - l0), n) / Vector3.Dot(l, n);

        Vector3 t = d * l + l0;

        return t;
    }
}
