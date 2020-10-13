using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace NonsensicalFrame
{
    class LogMessage
    {
        private string message;
        private string stackTrace;
        private LogType logType;
        private DateTime dateTime;

        public LogMessage(string _message, string _staclTrace, LogType _logType, DateTime _dateTime)
        {
            message = _message;
            stackTrace = _staclTrace;
            logType = _logType;
            dateTime = _dateTime;
        }

        public bool CheckType(LogType _logType)
        {
            return logType == _logType;
        }

        public override string ToString()
        {
            return dateTime + " , " + logType.ToString() + "\r\n    " + message + "\r\n    " + stackTrace;
        }
    }

    public class Debugger : MonoSingleton<Debugger>
    {
        private const bool useDebug = true;

        private Queue<LogMessage> logMessages;

        protected override void Awake()
        {
            base.Awake();

            logMessages = new Queue<LogMessage>();

            Application.logMessageReceived += HandleLog;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.RightControl) && Input.GetKeyDown(KeyCode.S))
            {
                string path = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), System.DateTime.Now.ToString("yyyyMMdd") + "UnityLog.txt");
                FileHelper.CreateAndWrite(path, GetAllLog());
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.RightControl) && Input.GetKeyDown(KeyCode.L))
            {
                string path = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), System.DateTime.Now.ToString("yyyyMMdd") + "UnityLog.txt");
                FileHelper.CreateAndWrite(path, GetLog (LogType.Log));
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.RightControl) && Input.GetKeyDown(KeyCode.A))
            {
                string path = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), System.DateTime.Now.ToString("yyyyMMdd") + "UnityLog.txt");
                FileHelper.CreateAndWrite(path, GetLog(LogType.Assert));
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.RightControl) && Input.GetKeyDown(KeyCode.E))
            {
                string path = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), System.DateTime.Now.ToString("yyyyMMdd") + "UnityLog.txt");
                FileHelper.CreateAndWrite(path, GetLog(LogType.Error));
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.RightControl) && Input.GetKeyDown(KeyCode.B))
            {
                string path = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), System.DateTime.Now.ToString("yyyyMMdd") + "UnityLog.txt");
                FileHelper.CreateAndWrite(path, GetLog(LogType.Exception));
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.RightControl) && Input.GetKeyDown(KeyCode.L))
            {
                string path = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), System.DateTime.Now.ToString("yyyyMMdd") + "UnityLog.txt");
                FileHelper.CreateAndWrite(path, GetLog(LogType.Warning));
            }
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void HandleLog(string message, string stackTrace, LogType type)
        {
            logMessages.Enqueue(new LogMessage(message, stackTrace, type, DateTime.Now));
        }

        private string GetAllLog()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in logMessages)
            {
                sb.Append(item.ToString());
                sb.Append("\r\n\r\n");
            }
            return sb.ToString();
        }

        private string GetLog(LogType _logType)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in logMessages)
            {
                if (item.CheckType(_logType))
                {
                    sb.Append(item.ToString());
                    sb.Append("\r\n\r\n");
                }
            }
            return sb.ToString();
        }

        private string GetString(object[] _objs)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in _objs)
            {
                sb.Append('|');
                sb.Append(item.ToString());
            }

            return sb.ToString();
        }

        public void Log(params object[] _objs)
        {
            if (useDebug)
            {
                Debug.Log(GetString(_objs));
            }
        }

        public void LogWarning(params object[] _objs)
        {
            if (useDebug)
            {
                Debug.LogWarning(GetString(_objs));
            }
        }

        public void LogError(params object[] _objs)
        {
            if (useDebug)
            {
                Debug.LogError(GetString(_objs));
            }
        }
    }
}
