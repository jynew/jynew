// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Screen Position", "Camera And Screen", "Screen space position, you can either get the <b>Screen</b> position as is or <b>Normalize</b> it to have it at the [0,1] range" )]
	public sealed class ScreenPosInputsNode : SurfaceShaderINParentNode
	{
		private const string ProjectStr = "Project";
		private const string UVInvertHack = "Scale and Offset";
		private readonly string[] m_outputTypeStr = { "Normalized", "Screen" };

		[SerializeField]
		private int m_outputTypeInt = 0;

		[SerializeField]
		private bool m_scaleAndOffset = false;

		private UpperLeftWidgetHelper m_upperLeftWidget = new UpperLeftWidgetHelper();

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentInput = SurfaceInputs.SCREEN_POS;
			InitialSetup();
			m_textLabelWidth = 65;
			m_autoWrapProperties = true;

			m_hasLeftDropdown = true;
			m_previewShaderGUID = "a5e7295278a404175b732f1516fb68a6";

			if( UIUtils.CurrentWindow != null && UIUtils.CurrentWindow.CurrentGraph != null && UIUtils.CurrentShaderVersion() <= 2400 )
				m_outputTypeInt = 1;

			ConfigureHeader();
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
			//base.DrawProperties();

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

		public override void Reset()
		{
			base.Reset();
		}

		public override void Destroy()
		{
			base.Destroy();
			m_upperLeftWidget = null;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
			{
				return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );
			}

			if( dataCollector.IsFragmentCategory && !dataCollector.UsingCustomScreenPos )
				base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalVar );

			string screenPos = string.Empty;
			if( m_outputTypeInt == 0 )
			{
				if( dataCollector.IsTemplate )
				{
					screenPos = dataCollector.TemplateDataCollectorInstance.GetScreenPosNormalized();
				}
				else
				{
					screenPos = GeneratorUtils.GenerateScreenPositionNormalized( ref dataCollector, UniqueId, m_currentPrecisionType, false );
				}
			}
			else
			{
				if( dataCollector.IsTemplate )
				{
					screenPos = dataCollector.TemplateDataCollectorInstance.GetScreenPos();
				}
				else
				{
					screenPos = GeneratorUtils.GenerateScreenPosition( ref dataCollector, UniqueId, m_currentPrecisionType, false );
				}
			}
			
			

			m_outputPorts[ 0 ].SetLocalValue( screenPos, dataCollector.PortCategory );
			return GetOutputVectorItem( 0, outputId, screenPos );

		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if( UIUtils.CurrentShaderVersion() > 2400 )
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

			if( UIUtils.CurrentShaderVersion() > 3107 )
			{
				m_scaleAndOffset = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
				m_scaleAndOffset = false;
			}

			ConfigureHeader();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_outputTypeInt );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_scaleAndOffset );
		}
	}
}
