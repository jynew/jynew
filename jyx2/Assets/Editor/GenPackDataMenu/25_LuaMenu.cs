using System;
using System.IO;
using System.Linq;
using UnityEditor;
using HSFrameWork.Common;
using HSFrameWork.ConfigTable.Editor.Impl;
using HSFrameWork.Common.Editor;
using HSFrameWork.ConfigTable.Editor;
using Jyx2.Editor;
using Jyx2;

namespace Jyx2.Editor
{
    public class LuaMenu
    {
        //George 暂时取消对这两个菜单项的自动变灰 2018-0117
        //[MenuItem("Tools♥/repack lua", true)]

        public static bool GameRunning()
        {
            return EditorApplication.isPlaying || EditorApplication.isPaused;
        }

        [MenuItem("XLua/Repack lua")]
        public static void RepackLuaMenu()
        {
            RepackLua(true);
        }

        [MenuItem("XLua/Repack lua(强制更新)")]
        public static void ForceRepackLuaMenu()
        {
            RepackLua(false);
        }

        public static void RepackLua(bool updateOnly)
        {
            using (HSUtils.ExeTimer("RepackLua"))
            {
                PackZipAndEncryptLua(updateOnly);
                AssetDatabase.Refresh();
                if (GameRunning())
                {
                    LuaManager.Init(true);
                    //TriggerManager.Init();
                }
            }
        }

        private static DateTime _lastReloadTime;

        /// <summary>
        /// 看是否有lua文件更新，如果有，则重新打包Lua
        /// </summary>
        public static void UpdateLuaInDesignMode(string title)
        {
            using (HSUtils.ExeTimer("UpdateLuaInDesignMode"))
            {
                DateTime now = DateTime.Now;
                if (new DirectoryInfo("data/lua").GetFiles("*.lua", SearchOption.AllDirectories)
                    .Where(fi => fi.LastWriteTime > _lastReloadTime)
                    .Any())
                {
                    MenuHelper.SafeDisplayProgressBar(title, "重新载入LUA", 0.8f);
                    RepackLua(true);
                    _lastReloadTime = now;
                }
            }
        }

        /// <summary>
        /// 编译所有的LUAJIT，老代码尚未修改（GG）
        /// </summary>
        //[MenuItem("Tools♥/complie luajit")]
        public static void ComplieLuaJit()
        {
            var files = Directory.GetFiles("./data/lua", "*.lua", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var fileName = file.Replace('\\', '/');
                var targetFile = fileName.Replace("/lua/", "/luajit/");
                var targetDir = targetFile.Replace(targetFile.Split('/').Last(), "");
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                var cmd = string.Format("-b {0} {1}", fileName, targetFile);
                //RunCommand (cmd);

                System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                info.FileName = @"luajit";
                info.Arguments = cmd;
                info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
                System.Diagnostics.Process pro = System.Diagnostics.Process.Start(info);
                pro.WaitForExit();
            }
        }

        private static string GetLuaNameInPack(string luaFile)
        {
            luaFile = luaFile.Replace('\\', '/');
            return luaFile.Substring(luaFile.LastIndexOf(GPDC.LuaPathShort));
        }

        /// <summary>
        /// 打包-压缩-加密所有Lua。每次打包会记录每个LUA文件的日期和大小。下次打包时判断如果有任何变化，就会重新打包；
        /// 如果没有变化而且updateOnly是true，则不会打包。如果updateOnly是false，则永远重新打包。
        /// </summary>
        public static void PackZipAndEncryptLua(bool updateOnly)
        {
            FolderPackUtils.PackZipAndEncryptFolder(updateOnly, "Lua", "*.lua",
                GPDC.LuaPath, GPDC.LuaBytes, GPDC.LastLuaSummaryFile, GetLuaNameInPack, HSUnityEnv.CELuaPath);
        }
    }
}
