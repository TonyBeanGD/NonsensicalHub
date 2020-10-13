using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace TcpCommunication
{
    public enum State
    {
        Start,
        Server,
        Client,
    }

    class Program
    {
        private static State Current_State;//当前状态枚举

        private static FileStream fs;//文件流
        private static StreamWriter sw;//写入流

        private static Socket socket;//套接字
        private static IPAddress ipa;//ip地址
        private static IPEndPoint ipep;//ip地址+端口号

        private static byte[] buff = new byte[1024]; //字节数据，保存接收数据，缓冲空间

        private static Dictionary<string, Socket> userDictionary = new Dictionary<string, Socket>(); //用来存放所有客户端

        static void Main(string[] args)
        {
            fs = new FileStream(@"Debug.txt", FileMode.Append, FileAccess.Write);
            sw = new StreamWriter(fs, Encoding.UTF8);

            while (true)
            {
                Init();//将初始化放于while循环中，确保程序在循环中使用不同功能时不会出错

                Refresh_Panel();

                if (Show_Function() == 1)
                {
                    break;
                }

                switch (Current_State)
                {
                    case State.Start:
                        break;
                    case State.Server:
                        Server();
                        break;
                    case State.Client:
                        Client();
                        break;
                    default:
                        sw.WriteLine(DateTime.Now.ToString() + "未知状态枚举:" + Current_State.ToString());
                        break;
                }
            }

            //关闭套接字和数据流
            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception)
            {

            }
            socket.Close();
            sw.Close();
            fs.Close();
        }

        /// <summary>
        /// 数据初始化
        /// </summary>
        private static void Init()
        {
            Current_State = State.Start;
            ipep = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 0);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// 展示功能,并根据输入的功能序号做出相应反应
        /// </summary>
        private static int Show_Function()
        {
            while (true)
            {
                Refresh_Panel();

                Console.WriteLine("1.服务器");
                Console.WriteLine("2.客户端");
                Console.WriteLine("3.阅读Debug日志");
                Console.WriteLine("4.退出程序");
                Console.Write("请输入要使用的功能序号：");

                switch (Console.ReadLine())
                {
                    case "1":
                        Current_State = State.Server;
                        return 0;
                    case "2":
                        Current_State = State.Client;
                        return 0;
                    case "3":
                        if (File.Exists(@"Debug.txt"))
                        {
                            Console.WriteLine("暂无debug内容");
                            Console.ReadLine();
                            return 0;
                        }
                        StreamReader sr = new StreamReader(@"Debug.txt", Encoding.UTF8);
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            Console.WriteLine(line.ToString());
                        }
                        return 0;
                    case "4":
                        return 1;
                    default:
                        Console.WriteLine("请输入正确的功能序号");
                        sw.WriteLine("what the fuck");
                        Console.ReadLine();
                        break;
                }
            }
        }

        /// <summary>
        /// 刷新面板，根据当前状态显示顶端信息
        /// </summary>
        private static void Refresh_Panel()
        {
            Console.Clear();
            switch (Current_State)
            {
                case State.Start:
                    {
                        Console.WriteLine("欢迎使用Tony's Lab LAN Connection");
                    }
                    break;
                case State.Server:
                    {
                        if (ipep != null)
                        {
                            Console.WriteLine("当前正在使用服务器功能,ip地址和端口号:" + ipep.ToString());
                        }
                    }
                    break;
                case State.Client:
                    {
                        if (ipep != null)
                        {
                            Console.WriteLine("当前正在使用客户端功能,ip地址和端口号:" + ipep.ToString());
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 服务器功能
        /// </summary>
        private static void Server()
        {
            string host = GetLocalIP();

            if (host == null)
            {
                Console.WriteLine("请确保你的电脑已连接网络");
                return;
            }

            Refresh_Panel();
            while (true)
            {
                try
                {
                    Console.Write("请输入端口号：");
                    string port = Console.ReadLine();

                    ipep = new IPEndPoint(IPAddress.Parse(host), int.Parse(port));
                    break;
                }
                catch (Exception ex)
                {
                    sw.WriteLine(DateTime.Now.ToString() + "服务器错误:" + ex.Message);
                    Console.WriteLine("请输入正确的端口号");
                }
            }

            socket.Bind(ipep);//绑定ip port

            socket.Listen(0);//使套接字处于监听状态

            socket.BeginAccept(AcceptCallback, null);

            Refresh_Panel();

            Console.WriteLine("服务器创建完毕");

            //保证服务器一直运行
            while (true)
            {
                string cmd = Console.ReadLine();
                switch (cmd)
                {
                    case "exit":
                        return;
                    case "clear":
                        Refresh_Panel();
                        break;
                    default:
                        SendAll("服务器消息" + cmd);
                        break;
                }
            }
        }

        /// <summary>
        /// 客户端功能
        /// </summary>
        private static void Client()
        {
            Refresh_Panel();
            Console.Write("请输入Sever的ipv4地址:");

            while (true)
            {
                try
                {
                    ipa = IPAddress.Parse(Console.ReadLine());
                    break;
                }
                catch (Exception ex)
                {
                    sw.WriteLine(DateTime.Now.ToString() + "客户端错误:" + ex.Message);
                    Console.WriteLine("请输入正确的ip地址");
                }
            }

            while (true)
            {
                try
                {
                    Console.Write("请输入端口号：");
                    string s = Console.ReadLine();
                    ipep = new IPEndPoint(ipa, int.Parse(s));
                    break;
                }
                catch (Exception ex)
                {
                    sw.WriteLine(DateTime.Now.ToString() + "客户端错误:" + ex.Message);
                    Console.WriteLine("请输入正确的端口号");
                }
            }

            Console.WriteLine("链接中。。");

            socket.Connect(ipep);

            Console.WriteLine("已连接");

            socket.BeginReceive(buff, 0, 1024, SocketFlags.None, ReceiveCallback, socket);

            while (true)
            {
                string msg = Console.ReadLine();

                switch (msg)
                {
                    case "exit":
                        return;
                    case "clear":
                        Refresh_Panel();
                        break;
                    default:
                        byte[] bytes = Encoding.UTF8.GetBytes(msg);
                        socket.Send(bytes);
                        break;
                }
            }
        }

        /// <summary>
        /// 向所有人发送消息
        /// </summary>
        private static void SendAll(string msg)
        {
            //这种写法客户端会出现中文乱码
            //byte[] sendBuff = System.Text.Encoding.Default.GetBytes(msg);

            buff = new byte[1024];
            //将字符串转换成byte字节数组
            buff = Encoding.UTF8.GetBytes(msg);

            foreach (var item in userDictionary)
            {
                try
                {
                    item.Value.Send(buff);
                }
                catch (Exception ex)
                {
                    sw.WriteLine(DateTime.Now.ToString() + "服务器向客户端发送信息错误:" + ex.Message);
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
                Socket client = socket.EndAccept(ar);
                userDictionary.Add(client.RemoteEndPoint.ToString(), client);
                Console.WriteLine("客户端" + client.RemoteEndPoint + "已连接");
                client.BeginReceive(buff, 0, buff.Length, SocketFlags.None, ReceiveCallback, client);
                socket.BeginAccept(AcceptCallback, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("客户端链接是出错" + ex.Message);
            }
        }

        /// <summary>
        /// 服务器：当收到客户端的数据执行；客户端：收到服务器发来的数据时执行
        /// </summary>
        private static void ReceiveCallback(IAsyncResult ar)
        {
            int recv;
            string msg;
            switch (Current_State)
            {
                case State.Server:
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
                            Console.WriteLine(key + "已经下线，没有收到字节");
                            SendAll(key + "已经下线");
                            return;
                        }

                        UTF8Encoding encoding = new UTF8Encoding(); //将字节数组转换string
                        msg = encoding.GetString(buff, 0, recv);
                        msg = client.RemoteEndPoint + "，说：" + msg;
                        SendAll(msg);

                        buff = new byte[1024];
                        client.BeginReceive(buff, 0, buff.Length, SocketFlags.None, ReceiveCallback, client); //继续接收
                        Console.WriteLine(msg);
                    }
                    catch (Exception ex)
                    {
                        sw.WriteLine(DateTime.Now.ToString() + "服务器接受用户信息时出错" + ex.Message);
                        Console.WriteLine("服务器接受用户信息时出错");
                        userDictionary.Remove(key);//把用户从列表中删除
                        SendAll(key + "已经下线");
                    }
                    break;
                case State.Client:
                    try
                    {
                        Socket _client = (Socket)ar.AsyncState;
                        recv = _client.EndReceive(ar);
                        msg = Encoding.UTF8.GetString(buff, 0, recv);

                        Console.WriteLine(msg);

                        _client.BeginReceive(buff, 0, 1024, SocketFlags.None, ReceiveCallback, _client);//继续接收
                    }
                    catch (Exception ex)
                    {
                        sw.WriteLine(DateTime.Now.ToString() + "客户端向服务器发送消息时错误:" + ex.Message);
                        Console.WriteLine("客户端向服务器发送消息时错误:");
                    }
                    break;
                default:
                    sw.WriteLine(DateTime.Now.ToString() + "未知状态枚举:" + Current_State.ToString());
                    break;
            }

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
