using System;
using System.Runtime.InteropServices;

public static class Input4WebGL {

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
	public static extern void InputShow (string GameObjectName,string v,int posStart,int posEnd);
    [DllImport("__Internal")]
	public static extern void InputEnd ();
#else
    public static void InputShow(string GameObjectName,string v,int posStart,int posEnd) { }
    public static void InputEnd() { }
#endif
}
