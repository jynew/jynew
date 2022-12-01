namespace UIWidgets
{
	using System;

	/// <summary>
	/// Updater.
	/// Replace Unity Update() with custom one without reflection.
	/// </summary>
	public static class Updater
	{
		static IUpdaterProxy proxy;

		/// <summary>
		/// Proxy to run Update().
		/// </summary>
		public static IUpdaterProxy Proxy
		{
			get
			{
				if (proxy == null)
				{
					proxy = UpdaterProxy.Instance;
				}

				return proxy;
			}

			set
			{
				proxy = value;
			}
		}

		#if UNITY_EDITOR && UNITY_2019_3_OR_NEWER
		[UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
		static void StaticInit()
		{
			proxy = null;
		}
		#endif

		/// <summary>
		/// Add target.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void Add(IUpdatable target)
		{
			Proxy.Add(target);
		}

		/// <summary>
		/// Remove target.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void Remove(IUpdatable target)
		{
			var p = proxy;
			if (p != null)
			{
				p.Remove(target);
			}
		}

		/// <summary>
		/// Add target to LateUpdate.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void AddLateUpdate(ILateUpdatable target)
		{
			Proxy.AddLateUpdate(target);
		}

		/// <summary>
		/// Remove target from LateUpdate.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void RemoveLateUpdate(ILateUpdatable target)
		{
			var p = proxy;
			if (p != null)
			{
				p.RemoveLateUpdate(target);
			}
		}

		/// <summary>
		/// Add target to FixedUpdate.
		/// </summary>
		/// <param name="target">target.</param>
		public static void AddFixedUpdate(IFixedUpdatable target)
		{
			Proxy.AddFixedUpdate(target);
		}

		/// <summary>
		/// Remove target from FixedUpdate.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void RemoveFixedUpdate(IFixedUpdatable target)
		{
			var p = proxy;
			if (p != null)
			{
				p.RemoveFixedUpdate(target);
			}
		}

		/// <summary>
		/// Add target to run update only once.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void RunOnce(IUpdatable target)
		{
			Proxy.RunOnce(target);
		}

		/// <summary>
		/// Add action to run only once.
		/// </summary>
		/// <param name="action">Action.</param>
		public static void RunOnce(Action action)
		{
			Proxy.RunOnce(action);
		}

		/// <summary>
		/// Remove target from run update only once.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void RemoveRunOnce(IUpdatable target)
		{
			var p = proxy;
			if (p != null)
			{
				p.RemoveRunOnce(target);
			}
		}

		/// <summary>
		/// Remove action from run only once.
		/// </summary>
		/// <param name="action">Action.</param>
		public static void RemoveRunOnce(Action action)
		{
			var p = proxy;
			if (p != null)
			{
				p.RemoveRunOnce(action);
			}
		}

		/// <summary>
		/// Add target to run update only once at next frame.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void RunOnceNextFrame(IUpdatable target)
		{
			Proxy.RunOnceNextFrame(target);
		}

		/// <summary>
		/// Add action to run only once at next frame.
		/// </summary>
		/// <param name="action">Action.</param>
		public static void RunOnceNextFrame(Action action)
		{
			Proxy.RunOnceNextFrame(action);
		}

		/// <summary>
		/// Remove target from run update only once at next frame.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void RemoveRunOnceNextFrame(IUpdatable target)
		{
			var p = proxy;
			if (p != null)
			{
				p.RemoveRunOnceNextFrame(target);
			}
		}

		/// <summary>
		/// Remove action from run only once at next frame.
		/// </summary>
		/// <param name="action">Action.</param>
		public static void RemoveRunOnceNextFrame(Action action)
		{
			var p = proxy;
			if (p != null)
			{
				p.RemoveRunOnceNextFrame(action);
			}
		}
	}
}