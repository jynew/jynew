// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Vertex ID", "Vertex Data", "Indicates current vertex number" )]
	public class VertexIdVariableNode : ParentNode
	{
		private const string VertexIdVarName = "ase_vertexId";
		private const string VertexIdRegistry = "uint "+ VertexIdVarName + " : SV_VertexID;";
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputPort( WirePortDataType.INT, "Out" );
			m_previewShaderGUID = "5934bf2c10b127a459177a3b622cea65";
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				UIUtils.ShowMessage( m_nodeAttribs.Name + " does not work on Tessellation port" );
				return "0";
			}

			if ( dataCollector.IsTemplate )
			{
				return dataCollector.TemplateDataCollectorInstance.GetVertexId();
			}
			else
			{
				if( dataCollector.IsFragmentCategory )
				{
					GenerateValueInVertex( ref dataCollector, WirePortDataType.UINT, Constants.VertexShaderInputStr + "."+ VertexIdVarName, VertexIdVarName, true );
					return Constants.InputVarStr + "."+ VertexIdVarName;
				}
				else
				{
					return Constants.VertexShaderInputStr + "."+ VertexIdVarName;
				}
			}
		}
		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			if( !dataCollector.IsTemplate )
				dataCollector.AddCustomAppData( VertexIdRegistry );

			base.PropagateNodeData( nodeData, ref dataCollector );
		}
	}
}
