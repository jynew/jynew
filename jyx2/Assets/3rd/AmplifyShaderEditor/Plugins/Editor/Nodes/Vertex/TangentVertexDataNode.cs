// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Vertex Tangent", "Vertex Data", "Vertex tangent vector in object space, can be used in both local vertex offset and fragment outputs" )]
	public sealed class TangentVertexDataNode : VertexDataNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentVertexData = "tangent";
			ChangeOutputProperties( 0, "XYZ", WirePortDataType.FLOAT3 );
			m_outputPorts[ 4 ].Visible = false;
			m_drawPreviewAsSphere = true;
			m_previewShaderGUID = "0a44bb521d06d6143a4acbc3602037f8";
		}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			base.PropagateNodeData( nodeData, ref dataCollector );
			dataCollector.DirtyNormal = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			string vertexTangent = string.Empty;
			if ( dataCollector.MasterNodeCategory == AvailableShaderTypes.Template )
			{
				vertexTangent = dataCollector.TemplateDataCollectorInstance.GetVertexTangent( m_currentPrecisionType ) + ".xyz";
				return GetOutputVectorItem( 0, outputId, vertexTangent );
			}

			if ( dataCollector.PortCategory == MasterNodePortCategory.Fragment || dataCollector.PortCategory == MasterNodePortCategory.Debug )
			{
				dataCollector.ForceNormal = true;
				dataCollector.AddToInput( UniqueId, SurfaceInputs.WORLD_NORMAL, m_currentPrecisionType );
				dataCollector.AddToInput( UniqueId, SurfaceInputs.INTERNALDATA, addSemiColon: false );
			}

			vertexTangent = GeneratorUtils.GenerateVertexTangent( ref dataCollector, UniqueId, m_currentPrecisionType );
			return GetOutputVectorItem( 0, outputId, vertexTangent );
		}
	}
}
