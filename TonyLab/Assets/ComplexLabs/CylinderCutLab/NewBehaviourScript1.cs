using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NonsensicalKit;
using System.Linq;
using NonsensicalKit.Custom;

namespace MyNamespace
{
    public class NewBehaviourScript1 : MonoBehaviour
    {
        [SerializeField] private Transform Knife;

        Mesh mesh;

        private CylinderBurin cylinderBurin;
        private CylinderModel cylinderModel;
        private void Awake()
        {
            mesh = gameObject.AddComponent<MeshFilter>().mesh;
            gameObject.AddComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/white");

            cylinderBurin = new CylinderBurin(Knife.position, Knife.GetComponent<MeshFilter>().mesh);
            cylinderModel = new CylinderModel(transform.position, Vector3.up, 0.001f,0.03f,0.03f);
        }

        private void Update()
        {
            cylinderBurin.MoveTo(Knife.position);
            cylinderBurin.RotateTo(Knife.rotation);
            cylinderBurin.ScaleTo(Knife.lossyScale);
            cylinderModel.MoveTo(transform.position);
            cylinderModel.RotateTo(transform.up);
            cylinderModel.CuttingBy(cylinderBurin);

            cylinderModel.ToMesh(mesh);
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Vector3[] temp = cylinderModel.GetLineArray();

                for (int i = 0; i < temp.Length; i += 2)
                {
                    if (i / 2 % 2 == 0)
                    {
                        Gizmos.color = Color.red;
                    }
                    else
                    {
                        Gizmos.color = Color.green;
                    }
                    Gizmos.DrawLine(temp[i], temp[i + 1]);
                }

                Vector3[] temp2 = cylinderModel.GetPointArray();

                Gizmos.color = Color.blue;
                for (int i = 0; i < temp2.Length; i += 2)
                {

                    Gizmos.DrawLine(temp2[i], temp2[i + 1]);
                }
            }
        }
    }

    class CylinderModel
    {
        private Vector3 _firstMiddlePointPosition;
        private Vector3 _dir;


        private readonly float _interval;
        private CylinderSection[] _cylinderSectionArray;
        private CylinderSectionBuffer[] _cylinderSectionBufferArray;

        public CylinderModel(Vector3 firstMiddlePointPosition, Vector3 dir, float interval, float radius, float height)
        {
            this._firstMiddlePointPosition = firstMiddlePointPosition;
            this._dir = dir;
            this._interval = interval;

            int pointCount = (int)(height / interval) + 1;
            _cylinderSectionArray = new CylinderSection[pointCount];

            for (int i = 0; i < pointCount; i++)
            {
                _cylinderSectionArray[i] = new CylinderSection();
                _cylinderSectionArray[i].Point.Add(0);
                _cylinderSectionArray[i].Point.Add(radius);
            }
        }

        public void MoveTo(Vector3 pos)
        {
            _firstMiddlePointPosition = pos;
        }

        public void RotateTo(Vector3 up)
        {
            _dir = up;
        }

        public void CuttingBy(CylinderBurin cylinderBurin)
        {
            _cylinderSectionBufferArray = new CylinderSectionBuffer[_cylinderSectionArray.Length];
            for (int i = 0; i < _cylinderSectionBufferArray.Length; i++)
            {
                _cylinderSectionBufferArray[i] = new CylinderSectionBuffer(GetWorldPos(i), _dir);
            }
            for (int i = 0; i < cylinderBurin.Mesh.triangles.Length; i += 3)
            {
                Vector3 p1 = cylinderBurin.CenterPoint + cylinderBurin.Rotation * (Vector3.Scale(cylinderBurin.Mesh.vertices[cylinderBurin.Mesh.triangles[i + 0]], cylinderBurin.LossyScale));
                Vector3 p2 = cylinderBurin.CenterPoint + cylinderBurin.Rotation * (Vector3.Scale(cylinderBurin.Mesh.vertices[cylinderBurin.Mesh.triangles[i + 1]], cylinderBurin.LossyScale));
                Vector3 p3 = cylinderBurin.CenterPoint + cylinderBurin.Rotation * (Vector3.Scale(cylinderBurin.Mesh.vertices[cylinderBurin.Mesh.triangles[i + 2]], cylinderBurin.LossyScale));
                CuttingWithLine(p1, p2);
                CuttingWithLine(p1, p3);
                CuttingWithLine(p2, p3);
            }

            CalculationBuffer();
        }

        private void CuttingWithLine(Vector3 linePoint1, Vector3 linePoint2)
        {
            Vector3 footDrop1 = VectorHelper.GetFootDrop(linePoint1, _firstMiddlePointPosition, _firstMiddlePointPosition + _dir);
            Vector3 footDrop2 = VectorHelper.GetFootDrop(linePoint2, _firstMiddlePointPosition, _firstMiddlePointPosition + _dir);

            float distance1 = Vector3.Distance(footDrop1, _firstMiddlePointPosition);
            float distance2 = Vector3.Distance(footDrop2, _firstMiddlePointPosition);

            int dir1 = Vector3.Dot(footDrop1 - _firstMiddlePointPosition, _dir) > 0 ? 1 : -1; ;
            int dir2 = Vector3.Dot(footDrop2 - _firstMiddlePointPosition, _dir) > 0 ? 1 : -1; ;

            int p1 = (int)(distance1 / _interval);
            int p2 = (int)(distance2 / _interval);

            p1 = p1 * dir1;
            p2 = p2 * dir2;

            if ((p1 >= _cylinderSectionArray.Length && p2 >= _cylinderSectionArray.Length)
                || (p1 < 0 && p2 < 0))
            {
                return;
            }

            int pMin = Mathf.Min(p1, p2);
            int pMax = Mathf.Max(p1, p2);
            int pMinLimit = Mathf.Max(pMin, 0);
            int pMaxLimit = Mathf.Min(pMax, _cylinderSectionArray.Length - 1);
            for (int i = pMinLimit; i <= pMaxLimit; i++)
            {
                float proportion;
                if ((pMax - pMin) == 0)
                {
                    proportion = 0;
                }
                else
                {
                    proportion = (float)(i - pMin) / (pMax - pMin);
                }

                Vector3 tempPoint;
                if (p1 < p2)
                {
                    tempPoint = Vector3.Lerp(linePoint1, linePoint2, proportion);
                }
                else
                {
                    tempPoint = Vector3.Lerp(linePoint2, linePoint1, proportion);
                }
                _cylinderSectionBufferArray[i].AddPoint(tempPoint);
            }
        }

        private void CalculationBuffer()
        {
            foreach (var item in _cylinderSectionBufferArray)
            {
                item.SortOutMin();
            }

            for (int i = 0; i < _cylinderSectionArray.Length; i++)
            {
                float min = _cylinderSectionBufferArray[i].MinDistance;
                float max = _cylinderSectionBufferArray[i].MaxDistance;

                if (_cylinderSectionBufferArray[i].HavePoint)
                {
                    if (_cylinderSectionBufferArray[i].IsSurrounded)
                    {
                        bool haveBiggest = false;
                        bool haveSmallest = false;
                        for (int j = 0; j < _cylinderSectionArray[i].Point.Count; j++)
                        {
                            if (_cylinderSectionArray[i].Point[j] > max)
                            {
                                haveBiggest = true;
                            }
                            else if (_cylinderSectionArray[i].Point[j] < max)
                            {
                                _cylinderSectionArray[i].Point.RemoveAt(j);
                                j--;
                                haveSmallest = true;
                            }
                        }
                        if (haveBiggest && haveSmallest)
                        {
                            _cylinderSectionArray[i].Point.Add(max);
                        }
                    }
                    else
                    {
                        bool haveBiggest = false;
                        bool haveBetween = false;
                        bool haveSmallest = false;

                        bool minBetween = _cylinderSectionArray[i].Point.CheckPos(min) % 2 == 0;
                        bool maxBetween = _cylinderSectionArray[i].Point.CheckPos(max) % 2 == 0;

                        for (int j = 0; j < _cylinderSectionArray[i].Point.Count; j++)
                        {

                            if (_cylinderSectionArray[i].Point[j] >= max)
                            {
                                haveBiggest = true;
                            }

                            if (_cylinderSectionArray[i].Point[j] <= min)
                            {
                                haveSmallest = true;
                            }

                            if (_cylinderSectionArray[i].Point[j] > min && _cylinderSectionArray[i].Point[j] <= max)
                            {
                                _cylinderSectionArray[i].Point.RemoveAt(j);
                                j--;
                                haveBetween = true;
                            }
                        }


                        if ((haveBiggest || haveBetween) && haveSmallest && !minBetween)
                        {
                            _cylinderSectionArray[i].Point.Add(min);

                        }
                        if (haveBiggest && !maxBetween)
                        {
                            _cylinderSectionArray[i].Point.Add(max);

                        }
                    }
                }
            }
        }

        private Vector3 GetWorldPos(int index)
        {
            return _firstMiddlePointPosition + _dir * index * _interval;
        }

        private Vector3 GetLocalPos(int index)
        {
            return Vector3.up * index * _interval;
        }

        public void ToMesh(Mesh mesh, int smoothness = 32)
        {
            MeshBuffer meshBuffer = new MeshBuffer();

            foreach (var item in _cylinderSectionArray)
            {
                item.InitCheck();
            }

            int? first = null;
            int? last = null;

            for (int i = 0; i < _cylinderSectionArray.Length; i++)
            {
                if (_cylinderSectionArray[i].Point.Count > 0)
                {
                    if (first == null)
                    {
                        first = i;
                    }
                    last = i;
                    if (i < _cylinderSectionArray.Length - 1)
                    {
                        for (int k = 0; k < _cylinderSectionArray[i].Point.Count - 1; k += 2)
                        {
                            if (i - 1 >= 0 && (_cylinderSectionArray[i - 1].Point.Count <= 0 || _cylinderSectionArray[i].Point[k + 1] > _cylinderSectionArray[i - 1].Point[_cylinderSectionArray[i - 1].Point.Count - 1]))
                            {
                                meshBuffer.AddRing(GetLocalPos(i), _cylinderSectionArray[i].Point[k], _cylinderSectionArray[i].Point[k + 1], Vector3.up, smoothness);
                            }
                        }
                        for (int k = 0; k < _cylinderSectionArray[i].Point.Count - 1; k += 2)
                        {
                            if (i + 1 < _cylinderSectionArray.Length && (_cylinderSectionArray[i + 1].Point.Count <= 0 || _cylinderSectionArray[i].Point[k + 1] > _cylinderSectionArray[i + 1].Point[_cylinderSectionArray[i + 1].Point.Count - 1]))
                            {
                                meshBuffer.AddRing(GetLocalPos(i), _cylinderSectionArray[i].Point[k], _cylinderSectionArray[i].Point[k + 1], -Vector3.up, smoothness);
                            }
                        }

                        for (int j = 0; j < _cylinderSectionArray[i].Point.Count; j++)
                        {
                            if (_cylinderSectionArray[i].check[j] == false)
                            {
                                int lastIndex = i + 1;
                                for (int k = i + 1; k < _cylinderSectionArray.Length; k++)
                                {
                                    if (j < _cylinderSectionArray[k].Point.Count)
                                    {
                                        if (NumHelper.IsNear(_cylinderSectionArray[i].Point[j], _cylinderSectionArray[k].Point[j]))
                                        {
                                            _cylinderSectionArray[k - 1].check[j] = true;
                                            lastIndex = k;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                if (j < _cylinderSectionArray[lastIndex].Point.Count)
                                {
                                    if (j % 2 == 0)
                                    {
                                        meshBuffer.AddRing3D(GetLocalPos(i), _cylinderSectionArray[i].Point[j], GetLocalPos(lastIndex), _cylinderSectionArray[lastIndex].Point[j], -Vector3.up, smoothness);
                                    }
                                    else
                                    {
                                        meshBuffer.AddRing3D(GetLocalPos(i), _cylinderSectionArray[i].Point[j], GetLocalPos(lastIndex), _cylinderSectionArray[lastIndex].Point[j], Vector3.up, smoothness);
                                    }
                                }

                            }

                        }

                    }
                }
            }
            if (first != null)
            {
                for (int i = 0; i < _cylinderSectionArray[(int)first].Point.Count - 1; i += 2)
                {
                    meshBuffer.AddRing(GetLocalPos((int)first), _cylinderSectionArray[(int)first].Point[i], _cylinderSectionArray[(int)first].Point[i + 1], Vector3.up, smoothness);
                }
            }
            if (last != null)
            {
                for (int i = 0; i < _cylinderSectionArray[(int)last].Point.Count - 1; i += 2)
                {
                    meshBuffer.AddRing(GetLocalPos((int)last), _cylinderSectionArray[(int)last].Point[i], _cylinderSectionArray[(int)last].Point[i + 1], -Vector3.up, smoothness);
                }
            }

            meshBuffer.Apply(mesh);
        }

        public Vector3[] GetLineArray()
        {
            Vector3[] linePointArray = new Vector3[_cylinderSectionBufferArray.Length * 4];

            Vector3 dir1 = VectorHelper.GetCommonVerticalLine(_dir, _dir).normalized;
            Vector3 dir2 = VectorHelper.GetCommonVerticalLine(dir1, _dir).normalized;

            for (int i = 0; i < _cylinderSectionBufferArray.Length; i++)
            {
                if (_cylinderSectionBufferArray[i].HavePoint)
                {
                    Vector3 centerPoint = GetWorldPos(i);
                    linePointArray[i * 4 + 0] = centerPoint + dir1 * _cylinderSectionBufferArray[i].MinDistance + dir2;
                    linePointArray[i * 4 + 1] = centerPoint + dir1 * _cylinderSectionBufferArray[i].MinDistance - dir2;
                    linePointArray[i * 4 + 2] = centerPoint + dir1 * _cylinderSectionBufferArray[i].MaxDistance + dir2;
                    linePointArray[i * 4 + 3] = centerPoint + dir1 * _cylinderSectionBufferArray[i].MaxDistance - dir2;
                }

            }

            return linePointArray;
        }

        public Vector3[] GetPointArray()
        {
            List<Vector3> pointArray = new List<Vector3>();

            Vector3 dir1 = VectorHelper.GetCommonVerticalLine(_dir, _dir).normalized;
            Vector3 dir2 = VectorHelper.GetCommonVerticalLine(dir1, _dir).normalized;

            for (int i = 0; i < _cylinderSectionArray.Length; i++)
            {
                Vector3 centerPoint = GetWorldPos(i);
                for (int j = 0; j < _cylinderSectionArray[i].Point.Count; j++)
                {
                    pointArray.Add(centerPoint + dir1 * _cylinderSectionArray[i].Point[j] + dir2);
                    pointArray.Add(centerPoint + dir1 * _cylinderSectionArray[i].Point[j] - dir2);
                }

            }

            return pointArray.ToArray();
        }
    }

    class CylinderSectionBuffer
    {
        public bool HavePoint;
        public bool IsSurrounded;
        public float MaxDistance;
        public float MinDistance;

        /// <summary>
        /// 中心点
        /// </summary>
        private Vector3 _center;

        /// <summary>
        /// 法线
        /// </summary>
        private Vector3 _normal;

        /// <summary>
        /// y轴方向
        /// </summary>
        private Vector3? _yAxis;

        private List<Vector3> _allPointList;

        private bool[] _havePoint;
        private List<Vector3> _quadrant1;
        private List<Vector3> _quadrant2;
        private List<Vector3> _quadrant3;
        private List<Vector3> _quadrant4;

        public List<Vector3> GetPointsListClone()
        {
            return new List<Vector3>(_allPointList.ToArray());
        }

        public CylinderSectionBuffer(Vector3 center, Vector3 normal)
        {
            HavePoint = false;
            IsSurrounded = false;
            MaxDistance = -1;
            _normal = normal;
            MinDistance = 2147483647;
            _center = center;
            _havePoint = new bool[4];
            _allPointList = new List<Vector3>();
            _quadrant1 = new List<Vector3>();
            _quadrant2 = new List<Vector3>();
            _quadrant3 = new List<Vector3>();
            _quadrant4 = new List<Vector3>();
        }

        public void SortOutMin()
        {
            for (int i = 0; i < _allPointList.Count - 1; i++)
            {
                for (int j = i + 1; j < _allPointList.Count; j++)
                {
                    Vector3? footDrop = VectorHelper.GetFootDropInLineSegment(_center, _allPointList[i], _allPointList[j]);
                    if (footDrop != null)
                    {
                        float sideDistance = Vector3.Distance((Vector3)footDrop, _center);

                        MinDistance = Mathf.Min(MinDistance, sideDistance);
                    }
                }
            }
        }

        public bool AddPoint(Vector3 newPoint)
        {
            HavePoint = true;
            _allPointList.Add(newPoint);
            float distance = Vector3.Distance(_center, newPoint);
            MaxDistance = Mathf.Max(MaxDistance, distance);
            MinDistance = Mathf.Min(MinDistance, distance);

            if (IsSurrounded == true)
            {
                return true;
            }
            if (_yAxis == null)
            {
                if (newPoint != _center)
                {
                    _yAxis = newPoint - _center;
                }

                return false;
            }

            Vector3 newDir = newPoint - _center;

            float signedAngle = Vector3.SignedAngle((Vector3)_yAxis, newDir, _normal);

            if (signedAngle > 0)
            {
                if (signedAngle >= 90)
                {
                    _havePoint[3] = true;
                    _quadrant4.Add(newPoint);
                }
                else
                {
                    _havePoint[0] = true;
                    _quadrant1.Add(newPoint);
                }
            }
            else
            {
                if (signedAngle <= -90)
                {
                    _havePoint[2] = true;
                    _quadrant3.Add(newPoint);
                }
                else
                {
                    _havePoint[1] = true;
                    _quadrant2.Add(newPoint);
                }
            }

            IsSurrounded = IsSurrounded == true ? true : CheckSurrounded();
            return IsSurrounded;
        }

        public bool CheckSurrounded()
        {
            //三四象限有点
            if (_havePoint[2] && _havePoint[3])
            {
                return true;
            }

            //包括一二象限在内的三个象限有点
            if (_havePoint[0] && _havePoint[1] && (_havePoint[2] || _havePoint[3]))
            {
                List<Vector3> exist;
                List<Vector3> inverse;

                if (_havePoint[2])
                {
                    exist = _quadrant3;
                    inverse = _quadrant1;
                }
                else
                {
                    exist = _quadrant4;
                    inverse = _quadrant2;
                }

                foreach (var existPoint in exist)
                {
                    foreach (var inversePoint in inverse)
                    {
                        Vector3 footDrop = VectorHelper.GetFootDrop(_center, existPoint, inversePoint);
                        Vector3 footDropDir = footDrop - _center;
                        if (Vector3.Angle(footDropDir, (Vector3)_yAxis) > 90)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }

    class CylinderSection
    {
        public AutoSortList<float> Point;

        public List<bool> check;

        public CylinderSection()
        {
            Point = new AutoSortList<float>();
        }
        public void InitCheck()
        {
            bool[] temp = new bool[Point.Count];
            check = new List<bool>(temp);
        }
    }

    class CylinderBurin
    {
        public Vector3 CenterPoint { get; private set; }
        public Vector3 LossyScale { get; private set; }
        public Quaternion Rotation { get; private set; }
        public Mesh Mesh { get; private set; }

        public CylinderBurin(Vector3 centerPoint, Mesh mesh)
        {
            CenterPoint = centerPoint;
            Mesh = mesh;
        }

        public void MoveTo(Vector3 pos)
        {
            CenterPoint = pos;
        }

        public void RotateTo(Quaternion rotation)
        {
            Rotation = rotation;
        }

        public void ScaleTo(Vector3 scale)
        {
            LossyScale = scale;
        }
    }
}
