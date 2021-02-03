using NonsensicalKit.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class OctreeModelTest : MonoBehaviour
{
    /// <summary>
    /// 是否使用线程
    /// </summary>
    [SerializeField] protected bool UseThread = true;
    [SerializeField] private OctreeKnifeTest OctreeKnifeTest;
    [SerializeField] private int level;

    private bool InitComplete;
    private OctreeModelPiece octreeModelPiece;

    private Thread initThread;
    private Thread cutThread;
    private Thread renderThread;

    private bool cutting;
    private bool needRender;
    private bool rendering;
    private bool needApply;
    private MeshBuffer meshBuffer;
    private void Awake()
    {
        octreeModelPiece = new OctreeModelPiece(transform, GetComponent<MeshFilter>().mesh);
    }

    private void Start()
    {
        if (UseThread)
        {
            initThread = new Thread(() =>
            {
                octreeModelPiece.InitNode(level);
                InitComplete = true;
            });
            initThread.Start();
        }
        else
        {
            octreeModelPiece.InitNode(level);
            InitComplete = true;
        }
    }

    private void Update()
    {

        octreeModelPiece.UpdateState(transform);
        if (InitComplete && OctreeKnifeTest.InitComplete)
        {
            if (UseThread)
            {
                if (cutting == false)
                {
                    cutting = true;
                    cutThread = new Thread(() =>
                    {
                        if (octreeModelPiece.CuttingBy(OctreeKnifeTest.octreeModelKnife))
                        {
                            needRender = true;
                        }
                        cutting = false;
                    });
                    cutThread.Start();
                }

                if (needRender && rendering == false)
                {
                    needRender = false;
                    rendering = true;
                    renderThread = new Thread(() =>
                    {
                        meshBuffer = octreeModelPiece.DrawMesh();
                        rendering = false; needApply = true;
                    });
                    renderThread.Start();
                }

                if (needApply)
                {
                    needApply = false;
                    GetComponent<MeshFilter>().mesh = meshBuffer.GetMesh();
                }
            }
            else
            {
                if (octreeModelPiece.CuttingBy(OctreeKnifeTest.octreeModelKnife))
                {
                    GetComponent<MeshFilter>().mesh = octreeModelPiece.DrawMesh().GetMesh();
                }
            }
        }

    }

    private void OnDestroy()
    {
        if (initThread != null)
        {
            initThread.Abort();
            initThread = null;
        }
        if (cutThread != null)
        {
            cutThread.Abort();
            cutThread = null;
        }
        if (renderThread != null)
        {
            renderThread.Abort();
            renderThread = null;
        }
    }
}
