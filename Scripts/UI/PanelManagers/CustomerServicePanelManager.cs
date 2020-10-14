using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EmojiInfo
{
    public int pos;
    public int type;

    public EmojiInfo(int pos, int type)
    {
        this.pos = pos;
        this.type = type;
    }
}

public class CustomerServicePanelManager : MonoBehaviour
{
    public static CustomerServicePanelManager _Instance;

    private Transform _panel_CustomerService;           //客服面板
    private Button _btn_Return;
    private Button _btn_ClosePanel;

    private InputField _ipf_Message;                    //消息输入框
    private Button _btn_Emoji;                          //emoji按钮
    private Button _btn_Enter;                          //确认按钮

    private ScrollRect _scr_Messages;                   //消息Scrollrect
    private Transform _MessageGroup;

    private Transform _panel_Emoji;                     //emoji面板

    private Transform _panel_FAQ;                       //FAQ面板
    private Button _btn_More;
    private Button _btn_Collapse;
    private Transform _FAQContent;

    private Transform _panel_Evaluation;
    private List<Button> _EvaluationOptions;

    private List<string> FAQ;

    private string[] _Emojis = new string[] { "[微笑]", "[嘻嘻]", "[哈哈]", "[可爱]", "[可怜]", "[挖鼻]", "[吃惊]", "[害羞]", "[挤眼]", "[闭嘴]", "[鄙视]", "[爱你]", "[泪]", "[偷笑]", "[亲亲]", "[生病]", "[太开心]", "[白眼]", "[右哼哼]", "[左哼哼]", "[嘘]", "[衰]", "[委屈]", "[吐]", "[哈欠]", "[抱抱]", "[怒]", "[疑问]", "[馋嘴]", "[拜拜]", "[思考]", "[汗]", "[困]", "[睡]", "[钱]", "[失望]", "[酷]", "[色]", "[哼]", "[鼓掌]", "[晕]", "[悲伤]", "[抓狂]", "[黑线]", "[阴险]", "[怒骂]", "[互粉]", "[心]", "[伤心]", "[猪头]", "[熊猫]", "[兔子]", "[ok]", "[耶]", "[good]", "[NO]", "[赞]", "[来]", "[弱]", "[草泥马]", "[神马]", "[囧]", "[浮云]", "[给力]", "[围观]", "[威武]", "[奥特曼]", "[礼物]", "[钟]", "[话筒]", "[蜡烛]", "[蛋糕]" };

    private string message = "";
    private bool isMessageReceive = false;

    private void ChangePanel(Panel panel)
    {
        if (panel == Panel.CustomerService)
        {
            GetSysService();
            _panel_CustomerService.gameObject.SetActive(true);

            AudioSourceManager._Instance.PlayCharacterSpeech(InitialResourcesManager.aud_CustomerService);
        }
        else
        {
            if (_panel_CustomerService.gameObject.activeSelf == true)
            {
                _panel_CustomerService.gameObject.SetActive(false);
            }
        }
    }

    void Awake()
    {
        GameStatic.OnMainPanelInit += () => StartCoroutine(Init_State());
        GameStatic.OnMainPanelInit += Init_Websocket;

        GameStatic.OnChangePanel += ChangePanel;

        _Instance = this;

        for (int i = 0; i < _Emojis.Length; i++)
        {
            _Emojis[i] = "face" + _Emojis[i];
        }

        Init_Variable();
        Init_Listener();
    }

    void Update()
    {
        if (isMessageReceive)
        {
            isMessageReceive = false;
            AddMessage(-1, 0, message, null);
        }

        _panel_Emoji.GetChild(1).GetComponent<RectTransform>().anchoredPosition = _panel_Emoji.GetChild(0).GetComponent<RectTransform>().anchoredPosition;
    }

    private void Init_Variable()
    {

        _panel_CustomerService = transform;

        _btn_Return = _panel_CustomerService.Find("Header").Find("btn_Return").GetComponent<Button>();
        _btn_ClosePanel = _panel_CustomerService.Find("UIBody").Find("btn_ClosePanel").GetComponent<Button>();

        _ipf_Message = _panel_CustomerService.Find("Footer").Find("ipf_Message").GetComponent<InputField>();
        _btn_Emoji = _panel_CustomerService.Find("Footer").Find("btn_Emoji").GetComponent<Button>();
        _btn_Enter = _panel_CustomerService.Find("Footer").Find("btn_Enter").GetComponent<Button>();

        _scr_Messages = _panel_CustomerService.Find("UIBody").GetComponent<ScrollRect>();
        _MessageGroup = _panel_CustomerService.Find("UIBody").Find("Group");

        _btn_More = _panel_CustomerService.Find("UIBody").Find("btn_FAQ").GetComponent<Button>();
        _panel_FAQ = _panel_CustomerService.Find("UIBody").Find("panel_FAQ");
        _btn_Collapse = _panel_FAQ.Find("btn_Collapse").GetComponent<Button>();
        _FAQContent = _panel_FAQ.Find("FAQArea").Find("FAQContent");

        _panel_Emoji = _panel_CustomerService.Find("UIBody").Find("panel_Emoji");

        _panel_Evaluation = _panel_CustomerService.Find("panel_Evaluation");

        _EvaluationOptions = new List<Button>();
        for (int i = 0; i < _panel_Evaluation.Find("group").childCount; i++)
        {
            _EvaluationOptions.Add(_panel_Evaluation.Find("group").GetChild(i).GetComponent<Button>());
        }
    }

    private void Init_Listener()
    {
        _btn_Return.onClick.AddListener(ReturnMainMenu);
        _btn_ClosePanel.onClick.AddListener(ClosePanel);

        _btn_Enter.onClick.AddListener(EnterButtonClick);
        _btn_Emoji.onClick.AddListener(() => { _btn_ClosePanel.gameObject.SetActive(!_panel_Emoji.gameObject.activeSelf); _panel_Emoji.gameObject.SetActive(!_panel_Emoji.gameObject.activeSelf); _panel_Emoji.GetComponent<ScrollRect>().verticalNormalizedPosition = 1; });

        _btn_More.onClick.AddListener(() => { _btn_ClosePanel.gameObject.SetActive(true); _panel_FAQ.gameObject.SetActive(true); });
        _btn_Collapse.onClick.AddListener(() => { _btn_ClosePanel.gameObject.SetActive(false); _panel_FAQ.gameObject.SetActive(false); });

        _ipf_Message.onValueChanged.AddListener(MessageValueChange);
    }

    private IEnumerator Init_State()
    {
        LoadingManager.LoadingStart(this.GetType().ToString());
        Reset();
        Init_EmojiPanel();
        yield return StartCoroutine(RefreshFAQ());

        LoadingManager.LoadingComplete(this.GetType().ToString());
    }

    /// <summary>
    /// 初始化Emoji面板
    /// </summary>
    private void Init_EmojiPanel()
    {
        for (int i = 0; i < _Emojis.Length; i++)
        {
            GameObject go = Instantiate(InitialResourcesManager.GifImage, _panel_Emoji.GetChild(0), false);
            GameObject buttonGO = Instantiate(InitialResourcesManager.TransparentButton, _panel_Emoji.GetChild(1), false);

            Button button = buttonGO.GetComponent<Button>();
            int index = i;
            button.onClick.AddListener(() => { _ipf_Message.text += _Emojis[index]; StartCoroutine(MoveTextEnd(_ipf_Message)); });
            StartCoroutine(go.GetComponent<UniGifImage>().SetGifFromUrlCoroutine("face/" + i.ToString() + ".gif"));
        }
    }

    private IEnumerator MoveTextEnd(InputField ipf)
    {
        ipf.ActivateInputField();
        yield return null;
        ipf.MoveTextEnd(false);
    }

    /// <summary>
    /// 关闭面板按钮监听
    /// </summary>
    private void ClosePanel()
    {
        if (_panel_Emoji.gameObject.activeSelf)
        {
            _panel_Emoji.gameObject.SetActive(false);
        }
        else if (_panel_FAQ.gameObject.activeSelf)
        {
            _panel_FAQ.gameObject.SetActive(false);
        }
        _btn_ClosePanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// 输入消息栏变更消息时，变更确认按钮显示
    /// </summary>
    /// <param name="msg"></param>
    private void MessageValueChange(string msg)
    {
        if (msg.Equals(string.Empty))
        {
            _btn_Enter.GetComponent<Image>().sprite = InitialResourcesManager.img_Plus;
        }
        else
        {
            _btn_Enter.GetComponent<Image>().sprite = InitialResourcesManager.img_Send;
        }
    }

    /// <summary>
    /// 初始化websocket相关
    /// </summary>
    private void Init_Websocket()
    {
        LoadingManager.LoadingStart(this.GetType().ToString());
        WebsocketHelper._Instance._SendMessageCallback += SendMessageCallBack;
        WebsocketHelper._Instance._ReceiveMessage += ReceiveMessage;
        LoadingManager.LoadingComplete(this.GetType().ToString());
    }

    private void Reset()
    {
        _ipf_Message.text = string.Empty;
        _panel_Evaluation.gameObject.SetActive(false);
        _panel_FAQ.gameObject.SetActive(false);
        _panel_Emoji.gameObject.SetActive(false);
        _btn_ClosePanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// 刷新FAQ
    /// </summary>
    private IEnumerator RefreshFAQ()
    {
        bool isReciver = false;
        bool isOk = false;

        while (true)
        {
            HttpManager._Instance.StartPost(@"not/common/getFaqAll", null, (unityWebRequest) =>
            {
                if (unityWebRequest == null)
                {
                    isReciver = true;
                    return;
                }

                JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

                if (jd["code"].ToString() == "1")
                {
                    FAQ = new List<string>();
                    for (int i = 0; i < jd["result"].Count; i++)
                    {
                        GameObject go = Instantiate(InitialResourcesManager.FAQElement, _FAQContent, false);
                        Button btn = go.GetComponent<Button>();
                        Text text = go.transform.GetChild(0).GetComponent<Text>();

                        text.text = (i + 1) + "." + jd["result"][i]["fagTitle"].ToString();

                        FAQ.Add(jd["result"][i]["faqAnswer"].ToString());

                        int index = i;
                        btn.onClick.AddListener(() =>
                        {
                            AddMessage(0, 0, FAQ[index], null);
                        });
                    }
                    isReciver = true;
                    isOk = true;
                }
                else
                {
                    Debug.LogWarning("获取FAQ失败");
                }
            });

            while (isReciver == false)
            {

                yield return new WaitForSeconds(0.2f);
            }
            isReciver = false;

            if (isOk)
            {
                break;
            }

            yield return new WaitForSeconds(2f);
        }
    }

    /// <summary>
    /// 申请客服
    /// </summary>
    private void GetSysService()
    {
        HttpManager._Instance.StartPost(@"webSocket/getSysService", null, (unityWebRequest) =>
          {
              if (unityWebRequest == null)
              {
                  TipsManager._Instance.OpenReConnectTipsPanel(GetSysService);
                  return;
              }

              Debug.Log(unityWebRequest.downloadHandler.text);
              JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

              if (jd["code"].ToString() == "1")
              {
                  AddMessage(0, 0, "客服 " + jd["result"]["serviceName"].ToString() + " 连接成功", null);
              }
              else
              {
                  AddMessage(0, 0, "客服连接失败", null);
              }
          });
    }

    /// <summary>
    /// 结束对话
    /// </summary>
    private void EndDialogue()
    {
        HttpManager._Instance.StartPost(@"webSocket/endDialogue", null, (unityWebRequest) =>
          {
              if (unityWebRequest == null)
              {
                  TipsManager._Instance.OpenReConnectTipsPanel(EndDialogue);
                  return;
              }

              JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

              if (jd["code"].ToString() == "1")
              {
                  EvaluateService();
              }
              else
              {
                  Debug.Log("结束对话失败");
              }
          });
    }

    /// <summary>
    /// 评价服务
    /// </summary>
    private void EvaluateService()
    {
        int i = 1;
        Dictionary<string, string> form = new Dictionary<string, string>();
        form.Add("evaluateLevel", i.ToString());

        HttpManager._Instance.StartPost(@"webSocket/evaluateService", form, (unityWebRequest) =>
         {
             if (unityWebRequest == null)
             {
                 TipsManager._Instance.OpenReConnectTipsPanel(EvaluateService);
                 return;
             }

             JsonData jd = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

             if (jd["code"].ToString() == "1")
             {
                 Debug.Log("评价成功");
             }
             else
             {
                 Debug.Log("评价失败");
             }
         });
    }

    /// <summary>
    /// 检测文本是否带有emoji标识
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="text"></param>
    private void CheckEmoji(string msg, Text text)
    {
        List<EmojiInfo> emojiInfos = new List<EmojiInfo>();

        for (int i = 0; i < _Emojis.Length; i++)
        {
            if (msg.Contains(_Emojis[i]))
            {
                List<int> indexs = new List<int>();

                int crtIndex = 0;
                while ((crtIndex = msg.IndexOf(_Emojis[i], crtIndex)) >= 0)
                {
                    indexs.Add(crtIndex);
                    emojiInfos.Add(new EmojiInfo(crtIndex, i));
                    crtIndex += _Emojis[i].Length;
                }
            }
        }

        for (int i = 0; i < emojiInfos.Count; i++)
        {
            for (int j = 0; j < emojiInfos.Count - i - 1; j++)
            {
                if (emojiInfos[j].pos > emojiInfos[j + 1].pos)
                {
                    EmojiInfo temp = emojiInfos[j];
                    emojiInfos[j] = emojiInfos[j + 1];
                    emojiInfos[j + 1] = temp;
                }
            }
        }

        int offset = 0;

        for (int i = 0; i < emojiInfos.Count; i++)
        {
            emojiInfos[i].pos += offset;
            offset -= _Emojis[emojiInfos[i].type].Length;
            offset += 3;
        }

        for (int i = 0; i < _Emojis.Length; i++)
        {
            msg = msg.Replace(_Emojis[i], "   ");
        }

        if (msg.Contains(" "))
        {
            msg = msg.Replace(" ", GameStatic.no_breaking_space);
        }

        text.text = msg;

        for (int i = 0; i < emojiInfos.Count; i++)
        {
            GameObject go = Instantiate(InitialResourcesManager.GifImage, text.transform, false);

            StartCoroutine(test(go, text, emojiInfos[i].pos, emojiInfos[i].type));
        }

    }

    /// <summary>
    /// 得到Text中字符的位置；canvas:所在的Canvas，text:需要定位的Text,charIndex:Text中的字符位置
    /// </summary>
    private Vector3 GetPosAtText(Canvas canvas, Text text, int charIndex)
    {
        string textStr = text.text;
        Vector3 charPos = Vector3.zero;
        if (charIndex <= textStr.Length && charIndex > 0)
        {
            TextGenerator textGen = new TextGenerator(textStr.Length);
            Vector2 extents = text.gameObject.GetComponent<RectTransform>().rect.size;
            textGen.Populate(textStr, text.GetGenerationSettings(extents));

            int newLine = textStr.Substring(0, charIndex).Split('\n').Length - 1;
            int indexOfTextQuad = (charIndex * 4) + (newLine * 4) - 4;
            if (indexOfTextQuad < textGen.vertexCount)
            {
                charPos = (textGen.verts[indexOfTextQuad + 0].position +
                    textGen.verts[indexOfTextQuad + 1].position +
                    textGen.verts[indexOfTextQuad + 2].position +
                    textGen.verts[indexOfTextQuad + 3].position) / 4f;
            }
        }
        charPos /= canvas.scaleFactor;//适应不同分辨率的屏幕
        charPos = text.transform.TransformPoint(charPos);//转换为世界坐标
        return charPos;
    }

    /// <summary>
    /// 添加消息文本，自动进行自适应
    /// </summary>
    /// <param name="go"></param>
    /// <param name="text"></param>
    /// <param name="pos"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private IEnumerator test(GameObject go, Text text, int pos, int type)
    {
        yield return null;
        yield return null;

        TextGenerator textGen = text.cachedTextGenerator;

        Vector3 imagePos = GetPosAtText(CanvasManager._Instance._Canvas.GetComponent<Canvas>(), text, pos + 2);

        imagePos = new Vector3(imagePos.x + text.fontSize * 0.002f, imagePos.y + text.fontSize * 0.005f, 0);
        go.GetComponent<RectTransform>().position = imagePos;

        yield return StartCoroutine(go.GetComponent<UniGifImage>().SetGifFromUrlCoroutine("face/" + type.ToString() + ".gif"));
        go.GetComponent<RectTransform>().sizeDelta = new Vector2(text.fontSize * (-0.005f * text.fontSize + 1), text.fontSize * (-0.005f * text.fontSize + 1));
    }

    /// <summary>
    /// 往页面上添加消息
    /// </summary>
    /// <param name="position">消息的位置，代表着是哪边发送的消息，小于0在左边，代表着客服发来的消息，大于0在右边，代表着用户发过去的消息，等于0在中间，代表着关于聊天的系统消息</param>
    /// <param name="type">消息的类型，0代表纯文本</param>
    /// <param name="msg">消息的内容</param>
    private void AddMessage(int position, int type, string msg, Action action)
    {
        GameObject go = Instantiate(InitialResourcesManager.MessageElement, _MessageGroup, false);

        if (position < 0)
        {
            go.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.MiddleLeft;
        }
        else if (position > 0)
        {
            go.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.MiddleRight;
        }
        else
        {
            go.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;
        }

        switch (type)
        {
            case 0:
                {
                    CheckEmoji(msg, go.transform.GetChild(0).GetChild(0).GetComponent<Text>());
                }
                break;
            case 1:
                {
                    go.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = msg;
                    go.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => { action(); });
                }
                break;
            case 2:
                {
                    go.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "图片消息" + msg;
                }
                break;
            default:
                {
                    Debug.LogError("AddMessage Undefined Index");
                }
                break;
        }

        StartCoroutine(AddMessage(go));
    }

    /// <summary>
    /// 往消息预制体上添加消息
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    private IEnumerator AddMessage(GameObject go)
    {
        yield return null;
        Transform text = go.transform.GetChild(0).GetChild(0);
        RectTransform textRect = text.GetComponent<RectTransform>();

        float curTextWidth = textRect.rect.width;

        if (curTextWidth >= 450)
        {
            text.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            text.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            textRect.sizeDelta = new Vector2(450, 10);
        }

        yield return null;

        go.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(textRect.rect.width + 20, textRect.rect.height + 20);

        go.transform.GetComponent<LayoutElement>().preferredHeight = go.transform.GetChild(0).GetComponent<RectTransform>().rect.height;
        textRect.anchoredPosition = new Vector2(0, 0);
        yield return null;
        _scr_Messages.verticalNormalizedPosition = 0f;
    }

    /// <summary>
    /// 返回主菜单
    /// </summary>
    private void ReturnMainMenu()
    {
        GameStatic.OnChangePanel?.Invoke(Panel.MainMenu);

        EndDialogue();

        Reset();
    }

    /// <summary>
    /// 确认按钮按下监听
    /// </summary>
    private void EnterButtonClick()
    {
        _panel_Emoji.gameObject.SetActive(false);
        if (_ipf_Message.text.Equals(string.Empty))
        {
            Debug.Log("Plus");
        }
        else
        {
            SendMessage();
            MessageValueChange(string.Empty);
        }
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    private void SendMessage()
    {
        if (_ipf_Message.text.Equals(string.Empty))
        {
            return;
        }
        AddMessage(1, 0, _ipf_Message.text, null);

        WebsocketHelper._Instance.SendMessageToServer(_ipf_Message.text);

        _ipf_Message.text = string.Empty;
    }

    /// <summary>
    /// 发送消息回调
    /// </summary>
    /// <param name="b"></param>
    private void SendMessageCallBack(bool b)
    {
        if (b)
        {
            //Debug.Log("消息发送成功");
        }
        else
        {
            Debug.LogError("发送消息失败");
        }
    }

    /// <summary>
    /// 收到消息事件
    /// </summary>
    /// <param name="msg"></param>
    private void ReceiveMessage(string msg)
    {
        message = msg;
        isMessageReceive = true;
    }

    /// <summary>
    /// 发送声音消息
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="action"></param>
    public void SendVoiceMessage(string msg, Action action)
    {
        AddMessage(1, 1, msg, action);
    }
}
