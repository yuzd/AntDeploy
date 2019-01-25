using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntDeploy.Util;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandHelper.RunDotnetExternalExe(@"H:\Csharp\yuzd\AntDeploy\AntDeploy\WindowsFormsAppTest\Test\MyProject",
                "publish -c Release",
                Console.WriteLine,
                Console.WriteLine);
            Console.ReadKey();
        }
    }
}
