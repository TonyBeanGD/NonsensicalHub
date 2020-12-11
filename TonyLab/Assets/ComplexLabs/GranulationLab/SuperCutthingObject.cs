
using NonsensicalKit;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

public sealed class SuperCutthingObject : CuttingObject
{
    private const int LENGTH = 200  ;
    private Vector3[] paths;
    private Thread calculationThread;
    
    private List<MeshBuffer> meshBuffers = new List<MeshBuffer>();

    private int index;

    private bool isRunned=false;

    protected override void Awake()
    {
        base.Awake();
        index = LENGTH;
        InitData();
    }

    protected override void Update()
    {
        base.Update();
        
        if (cutObject != null && cutObject.gameObject.activeSelf == true)
        {
            if (IsInit && cutObject.IsInit&&!isRunned)
            {
                isRunned = true;
                   calculationThread = new Thread(Calculation);
                calculationThread.Start();
            }
        }
        
        if (index< paths.Length)
        {
            if (meshBuffers[index]!=null)
            {
                cutObject.Refresh(meshBuffers[index]);
            }
            index++;
        }

        transform.position = granulation.point;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        calculationThread?.Abort();
    }

    private void InitData()
    {
        paths = new Vector3[LENGTH];
        for (int i = 0; i < LENGTH; i++)
        {
            paths[i] = (new Vector3(i * 0.01f, 0, 0));
        }
    }

    public void Calculation()
    {
        for (int i = 0; i < paths.Length; i++)
        {
            granulation.point = paths[i];
            
            Debug.Log("第" + i + "次碰撞计算开始：" + System.DateTime.Now.ToLongTimeString());
            bool needRefresh = CheckCutting();
            Debug.Log("第" + i + "次碰撞计算结束：" + System.DateTime.Now.ToLongTimeString());
            if (needRefresh)
            {
                meshBuffers.Add(cutObject.CalculationMesh());
            }
            else
            {
                meshBuffers.Add(null);
            }
            Debug.Log("第" + i + "次Mesh计算结束：" + System.DateTime.Now.ToLongTimeString());

        }

        index = 0;

        Debug.Log("开始存档：" + System.DateTime.Now.ToLongTimeString());

        StringBuilder sb = new StringBuilder();

        foreach (var item in meshBuffers)
        {
            sb.Append(item.ToData());
        }
        string text = sb.ToString();

        Debug.Log("转换结束：" + System.DateTime.Now.ToLongTimeString());
        FileHelper.WriteTxt(Path.Combine( Application.streamingAssetsPath,"Save.text"), text);

        Debug.Log("存档结束存档：" + System.DateTime.Now.ToLongTimeString());
    }
}
