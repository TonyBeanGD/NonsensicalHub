using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NonsensicalFrame
{
    public static class NumHelper
    {
        /// <summary>
        /// 获取传入数值的级数
        /// </summary>
        /// <param name="rawNum">传入数值</param>
        /// <returns>级数，个位数（如5）是0级，十位数（25）是1级，一位小数（0.5）是-1级</returns>
        public static int GetLevel(float rawNum)
        {
            if (rawNum == 0)
            {
                return 0;
            }
            rawNum = Mathf.Abs(rawNum);
            if (rawNum > 1)
            {
                int level = -1;
                float crtNum = rawNum;
                while (crtNum > 1)
                {
                    crtNum /= 10;
                    level++;
                }
                return level;
            }
            else
            {
                int level = 0;
                float crtNum = rawNum;
                while (crtNum < 1)
                {
                    crtNum *= 10;
                    level--;
                }
                return level;
            }
        }

        /// <summary>
        /// 根据传入float变量与当前等级求最近的整值float变量
        /// </summary>
        /// <param name="rawFloat">传入的float变量</param>
        /// <param name="level">传入的float变量</param>
        /// <returns>最近的整值float变量</returns>
        public static float GetNearValue(float rawFloat, int level)
        {
            float crtFloat = rawFloat / Mathf.Pow(10, level);
            float nearInt = Mathf.Round(crtFloat);
            return nearInt * Mathf.Pow(10, level);
        }

        public static Vector3 GetNearVector3(Vector3 rawVector3, int level)
        {
            return new Vector3(GetNearValue(rawVector3.x, level), GetNearValue(rawVector3.y, level), GetNearValue(rawVector3.z, level));
        }

        /// <summary>
        /// 进一法取整至整数
        /// </summary>
        /// <param name="rawFloat"></param>
        /// <returns></returns>
        public static int RoundingToInt_Add(this float rawFloat)
        {
            int rawFloatIntPart= (int)rawFloat;
            if (rawFloat- rawFloatIntPart > 0)
            {
                return rawFloatIntPart + 1;
            }
            else
            {
                return rawFloatIntPart;
            }
        }

        /// <summary>
        /// 判断两个float是否非常接近
        /// </summary>
        /// <param name="num1"></param>
        /// <param name="num2"></param>
        /// <returns></returns>
        public static bool IsNear(float num1, float num2)
        {
            float absX = Mathf.Abs(num1 - num2);
            return absX < 0.00001f;
        }
    }

}
