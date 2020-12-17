using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ResourcesHelper
{

    public static T Load<T>(string _resourvesPath) where T : UnityEngine.Object
    {
        return Resources.Load<T>(_resourvesPath);
    }

    public static void LoadImage(string _path, Action<Sprite> _action)
    {
        NonsensicalKit.NonsensicalUnityInstance.Instance.StartCoroutine(LoadImageCoroutine(_path, _action));
    }
    

    private static IEnumerator LoadImageCoroutine(string imagePath, Action<Sprite> _action)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, imagePath);
        WWW www = new WWW(filePath);
        yield return www;


        Texture2D tex = www.texture;

        if (tex != null)
        {
            Sprite mySprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            _action(mySprite);
        }
        else
        {
            Debug.LogWarning("加载图片错误：" + www.error);
            _action(null);
        }
    }
}
