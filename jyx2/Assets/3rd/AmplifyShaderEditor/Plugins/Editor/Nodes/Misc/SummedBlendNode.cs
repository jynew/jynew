// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Summed Blend", "Miscellaneous", "Mix all channels through weighted sum", null, KeyCode.None, true )]
	public sealed class SummedBlendNode : WeightedAvgNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_inputData = new string[ 6 ];
			m_previewShaderGUID = "eda18b96e13f78b49bbdaa4da3fead19";
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			GetInputData( ref dataCollector, ignoreLocalvar );

			string result = string.Empty;
			string localVarName = "weightedBlendVar" + OutputId;
			dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, m_inputPorts[ 0 ].DataType, localVarName, m_inputData[ 0 ] );

			if ( m_activeCount == 0 )
			{
				result = m_inputData[ 0 ];
			}
			else if ( m_activeCount == 1 )
			{
				result += localVarName + "*" + m_inputData[ 1 ];
			}
			else
			{
				for ( int i = 0; i < m_activeCount; i++ )
				{
					result += localVarName + Constants.VectorSuffixes[ i ] + "*" + m_inputData[ i + 1 ];
					if ( i != ( m_activeCount - 1 ) )
					{
						result += " + ";
					}
				}
			}

			result = UIUtils.AddBrackets( result );
			RegisterLocalVariable( 0, result, ref dataCollector, "weightedBlend" + OutputId );
			return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
		}
	}
}
