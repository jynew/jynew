using System.IO;
using UnityEditor;

namespace HanSquirrel.OpenSource
{
    ///上层唯一使用的类
    public class BasicModUtilsFacade
    {
        ///仅仅在开发调试的时候才需要keepNewSettings，这样会保留在Build过程中修改的设置，然而原始设置就丢失了。。。。。
        public static void Build(bool keepNewSettings = false) => BasicModUtils.Build(keepNewSettings);

        ///修复所有Addressable的设置。可以通过正常途径直接Build主干。如果发现混乱了，就调用这个保险函数
        public static void FixSettings()
        {
            BasicModUtils.FixSettings_ExcludeAllModGroups();
            BasicModUtils.FixDefaultGroupSettings();
            BasicModUtils.FixRootSettings();
        }
    }

    public class HSLowMenu
    {
        /// <summary>
        /// 在资源管理器中打开这个folder
        /// </summary>
        public static void OpenFolder(string folder)
        {
            if (!folder.ExistsAsFolder())
                return;

            string dummy = null;
            var    files = folder.GetFiles("*", SearchOption.TopDirectoryOnly);
            if (files.Length > 0)
                dummy = files[0].FullName;

            if (dummy == null)
                dummy = folder.Sub(".Dummy").WriteAllTextF("");
            EditorUtility.RevealInFinder(dummy);
        }
    }
}
