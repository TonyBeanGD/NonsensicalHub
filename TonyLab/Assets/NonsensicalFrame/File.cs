using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Nonsensical
{
    public class Nonsensical_File
    {
        /// <summary>
        /// 转移数据，将源文件的内容作为文本覆盖到目标文件（默认使用UTF-8编码）
        /// </summary>
        /// <param name="_filepath1">源文件</param>
        /// <param name="_filepath2">目标文件</param>
        /// <returns>当数据转移成功时时，返回真，否则返回假</returns>
        public static bool Transfer_Data(string _filepath1, string _filepath2)
        {
            if (!File.Exists(_filepath1) || !File.Exists(_filepath1))
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

            if (!File.Exists(_filepath1) || !File.Exists(_filepath1))
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

            if (!File.Exists(_filepath1) || !File.Exists(_filepath1))
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
        public static bool Create_And_Write(string _path, string _message)
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
                Nonsensical_Manager.Debug_Log(DateTime.Now.Date.ToShortTimeString() + ":" + e.Message);
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
    }
}
