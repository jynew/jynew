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
using System.Linq;
using i18n.TranslatorDef;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace i18n.Editor
{
    /// <summary>
    /// 自定义翻译面板绘制
    /// </summary>
    public class TranslationEditMenu : OdinEditorWindow
    {
        /// <summary>
        /// 默认窗口绘制
        /// 并添加了菜单选项条目
        /// </summary>
        [MenuItem("项目快速导航/翻译内容编辑")]
        private static void OpenWindow()
        {
            GetWindow<TranslationEditMenu>().titleContent = new GUIContent() { text = "翻译内容编辑" };
            GetWindow<TranslationEditMenu>().Show();
        }

        #region 翻译文件设置和读取

        /// <summary>
        /// 需要读取的翻译Translator文件
        /// 之所以这样设置的原因是因为可以拥有多个翻译来源文件，打包时候只选取一个即可
        /// 不必将所有文件统一打包到一个地方，方便进行操作
        /// </summary>
        [BoxGroup("翻译文件")] [LabelText("翻译来源文件")]
        public Translator defaultTranslator;

        /// <summary>
        /// 翻译内容列表
        /// </summary>
        [BoxGroup("翻译内容")] [Searchable] [TableList(ShowIndexLabels = true, ShowPaging = true, NumberOfItemsPerPage = 1)]
        public List<Translations> allTrabslations = new List<Translations>();
        
        /*
        /// <summary>
        /// 将Translator文件内的翻译内容读取到本窗口
        /// </summary>
        [BoxGroup("翻译文件")]
        [Button(ButtonSizes.Small, Name = "从文件读取翻译内容")]
        public void ReadTranslator()
        {
            allTrabslations.Clear();//清空防止重复
            //添加内容
            defaultTranslator.TranslationSet.ForEach(set =>
            {
                var translationContent = new TranslationContent();
                translationContent.Content = set.Key;
                translationContent.Translations = new List<TranslationsUnion>();
                set.Value.ForEach(translation =>
                {
                    var translationsUnion = new TranslationsUnion
                    {
                        Lang = translation.Key, TranslateContent = translation.Value
                    };
                    translationContent.Translations.Add(translationsUnion);
                });
                translationContents.Add(translationContent);
            });
        }

        #endregion

        #region 翻译主体部分相关


        /// <summary>
        /// 自动全部翻译成拼音
        /// 配合TranslationContents类中的GenPinYin实现
        /// </summary>
        [BoxGroup("翻译内容")]
        [Button(ButtonSizes.Small, Name = "全部自动翻译")]
        public void TranslateAll2English()
        {
            translationContents.ForEach(translationContent =>
            {
                translationContent.GenPinYin();
                //下面本来用于百度API接口翻译的等待，用来配合QPS
                //Thread.Sleep(1000);
            });
        }

        /// <summary>
        /// 翻译内容保存到Translator
        /// 保存前会先提示你是否操作（防止误操作）
        /// ------------------------------------------------------
        /// Q:为什么不直接修改Translator文件？
        /// A:因为直接修改会反复绘制Translator的字典列表导致大量内存消耗
        /// ------------------------------------------------------
        /// </summary>
        [BoxGroup("翻译内容")]
        [Button(ButtonSizes.Small, Name = "保存翻译")]
        public void UpdateTranslator()
        {
            //询问是否操作
            if (EditorUtility.DisplayDialog("警告", "此操作会先清除Translator中的所有已翻译内容，确定操作吗？", "Yes", "No"))
            {
                defaultTranslator.TranslationSet.Clear();//清空Translator内容
                translationContents.ForEach(translationContent =>
                {
                    if (translationContent.isDelete) return;//需要删除的则不转化
                    
                    //添加内容
                    defaultTranslator.TranslationSet.Add(translationContent.Content,
                        new Dictionary<TranslationUtility.LangFlag, string>());
                    translationContent.Translations.ForEach(translation =>
                    {
                        defaultTranslator.TranslationSet[translationContent.Content]
                            .Add(translation.Lang, translation.TranslateContent);
                    });
                });
                //重新读取到本地数组
                ReadTranslator();
                //告诉你成功了
                EditorUtility.DisplayDialog("信息", "已经完成翻译保存", "OK");
            }
        }

        */
        #endregion
    }
}