using System;
using System.Threading;

namespace DotOPDS.Utils
{
    class ConsoleStatus
    {
        private int counter;

        public void Update(string text, params object[] args)
        {
            Clear();

            counter++;
            switch (counter % 4)
            {
                case 0: Console.Write("/"); counter = 0; break;
                case 1: Console.Write("-"); break;
                case 2: Console.Write("\\"); break;
                case 3: Console.Write("|"); break;
            }

            var param = string.Format(text, args);
            Console.Write(" {0}", param);
            Thread.Sleep(500);
        }

        public void Clear()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth - 1));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}
