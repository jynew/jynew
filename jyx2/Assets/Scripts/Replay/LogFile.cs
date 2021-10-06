/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class LogFile
{
    public string fileFullPath;
    public string fileName;
    uint _maxFileSize;
    FileStream _fileStream;
    StreamWriter _writer;
    StreamReader _reader;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileSubPath">譬如 logs/game.log</param>
    /// <param name="maxFileSize">文件大小最大限制，字节为单位，值为0表示不限制</param>
    /// <param name="isCreateNew">为false表示如果已存在同名文件，要新建，为true则不新建</param>
    public LogFile(string fileSubPath, uint maxFileSize=0, bool isCreateNew=false)
    {
		fileFullPath = Path.Combine(Application.persistentDataPath, fileSubPath);
        fileName = new FileInfo(fileFullPath).Name;
        _maxFileSize = maxFileSize;

        if (isCreateNew)
        {
            if (File.Exists(fileFullPath))
                File.Delete(fileFullPath);
        }
        checkIsNeedClearLogFile();

        string dir = Path.GetDirectoryName(fileFullPath);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        _fileStream = new FileStream(fileFullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);// 不会锁死, 允许其它程序打开
        _writer = new StreamWriter(_fileStream);
        _reader = new StreamReader(_fileStream);
    }

    public void OnDispose()
    {
        if (_writer != null)
        {
            _writer.Close();
            _writer.Dispose();
            _writer = null;
        }
        if (_fileStream != null)
        {
            _fileStream.Dispose();
            _fileStream = null;
        }
    }

    public void Log(string szMsg)
    {
        _writer.Write(szMsg);
        _writer.Flush();
    }
    public void LogLine(string szMsg)
    {
        _writer.WriteLine(szMsg);
        _writer.Flush();
    }
    public string GetLastLines(int lineCount)
    {
        long position = _fileStream.Position; // 记录位置
        _fileStream.Position = 0; // 起始位置

        List<string> list = new List<string>();
        while (!_reader.EndOfStream)
        {
            string line = _reader.ReadLine();
            list.Add(line);
        }

        if (list.Count > lineCount)
            list.RemoveRange(0, list.Count - lineCount);

        string result = string.Empty;
        for (int i = list.Count - 1; i >= 0; i--)
            result += list[i] + "\r\n";

        _fileStream.Position = position; // 恢复位置
        return result;
    }
    void checkIsNeedClearLogFile()
    {
        if (File.Exists(fileFullPath))
        {
            FileInfo fileinfo = new FileInfo(fileFullPath);
            if (_maxFileSize > 0 && fileinfo.Length > _maxFileSize)//大于5M
                File.Delete(fileFullPath);
        }
    }
}
