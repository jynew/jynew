namespace UIWidgets.Examples
{
	using System.Collections;
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Resize selected items.
	/// </summary>
	public class ListViewSelectedItemResize : MonoBehaviour
	{
		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		public ListViewBase ListView;

		/// <summary>
		/// Default item size.
		/// </summary>
		[SerializeField]
		public Vector2 DefaultSize = new Vector2(212, 36);

		/// <summary>
		/// Selected item size.
		/// </summary>
		[SerializeField]
		public Vector2 SelectedSize = new Vector2(212, 72);

		/// <summary>
		/// Animation curve.
		/// </summary>
		[SerializeField]
		public AnimationCurve Curve = AnimationCurve.EaseInOut(0f, 0f, 0.5f, 1f);

		/// <summary>
		/// Animated with UnscaledTime.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime = true;

		Dictionary<int, IEnumerator> Animations = new Dictionary<int, IEnumerator>();

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected void Start()
		{
			ListView.OnSelect.AddListener(ProcessSelected);
			ListView.OnDeselect.AddListener(ProcessDeselected);
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected void OnDestroy()
		{
			ListView.OnSelect.RemoveListener(ProcessSelected);
			ListView.OnDeselect.RemoveListener(ProcessDeselected);
		}

		void ProcessSelected(int index, ListViewItem instance)
		{
			StartAnimation(instance, SelectedSize);
		}

		void ProcessDeselected(int index, ListViewItem instance)
		{
			StartAnimation(instance, DefaultSize);
		}

		void StartAnimation(ListViewItem instance, Vector2 size)
		{
			if (instance == null)
			{
				return;
			}

			StopAnimation(instance.Index);
			var animation = ResizeAnimation(instance, size);
			Animations[instance.Index] = animation;
			StartCoroutine(animation);
		}

		void StopAnimation(int index)
		{
			IEnumerator animation;
			if (Animations.TryGetValue(index, out animation))
			{
				StopCoroutine(animation);
				Animations.Remove(index);
			}
		}

		IEnumerator ResizeAnimation(ListViewItem instance, Vector2 size)
		{
			instance.DisableRecycling = true;

			var current = instance.RectTransform.rect.size;

			var length = Curve[Curve.length - 1].time;
			var start = UtilitiesTime.GetTime(UnscaledTime);
			var time = 0f;
			do
			{
				time = UtilitiesTime.GetTime(UnscaledTime) - start;

				var lerp_size = Vector2.Lerp(current, size, Curve.Evaluate(time / length));

				instance.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, lerp_size.x);
				instance.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, lerp_size.y);
				yield return null;
			}
			while (time < length);

			instance.DisableRecycling = false;

			Animations.Remove(instance.Index);
		}
	}
}