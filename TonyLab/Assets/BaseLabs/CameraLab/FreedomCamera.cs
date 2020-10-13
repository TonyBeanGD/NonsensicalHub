using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreedomCamera : MonoBehaviour
{
    public Texture2D handTexture;

    private const float mouseWheelRollSpeed = 1f;   //鼠标滚轮滚动速度
    private const float mouseWheelDownSpeed = 5f;   //鼠标滚轮按下速度
    private const float aroundSpeed = 25f;          //环绕速度速度
    private const float rotateSpeed = 5f;           //自转速度
    private const float moveSpeed = 0.1f;           //移动速度
    private const float focusTime = 0.5f;           //聚焦时间
    private const float ShiftMagnification = 3f;    //shift加速倍率

    private float orthographicSpeed;    //正交聚焦速度
    private bool canOperation = true;   //是否可以操作
    private float CamerafocusingDistance;
    private bool isApplicationFocused;

    private void Start()
    {
        CamerafocusingDistance = Camera.main.transform.position.magnitude;
        isApplicationFocused = Application.isFocused;
    }

    private void OnApplicationFocus(bool focus)
    {
        isApplicationFocused = focus;
    }

    private void Update()
    {
        if (canOperation)
        {
            CameraMove();

            if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt))
            {
                RotateAround();
            }
            else if (Input.GetMouseButton(1))
            {

                RotateSelf();
            }

            PressWheelMove();

            WheelChangeDistance();
        }
    }

    private void CameraMove()
    {
        Vector3 offset = Vector3.zero;

        offset += transform.forward * Input.GetAxisRaw("Vertical");
        offset += transform.right * Input.GetAxisRaw("Horizontal");

        offset *= moveSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            offset *= ShiftMagnification;
        }

        transform.position += offset;
    }

    private void RotateSelf()
    {
        float x = Input.GetAxisRaw("Mouse X");
        transform.RotateAround(transform.position, Vector3.up, x * rotateSpeed);

        float y = Input.GetAxisRaw("Mouse Y");
        transform.RotateAround(transform.position, -transform.right, y * rotateSpeed);
    }

    private void RotateAround()
    {
        Vector3 targetPos = transform.position + transform.forward * CamerafocusingDistance;

        float x = Input.GetAxisRaw("Mouse X");
        transform.RotateAround(new Vector3(targetPos.x, transform.position.y, targetPos.z), Vector3.up, x * aroundSpeed);

        float y = Input.GetAxisRaw("Mouse Y");
        transform.RotateAround(targetPos, -transform.right, y * aroundSpeed);
    }

    private void PressWheelMove()
    {
        if (Input.GetMouseButton(2))
        {
            Cursor.SetCursor(handTexture, Vector2.zero, CursorMode.ForceSoftware);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.position += (transform.right * -Input.GetAxis("Mouse X") + transform.up * -Input.GetAxis("Mouse Y")) * mouseWheelDownSpeed * Time.deltaTime * ShiftMagnification;
            }
            else
            {
                transform.position += (transform.right * -Input.GetAxis("Mouse X") + transform.up * -Input.GetAxis("Mouse Y")) * mouseWheelDownSpeed * Time.deltaTime;
            }
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
        }
    }

    private void WheelChangeDistance()
    {
        if (isApplicationFocused && CheckMousePosition())
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.position += transform.forward * Input.GetAxisRaw("Mouse ScrollWheel") * mouseWheelRollSpeed * ShiftMagnification;
            }
            else
            {
                transform.position += transform.forward * Input.GetAxisRaw("Mouse ScrollWheel") * mouseWheelRollSpeed;
            }
        }
    }

    //移动相机到传来的参数位置,打开相机操作模式，可以通过鼠标滚轮改变视角
    private void OpenOperateCamera(Vector3 pos, Vector3 rot)
    {
        Vector3 viewPoint = transform.position + transform.forward * CamerafocusingDistance;
        Vector3 offset = pos;

        Vector3 oldPos = transform.position;
        Vector3 oldRot = transform.localEulerAngles;

        transform.position = viewPoint + offset.normalized * CamerafocusingDistance;
        transform.LookAt(viewPoint);

        Vector3 newPos = transform.position;
        Vector3 newRot = transform.localEulerAngles;

        transform.position = oldPos;
        transform.localEulerAngles = oldRot;

        transform.position = newPos;
        transform.localEulerAngles = newRot;
        StartCoroutine(StopOperation(0.5f));
    }

    /// <summary>
    /// 聚焦操作
    /// </summary>
    public void DoFouce(float orthographicSpeed, Transform fouceTarget)
    {
        float moderateDistance = GetDistance(fouceTarget);
        Vector3 moveTargetPos = fouceTarget.TransformPoint(fouceTarget.GetComponent<BoxCollider>().center) - transform.forward.normalized * moderateDistance;
        transform.position = moveTargetPos;
        StartCoroutine(StopOperation(focusTime));
        CamerafocusingDistance = Vector3.Distance(fouceTarget.TransformPoint(fouceTarget.GetComponent<BoxCollider>().center), moveTargetPos);

    }
    
    private bool CheckMousePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        if (mousePos.x > 0 && mousePos.x < Screen.width && mousePos.y > 0 && mousePos.y < Screen.height)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 获取合适距离
    /// </summary>
    /// <returns></returns>
    private float GetDistance(Transform fouceTarget)
    {
        float maxsize = fouceTarget.GetComponent<BoxCollider>().size.x;
        if (fouceTarget.GetComponent<BoxCollider>().size.y > maxsize)
        {
            maxsize = fouceTarget.GetComponent<BoxCollider>().size.y;
        }
        if (fouceTarget.GetComponent<BoxCollider>().size.z > maxsize)
        {
            maxsize = fouceTarget.GetComponent<BoxCollider>().size.z;
        }
        if (maxsize < 1)
        {
            maxsize = 1;
        }
        return maxsize * 2;
    }

    private IEnumerator StopOperation(float time)
    {
        canOperation = false;
        yield return new WaitForSeconds(time);
        canOperation = true;
    }
}
