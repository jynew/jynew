// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public enum WireStatus
	{
		Default = 0,
		Highlighted,
		Selected
	}

	[Serializable]
	public sealed class WireReference
	{
		private WireStatus m_status = WireStatus.Default;



		[SerializeField]
		private int m_nodeId = -1;
		[SerializeField]
		private int m_portId = -1;
		[SerializeField]
		private WirePortDataType m_dataType = WirePortDataType.FLOAT;
		[SerializeField]
		private bool m_typeLocked = false;
		
		
		
		public WireReference()
		{
			m_nodeId = -1;
			m_portId = -1;
			m_dataType = WirePortDataType.FLOAT;
			m_typeLocked = false;
			m_status = WireStatus.Default;
		}

		public WireReference( int nodeId, int portId, WirePortDataType dataType, bool typeLocked )
		{
			m_portId = portId;
			m_nodeId = nodeId;
			m_dataType = dataType;
			m_typeLocked = typeLocked;
			m_status = WireStatus.Default;
		}

		public void Invalidate()
		{
			m_nodeId = -1;
			m_portId = -1;
			m_typeLocked = false;
			m_status = WireStatus.Default;
		}

		public void SetReference( int nodeId, int portId, WirePortDataType dataType, bool typeLocked )
		{
			m_nodeId = nodeId;
			m_portId = portId;
			m_dataType = dataType;
			m_typeLocked = typeLocked;
		}

		public void SetReference( WirePort port )
		{
			m_nodeId = port.NodeId;
			m_portId = port.PortId;
			m_dataType = port.DataType;
		}

		public bool IsValid
		{
			get { return ( m_nodeId != -1 && m_portId != -1 ); }
		}

		public int NodeId
		{
			get { return m_nodeId; }
		}

		public int PortId
		{
			get { return m_portId; }
			set { m_portId = value; }
		}

		public WirePortDataType DataType
		{
			get { return m_dataType; }
			set { m_dataType = value; }
		}

		public bool TypeLocked
		{
			get { return m_typeLocked; }
		}

		public WireStatus WireStatus
		{
			get { return m_status; }
			set { m_status = value; }
		}

		public override string ToString()
		{
			string dump = "";
			dump += "* Wire Reference *\n";
			dump += "NodeId : " + m_nodeId + "\n";
			dump += "PortId : " + m_portId + "\n";
			dump += "DataType " + m_dataType + "\n"; ;
			return dump;
		}

		public void WriteToString( ref string myString )
		{
			IOUtils.AddFieldToString( ref myString, "PortId", m_portId );
			IOUtils.AddFieldToString( ref myString, "NodeID", m_nodeId );
			IOUtils.AddFieldToString( ref myString, "DataType", m_dataType );
			IOUtils.AddFieldToString( ref myString, "TypeLocked", m_typeLocked );
		}
	}
}
