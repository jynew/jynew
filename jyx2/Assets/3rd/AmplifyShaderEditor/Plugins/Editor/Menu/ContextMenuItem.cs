// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public class ContextMenuItem
	{
		private const string PALETTE_NAME_MOD_STR = "   ";
		
		private string m_paletteName;
		private string m_name;
		private string m_category;
		private string m_description;
		private System.Type m_type;
		private GUIContent m_guiContent;
		private string m_nameWithShortcut;
		private AmplifyShaderFunction m_function;
		private NodeAttributes m_nodeAttributes;

		public ContextMenuItem( NodeAttributes nodeAttributes, System.Type type, string name, string category, string description, AmplifyShaderFunction function, KeyCode shortcut )
		{
			m_nodeAttributes = nodeAttributes;
			m_name = name;
			m_nameWithShortcut = shortcut != KeyCode.None ? ( name + " [ " + UIUtils.KeyCodeToString( shortcut ) + " ]" ) : name;
			m_paletteName = PALETTE_NAME_MOD_STR + m_name;
			m_type = type;
			m_category = category;
			m_description = description;
			m_function = function;
			m_guiContent = new GUIContent( m_nameWithShortcut, m_description );
		}

		public int CompareTo( ContextMenuItem item , bool useWeights )
		{
			if ( useWeights && NodeAttributes.SortOrderPriority > -1 && item.NodeAttributes.SortOrderPriority > -1 )
			{
				if ( NodeAttributes.SortOrderPriority > item.NodeAttributes.SortOrderPriority )
				{
					return 1;
				}
				else if ( NodeAttributes.SortOrderPriority == item.NodeAttributes.SortOrderPriority )
				{
					return m_name.CompareTo( item.Name );
				}
				else
				{
					return -1;
				}
			}
			return m_name.CompareTo( item.Name );
		}

		public string PaletteName { get { return m_paletteName; } }
		public string Name { get { return m_name; } }
		public string NameWithShortcut { get { return m_nameWithShortcut; } }
		public string Category { get { return m_category; } }
		public string Description { get { return m_description; } }
		public AmplifyShaderFunction Function { get { return m_function; } }
		public System.Type NodeType { get { return m_type; } }
		public GUIContent ItemUIContent { get { return m_guiContent; } }
		public NodeAttributes NodeAttributes { get { return m_nodeAttributes; } }

		public override string ToString()
		{
			return m_name + ":" + m_category + ":" + m_description;
		}

		public void Destroy()
		{
			m_guiContent = null;
			m_nodeAttributes = null;
		}
	}
}
