using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CustomHeaderAttribute))]
public class CustomHeaderDrawer : PropertyDrawer
{
    private readonly List<string> _displayNames = new List<string>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var att = (CustomHeaderAttribute)attribute;
        if (property.type == "Enum")
        {
            var type = property.serializedObject.targetObject.GetType();
            var field = type.GetField(property.name);
            var enumtype = field.FieldType;
            foreach (var enumName in property.enumNames)
            {
                var enumfield = enumtype.GetField(enumName);
                var hds = enumfield.GetCustomAttributes(typeof(CustomHeaderAttribute), false);
                _displayNames.Add(hds.Length <= 0 ? enumName : ((CustomHeaderAttribute)hds[0]).header);
            }
            EditorGUI.BeginChangeCheck();
            var value = EditorGUI.Popup(position, att.header, property.enumValueIndex, _displayNames.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                property.enumValueIndex = value;
            }
        }
        else 
        {
            label.text = att.header;
            //重绘GUI
            EditorGUI.PropertyField(position, property, label);
        }
    }
}
