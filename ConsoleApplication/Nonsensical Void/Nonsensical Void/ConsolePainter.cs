using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;

namespace Nonsensical_Void
{
    class ConsolePainter
    {
        public int Width { get; private set; } = 0;
        public int Height { get; private set; } = 0;

        private bool isRun = false;
        private int posX = 0;
        private int posY = 0;
        private int offsetX = 0;
        private int offsetY = 0;
        private PointInfo[,] Screen = null;
        private PointInfo[,] LastFrame = null;
        private Thread Update=null;

        public ConsolePainter(int _width, int _height)
        {
            Console.CursorVisible = false;

            Width = _width;
            Height = _height;

            Screen = CreateMap(_width, _height);
            LastFrame = CreateMap(_width, _height);
        }

        public ConsolePainter(int _posX, int _posY, int _height, int _width) : this(_height, _width)
        {
            posX = _posX;
            posY = _posY;
        }

        public void Paint(int _x, int _y, int _index)
        {
            if (_x < 1 || _x > Width - 1 || _y < 1 || _y > Height - 1)
            {
                return;
            }
            Screen[_x, _y].Set(_index);
        }

        public void Paint(int _x, int _y, char _c)
        {
            if (_x < 1 || _x > Width - 1 || _y < 1 || _y > Height - 1)
            {
                return;
            }
            Screen[_x, _y].str = _c.ToString();
        }

        public void Paint(int _x, int _y, string _str)
        {
            if (_y<=0||_y>=Height-1)
            {
                return;
            }
            string[] temp = GetStringFormat(_str);
            int max = _x + temp.Length < Width - 1 ? temp.Length : Width - 1 - _x;
            for (int xPlus = 0; xPlus < max; xPlus++)
            {
                Screen[_x + xPlus, _y].str = temp[xPlus].ToString();
            }
        }

        public void Open(bool autoUpdate,int frameTime=20)
        {
            for (int y = 0; y < Height; y++)
            {
                Console.SetCursorPosition(posX * 2, posY + y);
                for (int x = 0; x < Width; x++)
                {
                    Console.Write(Screen[x, y].str);
                }
            }
            isRun = true;
           
            if (autoUpdate==true)
            {
                Update = new Thread(() =>
                {
                    while (isRun == true)
                    {
                        UpdateScreen();
                        Thread.Sleep(frameTime);
                    }
                });

                Update.Start();
            }
        }

        public void Clear()
        {
            Screen = CreateMap(Width, Height);
            LastFrame = CreateMap(Width, Height);

            for (int i = 1; i < Height - 1; i++)
            {
                Console.SetCursorPosition(posX + 2, posY + i);
                StringBuilder sb = new StringBuilder();
                for (int j = 1; j < Width - 1; j++)
                {
                    sb.Append("  ");
                }
                Console.Write(sb.ToString());
            }
        }

        public void Close()
        {
            isRun = false;
            for (int i = 0; i < Height; i++)
            {
                Console.SetCursorPosition(posX + 0, posY + i);
                StringBuilder sb = new StringBuilder();
                for (int j = 0; j < Width; j++)
                {
                    sb.Append("  ");
                }
                Console.Write(sb.ToString());
            }
        }

        public void UpdateScreen()
        {
            int paintStartX = 0;
            int paintStartY = 0;
            bool isContinuously = false;
            string paintStr = string.Empty;
            for (int y = 1; y < Height - 1; y++)
            {
                for (int x = 1; x < Width - 1; x++)
                {
                    if (Screen[x, y].str != LastFrame[x, y].str)
                    {
                        paintStr += Screen[x, y].str;
                        if (isContinuously == false)
                        {
                            paintStartX = x;
                            paintStartY = y;
                            isContinuously = true;
                        }
                        LastFrame[x, y] = Screen[x, y].Clone();
                    }
                    else
                    {
                        if (isContinuously == true)
                        {
                            DoPaint((posX + paintStartX) * 2, posY + paintStartY, paintStr);
                            paintStr = string.Empty;
                            isContinuously = false;
                        }
                    }

                    if (isContinuously == true)
                    {
                        DoPaint((posX + paintStartX) * 2, posY + paintStartY, paintStr);
                        paintStr = string.Empty;
                        isContinuously = false;
                    }
                }
            }
        }

        private void DoPaint(int paintX, int paintY, string str)
        {
            int offset = 0;
            if (str.Length == 1 && str.Equals(" "))
            {
                return;
            }
            else if (str[0].Equals(' ') == true && str[1].Equals(' ') == false)
            {
                offset = 1;
                str = str.Substring(1);
            }
            Console.SetCursorPosition(paintX + offset, paintY);

            Console.Write(str);

            if (paintX * 0.5f - posX >= Width - 2 && GetStrWidth(str) > 2 - offset)
            {
                Console.SetCursorPosition(paintX + 2, paintY);

                Console.Write("█");
            }
        }

        private PointInfo[,] CreateMap(int _width, int _height)
        {
            PointInfo[,] crt = new PointInfo[Width, Height];

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    crt[i, j] = new PointInfo();
                    if (i == 0 || i == Width - 1 || j == 0 || j == Height - 1)
                    {
                        crt[i, j].Set(1);
                    }
                }
            }

            return crt;
        }

        private int GetStrWidth(string str)
        {
            return System.Text.Encoding.GetEncoding("GBK").GetByteCount(str);
        }

        private string[] GetStringFormat(string _str)
        {
            List<string> strs = new List<string>();

            string crtStr = string.Empty;
            for (int i = 0; i < _str.Length; i++)
            {
                if (crtStr.Length == 0)
                {
                    if (GetStrWidth(_str[i].ToString()) == 2)
                    {
                        strs.Add(_str[i].ToString());
                    }
                    else
                    {
                        crtStr = _str[i].ToString();
                    }
                }
                else
                {
                    crtStr += _str[i];
                    strs.Add(crtStr);
                    if (GetStrWidth(_str[i].ToString()) == 2)
                    {
                        crtStr = " ";
                    }
                    else
                    {
                        crtStr = string.Empty;
                    }
                }
            }
            if (crtStr.Length > 0)
            {
                strs.Add(crtStr);
            }

            return strs.ToArray();
        }

        public class PointInfo
        {
            private readonly string[] defaultValue = new string[]
            {
                "  ",
                "█",
                "□",
                "◎",
                "●",
                "▓",
                "☉"
            };

            public ConsoleColor color;

            public string str;

            public void Set(int index)
            {
                if (index < 0 || index > defaultValue.Length)
                {
                    index = 0;
                }

                str = defaultValue[index];
            }

            public PointInfo()
            {
                color = ConsoleColor.White;
                str = defaultValue[0];
            }

            public PointInfo Clone()
            {
                PointInfo pi = new PointInfo();

                pi.color = color;
                pi.str = str;

                return pi;
            }
        }
    }
}
