using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SafePasswordManager : MonoBehaviour
{
    public enum State
    {
        NewSafeFirst,
        NewSafeScened,
        OpenSafe
    }

    private Transform _window_SafePassword;
    private MainMenuPanelManager _panel;
    private Text[] txt_inputShows;
    private Text txt_Tips;
    private Button _btn_Close;

    private string lastPassword;

    private string password;

    private State crtState;

    private void Open(State state)
    {
        ChangeState( state);
        _window_SafePassword.gameObject.SetActive(true);
    }

    private void OnChangeWindow(MainMenuPanelManager.Window window)
    {
        if (window == MainMenuPanelManager.Window.NewSafe)
        {
            Open(State.NewSafeFirst);
        }
        else if(window == MainMenuPanelManager.Window.OpenSafe)
        {
            Open(State.OpenSafe);
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
        _window_SafePassword.gameObject.SetActive(false);
    }

    private void Init_Variable()
    {
        _window_SafePassword = transform;

        txt_Tips = _window_SafePassword.Find("txt_Tips").GetComponent<Text>();

        {
            Transform InputShows = _window_SafePassword.Find("InputShows");

            txt_inputShows = new Text[4];


            for (int i = 0; i < InputShows.childCount; i++)
            {
                txt_inputShows[i] = InputShows.GetChild(i).GetComponent<Text>();
            }
        }

        _btn_Close = _window_SafePassword.Find("img_Background").Find("btn_Close").GetComponent<Button>() ;
    }

    private void Init_Listener()
    {
        {
            Transform InputButton = _window_SafePassword.Find("InputButtons");
            for (int i = 0; i < InputButton.childCount; i++)
            {
                int index = i;
                InputButton.GetChild(i).GetComponent<Button>().onClick.AddListener(()=>OnInputButtonClick(index+1));
            }
        }

        _btn_Close.onClick.AddListener(()=>_window_SafePassword.gameObject.SetActive(false));
    }

    /// <summary>
    /// 返回主菜单
    /// </summary>
    private void Close()
    {
        Reset();
        _window_SafePassword.gameObject.SetActive(false);
    }

    private void Reset()
    {
       ChangeShow( string.Empty);
        for (int i = 0; i < txt_inputShows.Length; i++)
        {
            txt_inputShows[i].text = string.Empty;
        }
    }

    private void ChangeShow(string Password)
    {
        for (int i = 0; i < txt_inputShows.Length; i++)
        {
            if (i<Password.Length)
            {
                txt_inputShows[i].text = "*";
            }
            else
            {
                txt_inputShows[i].text = "";
            }
        }
        password = Password;
    }

    private void ChangeState(State state)
    {
        switch (state)
        {
            case State.NewSafeFirst:
                {
                    txt_Tips.text = "首次使用保险箱,请设置新密码";
                }
                break;
            case State.NewSafeScened:
                {
                    txt_Tips.text = "请再次输入密码";
                }
                break;
            case State.OpenSafe:
                {
                    txt_Tips.text = "请输入保险箱密码";
                }
                break;
            default:
                break;
        }

        crtState = state;
    }

    private void OnInputButtonClick(int index)
    {
        switch (index)
        {
            case 11:
                {
                    if (password.Length>0)
                    {
                        ChangeShow(password.Substring(0,password.Length-1));
                    }
                }
                break;
            case 12:
                {
                    OnComfirm();
                }
                break;
            default:
                {
                    if (password.Length<4)
                    {
                        ChangeShow(password+(index % 10).ToString());
                    }
                }
                break;
        }
    }

    private void OnComfirm()
    {
        switch (crtState)
        {
            case State.NewSafeFirst:
                {
                    lastPassword = password;
                    ChangeShow(string.Empty);
                    ChangeState(State.NewSafeScened);
                }
                break;
            case State.NewSafeScened:
                {
                    SetNewPassword();
                }
                break;
            case State.OpenSafe:
                {
                    OpenSafePanel();
                }
                break;
            default:
                Debug.LogError("Undefine Enum");
                break;
        }
    }


    /// <summary>
    /// 设置保险箱新密码
    /// </summary>
    private void SetNewPassword()
    {
        Dictionary<string, string> form = new Dictionary<string, string>();
        form.Add("safePass",lastPassword);
        form.Add("safePassCopy", password);

        HttpManager._Instance.StartPost(@"member/center/setSafePass", form, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(SetNewPassword);
                return;
            }

            JsonData jsonData = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
            if (jsonData["code"].ToString() == "1")
            {
                Reset() ;

                GameStatic.OnSetSafeMoneyText?.Invoke(jsonData["result"]["safeMoney"].ToString(), jsonData["result"]["walletMoney"].ToString());

                GameStatic.OnChangePanel?.Invoke(Panel.Safe);
            }
            else
            {
                TipsManager._Instance.OpenWarningBox(jsonData["msg"].ToString());
            }
        });
    }

    /// <summary>
    /// 开启保险箱面板
    /// </summary>
    private void OpenSafePanel()
    {
        Dictionary<string, string> form = new Dictionary<string, string>();
        form.Add("safePass", password);
        HttpManager._Instance.StartPost(@"member/center/checkSafePass", form, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(OpenSafePanel);
                return;
            }

            JsonData jsonData = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
            if (jsonData["code"].ToString() == "1")
            {
                Reset();

                GameStatic.OnSetSafeMoneyText?.Invoke(jsonData["result"]["safeMoney"].ToString(), jsonData["result"]["walletMoney"].ToString());

                GameStatic.OnChangePanel?.Invoke(Panel.Safe);
            }
            else
            {
                TipsManager._Instance.OpenWarningBox(jsonData["msg"].ToString());
            }
        });
    }
}