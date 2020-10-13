using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class WashCodePanelManager : MonoBehaviour
{
    private Transform _panel_WashCode;
    private List<Button> _Buttons;
    private Button _btn_Return;
    private Button _btn_WashCodeLog;
    private Button _btn_WashCodeProportion;

    private Transform _TableGroup;
    private Button _btn_ManualWashing;
    private Text _txt_TotalGameBettingNum;
    private Text _txt_LastWashingTimeNum;
    private Text _txt_WashingAmountnum;

    private Transform _panel_WashCodeLog;
    private Button _btn_CloseWashCodeLog;
    private Text _txt_NoLogWashCodeLog;
    private Transform _TableGroupWashCodeLog;

    private Transform _panel_WashCodeProportion;
    private Button _btn_CloseWashCodeProportion;
    private Text _txt_NoLogWashCodeProportion;
    private Transform _TableGroupWashCodeProportion;

    private int _CreatePanelIndex;

    private void ChangePanel(Panel panel)
    {
        if (panel == Panel.WashCode)
        {
            ChangePanel(0);
            _panel_WashCode.gameObject.SetActive(true);
            AudioSourceManager._Instance.PlayCharacterSpeech(InitialResourcesManager.aud_WashCode);
        }
        else
        {
            if (_panel_WashCode.gameObject.activeSelf == true)
            {
                _panel_WashCode.gameObject.SetActive(false);
            }
        }
    }

    void Awake()
    {
        GameStatic.OnChangePanel += ChangePanel;
        GameStatic.OnMainPanelInit += Init_State;
        GameStatic.OnLogin += InitWashCodeList;
        GameStatic.OnLogout += ()=>
        {
            Transform buttonsParent = _panel_WashCode.Find("Buttons");
            for (int i = 0; i < buttonsParent.childCount; i++)
            {
                Destroy(buttonsParent.GetChild(i)) ;
            }
        };
        _CreatePanelIndex = 1;
        Init_Variable();
        Init_Listener();
    }

    private void Init_Variable()
    {
        _Buttons = new List<Button>();

        _panel_WashCode = transform;

        _btn_Return = _panel_WashCode.Find("btn_Return").GetComponent<Button>();
        _btn_WashCodeLog = _panel_WashCode.Find("btn_XiMaJiLu").GetComponent<Button>();
        _btn_WashCodeProportion = _panel_WashCode.Find("TableBackground").Find("TableHeader").Find("btn_WashingRatio").GetComponent<Button>();

        _TableGroup = _panel_WashCode.Find("TableBackground").Find("TableBody").Find("Group");
        _btn_ManualWashing = _panel_WashCode.Find("TableBackground").Find("TableFooter").Find("btn_ManualWashing").GetComponent<Button>();
        _txt_TotalGameBettingNum = _panel_WashCode.Find("TableBackground").Find("TableFooter").Find("txt_TotalGameBettingNum").GetComponent<Text>();
        _txt_LastWashingTimeNum = _panel_WashCode.Find("TableBackground").Find("TableFooter").Find("txt_LastWashingTimeNum").GetComponent<Text>();
        _txt_WashingAmountnum = _panel_WashCode.Find("TableBackground").Find("TableFooter").Find("txt_WashingAmountnum").GetComponent<Text>();

        _panel_WashCodeLog = _panel_WashCode.Find("panel_WashCodeLog");
        _btn_CloseWashCodeLog = _panel_WashCodeLog.GetChild(0).GetChild(0).GetComponent<Button>();
        _txt_NoLogWashCodeLog = _panel_WashCodeLog.GetChild(0).GetChild(2).GetComponent<Text>();
        _TableGroupWashCodeLog = _panel_WashCodeLog.GetChild(0).GetChild(1).GetChild(0);

        _panel_WashCodeProportion = _panel_WashCode.Find("panel_WashCodeProportion");
        _btn_CloseWashCodeProportion = _panel_WashCodeProportion.GetChild(0).GetChild(0).GetComponent<Button>();
        _txt_NoLogWashCodeProportion = _panel_WashCodeProportion.GetChild(0).GetChild(2).GetComponent<Text>();
        _TableGroupWashCodeProportion = _panel_WashCodeProportion.GetChild(0).GetChild(1).GetChild(0);
    }

    private void Init_Listener()
    {
        _btn_Return.onClick.AddListener(ReturnMainMenu);

        _btn_ManualWashing.onClick.AddListener(ManualWashCode);

        _btn_WashCodeLog.onClick.AddListener(GetHistoryRecord);
        _btn_WashCodeProportion.onClick.AddListener(GetWashCodeRatio);

        _btn_CloseWashCodeLog.onClick.AddListener(() => { _panel_WashCodeLog.gameObject.SetActive(false); });
        _btn_CloseWashCodeProportion.onClick.AddListener(() => { _panel_WashCodeProportion.gameObject.SetActive(false); });
    }

    private void Init_State()
    {
        LoadingManager.LoadingStart(this.GetType().ToString());
        Reset();
        LoadingManager.LoadingComplete(this.GetType().ToString());
    }

    private void ChangePanel(int index)
    {
        _CreatePanelIndex = index + 1;
        for (int i = 0; i < _Buttons.Count; i++)
        {
            if (i == index)
            {
                _Buttons[i].transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                _Buttons[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        UpdateWashCodeInfo(index);
    }

    private void Reset()
    {
        _panel_WashCodeLog.gameObject.SetActive(false);
        _panel_WashCodeProportion.gameObject.SetActive(false);
    }

    private void ReturnMainMenu()
    {
        GameStatic.OnChangePanel?.Invoke(Panel.MainMenu);
        Reset();
    }

    private void InitWashCodeList()
    {
        HttpManager._Instance.StartPost(@"member/center/getWashCodeInfo", null, (unityWebRequest) =>
          {
              if (unityWebRequest == null)
              {
                  TipsManager._Instance.OpenReConnectTipsPanel(InitWashCodeList);
                  return;
              }

              JsonData jsonData = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

              if (jsonData["code"].ToString() == "1")
              {
                  Transform buttonsParent = _panel_WashCode.Find("Buttons");
                  JsonData result = jsonData["result"]["washCodeInfo"];

                  for (int i = 0; i < result.Count; i++)
                  {
                      GameObject crtButton = Instantiate(InitialResourcesManager.WashCodeButton, buttonsParent, false);

                      DynamicResourceManager._Instance.StartSetTexture(crtButton.GetComponent<Image>(), result[i]["icon"].ToString(),false);
                      DynamicResourceManager._Instance.StartSetTexture(crtButton.transform.GetChild(0).GetComponent<Image>(), result[i]["brightIcon"].ToString(), false);

                      int index = i;
                      crtButton.GetComponent<Button>().onClick.AddListener(() => { ChangePanel(index); });

                      _Buttons.Add(crtButton.GetComponent<Button>());
                  }
              }
              else
              {
                  Debug.LogWarning(jsonData["msg"].ToString());
              }
          });
    }

    private void UpdateWashCodeInfo(int index)
    {
        HttpManager._Instance.StartPost(@"member/center/getWashCodeInfo", null, (unityWebRequest) =>
          {
              if (unityWebRequest == null)
              {
                  TipsManager._Instance.OpenReConnectTipsPanel(() => { UpdateWashCodeInfo(index); });
                  return;
              }

              JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

              if (jd["code"].ToString() == "1")
              {
                  JsonData result = jd["result"]["washCodeInfo"][index]["list"];

                  int i = 0;
                  bool one = true;
                  int max = _TableGroup.childCount > result.Count ? _TableGroup.childCount : result.Count;
                  for (; i < max; i++, one = !one)
                  {
                      if (i < result.Count)
                      {
                          GameObject go;
                          if (i < _TableGroup.childCount)
                          {
                              go = _TableGroup.GetChild(i).gameObject;
                          }
                          else
                          {
                              go = one ? Instantiate(InitialResourcesManager.WashCodeTableContent_1, _TableGroup, false) : Instantiate( InitialResourcesManager.WashCodeTableContent_2, _TableGroup, false);
                          }

                          go.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = result[result.Count - 1 - i]["PNameOrGName"].ToString();
                          go.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = result[result.Count - 1 - i]["fee"].ToString();
                          go.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = result[result.Count - 1 - i]["ratio"].ToString();
                          go.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = result[result.Count - 1 - i]["backwater"].ToString();
                      }
                      else
                      {
                          Destroy(_TableGroup.GetChild(i).gameObject);
                      }
                  }

                  _txt_TotalGameBettingNum.text = jd["result"]["scatter"]["totalFee"].ToString();
                  _txt_LastWashingTimeNum.text = jd["result"]["scatter"]["lastTime"].ToString();
                  _txt_WashingAmountnum.text = jd["result"]["scatter"]["totalBackwater"].ToString();

              }
              else
              {
                  TipsManager._Instance.OpenWarningBox("获取洗码信息失败");
              }
          });
    }

    private void ManualWashCode()
    {
        HttpManager._Instance.StartPost(@"member/center/getManualWashCode", null, (unityWebRequest) =>
          {
              if (unityWebRequest == null)
              {
                  TipsManager._Instance.OpenReConnectTipsPanel(ManualWashCode);
                  return;
              }

              JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

              if (jd["code"].ToString() == "1")
              {
                  _txt_TotalGameBettingNum.text = jd["result"]["scatter"]["totalFee"].ToString();
                  _txt_LastWashingTimeNum.text = jd["result"]["scatter"]["lastTime"].ToString();
                  _txt_WashingAmountnum.text = jd["result"]["scatter"]["totalBackwater"].ToString();
                  TipsManager._Instance.OpenSuccessBox("洗码成功");
              }
              else
              {
                  TipsManager._Instance.OpenWarningBox(jd["msg"].ToString());
              }
          });
    }

    private void GetHistoryRecord()
    {
        HttpManager._Instance.StartPost(@"member/center/getHistoryRecord", null, (unityWebRequest) =>
        {
              if (unityWebRequest == null)
              {
                  TipsManager._Instance.OpenReConnectTipsPanel(GetHistoryRecord);
                  return;
              }

              JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

              if (jd["code"].ToString() == "1")
              {
                  _panel_WashCodeLog.gameObject.SetActive(true);
                  JsonData result = jd["result"];


                  bool one = true;
                  int max = _TableGroupWashCodeLog.childCount > result.Count ? _TableGroupWashCodeLog.childCount : result.Count;

                  _txt_NoLogWashCodeLog.gameObject.SetActive(max <= 0);


                  for (int i = 0; i < max; i++, one = !one)
                  {
                      if (i < result.Count)
                      {
                          JsonData crtResult = result[i];
                          GameObject go;
                          if (i < _TableGroupWashCodeLog.childCount)
                          {
                              go = _TableGroupWashCodeLog.GetChild(i).gameObject;
                          }
                          else
                          {
                              go = Instantiate(InitialResourcesManager.WashCodeLogTableContent, _TableGroupWashCodeLog, false);
                          }

                          go.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = GameStatic.DateFormat(crtResult["washCodeTime"].ToString());
                          go.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = crtResult["washCodeAmount"].ToString();
                          go.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = crtResult["washCodeMoney"].ToString();
                          go.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = crtResult["details"].ToString();
                      }
                      else
                      {
                          Destroy(_TableGroupWashCodeLog.GetChild(i).gameObject);
                      }
                  }
              }
              else
              {
                  TipsManager._Instance.OpenWarningBox("获取历史记录失败");
              }
        });
    }

    private void GetWashCodeRatio()
    {
        string type = _CreatePanelIndex.ToString();
        Dictionary<string, string> form = new Dictionary<string, string>();

        form.Add("platformType", type);

        HttpManager._Instance.StartPost(@"member/center/getWashCodeRatio", form, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(GetWashCodeRatio);
                return;
            }

            JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
            if (jd["code"].ToString() == "1")
            {
                _panel_WashCodeProportion.gameObject.SetActive(true);
                JsonData result = jd["result"];

                int i = 0;
                bool one = true;
                int max = _TableGroupWashCodeProportion.childCount > result.Count ? _TableGroupWashCodeProportion.childCount : result.Count;

                _txt_NoLogWashCodeProportion.gameObject.SetActive(max <= 0);

                for (; i < max; i++, one = !one)
                {
                    if (i < result.Count)
                    {
                        GameObject go;
                        if (i < _TableGroupWashCodeProportion.childCount)
                        {
                            go = _TableGroupWashCodeProportion.GetChild(i).gameObject;
                        }
                        else
                        {
                            go = Instantiate(InitialResourcesManager.WashCodeProportionTableContent, _TableGroupWashCodeProportion, false);
                        }

                        go.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = result[i]["platformName"].ToString();
                        go.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = result[i]["washCodeRange"].ToString();
                        go.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = result[i]["levelName"].ToString();
                        go.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = result[i]["nowRatio"].ToString();
                        go.transform.GetChild(4).GetChild(0).GetComponent<Text>().text = result[i]["nextRatio"].ToString();
                        go.transform.SetAsFirstSibling();
                    }
                    else
                    {
                        Destroy(_TableGroupWashCodeProportion.GetChild(i).gameObject);
                    }
                }
            }
            else
            {
                TipsManager._Instance.OpenWarningBox("获取洗码比例失败");
            }
        });
    }
}
