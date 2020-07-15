using System.Text;
using UnityEngine;

namespace NonsensicalFrame
{
    public class Debugger
    {
        private const bool useDebug = true;

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
