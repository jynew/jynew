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
            if (_path != null)
            {
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
    }
}