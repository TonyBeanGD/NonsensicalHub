using UnityEngine;

namespace NonsensicalKit.Custom
{
    public struct Float3
    {
        public float f1;
        public float f2;
        public float f3;

        public Float3(float _f1, float _f2, float _f3)
        {
            f1 = _f1;
            f2 = _f2;
            f3 = _f3;
        }

        public Float3(Vector3 _vector3)
        {
            f1 = _vector3.x;
            f2 = _vector3.y;
            f3 = _vector3.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(f1, f2, f3);
        }

        public static Float3 operator *(Float3 a, float b)
        {
            Float3 c = new Float3
            {
                f1 = a.f1 * b,
                f2 = a.f2 * b,
                f3 = a.f3 * b
            };
            return c;
        }

        public static Float3 operator /(Float3 a, float b)
        {
            Float3 c = new Float3
            {
                f1 = a.f1 / b,
                f2 = a.f2 / b,
                f3 = a.f3 / b
            };
            return c;
        }

        public static Float3 operator +(Float3 a, Float3 b)
        {
            Float3 c = new Float3
            {
                f1 = a.f1 + b.f1,
                f2 = a.f2 + b.f2,
                f3 = a.f3 + b.f3
            };
            return c;
        }
        public static Float3 operator -(Float3 a, Float3 b)
        {
            Float3 c = new Float3
            {
                f1 = a.f1 - b.f1,
                f2 = a.f2 - b.f2,
                f3 = a.f3 - b.f3
            };
            return c;
        }
        public static Float3 operator -(Float3 a)
        {
            Float3 c = new Float3
            {
                f1 = -a.f1,
                f2 = -a.f2,
                f3 = -a.f3
            };
            return c;
        }

        public override string ToString()
        {
            return $"({f1},{f2},{f3})";
        }

        public static explicit operator Float3(Vector3 v)
        {
            return new Float3(v.x, v.y, v.z);
        }
    }
}