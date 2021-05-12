using UnityEngine;

namespace Lean.Common
{
	/// <summary>This component will automatically destroy a GameObject after the specified amount of time.
	/// NOTE: If <b>Seconds</b> is set to -1, then you must manually call the  <b>DestroyNow</b> method from somewhere.</summary>
	[HelpURL(LeanHelper.HelpUrlPrefix + "LeanDestroy")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Destroy")]
	public class LeanDestroy : MonoBehaviour
	{
		/// <summary>The GameObject that will be destroyed.
		/// None/null = This GameObject.</summary>
		public GameObject Target;

		/// <summary>The amount of seconds remaining before this GameObject gets destroyed.
		/// -1 = You must manually call the DestroyNow method.</summary>
		public float Seconds = -1.0f;

		protected virtual void Update()
		{
			if (Seconds >= 0.0f)
			{
				Seconds -= Time.deltaTime;

				if (Seconds <= 0.0f)
				{
					DestroyNow();
				}
			}
		}

		/// <summary>You can manually call this method to destroy the current GameObject now.</summary>
		public void DestroyNow()
		{
			var finalTarget = Target;

			if (finalTarget == null)
			{
				finalTarget = gameObject;
			}

			Destroy(finalTarget);
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Inspector
{
	using UnityEditor;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(LeanDestroy), true)]
	public class LeanDestroy_Inspector : LeanInspector<LeanDestroy>
	{
		private bool showUnusedEvents;

		protected override void DrawInspector()
		{
			Draw("Target", "The GameObject that will be destroyed.\n\nNone/null = This GameObject.");
			Draw("Seconds", "The amount of seconds remaining before this GameObject gets destroyed.\n\n-1 = You must manually call the DestroyNow method.");
		}
	}
}
#endif