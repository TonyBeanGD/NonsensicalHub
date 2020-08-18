using System;
using System.IO;
using System.Text;
using System.Xml;

namespace NonsensicalFrame
{
    public class XmlHelper
    {
        /// <summary>
        /// 获取目标路径xml文件的根节点
        /// </summary>
        /// <param name="_path">目标文件路径</param>
        /// <returns>当目标文件存在并且获取成功时，返回目标xml文件根节点，否则返回null</returns>
        public static XmlNode GetRoot(string _path)
        {
            if (!System.IO.File.Exists(_path))
            {
                return null;
            }

            try
            {
                XmlDocument xmlfile = new XmlDocument();

                xmlfile.Load(_path);

                XmlNode nodes = xmlfile.SelectSingleNode("Root");
                return nodes;
            }
            catch (Exception)
            {

                return null;
            }
        }

        /// <summary>
        /// 获取当前工作目录的完全限定路径
        /// </summary>
        /// <returns>当前工作目录的完全限定路径</returns>
        public static string Get_Current_Path()
        {
            string path = null;
            if (System.Environment.CurrentDirectory == AppDomain.CurrentDomain.BaseDirectory)//Windows应用程序则相等  
            {
                path = AppDomain.CurrentDomain.BaseDirectory;
            }
            else
            {
                path = AppDomain.CurrentDomain.BaseDirectory + "Bin/";
            }
            return path;
        }

        /// <summary>
        /// 创建一个新的XML文件
        /// </summary>
        /// <param name="_path">创建文件的存放路径</param>
        /// <returns></returns>
        public static bool Create_New_Xml_File(string _path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null));
            XmlElement root = xmlDoc.CreateElement("Root");
            xmlDoc.AppendChild(root);
            try
            {
                xmlDoc.Save(_path);
                return true;
            }
            catch (Exception e)
            {
                Nonsensical_Manager.Debug_Log(DateTime.Now.Date.ToShortTimeString() + ":" + e.Message);
                return false;
            }
        }
    }
}
