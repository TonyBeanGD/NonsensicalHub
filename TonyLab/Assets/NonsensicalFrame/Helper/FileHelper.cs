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
        /// 创建或读取一个txt文件并往其中写入文本(覆盖)
        /// </summary>
        /// <param name="_path">文件路径</param>
        /// <param name="_text">写入的文本</param>
        /// <returns>创建并写入成功则返回true，否则返回false</returns>
        public static bool WriteTxt(string _path, string _text)
        {
            FileStream fs = null;
            StreamWriter sw = null;

            try
            {
                fs = new FileStream(_path, FileMode.Create);
                sw = new StreamWriter(fs, Encoding.UTF8);
                sw.Write(_text);
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
        public static List<string> ReadByLine()
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

        /// <summary>
        /// 获取文件内容字符串
        /// </summary>
        /// <param name="_path"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string GetFileString(string _path,Encoding encoding=null)
        {
            if (encoding==null)
            {
                encoding = Encoding.UTF8;
            }

            if (!System.IO.File.Exists(_path))
            {
                return null;
            }

            StreamReader sr = null;

            try
            {
                sr = new StreamReader(_path, encoding);
                string content = sr.ReadToEnd();
                return content;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }

        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        public static bool DeleteFile(string _path)
        {
            try
            {
                File.Delete(_path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
