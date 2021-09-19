using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GPUInstancer
{
    public class GPUInstancerMultiAddWindow : EditorWindow
    {
        private static GPUInstancerManagerEditor _managerEditor;

        private Texture2D dropIcon;

        public static void ShowWindow(Vector2 position, GPUInstancerManagerEditor managerEditor)
        {
            EditorWindow window = EditorWindow.GetWindow(typeof(GPUInstancerMultiAddWindow), true, "GPUI Multiple Add", true);
            window.minSize = new Vector2(400, 200);
            window.maxSize = new Vector2(400, 200);
            _managerEditor = managerEditor;
            ActiveEditorTracker.sharedTracker.isLocked = true;
        }

        void OnGUI()
        {
            if (_managerEditor == null || _managerEditor.GetManager() == null)
            {
                Close();
                return;
            }

            if (dropIcon == null)
                dropIcon = Resources.Load<Texture2D>(GPUInstancerConstants.EDITOR_TEXTURES_PATH +
#if UNITY_PRO_LICENSE
                GPUInstancerEditorConstants.DROP_ICON_PRO
#else
                GPUInstancerEditorConstants.DROP_ICON
#endif
                    );

            Rect buttonRect = GUILayoutUtility.GetRect(360, 180, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GPUInstancerEditorConstants.DrawColoredButton(new GUIContent("<size=18>Drop Files Here</size>", dropIcon), Color.clear,
#if UNITY_PRO_LICENSE
                Color.white,
#else
                GPUInstancerEditorConstants.Colors.dimgray,
#endif
                FontStyle.Bold, buttonRect,
                null,
                true, true,
                (o) =>
                {
                    _managerEditor.AddPickerObject(o);
                });
        }

        private void OnDisable()
        {
            ActiveEditorTracker.sharedTracker.isLocked = false;
            if (_managerEditor != null && _managerEditor.GetManager() != null)
                Selection.activeGameObject = _managerEditor.GetManager().gameObject;
        }
    }
}