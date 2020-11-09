using System;
using System.IO;
using System.Text;
using System.Xml;

namespace NonsensicalKit
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
        /// 创建一个新的XML文件
        /// </summary>
        /// <param name="_path">创建文件的存放路径</param>
        /// <returns></returns>
        public static bool CreateNewXmlFile(string _path)
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
                Debugger.Log(DateTime.Now.Date.ToShortTimeString() + ":" + e.Message);
                return false;
            }
        }
    }
}


//XmlElement new_ele = xml.CreateElement("fuck");//在一个文件中创建一个节点，创建后此节点存于内存中
//new_ele.SetAttribute("haha","ri");//给一个节点添加属性
//        new_ele.InnerText = "aaa";//改变节点的内部文本
//        nodes.AppendChild(new_ele);//将节点追加进另一节点中


// public static class XmlHelper
//{
//    private static void XmlSerializeInternal(Stream stream, object o, Encoding encoding)
//    {
//        if (o == null)
//            throw new ArgumentNullException("o");
//        if (encoding == null)
//            throw new ArgumentNullException("encoding");

//        XmlSerializer serializer = new XmlSerializer(o.GetType());

//        XmlWriterSettings settings = new XmlWriterSettings();
//        settings.Indent = true;
//        settings.NewLineChars = "\r\n";
//        settings.Encoding = encoding;
//        settings.IndentChars = "    ";

//        using (XmlWriter writer = XmlWriter.Create(stream, settings))
//        {
//            serializer.Serialize(writer, o);
//            writer.Close();
//        }
//    }

//    /// <summary>
//    /// 将一个对象序列化为XML字符串
//    /// </summary>
//    /// <param name="o">要序列化的对象</param>
//    /// <param name="encoding">编码方式</param>
//    /// <returns>序列化产生的XML字符串</returns>
//    public static string XmlSerialize(object o, Encoding encoding)
//    {
//        using (MemoryStream stream = new MemoryStream())
//        {
//            XmlSerializeInternal(stream, o, encoding);

//            stream.Position = 0;
//            using (StreamReader reader = new StreamReader(stream, encoding))
//            {
//                return reader.ReadToEnd();
//            }
//        }
//    }

//    /// <summary>
//    /// 将一个对象按XML序列化的方式写入到一个文件
//    /// </summary>
//    /// <param name="o">要序列化的对象</param>
//    /// <param name="path">保存文件路径</param>
//    /// <param name="encoding">编码方式</param>
//    public static void XmlSerializeToFile(object o, string path, Encoding encoding)
//    {
//        if (string.IsNullOrEmpty(path))
//            throw new ArgumentNullException("path");

//        using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
//        {
//            XmlSerializeInternal(file, o, encoding);
//        }
//    }

//    /// <summary>
//    /// 从XML字符串中反序列化对象
//    /// </summary>
//    /// <typeparam name="T">结果对象类型</typeparam>
//    /// <param name="s">包含对象的XML字符串</param>
//    /// <param name="encoding">编码方式</param>
//    /// <returns>反序列化得到的对象</returns>
//    public static T XmlDeserialize<T>(string s, Encoding encoding)
//    {
//        if (string.IsNullOrEmpty(s))
//            throw new ArgumentNullException("s");
//        if (encoding == null)
//            throw new ArgumentNullException("encoding");

//        XmlSerializer mySerializer = new XmlSerializer(typeof(T));
//        using (MemoryStream ms = new MemoryStream(encoding.GetBytes(s)))
//        {
//            using (StreamReader sr = new StreamReader(ms, encoding))
//            {
//                return (T)mySerializer.Deserialize(sr);
//            }
//        }
//    }

//    /// <summary>
//    /// 读入一个文件，并按XML的方式反序列化对象。
//    /// </summary>
//    /// <typeparam name="T">结果对象类型</typeparam>
//    /// <param name="path">文件路径</param>
//    /// <param name="encoding">编码方式</param>
//    /// <returns>反序列化得到的对象</returns>
//    public static T XmlDeserializeFromFile<T>(string path, Encoding encoding)
//    {
//        if (string.IsNullOrEmpty(path))
//            throw new ArgumentNullException("path");
//        if (encoding == null)
//            throw new ArgumentNullException("encoding");

//        string xml = File.ReadAllText(path, encoding);
//        return XmlDeserialize<T>(xml, encoding);
//    }
//}


    //private void ReadAppsetting()
    //{
    //    string file = Application.dataPath + "/Configs/xml.xml";

    //    if (!System.IO.File.Exists(file))
    //    {
    //        Debug.LogError("未找到xml文件");
    //    }
    //    XmlDocument xml = new XmlDocument();
    //    xml.Load(file);
    //    XmlNode nodes = xml.SelectSingleNode("Root");

    //    foreach (XmlElement item in nodes)
    //    {
    //        if (item.Name == "AppName")
    //        {
    //            Debug.Log("应用程序名称" + item.Attributes["value"].Value);
    //            continue;
    //        }
    //        if (item.Name == "Version")
    //        {
    //            Debug.Log("版本" + item.Attributes["value"].Value);
    //            continue;
    //        }
    //        if (item.Name == "LoginName")
    //        {
    //            Debug.Log("用户名" + item.Attributes["value"].Value);
    //            continue;
    //        }
    //        if (item.Name == "IsAutoLogin")
    //        {
    //            Debug.Log("是否自动登陆" + item.Attributes["value"].Value);
    //            continue;
    //        }
    //    }
    //}

    //private void WriteAppsetting()
    //{
    //    string file = Application.dataPath + "/Configs/xml.xml";

    //    if (!System.IO.File.Exists(file))
    //    {
    //        Debug.LogError("未找到xml文件");
    //    }
    //    XmlDocument xml = new XmlDocument();
    //    xml.Load(file);
    //    XmlNode appNameNode = xml.SelectSingleNode("Root/AppName");
    //    appNameNode.Attributes["value"].Value = "fangnilaingdegoupi";

    //    xml.Save(file);
    //}

    //<? xml version="1.0" encoding="utf-8"?>
    //<Root>
    //  <AppName value = "fangnilaingdegoupi" comment="应用程序名称" />
    //  <Version value = "1.0" comment="版本" />
    //  <LoginName value = "admin" comment="用户名" />
    //  <IsAutoLogin value = "true" comment="是否自动登陆" />
    //</Root>

    //XmlElement new_ele = xml.CreateElement("fuck");//在一个文件中创建一个节点，创建后此节点存于内存中
    //new_ele.SetAttribute("haha","ri");//给一个节点添加属性
    //        new_ele.InnerText = "aaa";//改变节点的内部文本
    //        nodes.AppendChild(new_ele);//将节点追加进另一节点中