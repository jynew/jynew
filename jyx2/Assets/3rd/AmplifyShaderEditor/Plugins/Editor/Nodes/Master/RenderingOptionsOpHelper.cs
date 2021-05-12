// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public enum DisableBatchingTagValues
	{
		True,
		False,
		LODFading
	}

	[Serializable]
	public class RenderingOptionsOpHelper
	{
		private const string RenderingOptionsStr = " Rendering Options";
		private readonly static GUIContent EmissionGIFlags = new GUIContent( "Emission GI Flag", "Modifies Emission GI flags" );
		private readonly static GUIContent LODCrossfadeContent = new GUIContent( " LOD Group Cross Fade", "Applies a dither crossfade to be used with LOD groups for smoother transitions. Uses one interpolator\nDefault: OFF" );
		private readonly static GUIContent DisableBatchingContent = new GUIContent( "Disable Batching", "\nDisables objects to be batched and used with DrawCallBatching Default: False" );
		private readonly static GUIContent IgnoreProjectorContent = new GUIContent( " Ignore Projector", "\nIf True then an object that uses this shader will not be affected by Projectors Default: False" );
		private readonly static GUIContent ForceNoShadowCastingContent = new GUIContent( " Force No Shadow Casting", "\nIf True then an object that is rendered using this subshader will never cast shadows Default: False" );
		private readonly static GUIContent ForceEnableInstancingContent = new GUIContent( " Force Enable Instancing", "\nIf True forces instancing on shader independent of having instanced properties" );
#if UNITY_5_6_OR_NEWER
		private readonly static GUIContent ForceDisableInstancingContent = new GUIContent( " Force Disable Instancing", "\nIf True forces disable instancing on shader independent of having instanced properties" );
#endif
		private readonly static GUIContent SpecularHightlightsContent = new GUIContent( " Fwd Specular Highlights Toggle", "\nIf True creates a material toggle to set Unity's internal specular highlight rendering keyword" );
		private readonly static GUIContent ReflectionsContent = new GUIContent( " Fwd Reflections Toggle", "\nIf True creates a material toggle to set Unity's internal reflections rendering keyword" );

		[SerializeField]
		private bool m_forceEnableInstancing = false;

		[SerializeField]
		private bool m_forceDisableInstancing = false;

		[SerializeField]
		private bool m_specularHighlightToggle = false;

		[SerializeField]
		private bool m_reflectionsToggle = false;

		[SerializeField]
		private bool m_lodCrossfade = false;

		[SerializeField]
		private DisableBatchingTagValues m_disableBatching = DisableBatchingTagValues.False;

		[SerializeField]
		private bool m_ignoreProjector = false;

		[SerializeField]
		private bool m_forceNoShadowCasting = false;

		[SerializeField]
		private List<CodeGenerationData> m_codeGenerationDataList;
		
		public RenderingOptionsOpHelper()
		{
			m_codeGenerationDataList = new List<CodeGenerationData>();
			m_codeGenerationDataList.Add( new CodeGenerationData( " Exclude Deferred", "exclude_path:deferred" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " Exclude Forward", "exclude_path:forward" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " Exclude Legacy Deferred", "exclude_path:prepass" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " Shadows", "noshadow" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " Ambient Light", "noambient" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " Per Vertex Light", "novertexlights" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " Lightmaps", "nolightmap " ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " Dynamic Global GI", "nodynlightmap" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " Directional lightmaps", "nodirlightmap" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " Built-in Fog", "nofog" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " Meta Pass", "nometa" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " Add Pass", "noforwardadd" ) );
		}

		public bool IsOptionActive( string option )
		{
			return !m_codeGenerationDataList.Find( x => x.Name.Equals( option ) ).IsActive;
		}

		public void Draw( ParentNode owner )
		{
			bool value = owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedRenderingOptions;
			NodeUtils.DrawPropertyGroup( ref value, RenderingOptionsStr, () =>
			{
				int codeGenCount = m_codeGenerationDataList.Count;
				// Starting from index 4 because other options are already contemplated with m_renderPath and add/receive shadows
				for( int i = 4; i < codeGenCount; i++ )
				{
					m_codeGenerationDataList[ i ].IsActive = !owner.EditorGUILayoutToggleLeft( m_codeGenerationDataList[ i ].Name, !m_codeGenerationDataList[ i ].IsActive );
				}
				m_lodCrossfade = owner.EditorGUILayoutToggleLeft( LODCrossfadeContent, m_lodCrossfade );
				m_ignoreProjector = owner.EditorGUILayoutToggleLeft( IgnoreProjectorContent, m_ignoreProjector );
				m_forceNoShadowCasting = owner.EditorGUILayoutToggleLeft( ForceNoShadowCastingContent, m_forceNoShadowCasting );
				if( owner.ContainerGraph.IsInstancedShader )
				{
					GUI.enabled = false;
					owner.EditorGUILayoutToggleLeft( ForceEnableInstancingContent, true );
					GUI.enabled = true;
				}
				else
				{
					m_forceEnableInstancing = owner.EditorGUILayoutToggleLeft( ForceEnableInstancingContent, m_forceEnableInstancing );
				}

#if UNITY_5_6_OR_NEWER
				m_forceDisableInstancing = owner.EditorGUILayoutToggleLeft( ForceDisableInstancingContent, m_forceDisableInstancing );
#endif
				m_specularHighlightToggle = owner.EditorGUILayoutToggleLeft( SpecularHightlightsContent, m_specularHighlightToggle );
				m_reflectionsToggle = owner.EditorGUILayoutToggleLeft( ReflectionsContent, m_reflectionsToggle );
				m_disableBatching = (DisableBatchingTagValues)owner.EditorGUILayoutEnumPopup( DisableBatchingContent, m_disableBatching );
				Material mat = owner.ContainerGraph.CurrentMaterial;
				if( mat != null )
				{
					mat.globalIlluminationFlags = (MaterialGlobalIlluminationFlags)owner.EditorGUILayoutEnumPopup( EmissionGIFlags, mat.globalIlluminationFlags );
				}
			} );
			owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedRenderingOptions = value;
		}

		public void Build( ref string OptionalParameters )
		{
			int codeGenCount = m_codeGenerationDataList.Count;

			for( int i = 0; i < codeGenCount; i++ )
			{
				if( m_codeGenerationDataList[ i ].IsActive )
				{
					OptionalParameters += m_codeGenerationDataList[ i ].Value + Constants.OptionalParametersSep;
				}
			}

#if UNITY_2017_1_OR_NEWER
		if( m_lodCrossfade )
		{
			OptionalParameters += Constants.LodCrossFadeOption2017 + Constants.OptionalParametersSep;
		}
#endif
		}

		public void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			for( int i = 0; i < m_codeGenerationDataList.Count; i++ )
			{
				m_codeGenerationDataList[ i ].IsActive = Convert.ToBoolean( nodeParams[ index++ ] );
			}

			if( UIUtils.CurrentShaderVersion() > 10005 )
			{
				m_lodCrossfade = Convert.ToBoolean( nodeParams[ index++ ] );
			}

			if( UIUtils.CurrentShaderVersion() > 10007 )
			{
				m_disableBatching = (DisableBatchingTagValues)Enum.Parse( typeof( DisableBatchingTagValues ), nodeParams[ index++ ] );
				m_ignoreProjector = Convert.ToBoolean( nodeParams[ index++ ] );
				m_forceNoShadowCasting = Convert.ToBoolean( nodeParams[ index++ ] );
			}

			if( UIUtils.CurrentShaderVersion() > 11002 )
			{
				m_forceEnableInstancing = Convert.ToBoolean( nodeParams[ index++ ] );
			}

			if( UIUtils.CurrentShaderVersion() > 15205 )
			{
				m_forceDisableInstancing = Convert.ToBoolean( nodeParams[ index++ ] );
			}

			if( UIUtils.CurrentShaderVersion() > 14403 )
			{
				m_specularHighlightToggle = Convert.ToBoolean( nodeParams[ index++ ] );
				m_reflectionsToggle = Convert.ToBoolean( nodeParams[ index++ ] );
			}
		}

		public void WriteToString( ref string nodeInfo )
		{
			for( int i = 0; i < m_codeGenerationDataList.Count; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_codeGenerationDataList[ i ].IsActive );
			}

			IOUtils.AddFieldValueToString( ref nodeInfo, m_lodCrossfade );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_disableBatching );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_ignoreProjector );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_forceNoShadowCasting );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_forceEnableInstancing );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_forceDisableInstancing );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_specularHighlightToggle );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_reflectionsToggle );
		}
		
		public void Destroy()
		{
			m_codeGenerationDataList.Clear();
			m_codeGenerationDataList = null;
		}

		public bool ForceEnableInstancing { get { return m_forceEnableInstancing; } }
		public bool ForceDisableInstancing { get { return m_forceDisableInstancing; } }

		public bool LodCrossfade { get { return m_lodCrossfade; } }
		public bool IgnoreProjectorValue { get { return m_ignoreProjector; } set { m_ignoreProjector = value; } }
		public bool SpecularHighlightToggle { get { return m_specularHighlightToggle; } set { m_specularHighlightToggle = value; } }
		public bool ReflectionsToggle { get { return m_reflectionsToggle; } set { m_reflectionsToggle = value; } }

		public string DisableBatchingTag { get { return ( m_disableBatching != DisableBatchingTagValues.False ) ? string.Format( Constants.TagFormat, "DisableBatching", m_disableBatching ) : string.Empty; } }
		public string IgnoreProjectorTag { get { return ( m_ignoreProjector ) ? string.Format( Constants.TagFormat, "IgnoreProjector", "True" ) : string.Empty; } }
		public string ForceNoShadowCastingTag { get { return ( m_forceNoShadowCasting ) ? string.Format( Constants.TagFormat, "ForceNoShadowCasting", "True" ) : string.Empty; } }
	}
}
