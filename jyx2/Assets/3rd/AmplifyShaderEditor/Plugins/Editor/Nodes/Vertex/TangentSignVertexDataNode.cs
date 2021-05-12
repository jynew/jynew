// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Vertex Tangent Sign", "Vertex Data", "Vertex tangent sign in object space, return the W value of tangent vector that contains only the sign of the tangent" )]
	public sealed class TangentSignVertexDataNode : ParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputVectorPorts( WirePortDataType.FLOAT, "Sign" );
			m_drawPreviewAsSphere = true;
			m_previewShaderGUID = "f5466d126f4bb1f49917eac88b1cb6af";
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			return GeneratorUtils.GenerateVertexTangentSign( ref dataCollector, UniqueId, m_currentPrecisionType ); ;
		}
	}
}
