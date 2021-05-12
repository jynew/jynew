// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Divide", "Math Operators", "Division of two values ( A / B )", null, KeyCode.D )]
	public sealed class SimpleDivideOpNode : DynamicTypeNode
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
			m_allowMatrixCheck = true;
			m_previewShaderGUID = "409f06d00d1094849b0834c52791fa72";
		}

		public override string BuildResults( int outputId,  ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			SetExtensibleInputData( outputId, ref dataCollector, ignoreLocalvar );	
			string result = "( " + m_extensibleInputResults[ 0 ];
			for ( int i = 1; i < m_extensibleInputResults.Count; i++ )
			{
				result += " / " + m_extensibleInputResults[ i ];
			}
			result += " )";
			return result;
		}
	}
}
