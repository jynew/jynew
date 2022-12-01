namespace UIWidgets.Examples
{
    using UIWidgets;
    using UnityEngine;

    /// <summary>
    /// HierarchyView node drag support.
    /// </summary>
    [RequireComponent(typeof(HierarchyItemView))]
    public partial class HierarchyViewNodeDragSupport : TreeViewCustomNodeDragSupport<HierarchyItemView, HierarchyItemView, GameObject>
    {
    }
}