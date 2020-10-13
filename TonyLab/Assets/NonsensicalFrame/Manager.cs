using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace NonsensicalFrame
{
    public class Manager
    {
        private static Manager instance;

        public static Manager GetInstance()
        {
            if (instance == null)
            {
                instance = new Manager();
                instance.ResetRootPath();
            }

            return instance;
        }

        /// <summary>
        /// 根路径
        /// </summary>
        private string rootPath;

        public void ResetRootPath()
        {
            rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NonsensicalHub");
        }

        public void SetRootPath(string _path)
        {
            rootPath = _path;
        }

        /// <summary>
        /// 根文件夹检测，如果根文件夹不完整，则补全根文件夹
        /// </summary>
        public void RootCheck()
        {
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);



                CreateNewConfigFile();
            }
            else
            {
                if (!System.IO.File.Exists(rootPath + "Config.xml"))
                {
                    CreateNewConfigFile();
                }
                else
                {
                    XmlHelper.GetRoot(rootPath + "Config.xml");
                }
            }
        }

        /// <summary>
        /// 在根文件夹里创建一个新的版本信息xml文件
        /// </summary>
        public void CreateNewConfigFile()
        {
            XmlHelper.CreateNewXmlFile(rootPath + "Config.xml");

            XmlNode root = XmlHelper.GetRoot(rootPath + "Config.xml");

            foreach (XmlNode item in root)
            {
                if (item.Name == "")
                {

                }
            }
        }

        public void Debug_Log(string _message)
        {
            if (System.IO.File.Exists(rootPath))
            {
                System.IO.File.Create(rootPath);
            }
        }
    }
}
