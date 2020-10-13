using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UdpCommunication
{
    class Sever
    {
        public void Run()
        {
            int recv;
            byte[] data = new byte[1024];
            //得到本机IP，设置端口号
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 8001);
            Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //绑定网络地址
            newsock.Bind(ipep);
            Console.WriteLine("This is a Server, host name is {0}", Dns.GetHostName());
            //等待客户机连接
            Console.WriteLine("Waiting for a client");
            //得到客户端IP
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)(sender);
            //接受数据
            recv = newsock.ReceiveFrom(data, ref Remote);
            Console.WriteLine("Message received from {0}", Remote.ToString());
            Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));
            //客户端连接成功后，发送信息
            string welcome = "Hi I am a Server";
            data = Encoding.ASCII.GetBytes(welcome);
            //发送信息
            newsock.SendTo(data, data.Length, SocketFlags.None, Remote);
            while (true)
            {
                data = new byte[1024];
                //发送接受的信息
                recv = newsock.ReceiveFrom(data, ref Remote);
                Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));
                newsock.SendTo(data, recv, SocketFlags.None, Remote);
            }
        }


    }
}
