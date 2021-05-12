// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Vertex TexCoord", "Vertex Data", "Vertex texture coordinates, can be used in both local vertex offset and fragment outputs" )]
	public sealed class TexCoordVertexDataNode : VertexDataNode
	{
		[SerializeField]
		private int m_texcoordSize = 2;

		[SerializeField]
		private int m_index = 0;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentVertexData = "texcoord";
			ChangeOutputProperties( 0, "UV", WirePortDataType.FLOAT2, false );
			m_outputPorts[ 1 ].Name = "U";
			m_outputPorts[ 2 ].Name = "V";
			m_outputPorts[ 3 ].Visible = false;
			m_outputPorts[ 4 ].Visible = false;
			m_outputPorts[ 3 ].Name = "W";
			m_outputPorts[ 4 ].Name = "T";
			m_autoWrapProperties = true;
			m_hasLeftDropdown = true;
			m_previewShaderGUID = "6c1bee77276896041bbb73b1b9e7f8ac";
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUI.BeginChangeCheck();
			m_texcoordSize = EditorGUILayoutIntPopup( Constants.AvailableUVSizesLabel, m_texcoordSize, Constants.AvailableUVSizesStr, Constants.AvailableUVSizes );
			if( EditorGUI.EndChangeCheck() )
			{
				UpdateOutput();
			}

			EditorGUI.BeginChangeCheck();
			m_index = EditorGUILayoutIntPopup( Constants.AvailableUVChannelLabel, m_index, Constants.AvailableUVSetsStr, Constants.AvailableUVChannels );
			if( EditorGUI.EndChangeCheck() )
			{
				if( m_index > 3 && m_containerGraph.IsStandardSurface )
				{
					UIUtils.ShowMessage( "Standard Surface doesn't allow access to this channel" );
					m_index = 0;
				}

				m_currentVertexData = ( m_index == 0 ) ? "texcoord" : "texcoord" + Constants.AvailableUVChannelsStr[ m_index ];
			}
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );

			if( m_dropdownEditing )
			{
				EditorGUI.BeginChangeCheck();
				m_texcoordSize = EditorGUIIntPopup( m_dropdownRect, m_texcoordSize, Constants.AvailableUVSizesStr, Constants.AvailableUVSizes, UIUtils.PropertyPopUp );
				if( EditorGUI.EndChangeCheck() )
				{
					UpdateOutput();
					m_dropdownEditing = false;
				}
			}
		}

		private void UpdateOutput()
		{
			if( m_texcoordSize == 3 )
			{
				m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT3, false );
				m_outputPorts[ 0 ].Name = "UVW";
				m_outputPorts[ 3 ].Visible = true;
				m_outputPorts[ 4 ].Visible = false;
			}
			else if( m_texcoordSize == 4 )
			{
				m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4, false );
				m_outputPorts[ 0 ].Name = "UVWT";
				m_outputPorts[ 3 ].Visible = true;
				m_outputPorts[ 4 ].Visible = true;
			}
			else
			{
				m_texcoordSize = 2;
				m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT2, false );
				m_outputPorts[ 0 ].Name = "UV";
				m_outputPorts[ 3 ].Visible = false;
				m_outputPorts[ 4 ].Visible = false;
			}
			m_sizeIsDirty = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if( dataCollector.IsTemplate )
			{
				if( !dataCollector.TemplateDataCollectorInstance.HasUV( m_index ) )
				{
					dataCollector.TemplateDataCollectorInstance.RegisterUV( m_index, m_outputPorts[ 0 ].DataType );
				}

				if( dataCollector.TemplateDataCollectorInstance.HasUV( m_index ) )
				{
					InterpDataHelper info = dataCollector.TemplateDataCollectorInstance.GetUVInfo( m_index );
					if( outputId == 0 )
					{
						return dataCollector.TemplateDataCollectorInstance.GetUVName( m_index, m_outputPorts[ 0 ].DataType );
					}
					else if( outputId <= TemplateHelperFunctions.DataTypeChannelUsage[ info.VarType ] )
					{
						return GetOutputVectorItem( 0, outputId, info.VarName );
					}
					Debug.LogWarning( "Attempting to access inexisting UV channel" );
				}
				else
				{
					Debug.LogWarning( "Attempting to access non-registered UV" );
				}
				return "0";
			}

			if( dataCollector.PortCategory == MasterNodePortCategory.Fragment || dataCollector.PortCategory == MasterNodePortCategory.Debug )
			{
				if( m_texcoordSize > 2 )
					dataCollector.UsingHigherSizeTexcoords = true;
			}

			WirePortDataType size = (WirePortDataType)( 1 << ( m_texcoordSize + 1 ) );
			string texcoords = GeneratorUtils.GenerateAutoUVs( ref dataCollector, UniqueId, m_index, null, size );
			return GetOutputVectorItem( 0, outputId, texcoords );
		}

		/// <summary>
		/// Generates UV properties and uniforms and returns the varible name to use in the fragment shader
		/// </summary>
		/// <param name="dataCollector"></param>
		/// <param name="uniqueId"></param>
		/// <param name="index"></param>
		/// <returns>frag variable name</returns>
		static public string GenerateFragUVs( ref MasterNodeDataCollector dataCollector, int uniqueId, int index, string propertyName = null, WirePortDataType size = WirePortDataType.FLOAT2 )
		{
			string dummyPropUV = "_texcoord" + ( index > 0 ? ( index + 1 ).ToString() : "" );
			string dummyUV = "uv" + ( index > 0 ? ( index + 1 ).ToString() : "" ) + dummyPropUV;

			dataCollector.AddToProperties( uniqueId, "[HideInInspector] " + dummyPropUV + "( \"\", 2D ) = \"white\" {}", 100 );
			dataCollector.AddToInput( uniqueId, dummyUV, size );

			string result = Constants.InputVarStr + "." + dummyUV;
			if( !string.IsNullOrEmpty( propertyName ) )
			{
				dataCollector.AddToUniforms( uniqueId, "uniform float4 " + propertyName + "_ST;" );
				dataCollector.AddToLocalVariables( uniqueId, PrecisionType.Float, size, "uv" + propertyName, result + " * " + propertyName + "_ST.xy + " + propertyName + "_ST.zw" );
				result = "uv" + propertyName;
			}

			return result;
		}

		static public string GenerateVertexUVs( ref MasterNodeDataCollector dataCollector, int uniqueId, int index, string propertyName = null, WirePortDataType size = WirePortDataType.FLOAT2 )
		{

			string result = Constants.VertexShaderInputStr + ".texcoord";
			if( index > 0 )
			{
				result += index.ToString();
			}

			switch( size )
			{
				default:
				case WirePortDataType.FLOAT2:
				{
					result += ".xy";
				}
				break;
				case WirePortDataType.FLOAT3:
				{
					result += ".xyz";
				}
				break;
				case WirePortDataType.FLOAT4: break;
			}

			if( !string.IsNullOrEmpty( propertyName ) )
			{
				dataCollector.AddToUniforms( uniqueId, "uniform float4 " + propertyName + "_ST;" );
				dataCollector.AddToVertexLocalVariables( uniqueId, UIUtils.WirePortToCgType( size ) + " uv" + propertyName + " = " + Constants.VertexShaderInputStr + ".texcoord" + ( index > 0 ? index.ToString() : string.Empty ) + " * " + propertyName + "_ST.xy + " + propertyName + "_ST.zw;" );
				result = "uv" + propertyName;
			}

			return result;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if( UIUtils.CurrentShaderVersion() > 2502 )
			{
				m_index = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			}

			if( UIUtils.CurrentShaderVersion() > 5111 )
			{
				m_texcoordSize = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				UpdateOutput();
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_index );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_texcoordSize );
		}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			base.PropagateNodeData( nodeData, ref dataCollector );
			if( dataCollector.IsTemplate )
			{
				dataCollector.TemplateDataCollectorInstance.SetUVUsage( m_index, m_texcoordSize );
			}
		}

		public override void OnMasterNodeReplaced( MasterNode newMasterNode )
		{
			base.OnMasterNodeReplaced( newMasterNode );
			if( m_index > 3 && newMasterNode is StandardSurfaceOutputNode )
			{
				m_index = 0;
				UIUtils.ShowMessage( "Resetting channel on Vertex TexCoord node.\nStandard Surface only allows usage of the first four." );
			}
		}
	}
}
