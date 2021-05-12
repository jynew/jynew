using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Wire Node", "Miscellaneous", "Wire Node", null, KeyCode.None, false )]
	public sealed class WireNode : ParentNode
	{
		private bool m_markedToDelete = false;

		[SerializeField]
		private WirePortDataType m_visualDataType = WirePortDataType.FLOAT;

		bool m_forceVisualDataUpdate = false;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.OBJECT, false, string.Empty );
			AddOutputPort( WirePortDataType.OBJECT, Constants.EmptyPortValue );
			m_tooltipText = string.Empty;
			m_drawPreview = false;
			m_drawPreviewExpander = false;
			m_canExpand = false;
			m_previewShaderGUID = "fa1e3e404e6b3c243b5527b82739d682";
		}

		public WirePortDataType GetLastInputDataTypeRecursively()
		{
			if( m_outputPorts[ 0 ].ExternalReferences.Count > 0 )
			{
				WireNode rightWire = m_outputPorts[ 0 ].GetInputNode( 0 ) as WireNode;
				if( rightWire != null )
					return rightWire.GetLastInputDataTypeRecursively();
				else
				{
					return m_outputPorts[ 0 ].GetInputConnection( 0 ).DataType;
				}
			}

			if( m_containerGraph.ParentWindow.WireReferenceUtils.OutputPortReference.IsValid )
				return m_containerGraph.ParentWindow.WireReferenceUtils.OutputPortReference.DataType;
			else
				return m_visualDataType;
		}

		public override WirePortDataType GetInputPortVisualDataTypeByArrayIdx( int portArrayIdx )
		{
			return m_visualDataType;
		}

		public override WirePortDataType GetOutputPortVisualDataTypeById( int portId )
		{
			return m_visualDataType;
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			m_inputPorts[ 0 ].MatchPortToConnection();
			m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );

			m_forceVisualDataUpdate = true;
		}

		public override void OnOutputPortConnected( int portId, int otherNodeId, int otherPortId )
		{
			base.OnOutputPortConnected( portId, otherNodeId, otherPortId );

			if( m_outputPorts[ portId ].ConnectionCount > 1 )
			{
				for( int i = 0; i < m_outputPorts[ portId ].ExternalReferences.Count; i++ )
				{
					if( m_outputPorts[ portId ].ExternalReferences[ i ].PortId != otherPortId )
					{
						UIUtils.DeleteConnection( true, m_outputPorts[ portId ].ExternalReferences[ i ].NodeId, m_outputPorts[ portId ].ExternalReferences[ i ].PortId, false, true );
					}
				}
			}

			m_inputPorts[ 0 ].NotifyExternalRefencesOnChange();
			m_forceVisualDataUpdate = true;
		}

		public override void OnConnectedInputNodeChanges( int portId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedInputNodeChanges( portId, otherNodeId, otherPortId, name, type );

			m_inputPorts[ 0 ].NotifyExternalRefencesOnChange();
			m_forceVisualDataUpdate = true;
		}

		public override void OnConnectedOutputNodeChanges( int outputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( outputPortId, otherNodeId, otherPortId, name, type );
			m_inputPorts[ 0 ].MatchPortToConnection();
			m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );

			m_forceVisualDataUpdate = true;
		}

		public override void OnInputPortDisconnected( int portId )
		{
			base.OnInputPortDisconnected( portId );
			TestIfValid();

			m_forceVisualDataUpdate = true;
			m_outputPorts[ 0 ].NotifyExternalRefencesOnChange();
		}

		public override void OnOutputPortDisconnected( int portId )
		{
			base.OnOutputPortDisconnected( portId );
			TestIfValid();

			m_forceVisualDataUpdate = true;
			m_inputPorts[ 0 ].NotifyExternalRefencesOnChange();
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			return m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
		}

		public override void DrawProperties()
		{
			if( m_markedToDelete )
				return;

			base.DrawProperties();
		}

		public override void OnNodeLayout( DrawInfo drawInfo )
		{
			if( m_firstDraw )
			{
				m_firstDraw = false;
				AfterCommonInit();
				OnNodeChange();
			}

			if( m_forceVisualDataUpdate )
			{
				m_forceVisualDataUpdate = false;
				m_visualDataType = GetLastInputDataTypeRecursively();
			}

			if( m_repopulateDictionaries )
			{
				m_repopulateDictionaries = false;

				m_inputPortsDict.Clear();
				int inputCount = m_inputPorts.Count;
				for( int i = 0; i < inputCount; i++ )
				{
					m_inputPortsDict.Add( m_inputPorts[ i ].PortId, m_inputPorts[ i ] );
				}

				m_outputPortsDict.Clear();
				int outputCount = m_outputPorts.Count;
				for( int i = 0; i < outputCount; i++ )
				{
					m_outputPortsDict.Add( m_outputPorts[ i ].PortId, m_outputPorts[ i ] );
				}
			}

			if( m_sizeIsDirty )
			{
				m_sizeIsDirty = false;
				m_extraSize.Set( 20f, 20f );
				m_position.width = m_extraSize.x + UIUtils.PortsSize.x;
				m_position.height = m_extraSize.y + UIUtils.PortsSize.y;

				Vec2Position -= Position.size * 0.5f;
				if( OnNodeChangeSizeEvent != null )
				{
					OnNodeChangeSizeEvent( this );
				}

				ChangeSizeFinished();
				//ChangeSize();
			}

			CalculatePositionAndVisibility( drawInfo );

			// Input Ports
			{
				m_currInputPortPos = m_globalPosition;
				m_currInputPortPos.width = drawInfo.InvertedZoom * UIUtils.PortsSize.x;
				m_currInputPortPos.height = drawInfo.InvertedZoom * UIUtils.PortsSize.y;
				m_currInputPortPos.position = m_globalPosition.center - m_currInputPortPos.size * 0.5f;
				int inputCount = m_inputPorts.Count;

				for( int i = 0; i < inputCount; i++ )
				{
					if( m_inputPorts[ i ].Visible )
					{
						// Button
						m_inputPorts[ i ].Position = m_currInputPortPos;

						if( !m_inputPorts[ i ].Locked )
						{
							float overflow = 2;
							float scaledOverflow = 3 * drawInfo.InvertedZoom;
							m_auxRect = m_currInputPortPos;
							m_auxRect.yMin -= scaledOverflow + overflow;
							m_auxRect.yMax += scaledOverflow + overflow;
							m_auxRect.xMin -= Constants.PORT_INITIAL_X * drawInfo.InvertedZoom + scaledOverflow + overflow;
							m_auxRect.xMax += m_inputPorts[ i ].LabelSize.x + Constants.PORT_TO_LABEL_SPACE_X * drawInfo.InvertedZoom + scaledOverflow + overflow;
							m_inputPorts[ i ].ActivePortArea = m_auxRect;
						}
						m_currInputPortPos.y += drawInfo.InvertedZoom * ( m_fontHeight + Constants.INPUT_PORT_DELTA_Y );
					}
				}
			}

			// Output Ports
			{
				m_currOutputPortPos = m_globalPosition;
				m_currOutputPortPos.width = drawInfo.InvertedZoom * UIUtils.PortsSize.x;
				m_currOutputPortPos.height = drawInfo.InvertedZoom * UIUtils.PortsSize.y;
				m_currOutputPortPos.position = m_globalPosition.center - m_currOutputPortPos.size * 0.5f;
				//m_currOutputPortPos.x += ( m_globalPosition.width - drawInfo.InvertedZoom * ( Constants.PORT_INITIAL_X + m_anchorAdjust ) );
				//m_currOutputPortPos.y += drawInfo.InvertedZoom * Constants.PORT_INITIAL_Y;// + m_extraHeaderHeight * drawInfo.InvertedZoom;
				int outputCount = m_outputPorts.Count;

				for( int i = 0; i < outputCount; i++ )
				{
					if( m_outputPorts[ i ].Visible )
					{
						//Button
						m_outputPorts[ i ].Position = m_currOutputPortPos;

						if( !m_outputPorts[ i ].Locked )
						{
							float overflow = 2;
							float scaledOverflow = 3 * drawInfo.InvertedZoom;
							m_auxRect = m_currOutputPortPos;
							m_auxRect.yMin -= scaledOverflow + overflow;
							m_auxRect.yMax += scaledOverflow + overflow;
							m_auxRect.xMin -= m_outputPorts[ i ].LabelSize.x + Constants.PORT_TO_LABEL_SPACE_X * drawInfo.InvertedZoom + scaledOverflow + overflow;
							m_auxRect.xMax += Constants.PORT_INITIAL_X * drawInfo.InvertedZoom + scaledOverflow + overflow;
							m_outputPorts[ i ].ActivePortArea = m_auxRect;
						}
						m_currOutputPortPos.y += drawInfo.InvertedZoom * ( m_fontHeight + Constants.INPUT_PORT_DELTA_Y );
					}
				}
			}
		}

		public override void OnNodeRepaint( DrawInfo drawInfo )
		{
			//base.OnRepaint( drawInfo );
			//return;
			if( !m_isVisible )
				return;

			m_colorBuffer = GUI.color;

			// Output Ports
			int outputCount = m_outputPorts.Count;
			for( int i = 0; i < outputCount; i++ )
			{
				if( m_outputPorts[ i ].Visible )
				{
					// Output Port Icon
					if( ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD4 )
					{
						if( m_outputPorts[ i ].Locked )
							GUI.color = Constants.LockedPortColor;
						else if( ContainerGraph.ParentWindow.Options.ColoredPorts )
							GUI.color = UIUtils.GetColorForDataType( m_visualDataType, false, false );
						else
							GUI.color = m_outputPorts[ i ].HasCustomColor ? m_outputPorts[ i ].CustomColor : UIUtils.GetColorForDataType( m_visualDataType, true, false );

						GUIStyle style = m_outputPorts[ i ].IsConnected ? UIUtils.GetCustomStyle( CustomStyle.PortFullIcon ) : UIUtils.GetCustomStyle( CustomStyle.PortEmptyIcon );
						GUI.Label( m_outputPorts[ i ].Position, string.Empty, style );

						GUI.color = m_colorBuffer;
					}

					// Output Port Label
					if( ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD3 )
					{
						if( m_outputPorts[ i ].Locked )
						{
							GUI.color = Constants.PortLockedTextColor;
							GUI.Label( m_outputPorts[ i ].LabelPosition, m_outputPorts[ i ].Name, UIUtils.OutputPortLabel );
							GUI.color = m_colorBuffer;
						}
						else
						{
							GUI.Label( m_outputPorts[ i ].LabelPosition, m_outputPorts[ i ].Name, UIUtils.OutputPortLabel );
						}
					}
				}
			}

			// Selection Box
			if( m_selected )
			{
				Rect selectionBox = m_globalPosition;
				selectionBox.size = Vector2.one * 16 * drawInfo.InvertedZoom + Vector2.one * 4;
				selectionBox.center = m_globalPosition.center;
				GUI.DrawTexture( selectionBox, UIUtils.WireNodeSelection );
				GUI.color = m_colorBuffer;
			}
		}

		public override void DrawGUIControls( DrawInfo drawInfo )
		{
			//base.DrawGUIControls( drawInfo );
		}

		public override void Draw( DrawInfo drawInfo )
		{
			if( m_markedToDelete )
				return;

			if( drawInfo.CurrentEventType == EventType.Repaint )
				OnNodeRepaint( drawInfo );
			//base.Draw( drawInfo );

			if( drawInfo.CurrentEventType == EventType.Repaint )
				TestIfValid();
		}

		bool TestIfValid()
		{
			if( !Alive )
				return false;

			bool result = true;
			if( !m_inputPorts[ 0 ].IsConnected )
			{
				if( !m_containerGraph.ParentWindow.WireReferenceUtils.InputPortReference.IsValid || m_containerGraph.ParentWindow.WireReferenceUtils.InputPortReference.IsValid && m_containerGraph.ParentWindow.WireReferenceUtils.InputPortReference.NodeId != UniqueId )
				{
					ContainerGraph.MarkWireNodeSequence( this, true );
					result = false;
				}
			}

			if( !m_outputPorts[ 0 ].IsConnected )
			{
				if( !m_containerGraph.ParentWindow.WireReferenceUtils.OutputPortReference.IsValid || m_containerGraph.ParentWindow.WireReferenceUtils.OutputPortReference.IsValid && m_containerGraph.ParentWindow.WireReferenceUtils.OutputPortReference.NodeId != UniqueId )
				{
					ContainerGraph.MarkWireNodeSequence( this, false );
					result = false;
				}
			}
			return result;
		}

		public Vector3 TangentDirection
		{
			get
			{
				ParentNode otherInputNode = null;
				ParentNode otherOutputNode = null;

				//defaults to itself so it can still calculate tangents
				WirePort otherInputPort = m_outputPorts[ 0 ];
				WirePort otherOutputPort = m_inputPorts[ 0 ];

				if( m_outputPorts[ 0 ].ConnectionCount > 0 )
				{
					otherInputNode = m_containerGraph.GetNode( m_outputPorts[ 0 ].ExternalReferences[ 0 ].NodeId );
					otherInputPort = otherInputNode.GetInputPortByUniqueId( m_outputPorts[ 0 ].ExternalReferences[ 0 ].PortId );
				}

				if( m_inputPorts[ 0 ].ConnectionCount > 0 )
				{
					otherOutputNode = m_containerGraph.GetNode( m_inputPorts[ 0 ].ExternalReferences[ 0 ].NodeId );
					otherOutputPort = otherOutputNode.GetOutputPortByUniqueId( m_inputPorts[ 0 ].ExternalReferences[ 0 ].PortId );
				}

				//TODO: it still generates crooked lines if wire nodes get too close to non-wire nodes (the fix would be to calculate the non-wire nodes magnitude properly)
				float mag = Constants.HORIZONTAL_TANGENT_SIZE * ContainerGraph.ParentWindow.CameraDrawInfo.InvertedZoom;

				Vector2 outPos;
				if( otherOutputNode != null && otherOutputNode.GetType() != typeof( WireNode ) )
					outPos = otherOutputPort.Position.position + Vector2.right * mag * 0.66f;
				else
					outPos = otherOutputPort.Position.position;

				Vector2 inPos;
				if( otherInputNode != null && otherInputNode.GetType() != typeof( WireNode ) )
					inPos = otherInputPort.Position.position - Vector2.right * mag * 0.66f;
				else
					inPos = otherInputPort.Position.position;

				Vector2 tangent = ( outPos - inPos ).normalized;
				return new Vector3( tangent.x, tangent.y );
			}
		}

		public override void RefreshExternalReferences()
		{
			base.RefreshExternalReferences();

			m_extraSize.Set( 20f, 20f );
			m_position.width = m_extraSize.x + UIUtils.PortsSize.x;
			m_position.height = m_extraSize.y + UIUtils.PortsSize.y;

			Vec2Position += Position.size * 0.5f;
		}

		public override void OnAfterDeserialize()
		{
			base.OnAfterDeserialize();
			m_sizeIsDirty = false;
		}

		public WireReference FindNewValidInputNode( WireNode current )
		{
			if( current.InputPorts[ 0 ].IsConnected )
			{
				ParentNode node = m_containerGraph.GetNode( current.InputPorts[ 0 ].ExternalReferences[ 0 ].NodeId );
				if( node != null )
				{
					WireNode wireNode = node as WireNode;
					if( wireNode != null && wireNode.MarkToDelete )
					{
						return FindNewValidInputNode( wireNode );
					}
					else
					{
						return current.InputPorts[ 0 ].ExternalReferences[ 0 ];
					}
				}
			}
			return null;
		}

		public WireReference FindNewValidOutputNode( WireNode current )
		{
			if( current.OutputPorts[ 0 ].IsConnected )
			{
				ParentNode node = m_containerGraph.GetNode( current.OutputPorts[ 0 ].ExternalReferences[ 0 ].NodeId );

				if( node != null )
				{
					WireNode wireNode = node as WireNode;
					if( wireNode != null && wireNode.MarkToDelete )
					{
						return FindNewValidOutputNode( wireNode );
					}
					else
					{
						return current.OutputPorts[ 0 ].ExternalReferences[ 0 ];
					}
				}
			}
			return null;
		}

		public override void Rewire()
		{
			//if ( m_inputPorts[ 0 ].ExternalReferences != null && m_inputPorts[ 0 ].ExternalReferences.Count > 0 )
			//{
			//WireReference backPort = m_inputPorts[ 0 ].ExternalReferences[ 0 ];
			//for ( int i = 0; i < m_outputPorts[ 0 ].ExternalReferences.Count; i++ )
			//{
			//	UIUtils.CurrentWindow.ConnectInputToOutput( m_outputPorts[ 0 ].ExternalReferences[ i ].NodeId, m_outputPorts[ 0 ].ExternalReferences[ i ].PortId, backPort.NodeId, backPort.PortId );
			//}
			//}
			MarkToDelete = true;
			WireReference outputReference = FindNewValidInputNode( this );
			WireReference inputReference = FindNewValidOutputNode( this );
			if( outputReference != null && inputReference != null )
			{
				ContainerGraph.ParentWindow.ConnectInputToOutput( inputReference.NodeId, inputReference.PortId, outputReference.NodeId, outputReference.PortId );
			}
		}

		public bool MarkToDelete
		{
			get { return m_markedToDelete; }
			set { m_markedToDelete = value; }
		}
	}
}
