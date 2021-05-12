using System.Collections.Generic;

namespace Lean.Pool
{
	/// <summary>This class allows you to pool normal C# classes, for example:
	/// var foo = Lean.Pool.LeanClassPool<Foo>.Spawn() ?? new Foo();
	/// Lean.Pool.LeanClassPool<Foo>.Despawn(foo);</summary>
	public static class LeanClassPool<T>
		where T : class
	{
		// Store cache of all despanwed classes here, in a list so we can search it
		private static List<T> cache = new List<T>();

		// This will either return a pooled class instance, or null
		public static T Spawn()
		{
			var count = cache.Count;

			if (count > 0)
			{
				var index    = count - 1;
				var instance = cache[index];

				cache.RemoveAt(index);

				return instance;
			}

			return null;
		}

		/// <summary>This will either return a pooled class instance, or null. If an instance it found, onSpawn will be called with it. NOTE: onSpawn is expected to not be null.</summary>
		public static T Spawn(System.Action<T> onSpawn)
		{
			var instance = default(T);

			TrySpawn(onSpawn, ref instance);

			return instance;
		}

		public static bool TrySpawn(System.Action<T> onSpawn, ref T instance)
		{
			var count = cache.Count;

			if (count > 0)
			{
				var index = count - 1;

				instance = cache[index];

				cache.RemoveAt(index);

				onSpawn(instance);

				return true;
			}

			return false;
		}

		/// <summary>This will either return a pooled class instance, or null.
		/// All pooled classes will be checked with match to see if they qualify.
		/// NOTE: match is expected to not be null.</summary>
		public static T Spawn(System.Predicate<T> match)
		{
			var instance = default(T);

			TrySpawn(match, ref instance);

			return instance;
		}

		public static bool TrySpawn(System.Predicate<T> match, ref T instance)
		{
			var index = cache.FindIndex(match);

			if (index >= 0)
			{
				instance = cache[index];

				cache.RemoveAt(index);

				return true;
			}

			return false;
		}

		/// <summary>This will either return a pooled class instance, or null.
		/// All pooled classes will be checked with match to see if they qualify.
		/// If an instance it found, onSpawn will be called with it.
		/// NOTE: match is expected to not be null.
		/// NOTE: onSpawn is expected to not be null.</summary>
		public static T Spawn(System.Predicate<T> match, System.Action<T> onSpawn)
		{
			var instance = default(T);

			TrySpawn(match, onSpawn, ref instance);

			return instance;
		}

		public static bool TrySpawn(System.Predicate<T> match, System.Action<T> onSpawn, ref T instance)
		{
			var index = cache.FindIndex(match);

			if (index >= 0)
			{
				instance = cache[index];

				cache.RemoveAt(index);

				onSpawn(instance);

				return true;
			}

			return false;
		}

		/// <summary>This will pool the passed class instance.</summary>
		public static void Despawn(T instance)
		{
			if (instance != null)
			{
				cache.Add(instance);
			}
		}

		/// <summary>This will pool the passed class instance.
		/// If you need to perform despawning code then you can do that via onDespawn.</summary>
		public static void Despawn(T instance, System.Action<T> onDespawn)
		{
			if (instance != null)
			{
				onDespawn(instance);

				cache.Add(instance);
			}
		}
	}
}