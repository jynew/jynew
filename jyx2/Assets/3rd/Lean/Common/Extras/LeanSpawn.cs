using UnityEngine;

namespace Lean.Common
{
	/// <summary>This component allows you to spawn a prefab at the specified world point.
	/// NOTE: For this component to work you must manually call the <b>Spawn</b> method from somewhere.</summary>
	[HelpURL(LeanHelper.HelpUrlPrefix + "LeanSpawn")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Spawn")]
	public class LeanSpawn : MonoBehaviour
	{
		public enum SourceType
		{
			ThisTransform,
			Prefab
		}

		/// <summary>The prefab that this component can spawn.</summary>
		public Transform Prefab;

		/// <summary>If you call <b>Spawn()</b>, where should the position come from?</summary>
		public SourceType DefaultPosition;

		/// <summary>If you call <b>Spawn()</b>, where should the rotation come from?</summary>
		public SourceType DefaultRotation;

		/// <summary>This will spawn <b>Prefab</b> at the current <b>Transform.position</b>.</summary>
		public void Spawn()
		{
			if (Prefab != null)
			{
				var position = DefaultPosition == SourceType.Prefab ? Prefab.position : transform.position;
				var rotation = DefaultRotation == SourceType.Prefab ? Prefab.rotation : transform.rotation;
				var clone    = Instantiate(Prefab, position, rotation);

				clone.gameObject.SetActive(true);
			}
		}

		/// <summary>This will spawn <b>Prefab</b> at the specified position in world space.</summary>
		public void Spawn(Vector3 position)
		{
			if (Prefab != null)
			{
				var rotation = DefaultRotation == SourceType.Prefab ? Prefab.rotation : transform.rotation;
				var clone    = Instantiate(Prefab, position, rotation);

				clone.gameObject.SetActive(true);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Inspector
{
	using UnityEditor;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(LeanSpawn), true)]
	public class LeanSpawn_Inspector : LeanInspector<LeanSpawn>
	{
		private bool showUnusedEvents;

		protected override void DrawInspector()
		{
			Draw("Prefab", "The prefab that this component can spawn.");
			Draw("Prefab", "If you call Spawn(), where should the position come from?");
			Draw("Prefab", "If you call Spawn(), where should the rotation come from?");
		}
	}
}
#endif