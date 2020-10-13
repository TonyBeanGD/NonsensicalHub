using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

public class PromotionPanelManager : MonoBehaviour
{
    private Transform _panel_Promotion;
    private Button _btn_Return;

    private Transform _panel_QRCode;
    private Button _btn_QRCodeClose;
    private RawImage _rimg_QRCode;

    private Transform _panel_TakeOutCommission;
    private Button _btn_TakeOutClose;
    private Text _txt_TakeOutCanReceive;
    private Text _txt_TakeOutWalletMoney;
    private InputField _ipf_TakeOut;
    private Button _btn_AllTurnOff;
    private Button _btn_TakeOut;
    private Button _btn_ContactGM;

    private Transform _panel_ReceiveLog;
    private Button _btn_ReceiveLogClose;
    private Transform _ReceiveLogGroup;

    private Transform _panel_RebateTable;
    private Button _btn_RebateTableClose;

    private List<Button> _Buttons;
    private List<Transform> _Panels;
    private Button _btn_ReceiveCommission;
    private Button _btn_ReceiveLog;
    private Button _btn_RebateTable;
    private Button _btn_QRCode;
    private Button _btn_CopyUrl;
    private Text _txt_MyID;
    private Text _txt_RecommendedID;
    private Text _txt_TuanDuiRenShuXinZeng;
    private Text _txt_JinRiTuanDuiYeJi;
    private Text _txt_ZhiShuRenShuXinZeng;
    private Text _txt_JinRiZiYingYeJi;
    private Text _txt_JinRiYongJinYuGu;
    private Text _txt_ZuoRiYongJinJieSuan;
    private Text _txt_HistoricalCommission;
    private Text _txt_CanReceiveCommission;
    private Text _txt_PromoteUrl;
    private RawImage _rimg_OriginQRCode;

    private Transform _DirectlyTableGroup;
    private InputField _ipf_DirectlyPlayerID;
    private Text _txt_DirectlyNoData;
    private Button _btn_DirectlyInquire;
    private Button _btn_DirectlyReset;
    private Text _txt_DirectNumber;
    private Text _txt_DirectAgent;
    private Text _txt_TotalWaterToday;

    private Transform _PerformanceTableGroup;
    private InputField _ipf_PerformancePlayerID;
    private Text _txt_PerformanceNoData;
    private Button _btn_PerformanceInquire;


    private void ChangePanel(Panel panel)
    {
        if (panel == Panel.Promotion)
        {
            Reset();
            ChangePanel(0);
            UpdateUI(0);
            _panel_Promotion.gameObject.SetActive(true);

            AudioSourceManager._Instance.PlayCharacterSpeech(InitialResourcesManager.aud_Promotion);
        }
        else
        {
            if (_panel_Promotion.gameObject.activeSelf == true)
            {
                _panel_Promotion.gameObject.SetActive(false);
            }
        }
    }

    private void OnLogin()
    {
        InitRebateTable();
        Reset();
    }

    void Awake()
    {
        GameStatic.OnChangePanel += ChangePanel;
        GameStatic.OnLogin += OnLogin;
        GameStatic.OnMainPanelInit += Init_State;
        Init_Variable();
        Init_Listener();
    }

    private void Init_Variable()
    {
        _panel_Promotion = transform;

        _btn_Return = _panel_Promotion.Find("btn_Return").GetComponent<Button>();

        _panel_QRCode = _panel_Promotion.Find("panel_QRCode");
        _btn_QRCodeClose = _panel_QRCode.GetChild(0).GetComponent<Button>();
        _rimg_QRCode = _panel_QRCode.GetChild(1).GetChild(0).GetComponent<RawImage>();

        _panel_TakeOutCommission = _panel_Promotion.Find("panel_TakeOutCommission");
        _txt_TakeOutCanReceive = _panel_TakeOutCommission.GetChild(1).Find("txt_KeLingQuJinE").GetChild(0).GetComponent<Text>();
        _txt_TakeOutWalletMoney = _panel_TakeOutCommission.GetChild(1).Find("txt_ZhanHaoYuE").GetChild(0).GetComponent<Text>();
        _btn_TakeOutClose = _panel_TakeOutCommission.GetChild(0).GetComponent<Button>();
        _ipf_TakeOut = _panel_TakeOutCommission.GetChild(1).Find("img_Input").GetChild(0).GetComponent<InputField>();
        _btn_AllTurnOff = _panel_TakeOutCommission.GetChild(1).Find("img_Input").GetChild(1).GetComponent<Button>();
        _btn_TakeOut = _panel_TakeOutCommission.GetChild(1).Find("btn_ConfirmReceipt").GetComponent<Button>();
        _btn_ContactGM = _panel_TakeOutCommission.GetChild(1).Find("txt_Tips").GetChild(1).GetComponent<Button>();

        _panel_ReceiveLog = _panel_Promotion.Find("panel_ReceiveLog");
        _btn_ReceiveLogClose = _panel_ReceiveLog.GetChild(0).GetComponent<Button>();
        _ReceiveLogGroup = _panel_ReceiveLog.GetChild(1).Find("ReceiveLogTable").GetChild(0);

        _panel_RebateTable = _panel_Promotion.Find("panel_RebateTable");
        _btn_RebateTableClose = _panel_RebateTable.GetChild(0).GetComponent<Button>();

        _Buttons = new List<Button>();
        for (int i = 0; i < _panel_Promotion.Find("Buttons").childCount; i++)
        {
            _Buttons.Add(_panel_Promotion.Find("Buttons").GetChild(i).GetComponent<Button>());
        }

        _Panels = new List<Transform>();
        for (int i = 0; i < _panel_Promotion.Find("Panels").childCount; i++)
        {
            _Panels.Add(_panel_Promotion.Find("Panels").GetChild(i));
        }

        _btn_ReceiveCommission = _Panels[0].Find("btn_ReceiveCommission").GetComponent<Button>();
        _btn_ReceiveLog = _Panels[0].Find("btn_ReceiveLog").GetComponent<Button>();
        _btn_RebateTable = _Panels[0].Find("btn_RebateTable").GetComponent<Button>();
        _btn_QRCode = _Panels[0].Find("panel_QRCode").GetChild(0).GetComponent<Button>();
        _btn_CopyUrl = _Panels[0].Find("Panel_ShareURL").GetChild(0).GetComponent<Button>();
        _txt_MyID = _Panels[0].GetChild(0).Find("txt_MyID").GetComponent<Text>();
        _txt_RecommendedID = _Panels[0].GetChild(0).Find("txt_RecommendedID").GetComponent<Text>();
        _txt_TuanDuiRenShuXinZeng = _Panels[0].GetChild(0).Find("txt_TuanDuiRenShuXinZeng").GetComponent<Text>();
        _txt_JinRiTuanDuiYeJi = _Panels[0].GetChild(0).Find("txt_JinRiTuanDuiYeJi").GetComponent<Text>();
        _txt_ZhiShuRenShuXinZeng = _Panels[0].GetChild(0).Find("txt_ZhiShuRenShuXinZeng").GetComponent<Text>();
        _txt_JinRiZiYingYeJi = _Panels[0].GetChild(0).Find("txt_JinRiZiYingYeJi").GetComponent<Text>();
        _txt_JinRiYongJinYuGu = _Panels[0].GetChild(0).Find("txt_JinRiYongJinYuGu").GetComponent<Text>();
        _txt_ZuoRiYongJinJieSuan = _Panels[0].GetChild(0).Find("txt_ZuoRiYongJinJieSuan").GetComponent<Text>();
        _txt_HistoricalCommission = _Panels[0].Find("panel_HistoricalCommission").GetChild(0).GetComponent<Text>();
        _txt_CanReceiveCommission = _Panels[0].Find("panel_CanReceiveCommission").GetChild(0).GetComponent<Text>();
        _txt_PromoteUrl = _Panels[0].Find("Panel_ShareURL").GetChild(1).GetComponent<Text>();
        _rimg_OriginQRCode = _Panels[0].Find("panel_QRCode").GetChild(0).GetComponent<RawImage>();

        _DirectlyTableGroup = _Panels[1].GetChild(0).Find("Mask").GetChild(0);
        _txt_DirectlyNoData = _Panels[1].GetChild(0).Find("txt_NoData").GetComponent<Text>();
        _ipf_DirectlyPlayerID = _Panels[1].GetChild(0).Find("ipf_PlayerID").GetComponent<InputField>();
        _btn_DirectlyInquire = _Panels[1].GetChild(0).Find("btn_Inquire").GetComponent<Button>();
        _btn_DirectlyReset = _Panels[1].GetChild(0).Find("btn_Reset").GetComponent<Button>();
        _txt_DirectNumber = _Panels[1].GetChild(0).Find("txt_DirectNumber").GetComponent<Text>();
        _txt_DirectAgent = _Panels[1].GetChild(0).Find("txt_DirectAgent").GetComponent<Text>();
        _txt_TotalWaterToday = _Panels[1].GetChild(0).Find("txt_Num").GetComponent<Text>();

        _PerformanceTableGroup = _Panels[2].GetChild(0).Find("Mask").GetChild(0);
        _txt_PerformanceNoData = _Panels[2].GetChild(0).Find("txt_NoData").GetComponent<Text>();
        _ipf_PerformancePlayerID = _Panels[2].GetChild(0).Find("img_TableFooter").GetChild(0).GetComponent<InputField>();
        _btn_PerformanceInquire = _Panels[2].GetChild(0).Find("img_TableFooter").GetChild(1).GetComponent<Button>();
    }

    private void Init_Listener()
    {
        _btn_Return.onClick.AddListener(ReturnMainMenu);

        _btn_AllTurnOff.onClick.AddListener(() => { _ipf_TakeOut.text = _txt_TakeOutCanReceive.text; });
        _btn_TakeOutClose.onClick.AddListener(() => { _panel_TakeOutCommission.gameObject.SetActive(false); });
        _btn_TakeOut.onClick.AddListener(TakOutCommission);

        _btn_QRCodeClose.onClick.AddListener(() => { _panel_QRCode.gameObject.SetActive(false); });

        _btn_ReceiveLogClose.onClick.AddListener(() => { _panel_ReceiveLog.gameObject.SetActive(false); });

        _btn_RebateTableClose.onClick.AddListener(() => { _panel_RebateTable.gameObject.SetActive(false); });

        for (int i = 0; i < _Buttons.Count; i++)
        {
            int index = i;

            _Buttons[i].onClick.AddListener(() => { ChangePanel(index); });
        }

        _btn_ReceiveCommission.onClick.AddListener(OpenTakeOutPanel);
        _btn_ReceiveLog.onClick.AddListener(GetCommissionRecord);
        _btn_RebateTable.onClick.AddListener(OpenRebateTable);
        _btn_QRCode.onClick.AddListener(OpenQRCodePanel);
        _btn_CopyUrl.onClick.AddListener(() => GameStatic.CopyString(_txt_PromoteUrl.text));

        _btn_DirectlyInquire.onClick.AddListener(QueryDirectReports);
        _btn_DirectlyReset.onClick.AddListener(() => { _ipf_DirectlyPlayerID.text = "";
            for (int i = 0; i < _DirectlyTableGroup.childCount; i++)
            {
                Destroy(_DirectlyTableGroup.GetChild(i).gameObject);
            }
        });

        _btn_PerformanceInquire.onClick.AddListener(PerformanceInquire);

        _btn_ContactGM.onClick.AddListener(() => GameStatic.OnChangePanel(Panel.CustomerService));
    }


    private void Init_State()
    {

        LoadingManager.LoadingStart(this.GetType().ToString());
        LoadingManager.LoadingComplete(this.GetType().ToString());
    }

    /// <summary>
    /// 开启取出佣金面板
    /// </summary>
    private void OpenTakeOutPanel()
    {
        _panel_TakeOutCommission.gameObject.SetActive(true);
        _ipf_TakeOut.text = string.Empty;
        _txt_TakeOutCanReceive.text = _txt_TakeOutCanReceive.text;
        _txt_TakeOutWalletMoney.text = _txt_TakeOutWalletMoney.text;
    }

    /// <summary>
    /// 面板切换
    /// </summary>
    /// <param name="index"></param>
    private void ChangePanel(int index)
    {
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
        UpdateUI(index);
    }

    /// <summary>
    /// 更新对应索引面板的UI
    /// </summary>
    /// <param name="index"></param>
    private void UpdateUI(int index)
    {
        switch (index)
        {
            case 0:
                GetPromotionInfo();
                break;
            case 1:
                OpenDirectQueryPanel();
                break;
            case 2:
                OpenPerformanceInquiryPanel();
                break;
            case 3:
                OpenPromotionTutorialPanel();
                break;
            default:
                Debug.Log("undefined index");
                break;
        }
    }


    /// <summary>
    /// 开启直属查询面板时进行值的初始化
    /// </summary>
    private void OpenDirectQueryPanel()
    {
        HttpManager._Instance.StartPost(@"promote/queryDirectly", null, (unityWebRequest) =>
          {
              if (unityWebRequest == null)
              {
                  TipsManager._Instance.OpenReConnectTipsPanel(OpenDirectQueryPanel);
                  return;
              }

              JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
              if (jd["code"].ToString() == "1")
              {
                  for (int i = 0; i < _DirectlyTableGroup.childCount; i++)
                  {
                      Destroy(_DirectlyTableGroup.GetChild(i).gameObject);
                  }

                  _txt_DirectNumber.text = jd["result"]["teamPeopleNumber"].ToString();
                  _txt_DirectAgent.text = jd["result"]["directlyProxy"].ToString();
                  _txt_TotalWaterToday.text = jd["result"]["todayMoney"].ToString();
              }
              else
              {
                  TipsManager._Instance.OpenWarningBox(jd["msg"].ToString());
              }
          });
    }

    private void OpenPerformanceInquiryPanel()
    {
        for (int i = 0; i < _PerformanceTableGroup.childCount; i++)
        {
            Destroy(_PerformanceTableGroup.GetChild(i).gameObject);
        }

        _ipf_PerformancePlayerID.text = string.Empty;
    }

    private void OpenPromotionTutorialPanel()
    {

    }

    /// <summary>
    /// 直属查询
    /// </summary>
    private void QueryDirectReports()
    {
        Dictionary<string, string> form = new Dictionary<string, string>();

        form.Add("mId", _ipf_DirectlyPlayerID.text);

        HttpManager._Instance.StartPost(@"promote/queryDirectReportsInfo", form, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(QueryDirectReports);
                return;
            }

            JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
            if (jd["code"].ToString() == "1")
            {
                for (int i = 0; i < _DirectlyTableGroup.childCount; i++)
                {
                    Destroy(_DirectlyTableGroup.GetChild(i).gameObject);
                }

                if (jd["result"] == null || jd["result"].Count == 0)
                {
                    _txt_DirectlyNoData.gameObject.SetActive(true);
                }
                else
                {
                    _txt_DirectlyNoData.gameObject.SetActive(false);

                    for (int i = 0; i < jd["result"].Count; i++)
                    {
                        JsonData jsonData = jd["result"][i];
                        GameObject go = Instantiate(InitialResourcesManager.PromotionTableElement, _DirectlyTableGroup, false);
                        go.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = CheckNull(jsonData["Id"]);
                        go.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = CheckNull(jsonData["name"]);
                        go.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = CheckNull(jsonData["todayMoney"]);
                        go.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = CheckNull(jsonData["grossAmount"]);
                        go.transform.GetChild(4).GetChild(0).GetComponent<Text>().text = CheckNull(jsonData["teamPeopleNumber"]);
                        go.transform.GetChild(5).GetChild(0).GetComponent<Text>().text = CheckNull(jsonData["directlyProxy"]);
                        go.transform.GetChild(6).GetChild(0).GetComponent<Text>().text = CheckNull(jsonData["操作"]);
                    }
                }
            }
            else
            {
                TipsManager._Instance.OpenWarningBox(jd["msg"].ToString());
            }
        });
    }

    /// <summary>
    /// 生成二维码 
    /// </summary>
    /// <param name="url">需要生产二维码的字符串</param>
    /// <param name="rimg">目标RawImage</param>
    /// <returns></returns>       
    private void SetQRCode(string url, RawImage rimg)
    {
        Texture2D encoded = new Texture2D(256, 256);
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = encoded.height,
                Width = encoded.width
            }
        };
        Color32[] color32 = writer.Write(url);

        encoded.SetPixels32(color32);
        encoded.Apply();
        //生成的二维码图片附给RawImage
        rimg.texture = encoded;
    }

    /// <summary>
    /// 业绩查询
    /// </summary>
    private void PerformanceInquire()
    {
        Dictionary<string, string> form = new Dictionary<string, string>();
        form.Add("mId", _ipf_PerformancePlayerID.text);

        HttpManager._Instance.StartPost(@"promote/queryStaffsInfo", form, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(PerformanceInquire);
                return;
            }

            JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
            if (jd["code"].ToString() == "1")
            {
                for (int i = 0; i < _PerformanceTableGroup.childCount; i++)
                {
                    Destroy(_PerformanceTableGroup.GetChild(i).gameObject);
                }

                if (jd["result"] != null && jd["result"].Count != 0)
                {
                    _txt_PerformanceNoData.gameObject.SetActive(false);
                    for (int i = 0; i < jd["result"].Count; i++)
                    {
                        JsonData jsonData = jd["result"][i];

                        GameObject go = Instantiate(InitialResourcesManager.PromotionTableElement, _PerformanceTableGroup, false);
                        go.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = GameStatic.DateFormat(CheckNull(jsonData["date"]));
                        go.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = CheckNull(jsonData["directlyIncrease"]);
                        go.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = CheckNull(jsonData["oneselfMoney"]);
                        go.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = CheckNull(jsonData["teamIncrease"]);
                        go.transform.GetChild(4).GetChild(0).GetComponent<Text>().text = CheckNull(jsonData["teamMoney"]);
                        go.transform.GetChild(5).GetChild(0).GetComponent<Text>().text = CheckNull(jsonData["getTheMoney"]);
                        go.transform.GetChild(6).GetChild(0).GetComponent<Text>().text = CheckNull(jsonData["operation"]);
                    }

                    _ipf_PerformancePlayerID.text = string.Empty;
                }
                else
                {
                    _txt_PerformanceNoData.gameObject.SetActive(true);
                }
            }
            else
            {
                TipsManager._Instance.OpenWarningBox(jd["msg"].ToString());
            }
        });
    }

    /// <summary>
    /// 重置
    /// </summary>
    private void Reset()
    {
        for (int i = 0; i < _Buttons.Count; i++)
        {
            _Buttons[i].transform.GetChild(0).gameObject.SetActive(false);
        }

        _panel_QRCode.gameObject.SetActive(false);
        _panel_TakeOutCommission.gameObject.SetActive(false);
        _panel_ReceiveLog.gameObject.SetActive(false);
        _panel_RebateTable.gameObject.SetActive(false);
    }

    /// <summary>
    /// 返回主菜单
    /// </summary>
    private void ReturnMainMenu()
    {
        GameStatic.OnChangePanel?.Invoke(Panel.MainMenu);
    }

    /// <summary>
    /// 获取推广信息表
    /// </summary>
    private void GetPromotionInfo()
    {
        if (!LocalFileManager._Instance._GameData._IsLogin)
        {
            return;
        }

        HttpManager._Instance.StartPost(@"promote/myPromotion", null, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(GetPromotionInfo);
                return;
            }

            Debug.Log(unityWebRequest.downloadHandler.text);
            JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
            if (jd["code"].ToString() == "1")
            {
                _txt_MyID.text = CheckNull(jd["result"]["myId"]);
                _txt_RecommendedID.text = CheckNull(jd["result"]["recommendedId"]);
                _txt_TuanDuiRenShuXinZeng.text = CheckNull(jd["result"]["increaseTeamPeople"]);
                _txt_JinRiTuanDuiYeJi.text = CheckNull(jd["result"]["teamMoney"]);
                _txt_ZhiShuRenShuXinZeng.text = CheckNull(jd["result"]["increaseDirectlyPeople"]);
                _txt_JinRiZiYingYeJi.text = CheckNull(jd["result"]["oneselfMoney"]);
                _txt_JinRiYongJinYuGu.text = CheckNull(jd["result"]["forecastCommissions"]);
                _txt_ZuoRiYongJinJieSuan.text = CheckNull(jd["result"]["yesterdayCommissions"]);
                _txt_HistoricalCommission.text = CheckNull(jd["result"]["historyCommissions"]);
                _txt_CanReceiveCommission.text = CheckNull(jd["result"]["extractedCommissions"]);
                _txt_TakeOutCanReceive.text = CheckNull(jd["result"]["extractedCommissions"]);
                _txt_PromoteUrl.text = CheckNull(jd["result"]["promoteUrl"]);
                SetQRCode(CheckNull(jd["result"]["promoteUrl"]), _rimg_OriginQRCode);
            }
            else
            {
                TipsManager._Instance.OpenWarningBox("获取推广信息失败");
            }
        });

        HttpManager._Instance.StartGetBanance((num) => { _txt_TakeOutWalletMoney.text = num; });

    }

    /// <summary>
    /// 检测JsonData对象是否为空
    /// </summary>
    /// <param name="checkTarget"></param>
    /// <returns></returns>
    private string CheckNull(JsonData checkTarget)
    {
        string str = "-";
        if (checkTarget != null)
        {
            str = checkTarget.ToString();
        }
        return str;
    }

    /// <summary>
    /// 开启佣金领取记录面板
    /// </summary>
    private void GetCommissionRecord()
    {
        _panel_ReceiveLog.gameObject.SetActive(true);

        HttpManager._Instance.StartPost(@"promote/getCommissionRecord", null, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(GetCommissionRecord);
                return;
            }

            Debug.Log(unityWebRequest.downloadHandler.text);
            JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
            if (jd["code"].ToString() == "1")
            {
                   JsonData result = jd["result"];
                for (int i = 0; i < result.Count; i++)
                {
                    string money = result[result.Count - 1 - i]["money"].ToString();
                    string date = result[result.Count - 1 - i]["Date"].ToString();

                    GameObject go;
                    if (i % 2 == 0)
                    {
                        go = Instantiate(InitialResourcesManager.TwoColorTableElement1, _ReceiveLogGroup, false);
                    }
                    else
                    {
                        go = Instantiate(InitialResourcesManager.TwoColorTableElement2, _ReceiveLogGroup, false);
                    }

                    go.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = date;
                    go.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = money;

                    for (int j = 2; j < go.transform.childCount; j++)
                    {
                        go.transform.GetChild(j).gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                TipsManager._Instance.OpenWarningBox(jd["msg"].ToString());
            }
        });
    }

    /// <summary>
    /// 开启二维码页面
    /// </summary>
    private void OpenQRCodePanel()
    {
        _panel_QRCode.gameObject.SetActive(true);
        SetQRCode(_txt_PromoteUrl.text, _rimg_QRCode);
    }

    /// <summary>
    /// 取出佣金
    /// </summary>
    private void TakOutCommission()
    {
        if (_ipf_TakeOut.text == string.Empty)
        {
            TipsManager._Instance.OpenWarningBox("数值不能为空");
            return;
        }

        double moneyNum = double.Parse(_ipf_TakeOut.text);

        Dictionary<string, string> form = new Dictionary<string, string>();

        form.Add("commissions", moneyNum.ToString());

        HttpManager._Instance.StartPost(@"promote/takeCommission", form, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(TakOutCommission);
                return;
            }

            JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
            if (jd["code"].ToString() == "1")
            {
                GetPromotionInfo();
            }
            else
            {
                TipsManager._Instance.OpenWarningBox(jd["msg"].ToString());
            }
        });
    }

    /// <summary>
    /// 开启返佣金额表
    /// </summary>
    private void OpenRebateTable()
    {
        _panel_RebateTable.gameObject.SetActive(true);
    }

    /// <summary>
    /// 初始化返佣金额表
    /// </summary>
    private void InitRebateTable()
    {
        List<Button> _RebateTableButtons = new List<Button>();
 
        List<Transform> _RebateTablePanels = new List<Transform>();

        Transform buttons = _panel_RebateTable.Find("Window").Find("ButtonsMask").Find("Buttons");
        Transform panels = _panel_RebateTable.Find("Window").Find("Panels");

        HttpManager._Instance.StartPost(@"promote/proxyLevel/queryGameProxyLevelAll", null, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenWarningBox("获取返佣金额表失败");
                return;
            }
            
            JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
            if (jd["code"].ToString() == "1")
            {
                for (int i = 0; i < buttons.childCount; i++)
                {
                    Destroy(buttons.GetChild(i).gameObject);
                    Destroy(panels.GetChild(i).gameObject);

                }
                for (int i = 0; i < jd["result"].Count; i++)
                {
                    _RebateTableButtons.Add(Instantiate(InitialResourcesManager.btn_RebateTable, buttons, false).GetComponent<Button>());

                    _RebateTableButtons[i].transform.Find("txt_Name").GetComponent<Text>().text=jd["result"][i]["name"].ToString();

                    _RebateTablePanels.Add(Instantiate(InitialResourcesManager.RebateTableMask, panels, false).transform);

                    JsonData list = jd["result"][i]["list"];
                    for (int j = 0; j < list.Count; j++)
                    {
                        string level = list[j]["proxyLevelName"].ToString();
                        string teamPerformance = list[j]["achievememt"].ToString() + "+";
                        string Rebate = "每万元返佣" + list[j]["returnAmount"].ToString() + "元";

                        GameObject tableElement;
                        if (j % 2 == 0)
                        {
                            tableElement = Instantiate(InitialResourcesManager.TwoColorTableElement1, _RebateTablePanels[i].GetChild(0), false);
                        }
                        else
                        {
                            tableElement = Instantiate(InitialResourcesManager.TwoColorTableElement2, _RebateTablePanels[i].GetChild(0), false);
                        }

                        tableElement.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = level;
                        tableElement.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = teamPerformance;
                        tableElement.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = Rebate;

                        for (int k = 3; k < 10; k++)
                        {
                            tableElement.transform.GetChild(k).gameObject.SetActive(false);
                        }
                    }
                }

                for (int i = 0; i < _RebateTableButtons.Count; i++)
                {
                    int index = i;
                    _RebateTableButtons[i].onClick.AddListener(() =>
                    {
                        for (int j = 0; j < _RebateTableButtons.Count; j++)
                        {
                            if (j == index)
                            {
                                _RebateTableButtons[j].transform.GetChild(0).gameObject.SetActive(true);
                                _RebateTableButtons[j].transform.GetChild(1).GetComponent<Text>().color = Color.black;
                                _RebateTablePanels[j].gameObject.SetActive(true);
                            }
                            else
                            {
                                _RebateTableButtons[j].transform.GetChild(0).gameObject.SetActive(false);
                                _RebateTableButtons[j].transform.GetChild(1).GetComponent<Text>().color = GameStatic.GoldColor;
                                _RebateTablePanels[j].gameObject.SetActive(false);
                            }
                        }
                    });

                }

                _RebateTableButtons[0].onClick?.Invoke();
            }
            else
            {
                TipsManager._Instance.OpenWarningBox(WebSocketSharp.Net.HttpUtility.UrlDecode(jd["msg"].ToString()));
            }
        });
    }
}