using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Nonsensical_Void
{
    class RetroSnakerConsoleOld
    {
        struct pos
        {
            public int x;
            public int y;

            public pos(int _x, int _y)
            {
                x = _x;
                y = _y;
            }
        }

        /// <summary>
        /// 地图蓝本
        /// </summary>
        static int[,] Original = new int[20, 20]
        {
            { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,2,2,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
        };

        /// <summary>
        /// 使用的地图
        /// </summary>
        static int[,] map = null;
        static bool end = false;
        static List<pos> snake = new List<pos>();
        static pos head, tail;
        static ConsoleKey crt_dir;
        static ConsoleKey last_dir;
        static int interval = 50;
        static int score = 0;
        static bool skip = false;
        static AutoResetEvent are = new AutoResetEvent(true);
        static bool Ok;
        static bool setfood;

       public static void RunGame()
        {
            Change_Difficulty();
            Thread getKey = new Thread(GetKey);
            Thread paint = new Thread(Running);
            paint.Start();
            getKey.Start();

            Console.CursorVisible = false;
            while (true)
            {
                Init();
                Paint_One(0, 21, "按回车键开始");
                Wait();
                Set_Food();
                end = false;
                are.Set();
                Paint_One(0, 21, "            ");
                while (!end)
                {

                }
                Paint_One(0, 21, "游戏结束！按回车键继续");
                Wait();
                Paint_One(0, 21, "                    ");
            }
        }

        static void Change_Difficulty()
        {
            int choice = 1, max = 3;
            while (true)
            {
                Console.Clear();
                if (choice == 1)
                {
                    Console.Write("◆");
                }
                else
                {
                    Console.Write("  ");
                }
                Console.WriteLine("低难度");
                if (choice == 2)
                {
                    Console.Write("◆");
                }
                else
                {
                    Console.Write("  ");
                }
                Console.WriteLine("中难度");
                if (choice == 3)
                {
                    Console.Write("◆");
                }
                else
                {
                    Console.Write("  ");
                }
                Console.WriteLine("高难度");
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.W:
                        if (choice == 1)
                        {
                            choice = max;
                        }
                        else
                        {
                            choice--;
                        }
                        break;
                    case ConsoleKey.S:
                        if (choice == max)
                        {
                            choice = 1;
                        }
                        else
                        {
                            choice++;
                        }
                        break;
                    case ConsoleKey.Enter:
                        switch (choice)
                        {
                            case 1:
                                interval = 500;
                                break;
                            case 2:
                                interval = 250;
                                break;
                            case 3:
                                interval = 50;
                                break;
                            default:
                                interval = 1000;
                                break;
                        }
                        return;
                }
            }
        }

        static void Wait()
        {
            Ok = false;

            while (!Ok)
            {

            }
        }

        static void Init()
        {
            setfood = false;
            snake.Clear();
            end = true;
            score = 0;
            map = (int[,])Original.Clone();
            crt_dir = ConsoleKey.A;
            last_dir = ConsoleKey.A;
            snake.Add(new pos(2, 15));
            snake.Add(new pos(2, 16));
            snake.Add(new pos(2, 17));
            head = snake[0];
            tail = snake[snake.Count - 1];
            Re_Paint();
        }

        static void GetKey()
        {
            while (true)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.A:
                        if (last_dir != ConsoleKey.D)
                        {
                            crt_dir = ConsoleKey.A;
                        }
                        break;
                    case ConsoleKey.D:
                        if (last_dir != ConsoleKey.A)
                        {
                            crt_dir = ConsoleKey.D;
                        }
                        break;
                    case ConsoleKey.S:
                        if (last_dir != ConsoleKey.W)
                        {
                            crt_dir = ConsoleKey.S;
                        }
                        break;
                    case ConsoleKey.W:
                        if (last_dir != ConsoleKey.S)
                        {
                            crt_dir = ConsoleKey.W;
                        }
                        break;
                    case ConsoleKey.Enter:
                        Ok = true;
                        break;
                    case ConsoleKey.Escape:
                        end = true;
                        break;
                }
                skip = true;
            }
        }

        static void Paint_One(int _x, int _y, string _s)
        {
            Console.SetCursorPosition(_x * 2, _y);
            Console.Write(_s);
        }

        static void Paint_One(int _x, int _y, int _i)
        {
            Console.SetCursorPosition(_x * 2, _y);
            switch (_i)
            {
                case 1:
                    Console.Write("█");
                    break;
                case 2:
                    Console.Write("◎");
                    break;
                case 3:
                    Console.Write("●");
                    break;
                case 4:
                    Console.Write("□");
                    break;
                default:
                    Console.Write("  ");
                    break;
            }
        }

        static void Re_Paint()
        {
            Console.Clear();
            for (int i = 0; i < Original.GetLength(0); i++)
            {
                for (int j = 0; j < Original.GetLength(1); j++)
                {
                    switch (map[i, j])
                    {
                        case 1:
                            Console.Write("█");
                            break;
                        case 2:
                            Console.Write("◎");
                            break;
                        case 3:
                            Console.Write("●");
                            break;
                        case 4:
                            Console.Write("□");
                            break;
                        default:
                            Console.Write("  ");
                            break;
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("分数：" + score);
        }

        static void Running()
        {
            pos target = new pos();
            while (true)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (skip)
                    {
                        break;
                    }
                    if (end)
                    {
                        are.WaitOne();
                    }
                    Thread.Sleep(interval / 10);
                }

                switch (crt_dir)
                {
                    case ConsoleKey.A:
                        last_dir = ConsoleKey.A;
                        target.x = head.x;
                        target.y = head.y - 1;
                        break;
                    case ConsoleKey.D:
                        last_dir = ConsoleKey.D;
                        target.x = head.x;
                        target.y = head.y + 1;
                        break;
                    case ConsoleKey.S:
                        last_dir = ConsoleKey.S;
                        target.x = head.x + 1;
                        target.y = head.y;
                        break;
                    case ConsoleKey.W:
                        last_dir = ConsoleKey.W;
                        target.x = head.x - 1;
                        target.y = head.y;
                        break;
                }

                if (map[target.x, target.y] == 1 || map[target.x, target.y] == 2)
                {
                    if (target.x != tail.x || target.y != tail.y)
                    {
                        end = true;
                    }
                    else
                    {
                        Change_One(head, 2);
                        head = target;
                        for (int i = snake.Count - 1; i > 0; i--)
                        {
                            snake[i] = snake[i - 1];
                        }
                        snake[0] = head;
                        Change_One(tail, 0);
                        tail = snake[snake.Count - 1];
                        Change_One(target, 3);
                    }
                }
                else
                {
                    Change_One(head, 2);
                    head = target;
                    for (int i = snake.Count - 1; i > 0; i--)
                    {
                        snake[i] = snake[i - 1];
                    }
                    snake[0] = head;


                    if (map[target.x, target.y] == 4)
                    {
                        score++;
                        snake.Add(new pos(tail.x, tail.y));
                        Paint_One(0, 20, "分数：" + score);
                        setfood = true;
                    }
                    else
                    {
                        Change_One(tail, 0);
                        tail = snake[snake.Count - 1];
                    }

                    Change_One(target, 3);
                }
                if (setfood)
                {
                    Set_Food();
                    setfood = false;
                }
                skip = false;
            }
        }

        static void Change_One(pos _p, int _i)
        {
            map[_p.x, _p.y] = _i;
            Paint_One(_p.y, _p.x, _i);
        }

        static void Set_Food()
        {
            Random random = new Random();
            int r_x = 0, r_y = 0;
            do
            {
                r_x = random.Next(1, 19);
                r_y = random.Next(1, 19);
            } while (map[r_x, r_y] == 2 || map[r_x, r_y] == 3);
            Change_One(new pos(r_x, r_y), 4);
        }
    }

    class RetroSnakerConsole
    {
        static ConsolePainter cp = new ConsolePainter(20, 22);
        static RetroSnaker rs = new RetroSnaker();

        static AutoResetEvent gameOver = new AutoResetEvent(false);

        public static void RunGame()
        {
            cp.Open(true);
            while (true)
            {
                cp.Clear();
                Change_Difficulty();
                cp.Clear();
                Paint();

                cp.Paint(1, rs.Height, "按回车键开始");

                while (Console.ReadKey(true).Key == ConsoleKey.Enter)
                {

                }

                cp.Paint(1, rs.Height, "            ");

                rs.Start(() => gameOver.Set(), Paint);
                gameOver.WaitOne();

                cp.Paint(1, rs.Height, "游戏结束！按回车键继续");

                while (Console.ReadKey(true).Key == ConsoleKey.Enter)
                {

                }

                cp.Paint(1, rs.Height, "                    ");
            }
        }

        static void Change_Difficulty()
        {
            int choice = 1, max = 3;

            cp.Paint(2, 1, "贪吃蛇");
            cp.Paint(1, 2, '◆');
            cp.Paint(2, 2, "低难度");
            cp.Paint(2, 3, "中难度");
            cp.Paint(2, 4, "高难度");

            while (true)
            {
                ConsoleKey ck = Console.ReadKey(true).Key;

                cp.Paint(1, choice + 1, "  ");

                switch (ck)
                {
                    case ConsoleKey.W:
                        if (choice == 1)
                        {
                            choice = max;
                        }
                        else
                        {
                            choice--;
                        }
                        break;
                    case ConsoleKey.S:
                        if (choice == max)
                        {
                            choice = 1;
                        }
                        else
                        {
                            choice++;
                        }
                        break;
                    case ConsoleKey.Enter:
                        switch (choice)
                        {
                            case 1:
                                rs.LoadMap(0, 500);
                                break;
                            case 2:
                                rs.LoadMap(0, 250);
                                break;
                            case 3:
                                rs.LoadMap(0, 50);
                                break;
                            default:
                                rs.LoadMap(0, 1000);
                                break;
                        }
                        return;
                }

                cp.Paint(1, choice + 1, '◆');
            }
        }

        static void Paint()
        {
            for (int i = 0; i < rs.Width; i++)
            {
                for (int j = 0; j < rs.Height; j++)
                {
                    cp.Paint(i, j, rs.Map[i, j].Type);
                }
            }
            cp.Paint(1, rs.Height + 1, "分数：" + rs.Score);

            cp.UpdateScreen();
        }
    }

    class RetroSnaker
    {
        public int MapCount { get; private set; } = 0;
        public int Width { get; private set; } = 0;
        public int Height { get; private set; } = 0;
        public int Score { get; private set; } = 0;
        public PointInfo[,] Map { get; private set; } = null;

        #region Maps
        private readonly static int[,] Map_0 = new int[20, 20]
        {
            { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,4,6,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
        };
        #endregion
        private List<int[,]> Maps = null;

        private int interval = 0;
        private bool isRun = false;
        private int Timer = 0;
        private Action gameOver;
        private Action change;
        private int crtDir = 0;

        private List<Point> snake = null;

        public RetroSnaker()
        {
            Maps = new List<int[,]>();
            Maps.Add(Map_0);
            MapCount = Maps.Count;
            gameOver += () => isRun = false;
        }

        public bool LoadMap(int index, int _interval = 50)
        {
            if (index < 0 || index >= MapCount)
            {
                return false;
            }

            Map = Rubbing(Maps[index]);

            Width = Map.GetLength(0);
            Height = Map.GetLength(1);

            snake = new List<Point>();

            #region Map_0 Only

            crtDir = 1;

            snake.Add(new Point(2, 15));
            snake.Add(new Point(2, 16));
            snake.Add(new Point(2, 17));

            #endregion

            interval = _interval;

            return true;
        }

        public void Start(Action _gameOver, Action _change)
        {
            isRun = true;
            Timer = 0;
            gameOver += _gameOver;
            change += _change;

            Thread Update = new Thread(RunUpdate);
            Update.Start();

            Thread getKey = new Thread(GetKey);
            getKey.Start();

            Set_Food();
        }

        private void RunUpdate()
        {
            while (isRun == true)
            {
                Thread.Sleep(20);
                Timer += 20;
                if (Timer >= interval)
                {
                    Crawl();
                    change?.Invoke();
                    int i = change.GetInvocationList().Length;
                    interval = 0;
                }
            }
        }

        private void Crawl()
        {
            int dirX = snake[0].x;
            int dirY = snake[0].y;

            switch (crtDir)
            {
                case 1:
                    dirX--;
                    break;
                case 2:
                    dirX++;
                    break;
                case 3:
                    dirY++;
                    break;
                case 4:
                    dirY--;
                    break;
            }


            if (Map[dirX, dirY].isDanger == true)
            {
                gameOver?.Invoke();
                return;
            }

            if (Map[snake[0].x, snake[0].y].EatFood() == true)
            {
                Score++;
                snake.Add(new Point(snake[snake.Count - 1].x, snake[snake.Count - 1].y)); ;
            }

            Map[snake[snake.Count - 1].x, snake[snake.Count - 1].y].isTail = false;
            Map[snake[snake.Count - 2].x, snake[snake.Count - 2].y].isTail = true;
            snake[snake.Count - 1] = snake[snake.Count - 2];

            for (int i = snake.Count - 2; i > 1; i--)
            {
                snake[i] = snake[i - 1];
                Map[snake[i].x, snake[i].y].isBody = true;
            }

            Map[snake[0].x, snake[0].y].isHead = false;
            Map[dirX, dirY].isHead = true;
            snake[0] = new Point(dirX, dirY);
        }

        private void GetKey()
        {
            while (true)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.A:
                        ChangeDir(1);
                        break;
                    case ConsoleKey.D:
                        ChangeDir(2);
                        break;
                    case ConsoleKey.S:
                        ChangeDir(3);
                        break;
                    case ConsoleKey.W:
                        ChangeDir(4);
                        break;
                    case ConsoleKey.Escape:
                        gameOver?.Invoke();
                        break;
                }
            }
        }

        private void Set_Food()
        {
            Random random = new Random();

            int r_x;
            int r_y;

            do
            {
                r_x = random.Next(1, 20);
                r_y = random.Next(1, 20);
            } while (Map[r_x, r_y].SetFood() == false);
        }

        private void ChangeDir(int dir)
        {
            switch (dir)
            {
                case 1:
                    if (crtDir != 2)
                    {
                        crtDir = 1;
                    }
                    break;
                case 2:
                    if (crtDir != 1)
                    {
                        crtDir = 2;
                    }
                    break;
                case 3:
                    if (crtDir != 4)
                    {
                        crtDir = 3;
                    }
                    break;
                case 4:
                    if (crtDir != 3)
                    {
                        crtDir = 4;
                    }
                    break;
            }
            Timer = interval;
        }

        /// <summary>
        /// 获取原始蓝本的拓本
        /// </summary>
        /// <returns></returns>
        private PointInfo[,] Rubbing(int[,] blueprint)
        {
            PointInfo[,] crt = new PointInfo[blueprint.GetLength(1), blueprint.GetLength(0)];

            for (int i = 0; i < blueprint.GetLength(1); i++)
            {
                for (int j = 0; j < blueprint.GetLength(0); j++)
                {
                    crt[i, j] = new PointInfo(blueprint[j, i]);
                }
            }

            return crt;
        }

        public struct Point
        {
            public int x;
            public int y;

            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        public class PointInfo
        {
            public bool isWall = false;
            public bool isHead = false;
            public bool isBody = false;
            public bool isTail = false;
            public bool hasFood = false;

            public bool isDanger
            {
                get
                {
                    if (isWall == true)
                    {
                        return true;
                    }
                    if (isBody == true)
                    {
                        return true;
                    }
                    if (isTail == true)
                    {
                        return true;
                    }
                    return false;
                }
            }

            public int Type
            {
                get
                {
                    if (isWall == true)
                    {
                        return 1;
                    }
                    if (isHead == true)
                    {
                        return 3;
                    }
                    if (isBody == true)
                    {
                        return 4;
                    }
                    if (isTail == true)
                    {
                        return 6;
                    }
                    if (hasFood == true)
                    {
                        return 2;
                    }

                    return 0;
                }
            }

            public PointInfo(int type)
            {
                switch (type)
                {
                    case 1:
                        isWall = true;
                        break;
                    case 3:
                        isHead = true;
                        break;
                    case 4:
                        isBody = true;
                        break;
                    case 6:
                        isTail = true;
                        break;
                    case 2:
                        hasFood = true;
                        break;
                }
            }

            public bool SetFood()
            {
                if (isWall == true)
                {
                    return false;
                }
                if (isHead == true)
                {
                    return false;
                }
                if (isBody == true)
                {
                    return false;
                }
                if (isTail == true)
                {
                    return false;
                }
                if (hasFood == true)
                {
                    return false;
                }

                hasFood = true;
                return true;
            }

            public bool EatFood()
            {
                if (hasFood == true)
                {
                    hasFood = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
