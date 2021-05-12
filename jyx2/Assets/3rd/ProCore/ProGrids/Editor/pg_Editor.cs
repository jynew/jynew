/**
 *	7/8/2013
 */
#define PRO

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

namespace ProGrids
{
	[InitializeOnLoad]
	public static class pg_Initializer
	{
		/**
		 * When opening Unity, remember whether or not ProGrids was open when Unity was shut down last.
		 */
		static pg_Initializer()
		{
			if (EditorPrefs.GetBool(pg_Constant.ProGridsIsEnabled))
			{
				if (pg_Editor.instance == null)
					pg_Editor.InitProGrids();
				else
					EditorApplication.delayCall += pg_Editor.instance.Initialize;
			}
		}
	}

	public class pg_Editor : ScriptableObject, ISerializationCallbackReceiver
	{

		#region MEMBERS

		public static pg_Editor instance
		{
			get
			{
				if (_instance == null)
				{
					pg_Editor[] editor = Resources.FindObjectsOfTypeAll<pg_Editor>();

					if (editor != null && editor.Length > 0)
					{
						_instance = editor[0];

						for (int i = 1; i < editor.Length; i++)
						{
							GameObject.DestroyImmediate(editor[i]);
						}
					}
				}

				return _instance;
			}

			set
			{
				_instance = value;
			}
		}
		private static pg_Editor _instance;

		Color oldColor;

		private bool useAxisConstraints
		{
			get { return EditorPrefs.GetBool(pg_Constant.UseAxisConstraints); }
			set { EditorPrefs.SetBool(pg_Constant.UseAxisConstraints, value); }
		}

		[SerializeField]
		private bool snapEnabled = true;
		[SerializeField]
		private SnapUnit snapUnit = SnapUnit.Meter;
#if PRO
		private float snapValue = 1f;                       // the actual snap value, taking into account unit size
		private float t_snapValue = 1f;                     // what the user sees
#else
	private float snapValue = .25f;
	private float t_snapValue = .25f;
#endif
		private bool drawGrid = true;
		private bool drawAngles = false;
		public float angleValue = 45f;
		private bool gridRepaint = true;
		public bool predictiveGrid = true;

		private bool _snapAsGroup = true;
		public bool snapAsGroup
		{
			get
			{
				return EditorPrefs.HasKey(pg_Constant.SnapAsGroup) ? EditorPrefs.GetBool(pg_Constant.SnapAsGroup) : true;
			}

			set
			{
				_snapAsGroup = value;
				EditorPrefs.SetBool(pg_Constant.SnapAsGroup, _snapAsGroup);
			}
		}

		public bool fullGrid { get; private set; }

		private bool _scaleSnapEnabled = false;
		public bool ScaleSnapEnabled
		{
			get
			{
				return EditorPrefs.HasKey(pg_Constant.SnapScale) ? EditorPrefs.GetBool(pg_Constant.SnapScale) : false;
			}

			set
			{
				_scaleSnapEnabled = value;
				EditorPrefs.SetBool(pg_Constant.SnapScale, _scaleSnapEnabled);
			}
		}

		private KeyCode m_IncreaseGridSizeShortcut = KeyCode.Equals;
		private KeyCode m_DecreaseGridSizeShortcut = KeyCode.Minus;
		private KeyCode m_NudgePerspectiveBackwardShortcut = KeyCode.LeftBracket;
		private KeyCode m_NudgePerspectiveForwardShortcut = KeyCode.RightBracket;
		private KeyCode m_NudgePerspectiveResetShortcut = KeyCode.Alpha0;
		private KeyCode m_CyclePerspectiveShortcut = KeyCode.Backslash;

		bool lockGrid = false;
		private Axis renderPlane = Axis.Y;

#if PG_DEBUG
	private GameObject _pivotGo;
	public GameObject pivotGo
	{
		get
		{
			if(_pivotGo == null)
			{
				GameObject find = GameObject.Find("PG_PIVOT_CUBE");

				if(find == null)
				{
					_pivotGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
					_pivotGo.name = "PG_PIVOT_CUBE";
				}
				else
					_pivotGo = find;
			}

			return _pivotGo;
		}

		set
		{
			_pivotGo = value;
		}
	}
#endif
		#endregion

		#region CONSTANT

		const int VERSION = 22;

#if PRO
		const int WINDOW_HEIGHT = 240;
#else
	const int WINDOW_HEIGHT = 260;
#endif

		const int DEFAULT_SNAP_MULTIPLIER = 2048;

		const int MAX_LINES = 150;              // the maximum amount of lines to display on screen in either direction
		public static float alphaBump;          // Every tenth line gets an alpha bump by this amount
		const int BUTTON_SIZE = 46;

		private Texture2D icon_extendoClose, icon_extendoOpen;

		[SerializeField]
		private pg_ToggleContent gc_SnapToGrid = new pg_ToggleContent("Snap", "", "Snaps all selected objects to grid.");
		[SerializeField]
		private pg_ToggleContent gc_GridEnabled = new pg_ToggleContent("Hide", "Show", "Toggles drawing of guide lines on or off.  Note that object snapping is not affected by this setting.");
		[SerializeField]
		private pg_ToggleContent gc_SnapEnabled = new pg_ToggleContent("On", "Off", "Toggles snapping on or off.");
		[SerializeField]
		private pg_ToggleContent gc_LockGrid = new pg_ToggleContent("Lock", "Unlck", "Lock the perspective grid center in place.");
		[SerializeField]
		private pg_ToggleContent gc_AngleEnabled = new pg_ToggleContent("> On", "> Off", "If on, ProGrids will draw angled line guides.  Angle is settable in degrees.");
		[SerializeField]
		private pg_ToggleContent gc_RenderPlaneX = new pg_ToggleContent("X", "X", "Renders a grid on the X plane.");
		[SerializeField]
		private pg_ToggleContent gc_RenderPlaneY = new pg_ToggleContent("Y", "Y", "Renders a grid on the Y plane.");
		[SerializeField]
		private pg_ToggleContent gc_RenderPlaneZ = new pg_ToggleContent("Z", "Z", "Renders a grid on the Z plane.");
		[SerializeField]
		private pg_ToggleContent gc_RenderPerspectiveGrid = new pg_ToggleContent("Full", "Plane", "Renders a 3d grid in perspective mode.");
		[SerializeField]
		private GUIContent gc_ExtendMenu = new GUIContent("", "Show or hide the scene view menu.");
		[SerializeField]
		private GUIContent gc_SnapIncrement = new GUIContent("", "Set the snap increment.");
		#endregion

		#region PREFERENCES

		/** Settings **/
		public Color gridColorX, gridColorY, gridColorZ;
		public Color gridColorX_primary, gridColorY_primary, gridColorZ_primary;

		// private bool lockOrthographic;

		public void LoadPreferences()
		{
			if ((EditorPrefs.HasKey(pg_Constant.PGVersion) ? EditorPrefs.GetInt(pg_Constant.PGVersion) : 0) != VERSION)
			{
				EditorPrefs.SetInt(pg_Constant.PGVersion, VERSION);
				pg_Preferences.ResetPrefs();
			}

			if (EditorPrefs.HasKey(pg_Constant.SnapEnabled))
			{
				snapEnabled = EditorPrefs.GetBool(pg_Constant.SnapEnabled);
			}

			menuOpen = EditorPrefs.GetBool(pg_Constant.ProGridsIsExtended, true);

			SetSnapValue(
				EditorPrefs.HasKey(pg_Constant.GridUnit) ? (SnapUnit)EditorPrefs.GetInt(pg_Constant.GridUnit) : SnapUnit.Meter,
				EditorPrefs.HasKey(pg_Constant.SnapValue) ? EditorPrefs.GetFloat(pg_Constant.SnapValue) : 1,
				EditorPrefs.HasKey(pg_Constant.SnapMultiplier) ? EditorPrefs.GetInt(pg_Constant.SnapMultiplier) : DEFAULT_SNAP_MULTIPLIER
				);

			m_IncreaseGridSizeShortcut = EditorPrefs.HasKey("pg_Editor::IncreaseGridSize")
				? (KeyCode)EditorPrefs.GetInt("pg_Editor::IncreaseGridSize")
				: KeyCode.Equals;
			m_DecreaseGridSizeShortcut = EditorPrefs.HasKey("pg_Editor::DecreaseGridSize")
				? (KeyCode)EditorPrefs.GetInt("pg_Editor::DecreaseGridSize")
				: KeyCode.Minus;
			m_NudgePerspectiveBackwardShortcut = EditorPrefs.HasKey("pg_Editor::NudgePerspectiveBackward")
				? (KeyCode)EditorPrefs.GetInt("pg_Editor::NudgePerspectiveBackward")
				: KeyCode.LeftBracket;
			m_NudgePerspectiveForwardShortcut = EditorPrefs.HasKey("pg_Editor::NudgePerspectiveForward")
				? (KeyCode)EditorPrefs.GetInt("pg_Editor::NudgePerspectiveForward")
				: KeyCode.RightBracket;
			m_NudgePerspectiveResetShortcut = EditorPrefs.HasKey("pg_Editor::NudgePerspectiveReset")
				? (KeyCode)EditorPrefs.GetInt("pg_Editor::NudgePerspectiveReset")
				: KeyCode.Alpha0;
			m_CyclePerspectiveShortcut = EditorPrefs.HasKey("pg_Editor::CyclePerspective")
				? (KeyCode)EditorPrefs.GetInt("pg_Editor::CyclePerspective")
				: KeyCode.Backslash;

			lockGrid = EditorPrefs.GetBool(pg_Constant.LockGrid);

			if (lockGrid)
			{
				if (EditorPrefs.HasKey(pg_Constant.LockedGridPivot))
				{
					string piv = EditorPrefs.GetString(pg_Constant.LockedGridPivot);
					string[] pivsplit = piv.Replace("(", "").Replace(")", "").Split(',');

					float x, y, z;
					if (!float.TryParse(pivsplit[0], out x)) goto NoParseForYou;
					if (!float.TryParse(pivsplit[1], out y)) goto NoParseForYou;
					if (!float.TryParse(pivsplit[2], out z)) goto NoParseForYou;

					pivot.x = x;
					pivot.y = y;
					pivot.z = z;

				NoParseForYou:
					;   // appease the compiler
				}

			}

			fullGrid = EditorPrefs.GetBool(pg_Constant.PerspGrid);

			renderPlane = EditorPrefs.HasKey(pg_Constant.GridAxis) ? (Axis)EditorPrefs.GetInt(pg_Constant.GridAxis) : Axis.Y;

			alphaBump = (EditorPrefs.HasKey("pg_alphaBump")) ? EditorPrefs.GetFloat("pg_alphaBump") : pg_Preferences.ALPHA_BUMP;

			gridColorX = (EditorPrefs.HasKey("gridColorX")) ? pg_Util.ColorWithString(EditorPrefs.GetString("gridColorX")) : pg_Preferences.GRID_COLOR_X;
			gridColorX_primary = new Color(gridColorX.r, gridColorX.g, gridColorX.b, gridColorX.a + alphaBump);
			gridColorY = (EditorPrefs.HasKey("gridColorY")) ? pg_Util.ColorWithString(EditorPrefs.GetString("gridColorY")) : pg_Preferences.GRID_COLOR_Y;
			gridColorY_primary = new Color(gridColorY.r, gridColorY.g, gridColorY.b, gridColorY.a + alphaBump);
			gridColorZ = (EditorPrefs.HasKey("gridColorZ")) ? pg_Util.ColorWithString(EditorPrefs.GetString("gridColorZ")) : pg_Preferences.GRID_COLOR_Z;
			gridColorZ_primary = new Color(gridColorZ.r, gridColorZ.g, gridColorZ.b, gridColorZ.a + alphaBump);

			drawGrid = (EditorPrefs.HasKey("showgrid")) ? EditorPrefs.GetBool("showgrid") : pg_Preferences.SHOW_GRID;

			predictiveGrid = EditorPrefs.HasKey(pg_Constant.PredictiveGrid) ? EditorPrefs.GetBool(pg_Constant.PredictiveGrid) : true;

			_snapAsGroup = snapAsGroup;
			_scaleSnapEnabled = ScaleSnapEnabled;
		}

		private GUISkin sixBySevenSkin;
		#endregion

		#region MENU

		[MenuItem("Tools/ProGrids/About", false, 0)]
		public static void MenuAboutProGrids()
		{
			pg_AboutWindow.Init("Assets/ProCore/ProGrids/About/pc_AboutEntry_ProGrids.txt", true);
		}

		[MenuItem("Tools/ProGrids/ProGrids Window", false, 15)]
		public static void InitProGrids()
		{
			if (instance == null)
			{
				EditorPrefs.SetBool(pg_Constant.ProGridsIsEnabled, true);
				instance = ScriptableObject.CreateInstance<pg_Editor>();
				instance.hideFlags = HideFlags.DontSave;
				EditorApplication.delayCall += instance.Initialize;
			}
			else
			{
				CloseProGrids();
			}

			SceneView.RepaintAll();
		}

		[MenuItem("Tools/ProGrids/Close ProGrids", true, 200)]
		public static bool VerifyCloseProGrids()
		{
			return instance != null || Resources.FindObjectsOfTypeAll<pg_Editor>().Length > 0;
		}

		[MenuItem("Tools/ProGrids/Close ProGrids")]
		public static void CloseProGrids()
		{
			foreach (pg_Editor editor in Resources.FindObjectsOfTypeAll<pg_Editor>())
				editor.Close();
		}

		[MenuItem("Tools/ProGrids/Cycle SceneView Projection", false, 101)]
		public static void CyclePerspective()
		{
			if (instance == null) return;

			SceneView scnvw = SceneView.lastActiveSceneView;
			if (scnvw == null) return;

			int nextOrtho = EditorPrefs.GetInt(pg_Constant.LastOrthoToggledRotation);
			switch (nextOrtho)
			{
				case 0:
					scnvw.orthographic = true;
					scnvw.LookAt(scnvw.pivot, Quaternion.Euler(Vector3.zero));
					nextOrtho++;
					break;

				case 1:
					scnvw.orthographic = true;
					scnvw.LookAt(scnvw.pivot, Quaternion.Euler(Vector3.up * -90f));
					nextOrtho++;
					break;

				case 2:
					scnvw.orthographic = true;
					scnvw.LookAt(scnvw.pivot, Quaternion.Euler(Vector3.right * 90f));
					nextOrtho++;
					break;

				case 3:
					scnvw.orthographic = false;
					scnvw.LookAt(scnvw.pivot, new Quaternion(-0.1f, 0.9f, -0.2f, -0.4f));
					nextOrtho = 0;
					break;
			}
			EditorPrefs.SetInt(pg_Constant.LastOrthoToggledRotation, nextOrtho);
		}

		[MenuItem("Tools/ProGrids/Cycle SceneView Projection", true, 101)]
		[MenuItem("Tools/ProGrids/Increase Grid Size", true, 203)]
		[MenuItem("Tools/ProGrids/Decrease Grid Size", true, 202)]
		public static bool VerifyGridSizeAdjustment()
		{
			return instance != null;
		}

		[MenuItem("Tools/ProGrids/Decrease Grid Size", false, 202)]
		public static void DecreaseGridSize()
		{
			if (instance == null) return;

			int multiplier = EditorPrefs.HasKey(pg_Constant.SnapMultiplier) ? EditorPrefs.GetInt(pg_Constant.SnapMultiplier) : DEFAULT_SNAP_MULTIPLIER;
			float val = EditorPrefs.HasKey(pg_Constant.SnapValue) ? EditorPrefs.GetFloat(pg_Constant.SnapValue) : 1f;

			if (multiplier > 1)
				multiplier /= 2;

			instance.SetSnapValue(instance.snapUnit, val, multiplier);

			SceneView.RepaintAll();
		}

		[MenuItem("Tools/ProGrids/Increase Grid Size", false, 203)]
		public static void IncreaseGridSize()
		{
			if (instance == null) return;

			int multiplier = EditorPrefs.HasKey(pg_Constant.SnapMultiplier) ? EditorPrefs.GetInt(pg_Constant.SnapMultiplier) : DEFAULT_SNAP_MULTIPLIER;
			float val = EditorPrefs.HasKey(pg_Constant.SnapValue) ? EditorPrefs.GetFloat(pg_Constant.SnapValue) : 1f;

			if (multiplier < int.MaxValue / 2)
				multiplier *= 2;

			instance.SetSnapValue(instance.snapUnit, val, multiplier);

			SceneView.RepaintAll();
		}

		[MenuItem("Tools/ProGrids/Nudge Perspective Backward", true, 304)]
		[MenuItem("Tools/ProGrids/Nudge Perspective Forward", true, 305)]
		[MenuItem("Tools/ProGrids/Reset Perspective Nudge", true, 306)]
		public static bool VerifyMenuNudgePerspective()
		{
			return instance != null && !instance.fullGrid && !instance.ortho && instance.lockGrid;
		}

		[MenuItem("Tools/ProGrids/Nudge Perspective Backward", false, 304)]
		public static void MenuNudgePerspectiveBackward()
		{
			if (!instance.lockGrid) return;
			instance.offset -= instance.snapValue;
			instance.gridRepaint = true;
			SceneView.RepaintAll();
		}

		[MenuItem("Tools/ProGrids/Nudge Perspective Forward", false, 305)]
		public static void MenuNudgePerspectiveForward()
		{
			if (!instance.lockGrid) return;
			instance.offset += instance.snapValue;
			instance.gridRepaint = true;
			SceneView.RepaintAll();
		}

		[MenuItem("Tools/ProGrids/Reset Perspective Nudge", false, 306)]
		public static void MenuNudgePerspectiveReset()
		{
			if (!instance.lockGrid) return;
			instance.offset = 0;
			instance.gridRepaint = true;
			SceneView.RepaintAll();
		}

		public static void ForceRepaint()
		{
			if (instance != null)
			{
				instance.gridRepaint = true;
				SceneView.RepaintAll();
			}
		}
		#endregion

		#region INITIALIZATION / SERIALIZATION

		public void OnBeforeSerialize() { }

		public void OnAfterDeserialize()
		{
			instance = this;
			SceneView.onSceneGUIDelegate += OnSceneGUI;
			EditorApplication.update += Update;
			EditorApplication.hierarchyWindowChanged += HierarchyWindowChanged;
		}

		void OnEnable()
		{
			instance.LoadGUIResources();
#if !UNITY_4_6 && !UNITY_4_7 && !UNITY_5_0 && !UNITY_5_1
			Selection.selectionChanged += OnSelectionChange;
#endif
		}

		public void Initialize()
		{
			SceneView.onSceneGUIDelegate -= OnSceneGUI;
			EditorApplication.update -= Update;
			EditorApplication.hierarchyWindowChanged -= HierarchyWindowChanged;

			SceneView.onSceneGUIDelegate += OnSceneGUI;
			EditorApplication.update += Update;
			EditorApplication.hierarchyWindowChanged += HierarchyWindowChanged;

			LoadGUIResources();
			LoadPreferences();
			instance = this;
			pg_GridRenderer.Init();

			SetMenuIsExtended(menuOpen);

			lastTime = Time.realtimeSinceStartup;

			// reset colors without changing anything
			menuOpen = !menuOpen;
			ToggleMenuVisibility();

			if (drawGrid)
				pg_Util.SetUnityGridEnabled(false);

			gridRepaint = true;
			RepaintSceneView();
		}

		void OnDestroy()
		{
			this.Close(true);
		}

		public void Close()
		{
			EditorPrefs.SetBool(pg_Constant.ProGridsIsEnabled, false);
			GameObject.DestroyImmediate(this);
#if !UNITY_4_6 && !UNITY_4_7 && !UNITY_5_0 && !UNITY_5_1
			Selection.selectionChanged -= OnSelectionChange;
#endif
		}

		public void Close(bool isBeingDestroyed)
		{
			pg_GridRenderer.Destroy();

			SceneView.onSceneGUIDelegate -= OnSceneGUI;
			EditorApplication.update -= Update;
			EditorApplication.hierarchyWindowChanged -= HierarchyWindowChanged;

			instance = null;

			foreach (System.Action<bool> listener in toolbarEventSubscribers)
				listener(false);

			pg_Util.SetUnityGridEnabled(true);

			SceneView.RepaintAll();
		}

		private void LoadGUIResources()
		{
			if (gc_GridEnabled.image_on == null)
				gc_GridEnabled.image_on = pg_IconUtility.LoadIcon("ProGrids2_GUI_Vis_On.png");

			if (gc_GridEnabled.image_off == null)
				gc_GridEnabled.image_off = pg_IconUtility.LoadIcon("ProGrids2_GUI_Vis_Off.png");

			if (gc_SnapEnabled.image_on == null)
				gc_SnapEnabled.image_on = pg_IconUtility.LoadIcon("ProGrids2_GUI_Snap_On.png");

			if (gc_SnapEnabled.image_off == null)
				gc_SnapEnabled.image_off = pg_IconUtility.LoadIcon("ProGrids2_GUI_Snap_Off.png");

			if (gc_SnapToGrid.image_on == null)
				gc_SnapToGrid.image_on = pg_IconUtility.LoadIcon("ProGrids2_GUI_PushToGrid_Normal.png");

			if (gc_LockGrid.image_on == null)
				gc_LockGrid.image_on = pg_IconUtility.LoadIcon("ProGrids2_GUI_PGrid_Lock_On.png");

			if (gc_LockGrid.image_off == null)
				gc_LockGrid.image_off = pg_IconUtility.LoadIcon("ProGrids2_GUI_PGrid_Lock_Off.png");

			if (gc_AngleEnabled.image_on == null)
				gc_AngleEnabled.image_on = pg_IconUtility.LoadIcon("ProGrids2_GUI_AngleVis_On.png");

			if (gc_AngleEnabled.image_off == null)
				gc_AngleEnabled.image_off = pg_IconUtility.LoadIcon("ProGrids2_GUI_AngleVis_Off.png");

			if (gc_RenderPlaneX.image_on == null)
				gc_RenderPlaneX.image_on = pg_IconUtility.LoadIcon("ProGrids2_GUI_PGrid_X_On.png");

			if (gc_RenderPlaneX.image_off == null)
				gc_RenderPlaneX.image_off = pg_IconUtility.LoadIcon("ProGrids2_GUI_PGrid_X_Off.png");

			if (gc_RenderPlaneY.image_on == null)
				gc_RenderPlaneY.image_on = pg_IconUtility.LoadIcon("ProGrids2_GUI_PGrid_Y_On.png");

			if (gc_RenderPlaneY.image_off == null)
				gc_RenderPlaneY.image_off = pg_IconUtility.LoadIcon("ProGrids2_GUI_PGrid_Y_Off.png");

			if (gc_RenderPlaneZ.image_on == null)
				gc_RenderPlaneZ.image_on = pg_IconUtility.LoadIcon("ProGrids2_GUI_PGrid_Z_On.png");

			if (gc_RenderPlaneZ.image_off == null)
				gc_RenderPlaneZ.image_off = pg_IconUtility.LoadIcon("ProGrids2_GUI_PGrid_Z_Off.png");

			if (gc_RenderPerspectiveGrid.image_on == null)
				gc_RenderPerspectiveGrid.image_on = pg_IconUtility.LoadIcon("ProGrids2_GUI_PGrid_3D_On.png");

			if (gc_RenderPerspectiveGrid.image_off == null)
				gc_RenderPerspectiveGrid.image_off = pg_IconUtility.LoadIcon("ProGrids2_GUI_PGrid_3D_Off.png");

			if (icon_extendoOpen == null)
				icon_extendoOpen = pg_IconUtility.LoadIcon("ProGrids2_MenuExtendo_Open.png");

			if (icon_extendoClose == null)
				icon_extendoClose = pg_IconUtility.LoadIcon("ProGrids2_MenuExtendo_Close.png");
		}

		#endregion

		#region INTERFACE

		GUIStyle gridButtonStyle = new GUIStyle();
		GUIStyle extendoStyle = new GUIStyle();
		GUIStyle gridButtonStyleBlank = new GUIStyle();
		GUIStyle backgroundStyle = new GUIStyle();
		bool guiInitialized = false;

		public float GetSnapIncrement()
		{
			return t_snapValue;
		}

		public void SetSnapIncrement(float inc)
		{
			SetSnapValue(snapUnit, Mathf.Max(inc, .001f), DEFAULT_SNAP_MULTIPLIER);
		}

		void RepaintSceneView()
		{
			SceneView.RepaintAll();
		}

		int MENU_HIDDEN { get { return menuIsOrtho ? -192 : -173; } }

		const int MENU_EXTENDED = 8;
		const int PAD = 3;
		Rect r = new Rect(8, MENU_EXTENDED, 42, 16);
		Rect backgroundRect = new Rect(00, 0, 0, 0);
		Rect extendoButtonRect = new Rect(0, 0, 0, 0);
		bool menuOpen = true;
		float menuStart = MENU_EXTENDED;
		const float MENU_SPEED = 500f;
		float deltaTime = 0f;
		float lastTime = 0f;
		const float FADE_SPEED = 2.5f;
		float backgroundFade = 1f;
		bool mouseOverMenu = false;
		Color menuBackgroundColor = new Color(0f, 0f, 0f, .5f);
		Color extendoNormalColor = new Color(.9f, .9f, .9f, .7f);
		Color extendoHoverColor = new Color(0f, 1f, .4f, 1f);
		bool extendoButtonHovering = false;
		bool menuIsOrtho = false;

		void Update()
		{
			deltaTime = Time.realtimeSinceStartup - lastTime;
			lastTime = Time.realtimeSinceStartup;

			if ((menuOpen && menuStart < MENU_EXTENDED) || (!menuOpen && menuStart > MENU_HIDDEN))
			{
				menuStart += deltaTime * MENU_SPEED * (menuOpen ? 1f : -1f);
				menuStart = Mathf.Clamp(menuStart, MENU_HIDDEN, MENU_EXTENDED);
				RepaintSceneView();
			}

			float a = menuBackgroundColor.a;
			backgroundFade = (mouseOverMenu || !menuOpen) ? FADE_SPEED : -FADE_SPEED;

			menuBackgroundColor.a = Mathf.Clamp(menuBackgroundColor.a + backgroundFade * deltaTime, 0f, .5f);
			extendoNormalColor.a = menuBackgroundColor.a;
			extendoHoverColor.a = (menuBackgroundColor.a / .5f);

			if (!Mathf.Approximately(menuBackgroundColor.a, a))
				RepaintSceneView();
		}

		void DrawSceneGUI()
		{
			GUI.backgroundColor = menuBackgroundColor;
			backgroundRect.x = r.x - 4;
			backgroundRect.y = 0;
			backgroundRect.width = r.width + 8;
			backgroundRect.height = r.y + r.height + PAD;
			GUI.Box(backgroundRect, "", backgroundStyle);

			// when hit testing mouse for showing the background, add some leeway
			backgroundRect.width += 32f;
			backgroundRect.height += 32f;
			GUI.backgroundColor = Color.white;

			if (!guiInitialized)
			{
				extendoStyle.normal.background = menuOpen ? icon_extendoClose : icon_extendoOpen;
				extendoStyle.hover.background = menuOpen ? icon_extendoClose : icon_extendoOpen;

				guiInitialized = true;
				backgroundStyle.normal.background = EditorGUIUtility.whiteTexture;

				Texture2D icon_button_normal = pg_IconUtility.LoadIcon("ProGrids2_Button_Normal.png");
				Texture2D icon_button_hover = pg_IconUtility.LoadIcon("ProGrids2_Button_Hover.png");

				if (icon_button_normal == null)
				{
					gridButtonStyleBlank = new GUIStyle("button");
				}
				else
				{
					gridButtonStyleBlank.normal.background = icon_button_normal;
					gridButtonStyleBlank.hover.background = icon_button_hover;
					gridButtonStyleBlank.normal.textColor = icon_button_normal != null ? Color.white : Color.black;
					gridButtonStyleBlank.hover.textColor = new Color(.7f, .7f, .7f, 1f);
				}

				gridButtonStyleBlank.padding = new RectOffset(1, 2, 1, 2);
				gridButtonStyleBlank.alignment = TextAnchor.MiddleCenter;
			}

			r.y = menuStart;

			gc_SnapIncrement.text = t_snapValue.ToString("#.####");

			if (GUI.Button(r, gc_SnapIncrement, gridButtonStyleBlank))
			{
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		// On Mac ShowAsDropdown and ShowAuxWindow both throw stack pop exceptions when initialized.
		pg_ParameterWindow options = EditorWindow.GetWindow<pg_ParameterWindow>(true, "ProGrids Settings", true);
		Rect screenRect = SceneView.lastActiveSceneView.position;
		options.editor = this;
		options.position = new Rect(screenRect.x + r.x + r.width + PAD,
										screenRect.y + r.y + 24,
										256,
										174);
#else
				pg_ParameterWindow options = ScriptableObject.CreateInstance<pg_ParameterWindow>();
				Rect screenRect = SceneView.lastActiveSceneView.position;
				options.editor = this;
				options.ShowAsDropDown(new Rect(screenRect.x + r.x + r.width + PAD,
												screenRect.y + r.y + 24,
												0,
												0),
												new Vector2(256, 174));
#endif
			}

			r.y += r.height + PAD;

			// Draw grid
			if (pg_ToggleContent.ToggleButton(r, gc_GridEnabled, drawGrid, gridButtonStyle, EditorStyles.miniButton))
				SetGridEnabled(!drawGrid);

			r.y += r.height + PAD;

			// Snap enabled
			if (pg_ToggleContent.ToggleButton(r, gc_SnapEnabled, snapEnabled, gridButtonStyle, EditorStyles.miniButton))
				SetSnapEnabled(!snapEnabled);

			r.y += r.height + PAD;

			// Push to grid
			if (pg_ToggleContent.ToggleButton(r, gc_SnapToGrid, true, gridButtonStyle, EditorStyles.miniButton))
				SnapToGrid(Selection.transforms);

			r.y += r.height + PAD;

			// Lock grid
			if (pg_ToggleContent.ToggleButton(r, gc_LockGrid, lockGrid, gridButtonStyle, EditorStyles.miniButton))
			{
				lockGrid = !lockGrid;
				EditorPrefs.SetBool(pg_Constant.LockGrid, lockGrid);
				EditorPrefs.SetString(pg_Constant.LockedGridPivot, pivot.ToString());

				// if we've modified the nudge value, reset the pivot here
				if (!lockGrid)
					offset = 0f;

				gridRepaint = true;

				RepaintSceneView();
			}

			if (menuIsOrtho)
			{
				r.y += r.height + PAD;

				if (pg_ToggleContent.ToggleButton(r, gc_AngleEnabled, drawAngles, gridButtonStyle, EditorStyles.miniButton))
					SetDrawAngles(!drawAngles);
			}

			/**
			 * Perspective Toggles
			 */
			r.y += r.height + PAD + 4;

			if (pg_ToggleContent.ToggleButton(r, gc_RenderPlaneX, (renderPlane & Axis.X) == Axis.X && !fullGrid, gridButtonStyle, EditorStyles.miniButton))
				SetRenderPlane(Axis.X);

			r.y += r.height + PAD;

			if (pg_ToggleContent.ToggleButton(r, gc_RenderPlaneY, (renderPlane & Axis.Y) == Axis.Y && !fullGrid, gridButtonStyle, EditorStyles.miniButton))
				SetRenderPlane(Axis.Y);

			r.y += r.height + PAD;

			if (pg_ToggleContent.ToggleButton(r, gc_RenderPlaneZ, (renderPlane & Axis.Z) == Axis.Z && !fullGrid, gridButtonStyle, EditorStyles.miniButton))
				SetRenderPlane(Axis.Z);

			r.y += r.height + PAD;

			if (pg_ToggleContent.ToggleButton(r, gc_RenderPerspectiveGrid, fullGrid, gridButtonStyle, EditorStyles.miniButton))
			{
				fullGrid = !fullGrid;
				gridRepaint = true;
				EditorPrefs.SetBool(pg_Constant.PerspGrid, fullGrid);
				RepaintSceneView();
			}

			r.y += r.height + PAD;

			extendoButtonRect.x = r.x;
			extendoButtonRect.y = r.y;
			extendoButtonRect.width = r.width;
			extendoButtonRect.height = r.height;

			GUI.backgroundColor = extendoButtonHovering ? extendoHoverColor : extendoNormalColor;
			gc_ExtendMenu.text = icon_extendoOpen == null ? (menuOpen ? "Close" : "Open") : "";
			if (GUI.Button(r, gc_ExtendMenu, icon_extendoOpen ? extendoStyle : gridButtonStyleBlank))
			{
				ToggleMenuVisibility();
				extendoButtonHovering = false;
			}
			GUI.backgroundColor = Color.white;
		}

		void ToggleMenuVisibility()
		{
			menuOpen = !menuOpen;
			EditorPrefs.SetBool(pg_Constant.ProGridsIsExtended, menuOpen);

			extendoStyle.normal.background = menuOpen ? icon_extendoClose : icon_extendoOpen;
			extendoStyle.hover.background = menuOpen ? icon_extendoClose : icon_extendoOpen;

			foreach (System.Action<bool> listener in toolbarEventSubscribers)
				listener(menuOpen);

			RepaintSceneView();
		}

		// skip color fading and stuff
		void SetMenuIsExtended(bool isExtended)
		{
			menuOpen = isExtended;
			menuIsOrtho = ortho;
			menuStart = menuOpen ? MENU_EXTENDED : MENU_HIDDEN;

			menuBackgroundColor.a = 0f;
			extendoNormalColor.a = menuBackgroundColor.a;
			extendoHoverColor.a = (menuBackgroundColor.a / .5f);

			extendoStyle.normal.background = menuOpen ? icon_extendoClose : icon_extendoOpen;
			extendoStyle.hover.background = menuOpen ? icon_extendoClose : icon_extendoOpen;

			foreach (System.Action<bool> listener in toolbarEventSubscribers)
				listener(menuOpen);

			EditorPrefs.SetBool(pg_Constant.ProGridsIsExtended, menuOpen);
		}

		private void OpenProGridsPopup()
		{
			if (EditorUtility.DisplayDialog(
				"Upgrade to ProGrids",              // Title
				"Enables all kinds of super-cool features, like different snap values, more units of measurement, and angles.",                       // Message
				"Upgrade",                          // Okay
				"Cancel"                            // Cancel
				))
				// #if UNITY_4
				// AssetStore.OpenURL(pg_Constant.ProGridsUpgradeURL);
				// #else
				Application.OpenURL(pg_Constant.ProGridsUpgradeURL);
			// #endif
		}
		#endregion

		#region ONSCENEGUI

		private Transform lastTransform;
		const string AXIS_CONSTRAINT_KEY = "s";
		const string TEMP_DISABLE_KEY = "d";
		private bool toggleAxisConstraint = false;
		private bool toggleTempSnap = false;
		private Vector3 lastPosition = Vector3.zero;
		// private Vector3 lastRotation = Vector3.zero;
		private Vector3 lastScale = Vector3.one;
		private Vector3 pivot = Vector3.zero, lastPivot = Vector3.zero;
		private Vector3 camDir = Vector3.zero, prevCamDir = Vector3.zero;
		// Distance from camera to pivot at the last time the grid mesh was updated.
		private float lastDistance = 0f;
		public float offset = 0f;

		private bool firstMove = true;

#if PROFILE_TIMES
	pb_Profiler profiler = new pb_Profiler();
#endif

		public bool ortho { get; private set; }
		private bool prevOrtho = false;

		float planeGridDrawDistance = 0f;

		public void OnSceneGUI(SceneView scnview)
		{
			bool isCurrentView = scnview == SceneView.lastActiveSceneView;

			if (isCurrentView)
			{
				Handles.BeginGUI();
				DrawSceneGUI();
				Handles.EndGUI();
			}

			// don't snap stuff in play mode
			if (EditorApplication.isPlayingOrWillChangePlaymode)
				return;

			Event e = Event.current;

			// repaint scene gui if mouse is near controls
			if (isCurrentView && e.type == EventType.MouseMove)
			{
				bool tmp = extendoButtonHovering;
				extendoButtonHovering = extendoButtonRect.Contains(e.mousePosition);

				if (extendoButtonHovering != tmp)
					RepaintSceneView();

				mouseOverMenu = backgroundRect.Contains(e.mousePosition);
			}

			if (e.Equals(Event.KeyboardEvent(AXIS_CONSTRAINT_KEY)))
			{
				toggleAxisConstraint = true;
			}

			if (e.Equals(Event.KeyboardEvent(TEMP_DISABLE_KEY)))
			{
				toggleTempSnap = true;
			}

			if (e.isKey)
			{
				toggleAxisConstraint = false;
				toggleTempSnap = false;
				bool used = true;

				if (e.keyCode == m_IncreaseGridSizeShortcut)
				{
					if (e.type == EventType.KeyUp)
						IncreaseGridSize();
				}
				else if (e.keyCode == m_DecreaseGridSizeShortcut)
				{
					if (e.type == EventType.KeyUp)
						DecreaseGridSize();
				}
				else if (e.keyCode == m_NudgePerspectiveBackwardShortcut)
				{
					if (e.type == EventType.KeyUp && VerifyMenuNudgePerspective())
						MenuNudgePerspectiveBackward();
				}
				else if (e.keyCode == m_NudgePerspectiveForwardShortcut)
				{
					if (e.type == EventType.KeyUp && VerifyMenuNudgePerspective())
						MenuNudgePerspectiveForward();
				}
				else if (e.keyCode == m_NudgePerspectiveResetShortcut)
				{
					if (e.type == EventType.KeyUp && VerifyMenuNudgePerspective())
						MenuNudgePerspectiveReset();
				}
				else if (e.keyCode == m_CyclePerspectiveShortcut)
				{
					if (e.type == EventType.KeyUp)
						CyclePerspective();
				}
				else
				{
					used = false;
				}

				if (used)
					e.Use();
			}

			Camera cam = Camera.current;

			if (cam == null)
				return;

			ortho = cam.orthographic && IsRounded(scnview.rotation.eulerAngles.normalized);

			camDir = pg_Util.CeilFloor(pivot - cam.transform.position);

			if (ortho && !prevOrtho || ortho != menuIsOrtho)
				OnSceneBecameOrtho(isCurrentView);

			if (!ortho && prevOrtho)
				OnSceneBecamePersp(isCurrentView);

			prevOrtho = ortho;

			float camDistance = Vector3.Distance(cam.transform.position, lastPivot);    // distance from camera to pivot

			if (fullGrid)
			{
				pivot = lockGrid || Selection.activeTransform == null ? pivot : Selection.activeTransform.position;
			}
			else
			{
				Vector3 sceneViewPlanePivot = pivot;

				Ray ray = new Ray(cam.transform.position, cam.transform.forward);
				Plane plane = new Plane(Vector3.up, pivot);
				float dist;

				// the only time a locked grid should ever move is if it's pivot is out
				// of the camera's frustum.
				if ((lockGrid && !cam.InFrustum(pivot)) || !lockGrid || scnview != SceneView.lastActiveSceneView)
				{
					if (plane.Raycast(ray, out dist))
						sceneViewPlanePivot = ray.GetPoint(Mathf.Min(dist, planeGridDrawDistance / 2f));
					else
						sceneViewPlanePivot = ray.GetPoint(Mathf.Min(cam.farClipPlane / 2f, planeGridDrawDistance / 2f));
				}

				if (lockGrid)
				{
					pivot = pg_Enum.InverseAxisMask(sceneViewPlanePivot, renderPlane) + pg_Enum.AxisMask(pivot, renderPlane);
				}
				else
				{
					pivot = Selection.activeTransform == null ? pivot : Selection.activeTransform.position;

					if (Selection.activeTransform == null || !cam.InFrustum(pivot))
					{
						pivot = pg_Enum.InverseAxisMask(sceneViewPlanePivot, renderPlane) + pg_Enum.AxisMask(Selection.activeTransform == null ? pivot : Selection.activeTransform.position, renderPlane);
					}
				}
			}

#if PG_DEBUG
		pivotGo.transform.position = pivot;
#endif

			if (drawGrid)
			{
				if (ortho)
				{
					// ortho don't care about pivots
					DrawGridOrthographic(cam);
				}
				else
				{
#if PROFILE_TIMES
				profiler.LogStart("DrawGridPerspective");
#endif

					if (gridRepaint || pivot != lastPivot || Mathf.Abs(camDistance - lastDistance) > lastDistance / 2 || camDir != prevCamDir)
					{
						prevCamDir = camDir;
						gridRepaint = false;
						lastPivot = pivot;
						lastDistance = camDistance;

						if (fullGrid)
						{
							//  if perspective and 3d, use pivot like normal
							pg_GridRenderer.DrawGridPerspective(cam, pivot, snapValue, new Color[3] { gridColorX, gridColorY, gridColorZ }, alphaBump);
						}
						else
						{
							if ((renderPlane & Axis.X) == Axis.X)
								planeGridDrawDistance = pg_GridRenderer.DrawPlane(cam, pivot + Vector3.right * offset, Vector3.up, Vector3.forward, snapValue, gridColorX, alphaBump);

							if ((renderPlane & Axis.Y) == Axis.Y)
								planeGridDrawDistance = pg_GridRenderer.DrawPlane(cam, pivot + Vector3.up * offset, Vector3.right, Vector3.forward, snapValue, gridColorY, alphaBump);

							if ((renderPlane & Axis.Z) == Axis.Z)
								planeGridDrawDistance = pg_GridRenderer.DrawPlane(cam, pivot + Vector3.forward * offset, Vector3.up, Vector3.right, snapValue, gridColorZ, alphaBump);

						}
					}
#if PROFILE_TIMES
				profiler.LogFinish("DrawGridPerspective");
#endif
				}
			}

			// Always keep track of the selection
			if (!Selection.transforms.Contains(lastTransform))
			{
				if (Selection.activeTransform)
				{
					lastTransform = Selection.activeTransform;
					lastPosition = Selection.activeTransform.position;
					lastScale = Selection.activeTransform.localScale;
				}
			}


			if (e.type == EventType.MouseUp)
				firstMove = true;

			if (!snapEnabled || GUIUtility.hotControl < 1)
				return;

			// Bugger.SetKey("Toggle Snap Off", toggleTempSnap);

			/**
			 *	Snapping (for all the junk in PG, this method is literally the only code that actually affects anything).
			 */
			if (Selection.activeTransform && pg_Util.SnapIsEnabled(Selection.activeTransform))
			{
				if (!FuzzyEquals(lastTransform.position, lastPosition))
				{
					Transform selected = lastTransform;

					if (!toggleTempSnap)
					{
						Vector3 old = selected.position;
						Vector3 mask = old - lastPosition;

						bool constraintsOn = toggleAxisConstraint ? !useAxisConstraints : useAxisConstraints;

						if (constraintsOn)
							selected.position = pg_Util.SnapValue(old, mask, snapValue);
						else
							selected.position = pg_Util.SnapValue(old, snapValue);

						Vector3 offset = selected.position - old;

						if (predictiveGrid && firstMove && !fullGrid)
						{
							firstMove = false;
							Axis dragAxis = pg_Util.CalcDragAxis(offset, scnview.camera);

							if (dragAxis != Axis.None && dragAxis != renderPlane)
								SetRenderPlane(dragAxis);
						}

						if (_snapAsGroup)
						{
							OffsetTransforms(Selection.transforms, selected, offset);
						}
						else
						{
							foreach (Transform t in Selection.transforms)
								t.position = constraintsOn ? pg_Util.SnapValue(t.position, mask, snapValue) : pg_Util.SnapValue(t.position, snapValue);
						}
					}

					lastPosition = selected.position;
				}

				if (!FuzzyEquals(lastTransform.localScale, lastScale) && _scaleSnapEnabled)
				{
					if (!toggleTempSnap)
					{
						Vector3 old = lastTransform.localScale;
						Vector3 mask = old - lastScale;

						if (predictiveGrid)
						{
							Axis dragAxis = pg_Util.CalcDragAxis(Selection.activeTransform.TransformDirection(mask), scnview.camera);
							if (dragAxis != Axis.None && dragAxis != renderPlane)
								SetRenderPlane(dragAxis);
						}

						foreach (Transform t in Selection.transforms)
							t.localScale = pg_Util.SnapValue(t.localScale, mask, snapValue);

						lastScale = lastTransform.localScale;
					}
				}
			}
		}

		void OnSelectionChange()
		{
			// Means we don't have to wait for script reloads
			// to respect IgnoreSnap attribute, and keeps the
			// cache small.
			pg_Util.ClearSnapEnabledCache();
		}

		void OnSceneBecameOrtho(bool isCurrentView)
		{
			pg_GridRenderer.Destroy();

			if (isCurrentView && ortho != menuIsOrtho)
				SetMenuIsExtended(menuOpen);
		}

		void OnSceneBecamePersp(bool isCurrentView)
		{
			if (isCurrentView && ortho != menuIsOrtho)
				SetMenuIsExtended(menuOpen);
		}
		#endregion

		#region GRAPHICS

		GameObject go;

		private void DrawGridOrthographic(Camera cam)
		{
			Axis camAxis = AxisWithVector(Camera.current.transform.TransformDirection(Vector3.forward).normalized);

			if (drawGrid)
			{
				switch (camAxis)
				{
					case Axis.X:
					case Axis.NegX:
						DrawGridOrthographic(cam, camAxis, gridColorX_primary, gridColorX);
						break;

					case Axis.Y:
					case Axis.NegY:
						DrawGridOrthographic(cam, camAxis, gridColorY_primary, gridColorY);
						break;

					case Axis.Z:
					case Axis.NegZ:
						DrawGridOrthographic(cam, camAxis, gridColorZ_primary, gridColorZ);
						break;
				}
			}
		}

		int PRIMARY_COLOR_INCREMENT = 10;
		Color previousColor;
		private void DrawGridOrthographic(Camera cam, Axis camAxis, Color primaryColor, Color secondaryColor)
		{
			previousColor = Handles.color;
			Handles.color = primaryColor;

			Vector3 bottomLeft = pg_Util.SnapToFloor(cam.ScreenToWorldPoint(Vector2.zero), snapValue);
			Vector3 bottomRight = pg_Util.SnapToFloor(cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth, 0f)), snapValue);
			Vector3 topLeft = pg_Util.SnapToFloor(cam.ScreenToWorldPoint(new Vector2(0f, cam.pixelHeight)), snapValue);
			Vector3 topRight = pg_Util.SnapToFloor(cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth, cam.pixelHeight)), snapValue);

			Vector3 axis = VectorWithAxis(camAxis);

			float width = Vector3.Distance(bottomLeft, bottomRight);
			float height = Vector3.Distance(bottomRight, topRight);

			// Shift lines to 10m forward of the camera
			bottomLeft += axis * 10f;
			topRight += axis * 10f;
			bottomRight += axis * 10f;
			topLeft += axis * 10f;

			/**
			 *	Draw Vertical Lines
			 */
			Vector3 cam_right = cam.transform.right;
			Vector3 cam_up = cam.transform.up;

			float _snapVal = snapValue;

			int segs = (int)Mathf.Ceil(width / _snapVal) + 2;

			float n = 2f;
			while (segs > MAX_LINES)
			{
				_snapVal = _snapVal * n;
				segs = (int)Mathf.Ceil(width / _snapVal) + 2;
				n++;
			}

			/// Screen start and end
			Vector3 bl = cam_right.Sum() > 0 ? pg_Util.SnapToFloor(bottomLeft, cam_right, _snapVal * PRIMARY_COLOR_INCREMENT) : pg_Util.SnapToCeil(bottomLeft, cam_right, _snapVal * PRIMARY_COLOR_INCREMENT);
			Vector3 start = bl - cam_up * (height + _snapVal * 2);
			Vector3 end = bl + cam_up * (height + _snapVal * 2);

			segs += PRIMARY_COLOR_INCREMENT;

			/// The current line start and end
			Vector3 line_start = Vector3.zero;
			Vector3 line_end = Vector3.zero;

			for (int i = -1; i < segs; i++)
			{
				line_start = start + (i * (cam_right * _snapVal));
				line_end = end + (i * (cam_right * _snapVal));
				Handles.color = i % PRIMARY_COLOR_INCREMENT == 0 ? primaryColor : secondaryColor;
				Handles.DrawLine(line_start, line_end);
			}

			/**
			 * Draw Horizontal Lines
			 */
			segs = (int)Mathf.Ceil(height / _snapVal) + 2;

			n = 2;
			while (segs > MAX_LINES)
			{
				_snapVal = _snapVal * n;
				segs = (int)Mathf.Ceil(height / _snapVal) + 2;
				n++;
			}

			Vector3 tl = cam_up.Sum() > 0 ? pg_Util.SnapToCeil(topLeft, cam_up, _snapVal * PRIMARY_COLOR_INCREMENT) : pg_Util.SnapToFloor(topLeft, cam_up, _snapVal * PRIMARY_COLOR_INCREMENT);
			start = tl - cam_right * (width + _snapVal * 2);
			end = tl + cam_right * (width + _snapVal * 2);

			segs += (int)PRIMARY_COLOR_INCREMENT;

			for (int i = -1; i < segs; i++)
			{
				line_start = start + (i * (-cam_up * _snapVal));
				line_end = end + (i * (-cam_up * _snapVal));
				Handles.color = i % PRIMARY_COLOR_INCREMENT == 0 ? primaryColor : secondaryColor;
				Handles.DrawLine(line_start, line_end);
			}

#if PRO
			if (drawAngles)
			{
				Vector3 cen = pg_Util.SnapValue(((topRight + bottomLeft) / 2f), snapValue);

				float half = (width > height) ? width : height;

				float opposite = Mathf.Tan(Mathf.Deg2Rad * angleValue) * half;

				Vector3 up = cam.transform.up * opposite;
				Vector3 right = cam.transform.right * half;

				Vector3 bottomLeftAngle = cen - (up + right);
				Vector3 topRightAngle = cen + (up + right);

				Vector3 bottomRightAngle = cen + (right - up);
				Vector3 topLeftAngle = cen + (up - right);

				Handles.color = primaryColor;

				// y = 1x+1
				Handles.DrawLine(bottomLeftAngle, topRightAngle);

				// y = -1x-1
				Handles.DrawLine(topLeftAngle, bottomRightAngle);
			}
#endif

			Handles.color = previousColor;
		}
		#endregion

		#region ENUM UTILITY

		public SnapUnit SnapUnitWithString(string str)
		{
			foreach (SnapUnit su in SnapUnit.GetValues(typeof(SnapUnit)))
			{
				if (su.ToString() == str)
					return su;
			}
			return (SnapUnit)0;
		}

		public Axis AxisWithVector(Vector3 val)
		{
			Vector3 v = new Vector3(Mathf.Abs(val.x), Mathf.Abs(val.y), Mathf.Abs(val.z));

			if (v.x > v.y && v.x > v.z)
			{
				if (val.x > 0)
					return Axis.X;
				else
					return Axis.NegX;
			}
			else
			if (v.y > v.x && v.y > v.z)
			{
				if (val.y > 0)
					return Axis.Y;
				else
					return Axis.NegY;
			}
			else
			{
				if (val.z > 0)
					return Axis.Z;
				else
					return Axis.NegZ;
			}
		}

		public Vector3 VectorWithAxis(Axis axis)
		{
			switch (axis)
			{
				case Axis.X:
					return Vector3.right;
				case Axis.Y:
					return Vector3.up;
				case Axis.Z:
					return Vector3.forward;
				case Axis.NegX:
					return -Vector3.right;
				case Axis.NegY:
					return -Vector3.up;
				case Axis.NegZ:
					return -Vector3.forward;

				default:
					return Vector3.forward;
			}
		}

		public bool IsRounded(Vector3 v)
		{
			return (Mathf.Approximately(v.x, 1f) || Mathf.Approximately(v.y, 1f) || Mathf.Approximately(v.z, 1f)) || v == Vector3.zero;
		}

		public Vector3 RoundAxis(Vector3 v)
		{
			return VectorWithAxis(AxisWithVector(v));
		}
		#endregion

		#region MOVING TRANSFORMS

		static bool FuzzyEquals(Vector3 lhs, Vector3 rhs)
		{
			return Mathf.Abs(lhs.x - rhs.x) < .001f && Mathf.Abs(lhs.y - rhs.y) < .001f && Mathf.Abs(lhs.z - rhs.z) < .001f;
		}

		public void OffsetTransforms(Transform[] trsfrms, Transform ignore, Vector3 offset)
		{
			foreach (Transform t in trsfrms)
			{
				if (t != ignore)
					t.position += offset;
			}
		}

		void HierarchyWindowChanged()
		{
			if (Selection.activeTransform != null)
				lastPosition = Selection.activeTransform.position;
		}
		#endregion

		#region SETTINGS

		public void SetSnapEnabled(bool enable)
		{
			EditorPrefs.SetBool(pg_Constant.SnapEnabled, enable);

			if (Selection.activeTransform)
			{
				lastTransform = Selection.activeTransform;
				lastPosition = Selection.activeTransform.position;
			}

			snapEnabled = enable;
			gridRepaint = true;
			RepaintSceneView();
		}

		public void SetSnapValue(SnapUnit su, float val, int multiplier)
		{
			int clamp_multiplier = (int)(Mathf.Min(Mathf.Max(1, multiplier), int.MaxValue));

			float value_multiplier = clamp_multiplier / (float)DEFAULT_SNAP_MULTIPLIER;

			/**
			 * multiplier is a value modifies the snap val.  100 = no change,
			 * 50 is half val, 200 is double val, etc.
			 */
			snapValue = pg_Enum.SnapUnitValue(su) * val * value_multiplier;
			RepaintSceneView();

			EditorPrefs.SetInt(pg_Constant.GridUnit, (int)su);
			EditorPrefs.SetFloat(pg_Constant.SnapValue, val);
			EditorPrefs.SetInt(pg_Constant.SnapMultiplier, clamp_multiplier);


			// update gui (only necessary when calling with editorpref values)
			t_snapValue = val * value_multiplier;
			snapUnit = su;

			switch (su)
			{
				case SnapUnit.Inch:
					PRIMARY_COLOR_INCREMENT = 12;   // blasted imperial units
					break;

				case SnapUnit.Foot:
					PRIMARY_COLOR_INCREMENT = 3;
					break;

				default:
					PRIMARY_COLOR_INCREMENT = 10;
					break;
			}

			if (EditorPrefs.GetBool(pg_Constant.SyncUnitySnap, true))
			{
				EditorPrefs.SetFloat("MoveSnapX", snapValue);
				EditorPrefs.SetFloat("MoveSnapY", snapValue);
				EditorPrefs.SetFloat("MoveSnapZ", snapValue);

				if (EditorPrefs.GetBool(pg_Constant.SnapScale, true))
					EditorPrefs.SetFloat("ScaleSnap", snapValue);

				// If Unity snap sync is enabled, refresh the Snap Settings window if it's open. 
				Type snapSettings = typeof(EditorWindow).Assembly.GetType("UnityEditor.SnapSettings");

				if (snapSettings != null)
				{
					FieldInfo snapInitialized = snapSettings.GetField("s_Initialized", BindingFlags.NonPublic | BindingFlags.Static);

					if (snapInitialized != null)
					{
						snapInitialized.SetValue(null, (object)false);

						EditorWindow win = Resources.FindObjectsOfTypeAll<EditorWindow>().FirstOrDefault(x => x.ToString().Contains("SnapSettings"));

						if (win != null)
							win.Repaint();
					}
				}
			}

			gridRepaint = true;
		}

		public void SetRenderPlane(Axis axis)
		{
			offset = 0f;
			fullGrid = false;
			renderPlane = axis;
			EditorPrefs.SetBool(pg_Constant.PerspGrid, fullGrid);
			EditorPrefs.SetInt(pg_Constant.GridAxis, (int)renderPlane);
			gridRepaint = true;
			RepaintSceneView();
		}

		public void SetGridEnabled(bool enable)
		{
			drawGrid = enable;

			if (!drawGrid)
				pg_GridRenderer.Destroy();
			else
				pg_Util.SetUnityGridEnabled(false);

			EditorPrefs.SetBool("showgrid", enable);

			gridRepaint = true;
			RepaintSceneView();
		}

		public void SetDrawAngles(bool enable)
		{
			drawAngles = enable;
			gridRepaint = true;
			RepaintSceneView();
		}

		private void SnapToGrid(Transform[] transforms)
		{
			Undo.RecordObjects(transforms as UnityEngine.Object[], "Snap to Grid");

			foreach (Transform t in transforms)
				t.position = pg_Util.SnapValue(t.position, snapValue);

			gridRepaint = true;

			PushToGrid(snapValue);
		}
		#endregion

		#region GLOBAL SETTING

		internal bool GetUseAxisConstraints() { return toggleAxisConstraint ? !useAxisConstraints : useAxisConstraints; }
		internal float GetSnapValue() { return snapValue; }
		internal bool GetSnapEnabled() { return (toggleTempSnap ? !snapEnabled : snapEnabled); }

		/**
		 * Returns the value of useAxisConstraints, accounting for the shortcut key toggle.
		 */
		public static bool UseAxisConstraints()
		{
			return instance != null ? instance.GetUseAxisConstraints() : false;
		}

		/**
		 * Return the current snap value.
		 */
		public static float SnapValue()
		{
			return instance != null ? instance.GetSnapValue() : 0f;
		}

		/**
		 * Return true if snapping is enabled, false otherwise.
		 */
		public static bool SnapEnabled()
		{
			return instance == null ? false : instance.GetSnapEnabled();
		}

		public static void AddPushToGridListener(System.Action<float> listener)
		{
			pushToGridListeners.Add(listener);
		}

		public static void RemovePushToGridListener(System.Action<float> listener)
		{
			pushToGridListeners.Remove(listener);
		}

		public static void AddToolbarEventSubscriber(System.Action<bool> listener)
		{
			toolbarEventSubscribers.Add(listener);
		}

		public static void RemoveToolbarEventSubscriber(System.Action<bool> listener)
		{
			toolbarEventSubscribers.Remove(listener);
		}

		public static bool SceneToolbarActive()
		{
			return instance != null;
		}

		[SerializeField]
		static List<System.Action<float>> pushToGridListeners = new List<System.Action<float>>();
		[SerializeField]
		static List<System.Action<bool>> toolbarEventSubscribers = new List<System.Action<bool>>();

		private void PushToGrid(float snapValue)
		{
			foreach (System.Action<float> listener in pushToGridListeners)
				listener(snapValue);
		}

		public static void OnHandleMove(Vector3 worldDirection)
		{
			if (instance != null)
				instance.OnHandleMove_Internal(worldDirection);
		}

		private void OnHandleMove_Internal(Vector3 worldDirection)
		{
			if (predictiveGrid && firstMove && !fullGrid)
			{
				firstMove = false;
				Axis dragAxis = pg_Util.CalcDragAxis(worldDirection, SceneView.lastActiveSceneView.camera);

				if (dragAxis != Axis.None && dragAxis != renderPlane)
					SetRenderPlane(dragAxis);
			}
		}
		#endregion
	}
}
