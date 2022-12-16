using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TapTap.Common.Editor
{
    public class TapFileHelper : System.IDisposable
    {
        private string filePath;

        public TapFileHelper(string fPath)
        {
            filePath = fPath;
            if (!System.IO.File.Exists(filePath))
            {
                Debug.LogError(filePath + "路径下文件不存在");
                return;
            }
        }

        public void WriteBelow(string below, string text)
        {
            StreamReader streamReader = new StreamReader(filePath);
            string all = streamReader.ReadToEnd();
            streamReader.Close();
            int beginIndex = all.IndexOf(below, StringComparison.Ordinal);
            if (beginIndex == -1)
            {
                Debug.LogError(filePath + "中没有找到字符串" + below);
                return;
            }

            int endIndex = all.LastIndexOf("\n", beginIndex + below.Length, StringComparison.Ordinal);
            all = all.Substring(0, endIndex) + "\n" + text + "\n" + all.Substring(endIndex);
            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(all);
            streamWriter.Close();
        }

        public void Replace(string below, string newText)
        {
            StreamReader streamReader = new StreamReader(filePath);
            string all = streamReader.ReadToEnd();
            streamReader.Close();
            int beginIndex = all.IndexOf(below, StringComparison.Ordinal);
            if (beginIndex == -1)
            {
                Debug.LogError(filePath + "中没有找到字符串" + below);
                return;
            }

            all = all.Replace(below, newText);
            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(all);
            streamWriter.Close();
        }

        public void Dispose()
        {
        }

        public static void CopyAndReplaceDirectory(string srcPath, string dstPath)
        {
            if (Directory.Exists(dstPath))
                Directory.Delete(dstPath, true);
            if (File.Exists(dstPath))
                File.Delete(dstPath);

            Directory.CreateDirectory(dstPath);

            foreach (var file in Directory.GetFiles(srcPath))
                File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));

            foreach (var dir in Directory.GetDirectories(srcPath))
                CopyAndReplaceDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
        }


        public static void DeleteFileBySuffix(string dir, string[] suffix)
        {
            if (!Directory.Exists(dir))
            {
                return;
            }

            foreach (var file in Directory.GetFiles(dir))
            {
                foreach (var suffixName in suffix)
                {
                    if (file.Contains(suffixName))
                    {
                        File.Delete(file);
                    }
                }
            }
        }

        public static string FilterFileByPrefix(string srcPath, string filterName)
        {
            if (!Directory.Exists(srcPath))
            {
                return null;
            }

            foreach (var dir in Directory.GetDirectories(srcPath))
            {
                string fileName = Path.GetFileName(dir);
                if (fileName.StartsWith(filterName))
                {
                    return Path.Combine(srcPath, Path.GetFileName(dir));
                }
            }

            return null;
        }

        public static string FilterFileBySuffix(string srcPath, string suffix)
        {
            if (!Directory.Exists(srcPath))
            {
                return null;
            }

            foreach (var dir in Directory.GetDirectories(srcPath))
            {
                string fileName = Path.GetFileName(dir);
                if (fileName.StartsWith(suffix))
                {
                    return Path.Combine(srcPath, Path.GetFileName(dir));
                }
            }

            return null;
        }

        public static FileInfo RecursionFilterFile(string dir, string fileName)
        {
            List<FileInfo> fileInfoList = new List<FileInfo>();
            Director(dir, fileInfoList);
            foreach (FileInfo item in fileInfoList)
            {
                if (fileName.Equals(item.Name))
                {
                    return item;
                }
            }

            return null;
        }

        public static void Director(string dir, List<FileInfo> list)
        {
            DirectoryInfo d = new DirectoryInfo(dir);
            FileInfo[] files = d.GetFiles();
            DirectoryInfo[] directs = d.GetDirectories();
            foreach (FileInfo f in files)
            {
                list.Add(f);
            }

            foreach (DirectoryInfo dd in directs)
            {
                Director(dd.FullName, list);
            }
        }
    }
}