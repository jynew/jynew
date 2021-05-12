// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "World To Object", "Object Transform", "Transforms input to Object Space" )]
	public sealed class WorldToObjectTransfNode : ParentTransfNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_matrixName = "unity_WorldToObject";
			m_matrixHDName = "GetWorldToObjectMatrix()";
			m_previewShaderGUID = "79a5efd1e3309f54d8ba3e7fdf5e459b";
		}
	}
}
