using NonsensicalKit;
using NonsensicalKit.Utility;
using NonsensicalKit.Custom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LatheSimulationNoScale : MonoBehaviour
{
    public bool _isOnInit;   //初始是否开启切削

    public float _sectionThickness = 0.1f;     //切面厚度
    public float _radius = 0.5f;        //半径
    public float _height = 1f;          //高度

    [SerializeField] private Transform _knife;                   //刀具对象

    [SerializeField] private bool _drawDebug;                   //绘制debug线条

    private Mesh _mesh;
    private bool _isOn;

    private CylinderBurin _cylinderBurin;
    private CylinderModelNoScale _cylinderModel;

    private void Awake()
    {
        _isOn = _isOnInit;
        Switch(_isOn);
    }

    public void Switch(bool isOn)
    {
        _isOn = isOn;
        if (_isOn)
        {
            if (GetComponent<MeshFilter>() == null || _knife == null)
            {
                _isOn = false;
                Debug.Log("信息不足，无法开启");
            }
            else
            {
                InitModel();
                SetBurin(_knife);
            }
        }

    }

    public void SetBurin(Transform knife)
    {
        _cylinderBurin = new CylinderBurin(knife.localToWorldMatrix, knife.GetComponent<MeshFilter>().mesh);
    }

    private void InitModel()
    {
        Vector3 length = transform.GetComponent<MeshFilter>().mesh.bounds.size;

        _mesh = gameObject.GetComponent<MeshFilter>().mesh;

        _cylinderModel = new CylinderModelNoScale(_sectionThickness, _radius, _height);
        UpdateCylinderModel();
        _cylinderModel.Init();

        _cylinderModel.ToMesh(_mesh);
    }

    private void Update()
    {
        if (_isOn && _cylinderBurin != null && _cylinderModel != null)
        {
            _cylinderBurin.TransformTo(_knife.localToWorldMatrix);

            UpdateCylinderModel();
            if (_cylinderModel.CuttingBy(_cylinderBurin))
            {
                Debug.Log(true);
                _cylinderModel.ToMesh(_mesh);
            }
            Debug.Log(false);
        }
    }

    private void UpdateCylinderModel()
    {
        _cylinderModel.TransformTo(transform.position, transform.up);
    }

    private void OnDrawGizmos()
    {
        if (_isOn && _drawDebug)
        {
            if (Application.isPlaying && this.enabled)
            {
                Vector3[] temp = _cylinderModel.GetLineArray();

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

                Vector3[] temp2 = _cylinderModel.GetPointArray();

                Gizmos.color = Color.blue;
                for (int i = 0; i < temp2.Length; i += 2)
                {

                    Gizmos.DrawLine(temp2[i], temp2[i + 1]);
                }
            }
        }
    }
}

/// <summary>
/// 圆柱模型
/// </summary>
class CylinderModelNoScale
{
    private Vector3 _pos;
    private Vector3 _up;

    int _smoothness;

    private readonly float _interval;
    private readonly float _height;
    private readonly float _radius;
    private CylinderSection[] _cylinderSectionArray;
    private CylinderSectionBuffer[] _cylinderSectionBufferArray;

    public CylinderModelNoScale(float interval, float radius, float height, int smoothness = 16)
    {
        this._interval = interval;
        this._radius = radius;
        this._height = height;


        _smoothness = smoothness;
    }

    public void TransformTo(Vector3 pos, Vector3 up)
    {
        this._pos = pos;
        this._up = up;
    }

    public void Init()
    {
        int pointCount = (int)(_height / _interval) + 1;
        _cylinderSectionArray = new CylinderSection[pointCount];
        float radius = _radius;
        for (int i = 0; i < pointCount; i++)
        {
            _cylinderSectionArray[i] = new CylinderSection();
            _cylinderSectionArray[i].Point.Add(0);
            _cylinderSectionArray[i].Point.Add(radius);
        }
    }

    public bool CuttingBy(CylinderBurin cylinderBurin)
    {
        _cylinderSectionBufferArray = new CylinderSectionBuffer[_cylinderSectionArray.Length];
        for (int i = 0; i < _cylinderSectionBufferArray.Length; i++)
        {
            _cylinderSectionBufferArray[i] = new CylinderSectionBuffer(GetWorldPos(i), _up);
        }
        bool changed = false;
        MeshBuffer meshBuffer = new MeshBuffer(cylinderBurin.Mesh);
        for (int i = 0; i < meshBuffer.triangles.Count; i += 3)
        {
            Vector3 p1 = MeshHelper.Operator(cylinderBurin.trans, meshBuffer.GetVerticeByTrianglesIndex(i + 0));
            Vector3 p2 = MeshHelper.Operator(cylinderBurin.trans, meshBuffer.GetVerticeByTrianglesIndex(i + 1));
            Vector3 p3 = MeshHelper.Operator(cylinderBurin.trans, meshBuffer.GetVerticeByTrianglesIndex(i + 2));
            changed = changed | CuttingWithLine(p1, p2);
            changed = changed | CuttingWithLine(p1, p3);
            changed = changed | CuttingWithLine(p2, p3);
        }
        if (changed)
        {
             return CalculationBuffer();
            //return CalculationBufferNoCut();
        }
        else
        {
            return false;
        }
    }

    private bool CuttingWithLine(Vector3 linePoint1, Vector3 linePoint2)
    {
        Vector3 footDrop1 = VectorHelper.GetFootDrop(linePoint1, _pos, _pos + _up);
        Vector3 footDrop2 = VectorHelper.GetFootDrop(linePoint2, _pos, _pos + _up);

        float distance1 = Vector3.Distance(footDrop1, _pos);
        float distance2 = Vector3.Distance(footDrop2, _pos);

        int dir1 = Vector3.Dot(footDrop1 - _pos, _up) > 0 ? 1 : -1;
        int dir2 = Vector3.Dot(footDrop2 - _pos, _up) > 0 ? 1 : -1;

        int p1 = Mathf.RoundToInt(distance1 / _interval);
        int p2 = Mathf.RoundToInt(distance2 / _interval);

        p1 = p1 * dir1;
        p2 = p2 * dir2;

        if ((p1 >= _cylinderSectionArray.Length && p2 >= _cylinderSectionArray.Length)
            || (p1 < 0 && p2 < 0))
        {
            return false;
        }

        int pMin = Mathf.Min(p1, p2) + 1;
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

        return true;
    }

    private bool CalculationBuffer()
    {
        foreach (var item in _cylinderSectionBufferArray)
        {
            item.SortOutMin();
        }

        bool changed = false;
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
                        changed = true;
                        _cylinderSectionArray[i].Point.Add(max);
                    }


                    changed = changed || haveSmallest;
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
                        changed = true;
                        _cylinderSectionArray[i].Point.Add(min);

                    }
                    if (haveBiggest && !maxBetween)
                    {
                        changed = true;
                        _cylinderSectionArray[i].Point.Add(max);

                    }

                    changed = changed || haveBetween;
                }
            }
        }

        return changed;
    }

    private bool CalculationBufferNoCut()
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
                            haveSmallest = true;
                        }
                    }
                    if (haveBiggest && haveSmallest)
                    {
                        return true;
                    }
                    if (haveSmallest)
                    {
                        return true;
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
                            haveBetween = true;
                        }
                    }

                    if ((haveBiggest || haveBetween) && haveSmallest && !minBetween)
                    {
                        return true;

                    }
                    if (haveBiggest && !maxBetween)
                    {
                        return true;

                    }

                    if (haveBetween)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private Vector3 GetWorldPos(int index)
    {
        return _pos + _up * index * _interval;
    }

    private Vector3 GetLocalPos(int index)
    {
        return Vector3.up * index * _interval;
    }

    public void ToMesh(Mesh mesh)
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
                            meshBuffer.AddRing(GetLocalPos(i), _cylinderSectionArray[i].Point[k], _cylinderSectionArray[i].Point[k + 1], Vector3.up, _smoothness);
                        }
                        if (i + 1 < _cylinderSectionArray.Length && (_cylinderSectionArray[i + 1].Point.Count <= 0 || _cylinderSectionArray[i].Point[k + 1] > _cylinderSectionArray[i + 1].Point[_cylinderSectionArray[i + 1].Point.Count - 1]))
                        {
                            meshBuffer.AddRing(GetLocalPos(i), _cylinderSectionArray[i].Point[k], _cylinderSectionArray[i].Point[k + 1], -Vector3.up, _smoothness);
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
                                if (NumHelper.IsNear(_cylinderSectionArray[i].Point[j], _cylinderSectionArray[lastIndex].Point[j]))
                                {
                                    meshBuffer.AddRing3D(GetLocalPos(i), _cylinderSectionArray[i].Point[j], GetLocalPos(lastIndex), _cylinderSectionArray[lastIndex].Point[j], j % 2 == 0 ? -Vector3.up : Vector3.up, _smoothness);
                                }
                                else
                                {
                                    if (_cylinderSectionArray[i].Point[j] > _cylinderSectionArray[lastIndex].Point[j])
                                    {
                                        meshBuffer.AddRing3D(GetLocalPos(i), _cylinderSectionArray[i].Point[j], GetLocalPos(i), _cylinderSectionArray[lastIndex].Point[j], -Vector3.up, _smoothness);
                                        meshBuffer.AddRing3D(GetLocalPos(i), _cylinderSectionArray[lastIndex].Point[j], GetLocalPos(lastIndex), _cylinderSectionArray[lastIndex].Point[j], j % 2 == 0 ? -Vector3.up : Vector3.up, _smoothness);
                                    }
                                    else
                                    {
                                        meshBuffer.AddRing3D(GetLocalPos(i), _cylinderSectionArray[i].Point[j], GetLocalPos(i), _cylinderSectionArray[lastIndex].Point[j], Vector3.up, _smoothness);
                                        meshBuffer.AddRing3D(GetLocalPos(i), _cylinderSectionArray[lastIndex].Point[j], GetLocalPos(lastIndex), _cylinderSectionArray[lastIndex].Point[j], j % 2 == 0 ? -Vector3.up : Vector3.up, _smoothness);
                                    }
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
                meshBuffer.AddRing(GetLocalPos((int)first), _cylinderSectionArray[(int)first].Point[i], _cylinderSectionArray[(int)first].Point[i + 1], Vector3.up, _smoothness);
            }
        }
        if (last != null)
        {
            for (int i = 0; i < _cylinderSectionArray[(int)last].Point.Count - 1; i += 2)
            {
                meshBuffer.AddRing(GetLocalPos((int)last), _cylinderSectionArray[(int)last].Point[i], _cylinderSectionArray[(int)last].Point[i + 1], -Vector3.up, _smoothness);
            }
        }

        meshBuffer.Apply(mesh);
    }

    public Vector3[] GetLineArray()
    {
        Vector3[] linePointArray = new Vector3[_cylinderSectionBufferArray.Length * 4];

        Vector3 dir1 = VectorHelper.GetCommonVerticalLine(_up, _up).normalized;
        Vector3 dir2 = VectorHelper.GetCommonVerticalLine(dir1, _up).normalized;

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

        Vector3 dir1 = VectorHelper.GetCommonVerticalLine(_up, _up).normalized;
        Vector3 dir2 = VectorHelper.GetCommonVerticalLine(dir1, _up).normalized;

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

/// <summary>
/// 圆柱切片缓存
/// </summary>
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

    /// <summary>
    /// 判断中心点到边的距离，求出点和边共同的最小值
    /// </summary>
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
        //Debug.Log(_center+"____"+ newPoint);
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

/// <summary>
/// 圆柱切片
/// </summary>
class CylinderSection
{
    public AutoSortList<float> Point;

    public bool[] check;

    public CylinderSection()
    {
        Point = new AutoSortList<float>();
    }
    public void InitCheck()
    {
        check = new bool[Point.Count];
    }
}

/// <summary>
/// 车刀
/// </summary>
class CylinderBurin
{
    public Mesh Mesh { get; private set; }
    public Matrix4x4 trans { get; private set; }

    public CylinderBurin(Matrix4x4 state, Mesh mesh)
    {
        trans = state;
        Mesh = mesh;
    }

    public void TransformTo(Matrix4x4 state)
    {
        trans = state;
    }
}
