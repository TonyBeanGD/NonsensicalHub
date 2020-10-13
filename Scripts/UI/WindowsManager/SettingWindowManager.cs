using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using LitJson;
using System.Linq;
using System.IO;

public class SettingWindowManager : MonoBehaviour
{
    private Transform _window_Setting;

    private Transform _ButtonCloseInMainSetting;

    private Slider _sld_BGMVolume;
    private Slider _sld_SEVolume;
    private Slider _sld_CSVolume;

    private InputField _OldPassword;

    private InputField _NewPassword;

    private InputField _ConfirmPassword;

    private Button _DoneButton;

    private Text _txt_TipText;

    /// <summary>
    /// 所有的面板   0退出 1密码设置 2音乐设置
    /// </summary>
    private List<Transform> _PanelList = new List<Transform>();

    /// <summary>
    /// 左侧功能按钮list   0退出按钮 1密码设置 2音乐设置
    /// </summary>
    private List<Button> _LeftButtonList = new List<Button>();

    /// <summary>
    /// 左侧按钮高光list
    /// </summary>
    private List<Image> _LeftHighlightList = new List<Image>();

    /// <summary>
    /// 左侧文字list
    /// </summary>
    private List<Image> _LeftTextList = new List<Image>();

    /// <summary>
    /// 记住账户 
    /// </summary>
    private Toggle _RememberAccountToggle;

    private Button _btn_Logout;
    private MainMenuPanelManager _panel;


    public void Open()
    {
        UpdateUIState();
        _window_Setting.gameObject.SetActive(true);
    }

    private void OnChangeWindow(MainMenuPanelManager.Window window)
    {
        if (window == MainMenuPanelManager.Window.Setting)
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
        Init_State();
        _window_Setting.gameObject.SetActive(false);
    }

    private void Init_Variable()
    {
        _window_Setting = transform;
        _ButtonCloseInMainSetting = _window_Setting.Find("img_Background").Find("btn_Close");

        for (int i = 0; i < _window_Setting.Find("img_Background").Find("Panels").transform.childCount; i++)
        {
            _PanelList.Add(_window_Setting.Find("img_Background").Find("Panels").transform.GetChild(i));
        }

        _LeftButtonList = _window_Setting.Find("img_Background").Find("Buttons").GetComponentsInChildren<Button>().ToList();

        _LeftHighlightList.Add(_LeftButtonList[0].transform.GetChild(0).GetComponent<Image>());
        _LeftHighlightList.Add(_LeftButtonList[1].transform.GetChild(0).GetComponent<Image>());
        _LeftHighlightList.Add(_LeftButtonList[2].transform.GetChild(0).GetComponent<Image>());

        _LeftTextList.Add(_LeftButtonList[0].transform.GetChild(1).GetComponent<Image>());
        _LeftTextList.Add(_LeftButtonList[1].transform.GetChild(1).GetComponent<Image>());
        _LeftTextList.Add(_LeftButtonList[2].transform.GetChild(1).GetComponent<Image>());

        _sld_BGMVolume = _PanelList[2].Find("BGMVolume").GetChild(0).GetComponent<Slider>();
        _sld_SEVolume = _PanelList[2].Find("SEVolume").GetChild(0).GetComponent<Slider>();
        _sld_CSVolume = _PanelList[2].Find("CSVolume").GetChild(0).GetComponent<Slider>();

        _RememberAccountToggle = _PanelList[0].Find("tog_RememberAccount").GetComponent<Toggle>();
        _btn_Logout = _PanelList[0].Find("btn_Logout").GetComponent<Button>();

        _OldPassword = _PanelList[1].Find("ipf_OldPassword").GetComponent<InputField>();
        _NewPassword = _PanelList[1].Find("ipf_NewPassword").GetComponent<InputField>();
        _ConfirmPassword = _PanelList[1].Find("ipf_ConfirmPassword").GetComponent<InputField>();
        _DoneButton = _PanelList[1].Find("btn_Confirm").GetComponent<Button>();
        _txt_TipText = _PanelList[1].Find("img_Error").GetChild(1).GetComponent<Text>();
    }

    private void Init_Listener()
    {
        _ButtonCloseInMainSetting.GetComponent<Button>().onClick.AddListener(() => this.Close());

        _LeftButtonList[0].onClick.AddListener(() => this.ChangePanel(0));
        _LeftButtonList[1].onClick.AddListener(() => this.ChangePanel(1));
        _LeftButtonList[2].onClick.AddListener(() => this.ChangePanel(2));

        _sld_BGMVolume.onValueChanged.AddListener((float value) => this.OnBGMValueChange(value));
        _sld_SEVolume.onValueChanged.AddListener((float value) => this.OnSEValueChange(value));
        _sld_CSVolume.onValueChanged.AddListener((float value) => this.OnCSValueChange(value));

        _RememberAccountToggle.onValueChanged.AddListener((bool isOn) => OnToggleValueChange(isOn, 1));

        _btn_Logout.onClick.AddListener(() =>
        {
            LocalFileManager._Instance._GameData._RememberToken = false;
            LocalFileManager._Instance._GameData._IsLogin = false;
            Close();
            GameStatic.OnLogout?.Invoke();
        });

        _DoneButton.onClick.AddListener(() => this.OnDoneButtonClick());
    }

    private void Init_State()
    {
        Reset();
    }

    private void OnBGMValueChange(float value)
    {

        LocalFileManager._Instance._GameData._BGMVolume = value;
        AudioSourceManager._Instance.SetBGMVolume(value);

    }

    private void OnSEValueChange(float value)
    {
        LocalFileManager._Instance._GameData._SEVolume = value;
        AudioSourceManager._Instance.SetSEVolume(value);
    }  
    
    private void OnCSValueChange(float value)
    {
        LocalFileManager._Instance._GameData._CSVolume = value;
        AudioSourceManager._Instance.SetCSVolume(value);
    }

    private void OnToggleValueChange(bool isOn, int index)
    {
        switch (index)
        {
            case 1:
                {
                    LocalFileManager._Instance._GameData._RememberToken = isOn;
                }
                break;
            default:
                {
                    print("Undefined Index");
                }
                break;
        }
    }

    /// <summary>
    /// 完成按钮监听
    /// </summary>
    private void OnDoneButtonClick()
    {
        if (!LocalFileManager._Instance._GameData._IsLogin)
        {
            TipsManager._Instance.OpenWarningBox("尚未登录");
            return;
        }

        Dictionary<string, string> form = new Dictionary<string, string>();
        form.Add("newPwd", _NewPassword.text);
        form.Add("oldPwd", _OldPassword.text);
        form.Add("retypePwd", _ConfirmPassword.text);

        HttpManager._Instance.StartPost(@"member/center/modifyPwd", form, (unityWebRequest) =>
         {
             if (unityWebRequest == null)
             {
                 TipsManager._Instance.OpenReConnectTipsPanel(OnDoneButtonClick);
                 return;
             }

             JsonData json = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

             if (json["code"].ToString() == "1")
             {
                 _OldPassword.text = string.Empty;
                 _NewPassword.text = string.Empty;
                 _ConfirmPassword.text = string.Empty;
             }

             _txt_TipText.transform.parent.gameObject.SetActive(true);
             _txt_TipText.text = json["msg"].ToString();
         });
    }

    private void ChangePanel(int count)
    {
        for (int i = 0; i < _LeftButtonList.Count; i++)
        {
            if (i == count)
            {
                _LeftHighlightList[i].enabled = true;
                _LeftTextList[i].color = Color.black;
                _PanelList[i].gameObject.SetActive(true);
            }
            else
            {
                _LeftHighlightList[i].enabled = false;
                _LeftTextList[i].color = Color.white;
                _PanelList[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 两个关闭按钮的监听
    /// </summary>
    private void Close()
    {
        LocalFileManager._Instance.SaveArchive();
        Reset();
        _window_Setting.gameObject.SetActive(false);
    }

    private void UpdateUIState()
    {
        _LeftButtonList[0].gameObject.SetActive(LocalFileManager._Instance._GameData._IsLogin);
        _LeftButtonList[1].gameObject.SetActive(LocalFileManager._Instance._GameData._IsLogin);

        ChangePanel(LocalFileManager._Instance._GameData._IsLogin ? 0 : 2);

        _sld_BGMVolume.value = (float)LocalFileManager._Instance._GameData._BGMVolume;
        _sld_SEVolume.value = (float)LocalFileManager._Instance._GameData._SEVolume;
        _sld_CSVolume.value = (float)LocalFileManager._Instance._GameData._CSVolume;
        _RememberAccountToggle.isOn = LocalFileManager._Instance._GameData._RememberToken;
    }

    private void Reset()
    {
        _OldPassword.text = string.Empty;
        _NewPassword.text = string.Empty;
        _ConfirmPassword.text = string.Empty;
        _txt_TipText.text = string.Empty;
        _txt_TipText.transform.parent.gameObject.SetActive(false);
    }
}
