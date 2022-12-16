// -----------------------------------------------------------------------
// <copyright file="ScrollViewEditor.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace TapSDK.UI.AillieoTech
{
    using System;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEditor.UI;
    using UnityEngine;
    using UnityEngine.UI;

    [CustomEditor(typeof(ScrollView))]
    public class ScrollViewEditor : ScrollRectEditor
    {
        private const string bgPath = "UI/Skin/Background.psd";
        private const string spritePath = "UI/Skin/UISprite.psd";
        private const string maskPath = "UI/Skin/UIMask.psd";
        private static Color panelColor = new Color(1f, 1f, 1f, 0.392f);
        private static Color defaultSelectableColor = new Color(1f, 1f, 1f, 1f);
        private static Vector2 thinElementSize = new Vector2(160f, 20f);
        private static Action<GameObject, MenuCommand> PlaceUIElementRoot;

        private SerializedProperty itemTemplate;
        private SerializedProperty poolSize;
        private SerializedProperty defaultItemSize;
        private SerializedProperty layoutType;

        private GUIStyle cachedCaption;

        private GUIStyle caption
        {
            get
            {
                if (this.cachedCaption == null)
                {
                    this.cachedCaption = new GUIStyle { richText = true, alignment = TextAnchor.MiddleCenter };
                }

                return this.cachedCaption;
            }
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("<b>Additional configs</b>", this.caption);
            EditorGUILayout.Space();
            this.DrawConfigInfo();
            this.serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("<b>For original ScrollRect</b>", this.caption);
            EditorGUILayout.Space();
            base.OnInspectorGUI();
            EditorGUILayout.EndVertical();
        }

        protected static void InternalAddScrollView<T>(MenuCommand menuCommand)
            where T : ScrollView
        {
            GetPrivateMethodByReflection();

            GameObject root = CreateUIElementRoot(typeof(T).Name, new Vector2(200, 200));
            PlaceUIElementRoot?.Invoke(root, menuCommand);

            GameObject viewport = CreateUIObject("Viewport", root);
            GameObject content = CreateUIObject("Content", viewport);

            var parent = menuCommand.context as GameObject;
            if (parent != null)
            {
                root.transform.SetParent(parent.transform, false);
            }

            Selection.activeGameObject = root;

            GameObject hScrollbar = CreateScrollbar();
            hScrollbar.name = "Scrollbar Horizontal";
            hScrollbar.transform.SetParent(root.transform, false);
            RectTransform hScrollbarRT = hScrollbar.GetComponent<RectTransform>();
            hScrollbarRT.anchorMin = Vector2.zero;
            hScrollbarRT.anchorMax = Vector2.right;
            hScrollbarRT.pivot = Vector2.zero;
            hScrollbarRT.sizeDelta = new Vector2(0, hScrollbarRT.sizeDelta.y);

            GameObject vScrollbar = CreateScrollbar();
            vScrollbar.name = "Scrollbar Vertical";
            vScrollbar.transform.SetParent(root.transform, false);
            vScrollbar.GetComponent<Scrollbar>().SetDirection(Scrollbar.Direction.BottomToTop, true);
            RectTransform vScrollbarRT = vScrollbar.GetComponent<RectTransform>();
            vScrollbarRT.anchorMin = Vector2.right;
            vScrollbarRT.anchorMax = Vector2.one;
            vScrollbarRT.pivot = Vector2.one;
            vScrollbarRT.sizeDelta = new Vector2(vScrollbarRT.sizeDelta.x, 0);

            RectTransform viewportRect = viewport.GetComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.sizeDelta = Vector2.zero;
            viewportRect.pivot = Vector2.up;

            RectTransform contentRect = content.GetComponent<RectTransform>();
            contentRect.anchorMin = Vector2.up;
            contentRect.anchorMax = Vector2.one;
            contentRect.sizeDelta = new Vector2(0, 300);
            contentRect.pivot = Vector2.up;

            ScrollView scrollRect = root.AddComponent<T>();
            scrollRect.content = contentRect;
            scrollRect.viewport = viewportRect;
            scrollRect.horizontalScrollbar = hScrollbar.GetComponent<Scrollbar>();
            scrollRect.verticalScrollbar = vScrollbar.GetComponent<Scrollbar>();
            scrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.horizontalScrollbarSpacing = -3;
            scrollRect.verticalScrollbarSpacing = -3;

            Image rootImage = root.AddComponent<Image>();
            rootImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(bgPath);
            rootImage.type = Image.Type.Sliced;
            rootImage.color = panelColor;

            Mask viewportMask = viewport.AddComponent<Mask>();
            viewportMask.showMaskGraphic = false;

            Image viewportImage = viewport.AddComponent<Image>();
            viewportImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(maskPath);
            viewportImage.type = Image.Type.Sliced;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            this.itemTemplate = this.serializedObject.FindProperty("itemTemplate");
            this.poolSize = this.serializedObject.FindProperty("poolSize");
            this.defaultItemSize = this.serializedObject.FindProperty("defaultItemSize");
            this.layoutType = this.serializedObject.FindProperty("layoutType");
        }

        protected virtual void DrawConfigInfo()
        {
            EditorGUILayout.PropertyField(this.itemTemplate);
            EditorGUILayout.PropertyField(this.poolSize);
            EditorGUILayout.PropertyField(this.defaultItemSize);
            this.layoutType.intValue = (int)(ScrollView.ItemLayoutType)EditorGUILayout.EnumPopup("layoutType", (ScrollView.ItemLayoutType)this.layoutType.intValue);
        }

        [MenuItem("GameObject/UI/DynamicScrollView", false, 90)]
        private static void AddScrollView(MenuCommand menuCommand)
        {
            InternalAddScrollView<ScrollView>(menuCommand);
        }

        private static GameObject CreateScrollbar()
        {
            // Create GOs Hierarchy
            GameObject scrollbarRoot = CreateUIElementRoot("Scrollbar", thinElementSize);
            GameObject sliderArea = CreateUIObject("Sliding Area", scrollbarRoot);
            GameObject handle = CreateUIObject("Handle", sliderArea);

            Image bgImage = scrollbarRoot.AddComponent<Image>();
            bgImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(bgPath);
            bgImage.type = Image.Type.Sliced;
            bgImage.color = defaultSelectableColor;

            Image handleImage = handle.AddComponent<Image>();
            handleImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(spritePath);
            handleImage.type = Image.Type.Sliced;
            handleImage.color = defaultSelectableColor;

            RectTransform sliderAreaRect = sliderArea.GetComponent<RectTransform>();
            sliderAreaRect.sizeDelta = new Vector2(-20, -20);
            sliderAreaRect.anchorMin = Vector2.zero;
            sliderAreaRect.anchorMax = Vector2.one;

            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 20);

            Scrollbar scrollbar = scrollbarRoot.AddComponent<Scrollbar>();
            scrollbar.handleRect = handleRect;
            scrollbar.targetGraphic = handleImage;
            SetDefaultColorTransitionValues(scrollbar);

            return scrollbarRoot;
        }

        private static GameObject CreateUIElementRoot(string name, Vector2 size)
        {
            var child = new GameObject(name);
            RectTransform rectTransform = child.AddComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            return child;
        }

        private static GameObject CreateUIObject(string name, GameObject parent)
        {
            var go = new GameObject(name);
            go.AddComponent<RectTransform>();
            SetParentAndAlign(go, parent);
            return go;
        }

        private static void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (parent == null)
            {
                return;
            }

            child.transform.SetParent(parent.transform, false);
            SetLayerRecursively(child, parent.layer);
        }

        private static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform t = go.transform;
            for (var i = 0; i < t.childCount; i++)
            {
                SetLayerRecursively(t.GetChild(i).gameObject, layer);
            }
        }

        private static void SetDefaultColorTransitionValues(Selectable slider)
        {
            ColorBlock colors = slider.colors;
            colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
            colors.pressedColor = new Color(0.698f, 0.698f, 0.698f);
            colors.disabledColor = new Color(0.521f, 0.521f, 0.521f);
        }

        private static void GetPrivateMethodByReflection()
        {
            if (PlaceUIElementRoot == null)
            {
                Assembly uiEditorAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(asm => asm.GetName().Name == "UnityEditor.UI");
                if (uiEditorAssembly != null)
                {
                    Type menuOptionType = uiEditorAssembly.GetType("UnityEditor.UI.MenuOptions");
                    if (menuOptionType != null)
                    {
                        MethodInfo miPlaceUIElementRoot = menuOptionType.GetMethod(
                            "PlaceUIElementRoot",
                            BindingFlags.NonPublic | BindingFlags.Static);
                        if (miPlaceUIElementRoot != null)
                        {
                            PlaceUIElementRoot = Delegate.CreateDelegate(
                                    typeof(Action<GameObject, MenuCommand>),
                                    miPlaceUIElementRoot)
                                as Action<GameObject, MenuCommand>;
                        }
                    }
                }
            }
        }
    }
}
