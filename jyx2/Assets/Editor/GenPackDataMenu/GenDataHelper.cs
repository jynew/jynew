using GLib;
using HSFrameWork.Common;
using HanSquirrel.ResourceManager;
using HSFrameWork.ConfigTable.Editor;

namespace Jyx2.Editor
{
    public static class GenDataHelper
    {


        public static void ClearAllCache()
        {
            HSUtils.LogWarning("清理和XML、filter、lua、ABMd5相关的所有中间文件和目标文件。");

            HSCTC.ClearAllCacheAndDstFiles();

            Mini.ClearDirectory(GPDC.CachePath);

            GPDC.LuaBytes.Delete();
            GPDC.LastLuaSummaryFile.Delete();
        }

    }
}
