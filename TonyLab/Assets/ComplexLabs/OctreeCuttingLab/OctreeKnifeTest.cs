using NonsensicalKit.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class OctreeKnifeTest : MonoBehaviour
{
    /// <summary>
    /// 是否使用线程
    /// </summary>
    [SerializeField] protected bool UseThread = true;
    [SerializeField] private int level;
    [SerializeField] private bool draw;
     private bool drawOver=false;

    [HideInInspector] public bool InitComplete;
    public OctreeModelKnife octreeModelKnife;

    private Thread InitThread;
    

    private void Awake()
    {
        InitComplete = false;
        octreeModelKnife = new OctreeModelKnife(transform, GetComponent<MeshFilter>().mesh);
    }

    private void Start()
    {
        if (UseThread)
        {
            NonsensicalDebugger.LogOnThread("刀具初始化开始");
            InitThread = new Thread(() =>
            {
                octreeModelKnife.InitNode(level);
                NonsensicalDebugger.LogOnThread("刀具初始化完成");
                InitComplete = true;
            });
            InitThread.Start();
        }
        else
        {
            octreeModelKnife.InitNode(level);
            InitComplete = true;
        }
    }

    private void Update()
    {
        if (draw&&!drawOver&& InitComplete)
        {
            drawOver = true;
            GetComponent<MeshFilter>().mesh = octreeModelKnife.DrawMesh().GetMesh();
            Debug.Log("draw");
        }
        octreeModelKnife.UpdateState(transform);
    }


    private void OnDestroy()
    {
        if (InitThread != null)
        {
            InitThread.Abort();
            InitThread = null;
        }
    }
}
