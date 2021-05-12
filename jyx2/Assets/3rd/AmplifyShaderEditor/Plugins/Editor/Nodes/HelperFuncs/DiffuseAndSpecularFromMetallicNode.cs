// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Diffuse And Specular From Metallic", "Miscellaneous", "Gets Diffuse and Specular values from Metallic. Uses DiffuseAndSpecularFromMetallic function from UnityStandardUtils." )]
	public class DiffuseAndSpecularFromMetallicNode : ParentNode
	{
		//half3 DiffuseAndSpecularFromMetallic (half3 albedo, half metallic, out half3 specColor, out half oneMinusReflectivity)
		private const string FuncFormat = "DiffuseAndSpecularFromMetallic({0},{1},{2},{3})";
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "Albedo" );
			AddInputPort( WirePortDataType.FLOAT, false, "Metallic" );
			AddOutputPort( WirePortDataType.FLOAT3, "Out" );
			AddOutputPort( WirePortDataType.FLOAT3, "Spec Color" );
			AddOutputPort( WirePortDataType.FLOAT, "One Minus Reflectivity" );
			m_previewShaderGUID = "c7c4485750948a045b5dab0985896e17";
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( dataCollector.IsSRP )
			{
				UIUtils.ShowMessage( "Diffuse And Specular From Metallic Node not compatible with SRP" );
				return "0";
			}

			if( m_outputPorts[ outputId ].IsLocalValue( dataCollector.PortCategory ) )
			{
				return m_outputPorts[ outputId ].LocalValue( dataCollector.PortCategory );
			}

			dataCollector.AddToIncludes( UniqueId, Constants.UnityStandardUtilsLibFuncs );

			string albedo = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string metallic = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );

			string specColorVar = "specColor" + OutputId;
			string oneMinusReflectivityVar = "oneMinusReflectivity" + OutputId;
			string varName = "diffuseAndSpecularFromMetallic" + OutputId;

			dataCollector.AddLocalVariable( UniqueId, PrecisionType.Half, WirePortDataType.FLOAT3, specColorVar, "(0).xxx" );
			dataCollector.AddLocalVariable( UniqueId, PrecisionType.Half, WirePortDataType.FLOAT, oneMinusReflectivityVar, "0" );


			string varValue = string.Format( FuncFormat, albedo, metallic, specColorVar, oneMinusReflectivityVar );
			dataCollector.AddLocalVariable( UniqueId, PrecisionType.Half, WirePortDataType.FLOAT3, varName, varValue );
			m_outputPorts[ 0 ].SetLocalValue( varName, dataCollector.PortCategory );
			m_outputPorts[ 1 ].SetLocalValue( specColorVar, dataCollector.PortCategory );
			m_outputPorts[ 2 ].SetLocalValue( oneMinusReflectivityVar, dataCollector.PortCategory );

			return m_outputPorts[ outputId ].LocalValue( dataCollector.PortCategory );
		}
	}
}
