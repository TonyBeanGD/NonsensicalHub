using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class SameFileCheck : MonoBehaviour
{
    private bool _Preparing = true;
    public Text _Txt_CrtState;
    public Text _Txt_PointPointPoint;
    private Thread t1;
    private string temp;

    private void Start()
    {
        t1 = new Thread(new ThreadStart(() => { FindSameFile("D:/"); }));
        t1.Start();
        StartCoroutine(CheckPreparing());
    }


    private void Update()
    {
        _Txt_CrtState.text = temp;
    }

    private IEnumerator CheckPreparing()
    {
        _Txt_CrtState.text = "_Preparing";
        WaitForSeconds wfs = new WaitForSeconds(0.5f);
        int counter = 1;
        StringBuilder sb = new StringBuilder();
        while (_Preparing)
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
            _Txt_PointPointPoint.text = sb.ToString();
            yield return wfs;
        }
        _Txt_PointPointPoint.text = "";
    }

    private void FindSameFile(string fatherPath)
    {
        _Preparing = true;
        temp = "GetFileName";
        FilesInfo fis = GetFilesInfo(fatherPath);

        temp = "FindSameFile";
        string[] originMd5 = fis._md5s;

        List<string> duplicates = new List<string>();
        List<string> md5s = new List<string>();

        for (int i = 0; i < originMd5.Length; i++)
        {
            if (originMd5[i] == string.Empty)
            {
                continue;
            }
            bool first = true;
            for (int j = i + 1; j < originMd5.Length; j++)
            {
                if (originMd5[i] == originMd5[j])
                {
                    if (fis._fis[i].Length != fis._fis[j].Length)
                    {
                        continue;
                    }
                    if (GetMD5HashFromFile(fis._fis[i].FullName) != GetMD5HashFromFile(fis._fis[j].FullName))
                        continue;
                    if (first)
                    {
                        duplicates.Add(fis._fis[i].FullName);
                        md5s.Add(originMd5[i]);
                    }
                    duplicates.Add(fis._fis[j].FullName);
                    md5s.Add(originMd5[j]);
                    originMd5[j] = string.Empty;
                }
            }
        }

        temp = "Writing";

        FileStream fs = new FileStream("D:/Test.md", FileMode.CreateNew);
        

        StringBuilder sb = new StringBuilder(); 
        for (int i = 0; i < duplicates.Count; i++)
        {
            sb = new StringBuilder(); 
            sb.AppendFormat("{0,-200}{1}\r\n", duplicates[i], md5s[i]);

            byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());

            fs.Write(bytes, 0, bytes.Length);
        }

        fs.Close();
        temp = "Complete";
        _Preparing = false;
    }

    private FilesInfo GetFilesInfo(string dirPath)
    {
        FilesInfo info = new FilesInfo();

        if (!Directory.Exists(dirPath))
        {
            info._message = dirPath + " is not a correct folder path!";
            return info;
        }

        Queue<string> dirq = new Queue<string>();
        dirq.Enqueue(dirPath);
        string s;

        while (dirq.Count > 0)
        {
            s = dirq.Dequeue();
            string[] filesPath = Directory.GetFiles(s);
            foreach (var item in filesPath)
            {
                FileInfo fi = new FileInfo(item);
                if ((fi.Attributes & FileAttributes.System) != FileAttributes.System)
                {
                    info._fis.Add(fi);
                }
            }
            string[] directorysPath = Directory.GetDirectories(s);
            foreach (var item in directorysPath)
            {
                DirectoryInfo di = new DirectoryInfo(item);

                if ((di.Attributes & FileAttributes.System) != FileAttributes.System)
                {
                    dirq.Enqueue(item);
                }
            }
        }
        info.Finish();
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
        public bool _Success;
        public List<FileInfo> _fis;
        public string _message;
        public string[] _md5s;
        public List<string> _errors;

        public FilesInfo()
        {
            _Success = false;
            _fis = new List<FileInfo>();
            _errors = new List<string>();
            _message = "No errors.";
        }

        public void Finish()
        {
            _Success = true;
        }
    }

    private void OnDestory()
    {
        t1.Abort();
    }
}
