// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Animancer.Editor
{
    /// <summary>[Editor-Only]
    /// A custom Inspector for <see cref="Sprite"/>s allows you to directly edit them instead of just showing their
    /// details like the default one does.
    /// </summary>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/SpriteEditor
    /// 
    [CustomEditor(typeof(Sprite), true), CanEditMultipleObjects]
    public class SpriteEditor : UnityEditor.Editor
    {
        /************************************************************************************************************************/

        private const string
            NameTooltip = "The asset name of the sprite",
            RectTooltip = "The texture area occupied by the sprite",
            PivotTooltip = "The origin point of the sprite relative to its Rect",
            BorderTooltip = "The edge sizes used when 9-Slicing the sprite for the UI system (ignored by SpriteRenderers)";

        [NonSerialized]
        private SerializedProperty
            _Name,
            _Rect,
            _Pivot,
            _Border;

        [NonSerialized]
        private TwinFloatField[]
            _RectFields,
            _PivotFields,
            _BorderFields;

        [NonSerialized]
        private bool _HasBeenModified;

        /************************************************************************************************************************/

        private void OnEnable()
        {
            InitialisePreview();

            _Name = serializedObject.FindProperty($"m{nameof(_Name)}");

            _Rect = serializedObject.FindProperty($"m{nameof(_Rect)}");
            if (_Rect != null)
            {
                _RectFields = new TwinFloatField[]
                {
                    new TwinFloatField(_Rect.FindPropertyRelative(nameof(Rect.x)), new GUIContent("X (Left)",
                        "The distance from the left edge of the texture to the left edge of the sprite"), false),
                    new TwinFloatField(_Rect.FindPropertyRelative(nameof(Rect.y)), new GUIContent("Y (Bottom)",
                        "The distance from the bottom edge of the texture to the bottom edge of the sprite"), false),
                    new TwinFloatField(_Rect.FindPropertyRelative(nameof(Rect.width)), new GUIContent("Width",
                        "The horizontal size of the sprite"), false),
                    new TwinFloatField(_Rect.FindPropertyRelative(nameof(Rect.height)), new GUIContent("Height",
                        "The vertical size of the sprite"), false),
                };
            }

            _Pivot = serializedObject.FindProperty($"m{nameof(_Pivot)}");
            if (_Pivot != null)
            {
                _PivotFields = new TwinFloatField[]
                {
                    new TwinFloatField(_Pivot.FindPropertyRelative(nameof(Vector2.x)), new GUIContent("X",
                        "The horizontal distance from the left edge of the sprite to the pivot point"), true),
                    new TwinFloatField(_Pivot.FindPropertyRelative(nameof(Vector2.y)), new GUIContent("Y",
                        "The vertical distance from the bottom edge of the sprite to the pivot point"), true),
                };
            }

            _Border = serializedObject.FindProperty($"m{nameof(_Border)}");
            if (_Border != null)
            {
                _BorderFields = new TwinFloatField[]
                {
                    new TwinFloatField(_Border.FindPropertyRelative(nameof(Vector4.x)), new GUIContent("Left",
                        BorderTooltip), false),
                    new TwinFloatField(_Border.FindPropertyRelative(nameof(Vector4.y)), new GUIContent("Bottom",
                        BorderTooltip), false),
                    new TwinFloatField(_Border.FindPropertyRelative(nameof(Vector4.z)), new GUIContent("Right",
                        BorderTooltip), false),
                    new TwinFloatField(_Border.FindPropertyRelative(nameof(Vector4.w)), new GUIContent("Top",
                        BorderTooltip), false),
                };
            }
        }

        /************************************************************************************************************************/

        private void OnDisable()
        {
            CleanUpPreview();

            if (_HasBeenModified)
            {
                var sprite = target as Sprite;
                if (sprite == null)
                    return;

                if (EditorUtility.DisplayDialog("Unapplied Import Settings",
                    $"Unapplied import settings for '{sprite.name}' in '{AssetDatabase.GetAssetPath(sprite)}'",
                    nameof(Apply), nameof(Revert)))
                    Apply();
            }
        }

        /************************************************************************************************************************/
        #region Inspector
        /************************************************************************************************************************/

        /// <summary>Called by the Unity editor to draw the custom Inspector GUI elements.</summary>
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            DoNameGUI();
            DoRectGUI();
            DoPivotGUI();
            DoBorderGUI();

            if (EditorGUI.EndChangeCheck())
                _HasBeenModified = true;

            GUILayout.Space(AnimancerGUI.StandardSpacing);
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUI.enabled = _HasBeenModified;
                if (GUILayout.Button(nameof(Revert)))
                    Revert();
                if (GUILayout.Button(nameof(Apply)))
                    Apply();
            }
            GUILayout.EndHorizontal();
        }

        /************************************************************************************************************************/

        private void DoNameGUI()
        {
            GUILayout.BeginHorizontal();
            var enabled = GUI.enabled;

            if (_Name.hasMultipleDifferentValues)
                GUI.enabled = false;

            EditorGUILayout.PropertyField(_Name, AnimancerGUI.TempContent("Name", NameTooltip), true);

            GUI.enabled = true;

            var changed = EditorGUI.EndChangeCheck();// Exclude the Rename button from the main change check.

            if (GUILayout.Button("Rename Tool", EditorStyles.miniButton, AnimancerGUI.DontExpandWidth))
                AnimancerToolsWindow.Open(typeof(AnimancerToolsWindow.RenameSprites));

            EditorGUI.BeginChangeCheck();
            if (changed)
                GUI.changed = true;

            GUI.enabled = enabled;
            GUILayout.EndHorizontal();
        }

        /************************************************************************************************************************/

        private static ConversionCache<float, string> _NormalizedSuffixCache, _PixelSuffixCache;

        private static float DoTwinFloatFieldGUI(Rect area, GUIContent label, float value, float normalizeMultiplier, bool isNormalized)
        {
            if (_PixelSuffixCache == null)
            {
                _PixelSuffixCache = new ConversionCache<float, string>((s) => s + "px");
                _NormalizedSuffixCache = new ConversionCache<float, string>((x) => x + "x");
            }

            var split = (area.width - EditorGUIUtility.labelWidth - AnimancerGUI.StandardSpacing) * 0.5f;
            var normalizedArea = AnimancerGUI.StealFromRight(ref area, Mathf.Floor(split), AnimancerGUI.StandardSpacing);

            var pixels = isNormalized ? value / normalizeMultiplier : value;
            var normalized = isNormalized ? value : value * normalizeMultiplier;

            EditorGUI.BeginChangeCheck();
            pixels = AnimancerGUI.DoSpecialFloatField(area, label, pixels, _PixelSuffixCache);
            if (EditorGUI.EndChangeCheck())
                value = isNormalized ? pixels * normalizeMultiplier : pixels;

            EditorGUI.BeginChangeCheck();
            normalized = AnimancerGUI.DoSpecialFloatField(normalizedArea, null, normalized, _NormalizedSuffixCache);
            if (EditorGUI.EndChangeCheck())
                value = isNormalized ? normalized : normalized / normalizeMultiplier;

            return value;
        }

        /************************************************************************************************************************/

        private void DoRectGUI()
        {
            var texture = ((Sprite)target).texture;
            _RectFields[0].normalizeMultiplier = _RectFields[2].normalizeMultiplier = 1f / texture.width;
            _RectFields[1].normalizeMultiplier = _RectFields[3].normalizeMultiplier = 1f / texture.height;

            TwinFloatField.DoGroupGUI(_Rect, AnimancerGUI.TempContent("Rect", RectTooltip), _RectFields);
        }

        /************************************************************************************************************************/

        private void DoPivotGUI()
        {
            var showMixedValue = EditorGUI.showMixedValue;

            var targets = this.targets;
            var size = targets[0] is Sprite sprite ? sprite.rect.size : Vector2.one;
            for (int i = 1; i < targets.Length; i++)
            {
                sprite = targets[i] as Sprite;
                if (sprite == null || sprite.rect.size != size)
                    EditorGUI.showMixedValue = true;
            }

            _PivotFields[0].normalizeMultiplier = 1f / size.x;
            _PivotFields[1].normalizeMultiplier = 1f / size.y;

            TwinFloatField.DoGroupGUI(_Pivot, AnimancerGUI.TempContent("Pivot", PivotTooltip), _PivotFields);

            EditorGUI.showMixedValue = showMixedValue;
        }

        /************************************************************************************************************************/

        private void DoBorderGUI()
        {
            var size = _Rect.rectValue.size;
            _BorderFields[0].normalizeMultiplier = _BorderFields[2].normalizeMultiplier = 1f / size.x;
            _BorderFields[1].normalizeMultiplier = _BorderFields[3].normalizeMultiplier = 1f / size.y;

            TwinFloatField.DoGroupGUI(_Border, AnimancerGUI.TempContent("Border", BorderTooltip), _BorderFields);
        }

        /************************************************************************************************************************/

        private void Revert()
        {
            AnimancerGUI.Deselect();
            _HasBeenModified = false;
            serializedObject.Update();
        }

        /************************************************************************************************************************/

        private void Apply()
        {
            AnimancerGUI.Deselect();
            _HasBeenModified = false;
            var targets = this.targets;

            var path = AssetDatabase.GetAssetPath(targets[0]);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            var spriteSheet = importer.spritesheet;
            var hasError = false;

            for (int i = 0; i < targets.Length; i++)
                Apply((Sprite)targets[i], spriteSheet, ref hasError);

            serializedObject.Update();

            if (!hasError)
            {
                importer.spritesheet = spriteSheet;
                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
            }
        }

        /************************************************************************************************************************/

        private void Apply(Sprite sprite, SpriteMetaData[] spriteSheet, ref bool hasError)
        {
            for (int i = 0; i < spriteSheet.Length; i++)
            {
                ref var spriteData = ref spriteSheet[i];
                if (spriteData.name == sprite.name &&
                    spriteData.rect == sprite.rect)
                {
                    if (!_Name.hasMultipleDifferentValues)
                        spriteData.name = _Name.stringValue;

                    if (!_Rect.hasMultipleDifferentValues)
                        spriteData.rect = _Rect.rectValue;

                    if (!_Pivot.hasMultipleDifferentValues)
                        spriteData.pivot = _Pivot.vector2Value;

                    if (!_Border.hasMultipleDifferentValues)
                        spriteData.border = _Border.vector4Value;

                    if (spriteData.rect.xMin < 0 ||
                        spriteData.rect.yMin < 0 ||
                        spriteData.rect.xMax > sprite.texture.width ||
                        spriteData.rect.yMax > sprite.texture.height)
                    {
                        hasError = true;
                        Debug.LogError($"This modification would have put '{sprite.name}' out of bounds" +
                            $" so these modifications were not applied.");
                    }

                    return;
                }
            }

            Debug.LogError($"Unable to find data for {sprite.name}", sprite);
        }

        /************************************************************************************************************************/
        #region Twin Float Field
        /************************************************************************************************************************/

        /// <summary>
        /// A wrapper around a <see cref="SerializedProperty"/> to display it using two float fields where one is
        /// normalized and the other is not.
        /// </summary>
        public sealed class TwinFloatField
        {
            /************************************************************************************************************************/

            /// <summary>The target property.</summary>
            public readonly SerializedProperty Property;

            /// <summary>The label to display next to the property.</summary>
            public readonly GUIContent Label;

            /// <summary>Is the serialized property value normalized?</summary>
            public readonly bool IsNormalized;

            /// <summary>The multiplier to turn a non-normalized value into a normalized one.</summary>
            public float normalizeMultiplier;

            /************************************************************************************************************************/

            /// <summary>Creates a new <see cref="TwinFloatField"/>.</summary>
            public TwinFloatField(SerializedProperty property, GUIContent label, bool isNormalized)
            {
                Property = property;
                Label = label;
                IsNormalized = isNormalized;
            }

            /************************************************************************************************************************/

            /// <summary>Draws a group of <see cref="TwinFloatField"/>s.</summary>
            public static void DoGroupGUI(SerializedProperty baseProperty, GUIContent label, TwinFloatField[] fields)
            {
                var height = (AnimancerGUI.LineHeight + AnimancerGUI.StandardSpacing) * (fields.Length + 1);
                var area = GUILayoutUtility.GetRect(0, height);

                area.height = AnimancerGUI.LineHeight;
                label = EditorGUI.BeginProperty(area, label, baseProperty);
                GUI.Label(area, label);
                EditorGUI.EndProperty();

                EditorGUI.indentLevel++;

                for (int i = 0; i < fields.Length; i++)
                {
                    AnimancerGUI.NextVerticalArea(ref area);
                    fields[i].DoTwinFloatFieldGUI(area);
                }

                EditorGUI.indentLevel--;
            }

            /************************************************************************************************************************/

            /// <summary>Draws this <see cref="TwinFloatField"/>.</summary>
            public void DoTwinFloatFieldGUI(Rect area)
            {
                var label = EditorGUI.BeginProperty(area, Label, Property);

                EditorGUI.BeginChangeCheck();

                var value = Property.floatValue;
                value = DoTwinFloatFieldGUI(area, label, value, normalizeMultiplier, IsNormalized);

                if (EditorGUI.EndChangeCheck())
                    Property.floatValue = value;

                EditorGUI.EndProperty();
            }

            /************************************************************************************************************************/

            private static ConversionCache<float, string> _NormalizedSuffixCache, _PixelSuffixCache;

            /// <summary>Draws two float fields.</summary>
            public static float DoTwinFloatFieldGUI(Rect area, GUIContent label, float value, float normalizeMultiplier, bool isNormalized)
            {
                if (_PixelSuffixCache == null)
                {
                    _PixelSuffixCache = new ConversionCache<float, string>((s) => s + "px");
                    _NormalizedSuffixCache = new ConversionCache<float, string>((x) => x + "x");
                }

                var split = (area.width - EditorGUIUtility.labelWidth - AnimancerGUI.StandardSpacing) * 0.5f;
                var normalizedArea = AnimancerGUI.StealFromRight(ref area, Mathf.Floor(split), AnimancerGUI.StandardSpacing);

                var pixels = isNormalized ? value / normalizeMultiplier : value;
                var normalized = isNormalized ? value : value * normalizeMultiplier;

                EditorGUI.BeginChangeCheck();
                pixels = AnimancerGUI.DoSpecialFloatField(area, label, pixels, _PixelSuffixCache);
                if (EditorGUI.EndChangeCheck())
                    value = isNormalized ? pixels * normalizeMultiplier : pixels;

                EditorGUI.BeginChangeCheck();
                normalized = AnimancerGUI.DoSpecialFloatField(normalizedArea, null, normalized, _NormalizedSuffixCache);
                if (EditorGUI.EndChangeCheck())
                    value = isNormalized ? normalized : normalized / normalizeMultiplier;

                return value;
            }

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Preview
        /************************************************************************************************************************/

        private static readonly Type
            DefaultEditorType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.SpriteInspector");

        private readonly Dictionary<Object, UnityEditor.Editor>
            TargetToDefaultEditor = new Dictionary<Object, UnityEditor.Editor>();

        /************************************************************************************************************************/

        private void InitialisePreview()
        {
            foreach (var target in targets)
            {
                if (!TargetToDefaultEditor.ContainsKey(target))
                {
                    var editor = CreateEditor(target, DefaultEditorType);
                    TargetToDefaultEditor.Add(target, editor);
                }
            }
        }

        /************************************************************************************************************************/

        private void CleanUpPreview()
        {
            foreach (var editor in TargetToDefaultEditor.Values)
                DestroyImmediate(editor);

            TargetToDefaultEditor.Clear();
        }

        /************************************************************************************************************************/

        private bool TryGetDefaultEditor(out UnityEditor.Editor editor)
            => TargetToDefaultEditor.TryGetValue(target, out editor);

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override string GetInfoString()
        {
            if (!TryGetDefaultEditor(out var editor))
                return null;

            return editor.GetInfoString();
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            if (!TryGetDefaultEditor(out var editor))
                return null;

            return editor.RenderStaticPreview(assetPath, subAssets, width, height);
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override bool HasPreviewGUI()
        {
            return TryGetDefaultEditor(out var editor) && editor.HasPreviewGUI();
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void OnPreviewGUI(Rect area, GUIStyle background)
        {
            if (TryGetDefaultEditor(out var editor))
                editor.OnPreviewGUI(area, background);

            var sprite = target as Sprite;
            if (sprite == null)
                return;

            EditorGUI.BeginChangeCheck();
            FitAspectRatio(ref area, sprite);
            DoPivotDotGUI(area, sprite);
            if (EditorGUI.EndChangeCheck())
                _HasBeenModified = true;
        }

        /************************************************************************************************************************/

        private static void FitAspectRatio(ref Rect area, Sprite sprite)
        {
            var areaAspect = area.width / area.height;
            var spriteAspect = sprite.rect.width / sprite.rect.height;
            if (areaAspect != spriteAspect)
            {
                if (areaAspect > spriteAspect)
                {
                    var width = area.height * spriteAspect;
                    area.x += (area.width - width) * 0.5f;
                    area.width = width;
                }
                else
                {
                    var height = area.width / spriteAspect;
                    area.y += (area.height - height) * 0.5f;
                    area.height = height;
                }
            }
        }

        /************************************************************************************************************************/

        private static readonly int PivotDotControlIDHint = "PivotDot".GetHashCode();

        private static GUIStyle _PivotDot;
        private static GUIStyle _PivotDotActive;

        private Vector2 _MouseDownPivot;

        private void DoPivotDotGUI(Rect area, Sprite sprite)
        {
            if (_PivotDot == null)
                _PivotDot = "U2D.pivotDot";
            if (_PivotDotActive == null)
                _PivotDotActive = "U2D.pivotDotActive";

            Vector2 pivot;
            if (_Pivot.hasMultipleDifferentValues)
            {
                pivot = sprite.pivot;
                pivot.x /= sprite.rect.width;
                pivot.y /= sprite.rect.height;
            }
            else
            {
                pivot = _Pivot.vector2Value;
            }
            pivot.x *= area.width;
            pivot.y *= area.height;

            var pivotArea = new Rect(
                area.x + pivot.x - _PivotDot.fixedWidth * 0.5f,
                area.yMax - pivot.y - _PivotDot.fixedHeight * 0.5f,
                _PivotDot.fixedWidth,
                _PivotDot.fixedHeight);

            var controlID = GUIUtility.GetControlID(PivotDotControlIDHint, FocusType.Keyboard);

            var currentEvent = Event.current;
            switch (currentEvent.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (currentEvent.button == 0 && pivotArea.Contains(Event.current.mousePosition) && !currentEvent.alt)
                    {
                        GUIUtility.hotControl = GUIUtility.keyboardControl = controlID;
                        _MouseDownPivot = pivot;
                        EditorGUIUtility.SetWantsMouseJumping(1);
                        currentEvent.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        pivot = Event.current.mousePosition;
                        pivot.x = InverseLerpUnclamped(area.x, area.xMax, pivot.x);
                        pivot.y = InverseLerpUnclamped(area.yMax, area.y, pivot.y);

                        if (currentEvent.control)
                        {
                            pivot.x = Mathf.Round(pivot.x * sprite.rect.width) / sprite.rect.width;
                            pivot.y = Mathf.Round(pivot.y * sprite.rect.height) / sprite.rect.height;
                        }

                        _Pivot.vector2Value = pivot;

                        GUI.changed = true;
                        currentEvent.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID && (currentEvent.button == 0 || currentEvent.button == 2))
                    {
                        EditorGUIUtility.SetWantsMouseJumping(0);
                        GUIUtility.hotControl = 0;
                        currentEvent.Use();
                    }
                    break;

                case EventType.KeyDown:
                    if (GUIUtility.hotControl == controlID && currentEvent.keyCode == KeyCode.Escape)
                    {
                        _Pivot.vector2Value = _MouseDownPivot;
                        GUIUtility.hotControl = 0;
                        GUI.changed = true;
                        currentEvent.Use();
                    }
                    break;

                case EventType.Repaint:
                    EditorGUIUtility.AddCursorRect(pivotArea, MouseCursor.Arrow, controlID);
                    var style = GUIUtility.hotControl == controlID ? _PivotDotActive : _PivotDot;
                    style.Draw(pivotArea, GUIContent.none, controlID);
                    break;
            }
        }

        /************************************************************************************************************************/

        /// <summary>The opposite of <see cref="Mathf.LerpUnclamped(float, float, float)"/>.</summary>
        public static float InverseLerpUnclamped(float a, float b, float value)
        {
            if (a == b)
                return 0;
            else
                return (value - a) / (b - a);
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}

#endif

