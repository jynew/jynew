// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class RenderingPlatformOpHelper
	{
		private const string RenderingPlatformsStr = " Rendering Platforms";
		private readonly string[] RenderingPlatformsLabels =    {   " Direct3D 9",
																	" Direct3D 11/12",
																	" OpenGL 3.x/4.x",
																	" OpenGL ES 2.0",
																	" OpenGL ES 3.x",
																	" iOS/Mac Metal",
																	" Direct3D 11 9.x",
																	" Xbox 360",
																	" Xbox One",
																	" PlayStation 4",
																	" PlayStation Vita",
																	" Nintendo 3DS",
																	" Nintendo Wii U" };

		[SerializeField]
		private bool[] m_renderingPlatformValues;

		public RenderingPlatformOpHelper()
		{
			m_renderingPlatformValues = new bool[ RenderingPlatformsLabels.Length ];
			for ( int i = 0; i < m_renderingPlatformValues.Length; i++ )
			{
				m_renderingPlatformValues[ i ] = true;
			}
		}


		public void Draw( ParentNode owner )
		{
			bool value = owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedRenderingPlatforms;
			NodeUtils.DrawPropertyGroup( ref value, RenderingPlatformsStr, () =>
			{
				 for ( int i = 0; i < m_renderingPlatformValues.Length; i++ )
				 {
					 m_renderingPlatformValues[ i ] = owner.EditorGUILayoutToggleLeft( RenderingPlatformsLabels[ i ], m_renderingPlatformValues[ i ] );
				 }
			 } );
			owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedRenderingPlatforms = value;
		}

		public void SetRenderingPlatforms( ref string ShaderBody )
		{
			int checkedPlatforms = 0;
			int uncheckedPlatforms = 0;

			for ( int i = 0; i < m_renderingPlatformValues.Length; i++ )
			{
				if ( m_renderingPlatformValues[ i ] )
				{
					checkedPlatforms += 1;
				}
				else
				{
					uncheckedPlatforms += 1;
				}
			}

			if ( checkedPlatforms > 0 && checkedPlatforms < m_renderingPlatformValues.Length )
			{
				string result = string.Empty;
				if ( checkedPlatforms < uncheckedPlatforms )
				{
					result = "only_renderers ";
					for ( int i = 0; i < m_renderingPlatformValues.Length; i++ )
					{
						if ( m_renderingPlatformValues[ i ] )
						{
							result += ( RenderPlatforms ) i + " ";
						}
					}
				}
				else
				{
					result = "exclude_renderers ";
					for ( int i = 0; i < m_renderingPlatformValues.Length; i++ )
					{
						if ( !m_renderingPlatformValues[ i ] )
						{
							result += ( RenderPlatforms ) i + " ";
						}
					}
				}
				MasterNode.AddShaderPragma( ref ShaderBody, result );
			}
		}


		public void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			for ( int i = 0; i < m_renderingPlatformValues.Length; i++ )
			{
				m_renderingPlatformValues[ i ] = Convert.ToBoolean( nodeParams[ index++ ] );
			}
		}

		public void WriteToString( ref string nodeInfo )
		{
			for ( int i = 0; i < m_renderingPlatformValues.Length; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_renderingPlatformValues[ i ] );
			}
		}
	}
}
