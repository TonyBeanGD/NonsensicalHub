//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;

///// <summary>
///// 碰撞放置点
///// </summary>
//public class ColliderMountPoint : MountPointBase
//{
//    [SerializeField]
//    private AreaObject areaObject;

//    [SerializeField]
//    private FixatorModel[] fixeds;

//    [SerializeField]
//    private int checkDir;

//    public bool CanSetModel
//    {
//        get
//        {
//            return !IsSetted;
//        }
//    }

//    public bool IsFixed { get; private set; }   //是否被固定住，如变位机夹住物料

//    private ColliderMountPointGroup dependentGroup;   //所属的对象组

//    private GameObject fadeModel;

//    private Action onSet;

//    public bool IsSetted { get; private set; }

//    public bool CanGetModel
//    {
//        get
//        {

//            return !IsFixed;
//        }
//    }

//    private void Awake()
//    {
//        mountPointType = MountPointType.ColiderMountPoint;
//        IsSetted = false;
//        IsFixed = false;
//        NotificationCenter.Instance().AttachObsever(ActionType.MountPointShowHighlight, ShowHighlight);
//        NotificationCenter.Instance().AttachObsever(ActionType.MountPointCloseHighlight, CloseHighlight);
//    }

//    private void Start()
//    {
//        if (modelName != null && modelName != string.Empty)
//        {
//            CreateFadeModel();
//        }
//    }

//    private void OnDestroy()
//    {
//        NotificationCenter.Instance().DetachObsever(ActionType.MountPointShowHighlight, ShowHighlight);
//        NotificationCenter.Instance().DetachObsever(ActionType.MountPointCloseHighlight, CloseHighlight);
//    }

//    public void AddOnSetListener(Action action)
//    {
//        onSet += action;
//    }

//    public void Init(string _modelName, int _checkDir, FixatorModel[] fms, AreaObject areaObject = null)
//    {
//        modelName = _modelName;
//        checkDir = _checkDir;
//        fixeds = fms;

//        CreateFadeModel();

//        if (areaObject != null)
//        {
//            if (transform.GetComponentInParent<ColliderMountPointGroup>() != null)
//            {
//                transform.GetComponentInParent<ColliderMountPointGroup>().JoinGroup(this);
//            }
//        }
//    }

//    /// <summary>
//    /// 被夹住
//    /// </summary>
//    public void Clamp()
//    {
//        IsFixed = true;
//    }

//    /// <summary>
//    /// 被松开
//    /// </summary>
//    public void Release()
//    {
//        IsFixed = false;
//    }

//    /// <summary>
//    /// 显示褪色模型
//    /// </summary>
//    public void ShowModel()
//    {
//        if (IsSetted == false)
//        {
//            fadeModel.gameObject.SetActive(true);
//        }
//    }

//    /// <summary>
//    /// 隐藏褪色模型
//    /// </summary>
//    public void HideModel()
//    {
//        if (IsSetted == false)
//        {
//            fadeModel.gameObject.SetActive(false);
//        }
//    }

//    /// <summary>
//    /// 被放置模型
//    /// </summary>
//    /// <param name="model"></param>
//    public void SetModel(GameObject model)
//    {
//        onSet?.Invoke();
//        IsSetted = true;
//        if (fadeModel != null)
//        {
//            fadeModel.SetActive(false);
//        }

//        model.transform.SetParent(transform);
//        model.transform.position = transform.position;
//        model.transform.rotation = transform.rotation;

//        if (dependentGroup != null)
//        {
//            dependentGroup.SetAreaObject(areaObject);
//        }

//    }

//    /// <summary>
//    /// 被获取模型
//    /// </summary>
//    public void GetModel()
//    {
//        if (IsSetted == true)
//        {
//            IsSetted = false;
//            fadeModel.gameObject.SetActive(false);

//            if (dependentGroup != null)
//            {
//                dependentGroup.GetAreaObject(areaObject);
//            }
//        }
//    }

//    /// <summary>
//    /// 设置所属组
//    /// </summary>
//    /// <param name="group"></param>
//    public void SetGroup(ColliderMountPointGroup group)
//    {
//        dependentGroup = group;
//    }

//    /// <summary>
//    /// 显示高光
//    /// </summary>
//    /// <param name="noti"></param>
//    protected override void ShowHighlight(Notification noti)
//    {
//        if (CheckNoti(noti) == false)
//        {
//            return;
//        }

//        if (IsSetted == false)
//        {
//            transform.Find("HighlightBox").GetComponent<SceneObjHighlightController>().ConstantOn(Color.green);
//        }
//    }

//    /// <summary>
//    /// 关闭高光
//    /// </summary>
//    /// <param name="noti"></param>
//    protected override void CloseHighlight(Notification noti)
//    {
//        if (CheckNoti(noti) == false)
//        {
//            return;
//        }

//        transform.Find("HighlightBox").GetComponent<SceneObjHighlightController>().CloseHighlighting2();
//    }

//    /// <summary>
//    /// 检测是否能够放置模型
//    /// </summary>
//    /// <param name="name"></param>
//    /// <returns></returns>
//    public bool CheckNameAndState(string name, Transform tsf)
//    {
//        if (IsFixed)
//        {
//            return false;
//        }

//        if (name.Equals(modelName) == false)
//        {
//            return false;
//        }

//        if (dependentGroup != null && dependentGroup.GroupCheck(areaObject) == false)
//        {
//            return false;
//        }

//        Quaternion q1 = Quaternion.LookRotation(tsf.forward, tsf.up);
//        Quaternion q2 = Quaternion.FromToRotation(-tsf.forward, tsf.up);
//        Quaternion q3 = Quaternion.FromToRotation(tsf.right, tsf.up);
//        Quaternion q4 = Quaternion.FromToRotation(-tsf.right, tsf.up);

//        Quaternion q5 = fadeModel.transform.rotation;

//        switch (checkDir)
//        {
//            case 4:
//                {
//                    if (Quaternion.Angle(q3, q5) < 30)
//                    {
//                        return true;
//                    }
//                    if (Quaternion.Angle(q4, q5) < 30)
//                    {
//                        return true;
//                    }
//                    goto case 2;
//                }
//            case 2:
//                {
//                    if (Quaternion.Angle(q2, q5) < 30)
//                    {
//                        return true;
//                    }
//                    goto case 1;
//                }
//            case 1:
//                {
//                    if (Quaternion.Angle(q1, q5) < 30)
//                    {
//                        return true;
//                    }
//                    return false;
//                }
//            default:
//                return true;
//        }

//    }

//    private void CreateFadeModel()
//    {
//        string path = Path.Combine(Application.streamingAssetsPath, "builtin.assetbundle");
//        NotificationCenter.Instance().PostDispatch(ActionType.LoadModelFormAB, new Notification(new Tuple<string, string, Action<GameObject>>(modelName, path,
//           (go) =>
//           {
//               fadeModel = go;
//               fadeModel.name = "fadeModel";
//               fadeModel.transform.SetParent(transform);

//               fadeModel.transform.position = transform.position;
//               fadeModel.transform.rotation = transform.rotation;

//               foreach (var item in fadeModel.GetComponentsInChildren<Transform>())
//               {
//                   if (item.GetComponent<MeshRenderer>() != null)
//                   {
//                       Material[] temp = item.GetComponent<MeshRenderer>().materials;

//                       for (int i = 0; i < temp.Length; i++)
//                       {
//                           temp[i] = Resources.Load<Material>("Materials/Fade");
//                       }
//                       item.GetComponent<MeshRenderer>().materials = temp;
//                   }
//                   if (item.GetComponent<MeshCollider>() != null)
//                   {
//                       Destroy(item.GetComponent<MeshCollider>());
//                   }
//               }

//               Transform[] nodes = transform.GetComponentsInChildren<Transform>();

//               foreach (var item in nodes)
//               {
//                   item.gameObject.tag = "ColliderPoint";
//               }

//               fadeModel.SetActive(false);
//           }), this));
//    }
//}
