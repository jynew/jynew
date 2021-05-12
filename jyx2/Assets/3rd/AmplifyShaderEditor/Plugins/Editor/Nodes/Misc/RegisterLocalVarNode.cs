// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Register Local Var", "Miscellaneous", "Forces a local variable to be written with the given name. Can then be fetched at any place with a <b>Get Local Var</b> node.", null, KeyCode.R )]
	public sealed class RegisterLocalVarNode : ParentNode
	{
		private const string LocalDefaultNameStr = "myVarName";
		private const string LocalVarNameStr = "Var Name";
		private const string OrderIndexStr = "Order Index";
		private const string AutoOrderIndexStr = "Auto Order";
		private const string ReferencesStr = "References";

		private const string GetLocalVarLabel = "( {0} ) Get Local Var";
		private string m_oldName = string.Empty;
		private bool m_reRegisterName = false;
		private int m_autoOrderIndex = int.MaxValue;
		private bool m_forceUpdate = true;
		private bool m_refSelect = false;

		private bool m_referencesVisible = false;

		[SerializeField]
		private string m_variableName = LocalDefaultNameStr;

		[SerializeField]
		private int m_orderIndex = -1;

		[SerializeField]
		private bool m_autoIndexActive = true;

		[SerializeField]
		private List<GetLocalVarNode> m_registeredGetLocalVars = new List<GetLocalVarNode>();

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, Constants.EmptyPortValue );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_textLabelWidth = 85;

			if( m_containerGraph != null )
				m_variableName += m_containerGraph.LocalVarNodes.NodesList.Count;

			m_oldName = m_variableName;
			UpdateTitle();
			m_previewShaderGUID = "5aaa1d3ea9e1fa64781647e035a82334";
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			m_containerGraph.LocalVarNodes.AddNode( this );
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

		void UpdateTitle()
		{
			SetAdditonalTitleText( string.Format( Constants.SubTitleVarNameFormatStr, m_variableName ) );
		}

		void DrawMainProperties()
		{
			EditorGUI.BeginChangeCheck();
			m_variableName = EditorGUILayoutTextField( LocalVarNameStr, m_variableName );
			if( EditorGUI.EndChangeCheck() )
			{
				CheckAndChangeName();
			}

			DrawPrecisionProperty();
		}

		public override void AfterDuplication()
		{
			base.AfterDuplication();
			CheckAndChangeName();
		}

		void CheckAndChangeName()
		{
			m_variableName = UIUtils.RemoveInvalidCharacters( m_variableName );
			if( string.IsNullOrEmpty( m_variableName ) )
			{
				m_variableName = LocalDefaultNameStr + OutputId;
			}
			bool isNumericName = UIUtils.IsNumericName( m_variableName );
			if( !isNumericName && m_containerGraph.ParentWindow.DuplicatePrevBufferInstance.IsLocalvariableNameAvailable( m_variableName ) )
			{
				m_containerGraph.ParentWindow.DuplicatePrevBufferInstance.ReleaseLocalVariableName( UniqueId, m_oldName );
				m_containerGraph.ParentWindow.DuplicatePrevBufferInstance.RegisterLocalVariableName( UniqueId, m_variableName );
				m_oldName = m_variableName;
				m_containerGraph.LocalVarNodes.UpdateDataOnNode( UniqueId, m_variableName );
				UpdateTitle();
				m_forceUpdate = true;
			}
			else
			{
				//if( isNumericName )
				//{
				//	UIUtils.ShowMessage( "Local variable name cannot start or be numerical values" );
				//}

				m_variableName = m_oldName;
				m_containerGraph.LocalVarNodes.UpdateDataOnNode( UniqueId, m_variableName );
			}
		}

		void DrawReferences()
		{
			int count = m_registeredGetLocalVars.Count;
			if( m_registeredGetLocalVars.Count > 0 )
			{
				for( int i = 0; i < count; i++ )
				{
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField( string.Format( GetLocalVarLabel, m_registeredGetLocalVars[ i ].UniqueId ) );
					if( GUILayout.Button( "\u25BA", "minibutton", GUILayout.Width( 17 ) ) )
					{
						m_containerGraph.ParentWindow.FocusOnNode( m_registeredGetLocalVars[ i ], 0, false );
					}
					EditorGUILayout.EndHorizontal();
				}

				if( GUILayout.Button( "Back" ) )
				{
					m_containerGraph.ParentWindow.FocusOnNode( this, 0, false );
				}
			}
			else
			{
				EditorGUILayout.HelpBox( "This node is not being referenced by any Get Local Var.", MessageType.Info );
			}
		}

		public override void DrawProperties()
		{
			NodeUtils.DrawPropertyGroup( ref m_propertiesFoldout, Constants.ParameterLabelStr, DrawMainProperties );
			NodeUtils.DrawPropertyGroup( ref m_referencesVisible, ReferencesStr, DrawReferences );
			//EditorGUILayout.LabelField(ConnStatus.ToString()+" "+m_activeConnections);
		}

		public override void OnEnable()
		{
			base.OnEnable();
			m_reRegisterName = true;
		}

		public void CheckReferenceSelection()
		{
			m_refSelect = false;
			int count = m_registeredGetLocalVars.Count;
			for( int i = 0; i < count; i++ )
			{
				if( m_registeredGetLocalVars[ i ].Selected )
					m_refSelect = true;
			}
		}

		public override void OnNodeRepaint( DrawInfo drawInfo )
		{
			base.OnNodeRepaint( drawInfo );
			if( m_isVisible && m_refSelect && !m_selected )
			{
				GUI.color = Constants.SpecialRegisterLocalVarSelectionColor;
				GUI.Label( m_globalPosition, string.Empty, UIUtils.GetCustomStyle( CustomStyle.NodeWindowOn ) );
				GUI.color = m_colorBuffer;
			}
		}

		public override void Draw( DrawInfo drawInfo )
		{

			base.Draw( drawInfo );
			if( m_reRegisterName )
			{
				m_reRegisterName = false;
				m_containerGraph.ParentWindow.DuplicatePrevBufferInstance.RegisterLocalVariableName( UniqueId, m_variableName );
			}

			if( m_forceUpdate )
			{
				m_forceUpdate = false;
				UpdateTitle();
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
			{
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
			}
			string result = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			RegisterLocalVariable( 0, result, ref dataCollector, m_variableName + OutputId );
			return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_variableName = GetCurrentParam( ref nodeParams );
			if( UIUtils.CurrentShaderVersion() > 14 )
				m_orderIndex = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );

			if( UIUtils.CurrentShaderVersion() > 3106 )
			{
				m_autoIndexActive = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			}
			else
			{
				m_autoIndexActive = false;
			}
			if( !m_isNodeBeingCopied )
			{
				m_containerGraph.LocalVarNodes.UpdateDataOnNode( UniqueId, m_variableName );
				m_containerGraph.ParentWindow.DuplicatePrevBufferInstance.ReleaseLocalVariableName( UniqueId, m_oldName );
				m_containerGraph.ParentWindow.DuplicatePrevBufferInstance.RegisterLocalVariableName( UniqueId, m_variableName );
			}
			m_forceUpdate = true;
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_variableName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_orderIndex );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_autoIndexActive );
		}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			if( m_autoOrderIndex < nodeData.OrderIndex )
			{
				nodeData.OrderIndex = m_autoOrderIndex - 1;
			}
			else
			{
				m_autoOrderIndex = nodeData.OrderIndex;
				nodeData.OrderIndex -= 1;
			}

			base.PropagateNodeData( nodeData, ref dataCollector );
		}

		public override void ResetNodeData()
		{
			base.ResetNodeData();
			m_autoOrderIndex = int.MaxValue;
		}

		public void RegisterGetLocalVar( GetLocalVarNode node )
		{
			if( !m_registeredGetLocalVars.Contains( node ) )
			{
				m_registeredGetLocalVars.Add( node );
				CheckReferenceSelection();
			}
		}

		public void UnregisterGetLocalVar( GetLocalVarNode node )
		{
			if( m_registeredGetLocalVars.Contains( node ) )
			{
				m_registeredGetLocalVars.Remove( node );
				CheckReferenceSelection();
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			m_containerGraph.LocalVarNodes.RemoveNode( this );
			m_containerGraph.ParentWindow.DuplicatePrevBufferInstance.ReleaseLocalVariableName( UniqueId, m_variableName );

			int count = m_registeredGetLocalVars.Count;
			for( int i = 0; i < count; i++ )
			{
				//GetLocalVarNode node =  m_containerGraph.GetNode( m_registeredGetLocalVars[ i ] ) as GetLocalVarNode;
				if( m_registeredGetLocalVars[ i ] != null )
					m_registeredGetLocalVars[ i ].ResetReference();
			}
			m_registeredGetLocalVars.Clear();
			m_registeredGetLocalVars = null;

			m_containerGraph.LocalVarNodes.RemoveNode( this );
		}

		public override void ActivateNode( int signalGenNodeId, int signalGenPortId, Type signalGenNodeType )
		{
			base.ActivateNode( signalGenNodeId, signalGenPortId, signalGenNodeType );
		}
		public override string DataToArray { get { return m_variableName; } }
		public List<GetLocalVarNode> NodeReferences { get { return m_registeredGetLocalVars; } }
	}
}
