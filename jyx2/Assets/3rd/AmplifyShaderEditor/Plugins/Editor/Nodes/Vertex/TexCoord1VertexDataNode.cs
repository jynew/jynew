// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "[VS] Vertex TexCoord1", "Vertex Data", "Second set of vertex texture coordinates. Only works on Vertex Shaders ports ( p.e. Local Vertex Offset Port )." ,null,UnityEngine.KeyCode.None,true,true, "[VS] Vertex TexCoord" )]
	public sealed class TexCoord1VertexDataNode : VertexDataNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentVertexData = "texcoord1";
		}
	}
}
