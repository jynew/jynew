// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Position From Transform", "Matrix Operators", "Gets the position vector from a transformation matrix" )]
	public sealed class PosFromTransformMatrix : ParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT4x4, true, Constants.EmptyPortValue );
			AddOutputPort( WirePortDataType.FLOAT4, "XYZW" );
			AddOutputPort( WirePortDataType.FLOAT, "X" );
			AddOutputPort( WirePortDataType.FLOAT, "Y" );
			AddOutputPort( WirePortDataType.FLOAT, "Z" );
			AddOutputPort( WirePortDataType.FLOAT, "W" );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );

			string value = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector  );
			string result = string.Format( "float4( {0},{1},{2},{3})", value + "[3][0]", value + "[3][1]", value + "[3][2]", value + "[3][3]" );
			RegisterLocalVariable( 0, result, ref dataCollector, "matrixToPos" + OutputId );

			return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );
		}
	}
}
