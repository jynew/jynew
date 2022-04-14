using System;
using System.Collections.Generic;
using System.IO;

namespace HanSquirrel.OpenSource
{
    ///GG 这里是BasicMod使用的一些扩展函数。纯粹个人习惯。如果不喜欢就直接替换得了。
    public static class MiscExts
    {
        public static bool IsNullOrEmptyG<T>(this List<T> list) => list == null || list.Count == 0;

        public static bool Visible(this string msg) => !string.IsNullOrWhiteSpace(msg);

        public static string Sub(this string path, string sub)
        {
            if (sub.StartsWith("/") || sub.StartsWith(@"\"))
                sub = sub.Substring(1);

            return Path.GetFullPath(Path.Combine(path, sub));
        }

        public static FileInfo[] GetFiles(this string folder, string pattern, SearchOption option)
        {
            return !folder.ExistsAsFolder() ? Array.Empty<FileInfo>() : new DirectoryInfo(folder).GetFiles(pattern, option);
        }

        public static bool ExistsAsFolder(this string path) => path != null && Directory.Exists(path);

        /// <summary>
        /// 会自动创建好文件所在目录。返回file
        /// </summary>
        public static string WriteAllTextF(this string file, string str)
        {
            new FileInfo(file).Directory.Create();
            File.WriteAllText(file, str);
            return file;
        }
        
        public static string ReplaceFileExt(this string path, string ext)
            => path.GetDir().Sub(path.NameWithoutExt() + ext);

        public static string GetDir(this string file) => new FileInfo(file).Directory.FullName;

        public static string NameWithoutExt(this string path) => Path.GetFileNameWithoutExtension(path);

        /// File.Move(file, dst)
        /// 会自动创建目标目录
        public static void MoveFileTo(this string file, string dst)
        {
            new FileInfo(dst).Directory.Create();
            File.Move(file, dst);
        }

        public static string CreateDir(this string path)
        {
            if (path.Visible() && !Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }


        /// <summary>
        /// 清空目录里面的所有内容（包括所有子目录和文件）。目录不存在也不会异常。
        /// </summary>
        public static string ClearDirectory(this string folder)
        {
            DirectoryInfo di = new DirectoryInfo(folder);
            if (di.Exists)
            {
                foreach (var dir in di.GetDirectories("*", SearchOption.TopDirectoryOnly))
                {
                    ClearDirectory(dir.FullName);
                    dir.Delete();
                }

                foreach (var file in di.GetFiles("*", SearchOption.TopDirectoryOnly))
                {
                    if (file.Attributes != FileAttributes.Normal)
                        file.Attributes = FileAttributes.Normal;
                    file.Delete();
                }
            }
            return folder;
        }
    }

    public sealed class DummyDisposable : IDisposable
    {
        public static readonly IDisposable I = new DummyDisposable();

        public void Dispose() { }
    }
}
