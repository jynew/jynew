// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	class ColorMaskHelper
	{
		private GUIContent ColorMaskContent = new GUIContent( "Color Mask", "Sets color channel writing mask, turning all off makes the object completely invisible\nDefault: RGBA" );
		private readonly char[] m_colorMaskChar = { 'R', 'G', 'B', 'A' };

		private GUIStyle m_leftToggleColorMask;
		private GUIStyle m_middleToggleColorMask;
		private GUIStyle m_rightToggleColorMask;


		[SerializeField]
		private bool[] m_colorMask = { true, true, true, true };

		[SerializeField]
		private InlineProperty m_inlineMask = new InlineProperty();

		public void Draw( UndoParentNode owner )
		{
			m_inlineMask.CustomDrawer( ref owner, DrawColorMaskControls, ColorMaskContent.text );
		}

		private void DrawColorMaskControls( UndoParentNode owner )
		{
			if( m_leftToggleColorMask == null || m_leftToggleColorMask.normal.background == null )
			{
				m_leftToggleColorMask = GUI.skin.GetStyle( "miniButtonLeft" );
			}

			if( m_middleToggleColorMask == null || m_middleToggleColorMask.normal.background == null )
			{
				m_middleToggleColorMask = GUI.skin.GetStyle( "miniButtonMid" );
			}

			if( m_rightToggleColorMask == null || m_rightToggleColorMask.normal.background == null )
			{
				m_rightToggleColorMask = GUI.skin.GetStyle( "miniButtonRight" );
			}


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField( ColorMaskContent, GUILayout.Width( 90 ) );

			m_colorMask[ 0 ] = owner.GUILayoutToggle( m_colorMask[ 0 ], "R", m_leftToggleColorMask );
			m_colorMask[ 1 ] = owner.GUILayoutToggle( m_colorMask[ 1 ], "G", m_middleToggleColorMask );
			m_colorMask[ 2 ] = owner.GUILayoutToggle( m_colorMask[ 2 ], "B", m_middleToggleColorMask );
			m_colorMask[ 3 ] = owner.GUILayoutToggle( m_colorMask[ 3 ], "A", m_rightToggleColorMask );

			EditorGUILayout.EndHorizontal();
		}

		public void BuildColorMask( ref string ShaderBody, bool customBlendAvailable )
		{
			int count = 0;
			string colorMask = string.Empty;
			for( int i = 0; i < m_colorMask.Length; i++ )
			{
				if( m_colorMask[ i ] )
				{
					count++;
					colorMask += m_colorMaskChar[ i ];
				}
			}

			if( ( count != m_colorMask.Length && customBlendAvailable ) || m_inlineMask.Active )
			{
				MasterNode.AddRenderState( ref ShaderBody, "ColorMask", m_inlineMask.GetValueOrProperty( ( ( count == 0 ) ? "0" : colorMask ) ) );
			}
		}

		public void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			for( int i = 0; i < m_colorMask.Length; i++ )
			{
				m_colorMask[ i ] = Convert.ToBoolean( nodeParams[ index++ ] );
			}

			if( UIUtils.CurrentShaderVersion() > 14501 )
				m_inlineMask.ReadFromString( ref index, ref nodeParams );
		}

		public void WriteToString( ref string nodeInfo )
		{
			for( int i = 0; i < m_colorMask.Length; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_colorMask[ i ] );
			}

			m_inlineMask.WriteToString( ref nodeInfo );
		}

		public void Destroy()
		{
			m_leftToggleColorMask = null;
			m_middleToggleColorMask = null;
			m_rightToggleColorMask = null;
		}
	}
}
