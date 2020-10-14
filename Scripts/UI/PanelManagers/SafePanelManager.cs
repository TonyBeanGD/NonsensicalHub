using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SafePanelManager : MonoBehaviour
{
    private Transform _panel_Safe;
    private Transform _Buttons;
    private Transform _Panels;
    private InputField _ipf_TransferInMoney;
    private InputField _ipf_TransferOutMoney;
    private Button _btn_TransferInConfirm;
    private Button _btn_TransferInClear;
    private Button _btn_TransferOutConfirm;
    private Button _btn_TransferOutClear;
    private Button _btn_Return;
    private Text _txt_TransferOutSafeMoney;
    private Text _txt_TransferOutWalletMoney;
    private Text _txt_TransferInSafeMoney;
    private Text _txt_TransferInWalletMoney;

    private Transform _DetailPages;
    private InputField _ipf_PageNum;
    private Button _btn_LastPage;
    private Button _btn_NextPage;

    private int _CrtPage;
    private int _PageMax;

    private void ChangePanel(Panel panel)
    {
        if (panel == Panel.Safe)
        {
            ResetSafePanel();


            _panel_Safe.gameObject.SetActive(true);
            AudioSourceManager._Instance.PlayCharacterSpeech(InitialResourcesManager.aud_Safe);
        }
        else
        {

            if (_panel_Safe.gameObject.activeSelf == true)
            {
                _panel_Safe.gameObject.SetActive(false);
            }
        }
    }

    void Awake()
    {
        _CrtPage = 0;

        GameStatic.OnChangePanel += ChangePanel;
        GameStatic.OnMainPanelInit += Init_State;
        GameStatic.OnSetSafeMoneyText += SetMoneyText;

        Init_Variable();
        Init_Listener();
    }

    void Update()
    {
        if (Time.frameCount % 3 == 0)
        {
            SlidePages();
        }
    }

    private void Init_Variable()
    {
        _panel_Safe = transform;
        _Buttons = _panel_Safe.Find("Buttons");
        _Panels = _panel_Safe.Find("Panels");
        _btn_Return = _panel_Safe.Find("img_Background").Find("btn_Return").GetComponent<Button>();
        _txt_TransferInSafeMoney = _Panels.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        _txt_TransferInWalletMoney = _Panels.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
        _ipf_TransferInMoney = _Panels.GetChild(0).GetChild(2).GetChild(0).GetComponent<InputField>();
        _btn_TransferInConfirm = _Panels.GetChild(0).GetChild(2).GetChild(1).GetComponent<Button>();
        _btn_TransferInClear = _Panels.GetChild(0).GetChild(2).GetChild(2).GetComponent<Button>();
        _txt_TransferOutSafeMoney = _Panels.GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>();
        _txt_TransferOutWalletMoney = _Panels.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>();
        _ipf_TransferOutMoney = _Panels.GetChild(1).GetChild(2).GetChild(0).GetComponent<InputField>();
        _btn_TransferOutConfirm = _Panels.GetChild(1).GetChild(2).GetChild(1).GetComponent<Button>();
        _btn_TransferOutClear = _Panels.GetChild(1).GetChild(2).GetChild(2).GetComponent<Button>();
        _DetailPages = _Panels.GetChild(2).GetChild(0).GetChild(0).GetChild(0);
        _btn_LastPage = _Panels.GetChild(2).GetChild(0).GetChild(1).GetChild(0).GetComponent<Button>();
        _btn_NextPage = _Panels.GetChild(2).GetChild(0).GetChild(1).GetChild(1).GetComponent<Button>();
        _ipf_PageNum = _Panels.GetChild(2).GetChild(0).GetChild(1).GetChild(2).GetComponent<InputField>();
    }

    private void Init_Listener()
    {
        for (int i = 0; i < _Buttons.childCount; i++)
        {
            int index = i;
            _Buttons.GetChild(index).GetComponent<Button>().onClick.AddListener(() => { ChangePanel(index); });
        }
        _btn_Return.onClick.AddListener(ReturnMainMenu);

        _btn_TransferInConfirm.onClick.AddListener(TransferIn);
        _btn_TransferInClear.onClick.AddListener(() => { _ipf_TransferInMoney.text = string.Empty; });
        _btn_TransferOutConfirm.onClick.AddListener(TransferOut);
        _btn_TransferOutClear.onClick.AddListener(() => { _ipf_TransferOutMoney.text = string.Empty; });
        _btn_LastPage.onClick.AddListener(() => { ChangePage(-1); });
        _btn_NextPage.onClick.AddListener(() => { ChangePage(1); });
        _ipf_PageNum.onEndEdit.AddListener((str) => { EditPageNumCheck(str); });

        _ipf_TransferOutMoney.onEndEdit.AddListener((str) => { _ipf_TransferOutMoney.text = CheckLength(str); });
        _ipf_TransferInMoney.onEndEdit.AddListener((str) => { _ipf_TransferInMoney.text = CheckLength(str); });
    }

    /// <summary>
    /// 检测字符串小数长度，只保留两位小数（去尾法）
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private string CheckLength(string str)
    {
        if (str.Equals(string.Empty))
        {
            return str;
        }
        double d = 0;
        if (double.TryParse(str, out d))
        {
            long i = (long)(d * 100);
            d = (double)i / 100.0;
        }

        return d.ToString();
    }

    private void Init_State()
    {
        LoadingManager.LoadingStart(this.GetType().ToString());
        ChangePanel(0);
        ResetSafePanel();
        LoadingManager.LoadingComplete(this.GetType().ToString());
    }

    /// <summary>
    /// 切换面板
    /// </summary>
    /// <param name="index"></param>
    private void ChangePanel(int index)
    {
        for (int i = 0; i < _Buttons.childCount; i++)
        {
            if (i == index)
            {
                _Buttons.GetChild(i).GetChild(0).gameObject.SetActive(true);
                _Panels.GetChild(i).gameObject.SetActive(true);

                if (i == 2)
                {
                    RefreshDetail();
                }
            }
            else
            {
                _Buttons.GetChild(i).GetChild(0).gameObject.SetActive(false);
                _Panels.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 保险箱钱数写入ui
    /// </summary>
    /// <param name="safeMoney"></param>
    /// <param name="walletMoney"></param>
    private void SetMoneyText(string safeMoney, string walletMoney)
    {
        _ipf_TransferInMoney.text = string.Empty;
        _ipf_TransferOutMoney.text = string.Empty;
        _txt_TransferInSafeMoney.text = safeMoney;
        _txt_TransferOutSafeMoney.text = safeMoney;
        _txt_TransferInWalletMoney.text = walletMoney;
        _txt_TransferOutWalletMoney.text = walletMoney;
    }

    /// <summary>
    /// 保险箱转入
    /// </summary>
    private void TransferIn()
    {
        if (_ipf_TransferInMoney.text.Equals(string.Empty))
        {
            return;

        }
        double num;

        if (double.TryParse(_ipf_TransferInMoney.text, out num) == false)
        {
            Debug.LogWarning("数字转换失败" + _ipf_TransferInMoney.text);
            return;

        }

        Dictionary<string, string> form = new Dictionary<string, string>();
        form.Add("enterMoney", _ipf_TransferInMoney.text);

        HttpManager._Instance.StartPost(@"member/center/safeInput", form, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenCustomTimeoutTipsPanel("重新转入", TransferIn);
                return;
            }

            JsonData jsonData = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
            if (jsonData["code"].ToString() == "1")
            {
                TipsManager._Instance.OpenSuccessBox("转入成功");
                SetMoneyText(jsonData["result"]["safeMoney"].ToString(), jsonData["result"]["walletMoney"].ToString());
            }
            else
            {
                TipsManager._Instance.OpenWarningBox(jsonData["msg"].ToString());
            }
        });
    }

    /// <summary>
    /// 保险箱转出
    /// </summary>
    private void TransferOut()
    {
        if (_ipf_TransferOutMoney.text.Equals(string.Empty))
        {
            return;
        }

        double num;
        if (double.TryParse(_ipf_TransferOutMoney.text, out num) == false)
        {
            Debug.LogWarning("数字转换失败" + _ipf_TransferOutMoney.text);
            return;
        }

        Dictionary<string, string> form = new Dictionary<string, string>();
        form.Add("outMoney", _ipf_TransferOutMoney.text);

        HttpManager._Instance.StartPost(@"member/center/safeOut", form, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenCustomTimeoutTipsPanel("重新转出", TransferOut);
                return;
            }

            JsonData jsonData = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
            if (jsonData["code"].ToString() == "1")
            {
                TipsManager._Instance.OpenSuccessBox("转出成功");
                SetMoneyText(jsonData["result"]["safeMoney"].ToString(), jsonData["result"]["walletMoney"].ToString());
            }
            else
            {
                TipsManager._Instance.OpenWarningBox(jsonData["msg"].ToString());
            }
        });

    }

    /// <summary>
    /// 页面页码输入检测
    /// </summary>
    /// <param name="str"></param>
    private void EditPageNumCheck(string str)
    {
        int offset = 0;
        if (!int.TryParse(str, out offset))
        {
            Debug.Log("请输入数字");
            offset = _CrtPage + 1;
        }
        ChangePage(offset - _CrtPage - 1);
    }

    /// <summary>
    /// 刷新详情
    /// </summary>
    private void RefreshDetail()
    {
        HttpManager._Instance.StartPost(@"member/center/safeDetail", null, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenCustomTimeoutTipsPanel("刷新", RefreshDetail);
                return;
            }

            JsonData jsonData = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

            if (jsonData["code"].ToString() == "1")
            {
                for (int i = 0; i < _DetailPages.childCount; i++)
                {
                    Destroy(_DetailPages.GetChild(i).gameObject);
                }

                _CrtPage = 0;
                _PageMax = jsonData["result"].Count / 7 + (jsonData["result"].Count % 7 == 0 ? 0 : 1);
                GameObject crtPageGO = null;
                for (int i = 0; i < jsonData["result"].Count; i++)
                {
                    if (i % 7 == 0)
                    {
                        crtPageGO = Instantiate(InitialResourcesManager.SafeDetailPage, _DetailPages, false);

                        Rect maskRect = _DetailPages.parent.GetComponent<RectTransform>().rect;
                        crtPageGO.GetComponent<RectTransform>().sizeDelta = new Vector2(maskRect.width, 0);
                    }
                    GameObject crtElement = Instantiate(InitialResourcesManager.SafeDetailTableContent, crtPageGO.transform, false);
                    crtElement.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = GameStatic.DateFormat(jsonData["result"][jsonData["result"].Count - 1 - i]["create_date"].ToString());
                    crtElement.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = jsonData["result"][jsonData["result"].Count - 1 - i]["memo"].ToString();
                    crtElement.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = jsonData["result"][jsonData["result"].Count - 1 - i]["money"].ToString();
                }
                ChangePage(0);
            }
            else
            {
                TipsManager._Instance.OpenWarningBox(jsonData["msg"].ToString());
            }
        });
    }

    /// <summary>
    /// 切换保险箱明细页面
    /// </summary>
    /// <param name="offset"></param>
    private void ChangePage(int offset)
    {
        _CrtPage += offset;
        if (_CrtPage <= 0)
        {
            _CrtPage = 0;
        }
        if (_CrtPage >= _PageMax)
        {
            _CrtPage = _PageMax - 1;
        }
        _ipf_PageNum.text = (_CrtPage + 1) + "/" + _PageMax;
    }

    private void SlidePages()
    {
        RectTransform rectTransform = _DetailPages.GetComponent<RectTransform>();
        if (_PageMax == 0)
        {
            return;
        }
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, new Vector2(-1.0f * (rectTransform.rect.width / _PageMax) * _CrtPage, 0), 0.8f);
    }

    /// <summary>
    /// 返回主菜单
    /// </summary>
    private void ReturnMainMenu()
    {
        ResetSafePanel();
        GameStatic.OnChangePanel?.Invoke(Panel.MainMenu);
    }

    /// <summary>
    /// 充值保险箱面板
    /// </summary>
    private void ResetSafePanel()
    {
        ChangePanel(0);
    }
}
