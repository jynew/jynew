// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Clamp", "Math Operators", "Value clamped to the range [min,max]" )]
	public sealed class ClampOpNode : ParentNode
	{

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, Constants.EmptyPortValue );
			AddInputPort( WirePortDataType.FLOAT, false, "Min" );
			AddInputPort( WirePortDataType.FLOAT, false, "Max" );
			m_inputPorts[ m_inputPorts.Count - 1 ].FloatInternalData = 1;
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_useInternalPortData = true;
			m_textLabelWidth = 55;
			m_previewShaderGUID = "ab6163c4b10bfc84da8e3c486520490a";
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			if ( portId == 0 )
			{
				m_inputPorts[ 0 ].MatchPortToConnection();
				m_inputPorts[ 1 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
				m_inputPorts[ 2 ].ChangeType( m_inputPorts[ 0 ].DataType, false );

				m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
			}
			//else
			//{
			//	_inputPorts[ portId ].MatchPortToConnection();
			//}
		}

		public override void OnConnectedOutputNodeChanges( int outputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( outputPortId, otherNodeId, otherPortId, name, type );
			if ( outputPortId == 0 )
			{
				m_inputPorts[ 0 ].MatchPortToConnection();
				m_inputPorts[ 1 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
				m_inputPorts[ 2 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
				m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			WirePortDataType valueType = m_inputPorts[ 0 ].ConnectionType();
			WirePortDataType minType = m_inputPorts[ 1 ].ConnectionType();
			WirePortDataType maxType = m_inputPorts[ 2 ].ConnectionType();

			string value = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string min = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );
			if ( minType != valueType )
			{
				min = UIUtils.CastPortType( ref dataCollector, m_currentPrecisionType, new NodeCastInfo( UniqueId, outputId ), null, m_inputPorts[ 1 ].DataType, m_inputPorts[ 0 ].DataType, min );
			}

			string max = m_inputPorts[ 2 ].GeneratePortInstructions( ref dataCollector );
			if ( maxType != valueType )
			{
				max = UIUtils.CastPortType( ref dataCollector, m_currentPrecisionType, new NodeCastInfo( UniqueId, outputId ), null, m_inputPorts[ 2 ].DataType, m_inputPorts[ 0 ].DataType, max );
			}

			string result = string.Empty;
			switch ( valueType )
			{
				case WirePortDataType.FLOAT:
				case WirePortDataType.FLOAT2:
				case WirePortDataType.FLOAT3:
				case WirePortDataType.FLOAT4:
				case WirePortDataType.INT:
				case WirePortDataType.COLOR:
				case WirePortDataType.OBJECT:
				{
					result = "clamp( " + value + " , " + min + " , " + max + " )";
				}
				break;
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{
					return UIUtils.InvalidParameter( this );
				}
			}

			RegisterLocalVariable( 0, result, ref dataCollector, "clampResult" + OutputId );
			return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
		}

	}
}
