// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
//
// Custom Node Remap
// Donated by The Four Headed Cat - @fourheadedcat
using UnityEngine;
using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Remap", "Math Operators", "Remap value from old min - max range to new min - max range", null, KeyCode.None, true, false, null, null, "The Four Headed Cat - @fourheadedcat" )]
	public sealed class TFHCRemapNode : DynamicTypeNode
	{

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_inputPorts[ 0 ].Name = Constants.EmptyPortValue;
			m_inputPorts[ 1 ].Name = "Min Old";
			AddInputPort( WirePortDataType.FLOAT, false, "Max Old" );
			m_inputPorts[ 2 ].FloatInternalData = 1;
			AddInputPort( WirePortDataType.FLOAT, false, "Min New" );
			AddInputPort( WirePortDataType.FLOAT, false, "Max New" );
			m_inputPorts[ 4 ].FloatInternalData = 1;
			m_textLabelWidth = 100;
			m_useInternalPortData = true;
			m_previewShaderGUID = "72dd1cbea889fa047b929d5191e360c0";
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			UpdateConnections();
		}

		public override void OnConnectedOutputNodeChanges( int outputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( outputPortId, otherNodeId, otherPortId, name, type );
			UpdateConnections();
		}

		void UpdateConnections()
		{
			m_inputPorts[ 0 ].MatchPortToConnection();
			m_inputPorts[ 1 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
			m_inputPorts[ 2 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
			m_inputPorts[ 3 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
			m_inputPorts[ 4 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
			m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			string value = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string oldMin = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ 0 ].DataType, ignoreLocalvar, true );
			string oldMax = m_inputPorts[ 2 ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ 0 ].DataType, ignoreLocalvar, true );
			string newMin = m_inputPorts[ 3 ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ 0 ].DataType, ignoreLocalvar, true );
			string newMax = m_inputPorts[ 4 ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ 0 ].DataType, ignoreLocalvar, true );
			string strout = "(" + newMin + " + (" + value + " - " + oldMin + ") * (" + newMax + " - " + newMin + ") / (" + oldMax + " - " + oldMin + "))";

			return CreateOutputLocalVariable( 0, strout, ref dataCollector );
		}
	}
}
