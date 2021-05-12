// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Decode Float RGBA", "Miscellaneous", "Decodes RGBA color into a float" )]
	public sealed class DecodeFloatRGBAHlpNode : HelperParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_funcType = "DecodeFloatRGBA";
			m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4, false );
			m_inputPorts[ 0 ].Name = "RGBA";
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT, false );
			m_previewShaderGUID = "f71b31b15ff3f2042bafbed40acd29f4";
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			m_localVarName = "decodeFloatRGBA" + OutputId;
		}
	}
}
