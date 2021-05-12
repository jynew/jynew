// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Parallax Offset", "UV Coordinates", "Calculates UV offset for parallax normal mapping" )]
	public sealed class ParallaxOffsetHlpNode : HelperParentNode
	{
		public readonly string[] ParallaxOffsetFunc = 
		{
			"inline float2 ParallaxOffset( half h, half height, half3 viewDir )\n",
			"{\n",
			"\th = h * height - height/2.0;\n",
			"\tfloat3 v = normalize( viewDir );\n",
			"\tv.z += 0.42;\n",
			"\treturn h* (v.xy / v.z);\n",
			"}\n"
		};

		void OnSRPActionEvent( int outputId, ref MasterNodeDataCollector dataCollector )
		{
			dataCollector.AddFunction( ParallaxOffsetFunc[ 0 ], ParallaxOffsetFunc, false );
		}

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_funcType = "ParallaxOffset";
			m_inputPorts[ 0 ].ChangeProperties( "H", WirePortDataType.FLOAT, false );
			AddInputPort( WirePortDataType.FLOAT, false, "Height" );
			AddInputPort( WirePortDataType.FLOAT3, false, "ViewDir" );
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT2, false );
			m_outputPorts[ 0 ].Name = "Out";
			OnHDAction = OnSRPActionEvent;
			OnLightweightAction = OnSRPActionEvent;
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			m_localVarName = "paralaxOffset" + OutputId;
		}
	}
}
