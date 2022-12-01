namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <content>
	/// Base class for custom ListViews.
	/// </content>
	public partial class ListViewCustom<TItemView, TItem> : ListViewCustomBase
		where TItemView : ListViewItem
	{
		/// <summary>
		/// ListView components pool.
		/// </summary>
		public class ListViewComponentPool
		{
			/// <summary>
			/// Indices difference.
			/// </summary>
			protected class Diff
			{
				/// <summary>
				/// Indices of the added items.
				/// </summary>
				protected List<int> Added = new List<int>();

				/// <summary>
				/// Indices of the removed items.
				/// </summary>
				protected List<int> Removed = new List<int>();

				/// <summary>
				/// Indices of the untouched items.
				/// </summary>
				protected List<int> Untouched = new List<int>();

				/// <summary>
				/// Indices of the displayed items.
				/// </summary>
				public List<int> Displayed = new List<int>();

				/// <summary>
				/// Calculate difference.
				/// </summary>
				/// <param name="current">Current indices.</param>
				/// <param name="required">Required indices.</param>
				public void Calculate(List<int> current, List<int> required)
				{
					Added.Clear();
					Removed.Clear();
					Untouched.Clear();
					Displayed.Clear();

					foreach (var index in required)
					{
						if (!current.Contains(index))
						{
							Added.Add(index);
						}
					}

					foreach (var index in current)
					{
						if (!required.Contains(index))
						{
							Removed.Add(index);
						}
						else
						{
							Untouched.Add(index);
						}
					}

					Displayed.AddRange(required);
				}

				/// <summary>
				/// Check if indices are same.
				/// </summary>
				/// <param name="current">Current indices.</param>
				/// <param name="required">Required indices.</param>
				/// <returns>true if indices are same; otherwise false.</returns>
				public bool Same(List<int> current, List<int> required)
				{
					if (current.Count != required.Count)
					{
						return false;
					}

					for (int i = 0; i < current.Count; i++)
					{
						if (current[i] != required[i])
						{
							return false;
						}
					}

					return true;
				}
			}

			/// <summary>
			/// The owner.
			/// </summary>
			public readonly ListViewCustom<TItemView, TItem> Owner;

			/// <summary>
			/// Owner ID.
			/// </summary>
			public readonly InstanceID OwnerID;

			/// <summary>
			/// Indices difference.
			/// </summary>
			protected Diff IndicesDiff = new Diff();

			/// <summary>
			/// Components comparer delegate.
			/// </summary>
			protected Comparison<TItemView> ComponentsComparerDelegate;

			/// <summary>
			/// Initializes a new instance of the <see cref="ListViewComponentPool"/> class.
			/// Use parents lists to avoid problem with creating copies of the original ListView.
			/// </summary>
			/// <param name="owner">Owner.</param>
			internal ListViewComponentPool(ListViewCustom<TItemView, TItem> owner)
			{
				Owner = owner;
				OwnerID = new InstanceID(owner);
				ComponentsComparerDelegate = ComponentsComparer;
			}

			/// <summary>
			/// Returns an enumerator that iterates through the <see cref="ListViewComponentPool" />.
			/// </summary>
			/// <returns>A <see cref="ListViewBase.ListViewComponentEnumerator{TItemView, TTemplateWrapper}" /> for the <see cref="ListViewComponentPool" />.</returns>
			public ListViewComponentEnumerator<TItemView, Template> GetEnumerator()
			{
				return new ListViewComponentEnumerator<TItemView, Template>(OwnerID, PoolEnumeratorMode.Active, Owner.Templates);
			}

			/// <summary>
			/// Returns an enumerator that iterates through the <see cref="ListViewComponentPool" />.
			/// </summary>
			/// <param name="mode">Mode.</param>
			/// <returns>A <see cref="ListViewBase.ListViewComponentEnumerator{TItemView, TTemplateWrapper}" /> for the <see cref="ListViewComponentPool" />.</returns>
			public ListViewComponentEnumerator<TItemView, Template> GetEnumerator(PoolEnumeratorMode mode)
			{
				return new ListViewComponentEnumerator<TItemView, Template>(OwnerID, mode, Owner.Templates);
			}

			/// <summary>
			/// Init this instance.
			/// </summary>
			public void Init()
			{
				foreach (var template in Owner.Templates)
				{
					template.UpdateId();
					template.AddCallbacks(OwnerID, Owner);
				}
			}

			/// <summary>
			/// Process locale changes.
			/// </summary>
			public void LocaleChanged()
			{
				foreach (var template in Owner.Templates)
				{
					template.LocaleChanged(OwnerID);
				}
			}

			/// <summary>
			/// Find component with the specified index.
			/// </summary>
			/// <param name="index">Index.</param>
			/// <returns>Component with the specified index.</returns>
			public TItemView Find(int index)
			{
				for (int i = 0; i < Owner.Components.Count; i++)
				{
					if (Owner.Components[i].Index == index)
					{
						return Owner.Components[i];
					}
				}

				return null;
			}

			/// <summary>
			/// Get template.
			/// </summary>
			/// <param name="component">Component.</param>
			/// <returns>Template.</returns>
			public Template GetTemplate(TItemView component)
			{
				var id = new InstanceID(component);

				foreach (var template in Owner.Templates)
				{
					if (template.TemplateID == id)
					{
						return template;
					}
				}

				var created = CreateTemplate(component);
				Owner.Templates.Add(created);

				return created;
			}

			/// <summary>
			/// Create template.
			/// </summary>
			/// <param name="component">Component.</param>
			/// <returns>Template.</returns>
			public Template CreateTemplate(TItemView component)
			{
				var created = ListViewItemTemplate<TItemView>.Create<Template>(component);
				created.AddCallbacks(OwnerID, Owner);

				return created;
			}

			/// <summary>
			/// Get template by item index.
			/// </summary>
			/// <param name="index">Index.</param>
			/// <returns>Template.</returns>
			public Template GetTemplate(int index)
			{
				var template = Owner.Index2Template(index);

				return GetTemplate(template);
			}

			/// <summary>
			/// Set the DisplayedIndices.
			/// </summary>
			/// <param name="newIndices">New indices.</param>
			/// <param name="action">Action.</param>
			public void DisplayedIndicesSet(List<int> newIndices, Action<TItemView> action)
			{
				PrepareInstances(newIndices);

				RequestInstances(newIndices, action, allAsNew: true);

				UpdateOwnerData(newIndices);
			}

			/// <summary>
			/// Update the DisplayedIndices.
			/// </summary>
			/// <param name="newIndices">New indices.</param>
			/// <param name="action">Action.</param>
			public void DisplayedIndicesUpdate(List<int> newIndices, Action<TItemView> action)
			{
				if (IndicesDiff.Same(Owner.ComponentsDisplayedIndices, newIndices))
				{
					return;
				}

				PrepareInstances(newIndices);

				RequestInstances(newIndices, action, allAsNew: false);

				UpdateOwnerData(newIndices);
			}

			/// <summary>
			/// Prepare instances to display specified indices.
			/// </summary>
			/// <param name="indices">Indices.</param>
			protected void PrepareInstances(List<int> indices)
			{
				foreach (var template in Owner.Templates)
				{
					template.RequiredInstances = 0;
				}

				foreach (var index in indices)
				{
					GetTemplate(index).Require(OwnerID, index);
				}

				foreach (var template in Owner.Templates)
				{
					template.RequestInstances(Owner, OwnerID);
				}
			}

			/// <summary>
			/// Request instances to display specified indices.
			/// </summary>
			/// <param name="indices">Indices.</param>
			/// <param name="action">Action.</param>
			/// <param name="allAsNew">Treat all instances as new.</param>
			protected void RequestInstances(List<int> indices, Action<TItemView> action, bool allAsNew = false)
			{
				Owner.Components.Clear();

				bool is_new;
				foreach (var index in indices)
				{
					var instance = GetTemplate(index).RequestInstance(OwnerID, index, out is_new);
					Owner.Components.Add(instance);

					if (is_new || allAsNew)
					{
						instance.Index = index;
						action(instance);
					}
				}
			}

			/// <summary>
			/// Update owner data.
			/// </summary>
			/// <param name="indices">Indices.</param>
			protected void UpdateOwnerData(List<int> indices)
			{
				SetOwnerItems();

				Owner.ComponentsDisplayedIndices.Clear();
				Owner.ComponentsDisplayedIndices.AddRange(indices);

				Owner.Components.Sort(ComponentsComparerDelegate);
				foreach (var c in Owner.Components)
				{
					if (Owner.ReversedOrder)
					{
						c.RectTransform.SetAsFirstSibling();
					}
					else
					{
						c.RectTransform.SetAsLastSibling();
					}
				}
			}

			/// <summary>
			/// Set the owner items.
			/// </summary>
			protected void SetOwnerItems()
			{
				Owner.UpdateComponents(Owner.Components);
			}

			/// <summary>
			/// Compare components by component index.
			/// </summary>
			/// <returns>A signed integer that indicates the relative values of x and y.</returns>
			/// <param name="x">The x coordinate.</param>
			/// <param name="y">The y coordinate.</param>
			protected int ComponentsComparer(TItemView x, TItemView y)
			{
				return Owner.ComponentsDisplayedIndices.IndexOf(x.Index).CompareTo(Owner.ComponentsDisplayedIndices.IndexOf(y.Index));
			}

			/// <summary>
			/// Apply function for each active component.
			/// </summary>
			/// <param name="action">Action.</param>
			public void ForEach(Action<TItemView> action)
			{
				foreach (var component in GetEnumerator(PoolEnumeratorMode.Active))
				{
					action(component);
				}
			}

			/// <summary>
			/// Apply function for each active and cached components.
			/// </summary>
			/// <param name="action">Action.</param>
			public void ForEachAll(Action<TItemView> action)
			{
				foreach (var component in GetEnumerator(PoolEnumeratorMode.All))
				{
					action(component);
				}
			}

			/// <summary>
			/// Apply function for each cached component.
			/// </summary>
			/// <param name="action">Action.</param>
			public void ForEachCache(Action<TItemView> action)
			{
				foreach (var component in GetEnumerator(PoolEnumeratorMode.Cache))
				{
					action(component);
				}
			}

			/// <summary>
			/// Apply function for each cached component.
			/// </summary>
			/// <param name="action">Action.</param>
			public void ForEachCache(Action<ListViewItem> action)
			{
				foreach (var component in GetEnumerator(PoolEnumeratorMode.Cache))
				{
					action(component);
				}
			}

			/// <summary>
			/// Set size of the components.
			/// </summary>
			/// <param name="size">Size.</param>
			public void SetSize(Vector2 size)
			{
				foreach (var template in Owner.Templates)
				{
					template.SetSize(OwnerID, size);
				}
			}

			/// <summary>
			/// Set the style.
			/// </summary>
			/// <param name="styleBackground">Style for the background.</param>
			/// <param name="styleText">Style for the text.</param>
			/// <param name="style">Full style data.</param>
			public void SetStyle(StyleImage styleBackground, StyleText styleText, Style style)
			{
				foreach (var template in Owner.Templates)
				{
					template.SetStyle(OwnerID, styleBackground, styleText, style);
				}
			}

			/// <summary>
			/// Disable templates.
			/// </summary>
			public virtual void DisableTemplates()
			{
				foreach (var template in Owner.Templates)
				{
					template.DisableTemplate();
				}
			}

			/// <summary>
			/// Destroy cache.
			/// </summary>
			/// <param name="excludeTemplates">Templates to exclude from destroy.</param>
			public virtual void Destroy(TItemView[] excludeTemplates)
			{
				for (int i = Owner.Templates.Count - 1; i >= 0; i--)
				{
					var template = Owner.Templates[i];
					if (Array.IndexOf(excludeTemplates, template.Template) != -1)
					{
						continue;
					}

					template.Destroy(OwnerID);
					Owner.Templates.RemoveAt(i);
				}
			}

			/// <summary>
			/// Destroy cache.
			/// </summary>
			public virtual void Destroy()
			{
				foreach (var template in Owner.Templates)
				{
					template.Destroy(OwnerID);
				}

				Owner.Templates.Clear();
			}
		}
	}
}