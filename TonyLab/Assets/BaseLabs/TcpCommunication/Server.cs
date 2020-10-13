using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace TcpCommunication
{
    class Server
    {
        private static Socket server;//套接字
        private static IPEndPoint ipep;//ip地址+端口号
        private static byte[] buff = new byte[1024]; //字节数据，保存接收数据，缓冲空间
        private static Dictionary<string, Socket> userDictionary = new Dictionary<string, Socket>(); //用来存放所有客户端

        static void Main(string[] args)
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            string host = GetLocalIP();
            
            if (host == null)
            {
                Console.WriteLine("请确认你的电脑已连接网络");
                Console.ReadLine();
                return;
            }

            while (true)
            {
                try
                {
                    Console.Write("请输入端口号：");
                    string port = Console.ReadLine();
                    ipep = new IPEndPoint(IPAddress.Parse(host), int.Parse(port));
                    break;
                }
                catch (Exception)
                {
                    Console.WriteLine("请输入正确的端口号");
                }
            }

            server.Bind(ipep);//绑定ip port

            server.Listen(0);//使套接字处于监听状态

            server.BeginAccept(AcceptCallback, null);

            Refresh_Panel();

            Console.WriteLine("服务器创建完毕");

            //保证服务器一直运行
            while (true)
            {
                string cmd = Console.ReadLine();
                switch (cmd)
                {
                    case "exit":
                        server.Shutdown(SocketShutdown.Both);
                        server.Close();
                        return;
                    case "clear":
                        Refresh_Panel();
                        break;
                    case "show":
                        foreach (var item in userDictionary)
                        {
                            Console.WriteLine(item.Value.RemoteEndPoint);
                        }
                        break;
                    default:
                        SendAll("#:SEMG:" + cmd);
                        break;
                }
            }
        }

        /// <summary>
        /// 刷新面板
        /// </summary>
        private static void Refresh_Panel()
        {
            Console.Clear();
            Console.WriteLine("服务器IPv4地址和端口号:" + ipep.ToString());
            Console.WriteLine("当前链接人数:" + userDictionary.Count);
        }

        /// <summary>
        /// 向所有人发送消息
        /// </summary>
        private static void SendAll(string msg)
        {

            byte[] temp = new byte[1024];
            //将字符串转换成byte字节数组
            temp = Encoding.UTF8.GetBytes(msg);

            foreach (var item in userDictionary)
            {
                try
                {
                    item.Value.Send(temp);
                }
                catch (Exception)
                {
                    Console.WriteLine(item.Key + ":断开连接");
                    userDictionary.Remove(item.Key);
                }
            }
        }

        /// <summary>
        /// 当有客户端连接时执行
        /// </summary>
        private static void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = server.EndAccept(ar);
                userDictionary.Add(client.RemoteEndPoint.ToString(), client);
                Console.WriteLine("客户端" + client.RemoteEndPoint + "已连接");

                buff = new byte[1024];

                client.BeginReceive(buff, 0, buff.Length, SocketFlags.None, ReceiveCallback, client);

                server.BeginAccept(AcceptCallback, null);
                Refresh_Panel();
            }
            catch (Exception ex)
            {
                Console.WriteLine("客户端链接时出错" + ex.Message);
            }
        }

        /// <summary>
        /// 当收到客户端的数据执行
        /// </summary>
        private static void ReceiveCallback(IAsyncResult ar)
        {
            int recv;
            string msg = "";

            Socket client = (Socket)ar.AsyncState;
            string key = client.RemoteEndPoint.ToString();

            try
            {
                //接收到字节数
                recv = client.EndReceive(ar);
                if (recv < 1)
                {
                    userDictionary.Remove(client.RemoteEndPoint.ToString());
                    client.Close();
                    Refresh_Panel();
                    Console.WriteLine(key + "已经下线，没有收到字节");
                    return;
                }

                UTF8Encoding encoding = new UTF8Encoding(); //将字节数组转换string
                string temp = encoding.GetString(buff, 0, recv);

                string temp2 = temp.Substring(0, 3);

                if (temp2 == "N#!")
                {
                    if (temp.Substring(3) == "HowMany")
                    {
                        msg = Send_Num();
                    }
                }
                else if (temp2 == "N#:")
                {
                    msg = temp;
                }
                else
                {
                    //待添加：粘包处理
                    buff = new byte[1024];
                    client.BeginReceive(buff, 0, buff.Length, SocketFlags.None, ReceiveCallback, client); //继续接收
                }

                Console.WriteLine(key + ":" + msg);
                SendAll(msg);

                buff = new byte[1024];
                client.BeginReceive(buff, 0, buff.Length, SocketFlags.None, ReceiveCallback, client); //继续接收
            }
            catch (Exception ex)
            {
                Console.WriteLine("服务器接受用户信息时出错:" + ex.Message);
                userDictionary.Remove(key);//把用户从列表中删除
            }
        }

        public static string Send_Num()
        {
            return "N#:HAVE" + userDictionary.Count.ToString();
        }

        /// <summary>  
        /// 获取当前使用的IP   （网上弄的方法1）
        /// </summary>  
        /// <returns></returns>  
        public static string GetLocalIP()
        {
            string result = RunApp("route", "print", true);
            Match m = Regex.Match(result, @"0.0.0.0\s+0.0.0.0\s+(\d+.\d+.\d+.\d+)\s+(\d+.\d+.\d+.\d+)");
            if (m.Success)
            {
                return m.Groups[2].Value;
            }
            else
            {
                try
                {
                    System.Net.Sockets.TcpClient c = new System.Net.Sockets.TcpClient();
                    c.Connect("www.baidu.com", 80);
                    string ip = ((System.Net.IPEndPoint)c.Client.LocalEndPoint).Address.ToString();
                    c.Close();
                    return ip;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>  
        /// 运行一个控制台程序并返回其输出参数   （网上弄的方法2，为上一个方法服务）
        /// </summary>  
        /// <param name="filename">程序名</param>  
        /// <param name="arguments">输入参数</param>  
        /// <returns></returns>  
        public static string RunApp(string filename, string arguments, bool recordLog)
        {
            try
            {
                if (recordLog)
                {
                    Trace.WriteLine(filename + " " + arguments);
                }
                Process proc = new Process();
                proc.StartInfo.FileName = filename;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.Arguments = arguments;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();

                using (System.IO.StreamReader sr = new System.IO.StreamReader(proc.StandardOutput.BaseStream, Encoding.Default))
                {
                    //string txt = sr.ReadToEnd();  
                    //sr.Close();  
                    //if (recordLog)  
                    //{  
                    //    Trace.WriteLine(txt);  
                    //}  
                    //if (!proc.HasExited)  
                    //{  
                    //    proc.Kill();  
                    //}  
                    //上面标记的是原文，下面是我自己调试错误后自行修改的  
                    Thread.Sleep(100);           //貌似调用系统的nslookup还未返回数据或者数据未编码完成，程序就已经跳过直接执行  
                                                 //txt = sr.ReadToEnd()了，导致返回的数据为空，故睡眠令硬件反应  
                    if (!proc.HasExited)         //在无参数调用nslookup后，可以继续输入命令继续操作，如果进程未停止就直接执行  
                    {                            //txt = sr.ReadToEnd()程序就在等待输入，而且又无法输入，直接掐住无法继续运行  
                        proc.Kill();
                    }
                    string txt = sr.ReadToEnd();
                    sr.Close();
                    if (recordLog)
                        Trace.WriteLine(txt);
                    return txt;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return ex.Message;
            }
        }
    }
}
