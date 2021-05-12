// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	public sealed class InputPort : WirePort
	{
		private const string InputDefaultNameStr = "Input";
		[SerializeField]
		private int m_externalNodeLink = -1;

		[SerializeField]
		private int m_externalPortLink = -1;

		[SerializeField]
		private string m_externalLinkId = string.Empty;

		[SerializeField]
		private bool m_typeLocked;

		[SerializeField]
		private string m_internalData = string.Empty;

		[SerializeField]
		private string m_internalDataWrapper = string.Empty;

		[SerializeField]
		private string m_dataName = string.Empty;

		[SerializeField]
		private string m_internalDataPropertyLabel = string.Empty;

		// this will only is important on master node
		[SerializeField]
		private MasterNodePortCategory m_category = MasterNodePortCategory.Fragment;

		[SerializeField]
		private PortGenType m_genType;

		private string m_propertyName = string.Empty;
		private int m_cachedPropertyId = -1;

		private int m_cachedIntShaderID = -1;
		private int m_cachedFloatShaderID = -1;
		private int m_cachedVectorShaderID = -1;
		private int m_cachedColorShaderID = -1;
		private int m_cached2DShaderID = -1;
		private int m_cachedDefaultTexShaderID = -1;

		[SerializeField]
		private bool m_drawInternalData = false;

		//[SerializeField]
		//private RenderTexture m_inputPreview = null;
		//[SerializeField]
		private RenderTexture m_inputPreviewTexture = null;
		private Material m_inputPreviewMaterial = null;
		private Shader m_inputPreviewShader = null;

		[SerializeField]
		private int m_previewInternalInt = 0;
		[SerializeField]
		private float m_previewInternalFloat = 0;
		[SerializeField]
		private Vector2 m_previewInternalVec2 = Vector2.zero;
		[SerializeField]
		private Vector3 m_previewInternalVec3 = Vector3.zero;
		[SerializeField]
		private Vector4 m_previewInternalVec4 = Vector4.zero;
		[SerializeField]
		private Color m_previewInternalColor = Color.clear;
		[SerializeField]
		private Matrix4x4 m_previewInternalMatrix4x4 = Matrix4x4.identity;

		private int m_propertyNameInt = 0;
		private ParentNode m_node = null;

		public InputPort() : base( -1, -1, WirePortDataType.FLOAT, string.Empty ) { m_typeLocked = true; }
		public InputPort( int nodeId, int portId, WirePortDataType dataType, string name, bool typeLocked, int orderId = -1, MasterNodePortCategory category = MasterNodePortCategory.Fragment, PortGenType genType = PortGenType.NonCustomLighting ) : base( nodeId, portId, dataType, name, orderId )
		{
			m_dataName = name;
			m_internalDataPropertyLabel = ( string.IsNullOrEmpty( name ) || name.Equals( Constants.EmptyPortValue ) ) ? InputDefaultNameStr : name;
			m_typeLocked = typeLocked;
			m_category = category;
			m_genType = genType;
		}

		public InputPort( int nodeId, int portId, WirePortDataType dataType, string name, string dataName, bool typeLocked, int orderId = -1, MasterNodePortCategory category = MasterNodePortCategory.Fragment, PortGenType genType = PortGenType.NonCustomLighting ) : base( nodeId, portId, dataType, name, orderId )
		{
			m_dataName = dataName;
			m_internalDataPropertyLabel = ( string.IsNullOrEmpty( name ) || name.Equals( Constants.EmptyPortValue ) ) ? InputDefaultNameStr : name;
			m_typeLocked = typeLocked;
			m_category = category;
			m_genType = genType;
		}

		public void SetExternalLink( int nodeId, int portId )
		{
			m_externalNodeLink = nodeId;
			m_externalPortLink = portId;
		}

		public override bool CheckValidType( WirePortDataType dataType )
		{
			if( m_typeLocked )
				return ( dataType == m_dataType );

			return base.CheckValidType( dataType );
		}

		public override void FullDeleteConnections()
		{
			UIUtils.DeleteConnection( true, m_nodeId, m_portId, true, true );
		}

		public override void NotifyExternalRefencesOnChange()
		{
			for( int i = 0; i < m_externalReferences.Count; i++ )
			{
				ParentNode node = UIUtils.GetNode( m_externalReferences[ i ].NodeId );
				if( node )
				{
					OutputPort port = node.GetOutputPortByUniqueId( m_externalReferences[ i ].PortId );
					port.UpdateInfoOnExternalConn( m_nodeId, m_portId, m_dataType );
					node.OnConnectedInputNodeChanges( m_externalReferences[ i ].PortId, m_nodeId, m_portId, m_name, m_dataType );
				}
			}
		}

		public void UpdatePreviewInternalData()
		{
			switch( m_dataType )
			{
				case WirePortDataType.INT: m_previewInternalInt = IntInternalData; break;
				case WirePortDataType.FLOAT: m_previewInternalFloat = FloatInternalData; break;
				case WirePortDataType.FLOAT2: m_previewInternalVec2 = Vector2InternalData; break;
				case WirePortDataType.FLOAT3: m_previewInternalVec3 = Vector3InternalData; break;
				case WirePortDataType.FLOAT4: m_previewInternalVec4 = Vector4InternalData; break;
				case WirePortDataType.COLOR: m_previewInternalColor = ColorInternalData; break;
			}
		}

		void UpdateVariablesFromInternalData()
		{
			string[] data = String.IsNullOrEmpty( m_internalData ) ? null : m_internalData.Split( IOUtils.VECTOR_SEPARATOR );
			bool reset = ( data == null || data.Length == 0 );
			m_internalDataUpdated = false;
			try
			{
				switch( m_dataType )
				{
					case WirePortDataType.OBJECT:
					case WirePortDataType.FLOAT: m_previewInternalFloat = reset ? 0 : Convert.ToSingle( data[ 0 ] ); break;
					case WirePortDataType.INT:
					{
						if( reset )
						{
							m_previewInternalInt = 0;
						}
						else
						{
							if( data[ 0 ].Contains( "." ) )
							{
								m_previewInternalInt = (int)Convert.ToSingle( data[ 0 ] );
							}
							else
							{
								m_previewInternalInt = Convert.ToInt32( data[ 0 ] );
							}
						}
					}
					break;
					case WirePortDataType.FLOAT2:
					{
						if( reset )
						{
							m_previewInternalVec2 = Vector2.zero;
						}
						else
						{
							if( data.Length < 2 )
							{
								m_previewInternalVec2.x = Convert.ToSingle( data[ 0 ] );
								m_previewInternalVec2.y = 0;
							}
							else
							{
								m_previewInternalVec2.x = Convert.ToSingle( data[ 0 ] );
								m_previewInternalVec2.y = Convert.ToSingle( data[ 1 ] );
							}
						}
					}
					break;
					case WirePortDataType.FLOAT3:
					{
						if( reset )
						{
							m_previewInternalVec3 = Vector3.zero;
						}
						else
						{
							int count = Mathf.Min( data.Length, 3 );
							for( int i = 0; i < count; i++ )
							{
								m_previewInternalVec3[ i ] = Convert.ToSingle( data[ i ] );
							}
							if( count < 3 )
							{
								for( int i = count; i < 3; i++ )
								{
									m_previewInternalVec3[ i ] = 0;
								}
							}
						}

					}
					break;
					case WirePortDataType.FLOAT4:
					{
						if( reset )
						{
							m_previewInternalVec4 = Vector4.zero;
						}
						else
						{
							int count = Mathf.Min( data.Length, 4 );
							for( int i = 0; i < count; i++ )
							{
								m_previewInternalVec4[ i ] = Convert.ToSingle( data[ i ] );
							}
							if( count < 4 )
							{
								for( int i = count; i < 4; i++ )
								{
									m_previewInternalVec4[ i ] = 0;
								}
							}
						}
					}
					break;
					case WirePortDataType.COLOR:
					{
						if( reset )
						{
							m_previewInternalColor = Color.black;
						}
						else
						{
							int count = Mathf.Min( data.Length, 4 );
							for( int i = 0; i < count; i++ )
							{
								m_previewInternalColor[ i ] = Convert.ToSingle( data[ i ] );
							}
							if( count < 4 )
							{
								for( int i = count; i < 4; i++ )
								{
									m_previewInternalColor[ i ] = 0;
								}
							}
						}
					}
					break;
					case WirePortDataType.FLOAT3x3:
					case WirePortDataType.FLOAT4x4:
					{
						if( reset )
						{
							m_previewInternalMatrix4x4 = Matrix4x4.identity;
						}
						else
						{
							int count = Mathf.Min( data.Length, 16 );
							int overallIdx = 0;
							for( int i = 0; i < 4; i++ )
							{
								for( int j = 0; j < 4; j++ )
								{
									if( overallIdx < count )
									{
										m_previewInternalMatrix4x4[ i, j ] = Convert.ToSingle( data[ overallIdx ] );
									}
									else
									{
										m_previewInternalMatrix4x4[ i, j ] = ( ( i == j ) ? 1 : 0 );
									}
									overallIdx++;
								}
							}
						}
					}
					break;
				}
			}
			catch( Exception e )
			{
				if( DebugConsoleWindow.DeveloperMode )
					Debug.LogException( e );
			}
		}

		void UpdateInternalDataFromVariables( bool forceDecimal = false )
		{
			switch( m_dataType )
			{
				case WirePortDataType.OBJECT:
				case WirePortDataType.FLOAT:
				{
					if( forceDecimal && m_previewInternalFloat == (int)m_previewInternalFloat )
						m_internalData = m_previewInternalFloat.ToString("0.0##############"); // to make sure integer values like 0 or 1 are generated as 0.0 and 1.0
					else
						m_internalData = m_previewInternalFloat.ToString();
					m_internalDataWrapper = string.Empty;
				}
				break;
				case WirePortDataType.INT:
				{
					m_internalData = m_previewInternalInt.ToString();
					m_internalDataWrapper = string.Empty;
				}
				break;
				case WirePortDataType.FLOAT2:
				{
					m_internalData = m_previewInternalVec2.x.ToString() + IOUtils.VECTOR_SEPARATOR +
									 m_previewInternalVec2.y.ToString();
					m_internalDataWrapper = "float2( {0} )";
				}
				break;
				case WirePortDataType.FLOAT3:
				{
					m_internalData = m_previewInternalVec3.x.ToString() + IOUtils.VECTOR_SEPARATOR +
									 m_previewInternalVec3.y.ToString() + IOUtils.VECTOR_SEPARATOR +
									 m_previewInternalVec3.z.ToString();
					m_internalDataWrapper = "float3( {0} )";
				}
				break;
				case WirePortDataType.FLOAT4:
				{
					m_internalData = m_previewInternalVec4.x.ToString() + IOUtils.VECTOR_SEPARATOR +
									 m_previewInternalVec4.y.ToString() + IOUtils.VECTOR_SEPARATOR +
									 m_previewInternalVec4.z.ToString() + IOUtils.VECTOR_SEPARATOR +
									 m_previewInternalVec4.w.ToString();

					m_internalDataWrapper = "float4( {0} )";
				}
				break;
				case WirePortDataType.COLOR:
				{
					m_internalData = m_previewInternalColor.r.ToString() + IOUtils.VECTOR_SEPARATOR +
									 m_previewInternalColor.g.ToString() + IOUtils.VECTOR_SEPARATOR +
									 m_previewInternalColor.b.ToString() + IOUtils.VECTOR_SEPARATOR +
									 m_previewInternalColor.a.ToString();

					m_internalDataWrapper = "float4( {0} )";
				}
				break;
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{
					m_internalData = m_previewInternalMatrix4x4[ 0, 0 ].ToString() + IOUtils.VECTOR_SEPARATOR + m_previewInternalMatrix4x4[ 0, 1 ].ToString() + IOUtils.VECTOR_SEPARATOR + m_previewInternalMatrix4x4[ 0, 2 ].ToString() + IOUtils.VECTOR_SEPARATOR + m_previewInternalMatrix4x4[ 0, 3 ].ToString() + IOUtils.VECTOR_SEPARATOR +
									 m_previewInternalMatrix4x4[ 1, 0 ].ToString() + IOUtils.VECTOR_SEPARATOR + m_previewInternalMatrix4x4[ 1, 1 ].ToString() + IOUtils.VECTOR_SEPARATOR + m_previewInternalMatrix4x4[ 1, 2 ].ToString() + IOUtils.VECTOR_SEPARATOR + m_previewInternalMatrix4x4[ 1, 3 ].ToString() + IOUtils.VECTOR_SEPARATOR +
									 m_previewInternalMatrix4x4[ 2, 0 ].ToString() + IOUtils.VECTOR_SEPARATOR + m_previewInternalMatrix4x4[ 2, 1 ].ToString() + IOUtils.VECTOR_SEPARATOR + m_previewInternalMatrix4x4[ 2, 2 ].ToString() + IOUtils.VECTOR_SEPARATOR + m_previewInternalMatrix4x4[ 2, 3 ].ToString() + IOUtils.VECTOR_SEPARATOR +
									 m_previewInternalMatrix4x4[ 3, 0 ].ToString() + IOUtils.VECTOR_SEPARATOR + m_previewInternalMatrix4x4[ 3, 1 ].ToString() + IOUtils.VECTOR_SEPARATOR + m_previewInternalMatrix4x4[ 3, 2 ].ToString() + IOUtils.VECTOR_SEPARATOR + m_previewInternalMatrix4x4[ 3, 3 ].ToString();

					if( m_dataType == WirePortDataType.FLOAT3x3 )
						m_internalDataWrapper = "float3x3( {0} )";
					else
						m_internalDataWrapper = "float4x4( {0} )";
				}
				break;
			}
		}

		//This gets the 3x3 matrix inside of the 4x4
		private string Matrix3x3WrappedData()
		{
			string tempInternal = string.Empty;

			string[] data = String.IsNullOrEmpty( m_internalData ) ? null : m_internalData.Split( IOUtils.VECTOR_SEPARATOR );
			if( data.Length == 16 )
			{
				int o = 0;
				for( int i = 0; i < 8; i++ )
				{
					if( i == 3 || i == 6 )
						o++;
					tempInternal += data[ i + o ] + IOUtils.VECTOR_SEPARATOR;
				}

				tempInternal += data[ 10 ];

				return String.Format( m_internalDataWrapper, tempInternal );
			}
			else
			{
				return String.Format( m_internalDataWrapper, m_internalData );
			}
		}

		private string SamplerWrappedData( ref MasterNodeDataCollector dataCollector )
		{
			m_internalData = "_Sampler" + PortId + UIUtils.GetNode( m_nodeId ).OutputId;
			dataCollector.AddToUniforms( m_nodeId, "uniform sampler2D " + m_internalData + ";" );

			return m_internalData;
		}

		//TODO: Replace GenerateShaderForOutput(...) calls to this one
		// This is a new similar method to GenerateShaderForOutput(...) which always autocasts
		public string GeneratePortInstructions( ref MasterNodeDataCollector dataCollector )
		{
			InputPort linkPort = ExternalLink;
			if( linkPort != null )
			{
				return linkPort.GeneratePortInstructions( ref dataCollector );
			}

			string result = string.Empty;
			if( m_externalReferences.Count > 0 && !m_locked )
			{
				result = UIUtils.GetNode( m_externalReferences[ 0 ].NodeId ).GenerateShaderForOutput( m_externalReferences[ 0 ].PortId, ref dataCollector, false );
				if( m_externalReferences[ 0 ].DataType != m_dataType )
				{
					result = UIUtils.CastPortType( ref dataCollector, UIUtils.GetNode( m_nodeId ).CurrentPrecisionType, new NodeCastInfo( m_externalReferences[ 0 ].NodeId, m_externalReferences[ 0 ].PortId ), null, m_externalReferences[ 0 ].DataType, m_dataType, result );
				}
			}
			else
			{
				UpdateInternalDataFromVariables( true );
				if( DataType == WirePortDataType.FLOAT3x3 )
					result = Matrix3x3WrappedData();
				else if( DataType == WirePortDataType.SAMPLER2D )
					result = SamplerWrappedData( ref dataCollector );
				else
					result = !String.IsNullOrEmpty( m_internalDataWrapper ) ? String.Format( m_internalDataWrapper, m_internalData ) : m_internalData;
			}
			return result;
		}

		public string GenerateShaderForOutput( ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			InputPort linkPort = ExternalLink;
			if( linkPort != null )
			{
				return linkPort.GenerateShaderForOutput( ref dataCollector, ignoreLocalVar );
			}

			string result = string.Empty;
			if( m_externalReferences.Count > 0 && !m_locked )
			{
				result = UIUtils.GetNode( m_externalReferences[ 0 ].NodeId ).GenerateShaderForOutput( m_externalReferences[ 0 ].PortId, ref dataCollector, ignoreLocalVar );
			}
			else
			{
				UpdateInternalDataFromVariables( true );
				if( !String.IsNullOrEmpty( m_internalDataWrapper ) )
				{
					if( DataType == WirePortDataType.FLOAT3x3 )
						result = Matrix3x3WrappedData();
					else
						result = String.Format( m_internalDataWrapper, m_internalData );
				}
				else
				{
					result = m_internalData;
				}
			}
			return result;
		}

		public string GenerateShaderForOutput( ref MasterNodeDataCollector dataCollector, WirePortDataType inputPortType, bool ignoreLocalVar, bool autoCast = false )
		{
			InputPort linkPort = ExternalLink;
			if( linkPort != null )
			{
				return linkPort.GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalVar, autoCast );
			}

			string result = string.Empty;
			if( m_externalReferences.Count > 0 && !m_locked )
			{
				result = UIUtils.GetNode( m_externalReferences[ 0 ].NodeId ).GenerateShaderForOutput( m_externalReferences[ 0 ].PortId, ref dataCollector, ignoreLocalVar );
				if( autoCast && m_externalReferences[ 0 ].DataType != inputPortType )
				{
					result = UIUtils.CastPortType( ref dataCollector, UIUtils.GetNode( m_nodeId ).CurrentPrecisionType, new NodeCastInfo( m_externalReferences[ 0 ].NodeId, m_externalReferences[ 0 ].PortId ), null, m_externalReferences[ 0 ].DataType, inputPortType, result );
				}
			}
			else
			{
				UpdateInternalDataFromVariables( true );
				if( !String.IsNullOrEmpty( m_internalDataWrapper ) )
				{
					if( DataType == WirePortDataType.FLOAT3x3 )
						result = Matrix3x3WrappedData();
					else
						result = String.Format( m_internalDataWrapper, m_internalData );
				}
				else
				{
					result = m_internalData;
				}
			}

			return result;
		}

		public OutputPort GetOutputConnection( int connID = 0 )
		{
			if( connID < m_externalReferences.Count )
			{
				return UIUtils.GetNode( m_externalReferences[ connID ].NodeId ).GetOutputPortByUniqueId( m_externalReferences[ connID ].PortId );
			}
			return null;
		}

		public ParentNode GetOutputNode( int connID = 0 )
		{
			if( connID < m_externalReferences.Count )
			{
				return UIUtils.GetNode( m_externalReferences[ connID ].NodeId );
			}
			return null;
		}

		public bool TypeLocked
		{
			get { return m_typeLocked; }
		}

		public void WriteToString( ref string myString )
		{
			if( m_externalReferences.Count != 1 || m_isDummy )
			{
				return;
			}

			IOUtils.AddTypeToString( ref myString, IOUtils.WireConnectionParam );
			IOUtils.AddFieldValueToString( ref myString, m_nodeId );
			IOUtils.AddFieldValueToString( ref myString, m_portId );
			IOUtils.AddFieldValueToString( ref myString, m_externalReferences[ 0 ].NodeId );
			IOUtils.AddFieldValueToString( ref myString, m_externalReferences[ 0 ].PortId );
			IOUtils.AddLineTerminator( ref myString );
		}

		public void ShowInternalData( Rect rect, UndoParentNode owner, bool useCustomLabel = false, string customLabel = null )
		{
			string label = ( useCustomLabel == true && customLabel != null ) ? customLabel : m_internalDataPropertyLabel;
			switch( m_dataType )
			{
				case WirePortDataType.OBJECT:
				case WirePortDataType.FLOAT:
				{
					FloatInternalData = owner.EditorGUIFloatField( rect, label, FloatInternalData );
				}
				break;
				case WirePortDataType.FLOAT2:
				{
					Vector2InternalData = owner.EditorGUIVector2Field( rect, label, Vector2InternalData );
				}
				break;
				case WirePortDataType.FLOAT3:
				{
					Vector3InternalData = owner.EditorGUIVector3Field( rect, label, Vector3InternalData );
				}
				break;
				case WirePortDataType.FLOAT4:
				{
					Vector4InternalData = owner.EditorGUIVector4Field( rect, label, Vector4InternalData );
				}
				break;
				case WirePortDataType.FLOAT3x3:
				{
					Matrix4x4 matrix = Matrix4x4InternalData;
					Vector3 currVec3 = Vector3.zero;
					for( int i = 0; i < 3; i++ )
					{
						Vector4 currVec = matrix.GetRow( i );
						currVec3.Set( currVec.x, currVec.y, currVec.z );
						EditorGUI.BeginChangeCheck();
						currVec3 = owner.EditorGUIVector3Field( rect, label + "[ " + i + " ]", currVec3 );
						rect.y += 2*EditorGUIUtility.singleLineHeight;
						if( EditorGUI.EndChangeCheck() )
						{
							currVec.Set( currVec3.x, currVec3.y, currVec3.z, currVec.w );
							matrix.SetRow( i, currVec );
						}
					}
					Matrix4x4InternalData = matrix;
				}
				break;
				case WirePortDataType.FLOAT4x4:
				{
					Matrix4x4 matrix = Matrix4x4InternalData;
					for( int i = 0; i < 4; i++ )
					{
						Vector4 currVec = matrix.GetRow( i );
						EditorGUI.BeginChangeCheck();
						currVec = owner.EditorGUIVector4Field( rect, label + "[ " + i + " ]", currVec );
						rect.y += 2*EditorGUIUtility.singleLineHeight;
						if( EditorGUI.EndChangeCheck() )
						{
							matrix.SetRow( i, currVec );
						}
					}
					Matrix4x4InternalData = matrix;
				}
				break;
				case WirePortDataType.COLOR:
				{
					ColorInternalData = owner.EditorGUIColorField( rect, label, ColorInternalData );
				}
				break;
				case WirePortDataType.INT:
				{
					IntInternalData = owner.EditorGUIIntField( rect, label, IntInternalData );
				}
				break;
			}
		}

		public void ShowInternalData( UndoParentNode owner, bool useCustomLabel = false, string customLabel = null )
		{
			string label = ( useCustomLabel == true && customLabel != null ) ? customLabel : m_internalDataPropertyLabel;
			switch( m_dataType )
			{
				case WirePortDataType.OBJECT:
				case WirePortDataType.FLOAT:
				{
					FloatInternalData = owner.EditorGUILayoutFloatField( label, FloatInternalData );
				}
				break;
				case WirePortDataType.FLOAT2:
				{
					Vector2InternalData = owner.EditorGUILayoutVector2Field( label, Vector2InternalData );
				}
				break;
				case WirePortDataType.FLOAT3:
				{
					Vector3InternalData = owner.EditorGUILayoutVector3Field( label, Vector3InternalData );
				}
				break;
				case WirePortDataType.FLOAT4:
				{
					Vector4InternalData = owner.EditorGUILayoutVector4Field( label, Vector4InternalData );
				}
				break;
				case WirePortDataType.FLOAT3x3:
				{
					Matrix4x4 matrix = Matrix4x4InternalData;
					Vector3 currVec3 = Vector3.zero;
					for( int i = 0; i < 3; i++ )
					{
						Vector4 currVec = matrix.GetRow( i );
						currVec3.Set( currVec.x, currVec.y, currVec.z );
						EditorGUI.BeginChangeCheck();
						currVec3 = owner.EditorGUILayoutVector3Field( label + "[ " + i + " ]", currVec3 );
						if( EditorGUI.EndChangeCheck() )
						{
							currVec.Set( currVec3.x, currVec3.y, currVec3.z, currVec.w );
							matrix.SetRow( i, currVec );
						}
					}
					Matrix4x4InternalData = matrix;
				}
				break;
				case WirePortDataType.FLOAT4x4:
				{
					Matrix4x4 matrix = Matrix4x4InternalData;
					for( int i = 0; i < 4; i++ )
					{
						Vector4 currVec = matrix.GetRow( i );
						EditorGUI.BeginChangeCheck();
						currVec = owner.EditorGUILayoutVector4Field( label + "[ " + i + " ]", currVec );
						if( EditorGUI.EndChangeCheck() )
						{
							matrix.SetRow( i, currVec );
						}
					}
					Matrix4x4InternalData = matrix;
				}
				break;
				case WirePortDataType.COLOR:
				{
					ColorInternalData = owner.EditorGUILayoutColorField( label, ColorInternalData );
				}
				break;
				case WirePortDataType.INT:
				{
					IntInternalData = owner.EditorGUILayoutIntField( label, IntInternalData );
				}
				break;
			}
		}

		public float FloatInternalData
		{
			set { m_previewInternalFloat = value; m_internalDataUpdated = false; }
			get { return m_previewInternalFloat; }
		}

		public int IntInternalData
		{
			set { m_previewInternalInt = value; m_internalDataUpdated = false; }
			get { return m_previewInternalInt; }
		}

		public Vector2 Vector2InternalData
		{
			set { m_previewInternalVec2 = value; m_internalDataUpdated = false; }
			get { return m_previewInternalVec2; }
		}

		public Vector3 Vector3InternalData
		{
			set { m_previewInternalVec3 = value; m_internalDataUpdated = false; }
			get { return m_previewInternalVec3; }
		}

		public Vector4 Vector4InternalData
		{
			set { m_previewInternalVec4 = value; m_internalDataUpdated = false; }
			get { return m_previewInternalVec4; }
		}

		public Color ColorInternalData
		{
			set { m_previewInternalColor = value; m_internalDataUpdated = false; }
			get { return m_previewInternalColor; }
		}

		public Matrix4x4 Matrix4x4InternalData
		{
			set { m_previewInternalMatrix4x4 = value; m_internalDataUpdated = false; }
			get { return m_previewInternalMatrix4x4; }
		}

		public string SamplerInternalData
		{
			set { InternalData = UIUtils.RemoveInvalidCharacters( value ); m_internalDataUpdated = false; }
			get { return m_internalData; }
		}

		public override void ForceClearConnection()
		{
			UIUtils.DeleteConnection( true, m_nodeId, m_portId, false, true );
		}

		private bool m_internalDataUpdated = false;
		private string m_displayInternalData = string.Empty;
		public string DisplayInternalData
		{
			get
			{
				if( !m_internalDataUpdated )
				{
					UpdateInternalDataFromVariables();
					m_internalDataUpdated = true;
					m_displayInternalData = "( "+ m_internalData + " )";
				}
				return m_displayInternalData;
			}
		}

		public string InternalData
		{
			get
			{
				UpdateInternalDataFromVariables();
				return m_internalData;
			}
			set
			{
				m_internalData = value;
				UpdateVariablesFromInternalData();
			}
		}

		public string WrappedInternalData
		{
			get
			{
				UpdateInternalDataFromVariables();
				return string.IsNullOrEmpty( m_internalDataWrapper ) ? m_internalData : String.Format( m_internalDataWrapper, m_internalData );
			}
		}

		public override WirePortDataType DataType
		{
			get { return base.DataType; }
			// must be set to update internal data. do not delete
			set
			{
				m_internalDataUpdated = false;
				switch( DataType )
				{
					case WirePortDataType.FLOAT:
					{
						switch( value )
						{
							case WirePortDataType.FLOAT2: m_previewInternalVec2.x = m_previewInternalFloat; break;
							case WirePortDataType.FLOAT3: m_previewInternalVec3.x = m_previewInternalFloat; break;
							case WirePortDataType.FLOAT4: m_previewInternalVec4.x = m_previewInternalFloat; break;
							case WirePortDataType.FLOAT3x3:
							case WirePortDataType.FLOAT4x4: m_previewInternalMatrix4x4[ 0 ] = m_previewInternalFloat; break;
							case WirePortDataType.COLOR: m_previewInternalColor.r = m_previewInternalFloat; break;
							case WirePortDataType.INT: m_previewInternalInt = (int)m_previewInternalFloat; break;
						}
					}
					break;
					case WirePortDataType.FLOAT2:
					{
						switch( value )
						{
							case WirePortDataType.FLOAT: m_previewInternalFloat = m_previewInternalVec2.x; break;
							case WirePortDataType.FLOAT3:
							{
								m_previewInternalVec3.x = m_previewInternalVec2.x;
								m_previewInternalVec3.y = m_previewInternalVec2.y;
							}
							break;
							case WirePortDataType.FLOAT4:
							{
								m_previewInternalVec4.x = m_previewInternalVec2.x;
								m_previewInternalVec4.y = m_previewInternalVec2.y;
							}
							break;
							case WirePortDataType.FLOAT3x3:
							case WirePortDataType.FLOAT4x4:
							{
								m_previewInternalMatrix4x4[ 0 ] = m_previewInternalVec2.x;
								m_previewInternalMatrix4x4[ 1 ] = m_previewInternalVec2.y;
							}
							break;
							case WirePortDataType.COLOR:
							{
								m_previewInternalColor.r = m_previewInternalVec2.x;
								m_previewInternalColor.g = m_previewInternalVec2.y;
							}
							break;
							case WirePortDataType.INT: m_previewInternalInt = (int)m_previewInternalVec2.x; break;
						}
					}
					break;
					case WirePortDataType.FLOAT3:
					{
						switch( value )
						{
							case WirePortDataType.FLOAT: m_previewInternalFloat = m_previewInternalVec3.x; break;
							case WirePortDataType.FLOAT2:
							{
								m_previewInternalVec2.x = m_previewInternalVec3.x;
								m_previewInternalVec2.y = m_previewInternalVec3.y;
							}
							break;
							case WirePortDataType.FLOAT4:
							{
								m_previewInternalVec4.x = m_previewInternalVec3.x;
								m_previewInternalVec4.y = m_previewInternalVec3.y;
								m_previewInternalVec4.z = m_previewInternalVec3.z;
							}
							break;
							case WirePortDataType.FLOAT3x3:
							case WirePortDataType.FLOAT4x4:
							{
								m_previewInternalMatrix4x4[ 0 ] = m_previewInternalVec3.x;
								m_previewInternalMatrix4x4[ 1 ] = m_previewInternalVec3.y;
								m_previewInternalMatrix4x4[ 2 ] = m_previewInternalVec3.z;
							}
							break;
							case WirePortDataType.COLOR:
							{
								m_previewInternalColor.r = m_previewInternalVec3.x;
								m_previewInternalColor.g = m_previewInternalVec3.y;
								m_previewInternalColor.b = m_previewInternalVec3.z;
							}
							break;
							case WirePortDataType.INT: m_previewInternalInt = (int)m_previewInternalVec3.x; break;
						}
					}
					break;
					case WirePortDataType.FLOAT4:
					{
						switch( value )
						{
							case WirePortDataType.FLOAT: m_previewInternalFloat = m_previewInternalVec4.x; break;
							case WirePortDataType.FLOAT2:
							{
								m_previewInternalVec2.x = m_previewInternalVec4.x;
								m_previewInternalVec2.y = m_previewInternalVec4.y;
							}
							break;
							case WirePortDataType.FLOAT3:
							{
								m_previewInternalVec3.x = m_previewInternalVec4.x;
								m_previewInternalVec3.y = m_previewInternalVec4.y;
								m_previewInternalVec3.z = m_previewInternalVec4.z;
							}
							break;
							case WirePortDataType.FLOAT3x3:
							case WirePortDataType.FLOAT4x4:
							{
								m_previewInternalMatrix4x4[ 0 ] = m_previewInternalVec4.x;
								m_previewInternalMatrix4x4[ 1 ] = m_previewInternalVec4.y;
								m_previewInternalMatrix4x4[ 2 ] = m_previewInternalVec4.z;
								m_previewInternalMatrix4x4[ 3 ] = m_previewInternalVec4.w;
							}
							break;
							case WirePortDataType.COLOR:
							{
								m_previewInternalColor.r = m_previewInternalVec4.x;
								m_previewInternalColor.g = m_previewInternalVec4.y;
								m_previewInternalColor.b = m_previewInternalVec4.z;
								m_previewInternalColor.a = m_previewInternalVec4.w;
							}
							break;
							case WirePortDataType.INT: m_previewInternalInt = (int)m_previewInternalVec4.x; break;
						}
					}
					break;
					case WirePortDataType.FLOAT3x3:
					case WirePortDataType.FLOAT4x4:
					{
						switch( value )
						{
							case WirePortDataType.FLOAT: m_previewInternalFloat = m_previewInternalMatrix4x4[ 0 ]; break;
							case WirePortDataType.FLOAT2:
							{
								m_previewInternalVec2.x = m_previewInternalMatrix4x4[ 0 ];
								m_previewInternalVec2.y = m_previewInternalMatrix4x4[ 1 ];
							}
							break;
							case WirePortDataType.FLOAT3:
							{
								m_previewInternalVec3.x = m_previewInternalMatrix4x4[ 0 ];
								m_previewInternalVec3.y = m_previewInternalMatrix4x4[ 1 ];
								m_previewInternalVec3.z = m_previewInternalMatrix4x4[ 2 ];
							}
							break;
							case WirePortDataType.FLOAT4:
							{
								m_previewInternalVec4.x = m_previewInternalMatrix4x4[ 0 ];
								m_previewInternalVec4.y = m_previewInternalMatrix4x4[ 1 ];
								m_previewInternalVec4.z = m_previewInternalMatrix4x4[ 2 ];
								m_previewInternalVec4.w = m_previewInternalMatrix4x4[ 3 ];
							}
							break;
							case WirePortDataType.COLOR:
							{
								m_previewInternalColor.r = m_previewInternalMatrix4x4[ 0 ];
								m_previewInternalColor.g = m_previewInternalMatrix4x4[ 1 ];
								m_previewInternalColor.b = m_previewInternalMatrix4x4[ 2 ];
								m_previewInternalColor.a = m_previewInternalMatrix4x4[ 3 ];
							}
							break;
							case WirePortDataType.INT: m_previewInternalInt = (int)m_previewInternalMatrix4x4[ 0 ]; break;
						}
					}
					break;
					case WirePortDataType.COLOR:
					{
						switch( value )
						{
							case WirePortDataType.FLOAT: m_previewInternalFloat = m_previewInternalColor.r; break;
							case WirePortDataType.FLOAT2:
							{
								m_previewInternalVec2.x = m_previewInternalColor.r;
								m_previewInternalVec2.y = m_previewInternalColor.g;
							}
							break;
							case WirePortDataType.FLOAT3:
							{
								m_previewInternalVec3.x = m_previewInternalColor.r;
								m_previewInternalVec3.y = m_previewInternalColor.g;
								m_previewInternalVec3.z = m_previewInternalColor.b;
							}
							break;
							case WirePortDataType.FLOAT4:
							{
								m_previewInternalVec4.x = m_previewInternalColor.r;
								m_previewInternalVec4.y = m_previewInternalColor.g;
								m_previewInternalVec4.z = m_previewInternalColor.b;
								m_previewInternalVec4.w = m_previewInternalColor.a;
							}
							break;
							case WirePortDataType.FLOAT3x3:
							case WirePortDataType.FLOAT4x4:
							{
								m_previewInternalMatrix4x4[ 0 ] = m_previewInternalColor.r;
								m_previewInternalMatrix4x4[ 1 ] = m_previewInternalColor.g;
								m_previewInternalMatrix4x4[ 2 ] = m_previewInternalColor.b;
								m_previewInternalMatrix4x4[ 3 ] = m_previewInternalColor.a;
							}
							break;
							case WirePortDataType.INT: m_previewInternalInt = (int)m_previewInternalColor.r; break;
						}
					}
					break;
					case WirePortDataType.INT:
					{
						switch( value )
						{
							case WirePortDataType.FLOAT: m_previewInternalFloat = m_previewInternalInt; break;
							case WirePortDataType.FLOAT2: m_previewInternalVec2.x = m_previewInternalInt; break;
							case WirePortDataType.FLOAT3: m_previewInternalVec3.x = m_previewInternalInt; break;
							case WirePortDataType.FLOAT4: m_previewInternalVec4.x = m_previewInternalInt; break;
							case WirePortDataType.FLOAT3x3:
							case WirePortDataType.FLOAT4x4: m_previewInternalMatrix4x4[ 0 ] = m_previewInternalInt; break;
							case WirePortDataType.COLOR: m_previewInternalColor.r = m_previewInternalInt; break;
						}
					}
					break;
				}
				base.DataType = value;
			}
		}

		public string DataName
		{
			get { return m_dataName; }
			set { m_dataName = value; }
		}

		public bool IsFragment { get { return m_category == MasterNodePortCategory.Fragment || m_category == MasterNodePortCategory.Debug; } }
		public MasterNodePortCategory Category
		{
			set { m_category = value; }
			get { return m_category; }
		}

		private int CachedIntPropertyID
		{
			get
			{
				if( m_cachedIntShaderID == -1 )
					m_cachedIntShaderID = Shader.PropertyToID( "_InputInt" );
				return m_cachedIntShaderID;
			}
		}

		private int CachedFloatPropertyID
		{
			get
			{
				if( m_cachedFloatShaderID == -1 )
					m_cachedFloatShaderID = Shader.PropertyToID( "_InputFloat" );
				return m_cachedFloatShaderID;
			}
		}

		private int CachedVectorPropertyID
		{
			get
			{
				if( m_cachedVectorShaderID == -1 )
					m_cachedVectorShaderID = Shader.PropertyToID( "_InputVector" );
				return m_cachedVectorShaderID;
			}
		}

		private int CachedColorPropertyID
		{
			get
			{
				if( m_cachedColorShaderID == -1 )
					m_cachedColorShaderID = Shader.PropertyToID( "_InputColor" );
				return m_cachedColorShaderID;
			}
		}

		private int CachedDefaultTexPropertyID
		{
			get
			{
				if( m_cachedDefaultTexShaderID == -1 )
					m_cachedDefaultTexShaderID = Shader.PropertyToID( "_Default" );
				return m_cachedDefaultTexShaderID;
			}
		}

		private int Cached2DPropertyID
		{
			get
			{
				if( m_cached2DShaderID == -1 )
					m_cached2DShaderID = Shader.PropertyToID( "_Input2D" );
				return m_cached2DShaderID;
			}
		}

		public int CachedPropertyId
		{
			get { return m_cachedPropertyId; }
		}

		public bool InputNodeHasPreview()
		{
			ParentNode node = GetOutputNode( 0 );

			if( node != null )
				return node.HasPreviewShader;

			return false;
		}

		public void PreparePortCacheID()
		{
			if( m_propertyNameInt != PortId || string.IsNullOrEmpty( m_propertyName ) )
			{
				m_propertyNameInt = PortId;
				m_propertyName = "_" + Convert.ToChar( PortId + 65 );
				m_cachedPropertyId = Shader.PropertyToID( m_propertyName );
			}

			if( m_cachedPropertyId == -1 )
				m_cachedPropertyId = Shader.PropertyToID( m_propertyName );
		}

		public void SetPreviewInputTexture()
		{
			PreparePortCacheID();

			if( (object)m_node == null )
				m_node = UIUtils.GetNode( NodeId );
			m_node.PreviewMaterial.SetTexture( m_cachedPropertyId, GetOutputConnection( 0 ).OutputPreviewTexture );
		}

		private void SetPortPreviewShader( Shader portShader )
		{
			if( m_inputPreviewShader != portShader )
			{
				m_inputPreviewShader = portShader;
				InputPreviewMaterial.shader = portShader;
			}
		}

		public void SetPreviewInputValue()
		{
			if( m_inputPreviewTexture == null )
			{
				m_inputPreviewTexture = new RenderTexture( 128, 128, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear );
				m_inputPreviewTexture.wrapMode = TextureWrapMode.Repeat;
			}

			switch( DataType )
			{
				case WirePortDataType.INT:
				{
					SetPortPreviewShader( UIUtils.IntShader );

					InputPreviewMaterial.SetInt( CachedIntPropertyID, m_previewInternalInt );
				}
				break;
				case WirePortDataType.FLOAT:
				{
					SetPortPreviewShader( UIUtils.FloatShader );

					InputPreviewMaterial.SetFloat( CachedFloatPropertyID, m_previewInternalFloat );
				}
				break;
				case WirePortDataType.FLOAT2:
				{
					SetPortPreviewShader( UIUtils.Vector2Shader );

					Vector2 v2 = m_previewInternalVec2;// Vector2InternalData;
					InputPreviewMaterial.SetVector( CachedVectorPropertyID, new Vector4( v2.x, v2.y, 0, 0 ) );
				}
				break;
				case WirePortDataType.FLOAT3:
				{
					SetPortPreviewShader( UIUtils.Vector3Shader );

					Vector3 v3 = m_previewInternalVec3;// Vector3InternalData;
					InputPreviewMaterial.SetVector( CachedVectorPropertyID, new Vector4( v3.x, v3.y, v3.z, 0 ) );
				}
				break;
				case WirePortDataType.FLOAT4:
				{
					SetPortPreviewShader( UIUtils.Vector4Shader );

					InputPreviewMaterial.SetVector( CachedVectorPropertyID, m_previewInternalVec4 );
				}
				break;
				case WirePortDataType.COLOR:
				{
					SetPortPreviewShader( UIUtils.ColorShader );

					InputPreviewMaterial.SetColor( CachedColorPropertyID, m_previewInternalColor );
				}
				break;
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{
					SetPortPreviewShader( UIUtils.FloatShader );

					InputPreviewMaterial.SetFloat( CachedFloatPropertyID, 1 );
				}
				break;
				case WirePortDataType.SAMPLER2D:
				{
					SetPortPreviewShader( UIUtils.Texture2DShader );
				}
				break;
				default:
				{
					SetPortPreviewShader( UIUtils.FloatShader );

					InputPreviewMaterial.SetFloat( CachedFloatPropertyID, 0 );
				}
				break;
			}

			RenderTexture temp = RenderTexture.active;
			RenderTexture.active = m_inputPreviewTexture;
			Graphics.Blit( null, m_inputPreviewTexture, InputPreviewMaterial );
			RenderTexture.active = temp;

			PreparePortCacheID();

			if( (object)m_node == null )
				m_node = UIUtils.GetNode( NodeId );
			m_node.PreviewMaterial.SetTexture( m_propertyName, m_inputPreviewTexture );
		}

		public override void ChangePortId( int newPortId )
		{
			if( IsConnected )
			{
				int count = ExternalReferences.Count;
				for( int connIdx = 0; connIdx < count; connIdx++ )
				{
					int nodeId = ExternalReferences[ connIdx ].NodeId;
					int portId = ExternalReferences[ connIdx ].PortId;
					ParentNode node = UIUtils.GetNode( nodeId );
					if( node != null )
					{
						OutputPort outputPort = node.GetOutputPortByUniqueId( portId );
						int outputCount = outputPort.ExternalReferences.Count;
						for( int j = 0; j < outputCount; j++ )
						{
							if( outputPort.ExternalReferences[ j ].NodeId == NodeId &&
								outputPort.ExternalReferences[ j ].PortId == PortId )
							{
								outputPort.ExternalReferences[ j ].PortId = newPortId;
							}
						}
					}
				}
			}

			PortId = newPortId;
		}
		
		public override void Destroy()
		{
			base.Destroy();
			//if ( m_inputPreview != null )
			//	UnityEngine.ScriptableObject.DestroyImmediate( m_inputPreview );
			//m_inputPreview = null;

			if( m_inputPreviewTexture != null )
				UnityEngine.ScriptableObject.DestroyImmediate( m_inputPreviewTexture );
			m_inputPreviewTexture = null;

			if( m_inputPreviewMaterial != null )
				UnityEngine.ScriptableObject.DestroyImmediate( m_inputPreviewMaterial );
			m_inputPreviewMaterial = null;

			m_inputPreviewShader = null;

			m_node = null;
		}

		public Shader InputPreviewShader
		{
			get
			{
				if( m_inputPreviewShader == null )
					m_inputPreviewShader = AssetDatabase.LoadAssetAtPath<Shader>( AssetDatabase.GUIDToAssetPath( "d9ca47581ac157145bff6f72ac5dd73e" ) ); //ranged float

				if( m_inputPreviewShader == null )
					m_inputPreviewShader = Shader.Find( "Unlit/Colored Transparent" );

				return m_inputPreviewShader;
			}
			set
			{
				m_inputPreviewShader = value;
			}
		}

		public Material InputPreviewMaterial
		{
			get
			{
				if( m_inputPreviewMaterial == null )
					m_inputPreviewMaterial = new Material( InputPreviewShader );

				return m_inputPreviewMaterial;
			}
			//set
			//{
			//	m_inputPreviewMaterial = value;
			//}
		}

		public override string Name
		{
			get { return m_name; }
			set
			{
				m_name = value;
				m_internalDataPropertyLabel = ( string.IsNullOrEmpty( value ) || value.Equals( Constants.EmptyPortValue ) ) ? InputDefaultNameStr : value;
				m_dirtyLabelSize = true;
			}
		}

		public string InternalDataName
		{
			get { return m_internalDataPropertyLabel; }
			set { m_internalDataPropertyLabel = value; }
		}

		public bool AutoDrawInternalData
		{
			get { return m_drawInternalData; }
			set { m_drawInternalData = value; }
		}

		public PortGenType GenType
		{
			get { return m_genType; }
			set { m_genType = value; }
		}

		public bool ValidInternalData
		{
			get
			{
				switch( m_dataType )
				{
					case WirePortDataType.FLOAT:
					case WirePortDataType.FLOAT2:
					case WirePortDataType.FLOAT3:
					case WirePortDataType.FLOAT4:
					case WirePortDataType.FLOAT3x3:
					case WirePortDataType.FLOAT4x4:
					case WirePortDataType.COLOR:
					case WirePortDataType.INT: return true;
					case WirePortDataType.OBJECT:
					case WirePortDataType.SAMPLER1D:
					case WirePortDataType.SAMPLER2D:
					case WirePortDataType.SAMPLER3D:
					case WirePortDataType.SAMPLERCUBE:
					default: return false;
				}
			}
		}

		public RenderTexture InputPreviewTexture
		{
			get
			{
				if( IsConnected )
					return GetOutputConnection( 0 ).OutputPreviewTexture;
				else
					return m_inputPreviewTexture;
			}
		}

		public string ExternalLinkId
		{
			get { return m_externalLinkId; }
			set
			{
				m_externalLinkId = value;
				if( string.IsNullOrEmpty( value ) )
				{
					m_externalNodeLink = -1;
					m_externalPortLink = -1;
				}
			}
		}

		public bool HasOwnOrLinkConnection { get { return IsConnected || HasConnectedExternalLink; } }
		public bool HasExternalLink { get { return m_externalNodeLink > -1 && m_externalPortLink > -1; } }

		public bool HasConnectedExternalLink
		{
			get
			{
				InputPort link = ExternalLink;
				return ( link != null && link.IsConnected );
			}
		}

		public InputPort ExternalLink
		{
			get
			{
				if( HasExternalLink )
				{
					ParentNode linkNode = UIUtils.GetNode( m_externalNodeLink );
					if( linkNode != null )
					{
						return linkNode.GetInputPortByUniqueId( m_externalPortLink );
					}
				}
				return null;
			}
		}

		public ParentNode ExternalLinkNode
		{
			get
			{
				if( HasExternalLink )
				{
					return UIUtils.GetNode( m_externalNodeLink );
				}
				return null;
			}
		}
	}
}
