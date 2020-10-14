using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager _Instance;

    public Image[] logoTargets;
    public Text WebsiteUrl;

    private Transform _LoadingPanel;

    private Transform _MaintenancePanel;
    private Transform _LowVersionPanel;

    private int _LoadingCompleteMaxCount;
    private static int _LoadingCompleteCrtCount;

    private Transform _ProgressBar;

    private float _CurrentProgress;
    private float _TargetProgress;

    public static void LoadingStart(string form)
    {
        //Debug.Log($"{form}Start");
    }

    public static void LoadingComplete(string form)
    {
        //Debug.Log($"{form}Complete");
        _LoadingCompleteCrtCount++;
    }
    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        GameStatic.OnLocalAssetBundleLoadComplete += () => { StartCoroutine(GameInit()); };
        _CurrentProgress = 0;
        _TargetProgress = 0;
        _LoadingPanel = transform;
        _ProgressBar = _LoadingPanel.transform.Find("ProgressBar");
        _MaintenancePanel = _LoadingPanel.Find("panel_Maintenance");
        _MaintenancePanel.gameObject.SetActive(false);
        _LowVersionPanel = _LoadingPanel.Find("panel_LowVersion");
        _LowVersionPanel.gameObject.SetActive(false);
        _Instance = this;
    }

    void Start()
    {
#if LOCAL
        _LoadingPanel.GetComponent<Image>().sprite = InitialResourcesManager.img_TestLoading;

#elif BENCHI
        
        _LoadingPanel.GetComponent<Image>().sprite = InitialResourcesManager.img_BenChiLoading;
#elif LESHI

        _LoadingPanel.GetComponent<Image>().sprite = InitialResourcesManager.img_LeShiLoding;
#endif
    }

    public void Maintenance()
    {
        _LoadingPanel.gameObject.SetActive(true);
        _MaintenancePanel.gameObject.SetActive(true);
    }

    public void LowVersion()
    {
        _LoadingPanel.gameObject.SetActive(true);
        _LowVersionPanel.gameObject.SetActive(true);
        Application.OpenURL(GameStatic.appInfo["url"].ToString());
    }

    private int VersionComparison(string version1, string version2)
    {
        string[] version1String = version1.Split(new char[] { '.' });
        string[] version2String = version2.Split(new char[] { '.' });

        for (int i = 0; i < version1String.Length; i++)
        {
            if (version1String[i].CompareTo(version2String[i]) != 0)
            {
                return version1String[i].CompareTo(version2String[i]);
            }
        }

        return 0;
    }

    private IEnumerator GameInit()
    {
        bool ok = false;
        bool reciver = false;
        bool isMaintenance = false;
        bool isLowVersion = false;

        while (ok == false)
        {
            HttpManager._Instance.StartPost(@"not/common/getAppInfo", null, (unityWebRequest) =>
            {
                if (unityWebRequest == null)
                {
                    reciver = true;
                    return;
                }

                Debug.Log(unityWebRequest.downloadHandler.text);
                JsonData jsonData = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

                if (jsonData["code"].ToString() == "1")
                {
                    if (jsonData["result"]["state"].ToString() == "0")
                    {
                        reciver = true;
                        isMaintenance = true;
                        return;
                    }

                    GameStatic.appInfo = JsonMapper.ToObject(jsonData["result"].ToJson());
                    WebsiteUrl.text = GameStatic.appInfo["url"].ToString();
                    DynamicResourceManager._Instance.StartSetTexture(logoTargets, GameStatic.appInfo["logo"].ToString());

                    if (VersionComparison(GameStatic.appInfo["version"].ToString(), Application.version) > 0)
                    {
                        isLowVersion = true;
                    }

                    if (GameStatic.appInfo["lua"].ToString().Equals(string.Empty) == false)
                    {
                        HttpManager._Instance.GetLuaText(GameStatic.appInfo["lua"].ToString(), (str) =>
                        {
                            LuaManager._Instance.CreateLua(str);
                        });
                    }
                }
                else
                {
                    Debug.LogError("获取App信息失败");
                }

                ok = true;
                reciver = true;
            });
            while (reciver == false)
            {
                yield return new WaitForSeconds(1);
            }
            if (isMaintenance)
            {
                Maintenance();

                yield break;
            }
            if (isLowVersion)
            {
                LowVersion();
                yield break;
            }

            reciver = false;
        }
        _TargetProgress = 0.2f;


        _LoadingCompleteMaxCount = GameStatic.OnMainPanelInit.GetInvocationList().Length;
        _LoadingCompleteCrtCount = 0;
        float proportion = 0.8f / _LoadingCompleteMaxCount;
        GameStatic.OnMainPanelInit();

        while (_LoadingCompleteMaxCount > _LoadingCompleteCrtCount)
        {
            _TargetProgress = 0.2f + proportion * _LoadingCompleteCrtCount;
            yield return new WaitForSeconds(1);
        }
        GameStatic.OnChangePanel?.Invoke(Panel.MainMenu);
        _TargetProgress = 1;
        yield return new WaitForSeconds(2);
        _LoadingPanel.gameObject.SetActive(false);
        GameStatic.OnInitComplete?.Invoke();
        AudioSourceManager._Instance.RandomCharaterSpeech();
    }

    void Update()
    {
        _CurrentProgress = 0.9f * _CurrentProgress + 0.1f * _TargetProgress;
        _ProgressBar.GetChild(0).GetComponent<Image>().fillAmount = _CurrentProgress;
        if (_CurrentProgress > 0.99f)
        {
            _CurrentProgress = 1;
        }
    }
}
