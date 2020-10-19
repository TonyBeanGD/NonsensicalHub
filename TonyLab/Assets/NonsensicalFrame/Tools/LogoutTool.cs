using UnityEngine;
using System;
using System.IO;

/// <summary>
/// Content：日志打印本地文本工具
/// Author：Henry
/// Date：2018/8/1
/// </summary>
public static class LogoutTool
{
    private static string pathStr = "";
    public static void Create(String content,string name=null)
    {
#if UNITY_EDITOR
        string nameStr;
        if (name == null)
        {
            nameStr = System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
        }
        else
        {
            nameStr = name;
        }
        string dirStr = Application.streamingAssetsPath + "//Log//" + DateTime.Now.ToString("yy.MM.dd") + "//";
        if (!Directory.Exists(dirStr))
        {
            Directory.CreateDirectory(dirStr);
        }
        pathStr = dirStr + nameStr + ".txt";
        FileStream fs = new FileStream(pathStr, FileMode.Append, FileAccess.Write, FileShare.Write);
        StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
        sw.WriteLine(content);
        sw.WriteLine();
        sw.Flush();
        sw.Close();
#endif

    }

    public static void Log(string content)
    {
#if UNITY_EDITOR
        try
        {
            FileStream fs = new FileStream(pathStr, FileMode.Append, FileAccess.Write, FileShare.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            string logTime = "LogTime:" + System.DateTime.Now.ToString();
            sw.WriteLine(logTime);
            sw.WriteLine("Log:" + content);
            sw.Flush();
            sw.Close();
        }
        catch (Exception ex)
        {
            Debug.Log("IO异常:" + ex.Message);
            return;
        }
#endif
    }

}
