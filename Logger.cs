using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;

namespace telebot
{
    public enum LogLevel
    {
        [Description("INFO")]
        Info,
        [Description("WARNING")]
        Warning,
        [Description("ERROR")]
        Error
    }

    public class Logger
    {
        public static void Log(LogLevel level, string message)
        {
            Console.ForegroundColor = GetConsoleColor(level);
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{GetDescription(level)}] {message}");
            Console.ResetColor();
        }

        private static ConsoleColor GetConsoleColor(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Info:
                    return ConsoleColor.Green;
                case LogLevel.Warning:
                    return ConsoleColor.Yellow;
                case LogLevel.Error:
                    return ConsoleColor.Red;
                default:
                    return ConsoleColor.White;
            }
        }

        private static string GetDescription(LogLevel level)
        {
            var fieldInfo = level.GetType().GetField(level.ToString());
            var descriptionAttribute = (DescriptionAttribute)fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute));
            return descriptionAttribute != null ? descriptionAttribute.Description : level.ToString();
        }
    }
}