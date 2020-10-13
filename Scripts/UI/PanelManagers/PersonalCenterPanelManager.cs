using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using UnityEngine.Networking;
using System.Collections;

public class PersonalCenterPanelManager : MonoBehaviour
{
    #region private Attributes
    /// <summary>
    /// 个人中心面板
    /// </summary>
    private Transform _Panel_PersonalCenter;

    /// <summary>
    /// 按钮集合   0个人信息 1游戏记录 2账户明细 3个人报表
    /// </summary>
    private List<Button> _ButtonList = new List<Button>();

    /// <summary>
    /// 按钮选中图片集合 
    /// </summary>
    private List<Image> _HightlightImageList = new List<Image>();

    /// <summary>
    /// 需要显示的面板集合 0个人信息 1游戏记录 2账户明细 3个人报表
    /// </summary>
    private List<Transform> _PanelList = new List<Transform>();

    /// <summary>
    /// 返回按钮
    /// </summary>
    private Transform _btn_Return;

    /// <summary>
    /// 编辑和完成按钮集合 0编辑 1完成
    /// </summary>
    private List<Button> _EditAndCompleteButton = new List<Button>();

    /// <summary>
    /// 是否是编辑模式
    /// </summary>
    private bool _isEdit = false;

    /// <summary>
    /// 个人信息的值的集合 0账号 1昵称 2姓名 3性别 4电话 5QQ 6邮箱 7微信
    /// </summary>
    private List<Text> _PersonalInfoVauleText = new List<Text>();

    /// <summary>
    /// 个人信息编辑的集合 0账号 1昵称 2姓名 3性别 4电话 5QQ 6邮箱 7微信
    /// </summary>
    private List<InputField> _PersonalInfoEdit = new List<InputField>();

    /// <summary>
    /// 个人信息里的领取按钮 0周礼金 1月礼金 2晋级礼金
    /// </summary>
    private List<Button> _PersonalReceiveButtons = new List<Button>();

    /// <summary>
    /// VIP滑动条
    /// </summary>
    private Image _VIPSlider;

    /// <summary>
    /// 需要更改的VIP图片
    /// </summary>
    private Image _img_VIPInfo;

    /// <summary>
    /// 图标下的VIP等级
    /// </summary>
    private Image _img_VIP;

    /// <summary>
    /// 当前VIP等级
    /// </summary>
    private Image _img_CurrentLevel;

    /// <summary>
    /// 下一个VIP等级
    /// </summary>
    private Image _img_NextVip;

    /// <summary>
    /// 这一级到下一级要显示的文字
    /// </summary>
    private Text _txt_UpgradeInformation;

    /// <summary>
    /// 0周礼金 1月礼金 2自动洗码 3晋级礼金
    /// </summary>
    private List<Text> _Text4 = new List<Text>();

    /// <summary>
    /// 表格的二维list
    /// </summary>
    private List<List<Transform>> _VIPFormList = new List<List<Transform>>();

    /// <summary>
    /// VIP表的父类
    /// </summary>
    private Transform _VIPForm;

    /// <summary>
    /// 游戏记录里的投注时间下拉列表
    /// </summary>
    private Dropdown _BetTimeDD1;

    /// <summary>
    /// 游戏平台下拉列表
    /// </summary>
    private Dropdown _GameStationDD;

    /// <summary>
    /// 0棋牌投注记录 1捕鱼投注记录 2电子投注记录 3视讯投注记录 4体育投注记录 5彩票投注记录 6彩票投注记录
    /// </summary>
    private List<Button> _GoodsTypeList = new List<Button>();

    /// <summary>
    /// 返回的json信息
    /// </summary>
    private JsonData jd;

    /// <summary>
    /// 查询按钮
    /// </summary>
    private Button _QueryButton1;

    /// <summary>
    /// 当前是哪个 1棋牌投注记录 2捕鱼投注记录 3电子投注记录 4视讯投注记录 5体育投注记录 6彩票投注记录
    /// </summary>
    private int _Current1 = 0;

    /// <summary>
    /// 游戏记录里的高光图片 
    /// </summary>
    private List<Image> _HLL = new List<Image>();

    ///// <summary>
    ///// 账户明细里的文字图片
    ///// </summary>
    //private List<Text> _TL = new List<Text>();

    /// <summary>
    /// 个人信息里的滚动条
    /// </summary>
    private ScrollRect _Sr1;

    /// <summary>
    /// 游戏记录表格父类
    /// </summary>
    private Transform _GameInfoTableGroup;

    /// <summary>
    /// 游戏记录里的水平滑动
    /// </summary>
    private ScrollRect _HorizontalSR;

    /// <summary>
    /// 账户明细里的投注时间下拉列表 
    /// </summary>
    private Dropdown _BetTimeDD2;

    /// <summary>
    /// 账户明细里的交易状态下拉列表
    /// </summary>
    private Dropdown _TransactionStateDD2;

    /// <summary>
    /// 账户明细里的查询按钮
    /// </summary>
    private Button _QueryButton2;

    /// <summary>
    /// 账户明细里列表的下方文字集合 0充值 1提现 2优惠 3返水 4余额
    /// </summary>
    private List<Text> _BottomList = new List<Text>();

    /// <summary>
    /// 账户明细表格父类
    /// </summary>
    private Transform _AccountDetailsTableGroup;

    /// <summary>
    /// 个人报表里的高光
    /// </summary>
    private List<Image> _HLL2 = new List<Image>();

    /// <summary>
    /// 个人报表里的滑动条
    /// </summary>
    private ScrollRect _SROfPersonalReport;

    /// <summary>
    /// 个人报表里的下拉框
    /// </summary>
    private Dropdown _TimeDD;

    /// <summary>
    /// 个人报表里的按钮集合
    /// </summary>
    private List<Button> _ButtonListOfPersonalReport;

    /// <summary>
    /// 个人报表里的查询按钮
    /// </summary>
    private Button _QueryButton3;

    /// <summary>
    /// 1棋牌报表 2捕鱼报表 3电子报表 4视讯报表 5体育报表 6彩票报表
    /// </summary>
    private int _Current2 = 0;

    /// <summary>
    /// 个人报表里需要修改的信息
    /// </summary>
    private List<Text> _TextListOfPersonalReport = new List<Text>();
    #endregion

    private void ChangePanel(Panel panel)
    {
        if (panel == Panel.PersonCenter)
        {
            Reset();
            _Panel_PersonalCenter.gameObject.SetActive(true);
            AudioSourceManager._Instance.PlayCharacterSpeech(InitialResourcesManager.aud_PersonalCenter);
        }
        else
        {
            if (_Panel_PersonalCenter.gameObject.activeSelf == true)
            {
                _Panel_PersonalCenter.gameObject.SetActive(false);
            }
        }
    }

    void Awake()
    {
        GameStatic.OnChangePanel += ChangePanel;
        GameStatic.OnMainPanelInit += Init_State;
        Init_Variable();
        Init_Listener();
    }

    #region Init
    private void Init_Variable()
    {
        _Panel_PersonalCenter = transform;

        _ButtonList = _Panel_PersonalCenter.Find("group_Button").GetComponentsInChildren<Button>().ToList();

        for (int i = 0; i < _ButtonList.Count; i++)
        {
            _HightlightImageList.Add(_ButtonList[i].transform.GetChild(0).GetComponent<Image>());
        }

        for (int i = 0; i < _Panel_PersonalCenter.Find("Mask").childCount; i++)
        {
            _PanelList.Add(_Panel_PersonalCenter.Find("Mask").GetChild(i));
        }

        _Sr1 = _PanelList[0].parent.GetComponent<ScrollRect>();
        _EditAndCompleteButton.Add(_Panel_PersonalCenter.Find("Mask").Find("panel_PlayerInfo").Find("Header_PersonalMessage").Find("btn_Edit").GetComponent<Button>());
        _EditAndCompleteButton.Add(_Panel_PersonalCenter.Find("Mask").Find("panel_PlayerInfo").Find("Header_PersonalMessage").Find("btn_Complete").GetComponent<Button>());

        _btn_Return = _Panel_PersonalCenter.Find("img_Background").Find("btn_Return");

        _PersonalInfoVauleText = _Panel_PersonalCenter.Find("Mask").Find("panel_PlayerInfo").Find("UIBody_PersonalMessage").Find("PersonInfo").Find("Value").GetComponentsInChildren<Text>().ToList();
        _PersonalInfoEdit = _Panel_PersonalCenter.Find("Mask").Find("panel_PlayerInfo").Find("UIBody_PersonalMessage").Find("PersonInfo").Find("Edit").GetComponentsInChildren<InputField>().ToList();
        _VIPSlider = _Panel_PersonalCenter.Find("Mask").Find("panel_PlayerInfo").Find("UIBody_PersonalMessage").Find("LevelInfo").Find("LevelProgressBar").GetComponent<Image>();
        _img_VIPInfo = _Panel_PersonalCenter.Find("Mask").Find("panel_PlayerInfo").Find("Header_VIPRight").Find("img_VIPInfo").GetComponent<Image>();
        _img_VIP = _Panel_PersonalCenter.Find("Mask").Find("panel_PlayerInfo").Find("UIBody_PersonalMessage").Find("img_Avatar").GetComponent<Image>();
        _img_CurrentLevel = _Panel_PersonalCenter.Find("Mask").Find("panel_PlayerInfo").Find("UIBody_PersonalMessage").Find("LevelInfo").Find("img_CurrentLevel").GetComponent<Image>();
        _img_NextVip = _Panel_PersonalCenter.Find("Mask").Find("panel_PlayerInfo").Find("UIBody_PersonalMessage").Find("LevelInfo").Find("img_NextVip").GetComponent<Image>();
        _txt_UpgradeInformation = _Panel_PersonalCenter.Find("Mask").Find("panel_PlayerInfo").Find("UIBody_PersonalMessage").Find("LevelInfo").Find("txt_UpgradeInformation").GetComponent<Text>();


        _PersonalReceiveButtons.Add(_Panel_PersonalCenter.Find("Mask").Find("panel_PlayerInfo").Find("UIBody_VIPRight").Find("panel_WeeklyGift").Find("Image").gameObject.AddComponent<Button>());
        _PersonalReceiveButtons.Add(_Panel_PersonalCenter.Find("Mask").Find("panel_PlayerInfo").Find("UIBody_VIPRight").Find("panel_MonthlyGift").Find("Image").gameObject.AddComponent<Button>());
        _PersonalReceiveButtons.Add(_Panel_PersonalCenter.Find("Mask").Find("panel_PlayerInfo").Find("UIBody_VIPRight").Find("panel_PromotionGift").Find("Image").gameObject.AddComponent<Button>());

        _Text4.Add(_Panel_PersonalCenter.Find("Mask").Find("panel_PlayerInfo").Find("UIBody_VIPRight").Find("panel_WeeklyGift").Find("txt_MoneyCount").GetComponent<Text>());
        _Text4.Add(_Panel_PersonalCenter.Find("Mask").Find("panel_PlayerInfo").Find("UIBody_VIPRight").Find("panel_MonthlyGift").Find("txt_MoneyCount").GetComponent<Text>());
        _Text4.Add(_Panel_PersonalCenter.Find("Mask").Find("panel_PlayerInfo").Find("UIBody_VIPRight").Find("panel_AutoWashCode").Find("txt_MoneyCount").GetComponent<Text>());
        _Text4.Add(_Panel_PersonalCenter.Find("Mask").Find("panel_PlayerInfo").Find("UIBody_VIPRight").Find("panel_PromotionGift").Find("txt_MoneyCount").GetComponent<Text>());

        _VIPForm = _Panel_PersonalCenter.Find("Mask").Find("panel_PlayerInfo").Find("UIBody_VIPLevelInfo");

        //从1开始
        for (int i = 1; i < _VIPForm.childCount; i++)
        {
            List<Transform> tmp = new List<Transform>();

            for (int j = 1; j < _VIPForm.GetChild(i).childCount; j++)
            {
                tmp.Add(_VIPForm.GetChild(i).GetChild(j).GetChild(0));
            }

            _VIPFormList.Add(tmp);
        }


        _BetTimeDD1 = _PanelList[1].Find("panel_ChessInfomation").Find("BettingTime").Find("Dropdown").GetComponent<Dropdown>();
        _GameStationDD = _PanelList[1].Find("panel_ChessInfomation").Find("GamingStation").Find("Dropdown").GetComponent<Dropdown>();
        _GoodsTypeList = _PanelList[1].Find("ScrollRect").Find("group_LogChoice").GetComponentsInChildren<Button>().ToList();
        _QueryButton1 = _PanelList[1].Find("panel_ChessInfomation").Find("QueryButton").GetComponent<Button>();
        _HorizontalSR = _PanelList[1].Find("ScrollRect").GetComponent<ScrollRect>();

        for (int i = 0; i < _PanelList[1].Find("ScrollRect").Find("group_LogChoice").childCount; i++)
        {
            _HLL.Add(_PanelList[1].Find("ScrollRect").Find("group_LogChoice").GetChild(i).GetChild(0).GetComponent<Image>());
        }

        _GameInfoTableGroup = _PanelList[1].Find("panel_ChessInfomation").Find("mask_Table").Find("group");

        _BetTimeDD2 = _PanelList[2].Find("BettingTime").Find("Dropdown").GetComponent<Dropdown>();
        _TransactionStateDD2 = _PanelList[2].Find("TransactionState").Find("Dropdown").GetComponent<Dropdown>();
        _QueryButton2 = _PanelList[2].Find("QueryButton").GetComponent<Button>();
        _AccountDetailsTableGroup = _PanelList[2].Find("mask_Table").Find("group");

        for (int i = 0; i < _PanelList[2].Find("Bottom").childCount; i++)
        {
            _BottomList.Add(_PanelList[2].Find("Bottom").GetChild(i).GetComponent<Text>());
        }

        for (int i = 0; i < _PanelList[3].Find("ScrollRect").Find("group_Buttons").childCount; i++)
        {
            _HLL2.Add(_PanelList[3].Find("ScrollRect").Find("group_Buttons").GetChild(i).GetChild(0).GetComponent<Image>());
        }

        _SROfPersonalReport = _PanelList[3].Find("ScrollRect").GetComponent<ScrollRect>();
        _TimeDD = _PanelList[3].Find("Table").Find("img_Background").Find("drd_Time").GetComponent<Dropdown>();
        _ButtonListOfPersonalReport = _PanelList[3].Find("ScrollRect").Find("group_Buttons").GetComponentsInChildren<Button>().ToList();
        _QueryButton3 = _PanelList[3].Find("Table").Find("img_Background").Find("QueryButton").GetComponent<Button>();

        _TextListOfPersonalReport.Add(_PanelList[3].Find("Table").Find("img_Background").Find("Text (1)").GetComponent<Text>());
        _TextListOfPersonalReport.Add(_PanelList[3].Find("Table").Find("img_Background").Find("Text (2)").GetComponent<Text>());
        _TextListOfPersonalReport.Add(_PanelList[3].Find("Table").Find("img_Background").Find("Text (3)").GetComponent<Text>());
        _TextListOfPersonalReport.Add(_PanelList[3].Find("Table").Find("img_Background").Find("Text").GetComponent<Text>());


    }

    private void Init_Listener()
    {
        _ButtonList[0].onClick.AddListener(() => this.Choose1(0));
        _ButtonList[1].onClick.AddListener(() => this.Choose1(1));
        _ButtonList[2].onClick.AddListener(() => this.Choose1(2));
        _ButtonList[3].onClick.AddListener(() => this.Choose1(3));

        _btn_Return.GetComponent<Button>().onClick.AddListener(() => this.Onbtn_ReturnClick());

        _EditAndCompleteButton[0].onClick.AddListener(() => this.OnEditOrCompleteButtonClick());
        _EditAndCompleteButton[1].onClick.AddListener(() => this.OnEditOrCompleteButtonClick());

        for (int i = 0; i < _PersonalReceiveButtons.Count; i++)
        {
            int index = i;

            _PersonalReceiveButtons[i].onClick.AddListener(() =>
            {
                Dictionary<string, string> formData = new Dictionary<string, string>();
                formData.Add("index", index.ToString());

                HttpManager._Instance.StartPost(@"member/center/receiveAward", formData, (unityWebRequest) =>
                {
                    if (unityWebRequest == null)
                    {
                        TipsManager._Instance.OpenReConnectTipsPanel(() => { GetTotallInfo("0"); });
                        return;
                    }
                    else
                    {
                        print(unityWebRequest.downloadHandler.text);
                        JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

                        if (jd["code"].ToString() == "1")
                        {
                            GetTotallInfo("0");
                        }
                    }

                });



            });
        }

        for (int i = 0; i < _GoodsTypeList.Count; i++)
        {
            int index = i;
            _GoodsTypeList[i].onClick.AddListener(() => this.OnGoodsTypeButtonClick(index));

            _GoodsTypeList[i].onClick.AddListener(() => Choose2(index, _HLL));
        }

        _QueryButton1.onClick.AddListener(() => this.OnQueryButtonClick(1));
        _QueryButton2.onClick.AddListener(() => this.OnQueryButtonClick(2));
        _QueryButton3.onClick.AddListener(() => this.OnQueryButtonClick(3));

        for (int i = 0; i < _ButtonListOfPersonalReport.Count; i++)
        {
            int a = i;
            _ButtonListOfPersonalReport[i].onClick.AddListener(() => this.OnPersonalReportButtonsClick(a));
            _ButtonListOfPersonalReport[i].onClick.AddListener(() => this.Choose3(a));
        }
    }

    private void Init_State()
    {
        LoadingManager.LoadingStart(this.GetType().ToString());
        LoadingManager.LoadingComplete(this.GetType().ToString());
        GameStatic.OnLogin += () => Choose1(0);
    }
    #endregion


    #region private Method

    /// <summary>
    /// 选择的UI特效
    /// </summary>
    /// <param name="i"></param>
    private void Choose1(int index)
    {
        for (int i = 0; i < _HightlightImageList.Count; i++)
        {
            if (i != index)
            {
                _HightlightImageList[i].enabled = false;
                _PanelList[i].gameObject.SetActive(false);
            }
            else
            {
                _HightlightImageList[i].enabled = true;
                _PanelList[i].gameObject.SetActive(true);
                GetTotallInfo(index.ToString());
            }
        }
    }

    /// <summary>
    /// 0棋牌投注记录 1捕鱼投注记录 2电子投注记录 3视讯投注记录 4体育投注记录 5彩票投注记录 的UI特效
    /// </summary>
    /// <param name="index"></param>
    /// <param name="HightlightImageList"></param>
    /// <param name="TextImageList"></param>
    private void Choose2(int index, List<Image> HightlightImageList)
    {
        for (int i = 0; i < HightlightImageList.Count; i++)
        {
            if (i != index)
            {
                HightlightImageList[i].gameObject.SetActive(false);
            }
            else
            {
                HightlightImageList[i].gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// 个人报表里的按钮切换
    /// </summary>
    private void Choose3(int index)
    {
        for (int i = 0; i < _HLL2.Count; i++)
        {
            if (i != index)
            {
                _HLL2[i].gameObject.SetActive(false);
            }
            else
            {
                _HLL2[i].gameObject.SetActive(true);
            }
        }
    }

    private void ClearnAllTable()
    {
        for (int i = 0; i < _GameInfoTableGroup.childCount; i++)
        {
            Destroy(_GameInfoTableGroup.GetChild(i).gameObject);
        }

        for (int i = 0; i < _AccountDetailsTableGroup.childCount; i++)
        {
            Destroy(_AccountDetailsTableGroup.GetChild(i).gameObject);
        }
    }

    private void Reset()
    {
        Choose1(0);
        Choose2(0, _HLL);
        Choose3(0);

        _Current1 = 0;
        _BetTimeDD1.value = 0;
        _GameStationDD.value = 0;
        _BetTimeDD2.value = 0;
        _TransactionStateDD2.value = 0;
        _SROfPersonalReport.normalizedPosition = new Vector2(0, 1);
        _HorizontalSR.normalizedPosition = new Vector2(0, 1);
        _Sr1.normalizedPosition = new Vector2(0, 1);
        _TimeDD.value = 0;
        _EditAndCompleteButton[0].gameObject.SetActive(true);
        _EditAndCompleteButton[1].gameObject.SetActive(false);

        ClearnAllTable();
    }

    /// <summary>
    /// 返回按钮
    /// </summary>
    private void Onbtn_ReturnClick()
    {
        GameStatic.OnChangePanel(Panel.MainMenu);
        Reset();
    }

    /// <summary>
    /// 根据索引获取相对应的信息
    /// </summary>
    private void GetTotallInfo(string index)
    {
        if (!LocalFileManager._Instance._GameData._IsLogin)
        {
            return;
        }

        Dictionary<string, string> formData = new Dictionary<string, string>();
        formData.Add("index", index);

        HttpManager._Instance.StartPost(@"member/center/getTotalInfo", formData, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(() => { GetTotallInfo(index); });
                return;
            }

            print(unityWebRequest.downloadHandler.text);
            jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

            if (jd["code"].ToString() == "1")
            {
                GetGameRecordUI(int.Parse(jd["result"]["index"].ToString())); ;
            }
            else
            {
                //失败
                print("返回失败");
            }

            ClearnAllTable();

        });
    }

    /// <summary>
    /// 动态获取UI信息
    /// </summary>
    /// <param name="jd"></param>
    private void GetGameRecordUI(int index)
    {
        if (index == 0)
        {
            AfterGetUserInfo();
        }
        else if (index == 1)
        {
            _BetTimeDD1.ClearOptions();

            List<string> item = new List<string>();

            for (int i = 0; i < jd["result"]["dateType"].Count; i++)
            {
                item.Add(jd["result"]["dateType"][i].ToString());
            }

            _BetTimeDD1.AddOptions(item);

            OnGoodsTypeButtonClick(0);
        }
        else if (index == 2)
        {
            _BetTimeDD2.ClearOptions();
            _TransactionStateDD2.ClearOptions();

            List<string> item1 = new List<string>();
            List<string> item2 = new List<string>();

            for (int i = 0; i < jd["result"]["dateType"].Count; i++)
            {
                item1.Add(jd["result"]["dateType"][i].ToString());
            }

            for (int i = 0; i < jd["result"]["cashType"].Count; i++)
            {
                item2.Add(jd["result"]["cashType"][i].ToString());
            }

            _BetTimeDD2.AddOptions(item1);
            _TransactionStateDD2.AddOptions(item2);

            _BottomList[0].text = jd["result"]["total"]["deposit"] + "金币";
            _BottomList[1].text = jd["result"]["total"]["withdrawDeposit"] + "金币";
            _BottomList[2].text = jd["result"]["total"]["discounts"] + "金币";
            _BottomList[3].text = jd["result"]["total"]["backwater"] + "金币";
            _BottomList[4].text = jd["result"]["total"]["balance"] + "金币";
        }
        else if (index == 3)
        {
            _TimeDD.ClearOptions();

            List<string> item1 = new List<string>();

            for (int i = 1; i < jd["result"]["dateType"].Count; i++)//从1开始 不要全部时间
            {
                item1.Add(jd["result"]["dateType"][i].ToString());
            }

            _TimeDD.AddOptions(item1);

            OnQueryButtonClick(3);
        }
        else
        {

        }
    }

    /// <summary>
    /// 个人报表里的按钮监听
    /// </summary>
    /// <param name="index"></param>
    private void OnPersonalReportButtonsClick(int index)
    {
        _Current2 = index + 1;
    }

    /// <summary>
    /// 0棋牌投注记录 1捕鱼投注记录 2电子投注记录 3视讯投注记录 4体育投注记录 5彩票投注记录 监听
    /// </summary>
    /// <param name="index"></param>
    private void OnGoodsTypeButtonClick(int index)
    {
        //print(index);
        //Choose2(index, _HLL, _TL);

        _Current1 = index + 1;

        _GameStationDD.ClearOptions();

        List<string> item = new List<string>();

        if (index == 0)
        {
            for (int i = 0; i < jd["result"]["棋牌投注记录"].Count; i++)
            {
                item.Add(jd["result"]["棋牌投注记录"][i].ToString());
            }
        }
        else if (index == 1)
        {
            for (int i = 0; i < jd["result"]["捕鱼投注记录"].Count; i++)
            {
                item.Add(jd["result"]["捕鱼投注记录"][i].ToString());
            }
        }
        else if (index == 2)
        {
            for (int i = 0; i < jd["result"]["电子投注记录"].Count; i++)
            {
                item.Add(jd["result"]["电子投注记录"][i].ToString());
            }
        }
        else if (index == 3)
        {
            for (int i = 0; i < jd["result"]["视讯投注记录"].Count; i++)
            {
                item.Add(jd["result"]["视讯投注记录"][i].ToString());
            }
        }
        else if (index == 4)
        {
            for (int i = 0; i < jd["result"]["体育投注记录"].Count; i++)
            {
                item.Add(jd["result"]["体育投注记录"][i].ToString());
            }
        }
        else if (index == 5)
        {
            for (int i = 0; i < jd["result"]["彩票投注记录"].Count; i++)
            {
                item.Add(jd["result"]["彩票投注记录"][i].ToString());
            }
        }

        _GameStationDD.AddOptions(item);

        //清空表
        //ClearnAllTable();
    }

    /// <summary>
    /// 查询按钮监听 1游戏记录的查询 2账户明细的查询
    /// </summary>
    private void OnQueryButtonClick(int index)
    {
        ClearnAllTable();

        //游戏记录的查询
        if (index == 1)
        {
            Dictionary<string, string> form = new Dictionary<string, string>();
            form.Add("betType", _Current1.ToString());
            form.Add("dateType", _BetTimeDD1.value.ToString());
            form.Add("platformName", _GameStationDD.options[_GameStationDD.value].text);

            HttpManager._Instance.StartPost(@"member/center/getMemberBetRecord", form, (unityWebRequest) =>
          {
              if (unityWebRequest == null)
              {
                  TipsManager._Instance.OpenReConnectTipsPanel(() => { OnQueryButtonClick(index); });
                  return;
              }

              JsonData jsonData = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

              if (jsonData["code"].ToString() == "1")
              {
                  GameObject prefab = InitialResourcesManager.PersonalCenterTableContent;

                  for (int i = 0; i < jsonData["result"].Count; i++)
                  {
                      Transform go = Instantiate(prefab, _GameInfoTableGroup, false).transform;
                      go.gameObject.SetActive(true);

                      go.transform.GetChild(0).GetComponentInChildren<Text>().text = jsonData["result"][i]["gameName"] == null ? string.Empty : jsonData["result"][i]["gameName"].ToString();
                      go.transform.GetChild(1).GetComponentInChildren<Text>().text = jsonData["result"][i]["goodsNum"] == null ? string.Empty : jsonData["result"][i]["goodsNum"].ToString();
                      go.transform.GetChild(2).GetComponentInChildren<Text>().text = jsonData["result"][i]["fee"] == null ? string.Empty : jsonData["result"][i]["fee"].ToString();
                      go.transform.GetChild(3).GetComponentInChildren<Text>().text = jsonData["result"][i]["createTime"] == null ? string.Empty : jsonData["result"][i]["createTime"].ToString();
                      go.transform.GetChild(4).GetComponentInChildren<Text>().text = jsonData["result"][i]["profit"] == null ? string.Empty : jsonData["result"][i]["profit"].ToString();
                  }

                  float x = _GameInfoTableGroup.GetComponent<RectTransform>().sizeDelta.x;
                  _GameInfoTableGroup.GetComponent<RectTransform>().sizeDelta = new Vector2(x, prefab.GetComponent<RectTransform>().sizeDelta.y * jsonData["result"].Count);
                  _GameInfoTableGroup.parent.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);
              }
              else
              {
                  TipsManager._Instance.OpenWarningBox(jsonData.ToString());
              }
          });
        }//账户明细的查询
        else if (index == 2)
        {
            Dictionary<string, string> form = new Dictionary<string, string>();
            form.Add("cashType", _TransactionStateDD2.value.ToString());
            form.Add("dateType", _BetTimeDD2.value.ToString());

            HttpManager._Instance.StartPost(@"member/center/getMemberCashInfo", form, (unityWebRequest) =>
           {
               if (unityWebRequest == null)
               {
                   TipsManager._Instance.OpenReConnectTipsPanel(() => { OnQueryButtonClick(index); });
                   return;
               }

               JsonData tmp = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

               if (tmp["code"].ToString() == "1")
               {
                   Transform prefab = InitialResourcesManager.PersonalCenterTableContent.transform;

                   for (int i = 0; i < tmp["result"]["getMemberCashInfo"].Count; i++)
                   {
                       Transform go = Instantiate(prefab, _AccountDetailsTableGroup, false);
                       go.gameObject.SetActive(true);

                       go.transform.GetChild(0).GetComponentInChildren<Text>().text = tmp["result"]["getMemberCashInfo"][i]["cashType"].ToString();
                       go.transform.GetChild(1).GetComponentInChildren<Text>().text = tmp["result"]["getMemberCashInfo"][i]["createDate"].ToString();
                       go.transform.GetChild(2).GetComponentInChildren<Text>().text = tmp["result"]["getMemberCashInfo"][i]["cashFeeOut"].ToString();
                       go.transform.GetChild(3).GetComponentInChildren<Text>().text = tmp["result"]["getMemberCashInfo"][i]["cashFeeIn"].ToString();
                       go.transform.GetChild(4).GetComponentInChildren<Text>().text = tmp["result"]["getMemberCashInfo"][i]["balance"].ToString();
                   }

                   _BottomList[0].text = jd["result"]["total"]["deposit"] + "金币";
                   _BottomList[1].text = jd["result"]["total"]["withdrawDeposit"] + "金币";
                   _BottomList[2].text = jd["result"]["total"]["discounts"] + "金币";
                   _BottomList[3].text = jd["result"]["total"]["backwater"] + "金币";
                   _BottomList[4].text = jd["result"]["total"]["balance"] + "金币";

                   float x = _AccountDetailsTableGroup.GetComponent<RectTransform>().sizeDelta.x;
                   _AccountDetailsTableGroup.GetComponent<RectTransform>().sizeDelta = new Vector2(x, prefab.GetComponent<RectTransform>().sizeDelta.y * tmp["result"]["getMemberCashInfo"].Count);
                   _AccountDetailsTableGroup.parent.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);
               }

           });
        }
        else if (index == 3)
        {
            Dictionary<string, string> form = new Dictionary<string, string>();
            form.Add("dateType", _TimeDD.value.ToString());
            form.Add("statementType", _Current2.ToString());
            HttpManager._Instance.StartPost(@"member/center/getMemberStatement", form, (unityWebRequest) =>
            {
                if (unityWebRequest == null)
                {
                    TipsManager._Instance.OpenReConnectTipsPanel(() => { OnQueryButtonClick(index); });
                    return;
                }

                //print(unityWebRequest.downloadHandler.text);
                JsonData tmp = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

                if (tmp["code"].ToString() == "1")
                {
                    if (tmp["result"] != null)
                    {
                        _TextListOfPersonalReport[0].text = tmp["result"]["fee"] + "元";
                        _TextListOfPersonalReport[1].text = tmp["result"]["payout"] + "元";
                        _TextListOfPersonalReport[2].text = tmp["result"]["backwater"] + "元";
                        _TextListOfPersonalReport[3].text = tmp["result"]["profit"] + "元";
                    }
                }
            });
        }
    }

    /// <summary>
    /// 获取用户信息后要做的事情
    /// </summary>
    private void AfterGetUserInfo()
    {
        //成功 0账号 1昵称 2姓名 3性别 4电话 5QQ 6邮箱 7微信
        _PersonalInfoVauleText[0].text = jd["result"]["getGameMemberInfo"]["username"] == null ? string.Empty : jd["result"]["getGameMemberInfo"]["username"].ToString();
        _PersonalInfoVauleText[1].text = jd["result"]["getGameMemberInfo"]["nickname"] == null ? string.Empty : jd["result"]["getGameMemberInfo"]["nickname"].ToString();
        _PersonalInfoVauleText[2].text = jd["result"]["getGameMemberInfo"]["name"] == null ? string.Empty : jd["result"]["getGameMemberInfo"]["name"].ToString();

        if (jd["result"]["getGameMemberInfo"]["sex"] != null)
        {
            if (jd["result"]["getGameMemberInfo"]["sex"].ToString().Equals("1"))
            {
                _PersonalInfoVauleText[3].text = "男";
            }
            else if (jd["result"]["getGameMemberInfo"]["sex"].ToString().Equals("0"))
            {
                _PersonalInfoVauleText[3].text = "女";
            }
        }
        else
        {
            _PersonalInfoVauleText[3].text = "";
        }

        _PersonalInfoVauleText[4].text = jd["result"]["getGameMemberInfo"]["phone"] == null ? string.Empty : jd["result"]["getGameMemberInfo"]["phone"].ToString();
        _PersonalInfoVauleText[5].text = jd["result"]["getGameMemberInfo"]["qq"] == null ? string.Empty : jd["result"]["getGameMemberInfo"]["qq"].ToString();
        _PersonalInfoVauleText[6].text = jd["result"]["getGameMemberInfo"]["email"] == null ? string.Empty : jd["result"]["getGameMemberInfo"]["email"].ToString();
        _PersonalInfoVauleText[7].text = jd["result"]["getGameMemberInfo"]["weixin"] == null ? string.Empty : jd["result"]["getGameMemberInfo"]["weixin"].ToString();

        _PersonalInfoEdit[0].text = jd["result"]["getGameMemberInfo"]["username"] == null ? string.Empty : jd["result"]["getGameMemberInfo"]["username"].ToString();
        _PersonalInfoEdit[1].text = jd["result"]["getGameMemberInfo"]["nickname"] == null ? string.Empty : jd["result"]["getGameMemberInfo"]["nickname"].ToString();
        _PersonalInfoEdit[2].text = jd["result"]["getGameMemberInfo"]["name"] == null ? string.Empty : jd["result"]["getGameMemberInfo"]["name"].ToString();

        if (jd["result"]["getGameMemberInfo"]["sex"] != null)
        {
            if (jd["result"]["getGameMemberInfo"]["sex"].ToString().Equals("1"))
            {
                _PersonalInfoEdit[3].text = "男";
            }
            else if (jd["result"]["getGameMemberInfo"]["sex"].ToString().Equals("0"))
            {
                _PersonalInfoEdit[3].text = "女";
            }
        }
        else
        {
            _PersonalInfoEdit[3].text = "";
        }
        //_PersonalInfoEdit[3].text = jd["result"]["getGameMemberInfo"]["sex"] == null ? string.Empty : jd["result"]["getGameMemberInfo"]["sex"].ToString();
        _PersonalInfoEdit[4].text = jd["result"]["getGameMemberInfo"]["phone"] == null ? string.Empty : jd["result"]["getGameMemberInfo"]["phone"].ToString();
        _PersonalInfoEdit[5].text = jd["result"]["getGameMemberInfo"]["qq"] == null ? string.Empty : jd["result"]["getGameMemberInfo"]["qq"].ToString();
        _PersonalInfoEdit[6].text = jd["result"]["getGameMemberInfo"]["email"] == null ? string.Empty : jd["result"]["getGameMemberInfo"]["email"].ToString();
        _PersonalInfoEdit[7].text = jd["result"]["getGameMemberInfo"]["weixin"] == null ? string.Empty : jd["result"]["getGameMemberInfo"]["weixin"].ToString();

        if (jd["result"]["scatter"]["weekGift_status"].ToString() == "0")
        {
            _PersonalReceiveButtons[0].GetComponent<Image>().sprite = InitialResourcesManager.img_CannotReceive;
            _PersonalReceiveButtons[0].enabled = false;
        }
        else if (jd["result"]["scatter"]["weekGift_status"].ToString() == "1")
        {
            _PersonalReceiveButtons[0].GetComponent<Image>().sprite = InitialResourcesManager.img_canReceive; 
            _PersonalReceiveButtons[0].enabled = true;
        }

        if (jd["result"]["scatter"]["monthGift_status"].ToString() == "0")
        {
            _PersonalReceiveButtons[1].GetComponent<Image>().sprite = InitialResourcesManager.img_CannotReceive;
            _PersonalReceiveButtons[1].enabled = false;
        }
        else if (jd["result"]["scatter"]["monthGift_status"].ToString() == "1")
        {
            _PersonalReceiveButtons[1].GetComponent<Image>().sprite = InitialResourcesManager.img_canReceive;
            _PersonalReceiveButtons[1].enabled = true;
        }

        if (jd["result"]["scatter"]["upgradeGift_status"].ToString() == "0")
        {
            _PersonalReceiveButtons[2].GetComponent<Image>().sprite = InitialResourcesManager.img_CannotReceive;
            _PersonalReceiveButtons[2].enabled = false;
        }
        else if (jd["result"]["scatter"]["upgradeGift_status"].ToString() == "1")
        {
            _PersonalReceiveButtons[2].GetComponent<Image>().sprite = InitialResourcesManager.img_canReceive;
            _PersonalReceiveButtons[2].enabled = true;
        }

        _Text4[0].text = jd["result"]["scatter"]["weekGift"] == null ? string.Empty : jd["result"]["scatter"]["weekGift"].ToString() + "元";
        _Text4[1].text = jd["result"]["scatter"]["monthGift"] == null ? string.Empty : jd["result"]["scatter"]["monthGift"].ToString() + "元";
        _Text4[2].text = jd["result"]["scatter"]["ratio"] == null ? string.Empty : jd["result"]["scatter"]["ratio"].ToString() + "%";
        _Text4[3].text = jd["result"]["scatter"]["upgradeGift"] == null ? string.Empty : jd["result"]["scatter"]["upgradeGift"].ToString() + "元";

        _txt_UpgradeInformation.text = jd["result"]["scatter"]["hint"] == null ? string.Empty : jd["result"]["scatter"]["hint"].ToString();
        _img_VIPInfo.sprite = InitialResourcesManager.img_gr_viptitle[int.Parse(jd["result"]["getGameMemberInfo"]["levelId"].ToString())];
        _img_VIP.sprite = InitialResourcesManager.img_gr_VIPIcon[int.Parse(jd["result"]["getGameMemberInfo"]["levelId"].ToString())];
        _img_CurrentLevel.sprite = InitialResourcesManager.img_gr_V[int.Parse(jd["result"]["getGameMemberInfo"]["levelId"].ToString())];

        int VIPLevel = int.Parse(jd["result"]["getGameMemberInfo"]["levelId"].ToString());

        if (VIPLevel < 10)
        {
            _img_NextVip.sprite = InitialResourcesManager.img_gr_V[VIPLevel + 1];
            _VIPSlider.fillAmount = jd["result"]["scatter"]["upgrade"] == null ? 0f : float.Parse(jd["result"]["scatter"]["upgrade"].ToString());
        }
        else
        {
            _img_NextVip.sprite = InitialResourcesManager.img_gr_V[VIPLevel];
            _VIPSlider.fillAmount = 1;
        }

        for (int i = 0; i < _VIPFormList.Count; i++)
        {
            if (jd["result"]["gameLevelInfo"][i]["minCumulBet"] != null)
            {
                float tmp = float.Parse(jd["result"]["gameLevelInfo"][i]["minCumulBet"].ToString()) / 10000;

                if (tmp <= 0)
                {
                    _VIPFormList[i][0].GetComponent<Text>().text = "0";
                }
                else
                {
                    _VIPFormList[i][0].GetComponent<Text>().text = tmp.ToString() + "万";
                }
            }

            //_VIPFormList[i][0].GetComponent<Text>().text = jd["result"]["gameLevelInfo"][i]["minCumulBet"] == null ? string.Empty : jd["result"]["gameLevelInfo"][i]["minCumulBet"].ToString() + "元";
            _VIPFormList[i][1].GetComponent<Text>().text = jd["result"]["gameLevelInfo"][i]["upgradeGift"] == null ? string.Empty : jd["result"]["gameLevelInfo"][i]["upgradeGift"].ToString();
            _VIPFormList[i][2].GetComponent<Text>().text = jd["result"]["gameLevelInfo"][i]["weekGift"] == null ? string.Empty : jd["result"]["gameLevelInfo"][i]["weekGift"].ToString();
            _VIPFormList[i][3].GetComponent<Text>().text = jd["result"]["gameLevelInfo"][i]["monthGift"] == null ? string.Empty : jd["result"]["gameLevelInfo"][i]["monthGift"].ToString();
            _VIPFormList[i][4].GetComponent<Text>().text = jd["result"]["gameLevelInfo"][i]["cumulUpgradeGift"] == null ? string.Empty : jd["result"]["gameLevelInfo"][i]["cumulUpgradeGift"].ToString();

            if (jd["result"]["gameLevelInfo"][i]["isDepositAccel"] != null)
            {
                if (jd["result"]["gameLevelInfo"][i]["isDepositAccel"].ToString() == "1")
                {
                    _VIPFormList[i][5].GetComponent<Image>().sprite = InitialResourcesManager.img_Check;
                }
                else if (jd["result"]["gameLevelInfo"][i]["isDepositAccel"].ToString() == "0")
                {
                    _VIPFormList[i][5].GetComponent<Image>().sprite = InitialResourcesManager.img_ShortLine; 
                }
            }
            else
            {
                _VIPFormList[i][5].GetComponent<Image>().sprite = InitialResourcesManager.img_ShortLine; 
                print("空了");
            }

            if (jd["result"]["gameLevelInfo"][i]["isExclusive"] != null)
            {
                if (jd["result"]["gameLevelInfo"][i]["isExclusive"].ToString() == "1")
                {
                    _VIPFormList[i][6].GetComponent<Image>().sprite = InitialResourcesManager.img_Check;
                }
                else if (jd["result"]["gameLevelInfo"][i]["isExclusive"].ToString() == "0")
                {
                    _VIPFormList[i][6].GetComponent<Image>().sprite = InitialResourcesManager.img_ShortLine; 
                }
            }
            else
            {
                _VIPFormList[i][6].GetComponent<Image>().sprite = InitialResourcesManager.img_ShortLine;
                print("空了");
            }
        }
    }

    /// <summary>
    /// 编辑或者完成按钮的监听
    /// </summary>
    private void OnEditOrCompleteButtonClick()
    {
        _isEdit = !_isEdit;

        if (_isEdit)
        {
            _EditAndCompleteButton[0].gameObject.SetActive(false);
            _EditAndCompleteButton[1].gameObject.SetActive(true);

            //赋值
            for (int i = 0; i < _PersonalInfoEdit.Count; i++)
            {
                _PersonalInfoEdit[i].transform.GetComponent<InputField>().text = _PersonalInfoVauleText[i].text;
                //_PersonalInfoEdit[i].transform.Find("Text").GetComponent<Text>().text = _PersonalInfoEdit[i].transform.Find("Placeholder").GetComponent<Text>().text;
            }

            _PersonalInfoEdit[0].transform.parent.gameObject.SetActive(true);
            _PersonalInfoVauleText[0].transform.parent.gameObject.SetActive(false);

        }
        else
        {
            _EditAndCompleteButton[0].gameObject.SetActive(true);
            _EditAndCompleteButton[1].gameObject.SetActive(false);

            _PersonalInfoEdit[0].transform.parent.gameObject.SetActive(false);
            _PersonalInfoVauleText[0].transform.parent.gameObject.SetActive(true);

            //请求提交更新
            Dictionary<string, string> form = new Dictionary<string, string>();
            form.Add("email", _PersonalInfoEdit[6].text);
            form.Add("avatar", "");
            form.Add("nickname", _PersonalInfoEdit[1].text);
            form.Add("qq", _PersonalInfoEdit[5].text);
            form.Add("sex", _PersonalInfoEdit[3].text);
            form.Add("weixin", _PersonalInfoEdit[7].text);

            HttpManager._Instance.StartPost(@"member/center/updateGameMemberInfo", form, (unityWebRequest) =>
            {
                if (unityWebRequest == null)
                {
                    TipsManager._Instance.OpenReConnectTipsPanel(OnEditOrCompleteButtonClick);
                    return;
                }

                JsonData jsonData = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

                if (jsonData["code"].ToString() == "1")
                {
                    _PersonalInfoVauleText[0].text = jsonData["result"]["getGameMemberInfo"]["username"] == null ? string.Empty : jsonData["result"]["getGameMemberInfo"]["username"].ToString();
                    _PersonalInfoVauleText[1].text = jsonData["result"]["getGameMemberInfo"]["nickname"] == null ? string.Empty : jsonData["result"]["getGameMemberInfo"]["nickname"].ToString();
                    _PersonalInfoVauleText[2].text = jsonData["result"]["getGameMemberInfo"]["name"] == null ? string.Empty : jsonData["result"]["getGameMemberInfo"]["name"].ToString();
                    if (jsonData["result"]["getGameMemberInfo"]["sex"] != null)
                    {
                        if (jsonData["result"]["getGameMemberInfo"]["sex"].ToString().Equals("1"))
                        {
                            _PersonalInfoVauleText[3].text = "男";
                        }
                        else if (jsonData["result"]["getGameMemberInfo"]["sex"].ToString().Equals("0"))
                        {
                            _PersonalInfoVauleText[3].text = "女";
                        }
                    }
                    else
                    {
                        _PersonalInfoVauleText[3].text = "";
                    }
                    _PersonalInfoVauleText[4].text = jsonData["result"]["getGameMemberInfo"]["phone"] == null ? string.Empty : jsonData["result"]["getGameMemberInfo"]["phone"].ToString();
                    _PersonalInfoVauleText[5].text = jsonData["result"]["getGameMemberInfo"]["qq"] == null ? string.Empty : jsonData["result"]["getGameMemberInfo"]["qq"].ToString();
                    _PersonalInfoVauleText[6].text = jsonData["result"]["getGameMemberInfo"]["email"] == null ? string.Empty : jsonData["result"]["getGameMemberInfo"]["email"].ToString();
                    _PersonalInfoVauleText[7].text = jsonData["result"]["getGameMemberInfo"]["weixin"] == null ? string.Empty : jsonData["result"]["getGameMemberInfo"]["weixin"].ToString();
                }
                else
                {
                    TipsManager._Instance.OpenWarningBox(jsonData["msg"].ToString());
                }
            });
        }
    }
    #endregion
}
