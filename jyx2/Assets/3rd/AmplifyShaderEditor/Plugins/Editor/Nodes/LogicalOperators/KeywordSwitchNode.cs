// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Keyword Switch", "Logical Operators", "Attributes a value according to the existance of a selected keyword", Deprecated = true, DeprecatedAlternativeType = typeof(StaticSwitch), DeprecatedAlternative = "Static Switch" )]
	public sealed class KeywordSwitchNode : ParentNode
	{
		private const string KeywordStr = "Keyword";
		private const string CustomStr = "Custom";

		[SerializeField]
		private string m_currentKeyword = string.Empty;

		[SerializeField]
		private int m_currentKeywordId = 0;

		[SerializeField]
		private WirePortDataType m_mainPortType = WirePortDataType.FLOAT;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, "True" );
			AddInputPort( WirePortDataType.FLOAT, false, "False" );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_textLabelWidth = 65;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUI.BeginChangeCheck();
			m_currentKeywordId = EditorGUILayoutPopup( KeywordStr, m_currentKeywordId, UIUtils.AvailableKeywords );
			if ( EditorGUI.EndChangeCheck() )
			{
				if ( m_currentKeywordId != 0 )
				{
					m_currentKeyword = UIUtils.AvailableKeywords[ m_currentKeywordId ];
				}	
			}
			if ( m_currentKeywordId == 0 )
			{
				m_currentKeyword = EditorGUILayoutTextField( CustomStr, m_currentKeyword );
			}
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			UpdateConnected( portId );
		}

		public override void OnConnectedOutputNodeChanges( int portId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( portId, otherNodeId, otherPortId, name, type );
			UpdateConnected( portId );
		}

		public override void OnInputPortDisconnected( int portId )
		{
			base.OnInputPortDisconnected( portId );
			UpdateDisconnected( portId );
		}

		void UpdateConnected( int portId )
		{
			m_inputPorts[ portId ].MatchPortToConnection();
			int otherPortId = ( portId + 1 ) % 2;
			if ( m_inputPorts[ otherPortId ].IsConnected )
			{
				m_mainPortType = ( UIUtils.GetPriority( m_inputPorts[ portId ].DataType ) > UIUtils.GetPriority( m_inputPorts[ otherPortId ].DataType ) ) ?
									m_inputPorts[ portId ].DataType :
									m_inputPorts[ otherPortId ].DataType;
			}
			else
			{
				m_mainPortType = m_inputPorts[ portId ].DataType;
				m_inputPorts[ otherPortId ].ChangeType( m_mainPortType, false );
			}
			m_outputPorts[ 0 ].ChangeType( m_mainPortType, false );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			string trueCode = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string falseCode = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );

			string localVarName = "simpleKeywordVar"+OutputId;
			string outType = UIUtils.PrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType );
			dataCollector.AddLocalVariable( UniqueId, "#ifdef " + m_currentKeyword, true );
			dataCollector.AddLocalVariable( UniqueId, outType + " " + localVarName  + " = " + trueCode + ";", true );
			dataCollector.AddLocalVariable( UniqueId, "#else", true );
			dataCollector.AddLocalVariable( UniqueId, outType + " " + localVarName + " = " + falseCode + ";", true );
			dataCollector.AddLocalVariable( UniqueId, "#endif", true );
			m_outputPorts[ 0 ].SetLocalValue( localVarName, dataCollector.PortCategory );

			return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
		}

		void UpdateDisconnected( int portId )
		{
			int otherPortId = ( portId + 1 ) % 2;
			if ( m_inputPorts[ otherPortId ].IsConnected )
			{
				m_mainPortType = m_inputPorts[ otherPortId ].DataType;
				m_inputPorts[ portId ].ChangeType( m_mainPortType, false );
			}
			m_outputPorts[ 0 ].ChangeType( m_mainPortType, false );
		}
		
		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_currentKeyword = GetCurrentParam( ref nodeParams );
			m_currentKeywordId = UIUtils.GetKeywordId( m_currentKeyword );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_currentKeyword );
		}
	}
}
