// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	public enum WirePortDataType
	{
		OBJECT = 1 << 1,
		FLOAT = 1 << 2,
		FLOAT2 = 1 << 3,
		FLOAT3 = 1 << 4,
		FLOAT4 = 1 << 5,
		FLOAT3x3 = 1 << 6,
		FLOAT4x4 = 1 << 7,
		COLOR = 1 << 8,
		INT = 1 << 9,
		SAMPLER1D = 1 << 10,
		SAMPLER2D = 1 << 11,
		SAMPLER3D = 1 << 12,
		SAMPLERCUBE = 1 << 13,
		UINT = 1 << 14
	}

	public enum VariableQualifiers
	{
		In = 0,
		Out,
		InOut
	}

	public struct WirePortDataTypeComparer : IEqualityComparer<WirePortDataType>
	{
		public bool Equals( WirePortDataType x, WirePortDataType y )
		{
			return x == y;
		}

		public int GetHashCode( WirePortDataType obj )
		{
			// you need to do some thinking here,
			return (int)obj;
		}
	}

	[System.Serializable]
	public class WirePort
	{
		private const double PortClickTime = 0.2;

		private double m_lastTimeClicked = -1;

		private Vector2 m_labelSize;
		private Vector2 m_unscaledLabelSize;
		protected bool m_dirtyLabelSize = true;

		private bool m_isEditable = false;
		private bool m_editingName = false;

		protected int m_portRestrictions = 0;

		private bool m_repeatButtonState = false;

		[SerializeField]
		private Rect m_position;

		[SerializeField]
		private Rect m_labelPosition;

		[SerializeField]
		protected int m_nodeId = -1;

		[SerializeField]
		protected int m_portId = -1;

		[SerializeField]
		protected int m_orderId = -1;

		[SerializeField]
		protected WirePortDataType m_dataType = WirePortDataType.FLOAT;

		[SerializeField]
		protected string m_name;

		[SerializeField]
		protected List<WireReference> m_externalReferences;

		[SerializeField]
		protected bool m_locked = false;

		[SerializeField]
		protected bool m_visible = true;

		[SerializeField]
		protected bool m_isDummy = false;

		[SerializeField]
		protected bool m_hasCustomColor = false;

		[SerializeField]
		protected Color m_customColor = Color.white;

		[SerializeField]
		protected Rect m_activePortArea;

		public WirePort( int nodeId, int portId, WirePortDataType dataType, string name, int orderId = -1 )
		{
			m_nodeId = nodeId;
			m_portId = portId;
			m_orderId = orderId;
			m_dataType = dataType;
			m_name = name;
			m_externalReferences = new List<WireReference>();
		}

		public virtual void Destroy()
		{
			m_externalReferences.Clear();
			m_externalReferences = null;
		}

		public void AddPortForbiddenTypes( params WirePortDataType[] forbiddenTypes )
		{
			if( forbiddenTypes != null )
			{
				if( m_portRestrictions == 0 )
				{
					//if no previous restrictions are detected then we set up the bit array so we can set is bit correctly
					m_portRestrictions = int.MaxValue;
				}

				for( int i = 0; i < forbiddenTypes.Length; i++ )
				{
					m_portRestrictions = m_portRestrictions & ( int.MaxValue - (int)forbiddenTypes[ i ] );
				}
			}
		}

		public void AddPortRestrictions( params WirePortDataType[] validTypes )
		{
			if( validTypes != null )
			{
				for( int i = 0; i < validTypes.Length; i++ )
				{
					m_portRestrictions = m_portRestrictions | (int)validTypes[ i ];
				}
			}
		}

		public void CreatePortRestrictions( params WirePortDataType[] validTypes )
		{
			m_portRestrictions = 0;
			if( validTypes != null )
			{
				for( int i = 0; i < validTypes.Length; i++ )
				{
					m_portRestrictions = m_portRestrictions | (int)validTypes[ i ];
				}
			}
		}

		public virtual bool CheckValidType( WirePortDataType dataType )
		{
			if( m_portRestrictions == 0 )
			{
				return true;
			}

			return ( m_portRestrictions & (int)dataType ) != 0;
		}

		public bool ConnectTo( WireReference port )
		{
			if( m_locked )
				return false;

			if( m_externalReferences.Contains( port ) )
				return false;

			m_externalReferences.Add( port );
			return true;
		}

		public bool ConnectTo( int nodeId, int portId )
		{
			if( m_locked )
				return false;


			foreach( WireReference reference in m_externalReferences )
			{
				if( reference.NodeId == nodeId && reference.PortId == portId )
				{
					return false;
				}
			}
			m_externalReferences.Add( new WireReference( nodeId, portId, m_dataType, false ) );
			return true;
		}

		public bool ConnectTo( int nodeId, int portId, WirePortDataType dataType, bool typeLocked )
		{
			if( m_locked )
				return false;

			foreach( WireReference reference in m_externalReferences )
			{
				if( reference.NodeId == nodeId && reference.PortId == portId )
				{
					return false;
				}
			}
			m_externalReferences.Add( new WireReference( nodeId, portId, dataType, typeLocked ) );
			return true;
		}

		public void DummyAdd( int nodeId, int portId )
		{
			m_externalReferences.Insert( 0, new WireReference( nodeId, portId, WirePortDataType.OBJECT, false ) );
			m_isDummy = true;
		}

		public void DummyRemove()
		{
			m_externalReferences.RemoveAt( 0 );
			m_isDummy = false;
		}

		public void DummyClear()
		{
			m_externalReferences.Clear();
			m_isDummy = false;
		}

		public WireReference GetConnection( int connID = 0 )
		{
			if( connID < m_externalReferences.Count )
				return m_externalReferences[ connID ];
			return null;
		}

		public void ChangeProperties( string newName, WirePortDataType newType, bool invalidateConnections )
		{
			Name = newName;
			ChangeType( newType, invalidateConnections );
			//if ( m_dataType != newType )
			//{
			//	DataType = newType;
			//	if ( invalidateConnections )
			//	{
			//		InvalidateAllConnections();
			//	}
			//	else
			//	{
			//		NotifyExternalRefencesOnChange();
			//	}
			//}
		}

		public void ChangeType( WirePortDataType newType, bool invalidateConnections )
		{
			if( m_dataType != newType )
			{
				//ParentNode node = UIUtils.GetNode( m_nodeId );
				//if ( node )
				//{
				//	Undo.RegisterCompleteObjectUndo( node.ContainerGraph.ParentWindow, Constants.UndoChangeTypeNodesId );
				//	Undo.RecordObject( node, Constants.UndoChangeTypeNodesId );
				//}
				DataType = newType;
				if( invalidateConnections )
				{
					InvalidateAllConnections();
				}
				else
				{
					NotifyExternalRefencesOnChange();
				}
			}
		}

        public virtual void ChangePortId( int newId ) { }
		public virtual void NotifyExternalRefencesOnChange() { }

		public void UpdateInfoOnExternalConn( int nodeId, int portId, WirePortDataType type )
		{
			for( int i = 0; i < m_externalReferences.Count; i++ )
			{
				if( m_externalReferences[ i ].NodeId == nodeId && m_externalReferences[ i ].PortId == portId )
				{
					m_externalReferences[ i ].DataType = type;
				}
			}
		}

		public void InvalidateConnection( int nodeId, int portId )
		{
			int id = -1;
			for( int i = 0; i < m_externalReferences.Count; i++ )
			{
				if( m_externalReferences[ i ].NodeId == nodeId && m_externalReferences[ i ].PortId == portId )
				{
					id = i;
					break;
				}
			}

			if( id > -1 )
				m_externalReferences.RemoveAt( id );
		}

		public void RemoveInvalidConnections()
		{
			Debug.Log( "Cleaning invalid connections" );
			List<WireReference> validConnections = new List<WireReference>();
			for( int i = 0; i < m_externalReferences.Count; i++ )
			{
				if( m_externalReferences[ i ].IsValid )
				{
					validConnections.Add( m_externalReferences[ i ] );
				}
				else
				{
					Debug.Log( "Detected invalid connection on node " + m_nodeId + " port " + m_portId );
				}
			}
			m_externalReferences.Clear();
			m_externalReferences = validConnections;
		}

		public void InvalidateAllConnections()
		{
			m_externalReferences.Clear();
		}

		public virtual void FullDeleteConnections() { }

		public bool IsConnectedTo( int nodeId, int portId )
		{
			if( m_locked )
				return false;

			for( int i = 0; i < m_externalReferences.Count; i++ )
			{
				if( m_externalReferences[ i ].NodeId == nodeId && m_externalReferences[ i ].PortId == portId )
					return true;
			}
			return false;
		}

		public WirePortDataType ConnectionType( int id = 0 )
		{
			return ( id < m_externalReferences.Count ) ? m_externalReferences[ id ].DataType : DataType;
		}

		public bool CheckMatchConnectionType( int id = 0 )
		{
			if( id < m_externalReferences.Count )
				return m_externalReferences[ id ].DataType == DataType;

			return false;
		}

		public void MatchPortToConnection( int id = 0 )
		{
			if( id < m_externalReferences.Count )
			{
				DataType = m_externalReferences[ id ].DataType;
			}
		}

		public void ResetWireReferenceStatus()
		{
			for( int i = 0; i < m_externalReferences.Count; i++ )
			{
				m_externalReferences[ i ].WireStatus = WireStatus.Default;
			}
		}

		public bool InsideActiveArea( Vector2 pos )
		{
			return m_activePortArea.Contains( pos );
		}

		public void Click()
		{
			if( m_isEditable )
			{
				if( ( EditorApplication.timeSinceStartup - m_lastTimeClicked ) < PortClickTime )
				{
					m_editingName = true;
					GUI.FocusControl( "port" + m_nodeId.ToString() + m_portId.ToString() );
					TextEditor te = (TextEditor)GUIUtility.GetStateObject( typeof( TextEditor ), GUIUtility.keyboardControl );
					if( te != null )
					{
						te.SelectAll();
					}
				}

				m_lastTimeClicked = EditorApplication.timeSinceStartup;
			}
		}

		public bool Draw( Rect textPos, GUIStyle style )
		{
			bool changeFlag = false;
			if( m_isEditable && m_editingName )
			{
				textPos.width = m_labelSize.x;
				EditorGUI.BeginChangeCheck();
				GUI.SetNextControlName( "port" + m_nodeId.ToString() + m_portId.ToString() );
				m_name = GUI.TextField( textPos, m_name, style );
				if( EditorGUI.EndChangeCheck() )
				{
					m_dirtyLabelSize = true;
					changeFlag = true;
				}

				if( Event.current.isKey && ( Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter ) )
				{
					m_editingName = false;
					GUIUtility.keyboardControl = 0;
				}
			}
			else
			{
				GUI.Label( textPos, m_name, style );
			}
			//GUI.Label( textPos, string.Empty );
			return changeFlag;
		}

		public void ResetEditing()
		{
			m_editingName = false;
		}

		public virtual void ForceClearConnection() { }

		public bool IsConnected
		{
			get { return ( m_externalReferences.Count > 0 && !m_locked ); }
		}

		public List<WireReference> ExternalReferences
		{
			get { return m_externalReferences; }
		}

		public int ConnectionCount
		{
			get { return m_externalReferences.Count; }
		}

		public Rect Position
		{
			get { return m_position; }
			set { m_position = value; }
		}

		public Rect LabelPosition
		{
			get { return m_labelPosition; }
			set { m_labelPosition = value; }
		}

		public int PortId
		{
			get { return m_portId; }
			set { m_portId = value; }
		}

		public int OrderId
		{
			get { return m_orderId; }
			set { m_orderId = value; }
		}


		public int NodeId
		{
			get { return m_nodeId; }
			set { m_nodeId = value; }
		}

		public virtual WirePortDataType DataType
		{
			get { return m_dataType; }
			set { m_dataType = value; }
		}

		public bool Visible
		{
			get { return m_visible; }
			set
			{
				m_visible = value;
				if( !m_visible && IsConnected )
				{
					ForceClearConnection();
				}
			}
		}

		public bool Locked
		{
			get { return m_locked; }
			set
			{
				//if ( m_locked && IsConnected )
				//{
				//	ForceClearConnection();
				//}
				m_locked = value;
			}
		}

		public virtual string Name
		{
			get { return m_name; }
			set { m_name = value; m_dirtyLabelSize = true; }
		}

		public bool DirtyLabelSize
		{
			get { return m_dirtyLabelSize; }
			set { m_dirtyLabelSize = value; }
		}

		public bool HasCustomColor
		{
			get { return m_hasCustomColor; }
		}

		public Color CustomColor
		{
			get { return m_customColor; }
			set
			{
				m_hasCustomColor = true;
				m_customColor = value;
			}
		}

		public Rect ActivePortArea
		{
			get { return m_activePortArea; }
			set { m_activePortArea = value; }
		}

		public Vector2 LabelSize
		{
			get { return m_labelSize; }
			set { m_labelSize = value; }
		}

		public Vector2 UnscaledLabelSize
		{
			get { return m_unscaledLabelSize; }
			set { m_unscaledLabelSize = value; }
		}

		public bool IsEditable
		{
			get { return m_isEditable; }
			set { m_isEditable = value; }
		}

		public bool Available { get { return m_visible && !m_locked; } }
		public override string ToString()
		{
			string dump = "";
			dump += "Order: " + m_orderId + "\n";
			dump += "Name: " + m_name + "\n";
			dump += " Type: " + m_dataType;
			dump += " NodeId : " + m_nodeId;
			dump += " PortId : " + m_portId;
			dump += "\nConnections:\n";
			foreach( WireReference wirePort in m_externalReferences )
			{
				dump += wirePort + "\n";
			}
			return dump;
		}

		public bool RepeatButtonState
		{
			get { return m_repeatButtonState; }
			set { m_repeatButtonState = value; }
		}
		public bool IsDummy { get { return m_isDummy; } }
	}
}
