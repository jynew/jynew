// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Encode Float RG ", "Miscellaneous", "Encodes [0..1] range float into a float2" )]
	public sealed class EncodeFloatRGHlpNode : HelperParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_funcType = "EncodeFloatRG ";
			m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT, false );
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT2, false );
			m_outputPorts[ 0 ].Name = "RG";
			m_previewShaderGUID = "a44b520baa5c39e41bc69a22ea46f24d";
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			m_localVarName = "encodeFloatRG" + OutputId;
		}
	}
}
