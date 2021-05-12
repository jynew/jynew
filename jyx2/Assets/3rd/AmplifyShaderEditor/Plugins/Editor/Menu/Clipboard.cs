// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace AmplifyShaderEditor
{

	public class ClipboardData
	{
		public string Data = string.Empty;
		public string Connections = string.Empty;
		public int OldNodeId = -1;
		public int NewNodeId = -1;
		
		public ClipboardData( string data, string connections, int oldNodeId )
		{
			Data = data;
			Connections = connections;
			OldNodeId = oldNodeId;
		}

		public override string ToString()
		{
			return Data + IOUtils.CLIPBOARD_DATA_SEPARATOR + Connections + IOUtils.CLIPBOARD_DATA_SEPARATOR + OldNodeId + IOUtils.CLIPBOARD_DATA_SEPARATOR + NewNodeId;
		}
	}

	public class Clipboard
	{
		private const string ClipboardId = "AMPLIFY_CLIPBOARD_ID";
		private readonly string[] ClipboardTagId = { "#CLIP_ITEM#" };
		private List<ClipboardData> m_clipboardStrData;
		private Dictionary<int, ClipboardData> m_clipboardAuxData;
		private Dictionary<string, ClipboardData> m_multiPassMasterNodeData;

		public Clipboard()
		{
			m_clipboardStrData = new List<ClipboardData>();
			m_clipboardAuxData = new Dictionary<int, ClipboardData>();
			m_multiPassMasterNodeData = new Dictionary<string, ClipboardData>();
		}
		
		public void AddMultiPassNodesToClipboard( List<TemplateMultiPassMasterNode> masterNodes )
		{
			m_multiPassMasterNodeData.Clear();
			int templatesAmount = masterNodes.Count;
			for( int i = 0; i < templatesAmount; i++ )
			{
				string data = string.Empty;
				string connection = string.Empty;
				masterNodes[ i ].FullWriteToString( ref data, ref connection );
				ClipboardData clipboardData = new ClipboardData( data , connection, masterNodes[i].UniqueId );
				m_multiPassMasterNodeData.Add( masterNodes[ i ].OriginalPassName , clipboardData );
			}
		}

		public void GetMultiPassNodesFromClipboard( List<TemplateMultiPassMasterNode> masterNodes )
		{
			int templatesAmount = masterNodes.Count;
			for( int i = 0; i < templatesAmount; i++ )
			{
				if( m_multiPassMasterNodeData.ContainsKey( masterNodes[ i ].OriginalPassName ) )
				{
					ClipboardData nodeData = m_multiPassMasterNodeData[ masterNodes[ i ].OriginalPassName ];
					string[] nodeParams = nodeData.Data.Split( IOUtils.FIELD_SEPARATOR );
					masterNodes[ i ].FullReadFromString( ref nodeParams );
				}
			}
		}

		public void AddToClipboard( List<ParentNode> selectedNodes , Vector3 initialPosition, ParentGraph graph )
		{
			//m_clipboardStrData.Clear();
			//m_clipboardAuxData.Clear();

			string clipboardData = IOUtils.Vector3ToString( initialPosition ) + ClipboardTagId[ 0 ];
			int masterNodeId = UIUtils.CurrentWindow.CurrentGraph.CurrentMasterNodeId;
			int count = selectedNodes.Count;
			for ( int i = 0; i < count; i++ )
			{
				if ( UIUtils.CurrentWindow.IsShaderFunctionWindow || !graph.IsMasterNode( selectedNodes[ i ] ))
				{
					string nodeData = string.Empty;
					string connections = string.Empty;
					selectedNodes[ i ].ClipboardFullWriteToString( ref nodeData, ref connections );
					clipboardData += nodeData;
					if ( !string.IsNullOrEmpty( connections ) )
					{
						connections = connections.Substring( 0, connections.Length - 1 );
						clipboardData += "\n" + connections;
					}
					if ( i < count - 1 )
						clipboardData += ClipboardTagId[ 0 ];

					//ClipboardData data = new ClipboardData( nodeData, connections, selectedNodes[ i ].UniqueId );
					//m_clipboardStrData.Add( data );
					//m_clipboardAuxData.Add( selectedNodes[ i ].UniqueId, data );
				}
			}

			if ( !string.IsNullOrEmpty( clipboardData ) )
			{
				EditorPrefs.SetString( ClipboardId, clipboardData );
			}
			//for ( int i = 0; i < selectedNodes.Count; i++ )
			//{
			//	if ( selectedNodes[ i ].UniqueId != masterNodeId )
			//	{
			//		WireNode wireNode = selectedNodes[ i ] as WireNode;
			//		if ( wireNode != null )
			//		{
			//			if ( !IsNodeChainValid( selectedNodes[ i ], true ) || !IsNodeChainValid( selectedNodes[ i ], false ) )
			//			{
			//				UnityEngine.Debug.Log( "found invalid wire port" );
			//			}
			//		}
			//	}
			//}
		}

		public Vector3 GetDataFromEditorPrefs()
		{
			Vector3 initialPos = Vector3.zero;
			m_clipboardStrData.Clear();
			m_clipboardAuxData.Clear();
			string clipboardData = EditorPrefs.GetString( ClipboardId, string.Empty );
			if ( !string.IsNullOrEmpty( clipboardData ) )
			{
				string[] clipboardDataArray = clipboardData.Split( ClipboardTagId, StringSplitOptions.None );
				initialPos = IOUtils.StringToVector3( clipboardDataArray[0] );
				for ( int i = 1; i < clipboardDataArray.Length; i++ )
				{
					if ( !string.IsNullOrEmpty( clipboardDataArray[ i ] ) )
					{
						int wiresIndex = clipboardDataArray[ i ].IndexOf( IOUtils.LINE_TERMINATOR );
						string nodeData = string.Empty;
						string connections = string.Empty;
						if ( wiresIndex < 0 )
						{
							nodeData = clipboardDataArray[ i ];
						}
						else
						{
							nodeData = clipboardDataArray[ i ].Substring( 0, wiresIndex );
							connections = clipboardDataArray[ i ].Substring( wiresIndex + 1 );
						}
						string[] nodeDataArr = nodeData.Split( IOUtils.FIELD_SEPARATOR );
						if ( nodeDataArr.Length > 2 )
						{
							int nodeId = Convert.ToInt32( nodeDataArr[ 2 ] );
							ClipboardData data = new ClipboardData( nodeData, connections, nodeId );
							m_clipboardStrData.Add( data );
							m_clipboardAuxData.Add( nodeId, data );
						}

					}
				}
			}
			return initialPos;
		}

		public bool IsNodeChainValid( ParentNode currentNode, bool forward )
		{
			WireNode wireNode = currentNode as WireNode;
			if ( wireNode == null )
			{
				return m_clipboardAuxData.ContainsKey( currentNode.UniqueId );
			}

			if ( forward )
			{
				if ( wireNode.InputPorts[ 0 ].ExternalReferences.Count > 0 )
				{
					int nodeId = wireNode.InputPorts[ 0 ].ExternalReferences[ 0 ].NodeId;
					if ( m_clipboardAuxData.ContainsKey( nodeId ) )
					{
						return IsNodeChainValid( UIUtils.GetNode( nodeId ), forward );
					}
				}
			}
			else
			{
				int nodeId = wireNode.OutputPorts[ 0 ].ExternalReferences[ 0 ].NodeId;
				if ( m_clipboardAuxData.ContainsKey( nodeId ) )
				{
					return IsNodeChainValid( UIUtils.GetNode( nodeId ), forward );
				}
			}
			return false;
		}

		public void GenerateFullString()
		{
			string data = string.Empty;
			for ( int i = 0; i < m_clipboardStrData.Count; i++ )
			{
				data += m_clipboardStrData[ i ].ToString();
				if ( i < m_clipboardStrData.Count - 1 )
				{
					data += IOUtils.LINE_TERMINATOR;
				}
			}
		}

		public void ClearClipboard()
		{
			m_clipboardStrData.Clear();
			m_clipboardAuxData.Clear();
			m_multiPassMasterNodeData.Clear();
		}

		public ClipboardData GetClipboardData( int oldNodeId )
		{
			if ( m_clipboardAuxData.ContainsKey( oldNodeId ) )
				return m_clipboardAuxData[ oldNodeId ];
			return null;
		}

		public int GeNewNodeId( int oldNodeId )
		{
			if ( m_clipboardAuxData.ContainsKey( oldNodeId ) )
				return m_clipboardAuxData[ oldNodeId ].NewNodeId;
			return -1;
		}

		public List<ClipboardData> CurrentClipboardStrData
		{
			get { return m_clipboardStrData; }
		}
	}
}
