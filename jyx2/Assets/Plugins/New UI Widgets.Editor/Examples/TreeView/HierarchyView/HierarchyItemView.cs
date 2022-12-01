namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// HierarchyItemView.
	/// </summary>
	public class HierarchyItemView : TreeViewComponentBase<GameObject>, IViewData<GameObject>
	{
		GameObject item;

		/// <summary>
		/// Gets or sets the item.
		/// </summary>
		/// <value>The item.</value>
		public GameObject Item
		{
			get
			{
				return item;
			}

			set
			{
				item = value;

				UpdateView();
			}
		}

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="depth">Depth.</param>
		public override void SetData(TreeNode<GameObject> node, int depth)
		{
			Node = node;
			base.SetData(Node, depth);

			Item = (Node == null) ? null : Node.Item;
		}

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(GameObject item)
		{
			Item = item;
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected virtual void UpdateView()
		{
			if (TextAdapter == null)
			{
				return;
			}

			if (Item == null)
			{
				TextAdapter.text = string.Empty;
			}
			else
			{
				var text = Item.name;
				if (!Item.activeInHierarchy)
				{
					text = string.Format("<color=grey>{0}</color>", text);
				}

				TextAdapter.text = text;
			}
		}
	}
}