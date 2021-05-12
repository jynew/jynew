// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	public enum ToolButtonType
	{
		Update = 0,
		Live,
		OpenSourceCode,
		CleanUnusedNodes,
		//SelectShader,
		New,
		Open,
		Save,
		Library,
		Options,
		Help,
		MasterNode,
		FocusOnMasterNode,
		FocusOnSelection,
		ShowInfoWindow,
		ShowTipsWindow,
		ShowConsole
	}

	public enum ToolbarType
	{
		File,
		Help
	}

	public class ToolbarMenuTab
	{
		private Rect m_tabArea;
		private GenericMenu m_tabMenu;
		public ToolbarMenuTab( float x, float y, float width, float height )
		{
			m_tabMenu = new GenericMenu();
			m_tabArea = new Rect( x, y, width, height );
		}

		public void ShowMenu()
		{
			m_tabMenu.DropDown( m_tabArea );
		}

		public void AddItem( string itemName, GenericMenu.MenuFunction callback )
		{
			m_tabMenu.AddItem( new GUIContent( itemName ), false, callback );
		}
	}

	[Serializable]
	public sealed class ToolsWindow : MenuParent
	{
		private static readonly Color RightIconsColorOff = new Color( 1f, 1f, 1f, 0.8f );
		private static readonly Color LeftIconsColorOff = new Color( 1f, 1f, 1f, 0.5f );

		private static readonly Color RightIconsColorOn = new Color( 1f, 1f, 1f, 1.0f );
		private static readonly Color LeftIconsColorOn = new Color( 1f, 1f, 1f, 0.8f );

		private const float TabY = 9;
		private const float TabX = 5;
		private const string ShaderFileTitleStr = "Current Shader";
		private const string FileToolbarStr = "File";
		private const string HelpToolbarStr = "Help";
		private const string LiveShaderStr = "Live Shader";
		private const string LoadOnSelectionStr = "Load on selection";
		private const string CurrentObjectStr = "Current Object: ";


		public ToolsMenuButton.ToolButtonPressed ToolButtonPressedEvt;
		//private GUIStyle m_toolbarButtonStyle;
		private GUIStyle m_toggleStyle;
		private GUIStyle m_borderStyle;

		private ToolsMenuButton m_updateButton;
		private ToolsMenuButton m_liveButton;
		private ToolsMenuButton m_openSourceCodeButton;

		private ToolsMenuButton m_focusOnSelectionButton;
		private ToolsMenuButton m_focusOnMasterNodeButton;
		private ToolsMenuButton m_showInfoWindowButton;
		private ToolsMenuButton m_showTipsWindowButton;
		private ToolsMenuButton m_cleanUnusedNodesButton;
		private ToolsMenuButton m_showConsoleWindowButton;

		//Used for collision detection to invalidate inputs on graph area
		private Rect m_areaLeft = new Rect( 0, 0, 140, 40 );
		private Rect m_areaRight = new Rect( 0, 0, 75, 40 );
		private Rect m_boxRect;
		private Rect m_borderRect;
		
		public const double InactivityRefreshTime = 0.25;
		private int m_currentSelected = 0;

		//Search Bar
		private const string SearchBarId = "ASE_SEARCH_BAR";
		private bool m_searchBarVisible = false;
		private bool m_selectSearchBarTextfield = false;
		private bool m_refreshSearchResultList = false;

		private Rect m_searchBarSize;
		private string m_searchBarValue = string.Empty;
		private List<ParentNode> m_searchResultNodes = new List<ParentNode>();

		// width and height are between [0,1] and represent a percentage of the total screen area
		public ToolsWindow( AmplifyShaderEditorWindow parentWindow ) : base( parentWindow, 0, 0, 0, 64, "Tools", MenuAnchor.TOP_LEFT, MenuAutoSize.NONE )
		{
			m_updateButton = new ToolsMenuButton( m_parentWindow, ToolButtonType.Update, 0, 0, -1, -1, IOUtils.UpdateOutdatedGUID, string.Empty, "Create and apply shader to material.", 5 );
			m_updateButton.ToolButtonPressedEvt += OnButtonPressedEvent;
			m_updateButton.AddState( IOUtils.UpdateOFFGUID );
			m_updateButton.AddState( IOUtils.UpdateUpToDatedGUID );

			m_liveButton = new ToolsMenuButton( m_parentWindow, ToolButtonType.Live, 0, 0, -1, -1, IOUtils.LiveOffGUID, string.Empty, "Automatically saves shader when canvas is changed.", 50 );
			m_liveButton.ToolButtonPressedEvt += OnButtonPressedEvent;
			m_liveButton.AddState( IOUtils.LiveOnGUID );
			m_liveButton.AddState( IOUtils.LivePendingGUID );

			//ToolsMenuButton cleanUnusedNodesButton = new ToolsMenuButton( m_parentWindow, ToolButtonType.CleanUnusedNodes, 0, 0, -1, -1, IOUtils.CleanupOFFGUID, string.Empty, "Remove all nodes not connected to the master node.", 77 );
			//cleanUnusedNodesButton.ToolButtonPressedEvt += OnButtonPressedEvent;
			//cleanUnusedNodesButton.AddState( IOUtils.CleanUpOnGUID );
			//m_list[ ( int ) ToolButtonType.CleanUnusedNodes ] = cleanUnusedNodesButton;

			m_openSourceCodeButton = new ToolsMenuButton( m_parentWindow, ToolButtonType.OpenSourceCode, 0, 0, -1, -1, IOUtils.OpenSourceCodeOFFGUID, string.Empty, "Open shader file in your default shader editor.", 80, false );
			m_openSourceCodeButton.ToolButtonPressedEvt += OnButtonPressedEvent;
			m_openSourceCodeButton.AddState( IOUtils.OpenSourceCodeONGUID );

			m_cleanUnusedNodesButton = new ToolsMenuButton( m_parentWindow, ToolButtonType.CleanUnusedNodes, 0, 0, -1, -1, IOUtils.CleanupOFFGUID, string.Empty, "Remove all nodes not connected to the master node.", 77 );
			m_cleanUnusedNodesButton.ToolButtonPressedEvt += OnButtonPressedEvent;
			m_cleanUnusedNodesButton.AddState( IOUtils.CleanUpOnGUID );

			m_showConsoleWindowButton = new ToolsMenuButton( m_parentWindow, ToolButtonType.ShowConsole, 0, 0, -1, -1, IOUtils.ShowConsoleWindowGUID, string.Empty, "Show internal console", 74 );
			m_showConsoleWindowButton.ToolButtonPressedEvt += OnButtonPressedEvent;
			m_showConsoleWindowButton.AddState( IOUtils.ShowConsoleWindowGUID );

		
			m_focusOnMasterNodeButton = new ToolsMenuButton( m_parentWindow, ToolButtonType.FocusOnMasterNode, 0, 0, -1, -1, IOUtils.FocusNodeGUID, string.Empty, "Focus on active master node.", -1, false );
			m_focusOnMasterNodeButton.ToolButtonPressedEvt += OnButtonPressedEvent;

			m_focusOnSelectionButton = new ToolsMenuButton( m_parentWindow, ToolButtonType.FocusOnSelection, 0, 0, -1, -1, IOUtils.FitViewGUID, string.Empty, "Focus on selection or fit to screen if none selected." );
			m_focusOnSelectionButton.ToolButtonPressedEvt += OnButtonPressedEvent;

			m_showInfoWindowButton = new ToolsMenuButton( m_parentWindow, ToolButtonType.ShowInfoWindow, 0, 0, -1, -1, IOUtils.ShowInfoWindowGUID, string.Empty, "Open Helper Window." );
			m_showInfoWindowButton.ToolButtonPressedEvt += OnButtonPressedEvent;

			m_showTipsWindowButton = new ToolsMenuButton( m_parentWindow, ToolButtonType.ShowTipsWindow, 0, 0, -1, -1, IOUtils.ShowTipsWindowGUID, string.Empty, "Open Quick Tips!" );
			m_showTipsWindowButton.ToolButtonPressedEvt += OnButtonPressedEvent;
			m_searchBarSize = new Rect( 0, TabY + 4, 110, 60 );
		}

		void OnShowPortLegend()
		{
			ParentWindow.ShowPortInfo();
		}

		override public void Destroy()
		{
			base.Destroy();
			//for ( int i = 0; i < m_list.Length; i++ )
			//{
			//	m_list[ i ].Destroy();
			//}
			//m_list = null;

			m_searchResultNodes.Clear();
			m_searchResultNodes = null;

			m_updateButton.Destroy();
			m_updateButton = null;

			m_liveButton.Destroy();
			m_liveButton = null;

			m_openSourceCodeButton.Destroy();
			m_openSourceCodeButton = null;

			m_focusOnMasterNodeButton.Destroy();
			m_focusOnMasterNodeButton = null;

			m_focusOnSelectionButton.Destroy();
			m_focusOnSelectionButton = null;

			m_showInfoWindowButton.Destroy();
			m_showInfoWindowButton = null;

			m_showTipsWindowButton.Destroy();
			m_showTipsWindowButton = null;

			m_cleanUnusedNodesButton.Destroy();
			m_cleanUnusedNodesButton = null;

			m_showConsoleWindowButton.Destroy();
			m_showConsoleWindowButton = null;
		}

		void OnButtonPressedEvent( ToolButtonType type )
		{
			if ( ToolButtonPressedEvt != null )
				ToolButtonPressedEvt( type );
		}

		public override void Draw( Rect parentPosition, Vector2 mousePosition, int mouseButtonId, bool hasKeyboadFocus )
		{
			base.Draw( parentPosition, mousePosition, mouseButtonId, hasKeyboadFocus );

			Color bufferedColor = GUI.color;
			m_areaLeft.x = m_transformedArea.x + TabX;
			m_areaRight.x = m_transformedArea.x + m_transformedArea.width - 75 - TabX;

			//if ( m_toolbarButtonStyle == null )
			//{
			//	m_toolbarButtonStyle = new GUIStyle( UIUtils.Button );
			//	m_toolbarButtonStyle.fixedWidth = 100;
			//}

			if ( m_toggleStyle == null )
			{
				m_toggleStyle = UIUtils.Toggle;
			}

			//for ( int i = 0; i < m_list.Length; i++ )
			//{
			//	GUI.color = m_list[ i ].IsInside( mousePosition ) ? LeftIconsColorOn : LeftIconsColorOff;
			//	m_list[ i ].Draw( TabX + m_transformedArea.x + m_list[ i ].ButtonSpacing, TabY );
			//}
			GUI.color = m_updateButton.IsInside( mousePosition ) ? LeftIconsColorOn : LeftIconsColorOff;
			m_updateButton.Draw( TabX + m_transformedArea.x + m_updateButton.ButtonSpacing, TabY );

			GUI.color = m_liveButton.IsInside( mousePosition ) ? LeftIconsColorOn : LeftIconsColorOff;
			m_liveButton.Draw( TabX + m_transformedArea.x + m_liveButton.ButtonSpacing, TabY );

			GUI.color = m_openSourceCodeButton.IsInside( mousePosition ) ? LeftIconsColorOn : LeftIconsColorOff;
			m_openSourceCodeButton.Draw( TabX + m_transformedArea.x + m_openSourceCodeButton.ButtonSpacing, TabY );

			if ( m_searchBarVisible )
			{
				m_searchBarSize.x = m_transformedArea.x + m_transformedArea.width - 270 - TabX;
				string currentFocus = GUI.GetNameOfFocusedControl();

				if ( Event.current.type == EventType.KeyDown )
				{
					KeyCode keyCode = Event.current.keyCode;
					if ( Event.current.shift )
					{
						if ( keyCode == KeyCode.F3 ||
							( ( keyCode == KeyCode.KeypadEnter || keyCode == KeyCode.Return ) &&
							( currentFocus.Equals( SearchBarId ) || string.IsNullOrEmpty( currentFocus ) ) ) )
							SelectPrevious();
					}
					else
					{
						if ( keyCode == KeyCode.F3 ||
							( ( keyCode == KeyCode.KeypadEnter || keyCode == KeyCode.Return ) &&
							( currentFocus.Equals( SearchBarId ) || string.IsNullOrEmpty( currentFocus ) ) ) )
							SelectNext();
					}
				}
				
				if( currentFocus.Equals( SearchBarId ) || ( m_parentWindow.CameraDrawInfo.CurrentEventType == EventType.MouseDown && m_searchBarSize.Contains( m_parentWindow.CameraDrawInfo.MousePosition ) ) || m_selectSearchBarTextfield )
				{
					EditorGUI.BeginChangeCheck();
					{
						GUI.SetNextControlName( SearchBarId );
						m_searchBarValue = EditorGUI.TextField( m_searchBarSize, m_searchBarValue, UIUtils.ToolbarSearchTextfield );
					}
					if ( EditorGUI.EndChangeCheck() )
					{
						m_refreshSearchResultList = true;
					}
				} else
				{
					GUI.Label( m_searchBarSize, m_searchBarValue, UIUtils.ToolbarSearchTextfield );
				}

				m_searchBarSize.x += m_searchBarSize.width;
				if ( m_parentWindow.CameraDrawInfo.CurrentEventType == EventType.MouseDown && m_searchBarSize.Contains( m_parentWindow.CameraDrawInfo.MousePosition ) )
				{
					if ( string.IsNullOrEmpty( m_searchBarValue ) )
					{
						m_searchBarVisible = false;
						m_refreshSearchResultList = false;
					}
					else
					{
						m_searchBarValue = string.Empty;
						m_searchResultNodes.Clear();
						m_currentSelected = -1;
					}
				}

				GUI.Label( m_searchBarSize, string.Empty, UIUtils.ToolbarSearchCancelButton );



				if ( Event.current.isKey && Event.current.keyCode == KeyCode.Escape )
				{
					m_searchBarVisible = false;
					m_refreshSearchResultList = false;
					GUI.FocusControl( null );
					m_selectSearchBarTextfield = false;
				}

				if ( m_refreshSearchResultList && ( m_parentWindow.CurrentInactiveTime > InactivityRefreshTime ) )
				{
					RefreshList();
				}
			}

			if ( m_selectSearchBarTextfield )
			{
				m_selectSearchBarTextfield = false;
				EditorGUI.FocusTextInControl( SearchBarId );
				//GUI.FocusControl( SearchBarId );
			}

			//if ( Event.current.control && Event.current.isKey && Event.current.keyCode == KeyCode.F && Event.current.type == EventType.KeyDown )
			if( m_parentWindow.CurrentCommandName.Equals("Find") )
			{
				if ( !m_searchBarVisible )
				{
					m_searchBarVisible = true;
					m_refreshSearchResultList = false;
				}
				m_selectSearchBarTextfield = true;
			}

			GUI.color = m_focusOnSelectionButton.IsInside( mousePosition ) ? RightIconsColorOn : RightIconsColorOff;
			m_focusOnSelectionButton.Draw( m_transformedArea.x + m_transformedArea.width - 30 - TabX, TabY );

			GUI.color = m_focusOnMasterNodeButton.IsInside( mousePosition ) ? RightIconsColorOn : RightIconsColorOff;
			m_focusOnMasterNodeButton.Draw( m_transformedArea.x + m_transformedArea.width - 65 - TabX, TabY );

			GUI.color = m_showInfoWindowButton.IsInside( mousePosition ) ? RightIconsColorOn : RightIconsColorOff;
			m_showInfoWindowButton.Draw( m_transformedArea.x + m_transformedArea.width - 110 - TabX, TabY );

			//GUI.color = m_showTipsWindowButton.IsInside( mousePosition ) ? RightIconsColorOn : RightIconsColorOff;
			//m_showTipsWindowButton.Draw( m_transformedArea.x + m_transformedArea.width - 140 - TabX, TabY );

			GUI.color = m_cleanUnusedNodesButton.IsInside( mousePosition ) ? RightIconsColorOn : RightIconsColorOff;
			m_cleanUnusedNodesButton.Draw( m_transformedArea.x + m_transformedArea.width - 140 - TabX, TabY );
			GUI.color = bufferedColor;

			//GUI.color = m_showConsoleWindowButton.IsInside( mousePosition ) ? RightIconsColorOn : RightIconsColorOff;
			//m_showConsoleWindowButton.Draw( m_transformedArea.x + m_transformedArea.width - 170 - TabX, TabY );
			//GUI.color = bufferedColor;
			
		}

		public void OnNodeRemovedFromGraph( ParentNode node )
		{
			m_searchResultNodes.Remove( node );
		}

		int m_previousNodeCount = 0;

		void RefreshList()
		{
			m_refreshSearchResultList = false;
			m_currentSelected = -1;
			m_searchResultNodes.Clear();
			if ( !string.IsNullOrEmpty( m_searchBarValue ) )
			{
				List<ParentNode> nodes = m_parentWindow.CurrentGraph.AllNodes;
				int count = nodes.Count;
				m_previousNodeCount = count;
				for ( int i = 0; i < count; i++ )
				{
					if ( nodes[ i ].TitleContent.text.IndexOf( m_searchBarValue, StringComparison.CurrentCultureIgnoreCase ) >= 0 )
					{
						m_searchResultNodes.Add( nodes[ i ] );
					}
				}
			}
		}

		void SelectNext()
		{
			if ( m_refreshSearchResultList || m_parentWindow.CurrentGraph.AllNodes.Count != m_previousNodeCount )
			{
				RefreshList();
			}

			if ( m_searchResultNodes.Count > 0 )
			{
				m_currentSelected = ( m_currentSelected + 1 ) % m_searchResultNodes.Count;
				m_parentWindow.FocusOnNode( m_searchResultNodes[ m_currentSelected ], 1, true ,true);
			}
		}

		void SelectPrevious()
		{
			if ( m_refreshSearchResultList || m_parentWindow.CurrentGraph.AllNodes.Count != m_previousNodeCount )
			{
				RefreshList();
			}

			if ( m_searchResultNodes.Count > 0 )
			{
				m_currentSelected = ( m_currentSelected > 1 ) ? ( m_currentSelected - 1 ) : ( m_searchResultNodes.Count - 1 );
				m_parentWindow.FocusOnNode( m_searchResultNodes[ m_currentSelected ], 1, true );
			}
		}


		public void SetStateOnButton( ToolButtonType button, int state, string tooltip )
		{
			switch ( button )
			{
				case ToolButtonType.New:
				case ToolButtonType.Open:
				case ToolButtonType.Save:
				case ToolButtonType.Library:
				case ToolButtonType.Options:
				case ToolButtonType.Help:
				case ToolButtonType.MasterNode: break;
				case ToolButtonType.OpenSourceCode:
				{
					m_openSourceCodeButton.SetStateOnButton( state, tooltip );
				}
				break;
				case ToolButtonType.Update:
				{
					m_updateButton.SetStateOnButton( state, tooltip );
				}
				break;
				case ToolButtonType.Live:
				{
					m_liveButton.SetStateOnButton( state, tooltip );
				}
				break;
				case ToolButtonType.CleanUnusedNodes:
				//case eToolButtonType.SelectShader:
				{
					m_cleanUnusedNodesButton.SetStateOnButton( state, tooltip );
				}
				break;
				case ToolButtonType.FocusOnMasterNode:
				{
					m_focusOnMasterNodeButton.SetStateOnButton( state, tooltip );
				}
				break;
				case ToolButtonType.FocusOnSelection:
				{
					m_focusOnSelectionButton.SetStateOnButton( state, tooltip );
				}
				break;

				case ToolButtonType.ShowInfoWindow:
				{
					m_showInfoWindowButton.SetStateOnButton( state, tooltip );
				}
				break;
				case ToolButtonType.ShowTipsWindow:
				{
					m_showTipsWindowButton.SetStateOnButton( state, tooltip );
				}
				break;
				case ToolButtonType.ShowConsole:
				{
					m_showConsoleWindowButton.SetStateOnButton( state, tooltip );
				}
				break;
			}
		}

		public void SetStateOnButton( ToolButtonType button, int state )
		{
			switch ( button )
			{
				case ToolButtonType.New:
				case ToolButtonType.Open:
				case ToolButtonType.Save:
				case ToolButtonType.Library:
				case ToolButtonType.Options:
				case ToolButtonType.Help:
				case ToolButtonType.MasterNode: break;
				case ToolButtonType.OpenSourceCode:
				{
					m_openSourceCodeButton.SetStateOnButton( state );
				}
				break;
				case ToolButtonType.Update:
				{
					m_updateButton.SetStateOnButton( state );
				}
				break;
				case ToolButtonType.Live:
				{
					m_liveButton.SetStateOnButton( state );
				}
				break;
				case ToolButtonType.CleanUnusedNodes:
				//case eToolButtonType.SelectShader:
				{
					m_cleanUnusedNodesButton.SetStateOnButton( state );
				}
				break;
				case ToolButtonType.FocusOnMasterNode:
				{
					m_focusOnMasterNodeButton.SetStateOnButton( state );
				}
				break;
				case ToolButtonType.FocusOnSelection:
				{
					m_focusOnSelectionButton.SetStateOnButton( state );
				}
				break;
				case ToolButtonType.ShowInfoWindow:
				{
					m_showInfoWindowButton.SetStateOnButton( state );
				}
				break;
				case ToolButtonType.ShowTipsWindow:
				{
					m_showTipsWindowButton.SetStateOnButton( state );
				}break;
				case ToolButtonType.ShowConsole:
				{
					m_showConsoleWindowButton.SetStateOnButton( state );
				}
				break;
			}
		}

		public void DrawShaderTitle( MenuParent nodeParametersWindow, MenuParent paletteWindow, float availableCanvasWidth, float graphAreaHeight, string shaderName )
		{
			float leftAdjust = nodeParametersWindow.IsMaximized ? nodeParametersWindow.RealWidth : 0;
			float rightAdjust = paletteWindow.IsMaximized ? 0 : paletteWindow.RealWidth;

			m_boxRect = new Rect( leftAdjust + rightAdjust, 0, availableCanvasWidth, 35 );
			m_boxRect.x += paletteWindow.IsMaximized ? 0 : -paletteWindow.RealWidth;
			m_boxRect.width += nodeParametersWindow.IsMaximized ? 0 : nodeParametersWindow.RealWidth;
			m_boxRect.width += paletteWindow.IsMaximized ? 0 : paletteWindow.RealWidth;

			m_borderRect = new Rect( m_boxRect );
			m_borderRect.height = graphAreaHeight;


			if ( m_borderStyle == null )
			{
				m_borderStyle = ( ParentWindow.CurrentGraph.CurrentMasterNode == null ) ? UIUtils.GetCustomStyle( CustomStyle.ShaderFunctionBorder ) : UIUtils.GetCustomStyle( CustomStyle.ShaderBorder );
			}

			GUI.Label( m_borderRect, shaderName, m_borderStyle );
			GUI.Label( m_boxRect, shaderName, UIUtils.GetCustomStyle( CustomStyle.MainCanvasTitle ) );
		}

		public override bool IsInside( Vector2 position )
		{
			if ( !m_isActive )
				return false;

			return m_boxRect.Contains( position ) || m_areaLeft.Contains( position ) || m_areaRight.Contains( position );
		}

		public GUIStyle BorderStyle
		{
			get { return m_borderStyle; }
			set { m_borderStyle = value; }
		}
	}
}
