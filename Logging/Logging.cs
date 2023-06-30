using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimbaDevPublicLauncher.Logging
{
    internal class Logging
    {
        public static string logFile = Launcher.Launch.currentDir + "\\SimbaLauncher.log";

        public void CreateLogFile()
        {
            if (!System.IO.File.Exists(logFile))
            {
                System.IO.File.Create(logFile);
            }
        }

        public static void LogToFile(string text)
        {
            string logText = "[LOG] " + text + " [" + DateTime.Now + "]\n";
            System.IO.File.AppendAllText(logFile, logText);
        }

        public static void ErrorToFile(string text)
        {
            string logText = "[ERROR] " + text + " [" + DateTime.Now + "]\n";
            System.IO.File.AppendAllText(logFile, logText);
        }

        public static void InputToFile(string text)
        {
            string logText = "[INPUT] " + text + " [" + DateTime.Now + "]\n";
            System.IO.File.AppendAllText(logFile, logText);
        }

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
            LogToFile(text);
        }

        public static void Error(string text)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("-");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(text + "\n");
            ErrorToFile(text);
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
            InputToFile(text);
            return Console.ReadLine();
        }
    }
}
