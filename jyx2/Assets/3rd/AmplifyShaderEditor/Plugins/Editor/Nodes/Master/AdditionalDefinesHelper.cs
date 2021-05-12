// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class AdditionalDefinesHelper
	{
		private const string AdditionalDefinesStr = " Additional Defines";
		private const float ShaderKeywordButtonLayoutWidth = 15;
		private ParentNode m_currentOwner;

		[SerializeField]
		private List<string> m_additionalDefines = new List<string>();
		public List<string> DefineList { get { return m_additionalDefines; } set { m_additionalDefines = value; } }

		[SerializeField]
		private List<string> m_outsideDefines = new List<string>();
		public List<string> OutsideList { get { return m_outsideDefines; } set { m_outsideDefines = value; } }

		public void Draw( ParentNode owner )
		{
			m_currentOwner = owner;
			bool value = owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedAdditionalDefines;
			NodeUtils.DrawPropertyGroup( ref value, AdditionalDefinesStr, DrawMainBody, DrawButtons );
			owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedAdditionalDefines = value;
		}

		void DrawButtons()
		{
			EditorGUILayout.Separator();

			// Add keyword
			if( GUILayout.Button( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
			{
				m_additionalDefines.Add( string.Empty );
				EditorGUI.FocusTextInControl( null );
			}

			//Remove keyword
			if( GUILayout.Button( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
			{
				if( m_additionalDefines.Count > 0 )
				{
					m_additionalDefines.RemoveAt( m_additionalDefines.Count - 1 );
					EditorGUI.FocusTextInControl( null );
				}
			}
		}

		void DrawMainBody()
		{
			EditorGUILayout.Separator();
			int itemCount = m_additionalDefines.Count;
			int markedToDelete = -1;
			for( int i = 0; i < itemCount; i++ )
			{
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUI.BeginChangeCheck();
					m_additionalDefines[ i ] = EditorGUILayout.TextField( m_additionalDefines[ i ] );
					if( EditorGUI.EndChangeCheck() )
					{
						m_additionalDefines[ i ] = UIUtils.RemoveShaderInvalidCharacters( m_additionalDefines[ i ] );
					}

					// Add new port
					if( m_currentOwner.GUILayoutButton( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
					{
						m_additionalDefines.Insert( i + 1, string.Empty );
						EditorGUI.FocusTextInControl( null );
					}

					//Remove port
					if( m_currentOwner.GUILayoutButton( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
					{
						markedToDelete = i;
					}
				}
				EditorGUILayout.EndHorizontal();
			}

			if( markedToDelete > -1 )
			{
				if( m_additionalDefines.Count > markedToDelete )
				{
					m_additionalDefines.RemoveAt( markedToDelete );
					EditorGUI.FocusTextInControl( null );
				}
			}
			EditorGUILayout.Separator();
			EditorGUILayout.HelpBox( "Please add your defines without the #define keywords", MessageType.Info );
		}

		public void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			int count = Convert.ToInt32( nodeParams[ index++ ] );
			for( int i = 0; i < count; i++ )
			{
				m_additionalDefines.Add( nodeParams[ index++ ] );
			}
		}

		public void WriteToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_additionalDefines.Count );
			for( int i = 0; i < m_additionalDefines.Count; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_additionalDefines[ i ] );
			}
		}

		public void AddToDataCollector( ref MasterNodeDataCollector dataCollector )
		{
			for( int i = 0; i < m_additionalDefines.Count; i++ )
			{
				if( !string.IsNullOrEmpty( m_additionalDefines[ i ] ) )
					dataCollector.AddToDefines( -1, m_additionalDefines[ i ] );
			}

			for( int i = 0; i < m_outsideDefines.Count; i++ )
			{
				if( !string.IsNullOrEmpty( m_outsideDefines[ i ] ) )
					dataCollector.AddToDefines( -1, m_outsideDefines[ i ] );
			}
		}

		public void Destroy()
		{
			m_additionalDefines.Clear();
			m_additionalDefines = null;
			m_currentOwner = null;
		}
	}
}
