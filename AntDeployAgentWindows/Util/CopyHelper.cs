using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                Directory.CreateDirectory(dstDir);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string dstFile = Path.Combine(dstDir, file.Name);
                file.CopyTo(dstFile, true);
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
    }
}
