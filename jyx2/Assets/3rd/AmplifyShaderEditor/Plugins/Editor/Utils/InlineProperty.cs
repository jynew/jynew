using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[System.Serializable]
	public class InlineProperty
	{
		[SerializeField]
		private float m_value = 0;

		[SerializeField]
		private bool m_active = false;

		[SerializeField]
		private int m_nodeId = -1;

		public InlineProperty() { }

		public InlineProperty( float val )
		{
			m_value = val;
		}

		public InlineProperty( int val )
		{
			m_value = val;
		}

		public void ResetProperty()
		{
			m_nodeId = -1;
			m_active = false;
		}
		
		public void CopyFrom( InlineProperty other )
		{
			m_value = other.m_value;
			m_active = other.m_active;
			m_nodeId = other.m_nodeId;
		}

		public void SetInlineByName( string propertyName )
		{
			m_nodeId = UIUtils.GetNodeIdByName( propertyName );
			m_active = m_nodeId != -1;
		}

		public void IntSlider( ref UndoParentNode owner, GUIContent content, int min, int max )
		{
			if( !m_active )
			{
				EditorGUILayout.BeginHorizontal();
				m_value = owner.EditorGUILayoutIntSlider( content, (int)m_value, min, max );
				if( GUILayout.Button( UIUtils.FloatIntIconON, UIUtils.FloatIntPickerONOFF, GUILayout.Width( 15 ), GUILayout.Height( 15 ) ) )
					m_active = !m_active;
				EditorGUILayout.EndHorizontal();
			}
			else
			{
				DrawPicker( ref owner, content );
			}
		}

		public void EnumTypePopup( ref UndoParentNode owner, string content, string[] displayOptions )
		{
			if( !m_active )
			{
				EditorGUILayout.BeginHorizontal();
				m_value = owner.EditorGUILayoutPopup( content, (int)m_value, displayOptions );
				if( GUILayout.Button( UIUtils.FloatIntIconON, UIUtils.FloatIntPickerONOFF, GUILayout.Width( 15 ), GUILayout.Height( 15 ) ) )
					m_active = !m_active;
				EditorGUILayout.EndHorizontal();

			}
			else
			{
				DrawPicker( ref owner, content );
			}
		}

		public void FloatField( ref UndoParentNode owner, string content )
		{
			if( !m_active )
			{
				EditorGUILayout.BeginHorizontal();
				m_value = owner.EditorGUILayoutFloatField( content, m_value );
				if( GUILayout.Button( UIUtils.FloatIntIconON, UIUtils.FloatIntPickerONOFF, GUILayout.Width( 15 ), GUILayout.Height( 15 ) ) )
					m_active = !m_active;
				EditorGUILayout.EndHorizontal();
			}
			else
			{
				DrawPicker( ref owner, content );
			}
		}


		public void CustomDrawer( ref UndoParentNode owner, DrawPropertySection Drawer, string content )
		{
			if( !m_active )
			{
				EditorGUILayout.BeginHorizontal();
				Drawer( owner );
				if( GUILayout.Button( UIUtils.FloatIntIconON, UIUtils.FloatIntPickerONOFF, GUILayout.Width( 15 ), GUILayout.Height( 15 ) ) )
					m_active = !m_active;
				EditorGUILayout.EndHorizontal();
			}
			else
			{
				DrawPicker( ref owner, content );
			}
		}

		public delegate void DrawPropertySection( UndoParentNode owner );

		private void DrawPicker( ref UndoParentNode owner, GUIContent content )
		{
			DrawPicker( ref owner, content.text );
		}

		private void DrawPicker( ref UndoParentNode owner, string content )
		{
			EditorGUILayout.BeginHorizontal();
			m_nodeId = owner.EditorGUILayoutIntPopup( content, m_nodeId, UIUtils.FloatIntNodeArr(), UIUtils.FloatIntNodeIds() );
			if( GUILayout.Button( UIUtils.FloatIntIconOFF, UIUtils.FloatIntPickerONOFF, GUILayout.Width( 15 ), GUILayout.Height( 15 ) ) )
				m_active = !m_active;
			EditorGUILayout.EndHorizontal();
		}

		public string GetValueOrProperty( bool parentesis = true )
		{
			if( m_active )
			{
				PropertyNode node = GetPropertyNode();
				if( node != null )
				{
					return parentesis?"[" + node.PropertyName + "]": node.PropertyName;
				}
				else
				{
					m_active = false;
					m_nodeId = -1;
					return m_value.ToString();
				}
			}
			else
			{
				return m_value.ToString();
			}
		}

		public string GetValueOrProperty( string defaultValue, bool parentesis = true )
		{
			if( m_active )
			{
				PropertyNode node = GetPropertyNode();
				if( node != null )
				{
					return parentesis?"[" + node.PropertyName + "]": node.PropertyName;
				}
				else if( !string.IsNullOrEmpty( defaultValue ) )
				{
					m_active = false;
					m_nodeId = -1;
					return defaultValue;
				}
				else
				{
					m_active = false;
					m_nodeId = -1;
					return m_value.ToString();
				}
			}
			else
			{
				return defaultValue;
			}
		}

		public void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			m_value = Convert.ToInt32( nodeParams[ index++ ] );
			m_active = Convert.ToBoolean( nodeParams[ index++ ] );
			m_nodeId = Convert.ToInt32( nodeParams[ index++ ] );
		}

		public void WriteToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_value );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_active );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_nodeId );
		}

		public void SetInlineNodeValue()
		{
			if( IsValid )
			{
				RangedFloatNode fnode = UIUtils.GetNode( m_nodeId ) as RangedFloatNode;
				if( fnode != null )
				{
					fnode.Value = m_value;
					fnode.SetMaterialValueFromInline( m_value );
				}
				else
				{
					IntNode inode = UIUtils.GetNode( m_nodeId ) as IntNode;
					inode.Value = (int)m_value;
					inode.SetMaterialValueFromInline( (int)m_value );
				}
			}
		}

		public bool IsValid { get { return m_active && m_nodeId != -1; } }

		public PropertyNode GetPropertyNode()
		{
			if( m_nodeId >= 0 )
				return UIUtils.GetNode( m_nodeId ) as PropertyNode;

			if( m_nodeId < -1 )
				return UIUtils.GetInternalTemplateNode( m_nodeId );

			return null;
		}

		public int IntValue { get { return (int)m_value; } set { m_value = value; } }
		public float FloatValue { get { return m_value; } set { m_value = value; } }
		public bool Active { get { return m_active; } set { m_active = value; } }
		public int NodeId { get { return m_nodeId; } set{ m_nodeId = value; } }
	}
}
