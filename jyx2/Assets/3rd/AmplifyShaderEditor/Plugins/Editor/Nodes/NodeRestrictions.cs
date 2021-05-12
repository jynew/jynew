// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
namespace AmplifyShaderEditor
{
	public class NodeRestrictionsData
	{
		private bool m_allPorts;
		private Dictionary<int, bool> m_portRestrictions;
		public NodeRestrictionsData()
		{
			m_portRestrictions = new Dictionary<int, bool>();
		}

		public NodeRestrictionsData( int port )
		{
			m_portRestrictions = new Dictionary<int, bool>();
			m_portRestrictions.Add( port, true );
		}

		public void SetAllPortRestiction( bool value )
		{
			m_allPorts = value;
		}

		public void AddRestriction( int port )
		{
			if ( !m_portRestrictions.ContainsKey( port ) )
				m_portRestrictions.Add( port, true );
			else
				m_portRestrictions[ port ] = true;
		}

		public void RemoveRestriction( int port )
		{
			if ( m_portRestrictions.ContainsKey( port ) )
				m_portRestrictions[ port ] = true;
		}

		public bool IsPortRestricted( int port )
		{
			if ( m_portRestrictions.ContainsKey( port ) )
				return m_portRestrictions[ port ];
			return false;
		}

		public void Destroy()
		{
			m_portRestrictions.Clear();
			m_portRestrictions = null;
		}

		public bool AllPortsRestricted
		{
			get
			{
				return m_allPorts;
			}
		}
	}

	public class NodeRestrictions
	{
		private Dictionary<System.Type, NodeRestrictionsData> m_restrictions;

		public NodeRestrictions()
		{
			m_restrictions = new Dictionary<System.Type, NodeRestrictionsData>();
		}

		public void AddTypeRestriction( System.Type type )
		{
			if ( !m_restrictions.ContainsKey( type ) )
				m_restrictions.Add( type, new NodeRestrictionsData() );

			m_restrictions[ type ].SetAllPortRestiction( true );

		}

		public void AddPortRestriction( System.Type type, int port )
		{
			if ( !m_restrictions.ContainsKey( type ) )
				m_restrictions.Add( type, new NodeRestrictionsData( port ) );
			else
			{
				m_restrictions[ type ].AddRestriction( port );
			}
		}

		public bool GetRestiction( System.Type type, int port )
		{
			if ( m_restrictions.Count == 0 || type == null )
				return false;

			if ( m_restrictions.ContainsKey( type ) )
			{
				if ( m_restrictions[ type ].AllPortsRestricted )
					return true;

				return m_restrictions[ type ].IsPortRestricted( port );
			}

			return false;
		}

		public void Destroy()
		{
			foreach ( KeyValuePair<System.Type, NodeRestrictionsData> pair in m_restrictions )
			{
				pair.Value.Destroy();
			}

			m_restrictions.Clear();
			m_restrictions = null;
		}
	}
}
