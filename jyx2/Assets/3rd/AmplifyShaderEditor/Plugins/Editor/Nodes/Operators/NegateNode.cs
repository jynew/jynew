using System;

namespace AmplifyShaderEditor
{
    [Serializable]
    [NodeAttributes( "Negate", "Math Operators", "Negate or invert an input value" )]
    public sealed class NegateNode : ParentNode
    {
        protected override void CommonInit( int uniqueId )
        {
            base.CommonInit( uniqueId );
            AddInputPort( WirePortDataType.FLOAT, false, Constants.EmptyPortValue );
            AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_useInternalPortData = true;
			m_previewShaderGUID = "b035bc40da1ac7c4eafad4116382ec79";
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
            string result = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );

            if ( result.StartsWith( "-" ) )
            {
                return result.Remove( 0, 1 );
            }
            else
            {
                return "-" + result;
            }
        }
    }
}
