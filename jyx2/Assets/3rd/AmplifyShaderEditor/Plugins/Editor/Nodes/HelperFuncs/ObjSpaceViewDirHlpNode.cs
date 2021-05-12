// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Object Space View Dir", "Object Transform", "Object space direction (not normalized) from given object space vertex position towards the camera" )]
	public sealed class ObjSpaceViewDirHlpNode : HelperParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_funcType = "ObjSpaceViewDir";
			//TODO: revisit this later
			m_funcLWFormatOverride = "( mul(unity_WorldToObject, float4(_WorldSpaceCameraPos.xyz, 1)).xyz - {0}.xyz )";
			m_funcHDFormatOverride = "( mul(GetWorldToObjectMatrix(), float4(_WorldSpaceCameraPos.xyz, 1)).xyz - {0}.xyz )";
			m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4, false );
			m_inputPorts[ 0 ].Vector4InternalData = new UnityEngine.Vector4( 0, 0, 0, 1 );
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT3, false );
			m_outputPorts[ 0 ].Name = "XYZ";
			AddOutputPort( WirePortDataType.FLOAT, "X" );
			AddOutputPort( WirePortDataType.FLOAT, "Y" );
			AddOutputPort( WirePortDataType.FLOAT, "Z" );
			m_previewShaderGUID = "c7852de24cec4a744b5358921e23feee";
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			m_localVarName = "objectSpaceViewDir" + OutputId;
		}
	}
}
