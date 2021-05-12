// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{

	public enum ASEStandardSurfaceWorkflow
	{
		Metallic = 0,
		Specular
	}

	[Serializable]
	[NodeAttributes( "Standard Surface Light", "Light", "Provides a way to create a standard surface light model in custom lighting mode", NodeAvailabilityFlags = (int)NodeAvailability.CustomLighting )]
	public sealed class CustomStandardSurface : ParentNode
	{
		private const string WorkflowStr = "Workflow";

		[SerializeField]
		private ASEStandardSurfaceWorkflow m_workflow = ASEStandardSurfaceWorkflow.Metallic;

		[SerializeField]
		private ViewSpace m_normalSpace = ViewSpace.Tangent;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "Albedo" );
			AddInputPort( WirePortDataType.FLOAT3, false, "Normal" );
			m_inputPorts[ 1 ].Vector3InternalData = Vector3.forward;
			AddInputPort( WirePortDataType.FLOAT3, false, "Emission" );
			AddInputPort( WirePortDataType.FLOAT, false, "Metallic" );
			AddInputPort( WirePortDataType.FLOAT, false, "Smoothness" );
			AddInputPort( WirePortDataType.FLOAT, false, "Occlusion" );
			m_inputPorts[ 5 ].FloatInternalData = 1;
			AddOutputPort( WirePortDataType.FLOAT3, "RGB" );
			m_autoWrapProperties = true;
			m_textLabelWidth = 100;
			m_errorMessageTypeIsError = NodeMessageType.Warning;
			m_errorMessageTooltip = "This node only returns correct information using a custom light model, otherwise returns 0";
		}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			base.PropagateNodeData( nodeData, ref dataCollector );
			if( m_inputPorts[ 1 ].IsConnected && m_normalSpace == ViewSpace.Tangent )
				dataCollector.DirtyNormal = true;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUI.BeginChangeCheck();
			m_workflow = (ASEStandardSurfaceWorkflow)EditorGUILayoutEnumPopup( WorkflowStr, m_workflow );
			if( EditorGUI.EndChangeCheck() )
			{
				UpdateSpecularMetallicPorts();
			}

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
				m_inputPorts[ 1 ].Name = "World Normal";
			else
				m_inputPorts[ 1 ].Name = "Normal";

			m_sizeIsDirty = true;
		}

		void UpdateSpecularMetallicPorts()
		{
			if( m_workflow == ASEStandardSurfaceWorkflow.Specular )
				m_inputPorts[ 3 ].ChangeProperties( "Specular", WirePortDataType.FLOAT3, false );
			else
				m_inputPorts[ 3 ].ChangeProperties( "Metallic", WirePortDataType.FLOAT, false );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( dataCollector.GenType == PortGenType.NonCustomLighting || dataCollector.CurrentCanvasMode != NodeAvailability.CustomLighting )
				return "float3(0,0,0)";

			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			string specularMode = string.Empty;
			if( m_workflow == ASEStandardSurfaceWorkflow.Specular )
				specularMode = "Specular";

			dataCollector.AddToInput( UniqueId, SurfaceInputs.WORLD_NORMAL, m_currentPrecisionType );

			if( dataCollector.DirtyNormal )
			{
				dataCollector.AddToInput( UniqueId, SurfaceInputs.INTERNALDATA, addSemiColon: false );
				dataCollector.ForceNormal = true;
			}

			dataCollector.AddLocalVariable( UniqueId, "SurfaceOutputStandard" + specularMode + " s" + OutputId + " = (SurfaceOutputStandard" + specularMode + " ) 0;" );
			dataCollector.AddLocalVariable( UniqueId, "s" + OutputId + ".Albedo = " + m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector ) + ";" );

			string normal = string.Empty;

			if( m_inputPorts[ 1 ].IsConnected )
			{
				normal = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );
				if( m_normalSpace == ViewSpace.Tangent )
				{
					normal = "WorldNormalVector( " + Constants.InputVarStr + " , " + normal + " )";
				}
			}
			else
			{
				normal = GeneratorUtils.GenerateWorldNormal( ref dataCollector, UniqueId );
			}



			dataCollector.AddLocalVariable( UniqueId, "s" + OutputId + ".Normal = "+ normal + ";" );
			dataCollector.AddLocalVariable( UniqueId, "s" + OutputId + ".Emission = " + m_inputPorts[ 2 ].GeneratePortInstructions( ref dataCollector ) + ";" );
			if( m_workflow == ASEStandardSurfaceWorkflow.Specular )
				dataCollector.AddLocalVariable( UniqueId, "s" + OutputId + ".Specular = " + m_inputPorts[ 3 ].GeneratePortInstructions( ref dataCollector ) + ";" );
			else
				dataCollector.AddLocalVariable( UniqueId, "s" + OutputId + ".Metallic = " + m_inputPorts[ 3 ].GeneratePortInstructions( ref dataCollector ) + ";" );
			dataCollector.AddLocalVariable( UniqueId, "s" + OutputId + ".Smoothness = " + m_inputPorts[ 4 ].GeneratePortInstructions( ref dataCollector ) + ";" );
			dataCollector.AddLocalVariable( UniqueId, "s" + OutputId + ".Occlusion = " + m_inputPorts[ 5 ].GeneratePortInstructions( ref dataCollector ) + ";\n" );

			dataCollector.AddLocalVariable( UniqueId, "data.light = gi.light;\n", true );

			dataCollector.AddLocalVariable( UniqueId, "UnityGI gi" + OutputId + " = gi;" );
			dataCollector.AddLocalVariable( UniqueId, "#ifdef UNITY_PASS_FORWARDBASE", true );
		
			dataCollector.AddLocalVariable( UniqueId, "Unity_GlossyEnvironmentData g" + OutputId + " = UnityGlossyEnvironmentSetup( s" + OutputId + ".Smoothness, data.worldViewDir, s" + OutputId + ".Normal, float3(0,0,0));" );
			dataCollector.AddLocalVariable( UniqueId, "gi" + OutputId + " = UnityGlobalIllumination( data, s" + OutputId + ".Occlusion, s" + OutputId + ".Normal, g" + OutputId + " );" );
			dataCollector.AddLocalVariable( UniqueId, "#endif\n", true );
			dataCollector.AddLocalVariable( UniqueId, "float3 surfResult" + OutputId + " = LightingStandard" + specularMode + " ( s" + OutputId + ", viewDir, gi" + OutputId + " ).rgb;" );
			dataCollector.AddLocalVariable( UniqueId, "surfResult" + OutputId + " += s" + OutputId + ".Emission;\n" );

			m_outputPorts[ 0 ].SetLocalValue( "surfResult" + OutputId, dataCollector.PortCategory );
			return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			if( ContainerGraph.CurrentCanvasMode == NodeAvailability.TemplateShader || ( ContainerGraph.CurrentStandardSurface != null && ContainerGraph.CurrentStandardSurface.CurrentLightingModel != StandardShaderLightModel.CustomLighting ) )
				m_showErrorMessage = true;
			else
				m_showErrorMessage = false;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if( UIUtils.CurrentShaderVersion() < 13204 )
			{
				m_workflow = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) ) ? ASEStandardSurfaceWorkflow.Specular : ASEStandardSurfaceWorkflow.Metallic;
			}
			else
			{
				m_workflow = (ASEStandardSurfaceWorkflow)Enum.Parse( typeof( ASEStandardSurfaceWorkflow ), GetCurrentParam( ref nodeParams ) );
			}
			UpdateSpecularMetallicPorts();

			if( UIUtils.CurrentShaderVersion() >= 14402 )
			{
				m_normalSpace = (ViewSpace)Enum.Parse( typeof( ViewSpace ), GetCurrentParam( ref nodeParams ) );
			}
			UpdatePort();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_workflow );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_normalSpace );
		}
	}
}
