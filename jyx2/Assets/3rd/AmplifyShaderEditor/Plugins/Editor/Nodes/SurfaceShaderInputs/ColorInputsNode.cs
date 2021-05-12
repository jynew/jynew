// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Color", "Surface Data", "Interpolated per-vertex color", null, UnityEngine.KeyCode.None, true, true, "Vertex Color", typeof( VertexColorNode ) )]
	public sealed class ColorInputsNode : SurfaceShaderINParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentInput = SurfaceInputs.COLOR;
			InitialSetup();
		}
	}
}
