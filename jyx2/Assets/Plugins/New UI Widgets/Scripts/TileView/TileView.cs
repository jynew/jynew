namespace UIWidgets
{
	/// <summary>
	/// Alias for TileViewCustom.
	/// </summary>
	/// <typeparam name="TItemView">Component type.</typeparam>
	/// <typeparam name="TItem">Item type.</typeparam>
	public class TileView<TItemView, TItem> : TileViewCustom<TItemView, TItem>
		where TItemView : ListViewItem
	{
	}
}