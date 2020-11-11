using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

namespace NonsensicalKit
{
    public static class StringHelper
    {
        #region PublicMethod

        /// <summary>
        /// 算式运算(仅支持加减乘除)
        /// </summary>
        /// <param name="_s"></param>
        /// <returns></returns>
        public static double? Calculation(string _s)
        {
            List<string> ls = Incision(_s);

            if (ls == null || ls.Count == 0)
            {
                return null;
            }
            ls = Brackets(ls);
            if ((ls = Exclude(ls)) == null)
            {
                return null;
            }

            ls = Multiplication_And_Division(ls);
            return Addition_And_Subtraction(ls);
        }

        /// <summary>
        /// 获取集合的可读字符串
        /// </summary>
        /// <param name="ienumerable"></param>
        /// <returns></returns>
        public static string GetSetString(IEnumerable ienumerable)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[");
            foreach (var item in ienumerable)
            {
                if (item.GetType()==typeof(Vector3))
                {
                    Vector3 temp = (Vector3)item;
                    sb.Append($"({temp.x},{temp.y},{temp.z})");
                }
                else
                {
                    sb.Append(item.ToString());
                }
                sb.Append(",");
            }
            if (sb[sb.Length - 1]!='[')
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]");

            return sb.ToString();
        }

        public static string GetFileName(string path)
        {
            string[] paths = path.Split(new char[] { '/','\\'});
            string name = paths[paths.Length-1];
            return name;
        }
        
        #endregion

        #region PrivateMethod
        /// <summary>
        ///  将传入的字符串以算数符号为界切开，并放入链表中
        /// </summary>
        /// <param name="_s">要切分的字符串</param>
        /// <returns></returns>
        private static List<string> Incision(string _s)
        {
            List<string> ls = new List<string>();
            bool flag = false;

            while (true)
            {
                for (int i = 0; i < _s.Length; i++)
                {
                    if (_s[i] == '+' || _s[i] == '-' || _s[i] == '*' || _s[i] == '/')
                    {
                        string temp1 = _s[i].ToString();
                        char[] temp2 = { _s[i] };
                        string[] ss = _s.Split(temp2, 2);
                        if (ss[0] != null && ss[0] != "")
                        {
                            ls.Add(ss[0]);
                        }
                        ls.Add(temp1);
                        if (ss[1] == "")
                        {
                            return null;
                        }
                        _s = ss[1];
                        flag = true;
                        break;
                    }
                    else if (_s[i] == '(' || _s[i] == '（')
                    {
                        if (i == 0)
                        {
                            if (!char.IsNumber(_s[i + 1]))
                            {
                                return null;
                            }
                        }
                        else
                        {
                            if ((_s[i - 1] != '+' && _s[i - 1] != '-' && _s[i - 1] != '*' && _s[i - 1] != '/') || !char.IsNumber(_s[i + 1]))
                            {
                                return null;
                            }
                        }

                        ls.Add(_s[i].ToString());
                        _s = _s.Substring(1);
                        flag = true;
                        break;
                    }
                    else if (_s[i] == ')' || _s[i] == '）')
                    {
                        if (i == _s.Length - 1)
                        {
                            if (!char.IsNumber(_s[i - 1]))
                            {
                                return null;
                            }
                        }
                        else
                        {
                            if (!char.IsNumber(_s[i - 1]) || (_s[i + 1] != '+' && _s[i + 1] != '-' && _s[i + 1] != '*' && _s[i + 1] != '/'))
                            {
                                return null;
                            }
                        }

                        string temp1 = _s[i].ToString();
                        char[] temp2 = { _s[i] };
                        string[] ss = _s.Split(temp2, 2);
                        ls.Add(ss[0]);
                        ls.Add(temp1);
                        _s = ss[1];
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    flag = false;
                    continue;
                }
                if (_s != null && _s != "")
                {
                    ls.Add(_s);
                }
                break;
            }
            return ls;
        }

        /// <summary>
        /// 将所有非数字或运算符号的字符排除
        /// </summary>
        /// <param name="_ls"></param>
        /// <returns></returns>
        private static List<string> Exclude(List<string> _ls)
        {
            for (int i = 0; i < _ls.Count; i++)
            {
                if (_ls[i] != "+" && _ls[i] != "-" && _ls[i] != "*" && _ls[i] != "/" && _ls[i] != "(" && _ls[i] != ")" && _ls[i] != "（" && _ls[i] != "）")
                {
                    for (int j = 0; j < _ls[i].Length; j++)
                    {
                        if (!char.IsNumber(_ls[i][j]) && _ls[i][j] != '.')
                        {
                            _ls[i] = _ls[i].Replace(_ls[i][j].ToString(), "");
                            j = -1;
                        }
                    }
                    if (_ls[i] == "")
                    {
                        return null;
                    }
                }
            }

            //Console.Write("输入的式子为：");
            //for (int i = 0; i < _ls.Count; i++)
            //{
            //    Console.Write(_ls[i]);
            //}
            //Console.WriteLine();

            return _ls;
        }

        /// <summary>
        /// 括弧运算
        /// </summary>
        /// <param name="_ls"></param>
        /// <returns></returns>
        private static List<string> Brackets(List<string> _ls)
        {
            for (int i = 0; i < _ls.Count; i++)
            {
                if (_ls[i] == "(" || _ls[i] == "（")
                {
                    int count = 1;
                    for (int j = i + 1; j < _ls.Count; j++)
                    {
                        if (_ls[j] == "(" || _ls[j] == "（")
                        {
                            count++;
                        }
                        else if ((_ls[j] == ")" || _ls[j] == "）"))
                        {
                            count--;
                            if (count == 0)
                            {
                                _ls[i] = Calculation(string.Join("", _ls.GetRange(i + 1, j - i - 1))).ToString();
                                _ls.RemoveRange(i + 1, j - i);
                                break;
                            }

                        }
                    }
                }
            }
            return _ls;
        }

        /// <summary>
        /// 进行乘除运算
        /// </summary>
        /// <param name="_ls"></param>
        /// <returns></returns>
        private static List<string> Multiplication_And_Division(List<string> _ls)
        {
            int i = 0;
            bool flag = false;
            double d = 0;

            while (true)
            {
                for (; i < _ls.Count; i++)
                {
                    if (_ls[i] == "*")
                    {
                        d = double.Parse(_ls[i - 1]) * double.Parse(_ls[i + 1]);
                        flag = true;
                        break;
                    }
                    else if (_ls[i] == "/")
                    {
                        d = double.Parse(_ls[i - 1]) / double.Parse(_ls[i + 1]);
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    _ls[i - 1] = d.ToString();
                    _ls.RemoveRange(i, 2);
                    flag = false;
                }
                else
                {
                    return _ls;
                }
            }
        }

        /// <summary>
        /// 进行加减运算
        /// </summary>
        /// <param name="_ls"></param>
        /// <returns></returns>
        private static double Addition_And_Subtraction(List<string> _ls)
        {
            double result = double.Parse(_ls[0]);

            for (int i = 1; i < _ls.Count; i += 2)
            {
                if (_ls[i] == "+")
                {
                    result += double.Parse(_ls[i + 1]);
                }
                else if (_ls[i] == "-")
                {
                    result -= double.Parse(_ls[i + 1]);
                }
            }
            return result;
        }
        #endregion
    }
}
