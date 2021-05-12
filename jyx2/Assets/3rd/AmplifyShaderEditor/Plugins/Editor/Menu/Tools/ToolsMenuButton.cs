// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	public sealed class ToolsMenuButton : ToolsMenuButtonParent
	{
		public delegate void ToolButtonPressed( ToolButtonType type );
		public event ToolButtonPressed ToolButtonPressedEvt;

		private Rect m_buttonArea;
		private List<Texture2D> m_buttonTexture;
		private string m_buttonTexturePath;
		private ToolButtonType m_buttonType;
		private GUIStyle m_style;
		private bool m_enabled = true;
		private bool m_drawOnFunction = true;

		private List<string> m_cachedStates;
		private int m_bufferedState = -1;
		private string m_bufferedTooltip = string.Empty;

		public ToolsMenuButton( AmplifyShaderEditorWindow parentWindow, ToolButtonType type, float x, float y, float width, float height, string texturePath, string text, string tooltip, float buttonSpacing = -1, bool drawOnFunction = true ) : base( parentWindow, text, tooltip, buttonSpacing )
		{
			m_buttonArea = new Rect( x, y, width, height );
			m_buttonType = type;

			m_buttonTexturePath = texturePath;
			m_cachedStates = new List<string>();
			m_drawOnFunction = drawOnFunction;
		}
		
		public void AddState( string state )
		{
			m_cachedStates.Add( state );
		}

		public override void Destroy()
		{
			ToolButtonPressedEvt = null;
			if ( m_buttonTexture != null )
			{
				for ( int i = 0; i < m_buttonTexture.Count; i++ )
				{
					Resources.UnloadAsset( m_buttonTexture[ i ] );
				}
				m_buttonTexture.Clear();
			}
			m_buttonTexture = null;
		}
		protected override void Init()
		{
			base.Init();
			if ( m_buttonTexture == null )
			{
				m_buttonTexturePath = AssetDatabase.GUIDToAssetPath( m_buttonTexturePath );
				m_buttonTexture = new List<Texture2D>();
				m_buttonTexture.Add( AssetDatabase.LoadAssetAtPath( m_buttonTexturePath, typeof( Texture2D ) ) as Texture2D );
			}

			if ( m_cachedStates.Count > 0 )
			{
				for ( int i = 0; i < m_cachedStates.Count; i++ )
				{
					m_cachedStates[ i ] = AssetDatabase.GUIDToAssetPath( m_cachedStates[ i ] );
					m_buttonTexture.Add( AssetDatabase.LoadAssetAtPath( m_cachedStates[ i ], typeof( Texture2D ) ) as Texture2D );
				}
				m_cachedStates.Clear();
			}

			if ( m_style == null )
			{
				m_style = new GUIStyle( UIUtils.Button );
				m_style.normal.background = m_buttonTexture[ 0 ];

				m_style.hover.background = m_buttonTexture[ 0 ];
				m_style.hover.textColor = m_style.normal.textColor;

				m_style.active.background = m_buttonTexture[ 0 ];
				m_style.active.textColor = m_style.normal.textColor;

				m_style.onNormal.background = m_buttonTexture[ 0 ];
				m_style.onNormal.textColor = m_style.normal.textColor;

				m_style.onHover.background = m_buttonTexture[ 0 ];
				m_style.onHover.textColor = m_style.normal.textColor;

				m_style.onActive.background = m_buttonTexture[ 0 ];
				m_style.onActive.textColor = m_style.normal.textColor;

				m_style.clipping = TextClipping.Overflow;
				m_style.fontStyle = FontStyle.Bold;
				m_style.alignment = TextAnchor.LowerCenter;
				m_style.contentOffset = new Vector2( 0, 15 );
				m_style.fontSize = 10;
				bool resizeFromTexture = false;
				if ( m_buttonArea.width > 0 )
				{
					m_style.fixedWidth = m_buttonArea.width;
				}
				else
				{
					resizeFromTexture = true;
				}

				if ( m_buttonArea.height > 0 )
				{
					m_style.fixedHeight = m_buttonArea.height;
				}
				else
				{
					resizeFromTexture = true;
				}

				if ( resizeFromTexture )
				{
					m_buttonArea.width = m_style.fixedWidth = m_buttonTexture[ 0 ].width;
					m_buttonArea.height = m_style.fixedHeight = m_buttonTexture[ 0 ].height;
				}
			}

		}
		public override void Draw()
		{
			base.Draw();
			bool guiEnabledBuffer = GUI.enabled;
			GUI.enabled = m_enabled;

			if ( GUILayout.Button( m_content, m_style ) && ToolButtonPressedEvt != null )
			{
				ToolButtonPressedEvt( m_buttonType );
			}
			GUI.enabled = guiEnabledBuffer;
		}

		public override void Draw( float x, float y )
		{
			if ( !(m_parentWindow.CameraDrawInfo.CurrentEventType == EventType.MouseDown || m_parentWindow.CameraDrawInfo.CurrentEventType == EventType.Repaint ) )
				return;

			if ( m_parentWindow.CurrentGraph.CurrentMasterNode == null && !m_drawOnFunction)
				return;


			base.Draw( x, y );

			if ( m_bufferedState > -1 )
			{
				if ( string.IsNullOrEmpty( m_bufferedTooltip ) )
				{
					SetStateOnButton( m_bufferedState );
				}
				else
				{
					SetStateOnButton( m_bufferedState, m_bufferedTooltip );
				}

				m_bufferedState = -1;
				m_bufferedTooltip = string.Empty;
			}


			m_buttonArea.x = x;
			m_buttonArea.y = y;
			
			if ( m_parentWindow.CameraDrawInfo.CurrentEventType == EventType.MouseDown && m_buttonArea.Contains( m_parentWindow.CameraDrawInfo.MousePosition ) && ToolButtonPressedEvt != null )
			{
				ToolButtonPressedEvt( m_buttonType );
				Event.current.Use();
				m_parentWindow.CameraDrawInfo.CurrentEventType = EventType.Used;
			}
			else if ( m_parentWindow.CameraDrawInfo.CurrentEventType == EventType.Repaint )
			{
				GUI.Label( m_buttonArea, m_content, m_style );
			}

			//if ( GUI.Button( m_buttonArea, m_content, m_style ) && ToolButtonPressedEvt != null )
			//{
			//	ToolButtonPressedEvt( m_buttonType );
			//}
		}

		public override void Draw( Vector2 pos )
		{
			Draw( pos.x, pos.y );
		}

		public override void SetStateOnButton( int state, string tooltip )
		{

			if ( m_buttonTexture == null || m_style == null )
			{
				m_bufferedState = state;
				m_bufferedTooltip = tooltip;
				return;
			}


			if ( state < 0 || state >= m_buttonTexture.Count )
			{
				return;
			}
			
			base.SetStateOnButton( state, tooltip );
			m_style.normal.background = m_buttonTexture[ state ];
			m_style.hover.background = m_buttonTexture[ state ];
			m_style.active.background = m_buttonTexture[ state ];
			m_style.onNormal.background = m_buttonTexture[ state ];
			m_style.onHover.background = m_buttonTexture[ state ];
			m_style.onActive.background = m_buttonTexture[ state ];
		}

		public override void SetStateOnButton( int state )
		{
			if ( m_buttonTexture == null || m_style == null )
			{
				m_bufferedState = state;
				return;
			}

			if ( state < 0 || state >= m_buttonTexture.Count )
			{
				return;
			}
			base.SetStateOnButton( state );
			m_style.normal.background = m_buttonTexture[ state ];
			m_style.hover.background = m_buttonTexture[ state ];
			m_style.active.background = m_buttonTexture[ state ];
			m_style.onNormal.background = m_buttonTexture[ state ];
			m_style.onHover.background = m_buttonTexture[ state ];
			m_style.onActive.background = m_buttonTexture[ state ];
		}

		public bool IsInside( Vector2 pos )
		{
			return m_buttonArea.Contains( pos );
		}

		public bool Enabled
		{
			get { return m_enabled; }
			set { m_enabled = value; }
		}
	}
}
