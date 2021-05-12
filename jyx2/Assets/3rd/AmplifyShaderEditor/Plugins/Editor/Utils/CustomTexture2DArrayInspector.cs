using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[CustomEditor( typeof( Texture2DArray ) )]
	public class CustomTexture2DArrayInspector : Editor
	{
		Texture2DArray m_target;
		[SerializeField]
		float m_index;
		Shader m_textureArrayPreview;
		Material m_previewMaterial;
		GUIStyle slider = null;
		GUIStyle thumb = null;
		GUIContent m_allButton = null;
		[SerializeField]
		bool m_seeAll;
		void OnEnable()
		{
			m_target = ( target as Texture2DArray );
			m_textureArrayPreview = AssetDatabase.LoadAssetAtPath<Shader>( AssetDatabase.GUIDToAssetPath( "610c24aad350fba4583068c6c22fa428" ) );
			m_previewMaterial = new Material( m_textureArrayPreview );
			slider = null;
			thumb = null;
		}

		public override void OnPreviewGUI( Rect r, GUIStyle background )
		{
			base.OnPreviewGUI( r, background );
			m_previewMaterial.SetTexture( "_MainTex", m_target );
			m_previewMaterial.SetFloat( "_Index", m_index );
			EditorGUI.DrawPreviewTexture( r, m_target, m_previewMaterial, ScaleMode.ScaleToFit, 1f );
		}

		private void OnDisable()
		{
			DestroyImmediate( m_previewMaterial );
			m_previewMaterial = null;
		}

		public override void OnInspectorGUI()
		{
			if( slider == null )
				slider = "preSlider";

			if( thumb == null )
				thumb = "preSliderThumb";

			if( m_allButton == null )
				m_allButton = EditorGUIUtility.IconContent( "PreTextureMipMapLow" );

			base.OnInspectorGUI();
		}

		public override bool HasPreviewGUI()
		{
			return true;
		}

		public override void OnPreviewSettings()
		{
			base.OnPreviewSettings();
			m_seeAll = GUILayout.Toggle( m_seeAll, m_allButton, "preButton" );
			EditorGUI.BeginDisabledGroup( m_seeAll );
			m_index = Mathf.Round( GUILayout.HorizontalSlider( m_index, 0, m_target.depth - 1, slider, thumb ) );
			EditorGUI.EndDisabledGroup();
		}

		public override void OnInteractivePreviewGUI( Rect r, GUIStyle background )
		{
			//base.OnInteractivePreviewGUI( r, background );
			if( m_seeAll )
			{
				int columns = Mathf.CeilToInt( Mathf.Sqrt( m_target.depth ) );
				float sizeX = r.width / columns - 20;
				float centerY = ( columns * columns ) - m_target.depth;
				int rows = columns;
				if( centerY >= columns )
					rows--;
				float sizeY = ( r.height - 16 ) / rows - 15;

				if( centerY >= columns )
					centerY = sizeY * 0.5f;
				else
					centerY = 0;

				Rect smallRect = r;
				if( rows > 1 )
					smallRect.y += ( 15 / ( rows - 1 ) );
				else
					smallRect.y += 15;
				smallRect.x = r.x + 10;
				smallRect.width = sizeX;
				smallRect.height = sizeY;

				for( int i = 0; i < m_target.depth; i++ )
				{
					m_previewMaterial.SetTexture( "_MainTex", m_target );
					m_previewMaterial.SetFloat( "_Index", i );
					EditorGUI.DrawPreviewTexture( smallRect, m_target, m_previewMaterial, ScaleMode.ScaleToFit, 1 );
					Rect dropRect = smallRect;

					float diff = smallRect.height - smallRect.width;
					if( diff > 0 )
						dropRect.y -= diff * 0.5f;
					dropRect.y += 16;
					EditorGUI.DropShadowLabel( dropRect, "[" + i + "]" );

					smallRect.x += sizeX + 20;
					if( ( ( i + 1 ) % ( columns ) ) == 0 )
					{
						smallRect.x = r.x + 10;
						smallRect.height = sizeY;
						smallRect.y += sizeY + 30;
					}
				}
			}
			else
			{
				m_previewMaterial.SetTexture( "_MainTex", m_target );
				m_previewMaterial.SetFloat( "_Index", m_index );
				EditorGUI.DrawPreviewTexture( r, m_target, m_previewMaterial, ScaleMode.ScaleToFit, 1f );
				EditorGUI.DropShadowLabel( r, "[" + m_index + "]" );
			}
		}
	}
}
