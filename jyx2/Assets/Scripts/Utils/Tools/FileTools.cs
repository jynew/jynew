/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Unity.SharpZipLib.Zip;

namespace Jyx2.Middleware
{
    /// <summary>
    /// 文件操作相关工具类
    /// </summary>
    public static class FileTools
    {
        /// <summary>
        /// 获取路径下所有文件
        /// </summary>
        /// <param name="_path">目标路径</param>
        /// <param name="_list"></param>
        public static void GetAllFilePath(string _path, List<string> _list, List<string> _filter = null)
        {
            if (_path == null) return;
            DirectoryInfo _dir = new DirectoryInfo(_path);

            if(!_dir.Exists)
            {
                UnityEngine.Debug.LogWarning("目标文件夹不存在, 路径:" + _path);
                return;
            }

            // GetFileSystemInfos方法可以获取到指定目录下的所有文件以及子文件夹
            FileSystemInfo[] _files = _dir.GetFileSystemInfos();

            for (int i = 0; i < _files.Length; i++)
            {
                // 文件夹
                if (_files[i] is DirectoryInfo)
                {
                    GetAllFilePath(_files[i].FullName, _list, _filter);
                }
                else
                {
                    string _filePath = _files[i].FullName;

                    // 用来判断最后的扩展名
                    string _temp = _filePath.ToLower();

                    if (_filter != null)
                    {
                        bool _b = false;

                        foreach (string _str in _filter)
                        {
                            // 检测结尾
                            if (_temp.EndsWith(_str))
                            {
                                _b = true;
                            }
                        }

                        if (!_b)
                        {
                            continue;
                        }
                    }

                    _filePath = _filePath.Replace("\\", "/");

                    _list.Add(_filePath);
                }
            }
        }

        public static async UniTask<byte[]> ReadAllBytesAsync(string path)
        {
            using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096);
            long length = fileStream.Length;
            if (length > int.MaxValue)
            {
                throw new IOException("File is too larger, more than 2GB, path:" + path);
            }

            int fileLength = (int)length;
            byte[] array = new byte[fileLength];
            await fileStream.ReadAsync(array, 0, fileLength);
            return array;
        }

        /// <summary>
        /// Unity的解压缩库全是同步的，这里加个异步带进度的
        /// </summary>
        /// <param name="archivePath">压缩包路径</param>
        /// <param name="password">压缩包密码</param>
        /// <param name="outputPath">解压缩路径</param>
        /// <returns></returns>
        
        public static async UniTask UnZipAsync(string archivePath, string password, string outputPath, CancellationToken cancellation, Action<long, long> OnProgress = null, int bufferSize = 4096)
        {
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            
            using FileStream stream = File.Open(archivePath, FileMode.Open);
            byte[] result = new byte[stream.Length];
            await stream.ReadAsync(result, 0, (int)stream.Length);
            using ZipFile zipFile = new ZipFile(stream);
            if (!string.IsNullOrEmpty(password))
            {
                zipFile.Password = password;
            }

            long totalBytes = 0;
            foreach (ZipEntry item in zipFile)
            {
                totalBytes += item.CompressedSize;
            }
            long unZippedBytes = 0;
            OnProgress?.Invoke(unZippedBytes, totalBytes);
            foreach (ZipEntry item in zipFile)
            {
                if (!item.IsFile)
                {
                    continue;
                }
                if (cancellation.IsCancellationRequested)
                    break;

                string name = item.Name;
                string path = Path.Combine(outputPath, name);
                string directoryName = Path.GetDirectoryName(path);
                if (directoryName.Length > 0)
                {
                    Directory.CreateDirectory(directoryName);
                }

                byte[] buffer = new byte[bufferSize];
                using Stream source = zipFile.GetInputStream(item);
                using Stream destination = File.Create(path);
                await CopyAsync(source, destination, buffer, cancellation);
                unZippedBytes += item.CompressedSize;
                OnProgress?.Invoke(unZippedBytes, totalBytes);
            }
        }

        public static long GetFileLength(string filePath)
        {
            if (!File.Exists(filePath))
                return 0;
            var fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }

        public static async UniTask CopyAsync(Stream source, Stream destination, byte[] buffer, CancellationToken cancellation)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (buffer.Length < 128)
            {
                throw new ArgumentException("Buffer is too small", "buffer");
            }

            bool isCopying = true;
            while (isCopying)
            {
                if (cancellation.IsCancellationRequested)
                    break;
                int num = await source.ReadAsync(buffer, 0, buffer.Length, cancellation);
                if (num > 0)
                {
                    await destination.WriteAsync(buffer, 0, num, cancellation);
                    continue;
                }

                isCopying = false;
            }
            destination.Flush();
        }

        static readonly string[] suffixes ={ "Bytes", "KB", "MB", "GB", "TB", "PB" };
        public static string FormatSize(long bytes)
        {
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return string.Format("{0:n1}{1}", number, suffixes[counter]);
        }
    }
}