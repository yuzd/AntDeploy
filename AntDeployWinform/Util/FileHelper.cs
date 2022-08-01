using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntDeployWinform.Util
{
    public class FileHelper
    {
        /// <summary>
        /// 对结果集重新排序
        /// </summary>
        /// <param name="files"></param>
        /// <param name="rootPath"></param>
        public static void ResortFileList(List<Models.FileStruct> files, string rootPath)
        {
            if (files == null || files.Count <= 0)
            {
                return;
            }

            files.Sort((x, y) =>
            {
                //当值是 -1 的时候，A排在B之前； 
                //当值是 0 的时候，A和B在相同位置； 
                //当值是 1 的时候，B在A的前面； 
                int value = 0;
                string xpath = Path.GetDirectoryName(x.FileFullName);
                string ypath = Path.GetDirectoryName(y.FileFullName);
                if (String.Equals(rootPath, xpath, StringComparison.OrdinalIgnoreCase)
                    && !String.Equals(rootPath, ypath, StringComparison.OrdinalIgnoreCase))
                {
                    value = 1;
                }
                else if (!String.Equals(rootPath, xpath, StringComparison.OrdinalIgnoreCase)
                         && String.Equals(rootPath, ypath, StringComparison.OrdinalIgnoreCase))
                {
                    value = -1;
                }

                if (value == 0)
                {
                    if (x.UpdateTime > y.UpdateTime)
                    {
                        value = -1;
                    }
                    else if (x.UpdateTime < y.UpdateTime)
                    {
                        value = 1;
                    }
                }

                if (value == 0)
                {
                    value = String.Compare(x.FileFullName, y.FileFullName, StringComparison.OrdinalIgnoreCase);
                }

                return value;
            });
        }

        /// <summary>
        /// 获得文件夹下所有的文件（递归）
        /// </summary>
        /// <param name="files">递归结果集</param>
        /// <param name="dir">文件夹的目录</param>
        /// <param name="ignoreList">正则表达式，排序规则</param>
        /// <param name="rootDir">根目录</param>
        /// <returns>返回所有文件</returns>
        public static void GetAllFileInfos(List<Models.FileStruct> files, string dir, List<string> ignoreList, string rootDir="")
        {
            if (!Directory.Exists(dir))
            {
                return;
            }
            if (String.IsNullOrWhiteSpace(rootDir))
            {
                rootDir = dir;
            }
            DirectoryInfo dirInfo=new DirectoryInfo(dir);            
            FileInfo[] allFile = dirInfo.GetFiles();
            if (allFile.Length > 0)
            {
                bool hasFile = false;
                var maxFileLastWriteTime = new DateTime(1970, 1, 1);
                foreach (FileInfo fi in allFile)
                {
                    string relativePath = "/" + fi.FullName.Substring(rootDir.Length).TrimStart('/', '\\').Replace("\\", "/");
                    if (ZipHelper.IsIgnore(relativePath, ignoreList))
                    {
                        continue;
                    }
                    if (files.All(u => !String.Equals(u.FileFullName, fi.FullName, StringComparison.OrdinalIgnoreCase)))
                    {
                        files.Add(new Models.FileStruct
                        {
                            FileFullName = fi.FullName,
                            UpdateTime = fi.LastWriteTime,
                            IsFile = true,
                            RelativePath= relativePath
                        });
                        if (fi.LastWriteTime > maxFileLastWriteTime)
                        {
                            maxFileLastWriteTime = fi.LastWriteTime;
                        }
                        hasFile = true;
                    }
                }
                if (hasFile && !String.Equals(rootDir, dirInfo.FullName, StringComparison.OrdinalIgnoreCase))
                {
                    if (files.All(u => !String.Equals(u.FileFullName, dirInfo.FullName, StringComparison.OrdinalIgnoreCase)))
                    {
                        files.Add(new Models.FileStruct
                        {
                            FileFullName = dirInfo.FullName,
                            UpdateTime = maxFileLastWriteTime,
                            IsFile = false
                        });
                    }
                }
            }
            else
            {
                // 如果这个文件夹下面一个文件都没有
                files.Add(new Models.FileStruct
                {
                    FileFullName = dirInfo.FullName,
                    UpdateTime = dirInfo.LastWriteTime,
                    IsFile = false
                });
            }
            DirectoryInfo[] allDir = dirInfo.GetDirectories();
            foreach (DirectoryInfo di in allDir)
            {
                GetAllFileInfos(files, di.FullName,ignoreList, rootDir);
            }
        }
    }
}
