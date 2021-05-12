using UnityEngine;

namespace Lean.Common.Examples
{
	/// <summary>This component will automatically update the event system if you switch to using the new <b>InputSystem</b>.</summary>
	[ExecuteInEditMode]
	[AddComponentMenu("Lean/Common/Upgrade EventSystem")]
	public class LeanUpgradeEventSystem : MonoBehaviour
	{
#if UNITY_EDITOR && ENABLE_INPUT_SYSTEM
		protected virtual void Awake()
		{
			var module = FindObjectOfType<UnityEngine.EventSystems.StandaloneInputModule>();

			if (module != null)
			{
				Debug.Log("Replacing old StandaloneInputModule with new InputSystemUIInputModule.", module.gameObject);

				module.gameObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

				DestroyImmediate(module);
			}
		}
#endif
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Examples
{
	using UnityEditor;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(LeanUpgradeEventSystem))]
	public class LeanUpgradeEventSystem_Editor : LeanInspector<LeanUpgradeEventSystem>
	{
		protected override void DrawInspector()
		{
			EditorGUILayout.HelpBox("This component will automatically update the event system if you switch to using the new InputSystem.", MessageType.Info);
		}
	}
}
#endif