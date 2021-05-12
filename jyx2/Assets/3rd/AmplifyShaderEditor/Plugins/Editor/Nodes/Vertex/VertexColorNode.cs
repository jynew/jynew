// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Vertex Color", "Vertex Data", "Vertex color interpolated on fragment" )]
	public sealed class VertexColorNode : VertexDataNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentVertexData = "color";
			m_outputPorts[ 0 ].Name = "RGBA";
			ConvertFromVectorToColorPorts();
			m_drawPreviewAsSphere = true;
			m_previewShaderGUID = "ca1d22db6470c5f4d9f93a9873b4f5bc";
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if( dataCollector.MasterNodeCategory == AvailableShaderTypes.Template )
			{
				string color = dataCollector.TemplateDataCollectorInstance.GetVertexColor( m_currentPrecisionType );
				return GetOutputColorItem( 0, outputId, color );
			}

			if( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				return base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalVar );
			}
			else
			{
				dataCollector.AddToInput( UniqueId, SurfaceInputs.COLOR );
				string result = Constants.InputVarStr + "." + Constants.ColorVariable;
				switch( outputId )
				{
					case 1: result += ".r"; break;
					case 2: result += ".g"; break;
					case 3: result += ".b"; break;
					case 4: result += ".a"; break;
				}
				return result;
			}
		}
	}
}
