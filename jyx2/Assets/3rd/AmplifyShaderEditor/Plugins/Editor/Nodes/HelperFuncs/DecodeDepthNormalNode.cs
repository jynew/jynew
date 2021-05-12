// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Decode Depth Normal", "Miscellaneous", "Decodes both Depth and Normal from a previously encoded pixel value" )]
	public sealed class DecodeDepthNormalNode : ParentNode
	{
		private const string DecodeDepthNormalFunc = "DecodeDepthNormal( {0}, {1}, {2} );";

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT4, false, "Encoded" );
			AddOutputPort( WirePortDataType.FLOAT, "Depth" );
			AddOutputPort( WirePortDataType.FLOAT3, "Normal" );
			m_previewShaderGUID = "dbf37c4d3ce0f0b41822584d6c9ba203";
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( m_outputPorts[ outputId ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ outputId ].LocalValue( dataCollector.PortCategory );

			dataCollector.AddToIncludes( UniqueId, Constants.UnityCgLibFuncs );
			string encodedValue = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string depthDecodedVal = "depthDecodedVal" + OutputId;
			string normalDecodedVal = "normalDecodedVal" + OutputId;
			RegisterLocalVariable( 0, "0", ref dataCollector, depthDecodedVal );
			RegisterLocalVariable( 1, "float3(0,0,0)", ref dataCollector, normalDecodedVal );
			dataCollector.AddLocalVariable( UniqueId, string.Format( DecodeDepthNormalFunc, encodedValue , depthDecodedVal, normalDecodedVal) );
			return m_outputPorts[ outputId ].LocalValue( dataCollector.PortCategory );
		}
	}
}
