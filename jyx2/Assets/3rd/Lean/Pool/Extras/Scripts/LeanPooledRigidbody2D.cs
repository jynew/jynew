using UnityEngine;

namespace Lean.Pool
{
	/// <summary>This component will automatically reset a Rigidbody2D when it gets spawned/despawned.</summary>
	[RequireComponent(typeof(Rigidbody2D))]
	[HelpURL(LeanPool.HelpUrlPrefix + "LeanPooledRigidbody2D")]
	[AddComponentMenu(LeanPool.ComponentPathPrefix + "Pooled Rigidbody2D")]
	public class LeanPooledRigidbody2D : MonoBehaviour, IPoolable
	{
		public void OnSpawn()
		{
		}

		public void OnDespawn()
		{
			var rigidbody2D = GetComponent<Rigidbody2D>();

			rigidbody2D.velocity        = Vector2.zero;
			rigidbody2D.angularVelocity = 0.0f;
		}
	}
}