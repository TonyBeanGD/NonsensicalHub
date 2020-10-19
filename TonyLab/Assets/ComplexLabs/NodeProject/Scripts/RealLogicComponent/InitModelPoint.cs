//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;

///// <summary>
///// 初始模型放置点
///// </summary>
//public class InitModelPoint : MonoBehaviour
//{
//    [SerializeField]
//    private string modelName;

//    [SerializeField]
//    private bool isCanMountModel;

//    [SerializeField]
//    private Vector3 offsetPos;

//    [SerializeField]
//    private Vector3 offsetRot;

//    [SerializeField]
//    private ColliderMountPoint colliderMountPoint;

//    [SerializeField]
//    private int checkDir ;

//    private void Start()
//    {
//        string path = Path.Combine(Application.streamingAssetsPath, "builtin.assetbundle");
//        NotificationCenter.Instance().PostDispatch(ActionType.LoadModelFormAB, new Notification(new Tuple<string, string, Action<GameObject>>(modelName, path,
//            (go) =>
//            {
//                go.transform.SetParent(transform);

//                go.transform.position = transform.position;
//                go.transform.rotation = transform.rotation;

//                if (isCanMountModel)
//                {
//                    go.AddComponent<CanMountModel>();
//                    go.GetComponent<CanMountModel>().Init(modelName, offsetPos, offsetRot, colliderMountPoint, checkDir);
//                    colliderMountPoint.SetModel(go);
//                }
//            }), this));
//    }
//}
