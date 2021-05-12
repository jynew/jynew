// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Vertex Position", "Vertex Data", "Vertex position vector in object space, can be used in both local vertex offset and fragment outputs" )]
	public sealed class PosVertexDataNode : VertexDataNode
	{
		private const string PropertyLabel = "Size";
		private readonly string[] SizeLabels = { "XYZ", "XYZW" };

		[SerializeField]
		private int m_sizeOption = 0;

		private UpperLeftWidgetHelper m_upperLeftWidget = new UpperLeftWidgetHelper();

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentVertexData = "vertex";
			ChangeOutputProperties( 0, "XYZ", WirePortDataType.FLOAT3 );
			m_drawPreviewAsSphere = true;
			m_outputPorts[ 4 ].Visible = false;
			m_hasLeftDropdown = true;
			m_autoWrapProperties = true;
			m_previewShaderGUID = "a5c14f759dd021b4b8d4b6eeb85ac227";
		}

		public override void Destroy()
		{
			base.Destroy();
			m_upperLeftWidget = null;
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			EditorGUI.BeginChangeCheck();
			m_sizeOption = m_upperLeftWidget.DrawWidget( this, m_sizeOption, SizeLabels );
			if( EditorGUI.EndChangeCheck() )
			{
				UpdatePorts();
			}
		}

		public override void DrawProperties()
		{
			EditorGUI.BeginChangeCheck();
			m_sizeOption = EditorGUILayoutPopup( PropertyLabel, m_sizeOption, SizeLabels );
			if ( EditorGUI.EndChangeCheck() )
			{
				UpdatePorts();
			}
		}

		void UpdatePorts()
		{
			if ( m_sizeOption == 0 )
			{
				ChangeOutputProperties( 0, SizeLabels[ 0 ], WirePortDataType.FLOAT3, false );
				m_outputPorts[ 4 ].Visible = false;
			}
			else
			{
				ChangeOutputProperties( 0, SizeLabels[ 1 ], WirePortDataType.FLOAT4, false );
				m_outputPorts[ 4 ].Visible = true;
			}
		}
		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{

			if ( dataCollector.MasterNodeCategory == AvailableShaderTypes.Template )
			{
				string vertexPos = dataCollector.TemplateDataCollectorInstance.GetVertexPosition( ( m_sizeOption == 0 ) ? WirePortDataType.FLOAT3 : WirePortDataType.FLOAT4, m_currentPrecisionType );
				return GetOutputVectorItem( 0, outputId, vertexPos );
			}


			if ( dataCollector.PortCategory == MasterNodePortCategory.Fragment || dataCollector.PortCategory == MasterNodePortCategory.Debug )
				base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalVar );

			WirePortDataType sizeType = m_sizeOption == 0 ? WirePortDataType.FLOAT3 : WirePortDataType.FLOAT4;

			string vertexPosition = GeneratorUtils.GenerateVertexPosition( ref dataCollector, UniqueId, sizeType );
			return GetOutputVectorItem( 0, outputId, vertexPosition );

			//if ( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			//{
			//	string vertexVar = base.GenerateShaderForOutput( 0, ref dataCollector, ignoreLocalVar );
			//	if ( outputId != 0 )
			//	{
			//		return GetOutputVectorItem( 0, outputId, vertexVar );
			//	}
			//	else if ( m_sizeOption == 0 )
			//	{
			//		vertexVar += ".xyz";
			//	}

			//	return vertexVar;
			//}
			//else
			//{

			//	string vertexVar = GeneratorUtils.GenerateVertexPositionOnFrag( ref dataCollector, UniqueId, m_currentPrecisionType );
			//	if ( outputId != 0 )
			//	{
			//		return GetOutputVectorItem( 0, outputId, vertexVar );
			//	}
			//	else if ( m_sizeOption == 0 )
			//	{
			//		vertexVar += ".xyz";
			//	}
			//	return GetOutputVectorItem( 0, outputId, vertexVar );
			//}
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if ( UIUtils.CurrentShaderVersion() > 7101 )
			{
				m_sizeOption = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				UpdatePorts();
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_sizeOption );
		}
	}
}
