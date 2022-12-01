namespace UIWidgets
{
	/// <summary>
	/// Base class for custom ListView for items with variable size.
	/// </summary>
	/// <typeparam name="TItemView">Type of DefaultItem component.</typeparam>
	/// <typeparam name="TItem">Type of item.</typeparam>
	public class ListViewCustomSize<TItemView, TItem> : ListViewCustom<TItemView, TItem>
		where TItemView : ListViewItem
	{
		[UnityEngine.SerializeField]
		[UnityEngine.HideInInspector]
		int listViewCustomSizeVersion = 0;

		/// <summary>
		/// Upgrade serialized data to the latest version.
		/// </summary>
		public override void Upgrade()
		{
			base.Upgrade();

			if (listViewCustomSizeVersion == 0)
			{
				listType = ListViewType.ListViewWithVariableSize;

				listViewCustomSizeVersion = 1;
			}
		}
	}
}