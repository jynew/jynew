// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

// THIS FILE IS DEPRECATED AND SHOULD NOT BE USED

using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class TemplateDataContainer
	{
		public int UNITY_VERSION = -1;
		public TemplateData TemplateDataRef;
	}

	[Serializable]
	public class VertexDataContainer
	{
		[SerializeField]
		private List<TemplateVertexData> m_vertexData;

		[SerializeField]
		private string m_vertexDataId = string.Empty;

		[SerializeField]
		private int m_vertexDataStartIdx = -1;

		public void Reload()
		{
			if( m_vertexData != null )
			{
				m_vertexData.Clear();
			}
		}

		public void Destroy()
		{
			if( m_vertexData != null )
			{
				m_vertexData.Clear();
				m_vertexData = null;
			}
		}


		public List<TemplateVertexData> VertexData { get { return m_vertexData; } set { m_vertexData = value; } }
		public string VertexDataId { get { return m_vertexDataId; } set { m_vertexDataId = value; } }
		public int VertexDataStartIdx { get { return m_vertexDataStartIdx; } set { m_vertexDataStartIdx = value; } }
	}

	[Serializable]
	public sealed class TemplateData : TemplateDataParent
	{
		[SerializeField]
		private string m_templateBody = string.Empty;

		[SerializeField]
		private string m_shaderNameId = string.Empty;

		[SerializeField]
		private List<TemplateProperty> m_propertyList = new List<TemplateProperty>();
		private Dictionary<string, TemplateProperty> m_propertyDict = new Dictionary<string, TemplateProperty>();

		[SerializeField]
		private List<TemplateInputData> m_inputDataList = new List<TemplateInputData>();
		private Dictionary<int, TemplateInputData> m_inputDataDict = new Dictionary<int, TemplateInputData>();

		//[SerializeField]
		//private List<TemplateCodeSnippetBase> m_snippetElementsList = new List<TemplateCodeSnippetBase>();
		//private Dictionary<string, TemplateCodeSnippetBase> m_snippetElementsDict = new Dictionary<string, TemplateCodeSnippetBase>();

		[SerializeField]
		private List<TemplateLocalVarData> m_localVarsList = new List<TemplateLocalVarData>();

		[SerializeField]
		private VertexDataContainer m_vertexDataContainer = new VertexDataContainer();

		[SerializeField]
		private TemplateInterpData m_interpolatorDataContainer;

		[SerializeField]
		private List<TemplateShaderPropertyData> m_availableShaderProperties = new List<TemplateShaderPropertyData>();

		[SerializeField]
		private TemplateFunctionData m_vertexFunctionData;

		[SerializeField]
		private TemplateFunctionData m_fragmentFunctionData;

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

		public TemplateData()
		{
			m_templateType = TemplateDataType.LegacySinglePass;
		}

		public TemplateData( string name )
		{
			m_templateType = TemplateDataType.LegacySinglePass;
			Name = name;
		}

		public TemplateData( string name, string guid )
		{
			m_templateType = TemplateDataType.LegacySinglePass;
			m_communityTemplate = false;
			if( !string.IsNullOrEmpty( guid ) )
			{
				string datapath = AssetDatabase.GUIDToAssetPath( guid );
				if( string.IsNullOrEmpty( datapath ) )
				{
					m_isValid = false;
					return;
				}

				string body = string.Empty;
				try
				{
					body = IOUtils.LoadTextFileFromDisk( datapath );
				}
				catch( Exception e )
				{
					Debug.LogException( e );
					m_isValid = false;
					return;
				}

				if( !string.IsNullOrEmpty( body ) )
				{
					LoadTemplateBody( body );
					Name = string.IsNullOrEmpty( name ) ? m_defaultShaderName : name;
					m_guid = guid;
				}
			}
		}

		public TemplateData( string name, string guid, string body )
		{
			m_templateType = TemplateDataType.LegacySinglePass;
			m_communityTemplate = true;
			if( !string.IsNullOrEmpty( body ) )
			{
				LoadTemplateBody( body );
				Name = string.IsNullOrEmpty( name ) ? m_defaultShaderName : name;
				m_guid = guid;
			}
		}

		public override bool Reload()
		{
			if( m_vertexDataContainer != null )
			{
				m_vertexDataContainer.Reload();
			}

			if( m_interpolatorDataContainer != null )
			{
				m_interpolatorDataContainer.Destroy();
			}

			if( m_availableShaderProperties != null )
			{
				m_availableShaderProperties.Clear();
			}

			if( m_propertyDict != null )
			{
				m_propertyDict.Clear();
			}

			if( m_propertyList != null )
			{
				m_propertyList.Clear();
			}

			if( m_inputDataDict != null )
			{
				m_inputDataDict.Clear();
			}

			if( m_inputDataList != null )
			{
				m_inputDataList.Clear();
			}

			if( m_localVarsList != null )
			{
				m_localVarsList.Clear();
			}

			//if( m_snippetElementsDict != null )
			//{
			//	m_snippetElementsDict.Clear();
			//}

			//if( m_snippetElementsList != null )
			//{
			//	for( int i = 0; i < m_snippetElementsList.Count; i++ )
			//	{
			//		GameObject.DestroyImmediate( m_snippetElementsList[ i ] );
			//		m_snippetElementsList[ i ] = null;
			//	}
			//	m_snippetElementsList.Clear();
			//}

			string datapath = AssetDatabase.GUIDToAssetPath( m_guid );
			string body = string.Empty;
			try
			{
				body = IOUtils.LoadTextFileFromDisk( datapath );
				body = body.Replace( "\r\n", "\n" );
			}
			catch( Exception e )
			{
				Debug.LogException( e );
				m_isValid = false;
			}
			LoadTemplateBody( body );
			if( m_communityTemplate )
			{
				Name = m_defaultShaderName;
			}
			return true;
		}

		void LoadTemplateBody( string body )
		{

			m_templateBody = body.Replace( "\r\n", "\n" ); ;

			if( m_templateBody.IndexOf( TemplatesManager.TemplateShaderNameBeginTag ) < 0 )
			{
				m_isValid = false;
				return;
			}

			//Fetching common tags
			FetchCommonTags();

			//Fetch function code areas
			FetchCodeAreas( TemplatesManager.TemplateVertexCodeBeginArea, MasterNodePortCategory.Vertex );
			FetchCodeAreas( TemplatesManager.TemplateFragmentCodeBeginArea, MasterNodePortCategory.Fragment );

			//Fetching inputs
			FetchInputs( MasterNodePortCategory.Fragment );
			FetchInputs( MasterNodePortCategory.Vertex );


			//Fetch local variables must be done after fetching code areas as it needs them to see is variable is on vertex or fragment
			TemplateHelperFunctions.FetchLocalVars( m_templateBody, ref m_localVarsList, m_vertexFunctionData, m_fragmentFunctionData );

			//Fetch snippets
		}

		void FetchSubShaderProperties()
		{
			Match match = Regex.Match( m_templateBody, @"Pass\s*{" );
			if( match.Groups.Count == 0 )
			{
				return;
			}

			int beginSubShader = m_templateBody.IndexOf( "SubShader" );
			int endSubShader = match.Groups[ 0 ].Index;
			if( beginSubShader > 0 && endSubShader > 0 && endSubShader > beginSubShader )
			{
				// ADD A PLACE TO INSERT GRAB PASSES
				int passIndex = m_templateBody.IndexOf( TemplatesManager.TemplatePassTag );
				if( passIndex < 0 )
				{
					int currIdx = endSubShader - 1;
					string identation = string.Empty;
					for( ; currIdx > 0; currIdx-- )
					{
						if( m_templateBody[ currIdx ] != '\n' )
						{
							identation = m_templateBody[ currIdx ] + identation;
						}
						else
						{
							identation = m_templateBody[ currIdx ] + identation;
							break;
						}
					}
					if( currIdx > 0 )
					{
						m_templateBody = m_templateBody.Insert( currIdx, identation + TemplatesManager.TemplatePassTag );
					}
				}

				// GET ALL THE MODULES
				string subBody = m_templateBody.Substring( beginSubShader, endSubShader - beginSubShader );
				//CULL MODE
				{
					int cullIdx = subBody.IndexOf( "Cull" );
					if( cullIdx > 0 )
					{
						int end = subBody.IndexOf( TemplatesManager.TemplateNewLine, cullIdx );
						string cullParams = subBody.Substring( cullIdx, end - cullIdx );
						m_cullModeData.CullModeId = cullParams;
						TemplateHelperFunctions.CreateCullMode( cullParams, ref m_cullModeData );
						if( m_cullModeData.DataCheck == TemplateDataCheck.Valid )
							AddId( cullParams, false, string.Empty );
					}
				}
				//COLOR MASK
				{
					int colorMaskIdx = subBody.IndexOf( "ColorMask" );
					if( colorMaskIdx > 0 )
					{
						int end = subBody.IndexOf( TemplatesManager.TemplateNewLine, colorMaskIdx );
						string colorMaskParams = subBody.Substring( colorMaskIdx, end - colorMaskIdx );
						m_colorMaskData.ColorMaskId = colorMaskParams;
						TemplateHelperFunctions.CreateColorMask( colorMaskParams, ref m_colorMaskData );
						if( m_colorMaskData.DataCheck == TemplateDataCheck.Valid )
							AddId( colorMaskParams, false );
					}
				}
				//BlEND MODE
				{
					int blendModeIdx = subBody.IndexOf( "Blend" );
					if( blendModeIdx > 0 )
					{
						int end = subBody.IndexOf( TemplatesManager.TemplateNewLine, blendModeIdx );
						string blendParams = subBody.Substring( blendModeIdx, end - blendModeIdx );
						m_blendData.BlendModeId = blendParams;
						TemplateHelperFunctions.CreateBlendMode( blendParams, ref m_blendData );
						if( m_blendData.ValidBlendMode )
						{
							AddId( blendParams, false );
						}
					}
				}
				//BLEND OP
				{
					int blendOpIdx = subBody.IndexOf( "BlendOp" );
					if( blendOpIdx > 0 )
					{
						int end = subBody.IndexOf( TemplatesManager.TemplateNewLine, blendOpIdx );
						string blendOpParams = subBody.Substring( blendOpIdx, end - blendOpIdx );
						BlendData.BlendOpId = blendOpParams;
						TemplateHelperFunctions.CreateBlendOp( blendOpParams, ref m_blendData );
						if( m_blendData.ValidBlendOp )
						{
							AddId( blendOpParams, false );
						}
					}

					m_blendData.DataCheck = ( m_blendData.ValidBlendMode || m_blendData.ValidBlendOp ) ? TemplateDataCheck.Valid : TemplateDataCheck.Invalid;
				}

				//STENCIL
				{
					int stencilIdx = subBody.IndexOf( "Stencil" );
					if( stencilIdx > -1 )
					{
						int stencilEndIdx = subBody.IndexOf( "}", stencilIdx );
						if( stencilEndIdx > 0 )
						{
							string stencilParams = subBody.Substring( stencilIdx, stencilEndIdx + 1 - stencilIdx );
							m_stencilData.StencilBufferId = stencilParams;
							TemplateHelperFunctions.CreateStencilOps( stencilParams, ref m_stencilData );
							if( m_stencilData.DataCheck == TemplateDataCheck.Valid )
							{
								AddId( stencilParams, true );
							}
						}
					}
				}

				//ZWRITE
				{
					int zWriteOpIdx = subBody.IndexOf( "ZWrite" );
					if( zWriteOpIdx > -1 )
					{
						int zWriteEndIdx = subBody.IndexOf( TemplatesManager.TemplateNewLine, zWriteOpIdx );
						if( zWriteEndIdx > 0 )
						{
							m_depthData.ZWriteModeId = subBody.Substring( zWriteOpIdx, zWriteEndIdx + 1 - zWriteOpIdx );
							TemplateHelperFunctions.CreateZWriteMode( m_depthData.ZWriteModeId, ref m_depthData );
							if( m_depthData.DataCheck == TemplateDataCheck.Valid )
							{
								AddId( m_depthData.ZWriteModeId, true );
							}
						}
					}
				}

				//ZTEST
				{
					int zTestOpIdx = subBody.IndexOf( "ZTest" );
					if( zTestOpIdx > -1 )
					{
						int zTestEndIdx = subBody.IndexOf( TemplatesManager.TemplateNewLine, zTestOpIdx );
						if( zTestEndIdx > 0 )
						{
							m_depthData.ZTestModeId = subBody.Substring( zTestOpIdx, zTestEndIdx + 1 - zTestOpIdx );
							TemplateHelperFunctions.CreateZTestMode( m_depthData.ZTestModeId, ref m_depthData );
							if( m_depthData.DataCheck == TemplateDataCheck.Valid )
							{
								AddId( m_depthData.ZTestModeId, true );
							}
						}
					}
				}

				//ZOFFSET
				{
					int zOffsetIdx = subBody.IndexOf( "Offset" );
					if( zOffsetIdx > -1 )
					{
						int zOffsetEndIdx = subBody.IndexOf( TemplatesManager.TemplateNewLine, zOffsetIdx );
						if( zOffsetEndIdx > 0 )
						{
							m_depthData.OffsetId = subBody.Substring( zOffsetIdx, zOffsetEndIdx + 1 - zOffsetIdx );
							TemplateHelperFunctions.CreateZOffsetMode( m_depthData.OffsetId, ref m_depthData );
							if( m_depthData.DataCheck == TemplateDataCheck.Valid )
							{
								AddId( m_depthData.OffsetId, true );
							}
						}
					}
				}

				//TAGS
				{
					int tagsIdx = subBody.IndexOf( "Tags" );
					if( tagsIdx > -1 )
					{
						int tagsEndIdx = subBody.IndexOf( "}", tagsIdx );
						if( tagsEndIdx > -1 )
						{
							m_tagData.Reset();
							m_tagData.TagsId = subBody.Substring( tagsIdx, tagsEndIdx + 1 - tagsIdx );
							TemplateHelperFunctions.CreateTags( ref m_tagData, true );
							m_tagData.DataCheck = TemplateDataCheck.Valid;
							AddId( m_tagData.TagsId, false );
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
			}
		}

		void FetchCommonTags()
		{
			// Name
			try
			{
				int nameBegin = m_templateBody.IndexOf( TemplatesManager.TemplateShaderNameBeginTag );
				if( nameBegin < 0 )
				{
					// Not a template
					return;
				}

				int nameEnd = m_templateBody.IndexOf( TemplatesManager.TemplateFullEndTag, nameBegin );
				int defaultBegin = nameBegin + TemplatesManager.TemplateShaderNameBeginTag.Length;
				int defaultLength = nameEnd - defaultBegin;
				m_defaultShaderName = m_templateBody.Substring( defaultBegin, defaultLength );
				int[] nameIdx = m_defaultShaderName.AllIndexesOf( "\"" );
				nameIdx[ 0 ] += 1; // Ignore the " character from the string
				m_defaultShaderName = m_defaultShaderName.Substring( nameIdx[ 0 ], nameIdx[ 1 ] - nameIdx[ 0 ] );
				m_shaderNameId = m_templateBody.Substring( nameBegin, nameEnd + TemplatesManager.TemplateFullEndTag.Length - nameBegin );
				AddId( m_shaderNameId, false );

			}
			catch( Exception e )
			{
				Debug.LogException( e );
				m_isValid = false;
			}

			FetchSubShaderProperties();
			// Vertex Data
			{
				int vertexDataTagBegin = m_templateBody.IndexOf( TemplatesManager.TemplateVertexDataTag );
				if( vertexDataTagBegin > -1 )
				{
					m_vertexDataContainer.VertexDataStartIdx = vertexDataTagBegin;
					int vertexDataTagEnd = m_templateBody.IndexOf( TemplatesManager.TemplateEndOfLine, vertexDataTagBegin );
					m_vertexDataContainer.VertexDataId = m_templateBody.Substring( vertexDataTagBegin, vertexDataTagEnd + TemplatesManager.TemplateEndOfLine.Length - vertexDataTagBegin );
					int dataBeginIdx = m_templateBody.LastIndexOf( '{', vertexDataTagBegin, vertexDataTagBegin );
					string vertexData = m_templateBody.Substring( dataBeginIdx + 1, vertexDataTagBegin - dataBeginIdx );

					int parametersBegin = vertexDataTagBegin + TemplatesManager.TemplateVertexDataTag.Length;
					string parameters = m_templateBody.Substring( parametersBegin, vertexDataTagEnd - parametersBegin );
					m_vertexDataContainer.VertexData = TemplateHelperFunctions.CreateVertexDataList( vertexData, parameters );
					AddId( m_vertexDataContainer.VertexDataId );
				}
			}

			// Available interpolators
			try
			{
				int interpDataBegin = m_templateBody.IndexOf( TemplatesManager.TemplateInterpolatorBeginTag );
				if( interpDataBegin > -1 )
				{
					int interpDataEnd = m_templateBody.IndexOf( TemplatesManager.TemplateEndOfLine, interpDataBegin );
					string interpDataId = m_templateBody.Substring( interpDataBegin, interpDataEnd + TemplatesManager.TemplateEndOfLine.Length - interpDataBegin );

					int dataBeginIdx = m_templateBody.LastIndexOf( '{', interpDataBegin, interpDataBegin );
					string interpData = m_templateBody.Substring( dataBeginIdx + 1, interpDataBegin - dataBeginIdx );

					m_interpolatorDataContainer = TemplateHelperFunctions.CreateInterpDataList( interpData, interpDataId, 8 );
					m_interpolatorDataContainer.InterpDataId = interpDataId;
					m_interpolatorDataContainer.InterpDataStartIdx = interpDataBegin;
					AddId( interpDataId );
				}
			}
			catch( Exception e )
			{
				Debug.LogException( e );
				m_isValid = false;
			}


			try
			{
				Dictionary<string, TemplateShaderPropertyData> duplicatesHelper = new Dictionary<string, TemplateShaderPropertyData>();
				m_availableShaderProperties = new List<TemplateShaderPropertyData>();

				// Common Tags
				for( int i = 0; i < TemplatesManager.CommonTags.Length; i++ )
				{
					int idx = m_templateBody.IndexOf( TemplatesManager.CommonTags[ i ].Id );
					if( idx > -1 )
					{
						string currentId = TemplatesManager.CommonTags[ i ].Id;

						TemplateCommonTagId commonTagId = (TemplateCommonTagId)i;
						switch( commonTagId )
						{
							// Properties
							case TemplateCommonTagId.Property:
							{
								TemplateHelperFunctions.CreateShaderPropertiesList( m_templateBody.Substring( 0, idx + TemplatesManager.CommonTags[ i ].Id.Length ), ref m_availableShaderProperties, ref duplicatesHelper );
							}
							break;
							// Globals
							case TemplateCommonTagId.Global:
							{
								TemplateHelperFunctions.CreateShaderGlobalsList( m_templateBody.Substring( 0, idx + TemplatesManager.CommonTags[ i ].Id.Length ), ref m_availableShaderProperties, ref duplicatesHelper );
							}
							break;

							//Tags
							//case TemplateCommonTagId.Tag:
							//{
							//	m_propertyList[ m_propertyList.Count - 1 ].Indentation = " ";
							//}
							//break;
							//case TemplateCommonTagId.CullMode:
							//{
							//	int newId = idx + TemplatesManager.CommonTags[ i ].Id.Length;
							//	int end = m_templateBody.IndexOf( TemplatesManager.TemplateNewLine, newId );
							//	string cullParams = m_templateBody.Substring( newId, end - newId );
							//	currentId = m_templateBody.Substring( idx, end - idx );
							//	m_cullModeData.CullModeId = currentId;
							//	TemplateHelperFunctions.CreateCullMode( cullParams, ref m_cullModeData );
							//}
							//break;
							//Blend Mode
							//case TemplateCommonTagId.BlendMode:
							//{
							//	int newId = idx + TemplatesManager.CommonTags[ i ].Id.Length;
							//	int end = m_templateBody.IndexOf( TemplatesManager.TemplateNewLine, newId );
							//	string blendParams = m_templateBody.Substring( newId, end - newId );
							//	currentId = m_templateBody.Substring( idx, end - idx );
							//	m_blendData.BlendModeId = currentId;
							//	TemplateHelperFunctions.CreateBlendMode( blendParams, ref m_blendData );
							//}break;
							//case TemplateCommonTagId.BlendOp:
							//{
							//	int newId = idx + TemplatesManager.CommonTags[ i ].Id.Length;
							//	int end = m_templateBody.IndexOf( TemplatesManager.TemplateNewLine, newId );
							//	currentId = m_templateBody.Substring( idx, end - idx );
							//	BlendData.BlendOpId = currentId;
							//	TemplateHelperFunctions.CreateBlendOp( m_templateBody.Substring( newId, end - newId ), ref m_blendData );
							//}break;
							//case TemplateCommonTagId.ColorMask:
							//{
							//	int newId = idx + TemplatesManager.CommonTags[ i ].Id.Length;
							//	int end = m_templateBody.IndexOf( TemplatesManager.TemplateNewLine, newId );
							//	string colorMaskParams = m_templateBody.Substring( newId, end - newId );
							//	currentId = m_templateBody.Substring( idx, end - idx );
							//	m_colorMaskData.ColorMaskId = currentId;
							//	TemplateHelperFunctions.CreateColorMask( colorMaskParams, ref m_colorMaskData );
							//}
							//break;
							//case TemplateCommonTagId.StencilOp:
							//{
							//    int id = m_templateBody.LastIndexOf( "Stencil" );
							//    if( id > -1 )
							//    {
							//        string stencilParams = m_templateBody.Substring( id, idx - id );
							//        currentId = stencilParams + TemplatesManager.TemplateStencilOpTag;
							//        m_stencilData.StencilBufferId = currentId;
							//        TemplateHelperFunctions.CreateStencilOps( stencilParams, ref m_stencilData );
							//    }

							//}
							//break;
							default:
							break;
						}

						//AddId( TemplatesManager.CommonTags[ i ] );
						AddId( currentId, TemplatesManager.CommonTags[ i ].SearchIndentation, TemplatesManager.CommonTags[ i ].CustomIndentation );
					}
				}

				duplicatesHelper.Clear();
				duplicatesHelper = null;
			}
			catch( Exception e )
			{
				Debug.LogException( e );
				m_isValid = false;
			}
		}

		void FetchCodeAreas( string begin, MasterNodePortCategory category )
		{
			int areaBeginIndexes = m_templateBody.IndexOf( begin );

			if( areaBeginIndexes > -1 )
			{
				int beginIdx = areaBeginIndexes + begin.Length;
				int endIdx = m_templateBody.IndexOf( TemplatesManager.TemplateEndOfLine, beginIdx );
				int length = endIdx - beginIdx;

				string parameters = m_templateBody.Substring( beginIdx, length );

				string[] parametersArr = parameters.Split( IOUtils.FIELD_SEPARATOR );

				string id = m_templateBody.Substring( areaBeginIndexes, endIdx + TemplatesManager.TemplateEndOfLine.Length - areaBeginIndexes );
				string inParameters = parametersArr[ 0 ];
				string outParameters = ( parametersArr.Length > 1 ) ? parametersArr[ 1 ] : string.Empty;
				if( category == MasterNodePortCategory.Fragment )
				{
					m_fragmentFunctionData = new TemplateFunctionData(-1, string.Empty, id, areaBeginIndexes, inParameters, outParameters, category );
				}
				else
				{
					m_vertexFunctionData = new TemplateFunctionData( -1, string.Empty,id, areaBeginIndexes, inParameters, outParameters, category );
				}
				AddId( id, true );
			}
		}

		void FetchInputs( MasterNodePortCategory portCategory )
		{
			string beginTag = ( portCategory == MasterNodePortCategory.Fragment ) ? TemplatesManager.TemplateInputsFragBeginTag : TemplatesManager.TemplateInputsVertBeginTag;
			int[] inputBeginIndexes = m_templateBody.AllIndexesOf( beginTag );
			if( inputBeginIndexes != null && inputBeginIndexes.Length > 0 )
			{
				for( int i = 0; i < inputBeginIndexes.Length; i++ )
				{
					int inputEndIdx = m_templateBody.IndexOf( TemplatesManager.TemplateEndSectionTag, inputBeginIndexes[ i ] );
					int defaultValueBeginIdx = inputEndIdx + TemplatesManager.TemplateEndSectionTag.Length;
					int endLineIdx = m_templateBody.IndexOf( TemplatesManager.TemplateFullEndTag, defaultValueBeginIdx );

					string defaultValue = m_templateBody.Substring( defaultValueBeginIdx, endLineIdx - defaultValueBeginIdx );
					string tagId = m_templateBody.Substring( inputBeginIndexes[ i ], endLineIdx + TemplatesManager.TemplateFullEndTag.Length - inputBeginIndexes[ i ] );

					int beginIndex = inputBeginIndexes[ i ] + beginTag.Length;
					int length = inputEndIdx - beginIndex;
					string inputData = m_templateBody.Substring( beginIndex, length );
					string[] inputDataArray = inputData.Split( IOUtils.FIELD_SEPARATOR );
					if( inputDataArray != null && inputDataArray.Length > 0 )
					{
						try
						{
							string portName = inputDataArray[ (int)TemplatePortIds.Name ];
							WirePortDataType dataType = (WirePortDataType)Enum.Parse( typeof( WirePortDataType ), inputDataArray[ (int)TemplatePortIds.DataType ].ToUpper() );

							int portUniqueIDArrIdx = (int)TemplatePortIds.UniqueId;
							int portUniqueId = ( portUniqueIDArrIdx < inputDataArray.Length ) ? Convert.ToInt32( inputDataArray[ portUniqueIDArrIdx ] ) : -1;
							if( portUniqueId < 0 )
								portUniqueId = m_inputDataList.Count;

							int portOrderArrayIdx = (int)TemplatePortIds.OrderId;
							int portOrderId = ( portOrderArrayIdx < inputDataArray.Length ) ? Convert.ToInt32( inputDataArray[ portOrderArrayIdx ] ) : -1;
							if( portOrderId < 0 )
								portOrderId = m_inputDataList.Count;

							AddInput( inputBeginIndexes[ i ], tagId, portName, defaultValue, dataType, portCategory, portUniqueId, portOrderId );
						}
						catch( Exception e )
						{
							Debug.LogException( e );
						}
					}
				}
			}
		}

		//void FetchSnippets()
		//{
		//	int[] codeSnippetAttribBeginIndexes = m_templateBody.AllIndexesOf( TemplatesManager.TemplateCodeSnippetAttribBegin );
		//	int[] codeSnippetAttribEndIndexes = m_templateBody.AllIndexesOf( TemplatesManager.TemplateCodeSnippetAttribEnd );
		//	int[] codeSnippetEndIndexes = m_templateBody.AllIndexesOf( TemplatesManager.TemplateCodeSnippetEnd );

		//	if( codeSnippetAttribBeginIndexes != null && codeSnippetAttribBeginIndexes.Length > 0 &&
		//			codeSnippetAttribEndIndexes != null && codeSnippetAttribEndIndexes.Length > 0 &&
		//			codeSnippetEndIndexes != null && codeSnippetEndIndexes.Length > 0 &&
		//			codeSnippetEndIndexes.Length == codeSnippetAttribBeginIndexes.Length &&
		//			codeSnippetAttribBeginIndexes.Length == codeSnippetAttribEndIndexes.Length )
		//	{
		//		for( int i = 0; i < codeSnippetAttribBeginIndexes.Length; i++ )
		//		{
		//			// get attributes
		//			int startAttribIndex = codeSnippetAttribBeginIndexes[ i ] + TemplatesManager.TemplateCodeSnippetAttribBegin.Length;
		//			int lengthAttrib = codeSnippetAttribEndIndexes[ i ] - startAttribIndex;
		//			string snippetAttribs = m_templateBody.Substring( startAttribIndex, lengthAttrib );
		//			string[] snippetAttribsArr = snippetAttribs.Split( IOUtils.FIELD_SEPARATOR );
		//			if( snippetAttribsArr != null && snippetAttribsArr.Length > 0 )
		//			{
		//				string attribName = snippetAttribsArr[ (int)TemplateCodeSnippetInfoIdx.Name ];
		//				TemplateCodeSnippetType attribType = (TemplateCodeSnippetType)Enum.Parse( typeof( TemplateCodeSnippetType ), snippetAttribsArr[ (int)TemplateCodeSnippetInfoIdx.Type ] );
		//				if( m_snippetElementsDict.ContainsKey( attribName ) )
		//				{
		//					if( m_snippetElementsDict[ attribName ].Type != attribType )
		//					{
		//						if( DebugConsoleWindow.DeveloperMode )
		//							Debug.LogWarning( "Found incompatible types for snippet " + attribName );
		//					}
		//				}
		//				else
		//				{
		//					switch( attribType )
		//					{
		//						case TemplateCodeSnippetType.Toggle:
		//						{
		//							//Register must be done by first instantiang the correct type and register it on both containers
		//							//Overrides don't work if we use the container reference into the other
		//							TemplateCodeSnippetToggle newSnippet = ScriptableObject.CreateInstance<TemplateCodeSnippetToggle>();
		//							newSnippet.Init( attribName, attribType );
		//							m_snippetElementsDict.Add( attribName, newSnippet );
		//							m_snippetElementsList.Add( newSnippet );
		//						}
		//						break;
		//					}

		//				}
		//				// Add initial tag indentation
		//				int indentationIndex = codeSnippetAttribBeginIndexes[ i ];
		//				int lengthAdjust = 0;
		//				for( ; indentationIndex > 0; indentationIndex--, lengthAdjust++ )
		//				{
		//					if( m_templateBody[ indentationIndex ] == TemplatesManager.TemplateNewLine )
		//					{
		//						indentationIndex += 1;
		//						lengthAdjust -= 1;
		//						break;
		//					}
		//				}

		//				if( indentationIndex > 0 )
		//				{
		//					string snippetId = m_templateBody.Substring( indentationIndex,
		//																 codeSnippetEndIndexes[ i ] + TemplatesManager.TemplateCodeSnippetEnd.Length - codeSnippetAttribBeginIndexes[ i ] + lengthAdjust );

		//					int snippetCodeStart = codeSnippetAttribEndIndexes[ i ] + TemplatesManager.TemplateCodeSnippetAttribEnd.Length;
		//					int snippetCodeLength = codeSnippetEndIndexes[ i ] - snippetCodeStart;
		//					//Remove possible identation characters present between tag and last instruction
		//					if( m_templateBody[ snippetCodeStart + snippetCodeLength - 1 ] != TemplatesManager.TemplateNewLine )
		//					{
		//						for( ; snippetCodeLength > 0; snippetCodeLength-- )
		//						{
		//							if( m_templateBody[ snippetCodeStart + snippetCodeLength - 1 ] == TemplatesManager.TemplateNewLine )
		//								break;
		//						}
		//					}

		//					if( snippetCodeLength > 0 )
		//					{
		//						string snippetCode = m_templateBody.Substring( snippetCodeStart, snippetCodeLength );
		//						TemplateCodeSnippetElement element = new TemplateCodeSnippetElement( snippetId, snippetCode );
		//						m_snippetElementsDict[ attribName ].AddSnippet( element );
		//					}
		//				}
		//			}
		//		}
		//	}
		//}

		//void RefreshSnippetInfo()
		//{
		//	if( m_snippetElementsDict == null )
		//	{
		//		m_snippetElementsDict = new Dictionary<string, TemplateCodeSnippetBase>();
		//	}

		//	if( m_snippetElementsDict.Count != m_snippetElementsList.Count )
		//	{
		//		m_snippetElementsDict.Clear();
		//		for( int i = 0; i < m_snippetElementsList.Count; i++ )
		//		{
		//			m_snippetElementsDict.Add( m_snippetElementsList[ i ].NameId, m_snippetElementsList[ i ] );
		//		}
		//	}
		//}

		//public void DrawSnippetProperties( ParentNode owner )
		//{
		//	for( int i = 0; i < m_snippetElementsList.Count; i++ )
		//	{
		//		m_snippetElementsList[ i ].DrawProperties( owner );
		//	}
		//}

		//public void InsertSnippets( ref string shaderBody )
		//{
		//	for( int i = 0; i < m_snippetElementsList.Count; i++ )
		//	{
		//		m_snippetElementsList[ i ].InsertSnippet( ref shaderBody );
		//	}
		//}

		public void AddId( string ID, bool searchIndentation = true )
		{
			AddId( ID, searchIndentation, string.Empty );
		}

		public void AddId( string ID, bool searchIndentation, string customIndentation )
		{
			int propertyIndex = m_templateBody.IndexOf( ID );
			if( propertyIndex > -1 )
			{
				if( searchIndentation )
				{
					int indentationIndex = -1;
					for( int i = propertyIndex; i > 0; i-- )
					{
						if( m_templateBody[ i ] == TemplatesManager.TemplateNewLine )
						{
							indentationIndex = i + 1;
							break;
						}
					}
					if( indentationIndex > -1 )
					{
						int length = propertyIndex - indentationIndex;
						string indentation = ( length > 0 ) ? m_templateBody.Substring( indentationIndex, length ) : string.Empty;
						m_propertyList.Add( new TemplateProperty( ID, indentation, false ) );
					}
				}
				else
				{
					m_propertyList.Add( new TemplateProperty( ID, customIndentation, true ) );
				}
			}
		}

		void BuildInfo()
		{
			if( m_propertyDict == null )
			{
				m_propertyDict = new Dictionary<string, TemplateProperty>();
			}

			if( m_propertyList.Count != m_propertyDict.Count )
			{
				m_propertyDict.Clear();
				for( int i = 0; i < m_propertyList.Count; i++ )
				{
					m_propertyDict.Add( m_propertyList[ i ].Id, m_propertyList[ i ] );
				}
			}
		}

		public void ResetTemplateUsageData()
		{
			BuildInfo();
			for( int i = 0; i < m_propertyList.Count; i++ )
			{
				m_propertyList[ i ].Used = false;
			}
		}

		public void AddInput( int tagStartIdx, string tagId, string portName, string defaultValue, WirePortDataType dataType, MasterNodePortCategory portCategory, int portUniqueId, int portOrderId )
		{
			TemplateInputData inputData = new TemplateInputData( tagStartIdx, tagStartIdx, tagId, portName, defaultValue, dataType, portCategory, portUniqueId, portOrderId, string.Empty );
			m_inputDataList.Add( inputData );
			m_inputDataDict.Add( inputData.PortUniqueId, inputData );
			AddId( tagId, false );
		}

		public override void Destroy()
		{
			if( m_vertexDataContainer != null )
			{
				m_vertexDataContainer.Destroy();
				m_vertexDataContainer = null;
			}

			if( m_interpolatorDataContainer != null )
			{
				m_interpolatorDataContainer.Destroy();
				m_interpolatorDataContainer = null;
			}

			if( m_availableShaderProperties != null )
			{
				m_availableShaderProperties.Clear();
				m_availableShaderProperties = null;
			}

			if( m_propertyDict != null )
			{
				m_propertyDict.Clear();
				m_propertyDict = null;
			}

			if( m_propertyList != null )
			{
				m_propertyList.Clear();
				m_propertyList = null;
			}

			if( m_inputDataDict != null )
			{
				m_inputDataDict.Clear();
				m_inputDataDict = null;
			}

			if( m_inputDataList != null )
			{
				m_inputDataList.Clear();
				m_inputDataList = null;
			}

			if( m_localVarsList != null )
			{
				m_localVarsList.Clear();
				m_localVarsList = null;
			}
			//if( m_snippetElementsDict != null )
			//{
			//	m_snippetElementsDict.Clear();
			//	m_snippetElementsDict = null;
			//}

			//if( m_snippetElementsList != null )
			//{
			//	for( int i = 0; i < m_snippetElementsList.Count; i++ )
			//	{
			//		GameObject.DestroyImmediate( m_snippetElementsList[ i ] );
			//		m_snippetElementsList[ i ] = null;
			//	}
			//	m_snippetElementsList.Clear();
			//	m_snippetElementsList = null;
			//}

			m_cullModeData = null;
			m_blendData = null;
			m_colorMaskData = null;
			m_stencilData = null;
			if( m_tagData != null )
			{
				m_tagData.Destroy();
				m_tagData = null;
			}
		}

		public void FillEmptyTags( ref string body )
		{
			body = body.Replace( TemplatesManager.TemplateLocalVarTag, string.Empty );
			for( int i = 0; i < m_propertyList.Count; i++ )
			{
				if( !m_propertyList[ i ].Used )
				{
					if( m_propertyList[ i ].UseCustomIndentation )
					{
						body = body.Replace( m_propertyList[ i ].Id, string.Empty );
					}
					else
					{
						body = body.Replace( m_propertyList[ i ].Indentation + m_propertyList[ i ].Id, string.Empty );
					}
				}
			}
		}

		public bool FillVertexInstructions( ref string body, params string[] values )
		{
			if( m_vertexFunctionData != null && !string.IsNullOrEmpty( m_vertexFunctionData.Id ) )
			{
				return FillTemplateBody( m_vertexFunctionData.Id, ref body, values );
			}

			if( values.Length > 0 )
			{
				UIUtils.ShowMessage( "Attemping to add vertex instructions on a template with no assigned vertex code area", MessageSeverity.Error );
				return false;
			}
			return true;
		}

		public bool FillFragmentInstructions( ref string body, params string[] values )
		{
			if( m_fragmentFunctionData != null && !string.IsNullOrEmpty( m_fragmentFunctionData.Id ) )
			{
				return FillTemplateBody( m_fragmentFunctionData.Id, ref body, values );
			}

			if( values.Length > 0 )
			{
				UIUtils.ShowMessage( "Attemping to add fragment instructions on a template with no assigned vertex code area", MessageSeverity.Error );
				return false;
			}
			return true;
		}

		// values must be unindented an without line feed
		public bool FillTemplateBody( string id, ref string body, params string[] values )
		{
			if( values.Length == 0 )
			{
				return true;
			}

			BuildInfo();

			if( m_propertyDict.ContainsKey( id ) )
			{
				string finalValue = string.Empty;
				for( int i = 0; i < values.Length; i++ )
				{

					if( m_propertyDict[ id ].AutoLineFeed )
					{
						string[] valuesArr = values[ i ].Split( '\n' );
						for( int j = 0; j < valuesArr.Length; j++ )
						{
							//first value will be automatically indented by the string replace
							finalValue += ( ( i == 0 && j == 0 ) ? string.Empty : m_propertyDict[ id ].Indentation ) + valuesArr[ j ];
							finalValue += TemplatesManager.TemplateNewLine;
						}

					}
					else
					{
						//first value will be automatically indented by the string replace
						finalValue += ( i == 0 ? string.Empty : m_propertyDict[ id ].Indentation ) + values[ i ];
					}
				}

				body = body.Replace( id, finalValue );
				m_propertyDict[ id ].Used = true;
				return true;
			}

			if( values.Length > 1 || !string.IsNullOrEmpty( values[ 0 ] ) )
			{
				UIUtils.ShowMessage( string.Format( "Attempting to write data into inexistant tag {0}. Please review the template {1} body and consider adding the missing tag.", id, m_name ), MessageSeverity.Error );
				return false;
			}

			return true;

		}

		public bool FillTemplateBody( string id, ref string body, List<PropertyDataCollector> values )
		{
			if( values.Count == 0 )
			{
				return true;
			}

			string[] array = new string[ values.Count ];
			for( int i = 0; i < values.Count; i++ )
			{
				array[ i ] = values[ i ].PropertyName;
			}
			return FillTemplateBody( id, ref body, array );
		}

		public TemplateInputData InputDataFromId( int id )
		{
			if( m_inputDataDict == null )
				m_inputDataDict = new Dictionary<int, TemplateInputData>();

			if( m_inputDataDict.Count != m_inputDataList.Count )
			{
				m_inputDataDict.Clear();
				for( int i = 0; i < m_inputDataList.Count; i++ )
				{
					m_inputDataDict.Add( m_inputDataList[ i ].PortUniqueId, m_inputDataList[ i ] );
				}
			}

			if( m_inputDataDict.ContainsKey( id ) )
				return m_inputDataDict[ id ];

			return null;
		}

		public string GetVertexData( TemplateInfoOnSematics info )
		{
			int count = m_vertexDataContainer.VertexData.Count;
			for( int i = 0; i < count; i++ )
			{
				if( m_vertexDataContainer.VertexData[ i ].DataInfo == info )
				{
					return string.Format( TemplateHelperFunctions.TemplateVarFormat, m_vertexFunctionData.InVarName, m_vertexDataContainer.VertexData[ i ].VarName );
				}
			}
			return string.Empty;
		}

		public string GetInterpolatedData( TemplateInfoOnSematics info )
		{
			int count = m_interpolatorDataContainer.Interpolators.Count;
			for( int i = 0; i < count; i++ )
			{
				if( m_interpolatorDataContainer.Interpolators[ i ].DataInfo == info )
				{
					return string.Format( TemplateHelperFunctions.TemplateVarFormat, m_fragmentFunctionData.InVarName, m_interpolatorDataContainer.Interpolators[ i ].VarName );
				}
			}
			return string.Empty;
		}

		public string InterpDataId { get { return m_interpolatorDataContainer.InterpDataId; } }
		public string VertexDataId { get { return m_vertexDataContainer.VertexDataId; } }
		public string ShaderNameId { get { return m_shaderNameId; } set { m_shaderNameId = value; } }
		public string TemplateBody { get { return m_templateBody; } set { m_templateBody = value; } }
		public List<TemplateInputData> InputDataList { get { return m_inputDataList; } set { m_inputDataList = value; } }
		public List<TemplateLocalVarData> LocalVarsList { get { return m_localVarsList; } }
		public List<TemplateVertexData> VertexDataList { get { return m_vertexDataContainer.VertexData; } }
		public TemplateInterpData InterpolatorData { get { return m_interpolatorDataContainer; } }
		public TemplateFunctionData VertexFunctionData { get { return m_vertexFunctionData; } set { m_vertexFunctionData = value; } }
		public TemplateFunctionData FragmentFunctionData { get { return m_fragmentFunctionData; } set { m_fragmentFunctionData = value; } }
		public TemplateFunctionData FragFunctionData { get { return m_fragmentFunctionData; } set { m_fragmentFunctionData = value; } }
		public List<TemplateShaderPropertyData> AvailableShaderProperties { get { return m_availableShaderProperties; } set { m_availableShaderProperties = value; } }
		public TemplateBlendData BlendData { get { return m_blendData; } set { m_blendData = value; } }
		public TemplateCullModeData CullModeData { get { return m_cullModeData; } set { m_cullModeData = value; } }
		public TemplateColorMaskData ColorMaskData { get { return m_colorMaskData; } set { m_colorMaskData = value; } }
		public TemplateStencilData StencilData { get { return m_stencilData; } set { m_stencilData = value; } }
		public TemplateDepthData DepthData { get { return m_depthData; } set { m_depthData = value; } }
		public TemplateTagsModuleData TagData { get { return m_tagData; } set { m_tagData = value; } }
		private List<TemplateProperty> PropertyList { get { return m_propertyList; } set { m_propertyList = value; } }
		public VertexDataContainer VertexDataContainer { get { return m_vertexDataContainer; } set { m_vertexDataContainer = value; } }
		public TemplateInterpData InterpolatorDataContainer { get { return m_interpolatorDataContainer; } set { m_interpolatorDataContainer = value; } }
	}
}
