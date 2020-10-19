//using DG.Tweening;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;
//using XProject.Models;

///// <summary>
///// 可吸附其他对象的对象
///// </summary>
//public class AdsorbableModel : MonoBehaviour
//{
//    private List<string> canAdsorbModelNames;   //可以吸附的模型名称链表
//    private string IOID;        //接受IO信号的ID
//    private List<IOSignalType> IOTypes; //IO信号类型
//    private bool activeState;     //是否可吸附（初始不可吸附的对象在被吸附时激活，初始可吸附的对象在吸附时休眠）

//    private bool isAdsorb;      //是否正在吸附中
//    private GameObject[] FadeModels;
//    private CanMountModel crtCanMountModel;

//    private GameObject[] OpenCloseTarget;
//    private float openCloseDistance;

//    private void Awake()
//    {
//        isAdsorb = false;
//        NotificationCenter.Instance().AttachObsever(ActionType.IOControlSignal, IOControlSignal);
//    }

//    private void Start()
//    {
//        FadeModels = new GameObject[canAdsorbModelNames.Count];

//        for (int index = 0; index < canAdsorbModelNames.Count; index++)
//        {
//            int j = index;
//            string path = Path.Combine(Application.streamingAssetsPath, "builtin.assetbundle");
//            NotificationCenter.Instance().PostDispatch(ActionType.LoadModelFormAB, new Notification(new Tuple<string, string, Action<GameObject>>(canAdsorbModelNames[j], path,
//                (go) =>
//                {
//                    FadeModels[j] = go;

//                    go.transform.SetParent(transform);

//                    go.transform.position = transform.position;
//                    go.transform.rotation = transform.rotation;

//                    foreach (var item in go.GetComponentsInChildren<Transform>())
//                    {
//                        if (item.GetComponent<MeshRenderer>() != null)
//                        {
//                            Material[] temp = item.GetComponent<MeshRenderer>().materials;

//                            for (int i = 0; i < temp.Length; i++)
//                            {
//                                temp[i] = Resources.Load<Material>("Materials/Fade");
//                            }
//                            item.GetComponent<MeshRenderer>().materials = temp;
//                        }
//                        if (item.GetComponent<MeshCollider>() != null)
//                        {
//                            Destroy(item.GetComponent<MeshCollider>());
//                        }
//                    }

//                    go.SetActive(false);

//                }), this));
//        }
//    }

//    private void OnDestroy()
//    {
//        NotificationCenter.Instance().DetachObsever(ActionType.IOControlSignal, IOControlSignal);
//    }

//    private void OnCollisionEnter(Collision collision)
//    {
//        if (collision.gameObject.tag == "ColliderPoint")
//        {
//            if (activeState == true)
//            {
//                if (crtCanMountModel == null)
//                {
//                    if (collision.gameObject.GetComponentInParent<CanMountModel>() != null)
//                    {
//                        int index = collision.gameObject.GetComponentInParent<CanMountModel>().CanGet(canAdsorbModelNames);
//                        if (index != -1)
//                        {
//                            if (collision.gameObject.GetComponentInParent<CanMountModel>().CheckAngle(FadeModels[index].transform) == true)
//                            {
//                                crtCanMountModel = collision.gameObject.GetComponentInParent<CanMountModel>();
//                                ShowFadeModel(crtCanMountModel.offsetPos, crtCanMountModel.offsetRot);
//                            }
//                        }
//                    }
//                }
//            }
//        }
//        else
//        {
//            Debug.Log("碰撞到错误的物体"+ collision.transform.name);
//        }
//    }

//    private void OnCollisionExit(Collision collision)
//    {
//        if (collision.gameObject.tag == "ColliderPoint")
//        {
//            if (activeState == true)
//            {
//                if (crtCanMountModel != null)
//                {
//                    if (collision.gameObject.GetComponentInParent<CanMountModel>() != null)
//                    {
//                        if (collision.gameObject.GetComponentInParent<CanMountModel>() == crtCanMountModel)
//                        {
//                            crtCanMountModel = null;
//                            HideFadeModel();
//                        }
//                    }
//                }
//            }
//        }
//    }

//    public void Init(string IOID, List<IOSignalType> IOTypes, List<string> canAdsorbModelNames, bool initState)
//    {
//        this.IOID = IOID;
//        this.IOTypes = new List<IOSignalType>(IOTypes.ToArray());
//        this.canAdsorbModelNames = new List<string>(canAdsorbModelNames.ToArray());
//        this.activeState = initState;

//        if (initState == true)
//        {
//            Activation();
//        }
//        else
//        {
//            Dormancy();
//        }
//    }

//    public void SetOpenClose(GameObject[] targets, float distance)
//    {
//        this.OpenCloseTarget = targets;
//        this.openCloseDistance = distance;
//    }

//    private void IOControlSignal(Notification noti)
//    {
//        Tuple<string, IOSignalType> tuple = noti.Arguments as Tuple<string, IOSignalType>;

//        if (crtCanMountModel == null || tuple.Item1.Equals(IOID) == false || IOTypes.Contains(tuple.Item2) == false)
//        {
//            return;
//        }

//        switch (tuple.Item2)
//        {
//            case IOSignalType.GripperGet:
//            case IOSignalType.GripperClose:
//            case IOSignalType.PositionerCylinderClamping:
//            case IOSignalType.SuckerSuction:
//                Adsorb();
//                break;
//            case IOSignalType.GripperSet:
//            case IOSignalType.GripperOpen:
//            case IOSignalType.PositionerCylinderRetracting:
//            case IOSignalType.SuckerStop:
//                Release();
//                break;
//        }
//    }

//    private void ShowFadeModel(Vector3 offsetPos, Vector3 offsetRot)
//    {
//        if (crtCanMountModel == null)
//        {
//            return;
//        }

//        int index = -1;
//        for (int i = 0; i < canAdsorbModelNames.Count; i++)
//        {
//            if (crtCanMountModel.CheckName(canAdsorbModelNames[i]) == true)
//            {
//                index = i;
//                break;
//            }
//        }

//        FadeModels[index].SetActive(true);

//        FadeModels[index].transform.SetParent(transform);
//        FadeModels[index].transform.localPosition = offsetPos;
//        FadeModels[index].transform.localEulerAngles = offsetRot;
//    }

//    private void HideFadeModel()
//    {
//        foreach (var item in FadeModels)
//        {
//            item.SetActive(false);
//        }
//    }

//    private void Adsorb()
//    {
//        if (crtCanMountModel == null && activeState == true)
//        {
//            return;
//        }

//        if (crtCanMountModel.CanAdsorbing() == true)
//        {
//            crtCanMountModel.Adsorbing(transform);

//            HideFadeModel();

//            Dormancy();

//            if (crtCanMountModel.GetComponent<AdsorbableModel>() != null)
//            {
//                crtCanMountModel.GetComponent<AdsorbableModel>().Activation();
//            }
//            else
//            {
//                crtCanMountModel.Activation();
//            }

//            if (OpenCloseTarget != null && OpenCloseTarget.Length > 0)
//            {
//                OpenCloseTarget[0].transform.DOMove(OpenCloseTarget[0].transform.position - OpenCloseTarget[0].transform.up * openCloseDistance, 0.3f);
//                OpenCloseTarget[1].transform.DOMove(OpenCloseTarget[1].transform.position + OpenCloseTarget[0].transform.up * openCloseDistance, 0.3f);
//            }
//        }
//    }

//    private void Release()
//    {
//        if (crtCanMountModel == null && activeState == false)
//        {
//            return;
//        }

//        if (crtCanMountModel.CanSetModel == true)
//        {
//            crtCanMountModel.SetModel();
//            crtCanMountModel.Dormancy();

//            Activation();

//            if (crtCanMountModel.GetComponent<AdsorbableModel>() != null)
//            {
//                crtCanMountModel.GetComponent<AdsorbableModel>().Dormancy();
//            }
//            else
//            {
//                crtCanMountModel.Dormancy();
//            }
//            crtCanMountModel = null;
//            if (OpenCloseTarget != null && OpenCloseTarget.Length > 0)
//            {
//                OpenCloseTarget[0].transform.DOMove(OpenCloseTarget[0].transform.position + OpenCloseTarget[0].transform.up * openCloseDistance, 0.3f);
//                OpenCloseTarget[1].transform.DOMove(OpenCloseTarget[1].transform.position - OpenCloseTarget[0].transform.up * openCloseDistance, 0.3f);
//            }
//        }
//    }

//    public void Activation()
//    {
//        activeState = true;
//        if (GetComponent<Rigidbody>() == null)
//        {
//            gameObject.AddComponent<Rigidbody>();
//        }

//        GetComponent<Rigidbody>().isKinematic = true;

//        MeshCollider[] meshColliders = transform.GetComponentsInChildren<MeshCollider>();

//        foreach (var item in meshColliders)
//        {
//            item.convex = true;
//        }
//    }

//    public void Dormancy()
//    {
//        activeState = false;
//        if (GetComponent<Rigidbody>() != null)
//        {
//            Destroy(GetComponent<Rigidbody>());
//        }

//        MeshCollider[] meshColliders = transform.GetComponentsInChildren<MeshCollider>();

//        foreach (var item in meshColliders)
//        {
//            item.convex = false;
//        }
//    }
//}
