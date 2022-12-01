namespace UIWidgets
{
	using System;
	using EasyLayoutNS;
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// ScrollRectBlock.
	/// </summary>
	public abstract class ScrollRectBlock : MonoBehaviourConditional
	{
		/// <summary>
		/// ScrollRect.
		/// </summary>
		[SerializeField]
		protected ScrollRect ScrollRect;

		/// <summary>
		/// Header.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("Header")]
		protected RectTransform Block;

		/// <summary>
		/// Is ScrollRect has horizontal direction.
		/// </summary>
		[SerializeField]
		public bool IsHorizontal = false;

		/// <summary>
		/// Display type.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("HeaderType")]
		protected ScrollRectHeaderType DisplayType = ScrollRectHeaderType.Reveal;

		/// <summary>
		/// Min size of the header.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("HeaderMinSize")]
		[EditorConditionEnum("DisplayType", new int[] { (int)ScrollRectHeaderType.Resize })]
		public float MinSize = 30f;

		/// <summary>
		/// Visible.
		/// </summary>
		[SerializeField]
		protected bool visible = true;

		/// <summary>
		/// Visible.
		/// </summary>
		public virtual bool Visible
		{
			get
			{
				return visible;
			}

			set
			{
				if (!isInited)
				{
					visible = value;
					return;
				}

				if (visible == value)
				{
					return;
				}

				ToggleVisibility(value);
			}
		}

		/// <summary>
		/// Last position of the ScrollRect.content.
		/// </summary>
		[NonSerialized]
		protected Vector2 LastPosition;

		/// <summary>
		/// Header size.
		/// </summary>
		[NonSerialized]
		protected Vector2 MaxSize;

		/// <summary>
		/// Layout.
		/// </summary>
		[NonSerialized]
		protected EasyLayout Layout;

		/// <summary>
		/// Margin value when header or footer hidden.
		/// </summary>
		[SerializeField]
		[Tooltip("Margin value when header or footer hidden. Set -1 to auto detect.")]
		protected float BaseMargin = -1f;

		/// <summary>
		/// ScrollRect transform.
		/// </summary>
		[NonSerialized]
		protected RectTransform ScrollRectTransform;

		/// <summary>
		/// Block resize listener.
		/// </summary>
		[NonSerialized]
		protected ResizeListener BlockResizeListener;

		[NonSerialized]
		bool isInited;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public virtual void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			if ((ScrollRect != null) && (Block != null))
			{
				ScrollRect.onValueChanged.AddListener(Scroll);
				ScrollRectTransform = ScrollRect.transform as RectTransform;

				LastPosition = GetPosition();

				LayoutRebuilder.ForceRebuildLayoutImmediate(Block);
				MaxSize = Block.rect.size;

				BlockResizeListener = Utilities.GetOrAddComponent<ResizeListener>(Block);
				BlockResizeListener.OnResizeNextFrame.AddListener(BlockResize);

				Layout = ScrollRect.content.GetComponent<EasyLayout>();

				InitReveal();

				InitLayout();

				ToggleVisibility(Visible);

				Updater.RunOnceNextFrame(Scroll);
			}
		}

		/// <summary>
		/// Block resize.
		/// </summary>
		protected virtual void BlockResize()
		{
			switch (DisplayType)
			{
				case ScrollRectHeaderType.Resize:
					break;
				case ScrollRectHeaderType.Reveal:
					MaxSize = Block.rect.size;
					Scroll();
					break;
				default:
					throw new NotSupportedException(string.Format("Unknown ScrollRectHeaderType: {0}", EnumHelper<ScrollRectHeaderType>.ToString(DisplayType)));
			}
		}

		/// <summary>
		/// Set header/footer size.
		/// </summary>
		/// <param name="size">Size.</param>
		public virtual void SetSize(float size)
		{
			if (IsHorizontal)
			{
				MaxSize.x = size;
				Block.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
			}
			else
			{
				MaxSize.y = size;
				Block.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
			}

			Scroll();
		}

		/// <summary>
		/// Destroy this instance.
		/// </summary>
		protected virtual void OnDestroy()
		{
			Updater.RemoveRunOnceNextFrame(Scroll);

			if (ScrollRect != null)
			{
				ScrollRect.onValueChanged.RemoveListener(Scroll);
			}

			if (BlockResizeListener != null)
			{
				BlockResizeListener.OnResizeNextFrame.RemoveListener(BlockResize);
			}
		}

		/// <summary>
		/// Toggle visibility.
		/// </summary>
		/// <param name="state">State.</param>
		protected void ToggleVisibility(bool state)
		{
			visible = state;
			if (visible)
			{
				Scroll(GetPosition());
			}
			else
			{
				Scroll(0f);
			}
		}

		/// <summary>
		/// Init for reveal display type.
		/// </summary>
		protected abstract void InitReveal();

		/// <summary>
		/// Init layout.
		/// </summary>
		protected abstract void InitLayout();

		/// <summary>
		/// Update layout.
		/// </summary>
		/// <param name="size">Size.</param>
		protected abstract void UpdateLayout(float size);

		/// <summary>
		/// Get position.
		/// </summary>
		/// <returns>position.</returns>
		protected virtual Vector2 GetPosition()
		{
			return ScrollRect.content.anchoredPosition;
		}

		/// <summary>
		/// Process ScrollRect.onValueChanged event.
		/// </summary>
		/// <param name="ignore">ScrollRect value.</param>
		protected void Scroll(Vector2 ignore)
		{
			Scroll();
		}

		/// <summary>
		/// Process scroll event.
		/// </summary>
		protected void Scroll()
		{
			if (!visible)
			{
				return;
			}

			var position = GetPosition();
			var rate = VisibilityRate(position);
			Scroll(rate);

			LastPosition = position;
		}

		/// <summary>
		/// Process scroll event.
		/// </summary>
		/// <param name="rate">Visibility rate.</param>
		protected void Scroll(float rate)
		{
			switch (DisplayType)
			{
				case ScrollRectHeaderType.Resize:
					Resize(rate);
					break;
				case ScrollRectHeaderType.Reveal:
					Reveal(rate);
					break;
				default:
					throw new NotSupportedException(string.Format("Unknown ScrollRectHeaderType: {0}", EnumHelper<ScrollRectHeaderType>.ToString(DisplayType)));
			}
		}

		/// <summary>
		/// Get visible rate.
		/// </summary>
		/// <param name="scrollPosition">Scroll position.</param>
		/// <returns>Visible rate.</returns>
		protected float VisibilityRate(Vector2 scrollPosition)
		{
			var scroll = IsHorizontal ? -scrollPosition.x : scrollPosition.y;
			var size = IsHorizontal ? MaxSize.x : MaxSize.y;
			var visible_rate = (size == 0f) ? 0f : 1f - Mathf.Clamp01(scroll / size);

			return visible_rate;
		}

		/// <summary>
		/// Get reveal position.
		/// </summary>
		/// <param name="rate">Visibility rate.</param>
		/// <returns>Position.</returns>
		protected abstract float RevealPosition(float rate);

		/// <summary>
		/// Reveal header.
		/// </summary>
		/// <param name="rate">Visibility rate.</param>
		protected void Reveal(float rate)
		{
			var current_pos = Block.anchoredPosition;
			if (IsHorizontal)
			{
				current_pos.x = RevealPosition(rate);
			}
			else
			{
				current_pos.y = RevealPosition(rate);
			}

			Block.anchoredPosition = current_pos;

			var new_size = (IsHorizontal ? MaxSize.x : MaxSize.y) * rate;
			UpdateLayout(new_size);
		}

		/// <summary>
		/// Resize header.
		/// </summary>
		/// <param name="rate">Visibility rate.</param>
		protected void Resize(float rate)
		{
			var new_size = Mathf.Lerp(MinSize, IsHorizontal ? MaxSize.x : MaxSize.y, rate);

			var axis = IsHorizontal ? RectTransform.Axis.Horizontal : RectTransform.Axis.Vertical;
			Block.SetSizeWithCurrentAnchors(axis, new_size);

			UpdateLayout(new_size);
		}
	}
}