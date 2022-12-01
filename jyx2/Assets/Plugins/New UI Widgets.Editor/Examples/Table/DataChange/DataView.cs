namespace UIWidgets.Examples.DataChange
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Data view.
	/// </summary>
	public class DataView : ListViewItem, IViewData<Data>
	{
		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public TextAdapter Name;

		/// <summary>
		/// Value.
		/// </summary>
		[SerializeField]
		public TextAdapter Value;

		/// <summary>
		/// Value background.
		/// </summary>
		[SerializeField]
		public Image ValueBackground;

		/// <summary>
		/// Color of unchanged value.
		/// </summary>
		[SerializeField]
		public Color ColorUnchanged = Color.black;

		/// <summary>
		/// Color of increased value.
		/// </summary>
		[SerializeField]
		public Color ColorDecrease = Color.red;

		/// <summary>
		/// Color of decreased value.
		/// </summary>
		[SerializeField]
		public Color ColorIncrease = Color.green;

		Data Item;

		/// <summary>
		/// Init graphics foreground.
		/// </summary>
		protected override void GraphicsForegroundInit()
		{
			if (GraphicsForegroundVersion == 0)
			{
				Foreground = new Graphic[]
				{
					UtilitiesUI.GetGraphic(Name),
					UtilitiesUI.GetGraphic(Value),
				};
				GraphicsForegroundVersion = 1;
			}
		}

		/// <summary>
		/// Init graphics background.
		/// </summary>
		protected override void GraphicsBackgroundInit()
		{
			if (GraphicsBackgroundVersion == 0)
			{
				graphicsBackground = Compatibility.EmptyArray<Graphic>();
				GraphicsBackgroundVersion = 1;
			}
		}

		/// <summary>
		/// Gets the objects to resize.
		/// </summary>
		/// <value>The objects to resize.</value>
		public GameObject[] ObjectsToResize
		{
			get
			{
				return new GameObject[]
				{
					Name.transform.parent.gameObject,
					Value.transform.parent.gameObject,
				};
			}
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(Data item)
		{
			Item = item;
			UpdateView();
		}

		/// <summary>
		/// Update view.
		/// </summary>
		protected virtual void UpdateView()
		{
			Name.text = Item.Name;
			Value.text = Item.Value.ToString();
			if (Item.Difference == 0)
			{
				ValueBackground.color = ColorUnchanged;
			}
			else
			{
				ValueBackground.color = (Item.Difference > 0) ? ColorIncrease : ColorDecrease;
			}
		}
	}
}