// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Camera Depth Fade", "Camera And Screen", "Outputs a 0 - 1 gradient representing the distance between the surface of this object and camera near plane" )]
	public sealed class CameraDepthFade : ParentNode
	{
		//{0} - Eye Depth
		//{1} - Offset
		//{2} - Distance
		private const string CameraDepthFadeFormat = "(( {0} -_ProjectionParams.y - {1} ) / {2})";

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "Vertex Position", -1, MasterNodePortCategory.Fragment, 2 );
			AddInputPort( WirePortDataType.FLOAT, false, "Length", -1, MasterNodePortCategory.Fragment, 0 );
			AddInputPort( WirePortDataType.FLOAT, false, "Offset", -1, MasterNodePortCategory.Fragment, 1 );
			GetInputPortByUniqueId( 0 ).FloatInternalData = 1;
			AddOutputPort( WirePortDataType.FLOAT, "Out" );
			m_useInternalPortData = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			InputPort vertexPort = GetInputPortByUniqueId( 2 );
			InputPort lengthPort = GetInputPortByUniqueId( 0 );
			InputPort offsetPort = GetInputPortByUniqueId( 1 );

			string distance = lengthPort.GeneratePortInstructions( ref dataCollector );
			string offset = offsetPort.GeneratePortInstructions( ref dataCollector );

			string value = string.Empty;
			string eyeDepth = string.Empty;

			if( dataCollector.IsTemplate )
			{
				if( vertexPort.IsConnected )
				{
					string varName = "customSurfaceDepth" + OutputId;
					GenerateInputInVertex( ref dataCollector, 2, varName, false );

					string formatStr = string.Empty;
					if( dataCollector.IsSRP )
						formatStr = "-TransformWorldToView(TransformObjectToWorld({0})).z";
					else
						formatStr = "-UnityObjectToViewPos({0}).z";

					string eyeInstruction = string.Format( formatStr, varName );
					eyeDepth = "customEye" + OutputId;
					dataCollector.TemplateDataCollectorInstance.RegisterCustomInterpolatedData( eyeDepth, WirePortDataType.FLOAT, m_currentPrecisionType, eyeInstruction );
				}
				else
				{
					eyeDepth = dataCollector.TemplateDataCollectorInstance.GetEyeDepth( m_currentPrecisionType );
				}

				value = string.Format( CameraDepthFadeFormat, eyeDepth, offset, distance );
				RegisterLocalVariable( 0, value, ref dataCollector, "cameraDepthFade" + OutputId );
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
			}

			if( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				string vertexVarName = string.Empty;
				if( vertexPort.IsConnected )
				{
					vertexVarName = vertexPort.GeneratePortInstructions( ref dataCollector );
				}
				else
				{
					vertexVarName = Constants.VertexShaderInputStr + ".vertex.xyz";
				}

				//dataCollector.AddVertexInstruction( "float cameraDepthFade" + UniqueId + " = (( -UnityObjectToViewPos( " + Constants.VertexShaderInputStr + ".vertex.xyz ).z -_ProjectionParams.y - " + offset + " ) / " + distance + ");", UniqueId );
				value = string.Format( CameraDepthFadeFormat, "-UnityObjectToViewPos( " + vertexVarName + " ).z", offset, distance );
				RegisterLocalVariable( 0, value, ref dataCollector, "cameraDepthFade" + OutputId );
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
			}

			dataCollector.AddToIncludes( UniqueId, Constants.UnityShaderVariables );

			if( dataCollector.TesselationActive )
			{
				if( vertexPort.IsConnected )
				{
					string vertexValue = vertexPort.GeneratePortInstructions( ref dataCollector );
					eyeDepth = "customSurfaceDepth" + OutputId;
					RegisterLocalVariable( 0, string.Format( "-UnityObjectToViewPos( {0} ).z", vertexValue ), ref dataCollector, eyeDepth );
				}
				else
				{
					eyeDepth = GeneratorUtils.GenerateScreenDepthOnFrag( ref dataCollector, UniqueId, m_currentPrecisionType );
				}
			}
			else
			{

				if( vertexPort.IsConnected )
				{
					string varName = "customSurfaceDepth" + OutputId;
					GenerateInputInVertex( ref dataCollector, 2, varName, false );
					dataCollector.AddToInput( UniqueId, varName, WirePortDataType.FLOAT );
					string vertexInstruction = "-UnityObjectToViewPos( " + varName + " ).z";
					dataCollector.AddToVertexLocalVariables( UniqueId, Constants.VertexShaderOutputStr + "." + varName + " = " + vertexInstruction + ";" );
					eyeDepth = Constants.InputVarStr + "." + varName;
				}
				else
				{
					dataCollector.AddToInput( UniqueId, "eyeDepth", WirePortDataType.FLOAT );
					string instruction = "-UnityObjectToViewPos( " + Constants.VertexShaderInputStr + ".vertex.xyz ).z";
					dataCollector.AddToVertexLocalVariables( UniqueId, Constants.VertexShaderOutputStr + ".eyeDepth = " + instruction + ";" );
					eyeDepth = Constants.InputVarStr + ".eyeDepth";
				}
			}

			value = string.Format( CameraDepthFadeFormat, eyeDepth, offset, distance );
			RegisterLocalVariable( 0, value, ref dataCollector, "cameraDepthFade" + OutputId );
			//dataCollector.AddToLocalVariables( UniqueId, "float cameraDepthFade" + UniqueId + " = (( " + Constants.InputVarStr + ".eyeDepth -_ProjectionParams.y - "+ offset + " ) / " + distance + ");" );

			return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
		}
	}
}
