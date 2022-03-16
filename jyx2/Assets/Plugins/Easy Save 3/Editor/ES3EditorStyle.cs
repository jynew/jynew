using System.Collections;
using UnityEngine;
using UnityEditor;

namespace ES3Editor
{
	public class EditorStyle
	{
		private static EditorStyle style = null;

		public GUIStyle area;
        public GUIStyle areaPadded;

		public GUIStyle menuButton;
		public GUIStyle menuButtonSelected;
		public GUIStyle smallSquareButton;

		public GUIStyle heading;
		public GUIStyle subheading;
		public GUIStyle subheading2;

		public GUIStyle boldLabelNoStretch;

		public GUIStyle link;

		public GUIStyle toggle;

        public Texture2D saveIconSelected;
        public Texture2D saveIconUnselected;

		public static EditorStyle Get { get{ if(style == null) style = new EditorStyle(); return style; } }

		public EditorStyle()
		{
			// An area with padding.
			area = new GUIStyle();
			area.padding = new RectOffset(10, 10, 10, 10);
            area.wordWrap = true;

            // An area with more padding.
            areaPadded = new GUIStyle();
            areaPadded.padding = new RectOffset(20, 20, 20, 20);
            areaPadded.wordWrap = true;

            // Unselected menu button.
            menuButton = new GUIStyle(EditorStyles.toolbarButton);
			menuButton.fontStyle = FontStyle.Normal;
			menuButton.fontSize = 14;
			menuButton.fixedHeight = 24;

			// Selected menu button.
			menuButtonSelected = new GUIStyle(menuButton);
			menuButtonSelected.fontStyle = FontStyle.Bold;

			// Main Headings
			heading = new GUIStyle(EditorStyles.label);
			heading.fontStyle = FontStyle.Bold;
			heading.fontSize = 24;

			subheading = new GUIStyle(heading);
			subheading.fontSize = 18;

			subheading2 = new GUIStyle(heading);
			subheading2.fontSize = 14;

			boldLabelNoStretch = new GUIStyle(EditorStyles.label);
			boldLabelNoStretch.stretchWidth = false;
			boldLabelNoStretch.fontStyle = FontStyle.Bold;

			link = new GUIStyle();
			link.fontSize = 16;
			if(EditorGUIUtility.isProSkin)
				link.normal.textColor = new Color (0.262f, 0.670f, 0.788f);
			else
				link.normal.textColor = new Color (0.129f, 0.129f, 0.8f);

			toggle = new GUIStyle(EditorStyles.toggle);
			toggle.stretchWidth = false;

            saveIconSelected = AssetDatabase.LoadAssetAtPath<Texture2D>(ES3Settings.PathToEasySaveFolder() + "Editor/es3Logo16x16.png");
            saveIconUnselected = AssetDatabase.LoadAssetAtPath<Texture2D>(ES3Settings.PathToEasySaveFolder() + "Editor/es3Logo16x16-bw.png");
        }
	}
}