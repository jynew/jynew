using GLib;
using HSFrameWork.Common;
using HanSquirrel.ResourceManager;
using HSFrameWork.ConfigTable.Editor;

namespace Jyx2.Editor
{
    public static class GenDataHelper
    {
        /// <summary>
        /// 压缩加密filter.txt，自动判断是否更新。只要filter日期变化，就会重新打包。
        /// </summary>
        public static void ZipAndEncryptFilter()
        {
            if(GPDC.FilterFile.Exists() && HSUnityEnv.CEFilterPath.Exists() && 
               GPDC.FilterTSFile.Exists() && GPDC.CEFilterTSFile.Exists() &&
               GPDC.FilterFile.LastWriteTime() == GPDC.FilterTSFile.LastWriteTime() &&
               HSUnityEnv.CEFilterPath.LastWriteTime() == GPDC.CEFilterTSFile.LastWriteTime())
            {
                HSUtils.Log("[{0}] 没有更新，因此不用重新生成 [{1}]。".Eat(GPDC.FilterFile.ShortName(), HSUnityEnv.CEFilterPath.ShortName()));
            }
            else
            {
                HSUtils.Log("[{0}]有更新，因此重新生成 [{1}]。".Eat(GPDC.FilterFile.ShortName(), HSUnityEnv.CEFilterPath.ShortName()));
                BinaryResourceLoader.SaveCEBinary(GPDC.FilterFile, HSUnityEnv.CEFilterPath);
                GPDC.FilterFile.Touch(GPDC.FilterTSFile);
                HSUnityEnv.CEFilterPath.Touch(GPDC.CEFilterTSFile);
            }
        }

        public static void ClearAllCache()
        {
            HSUtils.LogWarning("清理和XML、filter、lua、ABMd5相关的所有中间文件和目标文件。");

            HSCTC.ClearAllCacheAndDstFiles();

            Mini.ClearDirectory(GPDC.CachePath);

            GPDC.LuaBytes.Delete();
            GPDC.LastLuaSummaryFile.Delete();

            GPDC.FilterTSFile.Delete();
            GPDC.CEFilterTSFile.Delete();
        }

    }
}
