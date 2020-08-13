using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace NonsensicalFrame
{
    public class Nonsensical_Manager
    {
        private static string Version = "1.0";

        private static Nonsensical_Manager instance;

        /// <summary>
        /// 自定义日志路径
        /// </summary>
        private static string custom_log_path = null;

        /// <summary>
        /// 使用Nonsensical日志
        /// </summary>
        private static bool use_nonsensical_log = false;

        private Nonsensical_Manager()
        {
            instance = new Nonsensical_Manager();
        }

        public static Nonsensical_Manager Get_Instance()
        {
            if (instance == null)
            {
                instance = new Nonsensical_Manager();
            }

            return instance;
        }

        /// <summary>
        /// 自定义日志路径
        /// </summary>
        /// <param name="_path">日志路径</param>
        public void Custom_Log_Path(string _path)
        {
            custom_log_path = _path;
        }

        /// <summary>
        /// 使用Nonsensical日志
        /// </summary>
        /// <param name="_b"></param>
        public void Use_Nonsensical_Log(bool _b)
        {
            use_nonsensical_log = _b;
        }

        /// <summary>
        /// 根文件夹检测，如果根文件夹不完整，则补全根文件夹
        /// </summary>
        internal static void Root_Check()
        {
            if (!Directory.Exists(Root_Path))
            {
                Directory.CreateDirectory(Root_Path);

                Nonsensical_File.Create_And_Write(Root_Path + "readme.txt", "这个文件夹“Nonsensical”为Nonsensical相关软件的专用文件夹\r\n" +
                    "用于存储相关配置和存档\r\n");

                New_Version_Xml();
            }
            else
            {
                if (!File.Exists(Root_Path + "readme.txt"))
                {
                    Nonsensical_File.Create_And_Write(Root_Path + "readme.txt", "这个文件夹“Nonsensical”为Nonsensical相关软件的专用文件夹\r\n" +
                   "用于存储相关配置和存档\r\n");
                }

                if (!File.Exists(Root_Path + "Version.xml"))
                {
                    New_Version_Xml();
                }
                else
                {
                    Nonsensical_Xml.GetRoot(Root_Path + "Version.xml");
                }
            }
        }

        /// <summary>
        /// 在根文件夹里创建一个新的版本信息xml文件
        /// </summary>
        private static void New_Version_Xml()
        {
            Nonsensical_Xml.Create_New_Xml_File(Root_Path + "Version.xml");

            XmlNode root = Nonsensical_Xml.GetRoot(Root_Path + "Version.xml");

            foreach (XmlNode item in root)
            {
                if (item.Name=="")
                {
                   
                }
            }
        }

        /// <summary>
        /// 获取当前日期的字符串，以下划线隔开
        /// </summary>
        /// <returns>当前日期的字符串</returns>
        internal static string Get_Date_String()
        {
            return DateTime.Today.Year + "_" + DateTime.Today.Month + "_" + DateTime.Today.Day;
        }

        /// <summary>
        /// 
        /// </summary>
        private const string Root_Path = "D:/Nonsensical/";

        private void test()
        {
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        internal static void Debug_Log(string _message)
        {
            string path = null;
            if (use_nonsensical_log)
            {
                path = Root_Path + "Log/" + Get_Date_String() + ".txt";
            }
            else if (custom_log_path != null)
            {
                path = custom_log_path;
            }
            else
            {
                return;
            }

            if (File.Exists(path))
            {
                File.Create(path);
            }
        }
    }
}
