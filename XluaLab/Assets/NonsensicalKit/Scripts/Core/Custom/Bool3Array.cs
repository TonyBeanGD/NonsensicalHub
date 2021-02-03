namespace NonsensicalKit.Custom
{
    public struct Bool3Array
    {
        private readonly bool[] boolArray;

        public readonly int length0;
        public readonly int length1;
        public readonly int length2;

        private readonly int step0;
        private readonly int step1;

        public Bool3Array(int _length0, int _length1, int _length2)
        {
            boolArray = new bool[_length0 * _length1 * _length2];
            length0 = _length0;
            length1 = _length1;
            length2 = _length2;

            step0 = _length1 * _length2;
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

        public bool this[PointI3 int3]
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
}
