using System;
using System.Reflection;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UniRx
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class InspectorDisplayAttribute : PropertyAttribute
    {
        public string FieldName { get; private set; }
        public bool NotifyPropertyChanged { get; private set; }

        public InspectorDisplayAttribute(string fieldName = "value", bool notifyPropertyChanged = true)
        {
            FieldName = fieldName;
            NotifyPropertyChanged = notifyPropertyChanged;
        }
    }

    /// <summary>
    /// Enables multiline input field for StringReactiveProperty. Default line is 3.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class MultilineReactivePropertyAttribute : PropertyAttribute
    {
        public int Lines { get; private set; }

        public MultilineReactivePropertyAttribute()
        {
            Lines = 3;
        }

        public MultilineReactivePropertyAttribute(int lines)
        {
            this.Lines = lines;
        }
    }

    /// <summary>
    /// Enables range input field for Int/FloatReactiveProperty.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class RangeReactivePropertyAttribute : PropertyAttribute
    {
        public float Min { get; private set; }
        public float Max { get; private set; }

        public RangeReactivePropertyAttribute(float min, float max)
        {
            this.Min = min;
            this.Max = max;
        }
    }

#if UNITY_EDITOR


    // InspectorDisplay and for Specialized ReactiveProperty
    // If you want to customize other specialized ReactiveProperty
    // [UnityEditor.CustomPropertyDrawer(typeof(YourSpecializedReactiveProperty))]
    // public class ExtendInspectorDisplayDrawer : InspectorDisplayDrawer { } 

    [UnityEditor.CustomPropertyDrawer(typeof(InspectorDisplayAttribute))]
    [UnityEditor.CustomPropertyDrawer(typeof(IntReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(LongReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(ByteReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(FloatReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(DoubleReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(StringReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(BoolReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(Vector2ReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(Vector3ReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(Vector4ReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(ColorReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(RectReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(AnimationCurveReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(BoundsReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(QuaternionReactiveProperty))]
    public class InspectorDisplayDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            string fieldName;
            bool notifyPropertyChanged;
            {
                var attr = this.attribute as InspectorDisplayAttribute;
                fieldName = (attr == null) ? "value" : attr.FieldName;
                notifyPropertyChanged = (attr == null) ? true : attr.NotifyPropertyChanged;
            }

            if (notifyPropertyChanged)
            {
                EditorGUI.BeginChangeCheck();
            }
            var targetSerializedProperty = property.FindPropertyRelative(fieldName);
            if (targetSerializedProperty == null)
            {
                UnityEditor.EditorGUI.LabelField(position, label, new GUIContent() { text = "InspectorDisplay can't find target:" + fieldName });
                if (notifyPropertyChanged)
                {
                    EditorGUI.EndChangeCheck();
                }
                return;
            }
            else
            {
                EmitPropertyField(position, targetSerializedProperty, label);
            }

            if (notifyPropertyChanged)
            {
                if (EditorGUI.EndChangeCheck())
                {
                    property.serializedObject.ApplyModifiedProperties(); // deserialize to field

                    var paths = property.propertyPath.Split('.'); // X.Y.Z...
                    var attachedComponent = property.serializedObject.targetObject;

                    var targetProp = (paths.Length == 1)
                        ? fieldInfo.GetValue(attachedComponent)
                        : GetValueRecursive(attachedComponent, 0, paths);
                    if (targetProp == null) return;
                    var propInfo = targetProp.GetType().GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    var modifiedValue = propInfo.GetValue(targetProp, null); // retrieve new value

                    var methodInfo = targetProp.GetType().GetMethod("SetValueAndForceNotify", BindingFlags.IgnoreCase | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (methodInfo != null)
                    {
                        methodInfo.Invoke(targetProp, new object[] { modifiedValue });
                    }
                }
                else
                {
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        object GetValueRecursive(object obj, int index, string[] paths)
        {
            var path = paths[index];

            FieldInfo fldInfo = null;
            var type = obj.GetType();
            while (fldInfo == null)
            {
                // attempt to get information about the field
                fldInfo = type.GetField(path, BindingFlags.IgnoreCase | BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (fldInfo != null ||
                    type.BaseType == null || 
                    type.BaseType.IsSubclassOf(typeof(ReactiveProperty<>))) break;

                // if the field information is missing, it may be in the base class
                type = type.BaseType;
            }

            // If array, path = Array.data[index]
            if (fldInfo == null && path == "Array")
            {
                try
                {
                    path = paths[++index];
                    var m = Regex.Match(path, @"(.+)\[([0-9]+)*\]");
                    var arrayIndex = int.Parse(m.Groups[2].Value);
                    var arrayValue = (obj as System.Collections.IList)[arrayIndex];
                    if (index < paths.Length - 1)
                    {
                        return GetValueRecursive(arrayValue, ++index, paths);
                    }
                    else
                    {
                        return arrayValue;
                    }
                }
                catch
                {
                    Debug.Log("InspectorDisplayDrawer Exception, objType:" + obj.GetType().Name + " path:" + string.Join(", ", paths));
                    throw;
                }
            }
            else if (fldInfo == null)
            {
                throw new Exception("Can't decode path, please report to UniRx's GitHub issues:" + string.Join(", ", paths));
            }

            var v = fldInfo.GetValue(obj);
            if (index < paths.Length - 1)
            {
                return GetValueRecursive(v, ++index, paths);
            }

            return v;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attr = this.attribute as InspectorDisplayAttribute;
            var fieldName = (attr == null) ? "value" : attr.FieldName;

            var height = base.GetPropertyHeight(property, label);
            var valueProperty = property.FindPropertyRelative(fieldName);
            if (valueProperty == null)
            {
                return height;
            }

            if (valueProperty.propertyType == SerializedPropertyType.Rect)
            {
                return height * 2;
            }
            if (valueProperty.propertyType == SerializedPropertyType.Bounds)
            {
                return height * 3;
            }
            if (valueProperty.propertyType == SerializedPropertyType.String)
            {
                var multilineAttr = GetMultilineAttribute();
                if (multilineAttr != null)
                {
                    return ((!EditorGUIUtility.wideMode) ? 16f : 0f) + 16f + (float)((multilineAttr.Lines - 1) * 13);
                };
            }

            if (valueProperty.isExpanded)
            {
                var count = 0;
                var e = valueProperty.GetEnumerator();
                while (e.MoveNext()) count++;
                return ((height + 4) * count) + 6; // (Line = 20 + Padding) ?
            }

            return height;
        }

        protected virtual void EmitPropertyField(Rect position, UnityEditor.SerializedProperty targetSerializedProperty, GUIContent label)
        {
            var multiline = GetMultilineAttribute();
            if (multiline == null)
            {
                var range = GetRangeAttribute();
                if (range == null)
                {
                    UnityEditor.EditorGUI.PropertyField(position, targetSerializedProperty, label, includeChildren: true);
                }
                else
                {
                    if (targetSerializedProperty.propertyType == SerializedPropertyType.Float)
                    {
                        EditorGUI.Slider(position, targetSerializedProperty, range.Min, range.Max, label);
                    }
                    else if (targetSerializedProperty.propertyType == SerializedPropertyType.Integer)
                    {
                        EditorGUI.IntSlider(position, targetSerializedProperty, (int)range.Min, (int)range.Max, label);
                    }
                    else
                    {
                        EditorGUI.LabelField(position, label.text, "Use Range with float or int.");
                    }
                }
            }
            else
            {
                var property = targetSerializedProperty;

                label = EditorGUI.BeginProperty(position, label, property);
                var method = typeof(EditorGUI).GetMethod("MultiFieldPrefixLabel", BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic);
                position = (Rect)method.Invoke(null, new object[] { position, 0, label, 1 });

                EditorGUI.BeginChangeCheck();
                int indentLevel = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                var stringValue = EditorGUI.TextArea(position, property.stringValue);
                EditorGUI.indentLevel = indentLevel;
                if (EditorGUI.EndChangeCheck())
                {
                    property.stringValue = stringValue;
                }
                EditorGUI.EndProperty();
            }
        }

        MultilineReactivePropertyAttribute GetMultilineAttribute()
        {
            var fi = this.fieldInfo;
            if (fi == null) return null;
            return fi.GetCustomAttributes(false).OfType<MultilineReactivePropertyAttribute>().FirstOrDefault();
        }

        RangeReactivePropertyAttribute GetRangeAttribute()
        {
            var fi = this.fieldInfo;
            if (fi == null) return null;
            return fi.GetCustomAttributes(false).OfType<RangeReactivePropertyAttribute>().FirstOrDefault();
        }
    }

#endif
}