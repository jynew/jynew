// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Scale Matrix", "Matrix Transform", "Scale Matrix",null, UnityEngine.KeyCode.None, true, true, "Object Scale" )]
	public sealed class UnityScaleMatrix : ConstantShaderVariable
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputProperties( 0, "Out", WirePortDataType.FLOAT4x4 );
			m_value = "unity_Scale";
			m_drawPreview = false;
		}
	}
}
