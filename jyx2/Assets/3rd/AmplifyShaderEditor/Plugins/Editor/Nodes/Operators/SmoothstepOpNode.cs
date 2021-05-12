// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Smoothstep", "Math Operators", "Returns a smooth Hermite interpolation between 0 and 1, if input is in the range [min, max]." )]
	public sealed class SmoothstepOpNode : ParentNode
	{
		//[UnityEngine.SerializeField]
		//private WirePortDataType m_mainDataType = WirePortDataType.FLOAT;
		
		private int m_alphaPortId = 0;
		private int m_minPortId = 0;
		private int m_maxPortId = 0;
		private const string SmoothstepOpFormat = "smoothstep( {0} , {1} , {2})";//min max alpha
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, Constants.EmptyPortValue, -1, MasterNodePortCategory.Fragment, 0 );
			m_alphaPortId = m_inputPorts.Count - 1;
			AddInputPort( WirePortDataType.FLOAT, false, "Min", -1, MasterNodePortCategory.Fragment, 1 );
			m_minPortId = m_inputPorts.Count - 1;
			AddInputPort( WirePortDataType.FLOAT, false, "Max", -1, MasterNodePortCategory.Fragment, 2 );
			m_maxPortId = m_inputPorts.Count - 1;

			GetInputPortByUniqueId( m_maxPortId ).FloatInternalData = 1;

			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_useInternalPortData = true;
			m_textLabelWidth = 55;
			m_previewShaderGUID = "954cdd40a7a528344a0a4d3ff1db5176";
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			if( portId == 0 )
			{
				m_inputPorts[ 0 ].MatchPortToConnection();
				m_inputPorts[ 1 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
				m_inputPorts[ 2 ].ChangeType( m_inputPorts[ 0 ].DataType, false );

				m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
			}
		}

		public override void OnConnectedOutputNodeChanges( int outputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( outputPortId, otherNodeId, otherPortId, name, type );
			if( outputPortId == 0 )
			{
				m_inputPorts[ 0 ].MatchPortToConnection();
				m_inputPorts[ 1 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
				m_inputPorts[ 2 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
				m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
			}
		}

		//public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		//{
		//	base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
		//	UpdateConnection( portId );
		//}

		//public override void OnConnectedOutputNodeChanges( int inputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		//{
		//	base.OnConnectedOutputNodeChanges( inputPortId, otherNodeId, otherPortId, name, type );
		//	UpdateConnection( inputPortId );
		//}

		//public override void OnInputPortDisconnected( int portId )
		//{
		//	base.OnInputPortDisconnected( portId );
		//	UpdateConnection( portId );
		//}

		//void UpdateConnection( int portId )
		//{
		//	WirePortDataType type1 = WirePortDataType.FLOAT;
		//	if( m_inputPorts[ m_minPortId ].IsConnected )
		//		type1 = m_inputPorts[ m_minPortId ].GetOutputConnection( 0 ).DataType;

		//	WirePortDataType type2 = WirePortDataType.FLOAT;
		//	if( m_inputPorts[ m_maxPortId ].IsConnected )
		//		type2 = m_inputPorts[ m_maxPortId ].GetOutputConnection( 0 ).DataType;

		//	m_mainDataType = UIUtils.GetPriority( type1 ) > UIUtils.GetPriority( type2 ) ? type1 : type2;

		//	if( !m_inputPorts[ m_minPortId ].IsConnected && !m_inputPorts[ m_maxPortId ].IsConnected && m_inputPorts[ m_alphaPortId ].IsConnected )
		//		m_mainDataType = m_inputPorts[ m_alphaPortId ].GetOutputConnection( 0 ).DataType;

		//	m_inputPorts[ m_minPortId ].ChangeType( m_mainDataType, false );

		//	m_inputPorts[ m_maxPortId ].ChangeType( m_mainDataType, false );
		//	if( m_inputPorts[ m_alphaPortId ].IsConnected && m_inputPorts[ m_alphaPortId ].GetOutputConnection( 0 ).DataType == WirePortDataType.FLOAT )
		//		m_inputPorts[ m_alphaPortId ].ChangeType( WirePortDataType.FLOAT, false );
		//	else
		//		m_inputPorts[ m_alphaPortId ].ChangeType( m_mainDataType, false );

		//	m_outputPorts[ 0 ].ChangeType( m_mainDataType, false );
		//}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			string aValue = m_inputPorts[ m_minPortId ].GeneratePortInstructions( ref dataCollector );
			string bValue = m_inputPorts[ m_maxPortId ].GeneratePortInstructions( ref dataCollector );
			string interp = m_inputPorts[ m_alphaPortId ].GeneratePortInstructions( ref dataCollector );
			
			string result = string.Format( SmoothstepOpFormat, aValue, bValue, interp );

			RegisterLocalVariable( 0, result, ref dataCollector, "smoothstepResult" + OutputId );

			return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
		}
	}
}
