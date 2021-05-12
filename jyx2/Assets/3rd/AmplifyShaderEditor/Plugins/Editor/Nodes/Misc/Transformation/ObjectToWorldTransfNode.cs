// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Object To World", "Object Transform", "Transforms input to World Space" )]
	public sealed class ObjectToWorldTransfNode : ParentTransfNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_matrixName = "unity_ObjectToWorld";
			m_matrixHDName = "GetObjectToWorldMatrix()";
			m_previewShaderGUID = "a4044ee165813654486d0cecd0de478c";
		}
	}
}
