// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public class NodeUtils
	{

		public delegate void DrawPropertySection();

		public static void DrawPropertyGroup( string sectionName, DrawPropertySection DrawSection )
		{
			Color cachedColor = GUI.color;
			GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, 0.5f );
			EditorGUILayout.BeginHorizontal( UIUtils.MenuItemToolbarStyle );
			GUI.color = cachedColor;

			GUILayout.Label( sectionName, UIUtils.MenuItemToggleStyle );

			EditorGUILayout.EndHorizontal();


			cachedColor = GUI.color;
			GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, ( EditorGUIUtility.isProSkin ? 0.5f : 0.25f ) );
			EditorGUILayout.BeginVertical( UIUtils.MenuItemBackgroundStyle );
			GUI.color = cachedColor;
			DrawSection();
			EditorGUILayout.Separator();
			EditorGUILayout.EndVertical();
		}


		public static void DrawNestedPropertyGroup( ref bool foldoutValue, string sectionName, DrawPropertySection DrawSection, int horizontalSpacing = 15 )
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.Space( horizontalSpacing );
				EditorGUILayout.BeginVertical( EditorStyles.helpBox );
				{
					Color cachedColor = GUI.color;
					GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, 0.5f );
					EditorGUILayout.BeginHorizontal();
					{
						GUI.color = cachedColor;
						bool value = GUILayout.Toggle( foldoutValue, sectionName, UIUtils.MenuItemToggleStyle );
						if( Event.current.button == Constants.FoldoutMouseId )
						{
							foldoutValue = value;
						}
					}
					EditorGUILayout.EndHorizontal();
					EditorGUI.indentLevel--;
					if( foldoutValue )
					{
						cachedColor = GUI.color;
						GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, ( EditorGUIUtility.isProSkin ? 0.5f : 0.25f ) );
						{
							EditorGUILayout.BeginVertical( UIUtils.MenuItemBackgroundStyle );
							{
								GUI.color = cachedColor;
								DrawSection();
							}
							EditorGUILayout.EndVertical();
							EditorGUILayout.Separator();
						}
					}
					EditorGUI.indentLevel++;
				}
				EditorGUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();
		}

		public static void DrawNestedPropertyGroup( ref bool foldoutValue, string sectionName, DrawPropertySection DrawSection, DrawPropertySection HeaderSection )
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.Space( 15 );
				EditorGUILayout.BeginVertical( EditorStyles.helpBox );
				Color cachedColor = GUI.color;
				GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, 0.5f );
				EditorGUILayout.BeginHorizontal();
				GUI.color = cachedColor;

				bool value = GUILayout.Toggle( foldoutValue, sectionName, UIUtils.MenuItemToggleStyle );
				if( Event.current.button == Constants.FoldoutMouseId )
				{
					foldoutValue = value;
				}
				HeaderSection();
				EditorGUILayout.EndHorizontal();
				EditorGUI.indentLevel--;
				if( foldoutValue )
				{
					cachedColor = GUI.color;
					GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, ( EditorGUIUtility.isProSkin ? 0.5f : 0.25f ) );
					EditorGUILayout.BeginVertical( UIUtils.MenuItemBackgroundStyle );
					GUI.color = cachedColor;
					DrawSection();
					EditorGUILayout.EndVertical();
					EditorGUILayout.Separator();
				}
				EditorGUI.indentLevel++;
				EditorGUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();
		}

		public static void DrawNestedPropertyGroup( UndoParentNode owner, ref bool foldoutValue, ref bool enabledValue, string sectionName, DrawPropertySection DrawSection )
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.Space( 15 );
				EditorGUILayout.BeginVertical( EditorStyles.helpBox );
				Color cachedColor = GUI.color;
				GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, 0.5f );
				EditorGUILayout.BeginHorizontal();
				GUI.color = cachedColor;

				bool value = GUILayout.Toggle( foldoutValue, sectionName, UIUtils.MenuItemToggleStyle );
				if( Event.current.button == Constants.FoldoutMouseId )
				{
					foldoutValue = value;
				}
				
				value = ( (object)owner != null ) ? owner.GUILayoutToggle( enabledValue, string.Empty,UIUtils.MenuItemEnableStyle, GUILayout.Width( 16 ) ) :
										GUILayout.Toggle( enabledValue, string.Empty, UIUtils.MenuItemEnableStyle, GUILayout.Width( 16 ) );
				if( Event.current.button == Constants.FoldoutMouseId )
				{
					enabledValue = value;
				}
				

				EditorGUILayout.EndHorizontal();
				EditorGUI.indentLevel--;
				if( foldoutValue )
				{
					cachedColor = GUI.color;
					GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, ( EditorGUIUtility.isProSkin ? 0.5f : 0.25f ) );
					EditorGUILayout.BeginVertical( UIUtils.MenuItemBackgroundStyle );
					GUI.color = cachedColor;
					DrawSection();
					EditorGUILayout.EndVertical();
					EditorGUILayout.Separator();
				}
				EditorGUI.indentLevel++;
				EditorGUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();
		}


		public static void DrawPropertyGroup( ref bool foldoutValue, string sectionName, DrawPropertySection DrawSection )
		{
			Color cachedColor = GUI.color;
			GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, 0.5f );
			EditorGUILayout.BeginHorizontal( UIUtils.MenuItemToolbarStyle );
			GUI.color = cachedColor;

			bool value = GUILayout.Toggle( foldoutValue, sectionName, UIUtils.MenuItemToggleStyle );
			if( Event.current.button == Constants.FoldoutMouseId )
			{
				foldoutValue = value;
			}
			EditorGUILayout.EndHorizontal();

			if( foldoutValue )
			{
				cachedColor = GUI.color;
				GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, ( EditorGUIUtility.isProSkin ? 0.5f : 0.25f ) );
				EditorGUILayout.BeginVertical( UIUtils.MenuItemBackgroundStyle );
				{
					GUI.color = cachedColor;
					EditorGUI.indentLevel++;
					DrawSection();
					EditorGUI.indentLevel--;
					EditorGUILayout.Separator();
				}
				EditorGUILayout.EndVertical();
			}
		}

		public static void DrawPropertyGroup( ref bool foldoutValue, string sectionName, DrawPropertySection DrawSection, DrawPropertySection HeaderSection )
		{
			Color cachedColor = GUI.color;
			GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, 0.5f );
			EditorGUILayout.BeginHorizontal( UIUtils.MenuItemToolbarStyle );
			GUI.color = cachedColor;

			bool value = GUILayout.Toggle( foldoutValue, sectionName, UIUtils.MenuItemToggleStyle );
			if( Event.current.button == Constants.FoldoutMouseId )
			{
				foldoutValue = value;
			}
			HeaderSection();
			EditorGUILayout.EndHorizontal();

			if( foldoutValue )
			{
				cachedColor = GUI.color;
				GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, ( EditorGUIUtility.isProSkin ? 0.5f : 0.25f ) );
				EditorGUILayout.BeginVertical( UIUtils.MenuItemBackgroundStyle );
				{
					GUI.color = cachedColor;
					EditorGUI.indentLevel++;
					DrawSection();
					EditorGUI.indentLevel--;
					EditorGUILayout.Separator();
				}
				EditorGUILayout.EndVertical();
			}
		}


		public static bool DrawPropertyGroup( UndoParentNode owner, ref bool foldoutValue, ref bool enabledValue, string sectionName, DrawPropertySection DrawSection )
		{
			bool enableChanged = false;
			Color cachedColor = GUI.color;
			GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, 0.5f );
			EditorGUILayout.BeginHorizontal( UIUtils.MenuItemToolbarStyle );
			GUI.color = cachedColor;
			bool value = GUILayout.Toggle( foldoutValue, sectionName, UIUtils.MenuItemToggleStyle, GUILayout.ExpandWidth( true ) );
			if( Event.current.button == Constants.FoldoutMouseId )
			{
				foldoutValue = value;
			}
			EditorGUI.BeginChangeCheck();
			value = ( (object)owner != null ) ? owner.EditorGUILayoutToggle( string.Empty, enabledValue, UIUtils.MenuItemEnableStyle, GUILayout.Width( 16 ) ) :
											EditorGUILayout.Toggle( string.Empty, enabledValue, UIUtils.MenuItemEnableStyle, GUILayout.Width( 16 ) );
			if( Event.current.button == Constants.FoldoutMouseId )
			{
				enabledValue = value;
			}
			if( EditorGUI.EndChangeCheck() )
			{
				enableChanged = true;
			}

			EditorGUILayout.EndHorizontal();

			if( foldoutValue )
			{
				cachedColor = GUI.color;
				GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, ( EditorGUIUtility.isProSkin ? 0.5f : 0.25f ) );
				EditorGUILayout.BeginVertical( UIUtils.MenuItemBackgroundStyle );
				GUI.color = cachedColor;

				EditorGUILayout.Separator();
				EditorGUI.BeginDisabledGroup( !enabledValue );

				EditorGUI.indentLevel += 1;

				DrawSection();

				EditorGUI.indentLevel -= 1;
				EditorGUI.EndDisabledGroup();
				EditorGUILayout.Separator();
				EditorGUILayout.EndVertical();
			}

			return enableChanged;
		}
	}
}
