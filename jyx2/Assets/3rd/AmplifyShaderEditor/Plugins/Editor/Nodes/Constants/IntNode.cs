// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Int", "Constants And Properties", "Int property", null, KeyCode.Alpha0 )]
	public sealed class IntNode : PropertyNode
	{
		[SerializeField]
		private int m_defaultValue;

		[SerializeField]
		private int m_materialValue;

		private const float LabelWidth = 8;

		private int m_cachedPropertyId = -1;

		private bool m_isEditingFields;
		private int m_previousValue;
		private string m_fieldText = "0";

		public IntNode() : base() { }
		public IntNode( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { }
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			GlobalTypeWarningText = string.Format( GlobalTypeWarningText, "Int" );
			AddOutputPort( WirePortDataType.INT, Constants.EmptyPortValue );
			m_insideSize.Set( 50, 10 );
			m_selectedLocation = PreviewLocation.BottomCenter;
			m_precisionString = UIUtils.PrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType );
			m_drawPrecisionUI = false;
			m_availableAttribs.Add( new PropertyAttributes( "Enum", "[Enum]" ) );
			m_previewShaderGUID = "0f64d695b6ffacc469f2dd31432a232a";
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

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if( m_cachedPropertyId == -1 )
				m_cachedPropertyId = Shader.PropertyToID( "_InputInt" );

			if( m_materialMode && m_currentParameterType != PropertyType.Constant )
				PreviewMaterial.SetInt( m_cachedPropertyId, m_materialValue );
			else
				PreviewMaterial.SetInt( m_cachedPropertyId, m_defaultValue );
		}


		public override void CopyDefaultsToMaterial()
		{
			m_materialValue = m_defaultValue;
		}

		public override void DrawSubProperties()
		{
			m_defaultValue = EditorGUILayoutIntField( Constants.DefaultValueLabel, m_defaultValue );
		}

		public override void DrawMaterialProperties()
		{
			if( m_materialMode )
				EditorGUI.BeginChangeCheck();

			m_materialValue = EditorGUILayoutIntField( Constants.MaterialValueLabel, m_materialValue );

			if( m_materialMode && EditorGUI.EndChangeCheck() )
			{
				m_requireMaterialUpdate = true;
			}
		}

		public override void OnNodeLayout( DrawInfo drawInfo )
		{
			base.OnNodeLayout( drawInfo );

			m_propertyDrawPos = m_remainingBox;
			m_propertyDrawPos.x = m_remainingBox.x - LabelWidth * drawInfo.InvertedZoom;
			m_propertyDrawPos.width = drawInfo.InvertedZoom * Constants.FLOAT_DRAW_WIDTH_FIELD_SIZE;
			m_propertyDrawPos.height = drawInfo.InvertedZoom * Constants.FLOAT_DRAW_HEIGHT_FIELD_SIZE;
		}

		public override void DrawGUIControls( DrawInfo drawInfo )
		{
			base.DrawGUIControls( drawInfo );

			if( drawInfo.CurrentEventType != EventType.MouseDown )
				return;

			Rect hitBox = m_remainingBox;
			hitBox.xMin -= LabelWidth * drawInfo.InvertedZoom;
			bool insideBox = hitBox.Contains( drawInfo.MousePosition );

			if( insideBox )
			{
				GUI.FocusControl( null );
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
				float labelWidth = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = LabelWidth * drawInfo.InvertedZoom;

				if( m_materialMode && m_currentParameterType != PropertyType.Constant )
				{
					EditorGUI.BeginChangeCheck();
					m_materialValue = EditorGUIIntField( m_propertyDrawPos, "  ", m_materialValue, UIUtils.MainSkin.textField );
					if( EditorGUI.EndChangeCheck() )
					{
						m_requireMaterialUpdate = true;
						if( m_currentParameterType != PropertyType.Constant )
							BeginDelayedDirtyProperty();
					}
				}
				else
				{
					EditorGUI.BeginChangeCheck();

					m_defaultValue = EditorGUIIntField( m_propertyDrawPos, "  ", m_defaultValue, UIUtils.MainSkin.textField );

					if( EditorGUI.EndChangeCheck() )
						BeginDelayedDirtyProperty();
				}
				EditorGUIUtility.labelWidth = labelWidth;
			}
			else if( drawInfo.CurrentEventType == EventType.Repaint )
			{
				bool guiEnabled = GUI.enabled;
				GUI.enabled = m_currentParameterType != PropertyType.Global;
				Rect fakeField = m_propertyDrawPos;
				fakeField.xMin += LabelWidth * drawInfo.InvertedZoom;
				if( GUI.enabled )
				{
					Rect fakeLabel = m_propertyDrawPos;
					fakeLabel.xMax = fakeField.xMin;
					EditorGUIUtility.AddCursorRect( fakeLabel, MouseCursor.SlideArrow );
					EditorGUIUtility.AddCursorRect( fakeField, MouseCursor.Text );
				}
				bool currMode = m_materialMode && m_currentParameterType != PropertyType.Constant;
				int value = currMode ? m_materialValue : m_defaultValue;

				if( m_previousValue != value )
				{
					m_previousValue = value;
					m_fieldText = value.ToString();
				}

				GUI.Label( fakeField, m_fieldText, UIUtils.MainSkin.textField );
				GUI.enabled = guiEnabled;
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );

			if( m_currentParameterType != PropertyType.Constant )
				return PropertyData( dataCollector.PortCategory );

			return m_defaultValue.ToString();
		}

		public override string GetPropertyValue()
		{
			return PropertyAttributes + m_propertyName + "(\"" + m_propertyInspectorName + "\", Int) = " + m_defaultValue;
		}

		public override void UpdateMaterial( Material mat )
		{
			base.UpdateMaterial( mat );
			if( UIUtils.IsProperty( m_currentParameterType ) && !InsideShaderFunction )
			{
				mat.SetInt( m_propertyName, m_materialValue );
			}
		}

		public override void SetMaterialMode( Material mat, bool fetchMaterialValues )
		{
			base.SetMaterialMode( mat, fetchMaterialValues );
			if( fetchMaterialValues && m_materialMode && UIUtils.IsProperty( m_currentParameterType ) && mat.HasProperty( m_propertyName ) )
			{
				m_materialValue = mat.GetInt( m_propertyName );
			}
		}

		public override void ForceUpdateFromMaterial( Material material )
		{
			if( UIUtils.IsProperty( m_currentParameterType ) && material.HasProperty( m_propertyName ) )
				m_materialValue = material.GetInt( m_propertyName );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_defaultValue = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			if( UIUtils.CurrentShaderVersion() > 14101 )
				m_materialValue = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_defaultValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_materialValue );
		}

		public override string GetPropertyValStr()
		{
			return ( m_materialMode && m_currentParameterType != PropertyType.Constant ) ?
				m_materialValue.ToString( Mathf.Abs( m_materialValue ) > 1000 ? Constants.PropertyBigIntFormatLabel : Constants.PropertyIntFormatLabel ) :
				m_defaultValue.ToString( Mathf.Abs( m_defaultValue ) > 1000 ? Constants.PropertyBigIntFormatLabel : Constants.PropertyIntFormatLabel );
		}

		public override void SetGlobalValue() { Shader.SetGlobalInt( m_propertyName, m_defaultValue ); }
		public override void FetchGlobalValue() { m_materialValue = Shader.GetGlobalInt( m_propertyName ); }
		public int Value
		{
			get { return m_defaultValue; }
			set { m_defaultValue = value; }
		}

		public void SetMaterialValueFromInline( int val )
		{
			m_materialValue = val;
			m_requireMaterialUpdate = true;
		}
	}
}
