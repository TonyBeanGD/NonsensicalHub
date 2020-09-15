using System;

namespace Nonsensical_Void
{
    class Program
    {
        static void Main(string[] args)
        {
            //SkonbonConsole.RunGame();
            //RetroSnakerConsole.RunGame();
            Test();
            Console.ReadKey(true);
        }

        static void Test()
        {
            int i = 1;
            ConsolePainter cp = new ConsolePainter(0,0,20,20);

            cp.Open(true);

            while (true)
            {
                Console.ReadKey(true);
                cp.Paint(i++,1,"tT");
            }
        }
    }
}
