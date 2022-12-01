namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// HierarchyView node drop support.
	/// </summary>
	[RequireComponent(typeof(HierarchyItemView))]
	public partial class HierarchyViewNodeDropSupport : TreeViewCustomNodeDropSupport<HierarchyView, HierarchyItemView, GameObject>
	{
		/// <summary>
		/// Handle dropped data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public override void Drop(TreeNode<GameObject> data, PointerEventData eventData)
		{
			var type = GetDropType(eventData);

			TreeView.Nodes.BeginUpdate();

			var current = Source.Node;
			var parent = current.RealParent;
			int index;

			switch (type)
			{
				case TreeNodeDropType.Child:
					current.IsExpanded = true;
					data.Parent = current;

					data.Item.transform.SetParent(current.Item.transform);
					UpdateTransformOrder(current.Nodes);
					break;
				case TreeNodeDropType.Before:
					data.Parent = null;
					index = parent.Nodes.IndexOf(current);
					parent.Nodes.Insert(index, data);

					data.Item.transform.SetParent(parent.Item.transform);
					UpdateTransformOrder(parent.Nodes);
					break;
				case TreeNodeDropType.After:
					data.Parent = null;
					index = parent.Nodes.IndexOf(current) + 1;
					parent.Nodes.Insert(index, data);

					data.Item.transform.SetParent(parent.Item.transform);
					UpdateTransformOrder(parent.Nodes);
					break;
			}

			TreeView.Nodes.EndUpdate();

			HideDropIndicator();

			HoldCancel();
		}

		void UpdateTransformOrder(ObservableList<TreeNode<GameObject>> nodes)
		{
			for (int i = 0; i < nodes.Count; i++)
			{
				nodes[i].Item.transform.SetSiblingIndex(i);
			}
		}
	}
}