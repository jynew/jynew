// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Lerp", "Math Operators", "Linear interpolation of two scalars or vectors based on a weight", null, KeyCode.L )]
	public sealed class LerpOp : ParentNode
	{
		private const string LertOpFormat = "lerp( {0} , {1} , {2})";

		[UnityEngine.SerializeField]
		private WirePortDataType m_mainDataType = WirePortDataType.FLOAT;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_textLabelWidth = 55;
			AddInputPort( WirePortDataType.FLOAT, false, "A" );
			AddInputPort( WirePortDataType.FLOAT, false, "B" );
			AddInputPort( WirePortDataType.FLOAT, false, "Alpha" );
			AddOutputPort( WirePortDataType.FLOAT,Constants.EmptyPortValue);
			m_useInternalPortData = true;
			m_previewShaderGUID = "34d9c4cdcf1fadb49af2de3f90bbc57d";
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			UpdateConnection( portId );
		}

		public override void OnConnectedOutputNodeChanges( int inputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( inputPortId, otherNodeId, otherPortId, name, type );
			UpdateConnection( inputPortId );
		}

		public override void OnInputPortDisconnected( int portId )
		{
			base.OnInputPortDisconnected( portId );
			UpdateConnection( portId );
		}
		
		void UpdateConnection( int portId )
		{
			WirePortDataType type1 = WirePortDataType.FLOAT;
			if( m_inputPorts[ 0 ].IsConnected )
				type1 = m_inputPorts[ 0 ].GetOutputConnection( 0 ).DataType;

			WirePortDataType type2 = WirePortDataType.FLOAT;
			if( m_inputPorts[ 1 ].IsConnected )
				type2 = m_inputPorts[ 1 ].GetOutputConnection( 0 ).DataType;

			WirePortDataType typealpha = WirePortDataType.FLOAT;
			if( m_inputPorts[ 2 ].IsConnected )
				typealpha = m_inputPorts[ 2 ].GetOutputConnection( 0 ).DataType;

			m_mainDataType = UIUtils.GetPriority( type1 ) > UIUtils.GetPriority( type2 ) ? type1 : type2;

			if( !m_inputPorts[ 0 ].IsConnected && !m_inputPorts[ 1 ].IsConnected && m_inputPorts[ 2 ].IsConnected )
				m_mainDataType = m_inputPorts[ 2 ].GetOutputConnection( 0 ).DataType;

			m_inputPorts[ 0 ].ChangeType( m_mainDataType, false );

			m_inputPorts[ 1 ].ChangeType( m_mainDataType, false );
			if( m_inputPorts[ 2 ].IsConnected && ( typealpha == WirePortDataType.FLOAT || typealpha == WirePortDataType.INT ) )
				m_inputPorts[ 2 ].ChangeType( WirePortDataType.FLOAT, false );
			else
				m_inputPorts[ 2 ].ChangeType( m_mainDataType, false );

			m_outputPorts[ 0 ].ChangeType( m_mainDataType, false );
		}

		//void UpdateDisconnection( int portId )
		//{
		//	int otherPortId = ( portId + 1 ) % 2;
		//	if ( m_inputPorts[ otherPortId ].IsConnected )
		//	{
		//		m_mainDataType = m_inputPorts[ otherPortId ].DataType;
		//		m_inputPorts[ portId ].ChangeType( m_mainDataType, false );
		//		m_outputPorts[ 0 ].ChangeType( m_mainDataType, false );
		//	}
		//}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			string aValue = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, m_mainDataType, ignoreLocalVar, true );
			string bValue = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, m_mainDataType, ignoreLocalVar, true );
			string interp = string.Empty;
			if( m_inputPorts[ 2 ].DataType == WirePortDataType.FLOAT )
				interp = m_inputPorts[ 2 ].GeneratePortInstructions( ref dataCollector );
			else
				interp = m_inputPorts[ 2 ].GenerateShaderForOutput( ref dataCollector, m_mainDataType, ignoreLocalVar,true );
			string result = string.Format( LertOpFormat, aValue,bValue,interp);

			RegisterLocalVariable( 0, result, ref dataCollector, "lerpResult"+OutputId );
			
			return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
		}

		//public override void RefreshExternalReferences()
		//{
		//	if ( m_inputPorts[ 2 ].DataType != WirePortDataType.FLOAT )
		//	{
		//		m_inputPorts[ 2 ].ChangeType( WirePortDataType.FLOAT, false );
		//	}
		//}
	}
}
