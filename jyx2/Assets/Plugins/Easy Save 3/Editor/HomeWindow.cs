using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ES3Editor
{
	public class HomeWindow : SubWindow
	{
		Vector2 scrollPos = Vector2.zero;

		public HomeWindow(EditorWindow window) : base("Home", window){}

		public override void OnGUI()
		{

			var style = EditorStyle.Get;

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

			EditorGUILayout.BeginVertical(style.area);

			GUILayout.Label("Welcome to Easy Save", style.heading);

			EditorGUILayout.BeginVertical(style.area);
			GUILayout.Label("New To Easy Save?", style.subheading);
			EditorGUILayout.BeginVertical(style.area);
			ES3EditorUtility.DisplayLink("• See our Getting Started guide", "http://docs.moodkie.com/easy-save-3/getting-started/");
			EditorGUILayout.EndVertical();

			GUILayout.Label("Support", style.subheading);

			EditorGUILayout.BeginVertical(style.area);

			ES3EditorUtility.DisplayLink("• Contact us directly", "http://www.moodkie.com/contact/");
			ES3EditorUtility.DisplayLink("• Ask a question in our Easy Save 3 forums", "http://moodkie.com/forum/viewforum.php?f=12");
			ES3EditorUtility.DisplayLink("• Ask a question in the Unity Forum thread","https://forum.unity3d.com/threads/easy-save-the-complete-save-load-asset-for-unity.91040/");
			EditorGUILayout.EndVertical();

			GUILayout.Label("Documentation and Guides", style.subheading);

			EditorGUILayout.BeginVertical(style.area);

			ES3EditorUtility.DisplayLink("• Documentation", "http://docs.moodkie.com/product/easy-save-3/");
			ES3EditorUtility.DisplayLink("• Guides", "http://docs.moodkie.com/product/easy-save-3/es3-guides/");
			ES3EditorUtility.DisplayLink("• API Scripting Reference", "http://docs.moodkie.com/product/easy-save-3/es3-api/");
			ES3EditorUtility.DisplayLink("• Supported Types", "http://docs.moodkie.com/easy-save-3/es3-supported-types/");


			EditorGUILayout.EndVertical();

			GUILayout.Label("PlayMaker Documentation", style.subheading);

			EditorGUILayout.BeginVertical(style.area);

			ES3EditorUtility.DisplayLink("• Actions", "http://docs.moodkie.com/product/easy-save-3/es3-playmaker/es3-playmaker-actions/");
			ES3EditorUtility.DisplayLink("• Actions Overview", "http://docs.moodkie.com/easy-save-3/es3-playmaker/playmaker-actions-overview/");


			EditorGUILayout.EndVertical();

			EditorGUILayout.EndVertical();

			EditorGUILayout.EndVertical();

			EditorGUILayout.EndScrollView();

		}
	}
}
