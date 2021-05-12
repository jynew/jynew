// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Simple Contrast", "Image Effects", "Simple contrast matrix multiplication" )]
	public sealed class SimpleContrastOpNode : ParentNode
	{
		private const string InputTypeStr = "Contrast";
		private const string FunctionHeader = "CalculateContrast({0},{1})";
		private readonly string[] m_functionBody = { "float4 CalculateContrast( float contrastValue, float4 colorTarget )\n",
											"{\n",
											"\tfloat t = 0.5 * ( 1.0 - contrastValue );\n",
											"\treturn mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );\n",
											"}"};
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddPorts();
			m_textLabelWidth = 70;
			m_useInternalPortData = true;
			m_previewShaderGUID = "8d76799413f9f0547ac9b1de7ba798f1";
		}

		void AddPorts()
		{
			AddInputPort( WirePortDataType.COLOR, false, "RGBA", -1, MasterNodePortCategory.Fragment, 1 );
			AddInputPort( WirePortDataType.FLOAT, false, "Value", -1, MasterNodePortCategory.Fragment, 0 );
			AddOutputPort( WirePortDataType.COLOR, Constants.EmptyPortValue );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			string contrastValue = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );
			string colorTarget = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string result = dataCollector.AddFunctions( FunctionHeader, m_functionBody, false, contrastValue, colorTarget );

			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if( UIUtils.CurrentShaderVersion() < 5004 )
			{
				m_inputPorts[ 1 ].FloatInternalData = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
			}
		}
	}
}
