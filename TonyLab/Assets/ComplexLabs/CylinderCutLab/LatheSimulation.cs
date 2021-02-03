using NonsensicalKit;
using NonsensicalKit.Utility;
using NonsensicalKit.Custom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AxisDir
{
    XDir,
    YDir,
    ZDir,
}

public class LatheSimulation : MonoBehaviour
{
    public AxisDir _heightDir = AxisDir.YDir;        //高度方向（相对于模型mesh）
    public AxisDir _radiusDir = AxisDir.XDir;           //半径方向（相对于模型mesh）
    public float _sectionThickness = 0.1f;     //切面厚度

    [SerializeField] private bool _isOnInit;   //初始是否开启切削

    [SerializeField] private Transform _knife;                   //刀具对象

    [SerializeField] private bool _drawDebug;                   //绘制debug线条

    private Mesh _mesh;
    private bool _isOn;

    private CylinderBurin _cylinderBurin;
    private CylinderModel _cylinderModel;

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
                _cylinderModel.ToMesh(_mesh);
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

        float radius = 0;
        float height = 0;

        switch (_radiusDir)
        {
            case AxisDir.XDir:
                radius = length.x / 2;
                break;
            case AxisDir.YDir:
                radius = length.y / 2;
                break;
            case AxisDir.ZDir:
                radius = length.z / 2;
                break;
        }

        switch (_heightDir)
        {
            case AxisDir.XDir:
                height = length.x;
                break;
            case AxisDir.YDir:
                height = length.y;
                break;
            case AxisDir.ZDir:
                height = length.z;
                break;
        }
        _cylinderModel = new CylinderModel(_sectionThickness, radius, height);
        UpdateCylinderModel();
        _cylinderModel.Init();
    }

    private void Update()
    {
        if (_isOn && _cylinderBurin != null && _cylinderModel != null)
        {
            _cylinderBurin.TransformTo(_knife.localToWorldMatrix);

            UpdateCylinderModel();
            if (_cylinderModel.CuttingBy(_cylinderBurin))
            {
                _cylinderModel.ToMesh(_mesh);
            }
        }
    }

    private void UpdateCylinderModel()
    {
        float radius = 0;
        float height = 0;

        switch (_radiusDir)
        {
            case AxisDir.XDir:
                radius = transform.lossyScale.x;
                break;
            case AxisDir.YDir:
                radius = transform.lossyScale.y;
                break;
            case AxisDir.ZDir:
                radius = transform.lossyScale.z;
                break;
        }

        switch (_heightDir)
        {
            case AxisDir.XDir:
                height = transform.lossyScale.x;
                break;
            case AxisDir.YDir:
                height = transform.lossyScale.y;
                break;
            case AxisDir.ZDir:
                height = transform.lossyScale.z;
                break;
        }
        _cylinderModel.TransformTo(transform.position, transform.up, radius, height);

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
class CylinderModel
{
    private Vector3 _pos;
    private Vector3 _up;
    private float _radiusScale;
    private float _heightScale;

    int _smoothness;

    private readonly float _interval;
    private readonly float _height;
    private readonly float _radius;
    private CylinderSection[] _cylinderSectionArray;
    private CylinderSectionBuffer[] _cylinderSectionBufferArray;

    public CylinderModel(float interval, float radius, float height, int smoothness = 16)
    {
        this._interval = interval;
        this._radius = radius;
        this._height = height;


        _smoothness = smoothness;
    }

    public void TransformTo(Vector3 pos, Vector3 up, float radiusScale, float heightScale)
    {
        this._pos = pos;
        this._up = up;
        _radiusScale = radiusScale;
        _heightScale = heightScale;
    }

    public void Init()
    {
        int pointCount = (int)(_height * _heightScale / _interval) + 1;
        _cylinderSectionArray = new CylinderSection[pointCount];
        float radius = _radius * _radiusScale;
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
        for (int i = 0; i < cylinderBurin.Mesh.triangles.Length; i += 3)
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

        int pMin = Mathf.Min(p1, p2)+1;
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
                        _cylinderSectionArray[i].Point.Add(max);
                    }
                    changed = changed | haveSmallest;
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
                    changed = changed | (haveSmallest || haveBetween);
                }
            }
        }
        return changed;
    }

    private Vector3 GetWorldPos(int index)
    {
        return _pos + _up * index * _interval ;
    }

    private Vector3 GetLocalPos(int index)
    {
        return Vector3.up * index * _interval / _heightScale;
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
                            meshBuffer.AddRing(GetLocalPos(i), _cylinderSectionArray[i].Point[k] / _radiusScale, _cylinderSectionArray[i].Point[k + 1] / _radiusScale, Vector3.up, _smoothness);
                        }
                    }
                    for (int k = 0; k < _cylinderSectionArray[i].Point.Count - 1; k += 2)
                    {
                        if (i + 1 < _cylinderSectionArray.Length && (_cylinderSectionArray[i + 1].Point.Count <= 0 || _cylinderSectionArray[i].Point[k + 1] > _cylinderSectionArray[i + 1].Point[_cylinderSectionArray[i + 1].Point.Count - 1]))
                        {
                            meshBuffer.AddRing(GetLocalPos(i), _cylinderSectionArray[i].Point[k] / _radiusScale, _cylinderSectionArray[i].Point[k + 1] / _radiusScale, -Vector3.up, _smoothness);
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
                                    meshBuffer.AddRing3D(GetLocalPos(i), _cylinderSectionArray[i].Point[j] / _radiusScale, GetLocalPos(lastIndex), _cylinderSectionArray[lastIndex].Point[j] / _radiusScale, j % 2 == 0 ? -Vector3.up : Vector3.up, _smoothness);
                                }
                                else
                                {
                                    if (_cylinderSectionArray[i].Point[j] > _cylinderSectionArray[lastIndex].Point[j])
                                    {
                                        meshBuffer.AddRing3D(GetLocalPos(i), _cylinderSectionArray[i].Point[j] / _radiusScale, GetLocalPos(i), _cylinderSectionArray[lastIndex].Point[j] / _radiusScale, -Vector3.up, _smoothness);
                                        meshBuffer.AddRing3D(GetLocalPos(i), _cylinderSectionArray[lastIndex].Point[j] / _radiusScale, GetLocalPos(lastIndex), _cylinderSectionArray[lastIndex].Point[j] / _radiusScale, j % 2 == 0 ? -Vector3.up : Vector3.up, _smoothness);
                                    }
                                    else
                                    {
                                        meshBuffer.AddRing3D(GetLocalPos(i), _cylinderSectionArray[i].Point[j] / _radiusScale, GetLocalPos(i), _cylinderSectionArray[lastIndex].Point[j] / _radiusScale, Vector3.up, _smoothness);
                                        meshBuffer.AddRing3D(GetLocalPos(i), _cylinderSectionArray[lastIndex].Point[j] / _radiusScale, GetLocalPos(lastIndex), _cylinderSectionArray[lastIndex].Point[j] / _radiusScale, j % 2 == 0 ? -Vector3.up : Vector3.up, _smoothness);
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
                meshBuffer.AddRing(GetLocalPos((int)first), _cylinderSectionArray[(int)first].Point[i] / _radiusScale, _cylinderSectionArray[(int)first].Point[i + 1] / _radiusScale, Vector3.up, _smoothness);
            }
        }
        if (last != null)
        {
            for (int i = 0; i < _cylinderSectionArray[(int)last].Point.Count - 1; i += 2)
            {
                meshBuffer.AddRing(GetLocalPos((int)last), _cylinderSectionArray[(int)last].Point[i] / _radiusScale, _cylinderSectionArray[(int)last].Point[i + 1] / _radiusScale, -Vector3.up, _smoothness);
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
