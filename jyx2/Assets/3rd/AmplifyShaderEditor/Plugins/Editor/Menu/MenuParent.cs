// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public enum MenuAnchor
	{
		TOP_LEFT = 0,
		TOP_CENTER,
		TOP_RIGHT,
		MIDDLE_LEFT,
		MIDDLE_CENTER,
		MIDDLE_RIGHT,
		BOTTOM_LEFT,
		BOTTOM_CENTER,
		BOTTOM_RIGHT,
		NONE
	}

	public enum MenuAutoSize
	{
		MATCH_VERTICAL = 0,
		MATCH_HORIZONTAL,
		NONE
	}

	public class MenuParent
	{
		protected AmplifyShaderEditorWindow m_parentWindow = null;

		protected const float MinimizeButtonXSpacing = 5;
		protected const float MinimizeButtonYSpacing = 5.5f;
		protected const float ResizeAreaWidth = 5;

		protected const float MinimizeCollisionAdjust = 5;

		protected GUIStyle m_style;
		protected GUIContent m_content;
		protected Rect m_maximizedArea;
		protected Rect m_transformedArea;
		protected Rect m_resizeArea;
		protected MenuAnchor m_anchor;
		protected MenuAutoSize m_autoSize;
		protected bool m_isActive = true;
		protected bool m_isMaximized = true;

		protected bool m_lockOnMinimize = false;
		protected bool m_preLockState = false;

		protected Rect m_minimizedArea;
		protected Rect m_minimizeButtonPos;
		protected float m_realWidth;
		protected GUIStyle m_empty = new GUIStyle();

		protected float m_resizeDelta;

		protected bool m_isResizing = false;
		protected bool m_resizable = false;
		protected GUIStyle m_resizeAreaStyle;
		protected bool m_isMouseInside = false;
		protected Vector2 m_currentScrollPos;
		public MenuParent( AmplifyShaderEditorWindow parentWindow, float x, float y, float width, float height, string name, MenuAnchor anchor = MenuAnchor.NONE, MenuAutoSize autoSize = MenuAutoSize.NONE )
		{
			m_parentWindow = parentWindow;
			m_anchor = anchor;
			m_autoSize = autoSize;
			m_maximizedArea = new Rect( x, y, width, height );
			m_content = new GUIContent( GUIContent.none );
			m_content.text = name;
			m_transformedArea = new Rect();
			m_resizeArea = new Rect();
			m_resizeArea.width = ResizeAreaWidth;
			m_resizeAreaStyle = GUIStyle.none;
			m_currentScrollPos = Vector2.zero;
		}

		public void SetMinimizedArea( float x, float y, float width, float height )
		{
			m_minimizedArea = new Rect( x, y, width, height );
		}

		protected void InitDraw( Rect parentPosition, Vector2 mousePosition, int mouseButtonId )
		{
			if ( m_style == null )
			{
				m_style = new GUIStyle( UIUtils.TextArea );
				m_style.stretchHeight = true;
				m_style.stretchWidth = true;
				m_style.fontSize = ( int ) Constants.DefaultTitleFontSize;
				m_style.fontStyle = FontStyle.Normal;
				Texture minimizeTex = UIUtils.GetCustomStyle( CustomStyle.MaximizeButton ).normal.background;
				m_minimizeButtonPos = new Rect( 0, 0, minimizeTex.width, minimizeTex.height );
			}

			Rect currentArea = m_isMaximized ? m_maximizedArea : m_minimizedArea;

			if ( m_isMaximized )
			{
				if ( m_resizable )
				{
					if ( m_isResizing )
					{
						if ( m_anchor == MenuAnchor.TOP_LEFT )
							m_resizeDelta = ( ParentWindow.CurrentEvent.mousePosition.x - m_maximizedArea.width );
						else if ( m_anchor == MenuAnchor.TOP_RIGHT )
							m_resizeDelta = ParentWindow.CurrentEvent.mousePosition.x - ( parentPosition.width - m_maximizedArea.width);
					}
				}

				m_realWidth = m_maximizedArea.width;
				if ( m_resizable )
				{
					if ( m_anchor == MenuAnchor.TOP_LEFT )
					{
						currentArea.width += m_resizeDelta;
						m_realWidth += m_resizeDelta;
					}
					else if ( m_anchor == MenuAnchor.TOP_RIGHT )
					{
						currentArea.width -= m_resizeDelta;
						m_realWidth -= m_resizeDelta;
					}
				}
			}
			else
			{
				if ( currentArea.x < 0 )
				{
					m_realWidth = currentArea.width + currentArea.x;
				}
				else if ( ( currentArea.x + currentArea.width ) > parentPosition.width )
				{
					m_realWidth = parentPosition.width - currentArea.x;
				}
				if ( m_realWidth < 0 )
					m_realWidth = 0;
			}

			switch ( m_anchor )
			{
				case MenuAnchor.TOP_LEFT:
				{
					m_transformedArea.x = currentArea.x;
					m_transformedArea.y = currentArea.y;
					if ( m_isMaximized )
					{
						m_minimizeButtonPos.x = m_transformedArea.x + m_transformedArea.width - m_minimizeButtonPos.width - MinimizeButtonXSpacing;
						m_minimizeButtonPos.y = m_transformedArea.y + MinimizeButtonYSpacing;

						m_resizeArea.x = m_transformedArea.x + m_transformedArea.width;
						m_resizeArea.y = m_minimizeButtonPos.y;
						m_resizeArea.height = m_transformedArea.height;
					}
					else
					{
						float width = ( m_transformedArea.width - m_transformedArea.x );
						m_minimizeButtonPos.x = m_transformedArea.x + width * 0.5f - m_minimizeButtonPos.width * 0.5f;
						m_minimizeButtonPos.y = m_transformedArea.height * 0.5f - m_minimizeButtonPos.height * 0.5f;
					}
				}
				break;
				case MenuAnchor.TOP_CENTER:
				{
					m_transformedArea.x = parentPosition.width * 0.5f + currentArea.x;
					m_transformedArea.y = currentArea.y;
				}
				break;
				case MenuAnchor.TOP_RIGHT:
				{
					m_transformedArea.x = parentPosition.width - currentArea.x - currentArea.width;
					m_transformedArea.y = currentArea.y;
					if ( m_isMaximized )
					{
						m_minimizeButtonPos.x = m_transformedArea.x + MinimizeButtonXSpacing;
						m_minimizeButtonPos.y = m_transformedArea.y + MinimizeButtonYSpacing;

						m_resizeArea.x = m_transformedArea.x - ResizeAreaWidth;
						m_resizeArea.y = m_minimizeButtonPos.y;
						m_resizeArea.height = m_transformedArea.height;
					}
					else
					{
						float width = ( parentPosition.width - m_transformedArea.x );
						m_minimizeButtonPos.x = m_transformedArea.x + width * 0.5f - m_minimizeButtonPos.width * 0.5f;
						m_minimizeButtonPos.y = m_transformedArea.height * 0.5f - m_minimizeButtonPos.height * 0.5f;
					}
				}
				break;
				case MenuAnchor.MIDDLE_LEFT:
				{
					m_transformedArea.x = currentArea.x;
					m_transformedArea.y = parentPosition.height * 0.5f + currentArea.y;
				}
				break;
				case MenuAnchor.MIDDLE_CENTER:
				{
					m_transformedArea.x = parentPosition.width * 0.5f + currentArea.x;
					m_transformedArea.y = parentPosition.height * 0.5f + currentArea.y;
				}
				break;
				case MenuAnchor.MIDDLE_RIGHT:
				{
					m_transformedArea.x = parentPosition.width - currentArea.x - currentArea.width;
					m_transformedArea.y = parentPosition.height * 0.5f + currentArea.y;
				}
				break;
				case MenuAnchor.BOTTOM_LEFT:
				{
					m_transformedArea.x = currentArea.x;
					m_transformedArea.y = parentPosition.height - currentArea.y - currentArea.height;
				}
				break;
				case MenuAnchor.BOTTOM_CENTER:
				{
					m_transformedArea.x = parentPosition.width * 0.5f + currentArea.x;
					m_transformedArea.y = parentPosition.height - currentArea.y - currentArea.height;
				}
				break;
				case MenuAnchor.BOTTOM_RIGHT:
				{
					m_transformedArea.x = parentPosition.width - currentArea.x - currentArea.width;
					m_transformedArea.y = parentPosition.height - currentArea.y - currentArea.height;
				}
				break;

				case MenuAnchor.NONE:
				{
					m_transformedArea.x = currentArea.x;
					m_transformedArea.y = currentArea.y;
				}
				break;
			}

			switch ( m_autoSize )
			{
				case MenuAutoSize.MATCH_HORIZONTAL:
				{
					m_transformedArea.width = parentPosition.width - m_transformedArea.x;
					m_transformedArea.height = currentArea.height;
				}
				break;

				case MenuAutoSize.MATCH_VERTICAL:
				{
					m_transformedArea.width = currentArea.width;
					m_transformedArea.height = parentPosition.height - m_transformedArea.y;
				}
				break;
				case MenuAutoSize.NONE:
				{
					m_transformedArea.width = currentArea.width;
					m_transformedArea.height = currentArea.height;
				}
				break;
			}

		}
		public virtual void Draw( Rect parentPosition, Vector2 mousePosition, int mouseButtonId, bool hasKeyboadFocus )
		{
			InitDraw( parentPosition, mousePosition, mouseButtonId );
			if ( ParentWindow.CurrentEvent.type == EventType.MouseDrag && ParentWindow.CurrentEvent.button > 0 /*catches both middle and right mouse button*/ )
			{
				m_isMouseInside = IsInside( mousePosition );
				if ( m_isMouseInside )
				{
					m_currentScrollPos.x += Constants.MenuDragSpeed * ParentWindow.CurrentEvent.delta.x;
					if ( m_currentScrollPos.x < 0 )
						m_currentScrollPos.x = 0;
					m_currentScrollPos.y += Constants.MenuDragSpeed * ParentWindow.CurrentEvent.delta.y;
					if ( m_currentScrollPos.y < 0 )
						m_currentScrollPos.y = 0;

				}
			}
		}

		public void PostDraw()
		{
			if ( !m_isMaximized )
			{
				m_transformedArea.height = 35;
				GUI.Label( m_transformedArea, m_content, m_style );
			}

			Color colorBuffer = GUI.color;
			GUI.color = EditorGUIUtility.isProSkin ? Color.white : Color.black;
			bool guiEnabledBuffer = GUI.enabled;
			GUI.enabled = !m_lockOnMinimize;
			Rect buttonArea = m_minimizeButtonPos;

			buttonArea.x -= MinimizeCollisionAdjust;
			buttonArea.width += 2 * MinimizeCollisionAdjust;

			buttonArea.y -= MinimizeCollisionAdjust;
			buttonArea.height += 2 * MinimizeCollisionAdjust;

			if ( m_parentWindow.CameraDrawInfo.CurrentEventType == EventType.Repaint )
				GUI.Label( m_minimizeButtonPos, string.Empty, UIUtils.GetCustomStyle( m_isMaximized ? CustomStyle.MinimizeButton : CustomStyle.MaximizeButton ) );

			if( m_parentWindow.CameraDrawInfo.CurrentEventType == EventType.MouseDown && buttonArea.Contains( m_parentWindow.CameraDrawInfo.MousePosition ) )
			//if ( GUI.Button( buttonArea, string.Empty, m_empty ) )
			{
				m_isMaximized = !m_isMaximized;
				m_resizeDelta = 0;
			}

			if ( m_resizable && m_isMaximized )
			{
				EditorGUIUtility.AddCursorRect( m_resizeArea, MouseCursor.ResizeHorizontal );
				if ( !m_isResizing && GUI.RepeatButton( m_resizeArea, string.Empty, m_resizeAreaStyle ) )
				{
					m_isResizing = true;
				}
				else
				{
					if ( m_isResizing )
					{
						if ( ParentWindow.CurrentEvent.isMouse && ParentWindow.CurrentEvent.type != EventType.MouseDrag )
						{
							m_isResizing = false;
						}
					}
				}

				if ( m_realWidth < buttonArea.width )
				{
					// Auto-minimize
					m_isMaximized = false;
					m_resizeDelta = 0;
					m_isResizing = false;
				}
				else
				{
					float halfSizeWindow = 0.5f * ParentWindow.position.width;
					if ( m_realWidth > halfSizeWindow )
					{
						m_realWidth = 0.5f * ParentWindow.position.width;
						if ( m_resizeDelta > 0 )
						{
							m_resizeDelta = m_realWidth - m_maximizedArea.width;
						}
						else
						{
							m_resizeDelta = m_maximizedArea.width - m_realWidth;
						}
					}
				}
			}

			GUI.enabled = guiEnabledBuffer;
			GUI.color = colorBuffer;
			
		}

		public void OnLostFocus()
		{
			if ( m_isResizing )
			{
				m_isResizing = false;
			}
		}

		virtual public void Destroy()
		{
			m_empty = null;
			m_resizeAreaStyle = null;
		}

		public float InitialX
		{
			get { return m_maximizedArea.x; }
			set { m_maximizedArea.x = value; }
		}

		public float Width
		{
			get { return m_maximizedArea.width; }
			set { m_maximizedArea.width = value; }
		}

		public float RealWidth
		{
			get { return m_realWidth; }
		}
		public float Height
		{
			get { return m_maximizedArea.height; }
			set { m_maximizedArea.height = value; }
		}

		public Rect Size
		{
			get { return m_maximizedArea; }
		}

		public virtual bool IsInside( Vector2 position )
		{
			if ( !m_isActive )
				return false;

			return m_transformedArea.Contains( position );
		}

		public bool IsMaximized
		{
			get { return m_isMaximized; }
			set { m_isMaximized = value; }
		}

		public Rect TransformedArea
		{
			get { return m_transformedArea; }
		}

		public bool Resizable { set { m_resizable = value; } }
		public bool IsResizing { get { return m_isResizing; } }
		public bool LockOnMinimize
		{
			set
			{
				if ( m_lockOnMinimize == value )
					return;

				m_lockOnMinimize = value;
				if ( value )
				{
					m_preLockState = m_isMaximized;
					m_isMaximized = false;
				}
				else
				{
					m_isMaximized = m_preLockState;
				}
			}
		}
		public bool IsActive
		{
			get { return m_isActive; }
		}
		public AmplifyShaderEditorWindow ParentWindow { get { return m_parentWindow; } set { m_parentWindow = value; } }
	}
}
