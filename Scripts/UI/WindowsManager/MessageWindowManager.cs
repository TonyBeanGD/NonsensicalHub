using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MessageWindowManager : MonoBehaviour
{
    private Transform _window_Message;

    private List<Transform> _Buttons;
    private List<Transform> _Panels;


    private Button _btn_MessageClose;
    private Button _btn_ReadedAll;
    private Button _btn_DeleteAll;

    private Transform _MessageDetailBox;
    private Button _btn_DetailBoxClose;
    private MainMenuPanelManager _panel;


    public void Open()
    {
        Refresh();
        _window_Message.gameObject.SetActive(true);
    }

    private void OnChangeWindow(MainMenuPanelManager.Window window)
    {
        if (window == MainMenuPanelManager.Window.Message)
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
        Reset();
        _window_Message.gameObject.SetActive(false);
    }

    private void Init_Variable()
    {
        _window_Message = transform;
        _btn_MessageClose = _window_Message.GetChild(0).Find("btn_Close").GetComponent<Button>();
        _btn_ReadedAll = _window_Message.GetChild(0).Find("btn_ReadedAll").GetComponent<Button>();
        _btn_DeleteAll = _window_Message.GetChild(0).Find("btn_DeleteAll").GetComponent<Button>();

        _Buttons = new List<Transform>();
        for (int i = 0; i < _window_Message.GetChild(0).Find("Buttons").childCount; i++)
        {
            _Buttons.Add(_window_Message.GetChild(0).Find("Buttons").GetChild(i));
            int index = i;
            _Buttons[i].GetComponent<Button>().onClick.AddListener(()=> ChangePanel(index));
        }

        _Panels = new List<Transform>();
        for (int i = 0; i < _window_Message.GetChild(0).Find("Panels").childCount; i++)
        {
            _Panels.Add(_window_Message.GetChild(0).Find("Panels").GetChild(i));
        }

        _MessageDetailBox = _window_Message.GetChild(1);
        _btn_DetailBoxClose = _MessageDetailBox.GetChild(0).GetChild(0).GetComponent<Button>();
    }

    private void Init_Listener()
    {
        _btn_MessageClose.onClick.AddListener(Close);
        _btn_ReadedAll.onClick.AddListener(ReadedAll);
        _btn_DeleteAll.onClick.AddListener(DeleteAll);
        _btn_DetailBoxClose.onClick.AddListener(CloseMessageDetailBox);
    }

    private void ChangePanel(int index)
    {
        for (int i = 0; i < _Panels.Count; i++)
        {
            if (i==index)
            {
                _Buttons[i].GetChild(0).gameObject.SetActive(true);
                _Panels[i].gameObject.SetActive(true);
            }
            else
            {
                _Buttons[i].GetChild(0).gameObject.SetActive(false);
                _Panels[i].gameObject.SetActive(false);
            }
        }
    }

    private void ReadedAll()
    {
        HttpManager._Instance.StartPost(@"member/notice/gameNotice/batchUpdateBymId", null, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenTipsText("操作失败");
                return;
            }

            JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
            if (jd["code"].ToString() == "1")
            {
                Refresh();
                TipsManager._Instance.OpenTipsText(jd["msg"].ToString());
            }
            else
            {
                TipsManager._Instance.OpenTipsText(jd["msg"].ToString());
            }
        });
    }

    private void DeleteAll()
    {
        TipsManager._Instance.OpenComfrimDeletePanel(() =>
        {
            HttpManager._Instance.StartPost(@"member/notice/gameNotice/batchDeleteBymId", null, (unityWebRequest) =>
            {
                if (unityWebRequest == null)
                {
                    TipsManager._Instance.OpenTipsText("删除失败");
                    return;
                }

                JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
                if (jd["code"].ToString() == "1")
                {
                    Refresh();
                    TipsManager._Instance.OpenTipsText(jd["msg"].ToString());
                }
                else
                {
                    TipsManager._Instance.OpenTipsText(jd["msg"].ToString());
                }
            });
        });
    }

    /// <summary>
    /// 刷新公告列表
    /// </summary>
    private void Refresh()
    {
        HttpManager._Instance.StartPost(@"member/notice/gameNotice/findAll",null,(unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenReConnectTipsPanel(Refresh);
                return;
            }

            JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

            if (jd["code"].ToString() == "1")
            {
                for (int i = 0; i < _Panels[0].GetChild(0).childCount; i++)
                {
                    Destroy(_Panels[0].GetChild(0).GetChild(i).gameObject);
                }

                for (int i = 0; i < _Panels[1].GetChild(0).childCount; i++)
                {
                    Destroy(_Panels[1].GetChild(0).GetChild(i).gameObject);
                }

                for (int i = 0; i < jd["result"].Count; i++)
                {
                    GameObject go;
                    if (jd["result"][i]["type"].ToString().Equals("公告"))
                    {
                         go = Instantiate(InitialResourcesManager.MessageContent, _Panels[0].GetChild(0), false);
                    }
                    else
                    {
                        go = Instantiate(InitialResourcesManager.MessageContent, _Panels[1].GetChild(0), false);
                    }
                   
                    go.transform.Find("txt_Time").GetComponent<Text>().text = GameStatic.DateFormat(jd["result"][i]["create_date"].ToString());
                    go.transform.Find("txt_Title").GetComponent<Text>().text = jd["result"][i]["title"].ToString();

                    JsonData msgInfo = jd["result"][i];
                    go.GetComponent<Button>().onClick.AddListener(() => { StartCoroutine(OpenMessageDetailPanel(msgInfo)); });

                    if (jd["result"][i]["is_read"].ToString() == "1")
                    {
                        go.transform.Find("img_readState").GetComponent<Image>().sprite = InitialResourcesManager.img_Readed;
                    }
                    else
                    {
                        go.transform.Find("img_readState").GetComponent<Image>().sprite = InitialResourcesManager.img_unread;
                    }
                }
            }
            else
            {
                Debug.LogWarning("刷新公告列表失败");
            }
        });
    }

    /// <summary>
    /// 开启消息详细信息面板
    /// </summary>
    /// <param name="msgInfo"></param>
    /// <returns></returns>
    private IEnumerator OpenMessageDetailPanel(JsonData msgInfo)
    {
        _MessageDetailBox.gameObject.SetActive(true);
        Transform group = _MessageDetailBox.GetChild(0).Find("MessageMask").GetChild(0);
        group.GetChild(0).GetComponent<Text>().text = msgInfo["title"].ToString();
        group.GetChild(1).GetComponent<Text>().text = msgInfo["content"].ToString();
        group.GetChild(2).GetComponent<Text>().text = msgInfo["create_by"].ToString();
        string id = msgInfo["id"].ToString();
        group.GetChild(4).GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
        group.GetChild(4).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { TipsManager._Instance.OpenComfrimDeletePanel(() => { DeleteMsg(id); }); });

        yield return null;

        group.GetChild(3).GetComponent<Text>().text = GameStatic.DateFormat(msgInfo["create_date"].ToString());

        Dictionary<string, string> form = new Dictionary<string, string>();
        form.Add("nId", id.ToString());

        HttpManager._Instance.StartPost(@"member/notice/gameNotice/updateRead", form,(unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                Debug.LogWarning("记录已读失败");
                return;
            }

            JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
            if (jd["code"].ToString() == "1")
            {
                Refresh();
            }
            else
            {
                TipsManager._Instance.OpenTipsText(jd["msg"].ToString());
            }
        });
    }

    /// <summary>
    /// 删除消息
    /// </summary>
    /// <param name="id"></param>
    private void DeleteMsg(string id)
    {
        Dictionary<string, string> form = new Dictionary<string, string>();
        form.Add("nId", id.ToString());

        HttpManager._Instance.StartPost(@"member/notice/gameNotice/deleteId", form,(unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                TipsManager._Instance.OpenTipsText("删除失败");
                return;
            }

            JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
            if (jd["code"].ToString() == "1")
            {
                Refresh();
                TipsManager._Instance.OpenTipsText(jd["msg"].ToString());
            }
            else
            {
                TipsManager._Instance.OpenTipsText(jd["msg"].ToString());
            }
        });

        CloseMessageDetailBox();
    }

    /// <summary>
    /// 关闭消息面板
    /// </summary>
    private void Close()
    {
        Reset();
        _window_Message.gameObject.SetActive(false);
    }

    /// <summary>
    /// 关闭详细消息面板
    /// </summary>
    private void CloseMessageDetailBox()
    {
        _MessageDetailBox.gameObject.SetActive(false);
    }

    private void Reset()
    {
        ChangePanel(0); 
        CloseMessageDetailBox();
    }
}
