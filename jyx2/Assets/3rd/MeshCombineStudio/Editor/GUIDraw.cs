using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace MeshCombineStudio
{ 
    static public class GUIDraw
    {
        public static float indentSpace = 12;
        
        public static void DrawHeader(SerializedProperty foldout, GUIContent guiContent, Color color)
        {
            GUI.color = color;
            EditorGUILayout.BeginVertical("Box");
            GUI.color = Color.white;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            LabelWidthUnderline(guiContent, 14, true, foldout.boolValue);
            Rect rect = GUILayoutUtility.GetLastRect();
            rect.x = 20;
            rect.y += 3;
            rect.width = 20;
            rect.height = 20;
            foldout.boolValue = EditorGUI.Foldout(rect, foldout.boolValue, GUIContent.none);

            EditorGUILayout.EndHorizontal();
            if (foldout.boolValue) GUILayout.Space(4);
        }

        static public void DrawSpacer(float spaceBegin = 5, float height = 5, float spaceEnd = 5)
        {
            GUILayout.Space(spaceBegin - 1);
            EditorGUILayout.BeginHorizontal();
            GUI.color = new Color(0.5f, 0.5f, 0.5f, 1);
            GUILayout.Button(string.Empty, GUILayout.Height(height));
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(spaceEnd - 1);

            GUI.color = Color.white;
        }

        public static void PrefixAndLabel(GUIContent prefix, GUIContent label)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(prefix);
            EditorGUILayout.LabelField(label);
            EditorGUILayout.EndHorizontal();
        }

        public static void PrefixAnd2Labels(string prefix, string label, float labelWidth, string label2)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(prefix);
            EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));
            EditorGUILayout.LabelField(label2, GUILayout.Width(labelWidth));
            EditorGUILayout.EndHorizontal();
        }

        public static void PrefixAnd2Labels(GUIContent prefix, GUIContent label, float labelWidth, GUIContent label2)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(prefix);
            EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));
            EditorGUILayout.LabelField(label2, GUILayout.Width(labelWidth));
            EditorGUILayout.EndHorizontal();
        }

        static public void Label(string label, int fontSize)
        {
            int fontSizeOld = EditorStyles.label.fontSize;
            EditorStyles.boldLabel.fontSize = fontSize;
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel, GUILayout.Height(fontSize + 6));
            EditorStyles.boldLabel.fontSize = fontSizeOld;
        }

        static public void LabelWidthUnderline(GUIContent guiContent, int fontSize, bool boldLabel = true, bool drawUnderline = true)
        {
            int fontSizeOld = EditorStyles.label.fontSize;
            EditorStyles.boldLabel.fontSize = fontSize;
            EditorGUILayout.LabelField(guiContent, boldLabel ? EditorStyles.boldLabel : EditorStyles.label, GUILayout.Height(fontSize + 6));
            EditorStyles.boldLabel.fontSize = fontSizeOld;
            if (drawUnderline) DrawUnderLine();
            GUILayout.Space(5);
        }

        static public void PrefixAndLabelWidthUnderline(GUIContent prefix, GUIContent label, int fontSize, bool drawUnderline = true)
        {
            int fontSizeOld = EditorStyles.label.fontSize;
            EditorStyles.boldLabel.fontSize = fontSize;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(" ", EditorStyles.label, EditorStyles.boldLabel);
            Rect rect = GUILayoutUtility.GetLastRect();
            rect.height = fontSize + 6;
            EditorGUI.LabelField(rect, prefix, EditorStyles.boldLabel);
            GUILayout.Space(-2);
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel, GUILayout.Height(fontSize + 6));
            EditorGUILayout.EndHorizontal();
            EditorStyles.boldLabel.fontSize = fontSizeOld;
            if (drawUnderline) DrawUnderLine();
            GUILayout.Space(5);
        }

        static public void DrawUnderLine(float offsetY = 0)
        {
            Rect rect = GUILayoutUtility.GetLastRect();
            if (EditorGUIUtility.isProSkin) GUI.color = Color.grey; else GUI.color = Color.black;
            GUI.DrawTexture(new Rect(rect.x, rect.yMax + offsetY, rect.width, 1), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }

        static public void PropertyField(SerializedProperty property, GUIContent guiContent, bool indent = false, int indentMinus = 1)
        {
            EditorGUILayout.BeginHorizontal();
            if (indent) EditorGUI.indentLevel++;
            EditorGUILayout.PrefixLabel(guiContent);
            if (indent) EditorGUI.indentLevel -= indentMinus;
            EditorGUILayout.PropertyField(property, GUIContent.none);
            EditorGUILayout.EndHorizontal();
        }

        static public void MaskField(SerializedProperty property, string[] masks, GUIContent guiContent, bool indent = false, int indentMinus = 1)
        {
            EditorGUILayout.BeginHorizontal();
            if (indent) EditorGUI.indentLevel++;
            EditorGUILayout.PrefixLabel(guiContent);
            if (indent) EditorGUI.indentLevel -= indentMinus;
            property.intValue = EditorGUILayout.MaskField(property.intValue, masks);
            EditorGUILayout.EndHorizontal();
        }

        static public bool Toggle(bool toggle, GUIContent guiContent, bool indent = false)
        {
            EditorGUILayout.BeginHorizontal();
            if (indent) EditorGUI.indentLevel++;
            EditorGUILayout.PrefixLabel(guiContent);
            if (indent) EditorGUI.indentLevel--;
            toggle = EditorGUILayout.Toggle(GUIContent.none, toggle);
            EditorGUILayout.EndHorizontal();
            return toggle;
        }

        static public Enum EnumPopup(Enum enumValue, GUIContent guiContent, bool indent = false)
        {
            EditorGUILayout.BeginHorizontal();
            if (indent) EditorGUI.indentLevel++;
            EditorGUILayout.PrefixLabel(guiContent);
            if (indent) EditorGUI.indentLevel--;
            enumValue = EditorGUILayout.EnumPopup(enumValue);
            EditorGUILayout.EndHorizontal();
            return enumValue;
        }

        static public void LayerField(SerializedProperty property, GUIContent guiContent, bool indent = false)
        {
            EditorGUILayout.BeginHorizontal();
            if (indent) EditorGUI.indentLevel++;
            EditorGUILayout.PrefixLabel(guiContent);
            if (indent) EditorGUI.indentLevel--;
            property.intValue = EditorGUILayout.LayerField(GUIContent.none, property.intValue);
            EditorGUILayout.EndHorizontal();
        }

        static public bool PropertyArray(SerializedProperty property, GUIContent arrayName, GUIContent elementName, bool drawUnderLine = true, bool editArrayLength = true, bool indent = false, bool newElementNull = false)
        {
            EditorGUILayout.BeginHorizontal();
            // EditorGUI.indentLevel++;
            // property.isExpanded = EditorGUILayout.Toggle(property.isExpanded, GUILayout.Width(15));
            EditorGUILayout.PrefixLabel(new GUIContent(arrayName.text + " Size", arrayName.tooltip));

            GUI.changed = false;

            if (editArrayLength)
            {
                if (!indent) EditorGUI.indentLevel--;
                int oldSize = property.arraySize;
                property.arraySize = EditorGUILayout.IntField("", property.arraySize);
                if (GUI.changed && newElementNull && property.arraySize > oldSize)
                {
                    SerializedProperty prop = property.GetArrayElementAtIndex(property.arraySize - 1);
                    if (prop != null) prop.objectReferenceValue = null;
                }
                if (!indent) EditorGUI.indentLevel++;
            }

            // if (property.isExpanded)
            {
                EditorGUILayout.EndHorizontal();
                // if (indent) EditorGUI.indentLevel++;

                GUIContent elementNameCopy = new GUIContent(elementName);
                
                for (int i = 0; i < property.arraySize; i++)
                {
                    SerializedProperty elementProperty = property.GetArrayElementAtIndex(i);

                    elementNameCopy.text = elementName.text + " " + i;

                    PropertyField(elementProperty, elementNameCopy, true, indent ? 1 : 2);
                    
                    if (!indent) EditorGUI.indentLevel++; 
                }
                // if (indent) EditorGUI.indentLevel--;
            }

            return GUI.changed;
            // else EditorGUILayout.EndHorizontal();
            // EditorGUI.indentLevel--;
        }
    }
}