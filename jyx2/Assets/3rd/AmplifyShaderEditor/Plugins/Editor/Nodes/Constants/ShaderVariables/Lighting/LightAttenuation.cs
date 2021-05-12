// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Light Attenuation", "Light", "Contains light attenuation for all types of light", NodeAvailabilityFlags = (int)( NodeAvailability.CustomLighting | NodeAvailability.TemplateShader ) )]
	public sealed class LightAttenuation : ParentNode
	{
		static readonly string SurfaceError = "This node only returns correct information using a custom light model, otherwise returns 1";
		static readonly string TemplateError = "This node will only produce proper attenuation if the template contains a shadow caster pass";

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputPort( WirePortDataType.FLOAT, "Out" );
			m_errorMessageTypeIsError = NodeMessageType.Warning;
			m_errorMessageTooltip = SurfaceError;
			m_previewShaderGUID = "4b12227498a5c8d46b6c44ea018e5b56";
			m_drawPreviewAsSphere = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( dataCollector.IsTemplate && !dataCollector.IsSRP )
			{
				return dataCollector.TemplateDataCollectorInstance.GetLightAtten( UniqueId ); ;
			}

			if ( dataCollector.GenType == PortGenType.NonCustomLighting || dataCollector.CurrentCanvasMode != NodeAvailability.CustomLighting )
                return "1";

			dataCollector.UsingLightAttenuation = true;
			return "ase_lightAtten";
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			if( ContainerGraph.CurrentCanvasMode == NodeAvailability.TemplateShader )
			{
				m_showErrorMessage = true;
				m_errorMessageTypeIsError = NodeMessageType.Warning;
				m_errorMessageTooltip = TemplateError;
			} else
			{
				m_errorMessageTypeIsError = NodeMessageType.Error;
				m_errorMessageTooltip = SurfaceError;
				if ( ( ContainerGraph.CurrentStandardSurface != null && ContainerGraph.CurrentStandardSurface.CurrentLightingModel != StandardShaderLightModel.CustomLighting ) )
					m_showErrorMessage = true;
				else
					m_showErrorMessage = false;
			}


		}
	}
}
