// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Object Scale", "Vertex Data", "Object Scale extracted directly from its transform matrix" )]
	public class ObjectScaleNode : ParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputVectorPorts( WirePortDataType.FLOAT3, "XYZ" );
			m_drawPreviewAsSphere = true;
			m_previewShaderGUID = "5540033c6c52f51468938c1a42bd2730";
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			string objectScale = GeneratorUtils.GenerateObjectScale( ref dataCollector, UniqueId );
			return GetOutputVectorItem( 0, outputId, objectScale );
		}
	}
}
