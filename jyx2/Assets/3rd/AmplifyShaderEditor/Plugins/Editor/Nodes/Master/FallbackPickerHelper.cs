using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class FallbackPickerHelper : ScriptableObject
	{
		private const string FallbackFormat = "Fallback \"{0}\"";
		private const string FallbackShaderStr = "Fallback";
		private const string ShaderPoputContext = "CONTEXT/ShaderPopup";

		private Material m_dummyMaterial;
		private MenuCommand m_dummyCommand;

		[SerializeField]
		private string m_fallbackShader = string.Empty;

		public FallbackPickerHelper()
		{
			m_dummyMaterial = null;
			m_dummyCommand = null;
		}

		public void Draw( ParentNode owner )
		{
			EditorGUILayout.BeginHorizontal();
			m_fallbackShader = owner.EditorGUILayoutTextField( FallbackShaderStr, m_fallbackShader );
			if ( GUILayout.Button( string.Empty, UIUtils.InspectorPopdropdownFallback, GUILayout.Width( 17 ), GUILayout.Height( 19 ) ) )
			{
				EditorGUI.FocusTextInControl( null );
				GUI.FocusControl( null );
				DisplayShaderContext( owner, GUILayoutUtility.GetRect( GUIContent.none, EditorStyles.popup ) );
			}
			EditorGUILayout.EndHorizontal();
		}

		private void DisplayShaderContext( ParentNode node, Rect r )
		{
			if ( m_dummyCommand == null )
				m_dummyCommand = new MenuCommand( this, 0 );

			if ( m_dummyMaterial == null )
				m_dummyMaterial = new Material( Shader.Find( "Hidden/ASESShaderSelectorUnlit" ) );

#pragma warning disable 0618
			UnityEditorInternal.InternalEditorUtility.SetupShaderMenu( m_dummyMaterial );
#pragma warning restore 0618
			EditorUtility.DisplayPopupMenu( r, ShaderPoputContext, m_dummyCommand );
		}

		private void OnSelectedShaderPopup( string command, Shader shader )
		{
			if ( shader != null )
			{
				UIUtils.MarkUndoAction();
				Undo.RecordObject( this, "Selected fallback shader" );
				m_fallbackShader = shader.name;
			}
		}
		
		public void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			m_fallbackShader = nodeParams[ index++ ];
		}

		public void WriteToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_fallbackShader );
		}

		public void Destroy()
		{
			GameObject.DestroyImmediate( m_dummyMaterial );
			m_dummyMaterial = null;
			m_dummyCommand = null;
		}

		public string TabbedFallbackShader
		{
			get
			{
				if( string.IsNullOrEmpty( m_fallbackShader ) )
					return string.Empty;

				return "\t" + string.Format( FallbackFormat, m_fallbackShader ) + "\n";
			}
		}

		public string FallbackShader
		{
			get
			{
				if( string.IsNullOrEmpty( m_fallbackShader ) )
					return string.Empty;

				return string.Format( FallbackFormat, m_fallbackShader );
			}
		}

		public string RawFallbackShader
		{
			get
			{
				return m_fallbackShader;
			}
			set
			{
				m_fallbackShader = value;
			}
		}


		public bool Active { get { return !string.IsNullOrEmpty( m_fallbackShader ); } }

	}
}
