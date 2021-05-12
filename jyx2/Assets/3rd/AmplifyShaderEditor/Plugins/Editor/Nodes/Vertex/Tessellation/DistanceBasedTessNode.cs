
// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>


namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Distance-based Tessellation", "Miscellaneous", "Calculates tessellation based on distance from camera" )]
	public sealed class DistanceBasedTessNode : TessellationParentNode
	{
		private const string FunctionBody = "UnityDistanceBasedTess( v0.vertex, v1.vertex, v2.vertex, {0},{1},{2})";
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false,"Factor");
			AddInputPort( WirePortDataType.FLOAT, false, "Min Dist" );
			AddInputPort( WirePortDataType.FLOAT, false, "Max Dist" );
			AddOutputPort( WirePortDataType.FLOAT4, Constants.EmptyPortValue );
		}

		protected override string BuildTessellationFunction( ref MasterNodeDataCollector dataCollector )
		{
			return string.Format(	FunctionBody,
									m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector ),
									m_inputPorts[ 2 ].GeneratePortInstructions( ref dataCollector ),
									m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector ) );
		}
	}
}
