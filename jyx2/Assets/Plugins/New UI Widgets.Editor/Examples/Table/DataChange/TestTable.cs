namespace UIWidgets.Examples.DataChange
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Test table.
	/// </summary>
	public class TestTable : MonoBehaviour
	{
		Dictionary<string, Data> data = new Dictionary<string, Data>()
		{
			{ "Item 1", new Data("Item 1", 0) },
			{ "Item 2", new Data("Item 2", 8) },
			{ "Item 3", new Data("Item 3", 3) },
			{ "Item 4", new Data("Item 4", 6) },
			{ "Item 5", new Data("Item 5", 6) },
			{ "Item 6", new Data("Item 6", 6) },
			{ "Item 7", new Data("Item 7", 6) },
			{ "Item 8", new Data("Item 8", 6) },
			{ "Item 9", new Data("Item 9", 6) },
		};

		/// <summary>
		/// Table.
		/// </summary>
		[SerializeField]
		protected Table Table;

		/// <summary>
		/// Progressbar.
		/// </summary>
		[SerializeField]
		protected ProgressbarDeterminate Progress;

		[SerializeField]
		float updateTime = 5f;

		/// <summary>
		/// Process start event.
		/// </summary>
		protected void Start()
		{
			UpdateTable();
			StartCoroutine(Wait());
		}

		/// <summary>
		/// Increase value.
		/// </summary>
		/// <param name="key">Key.</param>
		public void IncreaseValue(string key)
		{
			data[key].Value += 1;
		}

		/// <summary>
		/// Decrease value.
		/// </summary>
		/// <param name="key">Key.</param>
		public void DecreaseValue(string key)
		{
			data[key].Value -= 1;
		}

		/// <summary>
		/// Update table.
		/// </summary>
		public void UpdateTable()
		{
			Table.DataSource.BeginUpdate();
			Table.DataSource.Clear();
			Table.DataSource.AddRange(data.Values);

			foreach (var item in Table.DataSource)
			{
				item.UpdateDifference();
			}

			Table.DataSource.EndUpdate();
		}

		IEnumerator Wait()
		{
			Progress.Speed = updateTime;
			Progress.Value = 100;
			Progress.Animate(0);
			yield return UtilitiesTime.Wait(updateTime, false);
			UpdateTable();

			StartCoroutine(Wait());
		}
	}
}