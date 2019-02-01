using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace AntDeploy.Util
{
    public class ZipHelper
    {
        private static readonly char s_pathSeperator = '/';

        /// <summary>
        /// 打包文件夹
        /// </summary>
        /// <param name="sourceDirectoryName"></param>
        /// <param name="compressionLevel"></param>
        /// <param name="includeBaseDirectory"></param>
        /// <returns></returns>
        public static byte[] DoCreateFromDirectory(string sourceDirectoryName, CompressionLevel? compressionLevel, bool includeBaseDirectory, List<string> ignoreList = null)
        {
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
                    foreach (FileSystemInfo enumerateFileSystemInfo in directoryInfo.EnumerateFileSystemInfos("*", SearchOption.AllDirectories))
                    {
                        flag = false;
                        int length = enumerateFileSystemInfo.FullName.Length - fullName.Length;
                        string entryName = EntryFromPath(enumerateFileSystemInfo.FullName, fullName.Length, length);

                        if (enumerateFileSystemInfo is FileInfo)
                        {
                            if (ignoreList != null)
                            {
                                var haveMatch = false;
                                foreach (var ignorRule in ignoreList)
                                {
                                    var isMatch = Regex.Match(enumerateFileSystemInfo.Name, ignorRule);
                                    if (isMatch.Success)
                                    {
                                        haveMatch = true;
                                        break;
                                    }
                                }

                                if (haveMatch)
                                {
                                    continue;
                                }
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
                return outStream.GetBuffer();
            }
        }

        public static MemoryStream DoCreateFromDirectory2(string sourceDirectoryName, CompressionLevel? compressionLevel, bool includeBaseDirectory, List<string> ignoreList = null)
        {
            sourceDirectoryName = Path.GetFullPath(sourceDirectoryName);
            var outStream = new MemoryStream();
            using (ZipArchive destination = new ZipArchive(outStream, ZipArchiveMode.Create, true))
            {
                bool flag = true;
                DirectoryInfo directoryInfo = new DirectoryInfo(sourceDirectoryName);
                string fullName = directoryInfo.FullName;
                if (includeBaseDirectory && directoryInfo.Parent != null)
                    fullName = directoryInfo.Parent.FullName;
                foreach (FileSystemInfo enumerateFileSystemInfo in directoryInfo.EnumerateFileSystemInfos("*", SearchOption.AllDirectories))
                {
                    flag = false;
                    int length = enumerateFileSystemInfo.FullName.Length - fullName.Length;
                    string entryName = EntryFromPath(enumerateFileSystemInfo.FullName, fullName.Length, length);

                    if (enumerateFileSystemInfo is FileInfo)
                    {
                        if (ignoreList != null)
                        {
                            var haveMatch = false;
                            foreach (var ignorRule in ignoreList)
                            {
                                var isMatch = Regex.Match(enumerateFileSystemInfo.Name, ignorRule);
                                if (isMatch.Success)
                                {
                                    haveMatch = true;
                                    break;
                                }
                            }

                            if (haveMatch)
                            {
                                continue;
                            }
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
            outStream.Seek(0, SeekOrigin.Begin);
            return outStream;
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
