using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace NonsensicalFrame
{
    public static class Debugger
    {
        public static void Log(object obj)
        {
            UnityEngine.Debug.Log(obj.ToString());
        }

        public static void LogMessage(object obj,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
        {
            string content = $" {obj }\r\n{memberName }\r\n({sourceFilePath} :{ sourceLineNumber}";
            UnityEngine.Debug.Log(content);
        }

    }
}
