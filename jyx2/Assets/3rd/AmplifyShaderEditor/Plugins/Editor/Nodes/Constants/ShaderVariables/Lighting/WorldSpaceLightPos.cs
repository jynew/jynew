// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEditor;
using UnityEngine;
namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "World Space Light Pos", "Light", "Light Position" )]
	public sealed class WorldSpaceLightPos : ShaderVariablesNode
	{
		private const string HelperText =
		"This node will behave differently according to light type." +
		"\n\n- For directional lights the Dir/Pos output will specify a world space direction and Type will be set to 0." +
		"\n\n- For other light types the Dir/Pos output will specify a world space position and Type will be set to 1.";
		private const string m_lightPosValue = "_WorldSpaceLightPos0";

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputProperties( 0, Constants.EmptyPortValue, WirePortDataType.FLOAT4 );
			AddOutputPort( WirePortDataType.FLOAT3, "Dir/Pos" );
			AddOutputPort( WirePortDataType.FLOAT, "Type" );
			m_previewShaderGUID = "2292a614672283c41a367b22cdde4620";
			m_drawPreviewAsSphere = true;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUILayout.HelpBox( HelperText, MessageType.Info );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalVar );
			string finalVar = m_lightPosValue;
			if( dataCollector.IsTemplate && dataCollector.TemplateDataCollectorInstance.IsSRP )
				finalVar = "_MainLightPosition";
			if( outputId == 1 )
			{
				return finalVar + ".xyz";
			}
			else if( outputId == 2 )
			{
				return finalVar + ".w";
			}
			else
			{
				return finalVar;
			}
		}

		public override void RefreshExternalReferences()
		{
			base.RefreshExternalReferences();
			if( !m_outputPorts[ 0 ].IsConnected )
			{
				m_outputPorts[ 0 ].Visible = false;
				m_sizeIsDirty = true;
			}
		}

		public override void RenderNodePreview()
		{
			if( !HasPreviewShader || !m_initialized )
				return;

			SetPreviewInputs();

			RenderTexture temp = RenderTexture.active;

			RenderTexture.active = m_outputPorts[ 0 ].OutputPreviewTexture;
			Graphics.Blit( null, m_outputPorts[ 0 ].OutputPreviewTexture, PreviewMaterial, 0 );
			Graphics.Blit( m_outputPorts[ 0 ].OutputPreviewTexture, m_outputPorts[ 1 ].OutputPreviewTexture );

			RenderTexture.active = m_outputPorts[ 2 ].OutputPreviewTexture;
			Graphics.Blit( null, m_outputPorts[ 2 ].OutputPreviewTexture, PreviewMaterial, 1 );
			RenderTexture.active = temp;
		}
	}
}
