using System;

namespace AmplifyShaderEditor
{
	public class SignalGeneratorNode : ParentNode, ISignalGenerator
	{
		public SignalGeneratorNode() : base() { }
		public SignalGeneratorNode( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { }
		
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			SelfPowered = true;
		}

		public void GenerateSignalPropagation()
		{
			System.Type myType = GetType();
			for ( int i = 0; i < m_inputPorts.Count; i++ )
			{
				if ( m_inputPorts[ i ].IsConnected )
				{
					m_inputPorts[ i ].GetOutputNode().ActivateNode( UniqueId, i, myType );
				}
			}
		}

		public void GenerateSignalInibitor()
		{
			for ( int i = 0; i < m_inputPorts.Count; i++ )
			{
				if( m_inputPorts[ i ].IsConnected )
				{
					ParentNode node = m_inputPorts[ i ].GetOutputNode();
					if( node != null )
						node.DeactivateNode( i, false );
				}
			}
		}
	}
}
