// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "[Old]Append", "Vector Operators", "Append channels to create a new component",null,KeyCode.V,true,true,"Append",typeof(DynamicAppendNode))]
	public sealed class AppendNode : ParentNode
	{
		private const string OutputTypeStr = "Output type";

		[SerializeField]
		private WirePortDataType m_selectedOutputType = WirePortDataType.FLOAT4;

		[SerializeField]
		private int m_selectedOutputTypeInt = 2;

		[SerializeField]
		private float[] m_defaultValues = { 0, 0, 0, 0 };
		private string[] m_defaultValuesStr = { "[0]", "[1]", "[2]", "[3]" };

		private readonly string[] m_outputValueTypes ={  "Vector2",
														"Vector3",
														"Vector4",
														"Color"};

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, "[0]" );
			AddInputPort( WirePortDataType.FLOAT, false, "[1]" );
			AddInputPort( WirePortDataType.FLOAT, false, "[2]" );
			AddInputPort( WirePortDataType.FLOAT, false, "[3]" );
			AddOutputPort( m_selectedOutputType, Constants.EmptyPortValue );
			m_textLabelWidth = 90;
			m_autoWrapProperties = true;
			m_previewShaderGUID = "d80ac81aabf643848a4eaa76f2f88d65";
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
					SetupPorts();
					m_dropdownEditing = false;
				}
			}
		}

		void SetupPorts()
		{
			switch ( m_selectedOutputTypeInt )
			{
				case 0: m_selectedOutputType = WirePortDataType.FLOAT2; break;
				case 1: m_selectedOutputType = WirePortDataType.FLOAT3; break;
				case 2: m_selectedOutputType = WirePortDataType.FLOAT4; break;
				case 3: m_selectedOutputType = WirePortDataType.COLOR; break;
			}

			UpdatePorts();
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUILayout.BeginVertical();

			EditorGUI.BeginChangeCheck();
			m_selectedOutputTypeInt = EditorGUILayoutPopup( OutputTypeStr, m_selectedOutputTypeInt, m_outputValueTypes );
			if ( EditorGUI.EndChangeCheck() )
			{
				SetupPorts();
			}

			int count = 0;
			switch ( m_selectedOutputType )
			{
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				{
					count = 4;
				}
				break;
				case WirePortDataType.FLOAT3:
				{
					count = 3;
				}
				break;
				case WirePortDataType.FLOAT2:
				{
					count = 2;
				}
				break;
				case WirePortDataType.OBJECT:
				case WirePortDataType.FLOAT:
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{ }
				break;
			}

			for ( int i = 0; i < count; i++ )
			{
				if ( !m_inputPorts[ i ].IsConnected )
					m_defaultValues[ i ] = EditorGUILayoutFloatField( m_defaultValuesStr[ i ], m_defaultValues[ i ] );
			}

			EditorGUILayout.EndVertical();
		}
		void UpdatePorts()
		{
			m_sizeIsDirty = true;
			ChangeOutputType( m_selectedOutputType, false );
			switch ( m_selectedOutputType )
			{
				case WirePortDataType.FLOAT4:
				case WirePortDataType.OBJECT:
				case WirePortDataType.COLOR:
				{
					m_inputPorts[ 0 ].Visible = true;
					m_inputPorts[ 1 ].Visible = true;
					m_inputPorts[ 2 ].Visible = true;
					m_inputPorts[ 3 ].Visible = true;
				}
				break;
				case WirePortDataType.FLOAT3:
				{
					m_inputPorts[ 0 ].Visible = true;
					m_inputPorts[ 1 ].Visible = true;
					m_inputPorts[ 2 ].Visible = true;
					m_inputPorts[ 3 ].Visible = false;
					if ( m_inputPorts[ 3 ].IsConnected )
						UIUtils.DeleteConnection( true, UniqueId, 3, false, true );
				}
				break;
				case WirePortDataType.FLOAT2:
				{
					m_inputPorts[ 0 ].Visible = true;
					m_inputPorts[ 1 ].Visible = true;
					m_inputPorts[ 2 ].Visible = false;
					if ( m_inputPorts[ 2 ].IsConnected )
						UIUtils.DeleteConnection( true, UniqueId, 2, false, true );

					m_inputPorts[ 3 ].Visible = false;
					if ( m_inputPorts[ 3 ].IsConnected )
						UIUtils.DeleteConnection( true, UniqueId, 3, false, true );
				}
				break;
				case WirePortDataType.FLOAT:
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{ }
				break;
			}
		}
		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			string value = string.Empty;
			switch ( m_selectedOutputType )
			{
				case WirePortDataType.FLOAT4:
				case WirePortDataType.OBJECT:
				case WirePortDataType.COLOR:
				{
					value = "float4( ";
					for ( int i = 0; i < 4; i++ )
					{
						value += m_inputPorts[ i ].IsConnected ? InputPorts[ i ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT, ignoreLocalVar, true ) : m_defaultValues[ i ].ToString();
						if ( i != 3 )
							value += " , ";
					}
					value += " )";
				}
				break;
				case WirePortDataType.FLOAT3:
				{
					value = "float3( ";
					for ( int i = 0; i < 3; i++ )
					{
						value += m_inputPorts[ i ].IsConnected ? InputPorts[ i ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT, ignoreLocalVar, true ) : m_defaultValues[ i ].ToString();
						if ( i != 2 )
							value += " , ";
					}
					value += " )";
				}
				break;
				case WirePortDataType.FLOAT2:
				{
					value = "float2( ";
					for ( int i = 0; i < 2; i++ )
					{
						value += m_inputPorts[ i ].IsConnected ? InputPorts[ i ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT, ignoreLocalVar, true ) : m_defaultValues[ i ].ToString();
						if ( i != 1 )
							value += " , ";
					}
					value += " )";
				}
				break;
				case WirePortDataType.FLOAT:
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{ }
				break;
			}

			RegisterLocalVariable( 0, value, ref dataCollector, "appendResult" + OutputId );
			return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_selectedOutputType = ( WirePortDataType ) Enum.Parse( typeof( WirePortDataType ), GetCurrentParam( ref nodeParams ) );
			switch ( m_selectedOutputType )
			{
				case WirePortDataType.FLOAT2: m_selectedOutputTypeInt = 0; break;
				case WirePortDataType.FLOAT3: m_selectedOutputTypeInt = 1; break;
				case WirePortDataType.FLOAT4: m_selectedOutputTypeInt = 2; break;
				case WirePortDataType.COLOR: m_selectedOutputTypeInt = 3; break;
			}
			for ( int i = 0; i < m_defaultValues.Length; i++ )
			{
				m_defaultValues[ i ] = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
			}
			UpdatePorts();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_selectedOutputType );
			for ( int i = 0; i < m_defaultValues.Length; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_defaultValues[ i ] );
			}
		}
	}
}
