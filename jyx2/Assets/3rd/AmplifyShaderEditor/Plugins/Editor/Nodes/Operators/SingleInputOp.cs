// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;


namespace AmplifyShaderEditor
{
	[Serializable]
	public class SingleInputOp : ParentNode
	{

		[SerializeField]
		protected string m_opName;
		//[SerializeField]
		//protected int m_validTypes;

		protected bool m_autoUpdateOutputPort = true;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, Constants.EmptyPortValue );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			//m_validTypes = 0;
			m_useInternalPortData = true;

		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			m_inputPorts[ 0 ].MatchPortToConnection();
			if ( m_autoUpdateOutputPort )
				m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
		}

		public override void OnConnectedOutputNodeChanges( int outputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( outputPortId, otherNodeId, otherPortId, name, type );
			m_inputPorts[ 0 ].MatchPortToConnection();
			if ( m_autoUpdateOutputPort )
				m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			string result = "0";
			if ( m_inputPorts[ 0 ].IsConnected )
			{
				ParentNode node = m_inputPorts[ 0 ].GetOutputNode();
				int localOutputId = m_inputPorts[ 0 ].ExternalReferences[ 0 ].PortId;
				result = m_opName + "( " + node.GenerateShaderForOutput( localOutputId, ref dataCollector, ignoreLocalvar ) + " )";
			}
			else
			{
				result = m_opName + "( " + m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector ) + " )";
			}

			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}
	}
}
