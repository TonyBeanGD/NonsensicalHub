using UnityEngine;

namespace NonsensicalKit
{

    /// <summary>
    /// 自定义Quaternion，用于求四元数差值
    /// https://stackoverflow.com/questions/22157435/difference-between-the-two-quaternions
    /// </summary>
    public struct Quat
    {
        float x;
        float y;
        float z;
        float w;

        public Quat(Quaternion q)
        {
            x = q.x;
            y = q.y;
            z = q.z;
            w = q.w;
        }

        public Quat(float _x, float _y, float _z, float _w)
        {
            x = _x;
            y = _y;
            z = _z;
            w = _w;
        }

        public Quaternion ToQuaternion()
        {
            return new Quaternion(x, y, z, w);
        }

        public static Quaternion Diff(Quaternion q1, Quaternion q2)
        {
            Quat dif = Quat.Diff(new Quat(q1), new Quat(q2));
            return dif.ToQuaternion();
        }

        public static Quat Diff(Quat a, Quat b)
        {
            Quat inv = a;
            inv = inv.inverse();
            return inv * b;
        }

        public Quat inverse()
        {
            Quat q = this;
            q = q.conjugate();
            return q / Quat.Dot(this, this);
        }

        public Quat conjugate()
        {
            Quat q;
            q.x = -this.x;
            q.y = -this.y;
            q.z = -this.z;
            q.w = this.w;

            return q;
        }

        public static float Dot(Quat q1, Quat q2)
        {
            return q1.x * q2.x + q1.y * q2.y + q1.z * q2.z + q1.w * q2.w;
        }

        public static Quat operator *(Quat q1, Quat q2)
        {
            Quat qu = new Quat
            {
                x = q1.w * q2.x + q1.x * q2.w + q1.y * q2.z - q1.z * q2.y,
                y = q1.w * q2.y + q1.y * q2.w + q1.z * q2.x - q1.x * q2.z,
                z = q1.w * q2.z + q1.z * q2.w + q1.x * q2.y - q1.y * q2.x,
                w = q1.w * q2.w - q1.x * q2.x - q1.y * q2.y - q1.z * q2.z
            };
            return qu;
        }

        public static Quat operator /(Quat q, float s)
        {
            return new Quat(q.x / s, q.y / s, q.z / s, q.w / s);
        }
    }
}