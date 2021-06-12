using HSFrameWork.Common;
using HSFrameWork.ConfigTable.Editor.Impl;
using UnityEditor;
using GLib;
using UnityEngine;
using HSFrameWork.ConfigTable.Editor;

namespace Jyx2.Editor
{
    ///所有生成数据的菜单命令的入口
    public class FinalPackMenuCmd
    {
        #region 打包机专用
        /// <summary>
        /// 打包机专用，相当于调用 Tools♥/[打包IOS]
        /// </summary>
        //[MenuItem("Tools♥/码农专用/[打包机 GenData4IOS简体]", false)]
        public static void PackSlient()
        {
            using (ProgressBarAutoHide.Get(0))
            using (HSUtils.ExeTimer("菜单: [打包机 GenData4IOS简体]"))
            using (RunTimeConfiger.EnterRobotMode)
                Pack4IOS();
        }

        /// <summary>
        /// 打包机专用，相当于调用 Tools♥/[打包ANDROID]
        /// </summary>
        //[MenuItem("Tools♥/码农专用/[打包机 GenData4Android简体]", false)]
        public static void Pack4AndroidSlient()
        {
            using (ProgressBarAutoHide.Get(0))
            using (HSUtils.ExeTimer("菜单: [打包机 GenData4Android简体]"))
            using (RunTimeConfiger.EnterRobotMode)
                Pack4Current();
        }
        #endregion

        //[MenuItem("Tools♥/[打包IOS简体]")]
        static void Pack4IOS()
        {
            MenuHelper.SafeWrapMenuAction("打包IOS简体", title => PackAllDataInner(true, title, false, null));
        }

        //[MenuItem("Tools♥/[打包IOS港澳台]")]
        static void Pack4IOSCHT()
        {
            MenuHelper.SafeWrapMenuAction("打包IOS港澳台", title => PackAllDataInner(true, title, false, "CHT"));
        }

        [MenuItem("配置表/[当前平台打包]")]
        static void Pack4Current()
        {
            MenuHelper.SafeWrapMenuAction("当前平台打包", title => PackAllDataInner(false, title, false, null));
        }

        //[MenuItem("Tools♥/[打包ANDROID港澳台]")]
        static void Pack4AndroidCHT()
        {
            MenuHelper.SafeWrapMenuAction("打包ANDROID港澳台", title => PackAllDataInner(false, title, false, "CHT"));
        }

        static void PackAllDataInner(bool isBuildForIOS, string title, bool force, string language)
        {
            using (HSUtils.ExeTimer("PackAllDataInner {0}".f(title)))
            {
                var bk = HSCTC.ActiveLanguage;
                HSCTC.ActiveLanguage = language;
                try
                {
                    GenDataMenuCmd.GenPackDataAllTheWay(title, force, true, false); //支持启动前和运行中取消
                    MoveAndGenMD5(isBuildForIOS, title);
                    Debug.LogWarning("▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬");
                    Debug.LogWarning("▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬ [{0}] 语言包 [{1}] 全部完成，恭喜。".EatWithTID(title, HSCTC.DisplayActiveLanguage).PadRight(60, '▬'));
                    Debug.LogWarning("▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬");
                    MenuHelper.SafeShow100Progress(title);
                }
                finally
                {
                    HSCTC.ActiveLanguage = bk;
                }
            }
        }

        private static void MoveAndGenMD5(bool isBuildForIOS, string title)
        {
            /*MenuHelper.SafeDisplayProgressBar(title, "AssetTool.Move*", 0.8f);
                AssetTool.MoveInAllExternalAB_CurrentTarget();
                */

            MenuHelper.SafeDisplayProgressBar(title, "GenerateABMd5", 0.9f);
            MD5Menu.GenerateStreamingAssetsMD5Summary();
        }

        [MenuItem("配置表/[!!清空所有配置表缓存!!]", false)]
        static void ClearAllCache()
        {
            if (MenuHelper.SafeDisplayDialog("请确认", "是否需要删除所有和XML、filter、lua相关的所有中间和目标文件？", "是", "取消"))
            {
                GenDataHelper.ClearAllCache();
                AssetDatabase.Refresh();
            }
        }
    }
}
