// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Grab Screen Position", "Camera And Screen", "Screen position correctly transformed to be used with Grab Screen Color" )]
	public sealed class GrabScreenPosition : ParentNode
	{
		private readonly string[] m_outputTypeStr = { "Normalized", "Screen" };

		[SerializeField]
		private int m_outputTypeInt = 0;

		private UpperLeftWidgetHelper m_upperLeftWidget = new UpperLeftWidgetHelper();

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputVectorPorts( WirePortDataType.FLOAT4, "XYZW" );
			m_autoWrapProperties = true;
			m_hasLeftDropdown = true;
			m_textLabelWidth = 65;
			ConfigureHeader();
		}

		public override void AfterCommonInit()
		{
			base.AfterCommonInit();

			if( PaddingTitleLeft == 0 )
			{
				PaddingTitleLeft = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
				if( PaddingTitleRight == 0 )
					PaddingTitleRight = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			m_upperLeftWidget = null;
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			EditorGUI.BeginChangeCheck();
			m_outputTypeInt = m_upperLeftWidget.DrawWidget( this, m_outputTypeInt, m_outputTypeStr );
			if( EditorGUI.EndChangeCheck() )
			{
				ConfigureHeader();
			}
		}

		public override void DrawProperties()
		{
			base.DrawProperties();

			EditorGUI.BeginChangeCheck();
			m_outputTypeInt = EditorGUILayoutPopup( "Type", m_outputTypeInt, m_outputTypeStr );
			if( EditorGUI.EndChangeCheck() )
			{
				ConfigureHeader();
			}
		}

		void ConfigureHeader()
		{
			SetAdditonalTitleText( string.Format( Constants.SubTitleTypeFormatStr, m_outputTypeStr[ m_outputTypeInt ] ) );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return GetOutputColorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );

			string localVarName = string.Empty;

			if( m_outputTypeInt == 0 )
				localVarName = GeneratorUtils.GenerateGrabScreenPositionNormalized( ref dataCollector, UniqueId, m_currentPrecisionType, !dataCollector.UsingCustomScreenPos );
			else
				localVarName = GeneratorUtils.GenerateGrabScreenPosition( ref dataCollector, UniqueId, m_currentPrecisionType, !dataCollector.UsingCustomScreenPos );

			m_outputPorts[ 0 ].SetLocalValue( localVarName, dataCollector.PortCategory );
			return GetOutputColorItem( 0, outputId, localVarName );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if( UIUtils.CurrentShaderVersion() > 3108 )
			{
				if( UIUtils.CurrentShaderVersion() < 6102 )
				{
					bool project = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
					m_outputTypeInt = project ? 0 : 1;
				}
				else
				{
					m_outputTypeInt = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				}
			}

			ConfigureHeader();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_outputTypeInt );
		}
	}
}
