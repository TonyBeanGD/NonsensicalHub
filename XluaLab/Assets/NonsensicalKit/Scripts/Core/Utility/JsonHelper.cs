using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace NonsensicalKit.Utility
{
    public static class JsonHelper
    {
        /// <summary>
        /// litjson获取集合键名
        /// </summary>
        /// <param name="originString"></param>
        public static void GetKeyName(string originString)
        {
            JsonData jsonData = JsonMapper.ToObject(originString);

            IDictionary idic = (IDictionary)jsonData;
            foreach (var item in idic.Keys)
            {
                Debug.Log(item.ToString());
            }
        }

        /// <summary>
        /// 动态生成json(使用递归)
        /// 输入：JsonData jd=test(new string[]{"1","2","3"},10086);
        /// 结果：Debug.Log(jd["0"]["1"]["2"]);    //=10086
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static JsonData Test(string[] a, int b, int pos = 0)
        {
            if (pos == a.Length)
            {
                return b;
            }
            else
            {
                JsonData jd = new JsonData();

                jd[a[(int)pos]] = Test(a, b, pos + 1);

                return jd;
            }
        }

        /// <summary>
        /// 动态生成json(不使用递归)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static JsonData Test2(string[] a, int b)
        {
            JsonData jd = new JsonData();

            int pos = a.Length - 1;

            while (pos >= 0)
            {
                if (pos == a.Length - 1)
                {
                    jd[a[pos]] = b;
                }
                else
                {
                    //直接使用jd[a[pos]] = temp;会导致堆栈溢出异常
                    JsonData temp = jd;
                    jd = new JsonData();
                    jd[a[pos]] = temp;
                }

                pos--;
            }

            return jd;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        public static void SaveFile<T>(string fileName, T data)
        {
            string dataJson = JsonConvert.SerializeObject(data);

            FileHelper.EnsureDir(Path.Combine(Application.streamingAssetsPath, "SaveJsonFiles"));

            FileHelper.WriteTxt(Path.Combine(Application.streamingAssetsPath, "SaveJsonFiles", fileName + ".json"), dataJson);
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T LoadFile<T>(string fileName)
        {
           
            string fullPath = Path.Combine(Application.streamingAssetsPath, "SaveJsonFiles", fileName + ".json");

            string dataJson = FileHelper.ReadAllText(fullPath);
            if (dataJson == null)
            {
                return default(T);
            }

            T data = JsonConvert.DeserializeObject<T>(dataJson);

            return data;
        }
    }

}
