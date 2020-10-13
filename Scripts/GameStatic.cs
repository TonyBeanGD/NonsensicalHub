using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 窗口类型
/// </summary>
public enum Panel
{
    MainMenu,
    PersonCenter,                //个人中心窗口
    Promotion,                   //推广赚钱窗口
    WashCode,                    //洗码窗口
    CustomerService,             //客服窗口
    Safe,                        //保险箱窗口
    Withdraw,                    //提现窗口
    Recharge,                    //充值窗口
}

public static class GameStatic
{
    /// <summary>
    /// 字体用的的金色
    /// </summary>
    public static readonly Color GoldColor = new Color(239 / (float)255, 208 / (float)255, 162 / (float)255);

    /// <summary>
    /// uncoding编码空格
    /// </summary>
    public static readonly string no_breaking_space = "\u00A0";

    /// <summary>
    /// 游戏是否正在开启中
    /// </summary>
    public static bool IsGameOpening = false;

    /// <summary>
    /// 平台类型
    /// </summary>
#if UNITY_WEBGL
    public static readonly string SourceType="0";
#elif UNITY_ANDROID
    public static readonly string SourceType = "1";
#elif UNITY_IOS
    public static readonly string SourceType = "2";
#else
    public static readonly string SourceType = "0";
#endif

#if LOCAL
    public static readonly string urlRoot = @"http://10.10.72.27:8081/";
    //public static readonly string WebsocketUrl = @"ws://10.10.72.27:9002/api/webSocket/1/";                               
    public static readonly string WebsocketUrl = @"ws://10.10.18.35:9002/api/webSocket/1/";
    public static readonly string HttpPostUrl = @"http://10.10.18.35:9000/api/";
#elif LESHI
    public static readonly string urlRoot = @"https://res.ls.mk/";
    public static readonly string WebsocketUrl = @"wss://service.im.ls.mk/api/webSocket/1/";
    public static readonly string HttpPostUrl = @"https://game.ls.mk/api/";
#elif BENCHI
    public static readonly string urlRoot = @"https://res.9974411.com/";
    public static readonly string WebsocketUrl= @"wss://service.im.9974411.com/api/webSocket/1/";
    public static readonly string HttpPostUrl= @"https://game.9974411.com/api/";
#endif

    /// <summary>
    /// APPInfo
    /// "id":1,"url":"https://www.baidu.com/","logo":"http://10.10.72.27:8081/version/157199940743382668.png","state":1,"isGraphVerifi":1,"isRealName":1,"isPhone":1,"isSmsCode":1,"promoteUrl":"","onlineServiceUrl":"","version":"","lua":""
    /// </summary>
    public static JsonData appInfo;

    /// <summary>
    /// 本地AB包加载完毕时
    /// </summary>
    public static Action OnLocalAssetBundleLoadComplete;

    /// <summary>
    /// 主要面板初始化的时候
    /// </summary>
    public static Action OnMainPanelInit;

    /// <summary>
    /// 登录的时候
    /// </summary>
    public static Action OnLogin;

    /// <summary>
    /// 登出的时候
    /// </summary>
    public static Action OnLogout;

    /// <summary>
    /// 初始化加载完毕的时候
    /// </summary>
    public static Action OnInitComplete;

    /// <summary>
    /// 切换面板的时候
    /// </summary>
    public static Action<Panel> OnChangePanel;

    /// <summary>
    /// 需要设置保险箱钱数的时候
    /// </summary>
    public static Action<string, string> OnSetSafeMoneyText;

    /// <summary>
    /// 下分的时候
    /// </summary>
    public static Action OnDownScore;


    /// <summary>
    /// 时间戳格式化
    /// </summary>
    /// <param name="originDateString"></param>
    /// <returns></returns>
    public static string DateFormat(string originDateString)
    {
        return DateTime.Parse(originDateString).ToString("yyyy-MM-dd HH:mm");
    }

    /// <summary>
    /// 检测jsondata是否为空
    /// </summary>
    /// <param name="jsondata">检测的JsonData</param>
    /// <returns></returns>
    public static string CheckNull(JsonData jsondata)
    {
        string str = string.Empty;

        if (jsondata != null)
        {
            str = jsondata.ToString();
        }

        return str;
    }

    /// <summary>
    /// 复制字符串到剪切板
    /// </summary>
    /// <param name="copyTarget">复制的字符串</param>
    public static void CopyString(string copyString)
    {
        GUIUtility.systemCopyBuffer = copyString;
        TipsManager._Instance.OpenSuccessLable("复制成功");
    }


    public static void Base64ToImg(Image imgComponent, string base64)
    {
        byte[] bytes = Convert.FromBase64String(base64);
        Texture2D tex2D = new Texture2D(100, 100);
        tex2D.LoadImage(bytes);
        Sprite s = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f));
        imgComponent.sprite = s;
        imgComponent.preserveAspect = true;
        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// 退出应用
    /// </summary>
    public static void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
