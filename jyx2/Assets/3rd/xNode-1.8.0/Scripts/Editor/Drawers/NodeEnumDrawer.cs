using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace XNodeEditor {
	[CustomPropertyDrawer(typeof(NodeEnumAttribute))]
	public class NodeEnumDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);

			EnumPopup(position, property, label);

			EditorGUI.EndProperty();
		}

		public static void EnumPopup(Rect position, SerializedProperty property, GUIContent label) {
			// Throw error on wrong type
			if (property.propertyType != SerializedPropertyType.Enum) {
				throw new ArgumentException("Parameter selected must be of type System.Enum");
			}

			// Add label
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			// Get current enum name
			string enumName = "";
			if (property.enumValueIndex >= 0 && property.enumValueIndex < property.enumDisplayNames.Length) enumName = property.enumDisplayNames[property.enumValueIndex];

#if UNITY_2017_1_OR_NEWER
			// Display dropdown
			if (EditorGUI.DropdownButton(position, new GUIContent(enumName), FocusType.Passive)) {
				// Position is all wrong if we show the dropdown during the node draw phase.
				// Instead, add it to onLateGUI to display it later.
				NodeEditorWindow.current.onLateGUI += () => ShowContextMenuAtMouse(property);
			}
#else
			// Display dropdown
			if (GUI.Button(position, new GUIContent(enumName), "MiniPopup")) {
				// Position is all wrong if we show the dropdown during the node draw phase.
				// Instead, add it to onLateGUI to display it later.
				NodeEditorWindow.current.onLateGUI += () => ShowContextMenuAtMouse(property);
			}
#endif
		}

		public static void ShowContextMenuAtMouse(SerializedProperty property) {
			// Initialize menu
			GenericMenu menu = new GenericMenu();

			// Add all enum display names to menu
			for (int i = 0; i < property.enumDisplayNames.Length; i++) {
				int index = i;
				menu.AddItem(new GUIContent(property.enumDisplayNames[i]), false, () => SetEnum(property, index));
			}

			// Display at cursor position
			Rect r = new Rect(Event.current.mousePosition, new Vector2(0, 0));
			menu.DropDown(r);
		}

		private static void SetEnum(SerializedProperty property, int index) {
			property.enumValueIndex = index;
			property.serializedObject.ApplyModifiedProperties();
			property.serializedObject.Update();
		}
	}
}