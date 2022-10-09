﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AntDeployCommand.Utils
{
    public class ZipHelper
    {
        private static readonly char s_pathSeperator = '/';


        public static List<FileSystemInfo> GetSelectDeployFiles(List<string> fileList)
        {
            List<FileSystemInfo> findlist = new List<FileSystemInfo>();
            foreach (var filePath in fileList)
            {
                findlist.Add(new FileInfo(filePath));
            }

            return findlist;
        }


        public static List<FileSystemInfo> GetFullFileInfo(List<string> fileList, string folderPath)
        {
            List<FileSystemInfo> findlist = new List<FileSystemInfo>();
            List<FileSystemInfo> folderlist = new List<FileSystemInfo>();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var filePath in fileList)
            {
                var fullPath = Path.Combine(folderPath, filePath);
                var fileArr = filePath.Split('/');
                if (fileArr.Length == 1)
                {
                    if (dic.ContainsKey(fullPath))
                    {
                        continue;
                    }

                    dic.Add(fullPath, filePath);
                    findlist.Add(new FileInfo(fullPath));
                }
                else
                {
                    for (int i = 0; i < fileArr.Length; i++)
                    {
                        if (i == fileArr.Length - 1)
                        {
                            if (dic.ContainsKey(fullPath))
                            {
                                continue;
                            }

                            dic.Add(fullPath, filePath);
                            findlist.Add(new FileInfo(Path.Combine(folderPath, string.Join(Path.DirectorySeparatorChar.ToString(), fileArr))));
                        }
                        else
                        {
                            string foldPath = Path.Combine(folderPath, string.Join(Path.DirectorySeparatorChar.ToString(), fileArr.Take(i + 1).ToList()));
                            if (Directory.Exists(foldPath))
                            {
                                if (dic.ContainsKey(foldPath))
                                {
                                    continue;
                                }

                                dic.Add(foldPath, foldPath);
                                folderlist.Add(new DirectoryInfo(foldPath));
                            }

                        }
                    }
                }

            }

            folderlist.AddRange(findlist);

            return folderlist;
        }


        public static FileSystemInfo[] FindFileDir(string beginpath)
        {
            List<FileSystemInfo> findlist = new List<FileSystemInfo>();

            /* I begin a recursion, following the order:
             * - Insert all the files in the current directory with the recursion
             * - Insert all subdirectories in the list and rebegin the recursion from there until the end
             */
            RecurseFind(beginpath, findlist);

            return findlist.ToArray();
        }

        private static void RecurseFind(string path, List<FileSystemInfo> list)
        {
            string[] fl = Directory.GetFiles(path);
            string[] dl = Directory.GetDirectories(path);
            if (fl.Length > 0 || dl.Length > 0)
            {
                //I begin with the files, and store all of them in the list
                foreach (string s in fl)
                    list.Add(new FileInfo(s));
                //I then add the directory and recurse that directory, the process will repeat until there are no more files and directories to recurse
                foreach (string s in dl)
                {
                    if (s.EndsWith(".git")) continue;
                    list.Add(new DirectoryInfo(s));
                    RecurseFind(s, list);
                }
            }
        }


        public static byte[] DoCreateFromDirectory(string sourceDirectoryName, List<string> fileList, CompressionLevel? compressionLevel, bool includeBaseDirectory, List<string> ignoreList = null, Func<int, bool> progress = null, bool isSelectDeploy = false)
        {
            //if (ignoreList != null)
            //{
            //    ignoreList.Add("/.git?");
            //}
            var haveFile = false;
            long lastProgressNumber = 0;
            sourceDirectoryName = Path.GetFullPath(sourceDirectoryName);
            using (var outStream = new MemoryStream())
            {
                using (ZipArchive destination = new ZipArchive(outStream, ZipArchiveMode.Create, false))
                {
                    bool flag = true;
                    DirectoryInfo directoryInfo = new DirectoryInfo(sourceDirectoryName);
                    string fullName = directoryInfo.FullName;
                    if (includeBaseDirectory && directoryInfo.Parent != null)
                        fullName = directoryInfo.Parent.FullName;
                    var allFile = isSelectDeploy ? GetSelectDeployFiles(fileList) : GetFullFileInfo(fileList, sourceDirectoryName);
                    var allFileLength = allFile.Count();
                    var index = 0;

                    foreach (FileSystemInfo enumerateFileSystemInfo in allFile)
                    {
                        index++;
                        var progressNumber = (((long)index * 100 / allFileLength));
                        if (progress != null && lastProgressNumber!= progressNumber)
                        {
                            lastProgressNumber = progressNumber;
                            var r = progress.Invoke((int)lastProgressNumber);
                            if (r)
                            {
                                throw new Exception("deploy task was canceled!");
                            }
                        }

                        flag = false;
                        int length = enumerateFileSystemInfo.FullName.Length - fullName.Length;
                        string entryName = EntryFromPath(enumerateFileSystemInfo.FullName, fullName.Length, length);
                        var mathchEntryName = includeBaseDirectory? entryName.Substring(directoryInfo.Name.Length): "/"+entryName;
                        if (ignoreList != null && ignoreList.Count > 0)
                        {
                            var haveMatch = false;
                            foreach (var ignorRule in ignoreList)
                            {
                                try
                                {
                                    if (ignorRule.StartsWith("*"))
                                    {
                                        var ignorRule2 = ignorRule.Substring(1);
                                        if (mathchEntryName.EndsWith(ignorRule2))
                                        {
                                            haveMatch = true;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        var isMatch = Regex.Match(mathchEntryName, ignorRule, RegexOptions.IgnoreCase);//忽略大小写
                                        if (isMatch.Success)
                                        {
                                            haveMatch = true;
                                            break;
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    throw new Exception($"Ignore Rule 【{ignorRule}】 regular error:" + ex.Message);
                                }
                            }

                            if (haveMatch)
                            {
                                continue;
                            }
                        }
                        if (enumerateFileSystemInfo is FileInfo)
                        {
                            haveFile = true;
                            DoCreateEntryFromFile(destination, enumerateFileSystemInfo.FullName, entryName, compressionLevel);
                        }
                        else
                        {
                            DirectoryInfo possiblyEmptyDir = enumerateFileSystemInfo as DirectoryInfo;
                            if (possiblyEmptyDir != null && IsDirEmpty(possiblyEmptyDir))
                                destination.CreateEntry(entryName + s_pathSeperator.ToString());
                        }
                    }

                    if ((includeBaseDirectory & flag))
                    {
                        string str = directoryInfo.Name;
                        destination.CreateEntry(str + s_pathSeperator.ToString());
                    }
                }
                if (!haveFile)
                {
                    throw new Exception("no file was packaged!");
                }
                return outStream.ToArray();
            }
        }

        /// <summary>
        /// 打包文件夹
        /// </summary>
        /// <param name="sourceDirectoryName"></param>
        /// <param name="compressionLevel"></param>
        /// <param name="includeBaseDirectory"></param>
        /// <returns></returns>
        public static byte[] DoCreateFromDirectory(string sourceDirectoryName, CompressionLevel? compressionLevel, bool includeBaseDirectory, List<string> ignoreList = null, Func<int, bool> progress = null)
        {
            var haveFile = false;
            long lastProgressNumber = 0;
            sourceDirectoryName = Path.GetFullPath(sourceDirectoryName);
            using (var outStream = new MemoryStream())
            {
                using (ZipArchive destination = new ZipArchive(outStream, ZipArchiveMode.Create, false))
                {
                    bool flag = true;
                    DirectoryInfo directoryInfo = new DirectoryInfo(sourceDirectoryName);
                    string fullName = directoryInfo.FullName;
                    if (includeBaseDirectory && directoryInfo.Parent != null)
                        fullName = directoryInfo.Parent.FullName;
                    var allFile = FindFileDir(sourceDirectoryName);
                    var allFileLength = allFile.Count();
                    var index = 0;
                    foreach (FileSystemInfo enumerateFileSystemInfo in allFile)
                    {
                        index++;
                        var progressNumber = (((long)index * 100 / allFileLength));
                        if (progress != null && lastProgressNumber != progressNumber)
                        {
                            lastProgressNumber = progressNumber;
                            var r = progress.Invoke((int)lastProgressNumber);
                            if (r)
                            {
                                throw new Exception("deploy task was canceled!");
                            }
                        }
                        flag = false;
                        int length = enumerateFileSystemInfo.FullName.Length - fullName.Length;
                        string entryName = EntryFromPath(enumerateFileSystemInfo.FullName, fullName.Length, length);
                        var mathchEntryName = includeBaseDirectory? entryName.Substring(directoryInfo.Name.Length): "/"+entryName;
                        if (ignoreList != null && ignoreList.Count > 0)
                        {
                            var haveMatch = false;
                            foreach (var ignorRule in ignoreList)
                            {
                                try
                                {
                                    if (ignorRule.StartsWith("*"))
                                    {
                                        var ignorRule2 = ignorRule.Substring(1);
                                        if (mathchEntryName.EndsWith(ignorRule2))
                                        {
                                            haveMatch = true;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        var isMatch = Regex.Match(mathchEntryName, ignorRule, RegexOptions.IgnoreCase);//忽略大小写
                                        if (isMatch.Success)
                                        {
                                            haveMatch = true;
                                            break;
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    throw new Exception($"Ignore Rule 【{ignorRule}】 regular error:" + ex.Message);
                                }
                            }

                            if (haveMatch)
                            {
                                continue;
                            }
                        }
                        if (enumerateFileSystemInfo is FileInfo)
                        {
                            haveFile = true;
                            if (entryName.Contains("Dockerfile"))
                            {
                                LogHelper.Info($"Find Dockerfile In Package: {mathchEntryName}");
                            }
                            DoCreateEntryFromFile(destination, enumerateFileSystemInfo.FullName, entryName, compressionLevel);
                        }
                        else
                        {
                            DirectoryInfo possiblyEmptyDir = enumerateFileSystemInfo as DirectoryInfo;
                            if (possiblyEmptyDir != null && IsDirEmpty(possiblyEmptyDir))
                                destination.CreateEntry(entryName + s_pathSeperator.ToString());
                        }
                    }

                    if ((includeBaseDirectory & flag))
                    {
                        string str = directoryInfo.Name;
                        destination.CreateEntry(str + s_pathSeperator.ToString());
                    }
                }
                if (!haveFile)
                {
                    throw new Exception("no file was packaged!");
                }
                return outStream.ToArray();
            }
        }

       

        internal static ZipArchiveEntry DoCreateEntryFromFile(
            ZipArchive destination,
            string sourceFileName,
            string entryName,
            CompressionLevel? compressionLevel)
        {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (sourceFileName == null)
                throw new ArgumentNullException(nameof(sourceFileName));
            if (entryName == null)
                throw new ArgumentNullException(nameof(entryName));
            using (Stream stream = (Stream)File.Open(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ZipArchiveEntry zipArchiveEntry = compressionLevel.HasValue ? destination.CreateEntry(entryName, compressionLevel.Value) : destination.CreateEntry(entryName);
                DateTime dateTime = File.GetLastWriteTime(sourceFileName);
                if (dateTime.Year < 1980 || dateTime.Year > 2107)
                    dateTime = new DateTime(1980, 1, 1, 0, 0, 0);
                zipArchiveEntry.LastWriteTime = (DateTimeOffset)dateTime;
                using (Stream destination1 = zipArchiveEntry.Open())
                    stream.CopyTo(destination1);
                return zipArchiveEntry;
            }
        }

        public static string EntryFromPath(string entry, int offset, int length)
        {
            for (; length > 0 && ((int)entry[offset] == (int)Path.DirectorySeparatorChar || (int)entry[offset] == (int)Path.AltDirectorySeparatorChar); --length)
                ++offset;
            if (length == 0)
                return string.Empty;
            char[] charArray = entry.ToCharArray(offset, length);
            for (int index = 0; index < charArray.Length; ++index)
            {
                if ((int)charArray[index] == (int)Path.DirectorySeparatorChar || (int)charArray[index] == (int)Path.AltDirectorySeparatorChar)
                    charArray[index] = s_pathSeperator;
            }
            return new string(charArray);
        }

        private static bool IsDirEmpty(DirectoryInfo possiblyEmptyDir)
        {
            using (IEnumerator<FileSystemInfo> enumerator = possiblyEmptyDir.EnumerateFileSystemInfos("*", SearchOption.AllDirectories).GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    FileSystemInfo current = enumerator.Current;
                    return false;
                }
            }
            return true;
        }
    }
}
