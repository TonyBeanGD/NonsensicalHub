using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NonsensicalServerLib;

namespace NonsensicalServer
{
    public class ConsoleLogger:ILogger
    {
        public void Log(object message)
        {
            Console.WriteLine(message);
        }
    }
}
