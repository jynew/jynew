// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public class ConfirmationWindow
	{
		public delegate ShaderLoadResult OnConfirmationSelected( bool value, Shader shader, Material material );
		public event OnConfirmationSelected OnConfirmationSelectedEvt;

		private const string m_yesStr = "Yes";
		private const string m_noStr = "No";
		private bool m_isActive = false;
		private string m_currentMessage;

		private GUIStyle m_areaStyle;
		private GUIContent m_content;
		private GUIStyle m_buttonStyle;
		private GUIStyle m_labelStyle;


		private Shader m_shader;
		private Material m_material;
		private Rect m_area;
		private bool m_autoDeactivate = true;


		public ConfirmationWindow( float x, float y, float width, float height )
		{
			m_content = new GUIContent( GUIContent.none );
			m_area = new Rect( x, y, width, height );
		}

		public void Destroy()
		{
			m_shader = null;
			OnConfirmationSelectedEvt = null;
		}

		public void ActivateConfirmation( Shader shader, Material material, string message, OnConfirmationSelected evt, bool autoDeactivate = true )
		{
			OnConfirmationSelectedEvt = evt;
			m_currentMessage = message;
			m_shader = shader;
			m_material = material;
			m_autoDeactivate = autoDeactivate;
			m_isActive = true;
		}

		public void OnGUI()
		{
			if ( m_areaStyle == null )
			{
				m_areaStyle = new GUIStyle( UIUtils.TextArea );
				m_areaStyle.stretchHeight = true;
				m_areaStyle.stretchWidth = true;
				m_areaStyle.fontSize = ( int ) Constants.DefaultTitleFontSize;
			}

			if ( m_buttonStyle == null )
			{
				m_buttonStyle = UIUtils.Button;
			}

			if ( m_labelStyle == null )
			{
				m_labelStyle = new GUIStyle( UIUtils.Label );
				m_labelStyle.alignment = TextAnchor.MiddleCenter;
				m_labelStyle.wordWrap = true;
			}

			m_area.x = ( int ) ( 0.5f * UIUtils.CurrentWindow.CameraInfo.width );
			m_area.y = ( int ) ( 0.5f * UIUtils.CurrentWindow.CameraInfo.height );

			GUILayout.BeginArea( m_area, m_content, m_areaStyle );
			{
				EditorGUILayout.BeginVertical();
				{
					EditorGUILayout.Separator();
					EditorGUILayout.LabelField( m_currentMessage, m_labelStyle );

					EditorGUILayout.Separator();
					EditorGUILayout.Separator();
					EditorGUILayout.BeginHorizontal();
					{
						if ( GUILayout.Button( m_yesStr, m_buttonStyle ) )
						{
							if ( OnConfirmationSelectedEvt != null )
								OnConfirmationSelectedEvt( true, m_shader, m_material );

							if ( m_autoDeactivate )
								Deactivate();
						}

						if ( GUILayout.Button( m_noStr, m_buttonStyle ) )
						{
							if ( OnConfirmationSelectedEvt != null )
								OnConfirmationSelectedEvt( false, m_shader, m_material );
							if ( m_autoDeactivate )
								Deactivate();
						}
					}
					EditorGUILayout.EndHorizontal();
				}
				EditorGUILayout.EndVertical();
			}
			GUILayout.EndArea();
		}

		public void Deactivate()
		{
			m_isActive = false;
			OnConfirmationSelectedEvt = null;
		}
		public bool IsActive { get { return m_isActive; } }
	}
}
