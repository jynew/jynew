//using HJJH2.Crossplatform.SavePojo;

using UnityEngine;
using UnityEditor;
using System.IO;
using GLib;
using HSFrameWork.Common.Editor;
using HSFrameWork.ConfigTable.Editor;
using System.Collections.Generic;
using HSFrameWork.Common;
using Jyx2;
using Jyx2.Setup;

namespace Jyx2.Editor
{
    /// <summary>
    /// Gen-Pack Data 相关程序的上下文
    /// </summary>
    public static class GPDC
    {
        public static readonly string LuaPathShort = LuaManager.LUAJIT_ENABLE ? "/luajit" : "/lua";
        public static readonly string LuaPath = HSCTC.AppDataPath.StandardSub("/../data" + LuaPathShort);
        public static readonly string LuaBytes = Application.dataPath.StandardSub("/../data/data/lua.bytes");

        public static readonly string FilterFile = Application.dataPath.StandardSub("/../data/data/filter.txt");

        public static readonly string CachePath = HSCTC.CachePath;
        public static readonly string FilterTSFile = CachePath.StandardSub("filter.ts");
        public static readonly string CEFilterTSFile = CachePath.StandardSub("cefilter.ts");
        public static readonly string LastLuaSummaryFile = CachePath.StandardSub("luasummary");

        private const string _firstOpenProjKey = "_jyx2_FirstOpenProj_";

        private static bool IsFirstOpenProj
        {
            get => EditorPrefs.GetBool(_firstOpenProjKey, true);
            set => EditorPrefs.SetBool(_firstOpenProjKey,value);
        }


        public static Dictionary<string, string> LanguageResourceMap { get; private set; }

        static GPDC()
        {
            LanguageResourceMap = new Dictionary<string, string>();
#if true
            LanguageResourceMap["3rd"] = HSCTC.AppDataPath.StandardSub("3rd");
            LanguageResourceMap["Atlas"] = HSCTC.AppDataPath.StandardSub("Atlas");
            LanguageResourceMap["BuildSource"] = HSCTC.AppDataPath.StandardSub("BuildSource");
            LanguageResourceMap["Resources"] = HSCTC.AppDataPath.StandardSub("Resources");
            LanguageResourceMap["UI"] = HSCTC.AppDataPath.StandardSub("UI");
            LanguageResourceMap["font"] = HSCTC.AppDataPath;
            LanguageResourceMap["logo"] = HSCTC.AppDataPath;
#endif
        }

        /// <summary>
        /// 不能在用的时候才初始化，因为有可能第一次使用是在线程池中。
        /// </summary>
        [InitializeOnLoadMethod]
        public static void OnProjectLoadedInEditor()
        {
            HSBootEditor.ColdBind(ConStr.GLOBAL_DESKEY, HSConfigTableInitHelperEditor.Create(), ConStr.NLogConfigAssetPath);
            Container.Register<IXLsReloader>(x => DesignToolMenu.Instance, ReuseScope.Container);

            Directory.CreateDirectory(CachePath);
            EditorPlayMode.PlayModeChanged += OnPlayModeChanged;

            if (IsFirstOpenProj)
            {
                Debug.Log("首次打开项目自动GenData");
                GenDataMenuCmd.GenerateData();
                IsFirstOpenProj = false;
            }
        }

        private static void OnPlayModeChanged(PlayModeState currentState, PlayModeState changedState)
        {
            if (changedState == PlayModeState.Stopped || (changedState == PlayModeState.Playing && currentState == PlayModeState.Stopped))
            {
                //if (LuaManagerStatus.LuaManagerInited)
                //    LuaManager.Clear();
            }
        }
    }
}
