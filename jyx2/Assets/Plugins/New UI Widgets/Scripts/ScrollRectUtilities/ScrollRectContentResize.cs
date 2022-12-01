namespace UIWidgets
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ScrollRect content resize.
	/// </summary>
	[RequireComponent(typeof(ScrollRect))]
	public class ScrollRectContentResize : MonoBehaviour
	{
		ResizeListener resizeListener;

		ScrollRect scrollRect;

		/// <summary>
		/// ScrollRect.
		/// </summary>
		protected ScrollRect ScrollRect
		{
			get
			{
				if (scrollRect == null)
				{
					scrollRect = GetComponent<ScrollRect>();
				}

				return scrollRect;
			}
		}

		/// <summary>
		/// Process start event.
		/// </summary>
		protected virtual void Start()
		{
			if (resizeListener == null)
			{
				resizeListener = Utilities.GetOrAddComponent<ResizeListener>(this);
			}

			resizeListener.OnResize.AddListener(Resize);

			Resize();
		}

		/// <summary>
		/// Process destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (resizeListener != null)
			{
				resizeListener.OnResize.RemoveListener(Resize);
			}
		}

		void Resize()
		{
			var size = (ScrollRect.transform as RectTransform).rect.size;

			var content = ScrollRect.content;
			for (int i = 0; i < content.childCount; i++)
			{
				var rt = content.GetChild(i) as RectTransform;
				if (ScrollRect.horizontal)
				{
					rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
				}

				if (ScrollRect.vertical)
				{
					rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
				}
			}
		}
	}
}