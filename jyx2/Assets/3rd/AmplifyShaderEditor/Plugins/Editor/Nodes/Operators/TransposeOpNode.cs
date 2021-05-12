// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Transpose", "Matrix Operators", "Transpose matrix of a matrix" )]
	public sealed class TransposeOpNode : SingleInputOp
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_opName = "transpose";
			m_drawPreview = false;
			m_inputPorts[ 0 ].CreatePortRestrictions(	WirePortDataType.FLOAT3x3,
														WirePortDataType.FLOAT4x4 );
			m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4x4, false );
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4x4, false );
		}
	}
}
