// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;

namespace AmplifyShaderEditor
{
	public enum AutoPanLocation
	{
		TOP = 0,
		BOTTOM,
		LEFT,
		RIGHT
	}

	public class AutoPanData
	{
		private Rect m_area;
		private float m_size;
		private Vector2 m_velocity;

		private GUIStyle m_style;
		private Color m_color = new Color( 1f, 0f, 0f, 0.5f );

		private AutoPanLocation m_location;
		private float m_adjustWidth = 0;
		private float m_adjustInitialX = 0;

		public AutoPanData( AutoPanLocation location, float size, Vector2 vel )
		{
			m_area = new Rect();
			m_size = size;
			m_velocity = vel;
			m_location = location;
		}

		public bool CheckArea( Vector2 mousePosition, Rect window, bool draw )
		{
			float totalSize = m_size + m_adjustWidth;
			switch ( m_location )
			{
				case AutoPanLocation.TOP:
				{
					m_area.x = m_adjustInitialX;
					m_area.y = 0;
					m_area.width = window.width;
					m_area.height = totalSize;
				}
				break;
				case AutoPanLocation.BOTTOM:
				{
					m_area.x = m_adjustInitialX;
					m_area.y = window.height - totalSize;
					m_area.width = window.width;
					m_area.height = totalSize;
				}
				break;
				case AutoPanLocation.LEFT:
				{
					m_area.x = m_adjustInitialX;
					m_area.y = 0;
					m_area.width = totalSize;
					m_area.height = window.height;
				}
				break;
				case AutoPanLocation.RIGHT:
				{
					m_area.x = m_adjustInitialX + window.width - totalSize;
					m_area.y = 0;
					m_area.width = totalSize;
					m_area.height = window.height;
				}
				break;
			}

			if ( draw )
			{
				if ( m_style == null )
				{
					m_style = UIUtils.Box;
				}
				Color bufferedColor = GUI.color;
				GUI.color = m_color;
				GUI.Label( m_area, string.Empty, m_style );
				GUI.color = bufferedColor;
			}
			return m_area.Contains( mousePosition );
		}

		public float AdjustWidth { set { m_adjustWidth = value; } }
		public float AdjustInitialX { set { m_adjustInitialX = value; } }
		public Vector2 Velocity { get { return m_velocity; } }
	}
}
