// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Object Space Light Dir", "Light", "Computes object space light direction (not normalized)" )]
	public sealed class ObjSpaceLightDirHlpNode : HelperParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_funcType = "ObjSpaceLightDir";
			m_inputPorts[ 0 ].Visible = false;
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT3, false );
			m_outputPorts[ 0 ].Name = "XYZ";

			AddOutputPort( WirePortDataType.FLOAT, "X" );
			AddOutputPort( WirePortDataType.FLOAT, "Y" );
			AddOutputPort( WirePortDataType.FLOAT, "Z" );

			m_useInternalPortData = false;
			m_previewShaderGUID = "c7852de24cec4a744b5358921e23feee";
			m_drawPreviewAsSphere = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( dataCollector.IsTemplate )
			{
				//Template must have its Light Mode correctly configured on tags to work as intended
				return GetOutputVectorItem( 0, outputId, dataCollector.TemplateDataCollectorInstance.GetObjectSpaceLightDir( m_currentPrecisionType ) );
			}

			dataCollector.AddToIncludes( UniqueId, Constants.UnityCgLibFuncs );
			dataCollector.AddToInput( UniqueId, SurfaceInputs.WORLD_POS );

			string vertexPos = GeneratorUtils.GenerateVertexPosition( ref dataCollector, UniqueId, WirePortDataType.FLOAT4 );
			return GetOutputVectorItem( 0, outputId, GeneratorUtils.GenerateObjectLightDirection( ref dataCollector, UniqueId, m_currentPrecisionType, vertexPos ) );
		}
	}
}
