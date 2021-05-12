// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class TemplateColorMaskModule : TemplateModuleParent
	{
		private const string ColorMaskOp = "ColorMask ";
		private const string ColorMaskOff = "ColorMask RGBA";
		private GUIContent ColorMaskContent = new GUIContent( "Color Mask", "Sets color channel writing mask, turning all off makes the object completely invisible\nDefault: RGBA" );
		private readonly char[] m_colorMaskChar = { 'R', 'G', 'B', 'A' };

		private GUIStyle m_leftToggleColorMask;
		private GUIStyle m_middleToggleColorMask;
		private GUIStyle m_rightToggleColorMask;

		public TemplateColorMaskModule() : base( "Color Mask" ) { }

		[SerializeField]
		private bool[] m_colorMask = { true, true, true, true };

		[SerializeField]
		private InlineProperty m_inlineColorMask = new InlineProperty();

		public void CopyFrom( TemplateColorMaskModule other, bool allData )
		{
			if( allData )
				m_independentModule = other.IndependentModule;

			for( int i = 0; i < m_colorMask.Length; i++ )
			{
				m_colorMask[ i ] = other.ColorMask[ i ];
			}
			m_inlineColorMask.CopyFrom( other.InlineColorMask );
		}

		public void ConfigureFromTemplateData( TemplateColorMaskData data )
		{
			bool newValidData = ( data.DataCheck == TemplateDataCheck.Valid );
			if( newValidData && m_validData != newValidData )
			{
				m_independentModule = data.IndependentModule;
				if( string.IsNullOrEmpty( data.InlineData ) )
				{
					for( int i = 0; i < 4; i++ )
					{
						m_colorMask[ i ] = data.ColorMaskData[ i ];
					}
					m_inlineColorMask.ResetProperty();
				}
				else
				{
					m_inlineColorMask.SetInlineByName( data.InlineData );
				}
			}
			
			m_validData = newValidData;
		}

		public override void Draw( UndoParentNode owner, bool style = true )
		{
			EditorGUI.BeginChangeCheck();
			{
				m_inlineColorMask.CustomDrawer( ref owner, DrawColorMaskControls, ColorMaskContent.text );
			}

			if( EditorGUI.EndChangeCheck() )
			{
				m_isDirty = true;
			}
		}

		private void DrawColorMaskControls( UndoParentNode owner )
		{
			if( m_leftToggleColorMask == null || m_leftToggleColorMask.normal.background == null )
			{
				m_leftToggleColorMask = GUI.skin.GetStyle( "ButtonLeft" );
			}

			if( m_middleToggleColorMask == null || m_middleToggleColorMask.normal.background == null )
			{
				m_middleToggleColorMask = GUI.skin.GetStyle( "ButtonMid" );
			}

			if( m_rightToggleColorMask == null || m_rightToggleColorMask.normal.background == null )
			{
				m_rightToggleColorMask = GUI.skin.GetStyle( "ButtonRight" );
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField( ColorMaskContent, GUILayout.Width( 90 ) );

			m_colorMask[ 0 ] = owner.GUILayoutToggle( m_colorMask[ 0 ], "R", m_leftToggleColorMask );
			m_colorMask[ 1 ] = owner.GUILayoutToggle( m_colorMask[ 1 ], "G", m_middleToggleColorMask );
			m_colorMask[ 2 ] = owner.GUILayoutToggle( m_colorMask[ 2 ], "B", m_middleToggleColorMask );
			m_colorMask[ 3 ] = owner.GUILayoutToggle( m_colorMask[ 3 ], "A", m_rightToggleColorMask );

			EditorGUILayout.EndHorizontal();
		}

		public override string GenerateShaderData( bool isSubShader )
		{
			if( m_inlineColorMask.IsValid )
				return ColorMaskOp + m_inlineColorMask.GetValueOrProperty();

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

			if( count != m_colorMask.Length )
			{
				return ColorMaskOp + ( ( count == 0 ) ? "0" : colorMask );
			}

			return ColorMaskOff;
		}

		public override void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			bool validDataOnMeta = m_validData;
			if( UIUtils.CurrentShaderVersion() > TemplatesManager.MPShaderVersion )
			{
				validDataOnMeta = Convert.ToBoolean( nodeParams[ index++ ] );
			}

			if( validDataOnMeta )
			{
				for( int i = 0; i < m_colorMask.Length; i++ )
				{
					m_colorMask[ i ] = Convert.ToBoolean( nodeParams[ index++ ] );
				}

				if( UIUtils.CurrentShaderVersion() > 15303 )
				{
					m_inlineColorMask.ReadFromString( ref index, ref nodeParams );
				}
			}
		}

		public override void WriteToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_validData );
			if( m_validData )
			{
				for( int i = 0; i < m_colorMask.Length; i++ )
				{
					IOUtils.AddFieldValueToString( ref nodeInfo, m_colorMask[ i ] );
				}
				m_inlineColorMask.WriteToString( ref nodeInfo );
			}
		}

		public bool[] ColorMask { get { return m_colorMask; } }

		public override void Destroy()
		{
			m_leftToggleColorMask = null;
			m_middleToggleColorMask = null;
			m_rightToggleColorMask = null;
			m_inlineColorMask = null;
		}

		public InlineProperty InlineColorMask { get { return m_inlineColorMask; } }
	}
}
