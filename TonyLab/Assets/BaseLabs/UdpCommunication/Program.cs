using System;

namespace UdpCommunication
{
    class Program
    {
        static void Main(string[] args)
        {
            if(Console.ReadLine()=="1")
            {
                Sever s = new Sever();
                s.Run();
            }
            else
            {
                Client c = new Client();
                c.Run();
            }
        }
    }
}
