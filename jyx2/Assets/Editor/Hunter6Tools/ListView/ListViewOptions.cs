using System;
namespace WH.Editor
{
    /// <summary>
    /// 列表选项
    /// </summary>
	internal enum ListViewOptions
	{
        /// <summary>
        /// 拖动重新排序
        /// </summary>
		wantsReordering = 1,
        /// <summary>
        /// 接受外部文件拖放
        /// </summary>
		wantsExternalFiles,
        /// <summary>
        /// 自定义拖曳
        /// </summary>
		wantsToStartCustomDrag = 4,
        /// <summary>
        /// 接受自定义拖曳
        /// </summary>
		wantsToAcceptCustomDrag = 8
	}
}
