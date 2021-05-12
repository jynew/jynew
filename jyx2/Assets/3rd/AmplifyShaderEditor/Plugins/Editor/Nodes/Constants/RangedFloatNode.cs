// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Float", "Constants And Properties", "Float property", null, KeyCode.Alpha1 )]
	public sealed class RangedFloatNode : PropertyNode
	{
		private const int OriginalFontSize = 11;

		private const string MinValueStr = "Min";
		private const string MaxValueStr = "Max";

		private const float LabelWidth = 8;

		[SerializeField]
		private float m_defaultValue = 0;

		[SerializeField]
		private float m_materialValue = 0;

		[SerializeField]
		private float m_min = 0;

		[SerializeField]
		private float m_max = 0;

		[SerializeField]
		private bool m_floatMode = true;

		private int m_cachedPropertyId = -1;

		private bool m_isEditingFields;
		private Vector3 m_previousValue = Vector3.zero;
		private string[] m_fieldText = new string[] { "0", "0", "0" };

		public RangedFloatNode() : base() { }
		public RangedFloatNode( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { }

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			GlobalTypeWarningText = string.Format( GlobalTypeWarningText, "Float" );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_insideSize.Set( 50, 0 );
			m_showPreview = false;
			m_selectedLocation = PreviewLocation.BottomCenter;
			m_precisionString = UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType );
			m_availableAttribs.Add( new PropertyAttributes( "Toggle", "[Toggle]" ) );
			m_availableAttribs.Add( new PropertyAttributes( "Int Range", "[IntRange]" ) );
			m_availableAttribs.Add( new PropertyAttributes( "Enum", "[Enum]" ) );
			m_previewShaderGUID = "d9ca47581ac157145bff6f72ac5dd73e";
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			UIUtils.RegisterFloatIntNode( this );
		}

		public override void Destroy()
		{
			base.Destroy();
			UIUtils.UnregisterFloatIntNode( this );
		}

		public override void OnDirtyProperty()
		{
			UIUtils.UpdateFloatIntDataNode( UniqueId, PropertyInspectorName );
		}

		public override void RefreshExternalReferences()
		{
			base.RefreshExternalReferences();
			OnPropertyNameChanged();
			OnDirtyProperty();
		}

		public void SetFloatMode( bool value )
		{
			if ( m_floatMode == value )
				return;

			m_floatMode = value;
			if ( value )
			{
				m_insideSize.x = 50;// + ( m_showPreview ? 50 : 0 );
				//m_firstPreviewDraw = true;
			}
			else
			{
				m_insideSize.x = 200;// + ( m_showPreview ? 0 : 0 );
				//m_firstPreviewDraw = true;
			}
			m_sizeIsDirty = true;
		}

		public override void CopyDefaultsToMaterial()
		{
			m_materialValue = m_defaultValue;
		}

		void DrawMinMaxUI()
		{
			EditorGUI.BeginChangeCheck();
			m_min = EditorGUILayoutFloatField( MinValueStr, m_min );
			m_max = EditorGUILayoutFloatField( MaxValueStr, m_max );
			if ( m_min > m_max )
				m_min = m_max;

			if ( m_max < m_min )
				m_max = m_min;

			if ( EditorGUI.EndChangeCheck() )
			{
				SetFloatMode( m_min == m_max );
			}
		}
		public override void DrawSubProperties()
		{
			DrawMinMaxUI();

			if ( m_floatMode )
			{
				m_defaultValue = EditorGUILayoutFloatField( Constants.DefaultValueLabel, m_defaultValue );
			}
			else
			{
				m_defaultValue = EditorGUILayoutSlider( Constants.DefaultValueLabel, m_defaultValue, m_min, m_max );
			}
		}

		public override void DrawMaterialProperties()
		{
			DrawMinMaxUI();

			EditorGUI.BeginChangeCheck();

			if ( m_floatMode )
			{
				m_materialValue = EditorGUILayoutFloatField( Constants.MaterialValueLabel, m_materialValue );
			}
			else
			{
				m_materialValue = EditorGUILayoutSlider( Constants.MaterialValueLabel, m_materialValue, m_min, m_max );
			}
			if ( EditorGUI.EndChangeCheck() )
			{
				//MarkForPreviewUpdate();
				if ( m_materialMode )
					m_requireMaterialUpdate = true;
			}
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if ( m_cachedPropertyId == -1 )
				m_cachedPropertyId = Shader.PropertyToID( "_InputFloat" );

			if ( m_materialMode && m_currentParameterType != PropertyType.Constant )
				PreviewMaterial.SetFloat( m_cachedPropertyId, m_materialValue );
			else
				PreviewMaterial.SetFloat( m_cachedPropertyId, m_defaultValue );
		}

		public override void OnNodeLayout( DrawInfo drawInfo )
		{
			base.OnNodeLayout( drawInfo );

			if ( m_floatMode )
			{
				m_propertyDrawPos = m_remainingBox;
				m_propertyDrawPos.x = m_remainingBox.x - LabelWidth * drawInfo.InvertedZoom;
				m_propertyDrawPos.width = drawInfo.InvertedZoom * Constants.FLOAT_DRAW_WIDTH_FIELD_SIZE;
				m_propertyDrawPos.height = drawInfo.InvertedZoom * Constants.FLOAT_DRAW_HEIGHT_FIELD_SIZE;
			}
			else
			{
				m_propertyDrawPos = m_remainingBox;
				m_propertyDrawPos.width = m_outputPorts[ 0 ].Position.x - m_propertyDrawPos.x - (m_outputPorts[ 0 ].LabelSize.x + (Constants.PORT_TO_LABEL_SPACE_X + 3) * drawInfo.InvertedZoom + 2);
				m_propertyDrawPos.height = drawInfo.InvertedZoom * Constants.FLOAT_DRAW_HEIGHT_FIELD_SIZE;
			}
		}

		public override void DrawGUIControls( DrawInfo drawInfo )
		{
			base.DrawGUIControls( drawInfo );

			if ( drawInfo.CurrentEventType != EventType.MouseDown )
				return;

			Rect hitBox = m_remainingBox;
			hitBox.xMin -= LabelWidth * drawInfo.InvertedZoom;
			bool insideBox = hitBox.Contains( drawInfo.MousePosition );

			if ( insideBox )
			{
				GUI.FocusControl( null );
				m_isEditingFields = true;
			}
			else if ( m_isEditingFields && !insideBox )
			{
				GUI.FocusControl( null );
				m_isEditingFields = false;
			}
		}
		void DrawFakeFloatMaterial( DrawInfo drawInfo )
		{
			if( m_floatMode )
			{
				//UIUtils.DrawFloat( this, ref m_propertyDrawPos, ref m_materialValue, LabelWidth * drawInfo.InvertedZoom );
				Rect fakeField = m_propertyDrawPos;
				fakeField.xMin += LabelWidth * drawInfo.InvertedZoom;
				if( GUI.enabled )
				{
					Rect fakeLabel = m_propertyDrawPos;
					fakeLabel.xMax = fakeField.xMin;
					EditorGUIUtility.AddCursorRect( fakeLabel, MouseCursor.SlideArrow );
					EditorGUIUtility.AddCursorRect( fakeField, MouseCursor.Text );
				}
				if( m_previousValue[ 0 ] != m_materialValue )
				{
					m_previousValue[ 0 ] = m_materialValue;
					m_fieldText[ 0 ] = m_materialValue.ToString();
				}

				GUI.Label( fakeField, m_fieldText[ 0 ], UIUtils.MainSkin.textField );
			}
			else
			{
				DrawFakeSlider( ref m_materialValue, drawInfo );
			}
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );

			if ( !m_isVisible )
				return;

			if ( m_isEditingFields && m_currentParameterType != PropertyType.Global )
			{
				if ( m_materialMode && m_currentParameterType != PropertyType.Constant )
				{
					EditorGUI.BeginChangeCheck();
					if ( m_floatMode )
					{
						UIUtils.DrawFloat( this, ref m_propertyDrawPos, ref m_materialValue, LabelWidth * drawInfo.InvertedZoom );
					}
					else
					{
						DrawSlider( ref m_materialValue, drawInfo );
					}
					if ( EditorGUI.EndChangeCheck() )
					{
						m_requireMaterialUpdate = true;
						if ( m_currentParameterType != PropertyType.Constant )
						{
							BeginDelayedDirtyProperty();
						}
					}
				}
				else
				{
					EditorGUI.BeginChangeCheck();

					if ( m_floatMode )
					{
						UIUtils.DrawFloat( this, ref m_propertyDrawPos, ref m_defaultValue, LabelWidth * drawInfo.InvertedZoom );
					}
					else
					{
						DrawSlider( ref m_defaultValue, drawInfo );
					}
					if ( EditorGUI.EndChangeCheck() )
					{
						BeginDelayedDirtyProperty();
					}

				}
			}
			else if ( drawInfo.CurrentEventType == EventType.Repaint && ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD4 )
			{
				if( m_currentParameterType == PropertyType.Global )
				{
					bool guiEnabled = GUI.enabled;
					GUI.enabled = false;
					DrawFakeFloatMaterial( drawInfo );
					GUI.enabled = guiEnabled;
				}
				else if ( m_materialMode && m_currentParameterType != PropertyType.Constant )
				{
					DrawFakeFloatMaterial( drawInfo );
				}
				else
				{
					if ( m_floatMode )
					{
						//UIUtils.DrawFloat( this, ref m_propertyDrawPos, ref m_defaultValue, LabelWidth * drawInfo.InvertedZoom );
						Rect fakeField = m_propertyDrawPos;
						fakeField.xMin += LabelWidth * drawInfo.InvertedZoom;
						Rect fakeLabel = m_propertyDrawPos;
						fakeLabel.xMax = fakeField.xMin;
						EditorGUIUtility.AddCursorRect( fakeLabel, MouseCursor.SlideArrow );
						EditorGUIUtility.AddCursorRect( fakeField, MouseCursor.Text );

						if ( m_previousValue[ 0 ] != m_defaultValue )
						{
							m_previousValue[ 0 ] = m_defaultValue;
							m_fieldText[ 0 ] = m_defaultValue.ToString();
						}

						GUI.Label( fakeField, m_fieldText[ 0 ], UIUtils.MainSkin.textField );
					}
					else
					{
						DrawFakeSlider( ref m_defaultValue, drawInfo );
					}
				}
			}
		}

		void DrawFakeSlider( ref float value, DrawInfo drawInfo )
		{
			float rangeWidth = 30 * drawInfo.InvertedZoom;
			float rangeSpacing = 5 * drawInfo.InvertedZoom;

			//Min
			Rect minRect = m_propertyDrawPos;
			minRect.width = rangeWidth;
			EditorGUIUtility.AddCursorRect( minRect, MouseCursor.Text );
			if ( m_previousValue[ 1 ] != m_min )
			{
				m_previousValue[ 1 ] = m_min;
				m_fieldText[ 1 ] = m_min.ToString();
			}
			GUI.Label( minRect, m_fieldText[ 1 ], UIUtils.MainSkin.textField );

			//Value Area
			Rect valRect = m_propertyDrawPos;
			valRect.width = rangeWidth;
			valRect.x = m_propertyDrawPos.xMax - rangeWidth - rangeWidth - rangeSpacing;
			EditorGUIUtility.AddCursorRect( valRect, MouseCursor.Text );
			if ( m_previousValue[ 0 ] != value )
			{
				m_previousValue[ 0 ] = value;
				m_fieldText[ 0 ] = value.ToString();
			}
			GUI.Label( valRect, m_fieldText[ 0 ], UIUtils.MainSkin.textField );

			//Max
			Rect maxRect = m_propertyDrawPos;
			maxRect.width = rangeWidth;
			maxRect.x = m_propertyDrawPos.xMax - rangeWidth;
			EditorGUIUtility.AddCursorRect( maxRect, MouseCursor.Text );
			if ( m_previousValue[ 2 ] != m_max )
			{
				m_previousValue[ 2 ] = m_max;
				m_fieldText[ 2 ] = m_max.ToString();
			}
			GUI.Label( maxRect, m_fieldText[ 2 ], UIUtils.MainSkin.textField );

			Rect sliderValRect = m_propertyDrawPos;
			sliderValRect.x = minRect.xMax + rangeSpacing;
			sliderValRect.xMax = valRect.xMin - rangeSpacing;
			Rect sliderBackRect = sliderValRect;
			sliderBackRect.height = 5 * drawInfo.InvertedZoom;
			sliderBackRect.center = new Vector2( sliderValRect.center.x, Mathf.Round( sliderValRect.center.y ) );


			GUI.Label( sliderBackRect, string.Empty, UIUtils.GetCustomStyle( CustomStyle.SliderStyle ) );

			sliderValRect.width = 10;
			float percent = ( value - m_min) / ( m_max-m_min );
			sliderValRect.x += percent * (sliderBackRect.width - 10 * drawInfo.InvertedZoom );
			GUI.Label( sliderValRect, string.Empty, UIUtils.RangedFloatSliderThumbStyle );
		}

		void DrawSlider( ref float value, DrawInfo drawInfo )
		{
			float rangeWidth = 30 * drawInfo.InvertedZoom;
			float rangeSpacing = 5 * drawInfo.InvertedZoom;

			//Min
			Rect minRect = m_propertyDrawPos;
			minRect.width = rangeWidth;
			m_min = EditorGUIFloatField( minRect, m_min, UIUtils.MainSkin.textField );

			//Value Area
			Rect valRect = m_propertyDrawPos;
			valRect.width = rangeWidth;
			valRect.x = m_propertyDrawPos.xMax - rangeWidth - rangeWidth - rangeSpacing;
			value = EditorGUIFloatField( valRect, value, UIUtils.MainSkin.textField );

			//Max
			Rect maxRect = m_propertyDrawPos;
			maxRect.width = rangeWidth;
			maxRect.x = m_propertyDrawPos.xMax - rangeWidth;
			m_max = EditorGUIFloatField( maxRect, m_max, UIUtils.MainSkin.textField );

			//Value Slider
			Rect sliderValRect = m_propertyDrawPos;
			sliderValRect.x = minRect.xMax + rangeSpacing;
			sliderValRect.xMax = valRect.xMin - rangeSpacing;
			Rect sliderBackRect = sliderValRect;
			sliderBackRect.height = 5 * drawInfo.InvertedZoom;
			sliderBackRect.center = new Vector2( sliderValRect.center.x, Mathf.Round( sliderValRect.center.y ));
			GUI.Label( sliderBackRect, string.Empty, UIUtils.GetCustomStyle( CustomStyle.SliderStyle ) );
			value = GUIHorizontalSlider( sliderValRect, value, m_min, m_max, GUIStyle.none, UIUtils.RangedFloatSliderThumbStyle );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			m_precisionString = UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType );

			if ( m_currentParameterType != PropertyType.Constant )
				return PropertyData( dataCollector.PortCategory );

			return IOUtils.Floatify( m_defaultValue );
		}

		public override string GetPropertyValue()
		{
			if ( m_floatMode )
			{
				return PropertyAttributes + m_propertyName + "(\"" + m_propertyInspectorName + "\", Float) = " + m_defaultValue;
			}
			else
			{
				return PropertyAttributes + m_propertyName + "(\"" + m_propertyInspectorName + "\", Range( " + m_min + " , " + m_max + ")) = " + m_defaultValue;
			}
		}

		public override void UpdateMaterial( Material mat )
		{
			base.UpdateMaterial( mat );
			if ( UIUtils.IsProperty( m_currentParameterType ) && !InsideShaderFunction )
			{
				mat.SetFloat( m_propertyName, m_materialValue );
			}
		}

		public override void SetMaterialMode( Material mat , bool fetchMaterialValues )
		{
			base.SetMaterialMode( mat , fetchMaterialValues );
			if ( fetchMaterialValues && m_materialMode && UIUtils.IsProperty( m_currentParameterType ) && mat.HasProperty( m_propertyName ) )
			{
				m_materialValue = mat.GetFloat( m_propertyName );
			}
		}

		public override void ForceUpdateFromMaterial( Material material )
		{
			if ( UIUtils.IsProperty( m_currentParameterType ) && material.HasProperty( m_propertyName ) )
				m_materialValue = material.GetFloat( m_propertyName );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_defaultValue = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
			if( UIUtils.CurrentShaderVersion() > 14101 )
			{
				m_materialValue = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
			}

			m_min = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
			m_max = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
			SetFloatMode( m_min == m_max );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_defaultValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_materialValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_min );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_max );
		}

		public override string GetPropertyValStr()
		{
			return ( m_materialMode && m_currentParameterType != PropertyType.Constant ) ?
				m_materialValue.ToString( Mathf.Abs( m_materialValue ) > 1000 ? Constants.PropertyBigFloatFormatLabel : Constants.PropertyFloatFormatLabel ) :
				m_defaultValue.ToString( Mathf.Abs( m_defaultValue ) > 1000 ? Constants.PropertyBigFloatFormatLabel : Constants.PropertyFloatFormatLabel );
		}

		public override void SetGlobalValue() { Shader.SetGlobalFloat( m_propertyName, m_defaultValue ); }
		public override void FetchGlobalValue() { m_materialValue = Shader.GetGlobalFloat( m_propertyName ); }
		public float Value
		{
			get { return m_defaultValue; }
			set { m_defaultValue = value; }
		}

		public void SetMaterialValueFromInline( float val )
		{
			m_materialValue = val;
			m_requireMaterialUpdate = true;
		}
	}
}
