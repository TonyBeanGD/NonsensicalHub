using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class RechargePanelManager : MonoBehaviour
{
    private Transform _panel_Recharge;
    private Transform _panel_RechargeLog;

    private List<Button> _Buttons;
    private List<Transform> _Panels;

    private Transform _VipGroup;
    private Transform _BankCardGroup;
    private Transform _BankCardTransferPanel;
    private InputField _ipf_DepositInformation;
    private InputField _ipf_DepositAmount;
    private Button _btn_ReturnTransfer;
    private Button _btn_SumbitTransfer;
    private Button _btn_Return;                     //返回
    private Button _btn_RechargeLog;                //充值记录
    private Button _btn_RefreshMoney;               //刷新钱数按钮
    private Text _txt_MoneyNum;                     //当前钱数文本

    private void ChangePanel(Panel panel)
    {
        if (panel == Panel.Recharge)
        {
            UIUpdate();
            _panel_Recharge.gameObject.SetActive(true);

            AudioSourceManager._Instance.PlayCharacterSpeech(InitialResourcesManager.aud_Recharge);
        }
        else
        {
            if (_panel_Recharge.gameObject.activeSelf == true)
            {
                _panel_Recharge.gameObject.SetActive(false);
            }
        }
    }

    void Awake()
    {
        //Application.targetFrameRate = 30;
        GameStatic.OnChangePanel += ChangePanel;
        GameStatic.OnLogin += OnLogin;
        GameStatic.OnMainPanelInit += Init_State;
        GameStatic.OnLogout += () =>
        {
            for (int i = 2; i < _panel_Recharge.Find("ButtonBG").Find("Mask").Find("Buttons").childCount; i++)
            {
                Destroy(_panel_Recharge.Find("ButtonBG").Find("Mask").Find("Buttons").GetChild(i).gameObject);
                Destroy(_panel_Recharge.Find("Panels").GetChild(i).gameObject);
            }
        };
        Init_Variable();
        Init_Listener();
    }


    private void Init_Variable()
    {
        _panel_Recharge = transform;

        _btn_Return = _panel_Recharge.Find("Header").Find("btn_Return").GetComponent<Button>();
        _btn_RechargeLog = _panel_Recharge.Find("Header").Find("btn_RechargeLog").GetComponent<Button>();
        _btn_RefreshMoney = _panel_Recharge.Find("Header").Find("btn_Refresh").GetComponent<Button>();

        _txt_MoneyNum = _panel_Recharge.Find("Header").Find("img_MoneyNum").GetChild(0).GetComponent<Text>();
        _panel_RechargeLog = _panel_Recharge.Find("panel_ReceiveLog");
    }

    private void InitButtonList()
    {
        _Buttons = new List<Button>();
        for (int i = 0; i < _panel_Recharge.Find("ButtonBG").Find("Mask").Find("Buttons").childCount; i++)
        {
            _Buttons.Add(_panel_Recharge.Find("ButtonBG").Find("Mask").Find("Buttons").GetChild(i).GetComponent<Button>());
        }

        _Panels = new List<Transform>();
        for (int i = 0; i < _panel_Recharge.Find("Panels").childCount; i++)
        {
            _Panels.Add(_panel_Recharge.Find("Panels").GetChild(i));
        }

        _VipGroup = _Panels[0].Find("ChoicePanel").GetChild(0);
        _BankCardGroup = _Panels[1].Find("ChoiceBankCard").GetChild(0);
        _BankCardTransferPanel = _Panels[1].Find("BankCardTransferPanel");

        _ipf_DepositInformation = _BankCardTransferPanel.Find("img_Background2").Find("ipf_DepositInformation").GetComponent<InputField>();
        _ipf_DepositAmount = _BankCardTransferPanel.Find("img_Background2").Find("ipf_DepositAmount").GetComponent<InputField>();
        _btn_ReturnTransfer = _BankCardTransferPanel.Find("btn_Return").GetComponent<Button>();
        _btn_SumbitTransfer = _BankCardTransferPanel.Find("btn_Submit").GetComponent<Button>();

        for (int i = 0; i < _Buttons.Count; i++)
        {
            int index = i;

            _Buttons[i].onClick.AddListener(() => { ChangePanel(index); });
        }
    }

    private void Init_Listener()
    {
        InitButtonList();

        _btn_Return.onClick.AddListener(ReturnMainMenu);

        _btn_RechargeLog.onClick.AddListener(GetRechargeHistory);

        _btn_RefreshMoney.onClick.AddListener(RefreshMoney);

        _btn_ReturnTransfer.onClick.AddListener(() => { _BankCardTransferPanel.gameObject.SetActive(false); });
        _btn_SumbitTransfer.onClick.AddListener(BankCardTransfer);

        _panel_RechargeLog.Find("btn_Return").GetComponent<Button>().onClick.AddListener(() => { _panel_RechargeLog.gameObject.SetActive(false); });
    }

    private void Init_State()
    {
        LoadingManager.LoadingStart(this.GetType().ToString());
        Reset();
        LoadingManager.LoadingComplete(this.GetType().ToString());
    }

    private void ChangePanel(int index)
    {
        for (int i = 0; i < _Buttons.Count; i++)
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

        if (index > 1)
        {
            if (_Panels[index].GetChild(0).GetChild(0).childCount > 0)
            {
                _Panels[index].GetChild(0).GetChild(0).GetChild(0).GetComponent<Button>().onClick.Invoke();
            }
        }
        else if (_panel_Recharge.gameObject.activeSelf == true)
        {

            AudioSourceManager._Instance.PlayCharacterSpeech(InitialResourcesManager.aud_VipOrBankCard);
        }
    }

    private void Reset()
    {
        _panel_RechargeLog.gameObject.SetActive(false);
        for (int i = 0; i < _Buttons.Count; i++)
        {
            _Buttons[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        _panel_Recharge.gameObject.SetActive(false);
        ChangePanel(0);
    }

    private void ReturnMainMenu()
    {
        GameStatic.OnChangePanel?.Invoke(Panel.MainMenu);
        Reset();
    }

    private void UIUpdate()
    {
        RefreshMoney();
    }

    /// <summary>
    /// 刷新钱数 
    /// </summary>
    private void RefreshMoney()
    {
        _txt_MoneyNum.text = "刷新中。。";
        HttpManager._Instance.StartGetBanance((balance) => _txt_MoneyNum.text = balance);
    }

    private void OnLogin()
    {
        GetBankCardRechargeInfo();
        GetVipRechageInfo();
        AcquisitionPayType();
    }

    /// <summary>
    /// 获取所有支付类型及其id
    /// </summary>
    private void AcquisitionPayType()
    {
        HttpManager._Instance.StartPost(@"pay/findAllPayType", null, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(AcquisitionPayType);
                return;
            }

            Debug.Log(unityWebRequest.downloadHandler.text);
            JsonData jsondata = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
            if (jsondata["code"].ToString() == "1")
            {
                JsonData result = jsondata["result"];
                for (int i = 0; i < result.Count; i++)
                {
                    int index = i + 2;
                    GetreChargeChannel(index, result[i]["id"].ToString());

                    GameObject button = Instantiate(InitialResourcesManager.btn_RechargeType, _panel_Recharge.Find("ButtonBG").Find("Mask").Find("Buttons"), false);

                    DynamicResourceManager._Instance.StartSetTexture(button.GetComponent<Image>(), result[i]["icon"].ToString(), false);
                    DynamicResourceManager._Instance.StartSetTexture(button.transform.GetChild(0).GetComponent<Image>(), result[i]["brightIcon"].ToString(), false);

                    string tagMessage = GameStatic.CheckNull(result[i]["discountRate"]);

                    if (tagMessage.Equals(string.Empty))
                    {
                        button.transform.Find("img_Tag").gameObject.SetActive(false);
                    }
                    else
                    {
                        button.transform.Find("img_Tag").gameObject.SetActive(true);
                        button.transform.Find("img_Tag").GetChild(0).GetComponent<Text>().text = tagMessage;
                    }

                    Instantiate(InitialResourcesManager.RechargeSubPanel, _panel_Recharge.Find("Panels"), false);
                }

                InitButtonList();
                ChangePanel(0);
            }
            else
            {
                TipsManager._Instance.OpenWarningBox(jsondata["msg"].ToString());
            }
        });
    }

    /// <summary>
    /// 获取银行卡充值信息
    /// </summary>
    private void GetBankCardRechargeInfo()
    {
        _BankCardTransferPanel.gameObject.SetActive(false);
        HttpManager._Instance.StartPost(@"pay/findBankTransferType", null, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(GetBankCardRechargeInfo);
                return;
            }

            JsonData jsonData = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

            if (jsonData["code"].ToString() == "1")
            {
                string tagMessage = GameStatic.CheckNull(jsonData["result"]["tag"]);

                if (tagMessage.Equals(string.Empty))
                {
                    _Buttons[0].transform.Find("img_Tag").gameObject.SetActive(false);
                }
                else
                {
                    _Buttons[0].transform.Find("img_Tag").gameObject.SetActive(true);
                    _Buttons[0].transform.Find("img_Tag").GetChild(0).GetComponent<Text>().text = tagMessage;
                }

                for (int i = 0; i < _BankCardGroup.childCount; i++)
                {
                    Destroy(_BankCardGroup.GetChild(i).gameObject);
                }

                JsonData result = jsonData["result"]["Obj"];

                for (int i = 0; i < result.Count; i++)
                {
                    string bankAccount = result[i]["bankAccount"].ToString();
                    string payee = result[i]["payee"].ToString();
                    string quota = result[i]["quota"].ToString();
                    string title = result[i]["title"].ToString();
                    string bankAddress = result[i]["bankAddress"].ToString();
                    string bankCardId = result[i]["id"].ToString();

                    string bankName = result[i]["bankTypeObj"]["bankName"].ToString();
                    string icon = result[i]["bankTypeObj"]["icon"].ToString();

                    GameObject crtPanel = Instantiate(InitialResourcesManager.panel_BankCard, _BankCardGroup, false);

                    DynamicResourceManager._Instance.StartSetTexture(crtPanel.transform.Find("img_Icon").GetComponent<Image>(), icon);
                    crtPanel.transform.Find("txt_BankName").GetComponent<Text>().text = bankName;
                    crtPanel.transform.Find("txt_Title").GetComponent<Text>().text = title;
                    crtPanel.transform.Find("txt_AccountNo").GetComponent<Text>().text = bankAccount;
                    crtPanel.transform.Find("btn_Submit").GetComponent<Button>().onClick.AddListener(() =>
                    {
                        _CrtBankCardId = bankCardId;
                        _BankCardTransferPanel.gameObject.SetActive(true);
                        _BankCardTransferPanel.Find("img_Background1").Find("Values").GetChild(0).GetComponent<Text>().text = bankName;
                        _BankCardTransferPanel.Find("img_Background1").Find("Values").GetChild(1).GetComponent<Text>().text = payee;
                        _BankCardTransferPanel.Find("img_Background1").Find("Values").GetChild(2).GetComponent<Text>().text = bankAccount;
                        _BankCardTransferPanel.Find("img_Background1").Find("Values").GetChild(3).GetComponent<Text>().text = bankAddress;

                        _BankCardTransferPanel.Find("img_Background1").Find("btn_Copy1").GetComponent<Button>().onClick.RemoveAllListeners();
                        _BankCardTransferPanel.Find("img_Background1").Find("btn_Copy1").GetComponent<Button>().onClick.AddListener(() => { GameStatic.CopyString(payee); });

                        _BankCardTransferPanel.Find("img_Background1").Find("btn_Copy2").GetComponent<Button>().onClick.RemoveAllListeners();
                        _BankCardTransferPanel.Find("img_Background1").Find("btn_Copy2").GetComponent<Button>().onClick.AddListener(() => { GameStatic.CopyString(bankAccount); });

                        _BankCardTransferPanel.Find("img_Background2").Find("ipf_DepositAmount").Find("Placeholder").GetComponent<Text>().text = quota;
                        _BankCardTransferPanel.Find("img_Background2").Find("ipf_DepositAmount").GetComponent<InputField>().text = string.Empty;
                        _BankCardTransferPanel.Find("img_Background2").Find("ipf_DepositInformation").GetComponent<InputField>().text = string.Empty;
                    });
                }
            }
            else
            {
                jsonData["msg"].ToString();
            }
        });
    }

    private string _CrtBankCardId;

    /// <summary>
    /// 获取vip充值通道信息
    /// </summary>
    private void GetVipRechageInfo()
    {
        HttpManager._Instance.StartPost(@"pay/findVipRechargeType", null, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(GetVipRechageInfo);
                return;
            }

            JsonData jsonData = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

            if (jsonData["code"].ToString() == "1")
            {
                for (int i = 0; i < _VipGroup.childCount; i++)
                {
                    Destroy(_VipGroup.GetChild(i).gameObject);
                }

                string tagMessage = GameStatic.CheckNull(jsonData["result"]["tag"]);

                if (tagMessage.Equals(string.Empty))
                {
                    _Buttons[1].transform.Find("img_Tag").gameObject.SetActive(false);
                }
                else
                {
                    _Buttons[1].transform.Find("img_Tag").gameObject.SetActive(true);
                    _Buttons[1].transform.Find("img_Tag").GetChild(0).GetComponent<Text>().text = tagMessage;
                }

                JsonData result = jsonData["result"]["Obj"];
                for (int i = 0; i < result.Count; i++)
                {
                    string accountNo = result[i]["accountNo"].ToString();
                    string iconUrl = result[i]["icon"].ToString();
                    string memo = result[i]["memo"].ToString();
                    string title = result[i]["title"].ToString();

                    GameObject crtPanel = Instantiate(InitialResourcesManager.panel_VIP, _VipGroup, false);

                    DynamicResourceManager._Instance.StartSetTexture(crtPanel.transform.Find("img_Icon").GetComponent<Image>(), iconUrl);
                    crtPanel.transform.Find("txt_Title").GetComponent<Text>().text = title;
                    crtPanel.transform.Find("txt_Type").GetComponent<Text>().text = memo;
                    crtPanel.transform.Find("txt_Num").GetComponent<Text>().text = accountNo;
                    crtPanel.transform.Find("btn_Copy").GetChild(0).GetComponent<Text>().text = "复制" + memo;
                    crtPanel.transform.Find("btn_Copy").GetComponent<Button>().onClick.RemoveAllListeners();
                    crtPanel.transform.Find("btn_Copy").GetComponent<Button>().onClick.AddListener(() => { GameStatic.CopyString(accountNo); });
                }
            }
            else
            {
                TipsManager._Instance.OpenWarningBox(jsonData["msg"].ToString());
            }
        });
    }

    /// <summary>
    /// 银行卡转账
    /// </summary>
    private void BankCardTransfer()
    {
        Dictionary<string, string> form = new Dictionary<string, string>();
        form.Add("btId", _CrtBankCardId);
        form.Add("info", _ipf_DepositInformation.text);
        form.Add("money", _ipf_DepositAmount.text);

        HttpManager._Instance.StartPost(@"pay/addBankTransferRecord", form, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(BankCardTransfer);
                return;
            }

            JsonData jsondata = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

            if (jsondata["code"].ToString() == "1")
            {
                TipsManager._Instance.OpenSuccessLable("信息已提交");
            }
            else
            {
                TipsManager._Instance.OpenWarningBox(jsondata["msg"].ToString());
            }
        });
    }

    /// <summary>
    /// 获取充值通道
    /// </summary>
    /// <param name="payTypeID"></param>
    private void GetreChargeChannel(int index, string payTypeID)
    {
        Dictionary<string, string> form = new Dictionary<string, string>();
        form.Add("payTypeId", payTypeID);

        HttpManager._Instance.StartPost(@"pay/findPayApiByPayType", form, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(() => { GetreChargeChannel(index, payTypeID); });
                return;
            }


            JsonData jsonData = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

            if (jsonData["code"].ToString() == "1")
            {
                Transform subPanel = _panel_Recharge.Find("Panels").GetChild(index);
                InputField ipf_money = subPanel.Find("img_Input").GetChild(0).GetComponent<InputField>();
                Button btn_submit = subPanel.Find("img_Input").GetChild(1).GetComponent<Button>();
                Transform buttonGroup = subPanel.Find("Header").GetChild(0);
                Transform moneyGroup = subPanel.Find("MoneyChoice").GetChild(0);
                Text txt_Tips = subPanel.Find("img_Input").Find("txt_Tips").GetComponent<Text>();

                for (int i = 0; i < jsonData["result"].Count; i++)
                {
                    string name = jsonData["result"][i]["name"].ToString();
                    string id = jsonData["result"][i]["id"].ToString();
                    bool fixedMoney = jsonData["result"][i]["fixed_money"].ToString().Equals("1") ? true : false;
                    string[] moneyList = JsonMapper.ToObject<string[]>(jsonData["result"][i]["moneyList"].ToJson());

                    string[] msgList = JsonMapper.ToObject<string[]>(jsonData["result"][i]["msg_list"].ToJson());

                    string msg = jsonData["result"][i]["msg"].ToString();

                    GameObject button = Instantiate(InitialResourcesManager.btn_PayChannel, buttonGroup, false);
                    button.transform.Find("txt_Name").GetComponent<Text>().text = name;
                    if (msg.Equals("-") || msg.Equals(string.Empty))
                    {
                        button.transform.Find("img_Tag").gameObject.SetActive(false);
                    }
                    else
                    {
                        button.transform.Find("img_Tag").gameObject.SetActive(true);
                        button.transform.Find("img_Tag").GetChild(0).GetComponent<Text>().text = msg;
                    }

                    button.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        ipf_money.text = string.Empty;
                        if (fixedMoney)
                        {
                            ipf_money.interactable = false;
                            txt_Tips.gameObject.SetActive(true);
                        }
                        else
                        {
                            ipf_money.interactable = true;
                            txt_Tips.gameObject.SetActive(false);
                        }

                        for (int j = 0; j < moneyGroup.childCount; j++)
                        {
                            Destroy(moneyGroup.GetChild(j).gameObject);
                        }

                        for (int j = 0; j < moneyList.Length; j++)
                        {
                            GameObject money = Instantiate(InitialResourcesManager.btn_MoneyNum, moneyGroup, false);
                            money.transform.GetChild(1).GetComponent<Text>().text = moneyList[j] + "元";
                            money.transform.Find("img_Hightlight").gameObject.SetActive(false);
                            //if (msgList[j].Equals("-") || msgList[j].Equals(string.Empty))
                            //{
                            //    money.transform.GetChild(2).gameObject.SetActive(false);
                            //}
                            //else
                            //{
                            //    money.transform.GetChild(2).gameObject.SetActive(true);
                            //    money.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = msgList[j];
                            //}

                            int k = j;
                            money.GetComponent<Button>().onClick.AddListener(() =>
                            {
                                ipf_money.text = money.transform.GetChild(1).GetComponent<Text>().text.Substring(0, money.transform.GetChild(1).GetComponent<Text>().text.Length - 1);
                                ChangeEffect(money.transform.parent, k);
                            });
                        }

                        for (int j = 0; j < button.transform.parent.childCount; j++)
                        {
                            if (button.transform.parent.GetChild(j).gameObject != button)
                            {
                                button.transform.parent.GetChild(j).GetChild(0).gameObject.SetActive(false);
                                button.transform.parent.GetChild(j).GetChild(1).GetComponent<Text>().color = GameStatic.GoldColor;
                            }
                            else
                            {
                                button.transform.parent.GetChild(j).GetChild(0).gameObject.SetActive(true);
                                button.transform.parent.GetChild(j).GetChild(1).GetComponent<Text>().color = Color.black;
                            }
                        }

                        btn_submit.onClick.RemoveAllListeners();
                        btn_submit.onClick.AddListener(() =>
                        {
                            if (ipf_money.Equals(string.Empty))
                            {
                                TipsManager._Instance.OpenTipsText("金额不能为空");
                                return;
                            }
                            OrderRecharge(id, ipf_money.text);
                        });
                    });

                }
            }
            else
            {
                TipsManager._Instance.OpenWarningBox(jsonData["msg"].ToString());
            }
        });
    }

    private void ChangeEffect(Transform parent, int index)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            if (i == index)
            {
                parent.GetChild(i).Find("img_Hightlight").gameObject.SetActive(true);
                parent.GetChild(i).Find("Text").GetComponent<Text>().color = Color.black;
            }
            else
            {
                parent.GetChild(i).Find("img_Hightlight").gameObject.SetActive(false);
                parent.GetChild(i).Find("Text").GetComponent<Text>().color = Color.white;
            }
        }

    }

    /// <summary>
    /// 获取充值记录
    /// </summary>
    private void GetRechargeHistory()
    {
        HttpManager._Instance.StartPost(@"pay/orderListCall", null, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(GetRechargeHistory);
                return;
            }

            JsonData jsonData = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

            if (jsonData["code"].ToString() == "1")
            {
                _panel_RechargeLog.gameObject.SetActive(true);
                Transform group = _panel_RechargeLog.Find("Window").Find("TableBody").GetChild(0);

                for (int i = 0; i < group.childCount; i++)
                {
                    Destroy(group.GetChild(i).gameObject);
                }

                for (int i = 0; i < jsonData["result"].Count; i++)
                {
                    GameObject go = Instantiate(InitialResourcesManager.RechageLogTableElement, group, false);
                    go.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = GameStatic.CheckNull(jsonData["result"][i]["order_no"]);
                    go.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = GameStatic.CheckNull(jsonData["result"][i]["channel"]);
                    go.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = GameStatic.CheckNull(jsonData["result"][i]["recharge_way"]);
                    go.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = GameStatic.CheckNull(jsonData["result"][i]["fee"]);
                    go.transform.GetChild(4).GetChild(0).GetComponent<Text>().text = GameStatic.CheckNull(jsonData["result"][i]["recharge_state"]);
                    go.transform.GetChild(5).GetChild(0).GetComponent<Text>().text = GameStatic.DateFormat(jsonData["result"][i]["create_date"].ToString());
                }
            }
            else
            {
                TipsManager._Instance.OpenWarningBox(jsonData["msg"].ToString());
            }

        });
    }

    /// <summary>
    /// 充值下单
    /// </summary>
    private void OrderRecharge(string id, string money)
    {
        Debug.Log(id);
        Debug.Log(money);

        Dictionary<string, string> form = new Dictionary<string, string>();
        form.Add("id", id);
        form.Add("monery", money);

        HttpManager._Instance.StartPost(@"pay/orderRemoteCall", form, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(() => OrderRecharge(id, money));
                return;
            }

            JsonData jsondata = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

            if (jsondata["code"].ToString() == "1")
            {
                Application.OpenURL(jsondata["result"]["url"].ToString());
            }
            else
            {
                TipsManager._Instance.OpenWarningBox(jsondata["msg"].ToString());
            }
        });
    }
}
