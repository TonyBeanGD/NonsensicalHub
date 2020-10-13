using System;
using System.Collections.Generic;
using System.Text;

namespace Nonsensical_Void
{
    static class SkonbonConsole
    {
        private static Skonbon skonbon = new Skonbon();

        private static ConsolePainter cp = new ConsolePainter(27, 25);

        public static void RunGame()
        {
            cp.Open(false);

            while (true)
            {
                cp.Clear();
                if (Main_Menu() == false)
                {
                    return;
                }

                cp.Clear();

                skonbon.LoadLevel(Choice_level());

                cp.Clear();

                RunTime();
            }
        }

        /// <summary>
        /// 主菜单
        /// </summary>
        /// <returns></returns>
        private static bool Main_Menu()
        {
            int choice = 1, max = 2;

            cp.Paint(2, 1, " 推箱子");

            cp.Paint(1, 2, '◆');

            cp.Paint(2, 2, "选择关卡");

            cp.Paint(2, 3, "退出游戏");


            cp.UpdateScreen();

            while (true)
            {
                ConsoleKey ck = Console.ReadKey(true).Key;

                cp.Paint(1, choice + 1, "  ");

                switch (ck)
                {
                    case ConsoleKey.A:
                        if (choice == 1)
                        {
                            choice = max;
                        }
                        else
                        {
                            choice--;
                        }
                        break;
                    case ConsoleKey.D:
                        if (choice == max)
                        {
                            choice = 1;
                        }
                        else
                        {
                            choice++;
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
                    case ConsoleKey.Enter:
                        if (choice == 1)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                }

                cp.Paint(1, choice + 1, "◆");

                cp.UpdateScreen();
            }
        }

        /// <summary>
        /// 选择关卡
        /// </summary>
        /// <returns></returns>
        private static int Choice_level()
        {
            int level = 0;

            cp.Paint(2, 1, "请选择关卡");


            cp.Paint(1, 2, '◆');

            for (int i = 0; i < skonbon.LevelCount; i++)
            {
                cp.Paint((i % 5) * 5 + 2, (i / 5) + 2, $"关卡{i}");
            }


            cp.UpdateScreen();

            while (true)
            {
                ConsoleKey ck = Console.ReadKey(true).Key;

                cp.Paint((level % 5) * 5 + 1, (level / 5) + 2, "  ");

                switch (ck)
                {
                    case ConsoleKey.A:
                        if (level == 0)
                        {
                            level = skonbon.LevelCount - 1;
                        }
                        else
                        {
                            level--;
                        }
                        break;
                    case ConsoleKey.D:
                        if (level == skonbon.LevelCount - 1)
                        {
                            level = 0;
                        }
                        else
                        {
                            level++;
                        }
                        break;
                    case ConsoleKey.S:
                        if (level > skonbon.LevelCount - 6)
                        {
                            level = skonbon.LevelCount - 1;
                        }
                        else
                        {
                            level += 5;
                        }
                        break;
                    case ConsoleKey.W:
                        if (level < 5)
                        {
                            level = 0;
                        }
                        else
                        {
                            level-=5;
                        }
                        break;
                    case ConsoleKey.Enter:
                        return level;
                }

                cp.Paint((level % 5) * 5 + 1, (level / 5) + 2, '◆');

                cp.UpdateScreen();
            }
        }

        /// <summary>
        /// 主运行函数
        /// </summary>
        private static void RunTime()
        {
            Paint();

            cp.Paint(1, skonbon.Height + 4, "  adsw移动,c撤销,r重新开始");
            cp.Paint(1, skonbon.Height + 5, "  █ 墙           □箱子");
            cp.Paint(1, skonbon.Height + 6, "  ◎目标点        ●玩家");
            cp.Paint(1, skonbon.Height + 7, "  ▓完成的目标点  ☉目标点上的玩家");
            cp.Paint(1, skonbon.Height + 8, "  esc键退出");
            cp.UpdateScreen();

            ConsoleKey ck;
            while (true)
            {
                Paint();

                cp.UpdateScreen();

                switch (ck = Console.ReadKey(true).Key)
                {
                    case ConsoleKey.R:
                        skonbon.Reset();
                        break;
                    case ConsoleKey.C:
                        skonbon.Revoke();
                        break;
                    case ConsoleKey.Escape:
                        return;
                    default:
                        skonbon.Move(ck);
                        break;
                }
                if (skonbon.IsComplete)
                {
                    cp.Clear();
                    cp.Paint(1, 1, "恭喜通关!按任意键继续");
                    cp.UpdateScreen();
                    Console.ReadKey();
                    return;
                }
            }
        }

        /// <summary>
        /// 绘制
        /// </summary>
        private static void Paint()
        {
            for (int i = 0; i < skonbon.Width; i++)
            {
                for (int j = 0; j < skonbon.Height; j++)
                {
                    cp.Paint(i + 2, j + 2, skonbon.PointMap[i, j].Type);
                }
            }

            cp.Paint(1, skonbon.Height + 3, "  已用步数:" + skonbon.StepCount);
        }
    }

    class Skonbon
    {
        public bool IsComplete
        {
            get
            {
                foreach (var item in PointMap)
                {
                    if (item.isTarget == true && item.hasBox == false)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public int StepCount
        {
            get
            {
                return step.Count;
            }
        }

        public int Width { get; private set; } = 0;
        public int Height { get; private set; } = 0;
        public int LevelCount { get; private set; } = 0;

        public PointInfo[,] PointMap { get; private set; } = null;

        #region LevelsMap
        private readonly static int[,] Level_0 = new int[10, 10]
          {
            { 1,1,1,1,1,1,1,1,1,1 },
            { 1,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,3,0,1 },
            { 1,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,2,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,1 },
            { 1,0,4,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,1 },
            { 1,1,1,1,1,1,1,1,1,1 },
          };
        private readonly static int[,] Level_1 = new int[11, 19]
        {
            { 0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0},
            { 0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0},
            { 0,0,0,0,1,2,0,0,1,0,0,0,0,0,0,0,0,0,0},
            { 0,0,1,1,1,0,0,2,1,1,0,0,0,0,0,0,0,0,0},
            { 0,0,1,0,0,2,0,2,0,1,0,0,0,0,0,0,0,0,0},
            { 1,1,1,0,1,0,1,1,0,1,0,0,0,1,1,1,1,1,1},
            { 1,0,0,0,1,0,1,1,0,1,1,1,1,1,0,0,3,3,1},
            { 1,0,2,0,0,2,0,0,0,0,0,0,0,0,0,0,3,3,1},
            { 1,1,1,1,1,0,1,1,1,0,1,4,1,1,0,0,3,3,1},
            { 0,0,0,0,1,0,0,0,0,0,1,1,1,1,1,1,1,1,1},
            { 0,0,0,0,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0},
        };
        private readonly static int[,] Level_2 = new int[10, 14]
        {
            { 1,1,1,1,1,1,1,1,1,1,1,1,0,0},
            { 1,3,3,0,0,1,0,0,0,0,0,1,1,1},
            { 1,3,3,0,0,1,0,2,0,0,2,0,0,1},
            { 1,3,3,0,0,1,2,1,1,1,1,0,0,1},
            { 1,3,3,0,0,0,0,4,0,1,1,0,0,1},
            { 1,3,3,0,0,1,0,1,0,0,2,0,1,1},
            { 1,1,1,1,1,1,0,1,1,2,0,2,0,1},
            { 0,0,1,0,2,0,0,2,0,2,0,2,0,1},
            { 0,0,1,0,0,0,0,1,0,0,0,0,0,1},
            { 0,0,1,1,1,1,1,1,1,1,1,1,1,1},
        };
        private readonly static int[,] Level_3 = new int[10, 17]
        {
            { 0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,0},
            { 0,0,0,0,0,0,0,0,1,0,0,0,0,0,4,1,0},
            { 0,0,0,0,0,0,0,0,1,0,2,1,2,0,1,1,0},
            { 0,0,0,0,0,0,0,0,1,0,2,0,0,2,1,0,0},
            { 0,0,0,0,0,0,0,0,1,1,2,0,2,0,1,0,0},
            { 1,1,1,1,1,1,1,1,1,0,2,0,1,0,1,1,1},
            { 1,3,3,3,3,0,0,1,1,0,2,0,0,2,0,0,1},
            { 1,1,3,3,3,0,0,0,0,2,0,0,2,0,0,0,1},
            { 1,3,3,3,3,0,0,1,1,1,1,1,1,1,1,1,1},
            { 1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0},
         };
        private readonly static int[,] Level_4 = new int[14, 19]
        {
            { 0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1},
            { 0,0,0,0,0,0,0,0,0,0,0,1,0,0,3,3,3,3,1},
            { 1,1,1,1,1,1,1,1,1,1,1,1,0,0,3,3,3,3,1},
            { 1,0,0,0,0,1,0,0,2,0,2,0,0,0,3,3,3,3,1},
            { 1,0,2,2,2,1,2,0,0,2,0,1,0,0,3,3,3,3,1},
            { 1,0,0,2,0,0,0,0,0,2,0,1,0,0,3,3,3,3,1},
            { 1,0,2,2,0,1,2,0,2,0,2,1,1,1,1,1,1,1,1},
            { 1,0,0,2,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0},
            { 1,1,0,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0},
            { 1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0,0},
            { 1,0,0,0,0,0,2,0,0,0,1,1,0,0,0,0,0,0,0},
            { 1,0,0,2,2,1,2,2,0,0,4,1,0,0,0,0,0,0,0},
            { 1,0,0,0,0,1,0,0,0,0,1,1,0,0,0,0,0,0,0},
            { 1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0},
         };
        private readonly static int[,] Level_5 = new int[13, 17]
        {
            { 0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0},
            { 0,0,0,0,0,0,0,0,1,0,0,0,1,1,1,1,1},
            { 0,0,0,0,0,0,0,0,1,0,1,2,1,1,0,0,1},
            { 0,0,0,0,0,0,0,0,1,0,0,0,0,0,2,0,1},
            { 1,1,1,1,1,1,1,1,1,0,1,1,1,0,0,0,1},
            { 1,0,0,0,0,0,0,1,1,0,2,0,0,2,1,1,1},
            { 1,0,0,0,0,0,0,0,0,2,0,2,2,0,1,1,0},
            { 1,0,0,0,0,0,0,1,1,2,0,0,2,0,4,1,0},
            { 1,1,1,1,1,1,1,1,1,0,0,2,0,0,1,1,0},
            { 0,0,0,0,0,0,0,0,1,0,2,0,2,0,0,1,0},
            { 0,0,0,0,0,0,0,0,1,1,1,0,1,1,0,1,0},
            { 0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1,0},
            { 0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0},
        };
        private readonly static int[,] Level_6 = new int[11, 12]
        {
            { 1,1,1,1,1,1,0,0,1,1,1,0},
            { 1,3,3,0,0,1,0,1,1,4,1,1},
            { 1,3,3,0,0,1,1,1,0,0,0,1},
            { 1,3,3,0,0,0,0,0,2,2,0,1},
            { 1,3,3,0,0,1,0,1,0,2,0,1},
            { 1,3,3,1,1,1,0,1,0,2,0,1},
            { 1,1,1,1,0,2,0,1,2,0,0,1},
            { 0,0,0,1,0,0,2,1,0,2,0,1},
            { 0,0,0,1,0,2,0,0,2,0,0,1},
            { 0,0,0,1,0,0,1,1,0,0,0,1},
            { 0,0,0,1,1,1,1,1,1,1,1,1},
         };
        #endregion

        private List<int[,]> levels = new List<int[,]>();
        private List<int> step = new List<int>();
        private int crtLevel;
        private int playerX = 0;
        private int playerY = 0;

        public Skonbon()
        {
            levels.Add(Level_0);
            levels.Add(Level_1);
            levels.Add(Level_2);
            levels.Add(Level_3);
            levels.Add(Level_4);
            levels.Add(Level_5);
            levels.Add(Level_6);

            LevelCount = levels.Count;
        }

        /// <summary>
        /// 加载关卡
        /// </summary>
        /// <param name="levelNum"></param>
        public void LoadLevel(int levelNum)
        {
            crtLevel = levelNum;

            PointMap = Rubbing(levels[crtLevel]);
            Search();
            Width = PointMap.GetLength(0);
            Height = PointMap.GetLength(1);
        }

        /// <summary>
        /// 重置当前关卡
        /// </summary>
        public void Reset()
        {
            PointMap = Rubbing(levels[crtLevel]);
            Search();
            step.Clear();
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="_ck"></param>
        public void Move(ConsoleKey _ck)
        {
            switch (_ck)
            {
                case ConsoleKey.A:
                    DoMove(1);
                    break;
                case ConsoleKey.D:
                    DoMove(2);
                    break;
                case ConsoleKey.S:
                    DoMove(3);
                    break;
                case ConsoleKey.W:
                    DoMove(4);
                    break;
            }
        }

        /// <summary>
        /// 撤销
        /// </summary>
        public void Revoke()
        {
            if (step.Count==0)
            {
                return;
            }
            List<int> temp = step.GetRange(0, step.Count-1);
            Reset();
            for (int i = 0; i < temp.Count; i++)
            {
                DoMove(temp[i]);
            }
        }

        /// <summary>
        /// 用于在初始化新场景时，搜索玩家的位置
        /// </summary>
        private void Search()
        {
            for (int i = 0; i < PointMap.GetLength(0); i++)
            {
                for (int j = 0; j < PointMap.GetLength(1); j++)
                {
                    if (PointMap[i, j].hasPlayer)
                    {
                        playerX = i;
                        playerY = j;
                        return;
                    }
                }
            }
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

        /// <summary>
        /// 进行移动
        /// </summary>
        /// <param name="dir"></param>
        private void DoMove(int dir)
        {
            int dirPointX = playerX;
            int dirPointY= playerY;
            int affectedPointX = playerX;
            int affectedPointY= playerY;
            switch (dir)
            {
                case 1:
                    dirPointX -=  1;
                    affectedPointX -= 2;
                    break;
                case 2:
                    dirPointX += 1;
                    affectedPointX += 2;
                    break;
                case 3:
                    dirPointY += 1;
                    affectedPointY += 2;
                    break;
                case 4:
                    dirPointY -= 1;
                    affectedPointY -= 2;
                    break;
            }

            if (PointMap[dirPointX, dirPointY].isWall==true)
            {
                return;
            }

            if (PointMap[dirPointX, dirPointY].hasBox == false)
            {

            }
            else if (PointMap[affectedPointX, affectedPointY].isWall == false && PointMap[affectedPointX, affectedPointY].hasBox == false)
            {
                PointMap[dirPointX, dirPointY].hasBox = false;
                PointMap[affectedPointX, affectedPointY].hasBox = true;
            }
            else
            {
                return;
            }

            PointMap[playerX, playerY].hasPlayer = false;
            PointMap[dirPointX, dirPointY].hasPlayer = true;
            step.Add(dir);
            playerX= dirPointX;
            playerY = dirPointY;
        }

        public class PointInfo
        {
            public bool hasBox=false;
            public bool hasPlayer = false;
            public bool isTarget = false;
            public bool isWall = false;

            public int Type
            {
                get
                {
                    if (isWall == true)
                    {
                        return 1;
                    }

                    if (isTarget == true)
                    {
                        if (hasBox == true)
                        {
                            return 5;
                        }
                        else if (hasPlayer == true)
                        {
                            return 6;
                        }
                        else
                        {
                            return 3;
                        }
                    }
                    else
                    {
                        if (hasBox == true)
                        {
                            return 2;
                        }
                        else if (hasPlayer == true)
                        {
                            return 4;
                        }
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
                    case 2:
                        hasBox = true;
                        break;
                    case 3:
                        isTarget = true;
                        break;
                    case 4:
                        hasPlayer = true;
                        break;
                    case 5:
                        hasBox = true;
                        isTarget = true;
                        break;
                    case 6:
                        hasPlayer = true;
                        isTarget = true;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}