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

            var mer = ZipHelper.DoCreateTarFromDirectory(
                @"E:\WorkSpace\github\Lito\Lito\Lito.APP\bin\Debug\netcoreapp2.1\publish",new List<string>
                {
                    "/win-x86?.+",
                    "web.config"
                });

            var filePath2= Path.Combine(@"E:\WorkSpace\github\Lito\Lito\Lito.APP\bin\Debug\netcoreapp2.1\logs", "test.tar");
            using (var fs = new FileStream(filePath2, FileMode.Create))
            {
                var bt = mer.GetBuffer();
                fs.Write(bt,0, bt.Length);
            }

            GitClient gitHelper = new GitClient(@"E:\WorkSpace\github\AntDeploy\Test\bin\Debug\testgit");
            var s=gitHelper.GetChanges();

            var s1 = ZipHelper.GetFullFileInfo(s, @"E:\WorkSpace\github\AntDeploy\Test\bin\Debug\testgit");
             var allFile = ZipHelper.FindFileDir(@"E:\WorkSpace\github\AntDeploy\Test\bin\Debug\testgit");


            
            //var buildResult = CommandHelper.RunMsbuild("E:\\WorkSpace\\github\\AntDeploy\\AntDeployAgentWindowsService\\AntDeployAgentWindowsService.csproj",
            //    Console.WriteLine,Console.WriteLine);



            //SSHClient sshClient = new SSHClient("192.168.0.7:22","root","kawayiyi@1",Console.WriteLine);
            //using (SSHClient sshClient = new SSHClient("192.168.159.131:22","root","admin",Console.WriteLine,Console.WriteLine)
            //{
            //    NetCoreENTRYPOINT = "Lito.APP.dll",
            //    NetCoreVersion = "2.1",
            //    NetCorePort = "5004",

            //})
            //{
            //    sshClient.Connect();w
            //    sshClient.PublishZip(@"E:\WorkSpace\github\Lito\Lito\Lito.APP\bin\Debug\netcoreapp2.1\publish\", new List<string>
            //    {
            //        "web.config"
            //    }, "publisher","publish.zip");
            //}
            



            var bytes= ZipHelper.DoCreateFromDirectory(
                @"E:\WorkSpace\github\Lito\Lito\Lito.APP\bin\Debug\netcoreapp2.1\publish\",
                CompressionLevel.Optimal, true,new List<string>
                {
                    "/win-x86?.+",
                    "web.config"
                });


            
            var filePath = Path.Combine(@"E:\WorkSpace\github\Lito\Lito\Lito.APP\bin\Debug\netcoreapp2.1\logs", "aa.zip");
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                fs.Write(bytes,0, bytes.Length);
            }

            //解压
            try
            {
                ZipFile.ExtractToDirectory(filePath, @"E:\WorkSpace\github\Lito\Lito\Lito.APP\bin\Debug\netcoreapp2.1\logs");
            }
            catch (Exception ex)
            {

            }


            return;

            CommandHelper.RunDotnetExternalExe(@"H:\Csharp\yuzd\AntDeploy\AntDeploy\WindowsFormsAppTest\Test\MyProject","dotnet",
                "publish -c Release",
                Console.WriteLine,
                Console.WriteLine);
            Console.ReadKey();
        }
    }
}
