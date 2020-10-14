using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsManager : MonoBehaviour
{
    public static TipsManager _Instance;

    public Transform _LoadingEffect;                //加载中特效

    public Transform _TargetParent;

    private List<Transform> _TipsPanels;            //提示面板对象池
    private List<Transform> _TipsBoxs;              //提示盒子对象池
    private List<Transform> _TipsLables;             //提示文本对象池
    private List<Transform> _TipsTexts;             //提示文本对象池

    private int _LoadingCounter;                    //加载特效计数器

    void Awake()
    {
        _Instance = this;
        Init_Variable();
        Init_State();
    }

    private void Init_Variable()
    {
        _TipsPanels = new List<Transform>();
        _TipsBoxs = new List<Transform>();
        _TipsLables = new List<Transform>();
        _TipsTexts = new List<Transform>();
    }

    private void Init_State()
    {
        _LoadingEffect.gameObject.SetActive(false);
    }

    #region public method

    /// <summary>
    /// 开启重连面板
    /// </summary>
    /// <param name="function"></param>
    public void OpenReConnectTipsPanel(Action function)
    {
        OpenTipsPanel(InitialResourcesManager.img_TiShi , false, InitialResourcesManager.img_Wifi ,InitialResourcesManager.img_TimeOut, "关闭", "重新连接", function);
    }

    /// <summary>
    /// 开启自定义超时面板
    /// </summary>
    /// <param name="button2Text"></param>
    /// <param name="function"></param>
    public void OpenCustomTimeoutTipsPanel(string button2Text, Action function)
    {
        OpenTipsPanel(InitialResourcesManager.img_TiShi, false, InitialResourcesManager.img_Wifi, InitialResourcesManager.img_TimeOut, "返回", button2Text, function);
    }


    /// <summary>
    /// 开启充值成功面板
    /// </summary>
    public void OpenRSTipsPanel()
    {
        OpenTipsPanel(InitialResourcesManager.img_ChongZhiChenGong, true,InitialResourcesManager.img_Bonus,null, string.Empty, "返回", null);
    }

    /// <summary>
    /// 开启确认删除面板
    /// </summary>
    /// <param name="action"></param>
    public void OpenComfrimDeletePanel(Action action)
    {
        OpenTipsPanel(InitialResourcesManager.img_TiShi, false, InitialResourcesManager.img_Delete ,InitialResourcesManager.img_WantToDelete , "返回", "删除", action) ;
    }

    /// <summary>
    /// 开启确认支付面板
    /// </summary>
    /// <param name="action"></param>
    public void CheckOrderPanel(Action action)
    {
        OpenTipsPanel(InitialResourcesManager.img_TiShi, false, InitialResourcesManager.img_Plus,InitialResourcesManager.img_TimeOut, "放弃支付", "确认支付", action);
    }

    /// <summary>
    /// 开启去绑定消息面板
    /// </summary>
    /// <param name="action"></param>
    public void OpenGoBindPanel(Action action)
    {
        OpenTipsPanel(InitialResourcesManager.img_BindTips , false,InitialResourcesManager.img_Bind,null, "返回", "去绑定", action);
    }

    /// <summary>
    /// 开启警告消息盒子
    /// </summary>
    /// <param name="msg"></param>
    public void OpenWarningBox(string msg)
    {
        OpenTipsBox(InitialResourcesManager.img_WarningIcon , msg);
    }

    /// <summary>
    /// 开启成功消息盒子
    /// </summary>
    /// <param name="msg"></param>
    public void OpenSuccessBox(string msg)
    {
        OpenTipsBox( InitialResourcesManager.img_SuccessIcon, msg);
    }  
    public void OpenSuccessLable(string msg)
    {
        OpenTipsLable(InitialResourcesManager.img_CopySuccess,msg);
    }

    /// <summary>
    /// 开启消息文本
    /// </summary>
    /// <param name="text"></param>
    public void OpenTipsText(string text,int? fontSize=20)
    {
        Transform crtTipsText = GetTipsText();
        crtTipsText.GetChild(0).GetComponent<Text>().text = text;
        crtTipsText.GetChild(0).GetComponent<Text>().fontSize = (int)fontSize;
        StartCoroutine(SetText(crtTipsText.GetChild(0).GetComponent<Text>()));
    }

    /// <summary>
    /// 开始加载特效
    /// </summary>
    public void StartLoading()
    {
        StartCoroutine(DelayedStart());
    }

    /// <summary>
    /// 结束加载特效
    /// </summary>
    public void StopLoading()
    {
        _LoadingCounter--;
        if (_LoadingCounter <= 0)
        {
            _LoadingEffect.gameObject.SetActive(false);
        }
    }
    #endregion


    #region private method
    /// <summary>
    /// 获取一个未使用的消息面板或者新建一个消息面板
    /// </summary>
    /// <returns></returns>
    private Transform GetTipsPanel()
    {
        for (int i = 0; i < _TipsPanels.Count; i++)
        {
            if (_TipsPanels[i].gameObject.activeSelf == false)
            {
                _TipsPanels[i].gameObject.SetActive(true);
                return _TipsPanels[i];
            }
        }

        GameObject crtGo = Instantiate(InitialResourcesManager.TipsPanel, _TargetParent, false);

        crtGo.transform.Find("img_Background").Find("btn_Left").GetComponent<Button>().onClick.AddListener(() => { crtGo.SetActive(false); });

        _TipsPanels.Add(crtGo.transform);

        return crtGo.transform;
    }

    /// <summary>
    /// 开启一个弹窗
    /// </summary>
    /// <param name="nameTextPath">名字文本图片路径</param>
    /// <param name="showLogo">是否显示log图片</param>
    /// <param name="BlowImagePath">提示文本图片路径</param>
    /// <param name="AboveImagePath">图标图片路径</param>
    /// <param name="leftText">左按钮文本</param>
    /// <param name="rightText">右按钮文本</param>
    /// <param name="function">继续按钮调用的action</param>
    private void OpenTipsPanel(Sprite nameTextPath, bool showLogo, Sprite AboveImagePath, Sprite BlowImagePath, string leftText, string rightText, Action function)
    {
        Transform crtTipsPanel = GetTipsPanel();

        Transform _OnePoint = crtTipsPanel.Find("img_Background").Find("OnePoint");                         //仅文字或图片时的放置点
        Transform _TwoPoint_1 = crtTipsPanel.Find("img_Background").Find("TwoPoint_1");                     //同时有文字和图片时的图片放置点
        Transform _TwoPoint_2 = crtTipsPanel.Find("img_Background").Find("TwoPoint_2");                     //同时有文字和图片时的文字放置点
        Transform _OnePointBtn = crtTipsPanel.Find("img_Background").Find("OnePointBtn");                   //仅返回按钮时的返回按钮放置点
        Transform _TwoPointBtn_1 = crtTipsPanel.Find("img_Background").Find("TwoPointBtn_1");               //有继续按钮时的返回按钮放置点    
        Transform _TwoPointBtn_2 = crtTipsPanel.Find("img_Background").Find("TwoPointBtn_2");               //有继续按钮时的继续按钮放置点    
        Button _btn_Left = crtTipsPanel.Find("img_Background").Find("btn_Left").GetComponent<Button>();     //返回按钮
        Button _btn_Right = crtTipsPanel.Find("img_Background").Find("btn_Right").GetComponent<Button>();   //继续按钮
        Text _txt_Left = _btn_Left.transform.GetChild(0).GetComponent<Text>();                              //左按钮上的文字
        Text _txt_Right = _btn_Right.transform.GetChild(0).GetComponent<Text>();                            //右按钮上的文字
        Image _img_Name = crtTipsPanel.Find("img_Background").Find("img_Name").GetComponent<Image>();       //提示名图片
        Image _img_BlowImage = crtTipsPanel.Find("img_Background").Find("img_Text").GetComponent<Image>();  //提示图标图片
        Image _img_AboveImage = crtTipsPanel.Find("img_Background").Find("img_Icon").GetComponent<Image>(); //提示文本图片
        Image _img_Logo = crtTipsPanel.Find("img_Background").Find("img_Logo").GetComponent<Image>();       //Logo图片

        _img_Name.sprite =nameTextPath;
        _img_Name.preserveAspect=true;

        _img_Logo.gameObject.SetActive(showLogo);
        DynamicResourceManager._Instance.StartSetTexture(_img_Logo, GameStatic.appInfo["logo"].ToString()) ;

        if (BlowImagePath !=null)
        {
            _img_AboveImage.gameObject.SetActive(true);
            _img_BlowImage.gameObject.SetActive(true);
            _img_AboveImage.transform.position = _TwoPoint_1.transform.position;
            _img_BlowImage.transform.position = _TwoPoint_2.transform.position;

            _img_AboveImage.sprite = AboveImagePath;
            _img_AboveImage.preserveAspect=true;
            _img_BlowImage.sprite =BlowImagePath;
            _img_BlowImage.preserveAspect=true;
        }
        else
        {
            _img_AboveImage.gameObject.SetActive(true);
            _img_BlowImage.gameObject.SetActive(false);
            _img_AboveImage.transform.position = _OnePoint.transform.position;

            _img_AboveImage.sprite =AboveImagePath;
            _img_AboveImage.preserveAspect=true;
        }

        if (leftText != string.Empty)
        {
            _btn_Left.gameObject.SetActive(true);
            _btn_Right.gameObject.SetActive(true);

            _txt_Left.text = leftText;
            _txt_Right.text = rightText;

            _btn_Left.transform.position = _TwoPointBtn_1.position;
            _btn_Right.transform.position = _TwoPointBtn_2.position;


            _btn_Right.onClick.RemoveAllListeners();
            _btn_Right.onClick.AddListener(() => { crtTipsPanel.gameObject.SetActive(false); });
            _btn_Right.onClick.AddListener(() => { function?.Invoke(); });
        }
        else
        {
            _btn_Left.gameObject.SetActive(false);
            _btn_Right.gameObject.SetActive(true);
            _txt_Right.text = rightText;
            _btn_Right.transform.position = _OnePointBtn.position;

            _btn_Right.onClick.RemoveAllListeners();
            _btn_Right.onClick.AddListener(() => { crtTipsPanel.gameObject.SetActive(false); });
            _btn_Right.onClick.AddListener(() => { function?.Invoke(); });
        }
    }

    /// <summary>
    /// 获取一个未使用的消息盒子或者新建一个消息盒子
    /// </summary>
    /// <returns></returns>
    private Transform GetTipsBox()
    {
        for (int i = 0; i < _TipsBoxs.Count; i++)
        {
            if (_TipsBoxs[i].gameObject.activeSelf == false)
            {
                _TipsBoxs[i].gameObject.SetActive(true);
                return _TipsBoxs[i];
            }
        }

        GameObject crtGO = Instantiate(InitialResourcesManager.TipsBox, _TargetParent, false);

        crtGO.transform.Find("btn_Close").GetComponent<Button>().onClick.AddListener(() => { crtGO.SetActive(false); });

        _TipsBoxs.Add(crtGO.transform);

        return crtGO.transform;
    }

    /// <summary>
    /// 开启消息盒子
    /// </summary>
    /// <param name="iconPath"></param>
    /// <param name="message"></param>
    private void OpenTipsBox(Sprite iconPath, string message)
    {
        Transform crtTipsBox = GetTipsBox();
        crtTipsBox.transform.Find("img_Background").Find("img_Icon").GetComponent<Image>().sprite = iconPath;
        crtTipsBox.transform.Find("img_Background").Find("txt_Tips").GetComponent<Text>().text = message;
    }

    /// <summary>
    /// 获取一个未使用的提示标签或者新建一个提示标签
    /// </summary>
    /// <returns></returns>
    private Transform GetTipsLable()
    {
        for (int i = 0; i < _TipsLables.Count; i++)
        {
            if (_TipsLables[i].gameObject.activeSelf == false)
            {
                _TipsLables[i].gameObject.SetActive(true);
                _TipsLables[i].transform.SetAsLastSibling();
                return _TipsLables[i];
            }
        }

        GameObject crtGO = Instantiate(InitialResourcesManager.TipsLable, _TargetParent, false);
        _TipsLables.Add(crtGO.transform);
        return crtGO.transform;
    }
    private void OpenTipsLable(Sprite iconPath, string message)
    {
        Transform crtTipsLable = GetTipsLable();
        //crtTipsBox.GetComponent<Animator>();

        crtTipsLable.transform.Find("img_Icon").GetComponent<Image>().sprite = iconPath;
        crtTipsLable.transform.Find("txt_Lable").GetComponent<Text>().text = message;

        StartCoroutine(FadingAway(crtTipsLable.gameObject)) ;
    }

    private IEnumerator FadingAway(GameObject go)
    {
        yield return new WaitForSeconds(2.5f);
        go.SetActive(false);
    }


    /// <summary>
    /// 获取一个未使用的提示文本或者新建一个提示文本
    /// </summary>
    /// <returns></returns>
    private Transform GetTipsText()
    {
        for (int i = 0; i < _TipsTexts.Count; i++)
        {
            if (_TipsTexts[i].gameObject.activeSelf == false)
            {
                _TipsTexts[i].gameObject.SetActive(true);
                return _TipsTexts[i];
            }
        }

        GameObject crtGO = Instantiate(InitialResourcesManager.TipsText, _TargetParent, false);
        _TipsTexts.Add(crtGO.transform);
        return crtGO.transform;
    }

    /// <summary>
    /// 使文本与背景适应
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private IEnumerator SetText(Text target)
    {
        yield return null;
        target.text = " " + target.text + " ";
    }


    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.2f);
        _LoadingCounter++;
        if (_LoadingCounter > 0)
        {
            _LoadingEffect.gameObject.SetActive(true);
        }
    }
    #endregion
}
