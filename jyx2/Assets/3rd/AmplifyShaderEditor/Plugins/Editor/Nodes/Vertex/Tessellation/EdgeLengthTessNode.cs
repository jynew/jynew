// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Edge Length Tessellation", "Miscellaneous", "Tessellation level computed based on triangle edge length on the screen" )]
	public sealed class EdgeLengthTessNode : TessellationParentNode
	{
		private const string FunctionBody = "UnityEdgeLengthBasedTess (v0.vertex, v1.vertex, v2.vertex, {0})";
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, "Edge Length" );
			AddOutputPort( WirePortDataType.FLOAT4, Constants.EmptyPortValue );
		}

		protected override string BuildTessellationFunction( ref MasterNodeDataCollector dataCollector )
		{
			return string.Format( FunctionBody, m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector ) );
		}
	}
}
