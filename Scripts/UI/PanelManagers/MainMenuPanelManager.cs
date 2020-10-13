using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using LitJson;
using UnityEngine.Networking;
using System;

public class MainMenuPanelManager : MonoBehaviour
{
    public enum Window
    {
        None,
        Login,
        Registered,
        RetrievePassword,
        Event,                       //活动窗口
        NewSafe,
        OpenSafe,
        Message,                     //消息窗口
        Setting,                     //设置窗口
    }

    private Transform _Panel_MainMenu;

    private Transform _Header;                                                      //UI头部
    private Transform _UIBody;                                                      //UI主体
    private Transform _Footer;                                                      //UI下部

    private Button _btn_Registered;                                             //注册按钮
    private Button _btn_Login;                                                  //登录按钮
    private Button _btn_Refresh;                                                //刷新按钮
    private Button _btn_Setting;                                                //设置按钮
    private Button _btn_Copy;                                                   //复制按钮
    private Button _btn_Avatar;                                                 //头像按钮
    private Button _btn_Promotion;                                              //推广赚钱按钮
    private Button _btn_Event;                                                  //活动按钮
    private Button _btn_WashCode;                                               //洗码按钮
    private Button _btn_Message;                                                //消息按钮
    private Button _btn_CustomerService;                                        //客服按钮
    private Button _btn_Safe;                                                   //保险箱按钮
    private Button _btn_withdraw;                                               //提现按钮
    private Button _btn_Recharge;                                               //充值按钮
    private Text _txt_Name;                                                     //姓名文本
    private Text _txt_MoneyCount;                                               //金钱文本

    private Transform _Logged;                                                      //登录后显示的VIP按钮
    private Transform _NotLogged;                                                   //登录前的按钮

    private Image _img_VIPText;

    private Transform _ButtonsParent;                                           //按钮父级
    private Transform _PanelsParent;                                            //面板父级

    private Transform _ScenedLevelPanels;

    private LoginWindowManager _window_Login;
    private RegisteredWindowManager _window_Registered;
    private RetrievePasswordWindowManager _window_RetrievePassword;
    private EventWindowManager _window_Event;
    private SafePasswordManager _window_SafePassword;
    private MessageWindowManager _window_Message;
    private SettingWindowManager _window_Setting;

    public Action<Window> _OnChangeWindow;

    private void ChangePanel(Panel panel)
    {
        if (panel == Panel.MainMenu)
        {
            ChangeGamePanel(0);
            Onbtn_RefreshClick();
            _OnChangeWindow(Window.None);
            _Panel_MainMenu.gameObject.SetActive(true);

        }
        else
        {
            if (_Panel_MainMenu.gameObject.activeSelf == true)
            {
                _Panel_MainMenu.gameObject.SetActive(false);
            }
        }
    }

    void Awake()
    {
        GameStatic.OnChangePanel += ChangePanel;
        GameStatic.OnLogin += OnUserLogin;
        GameStatic.OnLogout += DoLogout;
        GameStatic.OnMainPanelInit += () => StartCoroutine(Init_State());
        GameStatic.OnDownScore += DownScore;
        Init_Variable();
        Init_Listener();
        InitWindows();
    }

    void Update()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.RightControl) && Input.GetKey(KeyCode.X) && Input.GetKey(KeyCode.I) && Input.GetKeyDown(KeyCode.A))
        {
            DownScore();
            TipsManager._Instance.OpenSuccessBox("测试用下分");
        }
#endif
    }

    #region Init
    /// <summary>
    /// 初始化变量
    /// </summary>
    private void Init_Variable()
    {

        _Panel_MainMenu = transform;

        _Header = _Panel_MainMenu.Find("Header");
        _UIBody = _Panel_MainMenu.Find("UIBody");
        _Footer = _Panel_MainMenu.Find("Footer");

        _ButtonsParent = _UIBody.GetChild(0).GetChild(0).GetChild(0);
        _PanelsParent = _UIBody.GetChild(1);

        _btn_Registered = _Header.Find("NotLogged").Find("btn_Registered").GetComponent<Button>();
        _btn_Login = _Header.Find("NotLogged").Find("btn_Login").GetComponent<Button>();
        _btn_Refresh = _Header.Find("img_Money").Find("btn_PlusMoney").GetComponent<Button>();
        _btn_Setting = _Header.Find("btn_Set").GetComponent<Button>();
        _btn_Copy = _Header.Find("img_WebsiteURL").Find("btn_Copy").GetComponent<Button>();
        _btn_Avatar = _Header.Find("img_Avatar").GetComponent<Button>();
        _btn_Promotion = _Footer.Find("btn_Promotion").GetComponent<Button>();
        _btn_Event = _Footer.Find("btn_Event").GetComponent<Button>();
        _btn_WashCode = _Footer.Find("btn_WashCode").GetComponent<Button>();
        _btn_Message = _Footer.Find("btn_Message").GetComponent<Button>();
        _btn_CustomerService = _Footer.Find("btn_CustomerService").GetComponent<Button>();
        _btn_Safe = _Footer.Find("btn_Safe").GetComponent<Button>();
        _btn_withdraw = _Footer.Find("btn_Withdraw").GetComponent<Button>();
        _btn_Recharge = _Footer.Find("btn_Recharge").GetComponent<Button>();
        _txt_MoneyCount = _Header.Find("img_Money").Find("txt_MoneyVolume").GetComponent<Text>();
        _txt_Name = _Header.Find("txt_Name").GetComponent<Text>(); ;

        _Logged = _Header.Find("Logged");
        _NotLogged = _Header.Find("NotLogged");
        _img_VIPText = _Logged.Find("img_VIPText").GetComponent<Image>();

        _ScenedLevelPanels = _Panel_MainMenu.Find("ScenedLevelPanels");

        _window_Login = _Panel_MainMenu.Find("window_Login").GetComponent<LoginWindowManager>();
        _window_Registered = _Panel_MainMenu.Find("window_Registered").GetComponent<RegisteredWindowManager>();
        _window_RetrievePassword = _Panel_MainMenu.Find("window_RetrievePassword").GetComponent<RetrievePasswordWindowManager>();
        _window_Event = _Panel_MainMenu.Find("window_Event").GetComponent<EventWindowManager>();
        _window_SafePassword = _Panel_MainMenu.Find("window_SafePassword").GetComponent<SafePasswordManager>();
        _window_Message = _Panel_MainMenu.Find("window_Message").GetComponent<MessageWindowManager>();
        _window_Setting = _Panel_MainMenu.Find("window_Setting").GetComponent<SettingWindowManager>();
    }

    /// <summary>
    /// 初始化监听
    /// </summary>
    private void Init_Listener()
    {
        _btn_Registered.onClick.AddListener(Onbtn_RegisteredClick);
        _btn_Login.onClick.AddListener(Onbtn_LoginClick);
        _btn_Refresh.onClick.AddListener(Onbtn_RefreshClick);
        _btn_Setting.onClick.AddListener(Onbtn_SetClick);
        _btn_Copy.onClick.AddListener(Onbtn_CopyClick);
        _btn_Avatar.onClick.AddListener(OpenPersonPanel);
        _btn_Promotion.onClick.AddListener(Onbtn_PromotionClick);
        _btn_Event.onClick.AddListener(Onbtn_EventClick);
        _btn_WashCode.onClick.AddListener(Onbtn_WashCodeClick);
        _btn_Message.onClick.AddListener(Onbtn_MessageClick);
        _btn_CustomerService.onClick.AddListener(Onbtn_CustomerServiceClick);
        _btn_Safe.onClick.AddListener(Onbtn_SafeClick);
        _btn_withdraw.onClick.AddListener(Onbtn_WithdrawClick);
        _btn_Recharge.onClick.AddListener(Onbtn_RechargeClick);

        _Logged.GetComponent<Button>().onClick.AddListener(OpenPersonPanel);
    }

    public void InitWindows()
    {
        _window_Login.Init(this);
        _window_Registered.Init(this);
        _window_RetrievePassword.Init(this);
        _window_Event.Init(this);
        _window_SafePassword.Init(this);
        _window_Message.Init(this);
        _window_Setting.Init(this);
    }

    /// <summary>
    /// 初始化UI状态
    /// </summary>
    private IEnumerator Init_State()
    {
        LoadingManager.LoadingStart(this.GetType().ToString());
        _Logged.gameObject.SetActive(false);
        _NotLogged.gameObject.SetActive(true);

        if (LocalFileManager._Instance._GameData._RememberToken == false)
        {
            LocalFileManager._Instance._GameData._IsLogin = false;
        }

        if (LocalFileManager._Instance._GameData._IsLogin == true)
        {
            yield return StartCoroutine(CheckToken());
        }

        yield return StartCoroutine(GetRollingMessages());
        yield return StartCoroutine(GetGameList());
        LoadingManager.LoadingComplete(this.GetType().ToString());
    }
    #endregion

    /// <summary>
    /// 检测Token
    /// </summary>
    private IEnumerator CheckToken()
    {
        if (LocalFileManager._Instance._GameData._RememberToken == false)
        {
            yield break;
        }

        bool isDone = false;

        Dictionary<string, string> form = new Dictionary<string, string>();

        form.Add("sourceType", GameStatic.SourceType);

        HttpManager._Instance.StartPost(@"sso/login/checkToken", form, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                LocalFileManager._Instance._GameData._IsLogin = false;
                isDone = true;
                return;
            }

            isDone = true;
            JsonData ciphertext = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
            JsonData plainText = DataEncryptDecrypt.EncryptDecrypt.dataDecrypt(ciphertext.ToJson());

            if (plainText["code"].ToString() == "1")
            {
                HttpManager._Instance.SetToken(LocalFileManager._Instance._GameData._Token);
                GameStatic.OnLogin?.Invoke();
            }
            else
            {
                LocalFileManager._Instance._GameData._IsLogin = false;
                TipsManager._Instance.OpenWarningBox(plainText["msg"].ToString());
            }
        });

        while (isDone == false)
        {
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void OnUserLogin()
    {
        GetVipLevel();
        _txt_Name.GetComponent<Text>().text = LocalFileManager._Instance._GameData._Account;
        _Logged.gameObject.SetActive(true);
        _NotLogged.gameObject.SetActive(false);
        Onbtn_RefreshClick();
    }


    private void DoLogout()
    {
        _txt_Name.GetComponent<Text>().text = "未登录";
        _Logged.gameObject.SetActive(false);
        _NotLogged.gameObject.SetActive(true);

        GetVipLevel();
        Onbtn_RefreshClick();

        GameStatic.OnChangePanel(Panel.MainMenu);
        HttpManager._Instance.StartPost(@"sso/login/doLogout", null, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(DoLogout);
                return;
            }

            JsonData Temp = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
            JsonData jsonData = DataEncryptDecrypt.EncryptDecrypt.dataDecrypt(Temp.ToJson());
            if (jsonData["code"].ToString() == "1")
            {
                Debug.Log(jsonData["msg"].ToString());
            }
            else
            {
                Debug.Log(jsonData["msg"].ToString());
            }
        });
    }

    private IEnumerator GetRollingMessages()
    {
        bool isOk = false;
        bool isReceive = false;

        while (true)
        {
            HttpManager._Instance.StartPost(@"not/common/getRollingAll", null, (unityWebRequest) =>
            {
                if (unityWebRequest == null)
                {
                    isReceive = true;
                    return;
                }

                JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
                if (jd["code"].ToString() == "1")
                {
                    List<string> ls = new List<string>();
                    for (int i = 0; i < jd["result"].Count; i++)
                    {
                        ls.Add(jd["result"][i]["text"].ToString());
                    }
                    RollingSubtitleEffect._Instance.SetList(ls);
                    isOk = true;
                }
                else
                {
                    Debug.LogWarning(jd["msg"].ToString());
                }
                isReceive = true;
            });

            while (isReceive == false)
            {
                yield return new WaitForSeconds(0.2f);
            }
            isReceive = false;

            if (isOk)
            {
                break;
            }

            yield return new WaitForSeconds(2);
        }
    }

    /// <summary>
    /// 切换主菜单内面板
    /// </summary>
    /// <param name="panelType"></param>
    private void ChangeGamePanel(int panelType)
    {
        for (int i = 0; i < _PanelsParent.childCount; i++)
        {
            if (i == panelType)
            {
                _PanelsParent.GetChild(i).gameObject.SetActive(true);
                _ButtonsParent.GetChild(i).GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                _PanelsParent.GetChild(i).gameObject.SetActive(false);
                _ButtonsParent.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
        }
    }


    /// <summary>
    /// 获取游戏链表
    /// </summary>
    private IEnumerator GetGameList()
    {
        bool callBack = false;
        bool isOk = false;
        while (true)
        {
            HttpManager._Instance.StartPost(@"not/common/getGameCategoryList", null, (unityWebRequest) =>
            {
                if (unityWebRequest == null)
                {
                    callBack = true;
                    return;
                }

                Debug.Log(unityWebRequest.downloadHandler.text);

                JsonData jsonData = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

                if (jsonData["code"].ToString() == "1")
                {
                    JsonData result = jsonData["result"];

                    for (int i = 0; i < result.Count; i++)
                    {
                        string icon = result[i]["icon"].ToString();
                        string hightlight = result[i]["brightIcon"].ToString();
                        JsonData gameList = result[i]["list"];

                        GameObject crtButton = Instantiate(InitialResourcesManager.MainMenuButton, _ButtonsParent, false);
                        DynamicResourceManager._Instance.StartSetTexture(crtButton.GetComponent<Image>(), icon, false);
                        DynamicResourceManager._Instance.StartSetTexture(crtButton.transform.GetChild(0).GetComponent<Image>(), hightlight, false);

                        int index = i;
                        crtButton.GetComponent<Button>().onClick.AddListener(() => { ChangeGamePanel(index); });

                        GameObject crtPanel = Instantiate(InitialResourcesManager.MainMenuPanel, _PanelsParent, false);

                        if (result[i]["gameType"].ToString() == "棋牌游戏")
                        {
                            crtPanel.GetComponentInChildren<GridLayoutGroup>().constraintCount = 1;
                            crtPanel.GetComponentInChildren<GridAutosize>()._GT = GridType.One;
                        }

                        if (result[i]["isPlatform"].ToString() == "0")
                        {
                            for (int j = 0; j < gameList.Count; j++)
                            {
                                GameObject tempGameObject = Instantiate(InitialResourcesManager.GamePanelContent, crtPanel.transform.GetChild(0), false);

                                DynamicResourceManager._Instance.StartSetTexture(tempGameObject.GetComponent<Image>(), gameList[j]["icon"].ToString());
                                if (gameList[j]["state"].ToString() == "1")
                                {
                                    int index2 = j;
                                    tempGameObject.GetComponent<Button>().onClick.AddListener(() => RunGame(gameList[index2]["id"].ToString()));
                                }
                                else
                                {
                                    Material mat = new Material(Shader.Find("Custom/UI/Base"));
                                    mat.SetFloat("_Saturation", 0);

                                    tempGameObject.GetComponent<Image>().material = mat;
                                    tempGameObject.GetComponent<Button>().onClick.AddListener(() => { TipsManager._Instance.OpenTipsText("游戏维护中"); });
                                }
                            }
                        }
                        else
                        {
                            if (result[i]["gameType"].ToString() == "棋牌游戏")
                            {
                                GameObject crtScenedLevelPanel = Instantiate(InitialResourcesManager.ScenedLevelPanel, _ScenedLevelPanels, false);
                                BuildScenedLevelPanel(crtScenedLevelPanel, result[i]);
                                crtScenedLevelPanel.SetActive(false);
                                for (int j = 0; j < gameList.Count; j++)
                                {
                                    GameObject tempGameObject = Instantiate(InitialResourcesManager.GamePanelContentSpecial, crtPanel.transform.GetChild(0), false);
                                    tempGameObject.GetComponentInChildren<Text>().text = gameList[j]["platformName"].ToString();

                                    tempGameObject.GetComponent<Image>().sprite = InitialResourcesManager.img_gr_woman[j+1];


                                    int index2 = j;
                                    tempGameObject.GetComponent<Button>().onClick.AddListener(() => OpenScenedLevelPanel(crtScenedLevelPanel, index2));
                                }
                            }
                            else
                            {
                                GameObject crtScenedLevelPanel = Instantiate(InitialResourcesManager.ScenedLevelPanel, _ScenedLevelPanels, false);
                                BuildScenedLevelPanel(crtScenedLevelPanel, result[i]);
                                crtScenedLevelPanel.SetActive(false);
                                for (int j = 0; j < gameList.Count; j++)
                                {
                                    GameObject tempGameObject = Instantiate(InitialResourcesManager.GamePanelContent, crtPanel.transform.GetChild(0), false);
                                    DynamicResourceManager._Instance.StartSetTexture(tempGameObject.GetComponent<Image>(), gameList[j]["icon"].ToString());
                                    int index2 = j;
                                    tempGameObject.GetComponent<Button>().onClick.AddListener(() => OpenScenedLevelPanel(crtScenedLevelPanel, index2));
                                }
                            }
                        }
                    }

                    isOk = true;
                }
                else
                {

                    Debug.LogWarning(jsonData["msg"].ToString());
                }

                callBack = true;
            });

            while (callBack == false)
            {
                yield return new WaitForSeconds(0.2f);
            }

            if (isOk)
            {
                break;
            }
            else
            {
                yield return new WaitForSeconds(2f);
            }
        }



        ChangeGamePanel(0);
    }

    private void BuildScenedLevelPanel(GameObject targetPanel, JsonData jsonData)
    {
        DynamicResourceManager._Instance.StartSetTexture(targetPanel.transform.Find("img_Title").GetComponent<Image>(), jsonData["titleIcon"].ToString());
        targetPanel.transform.Find("btn_Return").GetComponent<Button>().onClick.AddListener(() => { targetPanel.SetActive(false); });


        for (int i = 0; i < jsonData["list"].Count; i++)
        {
            GameObject crtButton = Instantiate(InitialResourcesManager.SecendLevelMenuButton, targetPanel.transform.Find("Buttons").GetChild(0), false);
            crtButton.transform.Find("txt_Type").GetComponent<Text>().text = jsonData["list"][i]["platformName"].ToString();
            DynamicResourceManager._Instance.StartSetTexture(crtButton.transform.Find("img_Icon").GetComponent<Image>(), jsonData["list"][i]["icon"].ToString());
            int index = i;
            crtButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                for (int j = 0; j < targetPanel.transform.Find("Buttons").GetChild(0).childCount; j++)
                {
                    if (j == index)
                    {
                        targetPanel.transform.Find("Panels").GetChild(j).gameObject.SetActive(true);
                        targetPanel.transform.Find("Buttons").GetChild(0).GetChild(j).GetChild(0).gameObject.SetActive(true);
                        targetPanel.transform.Find("Buttons").GetChild(0).GetChild(j).Find("txt_Type").GetComponent<Text>().color = Color.black;
                    }
                    else
                    {
                        targetPanel.transform.Find("Panels").GetChild(j).gameObject.SetActive(false);
                        targetPanel.transform.Find("Buttons").GetChild(0).GetChild(j).GetChild(0).gameObject.SetActive(false);
                        targetPanel.transform.Find("Buttons").GetChild(0).GetChild(j).Find("txt_Type").GetComponent<Text>().color = GameStatic.GoldColor;
                    }
                }
            });

            GameObject crtPanel = Instantiate(InitialResourcesManager.MainMenuPanel, targetPanel.transform.Find("Panels"), false);
            JsonData games = jsonData["list"][i]["games"];


            for (int j = 0; j < games.Count; j++)
            {
                GameObject tempGameObject = Instantiate(InitialResourcesManager.GamePanelContent, crtPanel.transform.GetChild(0), false);
                DynamicResourceManager._Instance.StartSetTexture(tempGameObject.GetComponent<Image>(), games[j]["icon"].ToString());
                if (games[j]["state"].ToString() == "1")
                {
                    int index2 = j;
                    tempGameObject.GetComponent<Button>().onClick.AddListener(() => RunGame(games[index2]["id"].ToString()));
                }
                else
                {
                    Material mat = new Material(Shader.Find("Custom/UI/Base"));
                    mat.SetFloat("_Saturation", 0);

                    tempGameObject.GetComponent<Image>().material = mat;
                    tempGameObject.GetComponent<Button>().onClick.AddListener(() => { TipsManager._Instance.OpenTipsText("游戏维护中"); });
                }
            }
        }
    }

    private void OpenScenedLevelPanel(GameObject targetPanel, int index)
    {
        targetPanel.SetActive(true);
        targetPanel.transform.Find("Buttons").GetChild(0).GetChild(index).GetComponent<Button>().onClick.Invoke();
    }

    /// <summary>
    /// 开启游戏
    /// </summary>
    /// <param name="gid"></param>
    private void RunGame(string gid)
    {

        if (LocalFileManager._Instance._GameData._IsLogin == false)
        {
            Onbtn_LoginClick();
            return;
        }

        if (GameStatic.IsGameOpening == true)
        {
            return;
        }
        GameStatic.IsGameOpening = true;


        TipsManager._Instance.StartLoading();
        Dictionary<string, string> form = new Dictionary<string, string>();
        form.Add("id", gid);
        HttpManager._Instance.StartPost(@"game/run", form, (unityWebRequest) =>
         {
             if (unityWebRequest == null)
             {
                 GameStatic.IsGameOpening = false;
                 TipsManager._Instance.StopLoading();
                 TipsManager._Instance.OpenReConnectTipsPanel(() => RunGame(gid));
                 return;
             }
             JsonData jsonData = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

             if (jsonData["code"].ToString() == "1")
             {
                 AudioSourceManager._Instance.PlayCharacterSpeech(InitialResourcesManager.aud_StartGame);
                 UniWebController._Instance.OpenTestPage(jsonData["result"]["gameUrl"].ToString());
             }
             else
             {
                 GameStatic.IsGameOpening = false;
                 TipsManager._Instance.StopLoading();

                 TipsManager._Instance.OpenWarningBox(jsonData["msg"].ToString());
             }
         });
    }

    /// <summary>
    /// 下分：退出游戏
    /// </summary>
    private void DownScore()
    {
        HttpManager._Instance.StartPost(@"game/downScore", null, (unityWebRequest) =>
          {
              if (unityWebRequest == null)
              {
                  TipsManager._Instance.OpenReConnectTipsPanel(DownScore);
                  return;
              }

              JsonData jsonData = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

              if (jsonData["code"].ToString() == "1")
              {
                  Onbtn_RefreshClick();
                  Debug.Log(unityWebRequest.downloadHandler.text);
              }
              else
              {
                  Debug.LogWarning(unityWebRequest.downloadHandler.text);
              }
          });

    }

    private void GetVipLevel()
    {
        if (LocalFileManager._Instance._GameData._IsLogin == false)
        {
            _img_VIPText.sprite = InitialResourcesManager.img_gr_VIP[1];
            return;
        }

        Dictionary<string, string> form = new Dictionary<string, string>();
        form.Add("index", "0");

        HttpManager._Instance.StartPost(@"member/center/getTotalInfo", form, (unityWebRequest) =>
         {
             if (unityWebRequest == null)
             {
                 TipsManager._Instance.OpenReConnectTipsPanel(GetVipLevel);
                 return;
             }

             JsonData jsonData = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

             if (jsonData["code"].ToString() == "1")
             {
                 string temp = jsonData["result"]["getGameMemberInfo"]["levelId"].ToString();
                 _img_VIPText.sprite = InitialResourcesManager.img_gr_VIP[int.Parse(temp)];
             }
             else
             {
                 Debug.LogWarning(unityWebRequest.downloadHandler.text);
             }
         });

    }


    #region ButtonClickListener

    /// <summary>
    /// 开启个人中心面板
    /// </summary>
    private void OpenPersonPanel()
    {
        if (LocalFileManager._Instance._GameData._IsLogin)
        {
            GameStatic.OnChangePanel?.Invoke(Panel.PersonCenter);
        }
        else
        {
            Onbtn_RegisteredClick();
        }
    }

    /// <summary>
    /// 注册监听
    /// </summary>
    private void Onbtn_RegisteredClick()
    {
        _OnChangeWindow(Window.Registered);
    }

    /// <summary>
    /// 登录监听
    /// </summary>
    private void Onbtn_LoginClick()
    {
        _OnChangeWindow(Window.Login);
    }

    /// <summary>
    /// 刷新钱数监听
    /// </summary>
    private void Onbtn_RefreshClick()
    {
        _txt_MoneyCount.GetComponent<Text>().text = "刷新中。。";

        HttpManager._Instance.StartGetBanance((balance) => _txt_MoneyCount.GetComponent<Text>().text = balance);
    }

    /// <summary>
    /// 复制监听
    /// </summary>
    private void Onbtn_CopyClick()
    {
        GameStatic.CopyString(GameStatic.appInfo["url"].ToString());
    }

    /// <summary>
    /// 设置监听
    /// </summary>
    private void Onbtn_SetClick()
    {
        _OnChangeWindow(Window.Setting);
    }

    /// <summary>
    /// 推广赚钱监听
    /// </summary>
    private void Onbtn_PromotionClick()
    {
        if (LocalFileManager._Instance._GameData._IsLogin == false)
        {
            Onbtn_LoginClick();
            return;
        }
        GameStatic.OnChangePanel?.Invoke(Panel.Promotion);
    }

    /// <summary>
    /// 活动监听
    /// </summary>
    private void Onbtn_EventClick()
    {
        _OnChangeWindow(Window.Event);
    }

    /// <summary>
    /// 洗码监听
    /// </summary>
    private void Onbtn_WashCodeClick()
    {
        if (!LocalFileManager._Instance._GameData._IsLogin)
        {
            Onbtn_LoginClick();
            return;
        }

        GameStatic.OnChangePanel?.Invoke(Panel.WashCode);
    }

    /// <summary>
    /// 消息监听
    /// </summary>
    private void Onbtn_MessageClick()
    {
        if (!LocalFileManager._Instance._GameData._IsLogin)
        {
            Onbtn_LoginClick();
            return;
        }
        _OnChangeWindow(Window.Message);
    }

    /// <summary>
    /// 客服监听
    /// </summary>
    private void Onbtn_CustomerServiceClick()
    {
        if (!LocalFileManager._Instance._GameData._IsLogin)
        {
            Onbtn_LoginClick();
            return;
        }
        GameStatic.OnChangePanel?.Invoke(Panel.CustomerService);
    }

    /// <summary>
    /// 保险箱监听
    /// </summary>
    private void Onbtn_SafeClick()
    {
        if (!LocalFileManager._Instance._GameData._IsLogin)
        {
            Onbtn_LoginClick();
            return;
        }

        HttpManager._Instance.StartPost(@"member/center/judgeFirstSafe", null, (unityWebRequest) =>
          {
              if (unityWebRequest == null)
              {
                  TipsManager._Instance.OpenReConnectTipsPanel(Onbtn_SafeClick);
                  return;
              }

              JsonData jsonData = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

              if (jsonData["result"]["isFirst"].ToString() == "1")
              {
                  _OnChangeWindow(Window.NewSafe);
              }
              else
              {
                  _OnChangeWindow(Window.OpenSafe);
              }
          });
    }

    /// <summary>
    /// 提现监听
    /// </summary>
    private void Onbtn_WithdrawClick()
    {
        if (!LocalFileManager._Instance._GameData._IsLogin)
        {
            Onbtn_LoginClick();
            return;
        }
        else
        {
            GameStatic.OnChangePanel?.Invoke(Panel.Withdraw);
        }

    }

    /// <summary>
    /// 充值监听
    /// </summary>
    private void Onbtn_RechargeClick()
    {
        if (!LocalFileManager._Instance._GameData._IsLogin)
        {
            Onbtn_LoginClick();
            return;
        }
        GameStatic.OnChangePanel?.Invoke(Panel.Recharge);
    }
    #endregion
}
