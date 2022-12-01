namespace UIWidgets
{
	/// <summary>
	/// Base class for TileView's with items with different widths or heights.
	/// </summary>
	/// <typeparam name="TItemView">Component class.</typeparam>
	/// <typeparam name="TItem">Item class.</typeparam>
	public class TileViewCustomSize<TItemView, TItem> : ListViewCustom<TItemView, TItem>
		where TItemView : ListViewItem
	{
		[UnityEngine.SerializeField]
		[UnityEngine.HideInInspector]
		int tileViewCustomSizeVersion = 0;

		/// <summary>
		/// Upgrade serialized data to the latest version.
		/// </summary>
		public override void Upgrade()
		{
			base.Upgrade();

			if (tileViewCustomSizeVersion == 0)
			{
				listType = ListViewType.TileViewWithVariableSize;

				tileViewCustomSizeVersion = 1;
			}
		}
	}
}