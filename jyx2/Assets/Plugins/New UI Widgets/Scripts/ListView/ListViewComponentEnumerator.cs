namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UIWidgets.Styles;
	using UnityEngine.EventSystems;

	/// <summary>
	/// ListViewBase.
	/// You can use it for creating custom ListViews.
	/// </summary>
	public abstract partial class ListViewBase : UIBehaviour,
			ISelectHandler, IDeselectHandler,
			ISubmitHandler, ICancelHandler,
			IStylable, IUpgradeable
	{
		/// <summary>
		/// Enumerates the elements of a <see cref="ListViewCustom{TItemView, TItem}.ListViewComponentPool" />.
		/// </summary>
		/// <typeparam name="TItemView">Type of the components.</typeparam>
		/// <typeparam name="TTemplateWrapper">Type of the template wrapper.</typeparam>
		[Serializable]
		public struct ListViewComponentEnumerator<TItemView, TTemplateWrapper> : IEnumerator<TItemView>, IDisposable, IEnumerator
			where TItemView : ListViewItem
			where TTemplateWrapper : ListViewItemTemplate<TItemView>, new()
		{
			private readonly PoolEnumeratorMode mode;

			private readonly List<TTemplateWrapper> templates;

			private readonly int maxIndex;

			private int listIndex;

			private PoolEnumerator<TItemView> enumerator;

			private TItemView current;

			private InstanceID ownerID;

			/// <summary>
			/// Initializes a new instance of the <see cref="ListViewComponentEnumerator{TItemView, TTemplateWrapper}"/> struct.
			/// </summary>
			/// <param name="ownerID">Owner ID.</param>
			/// <param name="mode">Mode.</param>
			/// <param name="templates">Templates.</param>
			internal ListViewComponentEnumerator(InstanceID ownerID, PoolEnumeratorMode mode, List<TTemplateWrapper> templates)
			{
				this.ownerID = ownerID;
				this.mode = mode;
				this.templates = templates;

				enumerator = templates.Count > 0 ? templates[0].GetEnumerator(ownerID, mode) : default(PoolEnumerator<TItemView>);
				listIndex = -1;
				maxIndex = templates.Count;
				current = default(TItemView);
			}

			/// <summary>
			/// Releases all resources used by the <see cref="ListViewComponentEnumerator{TItemView, TTemplateWrapper}" />.
			/// </summary>
			public void Dispose()
			{
			}

			/// <summary>
			/// Advances the enumerator to the next element of the <see cref="ListViewCustom{TItemView, TItem}.ListViewComponentPool" />.
			/// </summary>
			/// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
			/// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created. </exception>
			public bool MoveNext()
			{
				if (listIndex == -1)
				{
					listIndex = 0;
				}

				if (listIndex < maxIndex)
				{
					if (enumerator.MoveNext())
					{
						current = enumerator.Current;
						return true;
					}
					else
					{
						listIndex++;
						if (listIndex == maxIndex)
						{
							current = default(TItemView);
							return false;
						}

						enumerator = templates[listIndex].GetEnumerator(ownerID, mode);
					}
				}

				return false;
			}

			/// <summary>
			/// Gets the element at the current position of the enumerator.
			/// </summary>
			/// <returns>The element in the <see cref="ListViewCustom{TItemView, TItem}.ListViewComponentPool" /> at the current position of the enumerator.</returns>
			public TItemView Current
			{
				get
				{
					return current;
				}
			}

			/// <summary>
			/// Gets the element at the current position of the enumerator.
			/// </summary>
			/// <returns>The element in the <see cref="ListViewCustom{TItemView, TItem}.ListViewComponentPool" /> at the current position of the enumerator.</returns>
			/// <exception cref="InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element. </exception>
			object IEnumerator.Current
			{
				get
				{
					if (listIndex == -1 || listIndex == maxIndex)
					{
						throw new InvalidOperationException("The enumerator is positioned before the first element of the collection or after the last element.");
					}

					return Current;
				}
			}

			/// <summary>
			/// Sets the enumerator to its initial position, which is before the first element in the collection.
			/// </summary>
			void IEnumerator.Reset()
			{
				enumerator = templates[0].GetEnumerator(ownerID, mode);
				listIndex = -2;
				current = default(TItemView);
			}

			/// <summary>
			/// Returns an enumerator that iterates through the <see cref="ListViewCustom{TItemView, TItem}.ListViewComponentPool" />.
			/// </summary>
			/// <returns>A <see cref="ListViewComponentEnumerator{TItemView, TTemplateWrapper}" /> for the <see cref="ListViewCustom{TItemView, TItem}.ListViewComponentPool" />.</returns>
			public ListViewComponentEnumerator<TItemView, TTemplateWrapper> GetEnumerator()
			{
				return this;
			}
		}
	}
}