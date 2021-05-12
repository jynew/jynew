// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class DynamicTypeNode : ParentNode
	{
		protected string m_inputA = string.Empty;
		protected string m_inputB = string.Empty;
		protected List<string> m_extensibleInputResults;
		protected bool m_dynamicOutputType = true;

		protected bool m_extensibleInputPorts = false;
		protected bool m_allowMatrixCheck = false;
		protected bool m_vectorMatrixOps = false;
		//[SerializeField]
		private int m_inputCount = 2;

		//[SerializeField]
		private int m_lastInputCount = 2;

		private bool m_previouslyDragging = false;
		private int m_beforePreviewCount = 0;

        [UnityEngine.SerializeField]
        protected WirePortDataType m_mainDataType = WirePortDataType.FLOAT;

        protected WirePortDataType[] m_dynamicRestrictions =
		{
			WirePortDataType.OBJECT,
			WirePortDataType.FLOAT,
			WirePortDataType.FLOAT2,
			WirePortDataType.FLOAT3,
			WirePortDataType.FLOAT4,
			WirePortDataType.COLOR,
			WirePortDataType.INT
		};
        
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_useInternalPortData = true;
			m_textLabelWidth = 35;
			AddPorts();
		}

		protected virtual void AddPorts()
		{
			AddInputPort( WirePortDataType.FLOAT, false, "A" );
			AddInputPort( WirePortDataType.FLOAT, false, "B" );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_inputPorts[ 0 ].CreatePortRestrictions( m_dynamicRestrictions );
			m_inputPorts[ 1 ].CreatePortRestrictions( m_dynamicRestrictions );
		}

		public override void OnConnectedOutputNodeChanges( int inputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			UpdateConnection( inputPortId );
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			UpdateConnection( portId );
		}

		public override void OnInputPortDisconnected( int portId )
		{
			base.OnInputPortDisconnected( portId );
			UpdateDisconnectedConnection( portId );
			UpdateConnection( portId );
			UpdateEmptyInputPorts( true );
		}

		void UpdateDisconnectedConnection( int portId )
		{
			if( m_extensibleInputPorts || m_allowMatrixCheck )
			{
				int higher = 0;
				int groupOneType = 0;
				int groupTwoType = 0;
				for( int i = 0; i < m_inputPorts.Count; i++ )
				{
					if( m_inputPorts[ i ].IsConnected )
					{
						int currentPriority = UIUtils.GetPriority( m_inputPorts[ i ].DataType );
						if( !m_vectorMatrixOps && currentPriority < 3 )
							currentPriority += 7;
						if( currentPriority > higher && currentPriority > 2 )
						{
							higher = currentPriority;
							m_mainDataType = m_inputPorts[ i ].DataType;
						}
						switch( m_inputPorts[ i ].DataType )
						{
							case WirePortDataType.FLOAT2:
							case WirePortDataType.FLOAT3:
							case WirePortDataType.FLOAT4:
							case WirePortDataType.COLOR:
							{
								groupOneType++;
								groupTwoType++;
							}
							break;
							case WirePortDataType.FLOAT3x3:
							{
								groupOneType++;
							}
							break;
							case WirePortDataType.FLOAT4x4:
							{
								groupTwoType++;
							}
							break;
						}
					}
				}

				for( int i = 0; i < m_inputPorts.Count; i++ )
				{
					if( !m_inputPorts[ i ].IsConnected )
					{
						m_inputPorts[ i ].ChangeType( m_mainDataType, false );
					}
				}

				if( groupOneType > 0 && m_mainDataType == WirePortDataType.FLOAT4x4 )
				{
					m_errorMessageTooltip = "Doing this operation with FLOAT4x4 value only works against other FLOAT4x4 or FLOAT values";
					m_showErrorMessage = true;
				}
				else if( groupTwoType > 0 && m_mainDataType == WirePortDataType.FLOAT3x3 )
				{
					m_errorMessageTooltip = "Doing this operation with FLOAT3x3 value only works against other FLOAT3x3 or FLOAT values";
					m_showErrorMessage = true;
				}
				else
				{
					m_showErrorMessage = false;
				}

				if( m_dynamicOutputType )
					m_outputPorts[ 0 ].ChangeType( m_mainDataType, false );
			}
			else

			if( m_inputPorts[ 0 ].DataType != m_inputPorts[ 1 ].DataType )
			{
				int otherPortId = ( portId + 1 ) % 2;
				if( m_inputPorts[ otherPortId ].IsConnected )
				{
					m_mainDataType = m_inputPorts[ otherPortId ].DataType;
					m_inputPorts[ portId ].ChangeType( m_mainDataType, false );
					if( m_dynamicOutputType )
						m_outputPorts[ 0 ].ChangeType( m_mainDataType, false );
				}
				else
				{
					if( UIUtils.GetPriority( m_inputPorts[ 0 ].DataType ) > UIUtils.GetPriority( m_inputPorts[ 1 ].DataType ) )
					{
						m_mainDataType = m_inputPorts[ 0 ].DataType;
						m_inputPorts[ 1 ].ChangeType( m_mainDataType, false );
					}
					else
					{
						m_mainDataType = m_inputPorts[ 1 ].DataType;
						m_inputPorts[ 0 ].ChangeType( m_mainDataType, false );
					}

					if( m_dynamicOutputType )
					{
						if( m_mainDataType != m_outputPorts[ 0 ].DataType )
						{
							m_outputPorts[ 0 ].ChangeType( m_mainDataType, false );
						}
					}
				}
			}
		}

		void UpdateConnection( int portId )
		{
			if( m_extensibleInputPorts || m_allowMatrixCheck )
			{
				m_inputPorts[ portId ].MatchPortToConnection();

				int higher = 0;
				int groupOneType = 0;
				int groupTwoType = 0;
				for( int i = 0; i < m_inputPorts.Count; i++ )
				{
					if( m_inputPorts[ i ].IsConnected )
					{
						int currentPriority = UIUtils.GetPriority( m_inputPorts[ i ].DataType );
						if( !m_vectorMatrixOps && currentPriority < 3 )
							currentPriority += 7;
						if( currentPriority > higher )
						{
							higher = currentPriority;
							m_mainDataType = m_inputPorts[ i ].DataType;
						}
						switch( m_inputPorts[ i ].DataType )
						{
							case WirePortDataType.FLOAT2:
							case WirePortDataType.FLOAT3:
							case WirePortDataType.FLOAT4:
							case WirePortDataType.COLOR:
							{
								groupOneType++;
								groupTwoType++;
							}
							break;
							case WirePortDataType.FLOAT3x3:
							{
								groupOneType++;
							}
							break;
							case WirePortDataType.FLOAT4x4:
							{
								groupTwoType++;
							}
							break;
						}
					}
				}

				for( int i = 0; i < m_inputPorts.Count; i++ )
				{
					if( !m_inputPorts[ i ].IsConnected )
					{
						m_inputPorts[ i ].ChangeType( m_mainDataType, false );
					}
				}
				if( groupOneType > 0 && m_mainDataType == WirePortDataType.FLOAT4x4 )
				{
					m_errorMessageTooltip = "Doing this operation with FLOAT4x4 value only works against other FLOAT4x4 or FLOAT values";
					m_showErrorMessage = true;
				}
				else if( groupTwoType > 0 && m_mainDataType == WirePortDataType.FLOAT3x3 )
				{
					m_errorMessageTooltip = "Doing this operation with FLOAT3x3 value only works against other FLOAT3x3 or FLOAT values";
					m_showErrorMessage = true;
				}
				else
				{
					m_showErrorMessage = false;
				}

				if( m_dynamicOutputType )
					m_outputPorts[ 0 ].ChangeType( m_mainDataType, false );
			}

			else
			{
				m_inputPorts[ portId ].MatchPortToConnection();
				int otherPortId = ( portId + 1 ) % 2;
				if( !m_inputPorts[ otherPortId ].IsConnected )
				{
					m_inputPorts[ otherPortId ].ChangeType( m_inputPorts[ portId ].DataType, false );
				}

				if( m_inputPorts[ 0 ].DataType == m_inputPorts[ 1 ].DataType )
				{
					m_mainDataType = m_inputPorts[ 0 ].DataType;
					if( m_dynamicOutputType )
						m_outputPorts[ 0 ].ChangeType( InputPorts[ 0 ].DataType, false );
				}
				else
				{
					if( UIUtils.GetPriority( m_inputPorts[ 0 ].DataType ) > UIUtils.GetPriority( m_inputPorts[ 1 ].DataType ) )
					{
						m_mainDataType = m_inputPorts[ 0 ].DataType;
					}
					else
					{
						m_mainDataType = m_inputPorts[ 1 ].DataType;
					}

					if( m_dynamicOutputType )
					{
						if( m_mainDataType != m_outputPorts[ 0 ].DataType )
						{
							m_outputPorts[ 0 ].ChangeType( m_mainDataType, false );
						}
					}
				}
			}
		}

		public override void OnNodeLayout( DrawInfo drawInfo )
		{
			base.OnNodeLayout( drawInfo );

			if( !m_extensibleInputPorts )
				return;

			if( m_previouslyDragging != m_containerGraph.ParentWindow.WireReferenceUtils.OutputPortReference.IsValid && m_containerGraph.ParentWindow.WireReferenceUtils.OutputPortReference.NodeId != UniqueId )
			{
				if( m_containerGraph.ParentWindow.WireReferenceUtils.OutputPortReference.IsValid )
				{
					m_beforePreviewCount = 2;
					for( int i = 2; i < m_inputPorts.Count; i++ )
					{
						if( m_inputPorts[ i ].IsConnected )
						{
							m_beforePreviewCount++;
						}
					}

					m_inputCount = m_beforePreviewCount + 1;
					if( m_inputCount <= 10 )
					{
						if( m_inputCount > m_lastInputCount )
						{
							Undo.RegisterCompleteObjectUndo( m_containerGraph.ParentWindow, Constants.UndoCreateDynamicPortId );
							RecordObject( Constants.UndoCreateDynamicPortId );

							AddInputPort( m_mainDataType, false, ( ( char ) ( 'A' + m_inputCount - 1 ) ).ToString() );
							m_inputPorts[ m_inputCount - 1 ].CreatePortRestrictions( m_dynamicRestrictions );
							if( Selected && ContainerGraph.ParentWindow.ParametersWindow.IsMaximized )
								Event.current.type = EventType.Used;
						}

						m_lastInputCount = m_inputCount;
						m_sizeIsDirty = true;
						m_isDirty = true;
						SetSaveIsDirty();
					}
				}
				else
				{
					bool hasEmpty = CheckValidConnections();
					if( hasEmpty )
						UpdateEmptyInputPorts( false );
				}

				m_previouslyDragging = m_containerGraph.ParentWindow.WireReferenceUtils.OutputPortReference.IsValid;
			}

			UpdateEmptyInputPorts( false );
		}

		private bool CheckValidConnections()
		{
			if( !m_extensibleInputPorts )
				return false;

			bool hasEmptyConnections = false;

			bool hasMatrix = m_inputPorts[ 0 ].DataType == WirePortDataType.FLOAT3x3 || m_inputPorts[ 0 ].DataType == WirePortDataType.FLOAT4x4 || m_inputPorts[ 1 ].DataType == WirePortDataType.FLOAT3x3 || m_inputPorts[ 1 ].DataType == WirePortDataType.FLOAT4x4;

			if( m_inputPorts.Count != m_beforePreviewCount )
			{
				if( hasMatrix )
				{
					bool showError = false;
					for( int i = m_inputPorts.Count - 1; i >= 2; i-- )
					{
						if( m_inputPorts[ i ].IsConnected )
						{
							showError = true;
							m_inputPorts[ i ].FullDeleteConnections();
						}

						hasEmptyConnections = true;
					}
					if( showError )
						m_containerGraph.ParentWindow.ShowMessage( "Matrix operations are only valid for the first two inputs to prevent errors" );
				}
				else
				{
					for( int i = m_inputPorts.Count - 1; i >= 2; i-- )
					{
						if( m_inputPorts[ i ].DataType == WirePortDataType.FLOAT3x3 || m_inputPorts[ i ].DataType == WirePortDataType.FLOAT4x4 )
						{
							m_containerGraph.ParentWindow.ShowMessage( "Matrix operations are only valid for the first two inputs to prevent errors" );
							m_inputPorts[ i ].FullDeleteConnections();
							hasEmptyConnections = true;
						}
						else if( !m_inputPorts[ i ].IsConnected )
						{
							hasEmptyConnections = true;
						}
					}
				}
			}

			return hasEmptyConnections;
		}

		private void UpdateEmptyInputPorts( bool recordUndo )
		{
			if( !m_extensibleInputPorts )
				return;

			if( !m_containerGraph.ParentWindow.WireReferenceUtils.OutputPortReference.IsValid )
			{
				if( recordUndo )
				{
					Undo.RegisterCompleteObjectUndo( m_containerGraph.ParentWindow, Constants.UndoDeleteDynamicPortId );
					RecordObject( Constants.UndoDeleteDynamicPortId );
				}

				bool hasDeleted = false;
				m_inputCount = 2;
				for( int i = m_inputPorts.Count - 1; i >= 2; i-- )
				{
					if( !m_inputPorts[ i ].IsConnected )
					{
						hasDeleted = true;
						DeleteInputPortByArrayIdx( i );
					}
					else
					{
						m_inputCount++;
					}
				}

				if( hasDeleted || m_inputCount != m_lastInputCount )
				{
					for( int i = 2; i < m_inputPorts.Count; i++ )
					{
						m_inputPorts[ i ].Name = ( ( char ) ( 'A' + i ) ).ToString();
					}

					m_beforePreviewCount = m_inputPorts.Count;
					m_inputCount = m_beforePreviewCount;
					m_lastInputCount = m_inputCount;
					m_sizeIsDirty = true;
					m_isDirty = true;
					SetSaveIsDirty();
				}
			}

			m_inputCount = Mathf.Clamp( m_inputCount, 2, 10 );
		}

		public virtual string BuildResults( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( !m_extensibleInputPorts )
				SetInputData( outputId, ref dataCollector, ignoreLocalvar );
			else
				SetExtensibleInputData( outputId, ref dataCollector, ignoreLocalvar );
			return string.Empty;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			string result = BuildResults( outputId, ref dataCollector, ignoreLocalvar );
			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}

		protected void SetInputData( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			m_inputA = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			if( m_inputPorts[ 0 ].DataType != m_mainDataType )
			{
				m_inputA = UIUtils.CastPortType( ref dataCollector, m_currentPrecisionType, new NodeCastInfo( UniqueId, outputId ), m_inputA, m_inputPorts[ 0 ].DataType, m_mainDataType, m_inputA );
			}
			m_inputB = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );
			if( m_inputPorts[ 1 ].DataType != m_mainDataType )
			{
				m_inputB = UIUtils.CastPortType( ref dataCollector, m_currentPrecisionType, new NodeCastInfo( UniqueId, outputId ), m_inputB, m_inputPorts[ 1 ].DataType, m_mainDataType, m_inputB );
			}
		}

		protected void SetExtensibleInputData( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			m_extensibleInputResults = new List<string>();
			for( int i = 0; i < m_inputPorts.Count; i++ )
			{
				m_extensibleInputResults.Add( m_inputPorts[ i ].GeneratePortInstructions( ref dataCollector ) );
				if( m_inputPorts[ i ].DataType != m_mainDataType && m_inputPorts[ i ].DataType != WirePortDataType.FLOAT && m_inputPorts[ i ].DataType != WirePortDataType.INT )
				{
					m_extensibleInputResults[ i ] = UIUtils.CastPortType( ref dataCollector, m_currentPrecisionType, new NodeCastInfo( UniqueId, outputId ), m_extensibleInputResults[ i ], m_inputPorts[ i ].DataType, m_mainDataType, m_extensibleInputResults[ i ] );
				}
			}
		}

		void UpdatePorts()
		{
			m_lastInputCount = Mathf.Clamp( m_inputCount, 2, 10 );

			for( int i = 2; i < m_inputCount; i++ )
			{
				AddInputPort( m_mainDataType, false, ( ( char ) ( 'A' + i ) ).ToString() );
				m_inputPorts[ i ].CreatePortRestrictions( m_dynamicRestrictions );
			}

			m_sizeIsDirty = true;
			SetSaveIsDirty();
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if( m_extensibleInputPorts && UIUtils.CurrentShaderVersion() > 10005 )
			{
				m_inputCount = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				UpdatePorts();
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			if( m_extensibleInputPorts )
				IOUtils.AddFieldValueToString( ref nodeInfo, m_inputCount );
		}
	}
}
