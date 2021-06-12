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
    }
}