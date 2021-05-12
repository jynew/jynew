// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "[VS] Vertex Color", "Vertex Data", "Vertex color. Only works on Vertex Shaders ports ( p.e. Local Vertex Offset Port ).", null,KeyCode.None,true,true,"Vertex Color",typeof(VertexColorNode))]
	public sealed class ColorVertexDataNode : VertexDataNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentVertexData = "color";
			ConvertFromVectorToColorPorts();
		}
	}
}
