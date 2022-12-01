namespace UIWidgets
{
	/// <summary>
	/// Base class for custom ListViews.
	/// </summary>
	public partial class ListViewCustom<TItemView, TItem> : ListViewCustomBase
		where TItemView : ListViewItem
	{
		/// <summary>
		/// Default template selector.
		/// </summary>
		protected class DefaultSelector : IListViewTemplateSelector<TItemView, TItem>
		{
			/// <summary>
			/// Template.
			/// </summary>
			protected TItemView Template;

			/// <summary>
			/// Templates.
			/// </summary>
			protected TItemView[] Templates;

			/// <summary>
			/// Initializes a new instance of the <see cref="DefaultSelector"/> class.
			/// </summary>
			/// <param name="template">Template.</param>
			public DefaultSelector(TItemView template)
			{
				Template = template;
				Templates = new[] { template };
			}

			/// <summary>
			/// Replace template.
			/// </summary>
			/// <param name="template">Template.</param>
			public void Replace(TItemView template)
			{
				Template = template;
				Templates = new[] { template };
			}

			/// <inheritdoc/>
			public TItemView Select(int index, TItem item)
			{
				return Template;
			}

			/// <inheritdoc/>
			public TItemView[] AllTemplates()
			{
				return Templates;
			}
		}
	}
}