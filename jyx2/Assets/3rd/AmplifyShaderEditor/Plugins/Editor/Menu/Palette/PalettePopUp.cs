// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;

namespace AmplifyShaderEditor
{
	public class PalettePopUp
	{
		private const int DeltaX = 5;
		private Rect m_areaSettings;
		private Vector2 m_mouseDeltaPos = new Vector2( 10, -10 );
		private bool m_isActive = false;
		private GUIContent m_content;
		private GUIStyle m_style;
		private GUIStyle m_fontStyle;
		private GUIContent m_labelContent;

		public PalettePopUp()
		{
			m_content = new GUIContent( GUIContent.none );
			m_areaSettings = new Rect( 0, 0, 100, 30 );
			m_labelContent = new GUIContent( "Test Label" );
		}

		public void Activate( string label )
		{
			m_labelContent.text = label;
			m_areaSettings.width = -1;
			m_isActive = true;
		}

		public void Deactivate()
		{
			m_isActive = false;
		}

		public void Draw( Vector2 mousePos )
		{
			if ( m_style == null )
			{
				m_style = UIUtils.TextArea;
			}

			if ( m_fontStyle == null )
			{
				m_fontStyle = new GUIStyle( UIUtils.Label );
				m_fontStyle.fontSize = 15;
			}

			if ( m_areaSettings.width < 0 )
			{
				m_areaSettings.width = m_fontStyle.CalcSize( m_labelContent ).x + 2 * DeltaX;
			}

			m_areaSettings.position = mousePos + m_mouseDeltaPos;
			GUI.Label( m_areaSettings, m_content, m_style );
			m_areaSettings.position += new Vector2( DeltaX,DeltaX);
			GUI.Label( m_areaSettings, m_labelContent, m_fontStyle );
		}

		public void Destroy()
		{
			m_content = null;
			m_style = null;
		}

		public bool IsActive
		{
			get { return m_isActive; }
		}
	}
}
