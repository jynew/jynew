// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
//
// Custom Node Vertex Binormal World
// Donated by Community Member Kebrus

using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "World Bitangent", "Surface Data", "Per pixel world bitangent vector", null, KeyCode.None, true, false, null, null, "kebrus" )]
	public sealed class VertexBinormalNode : ParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputVectorPorts( WirePortDataType.FLOAT3, "XYZ" );
			m_drawPreviewAsSphere = true;
			m_previewShaderGUID = "76873532ab67d2947beaf07151383cbe";
		}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			base.PropagateNodeData( nodeData, ref dataCollector );
			dataCollector.DirtyNormal = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( dataCollector.IsTemplate )
				return GetOutputVectorItem( 0, outputId, dataCollector.TemplateDataCollectorInstance.GetWorldBinormal( m_currentPrecisionType ) );

			if( dataCollector.PortCategory == MasterNodePortCategory.Fragment || dataCollector.PortCategory == MasterNodePortCategory.Debug )
			{
				dataCollector.ForceNormal = true;

				dataCollector.AddToInput( UniqueId, SurfaceInputs.WORLD_NORMAL, m_currentPrecisionType );
				dataCollector.AddToInput( UniqueId, SurfaceInputs.INTERNALDATA, addSemiColon: false );
			}

			string worldBitangent = GeneratorUtils.GenerateWorldBitangent( ref dataCollector, UniqueId );

			return GetOutputVectorItem( 0, outputId, worldBitangent );
		}
	}
}
