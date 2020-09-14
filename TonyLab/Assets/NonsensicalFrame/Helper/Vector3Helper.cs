using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NonsensicalFrame
{
    public class Vector3Helper
    {
        /// <summary>
        /// 获取点在直线上的垂足
        /// </summary>
        /// <param name="p0">直线点1</param>
        /// <param name="p1">直线点2</param>
        /// <param name="p2">直线外一点</param>
        /// <returns>p2在直线p0p1上的垂足坐标</returns>
        public static Vector3 GetFootDrop(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float numerator = (p1.x - p0.x) * (p2.x - p1.x) + (p1.y - p0.y) * (p2.y - p1.y) + (p1.z - p0.z) * (p2.z - p1.z);
            float denominator = Mathf.Pow((p2.x - p1.x), 2) + Mathf.Pow(p2.y - p1.y, 2) + Mathf.Pow(p2.z - p1.z, 2);
            float k = -numerator / denominator;
            Vector3 result = new Vector3(k * (p2.x - p1.x) + p1.x, k * (p2.y - p1.y) + p1.y, k * (p2.z - p1.z) + p1.z);

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
    }
}
