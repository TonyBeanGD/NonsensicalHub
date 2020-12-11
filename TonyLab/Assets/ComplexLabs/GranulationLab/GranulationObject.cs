using NonsensicalKit;
using NonsensicalKit.Custom;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// 可以使用chunk进行运算的优化
/// </summary>
public abstract class GranulationObject : MonoBehaviour
{
    /// <summary>
    /// 粒子化等级
    /// </summary>
    [SerializeField] protected int level = -1;
    [SerializeField] protected bool quickInit = true;

    [SerializeField] protected Transform Point1;

    public delegate void InitCompleteHandle();

    public event InitCompleteHandle OnInitComplete;

    /// <summary>
    /// 粒子化对象
    /// </summary>
    public Granulation granulation;

    [HideInInspector] public bool IsInit;

    /// <summary>
    /// 初始化线程
    /// </summary>
    private Thread initThread;

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(granulation.point + granulation.originOffset, granulation.point + granulation.originOffset + granulation._right);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(granulation.point + granulation.originOffset, granulation.point + granulation.originOffset + granulation._up);


        Gizmos.color = Color.blue;
        Gizmos.DrawLine(granulation.point + granulation.originOffset, granulation.point + granulation.originOffset + granulation._forward);
    }

    protected virtual void Awake()
    {
        IsInit = false;
    }

    protected virtual void Start()
    {
        if (GetComponent<MeshFilter>().mesh != null)
        {
            Vector3 pos = transform.position;
            Vector3 right = transform.right;
            Vector3 up = transform.up;
            Vector3 forward = transform.forward;
            Mesh mesh = GetComponent<MeshFilter>().mesh;
            Bounds bounds = mesh.bounds;
            Point1.localPosition = mesh.bounds.min;
            MeshBuffer meshBuffer = new MeshBuffer(mesh);
            initThread = new Thread(() => Init(pos, right, up, forward, meshBuffer, bounds));
            initThread.Start();
        }
        else
        {
            Debug.LogWarning(name + "没有挂载MeshFilter组件");
        }
    }

    protected virtual void Update()
    {
        GetState();
    }

    protected virtual void OnDestroy()
    {
        initThread?.Abort();
    }

    protected void Init(Vector3 pos, Vector3 right, Vector3 up, Vector3 forward, MeshBuffer meshBuffer, Bounds bounds)
    {
        Debug.Log("开始初始化：" + System.DateTime.Now.ToLongTimeString());
        granulation = new Granulation(level, pos, right, up, forward, meshBuffer, bounds, quickInit);
        IsInit = true;
        Debug.Log("结束初始化：" + System.DateTime.Now.ToLongTimeString());
        OnInitComplete?.Invoke();
    }

    protected void GetState()
    {
        granulation.point = transform.position;
        granulation._forward = transform.forward;
        granulation._up = transform.up;
        granulation._right = transform.right;

        granulation.originOffset = Point1.position- transform.position;
        granulation.origin = Point1.localPosition;
    }

    public struct Granulation
    {
        /// <summary>
        /// 承载者的世界坐标
        /// </summary>
        public Vector3 point;

        public Vector3 _right;

        public Vector3 _up;

        public Vector3 _forward;

        public int Level;
        public float step;
        public Bool3Array points;
        public int length0;
        public int length1;
        public int length2;
        /// <summary>
        ///  (0,0,0)位置的世界相对坐标
        /// </summary>
        public Vector3 originOffset;
        /// <summary>
        /// (0,0,0)位置的本地坐标
        /// </summary>
        public Vector3 origin;

        /// <summary>
        /// TODO:可以使用DEXEL模型进行初始化的优化(射线组)
        /// </summary>
        public Granulation(int level, Vector3 pos, Vector3 right, Vector3 up, Vector3 forward, MeshBuffer mesh, Bounds meshBounds, bool quickInit)
        {
            this.Level = level;
            step = Mathf.Pow(2, this.Level);

            this.point = pos;
            _right = right;
            _up = up;
            _forward = forward;

            origin = meshBounds.min;
            originOffset = meshBounds.min;

            int x = ((meshBounds.max.x - meshBounds.min.x) / step).RoundingToInt_Add();
            int y = ((meshBounds.max.y - meshBounds.min.y) / step).RoundingToInt_Add();
            int z = ((meshBounds.max.z - meshBounds.min.z) / step).RoundingToInt_Add();

            points = new Bool3Array(x, y, z);
            length0 = x;
            length1 = y;
            length2 = z;

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    for (int k = 0; k < z; k++)
                    {
                        //Vector3 offset = ((i + 0.5f) * right + (j + 0.5f) * up + (k + 0.5f) * forward) * step;
                        Vector3 offset = new Vector3((i + 0.5f), (j + 0.5f), (k + 0.5f)) * step;
                        if (quickInit)
                        {
                            points[i, j, k] = mesh.Contain_2(origin + offset);
                        }
                        else
                        {
                            points[i, j, k] = mesh.Contain(origin + offset);
                        }
                    }
                }
            }
        }
    }
}
