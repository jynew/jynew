// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>


namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Edge Length Tessellation With Cull", "Miscellaneous", "Tessellation level computed based on triangle edge length on the screen with patch frustum culling" )]
	public sealed class EdgeLengthCullTessNode : TessellationParentNode
	{
		private const string FunctionBody = "UnityEdgeLengthBasedTessCull( v0.vertex, v1.vertex, v2.vertex, {0},{1})";
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, "Edge Length" );
			AddInputPort( WirePortDataType.FLOAT, false, "Max Disp." );
			AddOutputPort( WirePortDataType.FLOAT4, Constants.EmptyPortValue );
		}
		
		protected override string BuildTessellationFunction( ref MasterNodeDataCollector dataCollector )
		{
			return string.Format(	FunctionBody,
									m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector ),
									m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector ) );
		}
	}
}
