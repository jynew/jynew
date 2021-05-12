// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[System.Serializable]
	public sealed class OutputPort : WirePort
	{
		[SerializeField]
		private bool m_connectedToMasterNode;

		[SerializeField]
		private bool[] m_isLocalValue = { false, false};

		[SerializeField]
		private string[] m_localOutputValue = { string.Empty,string.Empty};

		//[SerializeField]
		//private int m_isLocalWithPortType = 0;

		private RenderTexture m_outputPreview = null;
		private Material m_outputMaskMaterial = null;

		private int m_indexPreviewOffset = 0;

		public OutputPort( int nodeId, int portId, WirePortDataType dataType, string name ) : base( nodeId, portId, dataType, name ) { LabelSize = Vector2.zero; }

		public string ErrorValue
		{
			get
			{
				string value = string.Empty;
				switch( m_dataType )
				{
					default:
					case WirePortDataType.OBJECT:
					case WirePortDataType.INT:
					case WirePortDataType.FLOAT: value = "0"; break;
					case WirePortDataType.FLOAT2: value = "half2(0,0)"; break;
					case WirePortDataType.FLOAT3: value = "half3(0,0,0)"; break;
					case WirePortDataType.COLOR:
					case WirePortDataType.FLOAT4: value = "half4(0,0,0,0)"; break;
					case WirePortDataType.FLOAT3x3: value = "half3x3(0,0,0,0,0,0,0,0,0)"; break;
					case WirePortDataType.FLOAT4x4: value = "half4x4(0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)"; break;
				}
				return value;
			}
		}

		public bool ConnectedToMasterNode
		{
			get { return m_connectedToMasterNode; }
			set { m_connectedToMasterNode = value; }
		}

		public override void FullDeleteConnections()
		{
			UIUtils.DeleteConnection( false, m_nodeId, m_portId, true, true );
		}

		public bool HasConnectedNode
		{
			get
			{
				int count = m_externalReferences.Count;
				for( int i = 0; i < count; i++ )
				{
					if( UIUtils.GetNode( m_externalReferences[ i ].NodeId ).IsConnected )
						return true;
				}
				return false;
			}
		}
		public InputPort GetInputConnection( int connID = 0 )
		{
			if( connID < m_externalReferences.Count )
			{
				return UIUtils.GetNode( m_externalReferences[ connID ].NodeId ).GetInputPortByUniqueId( m_externalReferences[ connID ].PortId );
			}
			return null;
		}

		public ParentNode GetInputNode( int connID = 0 )
		{
			if( connID < m_externalReferences.Count )
			{
				return UIUtils.GetNode( m_externalReferences[ connID ].NodeId );
			}
			return null;
		}

		public override void NotifyExternalRefencesOnChange()
		{
			for( int i = 0; i < m_externalReferences.Count; i++ )
			{
				ParentNode node = UIUtils.GetNode( m_externalReferences[ i ].NodeId );
				if( node )
				{
					node.CheckSpherePreview();
					InputPort port = node.GetInputPortByUniqueId( m_externalReferences[ i ].PortId );
					port.UpdateInfoOnExternalConn( m_nodeId, m_portId, m_dataType );
					node.OnConnectedOutputNodeChanges( m_externalReferences[ i ].PortId, m_nodeId, m_portId, m_name, m_dataType );
				}
			}
		}

		public void ChangeTypeWithRestrictions( WirePortDataType newType, int restrictions )
		{
			if( m_dataType != newType )
			{
				DataType = newType;
				for( int i = 0; i < m_externalReferences.Count; i++ )
				{
					ParentNode inNode = UIUtils.GetNode( m_externalReferences[ i ].NodeId );
					InputPort inputPort = inNode.GetInputPortByUniqueId( m_externalReferences[ i ].PortId );

					bool valid = false;
					if( restrictions == 0 )
					{
						valid = true;
					}
					else
					{
						valid = ( restrictions & (int)inputPort.DataType ) != 0;
					}

					if( valid )
					{
						inNode.CheckSpherePreview();
						inputPort.UpdateInfoOnExternalConn( m_nodeId, m_portId, m_dataType );
						inNode.OnConnectedOutputNodeChanges( m_externalReferences[ i ].PortId, m_nodeId, m_portId, m_name, m_dataType );
					}
					else
					{
						InvalidateConnection( m_externalReferences[ i ].NodeId, m_externalReferences[ i ].PortId );
						inputPort.InvalidateConnection( NodeId, PortId );
						i--;
					}
				}
			}
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
                        InputPort inputPort = node.GetInputPortByUniqueId( portId );
                        int inputCount = inputPort.ExternalReferences.Count;
                        for( int j = 0; j < inputCount; j++ )
                        {
                            if( inputPort.ExternalReferences[ j ].NodeId == NodeId &&
                                inputPort.ExternalReferences[ j ].PortId == PortId )
                            {
                                inputPort.ExternalReferences[ j ].PortId = newPortId;
                            }
                        }
                    }
                }
            }

            PortId = newPortId;
        }

        public string ConfigOutputLocalValue( PrecisionType precisionType, string value, string customName, MasterNodePortCategory category )
		{
			int idx = UIUtils.PortCategorytoAttayIdx( category );
			ParentGraph currentGraph = UIUtils.GetNode( NodeId ).ContainerGraph;
			string autoGraphId = currentGraph.GraphId > 0 ? "_g" + currentGraph.GraphId : string.Empty;
			m_localOutputValue[idx] = string.IsNullOrEmpty( customName ) ? ( "temp_output_" + m_nodeId + "_" + PortId + autoGraphId ) : customName;
			m_isLocalValue[idx] = true;
			//m_isLocalWithPortType |= (int)category;
			return string.Format( Constants.LocalValueDecWithoutIdent, UIUtils.PrecisionWirePortToCgType( precisionType, DataType ), m_localOutputValue[idx], value );
		}

		public void SetLocalValue( string value, MasterNodePortCategory category )
		{
			int idx = UIUtils.PortCategorytoAttayIdx( category );
			m_isLocalValue[idx] = true;
			m_localOutputValue[ idx ] = value;
			//m_isLocalWithPortType |= (int)category;
		}

		public void ResetLocalValue()
		{
			for( int i = 0; i < m_localOutputValue.Length; i++ )
			{
				m_localOutputValue[ i ] = string.Empty;
				m_isLocalValue[i] = false;
			}
			//m_isLocalWithPortType = 0;
		}

		public void ResetLocalValueIfNot( MasterNodePortCategory category )
		{
			int idx = UIUtils.PortCategorytoAttayIdx( category );
			for( int i = 0; i < m_localOutputValue.Length; i++ )
			{
				if( i != idx )
				{
					m_localOutputValue[ i ] = string.Empty;
					m_isLocalValue[ i ] = false;
				}
			}
		}

		public void ResetLocalValueOnCategory( MasterNodePortCategory category )
		{
			int idx = UIUtils.PortCategorytoAttayIdx( category );
			m_localOutputValue[ idx ] = string.Empty;
			m_isLocalValue[ idx ] = false;
		}

		public bool IsLocalOnCategory( MasterNodePortCategory category )
		{
			int idx = UIUtils.PortCategorytoAttayIdx( category );
			return m_isLocalValue[ idx ];
			//return ( m_isLocalWithPortType & (int)category ) != 0; ;
		}

		public override void ForceClearConnection()
		{
			UIUtils.DeleteConnection( false, m_nodeId, m_portId, false, true );
		}

		public bool IsLocalValue( MasterNodePortCategory category )
		{
			int idx = UIUtils.PortCategorytoAttayIdx( category );
			return m_isLocalValue[ idx ];
		}

		public string LocalValue(MasterNodePortCategory category)
		{
			int idx = UIUtils.PortCategorytoAttayIdx( category );
			return m_localOutputValue[idx];
		}

		public RenderTexture OutputPreviewTexture
		{
			get
			{
				if( m_outputPreview == null )
				{
					m_outputPreview = new RenderTexture( 128, 128, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear );
					m_outputPreview.wrapMode = TextureWrapMode.Repeat;
				}

				return m_outputPreview;
			}
			set { m_outputPreview = value; }
		}

		public int IndexPreviewOffset
		{
			get { return m_indexPreviewOffset; }
			set { m_indexPreviewOffset = value; }
		}

		public override void Destroy()
		{
			base.Destroy();
			if( m_outputPreview != null )
				UnityEngine.ScriptableObject.DestroyImmediate( m_outputPreview );
			m_outputPreview = null;

			if( m_outputMaskMaterial != null )
				UnityEngine.ScriptableObject.DestroyImmediate( m_outputMaskMaterial );
			m_outputMaskMaterial = null;
		}

		public Material MaskingMaterial
		{
			get
			{
				if( m_outputMaskMaterial == null )
				{
					//m_outputMaskMaterial = new Material( AssetDatabase.LoadAssetAtPath<Shader>( AssetDatabase.GUIDToAssetPath( "9c34f18ebe2be3e48b201b748c73dec0" ) ) );
					m_outputMaskMaterial = new Material( UIUtils.MaskingShader );
				}
				return m_outputMaskMaterial;
			}
			//set { m_outputMaskMaterial = value; }
		}
	}
}
