namespace UIWidgets.Examples
{
	using System.Collections;
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test shared templates.
	/// </summary>
	public class TestListViewSharedTemplates : MonoBehaviour
	{
		/// <summary>
		/// ListViews.
		/// </summary>
		[SerializeField]
		protected List<ListViewIcons> Lists;

		List<ListViewIcons.Template> SharedTemplates = new List<ListViewIcons.Template>();

		List<ListViewIcons.Template> SharedTemplates2 = new List<ListViewIcons.Template>();

		/// <summary>
		/// Set shared templates.
		/// </summary>
		public void SetShared()
		{
			foreach (var l in Lists)
			{
				l.SetSharedTemplates(SharedTemplates);
			}
		}

		/// <summary>
		/// Reset shared templates.
		/// </summary>
		public void ResetShared()
		{
			foreach (var l in Lists)
			{
				l.SetSharedTemplates(null);
			}
		}

		/// <summary>
		/// Change shared templates.
		/// </summary>
		/// <param name="index">Index.</param>
		public void ChangeShared(int index)
		{
			Lists[index].SetSharedTemplates(SharedTemplates2);
		}

		/// <summary>
		/// Add items.
		/// </summary>
		/// <param name="index">Index.</param>
		public void AddItems(int index)
		{
			var items = Lists[index].DataSource;
			for (int i = 0; i < 40; i++)
			{
				var item = new ListViewIconsItemDescription() { Name = "Added " + i };
				items.Add(item);
			}
		}

		/// <summary>
		/// Clear.
		/// </summary>
		/// <param name="index">Index.</param>
		public void Clear(int index)
		{
			Lists[index].DataSource.Clear();
		}

		/// <summary>
		/// Destroy.
		/// </summary>
		/// <param name="index">Index.</param>
		public void Destroy(int index)
		{
			Destroy(Lists[index].gameObject);
		}

		/// <summary>
		/// Reset shared templates.
		/// </summary>
		/// <param name="index">Index.</param>
		public void ResetShared(int index)
		{
			Lists[index].SetSharedTemplates(null);
		}

		/// <summary>
		/// Duplicate.
		/// </summary>
		/// <param name="index">Index.</param>
		public void Duplicate(int index)
		{
			var source = Lists[index];
			var copy = Instantiate(source, source.transform.parent);

			(copy.transform as RectTransform).anchoredPosition = new Vector2(-300, 200);
			copy.DataSource.Clear();
			copy.DataSource.AddRange(source.DataSource);
		}
	}
}