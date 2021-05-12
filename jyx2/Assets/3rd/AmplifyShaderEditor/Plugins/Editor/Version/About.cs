// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public class About : EditorWindow
	{
		private const string AboutImageGUID = "8aba6bb20faf8824d9d81946542f1ce1";
		private Vector2 m_scrollPosition = Vector2.zero;
		private Texture2D m_aboutImage;

		[MenuItem( "Window/Amplify Shader Editor/About...", false, 2001 )]
		static void Init()
		{
			About window = (About)GetWindow( typeof( About ), true, "About Amplify Shader Editor" );
			window.minSize = new Vector2( 502, 290 );
			window.maxSize = new Vector2( 502, 290 );
			window.Show();
		}

		[MenuItem( "Window/Amplify Shader Editor/Manual", false, 2000 )]
		static void OpenManual()
		{
			Application.OpenURL( "http://wiki.amplify.pt/index.php?title=Unity_Products:Amplify_Shader_Editor/Manual" );
		}

		private void OnEnable()
		{
			m_aboutImage = AssetDatabase.LoadAssetAtPath<Texture2D>( AssetDatabase.GUIDToAssetPath( AboutImageGUID ) );
		}

		public void OnGUI()
		{
			m_scrollPosition = GUILayout.BeginScrollView( m_scrollPosition );

			GUILayout.BeginVertical();

			GUILayout.Space( 10 );

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Box( m_aboutImage, GUIStyle.none );

			if( Event.current.type == EventType.MouseUp && GUILayoutUtility.GetLastRect().Contains( Event.current.mousePosition ) )
				Application.OpenURL( "http://www.amplify.pt" );

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUIStyle labelStyle = new GUIStyle( EditorStyles.label );
			labelStyle.alignment = TextAnchor.MiddleCenter;
			labelStyle.wordWrap = true;

			GUILayout.Label( "\nAmplify Shader Editor " + VersionInfo.StaticToString(), labelStyle, GUILayout.ExpandWidth( true ) );

			GUILayout.Label( "\nCopyright (c) Amplify Creations, Lda. All rights reserved.\n", labelStyle, GUILayout.ExpandWidth( true ) );

			GUILayout.EndVertical();

			GUILayout.EndScrollView();
		}
	}
}
