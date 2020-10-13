using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class AssetsBundleManager : MonoBehaviour
{
    public static AssetsBundleManager _Instance;

    class AssetBundleInfo
    {
        public string url;
        public int version;

        public AssetBundle assetBundle;

        public AssetBundleInfo(string _url, int _version)
        {
            url = _url;
            version = _version;
        }
    }

    List<AssetBundleInfo> abs;

    private void InitAssetBundle()
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        string path = Path.Combine("file://" + Application.dataPath + "/Raw/", "AssetBundles/testlocal");
#else
        string path = Path.Combine(Application.streamingAssetsPath, "AssetBundles/testlocal");
#endif
  

        abs = new List<AssetBundleInfo>();
        abs.Add(new AssetBundleInfo(path, 5));
    }

    private void Awake()
    {
        GameStatic.OnInitComplete += () =>
        {
            while (abs.Count > 0)
            {
                abs[0].assetBundle.Unload(false);

                abs.RemoveAt(0);
            }
        };
        _Instance = this;
    }

    void Start()
    {
        InitAssetBundle();
        StartCoroutine(LoadAssetBundle());
    }

    IEnumerator ShowProgress(WWW www)
    {
        while (www.progress < 1)
        {
            Debug.Log("AssetBundle加载进度:"+www.progress);
            yield return null;
        }
        Debug.Log("AssetBundle加载进度:" + 1);
    }

    IEnumerator LoadAssetBundle()
    {
        for (int i = 0; i < abs.Count; i++)
        {
            WWW www = WWW.LoadFromCacheOrDownload(abs[i].url, abs[i].version);
            StartCoroutine(ShowProgress(www));
            yield return www;
            abs[i].assetBundle = www.assetBundle;
        }

        GameStatic.OnLocalAssetBundleLoadComplete();
    }

    public Sprite GetLocalSprite(string name)
    {
        if (abs.Count == 0)
        {
            return null;
        }
        Sprite sprite = abs[0].assetBundle.LoadAsset<Sprite>(name);
        return sprite;
    }
}
