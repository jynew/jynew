//// Amplify Shader Editor - Visual Shader Editing Tool
//// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

//using UnityEngine;
//using UnityEditor;
//using System;

//namespace AmplifyShaderEditor
//{
//	[Serializable]
//	[NodeAttributes( "[Old]Texture Coordinates", "Surface Data", "Texture UV coordinates set", null, KeyCode.U, false )]
//	public sealed class UVCoordsParentNode : ParentNode
//	{
//		private const string TilingStr = "Tiling";

//		[SerializeField]
//		private int m_textureCoordChannel = 0;

//		[SerializeField]
//		private int m_textureCoordSet = 0;

//		[SerializeField]
//		private Vector2 m_tiling = new Vector2( 1, 1 );
		
//		protected override void CommonInit( int uniqueId )
//		{
//			base.CommonInit( uniqueId );
//			AddOutputVectorPorts( WirePortDataType.FLOAT2, Constants.EmptyPortValue );
//			m_textLabelWidth = 75;
//		}

//		public override void DrawProperties()
//		{
//			base.DrawProperties();
//			int newChannel = EditorGUILayoutIntPopup( Constants.AvailableUVChannelLabel, m_textureCoordChannel, Constants.AvailableUVChannelsStr, Constants.AvailableUVChannels );
//			if ( newChannel != m_textureCoordChannel )
//			{
//				if ( UIUtils.IsChannelAvailable( newChannel ) )
//				{
//					UIUtils.ShowMessage( "Attempting to use an unoccupied used texture channel" );
//				}
//				else
//				{
//					m_textureCoordChannel = newChannel;
//				}
//			}
//			else if ( m_textureCoordChannel > -1 && UIUtils.IsChannelAvailable( m_textureCoordChannel ) )
//			{
//				UIUtils.ShowMessage( "Texture Channel " + m_textureCoordChannel + " is unavailable for TextureCoordinate node" );
//				m_textureCoordChannel = -1;
//			}

//			m_textureCoordSet = EditorGUILayoutIntPopup( Constants.AvailableUVSetsLabel, m_textureCoordSet, Constants.AvailableUVSetsStr, Constants.AvailableUVSets );

//			m_tiling = EditorGUILayoutVector2Field( TilingStr, m_tiling );
//		}

//		public override void Draw( DrawInfo drawInfo )
//		{
//			base.Draw( drawInfo );
//			if ( m_isVisible )
//			{
//				m_propertyDrawPos.x = m_globalPosition.x + Constants.FLOAT_WIDTH_SPACING;
//				m_propertyDrawPos.y = m_outputPorts[ 1 ].Position.y;
//				m_propertyDrawPos.width = 2.7f * drawInfo.InvertedZoom * Constants.FLOAT_DRAW_WIDTH_FIELD_SIZE;
//				m_propertyDrawPos.height = drawInfo.InvertedZoom * Constants.FLOAT_DRAW_HEIGHT_FIELD_SIZE;

//				m_propertyDrawPos.y = m_outputPorts[ 1 ].Position.y;
//				UIUtils.DrawFloat( this, ref m_propertyDrawPos, ref m_tiling.x );

//				m_propertyDrawPos.y = m_outputPorts[ 2 ].Position.y;
//				UIUtils.DrawFloat( this, ref m_propertyDrawPos, ref m_tiling.y );
//			}
//		}

//		public override void ReadFromString( ref string[] nodeParams )
//		{
//			base.ReadFromString( ref nodeParams );
//			m_textureCoordChannel = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
//			m_tiling.x = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
//			m_tiling.y = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
//		}

//		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
//		{
//			base.WriteToString( ref nodeInfo, ref connectionsInfo );
//			IOUtils.AddFieldValueToString( ref nodeInfo, m_textureCoordChannel );
//			IOUtils.AddFieldValueToString( ref nodeInfo, m_tiling.x );
//			IOUtils.AddFieldValueToString( ref nodeInfo, m_tiling.y );
//		}

//		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
//		{
//			string uvChannelDeclaration = IOUtils.GetUVChannelDeclaration( UIUtils.GetChannelName( m_textureCoordChannel ), m_textureCoordChannel, m_textureCoordSet );
//			dataCollector.AddToInput( UniqueId, uvChannelDeclaration, true );

//			if ( dataCollector.GetChannelUsage( m_textureCoordChannel ) != TextureChannelUsage.Used )
//				dataCollector.SetChannelUsage( m_textureCoordChannel, TextureChannelUsage.Required );

//			string uvTileStr = string.Empty;
//			switch ( outputId )
//			{
//				case 0: { uvTileStr = "float2( " + m_tiling.x + " , " + m_tiling.y + " )"; } break;
//				case 1: { uvTileStr = m_tiling.x.ToString(); } break;
//				case 2: { uvTileStr = m_tiling.y.ToString(); } break;
//			}
//			string uvChannelName = IOUtils.GetUVChannelName( UIUtils.GetChannelName( m_textureCoordChannel ), m_textureCoordSet );
//			return ( uvTileStr + "*" + GetOutputVectorItem( 0, outputId, Constants.InputVarStr + "." + uvChannelName ) );
//		}

//	}
//}
