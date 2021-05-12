namespace Lean.Pool
{
	/// <summary>If you implement this interface in a component on your pooled prefab, then the OnSpawn and OnDespawn methods will be automatically called when the associated LeanGameObjectPool.Notification = PoolableInterface.</summary>
	public interface IPoolable
	{
		/// <summary>Called when this poolable object is spawned.</summary>
		void OnSpawn();

		/// <summary>Called when this poolable object is despawned.</summary>
		void OnDespawn();
	}
}