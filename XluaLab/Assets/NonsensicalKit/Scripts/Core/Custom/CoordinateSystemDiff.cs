
using NonsensicalKit.Utility;
using UnityEngine;

namespace NonsensicalKit.Custom
{
    /// <summary>
    /// 坐标系差异运算，保存差异量，大量运算时效率更高
    /// </summary>
    public struct CoordinateSystemDiff
    {
        Quaternion quatDiff;
        PointF3 offset;

        public CoordinateSystemDiff(CoordinateSystem cs1, CoordinateSystem cs2, float _unitSize = 1)
        {
            quatDiff = Quat.Diff(cs2.rotation, cs1.rotation);

            Vector3 xPoint = VectorHelper.GetFootDrop(cs1.origin, cs2.origin, cs2.origin + cs2.right);
            Vector3 yPoint = VectorHelper.GetFootDrop(cs1.origin, cs2.origin, cs2.origin + cs2.up);
            Vector3 zPoint = VectorHelper.GetFootDrop(cs1.origin, cs2.origin, cs2.origin + cs2.forward);

            float x_distance = Vector3.Distance(cs2.origin, xPoint) / _unitSize;
            float y_distance = Vector3.Distance(cs2.origin, yPoint) / _unitSize;
            float z_distance = Vector3.Distance(cs2.origin, zPoint) / _unitSize;

            int x_dir = Vector3.Dot(xPoint - cs2.origin, cs2.right) > 0 ? 1 : -1;
            int y_dir = Vector3.Dot(yPoint - cs2.origin, cs2.up) > 0 ? 1 : -1;
            int z_dir = Vector3.Dot(zPoint - cs2.origin, cs2.forward) > 0 ? 1 : -1;

            offset = new PointF3(x_dir * x_distance, y_dir * y_distance, z_dir * z_distance);
        }

        /// <summary>
        /// 传入坐标系1的坐标，返回坐标系2的对应坐标
        /// </summary>
        /// <param name="cs1Pos"></param>
        /// <returns></returns>
        public PointF3 GetCoordinate(PointF3 cs1Pos)
        {
            Vector3 t2pos = quatDiff * cs1Pos.ToVector3();

            return (PointF3)t2pos + offset;
        }
    }

}