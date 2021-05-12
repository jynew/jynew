// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	public enum OutlineMode
	{
		VertexOffset,
		VertexScale
	}

	[Serializable]
	public sealed class OutlineOpHelper
	{

		private string[] ModeTags =
		{
			"Tags{ }",
			"Tags{ \"RenderType\" = \"TransparentCutout\"  \"Queue\" = \"AlphaTest+0\"}",
			"Tags{ \"RenderType\" = \"Transparent\"  \"Queue\" = \"Transparent+0\"}",
			"Tags{ \"RenderType\" = \"Transparent\"  \"Queue\" = \"Transparent+0\" }"
		};

		private string[] ModePragma =
		{
			string.Empty,
			string.Empty,
			"alpha:fade ",
			"alpha:premul "
		};


		private readonly string OutlineSurfaceConfig = "#pragma surface outlineSurf Outline {0} keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc ";

		private readonly string OutlineBodyStructBegin = "struct Input {";
		private readonly string OutlineBodyStructDefault = "\thalf filler;";
		private readonly string OutlineBodyStructEnd = "};";

		private readonly string OutlineDefaultUniformColor = "uniform half4 _ASEOutlineColor;";
		private readonly string OutlineDefaultUniformWidth = "uniform half _ASEOutlineWidth;";

		private readonly string OutlineDefaultVertexHeader = "void outlineVertexDataFunc( inout appdata_full v, out Input o )\n\t\t{";
		private readonly string OutlineTessVertexHeader = "void outlineVertexDataFunc( inout appdata_full v )\n\t\t{";

		private readonly string OutlineDefaultVertexOutputDeclaration = "\tUNITY_INITIALIZE_OUTPUT( Input, o );";

		private readonly string[] OutlineSurfBody = {
														"\to.Emission = _ASEOutlineColor.rgb;",
														"\to.Alpha = 1;"
		};

		private readonly string[] OutlineBodyDefaultSurfBegin = {
														"}",
														"inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }",
														"void outlineSurf( Input i, inout SurfaceOutput o )",
														"{"};

		private readonly string[] OutlineBodyDefaultSurfEnd = {
														"}",
														"ENDCG",
														"\n"};

		private const string OutlineInstancedHeader = "#pragma multi_compile_instancing";

		private readonly string[] OutlineBodyInstancedBegin = {
														"UNITY_INSTANCING_CBUFFER_START({0})",
														"\tUNITY_DEFINE_INSTANCED_PROP( half4, _ASEOutlineColor )",
														"\tUNITY_DEFINE_INSTANCED_PROP(half, _ASEOutlineWidth)",
														"UNITY_INSTANCING_CBUFFER_END",
														"void outlineVertexDataFunc( inout appdata_full v, out Input o )",
														"{",
														"\tUNITY_INITIALIZE_OUTPUT( Input, o );"};

		private readonly string[] OutlineBodyInstancedEnd = {
														"}",
														"inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }",
														"void outlineSurf( Input i, inout SurfaceOutput o ) { o.Emission = UNITY_ACCESS_INSTANCED_PROP( _ASEOutlineColor ).rgb; o.Alpha = 1; }",
														"ENDCG",
														"\n"};

		private const string WidthVariableAccessInstanced = "UNITY_ACCESS_INSTANCED_PROP( _ASEOutlineWidth )";

		private const string OutlineVertexOffsetMode = "\tv.vertex.xyz += ( v.normal * {0} );";
		private const string OutlineVertexScaleMode = "\tv.vertex.xyz *= ( 1 + {0});";
		private const string OutlineVertexCustomMode = "\tv.vertex.xyz += {0};";

		private const string OutlineColorLabel = "Color";
		private const string OutlineWidthLabel = "Width";

		private const string ColorPropertyName = "_ASEOutlineColor";
		private const string WidthPropertyName = "_ASEOutlineWidth";
		private const string ColorPropertyDec = "_ASEOutlineColor( \"Outline Color\", Color ) = ({0})";
		private const string OutlinePropertyDec = "_ASEOutlineWidth( \"Outline Width\", Float ) = {0}";

		private const string ModePropertyStr = "Mode";

		private const string NoFogStr = "No Fog";

		private const string BillboardInstructionFormat = "\t{0};";

		[SerializeField]
		private Color m_outlineColor;

		[SerializeField]
		private float m_outlineWidth;

		[SerializeField]
		private bool m_enabled;

		[SerializeField]
		private OutlineMode m_mode = OutlineMode.VertexOffset;

		[SerializeField]
		private bool m_noFog = true;

		private CullMode m_cullMode = CullMode.Front;
		private int m_zTestMode = 0;
		private int m_zWriteMode = 0;
		private bool m_dirtyInput = false;
		private string m_inputs = string.Empty;
		private List<PropertyDataCollector> m_inputList = new List<PropertyDataCollector>();
		private string m_uniforms = string.Empty;
		private List<PropertyDataCollector> m_uniformList = new List<PropertyDataCollector>();
		private string m_instructions = string.Empty;
		private string m_functions = string.Empty;
		private string m_includes = string.Empty;
		private string m_pragmas = string.Empty;
		private string m_defines = string.Empty;
		private string m_vertexData = string.Empty;
		private string m_grabPasses = string.Empty;
		private Dictionary<string, string> m_localFunctions;

		//private OutlineMode m_customMode = OutlineMode.VertexOffset;
		private int m_offsetMode = 0;
		private bool m_customNoFog = true;

		public void Draw( ParentNode owner, GUIStyle toolbarstyle, Material mat )
		{
			Color cachedColor = GUI.color;
			GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, 0.5f );
			EditorGUILayout.BeginHorizontal( toolbarstyle );
			GUI.color = cachedColor;
			owner.ContainerGraph.ParentWindow.InnerWindowVariables.OutlineActiveMode = owner.GUILayoutToggle( owner.ContainerGraph.ParentWindow.InnerWindowVariables.OutlineActiveMode , EditorVariablesManager.OutlineActiveMode.LabelName, UIUtils.MenuItemToggleStyle, GUILayout.ExpandWidth( true ) );
			EditorGUI.BeginChangeCheck();
			m_enabled = owner.EditorGUILayoutToggle( string.Empty, m_enabled, UIUtils.MenuItemEnableStyle, GUILayout.Width( 16 ) );
			if( EditorGUI.EndChangeCheck() )
			{
				if( m_enabled )
					UpdateToMaterial( mat );

				UIUtils.RequestSave();
			}
			EditorGUILayout.EndHorizontal();

			if( owner.ContainerGraph.ParentWindow.InnerWindowVariables.OutlineActiveMode )
			{
				cachedColor = GUI.color;
				GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, ( EditorGUIUtility.isProSkin ? 0.5f : 0.25f ) );
				EditorGUILayout.BeginVertical( UIUtils.MenuItemBackgroundStyle );
				GUI.color = cachedColor;

				EditorGUILayout.Separator();
				EditorGUI.BeginDisabledGroup( !m_enabled );

				EditorGUI.indentLevel += 1;
				{
					m_mode = (OutlineMode)owner.EditorGUILayoutEnumPopup( ModePropertyStr, m_mode );

					EditorGUI.BeginChangeCheck();
					m_outlineColor = owner.EditorGUILayoutColorField( OutlineColorLabel, m_outlineColor );
					if( EditorGUI.EndChangeCheck() && mat != null )
					{
						if( mat.HasProperty( ColorPropertyName ) )
						{
							mat.SetColor( ColorPropertyName, m_outlineColor );
						}
					}

					EditorGUI.BeginChangeCheck();
					m_outlineWidth = owner.EditorGUILayoutFloatField( OutlineWidthLabel, m_outlineWidth );
					if( EditorGUI.EndChangeCheck() && mat != null )
					{
						if( mat.HasProperty( WidthPropertyName ) )
						{
							mat.SetFloat( WidthPropertyName, m_outlineWidth );
						}
					}

					m_noFog = owner.EditorGUILayoutToggle( NoFogStr, m_noFog );
				}

				EditorGUI.indentLevel -= 1;
				EditorGUI.EndDisabledGroup();
				EditorGUILayout.Separator();
				EditorGUILayout.EndVertical();
			}
		}

		public void UpdateToMaterial( Material mat )
		{
			if( mat == null )
				return;

			if( mat.HasProperty( ColorPropertyName ) )
			{
				mat.SetColor( ColorPropertyName, m_outlineColor );
			}

			if( mat.HasProperty( WidthPropertyName ) )
			{
				mat.SetFloat( WidthPropertyName, m_outlineWidth );
			}
		}

		public void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			m_enabled = Convert.ToBoolean( nodeParams[ index++ ] );
			m_outlineWidth = Convert.ToSingle( nodeParams[ index++ ] );
			m_outlineColor = IOUtils.StringToColor( nodeParams[ index++ ] );
			if( UIUtils.CurrentShaderVersion() > 5004 )
			{
				m_mode = (OutlineMode)Enum.Parse( typeof( OutlineMode ), nodeParams[ index++ ] );
			}

			if( UIUtils.CurrentShaderVersion() > 13902 )
			{
				m_noFog = Convert.ToBoolean( nodeParams[ index++ ] );
			}
		}

		public void WriteToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_enabled );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_outlineWidth );
			IOUtils.AddFieldValueToString( ref nodeInfo, IOUtils.ColorToString( m_outlineColor ) );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_mode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_noFog );
		}

		public void AddToDataCollector( ref MasterNodeDataCollector dataCollector )
		{
			if( !dataCollector.UsingCustomOutlineColor )
				dataCollector.AddToProperties( -1, string.Format( ColorPropertyDec, IOUtils.ColorToString( m_outlineColor ) ), -1 );
			if( !dataCollector.UsingCustomOutlineWidth )
				dataCollector.AddToProperties( -1, string.Format( OutlinePropertyDec, m_outlineWidth ), -1 );
		}

		public void UpdateFromMaterial( Material mat )
		{
			if( mat.HasProperty( ColorPropertyName ) )
			{
				m_outlineColor = mat.GetColor( ColorPropertyName );
			}

			if( mat.HasProperty( WidthPropertyName ) )
			{
				m_outlineWidth = mat.GetFloat( WidthPropertyName );
			}
		}

		void AddMultibodyString( string body , List<string> list )
		{
			body = body.Replace( "\t\t", string.Empty );
			string[] strArr = body.Split( '\n' );
			for( int i = 0; i < strArr.Length; i++ )
			{
				list.Add( strArr[ i ] );
			}

		}
		public string[] OutlineFunctionBody( ref MasterNodeDataCollector dataCollector, bool instanced, bool isShadowCaster, string shaderName, string[] billboardInfo, ref TessellationOpHelper tessOpHelper, string target )
		{
			List<string> body = new List<string>();
			body.Add( ModeTags[ dataCollector.CustomOutlineSelectedAlpha ] );
			if( !string.IsNullOrEmpty( m_grabPasses ))
				body.Add( m_grabPasses.Replace( "\t\t",string.Empty ));	

			if( m_zWriteMode != 0 )
				body.Add( "ZWrite " + ZBufferOpHelper.ZWriteModeValues[ m_zWriteMode ] );
			if( m_zTestMode != 0 )
				body.Add( "ZTest " + ZBufferOpHelper.ZTestModeValues[ m_zTestMode ] );

			body.Add( "Cull " + m_cullMode );
			body.Add( "CGPROGRAM" );
			if( tessOpHelper.EnableTesselation )
			{
				body.Add( "#include \"" + TessellationOpHelper.TessInclude + "\"" );
				body.Add( "#pragma target " + target );
			}
			else
			{
				body.Add( "#pragma target 3.0" );
			}

			bool customOutline = dataCollector.UsingCustomOutlineColor || dataCollector.UsingCustomOutlineWidth || dataCollector.UsingCustomOutlineAlpha;
			int outlineMode = customOutline ? m_offsetMode : ( m_mode == OutlineMode.VertexOffset ? 0 : 1 );
			string extraOptions = ( customOutline ? m_customNoFog : m_noFog ) ? "nofog " : string.Empty;
			if( dataCollector.CustomOutlineSelectedAlpha > 0 )
			{
				extraOptions += ModePragma[ dataCollector.CustomOutlineSelectedAlpha ];
			}

			string surfConfig = string.Format( OutlineSurfaceConfig, extraOptions );

			if( tessOpHelper.EnableTesselation )
				tessOpHelper.WriteToOptionalParams( ref surfConfig );

			body.Add( surfConfig );
			if( !isShadowCaster )
			{
				AddMultibodyString( m_defines, body );
				AddMultibodyString( m_includes, body );
				AddMultibodyString( m_pragmas, body );
			}

			if( instanced )
			{
				body.Add( OutlineInstancedHeader );
			}

			if( customOutline )
			{
				if( isShadowCaster )
				{

					for( int i = 0; i < InputList.Count; i++ )
					{
						dataCollector.AddToInput( InputList[ i ].NodeId, InputList[ i ].PropertyName, true );
					}
				}
				else
				{
					if( !string.IsNullOrEmpty( m_inputs ) )
						body.Add( m_inputs.Trim( '\t', '\n' ) );
				}

				if( !DirtyInput && !isShadowCaster )
					body.Add( OutlineBodyStructDefault );

				if( !isShadowCaster )
					body.Add( OutlineBodyStructEnd );
			}
			else if( !isShadowCaster )
			{
				body.Add( OutlineBodyStructBegin );
				body.Add( OutlineBodyStructDefault );
				body.Add( OutlineBodyStructEnd );
			}

			if( instanced )
			{
				for( int i = 0; i < OutlineBodyInstancedBegin.Length; i++ )
				{
					body.Add( ( i == 0 ) ? string.Format( OutlineBodyInstancedBegin[ i ], shaderName ) : OutlineBodyInstancedBegin[ i ] );
				}

				if( (object)billboardInfo != null )
				{
					for( int j = 0; j < billboardInfo.Length; j++ )
					{
						body.Add( string.Format( BillboardInstructionFormat, billboardInfo[ j ] ) );
					}
				}

				switch( outlineMode )
				{
					case 0: body.Add( string.Format( OutlineVertexOffsetMode, WidthVariableAccessInstanced ) ); break;
					case 1: body.Add( string.Format( OutlineVertexScaleMode, WidthVariableAccessInstanced ) ); break;
					case 2: body.Add( string.Format( OutlineVertexCustomMode, WidthVariableAccessInstanced ) ); break;
				}
				for( int i = 0; i < OutlineBodyInstancedEnd.Length; i++ )
				{
					body.Add( OutlineBodyInstancedEnd[ i ] );
				}
			}
			else
			{
				if( customOutline )
				{
					if( isShadowCaster )
					{
						for( int i = 0; i < UniformList.Count; i++ )
						{
							dataCollector.AddToUniforms( UniformList[ i ].NodeId, UniformList[ i ].PropertyName );
						}

						foreach( KeyValuePair<string, string> kvp in m_localFunctions )
						{
							dataCollector.AddFunction( kvp.Key, kvp.Value );
						}
					}
					else
					{
						if( !string.IsNullOrEmpty( Uniforms ) )
							body.Add( Uniforms.Trim( '\t', '\n' ) );
					}
				}

				if( !dataCollector.UsingCustomOutlineColor )
					body.Add( OutlineDefaultUniformColor );

				if( !dataCollector.UsingCustomOutlineWidth )
					body.Add( OutlineDefaultUniformWidth );

				//Functions
				if( customOutline && !isShadowCaster )
					body.Add( Functions );

				if( tessOpHelper.EnableTesselation && !isShadowCaster )
				{
					body.Add( tessOpHelper.Uniforms().TrimStart( '\t' ) );
					body.Add( tessOpHelper.GetCurrentTessellationFunction.Trim( '\t', '\n' ) + "\n" );
				}

				if( tessOpHelper.EnableTesselation )
				{
					body.Add( OutlineTessVertexHeader );
				}
				else
				{
					body.Add( OutlineDefaultVertexHeader );
					body.Add( OutlineDefaultVertexOutputDeclaration );
				}

				if( customOutline )
				{
					if( !string.IsNullOrEmpty( VertexData ) )
						body.Add( "\t" + VertexData.Trim( '\t', '\n' ) );
				}

				if( (object)billboardInfo != null )
				{
					for( int j = 0; j < billboardInfo.Length; j++ )
					{
						body.Add( string.Format( BillboardInstructionFormat, billboardInfo[ j ] ) );
					}
				}

				switch( outlineMode )
				{
					case 0: body.Add( string.Format( OutlineVertexOffsetMode, dataCollector.UsingCustomOutlineWidth ? "outlineVar" : WidthPropertyName ) ); break;
					case 1: body.Add( string.Format( OutlineVertexScaleMode, dataCollector.UsingCustomOutlineWidth ? "outlineVar" : WidthPropertyName ) ); break;
					case 2: body.Add( string.Format( OutlineVertexCustomMode, dataCollector.UsingCustomOutlineWidth ? "outlineVar" : WidthPropertyName ) ); break;
				}
				for( int i = 0; i < OutlineBodyDefaultSurfBegin.Length; i++ )
				{
					body.Add( OutlineBodyDefaultSurfBegin[ i ] );
				}
				if( dataCollector.UsingCustomOutlineColor || dataCollector.CustomOutlineSelectedAlpha > 0 )
				{
					body.Add( "\t" + Instructions.Trim( '\t', '\n' ) );
				}
				else
				{
					for( int i = 0; i < OutlineSurfBody.Length; i++ )
					{
						body.Add( OutlineSurfBody[ i ] );
					}
				}

				for( int i = 0; i < OutlineBodyDefaultSurfEnd.Length; i++ )
				{
					body.Add( OutlineBodyDefaultSurfEnd[ i ] );
				}
			}

			string[] bodyArr = body.ToArray();
			body.Clear();
			body = null;
			return bodyArr;
		}


		public void Destroy()
		{
			m_inputList = null;
			m_uniformList = null;
			m_localFunctions = null;
		}

		public bool EnableOutline { get { return m_enabled; } }

		public bool UsingCullMode { get { return m_cullMode != CullMode.Front; } }
		public bool UsingZWrite { get { return m_zWriteMode != 0; } }
		public bool UsingZTest { get { return m_zTestMode != 0; } }
		public int ZWriteMode { get { return m_zWriteMode; } set { m_zWriteMode = value; } }
		public int ZTestMode { get { return m_zTestMode; } set { m_zTestMode = value; } }
		public CullMode OutlineCullMode { get { return m_cullMode; } set { m_cullMode = value; } }
		public string Inputs { get { return m_inputs; } set { m_inputs = value; } }
		public string Uniforms { get { return m_uniforms; } set { m_uniforms = value; } }
		public string Instructions { get { return m_instructions; } set { m_instructions = value; } }
		public string Functions { get { return m_functions; } set { m_functions = value; } }
		public string Includes { get { return m_includes; } set { m_includes = value; } }
		public string Pragmas { get { return m_pragmas; } set { m_pragmas = value; } }
		public string Defines { get { return m_defines; } set { m_defines = value; } }
		public string VertexData { get { return m_vertexData; } set { m_vertexData = value; } }
		public string GrabPasses { get { return m_grabPasses; } set { m_grabPasses = value; } }
		public List<PropertyDataCollector> InputList { get { return m_inputList; } set { m_inputList = value; } }
		public List<PropertyDataCollector> UniformList { get { return m_uniformList; } set { m_uniformList = value; } }
		public Dictionary<string, string> LocalFunctions { get { return m_localFunctions; } set { m_localFunctions = value; } }
		public bool DirtyInput { get { return m_dirtyInput; } set { m_dirtyInput = value; } }

		//public OutlineMode CustomMode { get { return m_customMode; } set { m_customMode = value; } }
		public int OffsetMode { get { return m_offsetMode; } set { m_offsetMode = value; } }
		public bool CustomNoFog { get { return m_customNoFog; } set { m_customNoFog = value; } }
	}
}
