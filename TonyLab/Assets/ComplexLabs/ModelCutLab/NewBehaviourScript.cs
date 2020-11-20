using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

}

class CylinderModel
{
    private Vector3 _firstMiddlePointPosition;
    private Vector3 _dir;

    private float _interval;
    private CylinderSection[] _cylinderSection;

    public CylinderModel(Vector3 firstMiddlePointPosition, Vector3 dir, float interval, float radius,float height)
    {
        this._firstMiddlePointPosition = firstMiddlePointPosition;
        this._dir = dir;
        this._interval = interval;

        int pointCount = (int)(height / interval);
        _cylinderSection = new CylinderSection[pointCount];

        foreach (var item in _cylinderSection)
        {
            item.Point.Add(0);
            item.Point.Add(radius);
        }
    }

    public void Cutting(CylinderBurinScan cylinderBurinScan)
    {
        throw new TODOException("Cutting");
    }

    public Mesh ToMesh()
    {
        throw new TODOException("ToMesh");
    }

    class CylinderSection
    {
        public List<float> Point;
    }
}

class TODOException : Exception
{
    private string _message;
    public TODOException(string message)
    {
        _message = message;
    }
    public override string Message
    {
        get
        {
            return "TODO:"+ _message;
        }
    }

}

class CylinderBurin
{
    public Vector3 CenterPoint { get; private set; }
    public Vector3[] Points { get; private set; }

    public CylinderBurin( Vector3 centerPoint, List<Vector3> points)
    {
        CenterPoint = centerPoint;
        Points = points.ToArray();
    }
}

class CylinderBurinScan
{
    public Vector3[] Points1 { get; private set; }
    public Vector3[] Points2 { get; private set; }

    public CylinderBurinScan(CylinderBurin cylinderBurin ,Vector3 dir )
    {
        Points1 = new Vector3[cylinderBurin.Points.Length];
        Points2 = new Vector3[cylinderBurin.Points.Length];

        for (int i = 0; i < cylinderBurin.Points.Length; i++)
        {
            Points1[i] = cylinderBurin.CenterPoint + cylinderBurin.Points[i];
            Points2[i] = cylinderBurin.CenterPoint + cylinderBurin.Points[i]+ dir;
        }
    }
}