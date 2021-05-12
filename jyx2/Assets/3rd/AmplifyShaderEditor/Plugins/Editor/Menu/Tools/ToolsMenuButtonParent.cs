// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;

namespace AmplifyShaderEditor
{
	public class ToolsMenuButtonParent
	{
		protected AmplifyShaderEditorWindow m_parentWindow = null;
		private float m_buttonSpacing = 10;
		private int m_currentState = 0;
		private bool m_isInitialized = false;
		protected GUIContent m_content;
		public ToolsMenuButtonParent( AmplifyShaderEditorWindow parentWindow, string text, string tooltip, float buttonSpacing )
		{
			m_parentWindow = parentWindow;
			m_content = new GUIContent( text, tooltip );

			if ( buttonSpacing > 0 )
				m_buttonSpacing = buttonSpacing;
		}

		public virtual void Draw()
		{
			if ( !m_isInitialized )
			{
				Init();
			}

			GUILayout.Space( m_buttonSpacing );
		}

		public virtual void Draw( Vector2 pos )
		{
			Draw( pos.x, pos.y );
		}

		public virtual void Draw( float x ,float y )
		{
			if ( !m_isInitialized )
			{
				Init();
			}
		}
		
		protected virtual void Init()
		{
			m_isInitialized = false;
		}

		public virtual void SetStateOnButton( int state, string tooltip )
		{
			m_currentState = state;
			m_content.tooltip = tooltip;
		}

		public virtual void SetStateOnButton( int state )
		{
			m_currentState = state;
		}

		public virtual void Destroy() { }

		public float ButtonSpacing
		{
			get { return m_buttonSpacing; }
		}

		public int CurrentState
		{
			get { return m_currentState; }
		}
	}
}
