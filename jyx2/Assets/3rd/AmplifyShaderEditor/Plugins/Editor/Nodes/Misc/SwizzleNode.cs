// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
//
// Custom Node Swizzle 
// Donated by Tobias Pott - @ Tobias Pott
// www.tobiaspott.de

using System;
using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Swizzle", "Vector Operators", "Swizzle components of vector types", null, KeyCode.Z, true, false, null, null, "Tobias Pott - @TobiasPott" )]
	public sealed class SwizzleNode : SingleInputOp
	{

		private const string OutputTypeStr = "Output type";

		[SerializeField]
		private WirePortDataType m_selectedOutputType = WirePortDataType.FLOAT4;

		[SerializeField]
		private int m_selectedOutputTypeInt = 3;

		[SerializeField]
		private int[] m_selectedOutputSwizzleTypes = new int[] { 0, 1, 2, 3 };

		[SerializeField]
		private int m_maskId;

		[SerializeField]
		private Vector4 m_maskValue = Vector4.one;

		private readonly string[] SwizzleVectorChannels = { "x", "y", "z", "w" };
		private readonly string[] SwizzleColorChannels = { "r", "g", "b", "a" };
		private readonly string[] SwizzleChannelLabels = { "Channel 0", "Channel 1", "Channel 2", "Channel 3" };

		private readonly string[] m_outputValueTypes ={  "Float",
														"Vector 2",
														"Vector 3",
														"Vector 4"};

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_inputPorts[ 0 ].CreatePortRestrictions( WirePortDataType.FLOAT,
														WirePortDataType.FLOAT2,
														WirePortDataType.FLOAT3,
														WirePortDataType.FLOAT4,
														WirePortDataType.COLOR,
														WirePortDataType.INT );


			m_inputPorts[ 0 ].DataType = WirePortDataType.FLOAT4;
			m_outputPorts[ 0 ].DataType = m_selectedOutputType;
			m_textLabelWidth = 90;
			m_autoWrapProperties = true;
			m_autoUpdateOutputPort = false;
			m_hasLeftDropdown = true;
			m_previewShaderGUID = "d20531704ce28b14bafb296f291f6608";
			SetAdditonalTitleText( "Value( XYZW )" );
			CalculatePreviewData();
		}

		public override void OnEnable()
		{
			base.OnEnable();
			m_maskId = Shader.PropertyToID( "_Mask" );
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();
			PreviewMaterial.SetVector( m_maskId, m_maskValue );
		}

		void CalculatePreviewData()
		{
			switch( m_outputPorts[ 0 ].DataType )
			{
				default: m_maskValue = Vector4.zero; break;
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT: m_maskValue = new Vector4( 1, 0, 0, 0 ); break;
				case WirePortDataType.FLOAT2: m_maskValue = new Vector4( 1, 1, 0, 0 ); break;
				case WirePortDataType.FLOAT3: m_maskValue = new Vector4( 1, 1, 1, 0 ); break;
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR: m_maskValue = Vector4.one; break;
			}

			m_previewMaterialPassId = -1;
			float passValue = 0;
			for( int i = 3; i > -1; i-- )
			{
				if( m_selectedOutputSwizzleTypes[ i ] > 0 )
				{
					passValue += Mathf.Pow( 4, 3 - i ) * m_selectedOutputSwizzleTypes[ i ];
				}
			}

			m_previewMaterialPassId = (int)passValue;

			if( m_previewMaterialPassId == -1 )
			{
				m_previewMaterialPassId = 0;
				if( DebugConsoleWindow.DeveloperMode )
				{
					UIUtils.ShowMessage( "Could not find pass ID for swizzle", MessageSeverity.Error );
				}
			}
		}

		public override void AfterCommonInit()
		{
			base.AfterCommonInit();

			if ( PaddingTitleLeft == 0 )
			{
				PaddingTitleLeft = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
				if ( PaddingTitleRight == 0 )
					PaddingTitleRight = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
			}
		}

		public override void OnConnectedOutputNodeChanges( int outputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( outputPortId, otherNodeId, otherPortId, name, type );
			UpdatePorts();
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			UpdatePorts();
		}

		public override void OnInputPortDisconnected( int portId )
		{
			base.OnInputPortDisconnected( portId );
			UpdatePorts();
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );

			if ( m_dropdownEditing )
			{
				EditorGUI.BeginChangeCheck();
				m_selectedOutputTypeInt = EditorGUIPopup( m_dropdownRect, m_selectedOutputTypeInt, m_outputValueTypes, UIUtils.PropertyPopUp );
				if ( EditorGUI.EndChangeCheck() )
				{
					switch ( m_selectedOutputTypeInt )
					{
						case 0: m_selectedOutputType = WirePortDataType.FLOAT; break;
						case 1: m_selectedOutputType = WirePortDataType.FLOAT2; break;
						case 2: m_selectedOutputType = WirePortDataType.FLOAT3; break;
						case 3: m_selectedOutputType = WirePortDataType.FLOAT4; break;
					}

					UpdatePorts();
					m_dropdownEditing = false;
				}
			}
		}

		public override void DrawProperties()
		{

			EditorGUILayout.BeginVertical();
			EditorGUI.BeginChangeCheck();
			m_selectedOutputTypeInt = EditorGUILayoutPopup( OutputTypeStr, m_selectedOutputTypeInt, m_outputValueTypes );
			if ( EditorGUI.EndChangeCheck() )
			{
				switch ( m_selectedOutputTypeInt )
				{
					case 0: m_selectedOutputType = WirePortDataType.FLOAT; break;
					case 1: m_selectedOutputType = WirePortDataType.FLOAT2; break;
					case 2: m_selectedOutputType = WirePortDataType.FLOAT3; break;
					case 3: m_selectedOutputType = WirePortDataType.FLOAT4; break;
				}

				UpdatePorts();
			}
			EditorGUILayout.EndVertical();

			// Draw base properties
			base.DrawProperties();

			EditorGUILayout.BeginVertical();

			int count = 0;

			switch ( m_selectedOutputType )
			{
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				count = 4;
				break;
				case WirePortDataType.FLOAT3:
				count = 3;
				break;
				case WirePortDataType.FLOAT2:
				count = 2;
				break;
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT:
				count = 1;
				break;
				case WirePortDataType.OBJECT:
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				break;
			}

			EditorGUI.BeginChangeCheck();
			if ( m_inputPorts[ 0 ].DataType == WirePortDataType.COLOR )
			{
				for ( int i = 0; i < count; i++ )
				{
					m_selectedOutputSwizzleTypes[ i ] = EditorGUILayoutPopup( SwizzleChannelLabels[ i ], m_selectedOutputSwizzleTypes[ i ], SwizzleColorChannels );
				}
			}
			else
			{
				for ( int i = 0; i < count; i++ )
				{
					m_selectedOutputSwizzleTypes[ i ] = EditorGUILayoutPopup( SwizzleChannelLabels[ i ], m_selectedOutputSwizzleTypes[ i ], SwizzleVectorChannels );
				}
			}
			if ( EditorGUI.EndChangeCheck() )
			{
				UpdatePorts();
			}

			EditorGUILayout.EndVertical();

		}

		void UpdatePorts()
		{
			ChangeOutputType( m_selectedOutputType, false );

			int count = 0;
			switch ( m_selectedOutputType )
			{
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				count = 4;
				break;
				case WirePortDataType.FLOAT3:
				count = 3;
				break;
				case WirePortDataType.FLOAT2:
				count = 2;
				break;
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT:
				count = 1;
				break;
				case WirePortDataType.OBJECT:
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				break;
			}

			int inputMaxChannelId = 0;
			switch ( m_inputPorts[ 0 ].DataType )
			{
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				inputMaxChannelId = 3;
				break;
				case WirePortDataType.FLOAT3:
				inputMaxChannelId = 2;
				break;
				case WirePortDataType.FLOAT2:
				inputMaxChannelId = 1;
				break;
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT:
				inputMaxChannelId = 0;
				break;
				case WirePortDataType.OBJECT:
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				break;
			}

			for ( int i = 0; i < count; i++ )
			{
				m_selectedOutputSwizzleTypes[ i ] = Mathf.Clamp( m_selectedOutputSwizzleTypes[ i ], 0, inputMaxChannelId );
			}

			// Update Title
			string additionalText = string.Empty;
			for ( int i = 0; i < count; i++ )
			{
				additionalText += GetSwizzleComponentForChannel( m_selectedOutputSwizzleTypes[ i ] ).ToUpper();
			}

			if ( additionalText.Length > 0 )
				SetAdditonalTitleText( "Value( " + additionalText + " )" );
			else
				SetAdditonalTitleText( string.Empty );

			CalculatePreviewData();
			m_sizeIsDirty = true;
		}

		public string GetSwizzleComponentForChannel( int channel )
		{
			if ( m_inputPorts[ 0 ].DataType == WirePortDataType.COLOR )
			{
				return SwizzleColorChannels[ channel ];
			}
			else
			{
				return SwizzleVectorChannels[ channel ];
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
			
			string value = string.Format( "({0}).", m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector ) );
			int count = 0;
			switch ( m_selectedOutputType )
			{
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				count = 4;
				break;
				case WirePortDataType.FLOAT3:
				count = 3;
				break;
				case WirePortDataType.FLOAT2:
				count = 2;
				break;
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT:
				count = 1;
				break;
				case WirePortDataType.OBJECT:
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				break;
			}
			for ( int i = 0; i < count; i++ )
			{
				value += GetSwizzleComponentForChannel( m_selectedOutputSwizzleTypes[ i ] );
			}

			return CreateOutputLocalVariable( 0, value, ref dataCollector );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_selectedOutputType = ( WirePortDataType ) Enum.Parse( typeof( WirePortDataType ), GetCurrentParam( ref nodeParams ) );
			switch ( m_selectedOutputType )
			{
				case WirePortDataType.FLOAT: m_selectedOutputTypeInt = 0; break;
				case WirePortDataType.FLOAT2: m_selectedOutputTypeInt = 1; break;
				case WirePortDataType.FLOAT3: m_selectedOutputTypeInt = 2; break;
				case WirePortDataType.COLOR:
				case WirePortDataType.FLOAT4: m_selectedOutputTypeInt = 3; break;
			}
			for ( int i = 0; i < m_selectedOutputSwizzleTypes.Length; i++ )
			{
				m_selectedOutputSwizzleTypes[ i ] = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			}

			UpdatePorts();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_selectedOutputType );
			for ( int i = 0; i < m_selectedOutputSwizzleTypes.Length; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_selectedOutputSwizzleTypes[ i ] );
			}
		}
	}
}
