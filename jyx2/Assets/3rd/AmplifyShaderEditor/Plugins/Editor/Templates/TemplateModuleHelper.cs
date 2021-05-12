// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{

	[Serializable]
	public class TemplateModulesHelper
	{
		[SerializeField]
		internal bool Foldout = false;

		private bool m_isDirty = false;

		[SerializeField]
		private TemplatesBlendModule m_blendOpHelper = new TemplatesBlendModule();

		[SerializeField]
		private TemplateCullModeModule m_cullModeHelper = new TemplateCullModeModule();

		[SerializeField]
		private TemplateColorMaskModule m_colorMaskHelper = new TemplateColorMaskModule();

		[SerializeField]
		private TemplatesStencilBufferModule m_stencilBufferHelper = new TemplatesStencilBufferModule();

		[SerializeField]
		private TemplateDepthModule m_depthOphelper = new TemplateDepthModule();

		[SerializeField]
		private TemplateTagsModule m_tagsHelper = new TemplateTagsModule();

		[SerializeField]
		private TemplateShaderModelModule m_shaderModelHelper = new TemplateShaderModelModule();

		[SerializeField]
		private TemplateAdditionalIncludesHelper m_additionalIncludes = new TemplateAdditionalIncludesHelper();

		[SerializeField]
		private TemplateAdditionalDefinesHelper m_additionalDefines = new TemplateAdditionalDefinesHelper();

		[SerializeField]
		private TemplateAdditionalPragmasHelper m_additionalPragmas = new TemplateAdditionalPragmasHelper();

		[SerializeField]
		private TemplateAdditionalDirectivesHelper m_additionalDirectives = new TemplateAdditionalDirectivesHelper(" Additional Directives");

		[SerializeField]
		private bool m_hasValidData = false;

		[SerializeField]
		private bool m_allModulesMode = false;

		public void CopyFrom( TemplateModulesHelper other )
		{
			m_allModulesMode = other.AllModulesMode;

			if( other.BlendOpHelper.IsDirty )
			{
				m_blendOpHelper.CopyFrom( other.BlendOpHelper, true );
			}

			if( other.CullModeHelper.IsDirty )
			{
				m_cullModeHelper.CopyFrom( other.CullModeHelper , true );
			}

			if( other.ColorMaskHelper.IsDirty )
			{
				m_colorMaskHelper.CopyFrom( other.ColorMaskHelper , true);
			}

			if( other.StencilBufferHelper.IsDirty )
			{
				m_stencilBufferHelper.CopyFrom( other.StencilBufferHelper,true );
			}

			if( other.DepthOphelper.IsDirty )
			{
				m_depthOphelper.CopyFrom( other.DepthOphelper,true );
			}

			if( other.TagsHelper.IsDirty )
			{
				m_tagsHelper.CopyFrom( other.TagsHelper );
			}

			if( other.ShaderModelHelper.IsDirty )
			{
				m_shaderModelHelper.CopyFrom( other.ShaderModelHelper, true );
			}
		}

		public void SyncWith( TemplateModulesHelper other )
		{

			if( m_blendOpHelper.ValidData && other.BlendOpHelper.ValidData )
			{
				m_blendOpHelper.CopyFrom( other.BlendOpHelper, false );
			}

			if( m_cullModeHelper.ValidData && other.CullModeHelper.ValidData )
			{
				m_cullModeHelper.CopyFrom( other.CullModeHelper, false );
			}

			if( m_colorMaskHelper.ValidData && other.ColorMaskHelper.ValidData )
			{
				m_colorMaskHelper.CopyFrom( other.ColorMaskHelper , false );
			}

			if( m_stencilBufferHelper.ValidData && other.StencilBufferHelper.ValidData )
			{
				m_stencilBufferHelper.CopyFrom( other.StencilBufferHelper, false );
			}

			if( m_depthOphelper.ValidData && other.DepthOphelper.ValidData )
			{
				m_depthOphelper.CopyFrom( other.DepthOphelper, false );
			}
			
			if( m_shaderModelHelper.ValidData && other.ShaderModelHelper.ValidData )
			{
				m_shaderModelHelper.CopyFrom( other.ShaderModelHelper , false);
			}
		}

		public void FetchDataFromTemplate( TemplateModulesData module )
		{
			m_allModulesMode = module.AllModulesMode;

			if( module.PragmaTag.IsValid )
			{
				m_hasValidData = true;
				//m_additionalPragmas.IsValid = true;
				//m_additionalPragmas.FillNativeItems( module.IncludePragmaContainer.PragmasList );

				//m_additionalIncludes.IsValid = true;
				//m_additionalIncludes.FillNativeItems( module.IncludePragmaContainer.IncludesList );

				//m_additionalDefines.IsValid = true;
				//m_additionalDefines.FillNativeItems( module.IncludePragmaContainer.DefinesList );

				m_additionalDirectives.FillNativeItems( module.IncludePragmaContainer.NativeDirectivesList );
				m_additionalDirectives.IsValid = true;
			}
			else
			{
				//m_additionalPragmas.IsValid = false;
				//m_additionalIncludes.IsValid = false;
				//m_additionalDefines.IsValid = false;
				m_additionalDirectives.IsValid = false;
			}

			m_blendOpHelper.ConfigureFromTemplateData( module.BlendData );
			if( module.BlendData.DataCheck == TemplateDataCheck.Valid )
			{
				m_hasValidData = true;
			}

			m_cullModeHelper.ConfigureFromTemplateData( module.CullModeData );
			if( module.CullModeData.DataCheck == TemplateDataCheck.Valid )
			{
				m_hasValidData = true;
			}

			m_colorMaskHelper.ConfigureFromTemplateData( module.ColorMaskData );
			if( module.ColorMaskData.DataCheck == TemplateDataCheck.Valid )
			{
				m_hasValidData = true;
			}

			m_stencilBufferHelper.ConfigureFromTemplateData( module.StencilData );
			if( module.StencilData.DataCheck == TemplateDataCheck.Valid )
			{
				m_hasValidData = true;
			}

			m_depthOphelper.ConfigureFromTemplateData( module.DepthData );
			if( module.DepthData.DataCheck == TemplateDataCheck.Valid )
			{
				m_hasValidData = true;
			}

			m_tagsHelper.ConfigureFromTemplateData( module.TagData );
			if( module.TagData.DataCheck == TemplateDataCheck.Valid )
			{
				m_hasValidData = true;
			}

			m_shaderModelHelper.ConfigureFromTemplateData( module.ShaderModel );
			if( module.ShaderModel.DataCheck == TemplateDataCheck.Valid )
			{
				m_hasValidData = true;
			}
		}

		public void OnLogicUpdate( TemplateModulesData currentModule )
		{
			if( currentModule.TagData.DataCheck == TemplateDataCheck.Valid )
				m_tagsHelper.OnLogicUpdate();
		}

		public void Draw( ParentNode owner, TemplateModulesData currentModule , TemplateModulesHelper parent = null )
		{
			if( currentModule.ShaderModel.DataCheck == TemplateDataCheck.Valid )
				m_shaderModelHelper.Draw( owner );

			m_isDirty = m_shaderModelHelper.IsDirty;

			if( currentModule.CullModeData.DataCheck == TemplateDataCheck.Valid )
				m_cullModeHelper.Draw( owner );

			m_isDirty = m_isDirty || m_cullModeHelper.IsDirty;

			if( currentModule.ColorMaskData.DataCheck == TemplateDataCheck.Valid )
				m_colorMaskHelper.Draw( owner );

			m_isDirty = m_isDirty || m_colorMaskHelper.IsDirty;

			if( currentModule.DepthData.DataCheck == TemplateDataCheck.Valid )
				m_depthOphelper.Draw( owner, false );

			m_isDirty = m_isDirty || m_depthOphelper.IsDirty;

			if( currentModule.BlendData.DataCheck == TemplateDataCheck.Valid )
				m_blendOpHelper.Draw( owner, false );

			m_isDirty = m_isDirty || m_blendOpHelper.IsDirty;


			if( currentModule.StencilData.DataCheck == TemplateDataCheck.Valid )
			{
				CullMode cullMode = CullMode.Back;
				if( currentModule.CullModeData.DataCheck == TemplateDataCheck.Valid )
				{
					cullMode = m_cullModeHelper.CurrentCullMode;
				}
				else if( parent != null && parent.CullModeHelper.ValidData )
				{
					cullMode = parent.CullModeHelper.CurrentCullMode;
				}
				m_stencilBufferHelper.Draw( owner, cullMode, false );
			}

			m_isDirty = m_isDirty || m_stencilBufferHelper.IsDirty;

			if( currentModule.TagData.DataCheck == TemplateDataCheck.Valid )
				m_tagsHelper.Draw( owner, false );

			m_isDirty = m_isDirty || m_tagsHelper.IsDirty;

			if( currentModule.PragmaTag.IsValid )
			{
				//m_additionalDefines.Draw( owner );
				//m_additionalIncludes.Draw( owner );
				//m_additionalPragmas.Draw( owner );
				m_additionalDirectives.Draw( owner , false);
			}

			m_isDirty = m_isDirty ||
						//m_additionalDefines.IsDirty ||
						//m_additionalIncludes.IsDirty ||
						//m_additionalPragmas.IsDirty || 
						m_additionalDirectives.IsDirty;
		}

		public void Destroy()
		{
			m_shaderModelHelper = null;
			m_blendOpHelper = null;
			m_cullModeHelper = null;
			m_colorMaskHelper.Destroy();
			m_colorMaskHelper = null;
			m_stencilBufferHelper.Destroy();
			m_stencilBufferHelper = null;
			m_tagsHelper.Destroy();
			m_tagsHelper = null;
			m_additionalDefines.Destroy();
			m_additionalDefines = null;
			m_additionalIncludes.Destroy();
			m_additionalIncludes = null;
			m_additionalPragmas.Destroy();
			m_additionalPragmas = null;
			m_additionalDirectives.Destroy();
			m_additionalDirectives = null;
		}

		public string GenerateAllModulesString( bool isSubShader )
		{
			string moduleBody = string.Empty;
			if( !ShaderModelHelper.IndependentModule )
			{
				moduleBody += ShaderModelHelper.GenerateShaderData( isSubShader ) + "\n";
			}

			if( !BlendOpHelper.IndependentModule )
			{
				if( BlendOpHelper.BlendModeEnabled )
					moduleBody += BlendOpHelper.CurrentBlendFactor + "\n";

				if( BlendOpHelper.BlendOpActive )
					moduleBody += BlendOpHelper.CurrentBlendOp + "\n";
			}

			if( !CullModeHelper.IndependentModule )
				moduleBody += CullModeHelper.GenerateShaderData( isSubShader ) + "\n";

			if( !ColorMaskHelper.IndependentModule )
				moduleBody += ColorMaskHelper.GenerateShaderData( isSubShader ) + "\n";

			if( !DepthOphelper.IndependentModule )
			{
				moduleBody += DepthOphelper.CurrentZWriteMode;
				moduleBody += DepthOphelper.CurrentZTestMode;
				if( DepthOphelper.OffsetEnabled )
					moduleBody += DepthOphelper.CurrentOffset;
			}

			if( !StencilBufferHelper.IndependentModule && StencilBufferHelper.Active )
			{
				CullMode cullMode = ( CullModeHelper.ValidData ) ? CullModeHelper.CurrentCullMode : CullMode.Back;
				moduleBody += StencilBufferHelper.CreateStencilOp( cullMode );
			}

			return moduleBody;
		}

		public void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			try
			{
				m_blendOpHelper.ReadFromString( ref index, ref nodeParams );
			}
			catch( Exception e )
			{
				Debug.LogException( e );
			}
			try
			{
				m_cullModeHelper.ReadFromString( ref index, ref nodeParams );
			}
			catch( Exception e )
			{
				Debug.LogException( e );
			}
			try
			{
				m_colorMaskHelper.ReadFromString( ref index, ref nodeParams );
			}
			catch( Exception e )
			{
				Debug.LogException( e );
			}
			try
			{
				m_stencilBufferHelper.ReadFromString( ref index, ref nodeParams );
			}
			catch( Exception e )
			{
				Debug.LogException( e );
			}
			try
			{
				m_depthOphelper.ReadFromString( ref index, ref nodeParams );
			}
			catch( Exception e )
			{
				Debug.LogException( e );
			}
			try
			{
				m_tagsHelper.ReadFromString( ref index, ref nodeParams );
			}
			catch( Exception e )
			{
				Debug.LogException( e );
			}
			try
			{
				m_shaderModelHelper.ReadFromString( ref index, ref nodeParams );
			}
			catch( Exception e )
			{
				Debug.LogException( e );
			}


			if( UIUtils.CurrentShaderVersion() < 15312 )
			{
				try
				{
					m_additionalDefines.ReadFromString( ref index, ref nodeParams );
				}
				catch( Exception e )
				{
					Debug.LogException( e );
				}
				try
				{
					m_additionalPragmas.ReadFromString( ref index, ref nodeParams );
				}
				catch( Exception e )
				{
					Debug.LogException( e );
				}
				try
				{
					m_additionalIncludes.ReadFromString( ref index, ref nodeParams );
				}
				catch( Exception e )
				{
					Debug.LogException( e );
				}

				m_additionalDirectives.AddItems( AdditionalLineType.Include, m_additionalIncludes.ItemsList );
				m_additionalDirectives.AddItems( AdditionalLineType.Define, m_additionalDefines.ItemsList );
				m_additionalDirectives.AddItems( AdditionalLineType.Pragma, m_additionalPragmas.ItemsList );

			}
			else
			{
				try
				{
					m_additionalDirectives.ReadFromString( ref index, ref nodeParams );
				}
				catch( Exception e )
				{
					Debug.LogException( e );
				}
			}
		}

		public void WriteToString( ref string nodeInfo )
		{
			m_blendOpHelper.WriteToString( ref nodeInfo );
			m_cullModeHelper.WriteToString( ref nodeInfo );
			m_colorMaskHelper.WriteToString( ref nodeInfo );
			m_stencilBufferHelper.WriteToString( ref nodeInfo );
			m_depthOphelper.WriteToString( ref nodeInfo );
			m_tagsHelper.WriteToString( ref nodeInfo );
			m_shaderModelHelper.WriteToString( ref nodeInfo );

			//m_additionalDefines.WriteToString( ref nodeInfo );
			//m_additionalPragmas.WriteToString( ref nodeInfo );
			//m_additionalIncludes.WriteToString( ref nodeInfo );

			m_additionalDirectives.WriteToString( ref nodeInfo );
		}

		public TemplatesBlendModule BlendOpHelper { get { return m_blendOpHelper; } }
		public TemplateCullModeModule CullModeHelper { get { return m_cullModeHelper; } }
		public TemplateColorMaskModule ColorMaskHelper { get { return m_colorMaskHelper; } }
		public TemplatesStencilBufferModule StencilBufferHelper { get { return m_stencilBufferHelper; } }
		public TemplateDepthModule DepthOphelper { get { return m_depthOphelper; } }
		public TemplateTagsModule TagsHelper { get { return m_tagsHelper; } }
		public TemplateShaderModelModule ShaderModelHelper { get { return m_shaderModelHelper; } }
		//public TemplateAdditionalIncludesHelper AdditionalIncludes { get { return m_additionalIncludes; } }
		//public TemplateAdditionalDefinesHelper AdditionalDefines { get { return m_additionalDefines; } }
		//public TemplateAdditionalPragmasHelper AdditionalPragmas { get { return m_additionalPragmas; } }
		public TemplateAdditionalDirectivesHelper AdditionalDirectives { get { return m_additionalDirectives; } }
		public bool AllModulesMode { get { return m_allModulesMode; } }
		public bool HasValidData { get { return m_hasValidData; } }
		public bool IsDirty
		{
			get { return m_isDirty; }
			set
			{
				m_isDirty = value;
				if( !value )
				{
					m_blendOpHelper.IsDirty = false;
					m_cullModeHelper.IsDirty = false;
					m_colorMaskHelper.IsDirty = false;
					m_stencilBufferHelper.IsDirty = false;
					m_tagsHelper.IsDirty = false;
					m_shaderModelHelper.IsDirty = false;
					//m_additionalDefines.IsDirty = false;
					//m_additionalPragmas.IsDirty = false;
					//m_additionalIncludes.IsDirty = false;
					m_additionalDirectives.IsDirty = false;
				}
			}
		}
		//	public bool Foldout { get { return m_foldout; } set { m_foldout = value;  } }
	}
}
