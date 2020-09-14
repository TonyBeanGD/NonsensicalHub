using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace NonsensicalFrame
{
    public class RandomHelper
    {
        /// <summary>
        /// 获取使用Guid作为种子返回的随机数
        /// </summary>
        /// <param name="max">返回值的绝对值小于max</param>
        /// <returns></returns>
        public static int GetRandomInt(int max)
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            int iSeed = BitConverter.ToInt32(buffer, 0);
            System.Random random = new System.Random(iSeed);
            int temp = random.Next(max * 2 - 1);
            temp = temp - max + 1;

            return temp;
        }

        /// <summary>
        /// 获取通过csp加密返回的随机数
        /// </summary>
        /// <param name="max">返回值的绝对值小于max</param>
        /// <returns></returns>
        public static int GetRandomIntPrime(int max)
        {
            byte[] randomBytes = new byte[4];
            RNGCryptoServiceProvider rngServiceProvider = new RNGCryptoServiceProvider();
            rngServiceProvider.GetBytes(randomBytes);
            Int32 result = BitConverter.ToInt32(randomBytes, 0);
            result %= max;

            return result;
        }
        
        /// <summary>
        /// 随机出在一个环内的坐标
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetRandomPointByC()
        {
            Vector2 v1 =new Vector2(UnityEngine.Random.Range(0f,1f), UnityEngine.Random.Range(0f, 1f)) * 20;
            Vector2 v2 = v1.normalized * (0.1f + v1.magnitude);
            Vector3 v3 = new Vector3(v2.x, 0, v2.y);
            return v3;
        }
    }
}
