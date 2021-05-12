// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Get Local Var", "Miscellaneous", "Use a registered local variable", null, KeyCode.G )]
	public class GetLocalVarNode : ParentNode
	{
		[SerializeField]
		private int m_referenceId = -1;

		[SerializeField]
		private float m_referenceWidth = -1;

		[SerializeField]
		private int m_nodeId = -1;

		[SerializeField]
		private RegisterLocalVarNode m_currentSelected = null;
		
		[SerializeField]
		private string m_registerLocalVarName = string.Empty;

		private int m_cachedPropertyId = -1;

		private string m_previousLabel = string.Empty;

		private bool m_refSelect = false;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputPort( WirePortDataType.OBJECT, Constants.EmptyPortValue );
			
			// This is needed for infinite loop detection
			AddInputPort( WirePortDataType.OBJECT, false, Constants.EmptyPortValue );
			m_inputPorts[ 0 ].Visible = false;

			m_outputPorts[ 0 ].Locked = true;
			m_textLabelWidth = 80;
			m_autoWrapProperties = true;
			m_hasLeftDropdown = true;
			m_previewShaderGUID = "f21a6e44c7d7b8543afacd19751d24c6";
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if( m_currentSelected != null )
			{
				if( m_drawPreviewAsSphere != m_currentSelected.SpherePreview )
				{
					m_drawPreviewAsSphere = m_currentSelected.SpherePreview;
					OnNodeChange();
				}
				//CheckSpherePreview();

				if( m_cachedPropertyId == -1 )
					m_cachedPropertyId = Shader.PropertyToID( "_A" );

				PreviewMaterial.SetTexture( m_cachedPropertyId, m_currentSelected.OutputPorts[ 0 ].OutputPreviewTexture );
			}
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			m_referenceId = EditorGUILayoutPopup( Constants.AvailableReferenceStr, m_referenceId, UIUtils.LocalVarNodeArr() );
			if( EditorGUI.EndChangeCheck() )
			{
				UpdateFromSelected();
			}

			if( GUILayout.Button( "\u25C4", "minibutton", GUILayout.Width( 17 ) ) && m_currentSelected )
			{
				UIUtils.FocusOnNode( m_currentSelected, 0, false );
			}
			EditorGUILayout.EndHorizontal();
			//EditorGUILayout.LabelField( ConnStatus.ToString() + " " + m_activeConnections );
		}

		public override void Destroy()
		{
			base.Destroy();
			CurrentSelected = null;
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );

			if( m_dropdownEditing )
			{
				EditorGUI.BeginChangeCheck();
				m_referenceId = EditorGUIPopup( m_dropdownRect, m_referenceId, UIUtils.LocalVarNodeArr(), UIUtils.PropertyPopUp );
				if( EditorGUI.EndChangeCheck() )
				{
					UpdateFromSelected();
					m_dropdownEditing = false;
				}
			}
		}

		public override void OnNodeLogicUpdate( DrawInfo drawInfo )
		{
			base.OnNodeLogicUpdate( drawInfo );
			UpdateLocalVar();
		}

		public override void OnNodeRepaint( DrawInfo drawInfo )
		{
			base.OnNodeRepaint( drawInfo );

			if( m_isVisible && m_refSelect && !m_selected )
			{
				GUI.color = Constants.SpecialGetLocalVarSelectionColor;
				GUI.Label( m_globalPosition, string.Empty, UIUtils.GetCustomStyle( CustomStyle.NodeWindowOn ) );
				GUI.color = m_colorBuffer;
			}
		}

		void UpdateFromSelected()
		{
			CurrentSelected = UIUtils.GetLocalVarNode( m_referenceId );
			if( m_currentSelected != null )
			{
				m_nodeId = m_currentSelected.UniqueId;
				m_outputPorts[ 0 ].Locked = false;
				m_outputPorts[ 0 ].ChangeType( m_currentSelected.OutputPorts[ 0 ].DataType, false );
				m_drawPreviewAsSphere = m_currentSelected.SpherePreview;
				CheckSpherePreview();

				m_previousLabel = m_currentSelected.DataToArray;
				SetAdditonalTitleText( string.Format( Constants.SubTitleVarNameFormatStr, m_currentSelected.DataToArray ) );
				m_referenceWidth = m_currentSelected.Position.width;
			}

			m_sizeIsDirty = true;
			m_isDirty = true;
		}

		void UpdateLocalVar()
		{
			m_refSelect = false;
			if( m_referenceId > -1 )
			{
				ParentNode newNode = UIUtils.GetLocalVarNode( m_referenceId );
				if( newNode != null )
				{
					if( newNode.UniqueId != m_nodeId )
					{
						CurrentSelected = null;
						int count = UIUtils.LocalVarNodeAmount();
						for( int i = 0; i < count; i++ )
						{
							ParentNode node = UIUtils.GetLocalVarNode( i );
							if( node.UniqueId == m_nodeId )
							{
								CurrentSelected = node as RegisterLocalVarNode;
								m_referenceId = i;
								break;
							}
						}
					}
				}

				if( m_currentSelected != null )
				{
					if( m_currentSelected.OutputPorts[ 0 ].DataType != m_outputPorts[ 0 ].DataType )
					{
						m_outputPorts[ 0 ].Locked = false;
						m_outputPorts[ 0 ].ChangeType( m_currentSelected.OutputPorts[ 0 ].DataType, false );
					}

					if( !m_previousLabel.Equals( m_currentSelected.DataToArray ) )
					{
						m_previousLabel = m_currentSelected.DataToArray;
						SetAdditonalTitleText( string.Format( Constants.SubTitleVarNameFormatStr, m_currentSelected.DataToArray ) );
					}

					if( m_referenceWidth != m_currentSelected.Position.width )
					{
						m_referenceWidth = m_currentSelected.Position.width;
						m_sizeIsDirty = true;
					}

					if( m_currentSelected.Selected )
						m_refSelect = true;
				}
				else
				{
					ResetReference();
				}
			}
		}

		public void ResetReference()
		{
			m_outputPorts[ 0 ].Locked = true;
			m_currentSelected = null;
			m_inputPorts[ 0 ].DummyClear();
			m_nodeId = -1;
			m_referenceId = -1;
			m_referenceWidth = -1;
			SetAdditonalTitleText( string.Empty );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( m_currentSelected != null )
			{
				return m_currentSelected.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			}
			else
			{
				Debug.LogError( "Attempting to access inexistant local variable" );
				return "0";
			}
		}


		//public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		//{
		//	base.PropagateNodeData( nodeData, ref dataCollector );
		//	if( m_currentSelected != null )
		//	{
		//		m_currentSelected.PropagateNodeData( nodeData, ref dataCollector );
		//	}
		//}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if( UIUtils.CurrentShaderVersion() > 15 )
			{
				m_nodeId = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				m_outputPorts[ 0 ].Locked = ( m_nodeId < 0 );
				if( UIUtils.CurrentShaderVersion() > 15500 )
				{
					m_registerLocalVarName = GetCurrentParam( ref nodeParams );
				}
			}
			else
			{
				m_referenceId = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				m_outputPorts[ 0 ].Locked = ( m_referenceId < 0 );
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			if( m_currentSelected != null )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_currentSelected.UniqueId );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_currentSelected.DataToArray );
			}
			else
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, -1 );
				IOUtils.AddFieldValueToString( ref nodeInfo, string.Empty );
			}

		}

		public override void OnNodeDoubleClicked( Vector2 currentMousePos2D )
		{
			if( m_currentSelected != null )
			{
				UIUtils.FocusOnNode( m_currentSelected, 0, true );
			}
		}

		public override void RefreshExternalReferences()
		{
			base.RefreshExternalReferences();
			if( UIUtils.CurrentShaderVersion() > 15 )
			{
				CurrentSelected = UIUtils.GetNode( m_nodeId ) as RegisterLocalVarNode;
				m_referenceId = UIUtils.GetLocalVarNodeRegisterId( m_nodeId );
				if( CurrentSelected == null && UIUtils.CurrentShaderVersion() > 15500 && !string.IsNullOrEmpty(m_registerLocalVarName))
				{
					CurrentSelected = m_containerGraph.LocalVarNodes.GetNodeByDataToArray( m_registerLocalVarName );
					if( CurrentSelected != null )
					{
						m_nodeId = CurrentSelected.UniqueId;
						m_referenceId = UIUtils.GetLocalVarNodeRegisterId( m_nodeId );
					} 
				}
			}
			else
			{
				CurrentSelected = UIUtils.GetLocalVarNode( m_referenceId );
				if( m_currentSelected != null )
				{
					m_nodeId = m_currentSelected.UniqueId;
				}
			}

			if( m_currentSelected != null )
			{
				m_outputPorts[ 0 ].Locked = false;
				m_outputPorts[ 0 ].ChangeType( m_currentSelected.OutputPorts[ 0 ].DataType, false );
			}
			else
			{
				m_outputPorts[ 0 ].Locked = true;
			}
		}

		public override void ActivateNode( int signalGenNodeId, int signalGenPortId, System.Type signalGenNodeType )
		{
			base.ActivateNode( signalGenNodeId, signalGenPortId, signalGenNodeType );
			if( m_activeConnections == 1 )
			{
				if( m_currentSelected != null )
				{
					m_currentSelected.ActivateNode( signalGenNodeId, signalGenPortId, signalGenNodeType );
				}
			}
		}

		public override void DeactivateNode( int deactivatedPort, bool forceComplete )
		{
			forceComplete = forceComplete || ( m_activeConnections == 1 );
			base.DeactivateNode( deactivatedPort, forceComplete );
			if( forceComplete && m_currentSelected != null )
			{
				m_currentSelected.DeactivateNode( deactivatedPort, false );
			}
		}

		public override void OnNodeSelected( bool value )
		{
			base.OnNodeSelected( value );
			if( m_currentSelected != null )
			{
				m_currentSelected.CheckReferenceSelection();
			}
		}
		
		public RegisterLocalVarNode CurrentSelected
		{
			get { return m_currentSelected; }
			set
			{
				// This is needed for infinite loop detection
				if( m_inputPorts != null )
					m_inputPorts[ 0 ].DummyClear();

				if( m_currentSelected != null )
				{
					m_currentSelected.UnregisterGetLocalVar( this );

					//if( m_currentSelected != value )
					m_currentSelected.DeactivateNode( 0, false );
				}
				
				if( value != null )
				{
					value.RegisterGetLocalVar( this );
					if( IsConnected && value != m_currentSelected )
						value.ActivateNode( UniqueId, 0, m_activeType );

					// This is needed for infinite loop detection
					m_inputPorts[ 0 ].DummyAdd( value.UniqueId, 0 ); ;
				}

				m_currentSelected = value;

			}
		}
	}
}
