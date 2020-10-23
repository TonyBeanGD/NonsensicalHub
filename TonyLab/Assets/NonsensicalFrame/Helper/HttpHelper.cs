using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using LitJson;
using UnityEngine;
using System.IO;

namespace NonsensicalFrame
{
    public static class HttpHelper
    {
        /// <summary>
        /// post请求协程
        /// </summary>
        /// <param name="url"></param>
        /// <param name="dss"></param>
        /// <param name="header"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerator Post(string url, Dictionary<string, string> dss, Dictionary<string, string> header, Action<UnityWebRequest> callback)
        {
            if (dss == null)
            {
                Debug.LogWarning("Post请求内容为空");
            }

            UnityWebRequest unityWebRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST)
            {
                downloadHandler = new DownloadHandlerBuffer(),
                useHttpContinue = false                            //当其值为true且后端未设置100-continue的回调时，会无法得到返回的回调
            };

            WWWForm form = new WWWForm();
            foreach (var item in dss)
            {
                form.AddField(item.Key, item.Value);
            }
            unityWebRequest = UnityWebRequest.Post(url, form);

            if (header != null)
            {
                foreach (var tmp in header)
                {
                    unityWebRequest.SetRequestHeader(tmp.Key, tmp.Value);
                }
            }


            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.isHttpError || unityWebRequest.isNetworkError)
            {
                Debug.LogWarning(url + "Post请求出错" + unityWebRequest.error + "|" + unityWebRequest.downloadHandler.text);
            }
            else
            {
                callback(unityWebRequest);
            }
            unityWebRequest.Dispose();
        }

        /// <summary>
        /// 请求图片协程
        /// </summary>
        /// <param name="url">图片地址,like 'https://image.demo.test.doulang9.com/logo.png'</param>
        /// <param name="action">请求发起后处理回调结果的委托,处理请求结果的图片</param>
        /// <returns></returns>
        public static IEnumerator GetTexture(string url, Action<Texture2D> actionResult)
        {
            UnityWebRequest unityWebRequest = new UnityWebRequest(url)
            {
                timeout = 300
            };
            DownloadHandlerTexture downloadTexture = new DownloadHandlerTexture(true);
            unityWebRequest.downloadHandler = downloadTexture;

            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
            {
                Debug.LogWarning(url + "请求图片出错" + unityWebRequest.error + "|" + unityWebRequest.downloadHandler.text);
            }
            else
            {
                actionResult?.Invoke(downloadTexture.texture);
            }
            unityWebRequest.Dispose();
        }

        /// <summary>
        /// 请求文本
        /// </summary>
        /// <param name="url"></param>
        /// <param name="actionResult"></param>
        /// <param name="removeBOM"></param>
        /// <returns></returns>
        public static IEnumerator GetText(string url, Action<string> actionResult,bool removeBOM=false)
        {
            UnityWebRequest unityWebRequest = new UnityWebRequest(url)
            {
                downloadHandler = new DownloadHandlerBuffer()
            };
            
            yield return unityWebRequest.SendWebRequest();
            
            if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
            {
                Debug.LogWarning(url + " 请求文本出错 " + unityWebRequest.error + "|" + unityWebRequest.downloadHandler.text);
                actionResult?.Invoke(null);
            }
            else
            {
                string text = unityWebRequest.downloadHandler.text;
                if (removeBOM)
                {
                    text = text.Substring(1);
                }
                actionResult?.Invoke(text);
            }
            unityWebRequest.Dispose();
        }

        /// <summary>
        /// 上传音频
        /// </summary>
        /// <param name="url"></param>
        /// <param name="b"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerator UploadAudio(string url, byte[] b, Dictionary<string, string> header, Action<UnityWebRequest> callback)
        {
            WWWForm form = new WWWForm();
            form.AddBinaryData("file", b, "audio.wav", "audio/wav");

            UnityWebRequest unityWebRequest = UnityWebRequest.Post(url, form);
            unityWebRequest.useHttpContinue = false;
            if (header != null)
            {
                foreach (var tmp in header)
                {
                    unityWebRequest.SetRequestHeader(tmp.Key, tmp.Value);
                }
            }


            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
            {
                Debug.LogWarning(url + "上传音频出错" + unityWebRequest.error + "|" + unityWebRequest.downloadHandler.text);
                callback?.Invoke(null);
            }
            else
            {
                callback?.Invoke(unityWebRequest);
                unityWebRequest.Dispose();
            }
        }
    }
}
