using System.Collections;
using Unity.Jobs;
using UnityEngine;

namespace NonsensicalFrame
{
    public struct Int3:IJob
    {
        public int i1;
        public int i2;
        public int i3;

        public Int3(int _i1, int _i2, int _i3)
        {
            i1 = _i1;
            i2 = _i2;
            i3 = _i3;
        }


        public Int3(Float3 _float3)
        {
            i1 = Mathf.RoundToInt(_float3.f1);
            i2 = Mathf.RoundToInt(_float3.f2);
            i3 = Mathf.RoundToInt(_float3.f3);
        }

        public static Int3 operator +(Int3 a, Int3 b)
        {
            Int3 c = new Int3
            {
                i1 = a.i1 + b.i1,
                i2 = a.i2 + b.i2,
                i3 = a.i3 + b.i3
            };
            return c;
        }
        public static Int3 operator -(Int3 a, Int3 b)
        {
            Int3 c = new Int3
            {
                i1 = a.i1 - b.i1,
                i2 = a.i2 - b.i2,
                i3 = a.i3 - b.i3
            };
            return c;
        }
        public static Int3 operator -(Int3 a)
        {
            Int3 c = new Int3
            {
                i1 = -a.i1,
                i2 = -a.i2,
                i3 = -a.i3
            };
            return c;
        }

        public int GetValue(Int3 a)
        {
            if (a.i1 != 0)
            {
                return this.i1;
            }
            else if (a.i2 != 0)
            {
                return this.i2;
            }
            else if (a.i3 != 0)
            {
                return this.i3;
            }
            else
            {
                return 0;
            }
        }

        public bool CheckBound(int max1, int max2, int max3)
        {
            if (i1 < 0 || i2 < 0 || i3 < 0 || i1 > max1 || i2 > max2 || i3 > max3)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override string ToString()
        {
            return $"({i1},{i2},{i3})";
        }

        public void Execute()
        {
            throw new System.NotImplementedException();
        }
    }

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
    }
}