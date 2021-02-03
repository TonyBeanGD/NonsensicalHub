using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public class XLuaInstance : MonoBehaviour
{
    private Action luaUpdate;
    private Action luaOnDestroy;

    private LuaTable scriptEnv;

    private LuaEnv luaEnv;

    public void Init(string luaScript)
    {
        luaEnv = XLuaManager.luaEnv;
        scriptEnv = luaEnv.NewTable();

        // 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
        LuaTable meta = luaEnv.NewTable();
        meta.Set("__index", luaEnv.Global);
        scriptEnv.SetMetaTable(meta);
        meta.Dispose();

        scriptEnv.Set("self", this);
        luaEnv.DoString(luaScript, "LuaTestScript", scriptEnv);

        Action luaAwake = scriptEnv.Get<Action>("Init");
        scriptEnv.Get("Update", out luaUpdate);
        scriptEnv.Get("OnDestroy", out luaOnDestroy);

        luaAwake?.Invoke();
    }

    void Update()
    {
        luaUpdate?.Invoke();
    }

    void OnDestroy()
    {
        luaOnDestroy?.Invoke();
        scriptEnv?.Dispose();
    }
}
