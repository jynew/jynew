// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
//
// Custom Node Vertex Tangent World
// Donated by Community Member Kebrus

using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "World Tangent", "Surface Data", "Per pixel world tangent vector", null, KeyCode.None, true, false, null, null, "kebrus" )]
	public sealed class VertexTangentNode : ParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputVectorPorts( WirePortDataType.FLOAT3, "XYZ" );
			m_drawPreviewAsSphere = true;
			m_previewShaderGUID = "61f0b80493c9b404d8c7bf56d59c3f81";
		}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			base.PropagateNodeData( nodeData , ref dataCollector );
			dataCollector.DirtyNormal = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( dataCollector.IsTemplate )
			{
				return GetOutputVectorItem( 0, outputId, dataCollector.TemplateDataCollectorInstance.GetWorldTangent( m_currentPrecisionType ) );
			}

			if( dataCollector.PortCategory == MasterNodePortCategory.Fragment || dataCollector.PortCategory == MasterNodePortCategory.Debug )
			{
				dataCollector.ForceNormal = true;

				dataCollector.AddToInput( UniqueId, SurfaceInputs.WORLD_NORMAL, m_currentPrecisionType );
				dataCollector.AddToInput( UniqueId, SurfaceInputs.INTERNALDATA, addSemiColon: false );
			}

			string worldTangent = GeneratorUtils.GenerateWorldTangent( ref dataCollector, UniqueId );

			return GetOutputVectorItem( 0, outputId, worldTangent );
		}
	}
}
