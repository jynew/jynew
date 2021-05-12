// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Matrix4X4", "Constants And Properties", "Matrix4X4 property" )]
	public sealed class Matrix4X4Node : MatrixParentNode
	{	
		private string[,] m_fieldText = new string[ 4, 4 ] { { "0", "0", "0", "0" }, { "0", "0", "0", "0" }, { "0", "0", "0", "0" }, { "0", "0", "0", "0" } };
		public Matrix4X4Node() : base() { }
		public Matrix4X4Node( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { }
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			GlobalTypeWarningText = string.Format( GlobalTypeWarningText, "Matrix" );
			AddOutputPort( WirePortDataType.FLOAT4x4, Constants.EmptyPortValue );
			m_insideSize.Set( Constants.FLOAT_DRAW_WIDTH_FIELD_SIZE * 4 + Constants.FLOAT_WIDTH_SPACING * 3, Constants.FLOAT_DRAW_HEIGHT_FIELD_SIZE * 4 + Constants.FLOAT_WIDTH_SPACING * 3 + Constants.OUTSIDE_WIRE_MARGIN );
			//m_defaultValue = new Matrix4x4();
			//m_materialValue = new Matrix4x4();
			m_drawPreview = false;
			m_precisionString = UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType );
		}

		public override void CopyDefaultsToMaterial()
		{
			m_materialValue = m_defaultValue;
		}

		public override void DrawSubProperties()
		{
			EditorGUILayout.LabelField( Constants.DefaultValueLabel );
			for ( int row = 0; row < 4; row++ )
			{
				EditorGUILayout.BeginHorizontal();
				for ( int column = 0; column < 4; column++ )
				{
					m_defaultValue[ row, column ] = EditorGUILayoutFloatField( string.Empty, m_defaultValue[ row, column ], GUILayout.MaxWidth( 55 ) );
				}
				EditorGUILayout.EndHorizontal();
			}
		}

		public override void DrawMaterialProperties()
		{
			if ( m_materialMode )
				EditorGUI.BeginChangeCheck();

			EditorGUILayout.LabelField( Constants.MaterialValueLabel );
			for ( int row = 0; row < 4; row++ )
			{
				EditorGUILayout.BeginHorizontal();
				for ( int column = 0; column < 4; column++ )
				{
					m_materialValue[ row, column ] = EditorGUILayoutFloatField( string.Empty, m_materialValue[ row, column ], GUILayout.MaxWidth( 55 ) );
				}
				EditorGUILayout.EndHorizontal();
			}

			if ( m_materialMode && EditorGUI.EndChangeCheck() )
				m_requireMaterialUpdate = true;
		}

		public override void OnNodeLayout( DrawInfo drawInfo )
		{
			base.OnNodeLayout( drawInfo );

			m_propertyDrawPos.position = m_remainingBox.position;
			m_propertyDrawPos.width = drawInfo.InvertedZoom * Constants.FLOAT_DRAW_WIDTH_FIELD_SIZE;
			m_propertyDrawPos.height = drawInfo.InvertedZoom * Constants.FLOAT_DRAW_HEIGHT_FIELD_SIZE;
		}

		public override void DrawGUIControls( DrawInfo drawInfo )
		{
			base.DrawGUIControls( drawInfo );

			if ( drawInfo.CurrentEventType != EventType.MouseDown )
				return;

			Rect hitBox = m_remainingBox;
			hitBox.height = m_insideSize.y * drawInfo.InvertedZoom;
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

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );

			if ( !m_isVisible )
				return;

			if ( m_isEditingFields && m_currentParameterType != PropertyType.Global )
			{
				bool currMode = m_materialMode && m_currentParameterType != PropertyType.Constant;
				Matrix4x4 value = currMode ? m_materialValue : m_defaultValue;

				EditorGUI.BeginChangeCheck();
				for ( int row = 0; row < 4; row++ )
				{
					for ( int column = 0; column < 4; column++ )
					{
						m_propertyDrawPos.position = m_remainingBox.position + Vector2.Scale( m_propertyDrawPos.size, new Vector2( column, row ) ) + new Vector2( Constants.FLOAT_WIDTH_SPACING * drawInfo.InvertedZoom * column, Constants.FLOAT_WIDTH_SPACING * drawInfo.InvertedZoom * row );
						value[ row, column ] = EditorGUIFloatField( m_propertyDrawPos, string.Empty, value[ row, column ], UIUtils.MainSkin.textField );
					}
				}

				if ( currMode )
				{
					m_materialValue = value;
				}
				else
				{
					m_defaultValue = value;
				}

				if ( EditorGUI.EndChangeCheck() )
				{
					m_requireMaterialUpdate = m_materialMode;
					BeginDelayedDirtyProperty();
				}
			}
			else if ( drawInfo.CurrentEventType == EventType.Repaint )
			{
				bool guiEnabled = GUI.enabled;
				GUI.enabled = m_currentParameterType != PropertyType.Global;

				bool currMode = m_materialMode && m_currentParameterType != PropertyType.Constant;
				Matrix4x4 value = currMode ? m_materialValue : m_defaultValue;
				for ( int row = 0; row < 4; row++ )
				{
					for ( int column = 0; column < 4; column++ )
					{
						Rect fakeField = m_propertyDrawPos;
						fakeField.position = m_remainingBox.position + Vector2.Scale( m_propertyDrawPos.size, new Vector2( column, row ) ) + new Vector2( Constants.FLOAT_WIDTH_SPACING * drawInfo.InvertedZoom * column, Constants.FLOAT_WIDTH_SPACING * drawInfo.InvertedZoom * row );
						if( GUI.enabled )
							EditorGUIUtility.AddCursorRect( fakeField, MouseCursor.Text );

						if ( m_previousValue[ row, column ] != value[ row, column ] )
						{
							m_previousValue[ row, column ] = value[ row, column ];
							m_fieldText[ row, column ] = value[ row, column ].ToString();
						}

						GUI.Label( fakeField, m_fieldText[ row, column ], UIUtils.MainSkin.textField );
					}
				}
				GUI.enabled = guiEnabled;
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			m_precisionString = UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType );
			if ( m_currentParameterType != PropertyType.Constant )
				return PropertyData( dataCollector.PortCategory );

			Matrix4x4 value = m_defaultValue;

			return m_precisionString+"(" +	value[ 0, 0 ] + "," + value[ 0, 1 ] + "," + value[ 0, 2 ] + "," + value[ 0, 3 ] + "," +
											+value[ 1, 0 ] + "," + value[ 1, 1 ] + "," + value[ 1, 2 ] + "," + value[ 1, 3 ] + "," +
											+value[ 2, 0 ] + "," + value[ 2, 1 ] + "," + value[ 2, 2 ] + "," + value[ 2, 3 ] + "," +
											+value[ 3, 0 ] + "," + value[ 3, 1 ] + "," + value[ 3, 2 ] + "," + value[ 3, 3 ] + ")";

		}

		
		public override void UpdateMaterial( Material mat )
		{
			base.UpdateMaterial( mat );
			if ( UIUtils.IsProperty( m_currentParameterType ) && !InsideShaderFunction )
			{
				mat.SetMatrix( m_propertyName, m_materialValue );
			}
		}

		public override void SetMaterialMode( Material mat , bool fetchMaterialValues )
		{
			base.SetMaterialMode( mat , fetchMaterialValues );
			if ( fetchMaterialValues && m_materialMode && UIUtils.IsProperty( m_currentParameterType ) && mat.HasProperty( m_propertyName ) )
			{
				m_materialValue = mat.GetMatrix( m_propertyName );
			}
		}

		public override void ForceUpdateFromMaterial( Material material )
		{
			if ( UIUtils.IsProperty( m_currentParameterType ) && material.HasProperty( m_propertyName ) )
				m_materialValue = material.GetMatrix( m_propertyName );
		}
		
		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_defaultValue = IOUtils.StringToMatrix4x4( GetCurrentParam( ref nodeParams ) );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, IOUtils.Matrix4x4ToString( m_defaultValue ) );
		}

		public override void ReadAdditionalClipboardData( ref string[] nodeParams )
		{
			base.ReadAdditionalClipboardData( ref nodeParams );
			m_materialValue = IOUtils.StringToMatrix4x4( GetCurrentParam( ref nodeParams ) );
		}

		public override void WriteAdditionalClipboardData( ref string nodeInfo )
		{
			base.WriteAdditionalClipboardData( ref nodeInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, IOUtils.Matrix4x4ToString( m_materialValue ) );
		}


		public override string GetPropertyValStr()
		{
			return ( m_materialMode && m_currentParameterType != PropertyType.Constant ) ? m_materialValue[ 0, 0 ].ToString( Mathf.Abs( m_materialValue[ 0, 0 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_materialValue[ 0, 1 ].ToString( Mathf.Abs( m_materialValue[ 0, 1 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_materialValue[ 0, 2 ].ToString( Mathf.Abs( m_materialValue[ 0, 2 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_materialValue[ 0, 3 ].ToString( Mathf.Abs( m_materialValue[ 0, 3 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.MATRIX_DATA_SEPARATOR +
																							m_materialValue[ 1, 0 ].ToString( Mathf.Abs( m_materialValue[ 1, 0 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_materialValue[ 1, 1 ].ToString( Mathf.Abs( m_materialValue[ 1, 1 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_materialValue[ 1, 2 ].ToString( Mathf.Abs( m_materialValue[ 1, 2 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_materialValue[ 1, 3 ].ToString( Mathf.Abs( m_materialValue[ 1, 3 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.MATRIX_DATA_SEPARATOR +
																							m_materialValue[ 2, 0 ].ToString( Mathf.Abs( m_materialValue[ 2, 0 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_materialValue[ 2, 1 ].ToString( Mathf.Abs( m_materialValue[ 2, 1 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_materialValue[ 2, 2 ].ToString( Mathf.Abs( m_materialValue[ 2, 2 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_materialValue[ 2, 3 ].ToString( Mathf.Abs( m_materialValue[ 2, 3 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.MATRIX_DATA_SEPARATOR +
																							m_materialValue[ 3, 0 ].ToString( Mathf.Abs( m_materialValue[ 3, 0 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_materialValue[ 3, 1 ].ToString( Mathf.Abs( m_materialValue[ 3, 1 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_materialValue[ 3, 2 ].ToString( Mathf.Abs( m_materialValue[ 3, 2 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_materialValue[ 3, 3 ].ToString( Mathf.Abs( m_materialValue[ 3, 3 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) :

																							m_defaultValue[ 0, 0 ].ToString( Mathf.Abs( m_defaultValue[ 0, 0 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_defaultValue[ 0, 1 ].ToString( Mathf.Abs( m_defaultValue[ 0, 1 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_defaultValue[ 0, 2 ].ToString( Mathf.Abs( m_defaultValue[ 0, 2 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_defaultValue[ 0, 3 ].ToString( Mathf.Abs( m_defaultValue[ 0, 3 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.MATRIX_DATA_SEPARATOR +
																							m_defaultValue[ 1, 0 ].ToString( Mathf.Abs( m_defaultValue[ 1, 0 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_defaultValue[ 1, 1 ].ToString( Mathf.Abs( m_defaultValue[ 1, 1 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_defaultValue[ 1, 2 ].ToString( Mathf.Abs( m_defaultValue[ 1, 2 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_defaultValue[ 1, 3 ].ToString( Mathf.Abs( m_defaultValue[ 1, 3 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.MATRIX_DATA_SEPARATOR +
																							m_defaultValue[ 2, 0 ].ToString( Mathf.Abs( m_defaultValue[ 2, 0 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_defaultValue[ 2, 1 ].ToString( Mathf.Abs( m_defaultValue[ 2, 1 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_defaultValue[ 2, 2 ].ToString( Mathf.Abs( m_defaultValue[ 2, 2 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_defaultValue[ 2, 3 ].ToString( Mathf.Abs( m_defaultValue[ 2, 3 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.MATRIX_DATA_SEPARATOR +
																							m_defaultValue[ 3, 0 ].ToString( Mathf.Abs( m_defaultValue[ 3, 0 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_defaultValue[ 3, 1 ].ToString( Mathf.Abs( m_defaultValue[ 3, 1 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_defaultValue[ 3, 2 ].ToString( Mathf.Abs( m_defaultValue[ 3, 2 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_defaultValue[ 3, 3 ].ToString( Mathf.Abs( m_defaultValue[ 3, 3 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel );
		}

	}
}
