// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

//#define ASE_CONSOLE_WINDOW

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	public sealed class DebugConsoleWindow : EditorWindow
	{
		private const float WindowSizeX = 250;
		private const float WindowSizeY = 250;
		private const float WindowPosX = 5;
		private const float WindowPosY = 5;
		private Rect m_availableArea;

		private bool m_wikiAreaFoldout = true;
		private bool m_miscAreaFoldout = true;
		private Vector2 m_currentScrollPos;

#if ASE_CONSOLE_WINDOW
		public readonly static bool DeveloperMode = true;
		public static bool UseShaderPanelsInfo = true;
		[MenuItem( "Window/Amplify Shader Editor/Open Debug Console" )]
		static void OpenMainShaderGraph()
		{
			OpenWindow();
		}
		[MenuItem( "Window/Amplify Shader Editor/Create Template Menu Items" )]
		public static void CreateTemplateMenuItems()
		{
			UIUtils.CurrentWindow.TemplatesManagerInstance.CreateTemplateMenuItems();
		}

#else
		public readonly static bool DeveloperMode = false;
		public static bool UseShaderPanelsInfo = false;
#endif

		public static DebugConsoleWindow OpenWindow()
		{
			if ( DeveloperMode )
			{
				DebugConsoleWindow currentWindow = ( DebugConsoleWindow ) DebugConsoleWindow.GetWindow( typeof( DebugConsoleWindow ), false, "ASE Debug Console" );
				currentWindow.titleContent.tooltip = "Debug Options for ASE. Intented only for ASE development team";
				currentWindow.minSize = new Vector2( WindowSizeX, WindowSizeY );
				currentWindow.maxSize = new Vector2( WindowSizeX, 2 * WindowSizeY ); ;
				currentWindow.wantsMouseMove = true;
				return currentWindow;
			}
			return null;
		}

		void OnGUI()
		{
			m_availableArea = new Rect( WindowPosX, WindowPosY, position.width - 2 * WindowPosX, position.height - 2 * WindowPosY );
			GUILayout.BeginArea( m_availableArea );
			{
				m_currentScrollPos = EditorGUILayout.BeginScrollView( m_currentScrollPos, GUILayout.Width( 0 ), GUILayout.Height( 0 ) );
				{
					EditorGUILayout.BeginVertical();
					{
						AmplifyShaderEditorWindow window = UIUtils.CurrentWindow;
						if ( window != null )
						{
							EditorGUILayout.Separator();

							NodeUtils.DrawPropertyGroup( ref m_wikiAreaFoldout, "Wiki Helper", ShowWikiHelperFunctions );

							EditorGUILayout.Separator();

							NodeUtils.DrawPropertyGroup( ref m_miscAreaFoldout, "Misc", ShowMiscFuntions );

							EditorGUILayout.Separator();
						}
						else
						{
							EditorGUILayout.LabelField( "Please open an ASE window to access debug options" );
						}
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndScrollView();
			}
			GUILayout.EndArea();
		}

		void ShowWikiHelperFunctions()
		{
			AmplifyShaderEditorWindow window = UIUtils.CurrentWindow;
			EditorGUILayout.Separator();

			if ( GUILayout.Button( "Nodes Screen Shots" ) )
			{
				window.CurrentNodeExporterUtils.ActivateAutoScreenShot( Application.dataPath + "/../NodesInfo/Shots/" );
			}

			EditorGUILayout.Separator();

			if( GUILayout.Button( "Nodes Undo Test" ) )
			{
				window.CurrentNodeExporterUtils.ActivateAutoUndo();
			}

			EditorGUILayout.Separator();

			if ( GUILayout.Button( "Nodes Info" ) )
			{
				window.CurrentPaletteWindow.DumpAvailableNodes( false, Application.dataPath + "/../NodesInfo/" );
				window.CurrentPaletteWindow.DumpAvailableNodes( true, Application.dataPath + "/../NodesInfo/" );
			}

			EditorGUILayout.Separator();

			if ( GUILayout.Button( "Shortcuts Info" ) )
			{
				window.ShortcutManagerInstance.DumpShortcutsToDisk( Application.dataPath + "/../NodesInfo/" );
			}
		}

		void ShowMiscFuntions()
		{
			AmplifyShaderEditorWindow window = UIUtils.CurrentWindow;
			if ( GUILayout.Button( "Force Example Shader Compilation" ) )
			{
				UIUtils.ForceExampleShaderCompilation();
			}
			EditorGUILayout.Separator();

			if ( GUILayout.Button( "Refresh Available Nodes" ) )
			{
				window.RefreshAvaibleNodes();
			}

			EditorGUILayout.Separator();

			if ( GUILayout.Button( "Dump Uniform Names" ) )
			{
				//window.CurrentPaletteWindow.NewList()
				window.DuplicatePrevBufferInstance.DumpUniformNames();
			}

			EditorGUILayout.Separator();

			if ( GUILayout.Button( "Force Palette Update" ) )
			{
				Debug.Log( UIUtils.CurrentWindow.IsShaderFunctionWindow );
				window.CurrentPaletteWindow.ForceUpdate = true;
			}

			EditorGUILayout.Separator();

			if( GUILayout.Button( "Detect Infinite Loops" ) )
			{
				if( window.IsShaderFunctionWindow )
				{
					Debug.Log( "Starting infinite loop detection over shader functions" );
					List<FunctionOutput> nodes = window.OutsideGraph.FunctionOutputNodes.NodesList;
					for( int i = 0; i < nodes.Count; i++ )
					{
						UIUtils.DetectNodeLoopsFrom( nodes[ i ], new Dictionary<int, int>() );
					}
				}
				else
				{
					if( window.OutsideGraph.MultiPassMasterNodes.Count > 0 )
					{
						Debug.Log( "Starting infinite loop detection over shader from template" );
						List<TemplateMultiPassMasterNode> nodes = window.OutsideGraph.MultiPassMasterNodes.NodesList;
						for( int i = 0; i < nodes.Count; i++ )
						{
							UIUtils.DetectNodeLoopsFrom( nodes[ i ], new Dictionary<int, int>() );
						}
					}
					else
					{
						Debug.Log( "Starting infinite loop detection over standard shader" );
						UIUtils.DetectNodeLoopsFrom( window.OutsideGraph.CurrentMasterNode, new Dictionary<int, int>() );
					}
				}
				Debug.Log( "End infinite loop detection" );
			}
		}
	}
}



