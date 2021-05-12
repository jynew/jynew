// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Encode View Normal Stereo", "Miscellaneous", "Encodes view space normal into two numbers in [0..1] range" )]
	public sealed class EncodeViewNormalStereoHlpNode : HelperParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_funcType = "EncodeViewNormalStereo";
			m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT3, false );
			m_inputPorts[ 0 ].Name = "XYZ";
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT2, false );
			m_previewShaderGUID = "3d0b3d482b7246c4cb60fa73e6ceac6c";
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			m_localVarName = "encodeViewNormalStereo" + OutputId;
		}
	}
}
