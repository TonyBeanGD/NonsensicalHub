using System;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Reco4life
{
    class Program
    {

        static void Main(string[] args)
        {
            string host = null;
            
            // UDP广播发送端口   
            UdpClient udpClient = new UdpClient(48899);
            try
            {
                udpClient.Connect(IPAddress.Parse("192.168.0.255"), 48899);//在192.168.0.255网络内广播从48899端口发出
                Byte[] sendBytes = Encoding.ASCII.GetBytes("YZ-RECOSCAN");
                Console.WriteLine("发送指令");
                udpClient.Send(sendBytes, sendBytes.Length);
                udpClient.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }           
            // UDP广播接收端口
            UdpClient udpClient_0 = new UdpClient(48899);//监控48899端口
            try
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                Byte[] receiveBytes = udpClient_0.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.ASCII.GetString(receiveBytes);

                Console.WriteLine("This is the message you received " +
                                             returnData.ToString());
                host = returnData.Substring(0,13);
                //Console.WriteLine(host);
                udpClient_0.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
            try
            {
              
                int port = 8899;
                
                string zhiling_0 = "AT+YZSWITCH=1,OFF,201801010101";
                IPAddress ip = IPAddress.Parse(host);
                IPEndPoint ipe = new IPEndPoint(ip, port);//把ip和端口转化为IPEndPoint实例
                Socket c = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建一个Socket
                Console.WriteLine("连接中");
                c.Connect(ipe);//连接到服务器
                for (int i=1; i < 100; i++)
                {
                    Console.WriteLine("请输入0-开启指令；1-关闭指令");
                    string zhiling = Console.ReadLine();
                    if (zhiling == "0")
                    {
                        zhiling_0 = "AT+YZSWITCH=1,ON,201801010101";
                    }
                    else if (zhiling == "1")
                    {
                        zhiling_0 = "AT+YZSWITCH=1,OFF,201801010101";
                    }
                    byte[] bs = Encoding.ASCII.GetBytes(zhiling_0);
                    Console.WriteLine("发送指令");
                    c.Send(bs, bs.Length, 0);//发送指令
                    string recvStr = "";
                    byte[] recvBytes = new byte[1024];
                    int bytes;
                    bytes = c.Receive(recvBytes, recvBytes.Length, 0);//从服务器端接受返回信息
                    recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);
                    Console.WriteLine("服务器返回信息：{0}", recvStr);//显示服务器返回信息                                       
                }
                c.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException:{0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException:{0}", e);
            }
            Console.WriteLine("press enter to exit");
            Console.ReadLine();
        }
    }
}