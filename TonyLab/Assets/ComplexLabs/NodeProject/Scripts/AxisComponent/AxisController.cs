//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using XProject.Models;
//using System;

//public enum AxisDirection
//{
//    X,
//    Y,
//    Z,
//    Null,
//}

///// <summary>
///// 轴状态
///// </summary>
//public enum ChoiceState
//{
//    MOVE,//移动
//    ROTATE,//旋转
//    SCALE,//缩放
//    SELECT,//选择状态（不显示轴）
//}

//public class AxisController : MonoBehaviour
//{
//    private bool isClickAxis;                //是否在点击轴

//    [SerializeField]
//    private GameObject crtTarget;

//    private bool useWorldAxis;        //使用世界坐标系（为否时使用本地坐标系）

//    private bool isPosClick = false;
//    private bool isRotClick = false;

//    private const float posAxisScale = 1.5f;    //坐标轴在屏幕上的长度
//    private const float rotAxisScale = 0.6f;    //旋转轴在屏幕上的长度

//    private Transform posAxis;              //坐标轴
//    private Transform rotAxis;              //旋转轴
//    private Transform scaAxis;              //缩放轴
//    private OperationRecord operateRecord;    //操作记录
//    private Vector3 vector;
//    private Vector3 lastPos;
//    private Vector3 startPos;
//    private AxisDirection dir;
//    private AxisDirection secondDir;
//    private Vector3 previousPos;                //先前的位置
//    private const float speed = 100f;            //旋转速度
//    private const float moveSpeed = 50f;       //移动速度
//    private Vector3 previousRot;                //先前的旋转
//    private float normalDistance;
//    private Vector2 screen_ZhouVector;          //当前轴的向量,   屏幕坐标
//    private Vector2 screen_MouseVector;         //当前鼠标移动向量,  屏幕坐标
//    private Transform zhouNode;                 //轴节点
//    private Vector3 Move_Vectot;
//    private Transform curClickAxis;          //当前点击的轴

//    private void Awake()
//    {
//        useWorldAxis = true;

//        NotificationCenter.Instance().AttachObsever(ActionType.PosAxisActivation, PosAxisActivation);
//        NotificationCenter.Instance().AttachObsever(ActionType.PosAxisDormancy, PosAxisDormancy);
//        NotificationCenter.Instance().AttachObsever(ActionType.RotAxisActivation, RotAxisActivation);
//        NotificationCenter.Instance().AttachObsever(ActionType.RotAxisDormancy, RotAxisDormancy);
//        NotificationCenter.Instance().AttachObsever(ActionType.SwitchAxis, SwitchAxis);
//        NotificationCenter.Instance().AttachObsever(ActionType.StopControllerAxis, StopControllerAxis);
//        NotificationCenter.Instance().AttachObsever(ActionType.LocationAxis, LocationAxis);
//        NotificationCenter.Instance().AttachObsever(ActionType.AxisMouseDown, MouseDown);
//        NotificationCenter.Instance().AttachObsever(ActionType.AxisMouseUp, MouseUp);
//        NotificationCenter.Instance().AttachObsever(ActionType.AxisMouseEnter, MouseEnter);
//        NotificationCenter.Instance().AttachObsever(ActionType.AxisMouseExit, MouseExit);

//        //获取轴
//        posAxis = transform.Find("坐标轴");
//        rotAxis = transform.Find("旋转轴");
//        scaAxis = transform.Find("缩放轴");
//        SwitchAxis(new Notification(3, this));
//    }
    
//    private void Update()
//    {
//        if (GameManager.instance.clickObjs.Count > 0)
//        {
//            transform.position = GameManager.instance.clickObjs[0].transform.TransformPoint(GameManager.instance.clickObjs[0].transform.GetComponent<BoxCollider>().center);
//            transform.rotation = GameManager.instance.clickObjs[0].transform.rotation;
//        }

//        if (crtTarget != null)
//        {
//            transform.position = crtTarget.transform.TransformPoint(crtTarget.transform.GetComponent<BoxCollider>().center);
//            transform.rotation = crtTarget.transform.rotation;
//            if (isPosClick)
//            {
//                if (Input.GetMouseButtonUp(0))
//                {
//                    PosAxisDormancy(new Notification(this));
//                }
//                else
//                {
//                    if (Input.GetKey(KeyCode.LeftShift))
//                    {

//                        startPos.x += Input.GetAxis("Mouse X") * 0.333f;
//                        startPos.y += Input.GetAxis("Mouse Y") * 0.333f;
//                    }
//                    else
//                    {

//                        startPos.x += Input.GetAxis("Mouse X");
//                        startPos.y += Input.GetAxis("Mouse Y");
//                    }
                    
//                    Vector3 worldPos = Camera.main.ScreenToWorldPoint(startPos);

//                    Vector3 offset = worldPos - lastPos;

//                    if (worldPos != lastPos)
//                    {

//                        switch (dir)
//                        {
//                            case AxisDirection.X:
//                                if (useWorldAxis)
//                                {
//                                    vector = Vector3.right;
//                                }
//                                else
//                                {
//                                    vector = crtTarget.transform.right;
//                                }
//                                break;
//                            case AxisDirection.Y:
//                                if (useWorldAxis)
//                                {
//                                    vector = Vector3.up;
//                                }
//                                else
//                                {
//                                    vector = crtTarget.transform.up;
//                                }
//                                break;
//                            case AxisDirection.Z:
//                                if (useWorldAxis)
//                                {
//                                    vector = Vector3.forward;
//                                }
//                                else
//                                {
//                                    vector = crtTarget.transform.forward;
//                                }
//                                break;
//                        }

//                        DoMove(vector.normalized * (offset.magnitude * Mathf.Cos(Vector3.Angle(offset, vector) / 180f * Mathf.PI)) * moveSpeed);

//                        if (secondDir != AxisDirection.Null)
//                        {
//                            switch (secondDir)
//                            {
//                                case AxisDirection.X:
//                                    if (useWorldAxis)
//                                    {
//                                        vector = Vector3.right;
//                                    }
//                                    else
//                                    {
//                                        vector = crtTarget.transform.right;
//                                    }
//                                    break;
//                                case AxisDirection.Y:
//                                    if (useWorldAxis)
//                                    {
//                                        vector = Vector3.up;
//                                    }
//                                    else
//                                    {
//                                        vector = crtTarget.transform.up;
//                                    }
//                                    break;
//                                case AxisDirection.Z:
//                                    if (useWorldAxis)
//                                    {
//                                        vector = Vector3.forward;
//                                    }
//                                    else
//                                    {
//                                        vector = crtTarget.transform.forward;
//                                    }
//                                    break;
//                            }
//                            DoMove(vector.normalized * (offset.magnitude * Mathf.Cos(Vector3.Angle(offset, vector) / 180f * Mathf.PI)) * moveSpeed);
//                        }

//                        lastPos = worldPos;

//                        transform.position = crtTarget.transform.TransformPoint(crtTarget.transform.GetComponent<BoxCollider>().center);
//                        transform.rotation = crtTarget.transform.rotation;

//                        SendSignalHelper.MoveModel(GameManager.instance.clickObjs[0].GetComponent<ModelInfo>().nodeInfo.selfID, crtTarget.transform);

//                    }
//                }
//                NotificationCenter.Instance().PostDispatch(ActionType.SetSursor, new Notification(this));
//            }



//            if (isRotClick)
//            {
//                if (Input.GetMouseButtonUp(0))
//                {
//                    RotAxisDormancy(new Notification(this));
//                }
//                else
//                {

//                    DoRotate();

//                   SendSignalHelper.RotateModel(crtTarget.GetComponent<ModelInfo>().nodeInfo.selfID, crtTarget.transform);

//                }
//                NotificationCenter.Instance().PostDispatch(ActionType.SetSursor, new Notification(this));
//            }

//            RefreshScale();

//            if (useWorldAxis)
//            {
//                posAxis.transform.rotation = Quaternion.Euler(new Vector3(-90, 180, 180));
//            }
//            else
//            {
//                posAxis.transform.localRotation = Quaternion.Euler(new Vector3(-90, 180, 180));
//            }

//            if (useWorldAxis)
//            {
//                rotAxis.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
//            }
//            else
//            {
//                rotAxis.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
//            }
//        }
//    }

//    private void OnDestroy()
//    {
//        NotificationCenter.Instance().DetachObsever(ActionType.PosAxisActivation, PosAxisActivation);
//        NotificationCenter.Instance().DetachObsever(ActionType.PosAxisDormancy, PosAxisDormancy);
//        NotificationCenter.Instance().DetachObsever(ActionType.RotAxisActivation, RotAxisActivation);
//        NotificationCenter.Instance().DetachObsever(ActionType.RotAxisDormancy, RotAxisDormancy);
//        NotificationCenter.Instance().DetachObsever(ActionType.SwitchAxis, SwitchAxis);
//        NotificationCenter.Instance().DetachObsever(ActionType.StopControllerAxis, StopControllerAxis);
//        NotificationCenter.Instance().DetachObsever(ActionType.LocationAxis, LocationAxis);
//        NotificationCenter.Instance().DetachObsever(ActionType.AxisMouseDown, MouseDown);
//        NotificationCenter.Instance().DetachObsever(ActionType.AxisMouseUp, MouseUp);
//        NotificationCenter.Instance().DetachObsever(ActionType.AxisMouseEnter, MouseEnter);
//        NotificationCenter.Instance().DetachObsever(ActionType.AxisMouseExit, MouseExit);
//    }

//    /// <summary>
//    /// 移动轴激活
//    /// </summary>
//    /// <param name="dir"></param>
//    public void PosAxisActivation(Notification noti)
//    {
//        Tuple<AxisDirection, AxisDirection> temp = noti.Arguments as Tuple<AxisDirection, AxisDirection>;
//        AxisDirection dir = temp.Item1;
//        AxisDirection second = temp.Item2;

//        normalDistance = GetNormalDistance();
//        isPosClick = true;
//        this.dir = dir;
//        this.secondDir = second;
//        previousPos = crtTarget.transform.position;

//        //lastPos = GetWorldPos();
//        startPos = new Vector3(0, 0, 1);
//        lastPos = Camera.main.ScreenToWorldPoint(startPos);

//        operateRecord = new OperationRecord(GameManager.instance.needControlTargets, OperateType.Move);

//        GameManager.instance.isTouchAxis = true;
//    }

//    /// <summary>
//    /// 移动轴休眠
//    /// </summary>
//    public void PosAxisDormancy(Notification noti)
//    {
//        isPosClick = false;

//        GameManager.instance.isTouchAxis = false;

//        //如果坐标不相等，记录下操作
//        if (crtTarget.transform.position != previousPos)
//        {
//            NotificationCenter.Instance().PostDispatch(ActionType.RecordOperater, new Notification(operateRecord, this));
//        }
//    }
//    /// <summary>
//    /// 旋转轴激活
//    /// </summary>
//    /// <param name="dir"></param>
//    public void RotAxisActivation(Notification noti)
//    {
//        AxisDirection dir = (AxisDirection)noti.Arguments;

//        isRotClick = true;

//        //鼠标按下，获取父节点，记录旋转，暂存操作
//        previousRot = crtTarget.transform.eulerAngles;

//        operateRecord = new OperationRecord(GameManager.instance.needControlTargets, OperateType.Rotate);

//        switch (dir)
//        {
//            case AxisDirection.X:
//                zhouNode = rotAxis.Find("nodex");
//                vector = Vector3.right;
//                break;
//            case AxisDirection.Y:
//                zhouNode = rotAxis.Find("nodey");
//                vector = Vector3.up;
//                break;
//            case AxisDirection.Z:
//                zhouNode = rotAxis.Find("nodez");
//                vector = Vector3.forward;
//                break;
//        }

//        GameManager.instance.isTouchAxis = true;
//    }

//    /// <summary>
//    /// 旋转轴休眠
//    /// </summary>
//    public void RotAxisDormancy(Notification noti)
//    {
//        isRotClick = false;
        
//        if (crtTarget.transform.eulerAngles != previousRot)
//        {
//            NotificationCenter.Instance().PostDispatch(ActionType.RecordOperater, new Notification(operateRecord, this));
//        }

//        GameManager.instance.isTouchAxis = false;
//    }

//    /// <summary>
//    /// 切换当前显示轴
//    /// </summary>
//    /// <param name="o">数字对应显示不同的轴，0代表坐标轴，1代表旋转轴，2代表缩放轴，3代表不显示任何轴，</param>
//    public void SwitchAxis(Notification noti)
//    {
//        int o = (int)noti.Arguments;
//        if (o == 0)
//        {
//            useWorldAxis = true;
//            posAxis.gameObject.SetActive(true);
//            rotAxis.gameObject.SetActive(false);
//            scaAxis.gameObject.SetActive(false);
//        }
//        else if (o == 1)
//        {
//            useWorldAxis = false;
//            posAxis.gameObject.SetActive(false);
//            rotAxis.gameObject.SetActive(true);
//            scaAxis.gameObject.SetActive(false);
//        }
//        else if (o == 2)
//        {
//            posAxis.gameObject.SetActive(false);
//            rotAxis.gameObject.SetActive(false);
//            scaAxis.gameObject.SetActive(true);
//        }
//        else
//        {
//            posAxis.gameObject.SetActive(false);
//            rotAxis.gameObject.SetActive(false);
//            scaAxis.gameObject.SetActive(false);
//        }
//    }

//    /// <summary>
//    /// 停止操作轴
//    /// </summary>
//    public void StopControllerAxis(Notification noti)
//    {
//        isPosClick = false;
//        isRotClick = false;
//        SwitchAxis(new Notification(3, this));
//    }

//    /// <summary>
//    /// 定位坐标轴
//    /// </summary>
//    public void LocationAxis(Notification noti)
//    {
//        if (GameManager.instance.clickObjs.Count > 0)
//        {
//            crtTarget = GameManager.instance.clickObjs[0].gameObject;
//            transform.position = crtTarget.transform.TransformPoint(crtTarget.transform.GetComponent<BoxCollider>().center);
//            if (!useWorldAxis)
//            {
//                transform.rotation = crtTarget.transform.rotation;
//            }
//        }
//    }

//    public void MouseDown(Notification noti)
//    {
//        Transform axis = noti.Arguments as Transform;

//        curClickAxis = axis;
//        isClickAxis = true;
//    }

//    public void MouseUp(Notification noti)
//    {
//        Tuple<Transform, Action> temp = noti.Arguments as Tuple<Transform, Action>;

//        Transform axis = temp.Item1;
//        Action close = temp.Item2;

//        if (curClickAxis == axis)
//            close?.Invoke();

//        curClickAxis = null;
//        isClickAxis = false;
//    }

//    public void MouseEnter(Notification noti)
//    {
//        Tuple<Transform, Action> temp = noti.Arguments as Tuple<Transform, Action>;

//        Transform axis = temp.Item1;
//        Action open = temp.Item2;
//        if (isClickAxis == false)
//        {
//            curClickAxis = axis;
//        }
//        else
//        {
//            if (curClickAxis != axis)
//                return;
//        }
//        open.Invoke();
//    }

//    public void MouseExit(Notification noti)
//    {
//        Tuple<Transform, Action> temp = noti.Arguments as Tuple<Transform, Action>;

//        Transform axis = temp.Item1;
//        Action close = temp.Item2;
//        if (isClickAxis == true && curClickAxis == axis)
//            return;
//        close.Invoke();
//    }

//    /// <summary>
//    /// 根据坐标轴当前位置刷新大小
//    /// </summary>
//    private void RefreshScale()
//    {
//        if (Camera.main.orthographic)
//        {
//            float orthographicSize = Camera.main.orthographicSize;
//            posAxis.localScale = new Vector3(orthographicSize * 70 / 5, orthographicSize * 70 / 5, orthographicSize * 70 / 5);
//        }
//        else
//        {
//            float ChangeScale = (Camera.main.transform.position - posAxis.position).magnitude * posAxisScale;

//            Transform crtposAxis = posAxis;
//            while (crtposAxis.parent != null)
//            {
//                ChangeScale *= 0.5f / crtposAxis.parent.localScale.x;
//                crtposAxis = crtposAxis.parent;
//            }

//            posAxis.localScale = Vector3.one * ChangeScale * 7;

//            ChangeScale = (Camera.main.transform.position - rotAxis.position).magnitude * rotAxisScale;

//            Transform crtrotAxis = rotAxis;
//            while (crtrotAxis.parent != null)
//            {
//                ChangeScale *= 0.5f / crtrotAxis.parent.localScale.x;
//                crtrotAxis = crtrotAxis.parent;
//            }

//            rotAxis.localScale = Vector3.one * ChangeScale;
//        }
//    }

//    private void DoMove(Vector3 offset)
//    {
//        float xxx = Vector3.Distance(transform.position, Camera.main.transform.position) / normalDistance;
//        offset *= xxx;

//        foreach (var item in GameManager.instance.needControlTargets)
//        {
//            item.GetComponent<ModelInfo>().ChangePosByOffset(offset);
//        }
//    }

//    private float GetNormalDistance()
//    {
//        float maxsize = GameManager.instance.clickObjs[0].GetComponent<BoxCollider>().size.x;

//        if (GameManager.instance.clickObjs[0].GetComponent<BoxCollider>().size.y > maxsize)
//        {
//            maxsize = GameManager.instance.clickObjs[0].GetComponent<BoxCollider>().size.y;
//        }
//        if (GameManager.instance.clickObjs[0].GetComponent<BoxCollider>().size.z > maxsize)
//        {
//            maxsize = GameManager.instance.clickObjs[0].GetComponent<BoxCollider>().size.z;
//        }
//        if (maxsize < 1)
//        {
//            maxsize = 1;
//        }
//        return (Camera.main.transform.forward.normalized * maxsize * 2).magnitude;
//    }

//    /// <summary>
//    /// 获取鼠标位置的世界坐标（深度由所处物体决定）
//    /// </summary>
//    /// <returns></returns>
//    private Vector3 GetWorldPos()
//    {
//        //获取需要移动物体的世界转屏幕坐标
//        Vector3 screenPos = Camera.main.WorldToScreenPoint(crtTarget.transform.position);
//        //获取鼠标位置
//        Vector3 mousePos = Input.mousePosition;
//        //因为鼠标只有X，Y轴，所以要赋予给鼠标Z轴
//        mousePos.z = screenPos.z;
//        //把鼠标的屏幕坐标转换成世界坐标
//        return Camera.main.ScreenToWorldPoint(mousePos);
//    }

//    private void DoRotate()
//    {
//        //screen_MouseVector = (Vector2)Input.mousePosition - lastMousePos;
//        screen_MouseVector = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
//        screen_ZhouVector = Camera.main.WorldToScreenPoint(zhouNode.position) - Camera.main.WorldToScreenPoint(rotAxis.position);
//        Move_Vectot = Vector3.Cross(Vector3.Normalize(screen_ZhouVector).normalized, screen_MouseVector);

//        //parentGo.transform.Rotate(vector * Move_Vectot.z * speed * Time.deltaTime, useWorldPos ? Space.World : Space.Self);

//        Quaternion p1 = GameManager.instance.needControlTargets[0].rotation;
//        GameManager.instance.needControlTargets[0].Rotate(vector * Move_Vectot.z * speed * Time.deltaTime, useWorldAxis ? Space.World : Space.Self);
//        GameManager.instance.needControlTargets[0].GetComponent<ModelInfo>().RememberRot();

//        Quaternion p2 = GameManager.instance.needControlTargets[0].rotation;
//        float angle = Quaternion.Angle(p1, p2);
//        if (Move_Vectot.z != 0)
//        {
//            angle *= Mathf.Abs(Move_Vectot.z) / Move_Vectot.z;

//            for (int i = 1; i < GameManager.instance.needControlTargets.Count; i++)
//            {
//                GameManager.instance.needControlTargets[i].RotateAround(GameManager.instance.needControlTargets[0].position, vector, angle);
//                GameManager.instance.needControlTargets[i].GetComponent<ModelInfo>().RememberRot();
//            }
//        }
//    }
//}
