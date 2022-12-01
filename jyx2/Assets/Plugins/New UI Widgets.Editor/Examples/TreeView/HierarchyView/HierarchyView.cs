namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// HierarchyView.
	/// </summary>
	public class HierarchyView : TreeViewCustom<HierarchyItemView, GameObject>
	{
		[SerializeField]
		GameObject rootGameObject;

		/// <summary>
		/// Root GameObject.
		/// </summary>
		public GameObject RootGameObject
		{
			get
			{
				return rootGameObject;
			}

			set
			{
				if (rootGameObject != value)
				{
					rootGameObject = value;
					ReloadHierarchy();
				}
			}
		}

		bool isHierarchyViewInited;

		/// <inheritdoc/>
		public override void Init()
		{
			if (isHierarchyViewInited)
			{
				return;
			}

			isHierarchyViewInited = true;

			base.Init();

			ReloadHierarchy();
		}

		/// <summary>
		/// Reload hierarchy.
		/// </summary>
		/// <param name="root">Root.</param>
		/// <param name="nodes">Nodes.</param>
		protected virtual void ReloadHierarchy(Transform root, ObservableList<TreeNode<GameObject>> nodes)
		{
			nodes.BeginUpdate();

			nodes.Clear();

			for (int i = 0; i < root.childCount; i++)
			{
				var t = root.GetChild(i);
				var node = new TreeNode<GameObject>(t.gameObject, new ObservableList<TreeNode<GameObject>>());
				ReloadHierarchy(t, node.Nodes);

				nodes.Add(node);
			}

			nodes.EndUpdate();
		}

		/// <summary>
		/// Reload hierarchy.
		/// </summary>
		public virtual void ReloadHierarchy()
		{
			if (Nodes == null)
			{
				Nodes = new ObservableList<TreeNode<GameObject>>();
			}

			if (RootGameObject == null)
			{
				Nodes.Clear();
				return;
			}

			ReloadHierarchy(RootGameObject.transform, Nodes);
		}
	}
}