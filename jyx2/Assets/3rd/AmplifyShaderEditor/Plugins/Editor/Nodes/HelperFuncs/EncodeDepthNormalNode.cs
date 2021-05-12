// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Encode Depth Normal", "Miscellaneous", "Encodes both Depth and Normal values into a Float4 value" )]
	public sealed class EncodeDepthNormalNode : ParentNode
	{
		private const string EncodeDepthNormalFunc = "EncodeDepthNormal( {0}, {1} )";
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, "Depth" );
			AddInputPort( WirePortDataType.FLOAT3, false, "Normal" );
			AddOutputPort( WirePortDataType.FLOAT4, Constants.EmptyPortValue );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			dataCollector.AddToIncludes( UniqueId, Constants.UnityCgLibFuncs );
			string depthValue = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string normalValue = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );

			RegisterLocalVariable( 0, string.Format( EncodeDepthNormalFunc, depthValue, normalValue ), ref dataCollector, "encodedDepthNormal" + OutputId );
			return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
		}
	}
}
