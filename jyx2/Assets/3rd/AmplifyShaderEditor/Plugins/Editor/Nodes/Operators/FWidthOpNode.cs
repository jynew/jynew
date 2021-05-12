// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "FWidth", "Math Operators", "Sum of approximate window-space partial derivatives magnitudes" )]
	public sealed class FWidthOpNode : SingleInputOp
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_opName = "fwidth";
			m_previewShaderGUID = "81ea481faaef9c8459a555479ba64df7";
			m_inputPorts[ 0 ].CreatePortRestrictions(	WirePortDataType.OBJECT,
														WirePortDataType.FLOAT ,
														WirePortDataType.FLOAT2,
														WirePortDataType.FLOAT3,
														WirePortDataType.FLOAT4,
														WirePortDataType.COLOR ,
														WirePortDataType.INT);
			//m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4, false );
		}
	}
}
