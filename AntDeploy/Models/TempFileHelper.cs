using System.IO;

namespace AntDeploy.Models
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
    }
}