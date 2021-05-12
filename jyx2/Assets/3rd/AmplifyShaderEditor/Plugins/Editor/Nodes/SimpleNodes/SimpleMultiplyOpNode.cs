// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Multiply", "Math Operators", "Multiplication of two or more values ( A * B * .. )\nIt also handles Matrices multiplication", null, KeyCode.M )]
	public sealed class SimpleMultiplyOpNode : DynamicTypeNode
	{
		protected override void CommonInit( int uniqueId )
		{
			m_dynamicRestrictions = new WirePortDataType[]
			{
				WirePortDataType.OBJECT,
				WirePortDataType.FLOAT,
				WirePortDataType.FLOAT2,
				WirePortDataType.FLOAT3,
				WirePortDataType.FLOAT4,
				WirePortDataType.COLOR,
				WirePortDataType.FLOAT3x3,
				WirePortDataType.FLOAT4x4,
				WirePortDataType.INT
			};

			base.CommonInit( uniqueId );
			m_extensibleInputPorts = true;
			m_vectorMatrixOps = true;
			m_previewShaderGUID = "1ba1e43e86415ff4bbdf4d81dfcf035b";
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			int count = 0;
			int inputCount = m_inputPorts.Count;
			for( int i = 2; i < inputCount; i++ )
			{
				count++;
				if( !m_inputPorts[ i ].IsConnected )
					PreviewMaterial.SetTexture( ( "_" + Convert.ToChar( i + 65 ) ), UnityEditor.EditorGUIUtility.whiteTexture );
			}

			m_previewMaterialPassId = count;
		}

		public override string BuildResults( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( m_inputPorts[ 0 ].DataType == WirePortDataType.FLOAT3x3 ||
				m_inputPorts[ 0 ].DataType == WirePortDataType.FLOAT4x4 ||
				m_inputPorts[ 1 ].DataType == WirePortDataType.FLOAT3x3 ||
				m_inputPorts[ 1 ].DataType == WirePortDataType.FLOAT4x4 )
			{
				m_inputA = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
				m_inputB = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );


				WirePortDataType autoCast = WirePortDataType.OBJECT;
				// Check matrix on first input
				if( m_inputPorts[ 0 ].DataType == WirePortDataType.FLOAT3x3 )
				{
					switch( m_inputPorts[ 1 ].DataType )
					{
						case WirePortDataType.OBJECT:
						case WirePortDataType.FLOAT:
						case WirePortDataType.INT:
						case WirePortDataType.FLOAT2:
						case WirePortDataType.FLOAT4:
						case WirePortDataType.COLOR:
						{
							m_inputB = UIUtils.CastPortType( ref dataCollector, m_currentPrecisionType, new NodeCastInfo( UniqueId, outputId ), m_inputB, m_inputPorts[ 1 ].DataType, WirePortDataType.FLOAT3, m_inputB );
							autoCast = WirePortDataType.FLOAT3;
						}
						break;
						case WirePortDataType.FLOAT4x4:
						{
							m_inputA = UIUtils.CastPortType( ref dataCollector, m_currentPrecisionType, new NodeCastInfo( UniqueId, outputId ), m_inputA, m_inputPorts[ 0 ].DataType, WirePortDataType.FLOAT4x4, m_inputA );
						}
						break;
						case WirePortDataType.FLOAT3:
						case WirePortDataType.FLOAT3x3: break;
					}
				}

				if( m_inputPorts[ 0 ].DataType == WirePortDataType.FLOAT4x4 )
				{
					switch( m_inputPorts[ 1 ].DataType )
					{
						case WirePortDataType.OBJECT:
						case WirePortDataType.FLOAT:
						case WirePortDataType.INT:
						case WirePortDataType.FLOAT2:
						case WirePortDataType.FLOAT3:
						{
							m_inputB = UIUtils.CastPortType( ref dataCollector, m_currentPrecisionType, new NodeCastInfo( UniqueId, outputId ), m_inputB, m_inputPorts[ 1 ].DataType, WirePortDataType.FLOAT4, m_inputB );
							autoCast = WirePortDataType.FLOAT4;
						}
						break;
						case WirePortDataType.FLOAT3x3:
						{
							m_inputB = UIUtils.CastPortType( ref dataCollector, m_currentPrecisionType, new NodeCastInfo( UniqueId, outputId ), m_inputB, m_inputPorts[ 1 ].DataType, WirePortDataType.FLOAT4x4, m_inputB );
						}
						break;
						case WirePortDataType.FLOAT4x4:
						case WirePortDataType.FLOAT4:
						case WirePortDataType.COLOR: break;
					}
				}

				// Check matrix on second input
				if( m_inputPorts[ 1 ].DataType == WirePortDataType.FLOAT3x3 )
				{
					switch( m_inputPorts[ 0 ].DataType )
					{
						case WirePortDataType.OBJECT:
						case WirePortDataType.FLOAT:
						case WirePortDataType.INT:
						case WirePortDataType.FLOAT2:
						case WirePortDataType.FLOAT4:
						case WirePortDataType.COLOR:
						{
							m_inputA = UIUtils.CastPortType( ref dataCollector, m_currentPrecisionType, new NodeCastInfo( UniqueId, outputId ), m_inputA, m_inputPorts[ 0 ].DataType, WirePortDataType.FLOAT3, m_inputA );
							autoCast = WirePortDataType.FLOAT3;
						}
						break;
						case WirePortDataType.FLOAT4x4:
						case WirePortDataType.FLOAT3:
						case WirePortDataType.FLOAT3x3: break;
					}
				}

				if( m_inputPorts[ 1 ].DataType == WirePortDataType.FLOAT4x4 )
				{
					switch( m_inputPorts[ 0 ].DataType )
					{
						case WirePortDataType.OBJECT:
						case WirePortDataType.FLOAT:
						case WirePortDataType.INT:
						case WirePortDataType.FLOAT2:
						case WirePortDataType.FLOAT3:
						{
							m_inputA = UIUtils.CastPortType( ref dataCollector, m_currentPrecisionType, new NodeCastInfo( UniqueId, outputId ), m_inputA, m_inputPorts[ 0 ].DataType, WirePortDataType.FLOAT4, m_inputA );
							autoCast = WirePortDataType.FLOAT4;
						}
						break;
						case WirePortDataType.FLOAT3x3:
						case WirePortDataType.FLOAT4x4:
						case WirePortDataType.FLOAT4:
						case WirePortDataType.COLOR: break;
					}
				}
				string result = "mul( " + m_inputA + ", " + m_inputB + " )";
				if( autoCast != WirePortDataType.OBJECT && autoCast != m_outputPorts[ 0 ].DataType )
				{
					result = UIUtils.CastPortType( ref dataCollector, m_currentPrecisionType, new NodeCastInfo( UniqueId, outputId ), result, autoCast, m_outputPorts[ 0 ].DataType, result );
				}
				return result;
			}
			else
			{
				base.BuildResults( outputId, ref dataCollector, ignoreLocalvar );
				string result = "( " + m_extensibleInputResults[ 0 ];
				for( int i = 1; i < m_extensibleInputResults.Count; i++ )
				{
					result += " * " + m_extensibleInputResults[ i ];
				}
				result += " )";
				return result;
			}
		}
	}
}
