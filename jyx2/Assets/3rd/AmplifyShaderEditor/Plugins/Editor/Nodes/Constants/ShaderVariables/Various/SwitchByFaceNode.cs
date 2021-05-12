// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Switch by Face", "Miscellaneous", "Switch which automaticaly uses a Face variable to select which input to use" )]
	public class SwitchByFaceNode : DynamicTypeNode
	{
		private const string SwitchOp = "((({0}>0)?({1}):({2})))";
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_inputPorts[ 0 ].Name = "Front";
			m_inputPorts[ 1 ].Name = "Back";
			m_textLabelWidth = 50;
			m_previewShaderGUID = "f4edf6febb54dc743b25bd5b56facea8";
		}

		public string GenerateErrorValue()
		{
			switch ( m_outputPorts[0].DataType )
			{
				case WirePortDataType.FLOAT2:
				{
					return "(0).xx";
				}
				case WirePortDataType.FLOAT3:
				{
					return "(0).xxx";
				}
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				{
					return "(0).xxxx";
				}
			}
			return "0";
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				UIUtils.ShowMessage( m_nodeAttribs.Name + " does not work on Tessellation port" );
				return GenerateErrorValue();
			}

			if ( dataCollector.PortCategory == MasterNodePortCategory.Vertex )
			{
				if ( dataCollector.TesselationActive )
				{
					UIUtils.ShowMessage( m_nodeAttribs.Name + " does not work properly on Vertex/Tessellation ports" );
					return GenerateErrorValue();
				}
				else
				{
					UIUtils.ShowMessage( m_nodeAttribs.Name + " does not work properly on Vertex ports" );
				}
			}

			if ( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			string front = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string back = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );

			dataCollector.AddToInput( UniqueId, SurfaceInputs.VFACE );
			string variable = string.Empty;
			if ( dataCollector.IsTemplate )
			{
				variable = dataCollector.TemplateDataCollectorInstance.GetVFace();
			}
			else
			{
				variable = ( ( dataCollector.PortCategory == MasterNodePortCategory.Vertex ) ? Constants.VertexShaderOutputStr : Constants.InputVarStr ) + "." + Constants.VFaceVariable;
			}

			string value = string.Format( SwitchOp, variable, front, back );
			RegisterLocalVariable( 0, value, ref dataCollector, "switchResult" + OutputId );
			return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
		}
	}
}
