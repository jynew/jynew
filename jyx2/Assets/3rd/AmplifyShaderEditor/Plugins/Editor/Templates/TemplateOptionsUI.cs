// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	// UI STRUCTURES
	[Serializable]
	public class TemplateOptionUIItem
	{
		public delegate void OnActionPerformed( TemplateOptionUIItem uiItem, params TemplateActionItem[] validActions );
		public event OnActionPerformed OnActionPerformedEvt;

		[SerializeField]
		private bool m_isVisible = true;

		[SerializeField]
		private int m_currentOption = 0;

		[SerializeField]
		private TemplateOptionsItem m_options;

		[SerializeField]
		private bool m_checkOnExecute = false;

		public TemplateOptionUIItem( TemplateOptionsItem options )
		{
			m_options = options;
			m_currentOption = m_options.DefaultOptionIndex;
		}

		public void Draw( UndoParentNode owner )
		{
			if( m_isVisible )
			{
				EditorGUI.BeginChangeCheck();
				switch( m_options.UIWidget )
				{
					case AseOptionsUIWidget.Dropdown:
					{
						m_currentOption = owner.EditorGUILayoutPopup( m_options.Name, m_currentOption, m_options.Options );
					}
					break;
					case AseOptionsUIWidget.Toggle:
					{
						m_currentOption = owner.EditorGUILayoutToggle( m_options.Name, m_currentOption == 1 ) ? 1 : 0;
					}
					break;
				}
				if( EditorGUI.EndChangeCheck() )
				{
					if( OnActionPerformedEvt != null )
					{
						OnActionPerformedEvt( this, m_options.ActionsPerOption[ m_currentOption ] );
					}
				}
			}
		}

		public void FillDataCollector( ref MasterNodeDataCollector dataCollector )
		{
			if( m_isVisible && m_checkOnExecute )
			{
				for( int i = 0; i < m_options.ActionsPerOption[ m_currentOption ].Length; i++ )
				{
					switch( m_options.ActionsPerOption[ m_currentOption ][i].ActionType )
					{
						case AseOptionsActionType.SetDefine:
						{
							dataCollector.AddToDefines( -1, m_options.ActionsPerOption[ m_currentOption ][ i ].ActionData );
						}
						break;
						case AseOptionsActionType.UnsetDefine:
						{
							dataCollector.AddToDefines( -1, m_options.ActionsPerOption[ m_currentOption ][ i ].ActionData, false );
						}
						break;
					}
				}
			}
		}

		public TemplateOptionsItem Options { get { return m_options; } }

		public void Destroy()
		{
			OnActionPerformedEvt = null;
		}

		public bool IsVisible
		{
			get { return m_isVisible; }
			set { m_isVisible = value; }
		}

		public bool CheckOnExecute
		{
			get { return m_checkOnExecute; }
			set { m_checkOnExecute = value; }
		}

		public int CurrentOption
		{
			get { return m_currentOption; }
			set
			{
				m_currentOption = Mathf.Clamp( value, 0, m_options.Options.Length -1 );
				if( OnActionPerformedEvt != null )
				{
					OnActionPerformedEvt( this, m_options.ActionsPerOption[ m_currentOption ] );
				}
			}
		}
		public bool EmptyEvent { get { return OnActionPerformedEvt == null;} }
	}
}
