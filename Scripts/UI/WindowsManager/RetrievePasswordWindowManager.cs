using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using LitJson;

public class RetrievePasswordWindowManager : MonoBehaviour
{
    /// <summary>
    /// 找回密码的主面板
    /// </summary>
    private Transform _window_RetrievePassword;

    /// <summary>
    /// 关闭按钮
    /// </summary>
    private Button _btn_Close;

    /// <summary>
    /// 电话输入
    /// </summary>
    private InputField _ipf_PhoneNumber;

    /// <summary>
    /// 验证码输入
    /// </summary>
    private InputField _ipf_GetVerificationCode;

    /// <summary>
    /// 重新获取验证码
    /// </summary>
    private Button _btn_GetVerificationCode;

    /// <summary>
    /// 错误信息文本
    /// </summary>
    private Text _txt_Error;

    /// <summary>
    /// 继续按钮
    /// </summary>
    private Button _btn_NextStep;

    /// <summary>
    /// 计时器
    /// </summary>
    private float _Timer = 60;

    /// <summary>
    /// 计时文本
    /// </summary>
    private Text _txt_Reacquire;

    /// <summary>
    /// 是否开启计时
    /// </summary>
    private bool _isBeginTimer = false;

    private MainMenuPanelManager _panel;

    public void Open()
    {
        _window_RetrievePassword.gameObject.SetActive(true);
    }

    private void OnChangeWindow(MainMenuPanelManager.Window window)
    {
        if (window == MainMenuPanelManager.Window.RetrievePassword)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    public void Init(MainMenuPanelManager panel)
    {
        _panel = panel;
        _panel._OnChangeWindow += OnChangeWindow;

        Init_Variable();
        Init_Listener();
        Reset();

        _window_RetrievePassword.gameObject.SetActive(false);
    }

    void Update()
    {
        UpdateTextTimer();
    }

    private void Init_Variable()
    {
        _window_RetrievePassword = transform;
        _btn_Close = _window_RetrievePassword.Find("img_Background").Find("btn_Close").GetComponent<Button>();
        _ipf_PhoneNumber = _window_RetrievePassword.Find("ipf_PhoneNumber").GetComponent<InputField>();
        _ipf_GetVerificationCode = _window_RetrievePassword.Find("ipf_GetVerificationCode").GetComponent<InputField>();
        _btn_GetVerificationCode = _ipf_PhoneNumber.transform.Find("btn_GetVerificationCode").GetComponent<Button>();
        _txt_Error = _window_RetrievePassword.Find("txt_Error").GetComponent<Text>();
        _btn_NextStep = _window_RetrievePassword.Find("btn_NextStep").GetComponent<Button>();
        _txt_Reacquire = _btn_GetVerificationCode.transform.Find("txt_Reacquire").GetComponent<Text>();
    }

    private void Init_Listener()
    {
        _btn_Close.onClick.AddListener(() => this.Close());
        _btn_GetVerificationCode.onClick.AddListener(() => this.On_btn_GetVerificationCodeClick());
        _btn_NextStep.onClick.AddListener(() => this.On_btn_NextStepClick());
    }

    /// <summary>
    /// 继续按钮的监听
    /// </summary>
    private void On_btn_NextStepClick()
    {
        Dictionary<string, string> form = new Dictionary<string, string>();
        form.Add("phone", _ipf_PhoneNumber.text);
        form.Add("smsCode", _ipf_GetVerificationCode.text);

        HttpManager._Instance.StartPost(@"sso/common/checkSms", form,(unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(On_btn_NextStepClick);
                return;
            }

            JsonData Temp = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
            JsonData res = DataEncryptDecrypt.EncryptDecrypt.dataDecrypt(Temp.ToJson());

            if (res["code"].ToString() == "1")
            {
                TipsManager._Instance.OpenSuccessBox(res["msg"].ToString());
            }
            else
            {
                TipsManager._Instance.OpenWarningBox(res["msg"].ToString());
            }
        });
    }

    /// <summary>
    /// 重新获取验证码监听
    /// </summary>
    private void On_btn_GetVerificationCodeClick()
    {
        HttpManager._Instance.GetVerificationCode(_ipf_PhoneNumber.text,(jsonData)=>
        {
            if (jsonData["code"].ToString() == "1")
            {
                _txt_Reacquire.text = "重新获取 60";
                _btn_GetVerificationCode.GetComponent<Button>().interactable = false;
                _isBeginTimer = true;
            }
            else
            {
                Debug.LogWarning("获取验证码失败");
            }

            _txt_Error.text = jsonData["msg"].ToString();
        });
    }

    /// <summary>
    /// 更新计时文本
    /// </summary>
    private void UpdateTextTimer()
    {
        if (_isBeginTimer)
        {
            _Timer -= Time.deltaTime;
            _txt_Reacquire.text = "重新获取" + _Timer.ToString("f2");

            if (_Timer <= 0)
            {
                _Timer = 60;
                _isBeginTimer = false;
                _txt_Reacquire.text = "获取验证码";
                _btn_GetVerificationCode.GetComponent<Button>().interactable = true;
            }
        }
    }
   
    private void Close()
    {
        Reset();
        _window_RetrievePassword.gameObject.SetActive(false);
    }

    /// <summary>
    /// 重置
    /// </summary>
    private void Reset()
    {
        _ipf_PhoneNumber.text = string.Empty;
        _ipf_GetVerificationCode.text = string.Empty;
        _txt_Error.text = string.Empty;
    }
}
