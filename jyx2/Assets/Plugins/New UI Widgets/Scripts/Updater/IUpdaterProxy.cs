namespace UIWidgets
{
	using System;

	/// <summary>
	/// Interface for the Updater proxy.
	/// Replace Unity Update() with custom one without reflection.
	/// </summary>
	public interface IUpdaterProxy
	{
		/// <summary>
		/// Add target.
		/// </summary>
		/// <param name="target">Target.</param>
		void Add(IUpdatable target);

		/// <summary>
		/// Remove target.
		/// </summary>
		/// <param name="target">Target.</param>
		void Remove(IUpdatable target);

		/// <summary>
		/// Add target to LateUpdate.
		/// </summary>
		/// <param name="target">Target.</param>
		void AddLateUpdate(ILateUpdatable target);

		/// <summary>
		/// Remove target from LateUpdate.
		/// </summary>
		/// <param name="target">Target.</param>
		void RemoveLateUpdate(ILateUpdatable target);

		/// <summary>
		/// Add target to LateUpdate.
		/// </summary>
		/// <param name="target">Target.</param>
		[Obsolete("Renamed to AddLateUpdate().")]
		void LateUpdateAdd(ILateUpdatable target);

		/// <summary>
		/// Remove target from LateUpdate.
		/// </summary>
		/// <param name="target">Target.</param>
		[Obsolete("Renamed to RemoveLateUpdate().")]
		void LateUpdateRemove(ILateUpdatable target);

		/// <summary>
		/// Add target to FixedUpdate.
		/// </summary>
		/// <param name="target">Target.</param>
		void AddFixedUpdate(IFixedUpdatable target);

		/// <summary>
		/// Remove target from FixedUpdate.
		/// </summary>
		/// <param name="target">Target.</param>
		void RemoveFixedUpdate(IFixedUpdatable target);

		/// <summary>
		/// Add target to run update only once.
		/// </summary>
		/// <param name="target">Target.</param>
		void RunOnce(IUpdatable target);

		/// <summary>
		/// Add action to run only once.
		/// </summary>
		/// <param name="action">Action.</param>
		void RunOnce(Action action);

		/// <summary>
		/// Remove target from run update only once.
		/// </summary>
		/// <param name="target">Target.</param>
		void RemoveRunOnce(IUpdatable target);

		/// <summary>
		/// Remove action from run only once.
		/// </summary>
		/// <param name="action">Action.</param>
		void RemoveRunOnce(Action action);

		/// <summary>
		/// Add target to run update only once at next frame.
		/// </summary>
		/// <param name="target">Target.</param>
		void RunOnceNextFrame(IUpdatable target);

		/// <summary>
		/// Add action to run only once at next frame.
		/// </summary>
		/// <param name="action">Action.</param>
		void RunOnceNextFrame(Action action);

		/// <summary>
		/// Remove target from run update only once at next frame.
		/// </summary>
		/// <param name="target">Target.</param>
		void RemoveRunOnceNextFrame(IUpdatable target);

		/// <summary>
		/// Remove action from run only once at next frame.
		/// </summary>
		/// <param name="action">Action.</param>
		void RemoveRunOnceNextFrame(Action action);
	}
}