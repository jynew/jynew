// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Screen Depth", "Camera And Screen", "Given a screen position returns the depth of the scene to the object as seen by the camera" )]
	public sealed class ScreenDepthNode : ParentNode
	{
		[SerializeField]
		private bool m_convertToLinear = true;

		[SerializeField]
		private int m_viewSpaceInt = 0;

		private const string ConvertToLinearStr = "Convert To Linear";

		private readonly string[] m_viewSpaceStr = { "Eye Space", "0-1 Space" };

		private readonly string[] m_vertexNameStr = { "eyeDepth", "clampDepth" };

		private UpperLeftWidgetHelper m_upperLeftWidget = new UpperLeftWidgetHelper();

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT4, false, "Pos" );
			AddOutputPort( WirePortDataType.FLOAT, "Depth" );
			m_autoWrapProperties = true;
			m_hasLeftDropdown = true;
			SetAdditonalTitleText( string.Format( Constants.SubTitleSpaceFormatStr, m_viewSpaceStr[ m_viewSpaceInt ] ) );
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
			m_viewSpaceInt = m_upperLeftWidget.DrawWidget( this, m_viewSpaceInt, m_viewSpaceStr );
			if( EditorGUI.EndChangeCheck() )
			{
				SetAdditonalTitleText( string.Format( Constants.SubTitleSpaceFormatStr, m_viewSpaceStr[ m_viewSpaceInt ] ) );
			}
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUI.BeginChangeCheck();
			m_viewSpaceInt = EditorGUILayoutPopup( "View Space", m_viewSpaceInt, m_viewSpaceStr );
			if( EditorGUI.EndChangeCheck() )
			{
				SetAdditonalTitleText( string.Format( Constants.SubTitleSpaceFormatStr, m_viewSpaceStr[ m_viewSpaceInt ] ) );
			}

			m_convertToLinear = EditorGUILayoutToggle( ConvertToLinearStr, m_convertToLinear );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				UIUtils.ShowNoVertexModeNodeMessage( this );
				return "0";
			}

			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return GetOutputColorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );

			if( !( dataCollector.IsTemplate && dataCollector.IsSRP ) )
				dataCollector.AddToIncludes( UniqueId, Constants.UnityCgLibFuncs );

			if( !dataCollector.IsTemplate || dataCollector.TemplateDataCollectorInstance.CurrentSRPType != TemplateSRPType.HD )
				dataCollector.AddToUniforms( UniqueId, "uniform sampler2D _CameraDepthTexture;" );

			string screenPos = string.Empty;
			if( m_inputPorts[ 0 ].IsConnected )
				screenPos = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			else
				screenPos = GeneratorUtils.GenerateScreenPosition( ref dataCollector, UniqueId, m_currentPrecisionType, !dataCollector.UsingCustomScreenPos );

			string screenDepthInstruction = TemplateHelperFunctions.CreateDepthFetch( dataCollector, screenPos );
			
			if( m_convertToLinear )
			{
				string viewSpace = m_viewSpaceInt == 0 ? "LinearEyeDepth" : "Linear01Depth";
				string formatStr = string.Empty;
				if( ( dataCollector.IsTemplate && dataCollector.IsSRP ) )
					formatStr = "(" + screenDepthInstruction + ",_ZBufferParams)";
				else
					formatStr = "(" + screenDepthInstruction + ")";
				screenDepthInstruction = viewSpace + formatStr;
			}
			else
			{
				if( m_viewSpaceInt == 0 )
				{
					screenDepthInstruction = string.Format( "({0}*( _ProjectionParams.z - _ProjectionParams.y ))", screenDepthInstruction );
				}
			}

			dataCollector.AddToLocalVariables( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT, m_vertexNameStr[ m_viewSpaceInt ] + OutputId, screenDepthInstruction );

			m_outputPorts[ 0 ].SetLocalValue( m_vertexNameStr[ m_viewSpaceInt ] + OutputId, dataCollector.PortCategory );
			return GetOutputColorItem( 0, outputId, m_vertexNameStr[ m_viewSpaceInt ] + OutputId );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_viewSpaceInt = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			if( UIUtils.CurrentShaderVersion() >= 13901 )
			{
				m_convertToLinear = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			}

			SetAdditonalTitleText( string.Format( Constants.SubTitleSpaceFormatStr, m_viewSpaceStr[ m_viewSpaceInt ] ) );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_viewSpaceInt );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_convertToLinear );
		}
	}

}
