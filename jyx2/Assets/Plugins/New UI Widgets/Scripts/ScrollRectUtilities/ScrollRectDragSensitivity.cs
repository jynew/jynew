namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// ScrollRect drag sensitivity.
	/// </summary>
	[RequireComponent(typeof(ScrollRect))]
	public class ScrollRectDragSensitivity : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
	{
		/// <summary>
		/// Content corners.
		/// </summary>
		[NonSerialized]
		protected readonly Vector3[] Corners = new Vector3[4];

		/// <summary>
		/// Sensitivity.
		/// </summary>
		[SerializeField]
		public Vector2 Sensitivity = Vector2.one;

		/// <summary>
		/// Start cursor.
		/// </summary>
		[NonSerialized]
		protected Vector2 StartCursor;

		/// <summary>
		/// Start position.
		/// </summary>
		[NonSerialized]
		protected Vector2 StartPosition;

		/// <summary>
		/// Is dragging?
		/// </summary>
		[NonSerialized]
		protected bool Dragging;

		ScrollRect scrollRect;

		/// <summary>
		/// ScrollRect.
		/// </summary>
		public ScrollRect ScrollRect
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

		RectTransform viewRectTransform;

		/// <summary>
		/// View RectTransform.
		/// </summary>
		protected RectTransform ViewRectTransform
		{
			get
			{
				if (viewRectTransform == null)
				{
					viewRectTransform = ScrollRect.viewport;

					if (viewRectTransform == null)
					{
						viewRectTransform = ScrollRect.transform as RectTransform;
					}
				}

				return viewRectTransform;
			}
		}

		/// <summary>
		/// Process begin drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			if (!ScrollRect.IsActive())
			{
				return;
			}

			StartCursor = Vector2.zero;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(ViewRectTransform, eventData.position, eventData.pressEventCamera, out StartCursor);
			StartPosition = ScrollRect.content.anchoredPosition;

			Dragging = true;
		}

		/// <summary>
		/// Process end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			Dragging = false;
		}

		/// <summary>
		/// Process drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnDrag(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			if (!ScrollRect.IsActive())
			{
				return;
			}

			if (!Dragging)
			{
				return;
			}

			Vector2 cursor;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(ViewRectTransform, eventData.position, eventData.pressEventCamera, out cursor))
			{
				return;
			}

			var view_bounds = new Bounds(ViewRectTransform.rect.center, ViewRectTransform.rect.size);
			var delta = cursor - StartCursor;
			var offset = CalculateOffset(StartPosition + delta - ScrollRect.content.anchoredPosition, ref view_bounds);
			delta += offset;

			if (ScrollRect.movementType == ScrollRect.MovementType.Elastic)
			{
				if (offset.x != 0f)
				{
					delta.x -= RubberDelta(offset.x, view_bounds.size.x);
				}

				if (offset.y != 0f)
				{
					delta.y -= RubberDelta(offset.y, view_bounds.size.y);
				}
			}

			delta.x *= Sensitivity.x;
			delta.y *= Sensitivity.y;

			SetContentAnchoredPosition(StartPosition + delta);
		}

		/// <summary>
		/// Set content position.
		/// </summary>
		/// <param name="position">Position.</param>
		protected virtual void SetContentAnchoredPosition(Vector2 position)
		{
			var content = ScrollRect.content;

			if (!ScrollRect.horizontal)
			{
				position.x = content.anchoredPosition.x;
			}

			if (!ScrollRect.vertical)
			{
				position.y = content.anchoredPosition.y;
			}

			if (position != content.anchoredPosition)
			{
				content.anchoredPosition = position;
			}
		}

		/// <summary>
		/// Rubber delta.
		/// </summary>
		/// <param name="overStretching">Over stretching.</param>
		/// <param name="viewSize">View size.</param>
		/// <returns>Delta.</returns>
		protected virtual float RubberDelta(float overStretching, float viewSize)
		{
			return (1f - (1f / ((Mathf.Abs(overStretching) * 0.55f / viewSize) + 1f))) * viewSize * Mathf.Sign(overStretching);
		}

		/// <summary>
		/// Calculate offset.
		/// </summary>
		/// <param name="delta">Delta.</param>
		/// <param name="viewBounds">View bounds.</param>
		/// <returns>Offset.</returns>
		protected virtual Vector2 CalculateOffset(Vector2 delta, ref Bounds viewBounds)
		{
			if (ScrollRect.movementType == ScrollRect.MovementType.Unrestricted)
			{
				return Vector2.zero;
			}

			var content_bounds = GetBounds();
			return UtilitiesScrollRect.InternalCalculateOffset(ref viewBounds, ref content_bounds, ScrollRect.horizontal, ScrollRect.vertical, ref delta);
		}

		/// <summary>
		/// Get bounds.
		/// </summary>
		/// <returns>Bounds.</returns>
		protected virtual Bounds GetBounds()
		{
			if (ScrollRect.content == null)
			{
				return default(Bounds);
			}

			ScrollRect.content.GetWorldCorners(Corners);

			var matrix = ViewRectTransform.worldToLocalMatrix;

			var v_min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			var v_max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
			for (var i = 0; i < 4; i++)
			{
				var lhs = matrix.MultiplyPoint3x4(Corners[i]);
				v_min = Vector3.Min(lhs, v_min);
				v_max = Vector3.Max(lhs, v_max);
			}

			var bounds = new Bounds(v_min, Vector3.zero);
			bounds.Encapsulate(v_max);

			return bounds;
		}
	}
}