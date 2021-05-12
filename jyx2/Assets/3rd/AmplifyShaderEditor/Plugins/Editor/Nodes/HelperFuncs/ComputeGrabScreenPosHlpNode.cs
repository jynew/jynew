// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Compute Grab Screen Pos", "Camera And Screen", "Computes texture coordinate for doing a screenspace-mapped texture sample. Input is clip space position" )]
	public sealed class ComputeGrabScreenPosHlpNode : HelperParentNode
	{
		private readonly string[] ComputeGrabScreenPosFunction =
		{
			"inline float4 ComputeGrabScreenPos( float4 pos )\n",
			"{\n",
			"#if UNITY_UV_STARTS_AT_TOP\n",
			"\tfloat scale = -1.0;\n",
			"#else\n",
			"\tfloat scale = 1.0;\n",
			"#endif\n",
			"\tfloat4 o = pos * 0.5f;\n",
			"\to.xy = float2( o.x, o.y*scale ) + o.w;\n",
			"#ifdef UNITY_SINGLE_PASS_STEREO\n",
			"\to.xy = TransformStereoScreenSpaceTex ( o.xy, pos.w );\n",
			"#endif\n",
			"\to.zw = pos.zw;\n",
			"\treturn o;\n",
			"}\n"
		};

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_funcType = "ComputeGrabScreenPos";
			m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4, false );
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4, false );
			m_outputPorts[ 0 ].Name = "XYZW";
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			m_localVarName = "computeGrabScreenPos" + OutputId;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( dataCollector.IsTemplate && dataCollector.TemplateDataCollectorInstance.CurrentSRPType == TemplateSRPType.HD )
			{
				dataCollector.AddFunction( m_funcType, ComputeGrabScreenPosFunction, false );
			}
			return base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
		}
	}
}
