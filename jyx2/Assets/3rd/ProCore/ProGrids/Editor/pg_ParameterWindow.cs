using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ProGrids
{

	public class pg_ParameterWindow : EditorWindow
	{
		public pg_Editor editor;

		GUIContent gc_predictiveGrid = new GUIContent("Predictive Grid", "If enabled, the grid will automatically render at the optimal axis based on movement.");
		GUIContent gc_snapAsGroup = new GUIContent("Snap as Group", "If enabled, selected objects will keep their relative offsets when moving.  If disabled, every object in the selection is snapped to grid independently.");

		void OnGUI()
		{
			GUILayout.Label("Snap Settings", EditorStyles.boldLabel);

			float snap = editor.GetSnapIncrement();

			EditorGUI.BeginChangeCheck();

			snap = EditorGUILayout.FloatField("Snap Value", snap);

			if(EditorGUI.EndChangeCheck())
				editor.SetSnapIncrement(snap);

			EditorGUI.BeginChangeCheck();
			int majorLineIncrement = EditorPrefs.GetInt(pg_Constant.MajorLineIncrement, 10);
			majorLineIncrement = EditorGUILayout.IntField("Major Line Increment", majorLineIncrement);
			majorLineIncrement = majorLineIncrement < 2 ? 2 : majorLineIncrement > 128 ? 128 : majorLineIncrement;
			if(EditorGUI.EndChangeCheck())
			{
				EditorPrefs.SetInt(pg_Constant.MajorLineIncrement, majorLineIncrement);
				pg_GridRenderer.majorLineIncrement = majorLineIncrement;
				pg_Editor.ForceRepaint();
			}

			editor.ScaleSnapEnabled = EditorGUILayout.Toggle("Snap On Scale", editor.ScaleSnapEnabled);

			SnapUnit _gridUnits = (SnapUnit)(EditorPrefs.HasKey(pg_Constant.GridUnit) ? EditorPrefs.GetInt(pg_Constant.GridUnit) : 0);

			bool snapAsGroup = editor.snapAsGroup;
			snapAsGroup = EditorGUILayout.Toggle(gc_snapAsGroup, snapAsGroup);
			if(snapAsGroup != editor.snapAsGroup)
				editor.snapAsGroup = snapAsGroup;

			EditorGUI.BeginChangeCheck();

			_gridUnits = (SnapUnit)EditorGUILayout.EnumPopup("Grid Units", _gridUnits);

			EditorGUI.BeginChangeCheck();
			editor.angleValue = EditorGUILayout.Slider("Angle", editor.angleValue, 0f, 180f);
			if(EditorGUI.EndChangeCheck())
				SceneView.RepaintAll();

			if( EditorGUI.EndChangeCheck() )
			{
				EditorPrefs.SetInt(pg_Constant.GridUnit, (int) _gridUnits);
				editor.LoadPreferences();
			}

			bool tmp = editor.predictiveGrid;
			tmp = EditorGUILayout.Toggle(gc_predictiveGrid, tmp);
			if( tmp != editor.predictiveGrid )
			{
				editor.predictiveGrid = tmp;
				EditorPrefs.SetBool(pg_Constant.PredictiveGrid, tmp);
			}

			GUILayout.FlexibleSpace();

			if( GUILayout.Button("Done"))
				this.Close();
		}
	}

}
