namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// HierarchyView drop support.
	/// </summary>
	[RequireComponent(typeof(HierarchyView))]
	public partial class HierarchyViewDropSupport : TreeViewCustomDropSupport<HierarchyView, HierarchyItemView, GameObject>
	{
		/// <inheritdoc/>
		public override void Drop(TreeNode<GameObject> data, PointerEventData eventData)
		{
			if (Source.Nodes == null)
			{
				Source.Nodes = new ObservableList<TreeNode<GameObject>>();
			}

			var index = Source.GetNearestIndex(eventData, DropPosition);
			var dropped = false;
			if (Source.IsValid(index))
			{
				var nearest_node = Source.DataSource[index].Node;
				var nearest_parent = nearest_node.Parent;
				if (nearest_parent == null)
				{
					data.Parent = null;
					Source.Nodes.Insert(index, data);
					dropped = true;

					data.Item.transform.SetParent(Source.RootGameObject.transform);
					UpdateTransformOrder(Source.Nodes);
				}
				else if (data.CanBeParent(nearest_parent))
				{
					index = nearest_parent.Nodes.IndexOf(nearest_node);
					data.Parent = null;
					nearest_parent.Nodes.Insert(index, data);
					dropped = true;

					data.Item.transform.SetParent(nearest_parent.Item.transform);
					UpdateTransformOrder(nearest_parent.Nodes);
				}
			}

			if (!dropped)
			{
				data.Parent = null;
				Source.Nodes.Add(data);

				data.Item.transform.SetParent(Source.RootGameObject.transform);
				UpdateTransformOrder(Source.Nodes);
			}

			HideDropIndicator();
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