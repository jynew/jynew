/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 *
 * 本文件作者：东方怂天(EasternDay)
 */

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace i18n.TranslatorDef
{
    /// <summary>
    /// 翻译结构体
    /// </summary>
    [Serializable]
    public class Translations
    {
        /// <summary>
        /// 记录内容来源
        /// </summary>
        [TextArea(5, 20)]
        public string token;

        /// <summary>
        /// 翻译对应原文本
        /// </summary>
        [TextArea(5, 20)]
        public string content;

        /// <summary>
        /// 翻译对应的所有翻译集合
        /// </summary>
        [TableList(AlwaysExpanded = true, DrawScrollView = true)]
        public Dictionary<TranslationUtility.LangFlag, string> Dict = new Dictionary<TranslationUtility.LangFlag, string>();
    }
}