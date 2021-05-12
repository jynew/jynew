// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "World Space Light Dir", "Light", "Computes normalized world space light direction" )]
	public sealed class WorldSpaceLightDirHlpNode : HelperParentNode
	{
		private const string NormalizeOptionStr = "Safe Normalize";

		[SerializeField]
		private bool m_safeNormalize = false;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_funcType = "UnityWorldSpaceLightDir";
			m_inputPorts[ 0 ].Visible = false;
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT3, false );
			m_outputPorts[ 0 ].Name = "XYZ";

			AddOutputPort( WirePortDataType.FLOAT, "X" );
			AddOutputPort( WirePortDataType.FLOAT, "Y" );
			AddOutputPort( WirePortDataType.FLOAT, "Z" );

			m_useInternalPortData = false;
			m_drawPreviewAsSphere = true;
			m_autoWrapProperties = true;
			m_textLabelWidth = 120;
			m_previewShaderGUID = "2e8dc46eb6fb2124d9f0007caf9567e3";
		}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			base.PropagateNodeData( nodeData, ref dataCollector );
			if( m_safeNormalize )
				dataCollector.SafeNormalizeLightDir = true;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			m_safeNormalize = EditorGUILayoutToggle( NormalizeOptionStr, m_safeNormalize );
			EditorGUILayout.HelpBox( "Having safe normalize ON makes sure your light vector is not zero even if there's no lights in your scene.", MessageType.None );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( dataCollector.IsTemplate )
				return GetOutputVectorItem( 0, outputId, dataCollector.TemplateDataCollectorInstance.GetWorldSpaceLightDir() ); ;

			dataCollector.AddToIncludes( UniqueId, Constants.UnityCgLibFuncs );
			dataCollector.AddToInput( UniqueId, SurfaceInputs.WORLD_POS );

			return GetOutputVectorItem( 0, outputId, GeneratorUtils.GenerateWorldLightDirection( ref dataCollector, UniqueId, m_currentPrecisionType ) );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if( UIUtils.CurrentShaderVersion() > 15201 )
			{
				m_safeNormalize = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_safeNormalize );
		}
	}
}
