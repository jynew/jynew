// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

// http://stackoverflow.com/questions/9320953/what-algorithm-does-photoshop-use-to-desaturate-an-image
// https://www.shadertoy.com/view/lsdXDH

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Desaturate", "Image Effects", "Generic desaturation operation" )]
	public sealed class DesaturateOpNode : ParentNode
	{
		private const string GenericDesaturateOp0 = "dot( {0}, float3( 0.299, 0.587, 0.114 ))";
		private const string GenericDesaturateOp1 = "lerp( {0}, {1}.xxx, {2} )";
		//private const string GenericDesaturateOp = "lerp( {0},dot({0},float3(0.299,0.587,0.114)).xxx,{1})";

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "RGB" );
			AddInputPort( WirePortDataType.FLOAT, false, "Fraction" );
			AddOutputPort( WirePortDataType.FLOAT3, Constants.EmptyPortValue );
			m_useInternalPortData = true;
			m_previewShaderGUID = "faabe9efdf44b9648a523f1742abdfd3";
		}

		void UpdatePorts( int portId )
		{
			if ( portId == 0 )
			{
				m_inputPorts[ 0 ].MatchPortToConnection();
				m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{

			if ( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			string initalColorValue = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string fraction = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );

			string initialColorVarName = "desaturateInitialColor" + OutputId;
			dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT3, initialColorVarName, initalColorValue );

			string dotVarName = "desaturateDot" + OutputId;
			string dotVarValue = string.Format( GenericDesaturateOp0, initialColorVarName );

			dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType,WirePortDataType.FLOAT, dotVarName, dotVarValue );
			RegisterLocalVariable( 0, string.Format( GenericDesaturateOp1, initialColorVarName, dotVarName,fraction ), ref dataCollector, "desaturateVar" + OutputId );

			return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
		}
	}
}
