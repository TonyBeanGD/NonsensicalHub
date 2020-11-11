using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace NonsensicalKit
{
    public class FileHelper
    {
        /// <summary>
        /// 转移数据，将源文件的内容作为文本覆盖到目标文件（默认使用UTF-8编码）
        /// </summary>
        /// <param name="_filepath1">源文件</param>
        /// <param name="_filepath2">目标文件</param>
        /// <param name="_origin">源文件编码格式</param>
        /// <param name="_target">目标文件编码格式</param>
        /// <returns>当转移成功时时，返回真，否则返回假</returns>
        public static bool TransferData(string _filepath1, string _filepath2, Encoding _origin =null, Encoding _target=null)
        {
            if (_origin==null)
            {
                _origin = Encoding.UTF8;
            }
            if (_target == null)
            {
                _target = Encoding.UTF8;
            }

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
                Debugger.Log(DateTime.Now.Date.ToShortTimeString() + ":" + e.Message);
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

        public static bool FileAppendWrite(string _path,string _name, string _text)
        {
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            string pathStr = _path + _name;
            try
            {
                using (FileStream fs = new FileStream(pathStr, FileMode.Append, FileAccess.Write, FileShare.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
                    {
                        sw.Write(_text );
                        sw.Flush();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                Debugger.Log("文件写入错误");
                return false;
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
        /// <returns></returns>
        public static string GetFileString(string _path)
        {
            if (!System.IO.File.Exists(_path))
            {
                return null;
            }
            
            using (StreamReader file = File.OpenText(_path))
            {
                string fileContent = file.ReadToEnd();
                return fileContent;
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
