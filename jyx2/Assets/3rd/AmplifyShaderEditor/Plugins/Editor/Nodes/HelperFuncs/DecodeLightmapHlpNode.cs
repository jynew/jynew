// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Decode Lightmap", "Miscellaneous", "Decodes color from Unity lightmap (RGBM or dLDR depending on platform)" )]
	public sealed class DecodeLightmapHlpNode : HelperParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_funcType = "DecodeLightmap";
			m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4, false );
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT3, false );
			m_previewShaderGUID = "c2d3bee1aee183343b31b9208cb402e9";
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			m_localVarName = "decodeLightMap" + OutputId;
		}
	}
}
