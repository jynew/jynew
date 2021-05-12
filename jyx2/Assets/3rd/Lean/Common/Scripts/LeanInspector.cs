#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace Lean.Common
{
	/// <summary>This class allows you to quickly make custom inspectors with common features.</summary>
	public class LeanInspector<T> : Editor
		where T : Object
	{
		protected T tgt;

		protected T[] tgts;

		private static readonly string[] propertyToExclude = new string[] { "m_Script" };

		private static GUIContent customContent = new GUIContent();
		
		private static GUIStyle expandStyle;

		public static void BeginError(bool error)
		{
			var rect = EditorGUILayout.BeginVertical(GUIStyle.none);

			EditorGUI.DrawRect(rect, error == true ? Color.red : Color.clear);
		}

		public static void EndError()
		{
			EditorGUILayout.EndVertical();
		}

		public static Rect Reserve()
		{
			var rect = EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField(GUIContent.none);
			EditorGUILayout.EndVertical();

			return rect;
		}

		public override void OnInspectorGUI()
		{
			tgt  = (T)target;
			tgts = targets.Select(t => (T)t).ToArray();

			EditorGUI.BeginChangeCheck();
			{
				serializedObject.Update();

				EditorGUILayout.Separator();

				DrawInspector();

				EditorGUILayout.Separator();

				serializedObject.ApplyModifiedProperties();
			}
			if (EditorGUI.EndChangeCheck() == true)
			{
				GUI.changed = true; Repaint();

				//Dirty();
			}
		}

		public virtual void OnSceneGUI()
		{
			tgt = (T)target;

			DrawScene();
		}

		protected void Each(System.Action<T> update, bool dirty = true)
		{
			if (dirty == true)
			{
				Undo.RecordObjects(tgts, "Inspector");
			}

			foreach (var t in tgts)
			{
				update(t);
			}

			if (dirty == true)
			{
				Dirty();
			}
		}

		protected bool Any(System.Func<T, bool> check)
		{
			foreach (var t in tgts)
			{
				if (check(t) == true)
				{
					return true;
				}
			}

			return false;
		}

		protected bool All(System.Func<T, bool> check)
		{
			foreach (var t in tgts)
			{
				if (check(t) == false)
				{
					return false;
				}
			}

			return true;
		}

		public static void DrawExpand(ref bool expand, Rect rect)
		{
			if (expandStyle == null)
			{
				expandStyle = new GUIStyle(EditorStyles.miniLabel); expandStyle.alignment = TextAnchor.MiddleRight;
			}

			if (EditorGUI.DropdownButton(new Rect(rect.position + Vector2.left * 15, new Vector2(15.0f, rect.height)), new GUIContent(expand ? "-" : "+"), FocusType.Keyboard, expandStyle) == true)
			{
				expand = !expand;
			}
		}

		protected bool DrawExpand(ref bool expand, string propertyPath, string overrideTooltip = null, string overrideText = null)
		{
			var rect     = Reserve();
			var property = serializedObject.FindProperty(propertyPath);

			customContent.text    = string.IsNullOrEmpty(overrideText   ) == false ? overrideText    : property.displayName;
			customContent.tooltip = string.IsNullOrEmpty(overrideTooltip) == false ? overrideTooltip : property.tooltip;

			DrawExpand(ref expand, rect);

			EditorGUI.BeginChangeCheck();

			EditorGUI.PropertyField(rect, property, customContent, true);

			var changed = EditorGUI.EndChangeCheck();

			return changed;
		}

		protected bool DrawMinMax(string propertyPath, float min, float max, string overrideTooltip = null, string overrideText = null)
		{
			var property = serializedObject.FindProperty(propertyPath);
			var value    = property.vector2Value;

			customContent.text    = string.IsNullOrEmpty(overrideText   ) == false ? overrideText    : property.displayName;
			customContent.tooltip = string.IsNullOrEmpty(overrideTooltip) == false ? overrideTooltip : property.tooltip;

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.MinMaxSlider(customContent, ref value.x, ref value.y, min, max);

			if (EditorGUI.EndChangeCheck() == true)
			{
				property.vector2Value = value;

				return true;
			}

			return false;
		}

		protected bool DrawEulerAngles(string propertyPath, string overrideTooltip = null, string overrideText = null)
		{
			var property = serializedObject.FindProperty(propertyPath);
			var mixed    = EditorGUI.showMixedValue;

			customContent.text    = string.IsNullOrEmpty(overrideText   ) == false ? overrideText    : property.displayName;
			customContent.tooltip = string.IsNullOrEmpty(overrideTooltip) == false ? overrideTooltip : property.tooltip;

			EditorGUI.BeginChangeCheck();

			EditorGUI.showMixedValue = property.hasMultipleDifferentValues;

			var oldEulerAngles = property.quaternionValue.eulerAngles;
			var newEulerAngles = EditorGUILayout.Vector3Field(customContent, oldEulerAngles);

			if (oldEulerAngles != newEulerAngles)
			{
				property.quaternionValue = Quaternion.Euler(newEulerAngles);
			}

			EditorGUI.showMixedValue = mixed;

			return EditorGUI.EndChangeCheck();
		}

		protected bool Draw(string propertyPath, string overrideTooltip = null, string overrideText = null)
		{
			var property = serializedObject.FindProperty(propertyPath);

			customContent.text    = string.IsNullOrEmpty(overrideText   ) == false ? overrideText    : property.displayName;
			customContent.tooltip = string.IsNullOrEmpty(overrideTooltip) == false ? overrideTooltip : property.tooltip;

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.PropertyField(property, customContent, true);

			return EditorGUI.EndChangeCheck();
		}

		protected virtual void DrawInspector()
		{
			DrawPropertiesExcluding(serializedObject, propertyToExclude);
		}

		protected virtual void DrawScene()
		{
		}

		protected void Dirty()
		{
			for (var i = targets.Length - 1; i >= 0; i--)
			{
				EditorUtility.SetDirty(targets[i]);
			}

			serializedObject.Update();
		}
	}
}
#endif