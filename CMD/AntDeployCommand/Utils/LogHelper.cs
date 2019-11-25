using System;
using System.Collections.Generic;
using System.Text;

namespace AntDeployCommand.Utils
{
    public class LogHelper
    {
        /// <summary>
        /// 日志
        /// </summary>
        public static Action<string> LoggerProvider = Console.WriteLine;

        public static void Info(string txt)
        {
            LoggerProvider($"【INFO】{txt}");
        }

        public static void Debug(string txt)
        {
            LoggerProvider($"【DEBG】{txt}");
        }

        public static void Warn(string txt)
        {
            LoggerProvider($"【WARN】{txt}");
        }

        public static void Error(string txt)
        {
            LoggerProvider($"【FAIL】{txt}");
        }
    }
}
