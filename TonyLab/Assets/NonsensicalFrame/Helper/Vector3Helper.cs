using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NonsensicalFrame
{
    public static class Vector3Helper
    {
        /// <summary>
        /// 获取点在直线上的垂足
        /// </summary>
        /// <param name="singlePoint">直线点1</param>
        /// <param name="linePoint1">直线点2</param>
        /// <param name="linePoint2">直线外一点</param>
        /// <returns>p2在直线p0p1上的垂足坐标</returns>
        public static Vector3 GetFootDrop(Vector3 singlePoint, Vector3 linePoint1, Vector3 linePoint2)
        {
            float numerator = (linePoint1.x - singlePoint.x) * (linePoint2.x - linePoint1.x) 
                + (linePoint1.y - singlePoint.y) * (linePoint2.y - linePoint1.y) 
                + (linePoint1.z - singlePoint.z) * (linePoint2.z - linePoint1.z);
            float denominator = Mathf.Pow((linePoint2.x - linePoint1.x), 2) + Mathf.Pow(linePoint2.y - linePoint1.y, 2) + Mathf.Pow(linePoint2.z - linePoint1.z, 2);
            float k = -numerator / denominator;
            Vector3 result = new Vector3(k * (linePoint2.x - linePoint1.x) + linePoint1.x, k * (linePoint2.y - linePoint1.y) + linePoint1.y, k * (linePoint2.z - linePoint1.z) + linePoint1.z);

            return result;
        }

        /// <summary>
        /// 获取摄像机看向地平线的视点
        /// </summary>
        /// <param name="cameraPos">摄像机位置</param>
        /// <param name="cameraForwardPos">摄像机前方位置</param>
        /// <param name="horizontal">地平线高度</param>
        /// <returns>没有看向地平线时返回null,否则返回视点的位置</returns>
        private static Vector3? GetViewPoint(Vector3 cameraPos, Vector3 cameraForwardPos,float horizontal)
        {
            if ((cameraPos.y - horizontal) * (cameraForwardPos.y - horizontal) > 0//当摄像机的点和摄像机的前方点没有在地平线两侧时
                       && Mathf.Abs(cameraPos.y) - Mathf.Abs(cameraForwardPos.y) < 0)//且当摄像机前方的位置比摄像机的位置更加远离地平线时的位置
            {
                //此时代表没有看向地面
                return null;
            }
            else
            {
                float h1 = Mathf.Abs(cameraPos.y - horizontal);
                float h2 = Mathf.Abs(cameraForwardPos.y - horizontal);
                float l1 = Vector3.Distance(cameraPos, cameraForwardPos);
                float l2 = h1 * l1 / (h1 - h2);
                return cameraForwardPos + (cameraForwardPos - cameraPos).normalized * l2;
            }
        }

        /// <summary>
        /// 将物体移动至UGUI中对应的位置
        /// </summary>
        /// <param name="changeTarget">需要更改位置的对象</param>
        /// <param name="targetCamera">渲染对象的相机</param>
        /// <param name="posTarget">UGUI中需放置位置的UI对象</param>
        /// <param name="renderCanvas">渲染ui的相机</param>
        /// <param name="zOffset">对象移动目标位置的深度</param>
        private static void MovePosByUGUI(Transform changeTarget, Camera targetCamera,RectTransform posTarget, RectTransform renderCanvas,float zOffset=10)
        {
            float x = (posTarget.localPosition.x + renderCanvas.rect.width * 0.5f) / renderCanvas.rect.width * Screen.width;
            float y = (posTarget.localPosition.y + renderCanvas.rect.height * 0.5f) / renderCanvas.rect.height * Screen.height;

            Vector3 screenPoint = new Vector3(x, y, zOffset);

            Vector3 worldPoint = targetCamera.ScreenToWorldPoint(screenPoint);

            changeTarget.position = worldPoint;
        }

        /// <summary>
        /// 求两个向量的公垂线，当两个向量平行时，随机返回一个与这两个向量垂直的向量
        /// </summary>
        /// <param name="vector3"></param>
        /// <param name="dir1"></param>
        /// <param name="dir2"></param>
        /// <returns></returns>
        private static Vector3 GetCommonVerticalLine(Vector3 dir1, Vector3 dir2)
        {
            Vector3 normal = Vector3.zero;
            normal = Vector3.Cross(dir1, dir2);


            //当两个向量平行时，Vector3.Cross求出来的公垂线为Vector3.Zero
            if (normal == Vector3.zero)
            {
                //随意一个向量求公垂线
                normal = Vector3.Cross(dir1, Vector3.up);

                //当仍然与随意取的向量平行时
                if (normal == Vector3.zero)
                {
                    //拿一个与之前向量不平行的向量求公垂线
                    normal = Vector3.Cross(dir1, Vector3.forward);
                }
            }

            return normal;
        }

        /// <summary>
        /// 根据子物体的旋转差值旋转
        /// </summary>
        /// <param name="rotateObject">需要旋转的对象</param>
        /// <param name="crtElementPoints">子物体面上不在一条直线上的三个点的数组</param>
        /// <param name="targetElementPoints">目标物体面上不在一条直线上的三个点的数组</param>
        private static void RotateByChildren(this Transform rotateObject,Vector3[] crtElementPoints, Vector3[] targetElementPoints)
        {
            Vector3 dir1 = crtElementPoints[0] - crtElementPoints[1];
            Vector3 dir2 = crtElementPoints[1] - crtElementPoints[2];
            Vector3 normal1 = GetCommonVerticalLine(dir1, dir2);

            Vector3 dir3 = targetElementPoints[0] - targetElementPoints[1];
            Vector3 dir4 = targetElementPoints[1] - targetElementPoints[2];
            Vector3 normal2 = GetCommonVerticalLine(dir3, dir4);

            float angle = Vector3.Angle(normal1, normal2);
            Vector3 axis = GetCommonVerticalLine(normal1, normal2);

            rotateObject.rotation = Quaternion.AngleAxis(angle, axis) * rotateObject.transform.rotation;
        }
    }
}
