using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Epsilon
{
    /// <summary>
    /// Epsilon加密器
    /// </summary>
    public class Ep_Encryptor
    {
        private Random random = new Random(int.Parse(DateTime.Now.ToString("HHmmssfff")));

        public Ep_Encryptor()
        {

        }

        /// <summary>
        /// 加密总方法
        /// </summary>
        /// <param name="_original">待加密的字符串</param>
        /// <returns>加密的结果字符串</returns>
        public string Encrypt(string _original)
        {
            Byte[] buffer = Encoding.UTF8.GetBytes(_original);

            _original = Random_Root(buffer);

            _original = TTWO(_original);

            _original = Cyclic_Addition(_original, 2);

            _original = Password1(_original);

            _original = Cyclic_Addition(_original, 16);

            return _original;
        }

        /// <summary>
        /// 随机种子
        /// </summary>
        /// <param name="_buff"></param>
        /// <returns></returns>
        private string Random_Root(Byte[] _buff)
        {
            int root = random.Next(740);
            string temp = root.ToString("000");
            for (int i = 0; i < _buff.Length; i++)
            {
                temp += (_buff[i] + root).ToString("000");
            }
            return temp;
        }

        /// <summary>
        /// 3321编码
        /// </summary>
        /// <param name="_c"></param>
        /// <returns></returns>
        private string TTWO(string _original)
        {
            string temp = null, temp2 = null;

            for (int i = 0; i < _original.Length; i++)
            {

                switch (_original[i] - 48)
                {
                    case 0:
                        temp2 = "0000";
                        break;
                    case 1:
                        temp2 = "0001";
                        break;
                    case 2:
                        temp2 = "0010";
                        break;
                    case 3:
                        switch (random.Next(3))
                        {
                            case 0:
                                temp2 = "0100";
                                break;
                            case 1:
                                temp2 = "1000";
                                break;
                            case 2:
                                temp2 = "0011";
                                break;
                            default:
                                temp2 = null;
                                break;
                        }
                        break;
                    case 4:
                        switch (random.Next(2))
                        {
                            case 0:
                                temp2 = "1001";
                                break;
                            case 1:
                                temp2 = "0101";
                                break;
                            default:
                                temp2 = null;
                                break;
                        }
                        break;
                    case 5:
                        switch (random.Next(2))
                        {
                            case 0:
                                temp2 = "1010";
                                break;
                            case 1:
                                temp2 = "0110";
                                break;
                            default:
                                temp2 = null;
                                break;
                        }
                        break;
                    case 6:
                        switch (random.Next(3))
                        {
                            case 0:
                                temp2 = "1011";
                                break;
                            case 1:
                                temp2 = "0111";
                                break;
                            case 2:
                                temp2 = "1100";
                                break;
                            default:
                                temp2 = null;
                                break;
                        }
                        break;
                    case 7:
                        temp2 = "1101";
                        break;
                    case 8:
                        temp2 = "1110";
                        break;
                    case 9:
                        temp2 = "1111";
                        break;
                    default:
                        temp2 = null;
                        break;
                }

                temp += temp2;
            }

            return temp;
        }

        /// <summary>
        /// 密码表1
        /// </summary>
        /// <param name="_original"></param>
        /// <returns></returns>
        private string Password1(string _original)
        {

            string temp = null, S = null;
            for (int i = 0; i < _original.Length; i += 3)
            {
                if (i + 2 < _original.Length)
                {
                    S = new string(new char[] { _original[i], _original[i + 1], _original[i + 2] });


                }
                else if (i + 1 < _original.Length)
                {
                    S = new string(new char[] { _original[i], _original[i + 1] });
                }
                else if (i < _original.Length)
                {
                    S = _original[i].ToString();
                }

                temp += string.Format("{0:X}", (int)Enum.Parse(typeof(password1), "p" + S));

                for (int r = random.Next(2); r == random.Next(2); r = random.Next(2))
                {

                    if (r == 0)
                    {
                        temp += "2";
                    }
                    else
                    {
                        temp += "D";
                    }
                }
            }

            return temp;
        }

        /// <summary>
        /// 循环相加
        /// </summary>
        /// <param name="_original"></param>
        /// <param name="_frombase"></param>
        /// <returns></returns>
        private string Cyclic_Addition(string _original, int _frombase)
        {
            string temp4 = _original[0].ToString();
            int it = Convert.ToInt16(temp4.ToString(), _frombase);
            for (int i = 1; i < _original.Length; i++)
            {
                it = Convert.ToInt16(_original[i].ToString(), _frombase) + it;
                if (it > _frombase - 1)
                {
                    it -= _frombase;
                }
                temp4 += String.Format("{0:X}", it);
            }
            return temp4;
        }
    }

    /// <summary>
    /// Epsilon解密器
    /// </summary>
    public class Ep_Decryptor
    {
        private string seed;

        /// <summary>
        /// 解密总方法
        /// </summary>
        /// <param name="_original">待解密字符串</param>
        /// <returns>解密后字符串，若解密出现错误则返回字符串："加密字符错误"</returns>
        public string Decrypt(string _original)
        {
            try
            {
                _original = Cyclic_Subtraction(_original, 16);

                _original = Reverse_Password1(_original);

                _original = Cyclic_Subtraction(_original, 2);

                _original = Reverse_TTWO(_original);

                _original = Reverse_Random_Root(_original);

                return _original;
            }
            catch (Exception)
            {
                return "加密字符错误";
            }

        }

        /// <summary>
        /// 循环相减
        /// </summary>
        /// <param name="_original"></param>
        /// <returns></returns>
        private string Cyclic_Subtraction(string _original, int frombase)
        {
            string temp = null;

            for (int i1 = _original.Length - 1; i1 > 0; i1--)
            {
                int tt = Convert.ToInt16(_original[i1].ToString(), frombase) - Convert.ToInt16(_original[i1 - 1].ToString(), frombase);

                if (tt < 0)
                {
                    tt += frombase;
                }

                temp = string.Format("{0:X}", tt) + temp;
            }

            temp = _original[0] + temp;

            return temp;
        }

        /// <summary>
        /// 反向密码表1
        /// </summary>
        /// <param name="_original"></param>
        /// <returns></returns>
        private string Reverse_Password1(string _original)
        {
            string temp = null;

            for (int i = 0; i < _original.Length; i++)
            {
                string pws = ((password1)Convert.ToInt16(_original[i].ToString(), 16)).ToString();

                if (pws.Substring(0, 1) != "n")
                {
                    string ss = pws.Substring(1);
                    temp += ss;
                }
            }

            return temp;
        }

        /// <summary>
        /// 反向3321编码
        /// </summary>
        /// <param name="_original"></param>
        /// <returns></returns>
        private string Reverse_TTWO(string _original)
        {
            string temp = null;
            for (int i = 0; i < _original.Length; i += 4)
            {
                temp += ((_original[i] - 48) * 3 + (_original[i + 1] - 48) * 3 + (_original[i + 2] - 48) * 2 + (_original[i + 3] - 48) * 1).ToString();
            }

            return temp;
        }

        /// <summary>
        /// 反响随机种子
        /// </summary>
        /// <param name="_original"></param>
        /// <returns></returns>
        private string Reverse_Random_Root(string _original)
        {
            int root = int.Parse(_original.Substring(0, 3));

            List<int> L_buffer = new List<int>();

            for (int i = 3; i < _original.Length; i += 3)
            {
                L_buffer.Add(int.Parse(_original.Substring(i, 3)) - root);
            }

            byte[] buffer = Array.ConvertAll(L_buffer.ToArray(), s => (byte)s);

            return new UTF8Encoding().GetString(buffer, 0, buffer.Length);
        }
    }
}
