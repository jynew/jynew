// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Color", "Constants And Properties", "Color property", null, KeyCode.Alpha5 )]
	public sealed class ColorNode : PropertyNode
	{
		private const string ColorSpaceStr = "Color Space";

		[SerializeField]
#if UNITY_2018_1_OR_NEWER
		[ColorUsage( true, true )]
#else
		[ColorUsage( true, true, float.MinValue, float.MinValue, float.MinValue, float.MaxValue )]
#endif
		private Color m_defaultValue = new Color( 0, 0, 0, 0 );

		[SerializeField]
#if UNITY_2018_1_OR_NEWER
		[ColorUsage( true, true )]
#else
		[ColorUsage( true, true, float.MinValue, float.MinValue, float.MinValue, float.MaxValue )]
#endif
		private Color m_materialValue = new Color( 0, 0, 0, 0 );

		[SerializeField]
		private bool m_isHDR = false;

		//[SerializeField]
		//private ASEColorSpace m_colorSpace = ASEColorSpace.Auto;
#if !UNITY_2018_1_OR_NEWER
		private ColorPickerHDRConfig m_hdrConfig = new ColorPickerHDRConfig( 0, float.MaxValue, 0, float.MaxValue );
#endif
		private GUIContent m_dummyContent;

		private int m_cachedPropertyId = -1;

		private bool m_isEditingFields;

		[SerializeField]
		private bool m_autoGammaToLinearConversion = true;

		private const string AutoGammaToLinearConversion = "IsGammaSpace() ? {0} : {1}";
		private const string AutoGammaToLinearStr = "Auto Gamma To Linear";

		public ColorNode() : base() { }
		public ColorNode( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { }

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			GlobalTypeWarningText = string.Format( GlobalTypeWarningText, "Color" );
			m_insideSize.Set( 100, 50 );
			m_dummyContent = new GUIContent();
			AddOutputColorPorts( "RGBA" );
			m_drawPreview = false;
			m_drawPreviewExpander = false;
			m_canExpand = false;
			m_selectedLocation = PreviewLocation.BottomCenter;
			m_precisionString = UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType );
			m_previewShaderGUID = "6cf365ccc7ae776488ae8960d6d134c3";
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if( m_cachedPropertyId == -1 )
				m_cachedPropertyId = Shader.PropertyToID( "_InputColor" );

			if( m_materialMode && m_currentParameterType != PropertyType.Constant )
				PreviewMaterial.SetColor( m_cachedPropertyId, m_materialValue );
			else
				PreviewMaterial.SetColor( m_cachedPropertyId, m_defaultValue );
		}

		public override void CopyDefaultsToMaterial()
		{
			m_materialValue = m_defaultValue;
		}

		public override void DrawSubProperties()
		{
			m_textLabelWidth = ( m_currentParameterType == PropertyType.Constant ) ? 152 : 105;

#if UNITY_2018_1_OR_NEWER
			m_defaultValue = EditorGUILayoutColorField( Constants.DefaultValueLabelContent, m_defaultValue, false, true, m_isHDR );
#else
			m_defaultValue = EditorGUILayoutColorField( Constants.DefaultValueLabelContent, m_defaultValue, false, true, m_isHDR, m_hdrConfig );
#endif
			if( m_currentParameterType == PropertyType.Constant )
			{
				
				m_autoGammaToLinearConversion = EditorGUILayoutToggle( AutoGammaToLinearStr, m_autoGammaToLinearConversion );
			}
		}

		//public override void DrawMainPropertyBlock()
		//{
		//	EditorGUILayout.BeginVertical();
		//	{

		//		PropertyType parameterType = (PropertyType)EditorGUILayoutEnumPopup( ParameterTypeStr, m_currentParameterType );
		//		if( parameterType != m_currentParameterType )
		//		{
		//			ChangeParameterType( parameterType );
		//			BeginPropertyFromInspectorCheck();
		//		}

		//		switch( m_currentParameterType )
		//		{
		//			case PropertyType.Property:
		//			case PropertyType.InstancedProperty:
		//			{
		//				ShowPropertyInspectorNameGUI();
		//				ShowPropertyNameGUI( true );
		//				ShowVariableMode();
		//				ShowPrecision();
		//				ShowToolbar();
		//			}
		//			break;
		//			case PropertyType.Global:
		//			{
		//				ShowPropertyInspectorNameGUI();
		//				ShowPropertyNameGUI( false );
		//				ShowVariableMode();
		//				ShowPrecision();
		//				ShowDefaults();
		//			}
		//			break;
		//			case PropertyType.Constant:
		//			{
		//				ShowPropertyInspectorNameGUI();
		//				ShowPrecision();
		//				m_colorSpace = (ASEColorSpace)EditorGUILayoutEnumPopup( ColorSpaceStr, m_colorSpace );
		//				ShowDefaults();
		//			}
		//			break;
		//		}
		//	}
		//	EditorGUILayout.EndVertical();
		//}

		public override void DrawMaterialProperties()
		{
			if( m_materialMode )
				EditorGUI.BeginChangeCheck();
#if UNITY_2018_1_OR_NEWER
			m_materialValue = EditorGUILayoutColorField( Constants.MaterialValueLabelContent, m_materialValue, false, true, m_isHDR );
#else
			m_materialValue = EditorGUILayoutColorField( Constants.MaterialValueLabelContent, m_materialValue, false, true, m_isHDR, m_hdrConfig );
#endif
			if( m_materialMode && EditorGUI.EndChangeCheck() )
				m_requireMaterialUpdate = true;
		}

		public override void OnNodeLayout( DrawInfo drawInfo )
		{
			base.OnNodeLayout( drawInfo );

			m_propertyDrawPos = m_globalPosition;
			m_propertyDrawPos.x = m_remainingBox.x;
			m_propertyDrawPos.y = m_remainingBox.y;
			m_propertyDrawPos.width = 80 * drawInfo.InvertedZoom;
			m_propertyDrawPos.height = m_remainingBox.height;
		}

		public override void DrawGUIControls( DrawInfo drawInfo )
		{
			base.DrawGUIControls( drawInfo );

			if( drawInfo.CurrentEventType != EventType.MouseDown )
				return;

			Rect hitBox = m_remainingBox;
			//hitBox.xMin -= LabelWidth * drawInfo.InvertedZoom;
			bool insideBox = hitBox.Contains( drawInfo.MousePosition );

			if( insideBox )
			{
				m_isEditingFields = true;
			}
			else if( m_isEditingFields && !insideBox )
			{
				GUI.FocusControl( null );
				m_isEditingFields = false;
			}
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );

			if( !m_isVisible )
				return;

			if( m_isEditingFields && m_currentParameterType != PropertyType.Global )
			{
				if( m_materialMode && m_currentParameterType != PropertyType.Constant )
				{
					EditorGUI.BeginChangeCheck();
#if UNITY_2018_1_OR_NEWER
					m_materialValue = EditorGUIColorField( m_propertyDrawPos, m_dummyContent, m_materialValue, false, true, m_isHDR );
#else
					m_materialValue = EditorGUIColorField( m_propertyDrawPos, m_dummyContent, m_materialValue, false, true, m_isHDR, m_hdrConfig );
#endif
					if( EditorGUI.EndChangeCheck() )
					{
						m_requireMaterialUpdate = true;
						if( m_currentParameterType != PropertyType.Constant )
						{
							BeginDelayedDirtyProperty();
						}
					}
				}
				else
				{
					EditorGUI.BeginChangeCheck();
#if UNITY_2018_1_OR_NEWER
					m_defaultValue = EditorGUIColorField( m_propertyDrawPos, m_dummyContent, m_defaultValue, false, true, m_isHDR );
#else
					m_defaultValue = EditorGUIColorField( m_propertyDrawPos, m_dummyContent, m_defaultValue, false, true, m_isHDR, m_hdrConfig );
#endif
					if( EditorGUI.EndChangeCheck() )
					{
						BeginDelayedDirtyProperty();
					}
				}
			}
			else if( drawInfo.CurrentEventType == EventType.Repaint )
			{
				if( m_materialMode && m_currentParameterType != PropertyType.Constant )
					EditorGUIUtility.DrawColorSwatch( m_propertyDrawPos, m_materialValue );
				else
					EditorGUIUtility.DrawColorSwatch( m_propertyDrawPos, m_defaultValue );

				GUI.Label( m_propertyDrawPos, string.Empty, UIUtils.GetCustomStyle( CustomStyle.SamplerFrame ) );
			}
		}

		public override void ConfigureLocalVariable( ref MasterNodeDataCollector dataCollector )
		{
			Color color = m_defaultValue;
			//switch( m_colorSpace )
			//{
			//	default:
			//	case ASEColorSpace.Auto: color = m_defaultValue; break;
			//	case ASEColorSpace.Gamma: color = m_defaultValue.gamma; break;
			//	case ASEColorSpace.Linear: color = m_defaultValue.linear; break;
			//}

			dataCollector.AddLocalVariable( UniqueId, CreateLocalVarDec( color.r + "," + color.g + "," + color.b + "," + color.a ) );

			m_outputPorts[ 0 ].SetLocalValue( m_propertyName , dataCollector.PortCategory);
			m_outputPorts[ 1 ].SetLocalValue( m_propertyName + ".r", dataCollector.PortCategory );
			m_outputPorts[ 2 ].SetLocalValue( m_propertyName + ".g", dataCollector.PortCategory );
			m_outputPorts[ 3 ].SetLocalValue( m_propertyName + ".b", dataCollector.PortCategory );
			m_outputPorts[ 4 ].SetLocalValue( m_propertyName + ".a", dataCollector.PortCategory );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			m_precisionString = UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType );

			if( m_currentParameterType != PropertyType.Constant )
				return GetOutputVectorItem( 0, outputId, PropertyData( dataCollector.PortCategory ) );

			// Constant Only Code

			if( m_outputPorts[ outputId ].IsLocalValue(dataCollector.PortCategory) )
			{
				return m_outputPorts[ outputId ].LocalValue( dataCollector.PortCategory );
			}
			if( m_autoGammaToLinearConversion )
			{
				if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
					return GetOutputColorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue(dataCollector.PortCategory) );
				
				Color linear = m_defaultValue.linear;

				string colorGamma = m_precisionString + "(" + m_defaultValue.r + "," + m_defaultValue.g + "," + m_defaultValue.b + "," + m_defaultValue.a + ")";
				string colorLinear = m_precisionString + "(" + linear.r + "," + linear.g + "," + linear.b + "," + m_defaultValue.a + ")";

				string result = string.Format( AutoGammaToLinearConversion, colorGamma, colorLinear );
				RegisterLocalVariable( 0, result, ref dataCollector, "color" + OutputId );
				return GetOutputColorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );
			}
			else
			{
				if( CheckLocalVariable( ref dataCollector ) )
				{
					return m_outputPorts[ outputId ].LocalValue( dataCollector.PortCategory );
				}

				Color color = m_defaultValue;
				//switch( m_colorSpace )
				//{
				//	default:
				//	case ASEColorSpace.Auto: color = m_defaultValue; break;
				//	case ASEColorSpace.Gamma: color = m_defaultValue.gamma; break;
				//	case ASEColorSpace.Linear: color = m_defaultValue.linear; break;
				//}
				string result = string.Empty;

				switch( outputId )
				{
					case 0:
					{
						result = m_precisionString + "(" + color.r + "," + color.g + "," + color.b + "," + color.a + ")";
					}
					break;

					case 1:
					{
						result = color.r.ToString();
					}
					break;
					case 2:
					{
						result = color.g.ToString();
					}
					break;
					case 3:
					{
						result = color.b.ToString();
					}
					break;
					case 4:
					{
						result = color.a.ToString();
					}
					break;
				}
				return result;
			}
		}

		protected override void OnAtrributesChanged()
		{
			CheckIfHDR();
		}

		public override void RefreshExternalReferences()
		{
			base.RefreshExternalReferences();
			CheckIfHDR();
		}

		void CheckIfHDR()
		{
			int count = m_selectedAttribs.Count;
			bool hdrBuffer = m_isHDR;
			m_isHDR = false;
			for( int i = 0; i < count; i++ )
			{
				if( m_selectedAttribs[ i ] == 1 /*HDR Property ID*/)
				{
					m_isHDR = true;
					break;
				}
			}

			if( hdrBuffer && !m_isHDR )
			{
				bool fireDirtyProperty = false;

				if( m_defaultValue.r > 1 || m_defaultValue.g > 1 || m_defaultValue.b > 1 )
				{
					float defaultColorLength = Mathf.Sqrt( m_defaultValue.r * m_defaultValue.r + m_defaultValue.g * m_defaultValue.g + m_defaultValue.b * m_defaultValue.b );
					m_defaultValue.r /= defaultColorLength;
					m_defaultValue.g /= defaultColorLength;
					m_defaultValue.b /= defaultColorLength;
					fireDirtyProperty = true;
				}

				if( m_materialValue.r > 1 || m_materialValue.g > 1 || m_materialValue.b > 1 )
				{
					float materialColorLength = Mathf.Sqrt( m_materialValue.r * m_materialValue.r + m_materialValue.g * m_materialValue.g + m_materialValue.b * m_materialValue.b );
					m_materialValue.r /= materialColorLength;
					m_materialValue.g /= materialColorLength;
					m_materialValue.b /= materialColorLength;
					fireDirtyProperty = true;
				}

				if( fireDirtyProperty )
					BeginDelayedDirtyProperty();
			}
		}

		public override string GetPropertyValue()
		{
			return PropertyAttributes + m_propertyName + "(\"" + m_propertyInspectorName + "\", Color) = (" + m_defaultValue.r + "," + m_defaultValue.g + "," + m_defaultValue.b + "," + m_defaultValue.a + ")";
		}

		public override void UpdateMaterial( Material mat )
		{
			base.UpdateMaterial( mat );

			if( UIUtils.IsProperty( m_currentParameterType ) && !InsideShaderFunction )
			{
				mat.SetColor( m_propertyName, m_materialValue );
			}
		}

		public override void SetMaterialMode( Material mat, bool fetchMaterialValues )
		{
			base.SetMaterialMode( mat, fetchMaterialValues );
			if( m_materialMode && fetchMaterialValues )
			{
				if( UIUtils.IsProperty( m_currentParameterType ) && mat.HasProperty( m_propertyName ) )
					MaterialValue = mat.GetColor( m_propertyName );
			}
		}

		public override void ForceUpdateFromMaterial( Material material )
		{
			if( UIUtils.IsProperty( m_currentParameterType ) && material.HasProperty( m_propertyName ) )
			{
				MaterialValue = material.GetColor( m_propertyName );
			}
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_defaultValue = IOUtils.StringToColor( GetCurrentParam( ref nodeParams ) );
			if( UIUtils.CurrentShaderVersion() > 14101 )
			{
				m_materialValue = IOUtils.StringToColor( GetCurrentParam( ref nodeParams ) );
			}

			if( UIUtils.CurrentShaderVersion() > 15900 )
			{
				m_autoGammaToLinearConversion = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			}
			else
			{
				m_autoGammaToLinearConversion = false;
			}
			//if( UIUtils.CurrentShaderVersion() > 14202 )
			//{
			//	m_colorSpace = (ASEColorSpace)Enum.Parse( typeof( ASEColorSpace ), GetCurrentParam( ref nodeParams ) );
			//}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, IOUtils.ColorToString( m_defaultValue ) );
			IOUtils.AddFieldValueToString( ref nodeInfo, IOUtils.ColorToString( m_materialValue ) );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_autoGammaToLinearConversion );
			//IOUtils.AddFieldValueToString( ref nodeInfo, m_colorSpace );
		}

		public override void SetGlobalValue() { Shader.SetGlobalColor( m_propertyName, m_defaultValue ); }
		public override void FetchGlobalValue() { m_materialValue = Shader.GetGlobalColor( m_propertyName ); }

		public override string GetPropertyValStr()
		{
			return ( m_materialMode && m_currentParameterType != PropertyType.Constant ) ? m_materialValue.r.ToString( Constants.PropertyVectorFormatLabel ) + IOUtils.VECTOR_SEPARATOR +
																						m_materialValue.g.ToString( Constants.PropertyVectorFormatLabel ) + IOUtils.VECTOR_SEPARATOR +
																						m_materialValue.b.ToString( Constants.PropertyVectorFormatLabel ) + IOUtils.VECTOR_SEPARATOR +
																						m_materialValue.a.ToString( Constants.PropertyVectorFormatLabel ) :
																						m_defaultValue.r.ToString( Constants.PropertyVectorFormatLabel ) + IOUtils.VECTOR_SEPARATOR +
																						m_defaultValue.g.ToString( Constants.PropertyVectorFormatLabel ) + IOUtils.VECTOR_SEPARATOR +
																						m_defaultValue.b.ToString( Constants.PropertyVectorFormatLabel ) + IOUtils.VECTOR_SEPARATOR +
																						m_defaultValue.a.ToString( Constants.PropertyVectorFormatLabel );
		}

		private Color MaterialValue
		{
			set
			{
				if( !m_isHDR && ( value.r > 1 || value.g > 1 || value.r > 1 ) )
				{
					float materialColorLength = Mathf.Sqrt( value.r * value.r + value.g * value.g + value.b * value.b );
					m_materialValue.r = value.r / materialColorLength;
					m_materialValue.g = value.g / materialColorLength;
					m_materialValue.b = value.b / materialColorLength;
					m_materialValue.a = value.a;
				}
				else
				{
					m_materialValue = value;
				}
			}
		}

		public Color Value
		{
			get { return m_defaultValue; }
			set { m_defaultValue = value; }
		}
	}
}
