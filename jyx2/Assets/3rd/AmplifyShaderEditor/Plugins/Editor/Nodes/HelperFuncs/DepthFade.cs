// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Depth Fade", "Surface Data", "Outputs a linear gradient representing the distance between the surface of this object and geometry behind" )]
	public sealed class DepthFade : ParentNode
	{
		private const string ConvertToLinearStr = "Convert To Linear";
		private const string SaturateStr = "Saturate";
		private const string MirrorStr = "Mirror";

		[SerializeField]
		private bool m_convertToLinear = true;

		[SerializeField]
		private bool m_saturate = false;

		[SerializeField]
		private bool m_mirror = true;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "Vertex Position", -1, MasterNodePortCategory.Fragment, 1 );
			AddInputPort( WirePortDataType.FLOAT, false, "Distance",-1,MasterNodePortCategory.Fragment,0 );
			GetInputPortByUniqueId(0).FloatInternalData = 1;
			AddOutputPort( WirePortDataType.FLOAT, "Out" );
			m_useInternalPortData = true;
			m_autoWrapProperties = true;
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
			string screenPosNorm = string.Empty;
			InputPort vertexPosPort = GetInputPortByUniqueId( 1 );
			if( vertexPosPort.IsConnected )
			{
				string vertexPosVar = "vertexPos" + OutputId;
				GenerateInputInVertex( ref dataCollector, 1, vertexPosVar, false );
				screenPos = GeneratorUtils.GenerateScreenPositionForValue( vertexPosVar,OutputId, ref dataCollector, UniqueId, m_currentPrecisionType, !dataCollector.UsingCustomScreenPos );
				screenPosNorm = GeneratorUtils.GenerateScreenPositionNormalizedForValue( vertexPosVar, OutputId, ref dataCollector, UniqueId, m_currentPrecisionType, !dataCollector.UsingCustomScreenPos );
			}
			else
			{
				screenPos = GeneratorUtils.GenerateScreenPosition( ref dataCollector, UniqueId, m_currentPrecisionType, !dataCollector.UsingCustomScreenPos );
				screenPosNorm = GeneratorUtils.GenerateScreenPositionNormalized( ref dataCollector, UniqueId, m_currentPrecisionType, !dataCollector.UsingCustomScreenPos );
			}

			string screenDepth = TemplateHelperFunctions.CreateDepthFetch( dataCollector, screenPos );
			if( m_convertToLinear )
			{
				if( dataCollector.IsTemplate && dataCollector.IsSRP )
					screenDepth = string.Format( "LinearEyeDepth({0},_ZBufferParams)", screenDepth );
				else
					screenDepth = string.Format( "LinearEyeDepth({0})", screenDepth );
			}
			else
			{
				screenDepth = string.Format( "({0}*( _ProjectionParams.z - _ProjectionParams.y ))", screenDepth );
			}

			string distance = GetInputPortByUniqueId( 0 ).GeneratePortInstructions( ref dataCollector );

			dataCollector.AddLocalVariable( UniqueId, "float screenDepth" + OutputId + " = " + screenDepth + ";" );

			string finalVarName = "distanceDepth" + OutputId;
			string finalVarValue = string.Empty;
			if( dataCollector.IsTemplate && dataCollector.IsSRP )
				finalVarValue  = "( screenDepth" + OutputId + " - LinearEyeDepth( " + screenPosNorm + ".z,_ZBufferParams ) ) / ( " + distance + " )";
			else
				finalVarValue =  "( screenDepth" + OutputId + " - LinearEyeDepth( " + screenPosNorm + ".z ) ) / ( " + distance + " )";

			if( m_mirror )
			{
				finalVarValue = string.Format( "abs( {0} )", finalVarValue );
			}

			if( m_saturate )
			{
				finalVarValue = string.Format( "saturate( {0} )", finalVarValue );
			}

			dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT, finalVarName, finalVarValue );
			m_outputPorts[ 0 ].SetLocalValue( finalVarName, dataCollector.PortCategory );
			return GetOutputColorItem( 0, outputId, finalVarName );
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			m_convertToLinear = EditorGUILayoutToggle( ConvertToLinearStr, m_convertToLinear );
			m_mirror = EditorGUILayoutToggle( MirrorStr, m_mirror );
			m_saturate = EditorGUILayoutToggle( SaturateStr, m_saturate );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if( UIUtils.CurrentShaderVersion() >= 13901 )
			{
				m_convertToLinear = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			}
			if( UIUtils.CurrentShaderVersion() > 15607 )
			{
				m_saturate = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			}

			if( UIUtils.CurrentShaderVersion() > 15700 )
			{
				m_mirror = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_convertToLinear );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_saturate );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_mirror );
		}
	}
}
