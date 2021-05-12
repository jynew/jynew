// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Refract", "Vector Operators", "Computes a refraction vector" )]
	public sealed class RefractOpVec : ParentNode
	{
		[UnityEngine.SerializeField]
		private WirePortDataType m_mainDataType = WirePortDataType.FLOAT;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT4, false , "Incident" );
			AddInputPort( WirePortDataType.FLOAT4, false , "Normal" );
			AddInputPort( WirePortDataType.FLOAT, false, "Eta" );
			AddOutputPort( WirePortDataType.FLOAT4, Constants.EmptyPortValue );
			m_textLabelWidth = 67;
			m_previewShaderGUID = "5ab44ca484bed8b4884b03b1c00fdc3d";
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
			if( portId == 2 )
				return;

			bool hasConnection = false;

			WirePortDataType type1 = WirePortDataType.FLOAT;
			if( m_inputPorts[ 0 ].IsConnected )
			{
				type1 = m_inputPorts[ 0 ].GetOutputConnection( 0 ).DataType;
				hasConnection = true;
			}
			WirePortDataType type2 = WirePortDataType.FLOAT;
			if( m_inputPorts[ 1 ].IsConnected )
			{
				type2 = m_inputPorts[ 1 ].GetOutputConnection( 0 ).DataType;
				hasConnection = true;
			}

			if( hasConnection )
			{
				m_mainDataType = UIUtils.GetPriority( type1 ) > UIUtils.GetPriority( type2 ) ? type1 : type2;
			}
			else
			{
				m_mainDataType = WirePortDataType.FLOAT4;
			}
			
			m_inputPorts[ 0 ].ChangeType( m_mainDataType, false );
			m_inputPorts[ 1 ].ChangeType( m_mainDataType, false );
			m_outputPorts[ 0 ].ChangeType( m_mainDataType, false );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			string incident = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string normal = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );
			string interp = m_inputPorts[ 2 ].GeneratePortInstructions( ref dataCollector );
			string result = "refract( " + incident + " , " + normal + " , " + interp + " )";

			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}
		
	}
}
