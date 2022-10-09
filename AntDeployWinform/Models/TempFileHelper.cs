using System;
using System.IO;

namespace AntDeployWinform.Models
{
    internal static class TempFileHelper
    {
        public static string CreateTempFile(string projectFile)
        {
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(path);    // Ensure temp path exists.
            var file = Path.Combine(path, Path.GetFileName(projectFile));
            File.Copy(projectFile, file, true);
            return file;
        }


        public static void RemoveTempFile(string file)
        {
            try
            {
                Directory.Delete(Path.GetDirectoryName(file), true);
            }
            catch
            {
            }
        }

        public static string CopyFileToTempFolder(string file, string folder, bool appendRondomName = true)
        {
            if (appendRondomName)
            {
                var file12 = Path.Combine(folder, Path.GetFileNameWithoutExtension(file) + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(file));
                File.Copy(file, file12, true);
                return file12;
            }
            var file1 = Path.Combine(folder, Path.GetFileName(file));
            File.Copy(file, file1, true);
            return file1;
        }

        public static void RemoveFileInTempFolder(string folder)
        {
            try
            {
                var d = new DirectoryInfo(folder);
                var files = d.GetFiles();
                foreach (var f in files)
                {
                    try
                    {
                        f.Delete();
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {
            }
        }
    }
}