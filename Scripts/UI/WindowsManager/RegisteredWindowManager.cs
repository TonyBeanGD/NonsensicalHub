using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using LitJson;
using System.IO;
using System.Collections.Generic;

public class RegisteredWindowManager : MonoBehaviour
{
    private Transform _window_Registered;               //注册面板
    private Button _btn_CloseInRegisteredPanel;         //注册面板里的关闭按钮

    private InputField _ipf_UserName;                   //用户名输入框
    private InputField _ipf_Password;                   //密码输入框
    private InputField _ipf_RePassword;                 //确认密码输入框
    private InputField _ipf_RealName;                   //真实姓名输入框
    private InputField _ipf_PhoneNumber;                //手机号输入框
    private InputField _ipf_SMSVerificationCode;        //短信校验输入框
    private InputField _ipf_ImageVerificationCode;      //图形校验输入框
    private InputField _ipf_PromoterId;                 //推广人id输入框
    private SMSCodeButtonController _btn_SendSMSCode;                   //获取短信验证码按钮
    private Button _btn_GetImageCode;                   //图形验证码按钮
    private Button _btn_DoRegistered;                   //下一步按钮
    private MainMenuPanelManager _panel;

    private string _imageCode;

    public void Open()
    {
        _btn_GetImageCode.onClick?.Invoke();
        _imageCode = string.Empty;
        ResetUI();
        CheckIP();
        CheckAppInfo();
        _window_Registered.gameObject.SetActive(true);
    }

    private void OnChangeWindow(MainMenuPanelManager.Window window)
    {
        if (window == MainMenuPanelManager.Window.Registered)
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
        _window_Registered.gameObject.SetActive(false);
    }

    private void Close()
    {
        _window_Registered.gameObject.SetActive(false);
        ResetUI();
    }

    private void Init_Variable()
    {
        _window_Registered = transform;
        _btn_CloseInRegisteredPanel = _window_Registered.Find("img_Background").Find("btn_Close").GetComponent<Button>();

        Transform _InputGroup = _window_Registered.Find("img_Background").Find("InputArea").Find("InputGroup");
        _ipf_UserName = _InputGroup.Find("ipf_UserName").GetComponent<InputField>();
        _ipf_Password = _InputGroup.Find("ipf_Password").GetComponent<InputField>();
        _ipf_RePassword = _InputGroup.Find("ipf_RePassword").GetComponent<InputField>();
        _ipf_RealName = _InputGroup.Find("ipf_RealName").GetComponent<InputField>();
        _ipf_PhoneNumber = _InputGroup.Find("ipf_PhoneNumber").GetComponent<InputField>();
        _ipf_SMSVerificationCode = _InputGroup.Find("ipf_SMSVerificationCode").GetComponent<InputField>();
        _ipf_ImageVerificationCode = _InputGroup.Find("ipf_ImageVerificationCode").GetComponent<InputField>();
        _ipf_PromoterId = _InputGroup.Find("ipf_PromoterId").GetComponent<InputField>();
        _btn_SendSMSCode = _ipf_SMSVerificationCode.transform.Find("btn_GetVerificationCode").GetComponent<SMSCodeButtonController>();
        _btn_GetImageCode = _ipf_ImageVerificationCode.transform.Find("img_Code").GetComponent<Button>();

        _btn_DoRegistered = _InputGroup.Find("btn_DoRegistered").GetComponent<Button>();
    }

    private void Init_Listener()
    {
        _btn_CloseInRegisteredPanel.GetComponent<Button>().onClick.AddListener(() => this.Close());
        _btn_DoRegistered.GetComponent<Button>().onClick.AddListener(() => this.Onbtn_DoRegisterClick());

        _btn_SendSMSCode.Init(()=> 
        {
            HttpManager._Instance.RegisterGetVerificationCode(_ipf_PhoneNumber.text,(jsonData)=> 
            {
                if (jsonData["code"].ToString() == "1")
                {
                    _btn_SendSMSCode.Run();
                }
                else
                {
                    TipsManager._Instance.OpenTipsText(jsonData["msg"].ToString());
                }
            });
        });

        _btn_GetImageCode.onClick.AddListener(()=> HttpManager._Instance.GetValidateCode(_btn_GetImageCode.GetComponent<Image>(),(str)=> { _imageCode = str; }));
    }


    /// <summary>
    /// 检查IP 推广会用到
    /// </summary>
    private void CheckIP()
    {
        HttpManager._Instance.StartPost(@"sso/register/checkIP", null, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(CheckIP);
                return;
            }

            JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

            _ipf_PromoterId.gameObject.SetActive(jd["code"].ToString() != "1");

        });
    }

    private void CheckAppInfo()
    {
        _ipf_RealName.gameObject.SetActive(GameStatic.appInfo["isUseRealName"].ToString() == "1");
        _ipf_PhoneNumber.gameObject.SetActive(GameStatic.appInfo["isUsePhone"].ToString() == "1");
        _ipf_SMSVerificationCode.gameObject.SetActive(GameStatic.appInfo["isUseSmsCode"].ToString() == "1");
        _ipf_ImageVerificationCode.gameObject.SetActive(GameStatic.appInfo["isUseGraphVerifi"].ToString() == "1");
    }

    /// <summary>
    /// 进行注册按钮监听
    /// </summary>
    private void Onbtn_DoRegisterClick()
    {
        if (GameStatic.appInfo["isUseGraphVerifi"].ToString() == "1")
        {
            if (_imageCode.Equals(string.Empty) == true)
            {
                TipsManager._Instance.OpenTipsText("未成功获取验证码");
                return;
            }
            if (_imageCode.Equals(_ipf_ImageVerificationCode.text) == false)
            {
                TipsManager._Instance.OpenTipsText("验证码输入错误");
                return;
            }
        }
     

        Dictionary<string, string> form = new Dictionary<string, string>();
        form.Add("username", _ipf_UserName.text);
        form.Add("password", _ipf_Password.text);
        form.Add("retypePassword", _ipf_RePassword.text);
        if (_ipf_RealName.text != string.Empty)
        {
            form.Add("realName", _ipf_RealName.text);
        }
        if (_ipf_PhoneNumber.text != string.Empty)
        {
            form.Add("phone", _ipf_PhoneNumber.text);
        }
        if (_ipf_SMSVerificationCode.text != string.Empty)
        {
            form.Add("smsCode", _ipf_SMSVerificationCode.text);
        }
        if (_ipf_PromoterId.text != string.Empty)
        {
            form.Add("recommendedId", _ipf_PromoterId.text);
        }
        form.Add("sourceType", GameStatic.SourceType);
        HttpManager._Instance.StartPost(@"sso/register/doRegister", form, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(Onbtn_DoRegisterClick);
                return;
            }

            JsonData Temp = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
            JsonData jd = DataEncryptDecrypt.EncryptDecrypt.dataDecrypt(Temp.ToJson());
            JsonData jsonData = JsonMapper.ToObject(jd.ToJson());
            if (jsonData["code"].ToString() == "1")
            {
                LocalFileManager._Instance._GameData._Account = _ipf_UserName.text;

                _panel._OnChangeWindow(MainMenuPanelManager.Window.Login);
            }
            else
            {
                TipsManager._Instance.OpenWarningBox(jsonData["msg"].ToString()) ;
            }
        });
    }

    /// <summary>
    /// 重置ui状态
    /// </summary>
    private void ResetUI()
    {
        _ipf_UserName.text = string.Empty;
        _ipf_Password.text = string.Empty;
        _ipf_RePassword.text = string.Empty;
        _ipf_RealName.text = string.Empty;
        _ipf_PhoneNumber.text = string.Empty;
        _ipf_SMSVerificationCode.text = string.Empty;
        _ipf_ImageVerificationCode.text = string.Empty;
        _ipf_PromoterId.text = string.Empty;

        _btn_SendSMSCode.Reset();
    }
}
