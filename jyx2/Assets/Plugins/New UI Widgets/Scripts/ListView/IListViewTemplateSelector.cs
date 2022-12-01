namespace UIWidgets
{
	/// <summary>
	/// ListView template selector interface.
	/// </summary>
	/// <typeparam name="TItemView">Component type.</typeparam>
	/// <typeparam name="TItem">Item type.</typeparam>
	public interface IListViewTemplateSelector<TItemView, TItem>
		where TItemView : ListViewItem
	{
		/// <summary>
		/// Get all possible templates.
		/// </summary>
		/// <returns>Templates.</returns>
		TItemView[] AllTemplates();

		/// <summary>
		/// Select template by item.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="item">Item.</param>
		/// <returns>Template.</returns>
		TItemView Select(int index, TItem item);
	}
}