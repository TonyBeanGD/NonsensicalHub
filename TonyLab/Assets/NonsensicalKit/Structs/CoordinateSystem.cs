using UnityEngine;

namespace NonsensicalKit
{
    /// <summary>
    /// 坐标系
    /// </summary>
    public struct CoordinateSystem
    {
        public Vector3 origin;
        public Vector3 right;
        public Vector3 up;
        public Vector3 forward;
        public Quaternion rotation;

        public CoordinateSystem(Transform transform)
        {
            origin = transform.position;
            right = transform.right;
            up = transform.up;
            forward = transform.forward;
            rotation = transform.rotation;
        }

        public CoordinateSystem(Vector3 _origin, Vector3 _right, Vector3 _up, Vector3 _forward)
        {
            origin = _origin;
            right = _right;
            up = _up;
            forward = _forward;
            rotation = Quaternion.LookRotation(_forward, _up);
        }

        /// <summary>
        /// 获取世界坐标在这个坐标系中的坐标
        /// </summary>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        public Float3 GetCoordinate(Vector3 worldPos)
        {
            Vector3 xPoint = VectorHelper.GetFootDrop(worldPos, this.origin, this.origin + this.right);
            Vector3 yPoint = VectorHelper.GetFootDrop(worldPos, this.origin, this.origin + this.up);
            Vector3 zPoint = VectorHelper.GetFootDrop(worldPos, this.origin, this.origin + this.forward);

            float x_distance = Vector3.Distance(this.origin, xPoint);
            float y_distance = Vector3.Distance(this.origin, yPoint);
            float z_distance = Vector3.Distance(this.origin, zPoint);

            int x_dir = Vector3.Dot(xPoint - origin, right) > 0 ? 1 : -1;
            int y_dir = Vector3.Dot(yPoint - origin, up) > 0 ? 1 : -1;
            int z_dir = Vector3.Dot(zPoint - origin, forward) > 0 ? 1 : -1;

            Float3 Point1Value = new Float3(x_dir * x_distance, y_dir * y_distance, z_dir * z_distance);

            return Point1Value;
        }

        /// <summary>
        /// 获取对应坐标的世界坐标
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Vector3 GetWorldPos(Float3 value)
        {
            return origin + right * value.f1 + up * value.f2 + forward * value.f3;
        }

        /// <summary>
        /// 获取另一个坐标系中的目标坐标在本坐标系中的坐标
        /// </summary>
        /// <param name="targetCoordinateSystem"></param>
        /// <param name="targetPointValue"></param>
        /// <returns></returns>
        public Float3 CoordinateSystemTransform(CoordinateSystem targetCoordinateSystem, Float3 targetPointValue)
        {
            Vector3 point2_World = targetCoordinateSystem.origin + targetPointValue.f1 * targetCoordinateSystem.right +
                targetPointValue.f2 * targetCoordinateSystem.up + targetPointValue.f3 * targetCoordinateSystem.forward;

            return GetCoordinate(point2_World);
        }

        public override string ToString()
        {
            return origin.ToString("f5") + "," + right.ToString("f5") + "," + up.ToString("f5") + "," + forward.ToString("f5");
        }
    }
}