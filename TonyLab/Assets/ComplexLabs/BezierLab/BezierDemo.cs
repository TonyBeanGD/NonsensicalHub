using NonsensicalKit.Utility;
using System;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BezierDemo : MonoBehaviour
{
    // 三次贝塞尔控制点
    public Transform[] controlPoints;

    // LineRenderer 
    private LineRenderer lineRenderer;
    private int layerOrder = 0;

    // 设置贝塞尔插值个数
    private int _segmentNum = 50;


    void Start()
    {
        if (!lineRenderer)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        lineRenderer.sortingLayerID = layerOrder;

      
    }

    void Update()
    {
        Tuple<Point, Point> p2 = GetPoint2(new Point(controlPoints[0].position.x, controlPoints[0].position.y), new Point(controlPoints[3].position.x, controlPoints[3].position.y), 0.5f, true);
        controlPoints[1].position = new Vector3((float)p2.Item1.X,  (float)p2.Item1.Y,0);
        controlPoints[2].position = new Vector3((float)p2.Item2.X,  (float)p2.Item2.Y,0);

        DrawThreePowerCurve();

    }


    Vector3[] points3;
    void DrawThreePowerCurve()
    {
        // 获取三次贝塞尔方程曲线
        points3 = BezierUtils.GetThreePowerBeizerList(controlPoints[0].position, controlPoints[1].position, controlPoints[2].position, controlPoints[3].position, _segmentNum);
        // 设置 LineRenderer 的点个数，并赋值点值
        lineRenderer.positionCount = (_segmentNum);
        lineRenderer.SetPositions(points3);

    }

    public Tuple<Point, Point> GetPoint2(Point start, Point end, double offset, bool negative)
    {
        Point control1 = new Point(start.X+(end.X - start.X) /3, start.Y + (end.Y - start.Y) /3);
        Point control2 = new Point(start.X + (end.X - start.X) /3*2, start.Y + (end.Y - start.Y) /3*2);

        Point dir = new Point(end.X - start.X, end.Y - start.Y);
        Point offset1 = new Point(-dir.Y* offset, dir.X* offset);
        Point offset2 = new Point(dir.Y* offset, -dir.X* offset);

        Tuple<Point, Point> point2 = null;
        if (!negative)
        {

            point2 = new Tuple<Point, Point>(new Point(control1.X + offset1.X, control1.Y + offset1.Y), new Point(control2.X + offset2.X, control2.Y + offset2.Y));
        }
        {
            point2 = new Tuple<Point, Point>(new Point(control1.X + offset2.X, control1.Y + offset2.Y), new Point(control2.X + offset1.X, control2.Y + offset1.Y));
        }

        return point2;
    }
    public struct Point
    {
        public double X;
        public double Y;
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return X.ToString() + "_" + Y.ToString();
        }
    }
}
 
public class BezierUtils
{

    /// <summary>
    /// 线性贝塞尔曲线，根据T值，计算贝塞尔曲线上面相对应的点
    /// </summary>
    /// <param name="t"></param>T值
    /// <param name="p0"></param>起始点
    /// <param name="p1"></param>控制点
    /// <returns></returns>根据T值计算出来的贝赛尔曲线点
    private static Vector3 CalculateLineBezierPoint(float t, Vector3 p0, Vector3 p1)
    {
        float u = 1 - t;

        Vector3 p = u * p0;
        p += t * p1;


        return p;
    }

    /// <summary>
    /// 二次贝塞尔曲线，根据T值，计算贝塞尔曲线上面相对应的点
    /// </summary>
    /// <param name="t"></param>T值
    /// <param name="p0"></param>起始点
    /// <param name="p1"></param>控制点
    /// <param name="p2"></param>目标点
    /// <returns></returns>根据T值计算出来的贝赛尔曲线点
    private static Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;

        return p;
    }

    /// <summary>
    /// 三次贝塞尔曲线，根据T值，计算贝塞尔曲线上面相对应的点
    /// </summary>
    /// <param name="t">插量值</param>
    /// <param name="p0">起点</param>
    /// <param name="p1">控制点1</param>
    /// <param name="p2">控制点2</param>
    /// <param name="p3">尾点</param>
    /// <returns></returns>
    private static Vector3 CalculateThreePowerBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float ttt = tt * t;
        float uuu = uu * u;

        Vector3 p = uuu * p0;
        p += 3 * t * uu * p1;
        p += 3 * tt * u * p2;
        p += ttt * p3;

        return p;
    }


    /// <summary>
    /// 获取存储贝塞尔曲线点的数组
    /// </summary>
    /// <param name="startPoint"></param>起始点
    /// <param name="controlPoint"></param>控制点
    /// <param name="endPoint"></param>目标点
    /// <param name="segmentNum"></param>采样点的数量
    /// <returns></returns>存储贝塞尔曲线点的数组
    public static Vector3[] GetLineBeizerList(Vector3 startPoint, Vector3 endPoint, int segmentNum)
    {
        Vector3[] path = new Vector3[segmentNum];
        for (int i = 1; i <= segmentNum; i++)
        {
            float t = i / (float)segmentNum;
            Vector3 pixel = CalculateLineBezierPoint(t, startPoint, endPoint);
            path[i - 1] = pixel;
            Debug.Log(path[i - 1]);
        }
        return path;

    }

    /// <summary>
    /// 获取存储的二次贝塞尔曲线点的数组
    /// </summary>
    /// <param name="startPoint"></param>起始点
    /// <param name="controlPoint"></param>控制点
    /// <param name="endPoint"></param>目标点
    /// <param name="segmentNum"></param>采样点的数量
    /// <returns></returns>存储贝塞尔曲线点的数组
    public static Vector3[] GetCubicBeizerList(Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint, int segmentNum)
    {
        Vector3[] path = new Vector3[segmentNum];
        for (int i = 1; i <= segmentNum; i++)
        {
            float t = i / (float)segmentNum;
            Vector3 pixel = CalculateCubicBezierPoint(t, startPoint,
                controlPoint, endPoint);
            path[i - 1] = pixel;
            Debug.Log(path[i - 1]);
        }
        return path;

    }

    /// <summary>
    /// 获取存储的三次贝塞尔曲线点的数组
    /// </summary>
    /// <param name="startPoint"></param>起始点
    /// <param name="controlPoint1"></param>控制点1
    /// <param name="controlPoint2"></param>控制点2
    /// <param name="endPoint"></param>目标点
    /// <param name="segmentNum"></param>采样点的数量
    /// <returns></returns>存储贝塞尔曲线点的数组
    public static Vector3[] GetThreePowerBeizerList(Vector3 startPoint, Vector3 controlPoint1, Vector3 controlPoint2, Vector3 endPoint, int segmentNum)
    {
        Vector3[] path = new Vector3[segmentNum];
        for (int i = 1; i <= segmentNum; i++)
        {
            float t = i / (float)segmentNum;
            Vector3 pixel = CalculateThreePowerBezierPoint(t, startPoint,
                controlPoint1, controlPoint2, endPoint);
            path[i - 1] = pixel;
            //Debug.Log(path[i - 1]);
        }
        return path;

    }
}