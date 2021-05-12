// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Face", "Vertex Data", "Indicates whether the rendered surface is facing the camera (1), or facing away from the camera(-1)" )]
	public class FaceVariableNode : ParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputPort( WirePortDataType.FLOAT, "Out" );
			m_previewShaderGUID = "4b0b5b9f16353b840a5f5ad2baab3c3c";
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				UIUtils.ShowMessage( m_nodeAttribs.Name + " does not work on Tessellation port" );
				return "0";
			}

			if ( dataCollector.PortCategory == MasterNodePortCategory.Vertex )
			{
				if ( dataCollector.TesselationActive )
				{
					UIUtils.ShowMessage( m_nodeAttribs.Name + " does not work properly on Vertex/Tessellation ports" );
					return "0";
				}
				else
				{
					UIUtils.ShowMessage( m_nodeAttribs.Name + " does not work propery on Vertex ports" );
				}
			}
			if ( dataCollector.IsTemplate )
			{
				return dataCollector.TemplateDataCollectorInstance.GetVFace();
			}
			else
			{
				dataCollector.AddToInput( UniqueId, SurfaceInputs.VFACE );
				string variable = ( dataCollector.PortCategory == MasterNodePortCategory.Vertex ) ? Constants.VertexShaderOutputStr : Constants.InputVarStr;
				return variable + "." + Constants.VFaceVariable;
			}
		}
	}
}
