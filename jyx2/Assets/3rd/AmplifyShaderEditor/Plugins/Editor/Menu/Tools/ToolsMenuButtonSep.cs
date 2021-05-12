// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public sealed class ToolsMenuButtonSep : ToolsMenuButtonParent
	{
		private Color m_splitterColor = EditorGUIUtility.isProSkin ? new Color( 0.157f, 0.157f, 0.157f ) : new Color( 0.5f, 0.5f, 0.5f );
		[SerializeField]
		private GUIStyle m_sepStyle;
		public ToolsMenuButtonSep( AmplifyShaderEditorWindow parentWindow = null, string text = null, string tooltip = null, float buttonSpacing = -1 ) : base( parentWindow, text, tooltip, buttonSpacing ) { }

		public override void Draw()
		{
			base.Draw();
			if ( m_sepStyle == null )
			{
				m_sepStyle = new GUIStyle();
				m_sepStyle.normal.background = Texture2D.whiteTexture;
				m_sepStyle.hover.background = Texture2D.whiteTexture;
				m_sepStyle.active.background = Texture2D.whiteTexture;
				m_sepStyle.onNormal.background = Texture2D.whiteTexture;
				m_sepStyle.onHover.background = Texture2D.whiteTexture;
				m_sepStyle.onActive.background = Texture2D.whiteTexture;
				m_sepStyle.stretchHeight = true;
			}
			Color originalColor = GUI.color;
			GUI.color = m_splitterColor;
			GUILayout.Box( string.Empty, m_sepStyle, GUILayout.MaxWidth( 2 ), GUILayout.ExpandHeight( true ) );
			GUI.color = originalColor;
		}

		public override void Destroy()
		{
			m_sepStyle = null;
		}
	}
}
