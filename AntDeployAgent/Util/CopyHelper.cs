using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace AntDeployAgentWindows.Util
{
    static class CopyHelper
    {
        /// <summary>
        /// copies if the files and folders are existent in referenced directory
        /// </summary>
        /// <param name="srcDir">source directory</param>
        /// <param name="dstDir">destination directory</param>
        /// <param name="refDir">reference directory</param>
        /// <param name="copySubDirs">copy sub directories</param>
        public static void DirectoryCopyWithRef(string srcDir, string dstDir, string refDir, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(refDir);

            if (!dir.Exists)
                throw new DirectoryNotFoundException("Reference directory does not exist or could not be found: " + refDir);

            if (!Directory.Exists(dstDir))
                Directory.CreateDirectory(dstDir);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string srcFile = Path.Combine(srcDir, file.Name);
                string dstFile = Path.Combine(dstDir, file.Name);
                if (File.Exists(srcFile))
                {
                    File.Copy(srcFile, dstFile, true);
                }
            }

            if (copySubDirs)
            {
                DirectoryInfo[] subDirs = dir.GetDirectories();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    string srcDirSub = Path.Combine(srcDir, subDir.Name);
                    string dstDirSub = Path.Combine(dstDir, subDir.Name);
                    DirectoryCopyWithRef(srcDirSub, dstDirSub, subDir.FullName, true);
                }
            }
        }

        /// <summary>
        /// 利用xcopy复制源文件夹到目标文件夹，覆盖
        /// 选用/S时对源目录下及其子目录下的所有文件进行COPY 除非指定/E参数，否则/S不会拷贝空目录
        /// /q 禁止显示“xcopy”的消息。/y 禁止提示确认要覆盖已存在的目标文件。
        /// /I 如果“Source”是一个目录或包含通配符，而“Destination”不存在，“xcopy”会假定“destination”指定目录名并创建一个新目录。然后，“xcopy”会将所有指定文件复制到新目录中。默认情况下，“xcopy”将提示您指定“Destination”是文件还是目录
        /// </summary>
        /// <param name="SolutionDirectory"></param>
        /// <param name="TargetDirectory"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static void ProcessXcopy(string SolutionDirectory, string TargetDirectory,Action<string> logger = null)
        {
            RunCommand($" xcopy " + "\"" + SolutionDirectory + "\"" + " " + "\"" + TargetDirectory + "\"" +
                       @" /s /e /Q /Y /I",null, logger);
            return;

            //ProcessStartInfo startInfo = new ProcessStartInfo();
            //startInfo.CreateNoWindow = false;
            //startInfo.UseShellExecute = false;
            ////Give the name as Xcopy
            //startInfo.FileName = "xcopy";
            ////make the window Hidden
            //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            ////Send the Source and destination as Arguments to the pro
            ////   RedirectStandardOutput = true,
            //startInfo.RedirectStandardOutput = true;
            //startInfo.Verb = "runas";

            //startInfo.Arguments = "\"" + SolutionDirectory + "\"" + " " + "\"" + TargetDirectory + "\"" + @" /s /e /Q /Y /I";
            //logger?.Invoke("xcopy " + startInfo.Arguments);
            //// Start the process with the info we specified.
            //// Call WaitForExit and then the using statement will close.
            //using (Process exeProcess = Process.Start(startInfo))
            //{
            //    // ReSharper disable once PossibleNullReferenceException
            //    exeProcess.WaitForExit();
            //    var output = exeProcess.StandardOutput.ReadToEnd();
            //    if (!string.IsNullOrEmpty(output))
            //    {
            //        var outputArr = output.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            //        foreach (var outPut in outputArr)
            //        {
            //            logger?.Invoke("【Xcopy】"+outPut);
            //        }
            //    }
            //    else
            //    {
            //        output = "return empty info.";
            //        logger?.Invoke("【Xcopy】" + output);
            //    }
            //    if (exeProcess.ExitCode != 0)
            //    {
            //        logger?.Invoke("【Error】"+output);
            //        throw new IOException(output);
            //    }
            //}
        }

        /// <summary>
        /// 运行bash命令
        /// </summary>
        /// <param name="commandToRun"></param>
        /// <param name="workingDirectory"></param>
        /// <param name="logger"></param>
        public static void RunCommand(string commandToRun, string workingDirectory = null, Action<string> logger = null)
        {
            try
            {
                if (string.IsNullOrEmpty(workingDirectory))
                {
                    workingDirectory = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory());
                }

                var processStartInfo = new ProcessStartInfo()
                {
                    FileName = (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd" : "bash"),
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    WorkingDirectory = workingDirectory
                };

                var process = Process.Start(processStartInfo);

                if (process == null)
                {
                    return;
                }

                process.StandardInput.WriteLine($"{commandToRun} & exit");
                process.WaitForExit();

                var output = process.StandardOutput.ReadToEnd();


                var prefix = "";
                var isSuccess = process.ExitCode == 0;
                if (!isSuccess)
                {
                    prefix = "【Error】";
                }
              
                if (!string.IsNullOrEmpty(output))
                {
                    var outputArr = output.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var outPut in outputArr)
                    {
                        logger?.Invoke(prefix + "【Command】 " + outPut);
                    }
                }
                else
                {
                    output = "return empty info.";
                    logger?.Invoke("【Command】 " + output);
                }

                try
                {
                    process.Kill();
                }
                catch (Exception)
                {
                    //ignore
                }
            }
            catch (Exception ex)
            {
                logger?.Invoke("【Error】【Command】" + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// copies source directory to the destionation directory
        /// </summary>
        /// <param name="srcDir">source directory</param>
        /// <param name="dstDir">destination directory</param>
        /// <param name="copySubDirs">copy sub directories</param>
        public static void DirectoryCopy(string srcDir, string dstDir, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(srcDir);

            if (!dir.Exists)
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + srcDir);

            if (!Directory.Exists(dstDir))
            {
                try
                {
                    Directory.CreateDirectory(dstDir);
                }
                catch (Exception ex)
                {
                    throw new Exception($"CreateDirectory:{dstDir} Fail:{ex.Message}");
                }
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string dstFile = Path.Combine(dstDir, file.Name);
                try
                {
                    file.CopyTo(dstFile, true);
                }
                catch (Exception ex)
                {
                    throw new Exception($"from:{file.FullName} copy to {dstFile} Fail:{ex.Message}");
                }
            }

            if (copySubDirs)
            {
                DirectoryInfo[] subDirs = dir.GetDirectories();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    string dstDirSub = Path.Combine(dstDir, subDir.Name);
                    DirectoryCopy(subDir.FullName, dstDirSub, copySubDirs);
                }
            }
        }

        public static void DirectoryCopy(string srcDir, string dstDir, bool copySubDirs, string parentDir,string parentName, List<string> backignoreUpList)
        {
            if (backignoreUpList == null || !backignoreUpList.Any())
            {
                DirectoryCopy(srcDir, dstDir, copySubDirs);
                return;
            }

            if (string.IsNullOrEmpty(parentDir))
            {
                throw new DirectoryNotFoundException("parentDir is empty ");
            }



            DirectoryInfo dir = new DirectoryInfo(srcDir);

            if (!dir.Exists)
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + srcDir);

            if (!Directory.Exists(dstDir))
            {
                try
                {
                    Directory.CreateDirectory(dstDir);
                }
                catch (Exception ex)
                {
                    throw new Exception($"CreateDirectory:{dstDir} Fail:{ex.Message}");
                }
            }


            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                int length = file.FullName.Length -parentDir.Length ;
                string entryName = EntryFromPath(file.FullName,parentDir.Length,length);
                string matchEntryName = entryName.Substring(parentName.Length);
                if (IsMacthIgnore(entryName, backignoreUpList)) continue;
                string dstFile = Path.Combine(dstDir, file.Name);
                try
                {
                    file.CopyTo(dstFile, true);
                }
                catch (Exception ex)
                {
                    throw new Exception($"from:{file.FullName} copy to {dstFile} Fail:{ex.Message}");
                }
            }

            if (copySubDirs)
            {
                DirectoryInfo[] subDirs = dir.GetDirectories();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    string dstDirSub = Path.Combine(dstDir, subDir.Name);
                    int length = subDir.FullName.Length - parentDir.Length;
                    string entryName = EntryFromPath(subDir.FullName,parentDir.Length, length);
                    if (IsMacthIgnore(entryName, backignoreUpList)) continue;
                    DirectoryCopy(subDir.FullName, dstDirSub, copySubDirs, parentDir,parentName, backignoreUpList);
                }
            }
        }

        private static bool IsMacthIgnore(string entryName, List<string> backignoreUpList)
        {
            var haveMatch = false;
            foreach (var ignorRule in backignoreUpList)
            {
                try
                {
                    if (ignorRule.StartsWith("*"))
                    {
                        var ignorRule2 = ignorRule.Substring(1);
                        if (entryName.EndsWith(ignorRule2))
                        {
                            haveMatch = true;
                            break;
                        }
                    }
                    else
                    {
                        var isMatch = Regex.Match(entryName, ignorRule, RegexOptions.IgnoreCase);//忽略大小写
                        if (isMatch.Success)
                        {
                            haveMatch = true;
                            break;
                        }
                    }

                }
                catch (Exception ex)
                {
                }
            }

            return haveMatch;
        }

        private static string EntryFromPath(string entry, int offset, int length)
        {
            for (; length > 0 && ((int)entry[offset] == (int)Path.DirectorySeparatorChar || (int)entry[offset] == (int)Path.AltDirectorySeparatorChar); --length)
                ++offset;
            if (length == 0)
                return string.Empty;
            char[] charArray = entry.ToCharArray(offset, length);
            for (int index = 0; index < charArray.Length; ++index)
            {
                if ((int)charArray[index] == (int)Path.DirectorySeparatorChar || (int)charArray[index] == (int)Path.AltDirectorySeparatorChar)
                    charArray[index] = '/';
            }
            return new string(charArray);
        }
    }
}
