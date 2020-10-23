//using DG.Tweening;
//using NonsensicalFrame;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.EventSystems;

//public class CameraController : MonoBehaviour
//{
//    [SerializeField]
//    private Texture2D handTexture;       //按下中键时的鼠标样式

//    //鼠标滚轮滚动速度
//    private const float mouseWheelRollSpeed = 2.2f;
//    //Alt+左键的缓速移动
//    private const float slowMoveSpeed = 0.05f;
//    //滑轮终点位置
//    private Vector3 mouseWheelEndPos = Vector3.zero;
//    //鼠标滑轮按下时拖动的速度
//    private const float mouseWheelDownSpeed = 9f;
//    //插值时间
//    private const float lerpTime = 0.1f;
//    //围绕速度
//    private const float aroundSpeed = 50f;
//    //移动速度
//    private const float moveSpeed = 0.3f;
//    //聚焦时间
//    private const float focusTime = 0.5f;
//    //正交聚焦速度
//    private float orthographicSpeed;

//    public bool needFouce = false;
//    //是否在聚焦
//    private bool isFocusing;
//    private bool canOperation = true;   //是否可以操作摄像机

//    private Vector3 originPos;          //初始位置
//    private Vector3 originRot;          //初始旋转

//    private bool isApplicationFocused = true;   //是否拥有程序焦点

//    private float CamerafocusingDistance = 3f;  //摄像机聚焦距离

//    private void Awake()
//    {
//        NotificationCenter.Instance.AttachObsever(EventName.PartClick, OpenOperateCamera);
//        NotificationCenter.Instance.AttachObsever(EventName.ResetScene,ResetCamera);
//        NotificationCenter.Instance.AttachObsever(EventName.CameraNeedFocus, NeedFouce);
//    }

//    void OnDestroy()
//    {
//        NotificationCenter.Instance.DetachObsever(EventName.PartClick, OpenOperateCamera);
//        NotificationCenter.Instance.DetachObsever(EventName.ResetScene, ResetCamera);
//        NotificationCenter.Instance.DetachObsever(EventName.CameraNeedFocus, NeedFouce);
//    }

//    private void Start()
//    {
//        isApplicationFocused = Application.isFocused;
//        CamerafocusingDistance = Camera.main.transform.position.magnitude;
//        originPos = Camera.main.transform.position;
//        originRot = Camera.main.transform.eulerAngles;
//    }

//    void OnApplicationFocus(bool focus)
//    {
//        isApplicationFocused = focus;
//    }

//    private void NeedFouce(object noti)
//    {
//        needFouce = true;
//    }

//    private void ResetCamera(object noti)
//    {
//        float orthographicSize = transform.GetComponent<Camera>().orthographicSize;
//        orthographicSpeed = (orthographicSize - 0.8f) / focusTime * Time.deltaTime;
//        DoFouce(orthographicSpeed, null);
//    }

//    bool CheckMousePosition()
//    {
//        Vector3 mousePos = Input.mousePosition;
//        if (mousePos.x > 0 && mousePos.x < Screen.width && mousePos.y > 0 && mousePos.y < Screen.height)
//        {
//            return true;
//        }
//        else
//        {
//            return false;
//        }
//    }

//    void FixedUpdate()
//    {
//        if (canOperation)
//        {
//            if (!GameManager.instance.isTouchAxis)
//            {
//                if (Input.GetKeyDown(KeyCode.F) || needFouce)
//                {
//                    needFouce = false;

//                    if (GameManager.instance.clickObjs.Count > 0)
//                    {
//                        float orthographicSize = transform.GetComponent<Camera>().orthographicSize;
//                        orthographicSpeed = (orthographicSize - 0.8f) / focusTime * Time.deltaTime;
//                        DoFouce(orthographicSpeed, GameManager.instance.clickObjs[0].transform);
//                    }
//                    else
//                    {
//                        float orthographicSize = transform.GetComponent<Camera>().orthographicSize;
//                        orthographicSpeed = (orthographicSize - 0.8f) / focusTime * Time.deltaTime;
//                        DoFouce(orthographicSpeed, null);
//                    }
//                }
//                CameraMove();

//                RotateSelf();
//                HorizontalAround();
//                VerticalAround();
//                PressWheelMove();

//                WheelChangeDistance();
//                WheelChangeLerp();
//            }
//            else
//            {
//                if (needFouce)
//                {
//                    needFouce = false;
//                }
//            }

//        }
//        FocusLerp();
//    }

//    private void CameraMove()
//    {
//        Vector3 offset = Vector3.zero;
//        if (Input.GetKey(KeyCode.W))
//        {
//            offset += transform.forward * moveSpeed;
//        }
//        if (Input.GetKey(KeyCode.S))
//        {
//            offset += -transform.forward * moveSpeed;
//        }
//        if (Input.GetKey(KeyCode.A))
//        {
//            offset += -transform.right * moveSpeed;
//        }
//        if (Input.GetKey(KeyCode.D))
//        {
//            offset += transform.right * moveSpeed;
//        }
//        if (Input.GetKey(KeyCode.Q))
//        {
//            offset += -transform.up * moveSpeed;
//        }
//        if (Input.GetKey(KeyCode.E))
//        {
//            offset += transform.up * moveSpeed;
//        }

//        if (Input.GetKey(KeyCode.LeftShift))
//        {
//            offset *= 0.3333f;
//        }
//        transform.position += offset;
//    }

//    private void RotateSelf()
//    {
//        if (Input.GetMouseButton(1) && !Input.GetKey(KeyCode.LeftAlt) && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
//        {
//            isFocusing = false;
//            float x = Input.GetAxis("Mouse X");
//            StartCoroutine(SynRotate(transform.position, Vector3.up, x * 10f));

//            float y = Input.GetAxis("Mouse Y");
//            StartCoroutine(SynRotate(transform.position, -transform.right, y * 10f));
//            NotificationCenter.Instance().PostDispatch(ActionType.SetSursor, new Notification(this));
//        }
//    }

//    //协程旋转
//    IEnumerator SynRotate(Vector3 aroundTarget, Vector3 dir, float angle)
//    {
//        transform.RotateAround(aroundTarget, dir, angle / 10);
//        yield return new WaitForSeconds(0.1f);
//    }

//    //水平围绕
//    private void HorizontalAround()
//    {
//        if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt) && !EventSystem.current.IsPointerOverGameObject())
//        {
//            float x = Input.GetAxis("Mouse X");

//            if (x != 0)
//            {
//                Vector3 targetPos = transform.position + transform.forward * CamerafocusingDistance;
//                StartCoroutine(SynRotate(new Vector3(targetPos.x, transform.position.y, targetPos.z), Vector3.up, x * aroundSpeed));
//                NotificationCenter.Instance().PostDispatch(ActionType.SetSursor, new Notification(this));
//            }

//        }
//    }

//    private void VerticalAround()
//    {
//        if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt) && !EventSystem.current.IsPointerOverGameObject())
//        {
//            float y = Input.GetAxis("Mouse Y");
//            if (y != 0)
//            {
//                Vector3 targetPos = transform.position + transform.forward * CamerafocusingDistance;
//                StartCoroutine(SynRotate(targetPos, -transform.right, y * aroundSpeed));

//                NotificationCenter.Instance().PostDispatch(ActionType.SetSursor, new Notification(this));
//            }
//        }
//    }

//    private void PressWheelMove()
//    {
//        if (Input.GetMouseButton(2))
//        {
//            Cursor.SetCursor(handTexture, Vector2.zero, CursorMode.ForceSoftware);

//            if (Input.GetKey(KeyCode.LeftShift) && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
//            {
//                transform.position += (transform.right * -Input.GetAxis("Mouse X") + transform.up * -Input.GetAxis("Mouse Y")) * mouseWheelDownSpeed * Time.deltaTime * 0.3333f;
//                isFocusing = false;
//            }
//            else
//            {
//                transform.position += (transform.right * -Input.GetAxis("Mouse X") + transform.up * -Input.GetAxis("Mouse Y")) * mouseWheelDownSpeed * Time.deltaTime;
//                isFocusing = false;
//            }

//            NotificationCenter.Instance().PostDispatch(ActionType.SetSursor, new Notification(this));
//        }
//        else
//        {
//            Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
//        }
//    }

//    private void WheelChangeDistance()
//    {
//        if (isApplicationFocused && CheckMousePosition() && !EventSystem.current.IsPointerOverGameObject())
//        {
//            float offset = 0;

//            offset += Input.mouseScrollDelta.y * mouseWheelRollSpeed;


//            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButton(1))
//            {
//                offset += (Input.GetAxisRaw("Mouse X") - Input.GetAxisRaw("Mouse Y")) * slowMoveSpeed;

//                NotificationCenter.Instance().PostDispatch(ActionType.SetSursor, new Notification(this));
//            }

//            if (Input.GetKey(KeyCode.LeftShift))
//            {
//                offset *= 0.3333f;
//            }

//            //正交摄像机
//            if (GetComponent<Camera>().orthographic == true)
//            {
//                if (GetComponent<Camera>().orthographicSize - offset <= 0)
//                {
//                    offset = GetComponent<Camera>().orthographicSize - 0.001f;
//                }

//                GetComponent<Camera>().orthographicSize -= offset * 0.2f;

//                transform.GetChild(0).GetComponent<Camera>().orthographicSize = GetComponent<Camera>().orthographicSize;

//                mouseWheelEndPos = transform.position;

//                buffer += transform.forward * offset;
//            }
//            else
//            {
//                mouseWheelEndPos = transform.position + buffer2 + transform.forward * offset;

//                buffer2 = Vector3.zero;
//            }
//        }
//    }

//    public Vector3 buffer;
//    public Vector3 buffer2;

//    public void ChangeMode(int i)
//    {
//        if (i == 0)
//        {
//            buffer = Vector3.zero;
//        }
//        else if (i == 1)
//        {
//            buffer2 = buffer;
//        }
//    }

//    private void WheelChangeLerp()
//    {
//        if (mouseWheelEndPos != Vector3.zero && transform.position != mouseWheelEndPos && isApplicationFocused && CheckMousePosition())
//        {
//            transform.position = Vector3.Lerp(transform.position, mouseWheelEndPos, lerpTime);
//            if (isFocusing && GameManager.instance.clickObjs.Count > 0)
//            {
//                Transform fouceTarget = GameManager.instance.clickObjs[0].transform;
//                if (fouceTarget.GetComponent<Collider>() != null)
//                {
//                    CamerafocusingDistance = Vector3.Distance(fouceTarget.GetComponent<Collider>().bounds.center, transform.position);
//                }
//                else
//                {
//                    CamerafocusingDistance = Vector3.Distance(fouceTarget.position, transform.position);
//                }
//            }
//        }
//    }

//    //移动相机到传来的参数位置,打开相机操作模式，可以通过鼠标滚轮改变视角
//    private void OpenOperateCamera(object noti)
//    {
//        if (noti != null)
//        {
//            Vector3 viewPoint = transform.position + transform.forward * CamerafocusingDistance;
//            Vector3 offset = (Vector3)noti;

//            Vector3 oldPos = transform.position;
//            Vector3 oldRot = transform.localEulerAngles;

//            transform.position = viewPoint + offset.normalized * CamerafocusingDistance;
//            transform.LookAt(viewPoint);

//            Vector3 newPos = transform.position;
//            Vector3 newRot = transform.localEulerAngles;

//            transform.position = oldPos;
//            transform.localEulerAngles = oldRot;

//            transform.DOKill();

//            transform.DOMove(newPos, 0.5f);
//            transform.DORotate(newRot, 0.5f, RotateMode.Fast);
//            StartCoroutine(StopOperation(0.5f));
//        }
//    }

//    /// <summary>
//    /// 聚焦操作
//    /// </summary>
//    public void DoFouce(float orthographicSpeed, Transform fouceTarget)
//    {
//        if (fouceTarget == null)
//        {
//            transform.DOMove(originPos, focusTime);
//            transform.DORotate(originRot, focusTime);
//            StartCoroutine(StopOperation(focusTime));
//            isOrthographicDoFouce = true;

//            CamerafocusingDistance = Camera.main.transform.position.magnitude;
//            isFocusing = true;
//        }
//        else
//        {
//            float moderateDistance = GetDistance(fouceTarget);

//            Vector3 moveTargetPos;
//            if (fouceTarget.GetComponent<Collider>() != null)
//            {
//                moveTargetPos = fouceTarget.GetComponent<Collider>().bounds.center - transform.forward.normalized * moderateDistance;
//            }
//            else
//            {
//                moveTargetPos = fouceTarget.position - transform.forward.normalized * moderateDistance;
//            }


//            transform.DOMove(moveTargetPos, focusTime);
//            StartCoroutine(StopOperation(focusTime));

//            isOrthographicDoFouce = true;

//            CamerafocusingDistance = moderateDistance;
//            isFocusing = true;
//        }
//    }

//    //正交摄像机插值聚焦
//    float recordFocusTime = 0;
//    bool isOrthographicDoFouce = false;

//    private void FocusLerp()
//    {
//        if (isOrthographicDoFouce)
//        {
//            recordFocusTime += Time.deltaTime;
//            transform.GetComponent<Camera>().orthographicSize -= orthographicSpeed;
//            transform.GetChild(0).GetComponent<Camera>().orthographicSize -= orthographicSpeed;
//            if (recordFocusTime >= focusTime)
//            {
//                recordFocusTime = 0;
//                isOrthographicDoFouce = false;
//            }
//        }
//    }

//    /// <summary>
//    /// 获取合适距离
//    /// </summary>
//    /// <returns></returns>
//    private float GetDistance(Transform fouceTarget)
//    {
//        if (fouceTarget.GetComponent<Collider>() == null)
//        {
//            return 1;
//        }
//        float maxsize = fouceTarget.GetComponent<Collider>().bounds.size.x;
//        if (fouceTarget.GetComponent<Collider>().bounds.size.y > maxsize)
//        {
//            maxsize = fouceTarget.GetComponent<Collider>().bounds.size.y;
//        }
//        if (fouceTarget.GetComponent<Collider>().bounds.size.z > maxsize)
//        {
//            maxsize = fouceTarget.GetComponent<Collider>().bounds.size.z;
//        }
//        if (maxsize < 1)
//        {
//            maxsize = 1;
//        }
//        return maxsize * 2;
//    }

//    private IEnumerator StopOperation(float time)
//    {
//        canOperation = false;
//        yield return new WaitForSeconds(time);
//        canOperation = true;
//    }
//}
