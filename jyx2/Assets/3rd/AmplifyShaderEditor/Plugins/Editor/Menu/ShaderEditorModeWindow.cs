// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public sealed class ShaderEditorModeWindow : MenuParent
	{
		private static readonly Color OverallColorOn = new Color( 1f, 1f, 1f, 0.9f );
		private static readonly Color OverallColorOff = new Color( 1f, 1f, 1f, 0.3f );
		private static readonly Color FontColorOff = new Color( 1f, 1f, 1f, 0.4f );
		private const float DeltaY = 15;
		private const float DeltaX = 10;

		private const float CollSizeX = 180;
		private const float CollSizeY = 70;

		//private static string MatFormat = "<size=20>MATERIAL</size>\n{0}";
		//private static string ShaderFormat = "<size=20>SHADER</size>\n{0}";
		//private const string CurrMatStr = "MATERIAL";
		//private const string CurrShaderStr = "SHADER";

		private const string NoMaterialStr = "No Material";
		private const string NoShaderStr = "No Shader";

		private bool m_init = true;
		private string m_previousShaderName = string.Empty;
		private string m_previousMaterialName = string.Empty;
		private string m_previousShaderFunctionName = string.Empty;

		private Vector2 m_auxVector2;
		private GUIContent m_leftAuxContent = new GUIContent();
		private GUIContent m_rightAuxContent = new GUIContent();
		private GUIStyle m_leftButtonStyle = null;
		private GUIStyle m_rightButtonStyle = null;
		private Rect m_leftButtonRect;
		private Rect m_rightButtonRect;

		public ShaderEditorModeWindow( AmplifyShaderEditorWindow parentWindow ) : base( parentWindow, 0, 0, 0, 0, "ShaderEditorModeWindow", MenuAnchor.BOTTOM_CENTER, MenuAutoSize.NONE ) { }

		public void ConfigStyle( GUIStyle style )
		{
			style.normal.textColor = FontColorOff;
			style.hover.textColor = FontColorOff;
			style.active.textColor = FontColorOff;
			style.focused.textColor = FontColorOff;

			style.onNormal.textColor = FontColorOff;
			style.onHover.textColor = FontColorOff;
			style.onActive.textColor = FontColorOff;
			style.onFocused.textColor = FontColorOff;
		}


		public void Draw( Rect graphArea, Vector2 mousePos, Shader currentShader, Material currentMaterial, float usableArea, float leftPos, float rightPos /*, bool showLastSelection*/ )
		{
			EventType currentEventType = Event.current.type;

			if ( !( currentEventType == EventType.Repaint || currentEventType == EventType.MouseDown ) )
				return;

			if ( m_init )
			{
				m_init = false;
				GUIStyle shaderModeTitle = UIUtils.GetCustomStyle( CustomStyle.ShaderModeTitle );
				GUIStyle shaderModeNoShader = UIUtils.GetCustomStyle( CustomStyle.ShaderModeNoShader );
				GUIStyle materialModeTitle = UIUtils.GetCustomStyle( CustomStyle.MaterialModeTitle );
				GUIStyle shaderNoMaterialModeTitle = UIUtils.GetCustomStyle( CustomStyle.ShaderNoMaterialModeTitle );

				ConfigStyle( shaderModeTitle );
				ConfigStyle( shaderModeNoShader );
				ConfigStyle( materialModeTitle );
				ConfigStyle( shaderNoMaterialModeTitle );
			}
			Color buffereredColor = GUI.color;

			MasterNode currentMasterNode = ParentWindow.CurrentGraph.CurrentMasterNode;
			// Shader Mode
			if ( currentMasterNode != null )
			{
				m_leftButtonStyle = UIUtils.GetCustomStyle( currentShader == null ? CustomStyle.ShaderModeNoShader : CustomStyle.ShaderModeTitle );
				m_leftButtonRect = graphArea;
				m_leftButtonRect.x = 10 + leftPos;
				m_leftButtonRect.y += m_leftButtonRect.height - 38 - 15;
				string shaderName = ( currentShader != null ) ? ( currentShader.name ) : NoShaderStr;

				if ( m_previousShaderName != shaderName )
				{
					m_previousShaderName = shaderName;
					m_leftAuxContent.text = "<size=20>SHADER</size>\n" + shaderName;
				}

				m_auxVector2 = m_leftButtonStyle.CalcSize( m_leftAuxContent );
				m_leftButtonRect.width = m_auxVector2.x + 30 + 4;
				m_leftButtonRect.height = 38;

				bool mouseOnTop = m_leftButtonRect.Contains( mousePos );
				GUI.color = mouseOnTop ? OverallColorOn : OverallColorOff;
				GUI.Label( m_leftButtonRect, m_leftAuxContent, m_leftButtonStyle );

				if ( currentEventType == EventType.MouseDown && mouseOnTop && currentShader != null )
				{
					Event.current.Use();
					Selection.activeObject = currentShader;
					EditorGUIUtility.PingObject( Selection.activeObject );
				}

				// Material Mode
				if ( currentMaterial != null )
				{
					m_rightButtonStyle = UIUtils.GetCustomStyle( CustomStyle.MaterialModeTitle );
					m_rightButtonRect = graphArea;
					string matName = ( currentMaterial != null ) ? ( currentMaterial.name ) : NoMaterialStr;

					if ( m_previousMaterialName != matName )
					{
						m_previousMaterialName = matName;
						m_rightAuxContent.text = "<size=20>MATERIAL</size>\n" + matName;
					}

					m_auxVector2 = m_rightButtonStyle.CalcSize( m_rightAuxContent );
					m_rightButtonRect.width = m_auxVector2.x + 30 + 4;
					m_rightButtonRect.height = 38;

					m_rightButtonRect.x = graphArea.xMax - m_rightButtonRect.width - rightPos - 10;
					m_rightButtonRect.y = graphArea.yMax - 38 - 15;

					bool mouseOnTopRight = m_rightButtonRect.Contains( mousePos );
					GUI.color = mouseOnTopRight ? OverallColorOn : OverallColorOff;
					GUI.Label( m_rightButtonRect, m_rightAuxContent, m_rightButtonStyle );

					if ( currentEventType == EventType.MouseDown && mouseOnTopRight )
					{
						Event.current.Use();
						Selection.activeObject = currentMaterial;
						EditorGUIUtility.PingObject( Selection.activeObject );
					}
				}
			}

			// Shader Function
			else if ( currentMasterNode == null && ParentWindow.CurrentGraph.CurrentOutputNode != null )
			{
				m_leftButtonStyle = UIUtils.GetCustomStyle( CustomStyle.ShaderFunctionMode );
				m_leftButtonRect = graphArea;
				m_leftButtonRect.x = 10 + leftPos;
				m_leftButtonRect.y += m_leftButtonRect.height - 38 - 15;
				string functionName = ( ParentWindow.CurrentGraph.CurrentShaderFunction != null ) ? ( ParentWindow.CurrentGraph.CurrentShaderFunction.name ) : "No Shader Function";

				if ( m_previousShaderFunctionName != functionName )
				{
					m_previousShaderFunctionName = functionName;
					m_leftAuxContent.text = "<size=20>SHADER FUNCTION</size>\n" + functionName;
				}

				m_auxVector2 = m_leftButtonStyle.CalcSize( m_leftAuxContent );
				m_leftButtonRect.width = m_auxVector2.x + 30 + 4;
				m_leftButtonRect.height = 38;

				bool mouseOnTop = m_leftButtonRect.Contains( mousePos );
				GUI.color = mouseOnTop ? OverallColorOn : OverallColorOff;
				GUI.Label( m_leftButtonRect, m_leftAuxContent, m_leftButtonStyle );

				if ( currentEventType == EventType.MouseDown && mouseOnTop && ParentWindow.CurrentGraph.CurrentShaderFunction != null )
				{
					Event.current.Use();
					Selection.activeObject = ParentWindow.CurrentGraph.CurrentShaderFunction;
					EditorGUIUtility.PingObject( Selection.activeObject );
				}
			}

			GUI.color = buffereredColor;
		}

		public override void Destroy()
		{
			base.Destroy();
			m_leftAuxContent = null;
			m_rightAuxContent = null;
			m_leftButtonStyle = null;
			m_rightButtonStyle = null;
		}
	}
}
