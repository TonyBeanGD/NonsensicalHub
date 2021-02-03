namespace NonsensicalKit.Custom
{


    public struct PointI3
    {
        public int i1;
        public int i2;
        public int i3;

        public PointI3(int _i1, int _i2, int _i3)
        {
            i1 = _i1;
            i2 = _i2;
            i3 = _i3;
        }


        public PointI3(PointF3 _float3)
        {
            i1 = (int)_float3.x;
            if (_float3.x - i1 > 0.5f)
            {
                i1++;
            }


            i2 = (int)_float3.y;
            if (_float3.y - i2 > 0.5f)
            {
                i2++;
            }


            i3 = (int)_float3.z;
            if (_float3.z - i3 > 0.5f)
            {
                i3++;
            }
        }

        public static PointI3 operator +(PointI3 a, PointI3 b)
        {
            PointI3 c = new PointI3
            {
                i1 = a.i1 + b.i1,
                i2 = a.i2 + b.i2,
                i3 = a.i3 + b.i3
            };
            return c;
        }
        public static PointI3 operator -(PointI3 a, PointI3 b)
        {
            PointI3 c = new PointI3
            {
                i1 = a.i1 - b.i1,
                i2 = a.i2 - b.i2,
                i3 = a.i3 - b.i3
            };
            return c;
        }
        public static PointI3 operator -(PointI3 a)
        {
            PointI3 c = new PointI3
            {
                i1 = -a.i1,
                i2 = -a.i2,
                i3 = -a.i3
            };
            return c;
        }

        public int GetValue(PointI3 a)
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
}