using System;

namespace DomainEventDemo
{
    static class Log
    {
        public static void Info(string message, params object[] args)
        {
            Write(ConsoleColor.White, message, args);
        }

        public static void Warn(string message, params object[] args)
        {
            Write(ConsoleColor.DarkYellow, message, args);
        }

        public static void Error(string message, params object[] args)
        {
            Write(ConsoleColor.Red, message, args);

        }

        public static void Write(ConsoleColor color, string message, params object[] args)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message, args);
            Console.ForegroundColor = prevColor;
        }


    }
}
