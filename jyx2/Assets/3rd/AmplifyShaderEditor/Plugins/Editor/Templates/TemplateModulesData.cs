// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public enum TemplateModuleDataType
	{
		ModuleShaderModel,
		ModuleBlendMode,
		ModuleBlendOp,
		ModuleCullMode,
		ModuleColorMask,
		ModuleStencil,
		ModuleZwrite,
		ModuleZTest,
		ModuleZOffset,
		ModuleTag,
		ModuleGlobals,
		ModuleFunctions,
		ModulePragma,
		ModulePass,
		ModuleInputVert,
		ModuleInputFrag,
		PassVertexFunction,
		PassFragmentFunction,
		PassVertexData,
		PassInterpolatorData,
		PassNameData,
		AllModules
		//EndPass
	}

	public enum TemplateSRPType
	{
		BuiltIn,
		HD,
		Lightweight
	}

	[Serializable]
	public class TemplateModulesData
	{
		[SerializeField]
		private TemplateBlendData m_blendData = new TemplateBlendData();

		[SerializeField]
		private TemplateCullModeData m_cullModeData = new TemplateCullModeData();

		[SerializeField]
		private TemplateColorMaskData m_colorMaskData = new TemplateColorMaskData();

		[SerializeField]
		private TemplateStencilData m_stencilData = new TemplateStencilData();

		[SerializeField]
		private TemplateDepthData m_depthData = new TemplateDepthData();

		[SerializeField]
		private TemplateTagsModuleData m_tagData = new TemplateTagsModuleData();

		[SerializeField]
		private TemplateTagData m_globalsTag = new TemplateTagData( TemplatesManager.TemplateGlobalsTag, true );

		[SerializeField]
		private TemplateTagData m_allModulesTag = new TemplateTagData( TemplatesManager.TemplateAllModulesTag, true );

		[SerializeField]
		private TemplateTagData m_functionsTag = new TemplateTagData( TemplatesManager.TemplateFunctionsTag, true );

		[SerializeField]
		private TemplateTagData m_pragmaTag = new TemplateTagData( TemplatesManager.TemplatePragmaTag, true );

		[SerializeField]
		private TemplateTagData m_passTag = new TemplateTagData( TemplatesManager.TemplatePassTag, true );

		[SerializeField]
		private TemplateTagData m_inputsVertTag = new TemplateTagData( TemplatesManager.TemplateInputsVertParamsTag, false );

		[SerializeField]
		private TemplateTagData m_inputsFragTag = new TemplateTagData( TemplatesManager.TemplateInputsFragParamsTag, false );

		[SerializeField]
		private TemplateShaderModelData m_shaderModel = new TemplateShaderModelData();

		[SerializeField]
		private TemplateSRPType m_srpType = TemplateSRPType.BuiltIn;

		[SerializeField]
		private bool m_srpIsPBR = false;

		[SerializeField]
		private string m_uniquePrefix;

		[SerializeField]
		private TemplateIncludePragmaContainter m_includePragmaContainer = new TemplateIncludePragmaContainter();

		[SerializeField]
		private bool m_allModulesMode = false;

		public void Destroy()
		{
			m_blendData = null;
			m_cullModeData = null;
			m_colorMaskData = null;
			m_stencilData = null;
			m_depthData = null;
			m_tagData.Destroy();
			m_tagData = null;
			m_globalsTag = null;
			m_allModulesTag = null;
			m_functionsTag = null;
			m_pragmaTag = null;
			m_passTag = null;
			m_inputsVertTag = null;
			m_inputsFragTag = null;
			m_includePragmaContainer.Destroy();
			m_includePragmaContainer = null;
		}

		public void ConfigureCommonTag( TemplateTagData tagData, TemplatePropertyContainer propertyContainer, TemplateIdManager idManager, string uniquePrefix, int offsetIdx, string subBody )
		{
			int id = subBody.IndexOf( tagData.Id );
			if ( id >= 0 )
			{
				tagData.StartIdx = offsetIdx + id;
				idManager.RegisterId( tagData.StartIdx, uniquePrefix + tagData.Id, tagData.Id );
				propertyContainer.AddId( subBody, tagData.Id, tagData.SearchIndentation );
			}
		}

		public TemplateModulesData( TemplateIdManager idManager, TemplatePropertyContainer propertyContainer, string uniquePrefix, int offsetIdx, string subBody, bool isSubShader )
		{
			if ( string.IsNullOrEmpty( subBody ) )
				return;

			m_uniquePrefix = uniquePrefix;
			//PRAGMAS AND INCLUDES
			TemplateHelperFunctions.CreatePragmaIncludeList( subBody, m_includePragmaContainer );

			//COMMON TAGS
			ConfigureCommonTag( m_globalsTag, propertyContainer, idManager, uniquePrefix, offsetIdx, subBody );
			ConfigureCommonTag( m_functionsTag, propertyContainer, idManager, uniquePrefix, offsetIdx, subBody );
			ConfigureCommonTag( m_pragmaTag, propertyContainer, idManager, uniquePrefix, offsetIdx, subBody );
			ConfigureCommonTag( m_passTag, propertyContainer, idManager, uniquePrefix, offsetIdx, subBody );
			ConfigureCommonTag( m_inputsVertTag, propertyContainer, idManager, uniquePrefix, offsetIdx, subBody );
			ConfigureCommonTag( m_inputsFragTag, propertyContainer, idManager, uniquePrefix, offsetIdx, subBody );

			//BlEND MODE
			{
				Match blendModeMatch = Regex.Match( subBody, TemplateHelperFunctions.BlendWholeWordPattern );
				if( blendModeMatch.Success )
				{
					int blendModeIdx = blendModeMatch.Index;
					
					int end = subBody.IndexOf( TemplatesManager.TemplateNewLine, blendModeIdx );
					string blendParams = subBody.Substring( blendModeIdx, end - blendModeIdx );
					m_blendData.BlendModeId = blendParams;
					m_blendData.BlendModeStartIndex = offsetIdx + blendModeIdx;
					idManager.RegisterId( m_blendData.BlendModeStartIndex, uniquePrefix + m_blendData.BlendModeId, m_blendData.BlendModeId );

					TemplateHelperFunctions.CreateBlendMode( blendParams, ref m_blendData );
					if( m_blendData.ValidBlendMode )
					{
						propertyContainer.AddId( subBody, blendParams, false );
					}
					
				}
			}
			//BLEND OP
			{
				Match blendOpMatch = Regex.Match( subBody, TemplateHelperFunctions.BlendOpWholeWordPattern );
				if( blendOpMatch.Success )
				{
					int blendOpIdx = blendOpMatch.Index;
					int end = subBody.IndexOf( TemplatesManager.TemplateNewLine, blendOpIdx );
					string blendOpParams = subBody.Substring( blendOpIdx, end - blendOpIdx );
					m_blendData.BlendOpId = blendOpParams;
					BlendData.BlendOpStartIndex = offsetIdx + blendOpIdx;
					idManager.RegisterId( m_blendData.BlendOpStartIndex, uniquePrefix + m_blendData.BlendOpId, m_blendData.BlendOpId );
					TemplateHelperFunctions.CreateBlendOp( blendOpParams, ref m_blendData );
					if( m_blendData.ValidBlendOp )
					{
						propertyContainer.AddId( subBody, blendOpParams, false );
					}
				}

				m_blendData.DataCheck = ( m_blendData.ValidBlendMode || m_blendData.ValidBlendOp ) ? TemplateDataCheck.Valid : TemplateDataCheck.Invalid;
			}
			//CULL MODE
			{
				Match cullMatch = Regex.Match( subBody, TemplateHelperFunctions.CullWholeWordPattern );
				if( cullMatch.Success )
				{
					int cullIdx = cullMatch.Index;
					int end = subBody.IndexOf( TemplatesManager.TemplateNewLine, cullIdx );
					string cullParams = subBody.Substring( cullIdx, end - cullIdx );
					m_cullModeData.CullModeId = cullParams;
					m_cullModeData.StartIdx = offsetIdx + cullIdx;
					idManager.RegisterId( m_cullModeData.StartIdx, uniquePrefix + m_cullModeData.CullModeId, m_cullModeData.CullModeId );
					TemplateHelperFunctions.CreateCullMode( cullParams, ref m_cullModeData );
					if( m_cullModeData.DataCheck == TemplateDataCheck.Valid )
						propertyContainer.AddId( subBody, cullParams, false, string.Empty );
					
				}
			}
			//COLOR MASK
			{
				Match colorMaskMatch = Regex.Match( subBody, TemplateHelperFunctions.ColorMaskWholeWordPattern );
				if( colorMaskMatch.Success )
				{
					int colorMaskIdx = colorMaskMatch.Index;
					int end = subBody.IndexOf( TemplatesManager.TemplateNewLine, colorMaskIdx );
					string colorMaskParams = subBody.Substring( colorMaskIdx, end - colorMaskIdx );
					m_colorMaskData.ColorMaskId = colorMaskParams;
					m_colorMaskData.StartIdx = offsetIdx + colorMaskIdx;
					idManager.RegisterId( m_colorMaskData.StartIdx, uniquePrefix + m_colorMaskData.ColorMaskId, m_colorMaskData.ColorMaskId );
					TemplateHelperFunctions.CreateColorMask( colorMaskParams, ref m_colorMaskData );
					if( m_colorMaskData.DataCheck == TemplateDataCheck.Valid )
						propertyContainer.AddId( subBody, colorMaskParams, false );
					
				}
			}
			//STENCIL
			{
				Match stencilMatch = Regex.Match( subBody, TemplateHelperFunctions.StencilWholeWordPattern );
				if( stencilMatch.Success )
				{
					int stencilIdx = stencilMatch.Index;
					int stencilEndIdx = subBody.IndexOf( "}", stencilIdx );
					if( stencilEndIdx > 0 )
					{
						string stencilParams = subBody.Substring( stencilIdx, stencilEndIdx + 1 - stencilIdx );
						m_stencilData.StencilBufferId = stencilParams;
						m_stencilData.StartIdx = offsetIdx + stencilIdx;
						idManager.RegisterId( m_stencilData.StartIdx, uniquePrefix + m_stencilData.StencilBufferId, m_stencilData.StencilBufferId );
						TemplateHelperFunctions.CreateStencilOps( stencilParams, ref m_stencilData );
						if( m_stencilData.DataCheck == TemplateDataCheck.Valid )
						{
							propertyContainer.AddId( subBody, stencilParams, true );
						}
					}
				}
				else
				{
					int stencilTagIdx = subBody.IndexOf( TemplatesManager.TemplateStencilTag );
					if( stencilTagIdx > -1 )
					{
						m_stencilData.SetIndependentDefault();
						m_stencilData.StencilBufferId = TemplatesManager.TemplateStencilTag;
						m_stencilData.StartIdx = offsetIdx + stencilTagIdx;
						idManager.RegisterId( m_stencilData.StartIdx, uniquePrefix + m_stencilData.StencilBufferId, m_stencilData.StencilBufferId );
						propertyContainer.AddId( subBody, m_stencilData.StencilBufferId, true );
					}
				}
			}
			//ZWRITE
			{
				Match zWriteMatch = Regex.Match( subBody, TemplateHelperFunctions.ZWriteWholeWordPattern );
				if( zWriteMatch.Success )
				{
					int zWriteOpIdx = zWriteMatch.Index;
					int zWriteEndIdx = subBody.IndexOf( TemplatesManager.TemplateNewLine, zWriteOpIdx );
					if( zWriteEndIdx > 0 )
					{
						m_depthData.ZWriteModeId = subBody.Substring( zWriteOpIdx, zWriteEndIdx + 1 - zWriteOpIdx );
						m_depthData.ZWriteStartIndex = offsetIdx + zWriteOpIdx;
						idManager.RegisterId( m_depthData.ZWriteStartIndex, uniquePrefix + m_depthData.ZWriteModeId, m_depthData.ZWriteModeId );
						TemplateHelperFunctions.CreateZWriteMode( m_depthData.ZWriteModeId, ref m_depthData );
						if( m_depthData.DataCheck == TemplateDataCheck.Valid )
						{
							propertyContainer.AddId( subBody, m_depthData.ZWriteModeId, true );
						}
					}
				}
			}

			//ZTEST
			{
				Match zTestMatch = Regex.Match( subBody, TemplateHelperFunctions.ZTestWholeWordPattern );
				if( zTestMatch.Success )
				{
					int zTestOpIdx = zTestMatch.Index;
					int zTestEndIdx = subBody.IndexOf( TemplatesManager.TemplateNewLine, zTestOpIdx );
					if( zTestEndIdx > 0 )
					{
						m_depthData.ZTestModeId = subBody.Substring( zTestOpIdx, zTestEndIdx + 1 - zTestOpIdx );
						m_depthData.ZTestStartIndex = offsetIdx + zTestOpIdx;
						idManager.RegisterId( m_depthData.ZTestStartIndex, uniquePrefix + m_depthData.ZTestModeId, m_depthData.ZTestModeId );
						TemplateHelperFunctions.CreateZTestMode( m_depthData.ZTestModeId, ref m_depthData );
						if( m_depthData.DataCheck == TemplateDataCheck.Valid )
						{
							propertyContainer.AddId( subBody, m_depthData.ZTestModeId, true );
						}
					}
				}
			}

			//ZOFFSET
			{
				Match zOffsetMatch = Regex.Match( subBody, TemplateHelperFunctions.ZOffsetWholeWordPattern );
				if( zOffsetMatch.Success )
				{
					int zOffsetIdx = zOffsetMatch.Index;
					int zOffsetEndIdx = subBody.IndexOf( TemplatesManager.TemplateNewLine, zOffsetIdx );
					if( zOffsetEndIdx > 0 )
					{
						m_depthData.OffsetId = subBody.Substring( zOffsetIdx, zOffsetEndIdx + 1 - zOffsetIdx );
						m_depthData.OffsetStartIndex = offsetIdx + zOffsetIdx;
						idManager.RegisterId( m_depthData.OffsetStartIndex, uniquePrefix + m_depthData.OffsetId, m_depthData.OffsetId );
						TemplateHelperFunctions.CreateZOffsetMode( m_depthData.OffsetId, ref m_depthData );
						if( m_depthData.DataCheck == TemplateDataCheck.Valid )
						{
							propertyContainer.AddId( subBody, m_depthData.OffsetId, true );
						}
					}
				}
			}
			//TAGS
			{
				Match tagsMatch = Regex.Match( subBody, TemplateHelperFunctions.TagsWholeWordPattern );
				if ( tagsMatch.Success )
				{
					int tagsIdx = tagsMatch.Index;
					int tagsEndIdx = subBody.IndexOf( "}", tagsIdx );
					if ( tagsEndIdx > -1 )
					{
						m_tagData.Reset();
						m_tagData.TagsId = subBody.Substring( tagsIdx, tagsEndIdx + 1 - tagsIdx );
						m_tagData.StartIdx = offsetIdx + tagsIdx;
						idManager.RegisterId( m_tagData.StartIdx, uniquePrefix + m_tagData.TagsId, m_tagData.TagsId );
						m_srpType = TemplateHelperFunctions.CreateTags( ref m_tagData, isSubShader );

						propertyContainer.AddId( subBody, m_tagData.TagsId, false );
						m_tagData.DataCheck = TemplateDataCheck.Valid;
					}
					else
					{
						m_tagData.DataCheck = TemplateDataCheck.Invalid;
					}
				}
				else
				{
					m_tagData.DataCheck = TemplateDataCheck.Invalid;
				}
			}

			//SHADER MODEL
			{
				Match match = Regex.Match( subBody, TemplateHelperFunctions.ShaderModelPattern );
				if ( match != null && match.Groups.Count > 1 )
				{
					if ( TemplateHelperFunctions.AvailableInterpolators.ContainsKey( match.Groups[ 1 ].Value ) )
					{
						m_shaderModel.Id = match.Groups[ 0 ].Value;
						m_shaderModel.StartIdx = offsetIdx + match.Index;
						m_shaderModel.Value = match.Groups[ 1 ].Value;
						m_shaderModel.InterpolatorAmount = TemplateHelperFunctions.AvailableInterpolators[ match.Groups[ 1 ].Value ];
						m_shaderModel.DataCheck = TemplateDataCheck.Valid;
						idManager.RegisterId( m_shaderModel.StartIdx, uniquePrefix + m_shaderModel.Id, m_shaderModel.Id );
					}
					else
					{
						m_shaderModel.DataCheck = TemplateDataCheck.Invalid;
					}
				}
			}

			// ALL MODULES
			int allModulesIndex = subBody.IndexOf( TemplatesManager.TemplateAllModulesTag );
			if( allModulesIndex > 0 )
			{
				//ONLY REGISTER MISSING TAGS
				ConfigureCommonTag( m_allModulesTag, propertyContainer, idManager, uniquePrefix, offsetIdx, subBody );
				m_allModulesMode = true;
				if( !m_blendData.IsValid )
					m_blendData.SetAllModulesDefault();

				if( !m_cullModeData.IsValid )
					m_cullModeData.SetAllModulesDefault();

				if( !m_colorMaskData.IsValid )
					m_colorMaskData.SetAllModulesDefault();

				if( !m_stencilData.IsValid )
					m_stencilData.SetAllModulesDefault();

				if( !m_depthData.IsValid )
					m_depthData.SetAllModulesDefault();

				if( !m_shaderModel.IsValid )
					m_shaderModel.SetAllModulesDefault();
			}
		}

		public bool HasValidData
		{
			get
			{
				return m_blendData.DataCheck == TemplateDataCheck.Valid ||
						m_cullModeData.DataCheck == TemplateDataCheck.Valid ||
						m_colorMaskData.DataCheck == TemplateDataCheck.Valid ||
						m_stencilData.DataCheck == TemplateDataCheck.Valid ||
						m_depthData.DataCheck == TemplateDataCheck.Valid ||
						m_tagData.DataCheck == TemplateDataCheck.Valid ||
						m_shaderModel.DataCheck == TemplateDataCheck.Valid ||
						m_globalsTag.IsValid ||
						m_allModulesTag.IsValid ||
						m_functionsTag.IsValid ||
						m_pragmaTag.IsValid ||
						m_passTag.IsValid ||
						m_inputsVertTag.IsValid ||
						m_inputsFragTag.IsValid;
			}
		}

		public TemplateBlendData BlendData { get { return m_blendData; } }
		public TemplateCullModeData CullModeData { get { return m_cullModeData; } }
		public TemplateColorMaskData ColorMaskData { get { return m_colorMaskData; } }
		public TemplateStencilData StencilData { get { return m_stencilData; } }
		public TemplateDepthData DepthData { get { return m_depthData; } }
		public TemplateTagsModuleData TagData { get { return m_tagData; } }
		public TemplateTagData GlobalsTag { get { return m_globalsTag; } }
		public TemplateTagData AllModulesTag { get { return m_allModulesTag; } }
		public TemplateTagData FunctionsTag { get { return m_functionsTag; } }
		public TemplateTagData PragmaTag { get { return m_pragmaTag; } }
		public TemplateTagData PassTag { get { return m_passTag; } }
		public TemplateTagData InputsVertTag { get { return m_inputsVertTag; } }
		public TemplateTagData InputsFragTag { get { return m_inputsFragTag; } }
		public TemplateShaderModelData ShaderModel { get { return m_shaderModel; } }
		public TemplateSRPType SRPType { get { return m_srpType; } set { m_srpType = value; } }
		public bool SRPIsPBR { get { return m_srpIsPBR; } set { m_srpIsPBR = value; } }
		public bool SRPIsPBRHD { get { return m_srpIsPBR && m_srpType == TemplateSRPType.HD; }  }
		public string UniquePrefix { get { return m_uniquePrefix; } }
		public TemplateIncludePragmaContainter IncludePragmaContainer { get { return m_includePragmaContainer; } }
		public bool AllModulesMode { get { return m_allModulesMode; } }
	}
}
