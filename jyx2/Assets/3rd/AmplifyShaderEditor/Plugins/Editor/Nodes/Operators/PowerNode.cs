// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Power", "Math Operators", "Base to the Exp-th power of scalars and vectors", null, KeyCode.E )]
	public sealed class PowerNode : ParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, "Base" );
			AddInputPort( WirePortDataType.FLOAT, false, "Exp" );
			m_inputPorts[ 1 ].FloatInternalData = 1;
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_useInternalPortData = true;
			m_textLabelWidth = 50;
			m_previewShaderGUID = "758cc2f8b537b4e4b93d9833075d138c";
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			UpdateConnections( portId );
		}

		public override void OnConnectedOutputNodeChanges( int inputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( inputPortId, otherNodeId, otherPortId, name, type );
			UpdateConnections( inputPortId );
		}
		
		void UpdateConnections( int inputPort )
		{
			m_inputPorts[ inputPort ].MatchPortToConnection();
			if ( inputPort == 0 )
			{
				m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			string x = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string y = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ 0 ].DataType, ignoreLocalvar, true );
			string result = "pow( " + x + " , " + y + " )";
			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}
	}
}
