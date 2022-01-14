using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Jyx2.MOD
{
    public class ZipTools
    {
        /// <summary>
        /// 将指定路径的文件或者文件夹压缩为zip文件
        /// </summary>
        /// <param name="srcPath">源文件或源文件夹,绝对路径,形如:D:/Test.txt</param>
        /// <param name="targetPath">目标文件,形如:D:/test.zip</param>
        /// <param name="level">压缩等级</param>
        /// <param name="password">设置密码</param>
        public static void CompressionZipFile(string srcPath, string targetPath, int level = 5, string password = null)
        {
            //如果指定路径下的文件或者文件夹不存在,抛出FileNotFoundException异常
            if (!File.Exists(srcPath) && !Directory.Exists(srcPath))
            {
                throw new FileNotFoundException("压缩文件" + srcPath + "不存在");
            }
            //如果给出的路径是一个文件夹,则判断该文件夹是否为空文件夹(不包含文件),是则抛出FileNotFoundException异常
            if (Directory.Exists(srcPath) && DirectoryIsNull(srcPath))
            {
                throw new FileNotFoundException("压缩目录" + srcPath + "下没有文件");
            }
            ZipOutputStream zipOutputStream = new ZipOutputStream(File.Create(targetPath));
            if (!string.IsNullOrEmpty(password))
            {
                zipOutputStream.Password = password;
            }
            zipOutputStream.SetLevel(level);
            string baseDirName = "";
            srcPath = GetFormatPath(srcPath);
            if (IsRoot(srcPath))
            {
                baseDirName = srcPath;
            }
            else
            {
                baseDirName = GetFormatPath(Path.GetDirectoryName(srcPath)) + "/";
            }
            AddZipEntry(srcPath, zipOutputStream, baseDirName);
            zipOutputStream.Finish();
            zipOutputStream.Close();
        }

        public static void CompressionZipFile(string[] srcPaths, string targetPath, int level = 5, string password = null)
        {
            if (srcPaths == null)
            {
                throw new ArgumentNullException(nameof(srcPaths));
            }
            foreach (var srcPath in srcPaths)
            {
                //如果指定路径下的文件或者文件夹不存在,抛出FileNotFoundException异常
                if (!File.Exists(srcPath) && !Directory.Exists(srcPath))
                {
                    throw new FileNotFoundException("压缩文件" + srcPath + "不存在");
                }
                //如果给出的路径是一个文件夹,则判断该文件夹是否为空文件夹(不包含文件),是则抛出FileNotFoundException异常
                if (Directory.Exists(srcPath) && DirectoryIsNull(srcPath))
                {
                    throw new FileNotFoundException("压缩目录" + srcPath + "下没有文件");
                }
            }
            ZipOutputStream zipOutputStream = new ZipOutputStream(File.Create(targetPath));
            if (!string.IsNullOrEmpty(password))
            {
                zipOutputStream.Password = password;
            }
            zipOutputStream.SetLevel(level);

            for(int i = 0;i<srcPaths.Length;++i)
            {
                string srcPath = srcPaths[i];
                string baseDirName = "";
                //如果给定的目录是一个文件夹,则是否需要将文件夹内的资源放在根目录下
                srcPath = GetFormatPath(srcPath);
                if (IsRoot(srcPath))
                {
                    baseDirName = srcPath;
                }
                else
                {
                    baseDirName = GetFormatPath(Path.GetDirectoryName(srcPath)) + "/";
                }
                AddZipEntry(srcPath, zipOutputStream, baseDirName);
            }

            zipOutputStream.Finish();
            zipOutputStream.Close();
        }

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="srcZipFilePath">压缩文件绝对路径</param>
        /// <param name="targetDirPath">解压文件到指定的目录</param>
        /// <param name="password">密码</param>
        public static void UnCompressionZipFile(string srcZipFilePath, string targetDirPath, string password="")
        {
            if (!Directory.Exists(targetDirPath))
            {
                Directory.CreateDirectory(targetDirPath);
            }
            using (ZipInputStream zipInputStream = new ZipInputStream(File.OpenRead(srcZipFilePath)))
            {
                if (!string.IsNullOrEmpty(password))
                {
                    zipInputStream.Password = password;
                }
                ZipEntry entry;
                while ((entry = zipInputStream.GetNextEntry()) != null)
                {
                    string filePath = Path.Combine(targetDirPath, entry.Name);
                    string fileParentPath = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(fileParentPath))
                    {
                        if (fileParentPath != null) Directory.CreateDirectory(fileParentPath);
                    }
                    using (FileStream fileStream = File.Create(filePath))
                    {
                        byte[] buffer = new byte[1024*1024];
                        int len = 0;
                        while ((len = zipInputStream.Read(buffer, 0, buffer.Length))>0)
                        {
                            fileStream.Write(buffer, 0, len);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 往压缩文件里面添加Entry
        /// </summary>
        /// <param name="srcPath">文件路径</param>
        /// <param name="zipOutputStream">ZipOutputStream</param>
        /// <param name="baseDirName">基础目录</param>
        private static void AddZipEntry(string srcPath, ZipOutputStream zipOutputStream, string baseDirName)
        {
            if (Directory.Exists(srcPath))
            {
                //如果是文件夹则递归调用
                string[] paths = Directory.GetFileSystemEntries(srcPath);
                foreach (var item in paths)
                {
                    AddZipEntry(GetFormatPath(item), zipOutputStream, baseDirName);
                }
            }
            else if (File.Exists(srcPath))
            {
                ZipEntry zipEntry = new ZipEntry(srcPath.Replace(baseDirName, ""));
                zipEntry.IsUnicodeText = true;
                zipOutputStream.PutNextEntry(zipEntry);
                FileInfo fileInfo = new FileInfo(srcPath);
                using (FileStream fileStream = fileInfo.OpenRead())
                {
                    byte[] buffer = new byte[1024 * 1024];
                    int len = 0;
                    while ((len = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        zipOutputStream.Write(buffer, 0, len);
                    }
                    fileStream.Dispose();
                }
            }
        }

        /// <summary>
        /// 用来判断一个目录下是否存在文件
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        private static bool DirectoryIsNull(string dirPath)
        {
            string[] paths = Directory.GetFileSystemEntries(dirPath);
            if (paths.Length == 0)
            {
                return true;
            }
            else
            {
                foreach (var item in paths)
                {
                    if (Directory.Exists(item))
                    {
                        if (!DirectoryIsNull(item))
                        {
                            return false;
                        }
                    }
                    else if (File.Exists(item))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// 格式化路径
        /// </summary>
        /// <param name="srcPath"></param>
        /// <returns></returns>
        private static string GetFormatPath(string srcPath)
        {
            srcPath = Path.GetFullPath(srcPath);
            srcPath = srcPath.Replace(@"\", "/");
            while (srcPath.Length > 1)
            {
                if ('/'.Equals(srcPath[0]) && '/'.Equals(srcPath[1]))
                {
                    srcPath = srcPath.Remove(0, 1);
                }
                else
                {
                    break;
                }
            }
            if (!Path.GetPathRoot(srcPath).Replace(@"\", "/").Equals(srcPath))
            {
                if (srcPath.LastIndexOf("/", StringComparison.Ordinal) == srcPath.Length - 1)
                {
                    srcPath = srcPath.Remove(srcPath.Length - 1);
                }
            }
            return srcPath;
        }

        /// <summary>
        /// 判断一个路径是否为根路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool IsRoot(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            path = GetFormatPath(path);
            if (GetFormatPath(Path.GetPathRoot(path)).Equals(path))
            {
                return true;
            }
            return false;
        }
    }
}