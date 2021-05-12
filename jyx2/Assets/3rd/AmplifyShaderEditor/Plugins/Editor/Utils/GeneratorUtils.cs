// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	public static class GeneratorUtils
	{
		public const string ObjectScaleStr = "ase_objectScale";
		public const string ScreenDepthStr = "ase_screenDepth";
		public const string ViewPositionStr = "ase_viewPos";
		public const string WorldViewDirectionStr = "ase_worldViewDir";
		public const string TangentViewDirectionStr = "ase_tanViewDir";
		public const string NormalizedViewDirStr = "ase_normViewDir";
		public const string ClipPositionStr = "ase_clipPos";
		public const string VertexPosition3Str = "ase_vertex3Pos";
		public const string VertexPosition4Str = "ase_vertex4Pos";
		public const string VertexNormalStr = "ase_vertexNormal";
		public const string VertexTangentStr = "ase_vertexTangent";
		public const string VertexTangentSignStr = "ase_vertexTangentSign";
		public const string VertexBitangentStr = "ase_vertexBitangent";
		public const string ScreenPositionStr = "ase_screenPos";
		public const string ScreenPositionNormalizedStr = "ase_screenPosNorm";
		public const string GrabScreenPositionStr = "ase_grabScreenPos";
		public const string GrabScreenPositionNormalizedStr = "ase_grabScreenPosNorm";
		public const string WorldPositionStr = "ase_worldPos";
		public const string RelativeWorldPositionStr = "ase_relWorldPos";
		public const string WorldLightDirStr = "ase_worldlightDir";
		public const string ObjectLightDirStr = "ase_objectlightDir";
		public const string WorldNormalStr = "ase_worldNormal";
		public const string NormalizedWorldNormalStr = "ase_normWorldNormal";
		public const string WorldReflectionStr = "ase_worldReflection";
		public const string WorldTangentStr = "ase_worldTangent";
		public const string WorldBitangentStr = "ase_worldBitangent";
		public const string WorldToTangentStr = "ase_worldToTangent";
		public const string ObjectToTangentStr = "ase_objectToTangent";
		public const string TangentToWorldPreciseStr = "ase_tangentToWorldPrecise";
		public const string TangentToWorldFastStr = "ase_tangentToWorldFast";
		public const string TangentToObjectStr = "ase_tangentToObject";
		public const string TangentToObjectFastStr = "ase_tangentToObjectFast";
		private const string Float3Format = "float3 {0} = {1};";
		private const string Float4Format = "float4 {0} = {1};";
		private const string GrabFunctionHeader = "inline float4 ASE_ComputeGrabScreenPos( float4 pos )";
		private const string GrabFunctionCall = "ASE_ComputeGrabScreenPos( {0} )";
		private static readonly string[] GrabFunctionBody = {
			"#if UNITY_UV_STARTS_AT_TOP",
			"float scale = -1.0;",
			"#else",
			"float scale = 1.0;",
			"#endif",
			"float4 o = pos;",
			"o.y = pos.w * 0.5f;",
			"o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;",
			"return o;"
		};

		// OBJECT SCALE
		static public string GenerateObjectScale( ref MasterNodeDataCollector dataCollector, int uniqueId )
		{
			if( dataCollector.IsTemplate )
				return dataCollector.TemplateDataCollectorInstance.GenerateObjectScale( ref dataCollector, uniqueId );

			//string value= "1/float3( length( unity_WorldToObject[ 0 ].xyz ), length( unity_WorldToObject[ 1 ].xyz ), length( unity_WorldToObject[ 2 ].xyz ) );";
			string value = "float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) )";
			dataCollector.AddLocalVariable( uniqueId, PrecisionType.Float, WirePortDataType.FLOAT3, ObjectScaleStr, value );
			return ObjectScaleStr;
		}

		// WORLD POSITION
		static public string GenerateWorldPosition( ref MasterNodeDataCollector dataCollector, int uniqueId )
		{
			if( dataCollector.IsTemplate )
				return dataCollector.TemplateDataCollectorInstance.GetWorldPos();

			dataCollector.AddToInput( -1, SurfaceInputs.WORLD_POS );

			string result = Constants.InputVarStr + ".worldPos";

			if( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
				result = "mul( unity_ObjectToWorld, " + Constants.VertexShaderInputStr + ".vertex )";

			dataCollector.AddToLocalVariables( dataCollector.PortCategory, uniqueId, string.Format( Float3Format, WorldPositionStr, result ) );

			return WorldPositionStr;
		}

		// WORLD REFLECTION
		static public string GenerateWorldReflection( ref MasterNodeDataCollector dataCollector, int uniqueId, bool normalize = false )
		{
			if( dataCollector.IsTemplate )
				return dataCollector.TemplateDataCollectorInstance.GetWorldReflection( UIUtils.CurrentWindow.CurrentGraph.CurrentPrecision, true, MasterNodePortCategory.Fragment, normalize );

			string precisionType = UIUtils.PrecisionWirePortToCgType( UIUtils.CurrentWindow.CurrentGraph.CurrentPrecision, WirePortDataType.FLOAT3 );
			string result = string.Empty;
			if( !dataCollector.DirtyNormal )
				result = Constants.InputVarStr + ".worldRefl";
			else
				result = "WorldReflectionVector( " + Constants.InputVarStr + ", " + precisionType + "( 0, 0, 1 ) )";

			if( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
				result = "UnityObjectToWorldNormal( " + Constants.VertexShaderInputStr + ".normal )";
			if( normalize )
			{
				result = string.Format( "normalize( {0} )", result );
			}

			dataCollector.AddToLocalVariables( dataCollector.PortCategory, uniqueId, string.Concat( precisionType, " ", WorldReflectionStr, " = ", result, ";" ) );
			return WorldReflectionStr;
		}

		// WORLD NORMAL
		static public string GenerateWorldNormal( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precisionType, string normal, string outputId )
		{
			if( dataCollector.IsTemplate )
				return dataCollector.TemplateDataCollectorInstance.GetWorldNormal( uniqueId, precisionType, normal, outputId );

			string tanToWorld = GenerateTangentToWorldMatrixFast( ref dataCollector, uniqueId, precisionType );
			return string.Format( "mul({0},{1})", tanToWorld, normal );

		}
		static public string GenerateWorldNormal( ref MasterNodeDataCollector dataCollector, int uniqueId, bool normalize = false )
		{
			if( dataCollector.IsTemplate )
				return dataCollector.TemplateDataCollectorInstance.GetWorldNormal( UIUtils.CurrentWindow.CurrentGraph.CurrentPrecision, true, MasterNodePortCategory.Fragment, normalize );

			string precisionType = UIUtils.PrecisionWirePortToCgType( UIUtils.CurrentWindow.CurrentGraph.CurrentPrecision, WirePortDataType.FLOAT3 );
			string result = string.Empty;
			if( !dataCollector.DirtyNormal )
				result = Constants.InputVarStr + ".worldNormal";
			else
				result = "WorldNormalVector( " + Constants.InputVarStr + ", " + precisionType + "( 0, 0, 1 ) )";

			if( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
				result = "UnityObjectToWorldNormal( " + Constants.VertexShaderInputStr + ".normal )";

			dataCollector.AddToLocalVariables( dataCollector.PortCategory, uniqueId, string.Concat( precisionType, " ", WorldNormalStr, " = ", result, ";" ) );
			if( normalize )
			{
				dataCollector.AddToLocalVariables( dataCollector.PortCategory, uniqueId, string.Concat( precisionType, " ", NormalizedWorldNormalStr, " = normalize( ", WorldNormalStr, " );" ) );
				return NormalizedWorldNormalStr;
			}
			return WorldNormalStr;
		}

		// WORLD TANGENT
		static public string GenerateWorldTangent( ref MasterNodeDataCollector dataCollector, int uniqueId )
		{
			if( dataCollector.IsTemplate )
				return dataCollector.TemplateDataCollectorInstance.GetWorldTangent( UIUtils.CurrentWindow.CurrentGraph.CurrentPrecision );

			string precisionType = UIUtils.PrecisionWirePortToCgType( UIUtils.CurrentWindow.CurrentGraph.CurrentPrecision, WirePortDataType.FLOAT3 );
			string result = "WorldNormalVector( " + Constants.InputVarStr + ", " + precisionType + "( 1, 0, 0 ) )";

			if( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
				result = "UnityObjectToWorldDir( " + Constants.VertexShaderInputStr + ".tangent.xyz )";

			dataCollector.AddToLocalVariables( dataCollector.PortCategory, uniqueId, string.Concat( precisionType, " ", WorldTangentStr, " = ", result, ";" ) );
			return WorldTangentStr;
		}

		// WORLD BITANGENT
		static public string GenerateWorldBitangent( ref MasterNodeDataCollector dataCollector, int uniqueId )
		{
			if( dataCollector.IsTemplate )
				return dataCollector.TemplateDataCollectorInstance.GetWorldBinormal( UIUtils.CurrentWindow.CurrentGraph.CurrentPrecision );

			string precisionType = UIUtils.PrecisionWirePortToCgType( UIUtils.CurrentWindow.CurrentGraph.CurrentPrecision, WirePortDataType.FLOAT3 );
			string result = "WorldNormalVector( " + Constants.InputVarStr + ", " + precisionType + "( 0, 1, 0 ) )";

			if( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				string worldNormal = GenerateWorldNormal( ref dataCollector, uniqueId );
				string worldTangent = GenerateWorldTangent( ref dataCollector, uniqueId );
				dataCollector.AddToVertexLocalVariables( uniqueId, string.Format( "half tangentSign = {0}.tangent.w * unity_WorldTransformParams.w;", Constants.VertexShaderInputStr ) );
				result = "cross( " + worldNormal + ", " + worldTangent + " ) * tangentSign";
			}

			dataCollector.AddToLocalVariables( dataCollector.PortCategory, uniqueId, string.Concat( precisionType, " ", WorldBitangentStr, " = ", result, ";" ) );
			return WorldBitangentStr;
		}

		// OBJECT TO TANGENT MATRIX
		static public string GenerateObjectToTangentMatrix( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision )
		{
			string normal = GenerateVertexNormal( ref dataCollector, uniqueId, precision );
			string tangent = GenerateVertexTangent( ref dataCollector, uniqueId, precision );
			string bitangen = GenerateVertexBitangent( ref dataCollector, uniqueId, precision );
			dataCollector.AddLocalVariable( uniqueId, precision, WirePortDataType.FLOAT3x3, ObjectToTangentStr, "float3x3( " + tangent + ", " + bitangen + ", " + normal + " )" );
			return ObjectToTangentStr;
		}

		// TANGENT TO OBJECT
		//static public string GenerateTangentToObjectMatrixFast( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision )
		//{
		//	string normal = GenerateVertexNormal( ref dataCollector, uniqueId, precision );
		//	string tangent = GenerateVertexTangent( ref dataCollector, uniqueId, precision );
		//	string bitangent = GenerateVertexBitangent( ref dataCollector, uniqueId, precision );

		//	string result = string.Format( "float3x3({0}.x,{1}.x,{2}.x,{0}.y,{1}.y,{2}.y,{0}.z,{1}.z,{2}.z)",tangent,bitangent,normal );
		//	dataCollector.AddLocalVariable( uniqueId, precision, WirePortDataType.FLOAT3x3, TangentToObjectFastStr, result );
		//	return TangentToObjectFastStr;
		//}

		//static public string GenerateTangentToObjectMatrixPrecise( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision )
		//{
		//	string objectToTangent = GenerateObjectToTangentMatrix( ref dataCollector, uniqueId, precision );
		//	Add3x3InverseFunction( ref dataCollector, UIUtils.PrecisionWirePortToCgType( precision, WirePortDataType.FLOAT ) );
		//	dataCollector.AddLocalVariable( uniqueId, precision, WirePortDataType.FLOAT3x3, TangentToObjectStr, string.Format( Inverse3x3Header, objectToTangent ) );
		//	return TangentToObjectStr;
		//}

		// WORLD TO TANGENT MATRIX
		static public string GenerateWorldToTangentMatrix( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision )
		{
			if( dataCollector.IsTemplate )
				return dataCollector.TemplateDataCollectorInstance.GetWorldToTangentMatrix( precision );

			if( dataCollector.IsFragmentCategory )
			{
				dataCollector.ForceNormal = true;

				dataCollector.AddToInput( -1, SurfaceInputs.WORLD_NORMAL, precision );
				dataCollector.AddToInput( -1, SurfaceInputs.INTERNALDATA, addSemiColon: false );
			}

			string worldNormal = GenerateWorldNormal( ref dataCollector, uniqueId );
			string worldTangent = GenerateWorldTangent( ref dataCollector, uniqueId );
			string worldBitangent = GenerateWorldBitangent( ref dataCollector, uniqueId );

			dataCollector.AddLocalVariable( uniqueId, precision, WirePortDataType.FLOAT3x3, WorldToTangentStr, "float3x3( " + worldTangent + ", " + worldBitangent + ", " + worldNormal + " )" );
			return WorldToTangentStr;
		}

		// TANGENT TO WORLD
		static public string GenerateTangentToWorldMatrixFast( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision )
		{
			if( dataCollector.IsTemplate )
				return dataCollector.TemplateDataCollectorInstance.GetTangentToWorldMatrixFast( precision );

			if( dataCollector.IsFragmentCategory )
			{
				dataCollector.ForceNormal = true;

				dataCollector.AddToInput( -1, SurfaceInputs.WORLD_NORMAL, precision );
				dataCollector.AddToInput( -1, SurfaceInputs.INTERNALDATA, addSemiColon: false );
			}

			string worldNormal = GenerateWorldNormal( ref dataCollector, uniqueId );
			string worldTangent = GenerateWorldTangent( ref dataCollector, uniqueId );
			string worldBitangent = GenerateWorldBitangent( ref dataCollector, uniqueId );

			string result = string.Format( "float3x3({0}.x,{1}.x,{2}.x,{0}.y,{1}.y,{2}.y,{0}.z,{1}.z,{2}.z)", worldTangent, worldBitangent, worldNormal );
			dataCollector.AddLocalVariable( uniqueId, precision, WirePortDataType.FLOAT3x3, TangentToWorldFastStr, result );
			return TangentToWorldFastStr;
		}

		static public string GenerateTangentToWorldMatrixPrecise( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision )
		{
			if( dataCollector.IsTemplate )
				return dataCollector.TemplateDataCollectorInstance.GetTangentToWorldMatrixPrecise( precision );

			if( dataCollector.IsFragmentCategory )
			{
				dataCollector.ForceNormal = true;

				dataCollector.AddToInput( -1, SurfaceInputs.WORLD_NORMAL, precision );
				dataCollector.AddToInput( -1, SurfaceInputs.INTERNALDATA, addSemiColon: false );
			}

			string worldToTangent = GenerateWorldToTangentMatrix( ref dataCollector, uniqueId, precision );
			Add3x3InverseFunction( ref dataCollector, UIUtils.PrecisionWirePortToCgType( precision, WirePortDataType.FLOAT ) );
			dataCollector.AddLocalVariable( uniqueId, precision, WirePortDataType.FLOAT3x3, TangentToWorldPreciseStr, string.Format( Inverse3x3Header, worldToTangent ) );
			return TangentToWorldPreciseStr;
		}

		// AUTOMATIC UVS
		static public string GenerateAutoUVs( ref MasterNodeDataCollector dataCollector, int uniqueId, int index, string propertyName = null, WirePortDataType size = WirePortDataType.FLOAT2, string scale = null, string offset = null, string outputId = null )
		{
			string result = string.Empty;
			string varName = string.Empty;

			string indexStr = index > 0 ? ( index + 1 ).ToString() : "";

			if( dataCollector.PortCategory == MasterNodePortCategory.Fragment || dataCollector.PortCategory == MasterNodePortCategory.Debug )
			{
				string sizeDif = string.Empty;
				if( size == WirePortDataType.FLOAT3 )
					sizeDif = "3";
				else if( size == WirePortDataType.FLOAT4 )
					sizeDif = "4";

				string dummyPropUV = "_tex" + sizeDif + "coord" + indexStr;
				string dummyUV = "uv" + indexStr + dummyPropUV;

				dataCollector.AddToProperties( uniqueId, "[HideInInspector] " + dummyPropUV + "( \"\", 2D ) = \"white\" {}", 100 );
				dataCollector.AddToInput( uniqueId, dummyUV, size );

				result = Constants.InputVarStr + "." + dummyUV;
			}
			else
			{
				result = Constants.VertexShaderInputStr + ".texcoord";
				if( index > 0 )
				{
					result += index.ToString();
				}

				switch( size )
				{
					default:
					case WirePortDataType.FLOAT2:
					{
						result += ".xy";
					}
					break;
					case WirePortDataType.FLOAT3:
					{
						result += ".xyz";
					}
					break;
					case WirePortDataType.FLOAT4: break;
				}
			}

			varName = "uv" + indexStr + "_TexCoord" + outputId;

			if( !string.IsNullOrEmpty( propertyName ) )
			{
				dataCollector.AddToUniforms( uniqueId, "uniform float4 " + propertyName + "_ST;" );
				if( size > WirePortDataType.FLOAT2 )
				{
					dataCollector.UsingHigherSizeTexcoords = true;
					dataCollector.AddToLocalVariables( dataCollector.PortCategory, uniqueId, PrecisionType.Float, size, "uv" + propertyName, result );
					dataCollector.AddToLocalVariables( dataCollector.PortCategory, uniqueId, "uv" + propertyName + ".xy = " + result + ".xy * " + propertyName + "_ST.xy + " + propertyName + "_ST.zw;" );
				}
				else
				{
					dataCollector.AddToLocalVariables( dataCollector.PortCategory, uniqueId, PrecisionType.Float, size, "uv" + propertyName, result + " * " + propertyName + "_ST.xy + " + propertyName + "_ST.zw" );
				}

				result = "uv" + propertyName;
			}
			else if( !string.IsNullOrEmpty( scale ) || !string.IsNullOrEmpty( offset ) )
			{
				if( size > WirePortDataType.FLOAT2 )
				{
					dataCollector.UsingHigherSizeTexcoords = true;
					dataCollector.AddToLocalVariables( dataCollector.PortCategory, uniqueId, PrecisionType.Float, size, varName, result );
					dataCollector.AddToLocalVariables( dataCollector.PortCategory, uniqueId, varName + ".xy = " + result + ".xy" + ( string.IsNullOrEmpty( scale ) ? "" : " * " + scale ) + ( string.IsNullOrEmpty( offset ) ? "" : " + " + offset ) + ";" );
				}
				else
				{
					dataCollector.AddToLocalVariables( dataCollector.PortCategory, uniqueId, PrecisionType.Float, size, varName, result + ( string.IsNullOrEmpty( scale ) ? "" : " * " + scale ) + ( string.IsNullOrEmpty( offset ) ? "" : " + " + offset ) );
				}

				result = varName;
			}
			else if( dataCollector.PortCategory == MasterNodePortCategory.Fragment )
			{
				if( size > WirePortDataType.FLOAT2 )
					dataCollector.UsingHigherSizeTexcoords = true;
			}

			return result;
		}

		// SCREEN POSITION NORMALIZED
		static public string GenerateScreenPositionNormalizedForValue( string customVertexPos, string outputId, ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision, bool addInput = true )
		{
			string stringPosVar = GenerateScreenPositionForValue( customVertexPos, outputId, ref dataCollector, uniqueId, precision, addInput );
			string varName = ScreenPositionNormalizedStr + uniqueId;
			dataCollector.AddLocalVariable( uniqueId, string.Format( "float4 {0} = {1} / {1}.w;", varName, stringPosVar ) );
			dataCollector.AddLocalVariable( uniqueId, varName + ".z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? " + varName + ".z : " + varName + ".z * 0.5 + 0.5;" );

			return varName;
		}
		static public string GenerateScreenPositionNormalized( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision, bool addInput = true )
		{
			string stringPosVar = GenerateScreenPosition( ref dataCollector, uniqueId, precision, addInput );

			dataCollector.AddLocalVariable( uniqueId, string.Format( "float4 {0} = {1} / {1}.w;", ScreenPositionNormalizedStr, stringPosVar ) );
			dataCollector.AddLocalVariable( uniqueId, ScreenPositionNormalizedStr + ".z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? " + ScreenPositionNormalizedStr + ".z : " + ScreenPositionNormalizedStr + ".z * 0.5 + 0.5;" );

			return ScreenPositionNormalizedStr;
		}

		// SCREEN POSITION
		static public string GenerateScreenPositionForValue( string customVertexPosition, string outputId, ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision, bool addInput = true )
		{
			if( dataCollector.IsTemplate )
				return dataCollector.TemplateDataCollectorInstance.GetScreenPosForValue( customVertexPosition, outputId );


			string value = GenerateVertexScreenPositionForValue( customVertexPosition, outputId, ref dataCollector, uniqueId, precision );
			string screenPosVarName = "screenPosition" + outputId;
			dataCollector.AddToInput( uniqueId, screenPosVarName, WirePortDataType.FLOAT4, precision );
			dataCollector.AddToVertexLocalVariables( uniqueId, Constants.VertexShaderOutputStr + "." + screenPosVarName + " = " + value + ";" );

			string screenPosVarNameOnFrag = ScreenPositionStr + outputId;
			string globalResult = Constants.InputVarStr + "." + screenPosVarName;
			dataCollector.AddLocalVariable( uniqueId, string.Format( "float4 {0} = {1};", screenPosVarNameOnFrag, globalResult ) );
			return screenPosVarNameOnFrag;

		}

		static public string GenerateScreenPosition( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision, bool addInput = true )
		{
			if( dataCollector.UsingCustomScreenPos && dataCollector.IsFragmentCategory )
			{
				string value = GenerateVertexScreenPosition( ref dataCollector, uniqueId, precision );
				dataCollector.AddToInput( uniqueId, "screenPosition", WirePortDataType.FLOAT4, precision );
				dataCollector.AddToVertexLocalVariables( uniqueId, Constants.VertexShaderOutputStr + ".screenPosition = " + value + ";" );

				string globalResult = Constants.InputVarStr + ".screenPosition";
				dataCollector.AddLocalVariable( uniqueId, string.Format( "float4 {0} = {1};", ScreenPositionStr, globalResult ) );
				return ScreenPositionStr;
			}
			else
			{
				if( !dataCollector.IsFragmentCategory )
					return GenerateVertexScreenPosition( ref dataCollector, uniqueId, precision );

				if( dataCollector.IsTemplate )
					return dataCollector.TemplateDataCollectorInstance.GetScreenPos();
			}


			if( addInput )
				dataCollector.AddToInput( uniqueId, SurfaceInputs.SCREEN_POS, precision );

			string result = Constants.InputVarStr + ".screenPos";
			dataCollector.AddLocalVariable( uniqueId, string.Format( "float4 {0} = float4( {1}.xyz , {1}.w + 0.00000000001 );", ScreenPositionStr, result ) );

			return ScreenPositionStr;
		}

		// GRAB SCREEN POSITION
		static public string GenerateGrabScreenPosition( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision, bool addInput = true, string customScreenPos = null )
		{
			string screenPos = string.Empty;
			if( string.IsNullOrEmpty( customScreenPos ) )
				screenPos = GenerateScreenPosition( ref dataCollector, uniqueId, precision, addInput );
			else
				screenPos = customScreenPos;

			string computeBody = string.Empty;
			IOUtils.AddFunctionHeader( ref computeBody, GrabFunctionHeader );
			foreach( string line in GrabFunctionBody )
				IOUtils.AddFunctionLine( ref computeBody, line );
			IOUtils.CloseFunctionBody( ref computeBody );
			string functionResult = dataCollector.AddFunctions( GrabFunctionCall, computeBody, screenPos );

			dataCollector.AddLocalVariable( uniqueId, precision, WirePortDataType.FLOAT4, GrabScreenPositionStr, functionResult );
			return GrabScreenPositionStr;
		}

		// GRAB SCREEN POSITION NORMALIZED
		static public string GenerateGrabScreenPositionNormalized( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision, bool addInput = true, string customScreenPos = null )
		{
			string stringPosVar = GenerateGrabScreenPosition( ref dataCollector, uniqueId, precision, addInput, customScreenPos );

			dataCollector.AddLocalVariable( uniqueId, string.Format( "float4 {0} = {1} / {1}.w;", GrabScreenPositionNormalizedStr, stringPosVar ) );
			return GrabScreenPositionNormalizedStr;
		}

		// SCREEN POSITION ON VERT
		static public string GenerateVertexScreenPositionForValue( string customVertexPosition, string outputId, ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision )
		{
			if( dataCollector.IsTemplate )
				return dataCollector.TemplateDataCollectorInstance.GetScreenPosForValue( customVertexPosition, outputId );

			string screenPosVarName = ScreenPositionStr + outputId;
			string value = string.Format( "ComputeScreenPos( UnityObjectToClipPos( {0}.vertex ) )", Constants.VertexShaderInputStr );
			dataCollector.AddToVertexLocalVariables( uniqueId, precision, WirePortDataType.FLOAT4, screenPosVarName, value );
			return screenPosVarName;
		}

		static public string GenerateVertexScreenPosition( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision )
		{
			if( dataCollector.IsTemplate )
				return dataCollector.TemplateDataCollectorInstance.GetScreenPos();

			string value = string.Format( "ComputeScreenPos( UnityObjectToClipPos( {0}.vertex ) )", Constants.VertexShaderInputStr );
			dataCollector.AddToVertexLocalVariables( uniqueId, precision, WirePortDataType.FLOAT4, ScreenPositionStr, value );
			return ScreenPositionStr;
		}

		// VERTEX POSITION
		static public string GenerateVertexPosition( ref MasterNodeDataCollector dataCollector, int uniqueId, WirePortDataType size )
		{
			string value = Constants.VertexShaderInputStr + ".vertex";
			if( size == WirePortDataType.FLOAT3 )
				value += ".xyz";

			if( dataCollector.PortCategory == MasterNodePortCategory.Fragment || dataCollector.PortCategory == MasterNodePortCategory.Debug )
			{
				dataCollector.AddToInput( uniqueId, SurfaceInputs.WORLD_POS );
				dataCollector.AddToIncludes( uniqueId, Constants.UnityShaderVariables );

				value = "mul( unity_WorldToObject, float4( " + Constants.InputVarStr + ".worldPos , 1 ) )";
			}
			string varName = VertexPosition4Str;
			if( size == WirePortDataType.FLOAT3 )
				varName = VertexPosition3Str;

			dataCollector.AddLocalVariable( uniqueId, PrecisionType.Float, size, varName, value );
			return varName;
		}

		// VERTEX NORMAL
		static public string GenerateVertexNormal( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision )
		{
			if( dataCollector.MasterNodeCategory == AvailableShaderTypes.Template )
			{
				return dataCollector.TemplateDataCollectorInstance.GetVertexNormal( UIUtils.CurrentWindow.CurrentGraph.CurrentPrecision );
			}

			string value = Constants.VertexShaderInputStr + ".normal.xyz";
			if( dataCollector.PortCategory == MasterNodePortCategory.Fragment || dataCollector.PortCategory == MasterNodePortCategory.Debug )
			{
				GenerateWorldNormal( ref dataCollector, uniqueId );
				dataCollector.AddToLocalVariables( uniqueId, precision, WirePortDataType.FLOAT3, VertexNormalStr, "mul( unity_WorldToObject, float4( " + WorldNormalStr + ", 0 ) )" );
			}
			else
			{
				dataCollector.AddLocalVariable( uniqueId, precision, WirePortDataType.FLOAT3, VertexNormalStr, value );
			}
			return VertexNormalStr;
		}

		// VERTEX TANGENT
		static public string GenerateVertexTangent( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision )
		{
			if( dataCollector.MasterNodeCategory == AvailableShaderTypes.Template )
			{
				return dataCollector.TemplateDataCollectorInstance.GetVertexTangent( UIUtils.CurrentWindow.CurrentGraph.CurrentPrecision );
			}

			string value = Constants.VertexShaderInputStr + ".tangent.xyz";
			if( dataCollector.PortCategory == MasterNodePortCategory.Fragment || dataCollector.PortCategory == MasterNodePortCategory.Debug )
			{
				GenerateWorldTangent( ref dataCollector, uniqueId );
				dataCollector.AddToLocalVariables( uniqueId, precision, WirePortDataType.FLOAT3, VertexTangentStr, "mul( unity_WorldToObject, float4( " + WorldTangentStr + ", 0 ) )" );
			}
			else
			{
				dataCollector.AddLocalVariable( uniqueId, precision, WirePortDataType.FLOAT3, VertexTangentStr, value );
			}
			return VertexTangentStr;
		}

		// VERTEX TANGENT SIGN
		static public string GenerateVertexTangentSign( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision )
		{
			if( dataCollector.MasterNodeCategory == AvailableShaderTypes.Template )
			{
				return dataCollector.TemplateDataCollectorInstance.GetTangentSign( UIUtils.CurrentWindow.CurrentGraph.CurrentPrecision );
			}

			string value = Constants.VertexShaderInputStr + ".tangent.w";
			if( dataCollector.IsFragmentCategory )
			{
				dataCollector.AddToInput( uniqueId, VertexTangentSignStr, WirePortDataType.FLOAT, PrecisionType.Half );
				dataCollector.AddToVertexLocalVariables( uniqueId, Constants.VertexShaderOutputStr + "." + VertexTangentSignStr + " = " + Constants.VertexShaderInputStr + ".tangent.w;" );
				return Constants.InputVarStr + "." + VertexTangentSignStr;
			}
			else
			{
				dataCollector.AddLocalVariable( uniqueId, precision, WirePortDataType.FLOAT, VertexTangentSignStr, value );
			}
			return VertexTangentSignStr;
		}

		// VERTEX BITANGENT
		static public string GenerateVertexBitangent( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision )
		{
			if( dataCollector.MasterNodeCategory == AvailableShaderTypes.Template )
			{
				return dataCollector.TemplateDataCollectorInstance.GetVertexBitangent( UIUtils.CurrentWindow.CurrentGraph.CurrentPrecision );
			}

			if( dataCollector.PortCategory == MasterNodePortCategory.Fragment || dataCollector.PortCategory == MasterNodePortCategory.Debug )
			{
				GenerateWorldBitangent( ref dataCollector, uniqueId );
				dataCollector.AddToLocalVariables( uniqueId, precision, WirePortDataType.FLOAT3, VertexBitangentStr, "mul( unity_WorldToObject, float4( " + WorldBitangentStr + ", 0 ) )" );
			}
			else
			{
				GenerateVertexNormal( ref dataCollector, uniqueId, precision );
				GenerateVertexTangent( ref dataCollector, uniqueId, precision );
				dataCollector.AddLocalVariable( uniqueId, precision, WirePortDataType.FLOAT3, VertexBitangentStr, "cross( " + VertexNormalStr + ", " + VertexTangentStr + ") * " + Constants.VertexShaderInputStr + ".tangent.w * unity_WorldTransformParams.w" );
			}
			return VertexBitangentStr;
		}

		// VERTEX POSITION ON FRAG
		static public string GenerateVertexPositionOnFrag( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision )
		{
			dataCollector.AddToInput( uniqueId, SurfaceInputs.WORLD_POS );
			dataCollector.AddToIncludes( uniqueId, Constants.UnityShaderVariables );

			string value = "mul( unity_WorldToObject, float4( " + Constants.InputVarStr + ".worldPos , 1 ) )";

			dataCollector.AddToLocalVariables( uniqueId, precision, WirePortDataType.FLOAT4, VertexPosition4Str, value );
			return VertexPosition4Str;
		}

		// CLIP POSITION ON FRAG
		static public string GenerateClipPositionOnFrag( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision )
		{
			if( dataCollector.IsTemplate )
				return dataCollector.TemplateDataCollectorInstance.GetClipPos();

			string vertexName = GenerateVertexPositionOnFrag( ref dataCollector, uniqueId, precision );
			string value = string.Format( "ComputeScreenPos( UnityObjectToClipPos( {0} ) )", vertexName );
			dataCollector.AddToLocalVariables( uniqueId, precision, WirePortDataType.FLOAT4, ClipPositionStr, value );
			return ClipPositionStr;
		}

		// VIEW DIRECTION
		static public string GenerateViewDirection( ref MasterNodeDataCollector dataCollector, int uniqueId, ViewSpace space = ViewSpace.World )
		{
			if( dataCollector.IsTemplate )
				return ( space == ViewSpace.Tangent ) ? dataCollector.TemplateDataCollectorInstance.GetTangentViewDir( UIUtils.CurrentWindow.CurrentGraph.CurrentPrecision ) : dataCollector.TemplateDataCollectorInstance.GetViewDir();

			PrecisionType precision = UIUtils.CurrentWindow.CurrentGraph.CurrentPrecision;
			string worldPos = GenerateWorldPosition( ref dataCollector, uniqueId );
			string safeNormalizeInstruction = string.Empty;
			if( dataCollector.SafeNormalizeViewDir )
			{
				if( dataCollector.IsTemplate && dataCollector.IsSRP )
				{
					safeNormalizeInstruction = "SafeNormalize";
				}
				else
				{
					if( dataCollector.IsTemplate )
						dataCollector.AddToIncludes( -1, Constants.UnityBRDFLib );
					safeNormalizeInstruction = "Unity_SafeNormalize";
				}
			}
			dataCollector.AddLocalVariable( uniqueId, precision, WirePortDataType.FLOAT3, WorldViewDirectionStr, ( dataCollector.SafeNormalizeViewDir ? safeNormalizeInstruction : "normalize" ) + "( UnityWorldSpaceViewDir( " + worldPos + " ) )" );

			if( space == ViewSpace.Tangent )
			{
				string worldToTangent = GenerateWorldToTangentMatrix( ref dataCollector, uniqueId, precision );
				dataCollector.AddLocalVariable( uniqueId, precision, WirePortDataType.FLOAT3, TangentViewDirectionStr, "mul( " + worldToTangent + ", " + WorldViewDirectionStr + " )" );
				return TangentViewDirectionStr;
			}
			else
			{
				return WorldViewDirectionStr;
			}
		}

		// VIEW POS
		static public string GenerateViewPositionOnFrag( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision )
		{
			if( dataCollector.IsTemplate )
				UnityEngine.Debug.LogWarning( "View Pos not implemented on Templates" );

			string vertexName = GenerateVertexPositionOnFrag( ref dataCollector, uniqueId, precision );
			string value = string.Format( "UnityObjectToViewPos( {0} )", vertexName );
			dataCollector.AddToLocalVariables( uniqueId, precision, WirePortDataType.FLOAT3, ViewPositionStr, value );
			return ViewPositionStr;
		}

		// SCREEN DEPTH 
		static public string GenerateScreenDepthOnFrag( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision )
		{
			if( dataCollector.IsTemplate )
				UnityEngine.Debug.LogWarning( "Screen Depth not implemented on Templates" );

			string viewPos = GenerateViewPositionOnFrag( ref dataCollector, uniqueId, precision );
			string value = string.Format( "-{0}.z", viewPos );
			dataCollector.AddToLocalVariables( uniqueId, precision, WirePortDataType.FLOAT, ScreenDepthStr, value );
			return ScreenDepthStr;
		}

		// LIGHT DIRECTION WORLD
		static public string GenerateWorldLightDirection( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision )
		{
			dataCollector.AddToIncludes( uniqueId, Constants.UnityCgLibFuncs );
			string worldPos = GeneratorUtils.GenerateWorldPosition( ref dataCollector, uniqueId );
			dataCollector.AddLocalVariable( uniqueId, "#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld" );
			dataCollector.AddLocalVariable( uniqueId, precision, WirePortDataType.FLOAT3, WorldLightDirStr, "0" );
			dataCollector.AddLocalVariable( uniqueId, "#else //aseld" );
			dataCollector.AddLocalVariable( uniqueId, precision, WirePortDataType.FLOAT3, WorldLightDirStr, ( dataCollector.SafeNormalizeLightDir ? "Unity_SafeNormalize" : "normalize" ) + "( UnityWorldSpaceLightDir( " + worldPos + " ) )" );
			dataCollector.AddLocalVariable( uniqueId, "#endif //aseld" );
			return WorldLightDirStr;
		}

		// LIGHT DIRECTION Object
		static public string GenerateObjectLightDirection( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision, string vertexPos )
		{
			dataCollector.AddToIncludes( uniqueId, Constants.UnityCgLibFuncs );
			dataCollector.AddLocalVariable( uniqueId, precision, WirePortDataType.FLOAT3, ObjectLightDirStr, "normalize( ObjSpaceLightDir( " + vertexPos + " ) )" );
			return ObjectLightDirStr;
		}

		//MATRIX INVERSE
		// 3x3
		public static string Inverse3x3Header = "Inverse3x3( {0} )";
		public static string[] Inverse3x3Function =
		{
			"{0}3x3 Inverse3x3({0}3x3 input)\n",
			"{\n",
			"\t{0}3 a = input._11_21_31;\n",
			"\t{0}3 b = input._12_22_32;\n",
			"\t{0}3 c = input._13_23_33;\n",
			"\treturn {0}3x3(cross(b,c), cross(c,a), cross(a,b)) * (1.0 / dot(a,cross(b,c)));\n",
			"}\n"
		};

		public static bool[] Inverse3x3FunctionFlags =
		{
			true,
			false,
			true,
			true,
			true,
			true,
			false
		};

		public static void Add3x3InverseFunction( ref MasterNodeDataCollector dataCollector, string precisionString )
		{
			if( !dataCollector.HasFunction( Inverse3x3Header ) )
			{
				//Hack to be used util indent is properly used
				int currIndent = UIUtils.ShaderIndentLevel;
				if( dataCollector.IsTemplate )
				{
					UIUtils.ShaderIndentLevel = 0;
				}
				else
				{
					UIUtils.ShaderIndentLevel = 1;
					UIUtils.ShaderIndentLevel++;
				}
				string finalFunction = string.Empty;
				for( int i = 0; i < Inverse3x3Function.Length; i++ )
				{
					finalFunction += UIUtils.ShaderIndentTabs + ( Inverse3x3FunctionFlags[ i ] ? string.Format( Inverse3x3Function[ i ], precisionString ) : Inverse3x3Function[ i ] );
				}


				UIUtils.ShaderIndentLevel = currIndent;

				dataCollector.AddFunction( Inverse3x3Header, finalFunction );
			}
		}
	}
}
