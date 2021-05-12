// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
#define CUSTOM_OPTIONS_AVAILABLE
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AmplifyShaderEditor
{

	[Serializable]
	public class TemplatePass
	{
		private const string DefaultPassNameStr = "SubShader {0} Pass {1}";

		[SerializeField]
		private int m_idx = -1;

		[SerializeField]
		private bool m_isInvisible = false;

		[SerializeField]
		private int m_invisibleOptions = 0;

		[SerializeField]
		private bool m_isMainPass = false;

		[SerializeField]
		private TemplateModulesData m_modules;

		[SerializeField]
		private List<TemplateInputData> m_inputDataList = new List<TemplateInputData>();
		private Dictionary<int, TemplateInputData> m_inputDataDict = new Dictionary<int, TemplateInputData>();

		[SerializeField]
		private TemplateFunctionData m_vertexFunctionData;

		[SerializeField]
		private TemplateFunctionData m_fragmentFunctionData;

		[SerializeField]
		private VertexDataContainer m_vertexDataContainer;

		[SerializeField]
		private TemplateInterpData m_interpolatorDataContainer;

		[SerializeField]
		private List<TemplateLocalVarData> m_localVarsList = new List<TemplateLocalVarData>();

		[SerializeField]
		private string m_uniquePrefix;

		[SerializeField]
		private TemplatePropertyContainer m_templateProperties = new TemplatePropertyContainer();

		[SerializeField]
		private List<TemplateShaderPropertyData> m_availableShaderGlobals = new List<TemplateShaderPropertyData>();

		[SerializeField]
		TemplateInfoContainer m_passNameContainer = new TemplateInfoContainer();
#if CUSTOM_OPTIONS_AVAILABLE
		[SerializeField]
		TemplateOptionsContainer m_customOptionsContainer = new TemplateOptionsContainer();
#endif
		public TemplatePass( TemplateModulesData subShaderModule, int subshaderIdx, int passIdx, TemplateIdManager idManager, string uniquePrefix, int offsetIdx, TemplatePassInfo passInfo, ref Dictionary<string, TemplateShaderPropertyData> duplicatesHelper )
		{
			m_idx = passIdx;

			m_uniquePrefix = uniquePrefix;

			m_isMainPass = passInfo.Data.Contains( TemplatesManager.TemplateMainPassTag );
			if( !m_isMainPass )
			{
				string id = string.Empty;
				int idIndex = 0;
				m_isInvisible = TemplateHelperFunctions.FetchInvisibleInfo( passInfo.Data, ref m_invisibleOptions, ref id, ref idIndex );
				if( m_isInvisible )
				{
					idManager.RegisterId( idIndex, uniquePrefix + id, id, true );
				}
			}
#if CUSTOM_OPTIONS_AVAILABLE
			m_customOptionsContainer = TemplateOptionsToolsHelper.GenerateOptionsContainer( passInfo.Data );
			if( m_customOptionsContainer.Enabled )
			{
				idManager.RegisterId( m_customOptionsContainer.Index, uniquePrefix + m_customOptionsContainer.Body, m_customOptionsContainer.Body, true );
			}
#endif
			FetchPassName( offsetIdx, passInfo.Data );
			if( m_passNameContainer.Index > -1 )
			{
				idManager.RegisterId( m_passNameContainer.Index, uniquePrefix + m_passNameContainer.Id, m_passNameContainer.Id );
			}
			else
			{
				m_passNameContainer.Data = string.Format( DefaultPassNameStr, subshaderIdx, passIdx );
			}

			m_modules = new TemplateModulesData( idManager, m_templateProperties, uniquePrefix + "Module", offsetIdx, passInfo.Data, false );

			if( !m_modules.PassTag.IsValid )
			{
				m_modules.PassTag.StartIdx = passInfo.GlobalStartIdx;
				m_templateProperties.AddId( passInfo.Data, m_modules.PassTag.Id, passInfo.LocalStartIdx, false );
				//m_modules.PassTag.StartIdx -= m_templateProperties.PropertyDict[ m_modules.PassTag.Id ].Indentation.Length;
				//m_templateProperties.PropertyDict[ m_modules.PassTag.Id ].UseIndentationAtStart = false;
				idManager.RegisterId( m_modules.PassTag.StartIdx, m_modules.UniquePrefix + m_modules.PassTag.Id, string.Empty );
			}

			m_modules.SRPType = subShaderModule.SRPType;
			if( m_modules.SRPType == TemplateSRPType.HD )
			{
				m_modules.SRPIsPBR = passInfo.Data.Contains( TemplateHelperFunctions.HDPBRTag );

			}

			Dictionary<string, TemplateShaderPropertyData> ownDuplicatesDict = new Dictionary<string, TemplateShaderPropertyData>( duplicatesHelper );
			TemplateHelperFunctions.CreateShaderGlobalsList( passInfo.Data, ref m_availableShaderGlobals, ref ownDuplicatesDict );

			// Vertex and Interpolator data
			FetchVertexAndInterpData( subShaderModule, offsetIdx, passInfo.Data );
			if( m_vertexDataContainer != null )
				idManager.RegisterId( m_vertexDataContainer.VertexDataStartIdx, uniquePrefix + m_vertexDataContainer.VertexDataId, m_vertexDataContainer.VertexDataId );

			if( m_interpolatorDataContainer != null )
				idManager.RegisterId( m_interpolatorDataContainer.InterpDataStartIdx, uniquePrefix + m_interpolatorDataContainer.InterpDataId, m_interpolatorDataContainer.InterpDataId );

			//Fetch function code areas
			FetchCodeAreas( offsetIdx, TemplatesManager.TemplateVertexCodeBeginArea, MasterNodePortCategory.Vertex, passInfo.Data );
			if( m_vertexFunctionData != null )
				idManager.RegisterId( m_vertexFunctionData.Position, uniquePrefix + m_vertexFunctionData.Id, m_vertexFunctionData.Id );

			FetchCodeAreas( offsetIdx, TemplatesManager.TemplateFragmentCodeBeginArea, MasterNodePortCategory.Fragment, passInfo.Data );
			if( m_fragmentFunctionData != null )
				idManager.RegisterId( m_fragmentFunctionData.Position, uniquePrefix + m_fragmentFunctionData.Id, m_fragmentFunctionData.Id );

			//Fetching inputs, must be do
			if( m_fragmentFunctionData != null )
				FetchInputs( offsetIdx, MasterNodePortCategory.Fragment, passInfo.Data );

			if( m_vertexFunctionData != null )
				FetchInputs( offsetIdx, MasterNodePortCategory.Vertex, passInfo.Data );

			//Fetch local variables must be done after fetching code areas as it needs them to see is variable is on vertex or fragment
			TemplateHelperFunctions.FetchLocalVars( passInfo.Data, ref m_localVarsList, m_vertexFunctionData, m_fragmentFunctionData );

			int localVarCount = m_localVarsList.Count;
			if( localVarCount > 0 )
			{
				idManager.RegisterTag( TemplatesManager.TemplateLocalVarTag );
				for( int i = 0; i < localVarCount; i++ )
				{
					if( m_localVarsList[ i ].IsSpecialVar )
					{
						idManager.RegisterTag( m_localVarsList[ i ].Id );
					}
				}
			}

			int inputsCount = m_inputDataList.Count;
			for( int i = 0; i < inputsCount; i++ )
			{
				if( m_inputDataList[ i ] != null )
					idManager.RegisterId( m_inputDataList[ i ].TagGlobalStartIdx, uniquePrefix + m_inputDataList[ i ].TagId, m_inputDataList[ i ].TagId );
			}

			//int passEndIndex = passInfo.Data.LastIndexOf( "}" );
			//if( passEndIndex > 0 )
			//{
			//	int identationIndex = -1;
			//	for( int i = passEndIndex; i >= 0; i-- )
			//	{
			//		if( passInfo.Data[ i ] == TemplatesManager.TemplateNewLine )
			//		{
			//			identationIndex = i + 1;
			//			break;
			//		}

			//		if( i == 0 )
			//		{
			//			identationIndex = 0;
			//		}
			//	}

			//	if( identationIndex > -1 )
			//	{
			//		int length = passEndIndex - identationIndex;
			//		string indentation = ( length > 0 ) ? passInfo.Data.Substring( identationIndex, length ) : string.Empty;
			//		TemplateProperty templateProperty = new TemplateProperty( TemplatesManager.TemplateEndPassTag, indentation, false );
			//		m_templateProperties.AddId( templateProperty );
			//		idManager.RegisterId( offsetIdx + passEndIndex, uniquePrefix + TemplatesManager.TemplateEndPassTag, string.Empty );
			//	}
			//}

			ownDuplicatesDict.Clear();
			ownDuplicatesDict = null;
		}

		public void Destroy()
		{
			m_passNameContainer = null;
#if CUSTOM_OPTIONS_AVAILABLE
			m_customOptionsContainer = null;
#endif
			if( m_templateProperties != null )
				m_templateProperties.Destroy();

			m_templateProperties = null;

			if( m_modules != null )
				m_modules.Destroy();

			m_modules = null;

			if( m_inputDataList != null )
				m_inputDataList.Clear();

			m_inputDataList = null;

			if( m_inputDataDict != null )
				m_inputDataDict.Clear();

			m_inputDataDict = null;

			m_vertexFunctionData = null;
			m_fragmentFunctionData = null;

			if( m_vertexDataContainer != null )
				m_vertexDataContainer.Destroy();

			m_vertexDataContainer = null;

			if( m_interpolatorDataContainer != null )
				m_interpolatorDataContainer.Destroy();

			if( m_localVarsList != null )
			{
				m_localVarsList.Clear();
				m_localVarsList = null;
			}

			m_interpolatorDataContainer = null;

			if( m_availableShaderGlobals != null )
				m_availableShaderGlobals.Clear();

			m_availableShaderGlobals = null;
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

		void FetchPassName( int offsetIdx, string body )
		{
			Match match = Regex.Match( body, TemplateHelperFunctions.PassNamePattern );
			if( match != null && match.Groups.Count > 1 )
			{
				m_passNameContainer.Id = match.Groups[ 0 ].Value;
				m_passNameContainer.Data = match.Groups[ 1 ].Value;
				m_passNameContainer.Index = offsetIdx + match.Index;
			}
		}

		void FetchVertexAndInterpData( TemplateModulesData subShaderModule, int offsetIdx, string body )
		{
			// Vertex Data
			try
			{
				int vertexDataTagBegin = body.IndexOf( TemplatesManager.TemplateVertexDataTag );
				if( vertexDataTagBegin > -1 )
				{
					m_vertexDataContainer = new VertexDataContainer();
					m_vertexDataContainer.VertexDataStartIdx = offsetIdx + vertexDataTagBegin;
					int vertexDataTagEnd = body.IndexOf( TemplatesManager.TemplateEndOfLine, vertexDataTagBegin );
					m_vertexDataContainer.VertexDataId = body.Substring( vertexDataTagBegin, vertexDataTagEnd + TemplatesManager.TemplateEndOfLine.Length - vertexDataTagBegin );
					int dataBeginIdx = body.LastIndexOf( '{', vertexDataTagBegin, vertexDataTagBegin );
					string vertexData = body.Substring( dataBeginIdx + 1, vertexDataTagBegin - dataBeginIdx );

					int parametersBegin = vertexDataTagBegin + TemplatesManager.TemplateVertexDataTag.Length;
					string parameters = body.Substring( parametersBegin, vertexDataTagEnd - parametersBegin );
					m_vertexDataContainer.VertexData = TemplateHelperFunctions.CreateVertexDataList( vertexData, parameters );
					m_templateProperties.AddId( body, m_vertexDataContainer.VertexDataId );
				}
			}
			catch( Exception e )
			{
				Debug.LogException( e );
			}

			// Available interpolators
			try
			{
				int interpDataBegin = body.IndexOf( TemplatesManager.TemplateInterpolatorBeginTag );
				if( interpDataBegin > -1 )
				{
					int interpDataEnd = body.IndexOf( TemplatesManager.TemplateEndOfLine, interpDataBegin );
					string interpDataId = body.Substring( interpDataBegin, interpDataEnd + TemplatesManager.TemplateEndOfLine.Length - interpDataBegin );

					int dataBeginIdx = body.LastIndexOf( '{', interpDataBegin, interpDataBegin );
					string interpData = body.Substring( dataBeginIdx + 1, interpDataBegin - dataBeginIdx );

					int interpolatorAmount = TemplateHelperFunctions.AvailableInterpolators[ "2.5" ];

					if( m_modules.ShaderModel.IsValid )
					{
						interpolatorAmount = m_modules.ShaderModel.InterpolatorAmount;
					}
					else if( subShaderModule.ShaderModel.IsValid )
					{
						interpolatorAmount = subShaderModule.ShaderModel.InterpolatorAmount;
					}

					m_interpolatorDataContainer = TemplateHelperFunctions.CreateInterpDataList( interpData, interpDataId, interpolatorAmount );
					m_interpolatorDataContainer.InterpDataId = interpDataId;
					m_interpolatorDataContainer.InterpDataStartIdx = offsetIdx + interpDataBegin;
					m_templateProperties.AddId( body, interpDataId );

				}
			}
			catch( Exception e )
			{
				Debug.LogException( e );
			}
		}

		void FetchCodeAreas( int offsetIdx, string begin, MasterNodePortCategory category, string body )
		{
			int areaBeginIndexes = body.IndexOf( begin );
			if( areaBeginIndexes > -1 )
			{
				int beginIdx = areaBeginIndexes + begin.Length;
				int endIdx = body.IndexOf( TemplatesManager.TemplateEndOfLine, beginIdx );
				int length = endIdx - beginIdx;

				string parameters = body.Substring( beginIdx, length );

				string[] parametersArr = parameters.Split( IOUtils.FIELD_SEPARATOR );

				string id = body.Substring( areaBeginIndexes, endIdx + TemplatesManager.TemplateEndOfLine.Length - areaBeginIndexes );
				string inParameters = parametersArr[ 0 ];
				string outParameters = ( parametersArr.Length > 1 ) ? parametersArr[ 1 ] : string.Empty;
				if( category == MasterNodePortCategory.Fragment )
				{
					string mainBodyName = string.Empty;
					int mainBodyLocalIndex = -1;

					Match mainBodyNameMatch = Regex.Match( body, TemplateHelperFunctions.FragmentPragmaPattern );
					if( mainBodyNameMatch != null && mainBodyNameMatch.Groups.Count == 2 )
					{
						mainBodyName = mainBodyNameMatch.Groups[ 1 ].Value;
						string pattern = string.Format( TemplateHelperFunctions.FunctionBodyStartPattern, mainBodyName );
						Match mainBodyIdMatch = Regex.Match( body, pattern );
						if( mainBodyIdMatch != null && mainBodyIdMatch.Groups.Count > 0 )
						{
							mainBodyLocalIndex = mainBodyIdMatch.Index;
						}

					}

					m_fragmentFunctionData = new TemplateFunctionData( mainBodyLocalIndex, mainBodyName, id, offsetIdx + areaBeginIndexes, inParameters, outParameters, category );
				}
				else
				{
					string mainBodyName = string.Empty;
					int mainBodyLocalIndex = -1;

					Match mainBodyNameMatch = Regex.Match( body, TemplateHelperFunctions.VertexPragmaPattern );
					if( mainBodyNameMatch != null && mainBodyNameMatch.Groups.Count == 2 )
					{
						mainBodyName = mainBodyNameMatch.Groups[ 1 ].Value;
						string pattern = string.Format( TemplateHelperFunctions.FunctionBodyStartPattern, mainBodyName );
						Match mainBodyIdMatch = Regex.Match( body, pattern );
						if( mainBodyIdMatch != null && mainBodyIdMatch.Groups.Count > 0 )
						{
							mainBodyLocalIndex = mainBodyIdMatch.Index;
						}
					}

					m_vertexFunctionData = new TemplateFunctionData( mainBodyLocalIndex, mainBodyName, id, offsetIdx + areaBeginIndexes, inParameters, outParameters, category );
				}
				m_templateProperties.AddId( body, id, true );
			}
		}

		void FetchInputs( int offset, MasterNodePortCategory portCategory, string body )
		{
			string beginTag = ( portCategory == MasterNodePortCategory.Fragment ) ? TemplatesManager.TemplateInputsFragBeginTag : TemplatesManager.TemplateInputsVertBeginTag;
			int[] inputBeginIndexes = body.AllIndexesOf( beginTag );
			if( inputBeginIndexes != null && inputBeginIndexes.Length > 0 )
			{
				for( int i = 0; i < inputBeginIndexes.Length; i++ )
				{
					int inputEndIdx = body.IndexOf( TemplatesManager.TemplateEndSectionTag, inputBeginIndexes[ i ] );
					int defaultValueBeginIdx = inputEndIdx + TemplatesManager.TemplateEndSectionTag.Length;
					int endLineIdx = body.IndexOf( TemplatesManager.TemplateFullEndTag, defaultValueBeginIdx );

					string defaultValue = body.Substring( defaultValueBeginIdx, endLineIdx - defaultValueBeginIdx );
					string tagId = body.Substring( inputBeginIndexes[ i ], endLineIdx + TemplatesManager.TemplateFullEndTag.Length - inputBeginIndexes[ i ] );

					int beginIndex = inputBeginIndexes[ i ] + beginTag.Length;
					int length = inputEndIdx - beginIndex;
					string inputData = body.Substring( beginIndex, length );
					string[] inputDataArray = inputData.Split( IOUtils.FIELD_SEPARATOR );

					if( inputDataArray != null && inputDataArray.Length > 0 )
					{
						try
						{
							string portName = inputDataArray[ (int)TemplatePortIds.Name ];
							WirePortDataType dataType = (WirePortDataType)Enum.Parse( typeof( WirePortDataType ), inputDataArray[ (int)TemplatePortIds.DataType ].ToUpper() );
							if( inputDataArray.Length == 3 )
							{
								int portOrderId = m_inputDataList.Count;
								int portUniqueId = -1;
								bool isInt = int.TryParse( inputDataArray[ 2 ], out portUniqueId );
								if( isInt )
								{
									if( portUniqueId < 0 )
										portUniqueId = m_inputDataList.Count;

									m_inputDataList.Add( new TemplateInputData( inputBeginIndexes[ i ], offset + inputBeginIndexes[ i ], tagId, portName, defaultValue, dataType, portCategory, portUniqueId, portOrderId, string.Empty ) );
									m_templateProperties.AddId( body, tagId, false );
								}
								else
								{
									portUniqueId = m_inputDataList.Count;
									m_inputDataList.Add( new TemplateInputData( inputBeginIndexes[ i ], offset + inputBeginIndexes[ i ], tagId, portName, defaultValue, dataType, portCategory, portUniqueId, portOrderId, inputDataArray[ 2 ] ) );
									m_templateProperties.AddId( body, tagId, false );
								}
							}
							else
							{
								int portUniqueIDArrIdx = (int)TemplatePortIds.UniqueId;
								int portUniqueId = ( portUniqueIDArrIdx < inputDataArray.Length ) ? Convert.ToInt32( inputDataArray[ portUniqueIDArrIdx ] ) : -1;
								if( portUniqueId < 0 )
									portUniqueId = m_inputDataList.Count;

								int portOrderArrayIdx = (int)TemplatePortIds.OrderId;
								int portOrderId = ( portOrderArrayIdx < inputDataArray.Length ) ? Convert.ToInt32( inputDataArray[ portOrderArrayIdx ] ) : -1;
								if( portOrderId < 0 )
									portOrderId = m_inputDataList.Count;

								int portLinkIdx = (int)TemplatePortIds.Link;
								string linkId = ( portLinkIdx < inputDataArray.Length ) ? inputDataArray[ portLinkIdx ] : string.Empty;
								m_inputDataList.Add( new TemplateInputData( inputBeginIndexes[ i ], offset + inputBeginIndexes[ i ], tagId, portName, defaultValue, dataType, portCategory, portUniqueId, portOrderId, linkId ) );
								m_templateProperties.AddId( body, tagId, false );
							}
						}
						catch( Exception e )
						{
							Debug.LogException( e );
						}
					}
				}
			}
		}

#if CUSTOM_OPTIONS_AVAILABLE
		public TemplateOptionsContainer CustomOptionsContainer { get { return m_customOptionsContainer; } }
#endif
		public TemplateModulesData Modules { get { return m_modules; } }
		public List<TemplateInputData> InputDataList { get { return m_inputDataList; } }
		public TemplateFunctionData VertexFunctionData { get { return m_vertexFunctionData; } }
		public TemplateFunctionData FragmentFunctionData { get { return m_fragmentFunctionData; } }
		public VertexDataContainer VertexDataContainer { get { return m_vertexDataContainer; } }
		public TemplateInterpData InterpolatorDataContainer { get { return m_interpolatorDataContainer; } }
		public string UniquePrefix { get { return m_uniquePrefix; } }
		public TemplatePropertyContainer TemplateProperties { get { return m_templateProperties; } }
		public List<TemplateShaderPropertyData> AvailableShaderGlobals { get { return m_availableShaderGlobals; } }
		public List<TemplateLocalVarData> LocalVarsList { get { return m_localVarsList; } }
		public TemplateInfoContainer PassNameContainer { get { return m_passNameContainer; } }
		public bool IsMainPass { get { return m_isMainPass; } set { m_isMainPass = value; } }
		public bool IsInvisible { get { return m_isInvisible; } }
		public int InvisibleOptions { get { return m_invisibleOptions; } }
		public int Idx { get { return m_idx; } }
		public bool AddToList
		{
			get
			{
				if( m_isInvisible )
				{
					return ( m_inputDataList.Count > 0 );
				}

				return true;
			}
		}
		public bool HasValidFunctionBody
		{
			get
			{
				if( m_fragmentFunctionData != null || m_vertexFunctionData != null )
					return true;
				return false;
			}
		}
	}
}
