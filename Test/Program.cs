using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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

            var bytes= ZipHelper.DoCreateFromDirectory(
                @"H:\Csharp\yuzd\AntDeploy\AntDeploy\WindowsFormsAppTest\Test\MyProject\bin\Release\netstandard2.0\publish",
                CompressionLevel.Optimal, true);

            
            var filePath = Path.Combine(@"H:\Csharp\yuzd\AntDeploy\AntDeploy\WindowsFormsAppTest\Test\MyProject\bin\Release\netstandard2.0\unzip", "aa.zip");
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                fs.Write(bytes,0, bytes.Length);
            }

            //解压
            try
            {
                ZipFile.ExtractToDirectory(filePath, @"H:\Csharp\yuzd\AntDeploy\AntDeploy\WindowsFormsAppTest\Test\MyProject\bin\Release\netstandard2.0\unzip");
            }
            catch (Exception ex)
            {

            }


            return;

            CommandHelper.RunDotnetExternalExe(@"H:\Csharp\yuzd\AntDeploy\AntDeploy\WindowsFormsAppTest\Test\MyProject",
                "publish -c Release",
                Console.WriteLine,
                Console.WriteLine);
            Console.ReadKey();
        }
    }
}
