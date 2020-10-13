using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonsensicalServerLib
{
    class DebugHelper
    {
        private static DebugHelper instance;
        public static DebugHelper Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }
                else
                {
                    instance = new DebugHelper();
                    return instance;
                }
            }
        }

        public ILogger Logger;

        public void Log(object message)
        {
            Logger.Log(message);
        }
    }
}
