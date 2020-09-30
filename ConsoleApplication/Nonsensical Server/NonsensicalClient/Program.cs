using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NonsensicalClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("http://localhost:4050/");
                request.Method = "POST";
                var requestStream = request.GetRequestStream();
                var data = Encoding.UTF8.GetBytes("这是一条post信息");
                requestStream.Write(data, 0, data.Length);
                var response = request.GetResponse();

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();
                    Console.WriteLine(content);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"发生错误:{e.Message}:{e.Data}");
            }
            
            Console.ReadKey();
        }
    }
}
