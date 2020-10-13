using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using static UnityEngine.UI.InputField;

public class LoginWindowManager : MonoBehaviour
{
    private Transform _window_Login;         //登陆面板
    private Button _btn_Close;              //关闭按钮
    private Button _btn_GoRegisrered;       //前往注册按钮
    private Button _btn_ForgetPassword;     //忘记密码按钮
    private InputField _ipf_Account;        //账号输入框
    private InputField _ipf_Password;       //密码输入框
    private Button _btn_Login;              //登录按钮
    private Toggle _tog_SaveAccounts;       //保存账号选项

    private MainMenuPanelManager _panel;

    public void Open()
    {
        UpdateUIState();
        _window_Login.gameObject.SetActive(true);
    }

    public void Init(MainMenuPanelManager panel)
    {
        _panel = panel;
        _panel._OnChangeWindow += OnChangeWindow;
        Init_Variable();
        Init_Listener();
        _window_Login.gameObject.SetActive(false);
    }

    private void OnChangeWindow(MainMenuPanelManager.Window window)
    {
        if (window == MainMenuPanelManager.Window.Login)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    private void Init_Variable()
    {
        _window_Login = transform;
        _btn_Close = _window_Login.Find("img_Background").Find("btn_Close").GetComponent<Button>();
        _btn_GoRegisrered = _window_Login.Find("btn_GoRegistered").GetComponent<Button>();
        _btn_ForgetPassword = _window_Login.Find("btn_ForgetPassword").GetComponent<Button>();
        _ipf_Account = _window_Login.Find("ipf_Account").GetComponent<InputField>();
        _ipf_Password = _window_Login.Find("ipf_Password").GetComponent<InputField>();
        _btn_Login = _window_Login.Find("btn_Login").GetComponent<Button>();
        _tog_SaveAccounts = _window_Login.Find("tog_SaveAccounts").GetComponent<Toggle>();
    }

    private void Init_Listener()
    {
        _btn_Close.onClick.AddListener(() => { Close(); });
        _btn_Login.onClick.AddListener(() => { LoginNoSms(); });
        _btn_ForgetPassword.onClick.AddListener(() => { ForgetPassword(); });
        _btn_GoRegisrered.onClick.AddListener(() => { GoRegistered(); });
        _tog_SaveAccounts.onValueChanged.AddListener((isOn) => { ChangeRemember(isOn); });
    }

    /// <summary>
    /// 忘记密码按钮监听
    /// </summary>
    private void ForgetPassword()
    {
        _panel._OnChangeWindow(MainMenuPanelManager.Window.RetrievePassword);
    }

    /// <summary>
    /// 开启注册面板
    /// </summary>
    private void GoRegistered()
    {

        _panel._OnChangeWindow(MainMenuPanelManager.Window.Registered);
    }

    /// <summary>
    /// 登录，输入框不为空的情况下发送登录请求
    /// </summary>
    private void Login()
    {
        if (_ipf_Account.text == "" || _ipf_Password.text == "")
        {
            TipsManager._Instance.OpenTipsText("账号或密码为空");
            return;
        }

        Dictionary<string, string> formData = new Dictionary<string, string>();
        formData.Add("username", _ipf_Account.text);
        formData.Add("password", _ipf_Password.text);
        formData.Add("sourceType", GameStatic.SourceType);

        HttpManager._Instance.StartPost(@"sso/login/doLogin", formData, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(Login);
                return;
            }

            Debug.Log(unityWebRequest.downloadHandler.text);
            JsonData Temp = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
            JsonData jsonData = DataEncryptDecrypt.EncryptDecrypt.dataDecrypt(Temp.ToJson());

            if (jsonData["code"].ToString() == "1")
            {
                //登陆成功时
                LocalFileManager._Instance._GameData._Account = _ipf_Account.text;
                LocalFileManager._Instance._GameData._Token = jsonData["result"]["token"].ToString();
                LocalFileManager._Instance._GameData._IsLogin = true;

                HttpManager._Instance.SetToken(jsonData["result"]["token"].ToString());

                GameStatic.OnLogin?.Invoke();
                Close();
            }
            else
            {
                //登录失败时
                TipsManager._Instance.OpenWarningBox(jsonData["msg"].ToString());
            }
        });
    }

    private void LoginNoSms()
    {
        if (_ipf_Account.text == "" || _ipf_Password.text == "")
        {
            TipsManager._Instance.OpenTipsText("账号或密码为空");
            return;
        }

        Dictionary<string, string> formData = new Dictionary<string, string>();
        formData.Add("username", _ipf_Account.text);
        formData.Add("password", _ipf_Password.text);
        formData.Add("sourceType", GameStatic.SourceType);

        HttpManager._Instance.StartPost(@"sso/login/doLogin/noSms", formData, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(LoginNoSms);
                return;
            }

            Debug.Log(unityWebRequest.downloadHandler.text);
            JsonData Temp = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
            JsonData jsonData = DataEncryptDecrypt.EncryptDecrypt.dataDecrypt(Temp.ToJson());

            if (jsonData["code"].ToString() == "1")
            {
                LocalFileManager._Instance._GameData._Account = _ipf_Account.text;
                LocalFileManager._Instance._GameData._Token = jsonData["result"]["token"].ToString();
                LocalFileManager._Instance._GameData._IsLogin = true;

                HttpManager._Instance.SetToken(jsonData["result"]["token"].ToString());

                GameStatic.OnLogin?.Invoke();
                Close();
            }
            else
            {
                TipsManager._Instance.OpenWarningBox(jsonData["msg"].ToString());
            }
        });
    }

    private void Close()
    {
        _window_Login.gameObject.SetActive(false);
    }

    private void ChangeRemember(bool isOn)
    {
        LocalFileManager._Instance._GameData._RememberToken = isOn;
    }

    private void UpdateUIState()
    {
        _ipf_Account.text = LocalFileManager._Instance._GameData._Account;
        _ipf_Password.text = "";
        _tog_SaveAccounts.isOn = LocalFileManager._Instance._GameData._RememberToken;
    }
}
