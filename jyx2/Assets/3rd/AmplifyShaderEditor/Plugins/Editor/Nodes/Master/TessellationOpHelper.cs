// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	public sealed class TessellationOpHelper
	{
		public const string TessellationPortStr = "Tessellation";


		public const string TessSurfParam = "tessellate:tessFunction";
		public const string TessInclude = "Tessellation.cginc";
		//public const string CustomAppData = "\t\tstruct appdata\n" +
		//									"\t\t{\n" +
		//									"\t\t\tfloat4 vertex : POSITION;\n" +
		//									"\t\t\tfloat4 tangent : TANGENT;\n" +
		//									"\t\t\tfloat3 normal : NORMAL;\n" +
		//									"\t\t\tfloat4 texcoord : TEXCOORD0;\n" +
		//									"\t\t\tfloat4 texcoord1 : TEXCOORD1;\n" +
		//									"\t\t\tfloat4 texcoord2 : TEXCOORD2;\n" +
		//									"\t\t\tfloat4 texcoord3 : TEXCOORD3;\n" +
		//									"\t\t\tfixed4 color : COLOR;\n" +
		//									"\t\t\tUNITY_VERTEX_INPUT_INSTANCE_ID\n" +
		//									"\t\t};\n\n";



		private const string TessUniformName = "_TessValue";
		private const string TessMinUniformName = "_TessMin";
		private const string TessMaxUniformName = "_TessMax";

		//private GUIContent EnableTessContent = new GUIContent( "Tessellation", "Activates the use of tessellation which subdivides polygons to increase geometry detail using a set of rules\nDefault: OFF" );
		private GUIContent TessFactorContent = new GUIContent( "Tess", "Tessellation factor\nDefault: 4" );
		private GUIContent TessMinDistanceContent = new GUIContent( "Min", "Minimum tessellation distance\nDefault: 10" );
		private GUIContent TessMaxDistanceContent = new GUIContent( "Max", "Maximum tessellation distance\nDefault: 25" );


		private readonly int[] TesselationTypeValues = { 0, 1, 2, 3 };
		private readonly string[] TesselationTypeLabels = { "Distance-based", "Fixed", "Edge Length", "Edge Length Cull" };
		private readonly string TesselationTypeStr = "Type";

		private const string TessProperty = "_TessValue( \"Max Tessellation\", Range( 1, 32 ) ) = {0}";
		private const string TessMinProperty = "_TessMin( \"Tess Min Distance\", Float ) = {0}";
		private const string TessMaxProperty = "_TessMax( \"Tess Max Distance\", Float ) = {0}";

		private const string TessFunctionOpen = "\t\tfloat4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )\n\t\t{\n";
		private const string TessFunctionClose = "\t\t}\n";

		// Custom function
		private const string CustomFunctionBody = "\t\t\treturn {0};\n";

		// Distance based function
		private const string DistBasedTessFunctionBody = "\t\t\treturn UnityDistanceBasedTess( v0.vertex, v1.vertex, v2.vertex, _TessMin, _TessMax, _TessValue );\n";

		// Fixed amount function
		private const string FixedAmountTessFunctionOpen = "\t\tfloat4 tessFunction( )\n\t\t{\n";
		private const string FixedAmountTessFunctionBody = "\t\t\treturn _TessValue;\n";

		// Edge Length
		private GUIContent EdgeLengthContent = new GUIContent( "Edge Length", "Tessellation levels ccomputed based on triangle edge length on the screen\nDefault: 4" );
		private const string EdgeLengthTessProperty = "_EdgeLength ( \"Edge length\", Range( 2, 50 ) ) = {0}";
		private const string EdgeLengthTessUniformName = "_EdgeLength";

		private const string EdgeLengthTessFunctionBody = "\t\t\treturn UnityEdgeLengthBasedTess (v0.vertex, v1.vertex, v2.vertex, _EdgeLength);\n";
		private const string EdgeLengthTessCullFunctionBody = "\t\t\treturn UnityEdgeLengthBasedTessCull (v0.vertex, v1.vertex, v2.vertex, _EdgeLength , _TessMaxDisp );\n";


		private const string EdgeLengthTessMaxDispProperty = "_TessMaxDisp( \"Max Displacement\", Float ) = {0}";
		private const string EdgeLengthTessMaxDispUniformName = "_TessMaxDisp";
		private GUIContent EdgeLengthTessMaxDisplacementContent = new GUIContent( "Max Disp.", "Max Displacement" );

		// Phong
		private GUIContent PhongEnableContent = new GUIContent( "Phong", "Modifies positions of the subdivided faces so that the resulting surface follows the mesh normals a bit\nDefault: OFF" );
		private GUIContent PhongStrengthContent = new GUIContent( "Strength", "Strength\nDefault: 0.5" );
		public const string PhongStrengthParam = "tessphong:_TessPhongStrength";

		private const string PhongStrengthProperty = "_TessPhongStrength( \"Phong Tess Strength\", Range( 0, 1 ) ) = {0}";
		private const string PhongStrengthUniformName = "_TessPhongStrength";

		[SerializeField]
		private bool m_enabled = false;

		//private bool m_expanded = false;

		[SerializeField]
		private int m_tessType = 2;

		[SerializeField]
		private float m_tessMinDistance = 10f;

		[SerializeField]
		private float m_tessMaxDistance = 25f;

		[SerializeField]
		private float m_tessFactor = 15f;

		[SerializeField]
		private float m_phongStrength = 0.5f;

		[SerializeField]
		private bool m_phongEnabled = false;

		[SerializeField]
		private string[] m_customData = { string.Empty, string.Empty, string.Empty };

		[SerializeField]
		private bool m_hasCustomFunction = false;

		[SerializeField]
		private string m_customFunction = String.Empty;

		[SerializeField]
		private string m_additionalData = string.Empty;

		[SerializeField]
		private StandardSurfaceOutputNode m_parentSurface;

		private Dictionary<string, bool> m_additionalDataDict = new Dictionary<string, bool>();

		private int m_masterNodeIndexPort = 0;
		private int m_vertexOffsetIndexPort = 0;
		//private int m_orderIndex = 1000;

		public void Draw( UndoParentNode owner,  GUIStyle toolbarstyle, Material mat, bool connectedInput )
		{
			Color cachedColor = GUI.color;
			GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, 0.5f );
			EditorGUILayout.BeginHorizontal( toolbarstyle );
			GUI.color = cachedColor;
			EditorGUI.BeginChangeCheck();
			m_parentSurface.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedTesselation = GUILayout.Toggle( m_parentSurface.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedTesselation, " Tessellation", UIUtils.MenuItemToggleStyle, GUILayout.ExpandWidth( true ) );
			if ( EditorGUI.EndChangeCheck() )
			{
				EditorPrefs.SetBool( "ExpandedTesselation", m_parentSurface.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedTesselation );
			}

			EditorGUI.BeginChangeCheck();
			m_enabled = owner.EditorGUILayoutToggle( string.Empty, m_enabled, UIUtils.MenuItemEnableStyle, GUILayout.Width( 16 ) );
			if ( EditorGUI.EndChangeCheck() )
			{
				if ( m_enabled )
					UpdateToMaterial( mat, !connectedInput );

				UIUtils.RequestSave();
			}

			EditorGUILayout.EndHorizontal();

			m_enabled = m_enabled || connectedInput;

			if ( m_parentSurface.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedTesselation )
			{
				cachedColor = GUI.color;
				GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, ( EditorGUIUtility.isProSkin ? 0.5f : 0.25f ) );
				EditorGUILayout.BeginVertical( UIUtils.MenuItemBackgroundStyle );
				GUI.color = cachedColor;

				EditorGUILayout.Separator();
				EditorGUI.BeginDisabledGroup( !m_enabled );

				EditorGUI.indentLevel += 1;

				m_phongEnabled = owner.EditorGUILayoutToggle( PhongEnableContent, m_phongEnabled );
				if ( m_phongEnabled )
				{
					EditorGUI.indentLevel += 1;
					EditorGUI.BeginChangeCheck();
					m_phongStrength = owner.EditorGUILayoutSlider( PhongStrengthContent, m_phongStrength, 0.0f, 1.0f );
					if ( EditorGUI.EndChangeCheck() && mat != null )
					{
						if ( mat.HasProperty( PhongStrengthUniformName ) )
							mat.SetFloat( PhongStrengthUniformName, m_phongStrength );
					}

					EditorGUI.indentLevel -= 1;
				}

				bool guiEnabled = GUI.enabled;
				GUI.enabled = !connectedInput && m_enabled;

				m_tessType = owner.EditorGUILayoutIntPopup( TesselationTypeStr, m_tessType, TesselationTypeLabels, TesselationTypeValues );

				switch ( m_tessType )
				{
					case 0:
					{
						EditorGUI.BeginChangeCheck();
						m_tessFactor = owner.EditorGUILayoutSlider( TessFactorContent, m_tessFactor, 1, 32 );
						if ( EditorGUI.EndChangeCheck() && mat != null )
						{
							if ( mat.HasProperty( TessUniformName ) )
								mat.SetFloat( TessUniformName, m_tessFactor );
						}

						EditorGUI.BeginChangeCheck();
						m_tessMinDistance = owner.EditorGUILayoutFloatField( TessMinDistanceContent, m_tessMinDistance );
						if ( EditorGUI.EndChangeCheck() && mat != null )
						{
							if ( mat.HasProperty( TessMinUniformName ) )
								mat.SetFloat( TessMinUniformName, m_tessMinDistance );
						}

						EditorGUI.BeginChangeCheck();
						m_tessMaxDistance = owner.EditorGUILayoutFloatField( TessMaxDistanceContent, m_tessMaxDistance );
						if ( EditorGUI.EndChangeCheck() && mat != null )
						{
							if ( mat.HasProperty( TessMaxUniformName ) )
								mat.SetFloat( TessMaxUniformName, m_tessMaxDistance );
						}
					}
					break;
					case 1:
					{
						EditorGUI.BeginChangeCheck();
						m_tessFactor = owner.EditorGUILayoutSlider( TessFactorContent, m_tessFactor, 1, 32 );
						if ( EditorGUI.EndChangeCheck() && mat != null )
						{
							if ( mat.HasProperty( TessUniformName ) )
								mat.SetFloat( TessUniformName, m_tessFactor );
						}
					}
					break;
					case 2:
					{
						EditorGUI.BeginChangeCheck();
						m_tessFactor = owner.EditorGUILayoutSlider( EdgeLengthContent, m_tessFactor, 2, 50 );
						if ( EditorGUI.EndChangeCheck() && mat != null )
						{
							if ( mat.HasProperty( EdgeLengthTessUniformName ) )
								mat.SetFloat( EdgeLengthTessUniformName, m_tessFactor );
						}
					}
					break;
					case 3:
					{
						EditorGUI.BeginChangeCheck();
						m_tessFactor = owner.EditorGUILayoutSlider( EdgeLengthContent, m_tessFactor, 2, 50 );
						if ( EditorGUI.EndChangeCheck() && mat != null )
						{
							if ( mat.HasProperty( EdgeLengthTessUniformName ) )
								mat.SetFloat( EdgeLengthTessUniformName, m_tessFactor );
						}

						EditorGUI.BeginChangeCheck();
						m_tessMaxDistance = owner.EditorGUILayoutFloatField( EdgeLengthTessMaxDisplacementContent, m_tessMaxDistance );
						if ( EditorGUI.EndChangeCheck() && mat != null )
						{
							if ( mat.HasProperty( TessMinUniformName ) )
								mat.SetFloat( TessMinUniformName, m_tessMaxDistance );
						}
					}
					break;
				}
				GUI.enabled = guiEnabled;
				EditorGUI.indentLevel -= 1;
				EditorGUI.EndDisabledGroup();
				EditorGUILayout.Separator();
				EditorGUILayout.EndVertical();
			}
		}

		public void UpdateToMaterial( Material mat, bool updateInternals )
		{
			if ( mat == null )
				return;

			if ( m_phongEnabled )
			{
				if ( mat.HasProperty( PhongStrengthUniformName ) )
					mat.SetFloat( PhongStrengthUniformName, m_phongStrength );
			}

			if ( updateInternals )
			{
				switch ( m_tessType )
				{
					case 0:
					{
						if ( mat.HasProperty( TessUniformName ) )
							mat.SetFloat( TessUniformName, m_tessFactor );

						if ( mat.HasProperty( TessMinUniformName ) )
							mat.SetFloat( TessMinUniformName, m_tessMinDistance );

						if ( mat.HasProperty( TessMaxUniformName ) )
							mat.SetFloat( TessMaxUniformName, m_tessMaxDistance );
					}
					break;
					case 1:
					{
						if ( mat.HasProperty( TessUniformName ) )
							mat.SetFloat( TessUniformName, m_tessFactor );
					}
					break;
					case 2:
					{

						if ( mat.HasProperty( EdgeLengthTessUniformName ) )
							mat.SetFloat( EdgeLengthTessUniformName, m_tessFactor );
					}
					break;
					case 3:
					{
						if ( mat.HasProperty( EdgeLengthTessUniformName ) )
							mat.SetFloat( EdgeLengthTessUniformName, m_tessFactor );

						if ( mat.HasProperty( TessMinUniformName ) )
							mat.SetFloat( TessMinUniformName, m_tessMaxDistance );
					}
					break;
				}
			}
		}
		public void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			m_enabled = Convert.ToBoolean( nodeParams[ index++ ] );
			m_tessType = Convert.ToInt32( nodeParams[ index++ ] );
			m_tessFactor = Convert.ToSingle( nodeParams[ index++ ] );
			m_tessMinDistance = Convert.ToSingle( nodeParams[ index++ ] );
			m_tessMaxDistance = Convert.ToSingle( nodeParams[ index++ ] );
			if ( UIUtils.CurrentShaderVersion() > 3001 )
			{
				m_phongEnabled = Convert.ToBoolean( nodeParams[ index++ ] );
				m_phongStrength = Convert.ToSingle( nodeParams[ index++ ] );
			}
		}

		public void WriteToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_enabled );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_tessType );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_tessFactor );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_tessMinDistance );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_tessMaxDistance );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_phongEnabled );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_phongStrength );
		}

		public string Uniforms()
		{
			string uniforms = string.Empty;
			switch( m_tessType )
			{
				case 0:
				{
					if( !m_hasCustomFunction )
					{

						//Tess
						uniforms += "\t\tuniform " + UIUtils.FinalPrecisionWirePortToCgType( PrecisionType.Float, WirePortDataType.FLOAT ) + " " + TessUniformName + ";\n";

						//Min
						uniforms += "\t\tuniform " + UIUtils.FinalPrecisionWirePortToCgType( PrecisionType.Float, WirePortDataType.FLOAT ) + " " + TessMinUniformName + ";\n";

						//Max
						uniforms += "\t\tuniform " + UIUtils.FinalPrecisionWirePortToCgType( PrecisionType.Float, WirePortDataType.FLOAT ) + " " + TessMaxUniformName + ";\n";
					}
				}
				break;
				case 1:
				//Tess
				if( !m_hasCustomFunction )
				{
					uniforms += "\t\tuniform " + UIUtils.FinalPrecisionWirePortToCgType( PrecisionType.Float, WirePortDataType.FLOAT ) + " " + TessUniformName + ";\n";
				}
				break;
			}

			if( m_phongEnabled )
			{
				uniforms += "\t\tuniform " + UIUtils.FinalPrecisionWirePortToCgType( PrecisionType.Float, WirePortDataType.FLOAT ) + " " + PhongStrengthUniformName + ";\n" ;
			}

			return uniforms;
		}

		public void AddToDataCollector( ref MasterNodeDataCollector dataCollector, int reorder )
		{
			int orderIndex = reorder;
			switch ( m_tessType )
			{
				case 0:
				{
					dataCollector.AddToIncludes( -1, TessellationOpHelper.TessInclude );
					if ( !m_hasCustomFunction )
					{
						//Tess
						dataCollector.AddToProperties( -1, string.Format( TessProperty, m_tessFactor ), orderIndex++ );
						dataCollector.AddToUniforms( -1, "uniform " + UIUtils.FinalPrecisionWirePortToCgType( PrecisionType.Float, WirePortDataType.FLOAT ) + " " + TessUniformName + ";" );

						//Min
						dataCollector.AddToProperties( -1, string.Format( TessMinProperty, m_tessMinDistance ), orderIndex++ );
						dataCollector.AddToUniforms( -1, "uniform " + UIUtils.FinalPrecisionWirePortToCgType( PrecisionType.Float, WirePortDataType.FLOAT ) + " " + TessMinUniformName + ";" );

						//Max
						dataCollector.AddToProperties( -1, string.Format( TessMaxProperty, m_tessMaxDistance ), orderIndex++ );
						dataCollector.AddToUniforms( -1, "uniform " + UIUtils.FinalPrecisionWirePortToCgType( PrecisionType.Float, WirePortDataType.FLOAT ) + " " + TessMaxUniformName + ";" );
					}
				}
				break;
				case 1:
				{
					//Tess
					if ( !m_hasCustomFunction )
					{
						dataCollector.AddToProperties( -1, string.Format( TessProperty, m_tessFactor ), orderIndex++ );
						dataCollector.AddToUniforms( -1, "uniform " + UIUtils.FinalPrecisionWirePortToCgType( PrecisionType.Float, WirePortDataType.FLOAT ) + " " + TessUniformName + ";" );
					}
				}
				break;
				case 2:
				{
					dataCollector.AddToIncludes( -1, TessellationOpHelper.TessInclude );

					//Tess
					if ( !m_hasCustomFunction )
					{
						dataCollector.AddToProperties( -1, string.Format( EdgeLengthTessProperty, m_tessFactor ), orderIndex++ );
						dataCollector.AddToUniforms( -1, "uniform " + UIUtils.FinalPrecisionWirePortToCgType( PrecisionType.Float, WirePortDataType.FLOAT ) + " " + EdgeLengthTessUniformName + ";" );
					}
				}
				break;
				case 3:
				{
					dataCollector.AddToIncludes( -1, TessellationOpHelper.TessInclude );

					if ( !m_hasCustomFunction )
					{
						//Tess
						dataCollector.AddToProperties( -1, string.Format( EdgeLengthTessProperty, m_tessFactor ), orderIndex++ );
						dataCollector.AddToUniforms( -1, "uniform " + UIUtils.FinalPrecisionWirePortToCgType( PrecisionType.Float, WirePortDataType.FLOAT ) + " " + EdgeLengthTessUniformName + ";" );

						//Max Displacement
						dataCollector.AddToProperties( -1, string.Format( EdgeLengthTessMaxDispProperty, m_tessMaxDistance ), orderIndex++ );
						dataCollector.AddToUniforms( -1, "uniform " + UIUtils.FinalPrecisionWirePortToCgType( PrecisionType.Float, WirePortDataType.FLOAT ) + " " + EdgeLengthTessMaxDispUniformName + ";" );
					}
				}
				break;
			}

			if ( m_phongEnabled )
			{
				dataCollector.AddToProperties( -1, string.Format( PhongStrengthProperty, m_phongStrength ), orderIndex++ );
				dataCollector.AddToUniforms( -1, "uniform " + UIUtils.FinalPrecisionWirePortToCgType( PrecisionType.Float, WirePortDataType.FLOAT ) + " " + PhongStrengthUniformName + ";" );
			}
		}

		//ToDo: Optimize material property fetches to use Id instead of string 
		public void UpdateFromMaterial( Material mat )
		{
			if ( m_enabled )
			{
				if ( m_phongEnabled )
				{
					if ( mat.HasProperty( PhongStrengthUniformName ) )
						m_phongStrength = mat.GetFloat( PhongStrengthUniformName );
				}

				switch ( m_tessType )
				{
					case 0:
					{
						if ( mat.HasProperty( TessUniformName ) )
							m_tessFactor = mat.GetFloat( TessUniformName );

						if ( mat.HasProperty( TessMinUniformName ) )
							m_tessMinDistance = mat.GetFloat( TessMinUniformName );

						if ( mat.HasProperty( TessMaxUniformName ) )
							m_tessMaxDistance = mat.GetFloat( TessMaxUniformName );
					}
					break;
					case 1:
					{
						if ( mat.HasProperty( TessUniformName ) )
							m_tessFactor = mat.GetFloat( TessUniformName );
					}
					break;
					case 2:
					{
						if ( mat.HasProperty( EdgeLengthTessUniformName ) )
							m_tessFactor = mat.GetFloat( EdgeLengthTessUniformName );
					}
					break;
					case 3:
					{
						if ( mat.HasProperty( EdgeLengthTessUniformName ) )
							m_tessFactor = mat.GetFloat( EdgeLengthTessUniformName );

						if ( mat.HasProperty( EdgeLengthTessMaxDispUniformName ) )
							m_tessMaxDistance = mat.GetFloat( EdgeLengthTessMaxDispUniformName );
					}
					break;
				}
			}
		}

		public void WriteToOptionalParams( ref string optionalParams )
		{
			optionalParams += TessellationOpHelper.TessSurfParam + Constants.OptionalParametersSep;
			if ( m_phongEnabled )
			{
				optionalParams += TessellationOpHelper.PhongStrengthParam + Constants.OptionalParametersSep;
			}
		}

		public void Reset()
		{
			m_hasCustomFunction = false;
			m_customFunction = string.Empty;

			m_additionalData = string.Empty;
			m_additionalDataDict.Clear();
			switch ( m_tessType )
			{
				case 0:
				{
					m_customData[ 0 ] = TessUniformName;
					m_customData[ 1 ] = TessMinUniformName;
					m_customData[ 2 ] = TessMaxUniformName;
				}
				break;
				case 1:
				{
					m_customData[ 0 ] = TessUniformName;
					m_customData[ 1 ] = string.Empty;
					m_customData[ 2 ] = string.Empty;
				}
				break;
				case 2:
				{
					m_customData[ 0 ] = EdgeLengthTessUniformName;
					m_customData[ 1 ] = string.Empty;
					m_customData[ 2 ] = string.Empty;
				}
				break;
				case 3:
				{
					m_customData[ 0 ] = EdgeLengthTessUniformName;
					m_customData[ 1 ] = EdgeLengthTessMaxDispUniformName;
					m_customData[ 2 ] = string.Empty;
				}
				break;
			}
		}

		public string GetCurrentTessellationFunction
		{
			get
			{
				if ( m_hasCustomFunction )
				{
					return TessFunctionOpen +
							m_customFunction +
							TessFunctionClose;
				}

				string tessFunction = string.Empty;
				switch ( m_tessType )
				{
					case 0:
					{
						tessFunction = TessFunctionOpen +
										DistBasedTessFunctionBody +
										TessFunctionClose;
					}
					break;
					case 1:
					{
						tessFunction = FixedAmountTessFunctionOpen +
										FixedAmountTessFunctionBody +
										TessFunctionClose;
					}
					break;
					case 2:
					{
						tessFunction = TessFunctionOpen +
										EdgeLengthTessFunctionBody +
										TessFunctionClose;
					}
					break;
					case 3:
					{
						tessFunction = TessFunctionOpen +
										EdgeLengthTessCullFunctionBody +
										TessFunctionClose;
					}
					break;
				}
				return tessFunction;
			}
		}

		public void AddAdditionalData( string data )
		{
			if ( !m_additionalDataDict.ContainsKey( data ) )
			{
				m_additionalDataDict.Add( data, true );
				m_additionalData += data;
			}
		}

		public void AddCustomFunction( string returnData )
		{
			m_hasCustomFunction = true;
			m_customFunction = m_additionalData + string.Format( CustomFunctionBody, returnData );
		}

		public void Destroy()
		{
			m_additionalDataDict.Clear();
			m_additionalDataDict = null;
		}

		public bool IsTessellationPort( int index )
		{
			return index == m_masterNodeIndexPort;
		}

		public bool EnableTesselation { get { return m_enabled; } }

		public int TessType { get { return m_tessType; } }
		public int MasterNodeIndexPort
		{
			get { return m_masterNodeIndexPort; }
			set { m_masterNodeIndexPort = value; }
		}
		public int VertexOffsetIndexPort
		{
			get { return m_vertexOffsetIndexPort; }
			set { m_vertexOffsetIndexPort = value; }
		}

		public StandardSurfaceOutputNode ParentSurface { get { return m_parentSurface; } set { m_parentSurface = value; } }
	}
}
