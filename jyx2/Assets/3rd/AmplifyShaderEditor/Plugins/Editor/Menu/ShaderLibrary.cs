// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class ShaderLibrary : EditorWindow
	{
		private const string SHADER_LIB_FILE = "/AmplifyShaderEditor/Resources/ShaderLibrary/ShaderLibrary.txt";
		private bool m_init = false;
		private Vector2 m_scrollPos = new Vector2();
		[SerializeField]
		private List<string> m_shaders = new List<string>();
		void Init()
		{
			m_init = true;
			string list = IOUtils.LoadTextFileFromDisk( Application.dataPath + SHADER_LIB_FILE );
			if ( String.IsNullOrEmpty( list ) )
				return;

			string[] listArr = list.Split( IOUtils.FIELD_SEPARATOR );
			for ( int i = 0; i < listArr.Length; i++ )
			{
				m_shaders.Add( listArr[ i ] );
			}

			UIUtils.MainSkin.customStyles[ 10 ].active.background = Texture2D.whiteTexture;

			UIUtils.MainSkin.customStyles[ 6 ].fixedHeight = UIUtils.MainSkin.customStyles[ 6 ].normal.background.height;
			UIUtils.MainSkin.customStyles[ 6 ].fixedWidth = UIUtils.MainSkin.customStyles[ 6 ].normal.background.width;

			UIUtils.MainSkin.customStyles[ 7 ].fixedHeight = UIUtils.MainSkin.customStyles[ 7 ].normal.background.height;
			UIUtils.MainSkin.customStyles[ 7 ].fixedWidth = UIUtils.MainSkin.customStyles[ 7 ].normal.background.width;

			UIUtils.MainSkin.customStyles[ 8 ].fixedHeight = UIUtils.MainSkin.customStyles[ 8 ].normal.background.height;
			UIUtils.MainSkin.customStyles[ 8 ].fixedWidth = UIUtils.MainSkin.customStyles[ 8 ].normal.background.width;

			UIUtils.MainSkin.customStyles[ 9 ].fixedHeight = UIUtils.MainSkin.customStyles[ 9 ].normal.background.height;
			UIUtils.MainSkin.customStyles[ 9 ].fixedWidth = UIUtils.MainSkin.customStyles[ 9 ].normal.background.width;
			
		}

		void OnGUI()
		{
			if ( !m_init )
			{
				Init();
			}

			Rect availableArea = position;
			
			availableArea.y = 100f;
			availableArea.x = 0.05f * availableArea.width;
			availableArea.height *= 0.5f;
			availableArea.width *= 0.9f;
			EditorGUILayout.BeginVertical();
			{
				EditorGUILayout.LabelField( "Shader Library", UIUtils.MainSkin.customStyles[ 5 ] );
				GUILayout.Space( 10 );
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Space( 0.05f * position.width );
					GUILayout.Button( string.Empty, UIUtils.MainSkin.customStyles[ 8 ] );
					GUILayout.Button( string.Empty, UIUtils.MainSkin.customStyles[ 9 ] );
					GUILayout.Space( 0.8f*position.width  );
					GUILayout.Button( string.Empty, UIUtils.MainSkin.customStyles[ 7 ] );
					GUILayout.Button( string.Empty, UIUtils.MainSkin.customStyles[ 6 ] );
				}
				EditorGUILayout.EndHorizontal();
				
				GUILayout.BeginArea( availableArea );
				m_scrollPos = EditorGUILayout.BeginScrollView( m_scrollPos, UIUtils.MainSkin.box );
				{
					for ( int i = 0; i < m_shaders.Count; i++ )
					{
						GUILayout.Button( m_shaders[ i ], UIUtils.MainSkin.customStyles[ 10 ] );
					}
				}
				EditorGUILayout.EndScrollView();
				GUILayout.EndArea();
			}
			EditorGUILayout.EndVertical();

		}
	}
}
