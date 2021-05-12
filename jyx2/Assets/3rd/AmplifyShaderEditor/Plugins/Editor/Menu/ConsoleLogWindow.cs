// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{

	public class ConsoleLogItem
	{
		public NodeMessageType ItemType;
		public string ItemMessage;
		public ConsoleLogItem( NodeMessageType itemType, string itemMessage )
		{
			ItemType = itemType;
			ItemMessage = itemMessage;
		}
	}

	public sealed class ConsoleLogWindow : MenuParent
	{
		const float ToolbarHeight = 32;
		private List<ConsoleLogItem> m_messages = new List<ConsoleLogItem>();

		public ConsoleLogWindow( AmplifyShaderEditorWindow parentWindow ) : base( parentWindow, 0, 0, 0, 128, string.Empty, MenuAnchor.BOTTOM_LEFT, MenuAutoSize.NONE )
		{
			m_isActive = false;
		}

		public void AddMessage( NodeMessageType itemType, string itemMessage )
		{
			//m_messages.Insert( 0, new ConsoleLogItem( itemType, itemMessage ) );
		}

		public override void Draw( Rect parentPosition, Vector2 mousePosition, int mouseButtonId, bool hasKeyboadFocus )
		{
			base.Draw( parentPosition, mousePosition, mouseButtonId, hasKeyboadFocus );

			Rect toolbarArea = m_transformedArea;

			toolbarArea.height = ToolbarHeight;
			GUILayout.BeginArea( toolbarArea, m_content, m_style );
			EditorGUILayout.BeginHorizontal();
			{
				if( GUILayout.Button( "Clear", GUILayout.MaxWidth( 50 ) ) )
				{
					m_messages.Clear();
				}
			}
			EditorGUILayout.EndHorizontal();
			GUILayout.EndArea();

			m_transformedArea.y += ToolbarHeight;
			m_transformedArea.height -= ToolbarHeight;

			GUILayout.BeginArea( m_transformedArea, m_content, m_style );
			{
				EditorGUILayout.BeginVertical();
				{
					m_currentScrollPos = EditorGUILayout.BeginScrollView( m_currentScrollPos, GUILayout.Width( 0 ), GUILayout.Height( 0 ) );
					{
						int count = m_messages.Count;
						for( int i = 0; i < count; i++ )
						{
							EditorGUILayout.LabelField( i + ": " + m_messages[ i ].ItemMessage );
						}
					}
					EditorGUILayout.EndScrollView();
				}
				EditorGUILayout.EndVertical();
			}
			GUILayout.EndArea();
			
			//if( Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.Alpha1 )
			//{
			//	UIUtils.ShowMessage( "Test Message " + m_messages.Count );
			//}

		}

		public void ClearMessages()
		{
			m_messages.Clear();
		}

		public void Toggle()
		{
			//m_isActive = !m_isActive;
		}

		public override void Destroy()
		{
			base.Destroy();
			m_messages.Clear();
			m_messages = null;
		}
	}
}
