using UnityEditor;
using HSFrameWork.ConfigTable.Editor.Impl;
using HSFrameWork.ConfigTable.Editor;
using HSFrameWork.Common;
using UnityEngine;
using GLib;
using HanSquirrel.ResourceManager;

namespace Jyx2.Editor
{
    ///所有生成数据的菜单命令的入口
    public class GenDataMenuCmd
    {
        [MenuItem("配置表/[GEN DATA]")]
        public static void GenerateData()
        {
            using (HSUtils.ExeTimer("菜单：[GEN DATA]"))
                MenuHelper.SafeWrapMenuAction("GENERATE DATA", title => GenerateDataWithEnding(title, false, false));
        }

        [MenuItem("配置表/[GEN DATA (强制更新)]")]
        public static void GenerateDataForce()
        {
            using (HSUtils.ExeTimer("菜单：[GEN DATA (强制更新)]"))
                MenuHelper.SafeWrapMenuAction("GENERATE DATA重新转换XLS", title => GenerateDataWithEnding(title, true, false));
        }

        //[MenuItem("Tools♥/[★★GEN DATA命令行]")]
        static void GenerateDataExe()
        {
            using (HSUtils.ExeTimer("菜单：[GEN DATA]"))
                MenuHelper.SafeWrapMenuAction("GENERATE DATA", title => GenerateDataWithEnding(title, false, true));
        }

        //[MenuItem("Tools♥/[★★GEN DATA命令行 (重载XLS)]")]
        static void GenerateDataForceExe()
        {
            using (HSUtils.ExeTimer("菜单：[GEN DATA (重载XLS)]"))
                MenuHelper.SafeWrapMenuAction("GENERATE DATA重新转换XLS", title => GenerateDataWithEnding(title, true, true));
        }

        private static void GenerateDataWithEnding(string title, bool force, bool xls2XMLExe)
        {
            GenPackDataAllTheWay(title, force, false, xls2XMLExe);
            MenuHelper.SafeShow100Progress(title);
        }

        /// <summary>
        /// 生成Assets/StreamingAssets/目录下的[value,filter,lua]
        /// 清理无用AB包 →→→ XLS →→→ 加密压缩的 Value/filter/lua →→→ BuildCurrent
        /// </summary>
        public static void GenPackDataAllTheWay(string title, bool force, bool buildab, bool xls2XMLExe)
        {
            using (HSUtils.ExeTimer("GenDataAllTheWay [{0}]".f(HSCTC.DisplayActiveLanguage)))
            {
                BeanDictMenu.GenBeanDictAllTheWay(title, force, xls2XMLExe); //【Assets/StreamingAssets/value】

                using (HSUtils.ExeTimer("ZipEncFilter"))
                {
                    MenuHelper.SafeDisplayProgressBar(title, "FinalPackHelper.ZipAndEncryptFilter", 0.6f);
                    GenDataHelper.ZipAndEncryptFilter();  //【Assets/StreamingAssets/filter】
                }

                using (HSUtils.ExeTimer("PackZipEncLua"))
                {
                    MenuHelper.SafeDisplayProgressBar(title, "FinalPackHelper.PackZipAndEncryptLua", 0.7f);
                    LuaMenu.PackZipAndEncryptLua(true); //【Assets/StreamingAssets/lua】
                }

                if (HSCTC.ConfigPath.Sub("skip_copy_language_assets").ExistsAsFile())
                {
                    HSUtils.LogWarning("当前配置禁用语言相关资源的复制。");
                }
                else
                {
                    using (HSUtils.ExeTimer("Sync Language Resources"))
                    {
                        foreach (var kv in GPDC.LanguageResourceMap)
                        {
                            var src = HSCTC.ActiveLanguageDir.StandardSub(kv.Key);
                            var dst = kv.Value;
                            if (src.ExistsAsFolder())
                            {
                                HSUtils.Log("Copying [{0}] to [{1}] ...", src, dst);
                                Mini.DirectoryCopy(src, dst, true, true);
                            }
                        }
                    }
                }

                using (HSUtils.ExeTimer("AssetDatabase.Refresh"))
                {
                    MenuHelper.SafeDisplayProgressBar(title, "AssetDatabase.Refresh", 0.8f);
                    AssetDatabase.Refresh();
                }

                if (buildab)
                {
                    using (HSUtils.ExeTimer("AssetTool.BuildCurrent"))
                    {
                        MenuHelper.SafeDisplayProgressBar(title, "AssetTool.BuildCurrent", 0.9f);
                        /*AssetTool.BuildCurrent();*/
                        Debug.LogWarning("▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ [ 成功完成BuildCurrent ] ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲".EatWithTID());
                    }
                }
                else if (ResourceLoader.LoadFromABAlways)
                {
                    HSUtils.LogWarning("当前设置永远从AB包加载。如果不打AB包，程序运行可能会异常。");
                }
            }
        }
    }
}
