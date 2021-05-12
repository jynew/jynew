// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Switch by Pipeline", "Functions", "Executes branch according to current pipeline", NodeAvailabilityFlags = (int)NodeAvailability.ShaderFunction )]
	public sealed class FunctionSwitchByPipeline : ParentNode
	{
		
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, "Standard" );
			AddInputPort( WirePortDataType.FLOAT, false, "Lightweight" );
			AddInputPort( WirePortDataType.FLOAT, false, "HD" );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );

		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			m_inputPorts[ portId ].MatchPortToConnection();
			UpdateOutputPort();
		}

		public override void OnConnectedOutputNodeChanges( int outputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( outputPortId, otherNodeId, otherPortId, name, type );
			m_inputPorts[ outputPortId ].MatchPortToConnection();
			UpdateOutputPort();
		}

		void UpdateOutputPort()
		{
			switch( m_containerGraph.CurrentSRPType )
			{
				case TemplateSRPType.BuiltIn:
				m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
				break;
				case TemplateSRPType.Lightweight:
				m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 1 ].DataType, false );
				break;
				case TemplateSRPType.HD:
				m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 2 ].DataType, false );
				break;
			}
		}
		
		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			switch( dataCollector.CurrentSRPType )
			{
				case TemplateSRPType.BuiltIn:
				return m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
				case TemplateSRPType.Lightweight:
				return m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );
				case TemplateSRPType.HD:
				return m_inputPorts[ 2 ].GeneratePortInstructions( ref dataCollector );
			}

			return "0";
		}
	}
}
