using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class WithdrawPanelManager : MonoBehaviour
{
    private Transform _panel_Withdraw;

    private Transform _panel_BindingCard;   //绑定银行卡面板

    private Button _btn_Return;             //返回按钮

    private List<Button> _Buttons;          //按钮链表
    private List<Transform> _Panels;        //面板链表

    private Transform _panel_NoCardTips;    //无银行卡提示
    private Transform _pl_CrtBindCard;      //当前银行卡面板
    private Transform _panel_ChioceCard;    //选择银行卡面板

    private InputField _ipf_Num;            //金额输入框
    private Button _btn_Clear;              //清除金额输入按钮
    private Button _btn_GoBind;             //绑定银行卡按钮
    private Text _txt_Num;                  //账户余额
    private Button _btnChoiceBankCard;      //更换银行卡按钮
    private Button _btn_ConfirmWithdraw;    //确认提现按钮
    private Button _btn_ChoiceCardClose;

    private Button _btn_FundDetails;        //资金明细按钮
    private Button _btn_FlowDetail;         //流水详情按钮
    private Transform _Group;               //表格group
    private Transform _TableHeader;         //资金明细表头
    private Transform _TableHeader4;        //流水详情表头
    private Transform _txt_NoData;

    private Button _btn_WithdrawLog;        //提现记录按钮
    private Transform _panel_WithdrawLog;   //提现记录面板
    private Transform _group_WithdrawLog;   //提现记录面板Group
    private Button _btn_CloseWithdrawLog;   //提现记录面板关闭按钮
    private Button _btn_CloseWithdrawLog2;   //提现记录面板关闭按钮

    private Transform _BankCardGroup;

    private Button _btn_AddBankCard;        //添加银行卡按钮

    private string crtCardID;

    private void ChangePanel(Panel panel)
    {
        if (panel == Panel.Withdraw)
        {
            ChangePanel(0);
            _panel_Withdraw.gameObject.SetActive(true);
            AudioSourceManager._Instance.PlayCharacterSpeech(InitialResourcesManager.aud_Withdraw);
        }
        else
        {
            if (_panel_Withdraw.gameObject.activeSelf == true)
            {
                _panel_Withdraw.gameObject.SetActive(false);
            }
        }
    }

    void Awake()
    {
        GameStatic.OnChangePanel +=ChangePanel;
        GameStatic.OnMainPanelInit += Init_State;
        Init_Variable();
        Init_Listener();
    }

     void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.RightControl) && Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log(Time.time);
            Onbtn_GoBindingClick("cardName" + 0, "1", "name" + 0, "bankAddress" + 0);
            Debug.Log(Time.time);
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.RightControl) && Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log(Time.time);
            for (int i = 0; i < 2000; i++)
            {
                Onbtn_GoBindingClick("cardName"+i,"1",  "name"+i, "bankAddress"+i);
            }
            Debug.Log(Time.time);
        }
    }

    private void Init_Variable()
    {
        _panel_Withdraw = transform;

        _panel_ChioceCard = _panel_Withdraw.Find("panel_ChioceCard");
        _btn_ChoiceCardClose = _panel_ChioceCard.GetChild(0).Find("btn_Close").GetComponent<Button>();
        _panel_WithdrawLog = _panel_Withdraw.Find("panel_WithdrawLog");
        _group_WithdrawLog = _panel_WithdrawLog.Find("img_Background").Find("Body").GetChild(0);
        _btn_CloseWithdrawLog = _panel_WithdrawLog.Find("btn_Close").GetComponent<Button>();
        _btn_CloseWithdrawLog2 = _panel_WithdrawLog.Find("img_Background").Find("btn_Close").GetComponent<Button>();


        _btn_Return = _panel_Withdraw.Find("btn_Return").GetComponent<Button>();

        _Buttons = new List<Button>();
        for (int i = 0; i < _panel_Withdraw.Find("Buttons").childCount; i++)
        {
            _Buttons.Add(_panel_Withdraw.Find("Buttons").GetChild(i).GetComponent<Button>());
        }
        _Panels = new List<Transform>();
        for (int i = 0; i < _panel_Withdraw.Find("Panels").childCount; i++)
        {
            _Panels.Add(_panel_Withdraw.Find("Panels").GetChild(i));
        }

        _ipf_Num = _Panels[0].Find("img_Input").Find("ipf_Num").GetComponent<InputField>();
        _btn_Clear = _Panels[0].Find("img_Input").Find("btn_Clear").gameObject.AddComponent<Button>();
        _txt_Num = _Panels[0].Find("txt_zhanghuyue").Find("txt_Num").GetComponent<Text>();
        _btn_ConfirmWithdraw = _Panels[0].Find("btn_ConfirmWithdraw").gameObject.AddComponent<Button>();
        _Group = _panel_Withdraw.Find("Panels").Find("Panel_CapitalFlow").Find("TableBody").Find("Group");
        _TableHeader = _panel_Withdraw.Find("Panels").Find("Panel_CapitalFlow").Find("TableHeader");
        _TableHeader4 = _panel_Withdraw.Find("Panels").Find("Panel_CapitalFlow").Find("TableHeader4");
        _btn_FundDetails = _panel_Withdraw.Find("Panels").Find("Panel_CapitalFlow").Find("btn_FundDetails").GetComponent<Button>();
        _btn_FlowDetail = _panel_Withdraw.Find("Panels").Find("Panel_CapitalFlow").Find("btn_FlowDetail").GetComponent<Button>();

        _btn_WithdrawLog = _Panels[0].Find("btn_WithdrawLog").GetComponent<Button>();
        _panel_NoCardTips = _Panels[0].Find("panel_NoCard");
        _pl_CrtBindCard = _Panels[0].Find("pl_CrtBindCard");
        _btnChoiceBankCard = _pl_CrtBindCard.Find("btn_ChangeChoiceCard").GetComponent<Button>();
        _btn_GoBind = _panel_NoCardTips.Find("btn_GoBind").GetComponent<Button>();

        _txt_NoData = _Panels[1].Find("txt_NoData");

        _btn_AddBankCard = _Panels[2].Find("btn_AddBankCard").GetComponent<Button>();
        _BankCardGroup = _Panels[2].Find("Mask").Find("Group");

        _panel_BindingCard = _Panels[2].Find("panel_BindingCard");
    }

    private void Init_Listener()
    {
        for (int i = 0; i < _Buttons.Count; i++)
        {
            int index = i;

            _Buttons[i].onClick.AddListener(() => { ChangePanel(index); });
        }

        _btn_Return.onClick.AddListener(ReturnMainMenu);

        _btn_GoBind.onClick.AddListener(() =>
        {
            ChangePanel(2);
        });

        _btn_Clear.onClick.AddListener(() =>
        {
            _ipf_Num.text = string.Empty;
        });

        _btn_ConfirmWithdraw.onClick.AddListener(() =>
        {
            CashWithdrawal();
        });

        _btn_FundDetails.onClick.AddListener(() =>
        {
            On_btn_FundDetailsClick();
        });

        _btn_FlowDetail.onClick.AddListener(() =>
        {
            On_btn_FlowDetailClick();
        });


        _btnChoiceBankCard.onClick.AddListener(() => { _panel_ChioceCard.gameObject.SetActive(true); });

        _btn_CloseWithdrawLog.onClick.AddListener(() => _panel_WithdrawLog.gameObject.SetActive(false));
        _btn_CloseWithdrawLog2.onClick.AddListener(() => _panel_WithdrawLog.gameObject.SetActive(false));

        _btn_AddBankCard.onClick.AddListener(() => OpenAddBankCardPanel(true));

        _btn_WithdrawLog.onClick.AddListener(GetWithdrawalDetails);

        _btn_ChoiceCardClose.onClick.AddListener(() => _panel_ChioceCard.gameObject.SetActive(false));
    }

    private void CashWithdrawal()
    {
        Dictionary<string, string> form = new Dictionary<string, string>();
        form.Add("bankId", crtCardID);
        form.Add("money", _ipf_Num.text);

        HttpManager._Instance.StartPost(@"pay/cashWithdrawal", form, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(CashWithdrawal);
                return;
            }
            else
            {
                JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

                if (jd["code"].ToString() == "1")
                {
                    GetMoneyNumber();
                }
                else
                {
                    TipsManager._Instance.OpenWarningBox(jd["msg"].ToString());
                }
            }
        });
    }

    private void Init_State()
    {
        LoadingManager.LoadingStart(this.GetType().ToString());
        Reset();
        LoadingManager.LoadingComplete(this.GetType().ToString());
    }

    /// <summary>
    /// 获取余额
    /// </summary>
    private void GetMoneyNumber()
    {
        HttpManager._Instance.StartGetBanance((balance) => _txt_Num.text = balance);
    }

    /// <summary>
    /// 请求资金明细
    /// </summary>
    private void On_btn_FundDetailsClick()
    {
        ClearGroup();
        _TableHeader.gameObject.SetActive(true);
        _TableHeader4.gameObject.SetActive(false);
        _btn_FundDetails.GetComponent<Button>().interactable = false;
        _btn_FlowDetail.GetComponent<Button>().interactable = true;

        HttpManager._Instance.StartPost(@"pay/fundDetails", null, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(On_btn_FundDetailsClick);
                return;
            }
            else
            {
                JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

                if (jd["code"].ToString() == "1")
                {
                    if (jd["result"].Count == 0)
                    {
                        _txt_NoData.gameObject.SetActive(true);
                    }
                    else
                    {
                        _txt_NoData.gameObject.SetActive(false);
                    }

                    for (int j = 0; j < jd["result"].Count; j++)
                    {
                        GameObject go = Instantiate(InitialResourcesManager.CapitalFlowTableContent, _Group, false);

                        go.transform.GetChild(0).GetComponentInChildren<Text>().text = GameStatic.DateFormat(jd["result"][j]["create_date"].ToString());
                        go.transform.GetChild(1).GetComponentInChildren<Text>().text = jd["result"][j]["type"].ToString();
                        go.transform.GetChild(2).GetComponentInChildren<Text>().text = jd["result"][j]["cash_fee_in"].ToString();
                    }
                }
                else
                {
                    TipsManager._Instance.OpenWarningBox(jd["msg"].ToString());
                }
            }
        });
    }

    /// <summary>
    /// 请求流水详情
    /// </summary>
    private void On_btn_FlowDetailClick()
    {
        ClearGroup();
        _TableHeader.gameObject.SetActive(false);
        _TableHeader4.gameObject.SetActive(true);
        _btn_FundDetails.GetComponent<Button>().interactable = true;
        _btn_FlowDetail.GetComponent<Button>().interactable = false;

        HttpManager._Instance.StartPost(@"pay/capitalFlow", null, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(On_btn_FlowDetailClick);
                return;
            }
            else
            {
                JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

                if (jd["result"].Count == 0)
                {
                    _txt_NoData.gameObject.SetActive(true);
                }
                else
                {
                    _txt_NoData.gameObject.SetActive(false);
                }

                if (jd["code"].ToString() == "1")
                {
                    for (int j = 0; j < jd["result"].Count; j++)
                    {
                        GameObject go = Instantiate(InitialResourcesManager.WashCodeTableContent_2, _Group, false);

                        go.transform.GetChild(0).GetComponentInChildren<Text>().text = GameStatic.DateFormat(jd["result"][j]["create_date"].ToString());
                        go.transform.GetChild(1).GetComponentInChildren<Text>().text = jd["result"][j]["cash_fee_in"].ToString();
                        go.transform.GetChild(2).GetComponentInChildren<Text>().text = jd["result"][j]["fee"].ToString();
                        go.transform.GetChild(3).GetComponentInChildren<Text>().text = jd["result"][j]["flow_state"].ToString();
                    }
                }
                else
                {
                    TipsManager._Instance.OpenWarningBox(jd["msg"].ToString());
                }
            }
        });
    }

    private void GetWithdrawalDetails()
    {
        HttpManager._Instance.StartPost(@"pay/cashWithdrawalDetails", null, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(GetWithdrawalDetails);
                return;
            }
            else
            {
                Debug.Log(unityWebRequest.downloadHandler.text);
                JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

                if (jd["code"].ToString() == "1")
                {
                    _panel_WithdrawLog.gameObject.SetActive(true);

                    for (int i = 0; i < _group_WithdrawLog.childCount; i++)
                    {
                        Destroy(_group_WithdrawLog.GetChild(i).gameObject);
                    }

                    for (int i = 0; i < jd["result"].Count; i++)
                    {


                        GameObject crt = Instantiate(InitialResourcesManager.WithdrawLogTableContent, _group_WithdrawLog, false);

                        crt.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = GameStatic.DateFormat(jd["result"][i]["create_date"].ToString());
                        crt.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = jd["result"][i]["fee"].ToString();
                        crt.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = jd["result"][i]["withdraw_state"].ToString();
                    }
                }
                else
                {
                    TipsManager._Instance.OpenWarningBox(jd["msg"].ToString());
                }
            }
        });
    }

    private void OpenAddBankCardPanel(bool canReturn)
    {
        _panel_BindingCard.gameObject.SetActive(true);
        _panel_BindingCard.Find("btn_Return").gameObject.SetActive(canReturn);
        HttpManager._Instance.StartPost(@"not/common/gameBankTypeList", null, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(() => OpenAddBankCardPanel(canReturn));
                return;
            }

            JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

            if (jd["code"].ToString() == "1")
            {

                List<string> item = new List<string>();
                BankIds = new List<string>();

                for (int i = 0; i < jd["result"].Count; i++)
                {
                    if (jd["result"][i]["bankName"] != null)
                    {
                        BankIds.Add(jd["result"][i]["id"].ToString());
                        item.Add(jd["result"][i]["bankName"].ToString());
                    }
                }

                _panel_BindingCard.GetChild(0).Find("drd_Bank").GetComponent<Dropdown>().ClearOptions();
                _panel_BindingCard.GetChild(0).Find("drd_Bank").GetComponent<Dropdown>().AddOptions(item);
                _panel_BindingCard.GetChild(0).Find("drd_Bank").GetComponent<Dropdown>().onValueChanged.RemoveAllListeners();
                _panel_BindingCard.GetChild(0).Find("drd_Bank").GetComponent<Dropdown>().onValueChanged.AddListener((index) => selectBankId = index); ;

                _panel_BindingCard.Find("btn_Return").GetComponent<Button>().onClick.RemoveAllListeners();
                _panel_BindingCard.Find("btn_Return").GetComponent<Button>().onClick.AddListener(() => _panel_BindingCard.gameObject.SetActive(false));

                _panel_BindingCard.Find("btn_GoBinding").GetComponent<Button>().onClick.RemoveAllListeners();
                _panel_BindingCard.Find("btn_GoBinding").GetComponent<Button>().onClick.AddListener(() =>
                {
                    Onbtn_GoBindingClick(_panel_BindingCard.GetChild(0).Find("ipf_BankCard").GetComponent<InputField>().text,
                    BankIds[selectBankId],
                   _panel_BindingCard.GetChild(0).Find("ipf_Name").GetComponent<InputField>().text,
                    _panel_BindingCard.GetChild(0).Find("ipf_AccountOpeningAddress").GetComponent<InputField>().text);
                });

                _panel_BindingCard.GetChild(0).Find("ipf_Name").GetComponent<InputField>().text = string.Empty;
                _panel_BindingCard.GetChild(0).Find("ipf_BankCard").GetComponent<InputField>().text = string.Empty;
                _panel_BindingCard.GetChild(0).Find("ipf_AccountOpeningAddress").GetComponent<InputField>().text = string.Empty;
                _panel_BindingCard.GetChild(0).Find("drd_Bank").GetComponent<Dropdown>().value = 0;
            }
            else
            {
                TipsManager._Instance.OpenWarningBox(jd["msg"].ToString());
            }
        });
    }

    private int selectBankId;
    private List<string> BankIds = new List<string>();

    /// <summary>
    /// 绑定按钮的监听
    /// </summary>
    private void Onbtn_GoBindingClick(string cardNum, string cardType, string name, string bankAddress)
    {
        Debug.Log(cardNum);
        Dictionary<string, string> form = new Dictionary<string, string>();
        form.Add("cardNum", cardNum);
        form.Add("cardType", cardType);
        form.Add("name", name);
        form.Add("bankAddress", bankAddress);
        HttpManager._Instance.StartPost(@"member/center/InsertBankCard", form, (UnityWebRequest) =>
        {
            if (UnityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(() => Onbtn_GoBindingClick(cardNum, cardType, name, bankAddress));
                return;
            }

            JsonData jsonData = JsonMapper.ToObject(UnityWebRequest.downloadHandler.text);
            if (jsonData["code"].ToString() == "1")
            {
                _panel_BindingCard.gameObject.SetActive(false);

                ChangePanel(2);
            }
            else
            {
                TipsManager._Instance.OpenWarningBox(jsonData["msg"].ToString());
            }
        });
    }

    /// <summary>
    /// 清空表组
    /// </summary>
    private void ClearGroup()
    {
        for (int i = 0; i < _Group.childCount; i++)
        {
            Destroy(_Group.GetChild(i).gameObject);
        }
    }

    private void GetCrtBankCard()
    {
        GetBankCardById((jsonData) =>
        {
            if (jsonData == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(GetCrtBankCard);
                return;
            }
            if (jsonData["code"].ToString() == "1")
            {
                for (int i = 0; i < _panel_ChioceCard.GetChild(0).Find("msk_ChoiceCard").GetChild(0).childCount; i++)
                {
                    Destroy(_panel_ChioceCard.GetChild(0).Find("msk_ChoiceCard").GetChild(0).GetChild(i).gameObject);
                }

                if (jsonData["result"].Count == 0)
                {
                    _panel_NoCardTips.gameObject.SetActive(true);

                    _pl_CrtBindCard.gameObject.SetActive(false);
                }
                else
                {
                    _panel_NoCardTips.gameObject.SetActive(false);

                    _pl_CrtBindCard.gameObject.SetActive(true);

                    for (int i = 0; i < jsonData["result"].Count; i++)
                    {
                        GameObject crt = Instantiate(InitialResourcesManager.ge_ChioceCard, _panel_ChioceCard.GetChild(0).Find("msk_ChoiceCard").GetChild(0), false);

                        crt.transform.Find("txt_BankName").GetComponent<Text>().text = jsonData["result"][i]["bank_name"].ToString();

                        string cardTailNumber = jsonData["result"][i]["card_num"].ToString();
                        if (cardTailNumber.Length > 4)
                        {
                            cardTailNumber = "尾号" + cardTailNumber.Substring(cardTailNumber.Length - 4);
                        }

                        crt.transform.Find("txt_CardNum").GetComponent<Text>().text = cardTailNumber;
                        DynamicResourceManager._Instance.StartSetTexture(crt.transform.Find("img_Icon").GetComponent<Image>(), jsonData["result"][i]["icon"].ToString());

                        string id = jsonData["result"][i]["id"].ToString();
                        int index = i;

                        crt.transform.GetComponent<Button>().onClick.AddListener(()=> 
                        {
                            if (crt.transform.Find("img_Choice").gameObject.activeSelf==false)
                            {
                                crtCardID = id;
                                _pl_CrtBindCard.Find("txt_BankName").GetComponent<Text>().text = jsonData["result"][index]["bank_name"].ToString();

                                string crtCardTailNumber = jsonData["result"][index]["card_num"].ToString();
                                if (crtCardTailNumber.Length > 4)
                                {
                                    crtCardTailNumber = "尾号" + crtCardTailNumber.Substring(crtCardTailNumber.Length - 4);
                                }
                                _pl_CrtBindCard.Find("txt_CardNumber").GetComponent<Text>().text = crtCardTailNumber;
                                DynamicResourceManager._Instance.StartSetTexture(_pl_CrtBindCard.Find("img_Icon").GetComponent<Image>(), jsonData["result"][index]["icon"].ToString());

                                for (int j = 0; j < crt.transform.parent.childCount; j++)
                                {
                                    if (j == index)
                                    {
                                        crt.transform.parent.GetChild(j).Find("img_Choice").gameObject.SetActive(true);
                                    }
                                    else
                                    {
                                        crt.transform.parent.GetChild(j).Find("img_Choice").gameObject.SetActive(false);
                                    }
                                }
                            }
                            modifyBankState(id); 
                        });   

                        if (jsonData["result"][i]["is_default"].ToString() == "1")
                        {
                            crtCardID = id;
                            crt.transform.Find("img_Choice").gameObject.SetActive(true);

                            _pl_CrtBindCard.Find("txt_BankName").GetComponent<Text>().text = jsonData["result"][i]["bank_name"].ToString(); 
                            string showCardTailNumber = jsonData["result"][i]["card_num"].ToString();
                            if (showCardTailNumber.Length > 4)
                            {
                                showCardTailNumber = "尾号" + showCardTailNumber.Substring(showCardTailNumber.Length - 4);
                            }
                            _pl_CrtBindCard.Find("txt_CardNumber").GetComponent<Text>().text = showCardTailNumber;
                            DynamicResourceManager._Instance.StartSetTexture(_pl_CrtBindCard.Find("img_Icon").GetComponent<Image>(), jsonData["result"][i]["icon"].ToString());
                        }
                        else
                        {
                            crt.transform.Find("img_Choice").gameObject.SetActive(false);
                        }
                    }
                }
            }
            else
            {
                TipsManager._Instance.OpenWarningBox(jsonData["msg"].ToString());
            }
        });
    }

    /// <summary>
    /// 获取银行卡信息
    /// </summary>
    /// <param name="action"></param>
    private void modifyBankState(string id)
    {
        Dictionary<string, string> form = new Dictionary<string, string>();
        form.Add("Id", id);
        HttpManager._Instance.StartPost(@"member/center/modifyBankState", form, (untiyWebRequest) =>
        {
            if (untiyWebRequest == null)
            {
                
                return;
            }
            Debug.Log(untiyWebRequest.downloadHandler.text);

        });
    }

    /// <summary>
    /// 获取银行卡信息
    /// </summary>
    /// <param name="action"></param>
    private void GetBankCardById(Action<JsonData> action)
    {
        HttpManager._Instance.StartPost(@"member/center/findGameBankBymId", null, (untiyWebRequest) =>
        {
            if (untiyWebRequest == null)
            {
                action(null);
                return;
            }

            Debug.Log(untiyWebRequest.downloadHandler.text);
            JsonData jsonData = JsonMapper.ToObject(untiyWebRequest.downloadHandler.text);
            action(jsonData);

        });
    }

    private void RefreshBankCardInfo()
    {
        GetBankCardById((jsonData) =>
        {
            if (jsonData == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(RefreshBankCardInfo);
                return;
            }
            if (jsonData["code"].ToString() == "1")
            {
                for (int i = 0; i < _BankCardGroup.childCount; i++)
                {
                    Destroy(_BankCardGroup.GetChild(i).gameObject);
                }

                if (jsonData["result"].Count == 0)
                {
                    OpenAddBankCardPanel(false);
                    return;
                }
                for (int i = 0; i < jsonData["result"].Count; i++)
                {
                    string bankName = jsonData["result"][i]["bank_name"].ToString();
                    string cardTailNumber = jsonData["result"][i]["card_num"].ToString();
                    if (cardTailNumber.Length > 4)
                    {
                        cardTailNumber = "尾号"+ cardTailNumber.Substring(cardTailNumber.Length - 4);
                    }

                    GameObject crt = Instantiate(InitialResourcesManager.BankCardGridContent, _BankCardGroup, false);
                    DynamicResourceManager._Instance.StartSetTexture(crt.transform.Find("img_BankIcon").GetComponent<Image>(), jsonData["result"][i]["icon"].ToString());
                    crt.transform.Find("txt_BankName").GetComponent<Text>().text = bankName;
                    crt.transform.Find("txt_UserName").GetComponent<Text>().text = cardTailNumber;
                }
            }
            else
            {
                TipsManager._Instance.OpenWarningBox(jsonData["msg"].ToString());
            }
        });
    }


    /// <summary>
    /// 切换面板
    /// </summary>
    /// <param name="index"></param>
    private void ChangePanel(int index)
    {
        if (LocalFileManager._Instance._GameData._IsLogin == false)
        {
            return;
        }

        if (index == 0)
        {
            GetMoneyNumber();
            GetCrtBankCard();
        }
        else if (index == 1)
        {
            On_btn_FundDetailsClick();
        }
        else if (index == 2)
        {
            RefreshBankCardInfo();
        }

        for (int i = 0; i < _Panels.Count; i++)
        {
            if (i == index)
            {
                _Buttons[i].transform.GetChild(0).gameObject.SetActive(true);
                _Panels[i].gameObject.SetActive(true);
            }
            else
            {
                _Buttons[i].transform.GetChild(0).gameObject.SetActive(false);
                _Panels[i].gameObject.SetActive(false);
            }
        }
    }

    private void Reset()
    {
        for (int i = 0; i < _Buttons.Count; i++)
        {
            _Buttons[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        _ipf_Num.text = string.Empty;

        _panel_Withdraw.gameObject.SetActive(false);
        _panel_BindingCard.gameObject.SetActive(false);

        _panel_WithdrawLog.gameObject.SetActive(false);
        _panel_ChioceCard.gameObject.SetActive(false);
    }

    private void ReturnMainMenu()
    {
        GameStatic.OnChangePanel?.Invoke(Panel.MainMenu);
        Reset();
    }
}
