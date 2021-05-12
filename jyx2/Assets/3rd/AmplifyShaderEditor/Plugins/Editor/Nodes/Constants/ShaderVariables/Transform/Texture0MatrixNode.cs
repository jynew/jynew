// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Texture 0 Matrix", "Matrix Transform", "Texture 0 Matrix", null, UnityEngine.KeyCode.None, true, true )]
	public sealed class Texture0MatrixNode : ConstantShaderVariable
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputProperties( 0, "Out", WirePortDataType.FLOAT4x4 );
			m_value = "UNITY_MATRIX_TEXTURE0";
			m_drawPreview = false;
		}
	}
}
