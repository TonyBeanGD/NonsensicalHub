using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AutoCuttingObject : CuttingObject
{

    [SerializeField] protected bool autoCheck = true;

    private Thread checkThread;
    private bool checkOver;


    protected override void Awake()
    {
        base.Awake();
        checkOver = true;

        base.OnCheckOver += () => { checkOver = true; };
    }
    
    protected override void Update()
    {
        base.Update();

        if (IsInit&&cutObject.IsInit)
        {
            if (autoCheck && checkOver)
            {
                checkOver = false;
                GetState();
                checkThread?.Abort();
                checkThread = new Thread(() => CheckCutting());
                checkThread.Start();
            }
        }
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();

        checkThread?.Abort();
    }

}
