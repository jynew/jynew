using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class ReordenatorNode : PropertyNode
	{
		[SerializeField]
		private List<PropertyNode> m_propertyList;

		[SerializeField]
		private string m_headerTitle = string.Empty;

		[SerializeField]
		private bool m_isInside;

		public ReordenatorNode() : base()
		{

		}

		public void Init( string entryName, string entryInspectorName, List<PropertyNode> list, bool register = true )
		{
			m_propertyName = entryName;
			m_propertyInspectorName = entryInspectorName;

			m_propertyList = list;

			if( register )
				UIUtils.RegisterPropertyNode( this );
		}

		public override void Destroy()
		{
			base.Destroy();

			m_propertyList.Clear();
			m_propertyList = null;

			UIUtils.UnregisterPropertyNode( this );
		}

		//public List<ParentNode> PropertyList
		//{
		//	get { return m_propertyList; }
		//}

		public int PropertyListCount
		{
			get { if ( m_propertyList != null ) return m_propertyList.Count; else return -1; }
		}

		public string HeaderTitle { get { return m_headerTitle; } set { m_headerTitle = value; } }

		public bool HasTitle { get { return !string.IsNullOrEmpty( m_headerTitle ); } }

		public bool IsInside { get { return m_isInside; } set { m_isInside = value; } }

		public int RecursiveSetOrderOffset( int offset, bool lockit, int order = -1 )
		{
			//Debug.Log( Locked + " " + PropertyName );

			if ( Locked )
				return offset;

			if( order > -1 )
				OrderIndex = order;

			int currentOffset = offset;
			
			if( m_propertyList != null )
				m_propertyList.Sort( ( x, y ) => { return ( x as PropertyNode ).OrderIndex.CompareTo( ( y as PropertyNode ).OrderIndex ); } );

			OrderIndexOffset = currentOffset - RawOrderIndex;
			currentOffset++;

			if ( m_propertyList != null )
				for ( int i = 0; i < m_propertyList.Count; i++ )
				{
					ReordenatorNode rnode = m_propertyList[ i ] as ReordenatorNode;
					if ( rnode != null )
					{
						currentOffset = rnode.RecursiveSetOrderOffset( currentOffset, false );
					}
					else
					{
						PropertyNode pnode = m_propertyList[ i ] as PropertyNode;
						{
							pnode.OrderIndexOffset = currentOffset - pnode.RawOrderIndex;// + ( HasTitle ? 1 : 0 );
						}
						currentOffset++;
					}
				}

			if ( lockit )
				Locked = true;

			return currentOffset;
		}

		public int RecursiveCount()
		{
			int amount = 0;
			if ( HasTitle )
				amount += 1;
			for ( int i = 0; i < m_propertyList.Count; i++ )
			{
				if ( ( m_propertyList[ i ] is ReordenatorNode ) )
					amount += ( m_propertyList[ i ] as ReordenatorNode ).RecursiveCount();
				else
					amount +=1;
			}
			return amount;
		}

		public void RecursiveLog()
		{
			Debug.LogWarning( OrderIndex+" HEADER "+ PropertyName );
			for( int i = 0; i < m_propertyList.Count; i++ )
			{
				if( ( m_propertyList[ i ] is ReordenatorNode ) )
					( m_propertyList[ i ] as ReordenatorNode ).RecursiveLog();
				else
					Debug.Log( ( m_propertyList[ i ] as PropertyNode ).OrderIndex+" "+( m_propertyList[ i ] as PropertyNode).PropertyName );
			}
		}

		public bool Locked = false;

		public void RecursiveClear()
		{
			Locked = false;
			if( m_propertyList != null)
			for ( int i = 0; i < m_propertyList.Count; i++ )
			{
				ReordenatorNode renode = ( m_propertyList[ i ] as ReordenatorNode );
				if ( renode != null )
				{
					renode.RecursiveClear();
				}
			}
		}

		public bool RecursiveConnectedProperties()
		{
			bool connected = false;
			if ( m_propertyList != null )
			{
				for ( int i = 0; i < m_propertyList.Count; i++ )
				{
					ReordenatorNode renode = ( m_propertyList[ i ] as ReordenatorNode );
					if ( renode != null )
					{
						bool temp = renode.RecursiveConnectedProperties();
						if( temp )
							connected = true;
					} else
					{
						if ( ( m_propertyList[ i ] as PropertyNode ).IsConnected )
							connected = true;
					}
				}
			}
			return connected;
		}
	}
}
