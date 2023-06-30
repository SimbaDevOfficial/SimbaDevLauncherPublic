using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimbaDevPublicLauncher.utils
{
    internal class Logging
    {
        public static void Log(string text)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("+");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(text + "\n");
        }

        public static string Input(string text)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("?");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(text + " ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(">");
            Console.ForegroundColor = ConsoleColor.White;
            return Console.ReadLine();
        }
    }
}
