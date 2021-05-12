using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ProGrids
{
	public class pg_Preferences
	{
		static Color _gridColorX;
		static Color _gridColorY;
		static Color _gridColorZ;
		static float _alphaBump;
		static bool _scaleSnapEnabled;
		static int _snapMethod;
		static float _BracketIncreaseValue;
		static SnapUnit _GridUnits;
		static bool _syncUnitySnap;

		static KeyCode _IncreaseGridSize = KeyCode.Equals;
		static KeyCode _DecreaseGridSize = KeyCode.Minus;
		static KeyCode _NudgePerspectiveBackward = KeyCode.LeftBracket;
		static KeyCode _NudgePerspectiveForward = KeyCode.RightBracket;
		static KeyCode _NudgePerspectiveReset = KeyCode.Alpha0;
		static KeyCode _CyclePerspective = KeyCode.Backslash;


		/** Defaults **/
		public static Color GRID_COLOR_X = new Color(.9f, .46f, .46f, .15f);
		public static Color GRID_COLOR_Y = new Color(.46f, .9f, .46f, .15f);
		public static Color GRID_COLOR_Z = new Color(.46f, .46f, .9f, .15f);
		public static float ALPHA_BUMP = .25f;
		public static bool USE_AXIS_CONSTRAINTS = false;
		public static bool SHOW_GRID = true;

		static string[] SnapMethod = new string[]
		{
			"Snap on Selected Axis",
			"Snap on All Axes"
		};

		static int[] SnapVals = new int[] { 1, 0 };
		static bool prefsLoaded = false;

		[PreferenceItem("ProGrids")]
		public static void PreferencesGUI()
		{
			if (!prefsLoaded)
				prefsLoaded = LoadPreferences();

			// EditorGUILayout.HelpBox("Changes will take effect on the next ProGrids open.", MessageType.Info);

			GUILayout.Label("Grid Colors per Axis", EditorStyles.boldLabel);
			_gridColorX = EditorGUILayout.ColorField("X Axis", _gridColorX);
			_gridColorY = EditorGUILayout.ColorField("Y Axis", _gridColorY);
			_gridColorZ = EditorGUILayout.ColorField("Z Axis", _gridColorZ);

			_alphaBump = EditorGUILayout.Slider(new GUIContent("Tenth Line Alpha", "Every 10th line will have it's alpha value bumped by this amount."), _alphaBump, 0f, 1f);

			// not used
			// _BracketIncreaseValue = EditorGUILayout.FloatField(new GUIContent("Grid Increment Value", "Affects the amount by which the bracket keys will increment or decrement that snap value."), _BracketIncreaseValue);

			_GridUnits = (SnapUnit)EditorGUILayout.EnumPopup("Grid Units", _GridUnits);

			_scaleSnapEnabled = EditorGUILayout.Toggle("Snap On Scale", _scaleSnapEnabled);

			// GUILayout.BeginHorizontal();
			// EditorGUILayout.PrefixLabel(new GUIContent("Axis Constraints", "If toggled, objects will be automatically grid aligned on all axes when moving."));

			_snapMethod = EditorGUILayout.IntPopup("Snap Method", _snapMethod, SnapMethod, SnapVals);

			_syncUnitySnap = EditorGUILayout.Toggle("Sync w/ Unity Snap", _syncUnitySnap);

			// GUILayout.EndHorizontal();

			GUILayout.Label("Shortcuts", EditorStyles.boldLabel);
			_IncreaseGridSize = (KeyCode)EditorGUILayout.EnumPopup("Increase Grid Size", _IncreaseGridSize);
			_DecreaseGridSize = (KeyCode)EditorGUILayout.EnumPopup("Decrease Grid Size", _DecreaseGridSize);
			_NudgePerspectiveBackward = (KeyCode)EditorGUILayout.EnumPopup("Nudge Perspective Backward", _NudgePerspectiveBackward);
			_NudgePerspectiveForward = (KeyCode)EditorGUILayout.EnumPopup("Nudge Perspective Forward", _NudgePerspectiveForward);
			_NudgePerspectiveReset = (KeyCode)EditorGUILayout.EnumPopup("Nudge Perspective Reset", _NudgePerspectiveReset);
			_CyclePerspective = (KeyCode)EditorGUILayout.EnumPopup("Cycle Perspective", _CyclePerspective);

			if (GUILayout.Button("Reset"))
			{
				if (EditorUtility.DisplayDialog("Delete ProGrids editor preferences?", "Are you sure you want to delete these?, this action cannot be undone.", "Yes", "No"))
					ResetPrefs();
			}

			if (GUI.changed)
				SetPreferences();
		}

		public static bool LoadPreferences()
		{
			_scaleSnapEnabled = EditorPrefs.HasKey("scaleSnapEnabled") ? EditorPrefs.GetBool("scaleSnapEnabled") : false;
			_gridColorX = (EditorPrefs.HasKey("gridColorX")) ? pg_Util.ColorWithString(EditorPrefs.GetString("gridColorX")) : GRID_COLOR_X;
			_gridColorY = (EditorPrefs.HasKey("gridColorY")) ? pg_Util.ColorWithString(EditorPrefs.GetString("gridColorY")) : GRID_COLOR_Y;
			_gridColorZ = (EditorPrefs.HasKey("gridColorZ")) ? pg_Util.ColorWithString(EditorPrefs.GetString("gridColorZ")) : GRID_COLOR_Z;
			_alphaBump = (EditorPrefs.HasKey("pg_alphaBump")) ? EditorPrefs.GetFloat("pg_alphaBump") : ALPHA_BUMP;
			_snapMethod = System.Convert.ToInt32(
				(EditorPrefs.HasKey(pg_Constant.UseAxisConstraints)) ? EditorPrefs.GetBool(pg_Constant.UseAxisConstraints) : USE_AXIS_CONSTRAINTS
				);
			_BracketIncreaseValue = EditorPrefs.HasKey(pg_Constant.BracketIncreaseValue) ? EditorPrefs.GetFloat(pg_Constant.BracketIncreaseValue) : .25f;
			_GridUnits = (SnapUnit)(EditorPrefs.HasKey(pg_Constant.GridUnit) ? EditorPrefs.GetInt(pg_Constant.GridUnit) : 0);
			_syncUnitySnap = EditorPrefs.GetBool(pg_Constant.SyncUnitySnap, true);

			_IncreaseGridSize = EditorPrefs.HasKey("pg_Editor::IncreaseGridSize")
				? (KeyCode)EditorPrefs.GetInt("pg_Editor::IncreaseGridSize")
				: KeyCode.Equals;
			_DecreaseGridSize = EditorPrefs.HasKey("pg_Editor::DecreaseGridSize")
				? (KeyCode)EditorPrefs.GetInt("pg_Editor::DecreaseGridSize")
				: KeyCode.Minus;
			_NudgePerspectiveBackward = EditorPrefs.HasKey("pg_Editor::NudgePerspectiveBackward")
				? (KeyCode)EditorPrefs.GetInt("pg_Editor::NudgePerspectiveBackward")
				: KeyCode.LeftBracket;
			_NudgePerspectiveForward = EditorPrefs.HasKey("pg_Editor::NudgePerspectiveForward")
				? (KeyCode)EditorPrefs.GetInt("pg_Editor::NudgePerspectiveForward")
				: KeyCode.RightBracket;
			_NudgePerspectiveReset = EditorPrefs.HasKey("pg_Editor::NudgePerspectiveReset")
				? (KeyCode)EditorPrefs.GetInt("pg_Editor::NudgePerspectiveReset")
				: KeyCode.Alpha0;
			_CyclePerspective = EditorPrefs.HasKey("pg_Editor::CyclePerspective")
				? (KeyCode)EditorPrefs.GetInt("pg_Editor::CyclePerspective")
				: KeyCode.Backslash;

			return true;
		}

		public static void SetPreferences()
		{
			EditorPrefs.SetBool("scaleSnapEnabled", _scaleSnapEnabled);
			EditorPrefs.SetString("gridColorX", _gridColorX.ToString("f3"));
			EditorPrefs.SetString("gridColorY", _gridColorY.ToString("f3"));
			EditorPrefs.SetString("gridColorZ", _gridColorZ.ToString("f3"));
			EditorPrefs.SetFloat("pg_alphaBump", _alphaBump);
			EditorPrefs.SetBool(pg_Constant.UseAxisConstraints, System.Convert.ToBoolean(_snapMethod));
			EditorPrefs.SetFloat(pg_Constant.BracketIncreaseValue, _BracketIncreaseValue);
			EditorPrefs.SetInt(pg_Constant.GridUnit, (int)_GridUnits);
			EditorPrefs.SetBool(pg_Constant.SyncUnitySnap, _syncUnitySnap);
			EditorPrefs.SetInt("pg_Editor::IncreaseGridSize", (int)_IncreaseGridSize);
			EditorPrefs.SetInt("pg_Editor::DecreaseGridSize", (int)_DecreaseGridSize);
			EditorPrefs.SetInt("pg_Editor::NudgePerspectiveBackward", (int)_NudgePerspectiveBackward);
			EditorPrefs.SetInt("pg_Editor::NudgePerspectiveForward", (int)_NudgePerspectiveForward);
			EditorPrefs.SetInt("pg_Editor::NudgePerspectiveReset", (int)_NudgePerspectiveReset);
			EditorPrefs.SetInt("pg_Editor::CyclePerspective", (int)_CyclePerspective);

			if (pg_Editor.instance != null)
			{
				pg_Editor.instance.LoadPreferences();
			}
		}

		public static void ResetPrefs()
		{
			EditorPrefs.DeleteKey("scaleSnapEnabled");
			EditorPrefs.DeleteKey("gridColorX");
			EditorPrefs.DeleteKey("gridColorY");
			EditorPrefs.DeleteKey("gridColorZ");
			EditorPrefs.DeleteKey("pg_alphaBump");
			EditorPrefs.DeleteKey(pg_Constant.UseAxisConstraints);
			EditorPrefs.DeleteKey(pg_Constant.BracketIncreaseValue);
			EditorPrefs.DeleteKey(pg_Constant.GridUnit);
			EditorPrefs.DeleteKey("showgrid");
			EditorPrefs.DeleteKey(pg_Constant.SnapMultiplier);
			EditorPrefs.DeleteKey(pg_Constant.SyncUnitySnap);
			EditorPrefs.DeleteKey("pg_Editor::IncreaseGridSize");
			EditorPrefs.DeleteKey("pg_Editor::DecreaseGridSize");
			EditorPrefs.DeleteKey("pg_Editor::NudgePerspectiveBackward");
			EditorPrefs.DeleteKey("pg_Editor::NudgePerspectiveForward");
			EditorPrefs.DeleteKey("pg_Editor::NudgePerspectiveReset");
			EditorPrefs.DeleteKey("pg_Editor::CyclePerspective");

			LoadPreferences();
		}
	}
}
