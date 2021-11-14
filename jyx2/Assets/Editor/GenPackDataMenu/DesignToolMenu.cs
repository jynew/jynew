using System.Threading;
using UnityEditor;
using HSFrameWork.Common;
using HSFrameWork.ConfigTable.Editor.Impl;
using HSFrameWork.ConfigTable;
using HSFrameWork.ConfigTable.Editor;
using Jyx2.Setup;

namespace Jyx2.Editor
{
    public class DesignToolMenu : HSSingleton<DesignToolMenu>, IXLsReloader
    {
        //[MenuItem("配置表/[重载策划配置]", true)]
        public static bool CanNewModeBeUsed()
        {
            return EditorApplication.isPlaying || EditorApplication.isPaused;
        }

        //[MenuItem("配置表/[重载策划配置]", false, 0)]
        public static void UpdateAll()
        {
            MenuHelper.SafeWrapMenuAction("★重载策划配置★", UpdateAllInner);
        }


        public static void UpdateAllInner(string title)
        {
            MenuHelper.SafeDisplayProgressBar(title, "检查新XLS需要转换为XML", 0.1f);

            using (HSUtils.ExeTimer("Xls2XMLHelperWin.SafeRunBlocked"))
                Xls2XMLHelperWin.SafeRunBlocked(false);

            MenuHelper.SafeDisplayProgressBar(title, "加载更新的XML", 0.5f);

            using (ProgressBarAutoHide.Get(0))
            {
                ConfigTable.VisitValues((beanDict) =>
                {
                    XMLBDUpdater.Instance.UpdateChanged(beanDict, CancellationToken.None, (s, p) => MenuHelper.SafeDisplayProgressBar("加载XML", s, p));
                });

                MenuHelper.SafeDisplayProgressBar(title, "查看lua更新情况", 0.7f);
                //LuaMenu.UpdateLuaInDesignMode(title);
            }

            MenuHelper.SafeShow100Progress(title);
        }

        void IXLsReloader.Do()
        {
            UpdateAll();
        }
    }
}
