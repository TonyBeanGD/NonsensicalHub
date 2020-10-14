using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class EventWindowManager : MonoBehaviour
{
    /// <summary>
    /// 活动主面板
    /// </summary>
    private Transform _window_Event;

    /// <summary>
    /// 返回按钮 活动界面
    /// </summary>
    private Button _btn_Return1;

    /// <summary>
    /// 0综合活动 1棋牌活动 2捕鱼活动 3电子活动 4视讯活动 5体育活动 的按钮
    /// </summary>
    private List<Button> _ButtonList = new List<Button>();

    /// <summary>
    /// 按钮高光list
    /// </summary>
    private List<Image> _ButtonHightLightList = new List<Image>();

    /// <summary>
    /// 0综合活动 1棋牌活动 2捕鱼活动 3电子活动 4视讯活动 5体育活动 的组
    /// </summary>
    private List<Transform> _GroupList = new List<Transform>();

    /// <summary>
    /// 滑动list
    /// </summary>
    private List<ScrollRect> _SRList = new List<ScrollRect>();

    private MainMenuPanelManager _panel;


    public void Open()
    {
        RequestEventData();
        _window_Event.gameObject.SetActive(true);

        AudioSourceManager._Instance.PlayCharacterSpeech(InitialResourcesManager.aud_Event);
    }

    private void OnChangeWindow(MainMenuPanelManager.Window window)
    {
        if (window == MainMenuPanelManager.Window.Event)
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
        _window_Event.gameObject.SetActive(false);
    }

    private void Init_Variable()
    {
        _window_Event = transform;
        _btn_Return1 = _window_Event.Find("img_Background").Find("btn_Return").GetComponent<Button>();
        //_ButtonList = _panel_Event.Find("img_Background").Find("group_Button").GetComponentsInChildren<Button>().ToList();

        //for (int i = 0; i < _ButtonList.Count; i++)
        //{
        //    //_ButtonHightLightList.Add(_panel_Event.Find("img_Background").Find("group_Button").GetChild(i).GetChild(0).GetComponent<Image>());
        //    //_ButtonTextList.Add(_panel_Event.Find("img_Background").Find("group_Button").GetChild(i).GetChild(1).GetComponent<Image>());
        //    //_GroupList.Add(_panel_Event.Find("img_Background").Find("Panels").GetChild(i).GetChild(0));
        //    //_SRList.Add(_panel_Event.Find("img_Background").Find("Panels").GetChild(i).GetComponent<ScrollRect>());
        //}
    }

    private void Init_Listener()
    {
        _btn_Return1.onClick.AddListener(() => this.Close());

        //for (int i = 0; i < _ButtonList.Count; i++)
        //{
        //    int index = i;
        //    _ButtonList[i].onClick.AddListener(() => this.ChangePanel(index));
        //}

    }



    /// <summary>
    /// 请求活动数据
    /// </summary>
    private void RequestEventData()
    {
        HttpManager._Instance.StartPost(@"not/common/gameActivityList", null, (unityWebRequest) =>
         {
             if (unityWebRequest == null)
             {
                 TipsManager._Instance.OpenReConnectTipsPanel(RequestEventData);
                 return;
             }

             JsonData jsonData = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

             if (jsonData["code"].ToString() == "1")
             {
                 _GroupList.Clear();
                 _SRList.Clear();
                 _ButtonHightLightList.Clear();
                 _ButtonList.Clear();

                 //成功
                 for (int i = 0; i < jsonData["result"].Count; i++)
                 {
                     int count = 0;

                     //创建面板容器
                     GameObject panel = GameObject.Instantiate<GameObject>(InitialResourcesManager.EventPanel);
                     panel.transform.SetParent(_window_Event.Find("img_Background").Find("Panels"), false);
                     _GroupList.Add(panel.transform.Find("Group"));
                     _SRList.Add(panel.GetComponent<ScrollRect>());

                     //创建按钮
                     GameObject EventButton = GameObject.Instantiate<GameObject>(InitialResourcesManager.EventButton);
                     EventButton.transform.SetParent(_window_Event.Find("img_Background").Find("Mask").Find("group_Button"), false);
                     _ButtonHightLightList.Add(EventButton.transform.Find("img_Hightlight").GetComponent<Image>());
                     EventButton.transform.Find("img_Text").GetComponent<Text>().text = jsonData["result"][i]["activityType"] == null ? string.Empty : jsonData["result"][i]["activityType"].ToString();
                     _ButtonList.Add(EventButton.GetComponent<Button>());
                     int index = i;
                     _ButtonList[i].onClick.AddListener(() => this.ChangePanel(index));

                     for (int j = 0; j < jsonData["result"][i]["list"].Count; j++)
                     {
                         //创建元素
                         GameObject Element = GameObject.Instantiate(InitialResourcesManager.EventElement);
                         //Element.transform.Find("txt_EventName").GetComponent<Text>().text = jsonData["result"][i]["list"][j]["title"].ToString();
                         //Element.transform.Find("txt_EventName").GetComponent<Text>().text = string.Empty;
                         DynamicResourceManager._Instance.StartSetTexture(Element.transform.Find("img_Event").GetComponent<Image>(), jsonData["result"][i]["list"][j]["preview"].ToString());

                         Element.transform.SetParent(_GroupList[i], false);
                         Element.SetActive(true);
                         count++;

                         //创建详情
                         GameObject Details = GameObject.Instantiate(InitialResourcesManager.EventDetails);

                         DynamicResourceManager._Instance.StartSetTexture(Details.GetComponent<Image>(), jsonData["result"][i]["list"][j]["detailsView"].ToString());
                         Details.transform.SetParent(_GroupList[i], false);
                         Details.SetActive(false);

                         //添加详情按钮监听
                         var g = _GroupList[i];//重要
                         Element.transform.Find("btn_Detail").GetComponent<Button>().onClick.AddListener(() => this.Onbtn_DetailClick(Details.transform, g));
                         Element.transform.Find("btn_AdvisoryService").GetComponent<Button>().onClick.AddListener(() =>
                         {
                             if (!LocalFileManager._Instance._GameData._IsLogin)
                             {
                                 Debug.Log("GameStatic.OnOpenLoginWindow?.Invoke();");
                                 return;
                             }
                             GameStatic.OnChangePanel?.Invoke(Panel.CustomerService);
                         });

                     }

                     if (InitialResourcesManager.EventElement.GetComponent<RectTransform>().sizeDelta.y * count > 620)
                     {
                         Vector2 vec2 = _GroupList[i].GetComponent<RectTransform>().sizeDelta;
                         vec2.y = InitialResourcesManager.EventElement.GetComponent<RectTransform>().sizeDelta.y * count;
                         _GroupList[i].GetComponent<RectTransform>().sizeDelta = vec2;
                     }
                 }
                 ChangePanel(0);
             }
             else
             {
                 TipsManager._Instance.OpenWarningBox(jsonData["msg"].ToString());
             }
         });


    }

    /// <summary>
    /// 详情按钮的点击
    /// </summary>
    private void Onbtn_DetailClick(Transform target, Transform group)
    {
        target.gameObject.SetActive(!target.gameObject.activeSelf);

        Vector2 vec2 = Vector2.zero;

        for (int i = 0; i < group.childCount; i++)
        {
            if (group.GetChild(i).gameObject.activeSelf)
            {
                vec2.y += (group.GetChild(i).GetComponent<RectTransform>().sizeDelta.y);
                vec2.x = (group.GetComponent<RectTransform>().sizeDelta.x);
            }
        }
        group.GetComponent<RectTransform>().sizeDelta = vec2;
    }


    /// <summary>
    /// 切换面板
    /// </summary>
    private void ChangePanel(int index)
    {
        for (int i = 0; i < _GroupList.Count; i++)
        {
            if (i == index)
            {
                if (_ButtonHightLightList[i] != null)
                {
                    _ButtonHightLightList[i].enabled = true;
                }

                if (_GroupList[i] != null)
                {
                    _GroupList[i].transform.parent.gameObject.SetActive(true);
                }
            }
            else
            {
                if (_ButtonHightLightList[i] != null)
                {
                    _ButtonHightLightList[i].enabled = false;
                }

                if (_GroupList[i] != null)
                {
                    _GroupList[i].transform.parent.gameObject.SetActive(false);
                }
            }
        }
    }

    private void Reset()
    {
        ChangePanel(0);

        for (int i = 0; i < _SRList.Count; i++)
        {
            if (_SRList[i] != null)
            {
                _SRList[i].normalizedPosition = new Vector2(0, 1);
            }
        }

        for (int i = 0; i < _GroupList.Count; i++)
        {
            if (_GroupList[i] != null)
            {
                for (int j = 0; j < _GroupList[i].childCount; j++)
                {
                    Destroy(_GroupList[i].GetChild(j).gameObject);
                }

                Destroy(_GroupList[i].parent.gameObject);
            }
        }

        for (int i = 0; i < _ButtonList.Count; i++)
        {
            if (_ButtonList[i] != null)
            {
                Destroy(_ButtonList[i].gameObject);
            }

        }
    }

    /// <summary>
    /// 返回的监听
    /// </summary>
    private void Close()
    {
        Reset();
        _window_Event.gameObject.SetActive(false);
    }
}


