

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace EZ4i18n{

/// <summary>
/// 翻译器主体，被设计为静态类。
/// 建议设计为单例模式。
/// </summary>
public class Translator
{
    #region 单例模式实现

    /// <summary>
    /// 单例模式对应的单例
    /// </summary>
    private static Translator instance;

    /// <summary>
    /// 单例模式获取单例的方法
    /// </summary>
    /// <returns>Translator的单例</returns>
    public static Translator GetInstance()
    {
        if (instance == null)
        {
            var translator = new Translator();
            Interlocked.CompareExchange<Translator>(ref instance, translator, null);
        }

        return instance;
    }

    #endregion

    #region 默认语言设定、获取

    private static string _defaultLang = "中文";

    /// <summary>
    /// 设置默认语言
    /// </summary>
    /// <param name="s">设置的默认的语言</param>
    public static void SetDefaultLang(string s)
    {
        _defaultLang = s;
    }

    /// <summary>
    /// 设置默认语言
    /// </summary>
    /// <param name="s">设置的默认的语言</param>
    public static string GetDefaultLang()
    {
        return _defaultLang;
    }

    #endregion


    #region 翻译语言设置

    private static string _curLang = "中文";

    /// <summary>
    /// 设置当前翻译语言
    /// </summary>
    /// <param name="s">设置的语言名称</param>
    public static void SetCurLang(string s)
    {
        _curLang = s;
    }

    /// <summary>
    /// 设置当前翻译语言
    /// </summary>
    /// <param name="s">设置的语言名称</param>
    public static string GetCurLang()
    {
        return _curLang;
    }

    #endregion

    #region 翻译文件存储路径设置

    private static string _langPath;

    public static void SetLangPath(string path)
    {
        _langPath = path;
    }

    public static string GetLangPath()
    {
        return _langPath;
    }

    #endregion

    public Dictionary<string, List<string>> translations = new Dictionary<string, List<string>>(); //翻译内容对应字典
    public Dictionary<string, int> sentence_index = new Dictionary<string, int>(); //默认对应对应文本的文字索引

    private Translator()
    {
        //判断给定路径是否存在，如果不存在就报错
        System.Diagnostics.Debug.Assert(Directory.Exists(_langPath),
            "The given language file directory does not exist!");
        //读取语言目录下所有txt文件
        var langDrectoryInfo = new DirectoryInfo(_langPath);
        foreach (var file in langDrectoryInfo.GetFiles("*.txt"))
        {
            var indexKey = Path.GetFileNameWithoutExtension(file.Name); //按照文件名作为翻译字典所听
            translations.Add(indexKey, new List<string>()); //增加对应项目
            var strArray = File.ReadAllLines(file.FullName); //读取文本内所有行
            for (var i = 0; i < strArray.Length; i++)
            {
                translations[indexKey].Add(strArray[i]); //增加对应翻译语句
                if (indexKey == GetDefaultLang())
                    //如果是默认语言，则在句子索引中增加记录
                    sentence_index.Add(strArray[i], i);
            }
        }
    }


    public string GetTranslation(string s)
    {
        var transSentenceList = new List<string>();
        var sentenceList = s.Split(new []{"\n","\r\n"}, StringSplitOptions.None);
        foreach (var sentence in sentenceList)
        {
            if (!sentence_index.ContainsKey(sentence)) 
                transSentenceList.Add(sentence);
            else
            {
                var index = sentence_index[sentence];
                transSentenceList.Add(translations[_curLang][index]);
            }
        }

        return string.Join("\n",transSentenceList);
    }
}

public static class StrExtensions
{
    /// <summary>
    /// 翻译
    /// String类的拓展方法
    /// </summary>
    /// <param name="str">来源的String</param>
    /// <returns>翻译结果</returns>
    public static string Translate(this string str)
    {
        return Translator.GetInstance().GetTranslation(str);
    }
}
}