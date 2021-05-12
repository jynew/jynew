// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
// 4x4 Invert by DBN in 
// http://answers.unity3d.com/questions/218333/shader-inversefloat4x4-function.html?childToView=641391#answer-641391 

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Inverse", "Matrix Operators", "Inverse matrix of a matrix" )]
	public sealed class InverseOpNode : SingleInputOp
	{
		private string Inverse4x4Header = "Inverse4x4( {0} )";

		//4x4
		private string[] Inverse4x4Function =
		{
			"{0}4x4 Inverse4x4({0}4x4 input)\n",
			"{\n",
			"\t#define minor(a,b,c) determinant({0}3x3(input.a, input.b, input.c))\n",
			"\t{0}4x4 cofactors = {0}4x4(\n",
			"\tminor( _22_23_24, _32_33_34, _42_43_44 ),\n",
			"\t-minor( _21_23_24, _31_33_34, _41_43_44 ),\n",
			"\tminor( _21_22_24, _31_32_34, _41_42_44 ),\n",
			"\t-minor( _21_22_23, _31_32_33, _41_42_43 ),\n",
			"\n",
			"\t-minor( _12_13_14, _32_33_34, _42_43_44 ),\n",
			"\tminor( _11_13_14, _31_33_34, _41_43_44 ),\n",
			"\t-minor( _11_12_14, _31_32_34, _41_42_44 ),\n",
			"\tminor( _11_12_13, _31_32_33, _41_42_43 ),\n",
			"\n",
			"\tminor( _12_13_14, _22_23_24, _42_43_44 ),\n",
			"\t-minor( _11_13_14, _21_23_24, _41_43_44 ),\n",
			"\tminor( _11_12_14, _21_22_24, _41_42_44 ),\n",
			"\t-minor( _11_12_13, _21_22_23, _41_42_43 ),\n",
			"\n",
			"\t-minor( _12_13_14, _22_23_24, _32_33_34 ),\n",
			"\tminor( _11_13_14, _21_23_24, _31_33_34 ),\n",
			"\t-minor( _11_12_14, _21_22_24, _31_32_34 ),\n",
			"\tminor( _11_12_13, _21_22_23, _31_32_33 ));\n",
			"\t#undef minor\n",
			"\treturn transpose( cofactors ) / determinant( input );\n",
			"}\n"
		};

		private bool[] Inverse4x4FunctionFlags =
		{
			true,
			false,
			true,
			true,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false
		};

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_opName = "inverse";
			m_drawPreview = false;

			m_inputPorts[ 0 ].CreatePortRestrictions( WirePortDataType.FLOAT3x3,
													  WirePortDataType.FLOAT4x4 );
			m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4x4, false );
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4x4, false );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			string precisionString = UIUtils.PrecisionWirePortToCgType( m_currentPrecisionType, WirePortDataType.FLOAT );
			string value = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );

			if ( m_outputPorts[ 0 ].DataType == WirePortDataType.FLOAT3x3 )
			{
				GeneratorUtils.Add3x3InverseFunction( ref dataCollector, precisionString );
				RegisterLocalVariable( 0, string.Format( GeneratorUtils.Inverse3x3Header, value ), ref dataCollector, "invertVal" + OutputId );
			}
			else
			{
				if ( !dataCollector.HasFunction( Inverse4x4Header ) )
				{
					//Hack to be used util indent is properly used
					int currIndent = UIUtils.ShaderIndentLevel;

					if ( dataCollector.IsTemplate )
					{
						UIUtils.ShaderIndentLevel = 0;
					}
					else
					{
						UIUtils.ShaderIndentLevel = 1;
						UIUtils.ShaderIndentLevel++;
					}

					string finalFunction = string.Empty;
					for ( int i = 0; i < Inverse4x4Function.Length; i++ )
					{
						finalFunction += UIUtils.ShaderIndentTabs + ( Inverse4x4FunctionFlags[ i ] ? string.Format( Inverse4x4Function[ i ], precisionString ) : Inverse4x4Function[ i ] );
					}

					
					UIUtils.ShaderIndentLevel = currIndent;

					dataCollector.AddFunction( Inverse4x4Header, finalFunction );
				}
				
				RegisterLocalVariable( 0, string.Format( Inverse4x4Header, value ), ref dataCollector, "invertVal" + OutputId );
			}

			return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
		}
	}
}
