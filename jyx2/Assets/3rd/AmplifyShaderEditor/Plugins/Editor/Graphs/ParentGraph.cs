// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class ParentGraph : ScriptableObject, ISerializationCallbackReceiver
	{
		public enum NodeLOD
		{
			LOD0,
			LOD1,
			LOD2,
			LOD3,
			LOD4,
			LOD5
		}

		private NodeLOD m_lodLevel = NodeLOD.LOD0;
		private GUIStyle nodeStyleOff;
		private GUIStyle nodeStyleOn;
		private GUIStyle nodeTitle;
		private GUIStyle commentaryBackground;

		public delegate void EmptyGraphDetected( ParentGraph graph );
		public event EmptyGraphDetected OnEmptyGraphDetectedEvt;

		public delegate void NodeEvent( ParentNode node );
		public event NodeEvent OnNodeEvent = null;
		public event NodeEvent OnNodeRemovedEvent;

		public delegate void DuplicateEvent();
		public event DuplicateEvent OnDuplicateEvent;

		public event MasterNode.OnMaterialUpdated OnMaterialUpdatedEvent;
		public event MasterNode.OnMaterialUpdated OnShaderUpdatedEvent;

		private bool m_afterDeserializeFlag = true;

		private bool m_foundDuplicates = false;

		//[SerializeField]
		private AmplifyShaderEditorWindow m_parentWindow = null;

		[SerializeField]
		private int m_validNodeId;

		[SerializeField]
		private List<ParentNode> m_nodes = new List<ParentNode>();

		// Sampler Nodes registry
		[SerializeField]
		private UsageListSamplerNodes m_samplerNodes = new UsageListSamplerNodes();

		[SerializeField]
		private UsageListFloatIntNodes m_floatNodes = new UsageListFloatIntNodes();

		[SerializeField]
		private UsageListTexturePropertyNodes m_texturePropertyNodes = new UsageListTexturePropertyNodes();

		[SerializeField]
		private UsageListTextureArrayNodes m_textureArrayNodes = new UsageListTextureArrayNodes();

		[SerializeField]
		private UsageListPropertyNodes m_propertyNodes = new UsageListPropertyNodes();

		[SerializeField]
		private UsageListPropertyNodes m_rawPropertyNodes = new UsageListPropertyNodes();

		[SerializeField]
		private UsageListScreenColorNodes m_screenColorNodes = new UsageListScreenColorNodes();

		[SerializeField]
		private UsageListRegisterLocalVarNodes m_localVarNodes = new UsageListRegisterLocalVarNodes();

		[SerializeField]
		private UsageListGlobalArrayNodes m_globalArrayNodes = new UsageListGlobalArrayNodes();

		[SerializeField]
		private UsageListFunctionInputNodes m_functionInputNodes = new UsageListFunctionInputNodes();

		[SerializeField]
		private UsageListFunctionNodes m_functionNodes = new UsageListFunctionNodes();

		[SerializeField]
		private UsageListFunctionOutputNodes m_functionOutputNodes = new UsageListFunctionOutputNodes();

		[SerializeField]
		private UsageListFunctionSwitchNodes m_functionSwitchNodes = new UsageListFunctionSwitchNodes();

		[SerializeField]
		private UsageListFunctionSwitchCopyNodes m_functionSwitchCopyNodes = new UsageListFunctionSwitchCopyNodes();

		[SerializeField]
		private UsageListTemplateMultiPassMasterNodes m_multiPassMasterNodes = new UsageListTemplateMultiPassMasterNodes();

		[SerializeField]
		private UsageListCustomExpressionsOnFunctionMode m_customExpressionsOnFunctionMode = new UsageListCustomExpressionsOnFunctionMode();


		[SerializeField]
		private int m_masterNodeId = Constants.INVALID_NODE_ID;

		[SerializeField]
		private bool m_isDirty;

		[SerializeField]
		private bool m_saveIsDirty = false;

		[SerializeField]
		private int m_nodeClicked;

		[SerializeField]
		private int m_loadedShaderVersion;

		[SerializeField]
		private int m_instancePropertyCount = 0;

		[SerializeField]
		private int m_virtualTextureCount = 0;

		[SerializeField]
		private int m_graphId = 0;

		[SerializeField]
		private PrecisionType m_currentPrecision = PrecisionType.Float;

		[SerializeField]
		private NodeAvailability m_currentCanvasMode = NodeAvailability.SurfaceShader;

		[SerializeField]
		private TemplateSRPType m_currentSRPType = TemplateSRPType.BuiltIn;

		//private List<ParentNode> m_visibleNodes = new List<ParentNode>();

		private List<ParentNode> m_nodePreviewList = new List<ParentNode>();

		private Dictionary<int, ParentNode> m_nodesDict = new Dictionary<int, ParentNode>();

		[NonSerialized]
		private List<ParentNode> m_selectedNodes = new List<ParentNode>();

		[NonSerialized]
		private List<ParentNode> m_markedForDeletion = new List<ParentNode>();

		[SerializeField]
		private List<WireReference> m_highlightedWires = new List<WireReference>();
		private System.Type m_masterNodeDefaultType;

		[SerializeField]
		private List<PropertyNode> m_internalTemplateNodesList = new List<PropertyNode>();
		private Dictionary<int, PropertyNode> m_internalTemplateNodesDict = new Dictionary<int, PropertyNode>();

		private NodeGrid m_nodeGrid;

		private bool m_markedToDeSelect = false;
		private int m_markToSelect = -1;
		private bool m_markToReOrder = false;

		private bool m_hasUnConnectedNodes = false;

		private bool m_checkSelectedWireHighlights = false;

		// Bezier info
		[SerializeField]
		private List<WireBezierReference> m_bezierReferences;
		private const int MaxBezierReferences = 50;
		private int m_wireBezierCount = 0;

		protected int m_normalDependentCount = 0;
		private bool m_forceCategoryRefresh = false;

		[SerializeField]
		private bool m_forceRepositionCheck = false;

		private bool m_isLoading = false;
		private bool m_isDuplicating = false;

		private bool m_changedLightingModel = false;

		public void ResetEvents()
		{
			OnNodeEvent = null;
			OnMaterialUpdatedEvent = null;
			OnShaderUpdatedEvent = null;
			OnEmptyGraphDetectedEvt = null;
			OnNodeRemovedEvent = null;
		}

		public void Init()
		{
			Undo.undoRedoPerformed += OnUndoRedoCallback;
			m_normalDependentCount = 0;
			m_nodes = new List<ParentNode>();
			m_samplerNodes = new UsageListSamplerNodes();
			m_samplerNodes.ContainerGraph = this;
			m_floatNodes = new UsageListFloatIntNodes();
			m_floatNodes.ContainerGraph = this;
			m_texturePropertyNodes = new UsageListTexturePropertyNodes();
			m_texturePropertyNodes.ContainerGraph = this;
			m_textureArrayNodes = new UsageListTextureArrayNodes();
			m_textureArrayNodes.ContainerGraph = this;
			m_propertyNodes = new UsageListPropertyNodes();
			m_propertyNodes.ContainerGraph = this;
			m_rawPropertyNodes = new UsageListPropertyNodes();
			m_rawPropertyNodes.ContainerGraph = this;
			m_customExpressionsOnFunctionMode = new UsageListCustomExpressionsOnFunctionMode();
			m_customExpressionsOnFunctionMode.ContainerGraph = this;
			m_screenColorNodes = new UsageListScreenColorNodes();
			m_screenColorNodes.ContainerGraph = this;
			m_localVarNodes = new UsageListRegisterLocalVarNodes();
			m_localVarNodes.ContainerGraph = this;
			m_globalArrayNodes = new UsageListGlobalArrayNodes();
			m_globalArrayNodes.ContainerGraph = this;
			m_functionInputNodes = new UsageListFunctionInputNodes();
			m_functionInputNodes.ContainerGraph = this;
			m_functionNodes = new UsageListFunctionNodes();
			m_functionNodes.ContainerGraph = this;
			m_functionOutputNodes = new UsageListFunctionOutputNodes();
			m_functionOutputNodes.ContainerGraph = this;
			m_functionSwitchNodes = new UsageListFunctionSwitchNodes();
			m_functionSwitchNodes.ContainerGraph = this;
			m_functionSwitchCopyNodes = new UsageListFunctionSwitchCopyNodes();
			m_functionSwitchCopyNodes.ContainerGraph = this;
			m_multiPassMasterNodes = new UsageListTemplateMultiPassMasterNodes();
			m_multiPassMasterNodes.ContainerGraph = this;
			m_selectedNodes = new List<ParentNode>();
			m_markedForDeletion = new List<ParentNode>();
			m_highlightedWires = new List<WireReference>();
			m_validNodeId = 0;
			IsDirty = false;
			SaveIsDirty = false;
			m_masterNodeDefaultType = typeof( StandardSurfaceOutputNode );

			m_bezierReferences = new List<WireBezierReference>( MaxBezierReferences );
			for( int i = 0; i < MaxBezierReferences; i++ )
			{
				m_bezierReferences.Add( new WireBezierReference() );
			}
		}

		private void OnUndoRedoCallback()
		{
			DeSelectAll();
		}

		private void OnEnable()
		{
			hideFlags = HideFlags.HideAndDontSave;
			m_nodeGrid = new NodeGrid();
			m_internalTemplateNodesDict = new Dictionary<int, PropertyNode>();
			m_nodesDict = new Dictionary<int, ParentNode>();
			nodeStyleOff = UIUtils.GetCustomStyle( CustomStyle.NodeWindowOff );
			nodeStyleOn = UIUtils.GetCustomStyle( CustomStyle.NodeWindowOn );
			nodeTitle = UIUtils.GetCustomStyle( CustomStyle.NodeHeader );
			commentaryBackground = UIUtils.GetCustomStyle( CustomStyle.CommentaryBackground );
		}

		public void UpdateRegisters()
		{
			m_samplerNodes.UpdateNodeArr();
			m_propertyNodes.UpdateNodeArr();
			m_rawPropertyNodes.UpdateNodeArr();
			m_customExpressionsOnFunctionMode.UpdateNodeArr();
			m_functionInputNodes.UpdateNodeArr();
			m_functionNodes.UpdateNodeArr();
			m_functionOutputNodes.UpdateNodeArr();
			m_functionSwitchNodes.UpdateNodeArr();
			m_functionSwitchCopyNodes.UpdateNodeArr();
			m_multiPassMasterNodes.UpdateNodeArr();
			m_texturePropertyNodes.UpdateNodeArr();
			m_textureArrayNodes.UpdateNodeArr();
			m_screenColorNodes.UpdateNodeArr();
			m_localVarNodes.UpdateNodeArr();
			m_globalArrayNodes.UpdateNodeArr();
		}

		public int GetValidId()
		{
			return m_validNodeId++;
		}

		void UpdateIdFromNode( ParentNode node )
		{
			if( node.UniqueId >= m_validNodeId )
			{
				m_validNodeId = node.UniqueId + 1;
			}
		}

		public void ResetNodeConnStatus()
		{
			for( int i = 0; i < m_nodes.Count; i++ )
			{
				if( m_nodes[ i ].ConnStatus == NodeConnectionStatus.Connected )
				{
					m_nodes[ i ].ConnStatus = NodeConnectionStatus.Not_Connected;
				}
			}
		}

		public void CleanUnusedNodes()
		{
			List<ParentNode> unusedNodes = new List<ParentNode>();
			for( int i = 0; i < m_nodes.Count; i++ )
			{
				if( m_nodes[ i ].ConnStatus == NodeConnectionStatus.Not_Connected )
				{
					unusedNodes.Add( m_nodes[ i ] );
				}
			}

			for( int i = 0; i < unusedNodes.Count; i++ )
			{
				DestroyNode( unusedNodes[ i ] );
			}
			unusedNodes.Clear();
			unusedNodes = null;

			IsDirty = true;
		}

		// Destroy all nodes excluding Master Node
		public void ClearGraph()
		{
			List<ParentNode> list = new List<ParentNode>();
			int count = m_nodes.Count;
			for( int i = 0; i < count; i++ )
			{
				if( m_nodes[ i ].UniqueId != m_masterNodeId )
				{
					list.Add( m_nodes[ i ] );
				}
			}

			while( list.Count > 0 )
			{
				DestroyNode( list[ 0 ] );
				list.RemoveAt( 0 );
			}
		}

		public void CleanNodes()
		{
			for( int i = 0; i < m_nodes.Count; i++ )
			{
				if( m_nodes[ i ] != null )
				{
					Undo.ClearUndo( m_nodes[ i ] );
					m_nodes[ i ].Destroy();
					GameObject.DestroyImmediate( m_nodes[ i ] );
				}
			}
			ClearInternalTemplateNodes();

			m_masterNodeId = Constants.INVALID_NODE_ID;
			m_validNodeId = 0;
			m_instancePropertyCount = 0;
			m_virtualTextureCount = 0;

			m_nodesDict.Clear();
			m_nodes.Clear();
			m_samplerNodes.Clear();
			m_propertyNodes.Clear();
			m_rawPropertyNodes.Clear();
			m_customExpressionsOnFunctionMode.Clear();
			m_functionInputNodes.Clear();
			m_functionNodes.Clear();
			m_functionOutputNodes.Clear();
			m_functionSwitchNodes.Clear();
			m_functionSwitchCopyNodes.Clear();
			m_multiPassMasterNodes.Clear();
			m_texturePropertyNodes.Clear();
			m_textureArrayNodes.Clear();
			m_screenColorNodes.Clear();
			m_localVarNodes.Clear();
			m_globalArrayNodes.Clear();
			m_selectedNodes.Clear();
			m_markedForDeletion.Clear();
		}

		public void ResetHighlightedWires()
		{
			for( int i = 0; i < m_highlightedWires.Count; i++ )
			{
				m_highlightedWires[ i ].WireStatus = WireStatus.Default;
			}
			m_highlightedWires.Clear();
		}

		public void HighlightWiresStartingNode( ParentNode node )
		{
			for( int outputIdx = 0; outputIdx < node.OutputPorts.Count; outputIdx++ )
			{
				for( int extIdx = 0; extIdx < node.OutputPorts[ outputIdx ].ExternalReferences.Count; extIdx++ )
				{
					WireReference wireRef = node.OutputPorts[ outputIdx ].ExternalReferences[ extIdx ];
					ParentNode nextNode = GetNode( wireRef.NodeId );
					if( nextNode && nextNode.ConnStatus == NodeConnectionStatus.Connected )
					{
						InputPort port = nextNode.GetInputPortByUniqueId( wireRef.PortId );
						if( port.ExternalReferences.Count == 0 || port.ExternalReferences[ 0 ].WireStatus == WireStatus.Highlighted )
						{
							// if even one wire is already highlighted then this tells us that this node was already been analysed
							return;
						}

						port.ExternalReferences[ 0 ].WireStatus = WireStatus.Highlighted;
						m_highlightedWires.Add( port.ExternalReferences[ 0 ] );
						HighlightWiresStartingNode( nextNode );
					}
				}
			}

			RegisterLocalVarNode regNode = node as RegisterLocalVarNode;
			if( (object)regNode != null )
			{
				int count = regNode.NodeReferences.Count;
				for( int i = 0; i < count; i++ )
				{
					HighlightWiresStartingNode( regNode.NodeReferences[ i ] );
				}
			}
		}

		void PropagateHighlightDeselection( ParentNode node, int portId = -1 )
		{
			if( portId > -1 )
			{
				InputPort port = node.GetInputPortByUniqueId( portId );
				port.ExternalReferences[ 0 ].WireStatus = WireStatus.Default;
			}

			if( node.Selected )
				return;

			for( int i = 0; i < node.InputPorts.Count; i++ )
			{
				if( node.InputPorts[ i ].ExternalReferences.Count > 0 && node.InputPorts[ i ].ExternalReferences[ 0 ].WireStatus == WireStatus.Highlighted )
				{
					// even though node is deselected, it receives wire highlight from a previous one 
					return;
				}
			}

			for( int outputIdx = 0; outputIdx < node.OutputPorts.Count; outputIdx++ )
			{
				for( int extIdx = 0; extIdx < node.OutputPorts[ outputIdx ].ExternalReferences.Count; extIdx++ )
				{
					WireReference wireRef = node.OutputPorts[ outputIdx ].ExternalReferences[ extIdx ];
					ParentNode nextNode = GetNode( wireRef.NodeId );
					PropagateHighlightDeselection( nextNode, wireRef.PortId );
				}
			}

			RegisterLocalVarNode regNode = node as RegisterLocalVarNode;
			if( (object)regNode != null )
			{
				int count = regNode.NodeReferences.Count;
				for( int i = 0; i < count; i++ )
				{
					PropagateHighlightDeselection( regNode.NodeReferences[ i ], -1 );
				}
			}
		}


		public void ResetNodesData()
		{
			int count = m_nodes.Count;
			for( int i = 0; i < count; i++ )
			{
				m_nodes[ i ].ResetNodeData();
			}
		}

		public void FullCleanUndoStack()
		{
			Undo.ClearUndo( this );
			int count = m_nodes.Count;
			for( int i = 0; i < count; i++ )
			{
				if( m_nodes[ i ] != null )
				{
					Undo.ClearUndo( m_nodes[ i ] );
				}
			}
		}

		public void FullRegisterOnUndoStack()
		{
			Undo.RegisterCompleteObjectUndo( this, Constants.UndoRegisterFullGrapId );
			int count = m_nodes.Count;
			for( int i = 0; i < count; i++ )
			{
				if( m_nodes[ i ] != null )
				{
					Undo.RegisterCompleteObjectUndo( m_nodes[ i ], Constants.UndoRegisterFullGrapId );
				}
			}
		}

		public void CheckPropertiesAutoRegister( ref MasterNodeDataCollector dataCollector )
		{
			List<PropertyNode> propertyNodesList = m_rawPropertyNodes.NodesList;
			int propertyCount = propertyNodesList.Count;
			for( int i = 0; i < propertyCount; i++ )
			{
				propertyNodesList[ i ].CheckIfAutoRegister( ref dataCollector );
			}
			propertyNodesList = null;

			List<GlobalArrayNode> globalArrayNodeList = m_globalArrayNodes.NodesList;
			int globalArrayCount = globalArrayNodeList.Count;
			for( int i = 0; i < globalArrayCount; i++ )
			{
				globalArrayNodeList[ i ].CheckIfAutoRegister( ref dataCollector );
			}
			globalArrayNodeList = null;

			//List<PropertyNode> propertyNodesList = m_propertyNodes.NodesList;
			//int propertyCount = propertyNodesList.Count;
			//for( int i = 0; i < propertyCount; i++ )
			//{
			//	propertyNodesList[ i ].CheckIfAutoRegister( ref dataCollector );
			//}
			//propertyNodesList = null;

			//List<ScreenColorNode> screenColorNodes = m_screenColorNodes.NodesList;
			//int screenColorNodesCount = screenColorNodes.Count;
			//for( int i = 0; i < screenColorNodesCount; i++ )
			//{
			//	screenColorNodes[ i ].CheckIfAutoRegister( ref dataCollector );
			//}
			//screenColorNodes = null;
		}

		public void SoftDestroy()
		{
			OnNodeRemovedEvent = null;

			m_masterNodeId = Constants.INVALID_NODE_ID;
			m_validNodeId = 0;
			
			m_nodeGrid.Destroy();
			//m_nodeGrid = null;

			ClearInternalTemplateNodes();

			for( int i = 0; i < m_nodes.Count; i++ )
			{
				if( m_nodes[ i ] != null )
				{
					m_nodes[ i ].Destroy();
					GameObject.DestroyImmediate( m_nodes[ i ] );
				}
			}

			m_instancePropertyCount = 0;

			m_nodes.Clear();
			//m_nodes = null;

			m_nodesDict.Clear();
			//m_nodesDict = null;

			m_samplerNodes.Clear();
			//m_samplerNodes = null;

			m_propertyNodes.Clear();
			m_rawPropertyNodes.Clear();
			//m_propertyNodes = null;

			m_customExpressionsOnFunctionMode.Clear();

			m_functionInputNodes.Clear();
			//m_functionInputNodes = null;

			m_functionNodes.Clear();
			//m_functionNodes = null;

			m_functionOutputNodes.Clear();
			//m_functionOutputNodes = null;

			m_functionSwitchNodes.Clear();
			//m_functionSwitchNodes = null;

			m_functionSwitchCopyNodes.Clear();
			//m_functionSwitchCopyNodes = null;

			m_texturePropertyNodes.Clear();
			//m_texturePropertyNodes = null;

			m_textureArrayNodes.Clear();
			//m_textureArrayNodes = null;

			m_screenColorNodes.Clear();
			//m_screenColorNodes = null;

			m_localVarNodes.Clear();
			//m_localVarNodes = null;

			m_globalArrayNodes.Clear();

			m_selectedNodes.Clear();
			//m_selectedNodes = null;

			m_markedForDeletion.Clear();
			//m_markedForDeletion = null;

			m_nodePreviewList.Clear();
			//m_nodePreviewList = null;

			IsDirty = true;

			OnNodeEvent = null;
			OnDuplicateEvent = null;
			//m_currentShaderFunction = null;

			OnMaterialUpdatedEvent = null;
			OnShaderUpdatedEvent = null;
			OnEmptyGraphDetectedEvt = null;

			nodeStyleOff = null;
			nodeStyleOn = null;
			nodeTitle = null;
			commentaryBackground = null;
		}




		public void Destroy()
		{
			Undo.undoRedoPerformed -= OnUndoRedoCallback;
			for( int i = 0; i < m_nodes.Count; i++ )
			{
				if( m_nodes[ i ] != null )
				{
					Undo.ClearUndo( m_nodes[ i ] );
					m_nodes[ i ].Destroy();
					GameObject.DestroyImmediate( m_nodes[ i ] );
				}
			}

			//Must be before m_propertyNodes.Destroy();
			ClearInternalTemplateNodes();
			m_internalTemplateNodesDict = null;
			m_internalTemplateNodesList = null;

			OnNodeRemovedEvent = null;

			m_masterNodeId = Constants.INVALID_NODE_ID;
			m_validNodeId = 0;
			m_instancePropertyCount = 0;

			m_nodeGrid.Destroy();
			m_nodeGrid = null;

			m_nodes.Clear();
			m_nodes = null;

			m_samplerNodes.Destroy();
			m_samplerNodes = null;

			m_propertyNodes.Destroy();
			m_propertyNodes = null;

			m_rawPropertyNodes.Destroy();
			m_rawPropertyNodes = null;

			m_customExpressionsOnFunctionMode.Destroy();
			m_customExpressionsOnFunctionMode = null;

			m_functionInputNodes.Destroy();
			m_functionInputNodes = null;

			m_functionNodes.Destroy();
			m_functionNodes = null;

			m_functionOutputNodes.Destroy();
			m_functionOutputNodes = null;

			m_functionSwitchNodes.Destroy();
			m_functionSwitchNodes = null;

			m_functionSwitchCopyNodes.Destroy();
			m_functionSwitchCopyNodes = null;

			m_multiPassMasterNodes.Destroy();
			m_multiPassMasterNodes = null;

			m_texturePropertyNodes.Destroy();
			m_texturePropertyNodes = null;

			m_textureArrayNodes.Destroy();
			m_textureArrayNodes = null;

			m_screenColorNodes.Destroy();
			m_screenColorNodes = null;

			m_localVarNodes.Destroy();
			m_localVarNodes = null;

			m_globalArrayNodes.Destroy();
			m_globalArrayNodes = null;

			m_selectedNodes.Clear();
			m_selectedNodes = null;

			m_markedForDeletion.Clear();
			m_markedForDeletion = null;


			m_nodesDict.Clear();
			m_nodesDict = null;

			m_nodePreviewList.Clear();
			m_nodePreviewList = null;

			IsDirty = true;

			OnNodeEvent = null;
			OnDuplicateEvent = null;
			//m_currentShaderFunction = null;

			OnMaterialUpdatedEvent = null;
			OnShaderUpdatedEvent = null;
			OnEmptyGraphDetectedEvt = null;

			nodeStyleOff = null;
			nodeStyleOn = null;
			nodeTitle = null;
			commentaryBackground = null;
		}

		void OnNodeChangeSizeEvent( ParentNode node )
		{
			m_nodeGrid.RemoveNodeFromGrid( node, true );
			m_nodeGrid.AddNodeToGrid( node );
		}

		public void OnNodeFinishMoving( ParentNode node, bool testOnlySelected, InteractionMode interactionMode )
		{
			if( OnNodeEvent != null )
			{
				OnNodeEvent( node );
				SaveIsDirty = true;
			}

			m_nodeGrid.RemoveNodeFromGrid( node, true );
			m_nodeGrid.AddNodeToGrid( node );

			//if( testOnlySelected )
			//{
			//	for( int i = m_visibleNodes.Count - 1; i > -1; i-- )
			//	{
			//		if( node.UniqueId != m_visibleNodes[ i ].UniqueId )
			//		{
			//			switch( interactionMode )
			//			{
			//				case InteractionMode.Target:
			//				{
			//					node.OnNodeInteraction( m_visibleNodes[ i ] );
			//				}
			//				break;
			//				case InteractionMode.Other:
			//				{
			//					m_visibleNodes[ i ].OnNodeInteraction( node );
			//				}
			//				break;
			//				case InteractionMode.Both:
			//				{
			//					node.OnNodeInteraction( m_visibleNodes[ i ] );
			//					m_visibleNodes[ i ].OnNodeInteraction( node );
			//				}
			//				break;
			//			}
			//		}
			//	}
			//}
			//else
			{
				for( int i = m_nodes.Count - 1; i > -1; i-- )
				{
					if( node.UniqueId != m_nodes[ i ].UniqueId )
					{
						switch( interactionMode )
						{
							case InteractionMode.Target:
							{
								node.OnNodeInteraction( m_nodes[ i ] );
							}
							break;
							case InteractionMode.Other:
							{
								m_nodes[ i ].OnNodeInteraction( node );
							}
							break;
							case InteractionMode.Both:
							{
								node.OnNodeInteraction( m_nodes[ i ] );
								m_nodes[ i ].OnNodeInteraction( node );
							}
							break;
						}
					}
				}
			}
		}


		public void OnNodeReOrderEvent( ParentNode node, int index )
		{
			if( node.Depth < index )
			{
				Debug.LogWarning( "Reorder canceled: This is a specific method for when reordering needs to be done and a its original index is higher than the new one" );
			}
			else
			{
				m_nodes.Remove( node );
				m_nodes.Insert( index, node );
				m_markToReOrder = true;
			}
		}

		public void AddNode( ParentNode node, bool updateId = false, bool addLast = true, bool registerUndo = true, bool fetchMaterialValues = true )
		{
			if( registerUndo )
			{
				UIUtils.MarkUndoAction();
				Undo.RegisterCompleteObjectUndo( ParentWindow, Constants.UndoCreateNodeId );
				Undo.RegisterCompleteObjectUndo( this, Constants.UndoCreateNodeId );
				Undo.RegisterCreatedObjectUndo( node, Constants.UndoCreateNodeId );
			}

			if( OnNodeEvent != null )
			{
				OnNodeEvent( node );
			}
			if( updateId )
			{
				node.UniqueId = GetValidId();
			}
			else
			{
				UpdateIdFromNode( node );
			}



			if( addLast )
			{
				m_nodes.Add( node );
				node.Depth = m_nodes.Count;
			}
			else
			{
				m_nodes.Insert( 0, node );
				node.Depth = 0;
			}

			if( m_nodesDict.ContainsKey( node.UniqueId ) )
			{
				//m_nodesDict[ node.UniqueId ] = node;
				m_foundDuplicates = true;
			}
			else
			{
				m_nodesDict.Add( node.UniqueId, node );
				node.SetMaterialMode( CurrentMaterial, fetchMaterialValues );
			}

			m_nodeGrid.AddNodeToGrid( node );
			node.OnNodeChangeSizeEvent += OnNodeChangeSizeEvent;
			node.OnNodeReOrderEvent += OnNodeReOrderEvent;
			IsDirty = true;
		}

		public void CheckForDuplicates()
		{
			if( m_foundDuplicates )
			{
				Debug.LogWarning( "Found duplicates:" );
				m_foundDuplicates = false;
				m_nodesDict.Clear();
				int count = m_nodes.Count;
				for( int i = 0; i < count; i++ )
				{
					if( m_nodesDict.ContainsKey( m_nodes[ i ].UniqueId ) )
					{
						m_nodes[ i ].UniqueId = GetValidId();
						m_nodesDict.Add( m_nodes[ i ].UniqueId, m_nodes[ i ] );
						Debug.LogWarning( "Assigning new ID to " + m_nodes[ i ].TypeName );
					}
					else
					{
						m_nodesDict.Add( m_nodes[ i ].UniqueId, m_nodes[ i ] );
					}
				}
			}
		}

		public ParentNode GetClickedNode()
		{
			if( m_nodeClicked < 0 )
				return null;
			return GetNode( m_nodeClicked );
		}

		public PropertyNode GetInternalTemplateNode( int nodeId )
		{
			if( m_internalTemplateNodesDict.Count != m_internalTemplateNodesList.Count )
			{
				m_internalTemplateNodesDict.Clear();
				int count = m_internalTemplateNodesList.Count;
				for( int i = 0; i < m_internalTemplateNodesList.Count; i++ )
				{
					if( m_internalTemplateNodesList[ i ] != null )
						m_internalTemplateNodesDict.Add( m_internalTemplateNodesList[ i ].UniqueId, m_internalTemplateNodesList[ i ] );
				}
			}

			if( m_internalTemplateNodesDict.ContainsKey( nodeId ) )
				return m_internalTemplateNodesDict[ nodeId ];

			return null;
		}

		public void AddInternalTemplateNode( TemplateShaderPropertyData data )
		{
			PropertyNode propertyNode = null;
			switch( data.PropertyDataType )
			{
				case WirePortDataType.FLOAT:
				propertyNode = CreateInstance<RangedFloatNode>(); break;
				case WirePortDataType.FLOAT4:
				propertyNode = CreateInstance<Vector4Node>();
				break;
				case WirePortDataType.COLOR:
				propertyNode = CreateInstance<ColorNode>();
				break;
				case WirePortDataType.INT:
				propertyNode = CreateInstance<IntNode>(); break;
				case WirePortDataType.SAMPLER1D:
				case WirePortDataType.SAMPLER2D:
				case WirePortDataType.SAMPLER3D:
				case WirePortDataType.SAMPLERCUBE:
				propertyNode = CreateInstance<SamplerNode>();
				break;
				default: return;
			}

			propertyNode.PropertyNameFromTemplate( data );

			// Create a negative unique Id to separate it from 
			// the regular ids on the main nodes list 
			// Its begins at -2 since -1 is used to detect invalid values
			int uniqueId = -( m_internalTemplateNodesList.Count + 2 );
			propertyNode.SetBaseUniqueId( uniqueId );

			//Register into Float/Int Nodes list to be available inline
			// Unique Id must be already set at this point to properly 
			// create array
			if( data.PropertyDataType == WirePortDataType.FLOAT ||
				data.PropertyDataType == WirePortDataType.INT )
				m_floatNodes.AddNode( propertyNode );

			m_internalTemplateNodesList.Add( propertyNode );
			m_internalTemplateNodesDict.Add( uniqueId, propertyNode );
		}

		public void ClearInternalTemplateNodes()
		{
			if( m_internalTemplateNodesList != null )
			{
				int count = m_internalTemplateNodesList.Count;
				for( int i = 0; i < count; i++ )
				{
					m_internalTemplateNodesList[ i ].Destroy();
					GameObject.DestroyImmediate( m_internalTemplateNodesList[ i ] );
				}

				m_internalTemplateNodesList.Clear();
				m_internalTemplateNodesDict.Clear();
			}
		}

		public ParentNode GetNode( int nodeId )
		{
			if( m_nodesDict.Count != m_nodes.Count )
			{
				m_nodesDict.Clear();
				int count = m_nodes.Count;
				for( int i = 0; i < count; i++ )
				{
					if( m_nodes[ i ] != null && !m_nodesDict.ContainsKey( m_nodes[ i ].UniqueId ) )
						m_nodesDict.Add( m_nodes[ i ].UniqueId, m_nodes[ i ] );
				}
			}

			if( m_nodesDict.ContainsKey( nodeId ) )
				return m_nodesDict[ nodeId ];

			return null;
		}

		public void ForceReOrder()
		{
			m_nodes.Sort( ( x, y ) => x.Depth.CompareTo( y.Depth ) );
		}

		public bool Draw( DrawInfo drawInfo )
		{
			MasterNode masterNode = GetNode( m_masterNodeId ) as MasterNode;
			if( m_forceCategoryRefresh && masterNode != null )
			{
				masterNode.RefreshAvailableCategories();
				m_forceCategoryRefresh = false;
			}

			SaveIsDirty = false;
			if( m_afterDeserializeFlag )
			{
				// this is now done after logic update... templates needs it this way
				//m_afterDeserializeFlag = false;

				CleanCorruptedNodes();
				if( m_nodes.Count == 0 )
				{
					//TODO: remove this temp from here
					NodeAvailability cachedCanvas = CurrentCanvasMode;
					ParentWindow.CreateNewGraph( "Empty" );
					CurrentCanvasMode = cachedCanvas;
					if( OnEmptyGraphDetectedEvt != null )
					{
						OnEmptyGraphDetectedEvt( this );
						SaveIsDirty = false;
					}
					else
					{
						SaveIsDirty = true;
					}
				}

				//for( int i = 0; i < m_nodes.Count; i++ )
				//{
				//	m_nodes[ i ].SetContainerGraph( this );
				//}
			}

			if( drawInfo.CurrentEventType == EventType.Repaint )
			{
				if( m_markedToDeSelect )
					DeSelectAll();

				if( m_markToSelect > -1 )
				{
					AddToSelectedNodes( GetNode( m_markToSelect ) );
					m_markToSelect = -1;
				}

				if( m_markToReOrder )
				{
					m_markToReOrder = false;
					int nodesCount = m_nodes.Count;
					for( int i = 0; i < nodesCount; i++ )
					{
						m_nodes[ i ].Depth = i;
					}
				}
			}

			if( drawInfo.CurrentEventType == EventType.Repaint )
			{
				// Resizing Nods per LOD level
				NodeLOD newLevel = NodeLOD.LOD0;
				float referenceValue;
				if( drawInfo.InvertedZoom > 0.5f )
				{
					newLevel = NodeLOD.LOD0;
					referenceValue = 4;
				}
				else if( drawInfo.InvertedZoom > 0.25f )
				{
					newLevel = NodeLOD.LOD1;
					referenceValue = 2;
				}
				else if( drawInfo.InvertedZoom > 0.15f )
				{
					newLevel = NodeLOD.LOD2;
					referenceValue = 1;
				}
				else if( drawInfo.InvertedZoom > 0.1f )
				{
					newLevel = NodeLOD.LOD3;
					referenceValue = 0;
				}
				else if( drawInfo.InvertedZoom > 0.07f )
				{
					newLevel = NodeLOD.LOD4;
					referenceValue = 0;
				}
				else
				{
					newLevel = NodeLOD.LOD5;
					referenceValue = 0;
				}

				// Just a sanity check
				nodeStyleOff = UIUtils.GetCustomStyle( CustomStyle.NodeWindowOff );
				nodeStyleOn = UIUtils.GetCustomStyle( CustomStyle.NodeWindowOn );//= UIUtils.GetCustomStyle( CustomStyle.NodeWindowOn );
				nodeTitle = UIUtils.GetCustomStyle( CustomStyle.NodeHeader );
				commentaryBackground = UIUtils.GetCustomStyle( CustomStyle.CommentaryBackground );

				if( newLevel != m_lodLevel || ( UIUtils.MainSkin != null && UIUtils.MainSkin.textField.border.left != referenceValue ) )
				{
					m_lodLevel = newLevel;
					switch( m_lodLevel )
					{
						default:
						case NodeLOD.LOD0:
						{
							UIUtils.MainSkin.textField.border = UIUtils.RectOffsetFour;
							nodeStyleOff.border = UIUtils.RectOffsetSix;
							UIUtils.NodeWindowOffSquare.border = UIUtils.RectOffsetFour;

							nodeStyleOn.border = UIUtils.RectOffsetSix;
							UIUtils.NodeWindowOnSquare.border = UIUtils.RectOffsetSix;

							nodeTitle.border.left = 6;
							nodeTitle.border.right = 6;
							nodeTitle.border.top = 6;
							nodeTitle.border.bottom = 4;

							UIUtils.NodeHeaderSquare.border = UIUtils.RectOffsetFour;
							commentaryBackground.border = UIUtils.RectOffsetSix;
						}
						break;
						case NodeLOD.LOD1:
						{
							UIUtils.MainSkin.textField.border = UIUtils.RectOffsetTwo;
							nodeStyleOff.border = UIUtils.RectOffsetFive;
							UIUtils.NodeWindowOffSquare.border = UIUtils.RectOffsetFive;

							nodeStyleOn.border = UIUtils.RectOffsetFive;
							UIUtils.NodeWindowOnSquare.border = UIUtils.RectOffsetFour;

							nodeTitle.border.left = 5;
							nodeTitle.border.right = 5;
							nodeTitle.border.top = 5;
							nodeTitle.border.bottom = 2;

							UIUtils.NodeHeaderSquare.border = UIUtils.RectOffsetThree;
							commentaryBackground.border = UIUtils.RectOffsetFive;
						}
						break;
						case NodeLOD.LOD2:
						{
							UIUtils.MainSkin.textField.border = UIUtils.RectOffsetOne;

							nodeStyleOff.border.left = 2;
							nodeStyleOff.border.right = 2;
							nodeStyleOff.border.top = 2;
							nodeStyleOff.border.bottom = 3;

							UIUtils.NodeWindowOffSquare.border = UIUtils.RectOffsetThree;

							nodeStyleOn.border.left = 4;
							nodeStyleOn.border.right = 4;
							nodeStyleOn.border.top = 4;
							nodeStyleOn.border.bottom = 3;

							UIUtils.NodeWindowOnSquare.border = UIUtils.RectOffsetThree;

							nodeTitle.border = UIUtils.RectOffsetTwo;
							UIUtils.NodeHeaderSquare.border = UIUtils.RectOffsetTwo;

							commentaryBackground.border.left = 2;
							commentaryBackground.border.right = 2;
							commentaryBackground.border.top = 2;
							commentaryBackground.border.bottom = 3;
						}
						break;
						case NodeLOD.LOD3:
						case NodeLOD.LOD4:
						case NodeLOD.LOD5:
						{
							UIUtils.MainSkin.textField.border = UIUtils.RectOffsetZero;

							nodeStyleOff.border.left = 1;
							nodeStyleOff.border.right = 1;
							nodeStyleOff.border.top = 1;
							nodeStyleOff.border.bottom = 2;

							UIUtils.NodeWindowOffSquare.border = UIUtils.RectOffsetTwo;

							nodeStyleOn.border = UIUtils.RectOffsetTwo;
							UIUtils.NodeWindowOnSquare.border = UIUtils.RectOffsetTwo;

							nodeTitle.border = UIUtils.RectOffsetOne;
							UIUtils.NodeHeaderSquare.border = UIUtils.RectOffsetOne;

							commentaryBackground.border.left = 1;
							commentaryBackground.border.right = 1;
							commentaryBackground.border.top = 1;
							commentaryBackground.border.bottom = 2;
						}
						break;
					}
				}
			}

			//m_visibleNodes.Clear();
			//int nullCount = 0;
			m_hasUnConnectedNodes = false;
			bool repaint = false;
			Material currentMaterial = masterNode != null ? masterNode.CurrentMaterial : null;
			EditorGUI.BeginChangeCheck();
			bool repaintMaterialInspector = false;

			int nodeCount = m_nodes.Count;
			for( int i = 0; i < nodeCount; i++ )
			{
				m_nodes[ i ].OnNodeLogicUpdate( drawInfo );
			}

			if( m_afterDeserializeFlag )
			{
				m_afterDeserializeFlag = false;
				if( CurrentCanvasMode == NodeAvailability.TemplateShader )
				{
					RefreshLinkedMasterNodes();
					CurrentMasterNode.OnRefreshLinkedPortsComplete();
					//RepositionTemplateNodes( CurrentMasterNode );
				}
			}

			if( m_forceRepositionCheck )
			{
				RepositionTemplateNodes( CurrentMasterNode );
			}

			//for( int i = 0; i < m_functionNodes.NodesList.Count; i++ )
			//{
			//	m_functionNodes.NodesList[ i ].LogicGraph();
			//}

			//for( int i = 0; i < UIUtils.FunctionSwitchCopyList().Count; i++ )
			//{
			//	UIUtils.FunctionSwitchCopyList()[ i ].CheckReference();
			//}



			// Dont use nodeCount variable because node count can change in this loop???
			nodeCount = m_nodes.Count;
			ParentNode node = null;
			for( int i = 0; i < nodeCount; i++ )
			{
				node = m_nodes[ i ];
				if( !node.IsOnGrid )
				{
					m_nodeGrid.AddNodeToGrid( node );
				}

				node.MovingInFrame = false;

				if( drawInfo.CurrentEventType == EventType.Repaint )
					node.OnNodeLayout( drawInfo );

				m_hasUnConnectedNodes = m_hasUnConnectedNodes ||
										( node.ConnStatus != NodeConnectionStatus.Connected && node.ConnStatus != NodeConnectionStatus.Island );

				if( node.RequireMaterialUpdate && currentMaterial != null )
				{
					node.UpdateMaterial( currentMaterial );
					repaintMaterialInspector = true;
				}

				//if( node.IsVisible )
				//	m_visibleNodes.Add( node );

				IsDirty = ( m_isDirty || node.IsDirty );
				SaveIsDirty = ( m_saveIsDirty || node.SaveIsDirty );
			}

			// Handles GUI controls
			nodeCount = m_nodes.Count;
			for( int i = nodeCount - 1; i >= 0; i-- )
			//for ( int i = 0; i < nodeCount; i++ )
			{
				node = m_nodes[ i ];
				bool restoreMouse = false;
				if( drawInfo.CurrentEventType == EventType.MouseDown && m_nodeClicked > -1 && node.UniqueId != m_nodeClicked )
				{
					restoreMouse = true;
					drawInfo.CurrentEventType = EventType.Ignore;
				}

				node.DrawGUIControls( drawInfo );

				if( restoreMouse )
				{
					drawInfo.CurrentEventType = EventType.MouseDown;
				}
			}

			// Draw connection wires
			if( drawInfo.CurrentEventType == EventType.Repaint )
				DrawWires( ParentWindow.WireTexture, drawInfo, ParentWindow.WindowContextPallete.IsActive, ParentWindow.WindowContextPallete.CurrentPosition );

			// Master Draw
			nodeCount = m_nodes.Count;
			for( int i = 0; i < nodeCount; i++ )
			{
				node = m_nodes[ i ];
				bool restoreMouse = false;
				if( drawInfo.CurrentEventType == EventType.MouseDown && m_nodeClicked > -1 && node.UniqueId != m_nodeClicked )
				{
					restoreMouse = true;
					drawInfo.CurrentEventType = EventType.Ignore;
				}

				node.Draw( drawInfo );

				if( restoreMouse )
				{
					drawInfo.CurrentEventType = EventType.MouseDown;
				}
			}

			// Draw Tooltip
			if( drawInfo.CurrentEventType == EventType.Repaint || drawInfo.CurrentEventType == EventType.MouseDown )
			{
				nodeCount = m_nodes.Count;
				for( int i = nodeCount - 1; i >= 0; i-- )
				{
					node = m_nodes[ i ];
					if( node.IsVisible && !node.IsMoving )
					{
						bool showing = node.ShowTooltip( drawInfo );
						if( showing )
							break;
					}
				}
			}

			if( repaintMaterialInspector )
			{
				if( ASEMaterialInspector.Instance != null )
				{
					ASEMaterialInspector.Instance.Repaint();
				}
			}

			if( m_checkSelectedWireHighlights )
			{
				m_checkSelectedWireHighlights = false;
				ResetHighlightedWires();
				for( int i = 0; i < m_selectedNodes.Count; i++ )
				{
					HighlightWiresStartingNode( m_selectedNodes[ i ] );
				}
			}

			if( EditorGUI.EndChangeCheck() )
			{
				SaveIsDirty = true;
				repaint = true;
			}

			if( drawInfo.CurrentEventType == EventType.Repaint )
			{
				// Revert LOD changes to LOD0 (only if it's different)
				if( UIUtils.MainSkin.textField.border.left != 4 )
				{
					UIUtils.MainSkin.textField.border = UIUtils.RectOffsetFour;
					nodeStyleOff.border = UIUtils.RectOffsetSix;
					UIUtils.NodeWindowOffSquare.border = UIUtils.RectOffsetFour;

					nodeStyleOn.border = UIUtils.RectOffsetSix;
					UIUtils.NodeWindowOnSquare.border = UIUtils.RectOffsetSix;

					nodeTitle.border.left = 6;
					nodeTitle.border.right = 6;
					nodeTitle.border.top = 6;
					nodeTitle.border.bottom = 4;

					UIUtils.NodeHeaderSquare.border = UIUtils.RectOffsetFour;
					commentaryBackground.border = UIUtils.RectOffsetSix;
				}
			}

			//if ( nullCount == m_nodes.Count )
			//	m_nodes.Clear();

			ChangedLightingModel = false;

			return repaint;
		}

		public bool UpdateMarkForDeletion()
		{
			if( m_markedForDeletion.Count != 0 )
			{
				DeleteMarkedForDeletionNodes();
				return true;
			}
			return false;
		}

		public void DrawWires( Texture2D wireTex, DrawInfo drawInfo, bool contextPaletteActive, Vector3 contextPalettePos )
		{
			//Handles.BeginGUI();
			//Debug.Log(GUI.depth);
			// Draw connected node wires
			m_wireBezierCount = 0;
			for( int nodeIdx = 0; nodeIdx < m_nodes.Count; nodeIdx++ )
			{
				ParentNode node = m_nodes[ nodeIdx ];
				if( (object)node == null )
					return;

				for( int inputPortIdx = 0; inputPortIdx < node.InputPorts.Count; inputPortIdx++ )
				{
					InputPort inputPort = node.InputPorts[ inputPortIdx ];
					if(  inputPort.ExternalReferences.Count > 0 && inputPort.Visible )
					{
						bool cleanInvalidConnections = false;
						for( int wireIdx = 0; wireIdx < inputPort.ExternalReferences.Count; wireIdx++ )
						{
							WireReference reference = inputPort.ExternalReferences[ wireIdx ];
							if( reference.NodeId != -1 && reference.PortId != -1 )
							{
								ParentNode outputNode = GetNode( reference.NodeId );
								if( outputNode != null )
								{
									OutputPort outputPort = outputNode.GetOutputPortByUniqueId( reference.PortId );
									Vector3 endPos = new Vector3( inputPort.Position.x, inputPort.Position.y );
									Vector3 startPos = new Vector3( outputPort.Position.x, outputPort.Position.y );
									float x = ( startPos.x < endPos.x ) ? startPos.x : endPos.x;
									float y = ( startPos.y < endPos.y ) ? startPos.y : endPos.y;
									float width = Mathf.Abs( startPos.x - endPos.x ) + outputPort.Position.width;
									float height = Mathf.Abs( startPos.y - endPos.y ) + outputPort.Position.height;
									Rect portsBoundingBox = new Rect( x, y, width, height );

									bool isVisible = node.IsVisible || outputNode.IsVisible;
									if( !isVisible )
									{
										isVisible = drawInfo.TransformedCameraArea.Overlaps( portsBoundingBox );
									}

									if( isVisible )
									{

										Rect bezierBB = DrawBezier( drawInfo.InvertedZoom, startPos, endPos, inputPort.DataType, outputPort.DataType, node.GetInputPortVisualDataTypeByArrayIdx( inputPortIdx ), outputNode.GetOutputPortVisualDataTypeById( reference.PortId ), reference.WireStatus, wireTex, node, outputNode );
										bezierBB.x -= Constants.OUTSIDE_WIRE_MARGIN;
										bezierBB.y -= Constants.OUTSIDE_WIRE_MARGIN;

										bezierBB.width += Constants.OUTSIDE_WIRE_MARGIN * 2;
										bezierBB.height += Constants.OUTSIDE_WIRE_MARGIN * 2;

										if( m_wireBezierCount < m_bezierReferences.Count )
										{
											m_bezierReferences[ m_wireBezierCount ].UpdateInfo( ref bezierBB, inputPort.NodeId, inputPort.PortId, outputPort.NodeId, outputPort.PortId );
										}
										else
										{
											m_bezierReferences.Add( new WireBezierReference( ref bezierBB, inputPort.NodeId, inputPort.PortId, outputPort.NodeId, outputPort.PortId ) );
										}
										m_wireBezierCount++;

									}
								}
								else
								{
									if( DebugConsoleWindow.DeveloperMode )
										UIUtils.ShowMessage( "Detected Invalid connection from node " + node.UniqueId + " port " + inputPortIdx + " to Node " + reference.NodeId + " port " + reference.PortId, MessageSeverity.Error );
									cleanInvalidConnections = true;
									inputPort.ExternalReferences[ wireIdx ].Invalidate();
								}
							}
						}

						if( cleanInvalidConnections )
						{
							inputPort.RemoveInvalidConnections();
						}
					}
				}
			}

			//Draw selected wire
			if( m_parentWindow.WireReferenceUtils.ValidReferences() )
			{
				if( m_parentWindow.WireReferenceUtils.InputPortReference.IsValid )
				{
					InputPort inputPort = GetNode( m_parentWindow.WireReferenceUtils.InputPortReference.NodeId ).GetInputPortByUniqueId( m_parentWindow.WireReferenceUtils.InputPortReference.PortId );
					Vector3 endPos = Vector3.zero;
					if( m_parentWindow.WireReferenceUtils.SnapEnabled )
					{
						Vector2 pos = ( m_parentWindow.WireReferenceUtils.SnapPosition + drawInfo.CameraOffset ) * drawInfo.InvertedZoom;
						endPos = new Vector3( pos.x, pos.y ) + UIUtils.ScaledPortsDelta;
					}
					else
					{
						endPos = contextPaletteActive ? contextPalettePos : new Vector3( Event.current.mousePosition.x, Event.current.mousePosition.y );
					}

					Vector3 startPos = new Vector3( inputPort.Position.x, inputPort.Position.y );
					DrawBezier( drawInfo.InvertedZoom, endPos, startPos, inputPort.DataType, inputPort.DataType, inputPort.DataType, inputPort.DataType, WireStatus.Default, wireTex );
				}

				if( m_parentWindow.WireReferenceUtils.OutputPortReference.IsValid )
				{
					OutputPort outputPort = GetNode( m_parentWindow.WireReferenceUtils.OutputPortReference.NodeId ).GetOutputPortByUniqueId( m_parentWindow.WireReferenceUtils.OutputPortReference.PortId );
					Vector3 endPos = Vector3.zero;
					if( m_parentWindow.WireReferenceUtils.SnapEnabled )
					{
						Vector2 pos = ( m_parentWindow.WireReferenceUtils.SnapPosition + drawInfo.CameraOffset ) * drawInfo.InvertedZoom;
						endPos = new Vector3( pos.x, pos.y ) + UIUtils.ScaledPortsDelta;
					}
					else
					{
						endPos = contextPaletteActive ? contextPalettePos : new Vector3( Event.current.mousePosition.x, Event.current.mousePosition.y );
					}
					Vector3 startPos = new Vector3( outputPort.Position.x, outputPort.Position.y );
					DrawBezier( drawInfo.InvertedZoom, startPos, endPos, outputPort.DataType, outputPort.DataType, outputPort.DataType, outputPort.DataType, WireStatus.Default, wireTex );
				}
			}
			//Handles.EndGUI();
		}

		Rect DrawBezier( float invertedZoom, Vector3 startPos, Vector3 endPos, WirePortDataType inputDataType, WirePortDataType outputDataType, WirePortDataType inputVisualDataType, WirePortDataType outputVisualDataType, WireStatus wireStatus, Texture2D wireTex, ParentNode inputNode = null, ParentNode outputNode = null )
		{
			startPos += UIUtils.ScaledPortsDelta;
			endPos += UIUtils.ScaledPortsDelta;

			// Calculate the 4 points for bezier taking into account wire nodes and their automatic tangents
			float mag = ( endPos - startPos ).magnitude;
			float resizedMag = Mathf.Min( mag * 0.66f, Constants.HORIZONTAL_TANGENT_SIZE * invertedZoom );

			Vector3 startTangent = new Vector3( startPos.x + resizedMag, startPos.y );
			Vector3 endTangent = new Vector3( endPos.x - resizedMag, endPos.y );

			if( (object)inputNode != null && inputNode.GetType() == typeof( WireNode ) )
				endTangent = endPos + ( ( inputNode as WireNode ).TangentDirection ) * mag * 0.33f;

			if( (object)outputNode != null && outputNode.GetType() == typeof( WireNode ) )
				startTangent = startPos - ( ( outputNode as WireNode ).TangentDirection ) * mag * 0.33f;

			///////////////Draw tangents
			//Rect box1 = new Rect( new Vector2( startTangent.x, startTangent.y ), new Vector2( 10, 10 ) );
			//box1.x -= box1.width * 0.5f;
			//box1.y -= box1.height * 0.5f;
			//GUI.Label( box1, string.Empty, UIUtils.Box );

			//Rect box2 = new Rect( new Vector2( endTangent.x, endTangent.y ), new Vector2( 10, 10 ) );
			//box2.x -= box2.width * 0.5f;
			//box2.y -= box2.height * 0.5f;
			//GUI.Label( box2, string.Empty, UIUtils.Box );

			//m_auxRect.Set( 0, 0, UIUtils.CurrentWindow.position.width, UIUtils.CurrentWindow.position.height );
			//GLDraw.BeginGroup( m_auxRect );

			int ty = 1;
			float wireThickness = 0;


			if( ParentWindow.Options.MultiLinePorts )
			{
				GLDraw.MultiLine = true;
				Shader.SetGlobalFloat( "_InvertedZoom", invertedZoom );

				WirePortDataType smallest = ( (int)outputDataType < (int)inputDataType ? outputDataType : inputDataType );
				smallest = ( (int)smallest < (int)outputVisualDataType ? smallest : outputVisualDataType );
				smallest = ( (int)smallest < (int)inputVisualDataType ? smallest : inputVisualDataType );

				switch( smallest )
				{
					case WirePortDataType.FLOAT2: ty = 2; break;
					case WirePortDataType.FLOAT3: ty = 3; break;
					case WirePortDataType.FLOAT4:
					case WirePortDataType.COLOR:
					{
						ty = 4;
					}
					break;
					default: ty = 1; break;
				}
				wireThickness = Mathf.Lerp( Constants.WIRE_WIDTH * ( ty * invertedZoom * -0.05f + 0.15f ), Constants.WIRE_WIDTH * ( ty * invertedZoom * 0.175f + 0.3f ), invertedZoom + 0.4f );
			}
			else
			{
				GLDraw.MultiLine = false;
				wireThickness = Mathf.Lerp( Constants.WIRE_WIDTH * ( invertedZoom * -0.05f + 0.15f ), Constants.WIRE_WIDTH * ( invertedZoom * 0.175f + 0.3f ), invertedZoom + 0.4f );
			}

			Rect boundBox = new Rect();
			int segments = 11;
			if( LodLevel <= ParentGraph.NodeLOD.LOD4 )
				segments = Mathf.Clamp( Mathf.FloorToInt( mag * 0.2f * invertedZoom ), 11, 35 );
			else
				segments = (int)( invertedZoom * 14.28f * 11 );

			if( ParentWindow.Options.ColoredPorts && wireStatus != WireStatus.Highlighted )
				boundBox = GLDraw.DrawBezier( startPos, startTangent, endPos, endTangent, UIUtils.GetColorForDataType( outputVisualDataType, false, false ), UIUtils.GetColorForDataType( inputVisualDataType, false, false ), wireThickness, segments, ty );
			else
				boundBox = GLDraw.DrawBezier( startPos, startTangent, endPos, endTangent, UIUtils.GetColorFromWireStatus( wireStatus ), wireThickness, segments, ty );
			//GLDraw.EndGroup();

			//GUI.Box( m_auxRect, string.Empty, UIUtils.CurrentWindow.CustomStylesInstance.Box );
			//GUI.Box( boundBox, string.Empty, UIUtils.CurrentWindow.CustomStylesInstance.Box );
			//if ( UIUtils.CurrentWindow.Options.ColoredPorts && wireStatus != WireStatus.Highlighted )
			//	Handles.DrawBezier( startPos, endPos, startTangent, endTangent, UIUtils.GetColorForDataType( outputDataType, false, false ), wireTex, wiresTickness );
			//else
			//	Handles.DrawBezier( startPos, endPos, startTangent, endTangent, UIUtils.GetColorFromWireStatus( wireStatus ), wireTex, wiresTickness );

			//Handles.DrawLine( startPos, startTangent );
			//Handles.DrawLine( endPos, endTangent );

			float extraBound = 30 * invertedZoom;
			boundBox.xMin -= extraBound;
			boundBox.xMax += extraBound;
			boundBox.yMin -= extraBound;
			boundBox.yMax += extraBound;

			return boundBox;
		}

		public void DrawBezierBoundingBox()
		{
			for( int i = 0; i < m_wireBezierCount; i++ )
			{
				m_bezierReferences[ i ].DebugDraw();
			}
		}

		public WireBezierReference GetWireBezierInPos( Vector2 position )
		{
			for( int i = 0; i < m_wireBezierCount; i++ )
			{
				if( m_bezierReferences[ i ].Contains( position ) )
					return m_bezierReferences[ i ];
			}
			return null;
		}


		public List<WireBezierReference> GetWireBezierListInPos( Vector2 position )
		{
			List<WireBezierReference> list = new List<WireBezierReference>();
			for( int i = 0; i < m_wireBezierCount; i++ )
			{
				if( m_bezierReferences[ i ].Contains( position ) )
					list.Add( m_bezierReferences[ i ] );
			}

			return list;
		}


		public void MoveSelectedNodes( Vector2 delta, bool snap = false )
		{
			//bool validMovement = delta.magnitude > 0.001f;
			//if ( validMovement )
			//{
			//	Undo.RegisterCompleteObjectUndo( ParentWindow, Constants.UndoMoveNodesId );
			//	for ( int i = 0; i < m_selectedNodes.Count; i++ )
			//	{
			//		if ( !m_selectedNodes[ i ].MovingInFrame )
			//		{
			//			Undo.RecordObject( m_selectedNodes[ i ], Constants.UndoMoveNodesId );
			//			m_selectedNodes[ i ].Move( delta, snap );
			//		}
			//	}
			//	IsDirty = true;
			//}

			bool performUndo = delta.magnitude > 0.01f;
			if( performUndo )
			{
				Undo.RegisterCompleteObjectUndo( ParentWindow, Constants.UndoMoveNodesId );
				Undo.RegisterCompleteObjectUndo( this, Constants.UndoMoveNodesId );
			}

			for( int i = 0; i < m_selectedNodes.Count; i++ )
			{
				if( !m_selectedNodes[ i ].MovingInFrame )
				{
					if( performUndo )
						m_selectedNodes[ i ].RecordObject( Constants.UndoMoveNodesId );
					m_selectedNodes[ i ].Move( delta, snap );
				}
			}

			IsDirty = true;
		}

		public void SetConnection( int InNodeId, int InPortId, int OutNodeId, int OutPortId )
		{
			ParentNode inNode = GetNode( InNodeId );
			ParentNode outNode = GetNode( OutNodeId );
			InputPort inputPort = null;
			OutputPort outputPort = null;
			if( inNode != null && outNode != null )
			{
				inputPort = inNode.GetInputPortByUniqueId( InPortId );
				outputPort = outNode.GetOutputPortByUniqueId( OutPortId );
				if( inputPort != null && outputPort != null )
				{
					if( inputPort.IsConnectedTo( OutNodeId, OutPortId ) || outputPort.IsConnectedTo( InNodeId, InPortId ) )
					{
						if( DebugConsoleWindow.DeveloperMode )
							UIUtils.ShowMessage( "Node/Port already connected " + InNodeId, MessageSeverity.Error );
						return;
					}

					if( !inputPort.CheckValidType( outputPort.DataType ) )
					{
						if( DebugConsoleWindow.DeveloperMode )
							UIUtils.ShowIncompatiblePortMessage( true, inNode, inputPort, outNode, outputPort );
						return;
					}

					if( !outputPort.CheckValidType( inputPort.DataType ) )
					{

						if( DebugConsoleWindow.DeveloperMode )
							UIUtils.ShowIncompatiblePortMessage( false, outNode, outputPort, inNode, inputPort );
						return;
					}
					if( !inputPort.Available || !outputPort.Available )
					{
						if( DebugConsoleWindow.DeveloperMode )
							UIUtils.ShowMessage( "Ports not available to connection", MessageSeverity.Warning );

						return;
					}

					if( inputPort.ConnectTo( OutNodeId, OutPortId, outputPort.DataType, false ) )
					{
						inNode.OnInputPortConnected( InPortId, OutNodeId, OutPortId );
					}


					if( outputPort.ConnectTo( InNodeId, InPortId, inputPort.DataType, inputPort.TypeLocked ) )
					{
						outNode.OnOutputPortConnected( OutPortId, InNodeId, InPortId );
					}
				}
				else if( (object)inputPort == null )
				{
					if( DebugConsoleWindow.DeveloperMode )
						UIUtils.ShowMessage( "Input Port " + InPortId + " doesn't exist on node " + InNodeId, MessageSeverity.Error );
				}
				else
				{
					if( DebugConsoleWindow.DeveloperMode )
						UIUtils.ShowMessage( "Output Port " + OutPortId + " doesn't exist on node " + OutNodeId, MessageSeverity.Error );
				}
			}
			else if( (object)inNode == null )
			{
				if( DebugConsoleWindow.DeveloperMode )
					UIUtils.ShowMessage( "Input node " + InNodeId + " doesn't exist", MessageSeverity.Error );
			}
			else
			{
				if( DebugConsoleWindow.DeveloperMode )
					UIUtils.ShowMessage( "Output node " + OutNodeId + " doesn't exist", MessageSeverity.Error );
			}
		}

		public void CreateConnection( int inNodeId, int inPortId, int outNodeId, int outPortId, bool registerUndo = true )
		{
			ParentNode outputNode = GetNode( outNodeId );
			if( outputNode != null )
			{
				OutputPort outputPort = outputNode.GetOutputPortByUniqueId( outPortId );
				if( outputPort != null )
				{
					ParentNode inputNode = GetNode( inNodeId );
					InputPort inputPort = inputNode.GetInputPortByUniqueId( inPortId );

					if( !inputPort.CheckValidType( outputPort.DataType ) )
					{
						UIUtils.ShowIncompatiblePortMessage( true, inputNode, inputPort, outputNode, outputPort );
						return;
					}

					if( !outputPort.CheckValidType( inputPort.DataType ) )
					{
						UIUtils.ShowIncompatiblePortMessage( false, outputNode, outputPort, inputNode, inputPort );
						return;
					}

					inputPort.DummyAdd( outputPort.NodeId, outputPort.PortId );
					outputPort.DummyAdd( inNodeId, inPortId );

					if( UIUtils.DetectNodeLoopsFrom( inputNode, new Dictionary<int, int>() ) )
					{
						inputPort.DummyRemove();
						outputPort.DummyRemove();
						m_parentWindow.WireReferenceUtils.InvalidateReferences();
						UIUtils.ShowMessage( "Infinite Loop detected" );
						Event.current.Use();
						return;
					}

					inputPort.DummyRemove();
					outputPort.DummyRemove();

					if( inputPort.IsConnected )
					{
						DeleteConnection( true, inNodeId, inPortId, true, false, registerUndo );
					}

					//link output to input
					if( outputPort.ConnectTo( inNodeId, inPortId, inputPort.DataType, inputPort.TypeLocked ) )
						outputNode.OnOutputPortConnected( outputPort.PortId, inNodeId, inPortId );

					//link input to output
					if( inputPort.ConnectTo( outputPort.NodeId, outputPort.PortId, outputPort.DataType, inputPort.TypeLocked ) )
						inputNode.OnInputPortConnected( inPortId, outputNode.UniqueId, outputPort.PortId );

					MarkWireHighlights();
				}
				SaveIsDirty = true;
				//ParentWindow.ShaderIsModified = true;
			}
		}

		public void DeleteInvalidConnections()
		{
			int count = m_nodes.Count;
			for( int nodeIdx = 0; nodeIdx < count; nodeIdx++ )
			{
				{
					int inputCount = m_nodes[ nodeIdx ].InputPorts.Count;
					for( int inputIdx = 0; inputIdx < inputCount; inputIdx++ )
					{
						if( !m_nodes[ nodeIdx ].InputPorts[ inputIdx ].Visible &&
							m_nodes[ nodeIdx ].InputPorts[ inputIdx ].IsConnected &&
							!m_nodes[ nodeIdx ].InputPorts[ inputIdx ].IsDummy )
						{
							DeleteConnection( true, m_nodes[ nodeIdx ].UniqueId, m_nodes[ nodeIdx ].InputPorts[ inputIdx ].PortId, true, true );
						}
					}
				}
				{
					int outputCount = m_nodes[ nodeIdx ].OutputPorts.Count;
					for( int outputIdx = 0; outputIdx < outputCount; outputIdx++ )
					{
						if( !m_nodes[ nodeIdx ].OutputPorts[ outputIdx ].Visible && m_nodes[ nodeIdx ].OutputPorts[ outputIdx ].IsConnected )
						{
							DeleteConnection( false, m_nodes[ nodeIdx ].UniqueId, m_nodes[ nodeIdx ].OutputPorts[ outputIdx ].PortId, true, true );
						}
					}
				}
			}
		}

		public void DeleteAllConnectionFromNode( int nodeId, bool registerOnLog, bool propagateCallback, bool registerUndo )
		{
			ParentNode node = GetNode( nodeId );
			if( (object)node == null )
				return;
			DeleteAllConnectionFromNode( node, registerOnLog, propagateCallback, registerUndo );
		}

		public void DeleteAllConnectionFromNode( ParentNode node, bool registerOnLog, bool propagateCallback, bool registerUndo )
		{

			for( int i = 0; i < node.InputPorts.Count; i++ )
			{
				if( node.InputPorts[ i ].IsConnected )
					DeleteConnection( true, node.UniqueId, node.InputPorts[ i ].PortId, registerOnLog, propagateCallback, registerUndo );
			}

			for( int i = 0; i < node.OutputPorts.Count; i++ )
			{
				if( node.OutputPorts[ i ].IsConnected )
					DeleteConnection( false, node.UniqueId, node.OutputPorts[ i ].PortId, registerOnLog, propagateCallback, registerUndo );
			}
		}

		public void DeleteConnection( bool isInput, int nodeId, int portId, bool registerOnLog, bool propagateCallback, bool registerUndo = true )
		{
			ParentNode node = GetNode( nodeId );
			if( (object)node == null )
				return;

			if( registerUndo )
			{
				UIUtils.MarkUndoAction();
				Undo.RegisterCompleteObjectUndo( ParentWindow, Constants.UndoDeleteConnectionId );
				Undo.RegisterCompleteObjectUndo( this, Constants.UndoDeleteConnectionId );
				node.RecordObject( Constants.UndoDeleteConnectionId );
			}

			if( isInput )
			{
				InputPort inputPort = node.GetInputPortByUniqueId( portId );
				if( inputPort != null && inputPort.IsConnected )
				{

					if( node.ConnStatus == NodeConnectionStatus.Connected )
					{
						node.DeactivateInputPortNode( portId, false );
						//inputPort.GetOutputNode().DeactivateNode( portId, false );
						m_checkSelectedWireHighlights = true;
					}

					for( int i = 0; i < inputPort.ExternalReferences.Count; i++ )
					{
						WireReference inputReference = inputPort.ExternalReferences[ i ];
						ParentNode outputNode = GetNode( inputReference.NodeId );
						if( registerUndo )
							outputNode.RecordObject( Constants.UndoDeleteConnectionId );
						outputNode.GetOutputPortByUniqueId( inputReference.PortId ).InvalidateConnection( inputPort.NodeId, inputPort.PortId );
						if( propagateCallback )
							outputNode.OnOutputPortDisconnected( inputReference.PortId );
					}
					inputPort.InvalidateAllConnections();
					if( propagateCallback )
						node.OnInputPortDisconnected( portId );
				}
			}
			else
			{
				OutputPort outputPort = node.GetOutputPortByUniqueId( portId );
				if( outputPort != null && outputPort.IsConnected )
				{
					if( propagateCallback )
						node.OnOutputPortDisconnected( portId );

					for( int i = 0; i < outputPort.ExternalReferences.Count; i++ )
					{
						WireReference outputReference = outputPort.ExternalReferences[ i ];
						ParentNode inputNode = GetNode( outputReference.NodeId );
						if( registerUndo )
							inputNode.RecordObject( Constants.UndoDeleteConnectionId );
						if( inputNode.ConnStatus == NodeConnectionStatus.Connected )
						{
							node.DeactivateNode( portId, false );
							m_checkSelectedWireHighlights = true;
						}
						inputNode.GetInputPortByUniqueId( outputReference.PortId ).InvalidateConnection( outputPort.NodeId, outputPort.PortId );
						if( propagateCallback )
							inputNode.OnInputPortDisconnected( outputReference.PortId );
					}
					outputPort.InvalidateAllConnections();
				}
			}
			IsDirty = true;
			SaveIsDirty = true;
		}

		//public void DeleteSelectedNodes()
		//{
		//	bool invalidateMasterNode = false;
		//	int count = m_selectedNodes.Count;
		//	for( int nodeIdx = 0; nodeIdx < count; nodeIdx++ )
		//	{
		//		ParentNode node = m_selectedNodes[ nodeIdx ];
		//		if( node.UniqueId == m_masterNodeId )
		//		{
		//			invalidateMasterNode = true;
		//		}
		//		else
		//		{
		//			DestroyNode( node );
		//		}
		//	}

		//	if( invalidateMasterNode )
		//	{
		//		CurrentOutputNode.Selected = false;
		//	}
		//	//Clear all references
		//	m_selectedNodes.Clear();
		//	IsDirty = true;
		//}

		public void DeleteNodesOnArray( ref ParentNode[] nodeArray )
		{
			bool invalidateMasterNode = false;
			for( int nodeIdx = 0; nodeIdx < nodeArray.Length; nodeIdx++ )
			{
				ParentNode node = nodeArray[ nodeIdx ];
				if( node.UniqueId == m_masterNodeId )
				{
					FunctionOutput fout = node as FunctionOutput;
					if( fout != null )
					{
						for( int i = 0; i < m_nodes.Count; i++ )
						{
							FunctionOutput secondfout = m_nodes[ i ] as FunctionOutput;
							if( secondfout != null && secondfout != fout )
							{
								secondfout.Function = fout.Function;
								AssignMasterNode( secondfout, false );

								DeselectNode( fout );
								DestroyNode( fout );
								break;
							}
						}
					}
					invalidateMasterNode = true;
				}
				else
				{
					DeselectNode( node );
					DestroyNode( node );
				}
				nodeArray[ nodeIdx ] = null;
			}

			if( invalidateMasterNode && CurrentMasterNode != null )
			{
				CurrentMasterNode.Selected = false;
			}

			//Clear all references
			nodeArray = null;
			IsDirty = true;
		}

		public void MarkWireNodeSequence( WireNode node, bool isInput )
		{
			if( node == null )
			{
				return;
			}

			if( m_markedForDeletion.Contains( node ) )
				return;

			m_markedForDeletion.Add( node );

			if( isInput && node.InputPorts[ 0 ].IsConnected )
			{
				MarkWireNodeSequence( GetNode( node.InputPorts[ 0 ].ExternalReferences[ 0 ].NodeId ) as WireNode, isInput );
			}
			else if( !isInput && node.OutputPorts[ 0 ].IsConnected )
			{
				MarkWireNodeSequence( GetNode( node.OutputPorts[ 0 ].ExternalReferences[ 0 ].NodeId ) as WireNode, isInput );
			}
		}

		public void UndoableDeleteSelectedNodes( List<ParentNode> nodeList )
		{
			if( nodeList.Count == 0 )
				return;

			List<ParentNode> validNode = new List<ParentNode>();

			for( int i = 0; i < nodeList.Count; i++ )
			{
				if( nodeList[ i ] != null && nodeList[ i ].UniqueId != m_masterNodeId )
				{
					validNode.Add( nodeList[ i ] );
				}
			}
			UIUtils.ClearUndoHelper();
			ParentNode[] selectedNodes = new ParentNode[ validNode.Count ];
			for( int i = 0; i < selectedNodes.Length; i++ )
			{
				if( validNode[ i ] != null )
				{
					selectedNodes[ i ] = validNode[ i ];
					UIUtils.CheckUndoNode( selectedNodes[ i ] );
				}
			}

			//Check nodes connected to deleted nodes to preserve connections on undo
			List<ParentNode> extraNodes = new List<ParentNode>();
			for( int selectedNodeIdx = 0; selectedNodeIdx < selectedNodes.Length; selectedNodeIdx++ )
			{
				// Check inputs
				if( selectedNodes[ selectedNodeIdx ] != null )
				{
					int inputIdxCount = selectedNodes[ selectedNodeIdx ].InputPorts.Count;
					if( inputIdxCount > 0 )
					{
						for( int inputIdx = 0; inputIdx < inputIdxCount; inputIdx++ )
						{
							if( selectedNodes[ selectedNodeIdx ].InputPorts[ inputIdx ].IsConnected )
							{
								int nodeIdx = selectedNodes[ selectedNodeIdx ].InputPorts[ inputIdx ].ExternalReferences[ 0 ].NodeId;
								if( nodeIdx > -1 )
								{
									ParentNode node = GetNode( nodeIdx );
									if( node != null && UIUtils.CheckUndoNode( node ) )
									{
										extraNodes.Add( node );
									}
								}
							}
						}
					}
				}

				// Check outputs
				if( selectedNodes[ selectedNodeIdx ] != null )
				{
					int outputIdxCount = selectedNodes[ selectedNodeIdx ].OutputPorts.Count;
					if( outputIdxCount > 0 )
					{
						for( int outputIdx = 0; outputIdx < outputIdxCount; outputIdx++ )
						{
							int inputIdxCount = selectedNodes[ selectedNodeIdx ].OutputPorts[ outputIdx ].ExternalReferences.Count;
							if( inputIdxCount > 0 )
							{
								for( int inputIdx = 0; inputIdx < inputIdxCount; inputIdx++ )
								{
									int nodeIdx = selectedNodes[ selectedNodeIdx ].OutputPorts[ outputIdx ].ExternalReferences[ inputIdx ].NodeId;
									if( nodeIdx > -1 )
									{
										ParentNode node = GetNode( nodeIdx );
										if( UIUtils.CheckUndoNode( node ) )
										{
											extraNodes.Add( node );
										}
									}
								}
							}
						}
					}

				}
			}

			UIUtils.ClearUndoHelper();
			//Record deleted nodes
			UIUtils.MarkUndoAction();
			Undo.RegisterCompleteObjectUndo( ParentWindow, Constants.UndoDeleteNodeId );
			Undo.RegisterCompleteObjectUndo( this, Constants.UndoDeleteNodeId );
			Undo.RecordObjects( selectedNodes, Constants.UndoDeleteNodeId );
			Undo.RecordObjects( extraNodes.ToArray(), Constants.UndoDeleteNodeId );

			//Record deleting connections
			for( int i = 0; i < selectedNodes.Length; i++ )
			{
				CurrentOutputNode.Selected = false;
				selectedNodes[ i ].Alive = false;
				DeleteAllConnectionFromNode( selectedNodes[ i ], false, true, true );
			}
			//Delete
			DeleteNodesOnArray( ref selectedNodes );

			extraNodes.Clear();
			extraNodes = null;

			EditorUtility.SetDirty( ParentWindow );

			ParentWindow.ForceRepaint();
		}


		public void DeleteMarkedForDeletionNodes()
		{
			UndoableDeleteSelectedNodes( m_markedForDeletion );
			m_markedForDeletion.Clear();
			IsDirty = true;

			//bool invalidateMasterNode = false;
			//int count = m_markedForDeletion.Count;
			//for ( int nodeIdx = 0; nodeIdx < count; nodeIdx++ )
			//{
			//	ParentNode node = m_markedForDeletion[ nodeIdx ];
			//	if ( node.UniqueId == m_masterNodeId )
			//	{
			//		invalidateMasterNode = true;
			//	}
			//	else
			//	{
			//		if ( node.Selected )
			//		{
			//			m_selectedNodes.Remove( node );
			//			node.Selected = false;
			//		}
			//		DestroyNode( node );
			//	}
			//}

			//if ( invalidateMasterNode )
			//{
			//	CurrentMasterNode.Selected = false;
			//}
			////Clear all references
			//m_markedForDeletion.Clear();
			//IsDirty = true;
		}

		public void DestroyNode( int nodeId )
		{
			ParentNode node = GetNode( nodeId );
			DestroyNode( node );
		}

		public void DestroyNode( ParentNode node, bool registerUndo = true, bool destroyMasterNode = false )
		{
			if( node == null )
			{
				UIUtils.ShowMessage( "Attempting to destroying a inexistant node ", MessageSeverity.Warning );
				return;
			}

			if( node.ConnStatus == NodeConnectionStatus.Connected && !m_checkSelectedWireHighlights )
			{
				ResetHighlightedWires();
				m_checkSelectedWireHighlights = true;
			}

			//TODO: check better placement of this code (reconnects wires from wire nodes)
			//if ( node.GetType() == typeof( WireNode ) )
			//{
			//	if ( node.InputPorts[ 0 ].ExternalReferences != null && node.InputPorts[ 0 ].ExternalReferences.Count > 0 )
			//	{
			//		WireReference backPort = node.InputPorts[ 0 ].ExternalReferences[ 0 ];
			//		for ( int i = 0; i < node.OutputPorts[ 0 ].ExternalReferences.Count; i++ )
			//		{
			//			UIUtils.CurrentWindow.ConnectInputToOutput( node.OutputPorts[ 0 ].ExternalReferences[ i ].NodeId, node.OutputPorts[ 0 ].ExternalReferences[ i ].PortId, backPort.NodeId, backPort.PortId );
			//		}
			//	}
			//}
			if( destroyMasterNode || ( node.UniqueId != m_masterNodeId && !m_multiPassMasterNodes.HasNode( node.UniqueId ) ) )
			{
				m_nodeGrid.RemoveNodeFromGrid( node, false );
				//Send Deactivation signal if active
				if( node.ConnStatus == NodeConnectionStatus.Connected )
				{
					node.DeactivateNode( -1, true );
				}

				//Invalidate references
				//Invalidate input references
				for( int inputPortIdx = 0; inputPortIdx < node.InputPorts.Count; inputPortIdx++ )
				{
					InputPort inputPort = node.InputPorts[ inputPortIdx ];
					if( inputPort.IsConnected )
					{
						for( int wireIdx = 0; wireIdx < inputPort.ExternalReferences.Count; wireIdx++ )
						{
							WireReference inputReference = inputPort.ExternalReferences[ wireIdx ];
							ParentNode outputNode = GetNode( inputReference.NodeId );
							outputNode.GetOutputPortByUniqueId( inputReference.PortId ).InvalidateConnection( inputPort.NodeId, inputPort.PortId );
							outputNode.OnOutputPortDisconnected( inputReference.PortId );
						}
						inputPort.InvalidateAllConnections();
					}
				}

				//Invalidate output reference
				for( int outputPortIdx = 0; outputPortIdx < node.OutputPorts.Count; outputPortIdx++ )
				{
					OutputPort outputPort = node.OutputPorts[ outputPortIdx ];
					if( outputPort.IsConnected )
					{
						for( int wireIdx = 0; wireIdx < outputPort.ExternalReferences.Count; wireIdx++ )
						{
							WireReference outputReference = outputPort.ExternalReferences[ wireIdx ];
							ParentNode outnode = GetNode( outputReference.NodeId );
							if( outnode != null )
							{
								outnode.GetInputPortByUniqueId( outputReference.PortId ).InvalidateConnection( outputPort.NodeId, outputPort.PortId );
								outnode.OnInputPortDisconnected( outputReference.PortId );
							}
						}
						outputPort.InvalidateAllConnections();
					}
				}

				//Remove node from main list
				//Undo.RecordObject( node, "Destroying node " + ( node.Attributes != null? node.Attributes.Name: node.GetType().ToString() ) );
				if( registerUndo )
				{
					UIUtils.MarkUndoAction();
					Undo.RegisterCompleteObjectUndo( ParentWindow, Constants.UndoDeleteNodeId );
					Undo.RegisterCompleteObjectUndo( this, Constants.UndoDeleteNodeId );
					node.RecordObjectOnDestroy( Constants.UndoDeleteNodeId );
				}

				if( OnNodeRemovedEvent != null )
					OnNodeRemovedEvent( node );

				m_nodes.Remove( node );
				m_nodesDict.Remove( node.UniqueId );
				node.Destroy();
				if( registerUndo )
					Undo.DestroyObjectImmediate( node );
				else
					DestroyImmediate( node );
				IsDirty = true;
				m_markToReOrder = true;
			}
			//else if( node.UniqueId == m_masterNodeId && node.GetType() == typeof(FunctionOutput) )
			//{
			//	Debug.Log( "Attempting to destroy a output node" );
			//	DeselectNode( node );
			//	UIUtils.ShowMessage( "Attempting to destroy a output node" );
			//}
			else
			{
				DeselectNode( node );
				UIUtils.ShowMessage( "Attempting to destroy a master node" );
			}
		}

		void AddToSelectedNodes( ParentNode node )
		{
			node.Selected = true;
			m_selectedNodes.Add( node );
			node.OnNodeStoppedMovingEvent += OnNodeFinishMoving;
			if( node.ConnStatus == NodeConnectionStatus.Connected )
			{
				HighlightWiresStartingNode( node );
			}
		}

		void RemoveFromSelectedNodes( ParentNode node )
		{
			node.Selected = false;
			m_selectedNodes.Remove( node );
			node.OnNodeStoppedMovingEvent -= OnNodeFinishMoving;
		}

		public void SelectNode( ParentNode node, bool append, bool reorder )
		{
			if( node == null )
				return;

			if( append )
			{
				if( !m_selectedNodes.Contains( node ) )
				{
					AddToSelectedNodes( node );
				}
			}
			else
			{
				DeSelectAll();
				AddToSelectedNodes( node );
			}
			if( reorder && !node.ReorderLocked )
			{
				m_nodes.Remove( node );
				m_nodes.Add( node );
				m_markToReOrder = true;
			}
		}

		public void MultipleSelection( Rect selectionArea, bool appendSelection = true )
		{
			if( !appendSelection )
			{
				for( int i = 0; i < m_nodes.Count; i++ )
				{
					if( selectionArea.Overlaps( m_nodes[ i ].Position, true ) )
					{
						RemoveFromSelectedNodes( m_nodes[ i ] );
					}
				}

				m_markedToDeSelect = false;
				ResetHighlightedWires();
			}
			else
			{
				for( int i = 0; i < m_nodes.Count; i++ )
				{
					if( !m_nodes[ i ].Selected && selectionArea.Overlaps( m_nodes[ i ].Position, true ) )
					{
						AddToSelectedNodes( m_nodes[ i ] );
					}
				}
			}

			// reorder nodes and highlight them
			for( int i = 0; i < m_selectedNodes.Count; i++ )
			{
				if( !m_selectedNodes[ i ].ReorderLocked )
				{
					m_nodes.Remove( m_selectedNodes[ i ] );
					m_nodes.Add( m_selectedNodes[ i ] );
					m_markToReOrder = true;
					if( m_selectedNodes[ i ].ConnStatus == NodeConnectionStatus.Connected )
					{
						HighlightWiresStartingNode( m_selectedNodes[ i ] );
					}
				}
			}
		}

		public void SelectAll()
		{
			for( int i = 0; i < m_nodes.Count; i++ )
			{
				if( !m_nodes[ i ].Selected )
					AddToSelectedNodes( m_nodes[ i ] );
			}
		}

		public void SelectMasterNode()
		{
			if( m_masterNodeId != Constants.INVALID_NODE_ID )
			{
				SelectNode( CurrentMasterNode, false, false );
			}
		}

		public void SelectOutputNode()
		{
			if( m_masterNodeId != Constants.INVALID_NODE_ID )
			{
				SelectNode( CurrentOutputNode, false, false );
			}
		}

		public void DeselectNode( int nodeId )
		{
			ParentNode node = GetNode( nodeId );
			if( node )
			{
				m_selectedNodes.Remove( node );
				node.Selected = false;
			}
		}

		public void DeselectNode( ParentNode node )
		{
			m_selectedNodes.Remove( node );
			node.Selected = false;
			PropagateHighlightDeselection( node );
		}



		public void DeSelectAll()
		{
			m_markedToDeSelect = false;
			for( int i = 0; i < m_selectedNodes.Count; i++ )
			{
				m_selectedNodes[ i ].Selected = false;
				m_selectedNodes[ i ].OnNodeStoppedMovingEvent -= OnNodeFinishMoving;
			}
			m_selectedNodes.Clear();
			ResetHighlightedWires();
		}

		public void AssignMasterNode()
		{
			if( m_selectedNodes.Count == 1 )
			{
				OutputNode newOutputNode = m_selectedNodes[ 0 ] as OutputNode;
				MasterNode newMasterNode = newOutputNode as MasterNode;
				if( newOutputNode != null )
				{
					if( m_masterNodeId != Constants.INVALID_NODE_ID && m_masterNodeId != newOutputNode.UniqueId )
					{
						OutputNode oldOutputNode = GetNode( m_masterNodeId ) as OutputNode;
						MasterNode oldMasterNode = oldOutputNode as MasterNode;
						if( oldOutputNode != null )
						{
							oldOutputNode.IsMainOutputNode = false;
							if( oldMasterNode != null )
							{
								oldMasterNode.ClearUpdateEvents();
							}
						}
					}
					m_masterNodeId = newOutputNode.UniqueId;
					newOutputNode.IsMainOutputNode = true;
					if( newMasterNode != null )
					{
						newMasterNode.OnMaterialUpdatedEvent += OnMaterialUpdatedEvent;
						newMasterNode.OnShaderUpdatedEvent += OnShaderUpdatedEvent;
					}
				}
			}

			IsDirty = true;
		}

		public void AssignMasterNode( OutputNode node, bool onlyUpdateGraphId )
		{
			AssignMasterNode( node.UniqueId, onlyUpdateGraphId );
			MasterNode masterNode = node as MasterNode;
			if( masterNode != null )
			{
				masterNode.OnMaterialUpdatedEvent += OnMaterialUpdatedEvent;
				masterNode.OnShaderUpdatedEvent += OnShaderUpdatedEvent;
			}
		}

		public void AssignMasterNode( int nodeId, bool onlyUpdateGraphId )
		{
			if( nodeId < 0 || m_masterNodeId == nodeId )
				return;

			if( m_masterNodeId > Constants.INVALID_NODE_ID )
			{
				OutputNode oldOutputNode = ( GetNode( nodeId ) as OutputNode );
				MasterNode oldMasterNode = oldOutputNode as MasterNode;
				if( oldOutputNode != null )
				{
					oldOutputNode.IsMainOutputNode = false;
					if( oldMasterNode != null )
					{
						oldMasterNode.ClearUpdateEvents();
					}
				}
			}

			if( onlyUpdateGraphId )
			{
				m_masterNodeId = nodeId;
			}
			else
			{
				OutputNode outputNode = ( GetNode( nodeId ) as OutputNode );
				if( outputNode != null )
				{
					outputNode.IsMainOutputNode = true;
					m_masterNodeId = nodeId;
				}
			}

			IsDirty = true;
		}

		public void RefreshOnUndo()
		{
			if( m_nodes != null )
			{
				int count = m_nodes.Count;
				for( int i = 0; i < count; i++ )
				{
					if( m_nodes[ i ] != null )
					{
						m_nodes[ i ].RefreshOnUndo();
					}
				}
			}
		}

		public void DrawGrid( DrawInfo drawInfo )
		{
			m_nodeGrid.DrawGrid( drawInfo );
		}

		public float MaxNodeDist
		{
			get { return m_nodeGrid.MaxNodeDist; }
		}

		public List<ParentNode> GetNodesInGrid( Vector2 transformedMousePos )
		{
			return m_nodeGrid.GetNodesOn( transformedMousePos );
		}

		public void FireMasterNode( Shader selectedShader )
		{
			( GetNode( m_masterNodeId ) as MasterNode ).Execute( selectedShader );
		}

		public Shader FireMasterNode( string pathname, bool isFullPath )
		{
			return ( GetNode( m_masterNodeId ) as MasterNode ).Execute( pathname, isFullPath );
		}

		public void ForceSignalPropagationOnMasterNode()
		{
			if( m_multiPassMasterNodes.Count > 0 )
			{
				int mpCount = m_multiPassMasterNodes.Count;
				for( int i = 0; i < mpCount; i++ )
				{
					m_multiPassMasterNodes.NodesList[ i ].GenerateSignalPropagation();
				}
			}
			else if( CurrentOutputNode != null )
				CurrentOutputNode.GenerateSignalPropagation();

			List<FunctionOutput> allOutputs = m_functionOutputNodes.NodesList;
			for( int i = 0; i < allOutputs.Count; i++ )
			{
				allOutputs[ i ].GenerateSignalPropagation();
			}

			//List<RegisterLocalVarNode> localVarNodes = m_localVarNodes.NodesList;
			//int count = localVarNodes.Count;
			//for( int i = 0; i < count; i++ )
			//{
			//	localVarNodes[ i ].GenerateSignalPropagation();
			//}
		}

		public void UpdateShaderOnMasterNode( Shader newShader )
		{
			( GetNode( m_masterNodeId ) as MasterNode ).UpdateFromShader( newShader );
		}

		public void CopyValuesFromMaterial( Material material )
		{
			Material currMaterial = CurrentMaterial;
			if( currMaterial == material )
			{
				for( int i = 0; i < m_nodes.Count; i++ )
				{
					m_nodes[ i ].ForceUpdateFromMaterial( material );
				}
			}
		}

		public void UpdateMaterialOnMasterNode( Material material )
		{
			( GetNode( m_masterNodeId ) as MasterNode ).UpdateMasterNodeMaterial( material );
		}

		public void SetMaterialModeOnGraph( Material mat, bool fetchMaterialValues = true )
		{
			for( int i = 0; i < m_nodes.Count; i++ )
			{
				m_nodes[ i ].SetMaterialMode( mat, fetchMaterialValues );
			}
		}

		public ParentNode CheckNodeAt( Vector3 pos, bool checkForRMBIgnore = false )
		{
			ParentNode selectedNode = null;

			// this is checked on the inverse order to give priority to nodes that are drawn on top  ( last on the list )
			for( int i = m_nodes.Count - 1; i > -1; i-- )
			{
				if( m_nodes[ i ].Contains( pos ) )
				{
					if( checkForRMBIgnore )
					{
						if( !m_nodes[ i ].RMBIgnore )
						{
							selectedNode = m_nodes[ i ];
							break;
						}
					}
					else
					{
						selectedNode = m_nodes[ i ];
						break;
					}
				}
			}
			return selectedNode;
		}

		public void ResetNodesLocalVariables()
		{
			for( int i = 0; i < m_nodes.Count; i++ )
			{
				m_nodes[ i ].Reset();
				m_nodes[ i ].ResetOutputLocals();

				FunctionNode fnode = m_nodes[ i ] as FunctionNode;
				if( fnode != null )
				{
					if( fnode.Function != null )
						fnode.FunctionGraph.ResetNodesLocalVariables();
				}
			}
		}

		public void ResetNodesLocalVariablesIfNot( MasterNodePortCategory category )
		{
			for( int i = 0; i < m_nodes.Count; i++ )
			{
				m_nodes[ i ].Reset();
				m_nodes[ i ].ResetOutputLocalsIfNot( category );

				FunctionNode fnode = m_nodes[ i ] as FunctionNode;
				if( fnode != null )
				{
					if( fnode.Function != null )
						fnode.FunctionGraph.ResetNodesLocalVariablesIfNot( category );
				}
			}
		}

		public void ResetNodesLocalVariables( ParentNode node )
		{
			if( node is GetLocalVarNode )
			{
				GetLocalVarNode localVarNode = node as GetLocalVarNode;
				if( localVarNode.CurrentSelected != null )
				{
					node = localVarNode.CurrentSelected;
				}
			}

			node.Reset();
			node.ResetOutputLocals();
			int count = node.InputPorts.Count;
			for( int i = 0; i < count; i++ )
			{
				if( node.InputPorts[ i ].IsConnected )
				{
					ResetNodesLocalVariables( m_nodesDict[ node.InputPorts[ i ].GetConnection().NodeId ] );
				}
			}
		}

		public void ResetNodesLocalVariablesIfNot( ParentNode node, MasterNodePortCategory category )
		{
			if( node is GetLocalVarNode )
			{
				GetLocalVarNode localVarNode = node as GetLocalVarNode;
				if( localVarNode.CurrentSelected != null )
				{
					node = localVarNode.CurrentSelected;
				}
			}

			node.Reset();
			node.ResetOutputLocalsIfNot( category );
			int count = node.InputPorts.Count;
			for( int i = 0; i < count; i++ )
			{
				if( node.InputPorts[ i ].IsConnected )
				{
					ResetNodesLocalVariablesIfNot( m_nodesDict[ node.InputPorts[ i ].GetConnection().NodeId ], category );
				}
			}
		}


		public override string ToString()
		{
			string dump = ( "Parent Graph \n" );
			for( int i = 0; i < m_nodes.Count; i++ )
			{
				dump += ( m_nodes[ i ] + "\n" );
			}
			return dump;
		}

		public void OrderNodesByGraphDepth()
		{
			if( CurrentMasterNode != null )
			{
				//CurrentMasterNode.SetupNodeCategories();
				int count = m_nodes.Count;
				for( int i = 0; i < count; i++ )
				{
					if( m_nodes[ i ].ConnStatus == NodeConnectionStatus.Island )
					{
						m_nodes[ i ].CalculateCustomGraphDepth();
					}
				}
			}
			else
			{
				//TODO: remove this dynamic list
				List<OutputNode> allOutputs = new List<OutputNode>();
				for( int i = 0; i < AllNodes.Count; i++ )
				{
					OutputNode temp = AllNodes[ i ] as OutputNode;
					if( temp != null )
						allOutputs.Add( temp );
				}

				for( int j = 0; j < allOutputs.Count; j++ )
				{
					allOutputs[ j ].SetupNodeCategories();
					int count = m_nodes.Count;
					for( int i = 0; i < count; i++ )
					{
						if( m_nodes[ i ].ConnStatus == NodeConnectionStatus.Island )
						{
							m_nodes[ i ].CalculateCustomGraphDepth();
						}
					}
				}
			}

			m_nodes.Sort( ( x, y ) => { return y.GraphDepth.CompareTo( x.GraphDepth ); } );
		}

		public void WriteToString( ref string nodesInfo, ref string connectionsInfo )
		{
			for( int i = 0; i < m_nodes.Count; i++ )
			{
				m_nodes[ i ].FullWriteToString( ref nodesInfo, ref connectionsInfo );
				IOUtils.AddLineTerminator( ref nodesInfo );
			}
		}

		public void Reset()
		{
			SaveIsDirty = false;
			IsDirty = false;
		}

		public void OnBeforeSerialize()
		{
			//DeSelectAll();
		}

		public void OnAfterDeserialize()
		{
			m_afterDeserializeFlag = true;
		}

		public void CleanCorruptedNodes()
		{
			for( int i = 0; i < m_nodes.Count; i++ )
			{
				if( (object)m_nodes[ i ] == null )
				{
					m_nodes.RemoveAt( i );
					CleanCorruptedNodes();
				}
			}
		}

		public void OnDuplicateEventWrapper()
		{
			if( OnDuplicateEvent != null )
			{
				AmplifyShaderEditorWindow temp = UIUtils.CurrentWindow;
				UIUtils.CurrentWindow = ParentWindow;
				OnDuplicateEvent();
				UIUtils.CurrentWindow = temp;
			}
		}

		public ParentNode CreateNode( AmplifyShaderFunction shaderFunction, bool registerUndo, int nodeId = -1, bool addLast = true )
		{
			FunctionNode newNode = ScriptableObject.CreateInstance<FunctionNode>();
			if( newNode )
			{
				newNode.ContainerGraph = this;
				newNode.CommonInit( shaderFunction, nodeId );
				newNode.UniqueId = nodeId;
				AddNode( newNode, nodeId < 0, addLast, registerUndo );
			}
			return newNode;
		}

		public ParentNode CreateNode( AmplifyShaderFunction shaderFunction, bool registerUndo, Vector2 pos, int nodeId = -1, bool addLast = true )
		{
			ParentNode newNode = CreateNode( shaderFunction, registerUndo, nodeId, addLast );
			if( newNode )
			{
				newNode.Vec2Position = pos;
			}
			return newNode;
		}

		public ParentNode CreateNode( System.Type type, bool registerUndo, int nodeId = -1, bool addLast = true )
		{
			ParentNode newNode = ScriptableObject.CreateInstance( type ) as ParentNode;
			if( newNode )
			{
				newNode.ContainerGraph = this;
				newNode.UniqueId = nodeId;
				AddNode( newNode, nodeId < 0, addLast, registerUndo );
			}
			return newNode;
		}

		public ParentNode CreateNode( System.Type type, bool registerUndo, Vector2 pos, int nodeId = -1, bool addLast = true )
		{
			ParentNode newNode = CreateNode( type, registerUndo, nodeId, addLast );
			if( newNode )
			{
				newNode.Vec2Position = pos;
			}
			return newNode;
		}

		public void FireMasterNodeReplacedEvent()
		{
			MasterNode masterNode = CurrentMasterNode;
			int count = m_nodes.Count;
			for( int i = 0; i < count; i++ )
			{
				if( m_nodes[ i ].UniqueId != m_masterNodeId )
				{
					m_nodes[ i ].OnMasterNodeReplaced( masterNode );
				}
			}
		}

		//Used over shader functions to propagate signal into their graphs
		public void FireMasterNodeReplacedEvent( MasterNode masterNode )
		{
			int count = m_nodes.Count;
			for( int i = 0; i < count; i++ )
			{
				if( m_nodes[ i ].UniqueId != masterNode.UniqueId )
				{
					m_nodes[ i ].OnMasterNodeReplaced( masterNode );
				}
			}
		}


		public void CrossCheckTemplateNodes( TemplateDataParent templateData )
		{
			/*Paulo*/
			DeSelectAll();
			TemplateMultiPassMasterNode newMasterNode = null;
			Dictionary<string, TemplateReplaceHelper> nodesDict = new Dictionary<string, TemplateReplaceHelper>();
			int mpNodeCount = m_multiPassMasterNodes.NodesList.Count;
			for( int i = 0; i < mpNodeCount; i++ )
			{
				nodesDict.Add( m_multiPassMasterNodes.NodesList[ i ].OriginalPassName, new TemplateReplaceHelper( m_multiPassMasterNodes.NodesList[ i ] ) );
			}

			TemplateMultiPassMasterNode currMasterNode = GetNode( m_masterNodeId ) as TemplateMultiPassMasterNode;

			TemplateMultiPass multipassData = templateData as TemplateMultiPass;
			m_currentSRPType = multipassData.SubShaders[ 0 ].Modules.SRPType;

			Vector2 currentPosition = currMasterNode.Vec2Position;
			for( int subShaderIdx = 0; subShaderIdx < multipassData.SubShaders.Count; subShaderIdx++ )
			{
				for( int passIdx = 0; passIdx < multipassData.SubShaders[ subShaderIdx ].Passes.Count; passIdx++ )
				{
					string currPassName = multipassData.SubShaders[ subShaderIdx ].Passes[ passIdx ].PassNameContainer.Data;
					if( nodesDict.ContainsKey( currPassName ) )
					{
						bool wasMainNode = nodesDict[ currPassName ].MasterNode.IsMainOutputNode;

						currentPosition.y += nodesDict[ currPassName ].MasterNode.Position.height + 10;
						nodesDict[ currPassName ].Used = true;
						nodesDict[ currPassName ].MasterNode.SetTemplate( multipassData, false, false, subShaderIdx, passIdx );
						if( wasMainNode && !nodesDict[ currPassName ].MasterNode.IsMainOutputNode )
						{
							nodesDict[ currPassName ].MasterNode.ReleaseResources();
						}
						else if( !wasMainNode && nodesDict[ currPassName ].MasterNode.IsMainOutputNode )
						{
							newMasterNode = nodesDict[ currPassName ].MasterNode;
						}
					}
					else
					{
						TemplateMultiPassMasterNode masterNode = CreateNode( typeof( TemplateMultiPassMasterNode ), false ) as TemplateMultiPassMasterNode;
						if( multipassData.SubShaders[ subShaderIdx ].Passes[ passIdx ].IsMainPass )
						{
							newMasterNode = masterNode;
							currMasterNode.ReleaseResources();
						}
						masterNode.Vec2Position = currentPosition;
						masterNode.SetTemplate( multipassData, true, true, subShaderIdx, passIdx );
						//currentPosition.y += masterNode.HeightEstimate + 10;
					}
				}
			}

			foreach( KeyValuePair<string, TemplateReplaceHelper> kvp in nodesDict )
			{
				if( !kvp.Value.Used )
					DestroyNode( kvp.Value.MasterNode, false, true );
			}
			nodesDict.Clear();

			if( newMasterNode != null )
			{
				m_masterNodeId = newMasterNode.UniqueId;
				newMasterNode.OnMaterialUpdatedEvent += OnMaterialUpdatedEvent;
				newMasterNode.OnShaderUpdatedEvent += OnShaderUpdatedEvent;
				newMasterNode.IsMainOutputNode = true;
			}
		}

		public void RefreshLinkedMasterNodes()
		{
			if( DebugConsoleWindow.DeveloperMode )
				Debug.Log( "Refresh linked master nodes" );

			int mpCount = m_multiPassMasterNodes.Count;
			if( mpCount > 1 )
			{
				Dictionary<string, List<InputPort>> registeredLinks = new Dictionary<string, List<InputPort>>();
				for( int i = 0; i < mpCount; i++ )
				{
					CheckLinkedPorts( ref registeredLinks, m_multiPassMasterNodes.NodesList[ mpCount - 1 - i ] );
				}

				foreach( KeyValuePair<string, List<InputPort>> kvp in registeredLinks )
				{
					int linkCount = kvp.Value.Count;
					if( linkCount == 1 )
					{
						kvp.Value[ 0 ].Visible = true;
					}
					else
					{
						kvp.Value[ 0 ].Visible = true;
						for( int i = 1; i < linkCount; i++ )
						{
							kvp.Value[ i ].SetExternalLink( kvp.Value[ 0 ].NodeId, kvp.Value[ 0 ].PortId );
							kvp.Value[ i ].Visible = false;
						}
					}
					kvp.Value.Clear();
				}
				registeredLinks.Clear();
				registeredLinks = null;
			}

			m_multiPassMasterNodes.NodesList.Sort( ( x, y ) => ( x.SubShaderIdx * 1000 + x.PassIdx ).CompareTo( y.SubShaderIdx * 1000 + y.PassIdx ) );
			m_multiPassMasterNodes.UpdateNodeArr();

			for( int i = 0; i < mpCount; i++ )
			{
				int visiblePorts = 0;
				for( int j = 0; j < m_multiPassMasterNodes.NodesList[ i ].InputPorts.Count; j++ )
				{
					if( m_multiPassMasterNodes.NodesList[ i ].InputPorts[ j ].Visible )
					{
						visiblePorts++;
					}
				}

				if( m_multiPassMasterNodes.NodesList[ i ].VisiblePorts != visiblePorts )
				{
					m_multiPassMasterNodes.NodesList[ i ].VisiblePorts = visiblePorts;
					ForceRepositionCheck = true;
				}

				m_multiPassMasterNodes.NodesList[ i ].Docking = visiblePorts <= 0;
			}

		}


		void CheckLinkedPorts( ref Dictionary<string, List<InputPort>> registeredLinks, TemplateMultiPassMasterNode masterNode )
		{
			if( masterNode.HasLinkPorts )
			{
				int inputCount = masterNode.InputPorts.Count;
				for( int i = 0; i < inputCount; i++ )
				{
					if( !string.IsNullOrEmpty( masterNode.InputPorts[ i ].ExternalLinkId ) )
					{
						string linkId = masterNode.InputPorts[ i ].ExternalLinkId;
						if( !registeredLinks.ContainsKey( masterNode.InputPorts[ i ].ExternalLinkId ) )
						{
							registeredLinks.Add( linkId, new List<InputPort>() );
						}

						if( masterNode.IsMainOutputNode )
						{
							registeredLinks[ linkId ].Insert( 0, masterNode.InputPorts[ i ] );
						}
						else
						{
							registeredLinks[ linkId ].Add( masterNode.InputPorts[ i ] );
						}
					}
					else
					{
						masterNode.InputPorts[ i ].Visible = true;
					}
				}
			}
			else
			{
				int inputCount = masterNode.InputPorts.Count;
				for( int i = 0; i < inputCount; i++ )
				{
					masterNode.InputPorts[ i ].Visible = true;
				}
			}
		}

		public MasterNode ReplaceMasterNode( AvailableShaderTypes newType, bool writeDefaultData = false, TemplateDataParent templateData = null )
		{
			DeSelectAll();
			ResetNodeConnStatus();
			MasterNode newMasterNode = null;
			List<TemplateMultiPassMasterNode> nodesToDelete = null;
			int mpNodeCount = m_multiPassMasterNodes.NodesList.Count;
			if( mpNodeCount > 0 )
			{
				nodesToDelete = new List<TemplateMultiPassMasterNode>();
				for( int i = 0; i < mpNodeCount; i++ )
				{
					if( m_multiPassMasterNodes.NodesList[ i ].UniqueId != m_masterNodeId )
					{
						nodesToDelete.Add( m_multiPassMasterNodes.NodesList[ i ] );
					}
				}
			}
			MasterNode currMasterNode = GetNode( m_masterNodeId ) as MasterNode;
			if( currMasterNode != null )
			{
				currMasterNode.ReleaseResources();
			}

			bool refreshLinkedMasterNodes = false;
			switch( newType )
			{
				default:
				case AvailableShaderTypes.SurfaceShader:
				{
					CurrentCanvasMode = NodeAvailability.SurfaceShader;
					m_currentSRPType = TemplateSRPType.BuiltIn;
					newMasterNode = CreateNode( typeof( StandardSurfaceOutputNode ), false ) as MasterNode;
				}
				break;
				case AvailableShaderTypes.Template:
				{
					CurrentCanvasMode = NodeAvailability.TemplateShader;
					if( templateData.TemplateType == TemplateDataType.LegacySinglePass )
					{
						newMasterNode = CreateNode( typeof( TemplateMasterNode ), false ) as MasterNode;
						( newMasterNode as TemplateMasterNode ).SetTemplate( templateData as TemplateData, writeDefaultData, false );
						m_currentSRPType = TemplateSRPType.BuiltIn;
					}
					else
					{
						/*Paulo*/
						TemplateMultiPass multipassData = templateData as TemplateMultiPass;
						m_currentSRPType = multipassData.SubShaders[ 0 ].Modules.SRPType;

						Vector2 currentPosition = currMasterNode.Vec2Position;

						for( int subShaderIdx = 0; subShaderIdx < multipassData.SubShaders.Count; subShaderIdx++ )
						{
							for( int passIdx = 0; passIdx < multipassData.SubShaders[ subShaderIdx ].Passes.Count; passIdx++ )
							{
								TemplateMultiPassMasterNode masterNode = CreateNode( typeof( TemplateMultiPassMasterNode ), false ) as TemplateMultiPassMasterNode;
								if( multipassData.SubShaders[ subShaderIdx ].Passes[ passIdx ].IsMainPass )
								{
									newMasterNode = masterNode;
									ParentWindow.IsShaderFunctionWindow = false;
									CurrentCanvasMode = NodeAvailability.TemplateShader;
								}
								masterNode.Vec2Position = currentPosition;
								masterNode.SetTemplate( multipassData, true, true, subShaderIdx, passIdx );
								//currentPosition.y += masterNode.HeightEstimate + 10;
							}
						}
						refreshLinkedMasterNodes = true;
						//RefreshLinkedMasterNodes();
					}
				}
				break;
			}

			if( currMasterNode != null )
			{
				newMasterNode.CopyFrom( currMasterNode );
				m_masterNodeId = -1;
				DestroyNode( currMasterNode, false, true );
			}

			if( nodesToDelete != null )
			{
				for( int i = 0; i < nodesToDelete.Count; i++ )
				{
					DestroyNode( nodesToDelete[ i ], false, true );
				}
				nodesToDelete.Clear();
			}

			if( refreshLinkedMasterNodes )
				RefreshLinkedMasterNodes();

			m_masterNodeId = newMasterNode.UniqueId;
			newMasterNode.OnMaterialUpdatedEvent += OnMaterialUpdatedEvent;
			newMasterNode.OnShaderUpdatedEvent += OnShaderUpdatedEvent;
			newMasterNode.IsMainOutputNode = true;
			CurrentMasterNode.OnRefreshLinkedPortsComplete();
			FullCleanUndoStack();
			return newMasterNode;
		}

		private void RepositionTemplateNodes( MasterNode newMasterNode )
		{
			m_forceRepositionCheck = false;

			int dockedElementsBefore = 0;
			int dockedElementsAfter = 0;
			int masterIndex = 0;
			bool foundMaster = false;
			for( int i = 0; i < MultiPassMasterNodes.Count; i++ )
			{
				if( MultiPassMasterNodes.NodesList[ i ].UniqueId == m_masterNodeId )
				{
					foundMaster = true;
					masterIndex = i;
				}

				if( !MultiPassMasterNodes.NodesList[ i ].IsInvisible && MultiPassMasterNodes.NodesList[ i ].Docking )
				{
					if( foundMaster )
						dockedElementsAfter++;
					else
						dockedElementsBefore++;
				}
			}

			if( dockedElementsBefore > 0 )
			{
				newMasterNode.UseSquareNodeTitle = true;
			}

			for( int i = masterIndex - 1; i >= 0; i-- )
			{
				float forwardTracking = 0;
				for( int j = i + 1; j <= masterIndex; j++ )
				{
					if( !MultiPassMasterNodes.NodesList[ i ].IsInvisible && !MultiPassMasterNodes.NodesList[ j ].Docking )
					{
						forwardTracking += MultiPassMasterNodes.NodesList[ j ].HeightEstimate + 10;
					}
				}
				MasterNode node = MultiPassMasterNodes.NodesList[ i ];
				node.Vec2Position = new Vector2( node.Vec2Position.x, newMasterNode.Position.y - forwardTracking - 33 * ( dockedElementsBefore ) );
			}

			for( int i = masterIndex + 1; i < MultiPassMasterNodes.Count; i++ )
			{
				if( MultiPassMasterNodes.NodesList[ i ].UniqueId == newMasterNode.UniqueId || MultiPassMasterNodes.NodesList[ i ].Docking )
					continue;

				float backTracking = 0;
				for( int j = i - 1; j >= masterIndex; j-- )
				{
					if( !MultiPassMasterNodes.NodesList[ i ].IsInvisible && !MultiPassMasterNodes.NodesList[ j ].Docking )
					{
						backTracking += MultiPassMasterNodes.NodesList[ j ].HeightEstimate + 10;
					}
				}
				MasterNode node = MultiPassMasterNodes.NodesList[ i ];
				node.Vec2Position = new Vector2( node.Vec2Position.x, newMasterNode.Position.y + backTracking + 33 * ( dockedElementsAfter ) );
			}
		}

		public void CreateNewEmpty( string name )
		{
			CleanNodes();
			if( m_masterNodeDefaultType == null )
				m_masterNodeDefaultType = typeof( StandardSurfaceOutputNode );

			MasterNode newMasterNode = CreateNode( m_masterNodeDefaultType, false ) as MasterNode;
			newMasterNode.SetName( name );
			m_masterNodeId = newMasterNode.UniqueId;

			ParentWindow.IsShaderFunctionWindow = false;
			CurrentCanvasMode = NodeAvailability.SurfaceShader;

			newMasterNode.OnMaterialUpdatedEvent += OnMaterialUpdatedEvent;
			newMasterNode.OnShaderUpdatedEvent += OnShaderUpdatedEvent;
			newMasterNode.IsMainOutputNode = true;
			LoadedShaderVersion = VersionInfo.FullNumber;
		}

		public void CreateNewEmptyTemplate( string templateGUID )
		{
			CleanNodes();
			TemplateDataParent templateData = m_parentWindow.TemplatesManagerInstance.GetTemplate( templateGUID );
			if( templateData.TemplateType == TemplateDataType.LegacySinglePass )
			{
				TemplateMasterNode newMasterNode = CreateNode( typeof( TemplateMasterNode ), false ) as TemplateMasterNode;
				m_masterNodeId = newMasterNode.UniqueId;

				ParentWindow.IsShaderFunctionWindow = false;
				CurrentCanvasMode = NodeAvailability.TemplateShader;
				m_currentSRPType = TemplateSRPType.BuiltIn;
				newMasterNode.OnMaterialUpdatedEvent += OnMaterialUpdatedEvent;
				newMasterNode.OnShaderUpdatedEvent += OnShaderUpdatedEvent;
				newMasterNode.IsMainOutputNode = true;

				newMasterNode.SetTemplate( templateData as TemplateData, true, true );
			}
			else
			{
				/*Paulo*/
				TemplateMultiPass multipassData = templateData as TemplateMultiPass;
				m_currentSRPType = multipassData.SubShaders[ 0 ].Modules.SRPType;

				Vector2 currentPosition = Vector2.zero;
				for( int subShaderIdx = 0; subShaderIdx < multipassData.SubShaders.Count; subShaderIdx++ )
				{
					for( int passIdx = 0; passIdx < multipassData.SubShaders[ subShaderIdx ].Passes.Count; passIdx++ )
					{
						TemplateMultiPassMasterNode newMasterNode = CreateNode( typeof( TemplateMultiPassMasterNode ), false ) as TemplateMultiPassMasterNode;
						if( multipassData.SubShaders[ subShaderIdx ].Passes[ passIdx ].IsMainPass )
						{
							m_masterNodeId = newMasterNode.UniqueId;

							ParentWindow.IsShaderFunctionWindow = false;
							CurrentCanvasMode = NodeAvailability.TemplateShader;

							newMasterNode.OnMaterialUpdatedEvent += OnMaterialUpdatedEvent;
							newMasterNode.OnShaderUpdatedEvent += OnShaderUpdatedEvent;
							newMasterNode.IsMainOutputNode = true;
						}
						newMasterNode.Vec2Position = currentPosition;
						newMasterNode.SetTemplate( multipassData, true, true, subShaderIdx, passIdx );

						//currentPosition.y += newMasterNode.HeightEstimate + 10;
					}
				}
				RefreshLinkedMasterNodes();
				CurrentMasterNode.OnRefreshLinkedPortsComplete();
			}

			LoadedShaderVersion = VersionInfo.FullNumber;
		}

		public void CreateNewEmptyFunction( AmplifyShaderFunction shaderFunction )
		{
			CleanNodes();
			FunctionOutput newOutputNode = CreateNode( typeof( FunctionOutput ), false ) as FunctionOutput;
			m_masterNodeId = newOutputNode.UniqueId;

			ParentWindow.IsShaderFunctionWindow = true;
			CurrentCanvasMode = NodeAvailability.ShaderFunction;

			newOutputNode.IsMainOutputNode = true;
		}

		public void ForceCategoryRefresh() { m_forceCategoryRefresh = true; }
		public void RefreshExternalReferences()
		{
			int count = m_nodes.Count;
			for( int i = 0; i < count; i++ )
			{
				m_nodes[ i ].RefreshExternalReferences();
			}
		}

		public Vector2 SelectedNodesCentroid
		{
			get
			{
				if( m_selectedNodes.Count == 0 )
					return Vector2.zero;
				Vector2 pos = new Vector2( 0, 0 );
				for( int i = 0; i < m_selectedNodes.Count; i++ )
				{
					pos += m_selectedNodes[ i ].Vec2Position;
				}

				pos /= m_selectedNodes.Count;
				return pos;
			}
		}

		public void AddVirtualTextureCount()
		{
			m_virtualTextureCount += 1;
		}

		public void RemoveVirtualTextureCount()
		{
			m_virtualTextureCount -= 1;
			if( m_virtualTextureCount < 0 )
			{
				Debug.LogWarning( "Invalid virtual texture count" );
			}
		}

		public bool HasVirtualTexture { get { return m_virtualTextureCount > 0; } }

		public void AddInstancePropertyCount()
		{
			m_instancePropertyCount += 1;
//			Debug.Log( "AddInstancePropertyCount "+this.GetInstanceID() + " " + m_instancePropertyCount );
		}

		public void RemoveInstancePropertyCount()
		{
			m_instancePropertyCount -= 1;
	//		Debug.Log( "RemoveInstancePropertyCount " + this.GetInstanceID() + " " + m_instancePropertyCount );

			if( m_instancePropertyCount < 0 )
			{
				Debug.LogWarning( "Invalid property instance count" );
			}
		}

		public int InstancePropertyCount { get { return m_instancePropertyCount; } set { m_instancePropertyCount = value; } }

		public bool IsInstancedShader { get { return m_instancePropertyCount > 0; } }

		public void AddNormalDependentCount() { m_normalDependentCount += 1; }

		public void RemoveNormalDependentCount()
		{
			m_normalDependentCount -= 1;
			if( m_normalDependentCount < 0 )
			{
				Debug.LogWarning( "Invalid normal dependentCount count" );
			}
		}

		public void SetModeFromMasterNode()
		{
			MasterNode masterNode = CurrentMasterNode;
			if( masterNode != null )
			{
				switch( masterNode.CurrentMasterNodeCategory )
				{
					default:
					case AvailableShaderTypes.SurfaceShader:
					{
						if( masterNode is StandardSurfaceOutputNode )
							CurrentCanvasMode = ParentWindow.CurrentNodeAvailability;
						else
							CurrentCanvasMode = NodeAvailability.SurfaceShader;
					}
					break;
					case AvailableShaderTypes.Template:
					{
						CurrentCanvasMode = NodeAvailability.TemplateShader;
					}
					break;
				}
			}
			else
			{

				CurrentCanvasMode = NodeAvailability.SurfaceShader;
			}
		}

		public bool IsMasterNode( ParentNode node )
		{
			return ( node.UniqueId == m_masterNodeId ) ||
					m_multiPassMasterNodes.HasNode( node.UniqueId );
		}

		public bool IsNormalDependent { get { return m_normalDependentCount > 0; } }

		public void MarkToDeselect() { m_markedToDeSelect = true; }
		public void MarkToSelect( int nodeId ) { m_markToSelect = nodeId; }
		public void MarkWireHighlights() { m_checkSelectedWireHighlights = true; }
		public List<ParentNode> SelectedNodes { get { return m_selectedNodes; } }
		public List<ParentNode> MarkedForDeletionNodes { get { return m_markedForDeletion; } }
		public int CurrentMasterNodeId { get { return m_masterNodeId; } }

		public Shader CurrentShader
		{
			get
			{
				MasterNode masterNode = GetNode( m_masterNodeId ) as MasterNode;
				if( masterNode != null )
					return masterNode.CurrentShader;
				return null;
			}
		}

		public Material CurrentMaterial
		{
			get
			{
				MasterNode masterNode = GetNode( m_masterNodeId ) as MasterNode;
				if( masterNode != null )
					return masterNode.CurrentMaterial;
				return null;
			}
		}
		
		public NodeAvailability CurrentCanvasMode { get { return m_currentCanvasMode; } set { m_currentCanvasMode = value; ParentWindow.LateRefreshAvailableNodes(); } }
		public OutputNode CurrentOutputNode { get { return GetNode( m_masterNodeId ) as OutputNode; } }
		public FunctionOutput CurrentFunctionOutput { get { return GetNode( m_masterNodeId ) as FunctionOutput; } }
		public MasterNode CurrentMasterNode { get { return GetNode( m_masterNodeId ) as MasterNode; } }
		public StandardSurfaceOutputNode CurrentStandardSurface { get { return GetNode( m_masterNodeId ) as StandardSurfaceOutputNode; } }
		public List<ParentNode> AllNodes { get { return m_nodes; } }
		public int NodeCount { get { return m_nodes.Count; } }
		//public List<ParentNode> VisibleNodes { get { return m_visibleNodes; } }

		public int NodeClicked
		{
			set { m_nodeClicked = value; }
			get { return m_nodeClicked; }
		}

		public bool IsDirty
		{
			set { m_isDirty = value && UIUtils.DirtyMask; }
			get
			{
				bool value = m_isDirty;
				m_isDirty = false;
				return value;
			}
		}

		public bool SaveIsDirty
		{
			set { m_saveIsDirty = value && UIUtils.DirtyMask; }
			get { return m_saveIsDirty; }
		}
		public int LoadedShaderVersion
		{
			get { return m_loadedShaderVersion; }
			set { m_loadedShaderVersion = value; }
		}

		public AmplifyShaderFunction CurrentShaderFunction
		{
			get { if( CurrentFunctionOutput != null ) return CurrentFunctionOutput.Function; else return null; }
			set { if( CurrentFunctionOutput != null ) CurrentFunctionOutput.Function = value; }
		}

		public bool HasUnConnectedNodes { get { return m_hasUnConnectedNodes; } }
		public UsageListSamplerNodes SamplerNodes { get { return m_samplerNodes; } }
		public UsageListFloatIntNodes FloatIntNodes { get { return m_floatNodes; } }
		public UsageListTexturePropertyNodes TexturePropertyNodes { get { return m_texturePropertyNodes; } }
		public UsageListTextureArrayNodes TextureArrayNodes { get { return m_textureArrayNodes; } }
		public UsageListPropertyNodes PropertyNodes { get { return m_propertyNodes; } }
		public UsageListPropertyNodes RawPropertyNodes { get { return m_rawPropertyNodes; } }
		public UsageListCustomExpressionsOnFunctionMode CustomExpressionOnFunctionMode { get { return m_customExpressionsOnFunctionMode; } }
		public UsageListScreenColorNodes ScreenColorNodes { get { return m_screenColorNodes; } }
		public UsageListRegisterLocalVarNodes LocalVarNodes { get { return m_localVarNodes; } }
		public UsageListGlobalArrayNodes GlobalArrayNodes { get { return m_globalArrayNodes; } }
		public UsageListFunctionInputNodes FunctionInputNodes { get { return m_functionInputNodes; } }
		public UsageListFunctionNodes FunctionNodes { get { return m_functionNodes; } }
		public UsageListFunctionOutputNodes FunctionOutputNodes { get { return m_functionOutputNodes; } }
		public UsageListFunctionSwitchNodes FunctionSwitchNodes { get { return m_functionSwitchNodes; } }
		public UsageListFunctionSwitchCopyNodes FunctionSwitchCopyNodes { get { return m_functionSwitchCopyNodes; } }
		public UsageListTemplateMultiPassMasterNodes MultiPassMasterNodes { get { return m_multiPassMasterNodes; } }

		public PrecisionType CurrentPrecision
		{
			get { return m_currentPrecision; }
			set { m_currentPrecision = value; }
		}

		public NodeLOD LodLevel
		{
			get { return m_lodLevel; }
		}

		public List<ParentNode> NodePreviewList { get { return m_nodePreviewList; } set { m_nodePreviewList = value; } }

		public void SetGraphId( int id )
		{
			m_graphId = id;
		}

		public int GraphId
		{
			get { return m_graphId; }
		}

		public AmplifyShaderEditorWindow ParentWindow
		{
			get { return m_parentWindow; }
			set { m_parentWindow = value; }
		}


		public bool ChangedLightingModel
		{
			get { return m_changedLightingModel; }
			set { m_changedLightingModel = value; }
		}

		public bool ForceRepositionCheck
		{
			get { return m_forceRepositionCheck; }
			set { m_forceRepositionCheck = value; }
		}

		public bool IsLoading { get { return m_isLoading; } set { m_isLoading = value; } }
		public bool IsDuplicating { get { return m_isDuplicating; } set { m_isDuplicating = value; } }
		public TemplateSRPType CurrentSRPType { get { return m_currentSRPType; }set { m_currentSRPType = value; } }
		public bool IsSRP { get { return m_currentSRPType == TemplateSRPType.Lightweight || m_currentSRPType == TemplateSRPType.HD; } }
		public bool IsHDRP { get { return m_currentSRPType == TemplateSRPType.HD; } }
		public bool IsLWRP { get { return m_currentSRPType == TemplateSRPType.Lightweight; } }
		public bool IsStandardSurface { get { return GetNode( m_masterNodeId ) is StandardSurfaceOutputNode; } }
	}
}
