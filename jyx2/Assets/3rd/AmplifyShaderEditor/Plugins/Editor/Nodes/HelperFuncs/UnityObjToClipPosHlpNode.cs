// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Object To Clip Pos", "Object Transform", "Transforms a point from object space to the camera’s clip space in homogeneous coordinates" )]
	public sealed class UnityObjToClipPosHlpNode : HelperParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_funcType = "UnityObjectToClipPos";
			//TODO: revisit this later
			m_funcLWFormatOverride = "TransformWorldToHClip(TransformObjectToWorld({0}))";
			m_funcHDFormatOverride = "TransformWorldToHClip(TransformObjectToWorld({0}))";
			m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT3, false );
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4, false );
			m_outputPorts[ 0 ].Name = "XYZW";
			AddOutputPort( WirePortDataType.FLOAT, "X" );
			AddOutputPort( WirePortDataType.FLOAT, "Y" );
			AddOutputPort( WirePortDataType.FLOAT, "Z" );
			AddOutputPort( WirePortDataType.FLOAT, "W" );
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			m_localVarName = "unityObjectToClipPos" + OutputId;
		}
	}
}
