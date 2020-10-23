using UnityEngine;
using System;
using System.IO;

namespace NonsensicalFrame
{
    public static class LogoutTool
    {
        public static void Logout(string content, string name = null)
        {
            name = name ?? System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NonsensicalHub","Log", DateTime.Now.ToLongDateString());

            FileHelper.FileAppendWrite(path, name, content);
        }
    }
}
