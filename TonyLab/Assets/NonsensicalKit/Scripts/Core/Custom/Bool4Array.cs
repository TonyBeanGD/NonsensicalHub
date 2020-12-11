namespace NonsensicalKit.Custom
{
    public struct Bool4Array
    {
        private readonly bool[] boolArray;

        public readonly int length0;
        public readonly int length1;
        public readonly int length2;
        public readonly int length3;

        private readonly int step0;
        private readonly int step1;
        private readonly int step2;

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

        public bool this[Int3 int3, int index3]
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

}