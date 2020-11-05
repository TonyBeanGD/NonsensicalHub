using System.Collections;
using Unity.Jobs;
using UnityEngine;

namespace NonsensicalKit
{
    public struct Bool3Array
    {
        private readonly bool[] boolArray;

        private readonly int length0;
        private readonly int length1;
        private readonly int length2;

        private readonly int step0;
        private readonly int step1;

        public Bool3Array(int _length0, int _length1, int _length2)
        {
            boolArray = new bool[_length0 * _length1 * _length2];
            length0 = _length0;
            length1 = _length1;
            length2 = _length2;

            step0 = _length1 * _length2 ;
            step1 = _length2;
        }

        public bool this[int index0, int index1, int index2]
        {
            get
            {
                return boolArray[index0 * step0 + index1 * step1 + index2];
            }
            set
            {
                    boolArray[index0 * step0 + index1 * step1 + index2] = value;
              
            }
        }

        public bool this[Int3 int3]
        {
            get
            {
                return boolArray[int3.i1 * step0 + int3.i2 * step1 + int3.i3];
            }
            set
            {
                boolArray[int3.i1 * step0 + int3.i2 * step1 + int3.i3] = value;

            }
        }
    }

    public struct Bool4Array
    {
        readonly bool[] boolArray;

        readonly int length0;
        readonly int length1;
        readonly int length2;
        readonly int length3;

        readonly int step0;
        readonly int step1;
        readonly int step2;

        public Bool4Array(int _length0, int _length1, int _length2, int _length3)
        {
            boolArray = new bool[_length0 * _length1 * _length2 * _length3];
            length0 = _length0;
            length1 = _length1;
            length2 = _length2;
            length3 = _length3;

            step0 = _length1 * _length2 * _length3;
            step1 = _length2 * _length3;
            step2 = _length3;
        }

        public bool this[int index0, int index1, int index2, int index3]
        {
            get
            {
                return boolArray[index0 * step0 + index1 * step1 + index2 * step2 + index3];
            }
            set
            {
                boolArray[index0 * step0 + index1 * step1 + index2 * step2 + index3] = value;
            }
        }

        public bool this[Int3 int3,int index3]
        {
            get
            {
                return boolArray[int3.i1 * step0 + int3.i2 * step1 + int3.i3 * step2 + index3];
            }
            set
            {
                boolArray[int3.i1 * step0 + int3.i2 * step1 + int3.i3 * step2 + index3] = value;
            }
        }
    }

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


        public Int3(Float3 _float3)
        {
            i1 = (int)_float3.f1;
            if (_float3.f1 - i1 > 0.5f)
            {
                i1++;
            }


            i2 = (int)_float3.f2;
            if (_float3.f2 - i2 > 0.5f)
            {
                i2++;
            }


            i3 = (int)_float3.f3;
            if (_float3.f3 - i3 > 0.5f)
            {
                i3++;
            }
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

    /// <summary>
    /// 坐标系差异运算
    /// </summary>
    public struct CoordinateSystemDiff
    {
        Quaternion quatDiff;
        Float3 offset;
        float unitSize;

        public CoordinateSystemDiff(CoordinateSystem cs1, CoordinateSystem cs2, float _unitSize = 1)
        {
            unitSize = _unitSize;

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

            offset = new Float3(x_dir * x_distance, y_dir * y_distance, z_dir * z_distance);
        }

        /// <summary>
        /// 传入坐标系1的坐标，返回坐标系2的坐标
        /// </summary>
        /// <param name="cs1Pos"></param>
        /// <returns></returns>
        public Float3 GetCoordinate(Float3 cs1Pos)
        {
            Vector3 t2pos = quatDiff * cs1Pos.ToVector3();

            return new Float3(t2pos) + offset;
        }
    }

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