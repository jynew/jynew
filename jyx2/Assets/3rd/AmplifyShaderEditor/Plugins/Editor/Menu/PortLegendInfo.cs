// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	class NodeDescriptionInfo
	{
		public bool FoldoutValue;
		public string Category;
		public string[,] Contents;
	}

	public sealed class PortLegendInfo : EditorWindow
	{
		private const string NoASEWindowWarning = "Please Open the ASE to get access to shortcut info";
		private const float PixelSeparator = 5;
		private const string EditorShortcutsTitle = "Editor Shortcuts";
		private const string MenuShortcutsTitle = "Menu Shortcuts";
		private const string NodesShortcutsTitle = "Nodes Shortcuts";
		private const string PortShortcutsTitle = "Port Shortcuts";
		private const string PortLegendTitle = "Port Legend";
		private const string NodesDescTitle = "Node Info";
		private const string CompatibleAssetsTitle = "Compatible Assets";

		private const string KeyboardUsageTemplate = "[{0}] - {1}";
		private const string m_lockedStr = "Locked Port";

		private const float WindowSizeX = 350;
		private const float WindowSizeY = 300;
		private const float WindowPosX = 5;
		private const float WindowPosY = 5;

		private int TitleLabelWidth = 150;
		private Rect m_availableArea;

		private bool m_portAreaFoldout = true;
		private bool m_editorShortcutAreaFoldout = true;
		private bool m_menuShortcutAreaFoldout = true;
		private bool m_nodesShortcutAreaFoldout = true;
		private bool m_nodesDescriptionAreaFoldout = true;
		private bool m_compatibleAssetsFoldout = true;

		private Vector2 m_currentScrollPos;

		private GUIStyle m_portStyle;
		private GUIStyle m_labelStyleBold;
		private GUIStyle m_labelStyle;

		private GUIStyle m_nodeInfoLabelStyleBold;
		private GUIStyle m_nodeInfoLabelStyle;

		private GUIStyle m_nodeInfoFoldoutStyle;

		private GUIContent m_content = new GUIContent( "Helper", "Shows helper info for ASE users" );
		private bool m_init = true;

		private List<ShortcutItem> m_editorShortcuts = null;
		private List<ShortcutItem> m_nodesShortcuts = null;
		private List<NodeDescriptionInfo> m_nodeDescriptionsInfo = null;
		private List<string[]> m_compatibleAssetsInfo = null;

		public static PortLegendInfo OpenWindow()
		{
			PortLegendInfo currentWindow = ( PortLegendInfo ) PortLegendInfo.GetWindow( typeof( PortLegendInfo ), false );
			currentWindow.minSize = new Vector2( WindowSizeX, WindowSizeY );
			currentWindow.maxSize = new Vector2( WindowSizeX * 2, 2 * WindowSizeY ); ;
			currentWindow.wantsMouseMove = true;
			return currentWindow;
		}

		public void Init()
		{
			m_init = false;
			wantsMouseMove = false;
			titleContent = m_content;
			m_portStyle = new GUIStyle( UIUtils.GetCustomStyle( CustomStyle.PortEmptyIcon ) );
			m_portStyle.alignment = TextAnchor.MiddleLeft;
			m_portStyle.imagePosition = ImagePosition.ImageOnly;
			m_portStyle.margin = new RectOffset( 5, 0, 5, 0 );

			m_labelStyleBold = new GUIStyle( UIUtils.InputPortLabel );
			m_labelStyleBold.fontStyle = FontStyle.Bold;
			m_labelStyleBold.fontSize = ( int ) ( Constants.TextFieldFontSize );


			m_labelStyle = new GUIStyle( UIUtils.InputPortLabel );
			m_labelStyle.clipping = TextClipping.Overflow;
			m_labelStyle.imagePosition = ImagePosition.TextOnly;
			m_labelStyle.contentOffset = new Vector2( -10, 0 );
			m_labelStyle.fontSize = ( int ) ( Constants.TextFieldFontSize );

			m_nodeInfoLabelStyleBold = new GUIStyle( UIUtils.InputPortLabel );
			m_nodeInfoLabelStyleBold.fontStyle = FontStyle.Bold;
			m_nodeInfoLabelStyleBold.fontSize = ( int ) ( Constants.TextFieldFontSize );

			m_nodeInfoLabelStyle = new GUIStyle( UIUtils.InputPortLabel );
			m_nodeInfoLabelStyle.clipping = TextClipping.Clip;
			m_nodeInfoLabelStyle.imagePosition = ImagePosition.TextOnly;
			m_nodeInfoLabelStyle.fontSize = ( int ) ( Constants.TextFieldFontSize );
			

			m_nodeInfoFoldoutStyle = new GUIStyle( ( GUIStyle ) "foldout" );
			m_nodeInfoFoldoutStyle.fontStyle = FontStyle.Bold;

			if ( !EditorGUIUtility.isProSkin )
			{
				m_labelStyleBold.normal.textColor = m_labelStyle.normal.textColor = Color.black;
			}

			m_availableArea = new Rect( WindowPosX, WindowPosY, WindowSizeX - 2 * WindowPosX, WindowSizeY - 2 * WindowPosY );
		}

		void DrawPort( WirePortDataType type )
		{
			EditorGUILayout.BeginHorizontal();
			{
				GUI.color = UIUtils.GetColorForDataType( type, false );
				GUILayout.Box( string.Empty, m_portStyle, GUILayout.Width( UIUtils.PortsSize.x ), GUILayout.Height( UIUtils.PortsSize.y ) );
				GUI.color = Color.white;
				EditorGUILayout.LabelField( UIUtils.GetNameForDataType( type ), m_labelStyle );
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Separator();
		}

		void OnGUI()
		{
			if ( !UIUtils.Initialized || UIUtils.CurrentWindow == null )
			{
				EditorGUILayout.LabelField( NoASEWindowWarning );
				return;
			}

			if ( m_init )
			{
				Init();
			}

			TitleLabelWidth = (int)(this.position.width * 0.42f);

			KeyCode key = Event.current.keyCode;
			if ( key == ShortcutsManager.ScrollUpKey )
			{
				m_currentScrollPos.y -= 10;
				if ( m_currentScrollPos.y < 0 )
				{
					m_currentScrollPos.y = 0;
				}
				Event.current.Use();
			}

			if ( key == ShortcutsManager.ScrollDownKey )
			{
				m_currentScrollPos.y += 10;
				Event.current.Use();
			}

			if ( Event.current.type == EventType.MouseDrag && Event.current.button > 0 )
			{
				m_currentScrollPos.x += Constants.MenuDragSpeed * Event.current.delta.x;
				if ( m_currentScrollPos.x < 0 )
				{
					m_currentScrollPos.x = 0;
				}

				m_currentScrollPos.y += Constants.MenuDragSpeed * Event.current.delta.y;
				if ( m_currentScrollPos.y < 0 )
				{
					m_currentScrollPos.y = 0;
				}
			}

			m_availableArea = new Rect( WindowPosX, WindowPosY, position.width - 2 * WindowPosX, position.height - 2 * WindowPosY );
			GUILayout.BeginArea( m_availableArea );
			{
				if ( GUILayout.Button( "Wiki Page" ) )
				{
					Application.OpenURL( Constants.HelpURL );
				}

				m_currentScrollPos = GUILayout.BeginScrollView( m_currentScrollPos );
				{
					EditorGUILayout.BeginVertical();
					{
						NodeUtils.DrawPropertyGroup( ref m_portAreaFoldout, PortLegendTitle, DrawPortInfo );
						float currLabelWidth = EditorGUIUtility.labelWidth;
						EditorGUIUtility.labelWidth = 1;
						NodeUtils.DrawPropertyGroup( ref m_editorShortcutAreaFoldout, EditorShortcutsTitle, DrawEditorShortcuts );
						NodeUtils.DrawPropertyGroup( ref m_menuShortcutAreaFoldout, MenuShortcutsTitle, DrawMenuShortcuts );
						NodeUtils.DrawPropertyGroup( ref m_nodesShortcutAreaFoldout, NodesShortcutsTitle, DrawNodesShortcuts );
						NodeUtils.DrawPropertyGroup( ref m_compatibleAssetsFoldout, CompatibleAssetsTitle, DrawCompatibleAssets );
						NodeUtils.DrawPropertyGroup( ref m_nodesDescriptionAreaFoldout, NodesDescTitle, DrawNodeDescriptions );
						EditorGUIUtility.labelWidth = currLabelWidth;
					}
					EditorGUILayout.EndVertical();
				}
				GUILayout.EndScrollView();
			}
			GUILayout.EndArea();

		}

		void DrawPortInfo()
		{
			Color originalColor = GUI.color;

			DrawPort( WirePortDataType.OBJECT );
			DrawPort( WirePortDataType.INT );
			DrawPort( WirePortDataType.FLOAT );
			DrawPort( WirePortDataType.FLOAT2 );
			DrawPort( WirePortDataType.FLOAT3 );
			DrawPort( WirePortDataType.FLOAT4 );
			DrawPort( WirePortDataType.COLOR );
			DrawPort( WirePortDataType.SAMPLER2D );
			DrawPort( WirePortDataType.FLOAT3x3 );
			DrawPort( WirePortDataType.FLOAT4x4 );

			EditorGUILayout.BeginHorizontal();
			{
				GUI.color = Constants.LockedPortColor;
				GUILayout.Box( string.Empty, m_portStyle, GUILayout.Width( UIUtils.PortsSize.x ), GUILayout.Height( UIUtils.PortsSize.y ) );
				GUI.color = Color.white;
				EditorGUILayout.LabelField( m_lockedStr, m_labelStyle );
			}
			EditorGUILayout.EndHorizontal();

			GUI.color = originalColor;
		}

		public void DrawEditorShortcuts()
		{
			AmplifyShaderEditorWindow window = UIUtils.CurrentWindow;
			if ( window != null )
			{
				if ( m_editorShortcuts == null )
				{
					m_editorShortcuts = window.ShortcutManagerInstance.AvailableEditorShortcutsList;
				}

				EditorGUI.indentLevel--;
				int count = m_editorShortcuts.Count;
				for ( int i = 0; i < count; i++ )
				{
					DrawItem( m_editorShortcuts[ i ].Name, m_editorShortcuts[ i ].Description );
				}
				DrawItem( "Ctrl + F", "Find nodes" );
				DrawItem( "LMB Drag", "Box selection" );
				DrawItem( "MMB/RMB Drag", "Camera pan" );
				DrawItem( "Alt + MMB/RMB Drag", "Zoom graph" );
				DrawItem( "Shift/Ctrl + Node Select", "Add/Remove from selection" );
				DrawItem( "Shift + Node Drag", "Node move with offset" );
				DrawItem( "Ctrl + Node Drag", "Node move with snap" );
				DrawItem( "MMB/RMB + Drag Panel", "Scroll panel" );
				DrawItem( "Alt + LMB Drag", "Additive box selection" );
				DrawItem( "Alt + Shift + Drag", "Subtractive box selection" );
				DrawItem( "Alt + Node Drag", "Auto-(Dis)Connect node on existing wire connection" );
				EditorGUI.indentLevel++;

			}
			else
			{
				EditorGUILayout.LabelField( NoASEWindowWarning );
			}
		}

		public void DrawMenuShortcuts()
		{
			AmplifyShaderEditorWindow window = UIUtils.CurrentWindow;
			if ( window != null )
			{
				EditorGUI.indentLevel--;
				DrawItem( ShortcutsManager.ScrollUpKey.ToString(), "Scroll Up Menu" );
				DrawItem( ShortcutsManager.ScrollDownKey.ToString(), "Scroll Down Menu" );
				DrawItem( "RMB Drag", "Scroll Menu" );
				EditorGUI.indentLevel++;
			}
			else
			{
				EditorGUILayout.LabelField( NoASEWindowWarning );
			}
		}

		void DrawItem( string name, string description )
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label( name, m_labelStyleBold , GUILayout.Width( TitleLabelWidth ) );
			GUILayout.Label( description, m_labelStyle );
			GUILayout.EndHorizontal();
			GUILayout.Space( PixelSeparator );
		}

		public void DrawNodesShortcuts()
		{
			AmplifyShaderEditorWindow window = UIUtils.CurrentWindow;
			if ( window != null )
			{
				if ( m_nodesShortcuts == null || m_nodesShortcuts.Count == 0 )
				{
					m_nodesShortcuts = window.ShortcutManagerInstance.AvailableNodesShortcutsList;
				}

				EditorGUI.indentLevel--;
				int count = m_nodesShortcuts.Count;
				for ( int i = 0; i < count; i++ )
				{
					DrawItem( m_nodesShortcuts[ i ].Name, m_nodesShortcuts[ i ].Description );
				}
				EditorGUI.indentLevel++;
			}
			else
			{
				EditorGUILayout.LabelField( NoASEWindowWarning );
			}
		}
		string CreateCompatibilityString( string source )
		{
			string[] split = source.Split( '.' );
			if ( split != null && split.Length > 1 )
			{
				return split[ 1 ];
			}
			else
			{
				return source;
			}
		}
		public void DrawCompatibleAssets()
		{
			AmplifyShaderEditorWindow window = UIUtils.CurrentWindow;
			if ( window != null )
			{
				if ( m_compatibleAssetsInfo == null )
				{
					m_compatibleAssetsInfo = new List<string[]>();
					List<ContextMenuItem> items = window.ContextMenuInstance.MenuItems;
					int count = items.Count;
					for ( int i = 0; i < count; i++ )
					{
						if ( items[ i ].NodeAttributes != null && items[ i ].NodeAttributes.CastType != null )
						{
							string types = string.Empty;
							if ( items[ i ].NodeAttributes.CastType.Length > 1 )
							{
								for ( int j = 0; j < items[ i ].NodeAttributes.CastType.Length; j++ )
								{
									types += CreateCompatibilityString( items[ i ].NodeAttributes.CastType[ j ].ToString() );


									if ( j < items[ i ].NodeAttributes.CastType.Length - 1 )
									{
										types += ", ";
									}
								}
							}
							else
							{
								types = CreateCompatibilityString( items[ i ].NodeAttributes.CastType[ 0 ].ToString() );
							}
							m_compatibleAssetsInfo.Add( new string[] { items[ i ].NodeAttributes.Name + ":   ", types } );
						}
					}
				}
				EditorGUI.indentLevel--;
				int nodeCount = m_compatibleAssetsInfo.Count;
				for ( int j = 0; j < nodeCount; j++ )
				{
					DrawItem( m_compatibleAssetsInfo[ j ][ 0 ], m_compatibleAssetsInfo[ j ][ 1 ] );
				}
				EditorGUI.indentLevel++;
			}
			else
			{
				EditorGUILayout.LabelField( NoASEWindowWarning );
			}
		}

		public void DrawNodeDescriptions()
		{
			AmplifyShaderEditorWindow window = UIUtils.CurrentWindow;
			if ( window != null )
			{
				if ( m_nodeDescriptionsInfo == null )
				{
					//fetch node info
					m_nodeDescriptionsInfo = new List<NodeDescriptionInfo>();
					Dictionary<string, PaletteFilterData> nodeData = window.CurrentPaletteWindow.BuildFullList();
					var enumerator = nodeData.GetEnumerator();
					while ( enumerator.MoveNext() )
					{
						List<ContextMenuItem> nodes = enumerator.Current.Value.Contents;
						int count = nodes.Count;

						NodeDescriptionInfo currInfo = new NodeDescriptionInfo();
						currInfo.Contents = new string[ count, 2 ];
						currInfo.Category = enumerator.Current.Key;

						for ( int i = 0; i < count; i++ )
						{
							currInfo.Contents[ i, 0 ] = nodes[ i ].Name + ':';
							currInfo.Contents[ i, 1 ] = nodes[ i ].Description;
						}
						m_nodeDescriptionsInfo.Add( currInfo );
					}
				}

				//draw
				{
					GUILayout.Space( 5 );
					int count = m_nodeDescriptionsInfo.Count;
					EditorGUI.indentLevel--;
					for ( int i = 0; i < count; i++ )
					{
						m_nodeDescriptionsInfo[ i ].FoldoutValue = EditorGUILayout.Foldout( m_nodeDescriptionsInfo[ i ].FoldoutValue, m_nodeDescriptionsInfo[ i ].Category, m_nodeInfoFoldoutStyle );
						if ( m_nodeDescriptionsInfo[ i ].FoldoutValue )
						{
							EditorGUI.indentLevel++;
							int nodeCount = m_nodeDescriptionsInfo[ i ].Contents.GetLength( 0 );
							for ( int j = 0; j < nodeCount; j++ )
							{
								GUILayout.Label( m_nodeDescriptionsInfo[ i ].Contents[ j, 0 ], m_nodeInfoLabelStyleBold );
								GUILayout.Label( m_nodeDescriptionsInfo[ i ].Contents[ j, 1 ], m_nodeInfoLabelStyle );
								GUILayout.Space( PixelSeparator );
							}
							EditorGUI.indentLevel--;
						}
						GUILayout.Space( PixelSeparator );
					}
					EditorGUI.indentLevel++;
				}
			}
			else
			{
				EditorGUILayout.LabelField( NoASEWindowWarning );
			}
		}

		private void OnDestroy()
		{
			m_nodesShortcuts = null;
			m_editorShortcuts = null;
			m_portStyle = null;
			m_labelStyle = null;
			m_labelStyleBold = null;
			m_nodeInfoLabelStyle = null;
			m_nodeInfoLabelStyleBold = null;
			m_nodeInfoFoldoutStyle = null;
			m_init = false;

			if ( m_nodeDescriptionsInfo != null )
			{
				m_nodeDescriptionsInfo.Clear();
				m_nodeDescriptionsInfo = null;
			}

			if( m_compatibleAssetsInfo != null )
			{
				m_compatibleAssetsInfo.Clear();
				m_compatibleAssetsInfo = null;
			}
		}
	}
}
