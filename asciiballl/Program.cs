using System;
using System.Timers;

namespace asciiballl
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(150, 60);
            string s = "";
            var timer = DateTime.Now;
            long a = 0;
            while (DateTime.Now.Subtract(timer).TotalMilliseconds <= 10000)
            {
                s = "";
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.SetCursorPosition(0, 0);
                for (int i = 0; i < 150 * 6; i++) s += "0000000000";
                for (int i = 0; i < 150 * 60; i++)
                Console.Write(s[i]);
                a++;
            }
            Console.Clear();
            Console.WriteLine(a);
            Console.ReadLine();
        }
    }
}
