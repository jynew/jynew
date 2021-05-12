// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	public sealed class TemplateCullModeModule : TemplateModuleParent
	{
		private const string CullModeFormatStr = "Cull ";

		public TemplateCullModeModule() : base("Cull Mode"){ }

        private static readonly string CullModeStr = "Cull Mode";

		[SerializeField]
		private CullMode m_cullMode = CullMode.Back;

		[SerializeField]
		private InlineProperty m_inlineCullMode = new InlineProperty();
		
		public void CopyFrom( TemplateCullModeModule other , bool allData )
		{
			if( allData )
				m_independentModule = other.IndependentModule;

			m_cullMode = other.CurrentCullMode;
			m_inlineCullMode.CopyFrom( other.CullInlineProperty );
		}

		public void ConfigureFromTemplateData( TemplateCullModeData data )
		{
			bool newValidData = ( data.DataCheck == TemplateDataCheck.Valid );

			if( newValidData && m_validData != newValidData )
			{
				m_independentModule = data.IndependentModule;
				if( string.IsNullOrEmpty( data.InlineData ) )
				{
					m_cullMode = data.CullModeData;
					m_inlineCullMode.IntValue = (int)m_cullMode;
					m_inlineCullMode.ResetProperty();
				}
				else
				{
					m_inlineCullMode.SetInlineByName( data.InlineData );
				}
			}

			m_validData = newValidData;
		}

		public override void Draw( UndoParentNode owner, bool style = true )
		{
			EditorGUI.BeginChangeCheck();
			//m_cullMode = (CullMode)owner.EditorGUILayoutEnumPopup( CullModeStr, m_cullMode );
			m_inlineCullMode.CustomDrawer( ref owner, ( x ) => { m_cullMode = (CullMode)owner.EditorGUILayoutEnumPopup( CullModeStr, m_cullMode ); }, CullModeStr );
			if( EditorGUI.EndChangeCheck() )
			{
				m_inlineCullMode.IntValue = (int)m_cullMode;
				m_isDirty = true;
			}
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
				if( UIUtils.CurrentShaderVersion() < 15304 )
				{
					m_cullMode = (CullMode)Enum.Parse( typeof( CullMode ), nodeParams[ index++ ] );
					m_inlineCullMode.IntValue = (int)m_cullMode;
				}
				else
				{
					m_inlineCullMode.ReadFromString( ref index, ref nodeParams );
					m_cullMode = (CullMode)m_inlineCullMode.IntValue;
				}
			}
		}

		public override void WriteToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_validData );
			if( m_validData )
			{
				//IOUtils.AddFieldValueToString( ref nodeInfo, m_cullMode );
				m_inlineCullMode.WriteToString( ref nodeInfo );
			}
		}

		public override string GenerateShaderData( bool isSubShader )
		{
			//return CullModeFormatStr + m_cullMode.ToString();
			return CullModeFormatStr + m_inlineCullMode.GetValueOrProperty( m_cullMode.ToString());
		}
		
		public override void Destroy()
		{
			base.Destroy();
			m_inlineCullMode = null;
		}

		public CullMode CurrentCullMode { get { return m_cullMode; } }
		public InlineProperty CullInlineProperty { get { return m_inlineCullMode; } }
	}
}
