// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Object To View Pos", "Object Transform", "Transforms a point from object space to view space" )]
	public sealed class UnityObjToViewPosHlpNode : HelperParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_funcType = "UnityObjectToViewPos";
			//TODO: revisit this later
			m_funcLWFormatOverride = "TransformWorldToView( TransformObjectToWorld( {0}) )";
			m_funcHDFormatOverride = "TransformWorldToView( TransformObjectToWorld( {0}) )";
			m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT3, false );
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT3, false );
			m_outputPorts[ 0 ].Name = "XYZ";
			AddOutputPort( WirePortDataType.FLOAT, "X" );
			AddOutputPort( WirePortDataType.FLOAT, "Y" );
			AddOutputPort( WirePortDataType.FLOAT, "Z" );
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			m_localVarName = "unityObjectToViewPos" + OutputId;
		}
	}
}
