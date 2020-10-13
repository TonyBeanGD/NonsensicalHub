using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace NonsensicalFrame
{
    public class SameFileCheck : MonoBehaviour
    {
        [SerializeField]
        private Text txt_CrtState;
        [SerializeField]
        private Text txt_Loading;
        [SerializeField]
        private Text txt_DetailedInformation;

        private bool isRunning;
        private Thread t1;
        private string stateText;
        private string detailInfoText;
        private uint fileCount;
        private uint compareCount;

        private List<string> duplicates = new List<string>();
        private List<string> md5s = new List<string>();

        private void Awake()
        {
            isRunning = true;
        }

        private void Start()
        {
            stateText = "_Preparing";
            t1 = new Thread(new ThreadStart(() => { FindSameFile("D:/"); }));
            t1.Start();
            StartCoroutine(LoadingEffect());
        }

        private void Update()
        {
            txt_CrtState.text = stateText;
            txt_DetailedInformation.text = detailInfoText;
        }

        private void OnDestroy()
        {
            t1.Abort();
        }

        private IEnumerator LoadingEffect()
        {
            WaitForSeconds wfs = new WaitForSeconds(0.5f);
            int counter = 1;
            StringBuilder sb = new StringBuilder();
            while (isRunning)
            {
                if (counter > 9)
                {
                    counter = 0;
                }
                counter++;

                sb = new StringBuilder();

                for (int i = 0; i < counter; i++)
                {
                    sb.Append(".");
                }
                txt_Loading.text = sb.ToString();
                yield return wfs;
            }
            txt_Loading.text = "";
        }

        private void FindSameFile(string _rootPath)
        {
            stateText = "GetFileName";
            FilesInfo fis = GetFilesInfo(_rootPath);

            stateText = "CheckSameFile";
            CompareFile(fis);

            stateText = "Writing";

            FileStream fs = new FileStream("D:/Test.txt", FileMode.CreateNew);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < duplicates.Count; i++)
            {
                sb = new StringBuilder();
                sb.AppendFormat("{0,-200}{1}\r\n", duplicates[i], md5s[i]);

                byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());

                fs.Write(bytes, 0, bytes.Length);
            }

            fs.Close();

            stateText = "Complete";
            isRunning = false;
        }

        private void CompareFile(FilesInfo fis)
        {
            compareCount = 0;
               duplicates = new List<string>();
            md5s = new List<string>();

            string[] originMd5 = fis.Md5s.ToArray();

            for (int i = 0; i < originMd5.Length; i++)
            {
                if (originMd5[i] == string.Empty)
                {
                    continue;
                }
                bool first = true;
                for (int j = i + 1; j < originMd5.Length; j++)
                {
                    if (originMd5[i].Equals(originMd5[j]))
                    {
                        if (fis.FileInfos[i].Length != fis.FileInfos[j].Length)
                        {
                            continue;
                        }
                        if (GetMD5HashFromFile(fis.FileInfos[i].FullName) != GetMD5HashFromFile(fis.FileInfos[j].FullName))
                            continue;
                        if (first)
                        {
                            duplicates.Add(fis.FileInfos[i].FullName);
                            md5s.Add(originMd5[i]);
                        }
                        duplicates.Add(fis.FileInfos[j].FullName);
                        md5s.Add(originMd5[j]);
                        originMd5[j] = string.Empty;
                    }
                }
            }
        }

        private FilesInfo GetFilesInfo(string dirPath)
        {
            fileCount = 0;
               FilesInfo info = new FilesInfo();

            if (!Directory.Exists(dirPath))
            {
                info.Messages.Add(dirPath + " is not a correct folder path!");
                return info;
            }

            Queue<string> dirq = new Queue<string>();
            dirq.Enqueue(dirPath);

            while (dirq.Count > 0)
            {
                string path = dirq.Dequeue();
                try
                {
                    string[] filesPath = Directory.GetFiles(path);
                    foreach (var item in filesPath)
                    {
                        FileInfo fi = new FileInfo(item);
                        if ((fi.Attributes & FileAttributes.System) != FileAttributes.System)
                        {
                            fileCount++;
                            detailInfoText = fileCount + "：" + fi.FullName;
                            info.FileInfos.Add(fi);
                            info.Md5s.Add(fi.ToString());
                        }
                    }
                    string[] directorysPath = Directory.GetDirectories(path);
                    foreach (var item in directorysPath)
                    {
                        DirectoryInfo di = new DirectoryInfo(item);

                        if ((di.Attributes & FileAttributes.System) != FileAttributes.System)
                        {
                            dirq.Enqueue(item);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message + "\n" + e.Data);
                }
            }
            return info;
        }

        /// <summary>
        /// 获取文件MD5值
        /// </summary>
        /// <param name="fileName">文件绝对路径</param>
        /// <returns>MD5值</returns>
        private string GetMD5HashFromFile(string fileName, int? size = 0)
        {
            FileStream file;
            StringBuilder sb;
            try
            {
                file = new FileStream(fileName, FileMode.Open);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message + "\n" + ex.Data);
                return string.Empty;
            }

            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bytes;
            byte[] retVal;
            if (size <= 0)
            {
                retVal = md5.ComputeHash(file);
            }
            else
            {
                bytes = new byte[(int)size];
                file.Read(bytes, 0, (int)size);
                retVal = md5.ComputeHash(bytes);
            }

            file.Close();

            sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        private class FilesInfo
        {
            public List<FileInfo> FileInfos;
            public List<string> Messages;
            public List<string> Md5s;

            public FilesInfo()
            {
                FileInfos = new List<FileInfo>();
                Messages = new List<string>();
                Md5s = new List<string>();
            }
        }
    }
}