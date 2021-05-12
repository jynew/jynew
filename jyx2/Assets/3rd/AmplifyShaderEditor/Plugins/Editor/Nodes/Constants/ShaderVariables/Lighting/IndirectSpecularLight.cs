// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Indirect Specular Light", "Light", "Indirect Specular Light", NodeAvailabilityFlags = (int)( NodeAvailability.CustomLighting | NodeAvailability.TemplateShader ) )]
	public sealed class IndirectSpecularLight : ParentNode
	{
		[SerializeField]
		private ViewSpace m_normalSpace = ViewSpace.Tangent;

		private const string DefaultErrorMessage = "This node only returns correct information using a custom light model, otherwise returns 0";
		private bool m_upgradeMessage = false;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "Normal" );
			AddInputPort( WirePortDataType.FLOAT, false, "Smoothness" );
			AddInputPort( WirePortDataType.FLOAT, false, "Occlusion" );
			m_inputPorts[ 0 ].Vector3InternalData = Vector3.forward;
			m_inputPorts[ 1 ].FloatInternalData = 0.5f;
			m_inputPorts[ 2 ].FloatInternalData = 1;
			m_inputPorts[ 1 ].AutoDrawInternalData = true;
			m_inputPorts[ 2 ].AutoDrawInternalData = true;
			m_autoWrapProperties = true;
			AddOutputPort( WirePortDataType.FLOAT3, "RGB" );
			m_errorMessageTypeIsError = NodeMessageType.Warning;
			m_errorMessageTooltip = DefaultErrorMessage;
			m_previewShaderGUID = "d6e441d0a8608954c97fa347d3735e92";
			m_drawPreviewAsSphere = true;
		}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			base.PropagateNodeData( nodeData, ref dataCollector );
			if( m_inputPorts[ 0 ].IsConnected )
				dataCollector.DirtyNormal = true;
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if( m_inputPorts[ 0 ].IsConnected )
				m_previewMaterialPassId = 1;
			else
				m_previewMaterialPassId = 0;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();

			EditorGUI.BeginChangeCheck();
			m_normalSpace = (ViewSpace)EditorGUILayoutEnumPopup( "Normal Space", m_normalSpace );
			if( EditorGUI.EndChangeCheck() )
			{
				UpdatePort();
			}
			if( !m_inputPorts[ 1 ].IsConnected )
				m_inputPorts[ 1 ].FloatInternalData = EditorGUILayout.FloatField( m_inputPorts[ 1 ].Name, m_inputPorts[ 1 ].FloatInternalData );
			if( !m_inputPorts[ 2 ].IsConnected )
				m_inputPorts[ 2 ].FloatInternalData = EditorGUILayout.FloatField( m_inputPorts[ 2 ].Name, m_inputPorts[ 2 ].FloatInternalData );
		}

		private void UpdatePort()
		{
			if( m_normalSpace == ViewSpace.World )
				m_inputPorts[ 0 ].ChangeProperties( "World Normal", m_inputPorts[ 0 ].DataType, false );
			else
				m_inputPorts[ 0 ].ChangeProperties( "Normal", m_inputPorts[ 0 ].DataType, false );

			m_sizeIsDirty = true;
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			if( m_upgradeMessage || ( ContainerGraph.CurrentStandardSurface != null && ContainerGraph.CurrentStandardSurface.CurrentLightingModel != StandardShaderLightModel.CustomLighting ) )
				m_showErrorMessage = true;
			else
				m_showErrorMessage = false;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( dataCollector.IsTemplate )
			{
				dataCollector.AddToIncludes( UniqueId, Constants.UnityLightingLib );
				string worldPos = dataCollector.TemplateDataCollectorInstance.GetWorldPos();
				string worldViewDir = dataCollector.TemplateDataCollectorInstance.GetViewDir( false, MasterNodePortCategory.Fragment );
				//string worldNormal = dataCollector.TemplateDataCollectorInstance.GetWorldNormal( PrecisionType.Float );

				string worldNormal = string.Empty;
				if( m_inputPorts[ 0 ].IsConnected )
				{
					if( m_normalSpace == ViewSpace.Tangent )
						worldNormal = dataCollector.TemplateDataCollectorInstance.GetWorldNormal( UniqueId, m_currentPrecisionType, m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector ), OutputId );
					else
						worldNormal = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
				}
				else
				{
					worldNormal = dataCollector.TemplateDataCollectorInstance.GetWorldNormal( PrecisionType.Float, false, MasterNodePortCategory.Fragment );
				}

				string tempsmoothness = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );
				string tempocclusion = m_inputPorts[ 2 ].GeneratePortInstructions( ref dataCollector );

				dataCollector.AddLocalVariable( UniqueId, "UnityGIInput data;" );
				dataCollector.AddLocalVariable( UniqueId, "UNITY_INITIALIZE_OUTPUT( UnityGIInput, data );" );
				dataCollector.AddLocalVariable( UniqueId, "data.worldPos = " + worldPos + ";" );
				dataCollector.AddLocalVariable( UniqueId, "data.worldViewDir = " + worldViewDir + ";" );
				dataCollector.AddLocalVariable( UniqueId, "data.probeHDR[0] = unity_SpecCube0_HDR;" );
				dataCollector.AddLocalVariable( UniqueId, "data.probeHDR[1] = unity_SpecCube1_HDR;" );
				dataCollector.AddLocalVariable( UniqueId, "#if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION //specdataif0" );
				dataCollector.AddLocalVariable( UniqueId, "\tdata.boxMin[0] = unity_SpecCube0_BoxMin;" );
				dataCollector.AddLocalVariable( UniqueId, "#endif //specdataif0" );
				dataCollector.AddLocalVariable( UniqueId, "#if UNITY_SPECCUBE_BOX_PROJECTION //specdataif1" );
				dataCollector.AddLocalVariable( UniqueId, "\tdata.boxMax[0] = unity_SpecCube0_BoxMax;" );
				dataCollector.AddLocalVariable( UniqueId, "\tdata.probePosition[0] = unity_SpecCube0_ProbePosition;" );
				dataCollector.AddLocalVariable( UniqueId, "\tdata.boxMax[1] = unity_SpecCube1_BoxMax;" );
				dataCollector.AddLocalVariable( UniqueId, "\tdata.boxMin[1] = unity_SpecCube1_BoxMin;" );
				dataCollector.AddLocalVariable( UniqueId, "\tdata.probePosition[1] = unity_SpecCube1_ProbePosition;" );
				dataCollector.AddLocalVariable( UniqueId, "#endif //specdataif1" );

				dataCollector.AddLocalVariable( UniqueId, "Unity_GlossyEnvironmentData g" + OutputId + " = UnityGlossyEnvironmentSetup( " + tempsmoothness + ", " + worldViewDir + ", " + worldNormal + ", float3(0,0,0));" );
				dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT3, "indirectSpecular" + OutputId, "UnityGI_IndirectSpecular( data, " + tempocclusion + ", " + worldNormal + ", g" + OutputId + " )" );
				return "indirectSpecular" + OutputId;
			}

			if( dataCollector.GenType == PortGenType.NonCustomLighting || dataCollector.CurrentCanvasMode != NodeAvailability.CustomLighting )
				return "float3(0,0,0)";

			string normal = string.Empty;
			if( m_inputPorts[ 0 ].IsConnected )
			{
				dataCollector.AddToInput( UniqueId, SurfaceInputs.WORLD_NORMAL, m_currentPrecisionType );
				dataCollector.AddToInput( UniqueId, SurfaceInputs.INTERNALDATA, addSemiColon: false );
				dataCollector.ForceNormal = true;

				normal = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
				if( m_normalSpace == ViewSpace.Tangent )
					normal = "WorldNormalVector( " + Constants.InputVarStr + " , " + normal + " )";

				dataCollector.AddLocalVariable( UniqueId, "float3 indirectNormal" + OutputId + " = " + normal + ";" );
				normal = "indirectNormal" + OutputId;
			}
			else
			{
				if( dataCollector.IsFragmentCategory )
				{
					dataCollector.AddToInput( UniqueId, SurfaceInputs.WORLD_NORMAL, m_currentPrecisionType );
					if( dataCollector.DirtyNormal )
					{
						dataCollector.AddToInput( UniqueId, SurfaceInputs.INTERNALDATA, addSemiColon: false );
						dataCollector.ForceNormal = true;
					}
				}

				normal = GeneratorUtils.GenerateWorldNormal( ref dataCollector, UniqueId );
			}

			string smoothness = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );
			string occlusion = m_inputPorts[ 2 ].GeneratePortInstructions( ref dataCollector );
			string viewDir = "data.worldViewDir";

			if( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				string worldPos = GeneratorUtils.GenerateWorldPosition( ref dataCollector, UniqueId );
				viewDir = GeneratorUtils.GenerateViewDirection( ref dataCollector, UniqueId );

				dataCollector.AddLocalVariable( UniqueId, "UnityGIInput data;" );
				dataCollector.AddLocalVariable( UniqueId, "UNITY_INITIALIZE_OUTPUT( UnityGIInput, data );" );
				dataCollector.AddLocalVariable( UniqueId, "data.worldPos = " + worldPos + ";" );
				dataCollector.AddLocalVariable( UniqueId, "data.worldViewDir = " + viewDir + ";" );
				dataCollector.AddLocalVariable( UniqueId, "data.probeHDR[0] = unity_SpecCube0_HDR;" );
				dataCollector.AddLocalVariable( UniqueId, "data.probeHDR[1] = unity_SpecCube1_HDR;" );
				dataCollector.AddLocalVariable( UniqueId, "#if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION //specdataif0" );
				dataCollector.AddLocalVariable( UniqueId, "data.boxMin[0] = unity_SpecCube0_BoxMin;" );
				dataCollector.AddLocalVariable( UniqueId, "#endif //specdataif0" );
				dataCollector.AddLocalVariable( UniqueId, "#if UNITY_SPECCUBE_BOX_PROJECTION //specdataif1" );
				dataCollector.AddLocalVariable( UniqueId, "data.boxMax[0] = unity_SpecCube0_BoxMax;" );
				dataCollector.AddLocalVariable( UniqueId, "data.probePosition[0] = unity_SpecCube0_ProbePosition;" );
				dataCollector.AddLocalVariable( UniqueId, "data.boxMax[1] = unity_SpecCube1_BoxMax;" );
				dataCollector.AddLocalVariable( UniqueId, "data.boxMin[1] = unity_SpecCube1_BoxMin;" );
				dataCollector.AddLocalVariable( UniqueId, "data.probePosition[1] = unity_SpecCube1_ProbePosition;" );
				dataCollector.AddLocalVariable( UniqueId, "#endif //specdataif1" );
			}

			dataCollector.AddLocalVariable( UniqueId, "Unity_GlossyEnvironmentData g" + OutputId + " = UnityGlossyEnvironmentSetup( " + smoothness + ", " + viewDir + ", " + normal + ", float3(0,0,0));" );
			dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT3, "indirectSpecular" + OutputId, "UnityGI_IndirectSpecular( data, " + occlusion + ", " + normal + ", g" + OutputId + " )" );

			return "indirectSpecular" + OutputId;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if( UIUtils.CurrentShaderVersion() > 13002 )
				m_normalSpace = (ViewSpace)Enum.Parse( typeof( ViewSpace ), GetCurrentParam( ref nodeParams ) );

			if( UIUtils.CurrentShaderVersion() < 13804 )
			{
				m_errorMessageTooltip = "Smoothness port was previously being used as Roughness, please check if you are correctly using it and save to confirm.";
				m_upgradeMessage = true;
				UIUtils.ShowMessage( "Indirect Specular Light node: Smoothness port was previously being used as Roughness, please check if you are correctly using it and save to confirm." );
			}

			UpdatePort();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_normalSpace );

			m_errorMessageTooltip = DefaultErrorMessage;
			m_upgradeMessage = false;
		}
	}
}
