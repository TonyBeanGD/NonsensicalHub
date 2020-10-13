using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Nonsensical_Void
{
    class TetrisConsole
    {
        /// <summary>
        /// 方块类型
        /// </summary>
        enum Block
        {
            I,//青色
            J,//蓝色
            L,//橙色
            O,//黄色
            Z,//红色
            S,//绿色
            T,//紫色
        }

        /// <summary>
        /// 代表方块的结构体
        /// </summary>
        struct Point
        {
            public int dir;
            public Block block;

            public int x;
            public int y;

            public int x_1
            {
                get
                {
                    switch (block)
                    {
                        case Block.I:
                            switch (dir)
                            {
                                case 1:
                                    return x - 1;
                                case 2:
                                    return x;
                                case 3:
                                    return x - 1;
                                case 4:
                                    return x;
                            }
                            break;
                        case Block.J:
                            switch (dir)
                            {
                                case 1:
                                    return x - 1;
                                case 2:
                                    return x;
                                case 3:
                                    return x + 1;
                                case 4:
                                    return x;
                            }
                            break;
                        case Block.L:
                            switch (dir)
                            {
                                case 1:
                                    return x - 1;
                                case 2:
                                    return x;
                                case 3:
                                    return x + 1;
                                case 4:
                                    return x;
                            }
                            break;
                        case Block.O:
                            switch (dir)
                            {
                                case 1:
                                    return x;
                                case 2:
                                    return x;
                                case 3:
                                    return x;
                                case 4:
                                    return x;
                            }
                            break;
                        case Block.Z:
                            switch (dir)
                            {
                                case 1:
                                    return x;
                                case 2:
                                    return x - 1;
                                case 3:
                                    return x;
                                case 4:
                                    return x - 1;
                            }
                            break;
                        case Block.S:
                            switch (dir)
                            {
                                case 1:
                                    return x;
                                case 2:
                                    return x + 1;
                                case 3:
                                    return x;
                                case 4:
                                    return x + 1;
                            }
                            break;
                        case Block.T:
                            switch (dir)
                            {
                                case 1:
                                    return x;
                                case 2:
                                    return x - 1;
                                case 3:
                                    return x;
                                case 4:
                                    return x + 1;
                            }
                            break;
                    }
                    return 0;
                }
            }
            public int y_1
            {
                get
                {
                    switch (block)
                    {
                        case Block.I:
                            switch (dir)
                            {
                                case 1:
                                    return y;
                                case 2:
                                    return y + 1;
                                case 3:
                                    return y;
                                case 4:
                                    return y + 1;
                            }
                            break;
                        case Block.J:
                            switch (dir)
                            {
                                case 1:
                                    return y;
                                case 2:
                                    return y + 1;
                                case 3:
                                    return y;
                                case 4:
                                    return y - 1;
                            }
                            break;
                        case Block.L:
                            switch (dir)
                            {
                                case 1:
                                    return y;
                                case 2:
                                    return y + 1;
                                case 3:
                                    return y;
                                case 4:
                                    return y - 1;
                            }
                            break;
                        case Block.O:
                            switch (dir)
                            {
                                case 1:
                                    return y - 1;
                                case 2:
                                    return y - 1;
                                case 3:
                                    return y - 1;
                                case 4:
                                    return y - 1;
                            }
                            break;
                        case Block.Z:
                            switch (dir)
                            {
                                case 1:
                                    return y - 1;
                                case 2:
                                    return y;
                                case 3:
                                    return y - 1;
                                case 4:
                                    return y;
                            }
                            break;
                        case Block.S:
                            switch (dir)
                            {
                                case 1:
                                    return y + 1;
                                case 2:
                                    return y;
                                case 3:
                                    return y + 1;
                                case 4:
                                    return y;
                            }
                            break;
                        case Block.T:
                            switch (dir)
                            {
                                case 1:
                                    return y - 1;
                                case 2:
                                    return y;
                                case 3:
                                    return y + 1;
                                case 4:
                                    return y;
                            }
                            break;
                    }
                    return 0;
                }
            }

            public int x_2
            {
                get
                {
                    switch (block)
                    {
                        case Block.I:
                            switch (dir)
                            {
                                case 1:
                                    return x;
                                case 2:
                                    return x;
                                case 3:
                                    return x;
                                case 4:
                                    return x;
                            }
                            break;
                        case Block.J:
                            switch (dir)
                            {
                                case 1:
                                    return x;
                                case 2:
                                    return x;
                                case 3:
                                    return x;
                                case 4:
                                    return x;
                            }
                            break;
                        case Block.L:
                            switch (dir)
                            {
                                case 1:
                                    return x;
                                case 2:
                                    return x;
                                case 3:
                                    return x;
                                case 4:
                                    return x;
                            }
                            break;
                        case Block.O:
                            switch (dir)
                            {
                                case 1:
                                    return x;
                                case 2:
                                    return x;
                                case 3:
                                    return x;
                                case 4:
                                    return x;
                            }
                            break;
                        case Block.Z:
                            switch (dir)
                            {
                                case 1:
                                    return x;
                                case 2:
                                    return x;
                                case 3:
                                    return x;
                                case 4:
                                    return x;
                            }
                            break;
                        case Block.S:
                            switch (dir)
                            {
                                case 1:
                                    return x;
                                case 2:
                                    return x;
                                case 3:
                                    return x;
                                case 4:
                                    return x;
                            }
                            break;
                        case Block.T:
                            switch (dir)
                            {
                                case 1:
                                    return x;
                                case 2:
                                    return x;
                                case 3:
                                    return x;
                                case 4:
                                    return x;
                            }
                            break;
                    }
                    return 0;
                }
            }
            public int y_2
            {
                get
                {
                    switch (block)
                    {
                        case Block.I:
                            switch (dir)
                            {
                                case 1:
                                    return y;
                                case 2:
                                    return y;
                                case 3:
                                    return y;
                                case 4:
                                    return y;
                            }
                            break;
                        case Block.J:
                            switch (dir)
                            {
                                case 1:
                                    return y;
                                case 2:
                                    return y;
                                case 3:
                                    return y;
                                case 4:
                                    return y;
                            }
                            break;
                        case Block.L:
                            switch (dir)
                            {
                                case 1:
                                    return y;
                                case 2:
                                    return y;
                                case 3:
                                    return y;
                                case 4:
                                    return y;
                            }
                            break;
                        case Block.O:
                            switch (dir)
                            {
                                case 1:
                                    return y;
                                case 2:
                                    return y;
                                case 3:
                                    return y;
                                case 4:
                                    return y;
                            }
                            break;
                        case Block.Z:
                            switch (dir)
                            {
                                case 1:
                                    return y;
                                case 2:
                                    return y;
                                case 3:
                                    return y;
                                case 4:
                                    return y;
                            }
                            break;
                        case Block.S:
                            switch (dir)
                            {
                                case 1:
                                    return y;
                                case 2:
                                    return y;
                                case 3:
                                    return y;
                                case 4:
                                    return y;
                            }
                            break;
                        case Block.T:
                            switch (dir)
                            {
                                case 1:
                                    return y;
                                case 2:
                                    return y;
                                case 3:
                                    return y;
                                case 4:
                                    return y;
                            }
                            break;
                    }
                    return 0;
                }
            }

            public int x_3
            {
                get
                {
                    switch (block)
                    {
                        case Block.I:
                            switch (dir)
                            {
                                case 1:
                                    return x + 1;
                                case 2:
                                    return x;
                                case 3:
                                    return x + 1;
                                case 4:
                                    return x;
                            }
                            break;
                        case Block.J:
                            switch (dir)
                            {
                                case 1:
                                    return x + 1;
                                case 2:
                                    return x;
                                case 3:
                                    return x - 1;
                                case 4:
                                    return x;
                            }
                            break;
                        case Block.L:
                            switch (dir)
                            {
                                case 1:
                                    return x + 1;
                                case 2:
                                    return x;
                                case 3:
                                    return x - 1;
                                case 4:
                                    return x;
                            }
                            break;
                        case Block.O:
                            switch (dir)
                            {
                                case 1:
                                    return x + 1;
                                case 2:
                                    return x + 1;
                                case 3:
                                    return x + 1;
                                case 4:
                                    return x + 1;
                            }
                            break;
                        case Block.Z:
                            switch (dir)
                            {
                                case 1:
                                    return x + 1;
                                case 2:
                                    return x;
                                case 3:
                                    return x + 1;
                                case 4:
                                    return x;
                            }
                            break;
                        case Block.S:
                            switch (dir)
                            {
                                case 1:
                                    return x + 1;
                                case 2:
                                    return x;
                                case 3:
                                    return x + 1;
                                case 4:
                                    return x;
                            }
                            break;
                        case Block.T:
                            switch (dir)
                            {
                                case 1:
                                    return x;
                                case 2:
                                    return x + 1;
                                case 3:
                                    return x;
                                case 4:
                                    return x - 1;
                            }
                            break;
                    }
                    return 0;
                }
            }
            public int y_3
            {
                get
                {
                    switch (block)
                    {
                        case Block.I:
                            switch (dir)
                            {
                                case 1:
                                    return y;
                                case 2:
                                    return y - 1;
                                case 3:
                                    return y;
                                case 4:
                                    return y - 1;
                            }
                            break;
                        case Block.J:
                            switch (dir)
                            {
                                case 1:
                                    return y;
                                case 2:
                                    return y - 1;
                                case 3:
                                    return y;
                                case 4:
                                    return y + 1;
                            }
                            break;
                        case Block.L:
                            switch (dir)
                            {
                                case 1:
                                    return y;
                                case 2:
                                    return y - 1;
                                case 3:
                                    return y;
                                case 4:
                                    return y + 1;
                            }
                            break;
                        case Block.O:
                            switch (dir)
                            {
                                case 1:
                                    return y - 1;
                                case 2:
                                    return y - 1;
                                case 3:
                                    return y - 1;
                                case 4:
                                    return y - 1;
                            }
                            break;
                        case Block.Z:
                            switch (dir)
                            {
                                case 1:
                                    return y;
                                case 2:
                                    return y - 1;
                                case 3:
                                    return y;
                                case 4:
                                    return y - 1;
                            }
                            break;
                        case Block.S:
                            switch (dir)
                            {
                                case 1:
                                    return y;
                                case 2:
                                    return y - 1;
                                case 3:
                                    return y;
                                case 4:
                                    return y - 1;
                            }
                            break;
                        case Block.T:
                            switch (dir)
                            {
                                case 1:
                                    return y + 1;
                                case 2:
                                    return y;
                                case 3:
                                    return y - 1;
                                case 4:
                                    return y;
                            }
                            break;
                    }
                    return 0;
                }
            }

            public int x_4
            {
                get
                {
                    switch (block)
                    {
                        case Block.I:
                            switch (dir)
                            {
                                case 1:
                                    return x + 2;
                                case 2:
                                    return x;
                                case 3:
                                    return x + 2;
                                case 4:
                                    return x;
                            }
                            break;
                        case Block.J:
                            switch (dir)
                            {
                                case 1:
                                    return x + 1;
                                case 2:
                                    return x - 1;
                                case 3:
                                    return x - 1;
                                case 4:
                                    return x + 1;
                            }
                            break;
                        case Block.L:
                            switch (dir)
                            {
                                case 1:
                                    return x + 1;
                                case 2:
                                    return x + 1;
                                case 3:
                                    return x - 1;
                                case 4:
                                    return x - 1;
                            }
                            break;
                        case Block.O:
                            switch (dir)
                            {
                                case 1:
                                    return x + 1;
                                case 2:
                                    return x + 1;
                                case 3:
                                    return x + 1;
                                case 4:
                                    return x + 1;
                            }
                            break;
                        case Block.Z:
                            switch (dir)
                            {
                                case 1:
                                    return x + 1;
                                case 2:
                                    return x + 1;
                                case 3:
                                    return x + 1;
                                case 4:
                                    return x + 1;
                            }
                            break;
                        case Block.S:
                            switch (dir)
                            {
                                case 1:
                                    return x + 1;
                                case 2:
                                    return x - 1;
                                case 3:
                                    return x + 1;
                                case 4:
                                    return x - 1;
                            }
                            break;
                        case Block.T:
                            switch (dir)
                            {
                                case 1:
                                    return x - 1;
                                case 2:
                                    return x;
                                case 3:
                                    return x + 1;
                                case 4:
                                    return x;
                            }
                            break;
                    }
                    return 0;
                }
            }
            public int y_4
            {
                get
                {
                    switch (block)
                    {
                        case Block.I:
                            switch (dir)
                            {
                                case 1:
                                    return y;
                                case 2:
                                    return y + 2;
                                case 3:
                                    return y;
                                case 4:
                                    return y + 2;
                            }
                            break;
                        case Block.J:
                            switch (dir)
                            {
                                case 1:
                                    return y - 1;
                                case 2:
                                    return y - 1;
                                case 3:
                                    return y + 1;
                                case 4:
                                    return y + 1;
                            }
                            break;
                        case Block.L:
                            switch (dir)
                            {
                                case 1:
                                    return y + 1;
                                case 2:
                                    return y - 1;
                                case 3:
                                    return y - 1;
                                case 4:
                                    return y + 1;
                            }
                            break;
                        case Block.O:
                            switch (dir)
                            {
                                case 1:
                                    return y;
                                case 2:
                                    return y;
                                case 3:
                                    return y;
                                case 4:
                                    return y;
                            }
                            break;
                        case Block.Z:
                            switch (dir)
                            {
                                case 1:
                                    return y + 1;
                                case 2:
                                    return y - 1;
                                case 3:
                                    return y + 1;
                                case 4:
                                    return y - 1;
                            }
                            break;
                        case Block.S:
                            switch (dir)
                            {
                                case 1:
                                    return y - 1;
                                case 2:
                                    return y - 1;
                                case 3:
                                    return y - 1;
                                case 4:
                                    return y - 1;
                            }
                            break;
                        case Block.T:
                            switch (dir)
                            {
                                case 1:
                                    return y;
                                case 2:
                                    return y + 1;
                                case 3:
                                    return y;
                                case 4:
                                    return y - 1;
                            }
                            break;
                    }
                    return 0;
                }
            }

            public Point(int _x, int _y, Block _b)
            {
                dir = 1;
                block = _b;
                x = _x;
                y = _y;
            }
        }

        /// <summary>
        /// 代表原始地图的二维数组
        /// </summary>
        static int[,] original = new int[22, 12]
        {
            { 1,1,1,1,1,1,1,1,1,1,1,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,1,1,1,1,1,1,1,1,1,1,1 },
        };

        /// <summary>
        /// 代表使用中地图的二维数组
        /// </summary>
        static int[,] map;

        /// <summary>
        /// 每帧长度
        /// </summary>
        static int interval = 500;

        /// <summary>
        /// 游戏结束变量
        /// </summary>
        static bool end;

        /// <summary>
        /// 分数变量
        /// </summary>
        static int score;

        /// <summary>
        /// 代表当前控制的方块
        /// </summary>
        static Point point;

        static bool skip;

        static Point nextblock;

        static object locker = new object();

        static object locker2 = new object();

        static Point fall_point;

        static void RunGame()
        {
            Console.CursorVisible = false;
            Thread running = new Thread(Running);
            while (true)
            {
                Init();
                running = new Thread(Running);
                running.Start();
                GetKey();
                running.Abort();
            }
        }

        static void Init()
        {
            skip = false;
            score = 0;
            end = false;
            map = (int[,])original.Clone();
            Console.Clear();
            Console.WriteLine("按任意键开始");
            Console.ReadKey(true);


            Get_Next();

            Create_Block();

            Paint();

            Get_Next();
        }

        /// <summary>
        /// 获取当前按键并作出对应反应
        /// </summary>
        static void GetKey()
        {
            while (!end)
            {
                Fall();
                ConsoleKey ck = Console.ReadKey(true).Key;
                lock (locker)
                {
                    Point temp = point;
                    switch (ck)
                    {
                        case ConsoleKey.Escape:
                            end = true;
                            break;
                        case ConsoleKey.A:
                            temp.y--;
                            if (Other_Check(temp))
                            {
                                Move_Block(temp);
                            }
                            break;
                        case ConsoleKey.LeftArrow:
                            temp.y--;
                            if (Other_Check(temp))
                            {
                                Move_Block(temp);
                            }
                            break;
                        case ConsoleKey.D:
                            temp.y++;
                            if (Other_Check(temp))
                            {
                                Move_Block(temp);
                            }
                            break;
                        case ConsoleKey.RightArrow:
                            temp.y++;
                            if (Other_Check(temp))
                            {
                                Move_Block(temp);
                            }
                            break;
                        case ConsoleKey.S:
                            Drop();
                            break;
                        case ConsoleKey.DownArrow:
                            Drop();
                            break;
                        case ConsoleKey.W:
                            if (temp.dir == 4)
                            {
                                temp.dir = 1;
                            }
                            else
                            {
                                temp.dir++;
                            }
                            if (Other_Check(temp))
                            {
                                Move_Block(temp);
                            }
                            break;
                        case ConsoleKey.UpArrow:
                            if (temp.dir == 4)
                            {
                                temp.dir = 1;
                            }
                            else
                            {
                                temp.dir++;
                            }
                            if (Other_Check(temp))
                            {
                                Move_Block(temp);
                            }
                            break;
                    }
                }
            }
        }

        static bool Point_Check(int _x, int _y)
        {
            if (_x == point.x_1 && _y == point.y_1)
            {
                return true;
            }
            if (_x == point.x_2 && _y == point.y_2)
            {
                return true;
            }
            if (_x == point.x_3 && _y == point.y_3)
            {
                return true;
            }
            if (_x == point.x_4 && _y == point.y_4)
            {
                return true;
            }
            return false;
        }

        static bool Other_Check(Point _p)
        {
            if (map[_p.x_1, _p.y_1] != 0 && !Point_Check(_p.x_1, _p.y_1))
            {
                return false;
            }
            if (map[_p.x_2, _p.y_2] != 0 && !Point_Check(_p.x_2, _p.y_2))
            {
                return false;
            }
            if (map[_p.x_3, _p.y_3] != 0 && !Point_Check(_p.x_3, _p.y_3))
            {
                return false;
            }
            if (map[_p.x_4, _p.y_4] != 0 && !Point_Check(_p.x_4, _p.y_4))
            {
                return false;
            }
            return true;
        }

        static void Drop()
        {
            skip = true;
            while (!Down())
            {
                Thread.Sleep(interval / 10);
            }
            skip = false;
        }

        static void Drop(Point _p)
        {
            while (true)
            {
                if (Down(_p))
                {
                    break;
                }
                else
                {
                    _p.x++;
                }
            }
        }

        static bool Down()
        {
            Point temp = point;
            temp.x++;
            if (!Other_Check(temp))
            {
                Row_Check();
                return true;
            }
            else
            {
                Move_Block(temp);
                return false;
            }
        }
        static bool Down(Point _p)
        {
            Point temp = _p;
            temp.x++;
            if (!Other_Check(temp))
            {
                fall_point = _p;
                return true;
            }
            else
            {
                return false;
            }
        }

        static void Move_Block(Point _p)
        {
            int type = (int)point.block + 2;
            Chance_Map(point, 0);
            Chance_Map(_p, type);
            Paint(point, 0);
            Paint(_p, type);
            point = _p;
        }

        static void Chance_Map(Point _p, int _i)
        {
            map[_p.x_1, _p.y_1] = _i;
            map[_p.x_2, _p.y_2] = _i;
            map[_p.x_3, _p.y_3] = _i;
            map[_p.x_4, _p.y_4] = _i;
        }

        static void Paint(Point _p, int _type)
        {
            Paint(_p.x_1, _p.y_1, _type);
            Paint(_p.x_2, _p.y_2, _type);
            Paint(_p.x_3, _p.y_3, _type);
            Paint(_p.x_4, _p.y_4, _type);
        }

        static void Paint(Point _p, int _type, string _s)
        {
            Paint(_p.x_1, _p.y_1, _type, _s);
            Paint(_p.x_2, _p.y_2, _type, _s);
            Paint(_p.x_3, _p.y_3, _type, _s);
            Paint(_p.x_4, _p.y_4, _type, _s);
        }

        /// <summary>
        /// 根据地图二维数组完全重新绘制地图
        /// </summary>
        static void Paint()
        {
            Console.Clear();
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    switch (map[i, j])
                    {
                        case 1:
                            Console.Write("█");
                            break;
                        case 2:
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write("■");
                            Console.ResetColor();
                            break;
                        case 3:
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write("■");
                            Console.ResetColor();
                            break;
                        case 4:
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Write("■");
                            Console.ResetColor();
                            break;
                        case 5:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("■");
                            Console.ResetColor();
                            break;
                        case 6:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("■");
                            Console.ResetColor();
                            break;
                        case 7:
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("■");
                            Console.ResetColor();
                            break;
                        case 8:
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.Write("■");
                            Console.ResetColor();
                            break;
                        default:
                            Console.Write("  ");
                            break;
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("分数：" + score);
            Console.SetCursorPosition(23, 1);
        }

        /// <summary>
        /// 在指定位置根据传过来的对应_i值绘制对应符号
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_i"></param>
        static void Paint(int _x, int _y, int _i)
        {
            Console.SetCursorPosition(_y * 2, _x);
            switch (_i)
            {
                case 1:
                    Console.Write("█");
                    break;
                case 2:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("■");
                    Console.ResetColor();
                    break;
                case 3:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("■");
                    Console.ResetColor();
                    break;
                case 4:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("■");
                    Console.ResetColor();
                    break;
                case 5:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("■");
                    Console.ResetColor();
                    break;
                case 6:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("■");
                    Console.ResetColor();
                    break;
                case 7:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("■");
                    Console.ResetColor();
                    break;
                case 8:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write("■");
                    Console.ResetColor();
                    break;
                default:
                    Console.Write("  ");
                    break;
            }
        }

        static void Paint(int _x, int _y, int _i, string _s)
        {
            Console.SetCursorPosition(_y * 2, _x);
            switch (_i)
            {
                case 2:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(_s);
                    Console.ResetColor();
                    break;
                case 3:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(_s);
                    Console.ResetColor();
                    break;
                case 4:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write(_s);
                    Console.ResetColor();
                    break;
                case 5:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(_s);
                    Console.ResetColor();
                    break;
                case 6:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(_s);
                    Console.ResetColor();
                    break;
                case 7:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(_s);
                    Console.ResetColor();
                    break;
                case 8:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write(_s);
                    Console.ResetColor();
                    break;
                default:
                    Console.Write("  ");
                    break;
            }
        }

        /// <summary>
        /// 在制定位置绘制字符串
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_s"></param>
        static void Paint(int _x, int _y, string _s)
        {
            Console.SetCursorPosition(_y * 2, _x);
            Console.Write(_s);
        }

        /// <summary>
        /// 运行线程，用于控制游戏稳定运行
        /// </summary>
        static void Running()
        {
            while (true)
            {
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(interval / 10);

                    if (skip)
                    {
                        break;
                    }

                    if (end)
                    {
                        Paint(23, 0, "游戏结束，按任意键继续");
                        return;
                    }
                }
                if (!skip)
                {
                    lock (locker)
                    {
                        Down();
                    }
                }
            }
        }

        static void Fall()
        {
            lock (locker2)
            {
                if (fall_point.x != 0)
                {
                    Clean_Fall();
                }

                if (point.x == 0)
                {
                    return;
                }

                fall_point = point;

                Drop(fall_point);

                if (Check_Fall())
                {
                    Paint(fall_point, (int)fall_point.block + 2, "□");
                }
            }
        }

        static void Clean_Fall()
        {
            if (map[fall_point.x_1, fall_point.y_1] == 0)
            {
                Paint(fall_point.x_1, fall_point.y_1, 0);
            }
            if (map[fall_point.x_2, fall_point.y_2] == 0)
            {
                Paint(fall_point.x_2, fall_point.y_2, 0);
            }
            if (map[fall_point.x_3, fall_point.y_3] == 0)
            {
                Paint(fall_point.x_3, fall_point.y_3, 0);
            }
            if (map[fall_point.x_4, fall_point.y_4] == 0)
            {
                Paint(fall_point.x_4, fall_point.y_4, 0);
            }
        }

        static bool Check_Fall()
        {
            if (map[fall_point.x_1, fall_point.y_1] != 0)
            {
                return false;
            }
            if (map[fall_point.x_2, fall_point.y_2] != 0)
            {
                return false;
            }
            if (map[fall_point.x_3, fall_point.y_3] != 0)
            {
                return false;
            }
            if (map[fall_point.x_4, fall_point.y_4] != 0)
            {
                return false;
            }
            return true;
        }

        static void Get_Next()
        {
            Random random = new Random(DateTime.Now.Millisecond);
            int r = random.Next(7);
            nextblock = new Point(2, 14, (Block)r);
            Paint(nextblock, (int)nextblock.block + 2);
        }

        /// <summary>
        /// 创建方块
        /// </summary>
        static void Create_Block()
        {
            point = nextblock;
            point.x = 2;
            point.y = 5;
            Chance_Map(point, (int)point.block + 2);
            Paint(point, (int)point.block + 2);
            Paint(nextblock, 0);
            Get_Next();
            Fall();
        }

        static void Row_Check()
        {
            bool fin = true;
            for (int i = 5; i < 21; i++)
            {
                fin = true;
                for (int j = 1; j < 11; j++)
                {
                    if (map[i, j] == 0)
                    {
                        fin = false;
                        break;
                    }
                }
                if (fin)
                {
                    Row_Drop(i);
                    i--;
                }
            }
            if (Death_Check())
            {
                end = true;
                return;
            }
            Create_Block();
        }

        static void Row_Drop(int _row)
        {
            for (int i = _row; i > 4; i--)
            {
                for (int j = 1; j < 11; j++)
                {
                    map[i, j] = map[i - 1, j];
                }
            }
            score++;
            Paint();
        }

        static bool Death_Check()
        {
            for (int i = 1; i < 5; i++)
            {
                for (int j = 1; j < 11; j++)
                {
                    if (map[i, j] != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    class Tetris
    {
       
    }
}
