using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{

	[Serializable]
	public class AdditionalSurfaceOptionsHelper
	{
		private const string AdditionalOptionsStr = " Additional Surface Options";
		

		private const float ShaderKeywordButtonLayoutWidth = 15;
		private ParentNode m_currentOwner;

		[SerializeField]
		private List<string> m_availableOptions = new List<string>();

		public void Draw( ParentNode owner )
		{
			m_currentOwner = owner;
			bool value = owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedCustomTags;
			NodeUtils.DrawPropertyGroup( ref value, AdditionalOptionsStr, DrawMainBody, DrawButtons );
			owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedCustomTags = value;
		}

		void DrawButtons()
		{
			EditorGUILayout.Separator();

			// Add tag
			if( GUILayout.Button( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
			{
				m_availableOptions.Add( string.Empty );
				EditorGUI.FocusTextInControl( null );
			}

			//Remove tag
			if( GUILayout.Button( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
			{
				if( m_availableOptions.Count > 0 )
				{
					m_availableOptions.RemoveAt( m_availableOptions.Count - 1 );
					EditorGUI.FocusTextInControl( null );
				}
			}
		}

		void DrawMainBody()
		{
			EditorGUILayout.Separator();
			int itemCount = m_availableOptions.Count;

			if( itemCount == 0 )
			{
				EditorGUILayout.HelpBox( "Your list is Empty!\nUse the plus button to add one.", MessageType.Info );
			}

			int markedToDelete = -1;
			float originalLabelWidth = EditorGUIUtility.labelWidth;
			for( int i = 0; i < itemCount; i++ )
			{

				EditorGUI.indentLevel += 1;
				EditorGUIUtility.labelWidth = 62;
				EditorGUILayout.BeginHorizontal();
				//Option
				EditorGUI.BeginChangeCheck();
				m_availableOptions[ i ] = EditorGUILayout.TextField( "["+i+"] -", m_availableOptions[ i ] );
				if( EditorGUI.EndChangeCheck() )
				{
					m_availableOptions[ i ] = UIUtils.RemoveShaderInvalidCharacters( m_availableOptions[ i ] );
				}
			
				EditorGUIUtility.labelWidth = originalLabelWidth;
				
				{
					// Add new port
					if( m_currentOwner.GUILayoutButton( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
					{
						m_availableOptions.Insert( i + 1, string.Empty );
						EditorGUI.FocusTextInControl( null );
					}

					//Remove port
					if( m_currentOwner.GUILayoutButton( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
					{
						markedToDelete = i;
					}
				}
				EditorGUILayout.EndHorizontal();
				EditorGUI.indentLevel -= 1;
			}

			if( markedToDelete > -1 )
			{
				if( m_availableOptions.Count > markedToDelete )
				{
					m_availableOptions.RemoveAt( markedToDelete );
					EditorGUI.FocusTextInControl( null );
				}
			}
			EditorGUILayout.Separator();
		}
		
		public void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			int count = Convert.ToInt32( nodeParams[ index++ ] );
			for( int i = 0; i < count; i++ )
			{
				m_availableOptions.Add( nodeParams[ index++ ] );
			}
		}

		public void WriteToString( ref string nodeInfo )
		{
			int optionsCount = m_availableOptions.Count;
			IOUtils.AddFieldValueToString( ref nodeInfo, optionsCount );
			for( int i = 0; i < optionsCount; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_availableOptions[ i ].ToString() );
			}
		}

		public void WriteToOptionalSurfaceOptions( ref string currentOptions)
		{
			int tagsCount = m_availableOptions.Count;
			if( tagsCount == 0 )
				return;

			string result = " ";

			for( int i = 0; i < tagsCount; i++ )
			{
				result += m_availableOptions[ i ];
				if( i < tagsCount - 1 )
				{
					result += " ";
				}
			}
			currentOptions = currentOptions +  result;
		}

		public void Destroy()
		{
			m_availableOptions.Clear();
			m_availableOptions = null;
			m_currentOwner = null;
		}
	}
}
