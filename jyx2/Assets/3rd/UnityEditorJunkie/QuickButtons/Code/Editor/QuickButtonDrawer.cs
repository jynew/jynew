// ---------------------------------------------------------------------------- 
// Author: Ryan Hipple
// Date:   03/12/2019
// ----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace RoboRyanTron.QuickButtons.Editor
{
    [CustomPropertyDrawer(typeof(QuickButton))]
    public class QuickButtonDrawer : PropertyDrawer
    {
        /// <summary>
        /// Get the c# object that a serialized property references by starting
        /// with the serialized property target object and reflecting each field
        /// in the field path until reaching the desired depth.
        /// </summary>
        /// <param name="property">Property to get an object for.</param>
        /// <param name="pathOffset">
        /// How far to move towards the end of the property path before
        /// returning the result. 0 returns the object at the end, the true
        /// target of the property. -1 returns the object that the object
        /// representing the property is defined on and so on.
        /// </param>
        /// <param name="skipList">
        /// Should arrays and lists be skipped as potential results?
        /// </param>
        /// <returns>
        /// The object represented by the serialized property or an object
        /// earlier in the property path if pathOffset is less than 0;
        /// </returns>
        private static object GetObjectForProperty(SerializedProperty property, int pathOffset = 0, bool skipList = true)
        {
            const BindingFlags flags = BindingFlags.Instance | 
                BindingFlags.NonPublic | BindingFlags.Public;
            
            Type t = property.serializedObject.targetObject.GetType();
            object obj = property.serializedObject.targetObject;

            // Simplify the format Unity uses for arrays to simplify parsing
            string path = property.propertyPath.Replace(".Array.data[", "[");
            
            // Split the path into a list of field names
            string[] props = path.Split('.');
            int end = props.Length - 1 + pathOffset;
            
            for (int i = 0; i < props.Length + pathOffset; i++)
            {
                // Attempt to get a name and array index
                string[] nameAndIndex = props[i].Split('[', ']');
                int arrayIndex = nameAndIndex.Length <= 1 ? -1 :
                    int.Parse(nameAndIndex[1]);

                // Search in base classes to resolve private types
                Type currentType = t;
                FieldInfo field = null;
                while (field == null && currentType != null)
                {
                    field = currentType.GetField(nameAndIndex[0], flags);
                    if (field == null)
                        currentType = currentType.BaseType;
                    else
                        obj = field.GetValue(obj);
                }
                if (field == null) obj = null;
                t = field?.FieldType;

                if (!skipList)
                {
                    if (i == end)
                        return obj;
                }
                
                // If there is an array index we need to step into the list or array
                if (arrayIndex >= 0)
                {
                    if (obj is IList col && arrayIndex < col.Count)
                    {
                        obj = col[arrayIndex];
                        t = t.IsArray ? t.GetElementType() : 
                            col.GetType().GetGenericArguments()[0];
                    }
                }
                
                if (i == end)
                    return obj;
            }
            return null;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Magic number 15 is reflected from Unity's EditorGUI.cs
            // "internal static float indent" property
            position.xMin += EditorGUI.indentLevel * 15;

            if (GUI.Button(position, label))
            {
                QuickButton button = GetObjectForProperty(property) as QuickButton;
                if (button == null)
                {
                    Debug.LogError("Unable to resolve QuickButton from property " + property.propertyPath);
                    return;
                }
                
                object target = GetObjectForProperty(property, -1) ?? 
                    property.serializedObject.targetObject;

                button.Invoke(target);
            }
        }
    }
}