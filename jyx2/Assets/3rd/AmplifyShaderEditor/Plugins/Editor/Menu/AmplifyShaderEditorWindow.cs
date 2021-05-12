// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
// Disabling Substance Deprecated warning

	public class AmplifyShaderEditorWindow : SearchableEditorWindow, ISerializationCallbackReceiver
	{
		public const double InactivitySaveTime = 1.0;

		public const string CopyCommand = "Copy";
		public const string PasteCommand = "Paste";
		public const string SelectAll = "SelectAll";
		public const string Duplicate = "Duplicate";
		public const string ObjectSelectorClosed = "ObjectSelectorClosed";
		public const string LiveShaderError = "Live Shader only works with an assigned Master Node on the graph";

		//public Texture2D MasterNodeOnTexture = null;
		//public Texture2D MasterNodeOffTexture = null;

		//public Texture2D GPUInstancedOnTexture = null;
		//public Texture2D GPUInstancedOffTexture = null;

		private bool m_initialized = false;
		private bool m_checkInvalidConnections = false;
		private bool m_afterDeserializeFlag = true;

		[SerializeField]
		private ParentGraph m_customGraph = null;

		// UI 
		private Rect m_graphArea;
		private Texture2D m_graphBgTexture;
		private Texture2D m_graphFgTexture;
		private GUIStyle m_graphFontStyle;
		//private GUIStyle _borderStyle;
		private Texture2D m_wireTexture;

		[SerializeField]
		TemplatesManager m_templatesManager;

		[SerializeField]
		private InnerWindowEditorVariables m_innerEditorVariables;

		[ SerializeField]
		private string m_lastpath;

		[SerializeField]
		private ASESelectionMode m_selectionMode = ASESelectionMode.Shader;

		[SerializeField]
		private DuplicatePreventionBuffer m_duplicatePreventionBuffer;

		[SerializeField]
		private double m_inactivityTime = 0;

		// Prevent save ops every tick when on live mode
		[SerializeField]
		private double m_lastTimeSaved = 0;

		[SerializeField]
		private bool m_cacheSaveOp = false;
		private const double SaveTime = 1;

		private bool m_markedToSave = false;

		// Graph logic
		[SerializeField]
		private ParentGraph m_mainGraphInstance;

		// Camera control
		[SerializeField]
		private Vector2 m_cameraOffset;

		private float m_cameraSpeed = 1;

		private Rect m_cameraInfo;

		[SerializeField]
		private float m_cameraZoom;

		[SerializeField]
		private Vector2 m_minNodePos;

		[SerializeField]
		private Vector2 m_maxNodePos;

		[SerializeField]
		private bool m_isDirty;

		[SerializeField]
		private bool m_saveIsDirty;

		[SerializeField]
		private bool m_repaintIsDirty;

		[SerializeField]
		private bool m_liveShaderEditing = false;

		[SerializeField]
		private bool m_shaderIsModified = false;

		[SerializeField]
		private string m_lastOpenedLocation = string.Empty;

		[SerializeField]
		private bool m_zoomChanged = true;

		[SerializeField]
		private float m_lastWindowWidth = 0;

		[SerializeField]
		private int m_graphCount = 0;

		[SerializeField]
		private double m_currentInactiveTime = 0;

		private bool m_ctrlSCallback = false;

		private bool m_altBoxSelection = false;
		private bool m_altDragStarted = false;
		private bool m_altPressDown = false;
		private bool m_altAvailable = true;

		// Events
		private Vector3 m_currentMousePos;
		private Vector2 m_keyEvtMousePos2D;
		private Vector2 m_currentMousePos2D;
		private Event m_currentEvent;
		private string m_currentCommandName = string.Empty;
		private bool m_insideEditorWindow;

		private bool m_lostFocus = false;
		// Selection box for multiple node selection 
		private bool m_multipleSelectionActive = false;
		private bool m_lmbPressed = false;
		private Vector2 m_multipleSelectionStart;
		private Rect m_multipleSelectionArea = new Rect( 0, 0, 0, 0 );
		private bool m_autoPanDirActive = false;
		private bool m_forceAutoPanDir = false;
		private bool m_refreshOnUndo = false;
		private bool m_loadShaderOnSelection = false;
		private bool m_refreshAvailableNodes = false;
		private double m_time;

		//Context Menu
		private Vector2 m_rmbStartPos;
		private Vector2 m_altKeyStartPos;
		private GraphContextMenu m_contextMenu;
		private ShortcutsManager m_shortcutManager;

		[SerializeField]
		private NodeAvailability m_currentNodeAvailability = NodeAvailability.SurfaceShader;
		//Clipboard
		private Clipboard m_clipboard;

		//Node Parameters Window
		[SerializeField]
		private bool m_nodeParametersWindowMaximized = true;
		private NodeParametersWindow m_nodeParametersWindow;

		// Tools Window
		private ToolsWindow m_toolsWindow;

		private ConsoleLogWindow m_consoleLogWindow;

		//Editor Options
		private OptionsWindow m_optionsWindow;

		// Mode Window
		private ShaderEditorModeWindow m_modeWindow;

		// Tools Window
		private TipsWindow m_tipsWindow;

		//Palette Window
		[SerializeField]
		private bool m_paletteWindowMaximized = true;
		private PaletteWindow m_paletteWindow;

		private ContextPalette m_contextPalette;
		private PalettePopUp m_palettePopup;
		private System.Type m_paletteChosenType;
		private AmplifyShaderFunction m_paletteChosenFunction;

		// In-Editor Message System
		GenericMessageUI m_genericMessageUI;
		private GUIContent m_genericMessageContent;

		// Drag&Drop Tool 
		private DragAndDropTool m_dragAndDropTool;

		//Custom Styles
		//private CustomStylesContainer m_customStyles;

		private AmplifyShaderFunction m_previousShaderFunction;

		private List<MenuParent> m_registeredMenus;

		private PreMadeShaders m_preMadeShaders;

		private AutoPanData[] m_autoPanArea;

		private DrawInfo m_drawInfo;
		private KeyCode m_lastKeyPressed = KeyCode.None;
		private System.Type m_commentaryTypeNode;

		private int m_onLoadDone = 0;

		private float m_copyPasteDeltaMul = 0;
		private Vector2 m_copyPasteInitialPos = Vector2.zero;
		private Vector2 m_copyPasteDeltaPos = Vector2.zero;

		private int m_repaintCount = 0;
		private bool m_forceUpdateFromMaterialFlag = false;
	
		private UnityEngine.Object m_delayedLoadObject = null;
		private double m_focusOnSelectionTimestamp;
		private double m_focusOnMasterNodeTimestamp;
		private double m_wiredDoubleTapTimestamp;

		private bool m_globalPreview = false;
		private bool m_globalShowInternalData = false;
		
		private const double AutoZoomTime = 0.25;
		private const double ToggleTime = 0.25;
		private const double WiredDoubleTapTime = 0.25;
		private const double DoubleClickTime = 0.25;

		private Material m_delayedMaterialSet = null;

		private bool m_mouseDownOnValidArea = false;

		private bool m_removedKeyboardFocus = false;

		private int m_lastHotControl = -1;

		[SerializeField]
		private bool m_isShaderFunctionWindow = false;

		private string m_currentTitle = string.Empty;
		private bool m_currentTitleMod = false;

		//private Material m_maskingMaterial = null;
		//private int m_cachedProjectInLinearId = -1;
		private int m_cachedEditorTimeId = -1;
		private int m_cachedEditorDeltaTimeId = -1;
		//private float m_repaintFrequency = 15;
		//private double m_repaintTimestamp = 0;

		// Smooth Zoom
		private bool m_smoothZoom = false;
		private float m_targetZoom;
		private double m_zoomTime;
		private Vector2 m_zoomPivot;
		private float m_targetZoomIncrement;
		private float m_zoomVelocity = 0;

		// Smooth Pan
		private bool m_smoothOffset = false;
		private double m_offsetTime;
		private Vector2 m_targetOffset;
		private Vector2 m_camVelocity = Vector2.zero;

		// Auto-Compile samples 
		private bool m_forcingMaterialUpdateFlag = false;
		private bool m_forcingMaterialUpdateOp = false;
		private List<Material> m_materialsToUpdate = new List<Material>();

		private NodeExporterUtils m_nodeExporterUtils;
		private bool m_performFullUndoRegister = true;

		[SerializeField]
		private AmplifyShaderFunction m_openedShaderFunction;

		[SerializeField]
		private bool m_openedAssetFromNode = false;

		private bool m_nodesLoadedCorrectly = false;
		private GUIContent NodesExceptionMessage = new GUIContent( "ASE is unable to load correctly due to some faulty other classes/plugin in your project. We advise to review all your imported plugins." );

		private bool m_replaceMasterNode = false;
		private AvailableShaderTypes m_replaceMasterNodeType;
		private string m_replaceMasterNodeData;
		private bool m_replaceMasterNodeDataFromCache;
		private NodeWireReferencesUtils m_wireReferenceUtils = new NodeWireReferencesUtils();

		private ParentNode m_nodeToFocus = null;
		private float m_zoomToFocus = 1.0f;
		private bool m_selectNodeToFocus = true;
		

		public bool CheckFunctions = false;

		// Unity Menu item
		[MenuItem( "Window/Amplify Shader Editor/Open Canvas", false, 1000 )]
		static void OpenMainShaderGraph()
		{
			if( IOUtils.AllOpenedWindows.Count > 0 )
			{
				AmplifyShaderEditorWindow currentWindow = CreateTab( "Empty", UIUtils.ShaderIcon );
				UIUtils.CurrentWindow = currentWindow;
				currentWindow.CreateNewGraph( "Empty" );
				currentWindow.Show();
			}
			else
			{
				AmplifyShaderEditorWindow currentWindow = OpenWindow( "Empty", UIUtils.ShaderIcon );
				currentWindow.CreateNewGraph( "Empty" );
				//currentWindow.Show();
			}
		}

		public static string GenerateTabTitle( string original, bool modified = false )
		{
			GUIContent content = new GUIContent( original );
			GUIStyle tabStyle = new GUIStyle( (GUIStyle)"dragtabdropwindow" );//  GUI.skin.FindStyle( "dragtabdropwindow" );
			string finalTitle = string.Empty;
			bool addEllipsis = false;
			for( int i = 1; i <= original.Length; i++ )
			{
				content.text = original.Substring( 0, i );
				Vector2 titleSize = tabStyle.CalcSize( content );
				int maxSize = modified ? 62 : 69;
				if( titleSize.x > maxSize )
				{
					addEllipsis = true;
					break;
				}
				else
				{
					finalTitle = content.text;
				}
			}
			if( addEllipsis )
				finalTitle += "..";
			if( modified )
				finalTitle += "*";
			return finalTitle;
		}

		public static void ConvertShaderToASE( Shader shader )
		{
			if( UIUtils.IsUnityNativeShader( shader ) )
			{
				Debug.LogWarningFormat( "Action not allowed. Attempting to load the native {0} shader into Amplify Shader Editor", shader.name );
				return;
			}

			if( IOUtils.AllOpenedWindows.Count > 0 )
			{
				AmplifyShaderEditorWindow openedTab = null;
				for( int i = 0; i < IOUtils.AllOpenedWindows.Count; i++ )
				{
					if( AssetDatabase.GetAssetPath( shader ).Equals( IOUtils.AllOpenedWindows[ i ].LastOpenedLocation ) )
					{
						openedTab = IOUtils.AllOpenedWindows[ i ];
						break;
					}
				}

				if( openedTab != null )
				{
					openedTab.wantsMouseMove = true;
					openedTab.ShowTab();
					UIUtils.CurrentWindow = openedTab;
				}
				else
				{
					EditorWindow openedWindow = AmplifyShaderEditorWindow.GetWindow<AmplifyShaderEditorWindow>();
					AmplifyShaderEditorWindow currentWindow = CreateTab();
					WindowHelper.AddTab( openedWindow, currentWindow );
					UIUtils.CurrentWindow = currentWindow;
				}
			}
			else
			{
				AmplifyShaderEditorWindow currentWindow = OpenWindow( shader.name, UIUtils.ShaderIcon );
				UIUtils.CurrentWindow = currentWindow;
			}

			if( IOUtils.IsASEShader( shader ) )
			{
				UIUtils.CurrentWindow.LoadProjectSelected( shader );
			}
			else
			{
				UIUtils.CreateEmptyFromInvalid( shader );
				UIUtils.ShowMessage( "Trying to open shader not created on ASE!\nBEWARE, old data will be lost if saving it here!", MessageSeverity.Warning );
				if( UIUtils.CurrentWindow.LiveShaderEditing )
				{
					UIUtils.ShowMessage( "Disabling Live Shader Editing. Must manually re-enable it.", MessageSeverity.Warning );
					UIUtils.CurrentWindow.LiveShaderEditing = false;
				}
			}
		}

		public static void LoadMaterialToASE( Material material )
		{
			if( IOUtils.AllOpenedWindows.Count > 0 )
			{
				AmplifyShaderEditorWindow openedTab = null;
				for( int i = 0; i < IOUtils.AllOpenedWindows.Count; i++ )
				{
					if( AssetDatabase.GetAssetPath( material.shader ).Equals( IOUtils.AllOpenedWindows[ i ].LastOpenedLocation ) )
					{
						openedTab = IOUtils.AllOpenedWindows[ i ];
						break;
					}
				}

				if( openedTab != null )
				{
					openedTab.wantsMouseMove = true;
					openedTab.ShowTab();
					UIUtils.CurrentWindow = openedTab;
				}
				else
				{
					EditorWindow openedWindow = AmplifyShaderEditorWindow.GetWindow<AmplifyShaderEditorWindow>();
					AmplifyShaderEditorWindow currentWindow = CreateTab();
					WindowHelper.AddTab( openedWindow, currentWindow );
					UIUtils.CurrentWindow = currentWindow;
				}
			}
			else
			{
				AmplifyShaderEditorWindow currentWindow = OpenWindow( material.name, UIUtils.MaterialIcon );
				UIUtils.CurrentWindow = currentWindow;
			}

			if( IOUtils.IsASEShader( material.shader ) )
			{
				UIUtils.CurrentWindow.LoadProjectSelected( material );
			}
			else
			{
				UIUtils.CreateEmptyFromInvalid( material.shader );
				UIUtils.SetDelayedMaterialMode( material );
			}
		}

		public static void LoadShaderFunctionToASE( AmplifyShaderFunction shaderFunction, bool openedAssetFromNode )
		{
			if( IOUtils.AllOpenedWindows.Count > 0 )
			{
				AmplifyShaderEditorWindow openedTab = null;
				for( int i = 0; i < IOUtils.AllOpenedWindows.Count; i++ )
				{
					if( AssetDatabase.GetAssetPath( shaderFunction ).Equals( IOUtils.AllOpenedWindows[ i ].LastOpenedLocation ) )
					{
						openedTab = IOUtils.AllOpenedWindows[ i ];
						break;
					}
				}

				if( openedTab != null )
				{
					openedTab.wantsMouseMove = true;
					openedTab.ShowTab();
					UIUtils.CurrentWindow = openedTab;
				}
				else
				{
					EditorWindow openedWindow = AmplifyShaderEditorWindow.GetWindow<AmplifyShaderEditorWindow>();
					AmplifyShaderEditorWindow currentWindow = CreateTab();
					WindowHelper.AddTab( openedWindow, currentWindow );
					UIUtils.CurrentWindow = currentWindow;
				}
			}
			else
			{
				AmplifyShaderEditorWindow currentWindow = OpenWindow( shaderFunction.FunctionName, UIUtils.ShaderFunctionIcon );
				UIUtils.CurrentWindow = currentWindow;
			}

			UIUtils.CurrentWindow.OpenedAssetFromNode = openedAssetFromNode;
			if( IOUtils.IsShaderFunction( shaderFunction.FunctionInfo ) )
			{
				UIUtils.CurrentWindow.LoadProjectSelected( shaderFunction );
			}
			else
			{
				UIUtils.CurrentWindow.titleContent.text = GenerateTabTitle( shaderFunction.FunctionName );
				UIUtils.CurrentWindow.titleContent.image = UIUtils.ShaderFunctionIcon;
				UIUtils.CreateEmptyFunction( shaderFunction );
			}
		}

		public static AmplifyShaderEditorWindow OpenWindow( string title = null, Texture icon = null )
		{
			AmplifyShaderEditorWindow currentWindow = (AmplifyShaderEditorWindow)AmplifyShaderEditorWindow.GetWindow( typeof( AmplifyShaderEditorWindow ), false );
			currentWindow.minSize = new Vector2( ( Constants.MINIMIZE_WINDOW_LOCK_SIZE - 150 ), 270 );
			currentWindow.wantsMouseMove = true;
			if( title != null )
				currentWindow.titleContent.text = GenerateTabTitle( title );
			if( icon != null )
				currentWindow.titleContent.image = icon;
			return currentWindow;
		}

		public static AmplifyShaderEditorWindow CreateTab( string title = null, Texture icon = null )
		{
			AmplifyShaderEditorWindow currentWindow = EditorWindow.CreateInstance<AmplifyShaderEditorWindow>();
			currentWindow.minSize = new Vector2( ( Constants.MINIMIZE_WINDOW_LOCK_SIZE - 150 ), 270 );
			currentWindow.wantsMouseMove = true;
			if( title != null )
				currentWindow.titleContent.text = GenerateTabTitle( title );
			if( icon != null )
				currentWindow.titleContent.image = icon;
			return currentWindow;
		}

		public double CalculateInactivityTime()
		{
			double currTime = EditorApplication.timeSinceStartup;
			switch( Event.current.type )
			{
				case EventType.MouseDown:
				case EventType.MouseUp:
				case EventType.MouseMove:
				case EventType.MouseDrag:
				case EventType.KeyDown:
				case EventType.KeyUp:
				case EventType.ScrollWheel:
				case EventType.DragUpdated:
				case EventType.DragPerform:
				case EventType.DragExited:
				case EventType.ValidateCommand:
				case EventType.ExecuteCommand:
				{
					m_inactivityTime = currTime;
					return 0;
				}
			}
			return currTime - m_inactivityTime;
		}
		
		// Shader Graph window
		public override void OnEnable()
		{
			base.OnEnable();
			if( m_templatesManager == null )
			{
				m_templatesManager = IOUtils.FirstValidTemplatesManager;
				if( m_templatesManager == null )
				{
					m_templatesManager = ScriptableObject.CreateInstance<TemplatesManager>();
					m_templatesManager.Init();
					if( TemplatesManager.ShowDebugMessages )
						Debug.Log( "Creating Manager" );
				}
				else
				{
					if( TemplatesManager.ShowDebugMessages )
						Debug.Log( "Assigning Manager" );
				}
			}
			else if( !m_templatesManager.Initialized )
			{
				if( TemplatesManager.ShowDebugMessages )
					Debug.Log( "Re-Initializing Manager" );
				m_templatesManager.Init();
			}

			if( m_innerEditorVariables == null )
			{
				m_innerEditorVariables = new InnerWindowEditorVariables();
				m_innerEditorVariables.Initialize();
			}

			if( m_mainGraphInstance == null )
			{
				m_mainGraphInstance = CreateInstance<ParentGraph>();
				m_mainGraphInstance.Init();
				m_mainGraphInstance.ParentWindow = this;
				m_mainGraphInstance.SetGraphId( 0 );
			}
			m_mainGraphInstance.ResetEvents();
			m_mainGraphInstance.OnNodeEvent += OnNodeStoppedMovingEvent;
			m_mainGraphInstance.OnMaterialUpdatedEvent += OnMaterialUpdated;
			m_mainGraphInstance.OnShaderUpdatedEvent += OnShaderUpdated;
			m_mainGraphInstance.OnEmptyGraphDetectedEvt += OnEmptyGraphDetected;
			m_mainGraphInstance.OnNodeRemovedEvent += m_toolsWindow.OnNodeRemovedFromGraph;
			GraphCount = 1;

			IOUtils.Init();
			IOUtils.AllOpenedWindows.Add( this );

			// Only runs once for multiple windows
			EditorApplication.update -= IOUtils.UpdateIO;
			EditorApplication.update += IOUtils.UpdateIO;
			
			EditorApplication.update -= UpdateTime;
			EditorApplication.update -= UpdateNodePreviewList;

			EditorApplication.update += UpdateTime;
			EditorApplication.update += UpdateNodePreviewList;

			if( CurrentSelection == ASESelectionMode.ShaderFunction )
			{
				IsShaderFunctionWindow = true;
			}
			else
			{
				IsShaderFunctionWindow = false;
			}

			m_optionsWindow = new OptionsWindow( this );
			m_optionsWindow.Init();

			m_contextMenu = new GraphContextMenu( m_mainGraphInstance );
			m_nodesLoadedCorrectly = m_contextMenu.CorrectlyLoaded;

			m_paletteWindow = new PaletteWindow( this )
			{
				Resizable = true
			};
			m_paletteWindow.OnPaletteNodeCreateEvt += OnPaletteNodeCreate;
			m_registeredMenus.Add( m_paletteWindow );

			m_contextPalette = new ContextPalette( this );
			m_contextPalette.OnPaletteNodeCreateEvt += OnContextPaletteNodeCreate;
			m_registeredMenus.Add( m_contextPalette );

			m_genericMessageUI = new GenericMessageUI();
			m_genericMessageUI.OnMessageDisplayEvent += ShowMessageImmediately;

			Selection.selectionChanged += OnProjectSelectionChanged;
#if UNITY_2018_1_OR_NEWER
			EditorApplication.projectChanged += OnProjectWindowChanged;
#else
			EditorApplication.projectWindowChanged += OnProjectWindowChanged;
#endif
			m_focusOnSelectionTimestamp = EditorApplication.timeSinceStartup;
			m_focusOnMasterNodeTimestamp = EditorApplication.timeSinceStartup;

			m_nodeParametersWindow.IsMaximized = m_innerEditorVariables.NodeParametersMaximized;
			if( DebugConsoleWindow.UseShaderPanelsInfo )
				m_nodeParametersWindow.IsMaximized = m_nodeParametersWindowMaximized;

			m_paletteWindow.IsMaximized = m_innerEditorVariables.NodePaletteMaximized;
			if( DebugConsoleWindow.UseShaderPanelsInfo )
				m_paletteWindow.IsMaximized = m_paletteWindowMaximized;

			m_shortcutManager = new ShortcutsManager();
			// REGISTER NODE SHORTCUTS
			foreach( KeyValuePair<KeyCode, ShortcutKeyData> kvp in m_contextMenu.NodeShortcuts )
			{
				m_shortcutManager.RegisterNodesShortcuts( kvp.Key, kvp.Value.Name );
			}

			// REGISTER EDITOR SHORTCUTS

			m_shortcutManager.RegisterEditorShortcut( true, EventModifiers.FunctionKey, KeyCode.F1, "Open Selected Node Wiki page", () =>
			{
				List<ParentNode> selectedNodes = m_mainGraphInstance.SelectedNodes;
				if( selectedNodes != null && selectedNodes.Count == 1 )
				{
					Application.OpenURL( selectedNodes[ 0 ].Attributes.NodeUrl );
				}
			} );


			m_shortcutManager.RegisterEditorShortcut( true, KeyCode.C, "Create Commentary", () =>
			{
				// Create commentary
				ParentNode[] selectedNodes = m_mainGraphInstance.SelectedNodes.ToArray();
				UIUtils.MarkUndoAction();
				Undo.RegisterCompleteObjectUndo( this, "Adding Commentary Node" );
				CommentaryNode node = m_mainGraphInstance.CreateNode( m_commentaryTypeNode, true, -1, false ) as CommentaryNode;
				node.CreateFromSelectedNodes( TranformedMousePos, selectedNodes );
				node.Focus();
				m_mainGraphInstance.DeSelectAll();
				m_mainGraphInstance.SelectNode( node, false, false );
				SetSaveIsDirty();
				ForceRepaint();
			} );


			m_shortcutManager.RegisterEditorShortcut( true, KeyCode.F, "Focus On Selection", () =>
			{
				OnToolButtonPressed( ToolButtonType.FocusOnSelection );
				ForceRepaint();
			} );

			//m_shortcutManager.RegisterEditorShortcut( true, EventModifiers.None, KeyCode.B, "New Master Node", () =>
			//{
			//	OnToolButtonPressed( ToolButtonType.MasterNode );
			//	ForceRepaint();
			//} );

			m_shortcutManager.RegisterEditorShortcut( true, EventModifiers.None, KeyCode.Space, "Open Node Palette", null, () =>
			{
				m_contextPalette.Show( m_currentMousePos2D, m_cameraInfo );
			} );


			m_shortcutManager.RegisterEditorShortcut( true, KeyCode.W, "Toggle Colored Line Mode", () =>
			{
				m_optionsWindow.ColoredPorts = !m_optionsWindow.ColoredPorts;
				ForceRepaint();

			} );

			m_shortcutManager.RegisterEditorShortcut( true, EventModifiers.Control, KeyCode.W, "Toggle Multi-Line Mode", () =>
			{
				m_optionsWindow.MultiLinePorts = !m_optionsWindow.MultiLinePorts;
				ForceRepaint();
			} );

			m_shortcutManager.RegisterEditorShortcut( true, KeyCode.P, "Global Preview", () =>
			{
				GlobalPreview = !GlobalPreview;
				EditorPrefs.SetBool( "GlobalPreview", GlobalPreview );

				ForceRepaint();
			} );

			GlobalShowInternalData = EditorPrefs.GetBool( "GlobalShowInternalData", false );
			m_shortcutManager.RegisterEditorShortcut( true, KeyCode.I, "Global Show Internal Data", () =>
			{
				GlobalShowInternalData = !GlobalShowInternalData;
				EditorPrefs.SetBool( "GlobalShowInternalData", GlobalShowInternalData );
				ForceRepaint();
			} );

			m_shortcutManager.RegisterEditorShortcut( true,EventModifiers.FunctionKey, KeyCode.Delete, "Delete selected nodes", DeleteSelectedNodeWithRepaint );
			m_shortcutManager.RegisterEditorShortcut( true, EventModifiers.FunctionKey, KeyCode.Backspace, "Delete selected nodes", DeleteSelectedNodeWithRepaint );

			m_liveShaderEditing = m_innerEditorVariables.LiveMode;

			UpdateLiveUI();
		}


		public AmplifyShaderEditorWindow()
		{
			m_minNodePos = new Vector2( float.MaxValue, float.MaxValue );
			m_maxNodePos = new Vector2( float.MinValue, float.MinValue );

			m_duplicatePreventionBuffer = new DuplicatePreventionBuffer();
			m_commentaryTypeNode = typeof( CommentaryNode );
			titleContent = new GUIContent( "Shader Editor" );
			autoRepaintOnSceneChange = true;

			m_currentMousePos = new Vector3( 0, 0, 0 );
			m_keyEvtMousePos2D = new Vector2( 0, 0 );
			m_multipleSelectionStart = new Vector2( 0, 0 );
			m_initialized = false;
			m_graphBgTexture = null;
			m_graphFgTexture = null;

			m_cameraOffset = new Vector2( 0, 0 );
			CameraZoom = 1;

			m_registeredMenus = new List<MenuParent>();

			m_nodeParametersWindow = new NodeParametersWindow( this )
			{
				Resizable = true
			};
			m_registeredMenus.Add( m_nodeParametersWindow );

			m_modeWindow = new ShaderEditorModeWindow( this );
			//_registeredMenus.Add( _modeWindow );

			m_toolsWindow = new ToolsWindow( this );
			m_toolsWindow.ToolButtonPressedEvt += OnToolButtonPressed;

			m_consoleLogWindow = new ConsoleLogWindow( this );

			m_tipsWindow = new TipsWindow( this );

			m_registeredMenus.Add( m_toolsWindow );
			m_registeredMenus.Add( m_consoleLogWindow );

			m_palettePopup = new PalettePopUp();

			m_clipboard = new Clipboard();

			m_genericMessageContent = new GUIContent();
			m_dragAndDropTool = new DragAndDropTool();
			m_dragAndDropTool.OnValidDropObjectEvt += OnValidObjectsDropped;

			//_confirmationWindow = new ConfirmationWindow( 100, 100, 300, 100 );

			m_saveIsDirty = false;

			m_preMadeShaders = new PreMadeShaders();

			Undo.undoRedoPerformed += UndoRedoPerformed;

			float autoPanSpeed = 2;
			m_autoPanArea = new AutoPanData[ 4 ];
			m_autoPanArea[ 0 ] = new AutoPanData( AutoPanLocation.TOP, 25, autoPanSpeed * Vector2.up );
			m_autoPanArea[ 1 ] = new AutoPanData( AutoPanLocation.BOTTOM, 25, autoPanSpeed * Vector2.down );
			m_autoPanArea[ 2 ] = new AutoPanData( AutoPanLocation.LEFT, 25, autoPanSpeed * Vector2.right );
			m_autoPanArea[ 3 ] = new AutoPanData( AutoPanLocation.RIGHT, 25, autoPanSpeed * Vector2.left );

			m_drawInfo = new DrawInfo();
			UIUtils.CurrentWindow = this;

			m_nodeExporterUtils = new NodeExporterUtils( this );
			m_repaintIsDirty = false;
			m_initialized = false;
		}

		public void SetStandardShader()
		{
			m_mainGraphInstance.ReplaceMasterNode( AvailableShaderTypes.SurfaceShader );
			m_mainGraphInstance.FireMasterNodeReplacedEvent();
		}

		public void SetTemplateShader( string templateName, bool writeDefaultData )
		{
			TemplateDataParent templateData = m_templatesManager.GetTemplate( ( string.IsNullOrEmpty( templateName ) ? "6e114a916ca3e4b4bb51972669d463bf" : templateName ) );
			m_mainGraphInstance.ReplaceMasterNode( AvailableShaderTypes.Template, writeDefaultData, templateData );
		}

		public void DeleteSelectedNodeWithRepaint()
		{
			DeleteSelectedNodes();
			SetSaveIsDirty();
		}


		void UndoRedoPerformed()
		{
			m_repaintIsDirty = true;
			m_saveIsDirty = true;
			m_removedKeyboardFocus = true;
			m_refreshOnUndo = true;
		}

		void Destroy()
		{
			Undo.ClearUndo( this );

			m_initialized = false;
			
			m_nodeExporterUtils.Destroy();
			m_nodeExporterUtils = null;

			m_delayedMaterialSet = null;

			m_materialsToUpdate.Clear();
			m_materialsToUpdate = null;
			
			GLDraw.Destroy();

			UIUtils.Destroy();
			m_preMadeShaders.Destroy();
			m_preMadeShaders = null;

			m_registeredMenus.Clear();
			m_registeredMenus = null;

			m_mainGraphInstance.Destroy();
			m_mainGraphInstance = null;

			Resources.UnloadAsset( m_graphBgTexture );
			m_graphBgTexture = null;

			Resources.UnloadAsset( m_graphFgTexture );
			m_graphFgTexture = null;

			Resources.UnloadAsset( m_wireTexture );
			m_wireTexture = null;

			m_contextMenu.Destroy();
			m_contextMenu = null;

			m_shortcutManager.Destroy();
			m_shortcutManager = null;

			m_nodeParametersWindow.Destroy();
			m_nodeParametersWindow = null;

			m_modeWindow.Destroy();
			m_modeWindow = null;

			m_toolsWindow.Destroy();
			m_toolsWindow = null;

			m_consoleLogWindow.Destroy();
			m_consoleLogWindow = null;

			m_tipsWindow.Destroy();
			m_tipsWindow = null;

			m_optionsWindow.Destroy();
			m_optionsWindow = null;

			m_paletteWindow.Destroy();
			m_paletteWindow = null;

			m_palettePopup.Destroy();
			m_palettePopup = null;

			m_contextPalette.Destroy();
			m_contextPalette = null;

			m_clipboard.ClearClipboard();
			m_clipboard = null;

			m_genericMessageUI.Destroy();
			m_genericMessageUI = null;
			m_genericMessageContent = null;

			m_dragAndDropTool.Destroy();
			m_dragAndDropTool = null;

			m_openedShaderFunction = null;

			UIUtils.CurrentWindow = null;
			m_duplicatePreventionBuffer.ReleaseAllData();
			m_duplicatePreventionBuffer = null;
#if UNITY_2018_1_OR_NEWER
			EditorApplication.projectChanged -= OnProjectWindowChanged;
#else
			EditorApplication.projectWindowChanged -= OnProjectWindowChanged;
#endif
			Selection.selectionChanged -= OnProjectSelectionChanged;

			IOUtils.AllOpenedWindows.Remove( this );

			if( IOUtils.AllOpenedWindows.Count == 0 )
			{
				m_templatesManager.Destroy();
				ScriptableObject.DestroyImmediate( m_templatesManager );
			}
			m_templatesManager = null;

			IOUtils.Destroy();

			Resources.UnloadUnusedAssets();
			GC.Collect();
		}

		void Init()
		{
			// = AssetDatabase.LoadAssetAtPath( Constants.ASEPath + "", typeof( Texture2D ) ) as Texture2D;
			m_graphBgTexture = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( IOUtils.GraphBgTextureGUID ), typeof( Texture2D ) ) as Texture2D;
			if( m_graphBgTexture != null )
			{
				m_graphFgTexture = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( IOUtils.GraphFgTextureGUID ), typeof( Texture2D ) ) as Texture2D;

				//Setup usable area
				m_cameraInfo = position;
				m_graphArea = new Rect( 0, 0, m_cameraInfo.width, m_cameraInfo.height );

				// Creating style state to show current selected object
				m_graphFontStyle = new GUIStyle()
				{
					fontSize = 32,
					alignment = TextAnchor.MiddleCenter,
					fixedWidth = m_cameraInfo.width,
					fixedHeight = 50,
					stretchWidth = true,
					stretchHeight = true
				};
				m_graphFontStyle.normal.textColor = Color.white;

				m_wireTexture = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( IOUtils.WireTextureGUID ), typeof( Texture2D ) ) as Texture2D;

				m_initialized = m_graphBgTexture != null &&
								m_graphFgTexture != null &&
								m_wireTexture != null;
			}
		}

		[OnOpenAssetAttribute()]
		static bool OnOpenAsset( int instanceID, int line )
		{
			if( line > -1 )
			{
				return false;
			}

			Shader selectedShader = Selection.activeObject as Shader;
			if( selectedShader != null )
			{
				if( IOUtils.IsASEShader( selectedShader ) )
				{
					ConvertShaderToASE( selectedShader );
					return true;
				}
			}
			else
			{
				Material mat = Selection.activeObject as Material;
				if( mat != null )
				{
					if( IOUtils.IsASEShader( mat.shader ) )
					{
						LoadMaterialToASE( mat );
						return true;
					}
				}
				else
				{
					AmplifyShaderFunction shaderFunction = Selection.activeObject as AmplifyShaderFunction;
					if( shaderFunction != null )
					{
						if( IOUtils.IsShaderFunction( shaderFunction.FunctionInfo ) )
						{
							LoadShaderFunctionToASE( shaderFunction, false );
							return true;
						}
					}
				}
			}
			return false;
		}


		[MenuItem( "Assets/Create/Amplify Shader/Surface", false, 84 )]
		[MenuItem( "Assets/Create/Shader/Amplify Surface Shader" )]
		public static void CreateNewShader()
		{

			string path = string.Empty; 
			if( Selection.activeObject != null )
			{
				path = ( IOUtils.dataPath + AssetDatabase.GetAssetPath( Selection.activeObject ) );
			}
			else
			{
				UnityEngine.Object[] selection = Selection.GetFiltered( typeof( UnityEngine.Object ), SelectionMode.DeepAssets );
				if( selection.Length > 0 && selection[ 0 ] != null )
				{
					path = ( IOUtils.dataPath + AssetDatabase.GetAssetPath( selection[0] ) );
				}
				else
				{
					path = Application.dataPath;
				}

			}

			if( path.IndexOf( '.' ) > -1 )
			{
				path = path.Substring( 0, path.LastIndexOf( '/' ) );
			}
			path += "/";
			
			if( IOUtils.AllOpenedWindows.Count > 0 )
			{
				EditorWindow openedWindow = AmplifyShaderEditorWindow.GetWindow<AmplifyShaderEditorWindow>();
				AmplifyShaderEditorWindow currentWindow = CreateTab();
				WindowHelper.AddTab( openedWindow, currentWindow );
				UIUtils.CurrentWindow = currentWindow;
				Shader shader = UIUtils.CreateNewEmpty( path );
				Selection.activeObject = shader;
			}
			else
			{
				AmplifyShaderEditorWindow currentWindow = OpenWindow();
				UIUtils.CurrentWindow = currentWindow;
				Shader shader = UIUtils.CreateNewEmpty( path );
				Selection.activeObject = shader;
			}
			//Selection.objects = new UnityEngine.Object[] { shader };
		}



		public static void CreateNewTemplateShader( string templateGUID )
		{
			string path = Selection.activeObject == null ? Application.dataPath : ( IOUtils.dataPath + AssetDatabase.GetAssetPath( Selection.activeObject ) );
			if( path.IndexOf( '.' ) > -1 )
			{
				path = path.Substring( 0, path.LastIndexOf( '/' ) );
			}
			path += "/";

			if( IOUtils.AllOpenedWindows.Count > 0 )
			{
				EditorWindow openedWindow = AmplifyShaderEditorWindow.GetWindow<AmplifyShaderEditorWindow>();
				AmplifyShaderEditorWindow currentWindow = CreateTab();
				WindowHelper.AddTab( openedWindow, currentWindow );
				UIUtils.CurrentWindow = currentWindow;
				Shader shader = UIUtils.CreateNewEmptyTemplate( templateGUID, path );
				Selection.activeObject = shader;
			}
			else
			{
				AmplifyShaderEditorWindow currentWindow = OpenWindow();
				UIUtils.CurrentWindow = currentWindow;
				Shader shader = UIUtils.CreateNewEmptyTemplate( templateGUID, path );
				Selection.activeObject = shader;
			}

			//Selection.objects = new UnityEngine.Object[] { shader };
		}

		[MenuItem( "Assets/Create/Amplify Shader Function", false, 84 )]
		[MenuItem( "Assets/Create/Shader/Amplify Shader Function" )]
		public static void CreateNewShaderFunction()
		{
			AmplifyShaderFunction asset = ScriptableObject.CreateInstance<AmplifyShaderFunction>();

			string path = AssetDatabase.GetAssetPath( Selection.activeObject );
			if( path == "" )
			{
				path = "Assets";
			}
			else if( System.IO.Path.GetExtension( path ) != "" )
			{
				path = path.Replace( System.IO.Path.GetFileName( AssetDatabase.GetAssetPath( Selection.activeObject ) ), "" );
			}

			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath( path + "/New ShaderFunction.asset" );

			var endNameEditAction = ScriptableObject.CreateInstance<DoCreateFunction>();
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists( asset.GetInstanceID(), endNameEditAction, assetPathAndName, AssetPreview.GetMiniThumbnail( asset ), null );
		}

		public void UpdateTabTitle( string newTitle, bool modified )
		{
			string[] titleArray = newTitle.Split( '/' );
			newTitle = titleArray[ titleArray.Length - 1 ];

			if( !( m_currentTitle.Equals( newTitle ) && m_currentTitleMod == modified ) )
			{
				this.titleContent.text = GenerateTabTitle( newTitle, modified );
			}
			m_currentTitle = newTitle;
			m_currentTitleMod = modified;
		}

		public void OnProjectWindowChanged()
		{
			Shader selectedShader = Selection.activeObject as Shader;
			if( selectedShader != null )
			{
				if( m_mainGraphInstance != null && m_mainGraphInstance.CurrentMasterNode != null && selectedShader == m_mainGraphInstance.CurrentMasterNode.CurrentShader )
				{
					m_lastOpenedLocation = AssetDatabase.GetAssetPath( selectedShader );
				}
			}
		}

		public void LoadProjectSelected( UnityEngine.Object selectedObject = null )
		{
			bool hasFocus = true;
			if( EditorWindow.focusedWindow != this )
			{
				hasFocus = false;
			}

			if( hasFocus && m_mainGraphInstance != null && m_mainGraphInstance.CurrentMasterNode != null )
			{
				LoadObject( selectedObject ?? Selection.activeObject );
			}
			else
			{
				m_delayedLoadObject = selectedObject ?? Selection.activeObject;
			}

			if( !hasFocus )
				Focus();
		}

		public void LoadObject( UnityEngine.Object objToLoad )
		{
			Shader selectedShader = objToLoad as Shader;
			Material selectedMaterial = objToLoad as Material;
			AmplifyShaderFunction selectedFunction = objToLoad as AmplifyShaderFunction;

			if( selectedFunction != null )
			{
				IsShaderFunctionWindow = true;
				m_mainGraphInstance.CurrentCanvasMode = NodeAvailability.ShaderFunction;
			}
			else
			{
				IsShaderFunctionWindow = false;
			}

			ASESelectionMode selectedFileType = ASESelectionMode.Shader;
			if( selectedShader != null )
			{
				selectedFileType = ASESelectionMode.Shader;
			}
			else if( selectedMaterial != null )
			{
				selectedFileType = ASESelectionMode.Material;
			}
			else if( selectedFunction != null )
			{
				selectedFileType = ASESelectionMode.ShaderFunction;
			}


			switch( CurrentSelection )
			{
				case ASESelectionMode.Shader:
				{
					if( ShaderIsModified )
					{
						Shader currShader = m_mainGraphInstance.CurrentMasterNode.CurrentShader;
						bool savePrevious = UIUtils.DisplayDialog( AssetDatabase.GetAssetPath( currShader ) );
						OnSaveShader( savePrevious, currShader, null, null );
					}
				}
				break;
				case ASESelectionMode.Material:
				{
					if( ShaderIsModified )
					{
						Shader currShader = m_mainGraphInstance.CurrentMasterNode.CurrentShader;
						bool savePrevious = UIUtils.DisplayDialog( AssetDatabase.GetAssetPath( currShader ) );
						OnSaveShader( savePrevious, currShader, m_mainGraphInstance.CurrentMasterNode.CurrentMaterial, null );
					}
				}
				break;
				case ASESelectionMode.ShaderFunction:
				{
					if( ShaderIsModified )
					{
						bool savePrevious = UIUtils.DisplayDialog( AssetDatabase.GetAssetPath( m_openedShaderFunction ) );
						OnSaveShader( savePrevious, null, null, selectedFunction );
					}
				}
				break;
			}

			switch( selectedFileType )
			{
				case ASESelectionMode.Shader:
				{
					LoadDroppedObject( true, selectedShader, null );
				}
				break;
				case ASESelectionMode.Material:
				{
					LoadDroppedObject( true, selectedMaterial.shader, selectedMaterial );
				}
				break;
				case ASESelectionMode.ShaderFunction:
				{
					LoadDroppedObject( true, null, null, selectedFunction );
				}
				break;
			}

			m_openedShaderFunction = m_mainGraphInstance.CurrentShaderFunction;

			//Need to force one graph draw because it wont call OnGui propertly since its focuses somewhere else
			// Focus() doesn't fix this since it only changes keyboard focus
			m_drawInfo.InvertedZoom = 1 / m_cameraZoom;
			m_mainGraphInstance.Draw( m_drawInfo );

			ShaderIsModified = false;
			Focus();
			Repaint();
		}

		public void OnProjectSelectionChanged()
		{
			if( m_loadShaderOnSelection )
			{
				LoadProjectSelected();
			}
		}

		ShaderLoadResult OnSaveShader( bool value, Shader shader, Material material, AmplifyShaderFunction function )
		{
			if( value )
			{
				SaveToDisk( false );
			}

			return value ? ShaderLoadResult.LOADED : ShaderLoadResult.FILE_NOT_FOUND;
		}

		public void ResetCameraSettings()
		{
			m_cameraInfo = position;
			m_cameraOffset = new Vector2( m_cameraInfo.width * 0.5f, m_cameraInfo.height * 0.5f );
			CameraZoom = 1;
		}

		public void Reset()
		{
			if( m_mainGraphInstance == null )
			{
				m_mainGraphInstance = CreateInstance<ParentGraph>();
				m_mainGraphInstance.Init();
				m_mainGraphInstance.ParentWindow = this;
				m_mainGraphInstance.SetGraphId( 0 );
			}
			m_mainGraphInstance.ResetEvents();
			m_mainGraphInstance.OnNodeEvent += OnNodeStoppedMovingEvent;
			m_mainGraphInstance.OnMaterialUpdatedEvent += OnMaterialUpdated;
			m_mainGraphInstance.OnShaderUpdatedEvent += OnShaderUpdated;
			m_mainGraphInstance.OnEmptyGraphDetectedEvt += OnEmptyGraphDetected;
			m_mainGraphInstance.OnNodeRemovedEvent += m_toolsWindow.OnNodeRemovedFromGraph;
			GraphCount = 1;

			FullCleanUndoStack();
			m_performFullUndoRegister = true;
			m_toolsWindow.BorderStyle = null;
			m_selectionMode = ASESelectionMode.Shader;
			ResetCameraSettings();
			UIUtils.ResetMainSkin();
			m_duplicatePreventionBuffer.ReleaseAllData();
			if( m_genericMessageUI != null )
				m_genericMessageUI.CleanUpMessageStack();
		}

		public Shader CreateNewGraph( string name )
		{
			Reset();
			UIUtils.DirtyMask = false;
			m_mainGraphInstance.CreateNewEmpty( name );
			m_lastOpenedLocation = string.Empty;
			UIUtils.DirtyMask = true;
			return m_mainGraphInstance.CurrentMasterNode.CurrentShader;
		}

		public Shader CreateNewTemplateGraph( string templateGUID )
		{
			Reset();
			UIUtils.DirtyMask = false;
			m_mainGraphInstance.CreateNewEmptyTemplate( templateGUID );
			m_lastOpenedLocation = string.Empty;
			UIUtils.DirtyMask = true;
			return m_mainGraphInstance.CurrentMasterNode.CurrentShader;
		}

		public Shader CreateNewGraph( Shader shader )
		{
			Reset();
			UIUtils.DirtyMask = false;
			m_mainGraphInstance.CreateNewEmpty( shader.name );
			m_mainGraphInstance.CurrentMasterNode.CurrentShader = shader;

			m_lastOpenedLocation = string.Empty;
			UIUtils.DirtyMask = true;
			return m_mainGraphInstance.CurrentMasterNode.CurrentShader;
		}

		public void CreateNewFunctionGraph( AmplifyShaderFunction shaderFunction )
		{
			Reset();
			UIUtils.DirtyMask = false;
			m_mainGraphInstance.CreateNewEmptyFunction( shaderFunction );
			m_mainGraphInstance.CurrentShaderFunction = shaderFunction;

			m_lastOpenedLocation = AssetDatabase.GetAssetPath( shaderFunction ); //string.Empty;
			UIUtils.DirtyMask = true;
			//return m_mainGraphInstance.CurrentMasterNode.CurrentShader;
		}

		public bool SaveToDisk( bool checkTimestamp )
		{
			if( checkTimestamp )
			{
				if( !m_cacheSaveOp )
				{
					m_lastTimeSaved = EditorApplication.timeSinceStartup;
					m_cacheSaveOp = true;
				}
				return false;
			}


			System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

			m_customGraph = null;
			m_cacheSaveOp = false;
			ShaderIsModified = false;
			m_mainGraphInstance.LoadedShaderVersion = VersionInfo.FullNumber;
			m_lastTimeSaved = EditorApplication.timeSinceStartup;

			if( m_mainGraphInstance.CurrentMasterNodeId == Constants.INVALID_NODE_ID )
			{
				Shader currentShader = m_mainGraphInstance.CurrentMasterNode != null ? m_mainGraphInstance.CurrentMasterNode.CurrentShader : null;
				string newShader;
				if( !String.IsNullOrEmpty( m_lastOpenedLocation ) )
				{
					newShader = m_lastOpenedLocation;
				}
				else if( currentShader != null )
				{
					newShader = AssetDatabase.GetAssetPath( currentShader );
				}
				else
				{
					newShader = EditorUtility.SaveFilePanel( "Select Shader to save", Application.dataPath, "MyShader", "shader" );
				}

				if( !String.IsNullOrEmpty( newShader ) )
				{
					ShowMessage( "No Master node assigned.\nShader file will only have node info" );
					IOUtils.StartSaveThread( GenerateGraphInfo(), newShader );
					AssetDatabase.Refresh();
					LoadFromDisk( newShader );
					System.Threading.Thread.CurrentThread.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;
					return true;
				}
			}
			else if( m_mainGraphInstance.CurrentMasterNode != null )
			{
				//m_mainGraphInstance.CurrentStandardSurface.ForceReordering();
				Shader currShader = m_mainGraphInstance.CurrentMasterNode.CurrentShader;
				if( currShader != null )
				{
					m_mainGraphInstance.FireMasterNode( currShader );
					Material material = m_mainGraphInstance.CurrentMaterial;
					m_lastpath = ( material != null ) ? AssetDatabase.GetAssetPath( material ) : AssetDatabase.GetAssetPath( currShader );
					EditorPrefs.SetString( IOUtils.LAST_OPENED_OBJ_ID, m_lastpath );
					System.Threading.Thread.CurrentThread.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;
					return true;
				}
				else
				{
					string shaderName;
					string pathName;
					IOUtils.GetShaderName( out shaderName, out pathName, Constants.DefaultShaderName, UIUtils.LatestOpenedFolder );
					if( !String.IsNullOrEmpty( pathName ) )
					{
						UIUtils.CurrentWindow.CurrentGraph.CurrentMasterNode.SetName( shaderName );
						m_mainGraphInstance.FireMasterNode( pathName, true );
						m_lastpath = pathName;
						EditorPrefs.SetString( IOUtils.LAST_OPENED_OBJ_ID, pathName );
						System.Threading.Thread.CurrentThread.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;
						return true;
					}
				}
			}
			else
			{
				//m_nodeParametersWindow.ForceReordering();
				m_mainGraphInstance.ResetNodesLocalVariables();

				List<FunctionInput> functionInputNodes = UIUtils.FunctionInputList();
				functionInputNodes.Sort( ( x, y ) => { return x.OrderIndex.CompareTo( y.OrderIndex ); } );
				for( int i = 0; i < functionInputNodes.Count; i++ )
				{
					functionInputNodes[ i ].OrderIndex = i;
				}

				List<FunctionOutput> functionOutputNodes = UIUtils.FunctionOutputList();
				functionOutputNodes.Sort( ( x, y ) => { return x.OrderIndex.CompareTo( y.OrderIndex ); } );
				for( int i = 0; i < functionOutputNodes.Count; i++ )
				{
					functionOutputNodes[ i ].OrderIndex = i;
				}

				m_mainGraphInstance.CurrentShaderFunction.AdditionalDirectives.UpdateSaveItemsFromDirectives();
				m_mainGraphInstance.CurrentShaderFunction.FunctionInfo = GenerateGraphInfo();
				m_mainGraphInstance.CurrentShaderFunction.FunctionInfo = IOUtils.AddAdditionalInfo( m_mainGraphInstance.CurrentShaderFunction.FunctionInfo );

				if( AssetDatabase.IsMainAsset( m_mainGraphInstance.CurrentShaderFunction ) )
				{
					EditorUtility.SetDirty( m_mainGraphInstance.CurrentShaderFunction );
				}
				else
				{
					//Debug.Log( LastOpenedLocation );
					//AssetDatabase.CreateAsset( m_mainGraphInstance.CurrentShaderFunction, LastOpenedLocation );
				}

				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				IOUtils.FunctionNodeChanged = true;
				m_lastpath = AssetDatabase.GetAssetPath( m_mainGraphInstance.CurrentShaderFunction );
				System.Threading.Thread.CurrentThread.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;
				//EditorPrefs.SetString( IOUtils.LAST_OPENED_OBJ_ID, AssetDatabase.GetAssetPath( m_mainGraphInstance.CurrentShaderFunction ) );
				return true;
			}
			System.Threading.Thread.CurrentThread.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;
			return false;
		}

		public void OnToolButtonPressed( ToolButtonType type )
		{
			switch( type )
			{
				case ToolButtonType.New:
				{
					UIUtils.CreateNewEmpty();
				}
				break;
				case ToolButtonType.Open:
				{
					UIUtils.OpenFile();
				}
				break;
				case ToolButtonType.Save:
				{
					SaveToDisk( false );
				}
				break;
				case ToolButtonType.Library:
				{
					ShowShaderLibrary();
				}
				break;
				case ToolButtonType.Options: { } break;
				case ToolButtonType.Update:
				{
					SaveToDisk( false );
				}
				break;
				case ToolButtonType.Live:
				{
					m_liveShaderEditing = !m_liveShaderEditing;
					m_innerEditorVariables.LiveMode = m_liveShaderEditing;
					// 0 off
					// 1 on
					// 2 pending
					if( m_liveShaderEditing && m_mainGraphInstance.CurrentMasterNode.CurrentShader == null )
					{
						m_liveShaderEditing = false;
					}

					UpdateLiveUI();

					if( m_liveShaderEditing )
					{
						SaveToDisk( false );
					}
				}
				break;
				case ToolButtonType.OpenSourceCode:
				{
					AssetDatabase.OpenAsset( m_mainGraphInstance.CurrentMasterNode.CurrentShader, 1 );
				}
				break;
				case ToolButtonType.MasterNode:
				{
					m_mainGraphInstance.AssignMasterNode();
				}
				break;

				case ToolButtonType.FocusOnMasterNode:
				{
					double currTime = EditorApplication.timeSinceStartup;
					bool autoZoom = ( currTime - m_focusOnMasterNodeTimestamp ) < AutoZoomTime;
					m_focusOnMasterNodeTimestamp = currTime;
					FocusOnNode( m_mainGraphInstance.CurrentMasterNode, autoZoom ? 1 : m_cameraZoom, true );
				}
				break;

				case ToolButtonType.FocusOnSelection:
				{

					List<ParentNode> selectedNodes = ( m_mainGraphInstance.SelectedNodes.Count > 0 ) ? m_mainGraphInstance.SelectedNodes : m_mainGraphInstance.AllNodes;

					Vector2 minPos = new Vector2( float.MaxValue, float.MaxValue );
					Vector2 maxPos = new Vector2( float.MinValue, float.MinValue );
					Vector2 centroid = Vector2.zero;

					for( int i = 0; i < selectedNodes.Count; i++ )
					{
						Rect currPos = selectedNodes[ i ].TruePosition;

						minPos.x = ( currPos.x < minPos.x ) ? currPos.x : minPos.x;
						minPos.y = ( currPos.y < minPos.y ) ? currPos.y : minPos.y;

						maxPos.x = ( ( currPos.x + currPos.width ) > maxPos.x ) ? ( currPos.x + currPos.width ) : maxPos.x;
						maxPos.y = ( ( currPos.y + currPos.height ) > maxPos.y ) ? ( currPos.y + currPos.height ) : maxPos.y;

					}

					centroid = ( maxPos - minPos );

					double currTime = EditorApplication.timeSinceStartup;
					bool autoZoom = ( currTime - m_focusOnSelectionTimestamp ) < AutoZoomTime;
					m_focusOnSelectionTimestamp = currTime;

					float zoom = m_cameraZoom;
					if( autoZoom )
					{
						zoom = 1f;
						float canvasWidth = m_cameraInfo.width;
						if( m_nodeParametersWindow.IsMaximized )
							canvasWidth -= m_nodeParametersWindow.RealWidth;
						if( m_paletteWindow.IsMaximized )
							canvasWidth -= m_paletteWindow.RealWidth;
						canvasWidth -= 40;
						//float canvasWidth = AvailableCanvasWidth;// - 20;
						float canvasHeight = AvailableCanvasHeight - 60;
						if( centroid.x > canvasWidth ||
							centroid.y > canvasHeight )
						{
							float hZoom = float.MinValue;
							float vZoom = float.MinValue;
							if( centroid.x > canvasWidth )
							{
								hZoom = ( centroid.x ) / canvasWidth;
							}

							if( centroid.y > canvasHeight )
							{
								vZoom = ( centroid.y ) / canvasHeight;
							}
							zoom = ( hZoom > vZoom ) ? hZoom : vZoom;
						}
					}

					minPos.y -= 20 * zoom;
					if( m_nodeParametersWindow.IsMaximized )
						minPos.x -= m_nodeParametersWindow.RealWidth * 0.5f * zoom;
					if( m_paletteWindow.IsMaximized )
						minPos.x += m_paletteWindow.RealWidth * 0.5f * zoom;

					FocusOnPoint( minPos + centroid * 0.5f, zoom );
				}
				break;
				case ToolButtonType.ShowInfoWindow:
				{
					PortLegendInfo.OpenWindow();
				}
				break;
				case ToolButtonType.ShowTipsWindow:
				{
					TipsWindow.ShowWindow( true );
				}
				break;
				case ToolButtonType.ShowConsole:
				{
					m_consoleLogWindow.Toggle();
				}
				break;
				case ToolButtonType.CleanUnusedNodes:
				{
					m_mainGraphInstance.CleanUnusedNodes();
				}
				break;
				case ToolButtonType.Help:
				{
					Application.OpenURL( Constants.HelpURL );
				}
				break;
			}
		}

		void UpdateLiveUI()
		{
			if( m_toolsWindow != null )
			{
				m_toolsWindow.SetStateOnButton( ToolButtonType.Live, ( m_liveShaderEditing ) ? 1 : 0 );
			}
		}

		public void FocusOnNode( ParentNode node, float zoom, bool selectNode, bool late = false )
		{
			if( late )
			{
				m_nodeToFocus = node;
				m_zoomToFocus = zoom;
				m_selectNodeToFocus = selectNode;
				return;
			}

			if( selectNode )
			{
				m_mainGraphInstance.SelectNode( node, false, false );
			}
			FocusOnPoint( node.CenterPosition, zoom );
		}

		public void FocusOnPoint( Vector2 point, float zoom, bool smooth = true )
		{
			if( zoom > 0.999f )
			{
				//CameraZoom = zoom;
				if( smooth )
					SmoothZoom( zoom );
				else
					CameraZoom = zoom;
			}

			if( smooth )
				SmoothCameraOffset( -point + new Vector2( ( m_cameraInfo.width ) * 0.5f, m_cameraInfo.height * 0.5f ) * CameraZoom );
			else
				//m_cameraOffset = -point + new Vector2( ( m_cameraInfo.width ) * 0.5f, m_cameraInfo.height * 0.5f ) * CameraZoom;
				m_cameraOffset = -point + new Vector2( ( m_cameraInfo.width + m_nodeParametersWindow.RealWidth - m_paletteWindow.RealWidth ) * 0.5f, m_cameraInfo.height * 0.5f ) * CameraZoom;
		}

		void SmoothZoom( float newZoom )
		{
			m_smoothZoom = true;
			m_zoomTime = 0;
			m_targetZoom = newZoom;
			m_zoomPivot = m_graphArea.center;
		}

		void SmoothCameraOffset( Vector2 newOffset )
		{
			m_smoothOffset = true;
			m_offsetTime = 0;
			m_targetOffset = newOffset;
		}

		void PreTestLeftMouseDown()
		{
			if( m_currentEvent.type == EventType.MouseDown && m_currentEvent.button == ButtonClickId.LeftMouseButton )
			{
				ParentNode node = m_mainGraphInstance.CheckNodeAt( m_currentMousePos );
				if( node != null )
				{
					m_mainGraphInstance.NodeClicked = node.UniqueId;
					return;
				}
			}

			m_mainGraphInstance.NodeClicked = -1;
		}



		void OnLeftMouseDown()
		{
			Focus();
			m_mouseDownOnValidArea = true;
			m_lmbPressed = true;
			if( m_currentEvent.alt )
			{
				m_altBoxSelection = true;
			}

			UIUtils.ShowContextOnPick = true;
			ParentNode node = ( m_mainGraphInstance.NodeClicked < 0 ) ? m_mainGraphInstance.CheckNodeAt( m_currentMousePos ) : m_mainGraphInstance.GetClickedNode();
			if( node != null )
			{
				m_mainGraphInstance.NodeClicked = node.UniqueId;
				m_altBoxSelection = false;

				if( m_contextMenu.CheckShortcutKey() )
				{
					if( node.ConnStatus == NodeConnectionStatus.Island )
					{
						if( !m_multipleSelectionActive )
						{
							ParentNode newNode = m_contextMenu.CreateNodeFromShortcutKey();
							if( newNode != null )
							{
								newNode.ContainerGraph = m_mainGraphInstance;
								newNode.Vec2Position = TranformedMousePos;
								m_mainGraphInstance.AddNode( newNode, true );
								m_mainGraphInstance.SelectNode( newNode, false, false );
								ForceRepaint();
							}
							( node as CommentaryNode ).AddNodeToCommentary( newNode );
						}
					}
				}
				else
				{
					if( node.OnClick( m_currentMousePos2D ) )
					{
						if( !node.Selected )
						{
							m_mainGraphInstance.SelectNode( node, ( m_currentEvent.modifiers == EventModifiers.Shift || m_currentEvent.modifiers == EventModifiers.Control ), true );
						}
						else if( m_currentEvent.modifiers == EventModifiers.Shift || m_currentEvent.modifiers == EventModifiers.Control )
						{
							m_mainGraphInstance.DeselectNode( node );
						}

						if( m_currentEvent.alt )
						{
							int conn = 0;
							for( int i = 0; i < node.InputPorts.Count; i++ )
							{
								if( node.InputPorts[ i ].IsConnected )
									conn++;
							}

							if( node.InputPorts.Count > 0 && node.OutputPorts.Count > 0 && conn > 0 && node.OutputPorts[ 0 ].IsConnected )
							{
								m_altDragStarted = true;
							}
						}

					}

					if( m_currentEvent.alt )
					{
						if( node.InputPorts.Count > 0 && node.OutputPorts.Count > 0 && node.InputPorts[ 0 ].IsConnected && node.OutputPorts[ 0 ].IsConnected )
						{
							m_altDragStarted = true;
						}
					}

					return;
				}
			}
			else if( !m_multipleSelectionActive )
			{
				ParentNode newNode = m_contextMenu.CreateNodeFromShortcutKey();
				if( newNode != null )
				{
					newNode.ContainerGraph = m_mainGraphInstance;
					newNode.Vec2Position = TranformedMousePos;
					m_mainGraphInstance.AddNode( newNode, true );
					m_mainGraphInstance.SelectNode( newNode, false, false );
					SetSaveIsDirty();
					ForceRepaint();
				}
				else
				{
					List<WireBezierReference> wireRefs = m_mainGraphInstance.GetWireBezierListInPos( m_currentMousePos2D );
					if( wireRefs != null && wireRefs.Count > 0 )
					{
						for( int i = 0; i < wireRefs.Count; i++ )
						{
							// Place wire code here
							ParentNode outNode = m_mainGraphInstance.GetNode( wireRefs[ i ].OutNodeId );
							ParentNode inNode = m_mainGraphInstance.GetNode( wireRefs[ i ].InNodeId );

							OutputPort outputPort = outNode.GetOutputPortByUniqueId( wireRefs[ i ].OutPortId );
							InputPort inputPort = inNode.GetInputPortByUniqueId( wireRefs[ i ].InPortId );

							// Calculate the 4 points for bezier taking into account wire nodes and their automatic tangents
							Vector3 endPos = new Vector3( inputPort.Position.x, inputPort.Position.y );
							Vector3 startPos = new Vector3( outputPort.Position.x, outputPort.Position.y );

							float mag = ( endPos - startPos ).magnitude;
							float resizedMag = Mathf.Min( mag, Constants.HORIZONTAL_TANGENT_SIZE * m_drawInfo.InvertedZoom );

							Vector3 startTangent = new Vector3( startPos.x + resizedMag, startPos.y );
							Vector3 endTangent = new Vector3( endPos.x - resizedMag, endPos.y );

							if( inNode != null && inNode.GetType() == typeof( WireNode ) )
								endTangent = endPos + ( ( inNode as WireNode ).TangentDirection ) * mag * 0.33f;

							if( outNode != null && outNode.GetType() == typeof( WireNode ) )
								startTangent = startPos - ( ( outNode as WireNode ).TangentDirection ) * mag * 0.33f;

							float dist = HandleUtility.DistancePointBezier( m_currentMousePos, startPos, endPos, startTangent, endTangent );
							if( dist < 10 )
							{
								double doubleTapTime = EditorApplication.timeSinceStartup;
								bool doubleTap = ( doubleTapTime - m_wiredDoubleTapTimestamp ) < WiredDoubleTapTime;
								m_wiredDoubleTapTimestamp = doubleTapTime;

								if( doubleTap )
								{
									Undo.RegisterCompleteObjectUndo( this, Constants.UndoCreateConnectionId );
									Undo.RegisterCompleteObjectUndo( m_mainGraphInstance, Constants.UndoCreateConnectionId );
									Undo.RecordObject( outNode, Constants.UndoCreateConnectionId );
									Undo.RecordObject( inNode, Constants.UndoCreateConnectionId );
									
									ParentNode wireNode = m_mainGraphInstance.CreateNode( typeof( WireNode ), true );
									if( wireNode != null )
									{
										wireNode.Vec2Position = TranformedMousePos;

										m_mainGraphInstance.CreateConnection( wireNode.InputPorts[ 0 ].NodeId, wireNode.InputPorts[ 0 ].PortId, outputPort.NodeId, outputPort.PortId );
										m_mainGraphInstance.CreateConnection( inputPort.NodeId, inputPort.PortId, wireNode.OutputPorts[ 0 ].NodeId, wireNode.OutputPorts[ 0 ].PortId );

										SetSaveIsDirty();
										ForceRepaint();
										Undo.IncrementCurrentGroup();
									}
								}

								break;
							}
						}
					}
					//Reset focus from any textfield which may be selected at this time
					GUIUtility.keyboardControl = 0;
				}
			}

			if( m_currentEvent.modifiers != EventModifiers.Shift && m_currentEvent.modifiers != EventModifiers.Control && !m_altBoxSelection )
				m_mainGraphInstance.DeSelectAll();

			if( m_wireReferenceUtils.ValidReferences() )
			{
				m_wireReferenceUtils.InvalidateReferences();
				return;
			}

			if( !m_contextMenu.CheckShortcutKey() && m_currentEvent.modifiers != EventModifiers.Shift && m_currentEvent.modifiers != EventModifiers.Control || m_altBoxSelection )
			{
				// Only activate multiple selection if no node is selected and shift key not pressed
				m_multipleSelectionActive = true;

				m_multipleSelectionStart = TranformedMousePos;
				m_multipleSelectionArea.position = m_multipleSelectionStart;
				m_multipleSelectionArea.size = Vector2.zero;
			}

			UseCurrentEvent();
		}

		void OnLeftMouseDrag()
		{
			if( m_lostFocus )
			{
				m_lostFocus = false;
				return;
			}

			if( m_altDragStarted )
			{
				m_altDragStarted = false;

				if( m_currentEvent.modifiers == EventModifiers.Alt && CurrentGraph.SelectedNodes.Count == 1 )
				{
					ParentNode node = CurrentGraph.SelectedNodes[ 0 ];
					int lastId = 0;
					int conn = 0;
					for( int i = 0; i < node.InputPorts.Count; i++ )
					{
						if( node.InputPorts[ i ].IsConnected )
						{
							conn++;
							lastId = i;
						}
					}

					if( conn > 1 )
						lastId = 0;



					OutputPort outputPort = node.InputPorts[ lastId ].GetOutputConnection( 0 );
					ParentNode outputNode = m_mainGraphInstance.GetNode( outputPort.NodeId );

					Undo.RegisterCompleteObjectUndo( this, Constants.UndoCreateConnectionId );
					node.RecordObject( Constants.UndoCreateConnectionId );
					outputNode.RecordObject( Constants.UndoCreateConnectionId );

					List<InputPort> inputPorts = new List<InputPort>();
					for( int i = 0; i < node.OutputPorts[ 0 ].ConnectionCount; i++ )
					{
						InputPort inputPort = node.OutputPorts[ 0 ].GetInputConnection( i );
						ParentNode inputNode = m_mainGraphInstance.GetNode( inputPort.NodeId );
						inputNode.RecordObject( Constants.UndoCreateConnectionId );
						inputPorts.Add( inputPort );
					}

					for( int i = 0; i < inputPorts.Count; i++ )
					{
						m_mainGraphInstance.CreateConnection( inputPorts[ i ].NodeId, inputPorts[ i ].PortId, outputPort.NodeId, outputPort.PortId );
					}

					UIUtils.DeleteConnection( true, node.UniqueId, node.InputPorts[ lastId ].PortId, false, true );

					SetSaveIsDirty();
					ForceRepaint();
				}
			}

			if( !m_wireReferenceUtils.ValidReferences() && !m_altBoxSelection)
			{
				if( m_mouseDownOnValidArea && m_insideEditorWindow )
				{
					if( m_currentEvent.control )
					{
						m_mainGraphInstance.MoveSelectedNodes( m_cameraZoom * m_currentEvent.delta, true );
					}
					else
					{
						m_mainGraphInstance.MoveSelectedNodes( m_cameraZoom * m_currentEvent.delta );
					}
					//m_mainGraphInstance.MoveSelectedNodes( m_cameraZoom * m_currentEvent.delta );
					m_autoPanDirActive = true;
				}
			}
			else
			{
				List<ParentNode> nodes = m_mainGraphInstance.GetNodesInGrid( m_drawInfo.TransformedMousePos );
				if( nodes != null && nodes.Count > 0 )
				{
					Vector2 currentPortPos = new Vector2();
					Vector2 mousePos = TranformedMousePos;

					if( m_wireReferenceUtils.InputPortReference.IsValid )
					{
						OutputPort currentPort = null;
						float smallestDistance = float.MaxValue;
						Vector2 smallestPosition = Vector2.zero;
						for( int nodeIdx = 0; nodeIdx < nodes.Count; nodeIdx++ )
						{
							List<OutputPort> outputPorts = nodes[ nodeIdx ].OutputPorts;
							if( outputPorts != null )
							{
								for( int o = 0; o < outputPorts.Count; o++ )
								{
									if( outputPorts[ o ].Available )
									{
										currentPortPos.x = outputPorts[ o ].Position.x;
										currentPortPos.y = outputPorts[ o ].Position.y;

										currentPortPos = currentPortPos * m_cameraZoom - m_cameraOffset;
										float dist = ( mousePos - currentPortPos ).sqrMagnitude;
										if( dist < smallestDistance )
										{
											smallestDistance = dist;
											smallestPosition = currentPortPos;
											currentPort = outputPorts[ o ];
										}
									}
								}
							}
						}

						if( currentPort != null && currentPort.Available && ( smallestDistance < Constants.SNAP_SQR_DIST || currentPort.InsideActiveArea( ( mousePos + m_cameraOffset ) / m_cameraZoom ) ) )
						{
							m_wireReferenceUtils.ActivateSnap( smallestPosition, currentPort );
						}
						else
						{
							m_wireReferenceUtils.DeactivateSnap();
						}
					}

					if( m_wireReferenceUtils.OutputPortReference.IsValid )
					{
						InputPort currentPort = null;
						float smallestDistance = float.MaxValue;
						Vector2 smallestPosition = Vector2.zero;
						for( int nodeIdx = 0; nodeIdx < nodes.Count; nodeIdx++ )
						{
							List<InputPort> inputPorts = nodes[ nodeIdx ].InputPorts;
							if( inputPorts != null )
							{
								for( int i = 0; i < inputPorts.Count; i++ )
								{
									if( inputPorts[ i ].Available )
									{
										currentPortPos.x = inputPorts[ i ].Position.x;
										currentPortPos.y = inputPorts[ i ].Position.y;

										currentPortPos = currentPortPos * m_cameraZoom - m_cameraOffset;
										float dist = ( mousePos - currentPortPos ).sqrMagnitude;
										if( dist < smallestDistance )
										{
											smallestDistance = dist;
											smallestPosition = currentPortPos;
											currentPort = inputPorts[ i ];
										}
									}
								}
							}
						}
						if( currentPort != null && currentPort.Available && ( smallestDistance < Constants.SNAP_SQR_DIST || currentPort.InsideActiveArea( ( mousePos + m_cameraOffset ) / m_cameraZoom ) ) )
						{
							m_wireReferenceUtils.ActivateSnap( smallestPosition, currentPort );
						}
						else
						{
							m_wireReferenceUtils.DeactivateSnap();
						}
					}
				}
				else if( m_wireReferenceUtils.SnapEnabled )
				{
					m_wireReferenceUtils.DeactivateSnap();
				}
			}
			UseCurrentEvent();
		}

		public void OnLeftMouseUp()
		{
			m_lmbPressed = false;
			if( m_multipleSelectionActive )
			{
				//m_multipleSelectionActive = false;
				UpdateSelectionArea();
				//m_mainGraphInstance.MultipleSelection( m_multipleSelectionArea, ( m_currentEvent.modifiers == EventModifiers.Shift || m_currentEvent.modifiers == EventModifiers.Control ), true );
				if( m_currentEvent.alt && m_altBoxSelection )
				{
					m_mainGraphInstance.MultipleSelection( m_multipleSelectionArea, !m_currentEvent.shift );
				}
				else
				{
					m_mainGraphInstance.DeSelectAll();
					m_mainGraphInstance.MultipleSelection( m_multipleSelectionArea );
				}
			}

			if( m_wireReferenceUtils.ValidReferences() )
			{
				//Check if there is some kind of port beneath the mouse ... if so connect to it
				ParentNode targetNode = m_wireReferenceUtils.SnapEnabled ? m_mainGraphInstance.GetNode( m_wireReferenceUtils.SnapPort.NodeId ) : m_mainGraphInstance.CheckNodeAt( m_currentMousePos );
				if( targetNode != null && targetNode.ConnStatus != NodeConnectionStatus.Island )
				{
					if( m_wireReferenceUtils.InputPortReference.IsValid && m_wireReferenceUtils.InputPortReference.NodeId != targetNode.UniqueId )
					{
						OutputPort outputPort = m_wireReferenceUtils.SnapEnabled ? targetNode.GetOutputPortByUniqueId( m_wireReferenceUtils.SnapPort.PortId ) : targetNode.CheckOutputPortAt( m_currentMousePos );
						if( outputPort != null && !outputPort.Locked && ( !m_wireReferenceUtils.InputPortReference.TypeLocked ||
													m_wireReferenceUtils.InputPortReference.DataType == WirePortDataType.OBJECT ||
													( m_wireReferenceUtils.InputPortReference.TypeLocked && outputPort.DataType == m_wireReferenceUtils.InputPortReference.DataType ) ) )
						{

							ParentNode originNode = m_mainGraphInstance.GetNode( m_wireReferenceUtils.InputPortReference.NodeId );
							InputPort inputPort = originNode.GetInputPortByUniqueId( m_wireReferenceUtils.InputPortReference.PortId );
							UIUtils.MarkUndoAction();
							Undo.RegisterCompleteObjectUndo( this, Constants.UndoCreateConnectionId );
							originNode.RecordObject( Constants.UndoCreateConnectionId );
							targetNode.RecordObject( Constants.UndoCreateConnectionId );

							if( !inputPort.CheckValidType( outputPort.DataType ) )
							{
								UIUtils.ShowIncompatiblePortMessage( true, originNode, inputPort, targetNode, outputPort );
								m_wireReferenceUtils.InvalidateReferences();
								UseCurrentEvent();
								return;
							}

							if( !outputPort.CheckValidType( inputPort.DataType ) )
							{
								UIUtils.ShowIncompatiblePortMessage( false, targetNode, outputPort, originNode, inputPort );
								m_wireReferenceUtils.InvalidateReferences();
								UseCurrentEvent();
								return;
							}

							inputPort.DummyAdd( outputPort.NodeId, outputPort.PortId );
							outputPort.DummyAdd( m_wireReferenceUtils.InputPortReference.NodeId, m_wireReferenceUtils.InputPortReference.PortId );

							if( UIUtils.DetectNodeLoopsFrom( originNode, new Dictionary<int, int>() ) )
							{
								inputPort.DummyRemove();
								outputPort.DummyRemove();
								m_wireReferenceUtils.InvalidateReferences();
								ShowMessage( "Infinite Loop detected" );
								UseCurrentEvent();
								return;
							}

							inputPort.DummyRemove();
							outputPort.DummyRemove();

							if( inputPort.IsConnected )
							{
								DeleteConnection( true, m_wireReferenceUtils.InputPortReference.NodeId, m_wireReferenceUtils.InputPortReference.PortId, true, false );
							}

							//link output to input
							if( outputPort.ConnectTo( m_wireReferenceUtils.InputPortReference.NodeId, m_wireReferenceUtils.InputPortReference.PortId, m_wireReferenceUtils.InputPortReference.DataType, m_wireReferenceUtils.InputPortReference.TypeLocked ) )
								targetNode.OnOutputPortConnected( outputPort.PortId, m_wireReferenceUtils.InputPortReference.NodeId, m_wireReferenceUtils.InputPortReference.PortId );

							//link input to output
							if( inputPort.ConnectTo( outputPort.NodeId, outputPort.PortId, outputPort.DataType, m_wireReferenceUtils.InputPortReference.TypeLocked ) )
								originNode.OnInputPortConnected( m_wireReferenceUtils.InputPortReference.PortId, targetNode.UniqueId, outputPort.PortId );
							m_mainGraphInstance.MarkWireHighlights();
						}
						else if( outputPort != null && m_wireReferenceUtils.InputPortReference.TypeLocked && m_wireReferenceUtils.InputPortReference.DataType != outputPort.DataType )
						{
							ShowMessage( "Attempting to connect a port locked to type " + m_wireReferenceUtils.InputPortReference.DataType + " into a port of type " + outputPort.DataType );
						}
						ShaderIsModified = true;
						SetSaveIsDirty();
					}

					if( m_wireReferenceUtils.OutputPortReference.IsValid && m_wireReferenceUtils.OutputPortReference.NodeId != targetNode.UniqueId )
					{
						InputPort inputPort = m_wireReferenceUtils.SnapEnabled ? targetNode.GetInputPortByUniqueId( m_wireReferenceUtils.SnapPort.PortId ) : targetNode.CheckInputPortAt( m_currentMousePos );
						if( inputPort != null && !inputPort.Locked && ( !inputPort.TypeLocked ||
													 inputPort.DataType == WirePortDataType.OBJECT ||
													 ( inputPort.TypeLocked && inputPort.DataType == m_wireReferenceUtils.OutputPortReference.DataType ) ) )
						{
							ParentNode originNode = m_mainGraphInstance.GetNode( m_wireReferenceUtils.OutputPortReference.NodeId );
							OutputPort outputPort = originNode.GetOutputPortByUniqueId( m_wireReferenceUtils.OutputPortReference.PortId );

							UIUtils.MarkUndoAction();
							Undo.RegisterCompleteObjectUndo( this, Constants.UndoCreateConnectionId );
							originNode.RecordObject( Constants.UndoCreateConnectionId );
							targetNode.RecordObject( Constants.UndoCreateConnectionId );

							if( !inputPort.CheckValidType( outputPort.DataType ) )
							{
								UIUtils.ShowIncompatiblePortMessage( true, targetNode, inputPort, originNode, outputPort );
								m_wireReferenceUtils.InvalidateReferences();
								UseCurrentEvent();
								return;
							}

							if( !outputPort.CheckValidType( inputPort.DataType ) )
							{
								UIUtils.ShowIncompatiblePortMessage( false, originNode, outputPort, targetNode, inputPort );
								m_wireReferenceUtils.InvalidateReferences();
								UseCurrentEvent();
								return;
							}

							inputPort.DummyAdd( m_wireReferenceUtils.OutputPortReference.NodeId, m_wireReferenceUtils.OutputPortReference.PortId );
							outputPort.DummyAdd( inputPort.NodeId, inputPort.PortId );
							if( UIUtils.DetectNodeLoopsFrom( targetNode, new Dictionary<int, int>() ) )
							{
								inputPort.DummyRemove();
								outputPort.DummyRemove();
								m_wireReferenceUtils.InvalidateReferences();
								ShowMessage( "Infinite Loop detected" );
								UseCurrentEvent();
								return;
							}

							inputPort.DummyRemove();
							outputPort.DummyRemove();

							if( inputPort.IsConnected )
							{
								if( m_currentEvent.control && m_wireReferenceUtils.SwitchPortReference.IsValid )
								{
									ParentNode oldOutputNode = UIUtils.GetNode( inputPort.GetConnection( 0 ).NodeId );
									OutputPort oldOutputPort = oldOutputNode.GetOutputPortByUniqueId( inputPort.GetConnection( 0 ).PortId );

									ParentNode switchNode = UIUtils.GetNode( m_wireReferenceUtils.SwitchPortReference.NodeId );
									InputPort switchPort = switchNode.GetInputPortByUniqueId( m_wireReferenceUtils.SwitchPortReference.PortId );

									switchPort.DummyAdd( oldOutputPort.NodeId, oldOutputPort.PortId );
									oldOutputPort.DummyAdd( switchPort.NodeId, switchPort.PortId );
									if( UIUtils.DetectNodeLoopsFrom( switchNode, new Dictionary<int, int>() ) )
									{
										switchPort.DummyRemove();
										oldOutputPort.DummyRemove();
										m_wireReferenceUtils.InvalidateReferences();
										ShowMessage( "Infinite Loop detected" );
										UseCurrentEvent();
										return;
									}

									switchPort.DummyRemove();
									oldOutputPort.DummyRemove();

									DeleteConnection( true, inputPort.NodeId, inputPort.PortId, true, false );
									ConnectInputToOutput( switchPort.NodeId, switchPort.PortId, oldOutputPort.NodeId, oldOutputPort.PortId );
								}
								else
								{
									DeleteConnection( true, inputPort.NodeId, inputPort.PortId, true, false );
								}
							}
							inputPort.InvalidateAllConnections();


							//link input to output
							if( inputPort.ConnectTo( m_wireReferenceUtils.OutputPortReference.NodeId, m_wireReferenceUtils.OutputPortReference.PortId, m_wireReferenceUtils.OutputPortReference.DataType, inputPort.TypeLocked ) )
								targetNode.OnInputPortConnected( inputPort.PortId, m_wireReferenceUtils.OutputPortReference.NodeId, m_wireReferenceUtils.OutputPortReference.PortId );
							//link output to input

							if( outputPort.ConnectTo( inputPort.NodeId, inputPort.PortId, inputPort.DataType, inputPort.TypeLocked ) )
								originNode.OnOutputPortConnected( m_wireReferenceUtils.OutputPortReference.PortId, targetNode.UniqueId, inputPort.PortId );
							m_mainGraphInstance.MarkWireHighlights();
						}
						else if( inputPort != null && inputPort.TypeLocked && inputPort.DataType != m_wireReferenceUtils.OutputPortReference.DataType )
						{
							ShowMessage( "Attempting to connect a " + m_wireReferenceUtils.OutputPortReference.DataType + " to a port locked to type " + inputPort.DataType );
						}
						ShaderIsModified = true;
						SetSaveIsDirty();
					}
					m_wireReferenceUtils.InvalidateReferences();
				}
				else
				{
					if( UIUtils.ShowContextOnPick )
						m_contextPalette.Show( m_currentMousePos2D, m_cameraInfo );
					else
						m_wireReferenceUtils.InvalidateReferences();
				}
			}
			else if( m_currentEvent.modifiers == EventModifiers.Alt && m_altAvailable && CurrentGraph.SelectedNodes.Count == 1 && !m_altBoxSelection && !m_multipleSelectionActive )
			{
				List<WireBezierReference> wireRefs = m_mainGraphInstance.GetWireBezierListInPos( m_currentMousePos2D );
				if( wireRefs != null && wireRefs.Count > 0 )
				{
					float closestDist = 50;
					int closestId = 0;

					for( int i = 0; i < wireRefs.Count; i++ )
					{
						ParentNode outNode = m_mainGraphInstance.GetNode( wireRefs[ i ].OutNodeId );
						ParentNode inNode = m_mainGraphInstance.GetNode( wireRefs[ i ].InNodeId );

						if( outNode == CurrentGraph.SelectedNodes[ 0 ] || inNode == CurrentGraph.SelectedNodes[ 0 ] )
							continue;

						OutputPort outputPort = outNode.GetOutputPortByUniqueId( wireRefs[ i ].OutPortId );
						InputPort inputPort = inNode.GetInputPortByUniqueId( wireRefs[ i ].InPortId );

						// Calculate the 4 points for bezier taking into account wire nodes and their automatic tangents
						Vector3 endPos = new Vector3( inputPort.Position.x, inputPort.Position.y );
						Vector3 startPos = new Vector3( outputPort.Position.x, outputPort.Position.y );

						float mag = ( endPos - startPos ).magnitude;
						float resizedMag = Mathf.Min( mag, Constants.HORIZONTAL_TANGENT_SIZE * m_drawInfo.InvertedZoom );

						Vector3 startTangent = new Vector3( startPos.x + resizedMag, startPos.y );
						Vector3 endTangent = new Vector3( endPos.x - resizedMag, endPos.y );

						if( inNode != null && inNode.GetType() == typeof( WireNode ) )
							endTangent = endPos + ( ( inNode as WireNode ).TangentDirection ) * mag * 0.33f;

						if( outNode != null && outNode.GetType() == typeof( WireNode ) )
							startTangent = startPos - ( ( outNode as WireNode ).TangentDirection ) * mag * 0.33f;

						//Vector2 pos = ( CurrentGraph.SelectedNodes[0].CenterPosition + m_cameraOffset ) / m_cameraZoom;

						float dist = HandleUtility.DistancePointBezier( /*pos*/ m_currentMousePos, startPos, endPos, startTangent, endTangent );
						if( dist < 40 )
						{
							if( dist < closestDist )
							{
								closestDist = dist;
								closestId = i;
							}
						}
					}

					if( closestDist < 40 )
					{
						ParentNode outNode = m_mainGraphInstance.GetNode( wireRefs[ closestId ].OutNodeId );
						ParentNode inNode = m_mainGraphInstance.GetNode( wireRefs[ closestId ].InNodeId );

						OutputPort outputPort = outNode.GetOutputPortByUniqueId( wireRefs[ closestId ].OutPortId );
						InputPort inputPort = inNode.GetInputPortByUniqueId( wireRefs[ closestId ].InPortId );

						ParentNode selectedNode = CurrentGraph.SelectedNodes[ 0 ];
						if( selectedNode.InputPorts.Count > 0 && selectedNode.OutputPorts.Count > 0 )
						{
							Undo.RegisterCompleteObjectUndo( this, Constants.UndoCreateConnectionId );
							selectedNode.RecordObject( Constants.UndoCreateConnectionId );
							inNode.RecordObject( Constants.UndoCreateConnectionId );
							outNode.RecordObject( Constants.UndoCreateConnectionId );

							m_mainGraphInstance.CreateConnection( selectedNode.UniqueId, selectedNode.InputPorts[ 0 ].PortId, outputPort.NodeId, outputPort.PortId );
							m_mainGraphInstance.CreateConnection( inputPort.NodeId, inputPort.PortId, selectedNode.UniqueId, selectedNode.OutputPorts[ 0 ].PortId );
						}

						SetSaveIsDirty();
						ForceRepaint();
					}
				}
			}
			UIUtils.ShowContextOnPick = true;
			m_altBoxSelection = false;
			m_multipleSelectionActive = false;
			UseCurrentEvent();
		}

		public void ConnectInputToOutput( int inNodeId, int inPortId, int outNodeId, int outPortId, bool registerUndo = true )
		{
			ParentNode inNode = m_mainGraphInstance.GetNode( inNodeId );
			ParentNode outNode = m_mainGraphInstance.GetNode( outNodeId );
			if( inNode != null && outNode != null )
			{
				InputPort inPort = inNode.GetInputPortByUniqueId( inPortId );
				OutputPort outPort = outNode.GetOutputPortByUniqueId( outPortId );
				if( inPort != null && outPort != null )
				{
					if( registerUndo )
					{
						Undo.RegisterCompleteObjectUndo( this, Constants.UndoCreateConnectionId );
						inNode.RecordObject( Constants.UndoCreateConnectionId );
						outNode.RecordObject( Constants.UndoCreateConnectionId );
					}

					if( inPort.ConnectTo( outNodeId, outPortId, outPort.DataType, inPort.TypeLocked ) )
					{
						inNode.OnInputPortConnected( inPortId, outNodeId, outPortId );
					}

					if( outPort.ConnectTo( inNodeId, inPortId, inPort.DataType, inPort.TypeLocked ) )
					{
						outNode.OnOutputPortConnected( outPortId, inNodeId, inPortId );
					}
				}
				m_mainGraphInstance.MarkWireHighlights();
				ShaderIsModified = true;
			}
		}

		void OnRightMouseDown()
		{
			Focus();
			m_rmbStartPos = m_currentMousePos2D;
			UseCurrentEvent();
		}

		void OnRightMouseDrag()
		{
			// We look at the control to detect when user hits a tooltip ( which has a hot control of 0 )
			// This needs to be checked because on this first "frame" of hitting a tooltip because it generates incorrect mouse delta values 
			if( GUIUtility.hotControl == 0 && m_lastHotControl != 0 )
			{
				m_lastHotControl = GUIUtility.hotControl;
				return;
			}

			m_lastHotControl = GUIUtility.hotControl;
			if( m_currentEvent.alt )
			{
				ModifyZoom( Constants.ALT_CAMERA_ZOOM_SPEED * ( m_currentEvent.delta.x + m_currentEvent.delta.y ), m_altKeyStartPos );
			}
			else
			{
				m_cameraOffset += m_cameraZoom * m_currentEvent.delta;
			}
			UseCurrentEvent();
		}

		void OnRightMouseUp()
		{
			//Resetting the hot control test variable so it can be used again on right mouse drag detection ( if we did not do this then m_lastHotControl could be left with a a value of 0 and wouldn't be able to be correctly used on rthe drag ) 
			m_lastHotControl = -1;

			if( ( m_rmbStartPos - m_currentMousePos2D ).sqrMagnitude < Constants.RMB_SCREEN_DIST )
			{
				ParentNode node = m_mainGraphInstance.CheckNodeAt( m_currentMousePos, true );
				if( node == null )
				{
					m_contextPalette.Show( m_currentMousePos2D, m_cameraInfo );
				}
			}
			UseCurrentEvent();
		}

		void UpdateSelectionArea()
		{
			m_multipleSelectionArea.size = TranformedMousePos - m_multipleSelectionStart;
		}

		public void OnValidObjectsDropped( UnityEngine.Object[] droppedObjs )
		{
			bool propagateDraggedObjsToNode = true;
			// Only supporting single drag&drop object selection
			if( droppedObjs.Length == 1 )
			{
				ShaderIsModified = true;
				SetSaveIsDirty();
				// Check if its a shader, material or game object  and if so load the shader graph code from it
				Shader newShader = droppedObjs[ 0 ] as Shader;
				Material newMaterial = null;
				if( newShader == null )
				{
					newMaterial = droppedObjs[ 0 ] as Material;
#if UNITY_2018_1_OR_NEWER
					bool isProcedural = ( newMaterial != null );
#else
// Disabling Substance Deprecated warning
#pragma warning disable 0618
					bool isProcedural = ( newMaterial != null && newMaterial is ProceduralMaterial );
#pragma warning restore 0618
#endif
					if( newMaterial != null && !isProcedural )
					{
						if( UIUtils.IsUnityNativeShader( AssetDatabase.GetAssetPath( newMaterial.shader ) ) )
						{
							return;
						}
						//newShader = newMaterial.shader;
						LoadMaterialToASE( newMaterial );
						//m_mainGraphInstance.UpdateMaterialOnMasterNode( newMaterial );
					}
					else
					{
						GameObject go = droppedObjs[ 0 ] as GameObject;
						if( go != null )
						{
							Renderer renderer = go.GetComponent<Renderer>();
							if( renderer )
							{
								newMaterial = renderer.sharedMaterial;
								newShader = newMaterial.shader;
							}
						}
					}
				}

				if( newShader != null )
				{
					ConvertShaderToASE( newShader );

					propagateDraggedObjsToNode = false;
				}

				// if not shader loading then propagate the seletion to whats below the mouse
				if( propagateDraggedObjsToNode )
				{
					ParentNode node = m_mainGraphInstance.CheckNodeAt( m_currentMousePos );
					if( node != null )
					{
						// if there's a node then pass the object into it to see if there's a setup with it
						node.OnObjectDropped( droppedObjs[ 0 ] );
					}
					else
					{
						// If not then check if there's a node that can be created through the dropped object
						ParentNode newNode = m_contextMenu.CreateNodeFromCastType( droppedObjs[ 0 ].GetType() );
						if( newNode )
						{
							newNode.ContainerGraph = m_mainGraphInstance;
							newNode.Vec2Position = TranformedMousePos;
							m_mainGraphInstance.AddNode( newNode, true );
							newNode.SetupFromCastObject( droppedObjs[ 0 ] );
							m_mainGraphInstance.SelectNode( newNode, false, false );
							ForceRepaint();
							bool find = false;
							if( newNode is FunctionNode && CurrentGraph.CurrentShaderFunction != null )
								find = SearchFunctionNodeRecursively( CurrentGraph.CurrentShaderFunction );

							if( find )
							{
								DestroyNode( newNode, false );
								ShowMessage( "Shader Function loop detected, new node was removed to prevent errors." );
							}
						}
					}
				}
			}
		}

		public bool SearchFunctionNodeRecursively( AmplifyShaderFunction function )
		{
			List<FunctionNode> graphList = UIUtils.FunctionList();

			bool nodeFind = false;

			for( int i = 0; i < graphList.Count; i++ )
			{
				ParentGraph temp = CustomGraph;
				CustomGraph = graphList[ i ].FunctionGraph;
				nodeFind = SearchFunctionNodeRecursively( function );
				CustomGraph = temp;

				//Debug.Log( "tested = " + node.Function.FunctionName + " : " + function.FunctionName );

				if( graphList[ i ].Function == function )
					return true;
			}

			return nodeFind;
		}

		public void SetDelayedMaterialMode( Material material )
		{
			if( material == null )
				return;
			m_delayedMaterialSet = material;
		}

		public ShaderLoadResult LoadDroppedObject( bool value, Shader shader, Material material, AmplifyShaderFunction shaderFunction = null )
		{
			UIUtils.CurrentWindow = this;
			ShaderLoadResult result;
			if( shaderFunction != null )
			{
				string assetDatapath = AssetDatabase.GetAssetPath( shaderFunction );
				string latestOpenedFolder = Application.dataPath + assetDatapath.Substring( 6 );
				UIUtils.LatestOpenedFolder = latestOpenedFolder.Substring( 0, latestOpenedFolder.LastIndexOf( '/' ) + 1 );
				result = LoadFromDisk( assetDatapath, shaderFunction );
				CurrentSelection = ASESelectionMode.ShaderFunction;
				IsShaderFunctionWindow = true;
				titleContent.text = GenerateTabTitle( shaderFunction.FunctionName );
				titleContent.image = UIUtils.ShaderFunctionIcon;
				m_lastpath = assetDatapath;
				m_nodeParametersWindow.OnShaderFunctionLoad();
				//EditorPrefs.SetString( IOUtils.LAST_OPENED_OBJ_ID, assetDatapath );
			}
			else if( value && shader != null )
			{
				string assetDatapath = AssetDatabase.GetAssetPath( shader );
				string latestOpenedFolder = Application.dataPath + assetDatapath.Substring( 6 );
				UIUtils.LatestOpenedFolder = latestOpenedFolder.Substring( 0, latestOpenedFolder.LastIndexOf( '/' ) + 1 );
				result = LoadFromDisk( assetDatapath );
				switch( result )
				{
					case ShaderLoadResult.LOADED:
					{
						m_mainGraphInstance.UpdateShaderOnMasterNode( shader );
					}
					break;
					case ShaderLoadResult.ASE_INFO_NOT_FOUND:
					{
						ShowMessage( "Loaded shader wasn't created with ASE. Saving it will remove previous data." );
						UIUtils.CreateEmptyFromInvalid( shader );
					}
					break;
					case ShaderLoadResult.FILE_NOT_FOUND:
					case ShaderLoadResult.UNITY_NATIVE_PATHS:
					{
						UIUtils.CreateEmptyFromInvalid( shader );
					}
					break;
				}

				m_mainGraphInstance.UpdateMaterialOnMasterNode( material );
				m_mainGraphInstance.SetMaterialModeOnGraph( material );

				if( material != null )
				{
					CurrentSelection = ASESelectionMode.Material;
					IsShaderFunctionWindow = false;
					titleContent.text = GenerateTabTitle( material.name );
					titleContent.image = UIUtils.MaterialIcon;
					if( material.HasProperty( IOUtils.DefaultASEDirtyCheckId ) )
					{
						material.SetInt( IOUtils.DefaultASEDirtyCheckId, 1 );
					}
					m_lastpath = AssetDatabase.GetAssetPath( material );
					EditorPrefs.SetString( IOUtils.LAST_OPENED_OBJ_ID, m_lastpath );
				}
				else
				{
					CurrentSelection = ASESelectionMode.Shader;
					IsShaderFunctionWindow = false;
					titleContent.text = GenerateTabTitle( shader.name );
					titleContent.image = UIUtils.ShaderIcon;
					m_lastpath = AssetDatabase.GetAssetPath( shader );
					EditorPrefs.SetString( IOUtils.LAST_OPENED_OBJ_ID, m_lastpath );
				}
			}
			else
			{
				result = ShaderLoadResult.FILE_NOT_FOUND;
			}
			return result;
		}

		bool InsideMenus( Vector2 position )
		{
			for( int i = 0; i < m_registeredMenus.Count; i++ )
			{
				if( m_registeredMenus[ i ].IsInside( position ) )
				{
					return true;
				}
			}
			return false;
		}

		void HandleGUIEvents()
		{
			if( m_currentEvent.type == EventType.KeyDown )
			{
				m_contextMenu.UpdateKeyPress( m_currentEvent.keyCode );
			}
			else if( m_currentEvent.type == EventType.KeyUp )
			{
				m_contextMenu.UpdateKeyReleased( m_currentEvent.keyCode );
			}

			if( InsideMenus( m_currentMousePos2D ) )
			{
				if( m_currentEvent.type == EventType.MouseDown )
				{
					m_mouseDownOnValidArea = false;
					UseCurrentEvent();
				}
				return;
			}
			else if( m_nodeParametersWindow.IsResizing || m_paletteWindow.IsResizing )
			{
				m_mouseDownOnValidArea = false;
			}

			int controlID = GUIUtility.GetControlID( FocusType.Passive );
			switch( m_currentEvent.GetTypeForControl( controlID ) )
			{
				case EventType.MouseDown:
				{
					GUIUtility.hotControl = controlID;
					switch( m_currentEvent.button )
					{
						case ButtonClickId.LeftMouseButton:
						{
							OnLeftMouseDown();
						}
						break;
						case ButtonClickId.RightMouseButton:
						case ButtonClickId.MiddleMouseButton:
						{
							OnRightMouseDown();
						}
						break;
					}
				}
				break;

				case EventType.MouseUp:
				{
					GUIUtility.hotControl = 0;
					switch( m_currentEvent.button )
					{
						case ButtonClickId.LeftMouseButton:
						{
							OnLeftMouseUp();
						}
						break;
						case ButtonClickId.MiddleMouseButton: break;
						case ButtonClickId.RightMouseButton:
						{
							OnRightMouseUp();
						}
						break;
					}
				}
				break;
				case EventType.MouseDrag:
				{
					switch( m_currentEvent.button )
					{
						case ButtonClickId.LeftMouseButton:
						{
							OnLeftMouseDrag();
						}
						break;
						case ButtonClickId.MiddleMouseButton:
						case ButtonClickId.RightMouseButton:
						{
							OnRightMouseDrag();
						}
						break;
					}
				}
				break;
				case EventType.ScrollWheel:
				{
					OnScrollWheel();
				}
				break;
				case EventType.KeyDown:
				{
					OnKeyboardDown();
				}
				break;
				case EventType.KeyUp:
				{
					OnKeyboardUp();
				}
				break;
				case EventType.ValidateCommand:
				{
					switch( m_currentEvent.commandName )
					{
						case CopyCommand:
						case PasteCommand:
						case SelectAll:
						case Duplicate:
						{
							m_currentEvent.Use();
						}
						break;
						case ObjectSelectorClosed:
						{
							m_mouseDownOnValidArea = false;
						}
						break;
					}
				}
				break;
				case EventType.ExecuteCommand:
				{
					m_currentEvent.Use();
					switch( m_currentEvent.commandName )
					{
						case CopyCommand:
						{
							CopyToClipboard();
						}
						break;
						case PasteCommand:
						{
							PasteFromClipboard( true );
						}
						break;
						case SelectAll:
						{
							m_mainGraphInstance.SelectAll();
							ForceRepaint();
						}
						break;
						case Duplicate:
						{
							CopyToClipboard();
							PasteFromClipboard( true );
						}
						break;
						case ObjectSelectorClosed:
						{
							m_mouseDownOnValidArea = false;
						}break;
					}
				}
				break;
				case EventType.Repaint:
				{
				}
				break;
			}

			m_dragAndDropTool.TestDragAndDrop( m_graphArea );

		}

		public void DeleteConnection( bool isInput, int nodeId, int portId, bool registerOnLog, bool propagateCallback )
		{
			m_mainGraphInstance.DeleteConnection( isInput, nodeId, portId, registerOnLog, propagateCallback );
		}

		void DeleteSelectedNodes()
		{
			if( m_mainGraphInstance.SelectedNodes.Count == 0 )
				return;

			UIUtils.ClearUndoHelper();
			ParentNode[] selectedNodes = new ParentNode[ m_mainGraphInstance.SelectedNodes.Count ];
			for( int i = 0; i < selectedNodes.Length; i++ )
			{
				selectedNodes[ i ] = m_mainGraphInstance.SelectedNodes[ i ];
				selectedNodes[ i ].Rewire();
				UIUtils.CheckUndoNode( selectedNodes[ i ] );
			}

			//Check nodes connected to deleted nodes to preserve connections on undo
			List<ParentNode> extraNodes = new List<ParentNode>();
			for( int selectedNodeIdx = 0; selectedNodeIdx < selectedNodes.Length; selectedNodeIdx++ )
			{
				// Check inputs
				{
					int inputIdxCount = selectedNodes[ selectedNodeIdx ].InputPorts.Count;
					if( inputIdxCount > 0 )
					{
						for( int inputIdx = 0; inputIdx < inputIdxCount; inputIdx++ )
						{
							if( selectedNodes[ selectedNodeIdx ].InputPorts[ inputIdx ].IsConnected )
							{
								int nodeIdx = selectedNodes[ selectedNodeIdx ].InputPorts[ inputIdx ].ExternalReferences[ 0 ].NodeId;
								if( nodeIdx > -1 )
								{
									ParentNode node = m_mainGraphInstance.GetNode( nodeIdx );
									if( node != null && UIUtils.CheckUndoNode( node ) )
									{
										extraNodes.Add( node );
									}
								}
							}
						}
					}
				}

				// Check outputs
				int outputIdxCount = selectedNodes[ selectedNodeIdx ].OutputPorts.Count;
				if( outputIdxCount > 0 )
				{
					for( int outputIdx = 0; outputIdx < outputIdxCount; outputIdx++ )
					{
						int inputIdxCount = selectedNodes[ selectedNodeIdx ].OutputPorts[ outputIdx ].ExternalReferences.Count;
						if( inputIdxCount > 0 )
						{
							for( int inputIdx = 0; inputIdx < inputIdxCount; inputIdx++ )
							{
								int nodeIdx = selectedNodes[ selectedNodeIdx ].OutputPorts[ outputIdx ].ExternalReferences[ inputIdx ].NodeId;
								if( nodeIdx > -1 )
								{
									ParentNode node = m_mainGraphInstance.GetNode( nodeIdx );
									if( UIUtils.CheckUndoNode( node ) )
									{
										extraNodes.Add( node );
									}
								}
							}
						}
					}
				}
			}
			
			UIUtils.ClearUndoHelper();
			//Undo.IncrementCurrentGroup();
			//Record deleted nodes
			UIUtils.MarkUndoAction();
			Undo.RegisterCompleteObjectUndo( this, Constants.UndoDeleteNodeId );
			Undo.RegisterCompleteObjectUndo( m_mainGraphInstance, Constants.UndoDeleteNodeId );
			Undo.RecordObjects( selectedNodes, Constants.UndoDeleteNodeId );
			Undo.RecordObjects( extraNodes.ToArray(), Constants.UndoDeleteNodeId );

			//Record deleting connections
			for( int i = 0; i < selectedNodes.Length; i++ )
			{
				selectedNodes[ i ].Alive = false;
				m_mainGraphInstance.DeleteAllConnectionFromNode( selectedNodes[ i ], false, true , true );
			}
			//Delete
			m_mainGraphInstance.DeleteNodesOnArray( ref selectedNodes );

			
			//Undo.IncrementCurrentGroup();
			extraNodes.Clear();
			extraNodes = null;

			EditorUtility.SetDirty( this );

			ForceRepaint();
		}

		void OnKeyboardUp()
		{
			CheckKeyboardCameraUp();

			if( m_altPressDown )
			{
				m_altPressDown = false;
			}
			
			if( m_shortcutManager.ActivateShortcut( m_currentEvent.modifiers, m_lastKeyPressed, false ) )
			{
				ForceRepaint();
			}
			m_lastKeyPressed = KeyCode.None;
		}

		bool OnKeyboardPress( KeyCode code )
		{
			return ( m_currentEvent.keyCode == code && m_lastKeyPressed == KeyCode.None );
		}

		void CheckKeyboardCameraDown()
		{
			if( m_contextPalette.IsActive )
				return;
			if( m_currentEvent.alt )
			{
				bool foundKey = false;
				float dir = 0;
				switch( m_currentEvent.keyCode )
				{
					case KeyCode.UpArrow: foundKey = true; dir = 1; break;
					case KeyCode.DownArrow: foundKey = true; dir = -1; break;
					case KeyCode.LeftArrow: foundKey = true; dir = 1; break;
					case KeyCode.RightArrow: foundKey = true; dir = -1; break;
				}
				if( foundKey )
				{
					ModifyZoom( Constants.ALT_CAMERA_ZOOM_SPEED * dir * m_cameraSpeed, new Vector2( m_cameraInfo.width * 0.5f, m_cameraInfo.height * 0.5f ) );
					if( m_cameraSpeed < 15 )
						m_cameraSpeed += 0.2f;
					UseCurrentEvent();
				}

				
			}
			else
			{
				bool foundKey = false;
				Vector2 dir = Vector2.zero;
				switch( m_currentEvent.keyCode )
				{
					case KeyCode.UpArrow: foundKey = true; dir = Vector2.up; break;
					case KeyCode.DownArrow: foundKey = true; dir = Vector2.down; break;
					case KeyCode.LeftArrow: foundKey = true; dir = Vector2.right; break;
					case KeyCode.RightArrow: foundKey = true; dir = Vector2.left; break;
				}
				if( foundKey )
				{
					m_cameraOffset += m_cameraZoom * m_cameraSpeed * dir;
					if( m_cameraSpeed < 15 )
						m_cameraSpeed += 0.2f;

					UseCurrentEvent();
				}
			}
		}

		void CheckKeyboardCameraUp()
		{
			switch( m_currentEvent.keyCode )
			{
				case KeyCode.UpArrow:
				case KeyCode.DownArrow:
				case KeyCode.LeftArrow:
				case KeyCode.RightArrow: m_cameraSpeed = 1;break;
			}
		}

		void OnKeyboardDown()
		{
			//if( DebugConsoleWindow.DeveloperMode )
			//{
			//	if( OnKeyboardPress( KeyCode.F8 ) )
			//	{
			//		Shader currShader = CurrentGraph.CurrentShader;
			//		ShaderUtilEx.OpenCompiledShader( currShader, ShaderInspectorPlatformsPopupEx.GetCurrentMode(), ShaderInspectorPlatformsPopupEx.GetCurrentPlatformMask(), ShaderInspectorPlatformsPopupEx.GetCurrentVariantStripping() == 0 );

			//		string filename = Application.dataPath;
			//		filename = filename.Replace( "Assets", "Temp/Compiled-" );
			//		string shaderFilename = AssetDatabase.GetAssetPath( currShader );
			//		int lastIndex = shaderFilename.LastIndexOf( '/' ) + 1;
			//		filename = filename + shaderFilename.Substring( lastIndex );

			//		string compiledContents = IOUtils.LoadTextFileFromDisk( filename );
			//		Debug.Log( compiledContents );
			//	}

			//	if( OnKeyboardPress( KeyCode.F9 ) )
			//	{
			//		m_nodeExporterUtils.CalculateShaderInstructions( CurrentGraph.CurrentShader );
			//	}
			//}

			CheckKeyboardCameraDown();

			if( m_lastKeyPressed == KeyCode.None )
			{
				m_shortcutManager.ActivateShortcut( m_currentEvent.modifiers, m_currentEvent.keyCode, true );
			}

			if( m_currentEvent.control && m_currentEvent.shift && m_currentEvent.keyCode == KeyCode.V )
			{
				PasteFromClipboard( false );
			}

			if( !m_altPressDown && ( OnKeyboardPress( KeyCode.LeftAlt ) || OnKeyboardPress( KeyCode.RightAlt ) || OnKeyboardPress( KeyCode.AltGr ) ) )
			{
				m_altPressDown = true;
				m_altAvailable = true;
				m_altKeyStartPos = m_currentMousePos2D;
			}

			if( m_currentEvent.keyCode != KeyCode.None && m_currentEvent.modifiers == EventModifiers.None )
			{
				m_lastKeyPressed = m_currentEvent.keyCode;
			}
		}

		void OnScrollWheel()
		{
			ModifyZoomSmooth( m_currentEvent.delta.y, m_currentMousePos2D );
			UseCurrentEvent();
		}

		void ModifyZoom( float zoomIncrement, Vector2 pivot )
		{
			float minCam = Mathf.Min( ( m_cameraInfo.width - ( m_nodeParametersWindow.RealWidth + m_paletteWindow.RealWidth ) ), ( m_cameraInfo.height - ( m_toolsWindow.Height ) ) );
			if( minCam < 1 )
				minCam = 1;

			float dynamicMaxZoom = m_mainGraphInstance.MaxNodeDist / minCam;

			Vector2 canvasPos = TranformPosition( pivot );
			if( zoomIncrement < 0 )
				CameraZoom = Mathf.Max( m_cameraZoom + zoomIncrement * Constants.CAMERA_ZOOM_SPEED, Constants.CAMERA_MIN_ZOOM );
			else if( CameraZoom < Mathf.Max( Constants.CAMERA_MAX_ZOOM, dynamicMaxZoom ) )
				CameraZoom = m_cameraZoom + zoomIncrement * Constants.CAMERA_ZOOM_SPEED;// Mathf.Min( m_cameraZoom + zoomIncrement * Constants.CAMERA_ZOOM_SPEED, Mathf.Max( Constants.CAMERA_MAX_ZOOM, dynamicMaxZoom ) );
			m_cameraOffset.x = pivot.x * m_cameraZoom - canvasPos.x;
			m_cameraOffset.y = pivot.y * m_cameraZoom - canvasPos.y;
		}

		void ModifyZoomSmooth( float zoomIncrement, Vector2 pivot )
		{
			if( m_smoothZoom && Mathf.Sign( m_targetZoomIncrement * zoomIncrement ) >= 0 )
				m_targetZoomIncrement += zoomIncrement;
			else
				m_targetZoomIncrement = zoomIncrement;

			m_smoothZoom = true;
			m_zoomTime = 0;

			float minCam = Mathf.Min( ( m_cameraInfo.width - ( m_nodeParametersWindow.RealWidth + m_paletteWindow.RealWidth ) ), ( m_cameraInfo.height - ( m_toolsWindow.Height ) ) );
			if( minCam < 1 )
				minCam = 1;

			float dynamicMaxZoom = m_mainGraphInstance.MaxNodeDist / minCam;
			if( m_targetZoomIncrement < 0 )
				m_targetZoom = Mathf.Max( m_cameraZoom + m_targetZoomIncrement * Constants.CAMERA_ZOOM_SPEED, Constants.CAMERA_MIN_ZOOM );
			else if( CameraZoom < Mathf.Max( Constants.CAMERA_MAX_ZOOM, dynamicMaxZoom ) )
				m_targetZoom = m_cameraZoom + m_targetZoomIncrement * Constants.CAMERA_ZOOM_SPEED;// Mathf.Min( m_cameraZoom + zoomIncrement * Constants.CAMERA_ZOOM_SPEED, Mathf.Max( Constants.CAMERA_MAX_ZOOM, dynamicMaxZoom ) );

			m_zoomPivot = pivot;
		}

		void OnSelectionChange()
		{
			ForceRepaint();
		}

		private void OnFocus()
		{
			EditorGUI.FocusTextInControl( null );
		}

		void OnLostFocus()
		{
			m_lostFocus = true;
			m_multipleSelectionActive = false;
			m_wireReferenceUtils.InvalidateReferences();
			m_genericMessageUI.CleanUpMessageStack();
			m_nodeParametersWindow.OnLostFocus();
			m_paletteWindow.OnLostFocus();
		}

		void CopyToClipboard()
		{
			m_copyPasteDeltaMul = 0;
			m_copyPasteDeltaPos = new Vector2( float.MaxValue, float.MaxValue );
			m_clipboard.ClearClipboard();
			m_copyPasteInitialPos = m_mainGraphInstance.SelectedNodesCentroid;
			m_clipboard.AddToClipboard( m_mainGraphInstance.SelectedNodes, m_copyPasteInitialPos, m_mainGraphInstance );
		}

		ParentNode CreateNodeFromClipboardData( int clipId )
		{
			string[] parameters = m_clipboard.CurrentClipboardStrData[ clipId ].Data.Split( IOUtils.FIELD_SEPARATOR );
			System.Type nodeType = System.Type.GetType( parameters[ IOUtils.NodeTypeId ] );
			NodeAttributes attributes = m_contextMenu.GetNodeAttributesForType( nodeType );
			if( attributes != null && !UIUtils.GetNodeAvailabilityInBitArray( attributes.NodeAvailabilityFlags, m_mainGraphInstance.CurrentCanvasMode ) && !UIUtils.GetNodeAvailabilityInBitArray( attributes.NodeAvailabilityFlags, m_currentNodeAvailability ) )
				return null;

			ParentNode newNode = (ParentNode)ScriptableObject.CreateInstance( nodeType );
			newNode.IsNodeBeingCopied = true;
			if( newNode != null )
			{
				newNode.ContainerGraph = m_mainGraphInstance;
				newNode.ClipboardFullReadFromString( ref parameters );
				m_mainGraphInstance.AddNode( newNode, true, true, true, false );
				newNode.IsNodeBeingCopied = false;
				m_clipboard.CurrentClipboardStrData[ clipId ].NewNodeId = newNode.UniqueId;
				return newNode;
			}
			return null;
		}

		void CreateConnectionsFromClipboardData( int clipId )
		{
			if( String.IsNullOrEmpty( m_clipboard.CurrentClipboardStrData[ clipId ].Connections ) )
				return;
			string[] lines = m_clipboard.CurrentClipboardStrData[ clipId ].Connections.Split( IOUtils.LINE_TERMINATOR );

			for( int lineIdx = 0; lineIdx < lines.Length; lineIdx++ )
			{
				string[] parameters = lines[ lineIdx ].Split( IOUtils.FIELD_SEPARATOR );

				int InNodeId = 0;
				int InPortId = 0;
				int OutNodeId = 0;
				int OutPortId = 0;

				try
				{
					InNodeId = Convert.ToInt32( parameters[ IOUtils.InNodeId ] );
					InPortId = Convert.ToInt32( parameters[ IOUtils.InPortId ] );

					OutNodeId = Convert.ToInt32( parameters[ IOUtils.OutNodeId ] );
					OutPortId = Convert.ToInt32( parameters[ IOUtils.OutPortId ] );
				}
				catch( Exception e )
				{
					Debug.LogException( e );
				}


				int newInNodeId = m_clipboard.GeNewNodeId( InNodeId );
				int newOutNodeId = m_clipboard.GeNewNodeId( OutNodeId );

				if( newInNodeId > -1 && newOutNodeId > -1 )
				{
					ParentNode inNode = m_mainGraphInstance.GetNode( newInNodeId );
					ParentNode outNode = m_mainGraphInstance.GetNode( newOutNodeId );

					InputPort inputPort = null;
					OutputPort outputPort = null;

					if( inNode != null && outNode != null )
					{
						inNode.IsNodeBeingCopied = true;
						outNode.IsNodeBeingCopied = true;
						inputPort = inNode.GetInputPortByUniqueId( InPortId );
						outputPort = outNode.GetOutputPortByUniqueId( OutPortId );
						if( inputPort != null && outputPort != null )
						{
							inputPort.ConnectTo( newOutNodeId, OutPortId, outputPort.DataType, false );
							outputPort.ConnectTo( newInNodeId, InPortId, inputPort.DataType, inputPort.TypeLocked );

							inNode.OnInputPortConnected( InPortId, newOutNodeId, OutPortId );
							outNode.OnOutputPortConnected( OutPortId, newInNodeId, InPortId );
						}

						inNode.IsNodeBeingCopied = false;
						outNode.IsNodeBeingCopied = false;
					}
				}
			}
		}

		void PasteFromClipboard( bool copyConnections )
		{
			m_mainGraphInstance.IsDuplicating = true;
			m_copyPasteInitialPos = m_clipboard.GetDataFromEditorPrefs();
			if( m_clipboard.CurrentClipboardStrData.Count == 0 )
			{
				return;
			}

			Vector2 deltaPos = TranformedKeyEvtMousePos - m_copyPasteInitialPos;
			if( ( m_copyPasteDeltaPos - deltaPos ).magnitude > 5.0f )
			{
				m_copyPasteDeltaMul = 0;
			}
			else
			{
				m_copyPasteDeltaMul += 1;
			}
			m_copyPasteDeltaPos = deltaPos;

			m_mainGraphInstance.DeSelectAll();
			UIUtils.InhibitMessages = true;

			if( m_clipboard.CurrentClipboardStrData.Count > 0 )
			{
				UIUtils.MarkUndoAction();
				Undo.RegisterCompleteObjectUndo( this, Constants.UndoPasteNodeId );
			}

			List<ParentNode> createdNodes = new List<ParentNode>();
			for( int i = 0; i < m_clipboard.CurrentClipboardStrData.Count; i++ )
			{
				ParentNode node = CreateNodeFromClipboardData( i );
				if( node != null )
				{
					m_clipboard.CurrentClipboardStrData[ i ].NewNodeId = node.UniqueId;
					Vector2 pos = node.Vec2Position;
					node.Vec2Position = pos + deltaPos + m_copyPasteDeltaMul * Constants.CopyPasteDeltaPos;
					//node.RefreshExternalReferences();
					node.AfterDuplication();
					createdNodes.Add( node );
					m_mainGraphInstance.SelectNode( node, true, false );
				}
			}
			
			if( copyConnections )
			{
				for( int i = 0; i < m_clipboard.CurrentClipboardStrData.Count; i++ )
				{
					CreateConnectionsFromClipboardData( i );
				}
			}

			// Refresh external references must always be called after all nodes are created
			for( int i = 0; i < createdNodes.Count; i++ )
			{
				createdNodes[ i ].RefreshExternalReferences();
			}
			createdNodes.Clear();
			createdNodes = null;
			//Need to force increment on Undo because if not Undo may incorrectly group consecutive pastes
			Undo.IncrementCurrentGroup();

			UIUtils.InhibitMessages = false;
			ShaderIsModified = true;
			SetSaveIsDirty();
			ForceRepaint();
			m_mainGraphInstance.IsDuplicating = false;
		}

		public string GenerateGraphInfo()
		{
			string graphInfo = IOUtils.ShaderBodyBegin + '\n';
			string nodesInfo = "";
			string connectionsInfo = "";
			graphInfo += VersionInfo.FullLabel + '\n';
			graphInfo += (
							m_cameraInfo.x.ToString() + IOUtils.FIELD_SEPARATOR +
							m_cameraInfo.y.ToString() + IOUtils.FIELD_SEPARATOR +
							m_cameraInfo.width.ToString() + IOUtils.FIELD_SEPARATOR +
							m_cameraInfo.height.ToString() + IOUtils.FIELD_SEPARATOR +
							m_cameraOffset.x.ToString() + IOUtils.FIELD_SEPARATOR +
							m_cameraOffset.y.ToString() + IOUtils.FIELD_SEPARATOR +
							m_cameraZoom.ToString() + IOUtils.FIELD_SEPARATOR +
							m_nodeParametersWindow.IsMaximized + IOUtils.FIELD_SEPARATOR +
							m_paletteWindow.IsMaximized + '\n'
							);
			m_mainGraphInstance.OrderNodesByGraphDepth();
			m_mainGraphInstance.WriteToString( ref nodesInfo, ref connectionsInfo );
			graphInfo += nodesInfo;
			graphInfo += connectionsInfo;
			graphInfo += IOUtils.ShaderBodyEnd + '\n';

			return graphInfo;
		}

		// TODO: this need to be fused to the main load function somehow
		public static void LoadFromMeta( ref ParentGraph graph, GraphContextMenu contextMenu, string meta )
		{
			graph.IsLoading = true;
			graph.CleanNodes();

			int checksumId = meta.IndexOf( IOUtils.CHECKSUM );
			if( checksumId > -1 )
			{
				string checkSumStoredValue = meta.Substring( checksumId );
				string trimmedBuffer = meta.Remove( checksumId );

				string[] typeValuePair = checkSumStoredValue.Split( IOUtils.VALUE_SEPARATOR );
				if( typeValuePair != null && typeValuePair.Length == 2 )
				{
					// Check read checksum and compare with the actual shader body to detect external changes
					string currentChecksumValue = IOUtils.CreateChecksum( trimmedBuffer );
					if( DebugConsoleWindow.DeveloperMode && !currentChecksumValue.Equals( typeValuePair[ 1 ] ) )
					{
						//ShowMessage( "Wrong checksum" );
					}

					trimmedBuffer = trimmedBuffer.Replace( "\r", string.Empty );
					// find node info body
					int shaderBodyId = trimmedBuffer.IndexOf( IOUtils.ShaderBodyBegin );
					if( shaderBodyId > -1 )
					{
						trimmedBuffer = trimmedBuffer.Substring( shaderBodyId );
						//Find set of instructions
						string[] instructions = trimmedBuffer.Split( IOUtils.LINE_TERMINATOR );
						// First line is to be ignored and second line contains version
						string[] versionParams = instructions[ 1 ].Split( IOUtils.VALUE_SEPARATOR );
						if( versionParams.Length == 2 )
						{
							int version = 0;
							try
							{
								version = Convert.ToInt32( versionParams[ 1 ] );
							}
							catch( Exception e )
							{
								Debug.LogException( e );
							}

							//if( version > versionInfo.FullNumber )
							//{
								//ShowMessage( "This shader was created on a new ASE version\nPlease install v." + version );
							//}

							if( DebugConsoleWindow.DeveloperMode )
							{
								//if( version < versionInfo.FullNumber )
								//{
									//ShowMessage( "This shader was created on a older ASE version\nSaving will update it to the new one." );
								//}
							}

							graph.LoadedShaderVersion = version;
						}
						else
						{
							//ShowMessage( "Corrupted version" );
						}

						// Dummy values,camera values can only be applied after node loading is complete
						Rect dummyCameraInfo = new Rect();
						Vector2 dummyCameraOffset = new Vector2();
						//float dummyCameraZoom = 0;
						//bool applyDummy = false;
						//bool dummyNodeParametersWindowMaximized = false;
						//bool dummyPaletteWindowMaximized = false;

						//Second line contains camera information ( position, size, offset and zoom )
						string[] cameraParams = instructions[ 2 ].Split( IOUtils.FIELD_SEPARATOR );
						if( cameraParams.Length == 9 )
						{
							//applyDummy = true;
							try
							{
								dummyCameraInfo.x = Convert.ToSingle( cameraParams[ 0 ] );
								dummyCameraInfo.y = Convert.ToSingle( cameraParams[ 1 ] );
								dummyCameraInfo.width = Convert.ToSingle( cameraParams[ 2 ] );
								dummyCameraInfo.height = Convert.ToSingle( cameraParams[ 3 ] );
								dummyCameraOffset.x = Convert.ToSingle( cameraParams[ 4 ] );
								dummyCameraOffset.y = Convert.ToSingle( cameraParams[ 5 ] );

								//dummyCameraZoom = Convert.ToSingle( cameraParams[ 6 ] );
								//dummyNodeParametersWindowMaximized = Convert.ToBoolean( cameraParams[ 7 ] );
								//dummyPaletteWindowMaximized = Convert.ToBoolean( cameraParams[ 8 ] );
							}
							catch( Exception e )
							{
								Debug.LogException( e );
							}
						}
						else
						{
							//ShowMessage( "Camera parameters are corrupted" );
						}

						// valid instructions are only between the line after version and the line before the last one ( which contains ShaderBodyEnd ) 
						for( int instructionIdx = 3; instructionIdx < instructions.Length - 1; instructionIdx++ )
						{
							//TODO: After all is working, convert string parameters to ints in order to speed up reading
							string[] parameters = instructions[ instructionIdx ].Split( IOUtils.FIELD_SEPARATOR );

							// All nodes must be created before wiring the connections ... 
							// Since all nodes on the save op are written before the wires, we can safely create them
							// If that order is not maintained the it's because of external editing and its the users responsability
							switch( parameters[ 0 ] )
							{
								case IOUtils.NodeParam:
								{
									string typeStr = parameters[ IOUtils.NodeTypeId ];
									//System.Type type = System.Type.GetType( parameters[ IOUtils.NodeTypeId ] );
									System.Type type = System.Type.GetType( IOUtils.NodeTypeReplacer.ContainsKey( typeStr ) ? IOUtils.NodeTypeReplacer[ typeStr ] : typeStr );
									if( type != null )
									{
										System.Type oldType = type;
										NodeAttributes attribs = contextMenu.GetNodeAttributesForType( type );
										if( attribs == null )
										{
											attribs = contextMenu.GetDeprecatedNodeAttributesForType( type );
											if( attribs != null )
											{
												if( attribs.Deprecated && attribs.DeprecatedAlternativeType != null )
												{
													type = attribs.DeprecatedAlternativeType;
													//ShowMessage( string.Format( "Node {0} is deprecated and was replaced by {1} ", attribs.Name, attribs.DeprecatedAlternative ) );
												}
											}
										}

										ParentNode newNode = (ParentNode)ScriptableObject.CreateInstance( type );
										if( newNode != null )
										{
											try
											{
												newNode.ContainerGraph = graph;
												if( oldType != type )
												{
													newNode.ParentReadFromString( ref parameters );
													newNode.ReadFromDeprecated( ref parameters, oldType );
												}
												else
													newNode.ReadFromString( ref parameters );


												if( oldType == type )
												{
													newNode.ReadInputDataFromString( ref parameters );
													if( UIUtils.CurrentShaderVersion() > 5107 )
													{
														newNode.ReadOutputDataFromString( ref parameters );
													}
												}
											}
											catch( Exception e )
											{
												Debug.LogException( e, newNode );
											}
											graph.AddNode( newNode, false, true, false );
										}
									}
									else
									{
										UIUtils.ShowMessage( string.Format( "{0} is not a valid ASE node ", parameters[ IOUtils.NodeTypeId ] ), MessageSeverity.Error );
									}
								}
								break;
								case IOUtils.WireConnectionParam:
								{
									int InNodeId = 0;
									int InPortId = 0;
									int OutNodeId = 0;
									int OutPortId = 0;

									try
									{
										InNodeId = Convert.ToInt32( parameters[ IOUtils.InNodeId ] );
										InPortId = Convert.ToInt32( parameters[ IOUtils.InPortId ] );
										OutNodeId = Convert.ToInt32( parameters[ IOUtils.OutNodeId ] );
										OutPortId = Convert.ToInt32( parameters[ IOUtils.OutPortId ] );
									}
									catch( Exception e )
									{
										Debug.LogException( e );
									}

									ParentNode inNode = graph.GetNode( InNodeId );
									ParentNode outNode = graph.GetNode( OutNodeId );

									//if ( UIUtils.CurrentShaderVersion() < 5002 )
									//{
									//	InPortId = inNode.VersionConvertInputPortId( InPortId );
									//	OutPortId = outNode.VersionConvertOutputPortId( OutPortId );
									//}

									InputPort inputPort = null;
									OutputPort outputPort = null;
									if( inNode != null && outNode != null )
									{

										if( UIUtils.CurrentShaderVersion() < 5002 )
										{
											InPortId = inNode.VersionConvertInputPortId( InPortId );
											OutPortId = outNode.VersionConvertOutputPortId( OutPortId );

											inputPort = inNode.GetInputPortByArrayId( InPortId );
											outputPort = outNode.GetOutputPortByArrayId( OutPortId );
										}
										else
										{
											inputPort = inNode.GetInputPortByUniqueId( InPortId );
											outputPort = outNode.GetOutputPortByUniqueId( OutPortId );
										}

										if( inputPort != null && outputPort != null )
										{
											bool inputCompatible = inputPort.CheckValidType( outputPort.DataType );
											bool outputCompatible = outputPort.CheckValidType( inputPort.DataType );
											if( inputCompatible && outputCompatible )
											{
												inputPort.ConnectTo( OutNodeId, OutPortId, outputPort.DataType, false );
												outputPort.ConnectTo( InNodeId, InPortId, inputPort.DataType, inputPort.TypeLocked );

												inNode.OnInputPortConnected( InPortId, OutNodeId, OutPortId, false );
												outNode.OnOutputPortConnected( OutPortId, InNodeId, InPortId );
											}
											else if( DebugConsoleWindow.DeveloperMode )
											{
												if( !inputCompatible )
													UIUtils.ShowIncompatiblePortMessage( true, inNode, inputPort, outNode, outputPort );

												if( !outputCompatible )
													UIUtils.ShowIncompatiblePortMessage( true, outNode, outputPort, inNode, inputPort );
											}
										}
										else if( DebugConsoleWindow.DeveloperMode )
										{
											if( inputPort == null )
											{
												UIUtils.ShowMessage( "Input Port " + InPortId + " doesn't exist on node " + InNodeId, MessageSeverity.Error );
											}
											else
											{
												UIUtils.ShowMessage( "Output Port " + OutPortId + " doesn't exist on node " + OutNodeId, MessageSeverity.Error );
											}
										}
									}
									else if( DebugConsoleWindow.DeveloperMode )
									{
										if( inNode == null )
										{
											UIUtils.ShowMessage( "Input node " + InNodeId + " doesn't exist", MessageSeverity.Error );
										}
										else
										{
											UIUtils.ShowMessage( "Output node " + OutNodeId + " doesn't exist", MessageSeverity.Error );
										}
									}
								}
								break;
							}
						}
					}
				}
			}

			graph.CheckForDuplicates();
			graph.UpdateRegisters();
			graph.RefreshExternalReferences();
			graph.ForceSignalPropagationOnMasterNode();
			graph.LoadedShaderVersion = VersionInfo.FullNumber;
			//Reset();
			graph.IsLoading = false;
		}

		public ShaderLoadResult LoadFromDisk( string pathname, AmplifyShaderFunction shaderFunction = null )
		{
			m_mainGraphInstance.IsLoading = true;
			System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

			FullCleanUndoStack();
			m_performFullUndoRegister = true;

			UIUtils.DirtyMask = false;
			if( UIUtils.IsUnityNativeShader( pathname ) )
			{
				ShowMessage( "Cannot edit native unity shaders.\nReplacing by a new one." );
				return ShaderLoadResult.UNITY_NATIVE_PATHS;
			}

			m_lastOpenedLocation = pathname;
			Lastpath = pathname;

			string buffer = string.Empty;
			if( shaderFunction == null )
				buffer = IOUtils.LoadTextFileFromDisk( pathname );
			else
				buffer = shaderFunction.FunctionInfo;

			if( String.IsNullOrEmpty( buffer ) )
			{
				ShowMessage( "Could not open file " + pathname );
				return ShaderLoadResult.FILE_NOT_FOUND;
			}

			if( !IOUtils.HasValidShaderBody( ref buffer ) )
			{
				return ShaderLoadResult.ASE_INFO_NOT_FOUND;
			}

			m_mainGraphInstance.CleanNodes();
			Reset();

			Shader shader = null;
			ShaderLoadResult loadResult = ShaderLoadResult.LOADED;
			// Find checksum value on body
			int checksumId = buffer.IndexOf( IOUtils.CHECKSUM );
			if( checksumId > -1 )
			{
				string checkSumStoredValue = buffer.Substring( checksumId );
				string trimmedBuffer = buffer.Remove( checksumId );

				string[] typeValuePair = checkSumStoredValue.Split( IOUtils.VALUE_SEPARATOR );
				if( typeValuePair != null && typeValuePair.Length == 2 )
				{
					// Check read checksum and compare with the actual shader body to detect external changes
					string currentChecksumValue = IOUtils.CreateChecksum( trimmedBuffer );
					if( DebugConsoleWindow.DeveloperMode && !currentChecksumValue.Equals( typeValuePair[ 1 ] ) )
					{
						ShowMessage( "Wrong checksum" );
					}

					trimmedBuffer = trimmedBuffer.Replace( "\r", string.Empty );
					// find node info body
					int shaderBodyId = trimmedBuffer.IndexOf( IOUtils.ShaderBodyBegin );
					if( shaderBodyId > -1 )
					{
						trimmedBuffer = trimmedBuffer.Substring( shaderBodyId );
						//Find set of instructions
						string[] instructions = trimmedBuffer.Split( IOUtils.LINE_TERMINATOR );
						// First line is to be ignored and second line contains version
						string[] versionParams = instructions[ 1 ].Split( IOUtils.VALUE_SEPARATOR );
						if( versionParams.Length == 2 )
						{
							int version = 0;
							try
							{
								version = Convert.ToInt32( versionParams[ 1 ] );
							}
							catch( Exception e )
							{
								Debug.LogException( e );
							}

							if( version > VersionInfo.FullNumber )
							{
								ShowMessage( "This shader was created on a new ASE version\nPlease install v." + version );
							}

							if( DebugConsoleWindow.DeveloperMode )
							{
								if( version < VersionInfo.FullNumber )
								{
									ShowMessage( "This shader was created on a older ASE version\nSaving will update it to the new one." );
								}
							}

							m_mainGraphInstance.LoadedShaderVersion = version;
						}
						else
						{
							ShowMessage( "Corrupted version" );
						}

						// Dummy values,camera values can only be applied after node loading is complete
						Rect dummyCameraInfo = new Rect();
						Vector2 dummyCameraOffset = new Vector2();
						float dummyCameraZoom = 0;
						bool applyDummy = false;
						bool dummyNodeParametersWindowMaximized = false;
						bool dummyPaletteWindowMaximized = false;

						//Second line contains camera information ( position, size, offset and zoom )
						string[] cameraParams = instructions[ 2 ].Split( IOUtils.FIELD_SEPARATOR );
						if( cameraParams.Length == 9 )
						{
							applyDummy = true;
							try
							{
								dummyCameraInfo.x = Convert.ToSingle( cameraParams[ 0 ] );
								dummyCameraInfo.y = Convert.ToSingle( cameraParams[ 1 ] );
								dummyCameraInfo.width = Convert.ToSingle( cameraParams[ 2 ] );
								dummyCameraInfo.height = Convert.ToSingle( cameraParams[ 3 ] );
								dummyCameraOffset.x = Convert.ToSingle( cameraParams[ 4 ] );
								dummyCameraOffset.y = Convert.ToSingle( cameraParams[ 5 ] );
								dummyCameraZoom = Convert.ToSingle( cameraParams[ 6 ] );
								
								float centerWidth = ( this.position.width - dummyCameraInfo.width ) * 0.5f * dummyCameraZoom;
								float centerHeight = ( this.position.height - dummyCameraInfo.height ) * 0.5f * dummyCameraZoom;

								dummyCameraInfo.x += centerWidth;
								dummyCameraOffset.x += centerWidth;
								dummyCameraInfo.y += centerHeight;
								dummyCameraOffset.y += centerHeight;
								dummyNodeParametersWindowMaximized = Convert.ToBoolean( cameraParams[ 7 ] );
								dummyPaletteWindowMaximized = Convert.ToBoolean( cameraParams[ 8 ] );
							}
							catch( Exception e )
							{
								Debug.LogException( e );
							}
						}
						else
						{
							ShowMessage( "Camera parameters are corrupted" );
						}

						// valid instructions are only between the line after version and the line before the last one ( which contains ShaderBodyEnd ) 
						for( int instructionIdx = 3; instructionIdx < instructions.Length - 1; instructionIdx++ )
						{
							//TODO: After all is working, convert string parameters to ints in order to speed up reading
							string[] parameters = instructions[ instructionIdx ].Split( IOUtils.FIELD_SEPARATOR );

							// All nodes must be created before wiring the connections ... 
							// Since all nodes on the save op are written before the wires, we can safely create them
							// If that order is not maintained the it's because of external editing and its the users responsability
							switch( parameters[ 0 ] )
							{
								case IOUtils.NodeParam:
								{
									string typeStr = parameters[ IOUtils.NodeTypeId ];
									System.Type type = System.Type.GetType( IOUtils.NodeTypeReplacer.ContainsKey( typeStr ) ? IOUtils.NodeTypeReplacer[ typeStr ] : typeStr );
									if( type != null )
									{
										System.Type oldType = type;
										NodeAttributes attribs = m_contextMenu.GetNodeAttributesForType( type );
										if( attribs == null )
										{
											attribs = m_contextMenu.GetDeprecatedNodeAttributesForType( type );
											if( attribs != null )
											{
												if( attribs.Deprecated )
												{
													if( attribs.DeprecatedAlternativeType != null )
													{
														type = attribs.DeprecatedAlternativeType;
														ShowMessage( string.Format( "Node {0} is deprecated and was replaced by {1} ", attribs.Name, attribs.DeprecatedAlternative ) );
													}
													else
													{
														if( string.IsNullOrEmpty( attribs.DeprecatedAlternative ))
															ShowMessage( string.Format( Constants.DeprecatedNoAlternativeMessageStr, attribs.Name, attribs.DeprecatedAlternative ), MessageSeverity.Normal, false );
														else
															ShowMessage( string.Format( Constants.DeprecatedMessageStr, attribs.Name, attribs.DeprecatedAlternative ), MessageSeverity.Normal, false );
													}
												}
											}
										}

										ParentNode newNode = (ParentNode)ScriptableObject.CreateInstance( type );
										if( newNode != null )
										{
											try
											{
												newNode.ContainerGraph = m_mainGraphInstance;
												if( oldType != type )
												{
													newNode.ParentReadFromString( ref parameters );
													newNode.ReadFromDeprecated( ref parameters, oldType );
												}
												else
													newNode.ReadFromString( ref parameters );


												if( oldType == type )
												{
													newNode.ReadInputDataFromString( ref parameters );
													if( UIUtils.CurrentShaderVersion() > 5107 )
													{
														newNode.ReadOutputDataFromString( ref parameters );
													}
												}
											}
											catch( Exception e )
											{
												Debug.LogException( e, newNode );
											}
											m_mainGraphInstance.AddNode( newNode, false, true, false );
										}
									}
									else
									{
										ShowMessage( string.Format( "{0} is not a valid ASE node ", parameters[ IOUtils.NodeTypeId ] ), MessageSeverity.Error );
									}
								}
								break;
								case IOUtils.WireConnectionParam:
								{
									int InNodeId = 0;
									int InPortId = 0;
									int OutNodeId = 0;
									int OutPortId = 0;

									try
									{
										InNodeId = Convert.ToInt32( parameters[ IOUtils.InNodeId ] );
										InPortId = Convert.ToInt32( parameters[ IOUtils.InPortId ] );
										OutNodeId = Convert.ToInt32( parameters[ IOUtils.OutNodeId ] );
										OutPortId = Convert.ToInt32( parameters[ IOUtils.OutPortId ] );
									}
									catch( Exception e )
									{
										Debug.LogException( e );
									}

									ParentNode inNode = m_mainGraphInstance.GetNode( InNodeId );
									ParentNode outNode = m_mainGraphInstance.GetNode( OutNodeId );

									//if ( UIUtils.CurrentShaderVersion() < 5002 )
									//{
									//	InPortId = inNode.VersionConvertInputPortId( InPortId );
									//	OutPortId = outNode.VersionConvertOutputPortId( OutPortId );
									//}

									InputPort inputPort = null;
									OutputPort outputPort = null;
									if( inNode != null && outNode != null )
									{

										if( UIUtils.CurrentShaderVersion() < 5002 )
										{
											InPortId = inNode.VersionConvertInputPortId( InPortId );
											OutPortId = outNode.VersionConvertOutputPortId( OutPortId );

											inputPort = inNode.GetInputPortByArrayId( InPortId );
											outputPort = outNode.GetOutputPortByArrayId( OutPortId );
										}
										else
										{
											inputPort = inNode.GetInputPortByUniqueId( InPortId );
											outputPort = outNode.GetOutputPortByUniqueId( OutPortId );
										}

										if( inputPort != null && outputPort != null )
										{
											bool inputCompatible = inputPort.CheckValidType( outputPort.DataType );
											bool outputCompatible = outputPort.CheckValidType( inputPort.DataType );
											if( inputCompatible && outputCompatible )
											{
												inputPort.ConnectTo( OutNodeId, OutPortId, outputPort.DataType, false );
												outputPort.ConnectTo( InNodeId, InPortId, inputPort.DataType, inputPort.TypeLocked );

												inNode.OnInputPortConnected( InPortId, OutNodeId, OutPortId, false );
												outNode.OnOutputPortConnected( OutPortId, InNodeId, InPortId );
											}
											else if( DebugConsoleWindow.DeveloperMode )
											{
												if( !inputCompatible )
													UIUtils.ShowIncompatiblePortMessage( true, inNode, inputPort, outNode, outputPort );

												if( !outputCompatible )
													UIUtils.ShowIncompatiblePortMessage( true, outNode, outputPort, inNode, inputPort );
											}
										}
										else if( DebugConsoleWindow.DeveloperMode )
										{
											if( inputPort == null )
											{
												UIUtils.ShowMessage( "Input Port " + InPortId + " doesn't exist on node " + InNodeId, MessageSeverity.Error );
											}
											else
											{
												UIUtils.ShowMessage( "Output Port " + OutPortId + " doesn't exist on node " + OutNodeId, MessageSeverity.Error );
											}
										}
									}
									else if( DebugConsoleWindow.DeveloperMode )
									{
										if( inNode == null )
										{
											UIUtils.ShowMessage( "Input node " + InNodeId + " doesn't exist", MessageSeverity.Error );
										}
										else
										{
											UIUtils.ShowMessage( "Output node " + OutNodeId + " doesn't exist", MessageSeverity.Error );
										}
									}
								}
								break;
							}
						}

						if( shaderFunction != null )
						{
							m_onLoadDone = 2;
							if( applyDummy )
							{
								m_cameraInfo = dummyCameraInfo;
								m_cameraOffset = dummyCameraOffset;
								CameraZoom = dummyCameraZoom;
								if( DebugConsoleWindow.UseShaderPanelsInfo )
								{
									m_nodeParametersWindowMaximized = m_nodeParametersWindow.IsMaximized = dummyNodeParametersWindowMaximized;
									m_paletteWindowMaximized = m_paletteWindow.IsMaximized = dummyPaletteWindowMaximized;
								}
							}

						}
						else
						{
							shader = AssetDatabase.LoadAssetAtPath<Shader>( pathname );
							if( shader )
							{
								
								m_onLoadDone = 2;
								if( applyDummy )
								{
									m_cameraInfo = dummyCameraInfo;
									m_cameraOffset = dummyCameraOffset;
									CameraZoom = dummyCameraZoom;
									if( DebugConsoleWindow.UseShaderPanelsInfo )
									{
										m_nodeParametersWindowMaximized = m_nodeParametersWindow.IsMaximized = dummyNodeParametersWindowMaximized;
										m_paletteWindowMaximized = m_paletteWindow.IsMaximized = dummyPaletteWindowMaximized;
									}
								}
							}
							else
							{
								ShowMessage( "Could not load shader asset" );
							}
						}
					}
					else
					{
						ShowMessage( "Graph info not found" );
					}
				}
				else
				{
					ShowMessage( "Corrupted checksum" );
				}
			}
			else
			{
				ShowMessage( "Checksum not found" );
			}

			//m_mainGraphInstance.LoadedShaderVersion = m_versionInfo.FullNumber;
			if( UIUtils.CurrentMasterNode() )
				UIUtils.CurrentMasterNode().ForcePortType();

			UIUtils.DirtyMask = true;
			m_checkInvalidConnections = true;

			m_mainGraphInstance.CheckForDuplicates();
			m_mainGraphInstance.UpdateRegisters();
			m_mainGraphInstance.RefreshExternalReferences();
			m_mainGraphInstance.ForceSignalPropagationOnMasterNode();

			if( shaderFunction != null )
			{
				//if( CurrentGraph.CurrentFunctionOutput == null )
				//{
				//	//Fix in case a function output node is not marked as main node
				//	CurrentGraph.AssignMasterNode( UIUtils.FunctionOutputList()[ 0 ], false );
				//}
				CurrentGraph.CurrentShaderFunction = shaderFunction;
			}
			else
			{
				if( shader != null )
				{
					m_mainGraphInstance.UpdateShaderOnMasterNode( shader );
					if( m_mainGraphInstance.CurrentCanvasMode == NodeAvailability.TemplateShader )
					{
						m_mainGraphInstance.RefreshLinkedMasterNodes();
						m_mainGraphInstance.CurrentMasterNode.OnRefreshLinkedPortsComplete();
					}
				}
			}

			m_mainGraphInstance.LoadedShaderVersion = VersionInfo.FullNumber;

			System.Threading.Thread.CurrentThread.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;

			m_mainGraphInstance.IsLoading = false;
			return loadResult;
		}

		public void FullCleanUndoStack()
		{
			Undo.ClearUndo( this );
			m_mainGraphInstance.FullCleanUndoStack();
		}

		public void FullRegisterOnUndoStack()
		{
			Undo.RegisterCompleteObjectUndo( this, Constants.UndoRegisterFullGrapId );
			m_mainGraphInstance.FullRegisterOnUndoStack();
		}

		public void ShowPortInfo()
		{
			GetWindow<PortLegendInfo>();
		}

		public void ShowShaderLibrary()
		{
			GetWindow<ShaderLibrary>();
		}

		public void ShowMessage( string message, MessageSeverity severity = MessageSeverity.Normal, bool registerTimestamp = true )
		{
			if( UIUtils.InhibitMessages || m_genericMessageUI == null )
				return;

			m_consoleLogWindow.AddMessage( NodeMessageType.Info, message );
			if( m_genericMessageUI.DisplayingMessage )
			{
				m_genericMessageUI.AddToQueue( message, severity );
			}
			else
			{
				if( registerTimestamp )
					m_genericMessageUI.StartMessageCounter();

				ShowMessageImmediately( message, severity );
			}
		}

		public void ShowMessageImmediately( string message, MessageSeverity severity = MessageSeverity.Normal )
		{
			if( UIUtils.InhibitMessages )
				return;

			switch( severity )
			{
				case MessageSeverity.Normal: { m_genericMessageContent.text = string.Empty; } break;
				case MessageSeverity.Warning: { m_genericMessageContent.text = "Warning!\n"; } break;
				case MessageSeverity.Error: { m_genericMessageContent.text = "Error!!!\n"; } break;
			}
			m_genericMessageContent.text += message;
			Debug.Log( message );
			try
			{
				ShowNotification( m_genericMessageContent );
			}
			catch( Exception e )
			{
				Debug.LogException( e );
			}
		}

		public bool MouseInteracted = false;


		void CheckNodeReplacement()
		{
			if( m_replaceMasterNode )
			{
				m_replaceMasterNode = false;
				switch( m_replaceMasterNodeType )
				{
					default:
					case AvailableShaderTypes.SurfaceShader:
					{
						SetStandardShader();
					}
					break;
					case AvailableShaderTypes.Template:
					{

						if( m_replaceMasterNodeDataFromCache )
						{
							TemplateDataParent templateData = m_templatesManager.GetTemplate( m_replaceMasterNodeData );
							m_mainGraphInstance.CrossCheckTemplateNodes( templateData );
							//m_clipboard.GetMultiPassNodesFromClipboard( m_mainGraphInstance.MultiPassMasterNodes.NodesList );
						}
						else
						{
							SetTemplateShader( m_replaceMasterNodeData, false );
						}
					}
					break;
				}
			}
		}

		void OnGUI()
		{

			AmplifyShaderEditorWindow cacheWindow = UIUtils.CurrentWindow;
			UIUtils.CurrentWindow = this;

			if( !m_initialized || (object)UIUtils.MainSkin == null || !UIUtils.Initialized )
			{
				UIUtils.InitMainSkin();
				Init();
			}

			m_currentEvent = Event.current;
			if( m_currentEvent.type == EventType.ExecuteCommand || m_currentEvent.type == EventType.ValidateCommand )
				m_currentCommandName = m_currentEvent.commandName;
			else
				m_currentCommandName = string.Empty;

			System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

			MouseInteracted = false;

			if( m_refreshOnUndo )
			{
				m_refreshOnUndo = false;
				m_mainGraphInstance.RefreshOnUndo();
			}

			if( m_refreshAvailableNodes )
			{
				RefreshAvaibleNodes();
			}

			if( m_previousShaderFunction != CurrentGraph.CurrentShaderFunction )
			{
				m_nodeParametersWindow.ForceUpdate = true;
				m_previousShaderFunction = CurrentGraph.CurrentShaderFunction;
			}

			if( m_nodeToFocus != null && m_currentEvent.type == EventType.Layout )
			{
				FocusOnNode( m_nodeToFocus, m_zoomToFocus, m_selectNodeToFocus );
				m_nodeToFocus = null;
			}

			m_mainGraphInstance.OnDuplicateEventWrapper();

			m_currentInactiveTime = CalculateInactivityTime();

			if( m_nodeParametersWindow != null && m_innerEditorVariables.NodeParametersMaximized!= m_nodeParametersWindow.IsMaximized )
				m_innerEditorVariables.NodeParametersMaximized = m_nodeParametersWindow.IsMaximized;
			if( m_paletteWindow != null && m_innerEditorVariables.NodePaletteMaximized != m_paletteWindow.IsMaximized )
				m_innerEditorVariables.NodePaletteMaximized = m_paletteWindow.IsMaximized;

			if( m_checkInvalidConnections )
			{
				m_checkInvalidConnections = false;
				m_mainGraphInstance.DeleteInvalidConnections();
			}

			//if ( m_repaintIsDirty )
			//{
			//	m_repaintIsDirty = false;
			//	ForceRepaint();
			//}

			if( m_forcingMaterialUpdateFlag )
			{
				Focus();
				if( m_materialsToUpdate.Count > 0 )
				{
					float percentage = 100.0f * (float)( UIUtils.TotalExampleMaterials - m_materialsToUpdate.Count ) / (float)UIUtils.TotalExampleMaterials;
					if( m_forcingMaterialUpdateOp ) // Read
					{
						Debug.Log( percentage + "% Recompiling " + m_materialsToUpdate[ 0 ].name );
						LoadDroppedObject( true, m_materialsToUpdate[ 0 ].shader, m_materialsToUpdate[ 0 ] );
					}
					else // Write
					{
						Debug.Log( percentage + "% Saving " + m_materialsToUpdate[ 0 ].name );
						SaveToDisk( false );
						m_materialsToUpdate.RemoveAt( 0 );
					}
					m_forcingMaterialUpdateOp = !m_forcingMaterialUpdateOp;
				}
				else
				{
					Debug.Log( "100% - All Materials compiled " );
					m_forcingMaterialUpdateFlag = false;
				}
			}


			if( m_removedKeyboardFocus )
			{
				m_removedKeyboardFocus = false;
				GUIUtility.keyboardControl = 0;
			}


			Vector2 pos = m_currentEvent.mousePosition;
			pos.x += position.x;
			pos.y += position.y;
			m_insideEditorWindow = position.Contains( pos );

			if( m_delayedLoadObject != null && m_mainGraphInstance.CurrentMasterNode != null )
			{
				LoadObject( m_delayedLoadObject );
				m_delayedLoadObject = null;
			}
			else if( m_delayedLoadObject != null && m_mainGraphInstance.CurrentOutputNode != null )
			{
				LoadObject( m_delayedLoadObject );
				m_delayedLoadObject = null;
			}

			if( m_delayedMaterialSet != null && m_mainGraphInstance.CurrentMasterNode != null )
			{
				m_mainGraphInstance.UpdateMaterialOnMasterNode( m_delayedMaterialSet );
				m_mainGraphInstance.SetMaterialModeOnGraph( m_delayedMaterialSet );
				CurrentSelection = ASESelectionMode.Material;
				IsShaderFunctionWindow = false;
				m_delayedMaterialSet = null;
			}

			Material currentMaterial = m_mainGraphInstance.CurrentMaterial;
			if( m_forceUpdateFromMaterialFlag )
			{
				Focus();
				m_forceUpdateFromMaterialFlag = false;
				if( currentMaterial != null )
				{
					m_mainGraphInstance.CopyValuesFromMaterial( currentMaterial );
					m_repaintIsDirty = true;
				}
			}

			m_repaintCount = 0;
			m_cameraInfo = position;

			//if( m_currentEvent.type == EventType.keyDown )
			if( m_currentEvent.type == EventType.Repaint )
				m_keyEvtMousePos2D = m_currentEvent.mousePosition;

			m_currentMousePos2D = m_currentEvent.mousePosition;
			m_currentMousePos.x = m_currentMousePos2D.x;
			m_currentMousePos.y = m_currentMousePos2D.y;

			m_graphArea.width = m_cameraInfo.width;
			m_graphArea.height = m_cameraInfo.height;

			m_autoPanDirActive = m_lmbPressed || m_forceAutoPanDir || m_multipleSelectionActive || m_wireReferenceUtils.ValidReferences();


			// Need to use it in order to prevent Mismatched LayoutGroup on ValidateCommand when rendering nodes
			//if( Event.current.type == EventType.ValidateCommand )
			//{
			//	Event.current.Use();
			//}

			// Nodes Graph background area
			//GUILayout.BeginArea( m_graphArea, "Nodes" );
			{
				// Camera movement is simulated by grabing the current camera offset, transforming it into texture space and manipulating the tiled texture uv coords
				GUI.DrawTextureWithTexCoords( m_graphArea, m_graphBgTexture,
					new Rect( ( -m_cameraOffset.x / m_graphBgTexture.width ),
								( m_cameraOffset.y / m_graphBgTexture.height ) - m_cameraZoom * m_cameraInfo.height / m_graphBgTexture.height,
								m_cameraZoom * m_cameraInfo.width / m_graphBgTexture.width,
								m_cameraZoom * m_cameraInfo.height / m_graphBgTexture.height ) );

				Color col = GUI.color;
				GUI.color = new Color( 1, 1, 1, 0.7f );
				GUI.DrawTexture( m_graphArea, m_graphFgTexture, ScaleMode.StretchToFill, true );
				GUI.color = col;
			}
			//GUILayout.EndArea();

			bool restoreMouse = false;
			if( InsideMenus( m_currentMousePos2D ) /*|| _confirmationWindow.IsActive*/ )
			{
				if( Event.current.type == EventType.MouseDown )
				{
					restoreMouse = true;
					Event.current.type = EventType.Ignore;
				}

				// Must guarantee that mouse up ops on menus will reset auto pan if it is set
				if( m_currentEvent.type == EventType.MouseUp && m_currentEvent.button == ButtonClickId.LeftMouseButton )
				{
					m_lmbPressed = false;
				}

			}
			// Nodes
			//GUILayout.BeginArea( m_graphArea );
			{
				m_drawInfo.CameraArea = m_cameraInfo;
				m_drawInfo.TransformedCameraArea = m_graphArea;

				m_drawInfo.MousePosition = m_currentMousePos2D;
				m_drawInfo.CameraOffset = m_cameraOffset;
				m_drawInfo.InvertedZoom = 1 / m_cameraZoom;
				m_drawInfo.LeftMouseButtonPressed = m_currentEvent.button == ButtonClickId.LeftMouseButton;
				m_drawInfo.CurrentEventType = m_currentEvent.type;
				m_drawInfo.ZoomChanged = m_zoomChanged;

				m_drawInfo.TransformedMousePos = m_currentMousePos2D * m_cameraZoom - m_cameraOffset;

				if( m_drawInfo.CurrentEventType == EventType.Repaint )
					UIUtils.UpdateMainSkin( m_drawInfo );

				// Draw mode indicator
				m_modeWindow.Draw( m_graphArea, m_currentMousePos2D, m_mainGraphInstance.CurrentShader, currentMaterial,
									0.5f * ( m_graphArea.width - m_paletteWindow.RealWidth - m_nodeParametersWindow.RealWidth ),
									( m_nodeParametersWindow.IsMaximized ? m_nodeParametersWindow.RealWidth : 0 ),
									( m_paletteWindow.IsMaximized ? m_paletteWindow.RealWidth : 0 )/*, m_openedAssetFromNode*/ );

				PreTestLeftMouseDown();
				//m_mainGraphInstance.DrawBezierBoundingBox();
				//CheckNodeReplacement();

				// Main Graph Draw
				m_repaintIsDirty = m_mainGraphInstance.Draw( m_drawInfo ) || m_repaintIsDirty;

				m_mainGraphInstance.DrawGrid( m_drawInfo );
				bool hasUnusedConnNodes = m_mainGraphInstance.HasUnConnectedNodes;
				m_toolsWindow.SetStateOnButton( ToolButtonType.CleanUnusedNodes, hasUnusedConnNodes ? 1 : 0 );

				m_zoomChanged = false;

				MasterNode masterNode = m_mainGraphInstance.CurrentMasterNode;
				if( masterNode != null )
				{
					m_toolsWindow.DrawShaderTitle( m_nodeParametersWindow, m_paletteWindow, AvailableCanvasWidth, m_graphArea.height, masterNode.CroppedShaderName );
				}
				else if( m_mainGraphInstance.CurrentOutputNode != null )
				{
					string functionName = string.Empty;

					if( m_mainGraphInstance.CurrentShaderFunction != null )
						functionName = m_mainGraphInstance.CurrentShaderFunction.FunctionName;
					m_toolsWindow.DrawShaderTitle( m_nodeParametersWindow, m_paletteWindow, AvailableCanvasWidth, m_graphArea.height, functionName );
				}
			}

			//GUILayout.EndArea();

			if( restoreMouse )
			{
				Event.current.type = EventType.MouseDown;
				m_drawInfo.CurrentEventType = EventType.MouseDown;
			}

			m_toolsWindow.InitialX = m_nodeParametersWindow.RealWidth;
			m_toolsWindow.Width = m_cameraInfo.width - ( m_nodeParametersWindow.RealWidth + m_paletteWindow.RealWidth );
			m_toolsWindow.Draw( m_cameraInfo, m_currentMousePos2D, m_currentEvent.button, false );

			m_tipsWindow.Draw( m_cameraInfo, m_currentMousePos2D, m_currentEvent.button, false );

			bool autoMinimize = false;
			if( position.width < m_lastWindowWidth && position.width < Constants.MINIMIZE_WINDOW_LOCK_SIZE )
			{
				autoMinimize = true;
			}

			if( autoMinimize )
				m_nodeParametersWindow.IsMaximized = false;

			ParentNode selectedNode = ( m_mainGraphInstance.SelectedNodes.Count == 1 ) ? m_mainGraphInstance.SelectedNodes[ 0 ] : m_mainGraphInstance.CurrentMasterNode;
			m_repaintIsDirty = m_nodeParametersWindow.Draw( m_cameraInfo, selectedNode, m_currentMousePos2D, m_currentEvent.button, false ) || m_repaintIsDirty; //TODO: If multiple nodes from the same type are selected also show a parameters window which modifies all of them 
			if( m_nodeParametersWindow.IsResizing )
				m_repaintIsDirty = true;

			// Test to ignore mouse on main palette when inside context palette ... IsInside also takes active state into account 
			bool ignoreMouseForPalette = m_contextPalette.IsInside( m_currentMousePos2D );
			if( ignoreMouseForPalette && Event.current.type == EventType.MouseDown )
			{
				Event.current.type = EventType.Ignore;
				m_drawInfo.CurrentEventType = EventType.Ignore;
			}
			if( autoMinimize )
				m_paletteWindow.IsMaximized = false;

			m_paletteWindow.Draw( m_cameraInfo, m_currentMousePos2D, m_currentEvent.button, !m_contextPalette.IsActive );
			if( m_paletteWindow.IsResizing )
			{
				m_repaintIsDirty = true;
			}

			if( ignoreMouseForPalette )
			{
				if( restoreMouse )
				{
					Event.current.type = EventType.MouseDown;
					m_drawInfo.CurrentEventType = EventType.MouseDown;
				}
			}

			if( m_contextPalette.IsActive )
			{
				m_contextPalette.Draw( m_cameraInfo, m_currentMousePos2D, m_currentEvent.button, m_contextPalette.IsActive );
			}

			if( m_palettePopup.IsActive )
			{
				m_palettePopup.Draw( m_currentMousePos2D );
				m_repaintIsDirty = true;
				int controlID = GUIUtility.GetControlID( FocusType.Passive );
				if( m_currentEvent.GetTypeForControl( controlID ) == EventType.MouseUp )
				{
					if( m_currentEvent.button == ButtonClickId.LeftMouseButton )
					{
						m_palettePopup.Deactivate();
						if( !InsideMenus( m_currentMousePos2D ) )
						{
							ParentNode newNode = CreateNode( m_paletteChosenType, TranformedMousePos, m_paletteChosenFunction );
							//Debug.Log("created menu");
							m_mainGraphInstance.SelectNode( newNode, false, false );

							bool find = false;
							if( newNode is FunctionNode && CurrentGraph.CurrentShaderFunction != null )
								find = SearchFunctionNodeRecursively( CurrentGraph.CurrentShaderFunction );

							if( find )
							{
								DestroyNode( newNode, false );
								ShowMessage( "Shader Function loop detected, new node was removed to prevent errors." );
							}
							else
							{
								newNode.RefreshExternalReferences();
							}
						}
					}
				}
			}

			if( m_consoleLogWindow.IsActive )
			{
				m_consoleLogWindow.InitialX = m_nodeParametersWindow.IsMaximized ? m_nodeParametersWindow.RealWidth : 0;
				m_consoleLogWindow.Width = m_cameraInfo.width - ( ( m_nodeParametersWindow.IsMaximized ? m_nodeParametersWindow.RealWidth : 0 ) + ( m_paletteWindow.IsMaximized ? m_paletteWindow.RealWidth : 0 ) );
				m_consoleLogWindow.Draw( m_cameraInfo, m_currentMousePos2D, m_currentEvent.button, false );
			}

			// Handle all events ( mouse interaction + others )
			if( !MouseInteracted )
				HandleGUIEvents();

			if( m_currentEvent.type == EventType.Repaint )
			{
				m_mainGraphInstance.UpdateMarkForDeletion();
			}
			// UI Overlay
			// Selection Box
			if( m_multipleSelectionActive )
			{
				UpdateSelectionArea();
				Rect transformedArea = m_multipleSelectionArea;
				transformedArea.position = ( transformedArea.position + m_cameraOffset ) / m_cameraZoom;
				transformedArea.size /= m_cameraZoom;

				if( transformedArea.width < 0 )
				{
					transformedArea.width = -transformedArea.width;
					transformedArea.x -= transformedArea.width;
				}

				if( transformedArea.height < 0 )
				{
					transformedArea.height = -transformedArea.height;
					transformedArea.y -= transformedArea.height;
				}
				Color original = GUI.color;
				GUI.color = Constants.BoxSelectionColor;
				GUI.Label( transformedArea, "", UIUtils.Box );
				GUI.color = original;
				//GUI.backgroundColor = original;
			}

			bool isResizing = m_nodeParametersWindow.IsResizing || m_paletteWindow.IsResizing;
			//Test boundaries for auto-pan
			if( !isResizing && m_autoPanDirActive )
			{
				m_autoPanArea[ (int)AutoPanLocation.LEFT ].AdjustInitialX = m_nodeParametersWindow.IsMaximized ? m_nodeParametersWindow.RealWidth : 0;
				m_autoPanArea[ (int)AutoPanLocation.RIGHT ].AdjustInitialX = m_paletteWindow.IsMaximized ? -m_paletteWindow.RealWidth : 0;
				Vector2 autoPanDir = Vector2.zero;
				for( int i = 0; i < m_autoPanArea.Length; i++ )
				{
					if( m_autoPanArea[ i ].CheckArea( m_currentMousePos2D, m_cameraInfo, false ) )
					{
						autoPanDir += m_autoPanArea[ i ].Velocity;
					}
				}
				m_cameraOffset += autoPanDir;
				if( !m_wireReferenceUtils.ValidReferences() && m_insideEditorWindow && !m_altBoxSelection )
				{
					m_mainGraphInstance.MoveSelectedNodes( -autoPanDir );
				}

				m_repaintIsDirty = true;
			}

			m_isDirty = m_isDirty || m_mainGraphInstance.IsDirty;
			if( m_isDirty )
			{
				m_isDirty = false;
				//ShaderIsModified = true;
				EditorUtility.SetDirty( this );
			}

			m_saveIsDirty = m_saveIsDirty || m_mainGraphInstance.SaveIsDirty;
			if( m_liveShaderEditing )
			{
				if( m_saveIsDirty )
				{
					if( m_liveShaderEditing && focusedWindow && m_currentInactiveTime > InactivitySaveTime )
					{
						m_saveIsDirty = false;
						if( m_mainGraphInstance.CurrentMasterNodeId != Constants.INVALID_NODE_ID )
						{
							SaveToDisk( true );
						}
						else
						{
							ShowMessage( LiveShaderError );
						}
					}
				}
			}
			else if( m_saveIsDirty )
			{
				ShaderIsModified = true;
				m_saveIsDirty = false;
			}

			if( m_onLoadDone > 0 )
			{
				m_onLoadDone--;
				if( m_onLoadDone == 0 )
				{
					ShaderIsModified = false;
				}
			}

			if( m_repaintIsDirty )
			{
				m_repaintIsDirty = false;
				Repaint();
				//ForceRepaint();
			}

			if( m_cacheSaveOp )
			{
				if( ( EditorApplication.timeSinceStartup - m_lastTimeSaved ) > SaveTime )
				{
					SaveToDisk( false );
				}
			}
			m_genericMessageUI.CheckForMessages();

			if( m_ctrlSCallback )
			{
				m_ctrlSCallback = false;
				OnToolButtonPressed( ToolButtonType.Update );
			}

			m_lastWindowWidth = position.width;
			m_nodeExporterUtils.Update();

			if( m_markedToSave )
			{
				m_markedToSave = false;
				SaveToDisk( false );
			}
			if( m_performFullUndoRegister )
			{
				m_performFullUndoRegister = false;
				FullRegisterOnUndoStack();
			}

			if( CheckFunctions )
				CheckFunctions = false;

			System.Threading.Thread.CurrentThread.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;

			UIUtils.CurrentWindow = cacheWindow;
			if( !m_nodesLoadedCorrectly )
			{
				try
				{
					ShowNotification( NodesExceptionMessage );
				}
				catch( Exception e )
				{
					Debug.LogException( e );
				}
			}

			CheckNodeReplacement();
		}

		
		void OnInspectorUpdate()
		{
			if( m_afterDeserializeFlag )
			{
				m_afterDeserializeFlag = false;
				//m_mainGraphInstance.ParentWindow = this;
			}

			if( IsShaderFunctionWindow && CurrentGraph.CurrentShaderFunction == null )
			{
				Close();
			}
		}

		public void SetCtrlSCallback( bool imediate )
		{
			//MasterNode node = _mainGraphInstance.CurrentMasterNode;
			if( /*node != null && node.CurrentShader != null && */m_shaderIsModified )
			{
				if( imediate )
				{
					OnToolButtonPressed( ToolButtonType.Update );
				}
				else
				{
					m_ctrlSCallback = true;
				}
			}
		}

		public void SetSaveIsDirty()
		{
			m_saveIsDirty = true && UIUtils.DirtyMask;
		}

		public void OnPaletteNodeCreate( System.Type type, string name, AmplifyShaderFunction function )
		{
			m_mainGraphInstance.DeSelectAll();
			m_paletteChosenType = type;
			m_paletteChosenFunction = function;
			m_palettePopup.Activate( name );
		}

		public void OnContextPaletteNodeCreate( System.Type type, string name, AmplifyShaderFunction function )
		{
			m_mainGraphInstance.DeSelectAll();
			ParentNode newNode = CreateNode( type, m_contextPalette.StartDropPosition * m_cameraZoom - m_cameraOffset, function );
			//Debug.Log( "created context" );
			m_mainGraphInstance.SelectNode( newNode, false, false );
			bool find = false;
			if( newNode is FunctionNode && CurrentGraph.CurrentShaderFunction != null )
				find = SearchFunctionNodeRecursively( CurrentGraph.CurrentShaderFunction );

			if( find )
			{
				DestroyNode( newNode, false );
				ShowMessage( "Shader Function loop detected, new node was removed to prevent errors." );
			}
			else
			{
				newNode.RefreshExternalReferences();
			}
		}

		void OnNodeStoppedMovingEvent( ParentNode node )
		{
			CheckZoomBoundaries( node.Vec2Position );
			//ShaderIsModified = true;
		}

		void OnRefreshFunctionNodeEvent( FunctionNode node )
		{
			Debug.Log( node );
		}

		void OnMaterialUpdated( MasterNode masterNode )
		{
			if( masterNode != null )
			{
				if( masterNode.CurrentMaterial )
				{
					m_toolsWindow.SetStateOnButton( ToolButtonType.Update, ShaderIsModified ? 0 : 2, ShaderIsModified ? "Click to update Shader preview." : "Preview up-to-date." );
				}
				else
				{
					m_toolsWindow.SetStateOnButton( ToolButtonType.Update, 1, "Set an active Material in the Master Node." );
				}
				UpdateLiveUI();
			}
			else
			{
				m_toolsWindow.SetStateOnButton( ToolButtonType.Update, 1, "Set an active Material in the Master Node." );
			}
		}

		void OnShaderUpdated( MasterNode masterNode )
		{
			m_toolsWindow.SetStateOnButton( ToolButtonType.OpenSourceCode, masterNode.CurrentShader != null ? 1 : 0 );
		}

		public void CheckZoomBoundaries( Vector2 newPosition )
		{
			if( newPosition.x < m_minNodePos.x )
			{
				m_minNodePos.x = newPosition.x;
			}
			else if( newPosition.x > m_maxNodePos.x )
			{
				m_maxNodePos.x = newPosition.x;
			}

			if( newPosition.y < m_minNodePos.y )
			{
				m_minNodePos.y = newPosition.y;
			}
			else if( newPosition.y > m_maxNodePos.y )
			{
				m_maxNodePos.y = newPosition.y;
			}
		}
		public void DestroyNode( ParentNode node, bool registerUndo = true ) { m_mainGraphInstance.DestroyNode( node, registerUndo ); }
		public ParentNode CreateNode( System.Type type, Vector2 position, AmplifyShaderFunction function = null, bool selectNode = true )
		{
			ParentNode node;
			if( function == null )
				node = m_mainGraphInstance.CreateNode( type, true );
			else
				node = m_mainGraphInstance.CreateNode( function, true );

			Vector2 newPosition = position;
			node.Vec2Position = newPosition;
			CheckZoomBoundaries( newPosition );

			// Connect node if a wire is active 
			if( m_wireReferenceUtils.ValidReferences() )
			{
				if( m_wireReferenceUtils.InputPortReference.IsValid )
				{
					ParentNode originNode = m_mainGraphInstance.GetNode( m_wireReferenceUtils.InputPortReference.NodeId );
					InputPort originPort = originNode.GetInputPortByUniqueId( m_wireReferenceUtils.InputPortReference.PortId );
					OutputPort outputPort = node.GetFirstOutputPortOfType( m_wireReferenceUtils.InputPortReference.DataType, true );
					if( outputPort != null && originPort.CheckValidType( outputPort.DataType ) && ( !m_wireReferenceUtils.InputPortReference.TypeLocked ||
												m_wireReferenceUtils.InputPortReference.DataType == WirePortDataType.OBJECT ||
												( m_wireReferenceUtils.InputPortReference.TypeLocked && outputPort.DataType == m_wireReferenceUtils.InputPortReference.DataType ) ) )
					{

						//link output to input
						if( outputPort.ConnectTo( m_wireReferenceUtils.InputPortReference.NodeId, m_wireReferenceUtils.InputPortReference.PortId, m_wireReferenceUtils.InputPortReference.DataType, m_wireReferenceUtils.InputPortReference.TypeLocked ) )
							node.OnOutputPortConnected( outputPort.PortId, m_wireReferenceUtils.InputPortReference.NodeId, m_wireReferenceUtils.InputPortReference.PortId );

						//link input to output
						if( originNode.GetInputPortByUniqueId( m_wireReferenceUtils.InputPortReference.PortId ).ConnectTo( outputPort.NodeId, outputPort.PortId, m_wireReferenceUtils.InputPortReference.DataType, m_wireReferenceUtils.InputPortReference.TypeLocked ) )
							originNode.OnInputPortConnected( m_wireReferenceUtils.InputPortReference.PortId, node.UniqueId, outputPort.PortId );
					}
				}

				if( m_wireReferenceUtils.OutputPortReference.IsValid )
				{
					ParentNode originNode = m_mainGraphInstance.GetNode( m_wireReferenceUtils.OutputPortReference.NodeId );
					InputPort inputPort = node.GetFirstInputPortOfType( m_wireReferenceUtils.OutputPortReference.DataType, true );

					if( inputPort != null && ( !inputPort.TypeLocked ||
													inputPort.DataType == WirePortDataType.OBJECT ||
													( inputPort.TypeLocked && inputPort.DataType == m_wireReferenceUtils.OutputPortReference.DataType ) ) )
					{

						inputPort.InvalidateAllConnections();
						//link input to output
						if( inputPort.ConnectTo( m_wireReferenceUtils.OutputPortReference.NodeId, m_wireReferenceUtils.OutputPortReference.PortId, m_wireReferenceUtils.OutputPortReference.DataType, inputPort.TypeLocked ) )
							node.OnInputPortConnected( inputPort.PortId, m_wireReferenceUtils.OutputPortReference.NodeId, m_wireReferenceUtils.OutputPortReference.PortId );
						//link output to input

						if( originNode.GetOutputPortByUniqueId( m_wireReferenceUtils.OutputPortReference.PortId ).ConnectTo( inputPort.NodeId, inputPort.PortId, m_wireReferenceUtils.OutputPortReference.DataType, inputPort.TypeLocked ) )
							originNode.OnOutputPortConnected( m_wireReferenceUtils.OutputPortReference.PortId, node.UniqueId, inputPort.PortId );
					}
				}
				m_wireReferenceUtils.InvalidateReferences();

				//for ( int i = 0; i < m_mainGraphInstance.VisibleNodes.Count; i++ )
				//{
				//	m_mainGraphInstance.VisibleNodes[ i ].OnNodeInteraction( node );
				//}
			}

			if( selectNode )
				m_mainGraphInstance.SelectNode( node, false, false );
			//_repaintIsDirty = true

			SetSaveIsDirty();
			ForceRepaint();
			return node;
		}

		public void UpdateTime()
		{
			if( UIUtils.CurrentWindow != this )
				return;

			double deltaTime = Time.realtimeSinceStartup - m_time;
			m_time = Time.realtimeSinceStartup;

			if( m_smoothZoom )
			{
				if( Mathf.Abs( m_targetZoom - m_cameraZoom ) < 0.001f )
				{
					m_smoothZoom = false;
					m_cameraZoom = m_targetZoom;
					m_zoomTime = 0;
				}
				else
				{
					m_zoomTime += deltaTime;
					Vector2 canvasPos = m_zoomPivot * m_cameraZoom;
					m_cameraZoom = Mathf.SmoothDamp( m_cameraZoom, m_targetZoom, ref m_zoomVelocity, 0.1f, 10000, (float)deltaTime * 1.5f );
					canvasPos = canvasPos - m_zoomPivot * m_cameraZoom;
					m_cameraOffset = m_cameraOffset - canvasPos;
					m_targetOffset = m_targetOffset - canvasPos;
				}

			}

			if( m_smoothOffset )
			{
				if( ( m_targetOffset - m_cameraOffset ).SqrMagnitude() < 1f )
				{
					m_smoothOffset = false;
					m_offsetTime = 0;
				}
				else
				{
					m_offsetTime += deltaTime;
					m_cameraOffset = Vector2.SmoothDamp( m_cameraOffset, m_targetOffset, ref m_camVelocity, 0.1f, 100000, (float)deltaTime * 1.5f );
				}
			}

			if( m_cachedEditorTimeId == -1 )
				m_cachedEditorTimeId = Shader.PropertyToID( "_EditorTime" );

			if( m_cachedEditorDeltaTimeId == -1 )
				m_cachedEditorDeltaTimeId = Shader.PropertyToID( "_EditorDeltaTime" );

			//Update Game View?
			//Shader.SetGlobalVector( "_Time", new Vector4( Time.realtimeSinceStartup / 20, Time.realtimeSinceStartup, Time.realtimeSinceStartup * 2, Time.realtimeSinceStartup * 3 ) );

			//System.Type T = System.Type.GetType( "UnityEditor.GameView,UnityEditor" );
			//UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll( T );
			//EditorWindow gameView = ( array.Length <= 0 ) ? null : ( ( EditorWindow ) array[ 0 ] );
			//gameView.Repaint();

			if( RenderSettings.sun != null )
			{
				Vector3 lightdir = -RenderSettings.sun.transform.forward;//.rotation.eulerAngles;

				Shader.SetGlobalVector( "_EditorWorldLightPos", new Vector4( lightdir.x, lightdir.y, lightdir.z, 0 ) );
				Shader.SetGlobalColor( "_EditorLightColor", RenderSettings.sun.color.linear );
			}
			Shader.SetGlobalFloat( "_EditorTime", (float)m_time );
			Shader.SetGlobalFloat( "_EditorDeltaTime", (float)deltaTime );
		}

		public void UpdateNodePreviewList()
		{
			if( UIUtils.CurrentWindow != this )
				return;

			UIUtils.CheckNullMaterials();

			for( int i = 0; i < CurrentGraph.AllNodes.Count; i++ )
			{
				ParentNode node = CurrentGraph.AllNodes[ i ];
				if( node != null )
				{
					node.RenderNodePreview();
				}
			}

			Repaint();
		}

		public void ForceRepaint()
		{
			m_repaintCount += 1;
			m_repaintIsDirty = true;
			//Repaint();
		}

		public void ForceUpdateFromMaterial() { m_forceUpdateFromMaterialFlag = true; }
		void UseCurrentEvent()
		{
			m_currentEvent.Use();
		}



		public void OnBeforeSerialize()
		{
			//if ( !UIUtils.SerializeFromUndo() )
			//{
			//	m_mainGraphInstance.DeSelectAll();
			//}

			if( DebugConsoleWindow.UseShaderPanelsInfo )
			{
				if( m_nodeParametersWindow != null )
					m_nodeParametersWindowMaximized = m_nodeParametersWindow.IsMaximized;

				if( m_paletteWindow != null )
					m_paletteWindowMaximized = m_paletteWindow.IsMaximized;
			}
		}

		public void OnAfterDeserialize()
		{
			m_afterDeserializeFlag = true;

			//m_customGraph = null;
			if( DebugConsoleWindow.UseShaderPanelsInfo )
			{
				if( m_nodeParametersWindow != null )
					m_nodeParametersWindow.IsMaximized = m_nodeParametersWindowMaximized;

				if( m_paletteWindow != null )
					m_paletteWindow.IsMaximized = m_paletteWindowMaximized;
			}
		}

		void OnDestroy()
		{
			m_ctrlSCallback = false;
			Destroy();
		}

		public override void OnDisable()
		{
			base.OnDisable();
			m_ctrlSCallback = false;
			EditorApplication.update -= UpdateTime;
			EditorApplication.update -= UpdateNodePreviewList;

			EditorApplication.update -= IOUtils.UpdateIO;
			for( int i = 0; i < IOUtils.AllOpenedWindows.Count; i++ )
			{
				if( IOUtils.AllOpenedWindows[ i ] != this )
				{
					EditorApplication.update += IOUtils.UpdateIO;
					break;
				}
			}
		}

		void OnEmptyGraphDetected( ParentGraph graph )
		{
			if( m_delayedLoadObject != null )
			{
				LoadObject( m_delayedLoadObject );
				m_delayedLoadObject = null;
				Repaint();
			}
			else
			{
				if( !string.IsNullOrEmpty( Lastpath ) )
				{
					Shader shader = AssetDatabase.LoadAssetAtPath<Shader>( Lastpath );
					if( shader == null )
					{
						Material material = AssetDatabase.LoadAssetAtPath<Material>( Lastpath );
						if( material != null )
						{
							LoadDroppedObject( true, material.shader, material, null );
						}
						else
						{
							AmplifyShaderFunction function = AssetDatabase.LoadAssetAtPath<AmplifyShaderFunction>( Lastpath );
							if( function != null )
							{
								LoadDroppedObject( true, null, null, function );
							}
						}
					}
					else
					{
						LoadDroppedObject( true, shader, null, null );
					}
					Repaint();
				}
			}
		}


		public void ForceMaterialsToUpdate( ref Dictionary<string, string> availableMaterials )
		{
			m_forcingMaterialUpdateOp = true;
			m_forcingMaterialUpdateFlag = true;
			m_materialsToUpdate.Clear();
			foreach( KeyValuePair<string, string> kvp in availableMaterials )
			{
				Material material = AssetDatabase.LoadAssetAtPath<Material>( AssetDatabase.GUIDToAssetPath( kvp.Value ) );
				if( material != null )
				{
					m_materialsToUpdate.Add( material );
				}
			}
		}

		public void ReplaceMasterNode( MasterNodeCategoriesData data, bool cacheMasterNodes )
		{
			m_replaceMasterNodeType = data.Category;
			m_replaceMasterNode = true;
			m_replaceMasterNodeData = data.Name;
			m_replaceMasterNodeDataFromCache = cacheMasterNodes;
			if( cacheMasterNodes )
			{
				m_clipboard.AddMultiPassNodesToClipboard( m_mainGraphInstance.MultiPassMasterNodes.NodesList );
			}
		}

		public Vector2 TranformPosition( Vector2 pos )
		{
			return pos * m_cameraZoom - m_cameraOffset;
		}

		public void UpdateTabTitle()
		{
			if( m_isShaderFunctionWindow )
			{
				if( m_openedShaderFunction != null )
				{
					this.titleContent.text = GenerateTabTitle( m_openedShaderFunction.FunctionName );
				}
			}
			else
			{
				if( m_selectionMode == ASESelectionMode.Material )
				{
					this.titleContent.text = GenerateTabTitle( m_mainGraphInstance.CurrentMaterial.name );
				}
				else
				{
					this.titleContent.text = GenerateTabTitle( m_mainGraphInstance.CurrentShader.name );
				}
			}
		}

		public ParentGraph CustomGraph
		{
			get { return m_customGraph; }
			set { m_customGraph = value; }
		}

		public ParentGraph CurrentGraph
		{
			get
			{
				if( m_customGraph != null )
					return m_customGraph;

				return m_mainGraphInstance;
			}
		}

		public void RefreshAvaibleNodes()
		{
			if( m_contextMenu != null && m_mainGraphInstance != null )
			{
				m_contextMenu.RefreshNodes( m_mainGraphInstance );
				m_paletteWindow.ForceUpdate = true;
				m_contextPalette.ForceUpdate = true;
				m_refreshAvailableNodes = false;
			}
		}

		public void LateRefreshAvailableNodes()
		{
			m_refreshAvailableNodes = true;
		}

		public ParentGraph OutsideGraph { get { return m_mainGraphInstance; } }

		public bool ShaderIsModified
		{
			get { return m_shaderIsModified; }
			set
			{
				m_shaderIsModified = value && UIUtils.DirtyMask;

				m_toolsWindow.SetStateOnButton( ToolButtonType.Save, m_shaderIsModified ? 1 : 0 );
				if( !IsShaderFunctionWindow )
				{
					MasterNode masterNode = m_mainGraphInstance.CurrentMasterNode;
					if( masterNode != null && masterNode.CurrentShader != null )
					{
						m_toolsWindow.SetStateOnButton( ToolButtonType.Update, m_shaderIsModified ? 0 : 2 );
						UpdateTabTitle( masterNode.ShaderName, m_shaderIsModified );
					}
					else
					{
						m_toolsWindow.SetStateOnButton( ToolButtonType.Update, 1 );
					}

					//if( m_mainGraphInstance.CurrentStandardSurface != null )
					//	UpdateTabTitle( m_mainGraphInstance.CurrentStandardSurface.ShaderName, m_shaderIsModified );
				}
				else
				{
					m_toolsWindow.SetStateOnButton( ToolButtonType.Update, m_shaderIsModified ? 0 : 2 );
					if( m_mainGraphInstance.CurrentShaderFunction != null )
						UpdateTabTitle( m_mainGraphInstance.CurrentShaderFunction.FunctionName, m_shaderIsModified );
				}

			}
		}
		public void MarkToRepaint() { m_repaintIsDirty = true; }
		public void RequestSave() { m_markedToSave = true; }
		public void RequestRepaint() { m_repaintIsDirty = true; }
		public OptionsWindow Options { get { return m_optionsWindow; } }
		public GraphContextMenu ContextMenuInstance { get { return m_contextMenu; } set { m_contextMenu = value; } }
		public ShortcutsManager ShortcutManagerInstance { get { return m_shortcutManager; } }

		public bool GlobalPreview
		{
			get { return m_globalPreview; }
			set { m_globalPreview = value; }
		}

		public bool GlobalShowInternalData
		{
			get { return m_globalShowInternalData; }
			set { m_globalShowInternalData = value; }
		}

		public double EditorTime
		{
			get { return m_time; }
			set { m_time = value; }
		}

		public ASESelectionMode CurrentSelection
		{
			get { return m_selectionMode; }
			set
			{
				m_selectionMode = value;
				switch( m_selectionMode )
				{
					default:
					case ASESelectionMode.Shader:
					{
						m_toolsWindow.BorderStyle = UIUtils.GetCustomStyle( CustomStyle.ShaderBorder );
					}
					break;
					case ASESelectionMode.Material:
					{
						m_toolsWindow.BorderStyle = UIUtils.GetCustomStyle( CustomStyle.MaterialBorder );
					}
					break;
					case ASESelectionMode.ShaderFunction:
					{
						m_toolsWindow.BorderStyle = UIUtils.GetCustomStyle( CustomStyle.ShaderFunctionBorder );
					}
					break;
				}
			}
		}

		public bool LiveShaderEditing
		{
			get { return m_liveShaderEditing; }
			set
			{
				m_liveShaderEditing = value;
				m_innerEditorVariables.LiveMode = m_liveShaderEditing;
				UpdateLiveUI();
			}
		}

		public NodeAvailability CurrentNodeAvailability
		{
			get { return m_currentNodeAvailability; }
			set
			{
				NodeAvailability cache = m_currentNodeAvailability;
				m_currentNodeAvailability = value;

				if( cache != value )
					RefreshAvaibleNodes();
			}
		}
		public void InvalidateAlt() { m_altAvailable = false; }
		public PaletteWindow CurrentPaletteWindow { get { return m_paletteWindow; } }
		public PreMadeShaders PreMadeShadersInstance { get { return m_preMadeShaders; } }
		public Rect CameraInfo { get { return m_cameraInfo; } }
		public Vector2 TranformedMousePos { get { return m_currentMousePos2D * m_cameraZoom - m_cameraOffset; } }
		public Vector2 TranformedKeyEvtMousePos { get { return m_keyEvtMousePos2D * m_cameraZoom - m_cameraOffset; } }
		public PalettePopUp PalettePopUpInstance { get { return m_palettePopup; } }
		public DuplicatePreventionBuffer DuplicatePrevBufferInstance { get { return m_duplicatePreventionBuffer; } }
		public NodeParametersWindow ParametersWindow { get { return m_nodeParametersWindow; } }
		public NodeExporterUtils CurrentNodeExporterUtils { get { return m_nodeExporterUtils; } }
		public AmplifyShaderFunction OpenedShaderFunction { get { return m_openedShaderFunction; }  }
		public DrawInfo CameraDrawInfo { get { return m_drawInfo; } }
		public string Lastpath { get { return m_lastpath; } set { m_lastpath = value; } }
		public string LastOpenedLocation { get { return m_lastOpenedLocation; } set { m_lastOpenedLocation = value; } }
		public float AvailableCanvasWidth { get { return ( m_cameraInfo.width - m_paletteWindow.RealWidth - m_nodeParametersWindow.RealWidth ); } }
		public float AvailableCanvasHeight { get { return ( m_cameraInfo.height ); } }
		public float CameraZoom { get { return m_cameraZoom; } set { m_cameraZoom = value; m_zoomChanged = true; } }
		public int GraphCount { get { return m_graphCount; } set { m_graphCount = value; } }
		public bool ForceAutoPanDir { get { return m_forceAutoPanDir; } set { m_forceAutoPanDir = value; } }
		public bool OpenedAssetFromNode { get { return m_openedAssetFromNode; } set { m_openedAssetFromNode = value; } }
		public bool IsShaderFunctionWindow { get { return m_isShaderFunctionWindow; } set { m_isShaderFunctionWindow = value; } }
		public bool NodesLoadedCorrectly { get { return m_nodesLoadedCorrectly; } set { m_nodesLoadedCorrectly = value; } }
		public double CurrentInactiveTime { get { return m_currentInactiveTime; } }
		public string ReplaceMasterNodeData { get { return m_replaceMasterNodeData; } }
		public AvailableShaderTypes ReplaceMasterNodeType { get { return m_replaceMasterNodeType; } }
		public NodeWireReferencesUtils WireReferenceUtils { get { return m_wireReferenceUtils; } }
		public ContextPalette WindowContextPallete { get { return m_contextPalette; } }
		// This needs to go to UIUtils
		public Texture2D WireTexture { get { return m_wireTexture; } }
		public Event CurrentEvent { get { return m_currentEvent; } }
		public string CurrentCommandName { get { return m_currentCommandName; } }
		public InnerWindowEditorVariables InnerWindowVariables { get { return m_innerEditorVariables; } }
		public TemplatesManager TemplatesManagerInstance { get { return m_templatesManager; } }
		public Material CurrentMaterial { get { return CurrentGraph.CurrentMaterial; } }
	}
}
