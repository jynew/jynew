// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "DDY", "Math Operators", "Approximate partial derivative with respect to window-space Y" )]
	public sealed class DdyOpNode : SingleInputOp
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_opName = "ddy";
			m_previewShaderGUID = "197dcc7f05339da47b6b0e681c475c5e";
			m_inputPorts[ 0 ].CreatePortRestrictions( WirePortDataType.OBJECT,
														WirePortDataType.FLOAT,
														WirePortDataType.FLOAT2,
														WirePortDataType.FLOAT3,
														WirePortDataType.FLOAT4,
														WirePortDataType.COLOR,
														WirePortDataType.INT );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( dataCollector.IsFragmentCategory )
				return base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			else
				return m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
		}
	}
}
