// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{

	[Serializable]
	[NodeAttributes( "Grab Screen Color", "Camera And Screen", "Grabed pixel color value from screen" )]
	public sealed class ScreenColorNode : PropertyNode
	{
		private readonly Color ReferenceHeaderColor = new Color( 0.6f, 3.0f, 1.25f, 1.0f );

		private const string SamplerType = "tex2D";
		private const string GrabTextureDefault = "_GrabTexture";
		private const string ScreenColorStr = "screenColor";

		[SerializeField]
		private TexReferenceType m_referenceType = TexReferenceType.Object;

		[SerializeField]
		private int m_referenceArrayId = -1;

		[SerializeField]
		private int m_referenceNodeId = -1;

		[SerializeField]
		private GUIStyle m_referenceIconStyle = null;

		private ScreenColorNode m_referenceNode = null;

		[SerializeField]
		private bool m_normalize = false;

		[SerializeField]
		private bool m_useCustomGrab = false;

		[SerializeField]
		private float m_referenceWidth = -1;

		//SRP specific code
		private string OpaqueTextureDefine = "REQUIRE_OPAQUE_TEXTURE 1";
		private string FetchOpaqueTexture = "SAMPLE_TEXTURE2D( _CameraOpaqueTexture, sampler_CameraOpaqueTexture, {0})";
		private string FetchVarName = "fetchOpaqueVal";
#if !UNITY_2018_3_OR_NEWER
		// Legacy SRP code
		private string DeclareOpaqueTextureObject = "TEXTURE2D( _CameraOpaqueTexture);";
		private string DeclareOpaqueTextureSampler = "SAMPLER( sampler_CameraOpaqueTexture);";
#endif
		public ScreenColorNode() : base() { }
		public ScreenColorNode( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { }

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );

			AddInputPort( WirePortDataType.FLOAT2, false, "UV" );
			AddOutputColorPorts( "RGBA" );

			m_currentParameterType = PropertyType.Global;
			m_underscoredGlobal = true;
			m_useVarSubtitle = true;
			m_customPrefix = "Grab Screen ";
			m_freeType = false;
			m_drawAttributes = false;
			m_showTitleWhenNotEditing = false;
			m_textLabelWidth = 125;
			m_showAutoRegisterUI = true;
			m_globalDefaultBehavior = false;
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			if( m_referenceType == TexReferenceType.Object )
				UIUtils.RegisterScreenColorNode( this );
		}

		void UpdateHeaderColor()
		{
			m_headerColorModifier = ( m_referenceType == TexReferenceType.Object ) ? Color.white : ReferenceHeaderColor;
		}

		public override void OnNodeLogicUpdate( DrawInfo drawInfo )
		{
			base.OnNodeLogicUpdate( drawInfo );
			if( m_referenceNodeId > -1 && m_referenceNode == null )
			{
				m_referenceNode = UIUtils.GetScreenColorNode( m_referenceNodeId ) as ScreenColorNode;
				if( m_referenceNode == null )
				{
					m_referenceNodeId = -1;
					m_referenceArrayId = -1;
					m_sizeIsDirty = true;
				}
			}

			if( m_showSubtitle == m_containerGraph.IsSRP )
			{
				m_showSubtitle = !m_containerGraph.IsSRP;
				m_sizeIsDirty = true;
			}
		}

		protected override void ChangeSizeFinished()
		{
			if( m_referenceType == TexReferenceType.Instance )
			{
				m_position.width += 20;
			}
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );

			CheckReference();

			if( SoftValidReference )
			{
				m_content.text = m_referenceNode.TitleContent.text + Constants.InstancePostfixStr;
				SetAdditonalTitleText( m_referenceNode.AdditonalTitleContent.text );

				if( m_referenceIconStyle == null )
				{
					m_referenceIconStyle = UIUtils.GetCustomStyle( CustomStyle.SamplerTextureIcon );
				}

				Rect iconPos = m_globalPosition;
				iconPos.width = 19 * drawInfo.InvertedZoom;
				iconPos.height = 19 * drawInfo.InvertedZoom;

				iconPos.y += 6 * drawInfo.InvertedZoom;
				iconPos.x += m_globalPosition.width - iconPos.width - 7 * drawInfo.InvertedZoom;

				if( GUI.Button( iconPos, string.Empty, m_referenceIconStyle ) )
				{
					UIUtils.FocusOnNode( m_referenceNode, 1, true );
				}
			}
		}

		void CheckReference()
		{
			if( m_referenceType != TexReferenceType.Instance )
			{
				return;
			}

			if( m_referenceArrayId > -1 )
			{
				ParentNode newNode = UIUtils.GetScreenColorNode( m_referenceArrayId );
				if( newNode == null || newNode.UniqueId != m_referenceNodeId )
				{
					m_referenceNode = null;
					int count = UIUtils.GetScreenColorNodeAmount();
					for( int i = 0; i < count; i++ )
					{
						ParentNode node = UIUtils.GetScreenColorNode( i );
						if( node.UniqueId == m_referenceNodeId )
						{
							m_referenceNode = node as ScreenColorNode;
							m_referenceArrayId = i;
							break;
						}
					}
				}
			}

			if( m_referenceNode == null && m_referenceNodeId > -1 )
			{
				m_referenceNodeId = -1;
				m_referenceArrayId = -1;
			}
		}

		public override void DrawMainPropertyBlock()
		{
			ShowAutoRegister();
			EditorGUI.BeginChangeCheck();
			m_referenceType = (TexReferenceType)EditorGUILayoutPopup( Constants.ReferenceTypeStr, (int)m_referenceType, Constants.ReferenceArrayLabels );
			if( EditorGUI.EndChangeCheck() )
			{
				m_sizeIsDirty = true;
				if( m_referenceType == TexReferenceType.Object )
				{
					UIUtils.RegisterScreenColorNode( this );
					m_content.text = m_propertyInspectorName;
				}
				else
				{
					UIUtils.UnregisterScreenColorNode( this );
					if( SoftValidReference )
					{
						m_content.text = m_referenceNode.TitleContent.text + Constants.InstancePostfixStr;
					}
				}
				UpdateHeaderColor();
			}

			if( m_referenceType == TexReferenceType.Object )
			{
				EditorGUI.BeginDisabledGroup( m_containerGraph.IsSRP );
				{
					EditorGUI.BeginChangeCheck();
					m_useCustomGrab = EditorGUILayoutToggle( "Custom Grab Pass", m_useCustomGrab );
					EditorGUI.BeginDisabledGroup( !m_useCustomGrab );
					DrawMainPropertyBlockNoPrecision();
					EditorGUI.EndDisabledGroup();

					m_normalize = EditorGUILayoutToggle( "Normalize", m_normalize );
					if( EditorGUI.EndChangeCheck() )
					{
						UpdatePort();
						if( m_useCustomGrab )
						{
							BeginPropertyFromInspectorCheck();
						}
					}
				}
				EditorGUI.EndDisabledGroup();
			}
			else
			{
				string[] arr = UIUtils.ScreenColorNodeArr();
				bool guiEnabledBuffer = GUI.enabled;
				if( arr != null && arr.Length > 0 )
				{
					GUI.enabled = true;
				}
				else
				{
					m_referenceArrayId = -1;
					GUI.enabled = false;
				}

				m_referenceArrayId = EditorGUILayoutPopup( Constants.AvailableReferenceStr, m_referenceArrayId, arr );
				GUI.enabled = guiEnabledBuffer;
				EditorGUI.BeginDisabledGroup( m_containerGraph.IsSRP );
				{
					EditorGUI.BeginChangeCheck();
					m_normalize = EditorGUILayoutToggle( "Normalize", m_normalize );
					if( EditorGUI.EndChangeCheck() )
					{
						UpdatePort();
					}
				}
				EditorGUI.EndDisabledGroup();
			}
		}

		private void UpdatePort()
		{
			if( m_normalize )
				m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4, false );
			else
				m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT2, false );
		}

		public override void DrawTitle( Rect titlePos )
		{
			if( !m_isEditing && ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD3 )
			{
				GUI.Label( titlePos, "Grab Screen Color", UIUtils.GetCustomStyle( CustomStyle.NodeTitle ) );
			}

			if( m_useCustomGrab || SoftValidReference )
			{
				base.DrawTitle( titlePos );
				m_previousAdditonalTitle = m_additionalContent.text;
			}
			else
			if( ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD3 )
			{
				SetAdditonalTitleTextOnCallback( GrabTextureDefault, ( instance, newSubTitle ) => instance.AdditonalTitleContent.text = string.Format( Constants.SubTitleVarNameFormatStr, newSubTitle ) );
				//GUI.Label( titlePos, PropertyInspectorName, UIUtils.GetCustomStyle( CustomStyle.NodeTitle ) );
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if( dataCollector.IsTemplate && dataCollector.CurrentSRPType == TemplateSRPType.HD )
			{
				UIUtils.ShowMessage( "GrabPasses are not supported on Unity HD Scriptable Rendering Pipeline." );
				return GetOutputColorItem( 0, outputId, "(0).xxxx" );
			}

			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return GetOutputColorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );

			string valueName = string.Empty;
			if( dataCollector.IsSRP )
			{
#if !UNITY_2018_3_OR_NEWER
				dataCollector.AddToUniforms( UniqueId, DeclareOpaqueTextureObject );
				dataCollector.AddToUniforms( UniqueId, DeclareOpaqueTextureSampler );
#endif
				valueName = FetchVarName + OutputId;
				dataCollector.AddToDefines( UniqueId, OpaqueTextureDefine );
				string uvCoords = GetUVCoords( ref dataCollector, ignoreLocalVar, false );
				dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT4, valueName, string.Format( FetchOpaqueTexture, uvCoords ) );
			}
			else
			{
				base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalVar );
				string propertyName = CurrentPropertyReference;
				OnPropertyNameChanged();
				bool emptyName = string.IsNullOrEmpty( m_propertyInspectorName ) || propertyName == GrabTextureDefault;
				dataCollector.AddGrabPass( emptyName ? string.Empty : propertyName );
				valueName = SetFetchedData( ref dataCollector, ignoreLocalVar );
			}

			m_outputPorts[ 0 ].SetLocalValue( valueName, dataCollector.PortCategory );
			return GetOutputColorItem( 0, outputId, valueName );
		}

		public string SetFetchedData( ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			string propertyName = CurrentPropertyReference;

			bool isProjecting = m_normalize;

			if( !m_inputPorts[ 0 ].IsConnected ) // to generate proper screen pos by itself
				isProjecting = true;

			if( ignoreLocalVar )
			{
				string samplerValue = SamplerType + ( isProjecting ? "proj" : "" ) + "( " + propertyName + ", " + GetUVCoords( ref dataCollector, ignoreLocalVar, isProjecting ) + " )";
				return samplerValue;
			}

			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			string samplerOp = SamplerType + ( isProjecting ? "proj" : "" ) + "( " + propertyName + ", " + GetUVCoords( ref dataCollector, ignoreLocalVar, isProjecting ) + " )";

			dataCollector.AddLocalVariable( UniqueId, UIUtils.PrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType ) + " " + ScreenColorStr + OutputId + " = " + samplerOp + ";" );
			return ScreenColorStr + OutputId;
		}

		public string GetUVCoords( ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar, bool isProjecting )
		{
			string result = string.Empty;

			if( m_inputPorts[ 0 ].IsConnected )
			{
				result = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, ( isProjecting ? WirePortDataType.FLOAT4 : WirePortDataType.FLOAT2 ), ignoreLocalVar, true );
			}
			else
			{
				string customScreenPos = null;

				if( dataCollector.IsTemplate )
					customScreenPos = dataCollector.TemplateDataCollectorInstance.GetScreenPos();

				if( isProjecting )
					result = GeneratorUtils.GenerateGrabScreenPosition( ref dataCollector, UniqueId, m_currentPrecisionType, !dataCollector.UsingCustomScreenPos, customScreenPos );
				else
					result = GeneratorUtils.GenerateGrabScreenPositionNormalized( ref dataCollector, UniqueId, m_currentPrecisionType, !dataCollector.UsingCustomScreenPos, customScreenPos );
			}

			if( isProjecting && !dataCollector.IsSRP )
				return "UNITY_PROJ_COORD( " + result + " )";
			else
				return result;
		}

		public override void Destroy()
		{
			base.Destroy();
			if( m_referenceType == TexReferenceType.Object )
			{
				UIUtils.UnregisterScreenColorNode( this );
			}
		}

		public bool SoftValidReference
		{
			get
			{
				if( m_referenceType == TexReferenceType.Instance && m_referenceArrayId > -1 )
				{
					m_referenceNode = UIUtils.GetScreenColorNode( m_referenceArrayId );
					if( m_referenceNode == null )
					{
						m_referenceArrayId = -1;
						m_referenceWidth = -1;
					}
					else if( m_referenceWidth != m_referenceNode.Position.width )
					{
						m_referenceWidth = m_referenceNode.Position.width;
						m_sizeIsDirty = true;
					}
					return m_referenceNode != null;
				}
				return false;
			}
		}

		public string CurrentPropertyReference
		{
			get
			{
				string propertyName = string.Empty;
				if( m_referenceType == TexReferenceType.Instance && m_referenceArrayId > -1 )
				{
					ScreenColorNode node = UIUtils.GetScreenColorNode( m_referenceArrayId );
					propertyName = ( node != null ) ? node.PropertyName : m_propertyName;
				}
				else if( !m_useCustomGrab )
				{
					propertyName = GrabTextureDefault;
				}
				else
				{
					propertyName = m_propertyName;
				}
				return propertyName;
			}
		}


		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if( UIUtils.CurrentShaderVersion() > 12 )
			{
				m_referenceType = (TexReferenceType)Enum.Parse( typeof( TexReferenceType ), GetCurrentParam( ref nodeParams ) );
				if( UIUtils.CurrentShaderVersion() > 22 )
				{
					m_referenceNodeId = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				}
				else
				{
					m_referenceArrayId = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				}

				if( m_referenceType == TexReferenceType.Instance )
				{
					UIUtils.UnregisterScreenColorNode( this );
				}

				UpdateHeaderColor();
			}

			if( UIUtils.CurrentShaderVersion() > 12101 )
			{
				m_useCustomGrab = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			}
			else
			{
				m_useCustomGrab = true;
			}

			if( UIUtils.CurrentShaderVersion() > 14102 )
			{
				m_normalize = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_referenceType );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( ( m_referenceNode != null ) ? m_referenceNode.UniqueId : -1 ) );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_useCustomGrab );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_normalize );
		}

		public override void RefreshExternalReferences()
		{
			base.RefreshExternalReferences();
			if( m_referenceType == TexReferenceType.Instance )
			{
				if( UIUtils.CurrentShaderVersion() > 22 )
				{
					m_referenceNode = UIUtils.GetNode( m_referenceNodeId ) as ScreenColorNode;
					m_referenceArrayId = UIUtils.GetScreenColorNodeRegisterId( m_referenceNodeId );
				}
				else
				{
					m_referenceNode = UIUtils.GetScreenColorNode( m_referenceArrayId );
					if( m_referenceNode != null )
					{
						m_referenceNodeId = m_referenceNode.UniqueId;
					}
				}
			}

			if( UIUtils.CurrentShaderVersion() <= 14102 )
			{
				if( m_inputPorts[ 0 ].DataType == WirePortDataType.FLOAT4 )
					m_normalize = true;
				else
					m_normalize = false;
			}
		}

		public override string PropertyName
		{
			get
			{
				if( m_useCustomGrab )
					return base.PropertyName;
				else
					return GrabTextureDefault;
			}
		}

		public override string GetPropertyValStr()
		{
			return PropertyName;
		}

		public override string DataToArray { get { return m_propertyName; } }

		public override string GetUniformValue()
		{
			if( SoftValidReference )
			{
				if( m_referenceNode.IsConnected )
					return string.Empty;

				return m_referenceNode.GetUniformValue();
			}

			return "uniform sampler2D " + PropertyName + ";";
		}

		public override bool GetUniformData( out string dataType, out string dataName )
		{
			if( SoftValidReference )
			{
				//if ( m_referenceNode.IsConnected )
				//{
				//	dataType = string.Empty;
				//	dataName = string.Empty;
				//}

				return m_referenceNode.GetUniformData( out dataType, out dataName );
			}

			dataType = "sampler2D";
			dataName = PropertyName;
			return true;
		}

		public override void CheckIfAutoRegister( ref MasterNodeDataCollector dataCollector )
		{
			if( m_autoRegister && m_connStatus != NodeConnectionStatus.Connected )
			{
				RegisterProperty( ref dataCollector );
				string propertyName = CurrentPropertyReference;
				bool emptyName = string.IsNullOrEmpty( m_propertyInspectorName ) || propertyName == GrabTextureDefault;
				dataCollector.AddGrabPass( emptyName ? string.Empty : propertyName );
			}
		}
	}
}
