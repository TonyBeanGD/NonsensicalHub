using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace NonsensicalKit
{
    public class RegexHelper : MonoBehaviour
    {
        ///<summary>
        /// 判断输入的字符串是否只包含数字和英文字母  
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNumAndEnCh(string input)
        {
            string pattern = @"^[A-Za-z0-9]+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }
    }
}
