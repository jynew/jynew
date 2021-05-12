// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Rotator", "UV Coordinates", "Rotates UVs or any Vector2 value from an Anchor point for a specified Time value")]
	public sealed class RotatorNode : ParentNode
	{
		private int m_cachedUsingEditorId = -1;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT2, false, "UV" );
			AddInputPort( WirePortDataType.FLOAT2, false, "Anchor" );
			AddInputPort( WirePortDataType.FLOAT, false, "Time" );
			AddOutputPort( WirePortDataType.FLOAT2, "Out" );
			m_useInternalPortData = true;
			m_inputPorts[ 2 ].FloatInternalData = 1;
			m_textLabelWidth = 50;
			m_previewShaderGUID = "e21408a1c7f12f14bbc2652f69bce1fc";
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if ( m_cachedUsingEditorId == -1 )
				m_cachedUsingEditorId = Shader.PropertyToID( "_UsingEditor" );

			PreviewMaterial.SetFloat( m_cachedUsingEditorId, (m_inputPorts[ 2 ].IsConnected ? 0 : 1 ) );
		}

		//public override void DrawProperties()
		//{
		//	base.DrawProperties();
		//	EditorGUILayout.HelpBox("Rotates UVs but can also be used to rotate other Vector2 values\n\nAnchor is the rotation point in UV space from which you rotate the UVs\nTime is the amount of rotation applied [0,1], if left unconnected it will use time as the default value", MessageType.None);
		//}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			string result = string.Empty;
			string uv = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string anchor = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );

			string time = m_inputPorts[ 2 ].GeneratePortInstructions( ref dataCollector );
			if ( !m_inputPorts[ 2 ].IsConnected )
			{
				if( !( dataCollector.IsTemplate && dataCollector.IsSRP ) )
					dataCollector.AddToIncludes( UniqueId, Constants.UnityShaderVariables );
				time += " * _Time.y";
			}

			result += uv;

			string cosVar = "cos" + OutputId;
			string sinVar = "sin" + OutputId;
			dataCollector.AddLocalVariable( UniqueId, "float " + cosVar + " = cos( "+time+" );");
			dataCollector.AddLocalVariable( UniqueId, "float " + sinVar + " = sin( "+time+" );");
			
			string value =  "mul( " + result + " - " + anchor + " , float2x2( "+cosVar+" , -"+sinVar+" , "+sinVar+" , "+cosVar+" )) + "+anchor;
			RegisterLocalVariable( 0, value, ref dataCollector, "rotator" + OutputId );

			return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
		}

		public override void RefreshExternalReferences()
		{
			base.RefreshExternalReferences();
			if( UIUtils.CurrentShaderVersion() < 13107 )
			{
				m_inputPorts[ 2 ].FloatInternalData = 1;
			}
		}
	}
}
