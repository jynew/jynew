// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Texture Coordinates", "UV Coordinates", "Texture UV coordinates set, if <b>Tex</b> is connected to a texture object it will use that texture scale factors, otherwise uses <b>Tilling</b> and <b>Offset</b> port values", null, KeyCode.U )]
	public sealed class TextureCoordinatesNode : ParentNode
	{

		private const string DummyPropertyDec = "[HideInInspector] _DummyTex{0}( \"\", 2D ) = \"white\"";
		private const string DummyUniformDec = "uniform sampler2D _DummyTex{0};";
		private const string DummyTexCoordDef = "uv{0}_DummyTex{0}";
		private const string DummyTexCoordSurfDef = "float2 texCoordDummy{0} = {1}.uv{2}_DummyTex{2}*{3} + {4};";
		private const string DummyTexCoordSurfVar = "texCoordDummy{0}";

		private readonly string[] Dummy = { string.Empty };

		private const string TilingStr = "Tiling";
		private const string OffsetStr = "Offset";
		private const string TexCoordStr = "texcoord_";

		[SerializeField]
		private int m_referenceArrayId = -1;

		[SerializeField]
		private int m_referenceNodeId = -1;

		[SerializeField]
		private int m_textureCoordChannel = 0;

		//[SerializeField]
		//private int m_texcoordId = -1;

		[SerializeField]
		private int m_texcoordSize = 2;

		[SerializeField]
		private string m_surfaceTexcoordName = string.Empty;

		[SerializeField]
		private TexturePropertyNode m_inputReferenceNode = null;

		private TexturePropertyNode m_referenceNode = null;

		private InputPort m_texPort = null;
		private InputPort m_tilingPort = null;
		private InputPort m_offsetPort = null;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.SAMPLER2D, false, "Tex", -1, MasterNodePortCategory.Fragment, 2 );
			m_texPort = m_inputPorts[ m_inputPorts.Count - 1 ];
			m_texPort.CreatePortRestrictions( WirePortDataType.SAMPLER1D, WirePortDataType.SAMPLER2D, WirePortDataType.SAMPLER3D, WirePortDataType.SAMPLERCUBE, WirePortDataType.OBJECT );

			AddInputPort( WirePortDataType.FLOAT2, false, "Tiling", -1, MasterNodePortCategory.Fragment, 0 );
			m_tilingPort = m_inputPorts[ m_inputPorts.Count - 1 ];
			m_tilingPort.Vector2InternalData = new Vector2( 1, 1 );
			AddInputPort( WirePortDataType.FLOAT2, false, "Offset", -1, MasterNodePortCategory.Fragment, 1 );
			m_offsetPort = m_inputPorts[ m_inputPorts.Count - 1 ];


			AddOutputVectorPorts( WirePortDataType.FLOAT2, "UV" );
			m_outputPorts[ 1 ].Name = "U";
			m_outputPorts[ 2 ].Name = "V";
			AddOutputPort( WirePortDataType.FLOAT, "W" );
			AddOutputPort( WirePortDataType.FLOAT, "T" );
			m_textLabelWidth = 90;
			m_useInternalPortData = true;
			m_autoWrapProperties = true;
			m_tilingPort.Category = MasterNodePortCategory.Vertex;
			m_offsetPort.Category = MasterNodePortCategory.Vertex;
			UpdateOutput();
			m_previewShaderGUID = "085e462b2de441a42949be0e666cf5d2";
		}

		public override void Reset()
		{
			m_surfaceTexcoordName = string.Empty;
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			if( portId == 2 )
			{
				m_inputReferenceNode = m_texPort.GetOutputNode() as TexturePropertyNode;
				UpdatePorts();
			}
		}

		public override void OnInputPortDisconnected( int portId )
		{
			base.OnInputPortDisconnected( portId );
			if( portId == 2 )
			{
				m_inputReferenceNode = null;
				UpdatePorts();
			}
		}

		void UpdateTitle()
		{
			if( m_inputReferenceNode != null )
			{
				m_additionalContent.text = string.Format( "Value( {0} )", m_inputReferenceNode.PropertyInspectorName );
			}
			else if( m_referenceArrayId > -1 && m_referenceNode != null )
			{
				m_referenceNode = UIUtils.GetTexturePropertyNode( m_referenceArrayId );
				m_additionalContent.text = string.Format( "Value( {0} )", m_referenceNode.PropertyInspectorName );
			}
			else
			{
				m_additionalContent.text = string.Empty;
			}
			m_sizeIsDirty = true;
		}

		void UpdatePorts()
		{
			if( m_inputReferenceNode != null || m_texPort.IsConnected )
			{
				m_tilingPort.Locked = true;
				m_offsetPort.Locked = true;
			}
			else if( m_referenceArrayId > -1 )
			{
				m_tilingPort.Locked = true;
				m_offsetPort.Locked = true;
			}
			else
			{
				m_tilingPort.Locked = false;
				m_offsetPort.Locked = false;
			}
		}

		public override void DrawProperties()
		{
			bool guiEnabledBuffer = GUI.enabled;

			EditorGUI.BeginChangeCheck();
			List<string> arr = ( m_inputReferenceNode != null ) ? null : new List<string>( UIUtils.TexturePropertyNodeArr() );

			if( arr != null && arr.Count > 0 )
			{
				arr.Insert( 0, "None" );
				GUI.enabled = true;
				m_referenceArrayId = EditorGUILayoutPopup( Constants.AvailableReferenceStr, m_referenceArrayId + 1, arr.ToArray() ) - 1;
			}
			else
			{
				m_referenceArrayId = -1;
				GUI.enabled = false;
				EditorGUILayoutPopup( Constants.AvailableReferenceStr, 0, Dummy );
			}

			GUI.enabled = guiEnabledBuffer;
			if( EditorGUI.EndChangeCheck() )
			{
				m_referenceNode = UIUtils.GetTexturePropertyNode( m_referenceArrayId );
				if( m_referenceNode != null )
				{
					m_referenceNodeId = m_referenceNode.UniqueId;
				}
				else
				{
					m_referenceNodeId = -1;
					m_referenceArrayId = -1;
				}

				UpdateTitle();
				UpdatePorts();
			}

			EditorGUI.BeginChangeCheck();
			m_texcoordSize = EditorGUILayoutIntPopup( Constants.AvailableUVSizesLabel, m_texcoordSize, Constants.AvailableUVSizesStr, Constants.AvailableUVSizes );
			if( EditorGUI.EndChangeCheck() )
			{
				UpdateOutput();
			}

			EditorGUI.BeginChangeCheck();
			m_textureCoordChannel = EditorGUILayoutIntPopup( Constants.AvailableUVSetsLabel, m_textureCoordChannel, Constants.AvailableUVSetsStr, Constants.AvailableUVSets );
			if( EditorGUI.EndChangeCheck() )
			{
				if( m_textureCoordChannel > 3 && m_containerGraph.IsStandardSurface )
				{
					UIUtils.ShowMessage( "Standard Surface doesn't allow access to this channel" );
					m_textureCoordChannel = 0;
				}
			}

			if( m_referenceArrayId > -1 )
				GUI.enabled = false;

			base.DrawProperties();

			GUI.enabled = guiEnabledBuffer;
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
				m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT2, false );
				m_outputPorts[ 0 ].Name = "UV";
				m_outputPorts[ 3 ].Visible = false;
				m_outputPorts[ 4 ].Visible = false;
			}
			m_sizeIsDirty = true;
		}

		public override void OnNodeLogicUpdate( DrawInfo drawInfo )
		{
			base.OnNodeLogicUpdate( drawInfo );
			CheckReference();
		}

		//public override void Draw( DrawInfo drawInfo )
		//{
		//	base.Draw( drawInfo );
		//	//CheckReference();
		//}

		void CheckReference()
		{
			if( m_referenceArrayId > -1 )
			{
				ParentNode newNode = UIUtils.GetTexturePropertyNode( m_referenceArrayId );
				if( newNode == null || newNode.UniqueId != m_referenceNodeId )
				{
					m_referenceNode = null;
					int count = UIUtils.GetTexturePropertyNodeAmount();
					for( int i = 0; i < count; i++ )
					{
						ParentNode node = UIUtils.GetTexturePropertyNode( i );
						if( node.UniqueId == m_referenceNodeId )
						{
							m_referenceNode = node as TexturePropertyNode;
							m_referenceArrayId = i;
							break;
						}
					}
				}
			}

			if( m_referenceNode == null && m_referenceNodeId > -1 )
			{
				m_referenceNodeId = -1;
				m_referenceArrayId = -1;
				UpdateTitle();
				UpdatePorts();
			}
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_textureCoordChannel = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			if( UIUtils.CurrentShaderVersion() > 2402 )
			{
				if( UIUtils.CurrentShaderVersion() > 2404 )
				{
					m_referenceNodeId = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				}
				else
				{
					m_referenceArrayId = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				}
			}

			if( UIUtils.CurrentShaderVersion() > 5001 )
			{
				m_texcoordSize = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				UpdateOutput();
			}
		}


		public override void RefreshExternalReferences()
		{
			base.RefreshExternalReferences();
			if( UIUtils.CurrentShaderVersion() > 2402 )
			{
				if( UIUtils.CurrentShaderVersion() > 2404 )
				{
					m_referenceNode = UIUtils.GetNode( m_referenceNodeId ) as TexturePropertyNode;
					if( m_referenceNodeId > -1 )
						m_referenceArrayId = UIUtils.GetTexturePropertyNodeRegisterId( m_referenceNodeId );
				}
				else
				{
					m_referenceNode = UIUtils.GetTexturePropertyNode( m_referenceArrayId );
					if( m_referenceNode != null )
					{
						m_referenceNodeId = m_referenceNode.UniqueId;
					}
				}
				UpdateTitle();
				UpdatePorts();
			}
		}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			if( dataCollector != null && dataCollector.TesselationActive )
			{
				base.PropagateNodeData( nodeData, ref dataCollector );
				return;
			}

			if( dataCollector.IsTemplate )
			{
				dataCollector.TemplateDataCollectorInstance.SetUVUsage( m_textureCoordChannel, m_texcoordSize );
			}

			UIUtils.SetCategoryInBitArray( ref m_category, nodeData.Category );

			MasterNodePortCategory propagateCategory = ( nodeData.Category != MasterNodePortCategory.Vertex && nodeData.Category != MasterNodePortCategory.Tessellation ) ? MasterNodePortCategory.Vertex : nodeData.Category;
			nodeData.Category = propagateCategory;
			nodeData.GraphDepth += 1;
			if( nodeData.GraphDepth > m_graphDepth )
			{
				m_graphDepth = nodeData.GraphDepth;
			}

			int count = m_inputPorts.Count;
			for( int i = 0; i < count; i++ )
			{
				if( m_inputPorts[ i ].IsConnected )
				{
					//m_inputPorts[ i ].GetOutputNode().PropagateNodeCategory( category );
					m_inputPorts[ i ].GetOutputNode().PropagateNodeData( nodeData, ref dataCollector );
				}
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_textureCoordChannel );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( ( m_referenceNode != null ) ? m_referenceNode.UniqueId : -1 ) );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_texcoordSize );
		}

		string GetValidPropertyName()
		{
			string propertyName = string.Empty;
			if( m_inputReferenceNode != null )
			{
				propertyName = m_inputReferenceNode.PropertyName;
			}
			else if( m_referenceArrayId > -1 )
			{
				m_referenceNode = UIUtils.GetTexturePropertyNode( m_referenceArrayId );
				if( m_referenceNode != null )
				{
					propertyName = m_referenceNode.PropertyName;
				}
			}

			return propertyName;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if( dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				UIUtils.ShowMessage( m_nodeAttribs.Name + " cannot be used on Master Node Tessellation port" );
				return "-1";
			}

			//bool isVertex = ( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation );

			string tiling = string.Empty;
			string offset = string.Empty;

			string portProperty = string.Empty;
			if( m_texPort.IsConnected )
				portProperty = m_texPort.GeneratePortInstructions( ref dataCollector );

			if( m_referenceArrayId > -1 )
			{
				TexturePropertyNode temp = UIUtils.GetTexturePropertyNode( m_referenceArrayId );
				if( temp != null )
				{
					portProperty = temp.BaseGenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalVar );
				}
			}

			//TEMPLATES
			if( dataCollector.MasterNodeCategory == AvailableShaderTypes.Template )
			{
				if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
					return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );

				string uvName = string.Empty;
				if( dataCollector.TemplateDataCollectorInstance.HasUV( m_textureCoordChannel ) )
				{
					uvName = dataCollector.TemplateDataCollectorInstance.GetUVName( m_textureCoordChannel, m_outputPorts[ 0 ].DataType );
				}
				else
				{
					uvName = dataCollector.TemplateDataCollectorInstance.RegisterUV( m_textureCoordChannel, m_outputPorts[ 0 ].DataType );
				}
				string currPropertyName = GetValidPropertyName();
				if( !string.IsNullOrEmpty( portProperty ) && portProperty != "0.0" )
				{
					currPropertyName = portProperty;
				}
				if( !string.IsNullOrEmpty( currPropertyName ) )
				{
					string finalTexCoordName = "uv" + currPropertyName;
					string dummyPropertyTexcoords = currPropertyName + "_ST";
					dataCollector.AddToUniforms( UniqueId, "float4", dummyPropertyTexcoords );

					if( m_texcoordSize > 2 )
					{
						dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, m_outputPorts[ 0 ].DataType, finalTexCoordName, uvName );
						dataCollector.AddLocalVariable( UniqueId, finalTexCoordName + ".xy", string.Format( Constants.TilingOffsetFormat, uvName + ".xy", dummyPropertyTexcoords + ".xy", dummyPropertyTexcoords + ".zw" ) + ";" );
						m_outputPorts[ 0 ].SetLocalValue( finalTexCoordName, dataCollector.PortCategory );
					}
					else
					{
						RegisterLocalVariable( 0, string.Format( Constants.TilingOffsetFormat, uvName, dummyPropertyTexcoords + ".xy", dummyPropertyTexcoords + ".zw" ), ref dataCollector, finalTexCoordName );
					}
					//RegisterLocalVariable( 0, string.Format( Constants.TilingOffsetFormat, uvName, dummyPropertyTexcoords+".xy", dummyPropertyTexcoords+".zw" ), ref dataCollector, finalTexCoordName );
				}
				else
				{
					string finalTexCoordName = "uv" + OutputId;
					tiling = m_tilingPort.GeneratePortInstructions( ref dataCollector );
					offset = m_offsetPort.GeneratePortInstructions( ref dataCollector );

					if( m_texcoordSize > 2 )
					{
						dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, m_outputPorts[ 0 ].DataType, finalTexCoordName, uvName );
						dataCollector.AddLocalVariable( UniqueId, finalTexCoordName + ".xy", string.Format( Constants.TilingOffsetFormat, uvName + ".xy", tiling, offset ) + ";" );
						m_outputPorts[ 0 ].SetLocalValue( finalTexCoordName, dataCollector.PortCategory );
					}
					else
					{
						RegisterLocalVariable( 0, string.Format( Constants.TilingOffsetFormat, uvName, tiling, offset ), ref dataCollector, finalTexCoordName );
					}
					//RegisterLocalVariable( 0, string.Format( Constants.TilingOffsetFormat, uvName, tiling, offset ), ref dataCollector, finalTexCoordName );
				}
				return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );
			}

			//SURFACE
			string propertyName = GetValidPropertyName();
			if( !string.IsNullOrEmpty( portProperty ) && portProperty != "0.0" )
			{
				propertyName = portProperty;
			}

			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );

			if( !m_tilingPort.IsConnected && m_tilingPort.Vector2InternalData == Vector2.one )
				tiling = null;
			else
				tiling = m_tilingPort.GeneratePortInstructions( ref dataCollector );

			if( !m_offsetPort.IsConnected && m_offsetPort.Vector2InternalData == Vector2.zero )
				offset = null;
			else
				offset = m_offsetPort.GeneratePortInstructions( ref dataCollector );
			if( !string.IsNullOrEmpty( propertyName ) /*m_referenceArrayId > -1*/ )
			{
				////m_referenceNode = UIUtils.GetTexturePropertyNode( m_referenceArrayId );
				////if ( m_referenceNode != null )
				////{
				////string propertyName = m_referenceNode.PropertyName;
				//int coordSet = ( ( m_textureCoordChannel < 0 ) ? 0 : m_textureCoordChannel );
				//string uvName = string.Empty;

				//string dummyPropUV = "_tex" + ( m_texcoordSize > 2 ? "" + m_texcoordSize : "" ) + "coord" + ( coordSet > 0 ? ( coordSet + 1 ).ToString() : "" );
				//string dummyUV = "uv" + ( coordSet > 0 ? ( coordSet + 1 ).ToString() : "" ) + dummyPropUV;

				//if ( isVertex )
				//{
				//	uvName = IOUtils.GetUVChannelName( propertyName, coordSet ) + m_texcoordSize.ToString() + coordSet.ToString();
				//	string vertexInput = Constants.VertexShaderInputStr + ".texcoord";
				//	if ( coordSet > 0 )
				//	{
				//		vertexInput += coordSet.ToString();
				//	}

				//	dataCollector.AddToVertexLocalVariables( UniqueId, "float" + m_texcoordSize + " " + uvName + " = " + vertexInput + ";" );
				//	dataCollector.AddToVertexLocalVariables( UniqueId, uvName + ".xy = " + vertexInput + ".xy * " + propertyName + "_ST.xy + " + propertyName + "_ST.zw;" );
				//}
				//else
				//{
				//	uvName = IOUtils.GetUVChannelName( propertyName, coordSet );
				//	if ( m_texcoordSize > 2 )
				//	{
				//		uvName += m_texcoordSize;
				//		dataCollector.UsingHigherSizeTexcoords = true;
				//		dataCollector.AddToLocalVariables( UniqueId, "float" + m_texcoordSize + " " + uvName + " = " + Constants.InputVarStr + "." + dummyUV + ";" );
				//		dataCollector.AddToLocalVariables( UniqueId, uvName + ".xy = " + Constants.InputVarStr + "." + dummyUV + ".xy * " + propertyName + "_ST.xy + " + propertyName + "_ST.zw;" );
				//	}
				//	else
				//	{
				//		dataCollector.AddToLocalVariables( UniqueId, PrecisionType.Float, WirePortDataType.FLOAT2, uvName, Constants.InputVarStr + "." + dummyUV + " * " + propertyName + "_ST.xy + " + propertyName + "_ST.zw" );
				//	}
				//}

				//dataCollector.AddToUniforms( UniqueId, "uniform float4 " + propertyName + "_ST;" );
				//dataCollector.AddToProperties( UniqueId, "[HideInInspector] " + dummyPropUV + "( \"\", 2D ) = \"white\" {}", 100 );
				//dataCollector.AddToInput( UniqueId, "float" + m_texcoordSize + " " + dummyUV, true );

				//return GetOutputVectorItem( 0, outputId, uvName );
				////}

				m_surfaceTexcoordName = GeneratorUtils.GenerateAutoUVs( ref dataCollector, UniqueId, m_textureCoordChannel, propertyName, m_outputPorts[ 0 ].DataType, tiling, offset, OutputId );
			}
			else
			{
				m_surfaceTexcoordName = GeneratorUtils.GenerateAutoUVs( ref dataCollector, UniqueId, m_textureCoordChannel, null, m_outputPorts[ 0 ].DataType, tiling, offset, OutputId );
			}
			//{
			//	if ( !isVertex && dataCollector.TesselationActive )
			//	{
			//		return GenerateFragShaderForOutput( outputId, ref dataCollector, ignoreLocalVar );
			//	}

			//	bool tessVertexMode = isVertex && dataCollector.TesselationActive;

			//	string texcoordName = TexCoordStr + OutputId;

			//	string uvChannel = m_textureCoordChannel == 0 ? ".xy" : m_textureCoordChannel + ".xy";

			//	MasterNodePortCategory portCategory = dataCollector.PortCategory;
			//	if ( dataCollector.PortCategory != MasterNodePortCategory.Vertex && dataCollector.PortCategory != MasterNodePortCategory.Tessellation )
			//		dataCollector.PortCategory = MasterNodePortCategory.Vertex;

			//	// We need to reset local variables if there are already created to force them to be created in the vertex function
			//	ContainerGraph.ResetNodesLocalVariablesIfNot( this, MasterNodePortCategory.Vertex );

			//	bool dirtySpecialVarsBefore = dataCollector.DirtySpecialLocalVariables;
			//	bool dirtyVertexVarsBefore = dataCollector.DirtyVertexVariables;

			//	string tiling = m_tilingPort.GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT2, false, true );
			//	string offset = m_offsetPort.GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT2, false, true );
			//	dataCollector.PortCategory = portCategory;

			//	string vertexUV = Constants.VertexShaderInputStr + ".texcoord" + uvChannel;

			//	if ( !tessVertexMode )
			//		dataCollector.AddToInput( UniqueId, "float" + m_texcoordSize + " " + texcoordName, true );

			//	bool resetLocals = false;
			//	// new texture coordinates are calculated on the vertex shader so we need to register its local vars
			//	if ( !dirtySpecialVarsBefore && dataCollector.DirtySpecialLocalVariables )
			//	{
			//		dataCollector.AddVertexInstruction( dataCollector.SpecialLocalVariables, UniqueId, false );
			//		dataCollector.ClearSpecialLocalVariables();
			//		resetLocals = true;
			//	}

			//	if ( !dirtyVertexVarsBefore && dataCollector.DirtyVertexVariables )
			//	{
			//		dataCollector.AddVertexInstruction( dataCollector.VertexLocalVariables, UniqueId, false );
			//		dataCollector.ClearVertexLocalVariables();
			//		resetLocals = true;
			//	}

			//	//Reset local variables again so they wont be caught on the fragment shader
			//	if ( resetLocals )
			//		ContainerGraph.ResetNodesLocalVariablesIfNot( this, MasterNodePortCategory.Vertex );

			//	if ( tessVertexMode )
			//	{
			//		dataCollector.AddToVertexLocalVariables(UniqueId, vertexUV + " = " + vertexUV + " * " + tiling + " + " + offset+";");
			//		m_surfaceTexcoordName = Constants.VertexShaderInputStr + "." + IOUtils.GetVertexUVChannelName( m_textureCoordChannel ) + ".xy";
			//	}
			//	else if( dataCollector.TesselationActive )
			//	{
			//		string texCoordPrefix = ( m_textureCoordChannel == 0 ) ? string.Empty : ( m_textureCoordChannel + 1 ).ToString();

			//		dataCollector.AddToProperties( UniqueId, string.Format( DummyPropertyDec, texCoordPrefix ) + " {}", -1 );
			//		dataCollector.AddToUniforms( UniqueId, string.Format( DummyUniformDec, texCoordPrefix ) );

			//		dataCollector.AddToInput( UniqueId, "float2 " + string.Format( DummyTexCoordDef, texCoordPrefix ), true );
			//		dataCollector.AddToSpecialLocalVariables( UniqueId, string.Format( DummyTexCoordSurfDef, OutputId, Constants.InputVarStr, texCoordPrefix, tiling, offset ) );
			//		m_surfaceTexcoordName = string.Format( DummyTexCoordSurfVar, OutputId );
			//	}
			//	else
			//	{
			//		//m_surfaceTexcoordName = GeneratorUtils.GenerateAutoUVs( ref dataCollector, UniqueId, m_textureCoordChannel, null, m_texcoordSize == 2 ? WirePortDataType.FLOAT2 : WirePortDataType.FLOAT3, tiling, offset, OutputId );
			//		dataCollector.AddToVertexLocalVariables( UniqueId, Constants.VertexShaderOutputStr + "." + texcoordName + ".xy = " + vertexUV + " * " + tiling + " + " + offset + ";" );
			//		m_surfaceTexcoordName = ( isVertex ? Constants.VertexShaderOutputStr : Constants.InputVarStr ) + "." + texcoordName;
			//	}
			//}

			m_outputPorts[ 0 ].SetLocalValue( m_surfaceTexcoordName, dataCollector.PortCategory );
			return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );
		}

		//public string GenerateFragShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		//{
		//	string tiling = m_tilingPort.GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT2, false, true );
		//	string offset = m_offsetPort.GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT2, false, true );

		//	string texCoordPrefix = ( m_textureCoordChannel == 0 ) ? string.Empty : ( m_textureCoordChannel + 1 ).ToString();


		//	dataCollector.AddToProperties( UniqueId, string.Format( DummyPropertyDec, texCoordPrefix ) + " {}", -1 );
		//	dataCollector.AddToUniforms( UniqueId, string.Format( DummyUniformDec, texCoordPrefix ) );

		//	dataCollector.AddToInput( UniqueId, "float2 " + string.Format( DummyTexCoordDef, texCoordPrefix ), true );
		//	dataCollector.AddToSpecialLocalVariables( UniqueId, string.Format( DummyTexCoordSurfDef, OutputId, Constants.InputVarStr, texCoordPrefix, tiling, offset ) );
		//	m_surfaceTexcoordName = string.Format( DummyTexCoordSurfVar, OutputId );

		//	return GetOutputVectorItem( 0, outputId, m_surfaceTexcoordName );
		//}


		public override void ReadInputDataFromString( ref string[] nodeParams )
		{
			if( UIUtils.CurrentShaderVersion() > 7003 )
			{
				base.ReadInputDataFromString( ref nodeParams );
			}
			else
			{
				for( int i = 0; i < 2 && i < nodeParams.Length && m_currentReadParamIdx < nodeParams.Length; i++ )
				{
					if( UIUtils.CurrentShaderVersion() < 5003 )
					{
						int newId = VersionConvertInputPortId( i ) + 1;
						if( UIUtils.CurrentShaderVersion() > 23 )
						{
							m_inputPorts[ newId ].DataType = (WirePortDataType)Enum.Parse( typeof( WirePortDataType ), nodeParams[ m_currentReadParamIdx++ ] );
						}

						m_inputPorts[ newId ].InternalData = nodeParams[ m_currentReadParamIdx++ ];
						if( m_inputPorts[ newId ].IsEditable && UIUtils.CurrentShaderVersion() >= 3100 && m_currentReadParamIdx < nodeParams.Length )
						{
							m_inputPorts[ newId ].Name = nodeParams[ m_currentReadParamIdx++ ];
						}
					}
					else
					{
						int portId = Convert.ToInt32( nodeParams[ m_currentReadParamIdx++ ] );
						WirePortDataType DataType = (WirePortDataType)Enum.Parse( typeof( WirePortDataType ), nodeParams[ m_currentReadParamIdx++ ] );
						string InternalData = nodeParams[ m_currentReadParamIdx++ ];
						bool isEditable = Convert.ToBoolean( nodeParams[ m_currentReadParamIdx++ ] );
						string Name = string.Empty;
						if( isEditable && m_currentReadParamIdx < nodeParams.Length )
						{
							Name = nodeParams[ m_currentReadParamIdx++ ];
						}

						InputPort inputPort = GetInputPortByUniqueId( portId );
						if( inputPort != null )
						{
							inputPort.DataType = DataType;
							inputPort.InternalData = InternalData;
							if( !string.IsNullOrEmpty( Name ) )
							{
								inputPort.Name = Name;
							}
						}
					}
				}
			}
		}

		public override void OnMasterNodeReplaced( MasterNode newMasterNode )
		{
			base.OnMasterNodeReplaced( newMasterNode );
			if( m_textureCoordChannel > 3 && newMasterNode is StandardSurfaceOutputNode )
			{
				m_textureCoordChannel = 0;
				UIUtils.ShowMessage( "Resetting channel on Texture Coordinates node.\nStandard Surface only allows usage of the first four." );
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			m_referenceNode = null;
		}

	}
}
