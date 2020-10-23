//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;

///// <summary>
///// 鼠标放置模型点
///// </summary>
//public class MouseMountPoint : MountPointBase
//{
//    private GameObject fadeModel;

//    private Action<string> OnSet;

//    private bool isSetted;

//    private void Awake()
//    {
//        mountPointType = MountPointType.MouseMountPoint;

//        isSetted = false;

//        NotificationCenter.Instance().AttachObsever(ActionType.MountPointShowHighlight, ShowHighlight);
//        NotificationCenter.Instance().AttachObsever(ActionType.MountPointCloseHighlight, CloseHighlight);
//    }

//    private void Start()
//    {
//        string path = Path.Combine(Application.streamingAssetsPath, "builtin.assetbundle");
//        NotificationCenter.Instance().PostDispatch(ActionType.LoadModelFormAB, new Notification(new Tuple<string, string, Action<GameObject>>(modelName, path,
//           (go) =>
//           {
//               fadeModel = go;

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
//               }
//               fadeModel.SetActive(false);
//           }), this));
//    }

//    private void OnDestroy()
//    {
//        NotificationCenter.Instance().DetachObsever(ActionType.MountPointShowHighlight, ShowHighlight);
//        NotificationCenter.Instance().DetachObsever(ActionType.MountPointCloseHighlight, CloseHighlight);
//    }

//    public void AddListener(Action<string> onSet)
//    {
//        OnSet += onSet;
//    }

//    public void ShowModel()
//    {
//        if (isSetted == false)
//        {
//            fadeModel.gameObject.SetActive(true);
//        }
//    }

//    public void HideModel()
//    {
//        if (isSetted == false)
//        {
//            fadeModel.gameObject.SetActive(false);
//        }
//    }

//    public bool SetModel(GameObject model)
//    {
//        if (isSetted == false)
//        {
//            OnSet?.Invoke(modelName);
//            isSetted = true;
//            fadeModel.gameObject.SetActive(false);

//            model.transform.SetParent(transform);

//            model.transform.position = transform.position;
//            model.transform.rotation = transform.rotation;
//            return true;
//        }
//        else
//        {
//            return false;
//        }
//    }

//    public void GetModel()
//    {
//        isSetted = false;
//        fadeModel.gameObject.SetActive(false);
//    }

//    protected override void ShowHighlight(Notification noti)
//    {
//        if (CheckNoti(noti) == false)
//        {
//            return;
//        }

//        if (isSetted == false)
//        {
//            transform.Find("HighlightBox").GetComponent<SceneObjHighlightController>().ConstantOn(Color.green);
//        }
//    }

//    protected override void CloseHighlight(Notification noti)
//    {
//        if (CheckNoti(noti) == false)
//        {
//            return;
//        }

//        transform.Find("HighlightBox").GetComponent<SceneObjHighlightController>().CloseHighlighting2();
//    }
//}
