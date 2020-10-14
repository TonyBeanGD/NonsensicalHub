using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;
using UnityEngine.UI;
using LitJson;

public class UniWebController : MonoBehaviour
{
    public static UniWebController _Instance;

    public RectTransform WebBG;

    private UniWebView WebView;

    private bool quiting = false;

    private void Awake()
    {
        _Instance = this;
    }

    private void OnReceivedMessage(UniWebView webView, UniWebViewMessage message)
    {
        JsonData jsonData = JsonMapper.ToObject(message.RawMessage.Substring(13));

        if (jsonData["type"] == null)
        {
            Debug.LogWarning($"未知的字符串：{message.RawMessage}");
            return;
        }

        switch (jsonData["type"].ToString())
        {
            case "0":
                {
                    Debug.Log("关闭面板");
                }
                break;
            case "1":
                {
                    if (quiting == true)
                    {
                        return;
                    }
                    quiting = true;

                    Screen.orientation = ScreenOrientation.LandscapeLeft;
                    Debug.Log(message.RawMessage);
                    CanvasManager._Instance.Mask(false);
                    StartCoroutine(CloseWebView());
                }
                break;
            case "10":
                {
                    Application.OpenURL(jsonData["url"].ToString());
                    Debug.Log($"下单成功返回url：{message.RawMessage}");
                }
                break;
            default:
                Debug.LogWarning($"未知的类型：{message.RawMessage}");
                break;
        }
    }

    private void OnLoadStart(UniWebView webView, string url)
    {
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = false;

        CanvasManager._Instance.Mask(true);
        AudioSourceManager._Instance.PauseBGM();
        TipsManager._Instance.StopLoading();
        webView.Show();
        GameStatic.IsGameOpening = false;

        webView.AddJavaScript(InitialResourcesManager.txt_JsTest.text, (payload) => { if (payload.resultCode != "0") Debug.Log($"{payload.resultCode}|{payload.data}"); });

        webView.EvaluateJavaScript("init()", (payload) => { if (payload.resultCode != "0") Debug.Log($"{payload.resultCode}|{payload.data}"); });
    }

    private void OnLoadComplete(UniWebView webView, int success, string errorMessage)
    {
        webView.AddJavaScript(InitialResourcesManager.txt_JsTest.text, (payload) => { if (payload.resultCode != "0") Debug.Log($"{payload.resultCode}|{payload.data}"); });

        webView.EvaluateJavaScript("init()", (payload) => { if (payload.resultCode != "0") Debug.Log($"{payload.resultCode}|{payload.data}"); });
    }

    private bool OnShouldClose(UniWebView webView)
    {
        Debug.Log("返回按钮");
        webView.EvaluateJavaScript("openQuitConfirm();", (payload) => { if (payload.resultCode != "0") Debug.Log($"{payload.resultCode}|{payload.data}"); });
        return false;
    }

    public void OpenTestPage(string url)
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        Application.OpenURL(url);
        GameStatic.IsGameOpening = false;
        TipsManager._Instance.StopLoading();
#elif UNITY_IOS || UNITY_ANDROID
        
        //TipsManager._Instance.OpenTipsText("开启游戏中");
        //LocalNotification.SendNotification(1, 1, "游戏大厅", "游戏已开启", new Color32(0xff, 0x44, 0x44, 255));

        if (WebView == null)
        {
            WebView = gameObject.AddComponent<UniWebView>();
            WebView.ReferenceRectTransform = WebBG;
            WebView.OnMessageReceived += OnReceivedMessage;
            WebView.OnPageStarted += OnLoadStart;
            WebView.OnPageFinished += OnLoadComplete;
            WebView.OnPageErrorReceived += (view, error, message) =>
            {
                Debug.LogError(message);
            };
            WebView.OnShouldClose += OnShouldClose;
        }
        WebView.Load(url);
#elif UNITY_WEBGL
        Application.ExternalEval("document.getElementById('mydiv').style.display='block';document.getElementById('myframe').src = '" + url + "';");
        Application.ExternalCall("createWindow");
#endif
    }

    private IEnumerator CloseWebView()
    {
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;

        WebView.CleanCache();
        Destroy(WebView);
        WebView = null;
        AudioSourceManager._Instance.ResumeBGM();
        yield return new WaitForSeconds(0.5F);

        quiting = false;
        GameStatic.OnDownScore?.Invoke();
    }
}
