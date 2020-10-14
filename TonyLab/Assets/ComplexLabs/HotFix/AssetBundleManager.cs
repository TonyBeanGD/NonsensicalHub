using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using NonsensicalFrame;

public class AssetBundleManager : MonoBehaviour
{
    //当前正在加载中的对象
    private Dictionary<string, Action<AssetBundleCreateRequest>> crtLoad = new Dictionary<string, Action<AssetBundleCreateRequest>>();
    //所有加载的模型ab包
    private Dictionary<string, AbMessage> allModelABDic = new Dictionary<string, AbMessage>();

    private void Awake()
    {
        NotificationCenter.Instance.AttachObsever(EventName.LoadModelFormAB, LoadABModel);
        NotificationCenter.Instance.AttachObsever(EventName.ResetAB, ResetAB);
    }

    private void OnDestroy()
    {
        NotificationCenter.Instance.DetachObsever(EventName.LoadModelFormAB, LoadABModel);
        NotificationCenter.Instance.DetachObsever(EventName.ResetAB, ResetAB);
    }

    public void LoadABModel(object obj)
    {
        Tuple<string, string, Action<GameObject>> temp = obj as Tuple<string, string, Action<GameObject>>;

        string modelName = temp.Item1.ToLower();
        string path = temp.Item2.ToLower();
        Action<GameObject> back = temp.Item3;

        ABInfo crtInfo = new ABInfo(modelName, path, back);

        if (!allModelABDic.ContainsKey(path))
        {
            //如果字典不包含ab包。协程加载ab包后生成模型
            if (crtLoad.Keys.Contains(path) == true)
            {
                crtLoad[path] += (abcr) => { LoadABRequest(abcr, crtInfo); };
            }
            else
            {
                crtLoad.Add(path, new Action<AssetBundleCreateRequest>((abcr) => { LoadABRequest(abcr, crtInfo); }));

                StartCoroutine(LoadModelAync(path));
            }
        }
        else
        {
            StartCoroutine(LoadFromABDic(crtInfo));
        }
    }

    //协程加载ab
    private IEnumerator LoadModelAync(string modelPath)
    {
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(modelPath);

        yield return request;

        crtLoad[modelPath](request);
        crtLoad.Remove(modelPath);
    }

    private void LoadABRequest(AssetBundleCreateRequest request, ABInfo abInfo)
    {
        if (request.assetBundle != null)
        {
            if (allModelABDic.Keys.Contains(abInfo.path) == false)
            {
                AbMessage abMsg = new AbMessage(request.assetBundle);
                allModelABDic.Add(abInfo.path, abMsg);
            }
            StartCoroutine(LoadFromABDic(abInfo));
        }
        else
        {
            Debug.LogError("未加载到AB包");
        }
    }

    //从ab包字典加载模型
    private IEnumerator LoadFromABDic(ABInfo abInfo)
    {
        allModelABDic[abInfo.path].loadCount++;
   
        AssetBundleRequest abr = allModelABDic[abInfo.path].assetBundle.LoadAssetAsync<GameObject>(abInfo.modelName);

        yield return abr;

        if (abr.asset != null)
        {
            GameObject newGameObject = (GameObject)abr.asset;

            newGameObject = Instantiate(newGameObject);

            abInfo.action(newGameObject);
        }
        else
        {
            Debug.LogError("未加载到对象");
        }
    }

    private void ResetAB(object obj)
    {
        bool unloadAllObjects = (bool)obj;
        
        AssetBundle.UnloadAllAssetBundles(unloadAllObjects);
        crtLoad.Clear();
        allModelABDic.Clear();
    }
}

/// <summary>
/// 记录ab包被需求的次数
/// </summary>
public class AbMessage
{
    public AssetBundle assetBundle;//ab包
    public int loadCount = 0;      //加载的次数
    public AbMessage(AssetBundle assetBundle, int loadCount = 0)
    {
        this.assetBundle = assetBundle;
    }
}

/// <summary>
/// 加载ab包时所用到的信息
/// </summary>
public class ABInfo
{
    public string modelName;
    public string path;
    public Action<GameObject> action;

    public ABInfo(string modelName, string path, Action<GameObject> action)
    {
        this.modelName = modelName;
        this.path = path;
        this.action = action;
    }
}

