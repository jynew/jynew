// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Encode Float RGBA", "Miscellaneous", "Encodes [0..1] range float into RGBA color, for storage in low precision render target" )]
	public sealed class EncodeFloatRGBAHlpNode : HelperParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_funcType = "EncodeFloatRGBA";
			m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT, false );
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4, false );
			m_outputPorts[ 0 ].Name = "RGBA";
			m_previewShaderGUID = "c21569bf5b9371b4ca13c0c00abd5562";
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			m_localVarName = "encodeFloatRGBA" + OutputId;
		}
	}
}
