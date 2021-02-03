using UnityEngine;

namespace NonsensicalKit.Custom
{
    public struct PointF3
    {
        public readonly static PointF3 zero = new PointF3(0,0,0);
        public readonly static PointF3 one = new PointF3(1,1,1);

        public float x;
        public float y;
        public float z;

        public PointF3(float _f1, float _f2, float _f3)
        {
            x = _f1;
            y = _f2;
            z = _f3;
        }

        public PointF3(Vector3 _vector3)
        {
            x = _vector3.x;
            y = _vector3.y;
            z = _vector3.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        public static PointF3 operator *(PointF3 a, float b)
        {
            PointF3 c = new PointF3
            {
                x = a.x * b,
                y = a.y * b,
                z = a.z * b
            };
            return c;
        }

        public static PointF3 operator /(PointF3 a, float b)
        {
            PointF3 c = new PointF3
            {
                x = a.x / b,
                y = a.y / b,
                z = a.z / b
            };
            return c;
        }

        public static PointF3 operator +(PointF3 a, PointF3 b)
        {
            PointF3 c = new PointF3
            {
                x = a.x + b.x,
                y = a.y + b.y,
                z = a.z + b.z
            };
            return c;
        }
        public static PointF3 operator -(PointF3 a, PointF3 b)
        {
            PointF3 c = new PointF3
            {
                x = a.x - b.x,
                y = a.y - b.y,
                z = a.z - b.z
            };
            return c;
        }
        public static PointF3 operator -(PointF3 a)
        {
            PointF3 c = new PointF3
            {
                x = -a.x,
                y = -a.y,
                z = -a.z
            };
            return c;
        }

        public override string ToString()
        {
            return $"({x},{y},{z})";
        }

        public static explicit operator PointF3(Vector3 v)
        {
            return new PointF3(v.x, v.y, v.z);
        }

       public static float Distance(PointF3 pos1,PointF3 pos2)
        {
            float f1Offset = pos1.x - pos2.x;
            float f2Offset = pos1.y - pos2.y;
            float f3Offset = pos1.z - pos2.z;
            float temp = (f1Offset) * (f1Offset) + (f2Offset) * (f2Offset) + (f3Offset) * (f3Offset);
            return Mathf.Sqrt(temp);
        }
    }
}