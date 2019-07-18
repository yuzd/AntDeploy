using System;
using System.Collections.Generic;
using System.Text;

namespace AntDeployCommand.Utils
{
    public class LogHelper
    {
        public static void Info(string txt)
        {
            Console.WriteLine($"【INFO】{txt}");
        }

        public static void Debug(string txt)
        {
            Console.WriteLine($"【DEBG】{txt}");
        }

        public static void Warn(string txt)
        {
            Console.WriteLine($"【WARN】{txt}");
        }

        public static void Error(string txt)
        {
            Console.WriteLine($"【FAIL】{txt}");
        }
    }
}
