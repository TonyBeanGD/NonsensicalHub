using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace NonsensicalFrame
{
    public class Debugger:MonoSingleton<Debugger>
    {
        private const bool useDebug = true;
        private string LogBuffer = string.Empty;

        protected override void Awake()
        {
            base.Awake();
            Application.logMessageReceived += HandleLog;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.RightControl) && Input.GetKeyDown(KeyCode.S))
            {
                string path = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), System.DateTime.Now.ToString("yyyyMMdd") + "UnityLog.txt");
                FileStream fs = new FileStream(path, FileMode.Create);
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(LogBuffer);
                fs.Write(buffer, 0, buffer.Length);
                fs.Close();
            }
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void HandleLog(string message, string stackTrace, LogType type)
        {
            LogBuffer += System.DateTime.Now + "\r\n    " + message + "\r\n    " + stackTrace + "\r\n\r\n";
        }

        public void Log(params object[] objs)
        {
            if (useDebug)
            {
                Debug.Log(GetString(objs));
            }
        }

        public void LogWarning(params object[] objs)
        {
            if (useDebug)
            {
                Debug.LogWarning(GetString(objs));
            }
        }

        public void LogError(params object[] objs)
        {
            if (useDebug)
            {
                Debug.LogError(GetString(objs));
            }
        }

        private string GetString(object[] objs)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in objs)
            {
                sb.Append('|');
                sb.Append(item.ToString());
            }

            return sb.ToString();
        }
    }
}
