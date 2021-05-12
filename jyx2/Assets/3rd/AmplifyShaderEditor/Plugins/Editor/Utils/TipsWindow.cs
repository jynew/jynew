// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System.IO;
using System.Reflection;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class TipsWindow : MenuParent
	{
		private static bool m_showWindow = false;
		private bool m_dontShowAtStart = false;

		private static List<string> AllTips = new List<string>() {
			"You can press W to toggle between a flat and color coded Wires and ports.",
			"You can press CTRL+W to toggle between multiline or singleline Wire connections.",
			"You can press P to globally open all node Previews.",
			"You can press F to Focus your selection, single tap centers the selection while double tap it to also zooms on in.",
			"You can press CTRL+F to open a search bar and Find a node by it's title",
			"You can press SPACE to open a context menu to add a new node and press TAB or SHIFT+TAB tocycle between the found nodes",
			"You can remove a node without breaking the graph connections by pressing ALT and then dragging the node out",
			"You can switch two input connections holding CTRL while dragging one input connection into the other",
		};

		int m_currentTip = 0;

		public TipsWindow( AmplifyShaderEditorWindow parentWindow ) : base( parentWindow, 0, 0, 0, 64, "Tips", MenuAnchor.TOP_LEFT, MenuAutoSize.NONE )
		{
			//m_dontShowAtStart = EditorPrefs.GetBool( "DontShowTipAtStart", false );
		}

		public override void Draw( Rect parentPosition, Vector2 mousePosition, int mouseButtonId, bool hasKeyboadFocus )
		{
			base.Draw( parentPosition, mousePosition, mouseButtonId, hasKeyboadFocus );

			DrawWindow( mousePosition );
		}

		public void DrawWindow( Vector2 mousePosition )
		{
			if( !m_showWindow )
				return;

			Rect windowRect = new Rect( 0, 0, Screen.width, Screen.height );
			Vector2 center = windowRect.center;
			windowRect.size = new Vector2( 300, 200 );
			windowRect.center = center;
			Color temp = GUI.color;
			GUI.color = Color.white;
			GUI.Label( windowRect, string.Empty, GUI.skin.FindStyle( "flow node 0" ) );

			if( Event.current.type == EventType.MouseDown && !windowRect.Contains( mousePosition ) )
				m_showWindow = false;

			Rect titleRect = windowRect;
			titleRect.height = 35;
			GUI.Label( titleRect, "Quick Tip!", GUI.skin.FindStyle( "TL Selection H2" ) );
			Rect button = titleRect;
			button.size = new Vector2( 14, 14 );
			button.y += 2;
			button.x = titleRect.xMax - 16;
			if( GUI.Button( button, string.Empty, GUI.skin.FindStyle( "WinBtnClose" ) ) )
				CloseWindow();

			button.y += 100;
			if( GUI.Button( button, ">" ) )
			{
				m_currentTip++;
				if( m_currentTip >= AllTips.Count )
					m_currentTip = 0;
			}
			
			Rect textRect = windowRect;
			textRect.yMin = titleRect.yMax;
			GUI.Label( textRect, AllTips[ m_currentTip ], GUI.skin.FindStyle( "WordWrappedLabel" ) );

			Rect footerRect = windowRect;
			footerRect.yMin = footerRect.yMax - 18;
			footerRect.x += 3;
			GUI.Label( footerRect, (m_currentTip + 1) + " of " + AllTips.Count + " tips" );
			footerRect.x += 170;
			EditorGUI.BeginChangeCheck();
			m_dontShowAtStart = GUI.Toggle( footerRect, m_dontShowAtStart, "Don't show at start" );
			if( EditorGUI.EndChangeCheck() )
			{
				EditorPrefs.SetBool( "DontShowTipAtStart", m_dontShowAtStart );
			}
			GUI.color = temp;

			if( Event.current.type == EventType.MouseDown && windowRect.Contains( mousePosition ) )
			{
				Event.current.Use();
				ParentWindow.MouseInteracted = true;
			}
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		public static void ShowWindow( bool toggle = true )
		{
			if( toggle )
				m_showWindow = !m_showWindow;
			else
				m_showWindow = true;

			//Test();
			//ExportCompiledShaders();
		}

		//public static void Test()
		//{
		//	Shader shader = UIUtils.CurrentWindow.CurrentGraph.CurrentShader;
		//	int mode = EditorPrefs.GetInt( "ShaderInspectorPlatformMode", 1 );
		//	int mask = EditorPrefs.GetInt( "ShaderInspectorPlatformMask", 524287 );
		//	bool strip = EditorPrefs.GetInt( "ShaderInspectorVariantStripping", 1 ) == 0;
		//	ShaderUtilEx.OpenCompiledShader( shader, mode, mask, strip );
		//}

		//public static void ExportCompiledShaders()
		//{
		//	Shader shader = UIUtils.CurrentWindow.CurrentGraph.CurrentShader;
		//	string shaderPath = AssetDatabase.GetAssetPath( shader );
		//	SerializedObject so = new SerializedObject( shader );
		//	SerializedProperty prop = so.FindProperty( "m_Script" );
		//	var compiledShaderString = prop.stringValue;
		//	Directory.CreateDirectory( Application.dataPath + "/../ShaderSource/" );
		//	if( compiledShaderString == null )
		//		return;
		//	var outputPath = Application.dataPath + "/../ShaderSource/" + Path.GetFileNameWithoutExtension( shaderPath ) + "_compiled.shader";
		//	var sw = File.CreateText( outputPath );
		//	sw.Write( compiledShaderString );
		//	sw.Close();
		//}

		public static void CloseWindow()
		{
			m_showWindow = false;
		}
	}
}
