// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Indirect Diffuse Light", "Light", "Indirect Lighting", NodeAvailabilityFlags = (int)( NodeAvailability.CustomLighting | NodeAvailability.TemplateShader ) )]
	public sealed class IndirectDiffuseLighting : ParentNode
	{
		[SerializeField]
		private ViewSpace m_normalSpace = ViewSpace.Tangent;

		private int m_cachedIntensityId = -1;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "Normal" );
			AddOutputPort( WirePortDataType.FLOAT3, "RGB" );
			m_inputPorts[ 0 ].Vector3InternalData = Vector3.forward;
			m_autoWrapProperties = true;
			m_errorMessageTypeIsError = NodeMessageType.Warning;
			m_errorMessageTooltip = "This node only returns correct information using a custom light model, otherwise returns 0";
			m_previewShaderGUID = "b45d57fa606c1ea438fe9a2c08426bc7";
			m_drawPreviewAsSphere = true;
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if( m_inputPorts[ 0 ].IsConnected )
				m_previewMaterialPassId = 1;
			else
				m_previewMaterialPassId = 0;

			if( m_cachedIntensityId == -1 )
				m_cachedIntensityId = Shader.PropertyToID( "_Intensity" );

			PreviewMaterial.SetFloat( m_cachedIntensityId, RenderSettings.ambientIntensity );
		}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			base.PropagateNodeData( nodeData, ref dataCollector );
			// This needs to be rechecked
			//if( m_inputPorts[ 0 ].IsConnected )
			dataCollector.DirtyNormal = true;
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
			if( ( ContainerGraph.CurrentStandardSurface != null && ContainerGraph.CurrentStandardSurface.CurrentLightingModel != StandardShaderLightModel.CustomLighting ) )
				m_showErrorMessage = true;
			else
				m_showErrorMessage = false;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( dataCollector.IsTemplate && dataCollector.IsFragmentCategory )
			{
				dataCollector.AddToIncludes( UniqueId, Constants.UnityLightingLib );

				string texcoord1 = string.Empty;
				string texcoord2 = string.Empty;

				if( dataCollector.TemplateDataCollectorInstance.HasInfo( TemplateInfoOnSematics.TEXTURE_COORDINATES1, false, MasterNodePortCategory.Vertex ) )
					texcoord1 = dataCollector.TemplateDataCollectorInstance.GetInfo( TemplateInfoOnSematics.TEXTURE_COORDINATES1, false, MasterNodePortCategory.Vertex ).VarName;
				else
					texcoord1 = dataCollector.TemplateDataCollectorInstance.RegisterInfoOnSemantic( MasterNodePortCategory.Vertex, TemplateInfoOnSematics.TEXTURE_COORDINATES1, TemplateSemantics.TEXCOORD1, "texcoord1", WirePortDataType.FLOAT4, PrecisionType.Float, false );

				if( dataCollector.TemplateDataCollectorInstance.HasInfo( TemplateInfoOnSematics.TEXTURE_COORDINATES2, false, MasterNodePortCategory.Vertex ) )
					texcoord2 = dataCollector.TemplateDataCollectorInstance.GetInfo( TemplateInfoOnSematics.TEXTURE_COORDINATES2, false, MasterNodePortCategory.Vertex ).VarName;
				else
					texcoord2 = dataCollector.TemplateDataCollectorInstance.RegisterInfoOnSemantic( MasterNodePortCategory.Vertex, TemplateInfoOnSematics.TEXTURE_COORDINATES2, TemplateSemantics.TEXCOORD2, "texcoord2", WirePortDataType.FLOAT4, PrecisionType.Float, false );

				string vOutName = dataCollector.TemplateDataCollectorInstance.CurrentTemplateData.VertexFunctionData.OutVarName;
				string fInName = dataCollector.TemplateDataCollectorInstance.CurrentTemplateData.FragmentFunctionData.InVarName;
				TemplateVertexData data = dataCollector.TemplateDataCollectorInstance.RequestNewInterpolator( WirePortDataType.FLOAT4, false, "ase_lmap" );

				string varName = "ase_lmap";
				if( data != null )
					varName = data.VarName;

				dataCollector.AddToVertexLocalVariables( UniqueId, "#ifdef DYNAMICLIGHTMAP_ON //dynlm" );
				dataCollector.AddToVertexLocalVariables( UniqueId, vOutName + "." + varName + ".zw = " + texcoord2 + ".xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;" );
				dataCollector.AddToVertexLocalVariables( UniqueId, "#endif //dynlm" );
				dataCollector.AddToVertexLocalVariables( UniqueId, "#ifdef LIGHTMAP_ON //stalm" );
				dataCollector.AddToVertexLocalVariables( UniqueId, vOutName + "." + varName + ".xy = " + texcoord1 + ".xy * unity_LightmapST.xy + unity_LightmapST.zw;" );
				dataCollector.AddToVertexLocalVariables( UniqueId, "#endif //stalm" );

				TemplateVertexData shdata = dataCollector.TemplateDataCollectorInstance.RequestNewInterpolator( WirePortDataType.FLOAT3, false, "ase_sh" );
				string worldPos = dataCollector.TemplateDataCollectorInstance.GetWorldPos( false, MasterNodePortCategory.Vertex );
				string worldNormal = dataCollector.TemplateDataCollectorInstance.GetWorldNormal( PrecisionType.Float, false, MasterNodePortCategory.Vertex );
				//Debug.Log( shdata );
				string shVarName = "ase_sh";
				if( shdata != null )
					shVarName = shdata.VarName;
				string outSH = vOutName + "." + shVarName + ".xyz";
				dataCollector.AddToVertexLocalVariables( UniqueId, "#ifndef LIGHTMAP_ON //nstalm" );
				dataCollector.AddToVertexLocalVariables( UniqueId, "#if UNITY_SHOULD_SAMPLE_SH //sh" );
				dataCollector.AddToVertexLocalVariables( UniqueId, outSH + " = 0;" );
				dataCollector.AddToVertexLocalVariables( UniqueId, "#ifdef VERTEXLIGHT_ON //vl" );
				dataCollector.AddToVertexLocalVariables( UniqueId, outSH + " += Shade4PointLights (" );
				dataCollector.AddToVertexLocalVariables( UniqueId, "unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0," );
				dataCollector.AddToVertexLocalVariables( UniqueId, "unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb," );
				dataCollector.AddToVertexLocalVariables( UniqueId, "unity_4LightAtten0, " + worldPos + ", " + worldNormal + ");" );
				dataCollector.AddToVertexLocalVariables( UniqueId, "#endif //vl" );
				dataCollector.AddToVertexLocalVariables( UniqueId, outSH + " = ShadeSHPerVertex (" + worldNormal + ", " + outSH + ");" );
				dataCollector.AddToVertexLocalVariables( UniqueId, "#endif //sh" );
				dataCollector.AddToVertexLocalVariables( UniqueId, "#endif //nstalm" );

				string fragWorldNormal = string.Empty;
				if( m_inputPorts[ 0 ].IsConnected )
				{
					if( m_normalSpace == ViewSpace.Tangent )
						fragWorldNormal = dataCollector.TemplateDataCollectorInstance.GetWorldNormal( UniqueId, m_currentPrecisionType, m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector ), OutputId );
					else
						fragWorldNormal = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
				} else
				{
					fragWorldNormal = dataCollector.TemplateDataCollectorInstance.GetWorldNormal( PrecisionType.Float, false, MasterNodePortCategory.Fragment );
				}

				dataCollector.AddLocalVariable( UniqueId, "UnityGIInput data" + OutputId + ";" );
				dataCollector.AddLocalVariable( UniqueId, "UNITY_INITIALIZE_OUTPUT( UnityGIInput, data" + OutputId + " );" );

				dataCollector.AddLocalVariable( UniqueId, "#if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON) //dylm" + OutputId );
				dataCollector.AddLocalVariable( UniqueId, "data" + OutputId + ".lightmapUV = "+ fInName + "."+ varName + ";" );
				dataCollector.AddLocalVariable( UniqueId, "#endif //dylm" + OutputId );

				dataCollector.AddLocalVariable( UniqueId, "#if UNITY_SHOULD_SAMPLE_SH //fsh" + OutputId );
				dataCollector.AddLocalVariable( UniqueId, "data" + OutputId + ".ambient = " + fInName + "." + shVarName + ";" );
				dataCollector.AddLocalVariable( UniqueId, "#endif //fsh" + OutputId );

				dataCollector.AddToLocalVariables( UniqueId, "UnityGI gi" + OutputId + " = UnityGI_Base(data" + OutputId + ", 1, " + fragWorldNormal + ");" );

				return "gi" + OutputId + ".indirect.diffuse";
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


			if( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT3, "indirectDiffuse" + OutputId, "ShadeSH9( float4( " + normal + ", 1 ) )" );
			}
			else
			{
				dataCollector.AddLocalVariable( UniqueId, "UnityGI gi" + OutputId + " = gi;" );
				dataCollector.AddLocalVariable( UniqueId, PrecisionType.Float, WirePortDataType.FLOAT3, "diffNorm" + OutputId, normal );
				dataCollector.AddLocalVariable( UniqueId, "gi" + OutputId + " = UnityGI_Base( data, 1, diffNorm" + OutputId + " );" );
				dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT3, "indirectDiffuse" + OutputId, "gi" + OutputId + ".indirect.diffuse + diffNorm" + OutputId + " * 0.0001" );
			}

			return "indirectDiffuse" + OutputId;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if( UIUtils.CurrentShaderVersion() > 13002 )
				m_normalSpace = (ViewSpace)Enum.Parse( typeof( ViewSpace ), GetCurrentParam( ref nodeParams ) );

			UpdatePort();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_normalSpace );
		}
	}
}
