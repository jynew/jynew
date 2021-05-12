using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
    [Serializable]
    [NodeAttributes( "Relay", "Miscellaneous", "Relay" )]
    public sealed class RelayNode : ParentNode
    {
        protected override void CommonInit( int uniqueId )
        {
            base.CommonInit( uniqueId );
            AddInputPort( WirePortDataType.OBJECT, false, Constants.EmptyPortValue );
            AddOutputPort( WirePortDataType.OBJECT, Constants.EmptyPortValue );
			m_previewShaderGUID = "74e4d859fbdb2c0468de3612145f4929";
		}

	    public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
        {
            base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
            m_inputPorts[ 0 ].MatchPortToConnection();
            m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
        }

        public override void OnConnectedOutputNodeChanges( int outputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
        {
            base.OnConnectedOutputNodeChanges( outputPortId, otherNodeId, otherPortId, name, type );
            m_inputPorts[ 0 ].MatchPortToConnection();
            m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
        }

        public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
        {
            base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
            return m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ 0 ].DataType, ignoreLocalvar );
        }
    }
}
