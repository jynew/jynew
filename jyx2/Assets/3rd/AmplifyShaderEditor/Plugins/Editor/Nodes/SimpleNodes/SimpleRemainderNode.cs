// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Remainder", "Math Operators", "Remainder between two variables" )]
	public sealed class SimpleRemainderNode : DynamicTypeNode
	{
		private const string VertexFragRemainder = "( {0} % {1} )";
		private const string SurfaceRemainder = "fmod( {0} , {1} )";

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_useInternalPortData = true;
			m_textLabelWidth = 35;
			ChangeInputType( WirePortDataType.INT, false );
			ChangeOutputType( WirePortDataType.INT, false );
			m_useInternalPortData = true;
			m_previewShaderGUID = "8fdfc429d6b191c4985c9531364c1a95";
		}

		public override string BuildResults( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			base.BuildResults( outputId, ref dataCollector, ignoreLocalvar );
			string opMode = dataCollector.IsTemplate ? VertexFragRemainder : SurfaceRemainder;
			string result = string.Empty;
			switch( m_outputPorts[ 0 ].DataType )
			{
				case WirePortDataType.FLOAT:
				case WirePortDataType.FLOAT2:
				case WirePortDataType.FLOAT3:
				case WirePortDataType.FLOAT4:
				case WirePortDataType.INT:
				case WirePortDataType.COLOR:
				case WirePortDataType.OBJECT:
				{
					result = string.Format( opMode, m_inputA, m_inputB );
				}
				break;
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{
					result = UIUtils.InvalidParameter( this );
				}
				break;
			}

			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}
	}
}
