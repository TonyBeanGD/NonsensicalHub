//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using XProject.Models;

//public class FixatorModel : MonoBehaviour {

//    public bool IsFixed { get; private set; }

//    private string IOID;
    
//    private ColliderMountPoint FixedTarget;
//    private Action OnClamp;
//    private Action OnRelease;
    
//    private void Awake()
//    {
//        NotificationCenter.Instance().AttachObsever(ActionType.IOControlSignal, IOControlSignal);
//    }

//    private void OnDestroy()
//    {
//        NotificationCenter.Instance().DetachObsever(ActionType.IOControlSignal, IOControlSignal);
//    }

//    public void Init(string _IOID, ColliderMountPoint _fixedTarget,bool _initFixed,Action _onClamp, Action _onRelease)
//    {
//        IOID = _IOID;
//        FixedTarget = _fixedTarget;
//        IsFixed = _initFixed;
//        OnClamp = _onClamp;
//        OnRelease = _onRelease;
//    }

//    private void IOControlSignal(Notification noti)
//    {
//        Tuple<string, IOSignalType> tuple = noti.Arguments as Tuple<string, IOSignalType>;

//        if (tuple.Item1.Equals(IOID) == false)
//        {
//            return;
//        }

//        switch (tuple.Item2)
//        {
//            case IOSignalType.PositionerCylinderClamping:
//                {
//                    if (IsFixed == false)
//                    {
//                        IsFixed = true;

//                        FixedTarget.Clamp();

//                        OnClamp();
//                    }
//                }
//                break;
//            case IOSignalType.PositionerCylinderRetracting:
//                {
//                    if (IsFixed == true)
//                    {

//                        IsFixed = false;

//                        FixedTarget.Release();

//                        OnRelease();
//                    }
//                }
//                break;
//        }
//    }
//}
