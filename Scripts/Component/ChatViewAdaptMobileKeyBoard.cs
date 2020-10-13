using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 移动设备输入框的自适应组件
/// </summary>
public class ChatViewAdaptMobileKeyBoard : MonoBehaviour
{
    private InputField _inputField;
    private Vector2 _adaptPanelOriginPos;
    private RectTransform _adaptPanelRt;
    private float RESOULUTION_HEIGHT = 750;

    private void Start()
    {
        _inputField = transform.GetComponent<InputField>();
        _inputField.onEndEdit.AddListener(OnEndEdit);
        _adaptPanelRt = CanvasManager._Instance._Canvas.Find("Main").GetComponent<RectTransform>();
        _adaptPanelOriginPos = _adaptPanelRt.anchoredPosition;
    }

    private void Update()
    {
        if (_inputField.isFocused)
        {

            float height = 0;

            if (Application.platform == RuntimePlatform.Android)
            {
                float keyboardHeight = AndroidGetKeyboardHeight() * RESOULUTION_HEIGHT / Screen.height;
                Debug.LogFormat("安卓平台检测到InputField.isFocused为真，获取键盘高度：{0}, Screen.height：{1}", keyboardHeight, Screen.height);
                height=keyboardHeight;
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                float keyboardHeight = IOSGetKeyboardHeight() * RESOULUTION_HEIGHT / Screen.height;
                Debug.LogFormat("IOS平台检测到键盘高度：{0},Screen.height: {1}", keyboardHeight, Screen.height);
                height=keyboardHeight;
            }
            else
            {
                height= 300f;
            }

            Vector3 pos = CanvasManager._Instance._MainCamera.WorldToScreenPoint(_inputField.transform.position);

            height = height - pos.y + _inputField.GetComponent<RectTransform>().sizeDelta.y*0.5f- _adaptPanelRt.anchoredPosition.y;

            if (height>0)
            {
                _adaptPanelRt.anchoredPosition = new Vector2(_adaptPanelRt.anchoredPosition.x, _adaptPanelRt.anchoredPosition.y+  height);
            }
        }
    }


    /// <summary>
    /// 结束编辑事件，TouchScreenKeyboard.isFocused为false的时候
    /// </summary>
    /// <param name="currentInputString"></param>
    private void OnEndEdit(string currentInputString)
    {
        _adaptPanelRt.anchoredPosition = _adaptPanelOriginPos;
    }

    /// <summary>
    /// 获取安卓平台上键盘的高度
    /// </summary>
    /// <returns></returns>
    public int AndroidGetKeyboardHeight()
    {
        using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").
                Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");

            using (AndroidJavaObject Rct = new AndroidJavaObject("android.graphics.Rect"))
            {
                View.Call("getWindowVisibleDisplayFrame", Rct);
                return Screen.height - Rct.Call<int>("height");
            }
        }
    }


    public float IOSGetKeyboardHeight()
    {
        return TouchScreenKeyboard.area.height;
    }
}