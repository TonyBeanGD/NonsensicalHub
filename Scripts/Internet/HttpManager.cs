using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HttpManager : MonoBehaviour
{
    public static HttpManager _Instance;

    private Dictionary<string, string> _header = new Dictionary<string, string>();  //post请求头

    private Dictionary<string, string> _aaform = new Dictionary<string, string>();  //aa表单，用于加入不需要数据的post接口

    private Dictionary<string, string> _postInfos = new Dictionary<string, string>();

    private void Awake()
    {
        _header.Add("Content-Type", "application/json");
        if (LocalFileManager._Instance._GameData._IsLogin)
        {
            _header.Add("token", LocalFileManager._Instance._GameData._Token);
        }
        else
        {
            _header.Add("token", string.Empty);
        }

        _aaform.Add("a", "a");
        _Instance = this;
    }

    private void Update()
    {
        if (GetInfos.CanGet() == true)
        {
            GetInfos.canRequest--;
            GetInfos.GetInfo gi = GetInfos.Pop();
            StartCoroutine(HttpHelper.GetTexture(gi.url, (texture) => { GetInfos.canRequest++; gi.actionResult(texture); }));
        }
    }

    public static class GetInfos
    {
        private static Stack<GetInfo> getInfos = new Stack<GetInfo>();

        public static int canRequest = 10;

        public static GetInfo Pop()
        {
            return getInfos.Pop();
        }

        public static void Push(GetInfo getInfo)
        {
            getInfos.Push(getInfo);
        }

        public static bool CanGet()
        {
            if (getInfos.Count > 0 && canRequest > 0)
            {
                return true;
            }
            return false;
        }

        public class GetInfo
        {
            public string url;
            public Action<Texture2D> actionResult;
            public GetInfo(string _url, Action<Texture2D> _actionResult)
            {
                url = _url;
                actionResult = _actionResult;
            }
        }
    }

    /// <summary>
    /// 设置表头中的token
    /// </summary>
    /// <param name="token"></param>
    public void SetToken(string token)
    {
        _header["token"] = token;
    }

    /// <summary>
    /// 通过http-get的方式获取图片
    /// </summary>
    /// <param name="url"></param>
    /// <param name="actionResult"></param>
    public void StartGetTexture(string url, Action<Texture2D> actionResult)
    {
        GetInfos.Push(new GetInfos.GetInfo(url, actionResult));
    }

    /// <summary>
    /// 开启post请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="dss"></param>
    /// <param name="callback"></param>
    public void StartPost(string url, Dictionary<string, string> dss, Action<UnityWebRequest> callback, bool? alwayDoIt = true)
    {
        url = Path.Combine(GameStatic.HttpPostUrl, url);
        if (dss == null)
        {
            dss = _aaform;
        }
        StartCoroutine(HttpHelper.Post(url, dss, _header, (UnityWebRequest) =>
        {
            if (alwayDoIt == true)
            {
                callback(UnityWebRequest);
                return;
            }

            if (_postInfos.ContainsKey(url) == false)
            {
                _postInfos.Add(url, UnityWebRequest.downloadHandler.text);
            }
            else
            {
                if (_postInfos[url].Equals(UnityWebRequest.downloadHandler.text) == true)
                {
                    return;
                }

                _postInfos[url] = UnityWebRequest.downloadHandler.text;
            }


            callback(UnityWebRequest);
        }));
    }

    /// <summary>
    /// 开始上传音频
    /// </summary>
    /// <param name="url"></param>
    /// <param name="b"></param>
    /// <param name="callback"></param>
    public void StartUploadAudio(string url, byte[] b, Action<UnityWebRequest> callback)
    {
        StartCoroutine(HttpHelper.UploadAudio(url, b, callback));
    }

    public void StartGetBanance(Action<string> action)
    {
        StartCoroutine(GetBalance(action));
    }

    private IEnumerator GetBalance(Action<string> action)
    {
        if (!LocalFileManager._Instance._GameData._IsLogin)
        {
            action("0");
            yield break;
        }

        bool isOk = false;
        bool isReceive = false;

        while (true)
        {
            StartPost(@"member/center/getMemberBalance", null, (untiyWebRequest) =>
            {
                if (untiyWebRequest == null)
                {
                    isReceive = true;
                    return;
                }
                JsonData jd = JsonMapper.ToObject(untiyWebRequest.downloadHandler.text);
                if (jd["code"].ToString() == "1")
                {
                    action(jd["result"]["balance"].ToString());
                }
                else
                {
                    TipsManager._Instance.OpenTipsText(jd["msg"].ToString());
                }
                isOk = true;
                isReceive = true;
            });

            while (isReceive == false)
            {
                yield return new WaitForSeconds(0.2f);
            }

            isReceive = true;

            if (isOk)
            {
                break;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    /// <summary>
    /// 重新获取验证码监听
    /// </summary>
    public void GetVerificationCode(string phoneNum, Action<JsonData> action)
    {
        Dictionary<string, string> form = new Dictionary<string, string>();
        form.Add("phone", phoneNum);
        StartPost(@"sso/common/sms", form, (unityWebRequest) =>
         {
             if (unityWebRequest == null)
             {
                 Debug.LogWarning("获取短信验证码错误");
                 return;
             }

             JsonData Temp = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
             JsonData res = DataEncryptDecrypt.EncryptDecrypt.dataDecrypt(Temp.ToJson());

             action(res);
         });
    }

    /// <summary>
    /// 重新获取验证码监听
    /// </summary>
    public void RegisterGetVerificationCode(string phoneNum, Action<JsonData> action)
    {
        Dictionary<string, string> form = new Dictionary<string, string>();
        form.Add("phone", phoneNum);
        StartPost(@"sso/register/sms", form, (unityWebRequest) =>
         {
             if (unityWebRequest == null)
             {
                 Debug.LogWarning("获取短信验证码错误");
                 return;
             }

             JsonData Temp = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
             JsonData res = DataEncryptDecrypt.EncryptDecrypt.dataDecrypt(Temp.ToJson());

             action(res);
         });
    }

    public void GetValidateCode(Image image, Action<string> action)
    {
        StartPost(@"not/common/getValidateCode", null, (unityWebRequest) =>
        {
            if (unityWebRequest == null)
            {
                return;
            }

            JsonData jsonData = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

            if (jsonData["code"].ToString() == "1")
            {
                GameStatic.Base64ToImg(image, jsonData["result"]["data"].ToString());
                action(jsonData["result"]["validateCode"].ToString());
            }
            else
            {
                TipsManager._Instance.OpenTipsText(jsonData["msg"].ToString());
            }
        });
    }

    public void GetLuaText(string str, Action<string> action)
    {
#if UNITY_EDITOR
        action(LocalFileManager._Instance.GetLua());
#else
        StartCoroutine(HttpHelper.GetText(str, action));
#endif

    }
}