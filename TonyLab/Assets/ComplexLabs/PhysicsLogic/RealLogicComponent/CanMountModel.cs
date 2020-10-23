//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// 可以被吸附的对象
///// </summary>
//public class CanMountModel : MonoBehaviour
//{
//    public bool CanSetModel
//    {
//        get
//        {
//            if (crtColliderMountPoint == null || crtColliderMountPoint.CanSetModel == false)
//            {
//                return false;
//            }
//            else
//            {
//                return true;
//            }
//        }
//    }

//    [SerializeField]
//    private string ModelName;

//    [SerializeField]
//    private ColliderMountPoint crtColliderMountPoint;

//    [SerializeField]
//    private bool isAdsorbing;   //是否正在被吸附

//    [SerializeField]
//    private int checkDir;

//    public Vector3 offsetPos { get; private set; }

//    public Vector3 offsetRot { get; private set; }

//    private void Awake()
//    {
//        isAdsorbing = false;
//    }

//    private void OnCollisionEnter(Collision collision)
//    {
//        if (collision.gameObject.tag == "ColliderPoint")
//        {
//            if (crtColliderMountPoint == null)
//            {
//                if (collision.gameObject.GetComponents<ColliderMountPoint>() != null)
//                {
//                    foreach (var item in collision.gameObject.GetComponents<ColliderMountPoint>())
//                    {
//                        if (item.CheckNameAndState(ModelName, this.transform) == true)
//                        {
//                            crtColliderMountPoint = item;
//                            crtColliderMountPoint.ShowModel();
//                            break;
//                        }
//                    }
//                }
//            }
//        }
//        else
//        {
//            Debug.Log("碰撞到错误的物体" + collision.transform.name);
//        }
//    }

//    private void OnCollisionExit(Collision collision)
//    {
//        if (collision.gameObject.tag == "ColliderPoint")
//        {
//            if (crtColliderMountPoint != null)
//            {
//                if (collision.gameObject.GetComponents<ColliderMountPoint>() != null)
//                {
//                    foreach (var item in collision.gameObject.GetComponents<ColliderMountPoint>())
//                    {
//                        if (item == crtColliderMountPoint)
//                        {
//                            crtColliderMountPoint.HideModel();
//                            crtColliderMountPoint = null;
//                            break;
//                        }
//                    }
//                }
//            }
//        }
//    }

//    public void Init(string modelName, Vector3 offsetPos, Vector3 offsetRot, ColliderMountPoint colliderMountPoint, int _checkDir)
//    {
//        this.ModelName = modelName;
//        this.offsetPos = offsetPos;
//        this.offsetRot = offsetRot;
//        this.crtColliderMountPoint = colliderMountPoint;
//        this.checkDir = _checkDir;

//        Transform[] nodes = transform.GetComponentsInChildren<Transform>();

//        foreach (var item in nodes)
//        {
//            item.gameObject.tag = "ColliderPoint";
//        }
//    }

//    public void Activation()
//    {
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

//    public int CanGet(List<string> names)
//    {
//        if (crtColliderMountPoint != null && crtColliderMountPoint.CanGetModel == false)
//        {
//            return -1;
//        }

//        for (int i = 0; i < names.Count; i++)
//        {
//            if (names[i].Equals(ModelName))
//            {
//                return i;
//            }
//        }

//        return -1;
//    }

//    public bool CheckAngle(Transform tsf)
//    {
//        tsf.localEulerAngles = offsetRot;
//        Quaternion q1 = Quaternion.LookRotation(tsf.forward, tsf.up);
//        Quaternion q2 = Quaternion.FromToRotation(-tsf.forward, tsf.up);
//        Quaternion q3 = Quaternion.FromToRotation(tsf.right, tsf.up);
//        Quaternion q4 = Quaternion.FromToRotation(-tsf.right, tsf.up);

//        Quaternion q5 = transform.rotation;

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
//            default: return true;
//        }
//    }

//    public bool CheckName(string name)
//    {
//        return ModelName.Equals(name);
//    }

//    public bool CanAdsorbing()
//    {
//        return !isAdsorbing;
//    }

//    public void Adsorbing(Transform parent)
//    {
//        transform.SetParent(parent);
//        transform.localPosition = offsetPos;
//        transform.localEulerAngles = offsetRot;
//        if (crtColliderMountPoint != null)
//        {
//            crtColliderMountPoint.GetModel();
//        }
//        isAdsorbing = true;
//    }

//    public void SetModel()
//    {
//        isAdsorbing = false;

//        crtColliderMountPoint.SetModel(gameObject);
//    }
//}
