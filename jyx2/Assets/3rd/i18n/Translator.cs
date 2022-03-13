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
using System.IO;
using i18n.Ext;
using i18n.TranslateAttacher;
using i18n.TranslatorDef;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace i18n
{
    /// <summary>
    /// 翻译器组件
    /// 所有的翻译基本都是基于此组件延伸
    /// 所有的翻译内容也是从该文件产生的Instance中读取
    /// </summary>
    [CreateAssetMenu(menuName = "翻译/新建Translator"), Serializable]
    public class Translator : SerializedScriptableObject
    {
        #region 翻译设置相关

        /// <summary>
        /// 翻译语言类别，用于转化为json文件
        /// </summary>
        [BoxGroup("翻译设置"), LabelText("翻译语言")] public TranslationUtility.LangFlag currentLang;
        
        /// <summary>
        /// 是否收集新的文本(游玩时)
        /// ------------------------------------------------
        /// 当我们进行游玩的时候，设置其为true则可以收集未翻译的新文本，方便后期翻译未翻译的文本
        /// 设置为false显然可以降低其内存使用，在语言完善翻译之后，显然可以可以不用继续收集，保证了Translator不会继续增长
        /// ------------------------------------------------
        /// </summary>
        [BoxGroup("翻译设置"), LabelText("是否收集")] public bool isCollectNewText;

        /// <summary>
        /// 文件輸出目錄
        /// </summary>
        [BoxGroup("翻译设置"), LabelText("文件輸出目錄"), FolderPath]
        public string outPath;

        /// <summary>
        /// 輸出翻譯為Json文件
        /// </summary>
        [BoxGroup("翻译设置"), Button(ButtonSizes.Medium, Name = "转化为Json")]
        public void Convert2Json()
        {
            using var sw = new StreamWriter(Path.Combine(outPath, $"{currentLang}.json"));
            sw.WriteLine(JsonUtility.ToJson(new Serialization<string, Translations>(TranslationSet), true));
            sw.Close();
        }

        /// <summary>
        /// 從Json讀取翻譯内容
        /// </summary>
        [BoxGroup("翻译设置"), Button(ButtonSizes.Medium, Name = "從Json讀取")]
        public void ReadFromJson()
        {
            using var sr = new StreamReader(Path.Combine(outPath, $"{currentLang}.json"));
            TranslationSet = JsonUtility.FromJson<Serialization<string, Translations>>(sr.ReadToEnd()).ToDictionary();
            sr.Close();
        }

        #endregion

        #region 翻译内容显示

        /// <summary>
        /// 所有翻译的集合
        /// </summary>
        [BoxGroup("翻译查看"), LabelText("翻译列表"), TableList(AlwaysExpanded = true, DrawScrollView = true)]
        public Dictionary<string, Translations> TranslationSet = new Dictionary<string, Translations>();

        #endregion

#if UNITY_EDITOR

        #region Text组件翻译

        //-------------------------------------------------------------------------
        //基本来说，需要翻译的部分就两个内容
        //一个是场景中的Text组件，另外一个是预制体中的Text组件
        //通过观察，场景目前只有一个；预制体只有几个面板需要
        //且他们的工作量都不大
        //这个部分只在编辑器中需要，打包之后并不需要
        //-------------------------------------------------------------------------

        /// <summary>
        /// 需要转化的场景列表
        /// </summary>
        [BoxGroup("Gameobject组件翻译"), LabelText("场景列表")]
        public List<SceneAsset> sceneList = new List<SceneAsset>();

        /// <summary>
        /// 需要转化的预制体列表
        /// </summary>
        [BoxGroup("Gameobject组件翻译"), LabelText("Prefab列表")]
        public List<GameObject> prefabList = new List<GameObject>();

        /// <summary>
        /// 给添加的Object进行组件绑定
        /// 对场景和预制体分别处理
        /// </summary>
        [BoxGroup("Gameobject组件翻译"), Button(ButtonSizes.Medium, Name = "对上述物体生成Attacher")]
        public void AddTextAttacher()
        {
            //----------------------------------------------------
            //场景处理，其原理基本如下：
            //遍历所有场景，对于每一个场景，打开它，获取所有Text组件，然后绑定TextAttacher（在此之前先清除所有TextAttacher）
            //确定绑定完毕后保存场景
            //---------------------------------------------------
            sceneList.ForEach(sceneAsset =>
            {
                var scene = EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(sceneAsset), OpenSceneMode.Single);
                scene.GetRootGameObjects()
                    .ForEach(rootGameObj =>
                    {
                        rootGameObj.GetComponentsInChildren<Text>(true)
                            .ForEach(textScript =>
                            {
                                while (textScript.gameObject.GetComponent<TextAttacher>())
                                    DestroyImmediate(textScript.gameObject.GetComponent<TextAttacher>());
                                if (isCollectNewText)
                                    textScript.gameObject.AddComponent<TextAttacher>().Refresh();
                                else
                                    textScript.gameObject.AddComponent<TextAttacher>();

                                EditorSceneManager.SaveScene(scene);
                                Debug.Log($"{textScript.gameObject.GetPath()}_添加Attacher成功.");
                            });
                        rootGameObj.GetComponentsInChildren<Dropdown>(true)
                            .ForEach(dropdownScript =>
                            {
                                while (dropdownScript.gameObject.GetComponent<DropdownAttacher>())
                                    DestroyImmediate(dropdownScript.gameObject.GetComponent<DropdownAttacher>());
                                dropdownScript.gameObject.AddComponent<DropdownAttacher>();
                                if (isCollectNewText)
                                    dropdownScript.gameObject.AddComponent<DropdownAttacher>().Refresh();
                                else
                                    dropdownScript.gameObject.AddComponent<DropdownAttacher>();

                                EditorSceneManager.SaveScene(scene);
                                Debug.Log($"{dropdownScript.gameObject.GetPath()}_添加Attacher成功.");
                            });
                    });
            });

            //预制体处理
            prefabList.ForEach(prefab =>
            {
                var tempPrefab = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                if (tempPrefab == null) return;
                tempPrefab.GetComponentsInChildren<Text>(true)
                    .ForEach(textScript =>
                    {
                        while (textScript.GetComponent<TextAttacher>())
                        {
                            DestroyImmediate(textScript.GetComponent<TextAttacher>());
                        }

                        if (isCollectNewText)
                            textScript.gameObject.AddComponent<TextAttacher>().Refresh();
                        else
                            textScript.gameObject.AddComponent<TextAttacher>();
                        Debug.Log($"{prefab.gameObject}_添加Attacher成功.");
                    });
                tempPrefab.GetComponentsInChildren<Dropdown>(true)
                    .ForEach(dropdownScript =>
                    {
                        while (dropdownScript.gameObject.GetComponent<DropdownAttacher>())
                            DestroyImmediate(dropdownScript.gameObject.GetComponent<DropdownAttacher>());
                        if (isCollectNewText)
                            dropdownScript.gameObject.AddComponent<DropdownAttacher>().Refresh();
                        else
                            dropdownScript.gameObject.AddComponent<DropdownAttacher>();
                        Debug.Log($"{dropdownScript.gameObject.GetPath()}_添加Attacher成功.");
                    });
                PrefabUtility.ApplyPrefabInstance(tempPrefab, InteractionMode.AutomatedAction);
                DestroyImmediate(tempPrefab);
            });
        }

        #endregion

#endif

        #region 文本获取和注册函数

        /// <summary>
        /// 获取文本的对应翻译内容
        /// </summary>
        /// <param name="fromToken">来源标记，仅当第一次添加时需要标记</param>
        /// <param name="contentStr">文本内容</param>
        /// <returns></returns>
        public string GetOrRegTranslation(string fromToken, string contentStr)
        {
            //不存在该翻译
            if (!TranslationSet.ContainsKey(contentStr))
            {
                if (isCollectNewText)
                {
                    TranslationSet.Add(contentStr, new Translations
                    {
                        token = fromToken, //标记来源
                        content = contentStr, //记录内容
                        translation = string.Empty
                    });
                    Debug.LogWarning($"【已收集】没有{contentStr}对应的翻译组内容！");
                }
                else
                {
                    Debug.LogWarning($"没有{contentStr}对应的翻译组内容！");
                }

                return contentStr;
            }

            //如果还没翻译
            return string.IsNullOrEmpty(TranslationSet[contentStr].translation)
                ? contentStr
                : TranslationSet[contentStr].translation;
        }

        #endregion
    }
}