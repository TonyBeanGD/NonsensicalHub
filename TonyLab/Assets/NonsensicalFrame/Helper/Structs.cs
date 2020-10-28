using System.Collections;
using UnityEngine;

namespace NonsensicalFrame
{
    public struct Int3
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
    }

    public struct Float3
    {
        public float x;
        public float y;
        public float z;

        public Float3(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public Float3(Vector3 _vector3)
        {
            x = _vector3.x;
            y = _vector3.y;
            z = _vector3.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }
}