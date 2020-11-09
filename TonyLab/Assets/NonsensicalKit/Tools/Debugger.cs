using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using System.Text;
using System.IO;
using System;

namespace NonsensicalKit
{
    public static class Debugger
    {
        public static Queue<string> messages=new Queue<string>();

        public static void Log(params object[] obj)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in obj)
            {
                if (item.GetType()==typeof(Vector3))
                {
                    Vector3 vector3 = (Vector3)item;
                    sb.Append($"({vector3.x},{vector3.y},{vector3.z})");
                }
                else
                {
                    sb.Append(item.ToString());
                }
                sb.Append(" ; ");
            }

            UnityEngine.Debug.Log(sb.ToString());
        }

        public static void LogToFile(string content, string name = null)
        {
            name = name ?? System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")+".txt";

            Debug.Log(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NonsensicalHub", "Log");

            FileHelper.FileAppendWrite(path, name, content);
        }

        public static void LogWithInfo(object obj,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
        {
            string content = $" {obj }\r\n{memberName }\r\n({sourceFilePath} :{ sourceLineNumber}";
            UnityEngine.Debug.Log(content);
        }
    }
}
