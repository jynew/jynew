namespace UIWidgets.WidgetGeneration
{
	using System;
	using System.Collections;
	using UnityEngine;

	/// <summary>
	/// Prefabs generator.
	/// </summary>
	[ExecuteInEditMode]
	public class PrefabsGeneratorStarter : MonoBehaviour
	{
		/// <summary>
		/// Full Type name of the prefabs generator class.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		public string Generator;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public void Start()
		{
			if (Application.isPlaying)
			{
				Destroy(this);
				return;
			}

			if (string.IsNullOrEmpty(Generator))
			{
				Destroy(this);
				return;
			}

			var type = UtilitiesEditor.GetType(Generator);
			if (type == null)
			{
				DestroyImmediate(gameObject);
				return;
			}

			StartCoroutine(RunGeneration(type));
		}

		IEnumerator RunGeneration(Type type)
		{
			yield return null; // delay 1 frame

			var method = type.GetMethod("Run");
			method.Invoke(null, new object[] { });

			DestroyImmediate(gameObject);
		}
	}
}