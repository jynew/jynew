namespace UIWidgets.Examples.DataChange
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Data.
	/// </summary>
	[Serializable]
	public class Data
	{
		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public string Name;

		/// <summary>
		/// Value.
		/// </summary>
		[SerializeField]
		public int Value;

		/// <summary>
		/// Previous value.
		/// </summary>
		[SerializeField]
		private int PreviousValue;

		/// <summary>
		/// Value difference.
		/// </summary>
		public int Difference
		{
			get;
			private set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Data"/> class.
		/// </summary>
		public Data()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Data"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="value">Value.</param>
		public Data(string name, int value)
		{
			Name = name;
			Value = value;
			PreviousValue = value;
			Difference = 0;
		}

		/// <summary>
		/// Update difference.
		/// </summary>
		public void UpdateDifference()
		{
			Difference = Value - PreviousValue;
			PreviousValue = Value;
		}
	}
}