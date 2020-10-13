using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NonsensicalServerLib;

namespace NonsensicalServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ExampleServer server = new ExampleServer("0.0.0.0", 4050);
            server.SetRoot(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"HttpTest"));
            server.SetLogger(new ConsoleLogger());
            server.Start();
        }
    }
}
