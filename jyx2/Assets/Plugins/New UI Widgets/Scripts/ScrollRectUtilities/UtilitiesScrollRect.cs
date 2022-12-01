namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ScrollRect utilities.
	/// </summary>
	public static class UtilitiesScrollRect
	{
		/// <summary>
		/// Time to stop ScrollRect inertia.
		/// </summary>
		/// <param name="scrollRect">ScrollRect.</param>
		/// <param name="stopVelocity">Minimum velocity to stop.</param>
		/// <param name="timeStep">Time step. Can be Time.deltaTime or 1f / fps.</param>
		/// <param name="horizontal">Time to stop at horizontal or vertical axis.</param>
		/// <returns>Time to stop.</returns>
		public static float TimeToStop(ScrollRect scrollRect, float stopVelocity, float timeStep = 0.01f, bool horizontal = false)
		{
			var content = scrollRect.content;
			var viewport = scrollRect.viewport;
			if (viewport == null)
			{
				viewport = scrollRect.transform as RectTransform;
			}

			var content_size = horizontal ? content.rect.size.x : content.rect.size.y;
			var view_size = horizontal ? viewport.rect.size.x : viewport.rect.size.y;
			var position = horizontal ? content.anchoredPosition.x : content.anchoredPosition.y;
			var velocity = horizontal ? scrollRect.velocity.x : scrollRect.velocity.y;

			switch (scrollRect.movementType)
			{
				case ScrollRect.MovementType.Unrestricted:
					if (!scrollRect.inertia)
					{
						return 0;
					}

					if (scrollRect.decelerationRate >= 1f)
					{
						return float.PositiveInfinity;
					}

					return -Mathf.Log(Mathf.Abs(velocity) - stopVelocity, scrollRect.decelerationRate);
				case ScrollRect.MovementType.Clamped:
					if (!scrollRect.inertia)
					{
						return 0;
					}

					if (scrollRect.decelerationRate >= 1f)
					{
						return float.PositiveInfinity;
					}

					var distance = (velocity < 0f) ? position : content_size - position - view_size;
					velocity = Mathf.Abs(velocity);
					var time = 0f;
					while (distance > 0f)
					{
						velocity *= Mathf.Pow(scrollRect.decelerationRate, timeStep);
						if (velocity < stopVelocity)
						{
							break;
						}

						distance -= velocity * timeStep;
						time += timeStep;
					}

					return time;
				case ScrollRect.MovementType.Elastic:
					if (scrollRect.decelerationRate >= 1f)
					{
						return float.PositiveInfinity;
					}

					var view_bounds = default(Bounds);
					view_bounds.min = new Vector3(-view_size / 2f, -view_size / 2f);
					view_bounds.max = new Vector3(view_size / 2f, view_size / 2f);

					var content_bounds = default(Bounds);
					var zero = Vector2.zero;
					var elastic_time = 0f;
					while (true)
					{
						content_bounds.min = new Vector3(content_size - (position + view_bounds.max.x), content_size - (position + view_bounds.max.y));
						content_bounds.max = new Vector3(position + view_bounds.max.x, position + view_bounds.max.y);
						var full_offset = InternalCalculateOffset(ref view_bounds, ref content_bounds, scrollRect.horizontal, scrollRect.vertical, ref zero);
						var offset = horizontal ? full_offset.x : full_offset.y;
						if (offset != 0f)
						{
							position = Mathf.SmoothDamp(position, position + offset, ref velocity, scrollRect.elasticity, float.PositiveInfinity, timeStep);

							velocity *= Mathf.Pow(scrollRect.decelerationRate, timeStep);
							if (Mathf.Abs(velocity) < stopVelocity)
							{
								break;
							}

							elastic_time += timeStep;
						}
						else if (scrollRect.inertia)
						{
							velocity *= Mathf.Pow(scrollRect.decelerationRate, timeStep);
							if (Mathf.Abs(velocity) < stopVelocity)
							{
								break;
							}

							position += velocity * timeStep;
							elastic_time += timeStep;
						}
						else
						{
							return elastic_time;
						}
					}

					return elastic_time;
			}

			return 0f;
		}

		/// <summary>
		/// Calculate offset.
		/// </summary>
		/// <param name="viewBounds">View bounds.</param>
		/// <param name="contentBounds">Content bounds.</param>
		/// <param name="horizontal">Horizontal scroll enabled.</param>
		/// <param name="vertical">Vertical scroll enabled.</param>
		/// <param name="delta">Delta.</param>
		/// <returns>Offset.</returns>
		public static Vector2 InternalCalculateOffset(ref Bounds viewBounds, ref Bounds contentBounds, bool horizontal, bool vertical, ref Vector2 delta)
		{
			var offset = Vector2.zero;

			var min = contentBounds.min;
			var max = contentBounds.max;

			if (horizontal)
			{
				min.x += delta.x;
				max.x += delta.x;

				var max_offset_x = viewBounds.max.x - max.x;
				var min_offset_x = viewBounds.min.x - min.x;
				if (min_offset_x < -0.001f)
				{
					offset.x = min_offset_x;
				}
				else if (max_offset_x > 0.001f)
				{
					offset.x = max_offset_x;
				}
			}

			if (vertical)
			{
				min.y += delta.y;
				max.y += delta.y;

				var max_offset_y = viewBounds.max.y - max.y;
				var min_offset_y = viewBounds.min.y - min.y;
				if (max_offset_y > 0.001f)
				{
					offset.y = max_offset_y;
				}
				else if (min_offset_y < -0.001f)
				{
					offset.y = min_offset_y;
				}
			}

			return offset;
		}
	}
}