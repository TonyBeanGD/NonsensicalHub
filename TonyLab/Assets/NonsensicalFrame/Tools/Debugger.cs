using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Text;

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
