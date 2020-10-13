using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace NonsensicalFrame
{
    public class FileHelper
    {
        /// <summary>
        /// 转移数据，将源文件的内容作为文本覆盖到目标文件（默认使用UTF-8编码）
        /// </summary>
        /// <param name="_filepath1">源文件</param>
        /// <param name="_filepath2">目标文件</param>
        /// <returns>当数据转移成功时时，返回真，否则返回假</returns>
        public static bool Transfer_Data(string _filepath1, string _filepath2)
        {
            if (!System.IO.File.Exists(_filepath1) || !System.IO.File.Exists(_filepath1))
            {
                return false;
            }

            StreamReader sr = null;
            FileStream fs = null;

            try
            {
                sr = new StreamReader(_filepath1, Encoding.UTF8);
                string content = sr.ReadToEnd();

                fs = new FileStream(_filepath2, FileMode.Create);
                byte[] data = Encoding.UTF8.GetBytes(content);
                fs.Write(data, 0, data.Length);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        /// <summary>
        /// 转移数据，将源文件的内容作为文本覆盖到目标文件（默认使用UTF-8编码）
        /// </summary>
        /// <param name="_filepath1">源文件</param>
        /// <param name="_filepath2">目标文件</param>
        /// <param name="_encoding">编码格式</param>
        /// <returns>当转移成功时时，返回真，否则返回假</returns>
        public static bool Transfer_Data(string _filepath1, string _filepath2, Encoding _encoding)
        {
            StreamReader sr = null;
            FileStream fs = null;

            if (!System.IO.File.Exists(_filepath1) || !System.IO.File.Exists(_filepath1))
            {
                return false;
            }

            try
            {
                sr = new StreamReader(_filepath1, Encoding.UTF8);
                string content = sr.ReadToEnd();

                fs = new FileStream(_filepath2, FileMode.Create);
                byte[] data = _encoding.GetBytes(content);
                fs.Write(data, 0, data.Length);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        /// <summary>
        /// 转移数据，将源文件的内容作为文本覆盖到目标文件（默认使用UTF-8编码）
        /// </summary>
        /// <param name="_filepath1">源文件</param>
        /// <param name="_filepath2">目标文件</param>
        /// <param name="_origin">源文件编码格式</param>
        /// <param name="_target">目标文件编码格式</param>
        /// <returns>当转移成功时时，返回真，否则返回假</returns>
        public static bool Transfer_Data(string _filepath1, string _filepath2, Encoding _origin, Encoding _target)
        {
            StreamReader sr = null;
            FileStream fs = null;

            if (!System.IO.File.Exists(_filepath1) || !System.IO.File.Exists(_filepath1))
            {
                return false;
            }

            try
            {
                sr = new StreamReader(_filepath1, _origin);
                string content = sr.ReadToEnd();

                fs = new FileStream(_filepath2, FileMode.Create);
                byte[] data = _target.GetBytes(content);
                fs.Write(data, 0, data.Length);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        /// <summary>
        /// 创建一个txt文件并往其中写入消息
        /// </summary>
        /// <param name="_path">创建文件路径</param>
        /// <param name="_message">写入的消息</param>
        /// <returns>创建并写入成功则返回true，否则返回false</returns>
        public static bool CreateAndWrite(string _path, string _message)
        {
            FileStream fs = null;
            StreamWriter sw = null;

            try
            {
                fs = new FileStream(_path, FileMode.Create);
                sw = new StreamWriter(fs, Encoding.UTF8);
                sw.Write(_message);
                return true;
            }
            catch (Exception e)
            {
                Manager.GetInstance().Debug_Log(DateTime.Now.Date.ToShortTimeString() + ":" + e.Message);
                return false;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        /// <summary>
        /// 读取文档，逐行输出：
        /// </summary>
        public List<string> ReadByLine()
        {
            StreamReader sr = new StreamReader(@"Debug.txt", Encoding.UTF8);
            string line;
            List<string> lines = new List<string>();
            while ((line = sr.ReadLine()) != null)
            {
                lines.Add(line.ToString());
            }
            return lines;
        }


        /// <summary>
        /// 获取当前工作目录的完全限定路径
        /// </summary>
        /// <returns>当前工作目录的完全限定路径</returns>
        public static string GetCurrentPath()
        {
            string path = null;
            if (System.Environment.CurrentDirectory == AppDomain.CurrentDomain.BaseDirectory)//Windows应用程序则相等  
            {
                path = AppDomain.CurrentDomain.BaseDirectory;
            }
            else
            {
                path = AppDomain.CurrentDomain.BaseDirectory + "Bin\\";
            }
            return path;
        }

       
    }
}

/* 
        ”我的文档“路径
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
		
		如何获取指定目录包含的文件和子目录
		1. DirectoryInfo.GetFiles()：获取目录中（不包含子目录）的文件，返回类型为FileInfo[]，支持通配符查找；
		2. DirectoryInfo.GetDirectories()：获取目录（不包含子目录）的子目录，返回类型为DirectoryInfo[]，支持通配符查找；
		3. DirectoryInfo. GetFileSystemInfos()：获取指定目录下（不包含子目录）的文件和子目录，返回类型为FileSystemInfo[]，支持通配符查找；
		如何获取指定文件的基本信息；
		FileInfo.Exists：获取指定文件是否存在；
		FileInfo.Name，FileInfo.Extensioin：获取文件的名称和扩展名；
		FileInfo.FullName：获取文件的全限定名称（完整路径）；
		FileInfo.Directory：获取文件所在目录，返回类型为DirectoryInfo；
		FileInfo.DirectoryName：获取文件所在目录的路径（完整路径）；
		FileInfo.Length：获取文件的大小（字节数）；
		FileInfo.IsReadOnly：获取文件是否只读；
		FileInfo.Attributes：获取或设置指定文件的属性，返回类型为FileAttributes枚举，可以是多个值的组合
		FileInfo.CreationTime、FileInfo.LastAccessTime、FileInfo.LastWriteTime：分别用于获取文件的创建时间、访问时间、修改时间；
 */
