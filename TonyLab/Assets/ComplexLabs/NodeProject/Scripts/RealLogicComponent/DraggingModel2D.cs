//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// 鼠标拖拽2d模型
///// </summary>
//public class DraggingModel2D : MonoBehaviour
//{
//    private string ModelName;
//    private BackpackItemInfo buffer;

//    private Transform LastTarget;

//    private void Awake()
//    {
//        NotificationCenter.Instance().AttachObsever(ActionType.SetDragModel2DtoBackpack, SetDragModel2DtoBackpack);
//    }

//    private void Update()
//    {
//        if (GameManager.instance.crtMouseState == MouseLeftState.Set2DModel)
//        {
//            Camera oc = Camera.main.transform.Find("OrthographicCamera").GetComponent<Camera>();

//            transform.position = oc.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 10));

//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//            RaycastHit hitInfos;

//            if (Physics.Raycast(ray, out hitInfos, 100, LayerMask.GetMask("MouseMountTarget")))
//            {
//                if (hitInfos.transform.GetComponent<MouseMountPoint>().CheckModelName(ModelName))
//                {
//                    if (LastTarget == null)
//                    {
//                        LastTarget = hitInfos.transform;
//                        hitInfos.transform.GetComponent<MouseMountPoint>().ShowModel();
//                    }
//                    else if (LastTarget != hitInfos.transform)
//                    {
//                        LastTarget.GetComponent<MouseMountPoint>().HideModel();
//                        LastTarget = hitInfos.transform;
//                        hitInfos.transform.GetComponent<MouseMountPoint>().ShowModel();
//                    }
//                    else
//                    {
//                        hitInfos.transform.GetComponent<MouseMountPoint>().ShowModel();
//                    }

//                    if (Input.GetMouseButtonDown(0))
//                    {
//                        SetModel(LastTarget.GetComponent<MouseMountPoint>());
//                    }
//                }
//            }
//            else
//            {
//                if (LastTarget != null)
//                {
//                    LastTarget.GetComponent<MouseMountPoint>().HideModel();
//                }
//            }

//            if (Input.GetMouseButtonDown(1))
//            {
//                ReturnToBackpack();
//            }
//        }
//    }

//    public void OnDestroy()
//    {
//        NotificationCenter.Instance().DetachObsever(ActionType.SetDragModel2DtoBackpack, SetDragModel2DtoBackpack);
//        GameManager.instance.crtMouseState = MouseLeftState.SingleClick;
//        NotificationCenter.Instance().PostDispatch(ActionType.MountPointCloseHighlight, new Notification(new Tuple<MountPointType, string>(MountPointType.MouseMountPoint, ModelName), this));
//    }

//    private void SetDragModel2DtoBackpack(Notification noti)
//    {
//        NotificationCenter.Instance().PostDispatch(ActionType.SetItemToBackpack, new Notification(buffer, this));

//        Destroy(gameObject);
//    }

//    public void Init(string _modelName, BackpackItemInfo _buffer)
//    {
//        this.ModelName = _modelName;
//        this.buffer = _buffer;

//        foreach (var item in transform.GetComponentsInChildren<Transform>())
//        {
//            item.gameObject.layer = LayerMask.NameToLayer("2DObject");
//        }
//        transform.localScale *= 0.2f;

//        GameManager.instance.crtMouseState = MouseLeftState.Set2DModel;

//        NotificationCenter.Instance().PostDispatch(ActionType.MountPointShowHighlight, new Notification(new Tuple<MountPointType, string>(MountPointType.MouseMountPoint, ModelName), this));
//    }

//    private void SetModel(MouseMountPoint mouseMountPoint)
//    {
//        if (LastTarget.GetComponent<MouseMountPoint>().SetModel(gameObject) == true)
//        {
//            foreach (var item in transform.GetComponentsInChildren<Transform>())
//            {
//                item.gameObject.layer = LayerMask.NameToLayer("Default");
//            }

//            transform.localScale *= 5f;
            
//            Destroy(this);
//        }
//    }

//    private void ReturnToBackpack()
//    {
//        NotificationCenter.Instance().PostDispatch(ActionType.SetItemToBackpack, new Notification(buffer, this));

//        Destroy(gameObject);
//    }
//}
