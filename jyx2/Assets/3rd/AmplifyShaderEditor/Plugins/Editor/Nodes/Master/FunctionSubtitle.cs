// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Function Subtitle", "Functions", "Adds a subtitle to its shader function", NodeAvailabilityFlags = (int)NodeAvailability.ShaderFunction )]
	public sealed class FunctionSubtitle : ParentNode
	{

		//protected override void CommonInit( int uniqueId )
		//{
		//	base.CommonInit( uniqueId );
		//	AddInputPort( WirePortDataType.FLOAT, false, Constants.EmptyPortValue );
		//	AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
		//	m_autoWrapProperties = true;
		//	m_textLabelWidth = 100;
		//	//SetTitleText( m_inputName );
		//	//SetAdditonalTitleText( "( " + m_inputValueTypes[ m_selectedInputTypeInt ] + " )" );
		//	m_previewShaderGUID = "04bc8e7b317dccb4d8da601680dd8140";
		//}
		[SerializeField]
		private string m_subttile = "Subtitle";

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, Constants.EmptyPortValue );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_autoWrapProperties = true;
			m_textLabelWidth = 100;
			SetTitleText( m_subttile );
			m_previewShaderGUID = "74e4d859fbdb2c0468de3612145f4929";
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			m_inputPorts[ 0 ].MatchPortToConnection();
			m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
		}

		public override void OnConnectedOutputNodeChanges( int outputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( outputPortId, otherNodeId, otherPortId, name, type );
			m_inputPorts[ 0 ].MatchPortToConnection();
			m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			return m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ 0 ].DataType, ignoreLocalvar );
		}

		//public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		//{
		//	base.PropagateNodeData( nodeData, ref dataCollector );

		//	//if( m_containerGraph.CurrentShaderFunction != null )
		//		//m_containerGraph.CurrentShaderFunction.FunctionSubtitle = m_subttile;
		//}

		public override void OnNodeLogicUpdate( DrawInfo drawInfo )
		{
			base.OnNodeLogicUpdate( drawInfo );
			//public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
			//{
			//	base.PropagateNodeData( nodeData, ref dataCollector );
			//Debug.Log( IsConnected + " " + m_containerGraph.CurrentFunctionOutput );
			if( m_containerGraph.CurrentFunctionOutput != null && IsConnected )
				m_containerGraph.CurrentFunctionOutput.SubTitle = m_subttile;
			//	m_containerGraph.CurrentShaderFunction.FunctionSubtitle = m_subttile;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUILayout.BeginVertical();
			EditorGUI.BeginChangeCheck();
			m_subttile = EditorGUILayoutTextField( "Name", m_subttile );
			if( EditorGUI.EndChangeCheck() )
			{
				SetTitleText( m_subttile );
				//UIUtils.UpdateFunctionInputData( UniqueId, m_inputName );
			}
			EditorGUI.BeginChangeCheck();
			//m_selectedInputTypeInt = EditorGUILayoutPopup( InputTypeStr, m_selectedInputTypeInt, m_inputValueTypes );
			//if( EditorGUI.EndChangeCheck() )
			//{
			//	UpdatePorts();
			//	SetAdditonalTitleText( "( " + m_inputValueTypes[ m_selectedInputTypeInt ] + " )" );
			//}

			//m_autoCast = EditorGUILayoutToggle( "Auto Cast", m_autoCast );

			//EditorGUILayout.Separator();
			//if( !m_inputPorts[ 0 ].IsConnected && m_inputPorts[ 0 ].ValidInternalData )
			//{
			//	m_inputPorts[ 0 ].ShowInternalData( this, true, "Default Value" );
			//}


			EditorGUILayout.EndVertical();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_subttile );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_subttile = GetCurrentParam( ref nodeParams );
			SetTitleText( m_subttile );
		}
	}
}
