namespace EasyLayoutNS
{
	using System.Collections.Generic;
	using EasyLayoutNS.Extensions;
	using UnityEngine;

	/// <summary>
	/// Base class for EasyLayout groups.
	/// </summary>
	public abstract class EasyLayoutBaseType
	{
		/// <summary>
		/// Element changed delegate.
		/// </summary>
		/// <param name="element">Element.</param>
		/// <param name="properties">Properties.</param>
		public delegate void ElementChanged(RectTransform element, DrivenTransformProperties properties);

		/// <summary>
		/// Element changed event.
		/// </summary>
		public event ElementChanged OnElementChanged;

		/// <summary>
		/// Target.
		/// </summary>
		protected RectTransform Target;

		/// <summary>
		/// Is horizontal layout?
		/// </summary>
		protected bool IsHorizontal;

		/// <summary>
		/// Available size.
		/// </summary>
		protected Vector2 InternalSize;

		/// <summary>
		/// Spacing.
		/// </summary>
		protected Vector2 Spacing;

		/// <summary>
		/// Constraint count.
		/// </summary>
		protected int ConstraintCount;

		/// <summary>
		/// Main axis size.
		/// </summary>
		protected float MainAxisSize;

		/// <summary>
		/// Sub axis size.
		/// </summary>
		protected float SubAxisSize;

		/// <summary>
		/// Children width resize setting.
		/// </summary>
		protected ChildrenSize ChildrenWidth;

		/// <summary>
		/// Children height resize setting.
		/// </summary>
		protected ChildrenSize ChildrenHeight;

		/// <summary>
		/// Inner padding.
		/// </summary>
		protected Padding PaddingInner;

		/// <summary>
		/// Place element from top to bottom.
		/// </summary>
		protected bool TopToBottom;

		/// <summary>
		/// Place element from right to left.
		/// </summary>
		protected bool RightToLeft;

		/// <summary>
		/// Left margin.
		/// </summary>
		protected float MarginLeft;

		/// <summary>
		/// Top margin.
		/// </summary>
		protected float MarginTop;

		/// <summary>
		/// Change elements rotation.
		/// </summary>
		protected bool ChangeRotation;

		/// <summary>
		/// Change elements pivot.
		/// </summary>
		protected bool ChangePivot;

		/// <summary>
		/// Elements group.
		/// </summary>
		protected LayoutElementsGroup ElementsGroup = new LayoutElementsGroup();

		/// <summary>
		/// Drived properties.
		/// </summary>
		protected DrivenTransformProperties DrivenProperties;

		/// <summary>
		/// Movement animation.
		/// </summary>
		protected bool MovementAnimation;

		/// <summary>
		/// Movement animation curve.
		/// </summary>
		protected AnimationCurve MovementCurve;

		/// <summary>
		/// Resize animation.
		/// </summary>
		protected bool ResizeAnimation;

		/// <summary>
		/// Resize animation curve.
		/// </summary>
		protected AnimationCurve ResizeCurve;

		/// <summary>
		/// Unscaled time.
		/// </summary>
		protected bool UnscaledTime;

		/// <summary>
		/// Movement targets.
		/// </summary>
		protected Dictionary<int, MovementTarget> MovementTargets = new Dictionary<int, MovementTarget>();

		/// <summary>
		/// Resize targets.
		/// </summary>
		protected Dictionary<int, ResizeTarget> ResizeTargets = new Dictionary<int, ResizeTarget>();

		/// <summary>
		/// Ended animations.
		/// </summary>
		protected List<int> AnimationEnded = new List<int>();

		/// <summary>
		/// Movement target.
		/// </summary>
		protected struct MovementTarget
		{
			/// <summary>
			/// Target.
			/// </summary>
			public readonly RectTransform Target;

			/// <summary>
			/// Start position.
			/// </summary>
			public readonly Vector2 StartPosition;

			/// <summary>
			/// Start pivot.
			/// </summary>
			public readonly Vector2 StartPivot;

			/// <summary>
			/// Start rotation.
			/// </summary>
			public readonly Quaternion StartRotation;

			/// <summary>
			/// End position.
			/// </summary>
			public readonly Vector2 EndPosition;

			/// <summary>
			/// End pivot.
			/// </summary>
			public readonly Vector2 EndPivot;

			/// <summary>
			/// End rotation.
			/// </summary>
			public readonly Quaternion EndRotation;

			/// <summary>
			/// Unscaled time.
			/// </summary>
			public readonly bool UnscaledTime;

			/// <summary>
			/// Start time.
			/// </summary>
			readonly float startTime;

			/// <summary>
			/// Initializes a new instance of the <see cref="MovementTarget"/> struct.
			/// </summary>
			/// <param name="target">Target.</param>
			/// <param name="position">Position.</param>
			/// <param name="pivot">Pivot.</param>
			/// <param name="rotation">Rotation.</param>
			/// <param name="unscaledTime">Unscaled time.</param>
			public MovementTarget(RectTransform target, Vector2 position, Vector2 pivot, Quaternion rotation, bool unscaledTime)
			{
				Target = target;

				StartPosition = Target.localPosition;
				StartPivot = Target.pivot;
				StartRotation = Target.localRotation;

				EndPosition = position;
				EndPivot = pivot;
				EndRotation = rotation;

				UnscaledTime = unscaledTime;

				startTime = EasyLayout.GetTime(UnscaledTime);
			}

			/// <summary>
			/// Animate.
			/// </summary>
			/// <param name="curve">Animation curve.</param>
			/// <returns>true if animated ended; otherwise false.</returns>
			public bool Animate(AnimationCurve curve)
			{
				var length = curve[curve.length - 1].time;
				var time = EasyLayout.GetTime(UnscaledTime);

				var delta = time - startTime;
				var value = curve.Evaluate(delta);

				Target.localPosition = Vector2.Lerp(StartPosition, EndPosition, value);
				Target.pivot = Vector2.Lerp(StartPivot, EndPivot, value);
				Target.localRotation = Quaternion.Lerp(StartRotation, EndRotation, value);

				return delta >= length;
			}
		}

		/// <summary>
		/// Resize target.
		/// </summary>
		protected struct ResizeTarget
		{
			/// <summary>
			/// Target.
			/// </summary>
			public readonly RectTransform Target;

			/// <summary>
			/// Start size.
			/// </summary>
			public readonly Vector2 StartSize;

			/// <summary>
			/// End size.
			/// </summary>
			public readonly Vector2 EndSize;

			/// <summary>
			/// Unscaled time.
			/// </summary>
			public readonly bool UnscaledTime;

			/// <summary>
			/// Start time.
			/// </summary>
			readonly float startTime;

			/// <summary>
			/// Initializes a new instance of the <see cref="ResizeTarget"/> struct.
			/// </summary>
			/// <param name="target">Target.</param>
			/// <param name="size">Size.</param>
			/// <param name="unscaledTime">Unscaled time.</param>
			public ResizeTarget(RectTransform target, Vector2 size, bool unscaledTime)
			{
				Target = target;
				StartSize = Target.rect.size;
				EndSize = size;
				UnscaledTime = unscaledTime;

				startTime = EasyLayout.GetTime(UnscaledTime);
			}

			/// <summary>
			/// Animate.
			/// </summary>
			/// <param name="curve">Animation curve.</param>
			/// <returns>true if animated ended; otherwise false.</returns>
			public bool Animate(AnimationCurve curve)
			{
				var length = curve[curve.length - 1].time;
				var delta = EasyLayout.GetTime(UnscaledTime) - startTime;

				var size = Vector2.Lerp(StartSize, EndSize, curve.Evaluate(delta));

				Target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
				Target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);

				return delta >= length;
			}
		}

		/// <summary>
		/// Group position.
		/// </summary>
		protected static readonly List<Vector2> GroupPositions = new List<Vector2>()
		{
			new Vector2(0.0f, 1.0f), // Anchors.UpperLeft
			new Vector2(0.5f, 1.0f), // Anchors.UpperCenter
			new Vector2(1.0f, 1.0f), // Anchors.UpperRight

			new Vector2(0.0f, 0.5f), // Anchors.MiddleLeft
			new Vector2(0.5f, 0.5f), // Anchors.MiddleCenter
			new Vector2(1.0f, 0.5f), // Anchors.MiddleRight

			new Vector2(0.0f, 0.0f), // Anchors.LowerLeft
			new Vector2(0.5f, 0.0f), // Anchors.LowerCenter
			new Vector2(1.0f, 0.0f), // Anchors.LowerRight
		};

		/// <summary>
		/// Load layout settings.
		/// </summary>
		/// <param name="layout">Layout.</param>
		public virtual void LoadSettings(EasyLayout layout)
		{
			Target = layout.transform as RectTransform;

			IsHorizontal = layout.IsHorizontal;

			Spacing = layout.Spacing;
			InternalSize = layout.InternalSize;
			MainAxisSize = layout.MainAxisSize;
			SubAxisSize = layout.SubAxisSize;

			PaddingInner = layout.PaddingInner;

			ConstraintCount = layout.ConstraintCount;
			ChildrenWidth = layout.ChildrenWidth;
			ChildrenHeight = layout.ChildrenHeight;

			TopToBottom = layout.TopToBottom;
			RightToLeft = layout.RightToLeft;

			MarginLeft = layout.GetMarginLeft();
			MarginTop = layout.GetMarginTop();

			ChangeRotation = layout.ResetRotation;

			MovementAnimation = layout.MovementAnimation;
			MovementCurve = layout.MovementCurve;
			ResizeAnimation = layout.ResizeAnimation;
			ResizeCurve = layout.ResizeCurve;
			UnscaledTime = layout.UnscaledTime;

			DrivenProperties = GetDrivenTransformProperties();
		}

		/// <summary>
		/// Get DrivenProperties.
		/// </summary>
		/// <returns>DrivenProperties.</returns>
		protected virtual DrivenTransformProperties GetDrivenTransformProperties()
		{
			var result = DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.AnchoredPositionZ;

			if (ChildrenWidth != ChildrenSize.DoNothing)
			{
				result |= DrivenTransformProperties.SizeDeltaX;
			}

			if (ChildrenHeight != ChildrenSize.DoNothing)
			{
				result |= DrivenTransformProperties.SizeDeltaY;
			}

			if (ChangeRotation)
			{
				result |= DrivenTransformProperties.Rotation;
			}

			return result;
		}

		/// <summary>
		/// Animate.
		/// </summary>
		public virtual void Animate()
		{
			foreach (var target in ResizeTargets)
			{
				if (target.Value.Animate(ResizeCurve))
				{
					AnimationEnded.Add(target.Key);
				}
			}

			foreach (var id in AnimationEnded)
			{
				ResizeTargets.Remove(id);
			}

			foreach (var target in MovementTargets)
			{
				if (target.Value.Animate(MovementCurve))
				{
					AnimationEnded.Add(target.Key);
				}
			}

			foreach (var id in AnimationEnded)
			{
				MovementTargets.Remove(id);
			}

			AnimationEnded.Clear();
		}

		/// <summary>
		/// Perform layout.
		/// </summary>
		/// <param name="elements">Elements.</param>
		/// <param name="setPositions">Set elements positions.</param>
		/// <param name="resizeType">Resize type.</param>
		/// <returns>Size of the group.</returns>
		public GroupSize PerformLayout(List<LayoutElementInfo> elements, bool setPositions, ResizeType resizeType)
		{
			ElementsGroup.SetElements(elements);

			SetInitialSizes();
			Group();
			ElementsGroup.Sort();

			CalculateSizes();
			var size = CalculateGroupSize();
			CalculatePositions(new Vector2(size.Width, size.Height));
			SetElementsSize(resizeType);

			if (setPositions)
			{
				SetPositions();
			}

			return size;
		}

		/// <summary>
		/// Get target position in the group.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>Position.</returns>
		public virtual EasyLayoutPosition GetElementPosition(RectTransform target)
		{
			return ElementsGroup.GetElementPosition(target);
		}

		/// <summary>
		/// Group elements.
		/// </summary>
		protected abstract void Group();

		/// <summary>
		/// Calculate sizes of the elements.
		/// </summary>
		protected abstract void CalculateSizes();

		/// <summary>
		/// Calculate positions of the elements.
		/// </summary>
		/// <param name="size">Size.</param>
		protected abstract void CalculatePositions(Vector2 size);

		/// <summary>
		/// Calculate group size.
		/// </summary>
		/// <returns>Size.</returns>
		protected abstract GroupSize CalculateGroupSize();

		readonly List<GroupSize> mainAxisSize = new List<GroupSize>();

		/// <summary>
		/// Calculate size of the group.
		/// </summary>
		/// <param name="isHorizontal">ElementsGroup are in horizontal order?</param>
		/// <param name="spacing">Spacing.</param>
		/// <param name="padding">Padding,</param>
		/// <returns>Size.</returns>
		protected virtual GroupSize CalculateGroupSize(bool isHorizontal, Vector2 spacing, Vector2 padding)
		{
			var per_block = isHorizontal ? ElementsGroup.Rows : ElementsGroup.Columns;

			var sub_size = ((per_block - 1) * spacing.y) + padding.y;
			var sub_axis_size = isHorizontal ? new GroupSize(0, sub_size) : new GroupSize(sub_size, 0);
			for (int i = 0; i < per_block; i++)
			{
				var block = isHorizontal ? ElementsGroup.GetRow(i) : ElementsGroup.GetColumn(i);
				var block_sub_size = default(GroupSize);
				for (var j = 0; j < block.Count; j++)
				{
					if (mainAxisSize.Count == j)
					{
						mainAxisSize.Add(default(GroupSize));
					}

					mainAxisSize[j] = mainAxisSize[j].Max(block[j]);

					block_sub_size.Max(block[j]);
				}

				sub_axis_size += block_sub_size;
			}

			var main_size = ((mainAxisSize.Count - 1) * spacing.x) + padding.x;
			var main_axis_size = isHorizontal ? new GroupSize(main_size, 0) : new GroupSize(0, main_size);
			main_axis_size += Sum(mainAxisSize);

			mainAxisSize.Clear();

			return isHorizontal
				? new GroupSize(main_axis_size, sub_axis_size)
				: new GroupSize(sub_axis_size, main_axis_size);
		}

		/// <summary>
		/// Set elements size.
		/// </summary>
		/// <param name="resizeType">Resize type.</param>
		protected void SetElementsSize(ResizeType resizeType)
		{
			foreach (var element in ElementsGroup.Elements)
			{
				SetElementSize(element, resizeType);
				OnElementChangedInvoke(element.Rect, DrivenProperties);
			}
		}

		/// <summary>
		/// Set element size.
		/// </summary>
		/// <param name="element">Element.</param>
		/// <param name="resizeType">Resize type.</param>
		protected virtual void SetElementSize(LayoutElementInfo element, ResizeType resizeType)
		{
			var resize_width = (ChildrenWidth != ChildrenSize.DoNothing) && resizeType.IsSet(ResizeType.Horizontal) && element.ChangedWidth;
			var resize_height = (ChildrenHeight != ChildrenSize.DoNothing) && resizeType.IsSet(ResizeType.Vertical) && element.ChangedHeight;

			var changed = resize_width || resize_height;
			if (!changed)
			{
				return;
			}

			if (ResizeAnimation && Application.isPlaying)
			{
				ResizeTarget settings;
				var id = element.Rect.GetInstanceID();
				if (!ResizeTargets.TryGetValue(id, out settings) && (settings.EndSize != element.NewSize))
				{
					ResizeTargets[id] = new ResizeTarget(element.Rect, element.NewSize, UnscaledTime);
				}
			}
			else
			{
				if (resize_width)
				{
					element.Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, element.NewWidth);
				}

				if (resize_height)
				{
					element.Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, element.NewHeight);
				}
			}
		}

		/// <summary>
		/// Invoke OnElementChanged event.
		/// </summary>
		/// <param name="element">Element.</param>
		/// <param name="properties">Properties.</param>
		protected void OnElementChangedInvoke(RectTransform element, DrivenTransformProperties properties)
		{
			var handlers = OnElementChanged;
			if (handlers != null)
			{
				handlers(element, properties);
			}
		}

		/// <summary>
		/// Set elements positions.
		/// </summary>
		protected virtual void SetPositions()
		{
			foreach (var element in ElementsGroup.Elements)
			{
				var changed = element.IsPositionChanged || (ChangePivot && element.ChangedPivot) || (ChangeRotation && element.ChangedRotation);
				if (!changed)
				{
					continue;
				}

				if (MovementAnimation && Application.isPlaying)
				{
					MovementTarget settings;
					var id = element.Rect.GetInstanceID();

					var rotation = Quaternion.Euler(element.NewEulerAngles);
					if (!MovementTargets.TryGetValue(id, out settings) || (settings.EndPosition != element.PositionPivot) || (ChangePivot && (settings.EndPivot != element.NewPivot)) || (ChangeRotation && (settings.EndRotation != rotation)))
					{
						MovementTargets[id] = new MovementTarget(element.Rect, element.PositionPivot, element.NewPivot, rotation, UnscaledTime);
					}
				}
				else
				{
					if (ChangeRotation)
					{
						element.Rect.localEulerAngles = element.NewEulerAngles;
					}

					if (ChangePivot)
					{
						element.Rect.pivot = element.NewPivot;
					}

					element.Rect.localPosition = element.PositionPivot;
				}
			}
		}

		/// <summary>
		/// Sum values of the list.
		/// </summary>
		/// <param name="list">List.</param>
		/// <returns>Sum.</returns>
		protected static GroupSize Sum(List<GroupSize> list)
		{
			var result = default(GroupSize);
			foreach (var item in list)
			{
				result += item;
			}

			return result;
		}

		/// <summary>
		/// Sum values of the list.
		/// </summary>
		/// <param name="list">List.</param>
		/// <returns>Sum.</returns>
		protected static float Sum(List<float> list)
		{
			var result = 0f;
			foreach (var item in list)
			{
				result += item;
			}

			return result;
		}

		#region Group

		/// <summary>
		/// Reverse list.
		/// </summary>
		/// <param name="list">List.</param>
		protected static void ReverseList(List<LayoutElementInfo> list)
		{
			list.Reverse();
		}

		/// <summary>
		/// Group elements by columns in the vertical order.
		/// </summary>
		/// <param name="maxColumns">Maximum columns count.</param>
		protected void GroupByColumnsVertical(int maxColumns)
		{
			int i = 0;
			for (int column = 0; column < maxColumns; column++)
			{
				int max_rows = Mathf.CeilToInt(((float)(ElementsGroup.Count - i)) / (maxColumns - column));
				for (int row = 0; row < max_rows; row++)
				{
					ElementsGroup.SetPosition(i, row, column);

					i += 1;
				}
			}
		}

		/// <summary>
		/// Group elements by columns in the horizontal order.
		/// </summary>
		/// <param name="maxColumns">Maximum columns count.</param>
		protected void GroupByColumnsHorizontal(int maxColumns)
		{
			int row = 0;

			for (int i = 0; i < ElementsGroup.Count; i += maxColumns)
			{
				int column = 0;
				var end = Mathf.Min(i + maxColumns, ElementsGroup.Count);
				for (int j = i; j < end; j++)
				{
					ElementsGroup.SetPosition(j, row, column);
					column += 1;
				}

				row += 1;
			}
		}

		/// <summary>
		/// Group elements by rows in the vertical order.
		/// </summary>
		/// <param name="maxRows">Maximum rows count.</param>
		protected void GroupByRowsVertical(int maxRows)
		{
			int column = 0;
			for (int i = 0; i < ElementsGroup.Count; i += maxRows)
			{
				int row = 0;
				var end = Mathf.Min(i + maxRows, ElementsGroup.Count);
				for (int j = i; j < end; j++)
				{
					ElementsGroup.SetPosition(j, row, column);
					row += 1;
				}

				column += 1;
			}
		}

		/// <summary>
		/// Group elements by rows in the horizontal order.
		/// </summary>
		/// <param name="maxRows">Maximum rows count.</param>
		protected void GroupByRowsHorizontal(int maxRows)
		{
			int i = 0;
			for (int row = 0; row < maxRows; row++)
			{
				int max_columns = Mathf.CeilToInt((float)(ElementsGroup.Count - i) / (maxRows - row));
				for (int column = 0; column < max_columns; column++)
				{
					ElementsGroup.SetPosition(i, row, column);
					i += 1;
				}
			}
		}

		/// <summary>
		/// Group the specified uiElements by columns.
		/// </summary>
		protected void GroupByColumns()
		{
			if (IsHorizontal)
			{
				GroupByColumnsHorizontal(ConstraintCount);
			}
			else
			{
				GroupByColumnsVertical(ConstraintCount);
			}
		}

		/// <summary>
		/// Group the specified uiElements by rows.
		/// </summary>
		protected void GroupByRows()
		{
			if (IsHorizontal)
			{
				GroupByRowsHorizontal(ConstraintCount);
			}
			else
			{
				GroupByRowsVertical(ConstraintCount);
			}
		}
		#endregion

		#region Sizes

		/// <summary>
		/// Resize elements.
		/// </summary>
		protected virtual void SetInitialSizes()
		{
			if (ChildrenWidth == ChildrenSize.DoNothing && ChildrenHeight == ChildrenSize.DoNothing)
			{
				return;
			}

			if (ElementsGroup.Count == 0)
			{
				return;
			}

			var max_size = FindMaxPreferredSize();

			foreach (var element in ElementsGroup.Elements)
			{
				SetInitialSize(element, max_size);
			}
		}

		Vector2 FindMaxPreferredSize()
		{
			var max_size = new Vector2(-1f, -1f);

			foreach (var element in ElementsGroup.Elements)
			{
				max_size.x = Mathf.Max(max_size.x, element.PreferredWidth);
				max_size.y = Mathf.Max(max_size.y, element.PreferredHeight);
			}

			if (ChildrenWidth != ChildrenSize.SetMaxFromPreferred)
			{
				max_size.x = -1f;
			}

			if (ChildrenHeight != ChildrenSize.SetMaxFromPreferred)
			{
				max_size.y = -1f;
			}

			return max_size;
		}

		void SetInitialSize(LayoutElementInfo element, Vector2 max_size)
		{
			if (ChildrenWidth != ChildrenSize.DoNothing)
			{
				element.NewWidth = (max_size.x != -1f) ? max_size.x : element.PreferredWidth;
			}

			if (ChildrenHeight != ChildrenSize.DoNothing)
			{
				element.NewHeight = (max_size.y != -1f) ? max_size.y : element.PreferredHeight;
			}
		}

		/// <summary>
		/// Resize elements width to fit.
		/// </summary>
		/// <param name="increaseOnly">Size can be only increased.</param>
		protected void ResizeWidthToFit(bool increaseOnly)
		{
			var width = InternalSize.x;
			for (int row = 0; row < ElementsGroup.Rows; row++)
			{
				ResizeToFit(width, ElementsGroup.GetRow(row), Spacing.x, RectTransform.Axis.Horizontal, increaseOnly);
			}
		}

		/// <summary>
		/// Resize specified elements to fit.
		/// </summary>
		/// <param name="size">Size.</param>
		/// <param name="elements">Elements.</param>
		/// <param name="spacing">Spacing.</param>
		/// <param name="axis">Axis to fit.</param>
		/// <param name="increaseOnly">Size can be only increased.</param>
		protected static void ResizeToFit(float size, List<LayoutElementInfo> elements, float spacing, RectTransform.Axis axis, bool increaseOnly)
		{
			var sizes = axis == RectTransform.Axis.Horizontal ? SizesInfo.GetWidths(elements) : SizesInfo.GetHeights(elements);
			var free_space = size - sizes.TotalPreferred - ((elements.Count - 1) * spacing);

			if (increaseOnly)
			{
				free_space = Mathf.Max(0f, free_space);
				size = Mathf.Max(0f, size);
				sizes.TotalMin = sizes.TotalPreferred;
			}

			var per_flexible = free_space > 0f ? free_space / sizes.TotalFlexible : 0f;

			var minPrefLerp = 1f;
			if (sizes.TotalMin != sizes.TotalPreferred)
			{
				minPrefLerp = Mathf.Clamp01((size - sizes.TotalMin - ((elements.Count - 1) * spacing)) / (sizes.TotalPreferred - sizes.TotalMin));
			}

			for (int i = 0; i < elements.Count; i++)
			{
				var element_size = Mathf.Lerp(sizes.Sizes[i].Min, sizes.Sizes[i].Preferred, minPrefLerp) + (per_flexible * sizes.Sizes[i].Flexible);
				elements[i].SetSize(axis, element_size);
			}
		}

		/// <summary>
		/// Shrink elements width to fit.
		/// </summary>
		protected void ShrinkWidthToFit()
		{
			var width = InternalSize.x;
			for (int row = 0; row < ElementsGroup.Rows; row++)
			{
				ShrinkToFit(width, ElementsGroup.GetRow(row), Spacing.x, RectTransform.Axis.Horizontal);
			}
		}

		/// <summary>
		/// Resize row height to fit.
		/// </summary>
		/// <param name="increaseOnly">Size can be only increased.</param>
		protected void ResizeRowHeightToFit(bool increaseOnly)
		{
			ResizeToFit(InternalSize.y, ElementsGroup, Spacing.y, RectTransform.Axis.Vertical, increaseOnly);
		}

		/// <summary>
		/// Shrink row height to fit.
		/// </summary>
		protected void ShrinkRowHeightToFit()
		{
			ShrinkToFit(InternalSize.y, ElementsGroup, Spacing.y, RectTransform.Axis.Vertical);
		}

		/// <summary>
		/// Shrink specified elements size to fit.
		/// </summary>
		/// <param name="size">Size.</param>
		/// <param name="elements">Elements.</param>
		/// <param name="spacing">Spacing.</param>
		/// <param name="axis">Axis to fit.</param>
		protected static void ShrinkToFit(float size, List<LayoutElementInfo> elements, float spacing, RectTransform.Axis axis)
		{
			var sizes = axis == RectTransform.Axis.Horizontal ? SizesInfo.GetWidths(elements) : SizesInfo.GetHeights(elements);

			float free_space = size - sizes.TotalPreferred - ((elements.Count - 1) * spacing);
			if (free_space > 0f)
			{
				return;
			}

			var per_flexible = free_space > 0f ? free_space / sizes.TotalFlexible : 0f;

			var minPrefLerp = 0f;
			if (sizes.TotalMin != sizes.TotalPreferred)
			{
				minPrefLerp = Mathf.Clamp01((size - sizes.TotalMin - ((elements.Count - 1) * spacing)) / (sizes.TotalPreferred - sizes.TotalMin));
			}

			for (int i = 0; i < elements.Count; i++)
			{
				var element_size = Mathf.Lerp(sizes.Sizes[i].Min, sizes.Sizes[i].Preferred, minPrefLerp) + (per_flexible * sizes.Sizes[i].Flexible);
				elements[i].SetSize(axis, element_size);
			}
		}

		/// <summary>
		/// Resize specified elements to fit.
		/// </summary>
		/// <param name="size">Size.</param>
		/// <param name="group">Group.</param>
		/// <param name="spacing">Spacing.</param>
		/// <param name="axis">Axis to fit.</param>
		/// <param name="increaseOnly">Size can be only increased.</param>
		protected static void ResizeToFit(float size, LayoutElementsGroup group, float spacing, RectTransform.Axis axis, bool increaseOnly)
		{
			var is_horizontal = axis == RectTransform.Axis.Horizontal;
			var sizes = is_horizontal ? SizesInfo.GetWidths(group) : SizesInfo.GetHeights(group);
			var n = is_horizontal ? group.Columns : group.Rows;
			var free_space = size - sizes.TotalPreferred - ((n - 1) * spacing);

			if (increaseOnly)
			{
				free_space = Mathf.Max(0f, free_space);
				size = Mathf.Max(0f, size);
				sizes.TotalMin = sizes.TotalPreferred;
			}

			var minPrefLerp = 1f;
			if (sizes.TotalMin != sizes.TotalPreferred)
			{
				minPrefLerp = Mathf.Clamp01((size - sizes.TotalMin - ((group.Rows - 1) * spacing)) / (sizes.TotalPreferred - sizes.TotalMin));
			}

			if (is_horizontal)
			{
				var per_flexible = free_space > 0f ? free_space / sizes.TotalFlexible : 0f;

				for (int column = 0; column < group.Columns; column++)
				{
					var element_size = Mathf.Lerp(sizes.Sizes[column].Min, sizes.Sizes[column].Preferred, minPrefLerp) + (per_flexible * sizes.Sizes[column].Flexible);

					foreach (var element in group.GetColumn(column))
					{
						element.SetSize(axis, element_size);
					}
				}
			}
			else
			{
				var per_flexible = free_space > 0f ? free_space / sizes.TotalFlexible : 0f;

				for (int rows = 0; rows < group.Rows; rows++)
				{
					var element_size = Mathf.Lerp(sizes.Sizes[rows].Min, sizes.Sizes[rows].Preferred, minPrefLerp) + (per_flexible * sizes.Sizes[rows].Flexible);
					foreach (var element in group.GetRow(rows))
					{
						element.SetSize(axis, element_size);
					}
				}
			}
		}

		/// <summary>
		/// Shrink specified elements to fit.
		/// </summary>
		/// <param name="size">Size.</param>
		/// <param name="group">Elements.</param>
		/// <param name="spacing">Spacing.</param>
		/// <param name="axis">Axis to fit.</param>
		protected static void ShrinkToFit(float size, LayoutElementsGroup group, float spacing, RectTransform.Axis axis)
		{
			var is_horizontal = axis == RectTransform.Axis.Horizontal;
			var sizes = is_horizontal ? SizesInfo.GetWidths(group) : SizesInfo.GetHeights(group);
			var n = is_horizontal ? group.Columns : group.Rows;

			var free_space = size - sizes.TotalPreferred - ((n - 1) * spacing);
			if (free_space > 0f)
			{
				return;
			}

			var minPrefLerp = 0f;
			if (sizes.TotalMin != sizes.TotalPreferred)
			{
				minPrefLerp = Mathf.Clamp01((size - sizes.TotalMin - ((group.Rows - 1) * spacing)) / (sizes.TotalPreferred - sizes.TotalMin));
			}

			if (is_horizontal)
			{
				var per_flexible = free_space > 0f ? free_space / sizes.TotalFlexible : 0f;

				for (int column = 0; column < group.Columns; column++)
				{
					var element_size = Mathf.Lerp(sizes.Sizes[column].Min, sizes.Sizes[column].Preferred, minPrefLerp) + (per_flexible * sizes.Sizes[column].Flexible);

					foreach (var element in group.GetColumn(column))
					{
						element.SetSize(axis, element_size);
					}
				}
			}
			else
			{
				var per_flexible = free_space > 0f ? free_space / sizes.TotalFlexible : 0f;

				for (int rows = 0; rows < group.Rows; rows++)
				{
					var element_size = Mathf.Lerp(sizes.Sizes[rows].Min, sizes.Sizes[rows].Preferred, minPrefLerp) + (per_flexible * sizes.Sizes[rows].Flexible);

					foreach (var element in group.GetRow(rows))
					{
						element.SetSize(axis, element_size);
					}
				}
			}
		}
		#endregion
	}
}