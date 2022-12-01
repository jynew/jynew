namespace UIWidgets
{
	/// <summary>
	/// ListView callbacks.
	/// </summary>
	/// <typeparam name="TItemView">Template type.</typeparam>
	public interface IListViewCallbacks<TItemView>
		where TItemView : ListViewItem
	{
		/// <summary>
		/// The function to call after component instantiated.
		/// </summary>
		/// <param name="instance">Instance.</param>
		void ComponentCreated(TItemView instance);

		/// <summary>
		/// The function to call before component destroyed.
		/// </summary>
		/// <param name="instance">Instance.</param>
		void ComponentDestroyed(TItemView instance);

		/// <summary>
		/// The function to call after component activated.
		/// </summary>
		/// <param name="instance">Instance.</param>
		void ComponentActivated(TItemView instance);

		/// <summary>
		/// The function to call after component moved to cache.
		/// </summary>
		/// <param name="instance">Instance.</param>
		void ComponentCached(TItemView instance);
	}
}