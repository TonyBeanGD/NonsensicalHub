using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Epsilon
{
    /// <summary>
    /// Epsilon管理器
    /// </summary>
    public class Ep_Manager
    {
        /// <summary>
        /// 版本
        /// </summary>
        private const string version = "3.1";

        /// <summary>
        /// 获取版本
        /// </summary>
        /// <returns></returns>
        public static string Get_Version()
        {
            return version;
        }

        /// <summary>
        /// 使用STA线程打开Form面板
        /// </summary>
        public static void Set_Form()
        {
            Thread InvokeThread = new Thread(new ThreadStart(Open_Form));
            InvokeThread.SetApartmentState(ApartmentState.STA);
            InvokeThread.Start();
            InvokeThread.Join();
        }

        /// <summary>
        /// 打开Form面板
        /// </summary>
        private static void Open_Form()
        {
            Ep_Form ef = new Ep_Form("Epsilon" + version);
            Application.Run(ef);
        }
    }

    /// <summary>
    /// Epsilon加密器
    /// </summary>
    public class Ep_Encryptor
    {
        private string seed;//4.0中加入seed算法

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

    /// <summary>
    /// Epsilon加密Form
    /// </summary>
    internal class Ep_Form : Form
    {
        public bool IsEncryptor;

        Panel panel_choice;
        Button btn_cho_encryptor;
        Button btn_cho_decryptor;
        Panel panel_ed;
        TextBox text_box_ed;
        Button btn_ed;
        Button btn_choice_file;
        Button btn_return;


        OpenFileDialog ofd_file_ed;

        public Ep_Form(string _name)
        {
            InitializeComponent(_name);
        }

        private void InitializeComponent(string _name)
        {
            panel_choice = new Panel();
            btn_cho_encryptor = new Button();
            btn_cho_decryptor = new Button();

            panel_ed = new Panel();
            text_box_ed = new TextBox();
            btn_ed = new Button();
            btn_choice_file = new Button();
            btn_return = new Button();

            ofd_file_ed = new OpenFileDialog();

            this.SuspendLayout();

            #region button_encryptor
            this.btn_cho_encryptor.Location = new System.Drawing.Point(85, 212);
            this.btn_cho_encryptor.Size = new System.Drawing.Size(75, 25);
            this.btn_cho_encryptor.TabIndex = 0;
            this.btn_cho_encryptor.Text = "加密";
            this.btn_cho_encryptor.UseVisualStyleBackColor = true;
            this.btn_cho_encryptor.Click += new System.EventHandler(this.btn_encryptor_Click);
            #endregion
            #region button_deceryptor
            this.btn_cho_decryptor.Location = new System.Drawing.Point(310, 212);
            this.btn_cho_decryptor.Size = new System.Drawing.Size(75, 25);
            this.btn_cho_decryptor.TabIndex = 1;
            this.btn_cho_decryptor.Text = "解密";
            this.btn_cho_decryptor.UseVisualStyleBackColor = true;
            this.btn_cho_decryptor.Click += new System.EventHandler(this.btn_decryptor_Click);
            #endregion

            #region panel_choice
            this.panel_choice.Controls.Add(this.btn_cho_encryptor);
            this.panel_choice.Controls.Add(this.btn_cho_decryptor);
            this.panel_choice.Dock = DockStyle.Fill;
            this.panel_choice.Location = new System.Drawing.Point(0, 0);
            this.panel_choice.Size = new System.Drawing.Size(450, 450);
            this.panel_choice.Visible = true;
            this.panel_choice.TabIndex = 2;
            #endregion

            #region text_box_ed
            text_box_ed.Location = new System.Drawing.Point(0, 0);
            text_box_ed.Multiline = true;
            text_box_ed.ScrollBars = ScrollBars.Vertical;
            text_box_ed.Size = new System.Drawing.Size(450, 400);
            text_box_ed.TabIndex = 0;
            #endregion

            #region btn_ed
            this.btn_ed.Location = new System.Drawing.Point(112, 412);
            this.btn_ed.Size = new System.Drawing.Size(75, 25);
            this.btn_ed.TabIndex = 1;
            this.btn_ed.Text = "button1";
            this.btn_ed.UseVisualStyleBackColor = true;
            this.btn_ed.Click += new System.EventHandler(this.btn_ed_Click);
            #endregion

            #region btn_choice_file
            this.btn_choice_file.Location = new System.Drawing.Point(262, 412);
            this.btn_choice_file.Size = new System.Drawing.Size(100, 25);
            this.btn_choice_file.TabIndex = 2;
            this.btn_choice_file.Text = "button1";
            this.btn_choice_file.UseVisualStyleBackColor = true;
            this.btn_choice_file.Click += new System.EventHandler(this.btn_Choice_File_Click);
            #endregion

            #region btn_return
            this.btn_return.Location = new System.Drawing.Point(370, 412);
            this.btn_return.Size = new System.Drawing.Size(75, 25);
            this.btn_return.TabIndex = 2;
            this.btn_return.Text = "返回";
            this.btn_return.UseVisualStyleBackColor = true;
            this.btn_return.Click += new System.EventHandler(this.btn_Return_Click);
            #endregion

            #region panel_ed
            panel_ed.Controls.Add(this.text_box_ed);
            panel_ed.Controls.Add(this.btn_ed);
            panel_ed.Controls.Add(this.btn_choice_file);
            panel_ed.Controls.Add(this.btn_return);
            panel_ed.Dock = DockStyle.Fill;
            panel_ed.Location = new System.Drawing.Point(0, 0);
            panel_ed.Size = new System.Drawing.Size(450, 450);
            panel_ed.Visible = false;
            panel_ed.TabIndex = 3;
            #endregion

            #region ofd_file
            this.ofd_file_ed.FileName = "openFileDialog1";
            this.ofd_file_ed.Filter = "文本文件|*.txt";
            this.ofd_file_ed.FileOk += new System.ComponentModel.CancelEventHandler(this.ofd_file_FileOk);
            #endregion

            #region Form
            Controls.Add(panel_choice);
            Controls.Add(panel_ed);
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(450, 450);
            Text = _name;
            #endregion

            ResumeLayout(false);

        }

        private void btn_encryptor_Click(object sender, EventArgs e)
        {
            IsEncryptor = true;
            btn_ed.Text = "加密";
            btn_choice_file.Text = "选择加密txt文件";
            panel_choice.Visible = false;
            panel_ed.Visible = true;
        }

        private void btn_decryptor_Click(object sender, EventArgs e)
        {
            IsEncryptor = false;
            btn_ed.Text = "解密";
            btn_choice_file.Text = "选择解密txt文件";
            panel_choice.Visible = false;
            panel_ed.Visible = true;
        }
        private void btn_ed_Click(object sender, EventArgs e)
        {
            if (IsEncryptor)
            {
                text_box_ed.Text = new Ep_Encryptor().Encrypt(text_box_ed.Text);
            }
            else
            {
                text_box_ed.Text = new Ep_Decryptor().Decrypt(text_box_ed.Text);
            }
        }
        private void btn_Choice_File_Click(object sender, EventArgs e)
        {
            ofd_file_ed.ShowDialog();
        }

        private void btn_Return_Click(object sender, EventArgs e)
        {
            text_box_ed.Text = null;
            panel_choice.Visible = true;
            panel_ed.Visible = false;
        }

        private void ofd_file_FileOk(object sender, CancelEventArgs e)
        {
            FileStream fs = new FileStream(ofd_file_ed.FileName, FileMode.Open, FileAccess.Read);

            StreamReader sr = new StreamReader(fs, Encoding.Default);

            string temp = sr.ReadToEnd();

            fs.Close();
            sr.Close();
            if (IsEncryptor)
            {
                Ep_Encryptor ee = new Ep_Encryptor();
                temp = ee.Encrypt(temp);
                FileStream fs2 = new FileStream(ofd_file_ed.FileName.Substring(0, ofd_file_ed.FileName.LastIndexOf("\\")) + "\\Epsilon.txt", FileMode.Create);

                StreamWriter sw = new StreamWriter(fs2);
                sw.Write(temp);
                sw.Close();
            }
            else
            {
                Ep_Decryptor ed = new Ep_Decryptor();
                temp = ed.Decrypt(temp);
                FileStream fs2 = new FileStream(ofd_file_ed.FileName.Substring(0, ofd_file_ed.FileName.LastIndexOf("\\")) + "\\Epsilon.txt", FileMode.Create);

                StreamWriter sw = new StreamWriter(fs2);
                sw.Write(temp);
                sw.Close();
            }

            System.Diagnostics.Process.Start(ofd_file_ed.FileName.Substring(0, ofd_file_ed.FileName.LastIndexOf("\\")));
        }
    }
}
