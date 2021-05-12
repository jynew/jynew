// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	public enum NormalizeType
	{
		Off,
		Regular,
		Safe
	}

	public class InterpDataHelper
	{
		public string VarName;
		public WirePortDataType VarType;
		public bool IsSingleComponent;
		public InterpDataHelper( WirePortDataType varType, string varName, bool isSingleComponent = true )
		{
			VarName = varName;
			VarType = varType;
			IsSingleComponent = isSingleComponent;
		}
	}

	public class TemplateCustomData
	{
		public WirePortDataType DataType;
		public string Name;
		public bool IsVertex;
		public bool IsFragment;
		public TemplateCustomData( string name, WirePortDataType dataType )
		{
			name = Name;
			DataType = dataType;
			IsVertex = false;
			IsFragment = false;
		}
	}

	public class TemplateInputParameters
	{
		public WirePortDataType Type;
		public string Name;
		public string Declaration;
		public TemplateSemantics Semantic;
		public TemplateInputParameters( WirePortDataType type, PrecisionType precision, string name, TemplateSemantics semantic )
		{
			Type = type;
			Name = name;
			Semantic = semantic;
			Declaration = string.Format( "{0} {1} : {2}", UIUtils.PrecisionWirePortToCgType( precision, type ), Name, Semantic );
		}
	}

	public class TemplateDataCollector
	{
#if UNITY_2018_2_OR_NEWER
		private const int MaxUV = 8;
		private int[] m_UVUsage = { 0, 0, 0, 0, 0, 0, 0, 0 };
#else
		private const int MaxUV = 4;
		private int[] m_UVUsage = { 0, 0, 0, 0 };
#endif
		private int m_multipassSubshaderIdx = 0;
		private int m_multipassPassIdx = 0;
		private TemplateMultiPass m_currentTemplate;
		private TemplateSRPType m_currentSRPType = TemplateSRPType.BuiltIn;

		private Dictionary<string, TemplateCustomData> m_customInterpolatedData;
		private Dictionary<string, TemplateVertexData> m_registeredVertexData;

		private Dictionary<TemplateInfoOnSematics, InterpDataHelper> m_availableFragData;
		private Dictionary<TemplateInfoOnSematics, InterpDataHelper> m_availableVertData;
		private TemplateInterpData m_interpolatorData;
		private Dictionary<TemplateSemantics, TemplateVertexData> m_vertexDataDict;
		private TemplateData m_currentTemplateData;
		private MasterNodeDataCollector m_currentDataCollector;
		public Dictionary<TemplateSemantics, TemplateInputParameters> m_vertexInputParams;
		public Dictionary<TemplateSemantics, TemplateInputParameters> m_fragmentInputParams;

		private Dictionary<TemplateInfoOnSematics, TemplateLocalVarData> m_specialVertexLocalVars;
		private Dictionary<TemplateInfoOnSematics, TemplateLocalVarData> m_specialFragmentLocalVars;

		private List<PropertyDataCollector> m_lateDirectivesList = new List<PropertyDataCollector>();
		private Dictionary<string, PropertyDataCollector> m_lateDirectivesDict = new Dictionary<string, PropertyDataCollector>();

		public void SetUVUsage( int uv, WirePortDataType type )
		{
			if( uv >= 0 && uv < MaxUV )
			{
				m_UVUsage[ uv ] = Mathf.Max( m_UVUsage[ uv ], TemplateHelperFunctions.DataTypeChannelUsage[ type ] );
			}
		}

		public void SetUVUsage( int uv, int size )
		{
			if( uv >= 0 && uv < MaxUV )
			{
				m_UVUsage[ uv ] = Mathf.Max( m_UVUsage[ uv ], size );
			}
		}

		public void CloseLateDirectives()
		{
			if( m_lateDirectivesList.Count > 0 )
			{
				m_lateDirectivesList.Add( new PropertyDataCollector( -1, string.Empty ) );
			}
		}
		public void AddHDLightInfo()
		{
			AddLateDirective( AdditionalLineType.Custom, "#if (SHADERPASS != SHADERPASS_FORWARD) //On forward this info is already included" );
			AddLateDirective( AdditionalLineType.Include, "HDRP/Lighting/LightDefinition.cs.hlsl" );
			AddLateDirective( AdditionalLineType.Include, "HDRP/Lighting/LightLoop/Shadow.hlsl" );
			AddLateDirective( AdditionalLineType.Include, "HDRP/Lighting/LightLoop/LightLoopDef.hlsl" );
			AddLateDirective( AdditionalLineType.Custom, "#endif // End of light info includes" );
		}

		public void AddLateDirective( AdditionalLineType type, string value )
		{

			if( !m_lateDirectivesDict.ContainsKey( value ) )
			{
				string formattedValue = string.Empty;
				switch( type )
				{
					case AdditionalLineType.Include: formattedValue = string.Format( Constants.IncludeFormat, value ); break;
					case AdditionalLineType.Define: formattedValue = string.Format( Constants.DefineFormat, value ); break;
					case AdditionalLineType.Pragma: formattedValue = string.Format( Constants.PragmaFormat, value ); break;
					case AdditionalLineType.Custom: formattedValue = value; break;
				}
				PropertyDataCollector property = new PropertyDataCollector( -1, formattedValue );
				m_lateDirectivesDict.Add( value, property );
				m_lateDirectivesList.Add( property );
			}
		}

		public void SetMultipassInfo( TemplateMultiPass currentTemplate, int subShaderIdx, int passIdx, TemplateSRPType currentSRPType )
		{
			m_currentTemplate = currentTemplate;
			m_multipassSubshaderIdx = subShaderIdx;
			m_multipassPassIdx = passIdx;
			m_currentSRPType = currentSRPType;
		}

		public bool HasDirective( AdditionalLineType type, string value )
		{
			switch( type )
			{
				case AdditionalLineType.Include:
				{
					return m_currentTemplate.SubShaders[ m_multipassSubshaderIdx ].Modules.IncludePragmaContainer.HasInclude( value ) ||
					m_currentTemplate.SubShaders[ m_multipassSubshaderIdx ].Passes[ m_multipassPassIdx ].Modules.IncludePragmaContainer.HasInclude( value );
				}
				case AdditionalLineType.Define:
				{
					return m_currentTemplate.SubShaders[ m_multipassSubshaderIdx ].Modules.IncludePragmaContainer.HasDefine( value ) ||
					m_currentTemplate.SubShaders[ m_multipassSubshaderIdx ].Passes[ m_multipassPassIdx ].Modules.IncludePragmaContainer.HasDefine( value );
				}
				case AdditionalLineType.Pragma:
				{
					return m_currentTemplate.SubShaders[ m_multipassSubshaderIdx ].Modules.IncludePragmaContainer.HasPragma( value ) ||
					m_currentTemplate.SubShaders[ m_multipassSubshaderIdx ].Passes[ m_multipassPassIdx ].Modules.IncludePragmaContainer.HasPragma( value );
				}
			}

			return false;
		}

		public void FillSpecialVariables( TemplatePass currentPass )
		{
			m_specialVertexLocalVars = new Dictionary<TemplateInfoOnSematics, TemplateLocalVarData>();
			m_specialFragmentLocalVars = new Dictionary<TemplateInfoOnSematics, TemplateLocalVarData>();
			int localVarAmount = currentPass.LocalVarsList.Count;
			for( int i = 0; i < localVarAmount; i++ )
			{
				if( currentPass.LocalVarsList[ i ].IsSpecialVar )
				{
					if( currentPass.LocalVarsList[ i ].Category == MasterNodePortCategory.Vertex )
					{
						m_specialVertexLocalVars.Add( currentPass.LocalVarsList[ i ].SpecialVarType, currentPass.LocalVarsList[ i ] );
					}
					else
					{
						m_specialFragmentLocalVars.Add( currentPass.LocalVarsList[ i ].SpecialVarType, currentPass.LocalVarsList[ i ] );
					}
				}
			}
		}

		public void BuildFromTemplateData( MasterNodeDataCollector dataCollector, TemplateData templateData )
		{
			m_registeredVertexData = new Dictionary<string, TemplateVertexData>();
			m_customInterpolatedData = new Dictionary<string, TemplateCustomData>();


			m_currentDataCollector = dataCollector;
			m_currentTemplateData = templateData;

			m_vertexDataDict = new Dictionary<TemplateSemantics, TemplateVertexData>();
			if( templateData.VertexDataList != null )
			{
				for( int i = 0; i < templateData.VertexDataList.Count; i++ )
				{
					m_vertexDataDict.Add( templateData.VertexDataList[ i ].Semantics, new TemplateVertexData( templateData.VertexDataList[ i ] ) );
				}
			}

			m_availableFragData = new Dictionary<TemplateInfoOnSematics, InterpDataHelper>();
			if( templateData.InterpolatorData != null && templateData.FragFunctionData != null )
			{
				m_interpolatorData = new TemplateInterpData( templateData.InterpolatorData );
				int fragCount = templateData.InterpolatorData.Interpolators.Count;
				for( int i = 0; i < fragCount; i++ )
				{
					string varName = string.Empty;
					if( templateData.InterpolatorData.Interpolators[ i ].IsSingleComponent )
					{
						varName = string.Format( TemplateHelperFunctions.TemplateVarFormat,
													templateData.FragFunctionData.InVarName,
													templateData.InterpolatorData.Interpolators[ i ].VarNameWithSwizzle );
					}
					else
					{
						varName = string.Format( templateData.InterpolatorData.Interpolators[ i ].VarNameWithSwizzle, templateData.FragFunctionData.InVarName );
					}

					m_availableFragData.Add( templateData.InterpolatorData.Interpolators[ i ].DataInfo,
					new InterpDataHelper( templateData.InterpolatorData.Interpolators[ i ].SwizzleType,
					varName,
					templateData.InterpolatorData.Interpolators[ i ].IsSingleComponent ) );
				}
			}

			m_availableVertData = new Dictionary<TemplateInfoOnSematics, InterpDataHelper>();
			if( templateData.VertexFunctionData != null && templateData.VertexDataList != null )
			{
				int vertCount = templateData.VertexDataList.Count;
				for( int i = 0; i < vertCount; i++ )
				{
					m_availableVertData.Add( templateData.VertexDataList[ i ].DataInfo,
					new InterpDataHelper( templateData.VertexDataList[ i ].SwizzleType,
					string.Format( TemplateHelperFunctions.TemplateVarFormat,
					templateData.VertexFunctionData.InVarName,
					templateData.VertexDataList[ i ].VarNameWithSwizzle ),
					templateData.VertexDataList[ i ].IsSingleComponent ) );
				}
			}
		}

		public void RegisterFragInputParams( WirePortDataType type, PrecisionType precision, string name, TemplateSemantics semantic )
		{
			if( m_fragmentInputParams == null )
				m_fragmentInputParams = new Dictionary<TemplateSemantics, TemplateInputParameters>();

			m_fragmentInputParams.Add( semantic, new TemplateInputParameters( type, precision, name, semantic ) );
		}

		public void RegisterVertexInputParams( WirePortDataType type, PrecisionType precision, string name, TemplateSemantics semantic )
		{
			if( m_vertexInputParams == null )
				m_vertexInputParams = new Dictionary<TemplateSemantics, TemplateInputParameters>();

			m_vertexInputParams.Add( semantic, new TemplateInputParameters( type, precision, name, semantic ) );
		}

		public string GetVertexId()
		{
			if( m_vertexInputParams != null && m_vertexInputParams.ContainsKey( TemplateSemantics.SV_VertexID ) )
			{
				if( m_currentDataCollector.PortCategory == MasterNodePortCategory.Vertex )
					return m_vertexInputParams[ TemplateSemantics.SV_VertexID ].Name;
			}
			else
			{
				RegisterVertexInputParams( WirePortDataType.UINT, PrecisionType.Float, TemplateHelperFunctions.SemanticsDefaultName[ TemplateSemantics.SV_VertexID ], TemplateSemantics.SV_VertexID );
			}

			if( m_currentDataCollector.PortCategory != MasterNodePortCategory.Vertex)
				RegisterCustomInterpolatedData( m_vertexInputParams[ TemplateSemantics.SV_VertexID ].Name, WirePortDataType.INT, PrecisionType.Float, m_vertexInputParams[ TemplateSemantics.SV_VertexID ].Name );

			return m_vertexInputParams[ TemplateSemantics.SV_VertexID ].Name;
		}
#if UNITY_EDITOR_WIN
		public string GetPrimitiveId()
		{
			if( m_fragmentInputParams != null && m_fragmentInputParams.ContainsKey( TemplateSemantics.SV_PrimitiveID ) )
				return m_fragmentInputParams[ TemplateSemantics.SV_PrimitiveID ].Name;

			RegisterFragInputParams( WirePortDataType.UINT, PrecisionType.Half, TemplateHelperFunctions.SemanticsDefaultName[ TemplateSemantics.SV_PrimitiveID ], TemplateSemantics.SV_PrimitiveID );
			return m_fragmentInputParams[ TemplateSemantics.SV_PrimitiveID ].Name;
		}
#endif
		public string GetVFace()
		{
			if( m_fragmentInputParams != null && m_fragmentInputParams.ContainsKey( TemplateSemantics.VFACE ) )
				return m_fragmentInputParams[ TemplateSemantics.VFACE ].Name;

			RegisterFragInputParams( WirePortDataType.FLOAT, PrecisionType.Half, TemplateHelperFunctions.SemanticsDefaultName[ TemplateSemantics.VFACE ], TemplateSemantics.VFACE );
			return m_fragmentInputParams[ TemplateSemantics.VFACE ].Name;
		}

		public bool HasUV( int uvChannel )
		{
			return ( m_currentDataCollector.PortCategory == MasterNodePortCategory.Fragment ) ? m_availableFragData.ContainsKey( TemplateHelperFunctions.IntToUVChannelInfo[ uvChannel ] ) : m_availableVertData.ContainsKey( TemplateHelperFunctions.IntToUVChannelInfo[ uvChannel ] );
		}

		public string GetUVName( int uvChannel, WirePortDataType dataType = WirePortDataType.FLOAT2 )
		{
			InterpDataHelper info = ( m_currentDataCollector.PortCategory == MasterNodePortCategory.Fragment ) ? m_availableFragData[ TemplateHelperFunctions.IntToUVChannelInfo[ uvChannel ] ] : m_availableVertData[ TemplateHelperFunctions.IntToUVChannelInfo[ uvChannel ] ];
			if( dataType != info.VarType )
				return info.VarName + UIUtils.GetAutoSwizzle( dataType );
			else
				return info.VarName;
		}

		public string GetTextureCoord( int uvChannel, string propertyName, int uniqueId, PrecisionType precisionType )
		{
			bool isVertex = ( m_currentDataCollector.PortCategory == MasterNodePortCategory.Vertex || m_currentDataCollector.PortCategory == MasterNodePortCategory.Tessellation );
			string uvChannelName = string.Empty;
			string propertyHelperVar = propertyName + "_ST";
			m_currentDataCollector.AddToUniforms( uniqueId, "float4", propertyHelperVar );
			string uvName = string.Empty;
			if( m_currentDataCollector.TemplateDataCollectorInstance.HasUV( uvChannel ) )
			{
				uvName = m_currentDataCollector.TemplateDataCollectorInstance.GetUVName( uvChannel );
			}
			else
			{
				uvName = m_currentDataCollector.TemplateDataCollectorInstance.RegisterUV( uvChannel );
			}

			uvChannelName = "uv" + propertyName;
			if( isVertex )
			{
				string value = string.Format( Constants.TilingOffsetFormat, uvName, propertyHelperVar + ".xy", propertyHelperVar + ".zw" );
				string lodLevel = "0";

				value = "float4( " + value + ", 0 , " + lodLevel + " )";
				m_currentDataCollector.AddLocalVariable( uniqueId, precisionType, WirePortDataType.FLOAT4, uvChannelName, value );
			}
			else
			{
				m_currentDataCollector.AddLocalVariable( uniqueId, precisionType, WirePortDataType.FLOAT2, uvChannelName, string.Format( Constants.TilingOffsetFormat, uvName, propertyHelperVar + ".xy", propertyHelperVar + ".zw" ) );
			}
			return uvChannelName;
		}

		public InterpDataHelper GetUVInfo( int uvChannel )
		{
			return ( m_currentDataCollector.PortCategory == MasterNodePortCategory.Fragment ) ? m_availableFragData[ TemplateHelperFunctions.IntToUVChannelInfo[ uvChannel ] ] : m_availableVertData[ TemplateHelperFunctions.IntToUVChannelInfo[ uvChannel ] ];
		}

		public string RegisterUV( int UVChannel, WirePortDataType size = WirePortDataType.FLOAT2 )
		{
			int channelsSize = TemplateHelperFunctions.DataTypeChannelUsage[ size ];
			if( m_UVUsage[ UVChannel ] > channelsSize )
			{
				size = TemplateHelperFunctions.ChannelToDataType[ m_UVUsage[ UVChannel ] ];
			}

			if( m_currentDataCollector.PortCategory == MasterNodePortCategory.Vertex )
			{
				TemplateSemantics semantic = TemplateHelperFunctions.IntToSemantic[ UVChannel ];

				if( m_vertexDataDict.ContainsKey( semantic ) )
				{
					return m_vertexDataDict[ semantic ].VarName;
				}

				string varName = TemplateHelperFunctions.BaseInterpolatorName + ( ( UVChannel > 0 ) ? UVChannel.ToString() : string.Empty );
				m_availableVertData.Add( TemplateHelperFunctions.IntToUVChannelInfo[ UVChannel ],
				new InterpDataHelper( WirePortDataType.FLOAT4,
				string.Format( TemplateHelperFunctions.TemplateVarFormat,
				m_currentTemplateData.VertexFunctionData.InVarName,
				 varName ) ) );

				m_currentDataCollector.AddToVertexInput(
				string.Format( TemplateHelperFunctions.TexFullSemantic,
				varName,
				semantic ) );
				RegisterOnVertexData( semantic, size, varName );
				return m_availableVertData[ TemplateHelperFunctions.IntToUVChannelInfo[ UVChannel ] ].VarName;
			}
			else
			{
				//search if the correct vertex data is set ... 
				TemplateInfoOnSematics info = TemplateHelperFunctions.IntToInfo[ UVChannel ];
				TemplateSemantics vertexSemantics = TemplateSemantics.NONE;
				foreach( KeyValuePair<TemplateSemantics, TemplateVertexData> kvp in m_vertexDataDict )
				{
					if( kvp.Value.DataInfo == info )
					{
						vertexSemantics = kvp.Key;
						break;
					}
				}

				// if not, add vertex data and create interpolator 
				if( vertexSemantics == TemplateSemantics.NONE )
				{
					vertexSemantics = TemplateHelperFunctions.IntToSemantic[ UVChannel ];

					if( !m_vertexDataDict.ContainsKey( vertexSemantics ) )
					{
						string varName = TemplateHelperFunctions.BaseInterpolatorName + ( ( UVChannel > 0 ) ? UVChannel.ToString() : string.Empty );
						m_availableVertData.Add( TemplateHelperFunctions.IntToUVChannelInfo[ UVChannel ],
						new InterpDataHelper( WirePortDataType.FLOAT4,
						string.Format( TemplateHelperFunctions.TemplateVarFormat,
						m_currentTemplateData.VertexFunctionData.InVarName,
						 varName ) ) );

						m_currentDataCollector.AddToVertexInput(
						string.Format( TemplateHelperFunctions.TexFullSemantic,
						varName,
						vertexSemantics ) );
						RegisterOnVertexData( vertexSemantics, size, varName );
					}
				}

				// either way create interpolator
				TemplateVertexData availableInterp = RequestNewInterpolator( size, false );
				if( availableInterp != null )
				{
					string interpVarName = m_currentTemplateData.VertexFunctionData.OutVarName + "." + availableInterp.VarNameWithSwizzle;
					InterpDataHelper vertInfo = m_availableVertData[ TemplateHelperFunctions.IntToUVChannelInfo[ UVChannel ] ];
					string interpDecl = string.Format( TemplateHelperFunctions.TemplateVariableDecl, interpVarName, TemplateHelperFunctions.AutoSwizzleData( vertInfo.VarName, vertInfo.VarType, size ) );
					m_currentDataCollector.AddToVertexInterpolatorsDecl( interpDecl );
					string finalVarName = m_currentTemplateData.FragFunctionData.InVarName + "." + availableInterp.VarNameWithSwizzle;
					m_availableFragData.Add( TemplateHelperFunctions.IntToUVChannelInfo[ UVChannel ], new InterpDataHelper( size, finalVarName ) );
					return finalVarName;
				}
			}
			return string.Empty;
		}
		////////////////////////////////////////////////////////////////////////////////////////////////
		bool IsSemanticUsedOnInterpolator( TemplateSemantics semantics )
		{
			for( int i = 0; i < m_interpolatorData.Interpolators.Count; i++ )
			{
				if( m_interpolatorData.Interpolators[ i ].Semantics == semantics )
				{
					return true;
				}
			}
			return false;
		}

		public bool HasInfo( TemplateInfoOnSematics info, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			MasterNodePortCategory category = useMasterNodeCategory ? m_currentDataCollector.PortCategory : customCategory;
			return ( category == MasterNodePortCategory.Fragment ) ? m_availableFragData.ContainsKey( info ) : m_availableVertData.ContainsKey( info );
		}

		public InterpDataHelper GetInfo( TemplateInfoOnSematics info, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			MasterNodePortCategory category = useMasterNodeCategory ? m_currentDataCollector.PortCategory : customCategory;
			return ( category == MasterNodePortCategory.Fragment ) ? m_availableFragData[ info ] : m_availableVertData[ info ];
		}

		public string RegisterInfoOnSemantic( TemplateInfoOnSematics info, TemplateSemantics semantic, string name, WirePortDataType dataType, PrecisionType precisionType, bool requestNewInterpolator, string dataName = null )
		{
			return RegisterInfoOnSemantic( m_currentDataCollector.PortCategory, info, semantic, name, dataType, precisionType, requestNewInterpolator, dataName );
		}
		// This should only be used to semantics outside the text coord set
		public string RegisterInfoOnSemantic( MasterNodePortCategory portCategory, TemplateInfoOnSematics info, TemplateSemantics semantic, string name, WirePortDataType dataType, PrecisionType precisionType, bool requestNewInterpolator, string dataName = null )
		{
			if( portCategory == MasterNodePortCategory.Vertex )
			{
				if( m_vertexDataDict.ContainsKey( semantic ) )
				{
					return m_vertexDataDict[ semantic ].VarName;
				}

				m_availableVertData.Add( info,
				new InterpDataHelper( dataType,
				string.Format( TemplateHelperFunctions.TemplateVarFormat,
				m_currentTemplateData.VertexFunctionData.InVarName,
				name ) ) );

				string vertInputVarType = UIUtils.FinalPrecisionWirePortToCgType( precisionType, dataType );
				m_currentDataCollector.AddToVertexInput(
				string.Format( TemplateHelperFunctions.InterpFullSemantic,
				vertInputVarType,
				name,
				semantic ) );
				RegisterOnVertexData( semantic, dataType, name );
				return m_availableVertData[ info ].VarName;
			}
			else
			{
				//search if the correct vertex data is set ... 
				TemplateSemantics vertexSemantics = TemplateSemantics.NONE;
				foreach( KeyValuePair<TemplateSemantics, TemplateVertexData> kvp in m_vertexDataDict )
				{
					if( kvp.Value.DataInfo == info )
					{
						vertexSemantics = kvp.Key;
						break;
					}
				}

				// if not, add vertex data and create interpolator 
				if( vertexSemantics == TemplateSemantics.NONE )
				{
					vertexSemantics = semantic;

					if( !m_vertexDataDict.ContainsKey( vertexSemantics ) )
					{
						m_availableVertData.Add( info,
						new InterpDataHelper( dataType,
						string.Format( TemplateHelperFunctions.TemplateVarFormat,
						m_currentTemplateData.VertexFunctionData.InVarName,
						name ) ) );

						string vertInputVarType = UIUtils.FinalPrecisionWirePortToCgType( precisionType, dataType );
						m_currentDataCollector.AddToVertexInput(
						string.Format( TemplateHelperFunctions.InterpFullSemantic,
						vertInputVarType,
						name,
						vertexSemantics ) );
						RegisterOnVertexData( vertexSemantics, dataType, name );
					}
				}

				// either way create interpolator

				TemplateVertexData availableInterp = null;
				if( requestNewInterpolator || IsSemanticUsedOnInterpolator( semantic ) )
				{
					availableInterp = RequestNewInterpolator( dataType, false, dataName );
				}
				else
				{
					availableInterp = RegisterOnInterpolator( semantic, dataType, dataName );
				}

				if( availableInterp != null )
				{
					string interpVarName = m_currentTemplateData.VertexFunctionData.OutVarName + "." + availableInterp.VarNameWithSwizzle;
					string interpDecl = string.Format( TemplateHelperFunctions.TemplateVariableDecl, interpVarName, TemplateHelperFunctions.AutoSwizzleData( m_availableVertData[ info ].VarName, m_availableVertData[ info ].VarType, dataType ) );
					m_currentDataCollector.AddToVertexInterpolatorsDecl( interpDecl );
					string finalVarName = m_currentTemplateData.FragFunctionData.InVarName + "." + availableInterp.VarNameWithSwizzle;
					m_availableFragData.Add( info, new InterpDataHelper( dataType, finalVarName ) );
					return finalVarName;
				}
			}
			return string.Empty;
		}

		TemplateVertexData RegisterOnInterpolator( TemplateSemantics semantics, WirePortDataType dataType, string vertexDataName = null )
		{
			if( vertexDataName == null )
			{
				if( TemplateHelperFunctions.SemanticsDefaultName.ContainsKey( semantics ) )
				{
					vertexDataName = TemplateHelperFunctions.SemanticsDefaultName[ semantics ];
				}
				else
				{
					vertexDataName = string.Empty;
					Debug.LogError( "No valid name given to vertex data" );
				}
			}

			TemplateVertexData data = new TemplateVertexData( semantics, dataType, vertexDataName );
			m_interpolatorData.Interpolators.Add( data );
			string interpolator = string.Format( TemplateHelperFunctions.InterpFullSemantic, UIUtils.WirePortToCgType( dataType ), data.VarName, data.Semantics );
			m_currentDataCollector.AddToInterpolators( interpolator );
			return data;
		}

		public void RegisterOnVertexData( TemplateSemantics semantics, WirePortDataType dataType, string varName )
		{
			m_vertexDataDict.Add( semantics, new TemplateVertexData( semantics, dataType, varName ) );
		}

		public TemplateVertexData RequestMacroInterpolator( string varName )
		{
			if( varName != null && m_registeredVertexData.ContainsKey( varName ) )
			{
				return m_registeredVertexData[ varName ];
			}

			for( int i = 0; i < m_interpolatorData.AvailableInterpolators.Count; i++ )
			{
				if( !m_interpolatorData.AvailableInterpolators[ i ].IsFull )
				{
					TemplateVertexData data = m_interpolatorData.AvailableInterpolators[ i ].RequestChannels( WirePortDataType.FLOAT4, false, varName );
					if( data != null )
					{
						if( !m_registeredVertexData.ContainsKey( data.VarName ) )
						{
							m_registeredVertexData.Add( data.VarName, data );
						}
						if( m_interpolatorData.AvailableInterpolators[ i ].Usage == 1 )
						{
							string interpolator = string.Format( TemplateHelperFunctions.InterpMacro, varName, TemplateHelperFunctions.SemanticToInt[ data.Semantics ] );
							m_currentDataCollector.AddToInterpolators( interpolator );
						}
						return data;
					}
				}
			}
			return null;
		}


		public TemplateVertexData RequestNewInterpolator( WirePortDataType dataType, bool isColor, string varName = null )
		{
			if( varName != null && m_registeredVertexData.ContainsKey( varName ) )
			{
				return m_registeredVertexData[ varName ];
			}

			for( int i = 0; i < m_interpolatorData.AvailableInterpolators.Count; i++ )
			{
				if( !m_interpolatorData.AvailableInterpolators[ i ].IsFull )
				{
					TemplateVertexData data = m_interpolatorData.AvailableInterpolators[ i ].RequestChannels( dataType, isColor, varName );
					if( data != null )
					{
						if( !m_registeredVertexData.ContainsKey( data.VarName ) )
						{
							m_registeredVertexData.Add( data.VarName, data );
						}

						if( m_interpolatorData.AvailableInterpolators[ i ].Usage == 1 )
						{
							// First time using this interpolator, so we need to register it
							string interpolator = string.Format( TemplateHelperFunctions.TexFullSemantic,
																	data.VarName, data.Semantics );
							m_currentDataCollector.AddToInterpolators( interpolator );
						}
						return data;
					}
				}
			}
			return null;
		}

		// Unused channels in interpolators must be set to something so the compiler doesn't generate warnings
		public List<string> GetInterpUnusedChannels()
		{
			List<string> resetInstrucctions = new List<string>();

			if( m_interpolatorData != null )
			{
				for( int i = 0; i < m_interpolatorData.AvailableInterpolators.Count; i++ )
				{
					if( m_interpolatorData.AvailableInterpolators[ i ].Usage > 0 && !m_interpolatorData.AvailableInterpolators[ i ].IsFull )
					{
						string channels = string.Empty;
						bool[] availableChannels = m_interpolatorData.AvailableInterpolators[ i ].AvailableChannels;
						for( int j = 0; j < availableChannels.Length; j++ )
						{
							if( availableChannels[ j ] )
							{
								channels += TemplateHelperFunctions.VectorSwizzle[ j ];
							}
						}

						resetInstrucctions.Add( string.Format( "{0}.{1}.{2} = 0;", m_currentTemplateData.VertexFunctionData.OutVarName, m_interpolatorData.AvailableInterpolators[ i ].Name, channels ) );
					}
				}
			}

			if( resetInstrucctions.Count > 0 )
			{
				resetInstrucctions.Insert( 0, "\n//setting value to unused interpolator channels and avoid initialization warnings" );
			}

			return resetInstrucctions;
		}

		bool GetCustomInterpolatedData( TemplateInfoOnSematics info, WirePortDataType type, PrecisionType precisionType, ref string result, bool useMasterNodeCategory, MasterNodePortCategory customCategory )
		{
			MasterNodePortCategory category = useMasterNodeCategory ? m_currentDataCollector.PortCategory : customCategory;
			if( category == MasterNodePortCategory.Vertex )
			{
				if( m_specialVertexLocalVars.ContainsKey( info ) )
				{
					result = m_specialVertexLocalVars[ info ].LocalVarName;
					if( m_specialVertexLocalVars[ info ].DataType != type )
					{
						result = TemplateHelperFunctions.AutoSwizzleData( result, m_specialVertexLocalVars[ info ].DataType, type );
					}
					return true;
				}
			}

			if( category == MasterNodePortCategory.Fragment )
			{
				if( m_specialFragmentLocalVars.ContainsKey( info ) )
				{
					result = m_specialFragmentLocalVars[ info ].LocalVarName;
					if( m_specialFragmentLocalVars[ info ].DataType != type )
					{
						result = TemplateHelperFunctions.AutoSwizzleData( result, m_specialFragmentLocalVars[ info ].DataType, type );
					}
					return true;
				}

				if( m_availableFragData.ContainsKey( info ) )
				{
					if( m_availableFragData[ info ].IsSingleComponent )
					{
						result = m_availableFragData[ info ].VarName;
						if( m_availableFragData[ info ].VarType != type )
						{
							result = TemplateHelperFunctions.AutoSwizzleData( result, m_availableFragData[ info ].VarType, type );
						}
						return true;
					}
					else if( TemplateHelperFunctions.InfoToLocalVar.ContainsKey( info ) && TemplateHelperFunctions.InfoToWirePortType.ContainsKey( info ) )
					{
						result = TemplateHelperFunctions.InfoToLocalVar[ info ];
						m_currentDataCollector.AddLocalVariable( -1, precisionType, TemplateHelperFunctions.InfoToWirePortType[ info ], result, m_availableFragData[ info ].VarName );
						return true;
					}
				}
			}
			return false;
		}

		public string GetVertexPosition( WirePortDataType type, PrecisionType precisionType, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			if( HasInfo( TemplateInfoOnSematics.POSITION, useMasterNodeCategory, customCategory ) )
			{
				InterpDataHelper info = GetInfo( TemplateInfoOnSematics.POSITION, useMasterNodeCategory, customCategory );
				if( type != info.VarType )
					return TemplateHelperFunctions.AutoSwizzleData( info.VarName, info.VarType, type );
				else
					return info.VarName;
			}
			else
			{
				MasterNodePortCategory portCategory = useMasterNodeCategory ? m_currentDataCollector.PortCategory : customCategory;
				string name = "ase_vertex_pos";
				string varName = RegisterInfoOnSemantic( portCategory, TemplateInfoOnSematics.POSITION, TemplateSemantics.POSITION, name, WirePortDataType.FLOAT4, precisionType, true );
				if( type != WirePortDataType.FLOAT4 )
					return TemplateHelperFunctions.AutoSwizzleData( varName, WirePortDataType.FLOAT4, type );
				else
					return varName;
			}
		}

		public string GetVertexColor( PrecisionType precisionType )
		{
			if( HasInfo( TemplateInfoOnSematics.COLOR ) )
			{
				return GetInfo( TemplateInfoOnSematics.COLOR ).VarName;
			}
			else
			{
				string name = "ase_color";
				return RegisterInfoOnSemantic( TemplateInfoOnSematics.COLOR, TemplateSemantics.COLOR, name, WirePortDataType.FLOAT4, precisionType, false );
			}
		}

		public string GetVertexNormal( PrecisionType precisionType, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			if( HasInfo( TemplateInfoOnSematics.NORMAL, useMasterNodeCategory, customCategory ) )
			{
				InterpDataHelper info = GetInfo( TemplateInfoOnSematics.NORMAL, useMasterNodeCategory, customCategory );
				return TemplateHelperFunctions.AutoSwizzleData( info.VarName, info.VarType, WirePortDataType.FLOAT3 );
			}
			else
			{
				MasterNodePortCategory category = useMasterNodeCategory ? m_currentDataCollector.PortCategory : customCategory;
				string name = "ase_normal";
				return RegisterInfoOnSemantic( category, TemplateInfoOnSematics.NORMAL, TemplateSemantics.NORMAL, name, WirePortDataType.FLOAT3, precisionType, false );
			}
		}

		public string GetWorldNormal( PrecisionType precisionType, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment, bool normalize = false )
		{
			string result = string.Empty;
			if( GetCustomInterpolatedData( TemplateInfoOnSematics.WORLD_NORMAL, WirePortDataType.FLOAT3, precisionType, ref result, useMasterNodeCategory, customCategory ) )
			{
				if( normalize )
					return string.Format( "normalize( {0} )", result );
				else
					return result;
			}

			string varName = normalize ? "normalizeWorldNormal" : GeneratorUtils.WorldNormalStr;
			if( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;
			string vertexNormal = GetVertexNormal( precisionType, false, MasterNodePortCategory.Vertex );
			string formatStr = string.Empty;
			if( IsSRP )
				formatStr = "TransformObjectToWorldNormal({0})";
			else
				formatStr = "UnityObjectToWorldNormal({0})";
			string worldNormalValue = string.Format( formatStr, vertexNormal );

			if( normalize )
				worldNormalValue = string.Format( "normalize( {0} )", worldNormalValue );

			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT3, PrecisionType.Float, worldNormalValue, useMasterNodeCategory, customCategory );
			return varName;
		}

		public string GetWorldNormal( int uniqueId, PrecisionType precisionType, string normal, string outputId )
		{
			string tanToWorld0 = string.Empty;
			string tanToWorld1 = string.Empty;
			string tanToWorld2 = string.Empty;

			GetWorldTangentTf( precisionType, out tanToWorld0, out tanToWorld1, out tanToWorld2, true );

			string tanNormal = "tanNormal" + outputId;
			m_currentDataCollector.AddLocalVariable( uniqueId, "float3 " + tanNormal + " = " + normal + ";" );
			return string.Format( "float3(dot({1},{0}), dot({2},{0}), dot({3},{0}))", tanNormal, tanToWorld0, tanToWorld1, tanToWorld2 );
		}

		public string GetVertexTangent( PrecisionType precisionType, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			if( HasInfo( TemplateInfoOnSematics.TANGENT, useMasterNodeCategory, customCategory ) )
			{
				InterpDataHelper info = GetInfo( TemplateInfoOnSematics.TANGENT, useMasterNodeCategory, customCategory );
				return info.VarName;
			}
			else
			{
				MasterNodePortCategory category = useMasterNodeCategory ? m_currentDataCollector.PortCategory : customCategory;
				string name = "ase_tangent";
				return RegisterInfoOnSemantic( category, TemplateInfoOnSematics.TANGENT, TemplateSemantics.TANGENT, name, WirePortDataType.FLOAT4, precisionType, false );
			}
		}

		public string GetVertexBitangent( PrecisionType precisionType, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string varName = GeneratorUtils.VertexBitangentStr;
			if( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;

			string tangentValue = GetVertexTangent( precisionType, false, MasterNodePortCategory.Vertex );
			string normalValue = GetVertexNormal( precisionType, false, MasterNodePortCategory.Vertex );

			string bitangentValue = string.Format( "cross({0},{1})", normalValue, tangentValue );
			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT, PrecisionType.Float, bitangentValue, useMasterNodeCategory, customCategory );
			return varName;
		}

		public string GetWorldTangent( PrecisionType precisionType, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string result = string.Empty;
			if( GetCustomInterpolatedData( TemplateInfoOnSematics.WORLD_TANGENT, WirePortDataType.FLOAT3, precisionType, ref result, useMasterNodeCategory, customCategory ) )
			{
				return result;
			}

			string varName = GeneratorUtils.WorldTangentStr;
			if( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;

			string vertexTangent = GetVertexTangent( precisionType, false, MasterNodePortCategory.Vertex );
			string formatStr = string.Empty;

			if( IsSRP )
				formatStr = "TransformObjectToWorldDir({0}.xyz)";
			else
				formatStr = "UnityObjectToWorldDir({0})";

			string worldTangentValue = string.Format( formatStr, vertexTangent );
			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT3, PrecisionType.Float, worldTangentValue, useMasterNodeCategory, customCategory );
			return varName;
		}

		public string GetTangentSign( PrecisionType precisionType, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string varName = GeneratorUtils.VertexTangentSignStr;
			if( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;

			string tangentValue = GetVertexTangent( precisionType, false, MasterNodePortCategory.Vertex );
			string tangentSignValue = string.Format( "{0}.w * unity_WorldTransformParams.w", tangentValue );
			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT, PrecisionType.Float, tangentSignValue, useMasterNodeCategory, customCategory );
			return varName;
		}


		public string GetWorldBinormal( PrecisionType precisionType, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string result = string.Empty;
			if( GetCustomInterpolatedData( TemplateInfoOnSematics.WORLD_BITANGENT, WirePortDataType.FLOAT3, precisionType, ref result, useMasterNodeCategory, customCategory ) )
			{
				return result;
			}

			string varName = GeneratorUtils.WorldBitangentStr;
			if( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;

			string worldNormal = GetWorldNormal( precisionType, false, MasterNodePortCategory.Vertex );
			string worldtangent = GetWorldTangent( precisionType, false, MasterNodePortCategory.Vertex );
			string tangentSign = GetTangentSign( precisionType, false, MasterNodePortCategory.Vertex );
			string worldBinormal = string.Format( "cross( {0}, {1} ) * {2}", worldNormal, worldtangent, tangentSign );
			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT3, PrecisionType.Float, worldBinormal, useMasterNodeCategory, customCategory );
			return varName;
		}

		public string GetWorldReflection( PrecisionType precisionType, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment, bool normalize = false )
		{
			string varName = GeneratorUtils.WorldReflectionStr;//UIUtils.GetInputValueFromType( SurfaceInputs.WORLD_REFL );
			if( normalize )
				varName = "normalized" + varName;

			if( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;

			string worldNormal = GetWorldNormal( precisionType );
			string worldViewDir = GetViewDir();
			string worldRefl = string.Format( "reflect(-{0}, {1})", worldViewDir, worldNormal );

			if( normalize )
				worldRefl = string.Format( "normalize( {0} )", worldRefl );

			m_currentDataCollector.AddLocalVariable( -1, precisionType, WirePortDataType.FLOAT3, varName, worldRefl );
			return varName;
		}

		public string GetWorldReflection( PrecisionType precisionType, string normal )
		{
			string tanToWorld0 = string.Empty;
			string tanToWorld1 = string.Empty;
			string tanToWorld2 = string.Empty;

			GetWorldTangentTf( precisionType, out tanToWorld0, out tanToWorld1, out tanToWorld2 );
			string worldRefl = GetViewDir();

			return string.Format( "reflect( -{0}, float3( dot( {2}, {1} ), dot( {3}, {1} ), dot( {4}, {1} ) ) )", worldRefl, normal, tanToWorld0, tanToWorld1, tanToWorld2 );
		}

		public string GetLightAtten( int uniqueId, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			//string result = string.Empty;
			//if( GetCustomInterpolatedData( TemplateInfoOnSematics.WORLD_POSITION, PrecisionType.Float, ref result, useMasterNodeCategory, customCategory ) )
			//{
			//	return result;
			//}

			//string varName = GeneratorUtils.WorldPositionStr;//UIUtils.GetInputValueFromType( SurfaceInputs.WORLD_POS );
			//if( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
			//	return varName;

			//if( !m_availableVertData.ContainsKey( TemplateInfoOnSematics.POSITION ) )
			//{
			//	UIUtils.ShowMessage( "Attempting to access inexisting vertex position to calculate world pos" );
			//	return "fixed3(0,0,0)";
			//}

			//string vertexPos = m_availableVertData[ TemplateInfoOnSematics.POSITION ].VarName;
			//string worldPosConversion = string.Format( "mul(unity_ObjectToWorld, {0}).xyz", vertexPos );

			//RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT3, PrecisionType.Float, worldPosConversion, useMasterNodeCategory, customCategory );
			//return varName;

			m_currentDataCollector.AddToIncludes( uniqueId, Constants.UnityAutoLightLib );
			m_currentDataCollector.AddToDefines( uniqueId, "ASE_SHADOWS 1" );
#if UNITY_5_6_OR_NEWER
				RequestMacroInterpolator( "UNITY_SHADOW_COORDS" );
#else
			RequestMacroInterpolator( "SHADOW_COORDS" );
			m_currentDataCollector.AddToPragmas( uniqueId, "multi_compile_fwdbase" );
#endif
			//string vOutName = CurrentTemplateData.VertexFunctionData.OutVarName;
			string fInName = CurrentTemplateData.FragmentFunctionData.InVarName;
			string worldPos = GetWorldPos();
			m_currentDataCollector.AddLocalVariable( uniqueId, "UNITY_LIGHT_ATTENUATION(ase_atten, " + fInName + ", " + worldPos + ")" );
			return "ase_atten";

		}

		public string GenerateObjectScale( ref MasterNodeDataCollector dataCollector, int uniqueId )
		{
			string value = string.Empty;

			if( m_currentSRPType == TemplateSRPType.HD )
			{
				value = "float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) )";
			}
			else
			{
				value = "float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) )";
			}
			dataCollector.AddLocalVariable( uniqueId, PrecisionType.Float, WirePortDataType.FLOAT3, GeneratorUtils.ObjectScaleStr, value );
			return GeneratorUtils.ObjectScaleStr;
		}

		public string GetWorldPos( bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string result = string.Empty;
			if( GetCustomInterpolatedData( TemplateInfoOnSematics.WORLD_POSITION, WirePortDataType.FLOAT3, PrecisionType.Float, ref result, useMasterNodeCategory, customCategory ) )
			{
				return result;
			}
			else if( m_currentSRPType == TemplateSRPType.HD )
			{
				if( GetCustomInterpolatedData( TemplateInfoOnSematics.RELATIVE_WORLD_POS, WirePortDataType.FLOAT3, PrecisionType.Float, ref result, useMasterNodeCategory, customCategory ) )
				{
					string worldPosVarName = GeneratorUtils.WorldPositionStr;
					string relWorldPosConversion = string.Format( "GetAbsolutePositionWS( {0} )", result );
					m_currentDataCollector.AddLocalVariable( -1, PrecisionType.Float, WirePortDataType.FLOAT3, worldPosVarName, relWorldPosConversion );
					return worldPosVarName;
				}
			}

			string varName = GeneratorUtils.WorldPositionStr;//UIUtils.GetInputValueFromType( SurfaceInputs.WORLD_POS );
			if( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;

			if( !m_availableVertData.ContainsKey( TemplateInfoOnSematics.POSITION ) )
			{
				UIUtils.ShowMessage( "Attempting to access inexisting vertex position to calculate world pos" );
				return "half3(0,0,0)";
			}

			string vertexPos = m_availableVertData[ TemplateInfoOnSematics.POSITION ].VarName;

			string worldPosConversion = string.Empty;
			if( m_currentSRPType == TemplateSRPType.HD )
			{
#if UNITY_2018_3_OR_NEWER
				worldPosConversion = string.Format( "GetAbsolutePositionWS( TransformObjectToWorld( ({0}).xyz ) )", vertexPos );
#else
				worldPosConversion = string.Format( "GetAbsolutePositionWS( mul( GetObjectToWorldMatrix(), {0}).xyz )", vertexPos );
#endif
			}
			else
			{
				worldPosConversion = string.Format( "mul(unity_ObjectToWorld, {0}).xyz", vertexPos );
			}
			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT3, PrecisionType.Float, worldPosConversion, useMasterNodeCategory, customCategory );
			return varName;
		}

		public string GetClipPosForValue( string customVertexPos, string outputId, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string varName = GeneratorUtils.ClipPositionStr + outputId;
			if( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;

			if( !m_availableVertData.ContainsKey( TemplateInfoOnSematics.POSITION ) )
			{
				UIUtils.ShowMessage( "Attempting to access inexisting vertex position to calculate clip pos" );
				return "half4(0,0,0,0)";
			}

			string formatStr = string.Empty;
			switch( m_currentSRPType )
			{
				default:
				case TemplateSRPType.BuiltIn:
				formatStr = "UnityObjectToClipPos({0})";
				break;
				case TemplateSRPType.HD:
				formatStr = "TransformWorldToHClip( TransformObjectToWorld({0}))";
				break;
				case TemplateSRPType.Lightweight:
				formatStr = "TransformObjectToHClip(({0}).xyz)";
				break;
			}

			string clipSpaceConversion = string.Format( formatStr, customVertexPos );
			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT4, PrecisionType.Float, clipSpaceConversion, useMasterNodeCategory, customCategory );
			return varName;
		}

		public string GetClipPos( bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string varName = GeneratorUtils.ClipPositionStr;// "clipPos";
			if( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;

			if( !m_availableVertData.ContainsKey( TemplateInfoOnSematics.POSITION ) )
			{
				UIUtils.ShowMessage( "Attempting to access inexisting vertex position to calculate clip pos" );
				return "half4(0,0,0,0)";
			}

			string vertexPos = m_availableVertData[ TemplateInfoOnSematics.POSITION ].VarName;

			string formatStr = string.Empty;
			switch( m_currentSRPType )
			{
				default:
				case TemplateSRPType.BuiltIn:
				formatStr = "UnityObjectToClipPos({0})";
				break;
				case TemplateSRPType.HD:
				formatStr = "TransformWorldToHClip( TransformObjectToWorld({0}))";
				break;
				case TemplateSRPType.Lightweight:
				formatStr = "TransformObjectToHClip(({0}).xyz)";
				break;
			}

			string clipSpaceConversion = string.Format( formatStr, vertexPos );
			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT4, PrecisionType.Float, clipSpaceConversion, useMasterNodeCategory, customCategory );
			return varName;
		}

		public string GetScreenPosForValue( string customVertexPos, string outputId, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string varName = UIUtils.GetInputValueFromType( SurfaceInputs.SCREEN_POS ) + outputId;
			if( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;

			string clipSpacePos = GetClipPosForValue( customVertexPos, outputId, false, MasterNodePortCategory.Vertex );
			string screenPosConversion = string.Empty;
			if( m_currentSRPType == TemplateSRPType.HD )
			{
				screenPosConversion = string.Format( "ComputeScreenPos( {0} , _ProjectionParams.x )", clipSpacePos );
			}
			else
			{
				screenPosConversion = string.Format( "ComputeScreenPos({0})", clipSpacePos );
			}
			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT4, PrecisionType.Float, screenPosConversion, useMasterNodeCategory, customCategory );
			return varName;
		}

		public string GetScreenPos( bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string varName = UIUtils.GetInputValueFromType( SurfaceInputs.SCREEN_POS );
			if( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;

			string clipSpacePos = GetClipPos( false, MasterNodePortCategory.Vertex );
			string screenPosConversion = string.Empty;
			if( m_currentSRPType == TemplateSRPType.HD )
			{
				screenPosConversion = string.Format( "ComputeScreenPos( {0} , _ProjectionParams.x )", clipSpacePos );
			}
			else
			{
				screenPosConversion = string.Format( "ComputeScreenPos({0})", clipSpacePos );
			}
			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT4, PrecisionType.Float, screenPosConversion, useMasterNodeCategory, customCategory );
			return varName;
		}

		public string GetScreenPosNormalized( bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string varName = GeneratorUtils.ScreenPositionNormalizedStr;// "norm" + UIUtils.GetInputValueFromType( SurfaceInputs.SCREEN_POS );
			string screenPos = GetScreenPos( useMasterNodeCategory, customCategory );
			string normalizedValue = string.Format( "float4 {0} = {1}/{1}.w;", varName, screenPos );
			string clipPlaneTestOp = string.Format( "{0}.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? {0}.z : {0}.z * 0.5 + 0.5;", varName );
			m_currentDataCollector.AddLocalVariable( -1, normalizedValue );
			m_currentDataCollector.AddLocalVariable( -1, clipPlaneTestOp );
			return varName;
		}

		public string GetViewDir( bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment, NormalizeType normalizeType = NormalizeType.Regular )
		{
			string result = string.Empty;
			if( GetCustomInterpolatedData( TemplateInfoOnSematics.WORLD_VIEW_DIR, WirePortDataType.FLOAT3, PrecisionType.Float, ref result, useMasterNodeCategory, customCategory ) )
				return result;

			string varName = GeneratorUtils.WorldViewDirectionStr;//UIUtils.GetInputValueFromType( SurfaceInputs.VIEW_DIR );
			if( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;

			string worldPos = GetWorldPos();

			string formatStr = string.Empty;
			if( IsSRP )
				formatStr = "( _WorldSpaceCameraPos.xyz - {0} )";
			else
				formatStr = "UnityWorldSpaceViewDir({0})";

			string viewDir = string.Format( formatStr, worldPos );
			m_currentDataCollector.AddLocalVariable( -1, PrecisionType.Float, WirePortDataType.FLOAT3, varName, viewDir );

			switch( normalizeType )
			{
				default:
				case NormalizeType.Off:
				break;
				case NormalizeType.Regular:
				m_currentDataCollector.AddLocalVariable( -1, varName + " = normalize(" + varName + ");" );
				break;
				case NormalizeType.Safe:
				m_currentDataCollector.AddLocalVariable( -1, varName + " = " + TemplateHelperFunctions.SafeNormalize( m_currentDataCollector, varName ) + ";" );
				break;
			}


			//RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT3, PrecisionType.Float, viewDir, useMasterNodeCategory, customCategory );
			return varName;
		}

		public string GetTangentViewDir( PrecisionType precisionType, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment, NormalizeType normalizeType = NormalizeType.Regular )
		{
			string varName = GeneratorUtils.TangentViewDirectionStr;
			if( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;

			string tanToWorld0 = string.Empty;
			string tanToWorld1 = string.Empty;
			string tanToWorld2 = string.Empty;

			GetWorldTangentTf( precisionType, out tanToWorld0, out tanToWorld1, out tanToWorld2 );
			string viewDir = GetViewDir();
			string tanViewDir = string.Format( " {0} * {3}.x + {1} * {3}.y  + {2} * {3}.z", tanToWorld0, tanToWorld1, tanToWorld2, viewDir );

			m_currentDataCollector.AddLocalVariable( -1, PrecisionType.Float, WirePortDataType.FLOAT3, varName, tanViewDir );
			switch( normalizeType )
			{
				default:
				case NormalizeType.Off: break;
				case NormalizeType.Regular:
				m_currentDataCollector.AddLocalVariable( -1, varName + " = normalize(" + varName + ");" );
				break;
				case NormalizeType.Safe:
				m_currentDataCollector.AddLocalVariable( -1, varName + " = " + TemplateHelperFunctions.SafeNormalize( m_currentDataCollector, varName ) + ";" );
				break;
			}

			return varName;
		}

		public void GetWorldTangentTf( PrecisionType precisionType, out string tanToWorld0, out string tanToWorld1, out string tanToWorld2, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			tanToWorld0 = "tanToWorld0";
			tanToWorld1 = "tanToWorld1";
			tanToWorld2 = "tanToWorld2";

			if( HasCustomInterpolatedData( tanToWorld0, useMasterNodeCategory, customCategory ) ||
				 HasCustomInterpolatedData( tanToWorld1, useMasterNodeCategory, customCategory ) ||
				 HasCustomInterpolatedData( tanToWorld2, useMasterNodeCategory, customCategory ) )
				return;

			string worldTangent = GetWorldTangent( precisionType, useMasterNodeCategory, customCategory );
			string worldNormal = GetWorldNormal( precisionType, useMasterNodeCategory, customCategory );
			string worldBinormal = GetWorldBinormal( precisionType, useMasterNodeCategory, customCategory );

			string tanToWorldVar0 = string.Format( "float3( {0}.x, {1}.x, {2}.x )", worldTangent, worldBinormal, worldNormal );
			string tanToWorldVar1 = string.Format( "float3( {0}.y, {1}.y, {2}.y )", worldTangent, worldBinormal, worldNormal );
			string tanToWorldVar2 = string.Format( "float3( {0}.z, {1}.z, {2}.z )", worldTangent, worldBinormal, worldNormal );

			if( customCategory == MasterNodePortCategory.Vertex )
			{
				RegisterCustomInterpolatedData( tanToWorld0, WirePortDataType.FLOAT3, PrecisionType.Float, tanToWorldVar0, useMasterNodeCategory, customCategory );
				RegisterCustomInterpolatedData( tanToWorld1, WirePortDataType.FLOAT3, PrecisionType.Float, tanToWorldVar1, useMasterNodeCategory, customCategory );
				RegisterCustomInterpolatedData( tanToWorld2, WirePortDataType.FLOAT3, PrecisionType.Float, tanToWorldVar2, useMasterNodeCategory, customCategory );
			}
			else
			{
				m_currentDataCollector.AddLocalVariable( -1, precisionType, WirePortDataType.FLOAT3, tanToWorld0, tanToWorldVar0 );
				m_currentDataCollector.AddLocalVariable( -1, precisionType, WirePortDataType.FLOAT3, tanToWorld1, tanToWorldVar1 );
				m_currentDataCollector.AddLocalVariable( -1, precisionType, WirePortDataType.FLOAT3, tanToWorld2, tanToWorldVar2 );
			}
		}

		public string GetTangentToWorldMatrixFast( PrecisionType precisionType, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string worldTangent = GetWorldTangent( precisionType );
			string worldNormal = GetWorldNormal( precisionType );
			string worldBinormal = GetWorldBinormal( precisionType );

			string varName = GeneratorUtils.TangentToWorldFastStr;
			if( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;

			string result = string.Format( "float3x3({0}.x,{1}.x,{2}.x,{0}.y,{1}.y,{2}.y,{0}.z,{1}.z,{2}.z)", worldTangent, worldBinormal, worldNormal );
			m_currentDataCollector.AddLocalVariable( -1, precisionType, WirePortDataType.FLOAT3x3, GeneratorUtils.TangentToWorldFastStr, result );
			return GeneratorUtils.TangentToWorldFastStr;
		}

		public string GetTangentToWorldMatrixPrecise( PrecisionType precisionType, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment)
		{
			string worldToTangent = GetWorldToTangentMatrix( precisionType, useMasterNodeCategory, customCategory );
			GeneratorUtils.Add3x3InverseFunction( ref m_currentDataCollector, UIUtils.PrecisionWirePortToCgType( precisionType, WirePortDataType.FLOAT ) );
			m_currentDataCollector.AddLocalVariable( -1, precisionType, WirePortDataType.FLOAT3x3, GeneratorUtils.TangentToWorldPreciseStr, string.Format( GeneratorUtils.Inverse3x3Header, worldToTangent ) );
			return GeneratorUtils.TangentToWorldPreciseStr;
		}

		public string GetWorldToTangentMatrix( PrecisionType precisionType, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string worldTangent = GetWorldTangent( precisionType );
			string worldNormal = GetWorldNormal( precisionType );
			string worldBinormal = GetWorldBinormal( precisionType );

			string varName = GeneratorUtils.WorldToTangentStr;// "worldToTanMat";
			if( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;
			string worldTanMat = string.Format( "float3x3({0},{1},{2})", worldTangent, worldBinormal, worldNormal );

			m_currentDataCollector.AddLocalVariable( -1, PrecisionType.Float, WirePortDataType.FLOAT3x3, varName, worldTanMat );
			return varName;
		}

		public string GetObjectToViewPos( PrecisionType precisionType, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			string varName = "objectToViewPos";
			if( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;
			string vertexPos = GetVertexPosition( WirePortDataType.FLOAT3, precisionType, false, MasterNodePortCategory.Vertex );

			string formatStr = string.Empty;
			if( IsSRP )
				formatStr = "TransformWorldToView(TransformObjectToWorld({0}))";
			else
				formatStr = "UnityObjectToViewPos({0})";

			string objectToViewPosValue = string.Format( formatStr, vertexPos );
			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT3, PrecisionType.Float, objectToViewPosValue, useMasterNodeCategory, customCategory );
			return varName;
		}

		public string GetEyeDepth( PrecisionType precisionType, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment, int viewSpace = 0 )
		{
			string varName = "eyeDepth";
			if( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;
			string objectToView = GetObjectToViewPos( precisionType, false, MasterNodePortCategory.Vertex );
			string eyeDepthValue = string.Format( "-{0}.z", objectToView );
			if( viewSpace == 1 )
			{
				eyeDepthValue += " * _ProjectionParams.w";
			}

			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT, PrecisionType.Float, eyeDepthValue, useMasterNodeCategory, customCategory );
			return varName;
		}

		public string GetObjectSpaceLightDir( PrecisionType precisionType, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			if( !IsSRP )
			{
				m_currentDataCollector.AddToIncludes( -1, Constants.UnityLightingLib );
				m_currentDataCollector.AddToIncludes( -1, Constants.UnityAutoLightLib );
			}

			string varName = "objectSpaceLightDir";

			if( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;

			string vertexPos = GetVertexPosition( WirePortDataType.FLOAT4, precisionType, false, MasterNodePortCategory.Vertex );

			string objectSpaceLightDir = string.Empty;
			switch( m_currentSRPType )
			{
				default:
				case TemplateSRPType.BuiltIn:
				objectSpaceLightDir = string.Format( "ObjSpaceLightDir({0})", vertexPos );
				break;
				case TemplateSRPType.HD:
				string worldSpaceLightDir = GetWorldSpaceLightDir( useMasterNodeCategory, customCategory );
				objectSpaceLightDir = string.Format( "mul( GetWorldToObjectMatrix(), {0} ).xyz", worldSpaceLightDir );
				break;
				case TemplateSRPType.Lightweight:
				objectSpaceLightDir = "mul( unity_WorldToObject, _MainLightPosition ).xyz";
				break;
			}

			RegisterCustomInterpolatedData( varName, WirePortDataType.FLOAT3, PrecisionType.Float, objectSpaceLightDir, useMasterNodeCategory, customCategory );
			return varName;
		}

		public string GetWorldSpaceLightDir( bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			if( !IsSRP )
			{
				m_currentDataCollector.AddToIncludes( -1, Constants.UnityLightingLib );
				m_currentDataCollector.AddToIncludes( -1, Constants.UnityAutoLightLib );
			}
			else
			{

				string lightVar;
				if( m_currentSRPType == TemplateSRPType.HD )
				{
					AddHDLightInfo();
					lightVar = string.Format( TemplateHelperFunctions.HDLightInfoFormat, "0", "forward" );
				}
				else
				{
					lightVar = "_MainLightPosition.xyz";
				}
				return m_currentDataCollector.SafeNormalizeLightDir ? string.Format( "SafeNormalize({0})", lightVar ) : lightVar;
			}

			string varName = "worldSpaceLightDir";
			if( HasCustomInterpolatedData( varName, useMasterNodeCategory, customCategory ) )
				return varName;

			string worldPos = GetWorldPos( useMasterNodeCategory, customCategory );
			string worldSpaceLightDir = string.Format( "UnityWorldSpaceLightDir({0})", worldPos );
			if( m_currentDataCollector.SafeNormalizeLightDir )
			{
				if( IsSRP )
				{
					worldSpaceLightDir = string.Format( "SafeNormalize{0})", worldSpaceLightDir );
				}
				else
				{
					m_currentDataCollector.AddToIncludes( -1, Constants.UnityBRDFLib );
					worldSpaceLightDir = string.Format( "Unity_SafeNormalize({0})", worldSpaceLightDir );
				}
			}

			m_currentDataCollector.AddLocalVariable( -1, PrecisionType.Float, WirePortDataType.FLOAT3, varName, worldSpaceLightDir );
			return varName;
		}

		public void RegisterCustomInterpolatedData( string name, WirePortDataType dataType, PrecisionType precision, string vertexInstruction, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			bool addLocalVariable = !name.Equals( vertexInstruction );

			MasterNodePortCategory category = useMasterNodeCategory ? m_currentDataCollector.PortCategory : customCategory;

			if( !m_customInterpolatedData.ContainsKey( name ) )
			{
				m_customInterpolatedData.Add( name, new TemplateCustomData( name, dataType ) );
			}

			if( !m_customInterpolatedData[ name ].IsVertex )
			{
				m_customInterpolatedData[ name ].IsVertex = true;
				if( addLocalVariable )
					m_currentDataCollector.AddToVertexLocalVariables( -1, precision, dataType, name, vertexInstruction );
			}

			if( category == MasterNodePortCategory.Fragment )
			{
				if( !m_customInterpolatedData[ name ].IsFragment )
				{
					m_customInterpolatedData[ name ].IsFragment = true;
					TemplateVertexData interpData = RequestNewInterpolator( dataType, false );
					if( interpData == null )
					{
						Debug.LogErrorFormat( "Could not assign interpolator of type {0} to variable {1}", dataType, name );
						return;
					}

					m_currentDataCollector.AddToVertexLocalVariables( -1, m_currentTemplateData.VertexFunctionData.OutVarName + "." + interpData.VarNameWithSwizzle, name );
					m_currentDataCollector.AddToLocalVariables( -1, precision, dataType, name, m_currentTemplateData.FragFunctionData.InVarName + "." + interpData.VarNameWithSwizzle );
				}
			}
		}

		public bool HasCustomInterpolatedData( string name, bool useMasterNodeCategory = true, MasterNodePortCategory customCategory = MasterNodePortCategory.Fragment )
		{
			if( m_customInterpolatedData.ContainsKey( name ) )
			{
				MasterNodePortCategory category = useMasterNodeCategory ? m_currentDataCollector.PortCategory : customCategory;
				return ( category == MasterNodePortCategory.Fragment ) ? m_customInterpolatedData[ name ].IsFragment : m_customInterpolatedData[ name ].IsVertex;
			}
			return false;
		}

		public bool HasFragmentInputParams
		{
			get
			{
				if( m_fragmentInputParams != null )
					return m_fragmentInputParams.Count > 0;

				return false;
			}
		}

		public string FragInputParamsStr
		{
			get
			{
				string value = string.Empty;
				if( m_fragmentInputParams != null && m_fragmentInputParams.Count > 0 )
				{
					int count = m_fragmentInputParams.Count;
					if( count > 0 )
					{
						value = ", ";
						foreach( KeyValuePair<TemplateSemantics, TemplateInputParameters> kvp in m_fragmentInputParams )
						{
							value += kvp.Value.Declaration;

							if( --count > 0 )
							{
								value += " , ";
							}
						}
					}
				}
				return value;
			}
		}

		public string VertexInputParamsStr
		{
			get
			{
				string value = string.Empty;
				if( m_vertexInputParams != null && m_vertexInputParams.Count > 0 )
				{
					int count = m_vertexInputParams.Count;
					if( count > 0 )
					{
						value = ", ";
						foreach( KeyValuePair<TemplateSemantics, TemplateInputParameters> kvp in m_vertexInputParams )
						{
							value += kvp.Value.Declaration;

							if( --count > 0 )
							{
								value += " , ";
							}
						}
					}
				}
				return value;
			}
		}

		public void Destroy()
		{
			m_currentTemplate = null;

			m_currentTemplateData = null;

			m_currentDataCollector = null;

			if( m_lateDirectivesList != null )
			{
				m_lateDirectivesList.Clear();
				m_lateDirectivesList = null;
			}

			if( m_lateDirectivesDict != null )
			{
				m_lateDirectivesDict.Clear();
				m_lateDirectivesDict = null;
			}

			if( m_registeredVertexData != null )
			{
				m_registeredVertexData.Clear();
				m_registeredVertexData = null;
			}

			if( m_vertexInputParams != null )
			{
				m_vertexInputParams.Clear();
				m_vertexInputParams = null;
			}

			if( m_fragmentInputParams != null )
			{
				m_fragmentInputParams.Clear();
				m_fragmentInputParams = null;
			}

			if( m_vertexDataDict != null )
			{
				m_vertexDataDict.Clear();
				m_vertexDataDict = null;
			}

			if( m_interpolatorData != null )
			{
				m_interpolatorData.Destroy();
				m_interpolatorData = null;
			}

			if( m_availableFragData != null )
			{
				m_availableFragData.Clear();
				m_availableFragData = null;
			}

			if( m_availableVertData != null )
			{
				m_availableVertData.Clear();
				m_availableVertData = null;
			}

			if( m_customInterpolatedData != null )
			{
				m_customInterpolatedData.Clear();
				m_customInterpolatedData = null;
			}

			if( m_specialVertexLocalVars != null )
			{
				m_specialVertexLocalVars.Clear();
				m_specialVertexLocalVars = null;
			}

			if( m_specialFragmentLocalVars != null )
			{
				m_specialFragmentLocalVars.Clear();
				m_specialFragmentLocalVars = null;
			}
		}

		public Dictionary<TemplateSemantics, TemplateInputParameters> FragInputParameters { get { return m_fragmentInputParams; } }

		public bool HasVertexInputParams
		{
			get
			{
				if( m_vertexInputParams != null )
					return m_vertexInputParams.Count > 0;

				return false;
			}
		}

		public Dictionary<TemplateSemantics, TemplateInputParameters> VertexInputParameters { get { return m_vertexInputParams; } }
		public TemplateData CurrentTemplateData { get { return m_currentTemplateData; } }
		public int MultipassSubshaderIdx { get { return m_multipassSubshaderIdx; } }
		public int MultipassPassIdx { get { return m_multipassPassIdx; } }
		public TemplateSRPType CurrentSRPType { get { return m_currentSRPType; } }
		public bool IsHDRP { get { return m_currentSRPType == TemplateSRPType.HD; } }
		public bool IsLWRP { get { return m_currentSRPType == TemplateSRPType.Lightweight; } }
		public bool IsSRP { get { return ( m_currentSRPType == TemplateSRPType.Lightweight || m_currentSRPType == TemplateSRPType.HD ); } }
		public TemplateInterpData InterpData { get { return m_interpolatorData; } }
		public List<PropertyDataCollector> LateDirectivesList { get { return m_lateDirectivesList; } }
	}
}
