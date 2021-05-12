// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Blend Normals", "Textures", "Blend Normals" )]
	public class BlendNormalsNode : ParentNode
	{
		public readonly static string[] ModeListStr = { "Tangent Normals", "Reoriented Tangent Normals", "Reoriented World Normals" };
		public readonly static int[] ModeListInt = { 0, 1, 2 };

		[SerializeField]
		public int m_selectedMode = 0;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "Normal A" );
			AddInputPort( WirePortDataType.FLOAT3, false, "Normal B" );
			AddInputPort( WirePortDataType.FLOAT3, false, "Vertex Normal" );
			m_inputPorts[ 2 ].Visible = false;
			AddOutputPort( WirePortDataType.FLOAT3, "XYZ" );
			m_useInternalPortData = true;
			m_previewShaderGUID = "bcdf750ff5f70444f98b8a3efa50dc6f";
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( !( dataCollector.IsTemplate && dataCollector.IsSRP ) )
				dataCollector.AddToIncludes( UniqueId, Constants.UnityStandardUtilsLibFuncs );

			string _inputA = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string _inputB = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );
			string result = "BlendNormals( " + _inputA + " , " + _inputB + " )";

			if( dataCollector.IsTemplate && dataCollector.IsSRP )
			{
				switch( m_selectedMode )
				{
					default:
					case 0:
					result = "BlendNormal( " + _inputA + " , " + _inputB + " )";
					break;
					case 1:
					result = "BlendNormalRNM( " + _inputA + " , " + _inputB + " )";
					break;
					case 2:
					string inputC = m_inputPorts[ 2 ].GeneratePortInstructions( ref dataCollector );
					result = "BlendNormalWorldspaceRNM( " + _inputA + " , " + _inputB + ", " + inputC + " )";
					break;
				}
			}
			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			if( ContainerGraph.IsSRP )
			{
				NodeUtils.DrawPropertyGroup( ref m_propertiesFoldout, Constants.ParameterLabelStr, () =>
				{
					EditorGUI.BeginChangeCheck();
					m_selectedMode = EditorGUILayoutIntPopup( "Mode", m_selectedMode, ModeListStr, ModeListInt );
					if( EditorGUI.EndChangeCheck() )
					{
						if( m_selectedMode == 2 )
						{
							m_inputPorts[ 2 ].Visible = true;
						}
						else
						{
							m_inputPorts[ 2 ].Visible = false;
						}
						m_sizeIsDirty = true;
					}
				} );
			}
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if( UIUtils.CurrentShaderVersion() > 14503 )
				m_selectedMode = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_selectedMode );
		}
	}
}
