namespace UIWidgets
{
	/// <summary>
	/// Base class for custom ListView for items with variable height.
	/// </summary>
	/// <typeparam name="TItemView">Type of DefaultItem component.</typeparam>
	/// <typeparam name="TItem">Type of item.</typeparam>
	public class ListViewCustomHeight<TItemView, TItem> : ListViewCustomSize<TItemView, TItem>
		where TItemView : ListViewItem
	{
	}
}