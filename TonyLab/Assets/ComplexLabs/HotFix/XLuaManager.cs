using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using XLua;
using LitJson;
using System.IO;
using System.Text;
using NonsensicalKit;

public class XLuaManager : MonoBehaviour
{
    public static XLuaManager _Instance;

    internal static LuaEnv luaEnv = new LuaEnv(); //all lua behaviour shared one luaenv only!
    internal static float lastGCTime = 0;
    internal const float GCInterval = 1;//1 second 

    private string LoaclLuaDic;

    private void Awake()
    {
        LoaclLuaDic = Path.Combine(Application.streamingAssetsPath, "Xlua", "LuaText");
        _Instance = this;
        StartCoroutine(CheckConfig());
    }

    private void Update()
    {
        if (Time.time - lastGCTime > GCInterval)
        {
            luaEnv.Tick();
            lastGCTime = Time.time;
        }
    }

    public void CreateLua(string luaText)
    {
        GameObject gp = new GameObject("lua instance");
        XLuaInstance li = gp.AddComponent<XLuaInstance>();
        li.Init(luaText);
    }

    private IEnumerator CheckConfig()
    {
        LuaConfig localLuaConfig = null;
        LuaConfig serverLuaConfig = null;
        int loadCount = 0;
        string localConfig = string.Empty;
        string serverConfig = string.Empty;
        StartCoroutine(HttpHelper.GetText(Path.Combine(Application.streamingAssetsPath, "XLua", "XLuaLocalConfig.json"), (text) => { localConfig = text; loadCount++; }, true));
        StartCoroutine(HttpHelper.GetText(Path.Combine(Application.streamingAssetsPath, "XLua", "XLuaSimulateServerConfig.json"), (text) => { serverConfig = text; loadCount++; }, false));

        while (loadCount < 2)
        {
            yield return null;
        }

        if (serverConfig != null)
        {
            serverLuaConfig = JsonMapper.ToObject<LuaConfig>(serverConfig);
        }

        if (serverLuaConfig == null)
        {
            Debug.LogWarning("服务器lua配置文件出错");
            yield break;
        }

        if (localConfig == null)
        {
            localLuaConfig = serverLuaConfig.Clone();
        }
        else
        {
            localLuaConfig = JsonMapper.ToObject<LuaConfig>(localConfig);
            if (localLuaConfig == null)
            {
                localLuaConfig = serverLuaConfig.Clone();
            }
        }

        localLuaConfig.Update(serverLuaConfig, LoaclLuaDic);
        localLuaConfig.scriptList.Sort();
        StartCoroutine(LoadXlua(localLuaConfig));

        FileHelper.WriteTxt(Path.Combine(Application.streamingAssetsPath, "XLua", "XLuaLocalConfig.json"), JsonMapper.ToJson(localLuaConfig));
    }

    private IEnumerator LoadXlua(LuaConfig luaConfig)
    {
        foreach (var item in luaConfig.scriptList)
        {
            string[] temp = item.path.Split(new char[] { '/', '\\' });
            string name = temp[temp.Length - 1];
            if (File.Exists(Path.Combine(LoaclLuaDic, name)) == true)
            {
                StartCoroutine(HttpHelper.GetText(Path.Combine(LoaclLuaDic, name), (text) =>
                {
                    item.luaText = text;
                    item.loadComlete = true;
                }, true));
            }
            else
            {
                StartCoroutine(HttpHelper.GetText(item.path, (text) =>
                {
                    FileHelper.WriteTxt(Path.Combine(LoaclLuaDic, name), text);
                    item.luaText = text;
                    item.loadComlete = true;
                }, true));
            }
        }

        while (true)
        {
            yield return null;

            bool allComplete = true;

            foreach (var item in luaConfig.scriptList)
            {
                if (item.loadComlete == false)
                {
                    allComplete = false;
                }
            }

            if (allComplete)
            {
                break;
            }
        }

        foreach (var item in luaConfig.scriptList)
        {
            CreateLua(item.luaText);
        }
    }

    private class LuaConfig
    {
        public List<LuaConfigElement> scriptList;

        public LuaConfig()
        {
            scriptList = new List<LuaConfigElement>();
        }

        private LuaConfigElement GetElementByPath(string _path)
        {
            foreach (var item in scriptList)
            {
                if (item.path.Equals(_path))
                {
                    return item;
                }
            }
            return null;
        }

        public LuaConfig Clone()
        {
            LuaConfig copy = new LuaConfig();

            List<LuaConfigElement> copyScriptList = new List<LuaConfigElement>();

            foreach (var item in scriptList)
            {
                copyScriptList.Add(item.Clone());
            }

            return copy;
        }

        public void Update(LuaConfig target, string dicPath)
        {
            //删除更新后不存在或者有新版本的对象
            for (int i = 0; i < scriptList.Count; i++)
            {
                string[] temp = scriptList[i].path.Split(new char[] { '/', '\\' });
                string name = temp[temp.Length - 1];

                if (target.GetElementByPath(scriptList[i].path) == null)
                {
                    NonsensicalKit.FileHelper.DeleteFile(Path.Combine(dicPath, name));
                    scriptList.RemoveAt(i);
                    i--;
                }
                else
                {
                    if (target.GetElementByPath(scriptList[i].path).version > scriptList[i].version)
                    {
                        NonsensicalKit.FileHelper.DeleteFile(Path.Combine(dicPath, name));
                        scriptList.RemoveAt(i);
                        i--;
                    }
                }
            }
            //添加需要更新的对象并更新仍然存在的对象
            for (int i = 0; i < target.scriptList.Count; i++)
            {
                if (this.GetElementByPath(target.scriptList[i].path) == null)
                {
                    this.scriptList.Add(target.scriptList[i].Clone());
                }
                else
                {
                    LuaConfigElement crtElement = this.GetElementByPath(target.scriptList[i].path);
                    crtElement.order = target.scriptList[i].order;
                }
            }
        }

        public class LuaConfigElement : IComparable<LuaConfigElement>
        {
            public string path;
            public int version;
            public int order;

            [NonSerialized]
            public string luaText;
            [NonSerialized]
            public bool loadComlete;

            public LuaConfigElement()
            {
                loadComlete = false;
            }

            public LuaConfigElement Clone()
            {
                LuaConfigElement copy = new LuaConfigElement
                {
                    path = this.path,
                    version = this.version,
                    order = this.order,
                    luaText = this.luaText,
                    loadComlete = this.loadComlete,
                };

                return copy;
            }

            public int CompareTo(LuaConfigElement obj)
            {
                return obj.order.CompareTo(order);
            }

            public override string ToString()
            {
                return $"path:{path},luaText:{luaText},order:{order}";
            }
        }
    }
}
