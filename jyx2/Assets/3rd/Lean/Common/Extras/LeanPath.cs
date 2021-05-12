using UnityEngine;
using System.Collections.Generic;

namespace Lean.Common
{
	/// <summary>This component stores a list of points that form a path.</summary>
	[ExecuteInEditMode]
	[HelpURL(LeanHelper.HelpUrlPrefix + "LeanPath")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Path")]
	public class LeanPath : MonoBehaviour
	{
		/// <summary>The points along the path.</summary>
		[Tooltip("The points along the path.")]
		public List<Vector3> Points;

		/// <summary>Do these points loop back to the start?</summary>
		[Tooltip("Do these points loop back to the start?")]
		public bool Loop;

		/// <summary>The coordinate system for the points.</summary>
		[Tooltip("The coordinate system for the points.")]
		public Space Space = Space.Self;

		/// <summary>The amount of lines between each path point when read from LeanScreenDepth.</summary>
		[Tooltip("The amount of lines between each path point when read from LeanScreenDepth.")]
		public int Smoothing = 1;

		/// <summary>This allows you to draw a visual of the path using a <b>LineRenderer</b>.</summary>
		[Tooltip("This allows you to draw a visual of the path using a LineRenderer.")]
		public LineRenderer Visual;

		public static Vector3 LastWorldNormal = Vector3.forward;

		public int PointCount
		{
			get
			{
				if (Points != null)
				{
					var count = Points.Count;

					if (count >= 2)
					{
						if (Loop == true)
						{
							return count + 1;
						}
						else
						{
							return count;
						}
					}
				}

				return 0;
			}
		}

		public int GetPointCount(int smoothing = -1)
		{
			if (Points != null)
			{
				if (smoothing < 0)
				{
					smoothing = Smoothing;
				}

				var count = Points.Count;

				if (count >= 2 && smoothing >= 1)
				{
					if (Loop == true)
					{
						return count * smoothing + 1;
					}
					else
					{
						return (count - 1) * smoothing + 1;
					}
				}
			}

			return 0;
		}

		public Vector3 GetSmoothedPoint(float index)
		{
			if (Points == null)
			{
				throw new System.IndexOutOfRangeException();
			}

			var count = Points.Count;

			if (count < 2)
			{
				throw new System.Exception();
			}

			// Get int and fractional part of float index
			var i = (int)index;
			var t = Mathf.Abs(index - i);

			// Get 4 control points
			var a = GetPointRaw(i - 1, count);
			var b = GetPointRaw(i    , count);
			var c = GetPointRaw(i + 1, count);
			var d = GetPointRaw(i + 2, count);

			// Interpolate and return
			var p = default(Vector3);

			p.x = CubicInterpolate(a.x, b.x, c.x, d.x, t);
			p.y = CubicInterpolate(a.y, b.y, c.y, d.y, t);
			p.z = CubicInterpolate(a.z, b.z, c.z, d.z, t);

			return p;
		}

		public Vector3 GetPoint(int index, int smoothing = -1)
		{
			if (Points == null)
			{
				throw new System.IndexOutOfRangeException();
			}

			if (smoothing < 0)
			{
				smoothing = Smoothing;
			}

			if (smoothing < 1)
			{
				throw new System.ArgumentOutOfRangeException();
			}

			var count = Points.Count;

			if (count < 2)
			{
				throw new System.Exception();
			}

			if (smoothing > 0)
			{
				return GetSmoothedPoint(index / (float)smoothing);
			}

			return GetPointRaw(index, count);
		}

		private Vector3 GetPointRaw(int index, int count)
		{
			if (Loop == true)
			{
				index = Mod(index, count);
			}
			else
			{
				index = Mathf.Clamp(index, 0, count - 1);
			}

			var point = Points[index];

			if (Space == Space.Self)
			{
				point = transform.TransformPoint(point);
			}

			return point;
		}

		public void SetLine(Vector3 a, Vector3 b)
		{
			if (Points == null)
			{
				Points = new List<Vector3>();
			}
			else
			{
				Points.Clear();
			}

			Points.Add(a);
			Points.Add(b);
		}

		public bool TryGetClosest(Vector3 position, ref Vector3 closestPoint, ref int closestIndexA, ref int closestIndexB, int smoothing = -1)
		{
			var count = GetPointCount(smoothing);

			if (count >= 2)
			{
				var indexA          = 0;
				var pointA          = GetPoint(indexA, smoothing);
				var closestDistance = float.PositiveInfinity;

				for (var i = 1; i < count; i++)
				{
					var indexB   = i;
					var pointB   = GetPoint(indexB, smoothing);
					var point    = GetClosestPoint(position, pointA, pointB - pointA);
					var distance = Vector3.Distance(position, point);

					if (distance < closestDistance)
					{
						closestIndexA   = indexA;
						closestIndexB   = i;
						closestPoint    = point;
						closestDistance = distance;

						LastWorldNormal = Vector3.Normalize(point - pointB);
					}

					pointA = pointB;
					indexA = indexB;
				}

				return true;
			}

			return false;
		}

		public bool TryGetClosest(Vector3 position, ref Vector3 closestPoint, int smoothing = -1)
		{
			var closestIndexA = default(int);
			var closestIndexB = default(int);

			return TryGetClosest(position, ref closestPoint, ref closestIndexA, ref closestIndexB, smoothing);
		}

		public bool TryGetClosest(Ray ray, ref Vector3 closestPoint, ref int closestIndexA, ref int closestIndexB, int smoothing = -1)
		{
			var count = GetPointCount(smoothing);

			if (count >= 2)
			{
				var indexA          = 0;
				var pointA          = GetPoint(0, smoothing);
				var closestDistance = float.PositiveInfinity;

				for (var i = 1; i < count; i++)
				{
					var pointB   = GetPoint(i, smoothing);
					var point    = GetClosestPoint(ray, pointA, pointB - pointA);
					var distance = GetClosestDistance(ray, point);

					if (distance < closestDistance)
					{
						closestIndexA   = indexA;
						closestIndexB   = i;
						closestPoint    = point;
						closestDistance = distance;

						LastWorldNormal = Vector3.Normalize(point - pointB);
					}

					pointA = pointB;
					indexA = i;
				}

				return true;
			}

			return false;
		}

		public bool TryGetClosest(Ray ray, ref Vector3 currentPoint, int smoothing = -1)
		{
			var closestIndexA = default(int);
			var closestIndexB = default(int);

			return TryGetClosest(ray, ref currentPoint, ref closestIndexA, ref closestIndexB, smoothing);
		}

		public bool TryGetClosest(Ray ray, ref Vector3 currentPoint, int smoothing = -1, float maximumDelta = -1.0f)
		{
			if (maximumDelta > 0.0f)
			{
				var closestPoint = currentPoint;

				if (TryGetClosest(ray, ref closestPoint, smoothing) == true)
				{
					// Move toward closest point
					var targetPoint = Vector3.MoveTowards(currentPoint, closestPoint, maximumDelta);

					return TryGetClosest(targetPoint, ref currentPoint, smoothing);
				}

				return false;
			}

			return TryGetClosest(ray, ref currentPoint, smoothing);
		}

		private Vector3 GetClosestPoint(Vector3 position, Vector3 origin, Vector3 direction)
		{
			var denom = Vector3.Dot(direction, direction);

			// If the line doesn't point anywhere, return origin
			if (denom == 0.0f)
			{
				return origin;
			}

			var dist01 = Vector3.Dot(position - origin, direction) / denom;

			return origin + direction * Mathf.Clamp01(dist01);
		}

		private Vector3 GetClosestPoint(Ray ray, Vector3 origin, Vector3 direction)
		{
			var crossA = Vector3.Cross(ray.direction, direction);
			var denom  = Vector3.Dot(crossA, crossA);

			// If lines are parallel, we can return any point on line
			if (denom == 0.0f)
			{
				return origin;
			}

			var crossB = Vector3.Cross(ray.direction, ray.origin - origin);
			var dist01 = Vector3.Dot(crossA, crossB) / denom;

			return origin + direction * Mathf.Clamp01(dist01);
		}

		private float GetClosestDistance(Ray ray, Vector3 point)
		{
			var denom = Vector3.Dot(ray.direction, ray.direction);

			// If the ray doesn't point anywhere, return distance from origin to point
			if (denom == 0.0f)
			{
				return Vector3.Distance(ray.origin, point);
			}

			var dist01 = Vector3.Dot(point - ray.origin, ray.direction) / denom;

			return Vector3.Distance(point, ray.GetPoint(dist01));
		}

		private int Mod(int a, int b)
		{
			a %= b; return a < 0 ? a + b : a;
		}

		private float CubicInterpolate(float a, float b, float c, float d, float t)
		{
			var tt  = t * t;
			var ttt = tt * t;

			var e = a - b;
			var f = d - c;
			var g = f - e;
			var h = e - g;
			var i = c - a;

			return g * ttt + h * tt + i * t + b;
		}

		public void UpdateVisual()
		{
			if (Visual != null)
			{
				var count = GetPointCount();

				Visual.positionCount = count;

				for (var i = 0; i < count; i++)
				{
					Visual.SetPosition(i, GetPoint(i));
				}
			}
		}

		protected virtual void Update()
		{
			UpdateVisual();
		}

#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			var count = GetPointCount();

			if (count >= 2)
			{
				var pointA = GetPoint(0);

				for (var i = 1; i < count; i++)
				{
					var pointB = GetPoint(i);

					Gizmos.DrawLine(pointA, pointB);

					pointA = pointB;
				}
			}
		}
#endif
	}
}