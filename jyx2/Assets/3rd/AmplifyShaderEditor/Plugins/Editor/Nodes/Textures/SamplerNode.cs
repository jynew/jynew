// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	// Disabling Substance Deprecated warning

	public enum TexReferenceType
	{
		Object = 0,
		Instance
	}

	public enum MipType
	{
		Auto,
		MipLevel,
		MipBias,
		Derivative
	}

	public enum ReferenceState
	{
		Self,
		Connected,
		Instance
	}

	[Serializable]
#if UNITY_2018_1_OR_NEWER
	[NodeAttributes( "Texture Sample", "Textures", "Samples a chosen texture and returns its color values, <b>Texture</b> and <b>UVs</b> can be overriden and you can select different mip modes and levels. It can also unpack and scale textures marked as normalmaps.", KeyCode.T, true, 0, int.MaxValue, typeof( Texture ), typeof( Texture2D ), typeof( Texture3D ), typeof( Cubemap ), typeof( CustomRenderTexture ))]
#else

	// Disabling Substance Deprecated warning
#pragma warning disable 0618
	[NodeAttributes( "Texture Sample", "Textures", "Samples a chosen texture and returns its color values, <b>Texture</b> and <b>UVs</b> can be overriden and you can select different mip modes and levels. It can also unpack and scale textures marked as normalmaps.", KeyCode.T, true, 0, int.MaxValue, typeof( Texture ), typeof( Texture2D ), typeof( Texture3D ), typeof( Cubemap ), typeof( ProceduralTexture ),typeof( RenderTexture )
#if UNITY_2017_1_OR_NEWER
		,typeof( CustomRenderTexture )
#endif
		)]
#pragma warning restore 0618
#endif
	public sealed class SamplerNode : TexturePropertyNode
	{
		private const string MipModeStr = "Mip Mode";

		private const string DefaultTextureUseSematicsStr = "Use Semantics";
		private const string DefaultTextureIsNormalMapsStr = "Is Normal Map";

		private const string NormalScaleStr = "Scale";

		private float InstanceIconWidth = 19;
		private float InstanceIconHeight = 19;

		private readonly Color ReferenceHeaderColor = new Color( 2.66f, 1.02f, 0.6f, 1.0f );

		public readonly static int[] AvailableAutoCast = { 0, 1, 2, 3, 4 };
		public readonly static string[] AvailableAutoCastStr = { "Auto", "Locked To Texture 1D", "Locked To Texture 2D", "Locked To Texture 3D", "Locked To Cube" };

		[SerializeField]
		private int m_textureCoordSet = 0;

		[SerializeField]
		private string m_normalMapUnpackMode;

		[SerializeField]
		private bool m_autoUnpackNormals = false;

		[SerializeField]
		private bool m_useSemantics;

		[SerializeField]
		private string m_samplerType;

		[SerializeField]
		private MipType m_mipMode = MipType.Auto;

		[SerializeField]
		private TexReferenceType m_referenceType = TexReferenceType.Object;

		[SerializeField]
		private int m_referenceArrayId = -1;

		[SerializeField]
		private int m_referenceNodeId = -1;

		private SamplerNode m_referenceSampler = null;

		[SerializeField]
		private GUIStyle m_referenceStyle = null;

		[SerializeField]
		private GUIStyle m_referenceIconStyle = null;

		[SerializeField]
		private GUIContent m_referenceContent = null;

		[SerializeField]
		private float m_referenceWidth = -1;

		private string m_previousAdditionalText = string.Empty;

		private int m_cachedUvsId = -1;
		private int m_cachedUnpackId = -1;
		private int m_cachedLodId = -1;

		private InputPort m_texPort;
		private InputPort m_uvPort;
		private InputPort m_lodPort;
		private InputPort m_ddxPort;
		private InputPort m_ddyPort;
		private InputPort m_normalPort;

		private OutputPort m_colorPort;

		private TexturePropertyNode m_previewTextProp = null;
		private ReferenceState m_state = ReferenceState.Self;

		public SamplerNode() : base() { }
		public SamplerNode( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { }
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_defaultTextureValue = TexturePropertyValues.white;
			AddInputPort( WirePortDataType.SAMPLER2D, false, "Tex" );
			m_inputPorts[ 0 ].CreatePortRestrictions( WirePortDataType.SAMPLER1D, WirePortDataType.SAMPLER2D, WirePortDataType.SAMPLER3D, WirePortDataType.SAMPLERCUBE, WirePortDataType.OBJECT );
			AddInputPort( WirePortDataType.FLOAT2, false, "UV" );
			AddInputPort( WirePortDataType.FLOAT, false, "Level" );
			AddInputPort( WirePortDataType.FLOAT2, false, "DDX" );
			AddInputPort( WirePortDataType.FLOAT2, false, "DDY" );
			AddInputPort( WirePortDataType.FLOAT, false, NormalScaleStr );

			m_texPort = m_inputPorts[ 0 ];
			m_uvPort = m_inputPorts[ 1 ];
			m_lodPort = m_inputPorts[ 2 ];
			m_ddxPort = m_inputPorts[ 3 ];
			m_ddyPort = m_inputPorts[ 4 ];
			m_normalPort = m_inputPorts[ 5 ];
			m_normalPort.AutoDrawInternalData = true;
			m_lodPort.Visible = false;
			m_ddxPort.Visible = false;
			m_ddyPort.Visible = false;
			m_normalPort.Visible = m_autoUnpackNormals;
			m_normalPort.FloatInternalData = 1.0f;

			//Remove output port (sampler)
			m_outputPortsDict.Remove( m_outputPorts[ 0 ].PortId );
			m_outputPorts.RemoveAt( 0 );

			AddOutputColorPorts( "RGBA" );
			m_colorPort = m_outputPorts[ 0 ];
			m_currentParameterType = PropertyType.Property;
			//	m_useCustomPrefix = true;
			m_customPrefix = "Texture Sample ";
			m_referenceContent = new GUIContent( string.Empty );
			m_freeType = false;
			m_useSemantics = true;
			m_drawPicker = false;
			ConfigTextureData( TextureType.Texture2D );
			m_selectedLocation = PreviewLocation.TopCenter;
			m_previewShaderGUID = "7b4e86a89b70ae64993bf422eb406422";

			m_errorMessageTooltip = "A texture object marked as normal map is connected to this sampler. Please consider turning on the Unpack Normal Map option";
			m_errorMessageTypeIsError = NodeMessageType.Warning;
			m_textLabelWidth = 135;
		}

		public override void SetPreviewInputs()
		{
			//TODO: rewrite this to be faster
			base.SetPreviewInputs();

			if( m_cachedUvsId == -1 )
				m_cachedUvsId = Shader.PropertyToID( "_CustomUVs" );

			PreviewMaterial.SetInt( m_cachedUvsId, ( m_uvPort.IsConnected ? 1 : 0 ) );

			if( m_cachedUnpackId == -1 )
				m_cachedUnpackId = Shader.PropertyToID( "_Unpack" );

			PreviewMaterial.SetInt( m_cachedUnpackId, m_autoUnpackNormals ? 1 : 0 );

			if( m_cachedLodId == -1 )
				m_cachedLodId = Shader.PropertyToID( "_LodType" );

			PreviewMaterial.SetInt( m_cachedLodId, ( m_mipMode == MipType.MipLevel ? 1 : ( m_mipMode == MipType.MipBias ? 2 : 0 ) ) );

			if( m_typeId == -1 )
				m_typeId = Shader.PropertyToID( "_Type" );

			bool usingTexture = false;
			if( m_texPort.IsConnected )
			{
				usingTexture = true;
				SetPreviewTexture( m_texPort.InputPreviewTexture );
			}
			else if( SoftValidReference && m_referenceSampler.TextureProperty != null )
			{
				if( m_referenceSampler.TextureProperty.Value != null )
				{
					usingTexture = true;
					SetPreviewTexture( m_referenceSampler.TextureProperty.Value );
				}
				else
				{
					usingTexture = true;
					SetPreviewTexture( m_referenceSampler.PreviewTexture );
				}
			}
			else if( TextureProperty != null )
			{
				if( TextureProperty.Value != null )
				{
					usingTexture = true;
					SetPreviewTexture( TextureProperty.Value );
				}
			}

			if( m_defaultId == -1 )
				m_defaultId = Shader.PropertyToID( "_Default" );

			if( usingTexture )
				PreviewMaterial.SetInt( m_defaultId, 0 );
			else
				PreviewMaterial.SetInt( m_defaultId, ( (int)m_defaultTextureValue ) + 1 );
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			if( m_referenceType == TexReferenceType.Object )
			{
				UIUtils.RegisterSamplerNode( this );
				UIUtils.RegisterPropertyNode( this );
			}
			m_textureProperty = this;
		}

		public void ConfigSampler()
		{
			switch( m_currentType )
			{
				case TextureType.Texture1D:
				m_samplerType = "tex1D";
				break;
				case TextureType.ProceduralTexture:
				case TextureType.Texture2D:
				m_samplerType = "tex2D";
				break;
				case TextureType.Texture3D:
				m_samplerType = "tex3D";
				break;
				case TextureType.Cube:
				m_samplerType = "texCUBE";
				break;
			}
		}

		public override void DrawSubProperties()
		{
			ShowDefaults();

			DrawSamplerOptions();

			EditorGUI.BeginChangeCheck();
			Type currType = ( m_autocastMode == AutoCastType.Auto ) ? typeof( Texture ) : m_textureType;
			m_defaultValue = EditorGUILayoutObjectField( Constants.DefaultValueLabel, m_defaultValue, currType, false ) as Texture;
			if( EditorGUI.EndChangeCheck() )
			{
				CheckTextureImporter( true );
				SetAdditonalTitleText( string.Format( Constants.PropertyValueLabel, GetPropertyValStr() ) );
				ConfigureInputPorts();
				ConfigureOutputPorts();
				//ResizeNodeToPreview();
			}
		}

		public override void DrawMaterialProperties()
		{
			ShowDefaults();

			DrawSamplerOptions();

			EditorGUI.BeginChangeCheck();
			Type currType = ( m_autocastMode == AutoCastType.Auto ) ? typeof( Texture ) : m_textureType;
			m_materialValue = EditorGUILayoutObjectField( Constants.MaterialValueLabel, m_materialValue, currType, false ) as Texture;
			if( EditorGUI.EndChangeCheck() )
			{
				CheckTextureImporter( true );
				SetAdditonalTitleText( string.Format( Constants.PropertyValueLabel, GetPropertyValStr() ) );
				ConfigureInputPorts();
				ConfigureOutputPorts();
			}
		}

		new void ShowDefaults()
		{
			m_defaultTextureValue = (TexturePropertyValues)EditorGUILayoutEnumPopup( DefaultTextureStr, m_defaultTextureValue );
			AutoCastType newAutoCast = (AutoCastType)EditorGUILayoutIntPopup( AutoCastModeStr, (int)m_autocastMode, AvailableAutoCastStr, AvailableAutoCast );
			//AutoCastType newAutoCast = (AutoCastType)EditorGUILayoutEnumPopup( AutoCastModeStr, m_autocastMode );
			if( newAutoCast != m_autocastMode )
			{
				m_autocastMode = newAutoCast;
				if( m_autocastMode != AutoCastType.Auto )
				{
					ConfigTextureData( m_currentType );
					ConfigureInputPorts();
					ConfigureOutputPorts();
				}
			}
		}

		public override void AdditionalCheck()
		{
			m_autoUnpackNormals = m_isNormalMap;
			ConfigureInputPorts();
			ConfigureOutputPorts();
		}


		public override void OnConnectedOutputNodeChanges( int portId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( portId, otherNodeId, otherPortId, name, type );
			if( portId == m_texPort.PortId )
			{
				m_textureProperty = m_texPort.GetOutputNode( 0 ) as TexturePropertyNode;
				if( m_textureProperty != null )
				{
					m_currentType = m_textureProperty.CurrentType;
					ConfigureInputPorts();
					ConfigureOutputPorts();
				}
			}
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );

			if( portId == m_texPort.PortId )
			{
				m_textureProperty = m_texPort.GetOutputNode( 0 ) as TexturePropertyNode;

				if( m_textureProperty == null )
				{
					m_textureProperty = this;
					// This cast fails only from within shader functions if connected to a Sampler Input
					// and in this case property is set by what is connected to that input
					UIUtils.UnregisterPropertyNode( this );
					UIUtils.UnregisterTexturePropertyNode( this );
				}
				else
				{
					//if( m_autocastMode == AutoCastType.Auto )
					//{
					m_currentType = m_textureProperty.CurrentType;
					//}


					//if ( m_textureProperty is VirtualTexturePropertyNode )
					//{
					//	AutoUnpackNormals = ( m_textureProperty as VirtualTexturePropertyNode ).Channel == VirtualChannel.Normal;
					//}
					//else if( m_textureProperty.IsValid )
					//{

					//	AutoUnpackNormals = m_textureProperty.IsNormalMap;
					//}

					UIUtils.UnregisterPropertyNode( this );
					UIUtils.UnregisterTexturePropertyNode( this );
				}

				ConfigureInputPorts();
				ConfigureOutputPorts();
				//ResizeNodeToPreview();
			}
		}

		public override void OnInputPortDisconnected( int portId )
		{
			base.OnInputPortDisconnected( portId );

			if( portId == m_texPort.PortId )
			{
				m_textureProperty = this;

				if( m_referenceType == TexReferenceType.Object )
				{
					UIUtils.RegisterPropertyNode( this );
					UIUtils.RegisterTexturePropertyNode( this );
				}

				ConfigureOutputPorts();
				//ResizeNodeToPreview();
			}
		}

		private void ForceInputPortsChange()
		{
			m_texPort.ChangeType( WirePortDataType.SAMPLER2D, false );
			m_normalPort.ChangeType( WirePortDataType.FLOAT, false );
			switch( m_currentType )
			{
				case TextureType.Texture1D:
				m_uvPort.ChangeType( WirePortDataType.FLOAT, false );
				m_ddxPort.ChangeType( WirePortDataType.FLOAT, false );
				m_ddyPort.ChangeType( WirePortDataType.FLOAT, false );
				break;
				case TextureType.ProceduralTexture:
				case TextureType.Texture2D:
				m_uvPort.ChangeType( WirePortDataType.FLOAT2, false );
				m_ddxPort.ChangeType( WirePortDataType.FLOAT2, false );
				m_ddyPort.ChangeType( WirePortDataType.FLOAT2, false );
				break;
				case TextureType.Texture3D:
				case TextureType.Cube:
				m_uvPort.ChangeType( WirePortDataType.FLOAT3, false );
				m_ddxPort.ChangeType( WirePortDataType.FLOAT3, false );
				m_ddyPort.ChangeType( WirePortDataType.FLOAT3, false );
				break;
			}
		}

		public override void ConfigureInputPorts()
		{
			m_normalPort.Visible = AutoUnpackNormals;

			switch( m_mipMode )
			{
				case MipType.Auto:
				m_lodPort.Visible = false;
				m_ddxPort.Visible = false;
				m_ddyPort.Visible = false;
				break;
				case MipType.MipLevel:
				m_lodPort.Name = "Level";
				m_lodPort.Visible = true;
				m_ddxPort.Visible = false;
				m_ddyPort.Visible = false;
				break;
				case MipType.MipBias:
				m_lodPort.Name = "Bias";
				m_lodPort.Visible = true;
				m_ddxPort.Visible = false;
				m_ddyPort.Visible = false;
				break;
				case MipType.Derivative:
				m_lodPort.Visible = false;
				m_ddxPort.Visible = true;
				m_ddyPort.Visible = true;
				break;
			}

			switch( m_currentType )
			{
				case TextureType.Texture1D:
				m_uvPort.ChangeType( WirePortDataType.FLOAT, false );
				m_ddxPort.ChangeType( WirePortDataType.FLOAT, false );
				m_ddyPort.ChangeType( WirePortDataType.FLOAT, false );
				break;
				case TextureType.ProceduralTexture:
				case TextureType.Texture2D:
				m_uvPort.ChangeType( WirePortDataType.FLOAT2, false );
				m_ddxPort.ChangeType( WirePortDataType.FLOAT2, false );
				m_ddyPort.ChangeType( WirePortDataType.FLOAT2, false );
				break;
				case TextureType.Texture3D:
				case TextureType.Cube:
				m_uvPort.ChangeType( WirePortDataType.FLOAT3, false );
				m_ddxPort.ChangeType( WirePortDataType.FLOAT3, false );
				m_ddyPort.ChangeType( WirePortDataType.FLOAT3, false );
				break;
			}

			m_sizeIsDirty = true;
		}

		public override void ConfigureOutputPorts()
		{
			m_outputPorts[ m_colorPort.PortId + 4 ].Visible = !AutoUnpackNormals;

			if( !AutoUnpackNormals )
			{
				m_colorPort.ChangeProperties( "RGBA", WirePortDataType.COLOR, false );
				m_outputPorts[ m_colorPort.PortId + 1 ].ChangeProperties( "R", WirePortDataType.FLOAT, false );
				m_outputPorts[ m_colorPort.PortId + 2 ].ChangeProperties( "G", WirePortDataType.FLOAT, false );
				m_outputPorts[ m_colorPort.PortId + 3 ].ChangeProperties( "B", WirePortDataType.FLOAT, false );
				m_outputPorts[ m_colorPort.PortId + 4 ].ChangeProperties( "A", WirePortDataType.FLOAT, false );

			}
			else
			{
				m_colorPort.ChangeProperties( "XYZ", WirePortDataType.FLOAT3, false );
				m_outputPorts[ m_colorPort.PortId + 1 ].ChangeProperties( "X", WirePortDataType.FLOAT, false );
				m_outputPorts[ m_colorPort.PortId + 2 ].ChangeProperties( "Y", WirePortDataType.FLOAT, false );
				m_outputPorts[ m_colorPort.PortId + 3 ].ChangeProperties( "Z", WirePortDataType.FLOAT, false );
			}

			m_sizeIsDirty = true;
		}

		public override void OnObjectDropped( UnityEngine.Object obj )
		{
			base.OnObjectDropped( obj );
			ConfigFromObject( obj );
		}

		public override void SetupFromCastObject( UnityEngine.Object obj )
		{
			base.SetupFromCastObject( obj );
			ConfigFromObject( obj );
		}

		void UpdateHeaderColor()
		{
			m_headerColorModifier = ( m_referenceType == TexReferenceType.Object ) ? Color.white : ReferenceHeaderColor;
		}

		public void DrawSamplerOptions()
		{
			m_textureCoordSet = EditorGUILayoutIntPopup( Constants.AvailableUVSetsLabel, m_textureCoordSet, Constants.AvailableUVSetsStr, Constants.AvailableUVSets );

			MipType newMipMode = (MipType)EditorGUILayoutEnumPopup( MipModeStr, m_mipMode );
			if( newMipMode != m_mipMode )
			{
				m_mipMode = newMipMode;
				ConfigureInputPorts();
				ConfigureOutputPorts();
				//ResizeNodeToPreview();
			}

			EditorGUI.BeginChangeCheck();
			m_autoUnpackNormals = EditorGUILayoutToggle( "Unpack Normal Map", m_autoUnpackNormals );
			if( m_autoUnpackNormals && !m_normalPort.IsConnected )
			{
				m_normalPort.FloatInternalData = EditorGUILayoutFloatField( NormalScaleStr, m_normalPort.FloatInternalData );
			}

			if( EditorGUI.EndChangeCheck() )
			{
				ConfigureInputPorts();
				ConfigureOutputPorts();
				//ResizeNodeToPreview();
			}
			if( m_showErrorMessage )
			{
				EditorGUILayout.HelpBox( m_errorMessageTooltip, MessageType.Warning );
			}
		}

		public override void DrawMainPropertyBlock()
		{
			EditorGUI.BeginChangeCheck();
			m_referenceType = (TexReferenceType)EditorGUILayoutPopup( Constants.ReferenceTypeStr, (int)m_referenceType, Constants.ReferenceArrayLabels );
			if( EditorGUI.EndChangeCheck() )
			{
				if( m_referenceType == TexReferenceType.Object )
				{
					UIUtils.RegisterSamplerNode( this );
					UIUtils.RegisterPropertyNode( this );
					if( !m_texPort.IsConnected )
						UIUtils.RegisterTexturePropertyNode( this );

					SetTitleText( m_propertyInspectorName );
					SetAdditonalTitleText( string.Format( Constants.PropertyValueLabel, GetPropertyValStr() ) );
					m_referenceArrayId = -1;
					m_referenceNodeId = -1;
					m_referenceSampler = null;
					m_textureProperty = m_texPort.IsConnected ? m_texPort.GetOutputNode( 0 ) as TexturePropertyNode : this;

				}
				else
				{
					UIUtils.UnregisterSamplerNode( this );
					UIUtils.UnregisterPropertyNode( this );
					if( !m_texPort.IsConnected )
						UIUtils.UnregisterTexturePropertyNode( this );
				}
				UpdateHeaderColor();
			}

			if( m_referenceType == TexReferenceType.Object )
			{
				EditorGUI.BeginChangeCheck();
				if( m_texPort.IsConnected )
				{
					m_drawAttributes = false;
					DrawSamplerOptions();
				}
				else
				{
					m_drawAttributes = true;
					base.DrawMainPropertyBlock();
				}
				if( EditorGUI.EndChangeCheck() )
				{
					OnPropertyNameChanged();
				}
			}
			else
			{
				m_drawAttributes = true;
				string[] arr = UIUtils.SamplerNodeArr();
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

				EditorGUI.BeginChangeCheck();
				m_referenceArrayId = EditorGUILayoutPopup( Constants.AvailableReferenceStr, m_referenceArrayId, arr );
				if( EditorGUI.EndChangeCheck() )
				{
					m_referenceSampler = UIUtils.GetSamplerNode( m_referenceArrayId );
					if( m_referenceSampler != null )
					{
						m_referenceNodeId = m_referenceSampler.UniqueId;
					}
					else
					{
						m_referenceArrayId = -1;
						m_referenceNodeId = -1;
					}
				}
				GUI.enabled = guiEnabledBuffer;

				DrawSamplerOptions();
			}
		}

		public override void OnPropertyNameChanged()
		{
			base.OnPropertyNameChanged();
			UIUtils.UpdateSamplerDataNode( UniqueId, PropertyInspectorName );
		}

		public override void DrawGUIControls( DrawInfo drawInfo )
		{
			base.DrawGUIControls( drawInfo );

			if( m_state != ReferenceState.Self && drawInfo.CurrentEventType == EventType.MouseDown && m_previewRect.Contains( drawInfo.MousePosition ) && drawInfo.LeftMouseButtonPressed )
			{
				UIUtils.FocusOnNode( m_previewTextProp, 1, true );
				Event.current.Use();
			}
		}

		private Rect m_iconPos;

		public override void OnNodeLogicUpdate( DrawInfo drawInfo )
		{
			base.OnNodeLogicUpdate( drawInfo );
			CheckReference();

			if( SoftValidReference )
			{
				m_state = ReferenceState.Instance;
				m_previewTextProp = m_referenceSampler.TextureProperty;
			}
			else if( m_texPort.IsConnected )
			{
				m_state = ReferenceState.Connected;
				m_previewTextProp = TextureProperty;
			}
			else
			{
				m_state = ReferenceState.Self;
			}

			if( m_previewTextProp == null )
				m_previewTextProp = this;
		}

		public override void OnNodeLayout( DrawInfo drawInfo )
		{
			base.OnNodeLayout( drawInfo );

			if( m_drawPreview )
			{
				m_iconPos = m_globalPosition;
				m_iconPos.width = InstanceIconWidth * drawInfo.InvertedZoom;
				m_iconPos.height = InstanceIconHeight * drawInfo.InvertedZoom;

				m_iconPos.y += 10 * drawInfo.InvertedZoom;
				m_iconPos.x += m_globalPosition.width - m_iconPos.width - 5 * drawInfo.InvertedZoom;
			}
		}

		public override void OnNodeRepaint( DrawInfo drawInfo )
		{
			base.OnNodeRepaint( drawInfo );

			if( !m_isVisible )
				return;

			if( drawInfo.CurrentEventType != EventType.Repaint )
				return;

			switch( m_state )
			{
				default:
				case ReferenceState.Self:
				{
					m_drawPreview = false;
					//SetTitleText( PropertyInspectorName /*m_propertyInspectorName*/ );
					//small optimization, string format or concat on every frame generates garbage
					//string tempVal = GetPropertyValStr();
					//if ( !m_previousAdditionalText.Equals( tempVal ) )
					//{
					//	m_previousAdditionalText = tempVal;
					//	m_additionalContent.text = string.Concat( "Value( ", tempVal, " )" );
					//}

					m_drawPicker = true;
				}
				break;
				case ReferenceState.Connected:
				{
					m_drawPreview = true;
					m_drawPicker = false;

					SetTitleText( m_previewTextProp.PropertyInspectorName + " (Input)" );
					m_previousAdditionalText = m_previewTextProp.AdditonalTitleContent.text;
					SetAdditonalTitleText( m_previousAdditionalText );
					// Draw chain lock
					GUI.Label( m_iconPos, string.Empty, UIUtils.GetCustomStyle( CustomStyle.SamplerTextureIcon ) );

					// Draw frame around preview
					GUI.Label( m_previewRect, string.Empty, UIUtils.GetCustomStyle( CustomStyle.SamplerFrame ) );
				}
				break;
				case ReferenceState.Instance:
				{
					m_drawPreview = true;
					m_drawPicker = false;

					//SetTitleText( m_previewTextProp.PropertyInspectorName + Constants.InstancePostfixStr );
					//m_previousAdditionalText = m_previewTextProp.AdditonalTitleContent.text;
					//SetAdditonalTitleText( m_previousAdditionalText );

					SetTitleTextOnCallback( m_previewTextProp.PropertyInspectorName, ( instance, newTitle ) => instance.TitleContent.text = newTitle + Constants.InstancePostfixStr );
					SetAdditonalTitleText( m_previewTextProp.AdditonalTitleContent.text );

					// Draw chain lock
					GUI.Label( m_iconPos, string.Empty, UIUtils.GetCustomStyle( CustomStyle.SamplerTextureIcon ) );

					// Draw frame around preview
					GUI.Label( m_previewRect, string.Empty, UIUtils.GetCustomStyle( CustomStyle.SamplerFrame ) );
				}
				break;
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
				ParentNode newNode = UIUtils.GetSamplerNode( m_referenceArrayId );
				if( newNode == null || newNode.UniqueId != m_referenceNodeId )
				{
					m_referenceSampler = null;
					int count = UIUtils.GetSamplerNodeAmount();
					for( int i = 0; i < count; i++ )
					{
						ParentNode node = UIUtils.GetSamplerNode( i );
						if( node.UniqueId == m_referenceNodeId )
						{
							m_referenceSampler = node as SamplerNode;
							m_referenceArrayId = i;
							break;
						}
					}
				}
				else
				{
					// Set References Options
					AutoCastType newAutoCast = m_referenceSampler.AutocastMode;
					if( newAutoCast != m_autocastMode )
					{
						m_autocastMode = newAutoCast;
						if( m_autocastMode != AutoCastType.Auto )
						{
							ConfigTextureData( m_currentType );
							ConfigureInputPorts();
							ConfigureOutputPorts();
							//ResizeNodeToPreview();
						}
					}
				}
			}

			if( m_referenceSampler == null && m_referenceNodeId > -1 )
			{
				m_referenceNodeId = -1;
				m_referenceArrayId = -1;
			}
		}

		public void SetTitleTextDelay( string newText )
		{
			if( !newText.Equals( m_content.text ) )
			{
				m_content.text = newText;
				BeginDelayedDirtyProperty();
			}
		}

		public void SetAdditonalTitleTextDelay( string newText )
		{
			if( !newText.Equals( m_additionalContent.text ) )
			{
				m_additionalContent.text = newText;
				BeginDelayedDirtyProperty();
			}
		}

		private void DrawTexturePropertyPreview( DrawInfo drawInfo, bool instance )
		{
			if( drawInfo.CurrentEventType != EventType.Repaint )
				return;

			Rect newPos = m_previewRect;

			TexturePropertyNode texProp = null;
			if( instance )
				texProp = m_referenceSampler.TextureProperty;
			else
				texProp = TextureProperty;

			if( texProp == null )
				texProp = this;

			float previewSizeX = PreviewSizeX;
			float previewSizeY = PreviewSizeY;
			newPos.width = previewSizeX * drawInfo.InvertedZoom;
			newPos.height = previewSizeY * drawInfo.InvertedZoom;

			SetTitleText( texProp.PropertyInspectorName + ( instance ? Constants.InstancePostfixStr : " (Input)" ) );
			SetAdditonalTitleText( texProp.AdditonalTitleContent.text );

			if( m_referenceStyle == null )
			{
				m_referenceStyle = UIUtils.GetCustomStyle( CustomStyle.SamplerTextureRef );
			}

			if( m_referenceIconStyle == null || m_referenceIconStyle.normal == null )
			{
				m_referenceIconStyle = UIUtils.GetCustomStyle( CustomStyle.SamplerTextureIcon );
				if( m_referenceIconStyle != null && m_referenceIconStyle.normal != null && m_referenceIconStyle.normal.background != null )
				{
					InstanceIconWidth = m_referenceIconStyle.normal.background.width;
					InstanceIconHeight = m_referenceIconStyle.normal.background.height;
				}
			}

			Rect iconPos = m_globalPosition;
			iconPos.width = InstanceIconWidth * drawInfo.InvertedZoom;
			iconPos.height = InstanceIconHeight * drawInfo.InvertedZoom;

			iconPos.y += 10 * drawInfo.InvertedZoom;
			iconPos.x += m_globalPosition.width - iconPos.width - 5 * drawInfo.InvertedZoom;

			//if ( GUI.Button( newPos, string.Empty, UIUtils.GetCustomStyle( CustomStyle.SamplerTextureRef )/* m_referenceStyle */) ||
			//	GUI.Button( iconPos, string.Empty, m_referenceIconStyle )
			//	)
			//{
			//	UIUtils.FocusOnNode( texProp, 1, true );
			//}

			if( texProp.Value != null )
			{
				DrawPreview( drawInfo, m_previewRect );
				GUI.Label( newPos, string.Empty, UIUtils.GetCustomStyle( CustomStyle.SamplerFrame ) );
				//UIUtils.GetCustomStyle( CustomStyle.SamplerButton ).fontSize = ( int )Mathf.Round( 9 * drawInfo.InvertedZoom );
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if( dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				UIUtils.ShowMessage( m_nodeAttribs.Name + " cannot be used on Master Node Tessellation port" );
				return "(-1)";
			}

			OnPropertyNameChanged();

			ConfigSampler();
			string portProperty = string.Empty;
			if( m_texPort.IsConnected )
				portProperty = m_texPort.GenerateShaderForOutput( ref dataCollector, true );

			if( SoftValidReference )
			{
				OrderIndex = m_referenceSampler.RawOrderIndex;
				if( m_referenceSampler.TexPort.IsConnected )
					portProperty = m_referenceSampler.TexPort.GeneratePortInstructions( ref dataCollector );
			}

			if( m_autoUnpackNormals )
			{
				bool isScaledNormal = false;
				if( m_normalPort.IsConnected )
				{
					isScaledNormal = true;
				}
				else
				{
					if( m_normalPort.FloatInternalData != 1 )
					{
						isScaledNormal = true;
					}
				}

				string scaleValue = isScaledNormal ? m_normalPort.GeneratePortInstructions( ref dataCollector ):"1.0f";
				m_normalMapUnpackMode = TemplateHelperFunctions.CreateUnpackNormalStr( dataCollector, isScaledNormal , scaleValue );

				if( isScaledNormal )
				{
					if( !( dataCollector.IsTemplate && dataCollector.IsSRP ) )
					{
						dataCollector.AddToIncludes( UniqueId, Constants.UnityStandardUtilsLibFuncs );
					}
				}
				
			}
			if( IsObject && ( !m_texPort.IsConnected || portProperty == "0.0" ) )
				base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalVar );

			string valueName = SetFetchedData( ref dataCollector, ignoreLocalVar, outputId, portProperty );
			if( TextureProperty is VirtualTextureObject )
			{
				return valueName;
			}
			else
			{

				return GetOutputColorItem( 0, outputId, valueName );
			}
		}

		public string SampleVirtualTexture( VirtualTextureObject node, string coord )
		{
			string sampler = string.Empty;
			switch( node.Channel )
			{
				default:
				case VirtualChannel.Albedo:
				case VirtualChannel.Base:
				sampler = "VTSampleAlbedo( " + coord + " )";
				break;
				case VirtualChannel.Normal:
				case VirtualChannel.Height:
				case VirtualChannel.Occlusion:
				case VirtualChannel.Displacement:
				sampler = "VTSampleNormal( " + coord + " )";
				break;
				case VirtualChannel.Specular:
				case VirtualChannel.SpecMet:
				case VirtualChannel.Material:
				sampler = "VTSampleSpecular( " + coord + " )";
				break;
			}
			return sampler;
		}

		public string SetFetchedData( ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar, int outputId, string portProperty = null )
		{
			m_precisionString = UIUtils.PrecisionWirePortToCgType( UIUtils.GetFinalPrecision( m_currentPrecisionType ), m_colorPort.DataType );
			string propertyName = CurrentPropertyReference;
			if( !string.IsNullOrEmpty( portProperty ) && portProperty != "0.0" )
			{
				propertyName = portProperty;
			}

			string mipType = "";
			if( m_lodPort.IsConnected )
			{
				switch( m_mipMode )
				{
					case MipType.Auto:
					break;
					case MipType.MipLevel:
					mipType = "lod";
					break;
					case MipType.MipBias:
					mipType = "bias";
					break;
					case MipType.Derivative:
					break;
				}
			}

			if( ignoreLocalVar )
			{
				if( TextureProperty is VirtualTextureObject )
					Debug.Log( "TODO" );

				if( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
				{
					mipType = "lod";
				}

				string samplerValue = m_samplerType + mipType + "( " + propertyName + ", " + GetUVCoords( ref dataCollector, ignoreLocalVar, portProperty ) + " )";
				AddNormalMapTag( ref samplerValue );
				return samplerValue;
			}

			VirtualTextureObject vtex = ( TextureProperty as VirtualTextureObject );

			if( vtex != null )
			{
				string atPathname = AssetDatabase.GUIDToAssetPath( Constants.ATSharedLibGUID );
				if( string.IsNullOrEmpty( atPathname ) )
				{
					UIUtils.ShowMessage( "Could not find Amplify Texture on your project folder. Please install it and re-compile the shader.", MessageSeverity.Error );
				}
				else
				{
					//Need to see if the asset really exists because AssetDatabase.GUIDToAssetPath() can return a valid path if
					// the asset was previously imported and deleted after that
					UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>( atPathname );
					if( obj == null )
					{
						UIUtils.ShowMessage( "Could not find Amplify Texture on your project folder. Please install it and re-compile the shader.", MessageSeverity.Error );
					}
					else
					{
						if( m_colorPort.IsLocalValue( dataCollector.PortCategory ) )
							return m_colorPort.LocalValue( dataCollector.PortCategory );

						//string remapPortR = ".r";
						//string remapPortG = ".g";
						//string remapPortB = ".b";
						//string remapPortA = ".a";

						//if ( vtex.Channel == VirtualChannel.Occlusion )
						//{
						//	remapPortR = ".r"; remapPortG = ".r"; remapPortB = ".r"; remapPortA = ".r";
						//}
						//else if ( vtex.Channel == VirtualChannel.SpecMet && ( ContainerGraph.CurrentStandardSurface != null && ContainerGraph.CurrentStandardSurface.CurrentLightingModel == StandardShaderLightModel.Standard ) )
						//{
						//	remapPortR = ".r"; remapPortG = ".r"; remapPortB = ".r";
						//}
						//else if ( vtex.Channel == VirtualChannel.Height || vtex.Channel == VirtualChannel.Displacement )
						//{
						//	remapPortR = ".b"; remapPortG = ".b"; remapPortB = ".b"; remapPortA = ".b";
						//}

						dataCollector.AddToPragmas( UniqueId, IOUtils.VirtualTexturePragmaHeader );
						dataCollector.AddToIncludes( UniqueId, atPathname );

						string lodBias = string.Empty;
						if( dataCollector.IsFragmentCategory )
						{
							lodBias = m_mipMode == MipType.MipLevel ? "Lod" : m_mipMode == MipType.MipBias ? "Bias" : "";
						}
						else
						{
							lodBias = "Lod";
						}

						int virtualCoordId = dataCollector.GetVirtualCoordinatesId( UniqueId, GetVirtualUVCoords( ref dataCollector, ignoreLocalVar, portProperty ), lodBias );
						string virtualSampler = SampleVirtualTexture( vtex, Constants.VirtualCoordNameStr + virtualCoordId );
						string virtualVariable = dataCollector.AddVirtualLocalVariable( UniqueId, "virtualNode" + OutputId, virtualSampler );

						if( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
							dataCollector.AddToVertexLocalVariables( UniqueId, "float4 " + virtualVariable + " = " + virtualSampler + ";" );
						else
							dataCollector.AddToLocalVariables( UniqueId, "float4 " + virtualVariable + " = " + virtualSampler + ";" );

						AddNormalMapTag( ref virtualVariable );

						switch( vtex.Channel )
						{
							default:
							case VirtualChannel.Albedo:
							case VirtualChannel.Base:
							case VirtualChannel.Normal:
							case VirtualChannel.Specular:
							case VirtualChannel.SpecMet:
							case VirtualChannel.Material:
							virtualVariable = GetOutputColorItem( 0, outputId, virtualVariable );
							break;
							case VirtualChannel.Displacement:
							case VirtualChannel.Height:
							{
								if( outputId > 0 )
									virtualVariable += ".b";
								else
								{
									dataCollector.AddLocalVariable( UniqueId, "float4 virtual_cast_" + OutputId + " = " + virtualVariable + ".b;" );
									virtualVariable = "virtual_cast_" + OutputId;
								}
								//virtualVariable = UIUtils.CastPortType( dataCollector.PortCategory, m_currentPrecisionType, new NodeCastInfo( UniqueId, outputId ), virtualVariable, WirePortDataType.FLOAT, WirePortDataType.FLOAT4, virtualVariable );
							}
							break;
							case VirtualChannel.Occlusion:
							{
								if( outputId > 0 )
									virtualVariable += ".r";
								else
								{
									dataCollector.AddLocalVariable( UniqueId, "float4 virtual_cast_" + OutputId + " = " + virtualVariable + ".r;" );
									virtualVariable = "virtual_cast_" + OutputId;
								}
							}
							break;
						}

						//for ( int i = 0; i < m_outputPorts.Count; i++ )
						//{
						//	if ( m_outputPorts[ i ].IsConnected )
						//	{

						//		//TODO: make the sampler not generate local variables at all times
						//		m_textureFetchedValue = "virtualNode" + OutputId;
						//		m_isTextureFetched = true;

						//		//dataCollector.AddToLocalVariables( m_uniqueId, m_precisionString + " " + m_textureFetchedValue + " = " + virtualSampler + ";" );
						//		if ( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
						//			dataCollector.AddToVertexLocalVariables( UniqueId, m_precisionString + " " + m_textureFetchedValue + " = " + virtualSampler + ";" );
						//		else
						//			dataCollector.AddToLocalVariables( UniqueId, m_precisionString + " " + m_textureFetchedValue + " = " + virtualSampler + ";" );

						//		m_colorPort.SetLocalValue( m_textureFetchedValue );
						//		m_outputPorts[ m_colorPort.PortId + 1 ].SetLocalValue( m_textureFetchedValue + remapPortR );
						//		m_outputPorts[ m_colorPort.PortId + 2 ].SetLocalValue( m_textureFetchedValue + remapPortG );
						//		m_outputPorts[ m_colorPort.PortId + 3 ].SetLocalValue( m_textureFetchedValue + remapPortB );
						//		m_outputPorts[ m_colorPort.PortId + 4 ].SetLocalValue( m_textureFetchedValue + remapPortA );
						//		return m_textureFetchedValue;
						//	}
						//}

						return virtualVariable;
					}
				}
			}

			if( m_colorPort.IsLocalValue( dataCollector.PortCategory ) )
				return m_colorPort.LocalValue( dataCollector.PortCategory );

			if( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				mipType = "lod";
			}

			string samplerOp = m_samplerType + mipType + "( " + propertyName + ", " + GetUVCoords( ref dataCollector, ignoreLocalVar, portProperty ) + " )";
			AddNormalMapTag( ref samplerOp );

			int connectedPorts = 0;
			for( int i = 0; i < m_outputPorts.Count; i++ )
			{
				if( m_outputPorts[ i ].IsConnected )
				{
					connectedPorts += 1;
					if( connectedPorts > 1 || m_outputPorts[ i ].ConnectionCount > 1 )
					{
						// Create common local var and mark as fetched
						string textureFetchedValue = m_samplerType + "Node" + OutputId;

						if( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
							dataCollector.AddToVertexLocalVariables( UniqueId, m_precisionString + " " + textureFetchedValue + " = " + samplerOp + ";" );
						else
							dataCollector.AddToLocalVariables( UniqueId, m_precisionString + " " + textureFetchedValue + " = " + samplerOp + ";" );


						m_colorPort.SetLocalValue( textureFetchedValue, dataCollector.PortCategory );
						m_outputPorts[ m_colorPort.PortId + 1 ].SetLocalValue( textureFetchedValue + ".r", dataCollector.PortCategory );
						m_outputPorts[ m_colorPort.PortId + 2 ].SetLocalValue( textureFetchedValue + ".g", dataCollector.PortCategory );
						m_outputPorts[ m_colorPort.PortId + 3 ].SetLocalValue( textureFetchedValue + ".b", dataCollector.PortCategory );
						m_outputPorts[ m_colorPort.PortId + 4 ].SetLocalValue( textureFetchedValue + ".a", dataCollector.PortCategory );
						return textureFetchedValue;
					}
				}
			}
			return samplerOp;
		}

		private void AddNormalMapTag( ref string value )
		{
			if( m_autoUnpackNormals )
			{
				value = string.Format( m_normalMapUnpackMode, value );
			}
		}

		public override void ReadOutputDataFromString( ref string[] nodeParams )
		{
			base.ReadOutputDataFromString( ref nodeParams );
			ConfigureOutputPorts();
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			string defaultTextureGUID = GetCurrentParam( ref nodeParams );
			if( UIUtils.CurrentShaderVersion() > 14101 )
			{
				m_defaultValue = AssetDatabase.LoadAssetAtPath<Texture>( AssetDatabase.GUIDToAssetPath( defaultTextureGUID ) );
				string materialTextureGUID = GetCurrentParam( ref nodeParams );
				m_materialValue = AssetDatabase.LoadAssetAtPath<Texture>( AssetDatabase.GUIDToAssetPath( materialTextureGUID ) );
			}
			else
			{
				m_defaultValue = AssetDatabase.LoadAssetAtPath<Texture>( defaultTextureGUID );
			}
			m_useSemantics = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			m_textureCoordSet = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_isNormalMap = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			m_defaultTextureValue = (TexturePropertyValues)Enum.Parse( typeof( TexturePropertyValues ), GetCurrentParam( ref nodeParams ) );
			m_autocastMode = (AutoCastType)Enum.Parse( typeof( AutoCastType ), GetCurrentParam( ref nodeParams ) );
			m_autoUnpackNormals = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );

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
					UIUtils.UnregisterSamplerNode( this );
					UIUtils.UnregisterPropertyNode( this );
				}
				UpdateHeaderColor();
			}
			if( UIUtils.CurrentShaderVersion() > 2406 )
				m_mipMode = (MipType)Enum.Parse( typeof( MipType ), GetCurrentParam( ref nodeParams ) );


			if( UIUtils.CurrentShaderVersion() > 3201 )
				m_currentType = (TextureType)Enum.Parse( typeof( TextureType ), GetCurrentParam( ref nodeParams ) );

			if( m_defaultValue == null )
			{
				ConfigureInputPorts();
				ConfigureOutputPorts();
				//ResizeNodeToPreview();
			}
			else
			{
				if( m_materialValue == null )
				{
					ConfigFromObject( m_defaultValue, false, false );
				}
				else
				{
					CheckTextureImporter( false, false );
				}
				ConfigureInputPorts();
				ConfigureOutputPorts();
			}
		}

		public override void RefreshExternalReferences()
		{
			base.RefreshExternalReferences();
			ForceInputPortsChange();

			EditorGUI.BeginChangeCheck();
			if( m_referenceType == TexReferenceType.Instance )
			{
				if( UIUtils.CurrentShaderVersion() > 22 )
				{
					m_referenceSampler = UIUtils.GetNode( m_referenceNodeId ) as SamplerNode;
					m_referenceArrayId = UIUtils.GetSamplerNodeRegisterId( m_referenceNodeId );
				}
				else
				{
					m_referenceSampler = UIUtils.GetSamplerNode( m_referenceArrayId );
					if( m_referenceSampler != null )
					{
						m_referenceNodeId = m_referenceSampler.UniqueId;
					}
				}
			}

			if( EditorGUI.EndChangeCheck() )
			{
				OnPropertyNameChanged();
			}
		}

		public override void ReadAdditionalData( ref string[] nodeParams ) { }
		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( m_defaultValue != null ) ? AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath( m_defaultValue ) ) : Constants.NoStringValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( m_materialValue != null ) ? AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath( m_materialValue ) ) : Constants.NoStringValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_useSemantics.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_textureCoordSet.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_isNormalMap.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_defaultTextureValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_autocastMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_autoUnpackNormals );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_referenceType );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( ( m_referenceSampler != null ) ? m_referenceSampler.UniqueId : -1 ) );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_mipMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_currentType );
		}

		public override void WriteAdditionalToString( ref string nodeInfo, ref string connectionsInfo ) { }

		public string GetVirtualUVCoords( ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar, string portProperty )
		{
			string bias = "";
			if( !dataCollector.IsFragmentCategory || m_mipMode == MipType.MipBias || m_mipMode == MipType.MipLevel )
			{
				string lodLevel = m_lodPort.GeneratePortInstructions( ref dataCollector );
				bias += ", " + lodLevel;
			}

			if( m_uvPort.IsConnected )
			{
				string uvs = m_uvPort.GeneratePortInstructions( ref dataCollector );
				return uvs + bias;
			}
			else
			{
				string propertyName = CurrentPropertyReference;
				if( !string.IsNullOrEmpty( portProperty ) )
				{
					propertyName = portProperty;
				}
				string uvChannelName = IOUtils.GetUVChannelName( propertyName, m_textureCoordSet );


				string uvCoord = string.Empty;
				if( dataCollector.IsTemplate )
				{
					string uvName = string.Empty;
					if( dataCollector.TemplateDataCollectorInstance.HasUV( m_textureCoordSet ) )
					{
						uvName = dataCollector.TemplateDataCollectorInstance.GetUVName( m_textureCoordSet, m_uvPort.DataType );
					}
					else
					{
						uvName = dataCollector.TemplateDataCollectorInstance.RegisterUV( m_textureCoordSet, m_uvPort.DataType );
					}

					string attr = GetPropertyValue();

					if( attr.IndexOf( "[NoScaleOffset]" ) > -1 )
					{
						dataCollector.AddLocalVariable( UniqueId, PrecisionType.Float, WirePortDataType.FLOAT2, uvChannelName, uvName );
					}
					else
					{
						dataCollector.AddToUniforms( UniqueId, "uniform float4 " + propertyName + "_ST;" );
						dataCollector.AddLocalVariable( UniqueId, PrecisionType.Float, WirePortDataType.FLOAT2, uvChannelName, uvName + " * " + propertyName + "_ST.xy + " + propertyName + "_ST.zw" );
					}
					uvCoord = uvChannelName;
				}
				else
				{
					if( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
					{
						uvCoord = Constants.VertexShaderInputStr + ".texcoord";
						if( m_textureCoordSet > 0 )
						{
							uvCoord += m_textureCoordSet.ToString();
						}
					}
					else
					{
						propertyName = CurrentPropertyReference;
						if( !string.IsNullOrEmpty( portProperty ) && portProperty != "0.0" )
						{
							propertyName = portProperty;
						}
						uvChannelName = IOUtils.GetUVChannelName( propertyName, m_textureCoordSet );

						string dummyPropUV = "_texcoord" + ( m_textureCoordSet > 0 ? ( m_textureCoordSet + 1 ).ToString() : "" );
						string dummyUV = "uv" + ( m_textureCoordSet > 0 ? ( m_textureCoordSet + 1 ).ToString() : "" ) + dummyPropUV;

						dataCollector.AddToProperties( UniqueId, "[HideInInspector] " + dummyPropUV + "( \"\", 2D ) = \"white\" {}", 100 );
						dataCollector.AddToInput( UniqueId, dummyUV, WirePortDataType.FLOAT2 );

						string attr = GetPropertyValue();

						if( attr.IndexOf( "[NoScaleOffset]" ) > -1 )
						{
							dataCollector.AddToLocalVariables( UniqueId, PrecisionType.Float, WirePortDataType.FLOAT2, uvChannelName, Constants.InputVarStr + "." + dummyUV );
						}
						else
						{
							dataCollector.AddToUniforms( UniqueId, "uniform float4 " + propertyName + "_ST;" );
							dataCollector.AddToLocalVariables( UniqueId, PrecisionType.Float, WirePortDataType.FLOAT2, uvChannelName, Constants.InputVarStr + "." + dummyUV + " * " + propertyName + "_ST.xy + " + propertyName + "_ST.zw" );
						}
						uvCoord = uvChannelName;
					}
				}
				return uvCoord + bias;
			}
		}

		public string GetUVCoords( ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar, string portProperty )
		{
			bool isVertex = ( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation );

			// make sure the final result is always a float4 with empty 0's in the middle
			string uvAppendix = ", ";
			int coordSize = 3;
			if( m_uvPort.DataType == WirePortDataType.FLOAT2 )
			{
				uvAppendix = ", 0, ";
				coordSize = 2;
			}
			else if( m_uvPort.DataType == WirePortDataType.FLOAT )
			{
				uvAppendix = ", 0, 0, ";
				coordSize = 1;
			}

			string uvs = m_uvPort.GeneratePortInstructions( ref dataCollector );

			// generate automatic UVs if not connected
			if( !m_uvPort.IsConnected )
			{
				string propertyName = CurrentPropertyReference;

				// check for references
				if( !string.IsNullOrEmpty( portProperty ) && portProperty != "0.0" )
					propertyName = portProperty;

				int coordSet = ( ( m_textureCoordSet < 0 ) ? 0 : m_textureCoordSet );
				string uvName = IOUtils.GetUVChannelName( propertyName, coordSet );
				string dummyPropUV = "_tex" + ( coordSize != 2 ? "" + coordSize : "" ) + "coord" + ( coordSet > 0 ? ( coordSet + 1 ).ToString() : "" );
				string dummyUV = "uv" + ( coordSet > 0 ? ( coordSet + 1 ).ToString() : "" ) + dummyPropUV;

				string attr = GetPropertyValue();
				bool scaleOffset = true;
				if( attr.IndexOf( "[NoScaleOffset]" ) > -1 )
					scaleOffset = false;

				if( scaleOffset )
					dataCollector.AddToUniforms( UniqueId, "float4", propertyName + "_ST" );

				dataCollector.AddToProperties( UniqueId, "[HideInInspector] " + dummyPropUV + "( \"\", 2D ) = \"white\" {}", 9999 );

				string coordInput = string.Empty;
				if( isVertex )
				{
					coordInput = Constants.VertexShaderInputStr + ".texcoord";
					if( coordSet > 0 )
						coordInput += coordSet.ToString();
				}
				else
				{
					coordInput = Constants.InputVarStr + "." + dummyUV;
					dataCollector.AddToInput( UniqueId, dummyUV, m_uvPort.DataType );
				}

				if( dataCollector.MasterNodeCategory == AvailableShaderTypes.Template )
				{
					if( dataCollector.TemplateDataCollectorInstance.HasUV( m_textureCoordSet ) )
						coordInput = dataCollector.TemplateDataCollectorInstance.GetUVName( m_textureCoordSet, m_uvPort.DataType );
					else
						coordInput = dataCollector.TemplateDataCollectorInstance.RegisterUV( m_textureCoordSet, m_uvPort.DataType );
				}

				if( !scaleOffset )
					uvName += OutputId;

				if( coordSize > 2 )
				{
					uvName += coordSize;
					dataCollector.UsingHigherSizeTexcoords = true;
					dataCollector.AddLocalVariable( UniqueId, "float" + coordSize + " " + uvName + " = " + coordInput + ";" );
					if( scaleOffset )
						dataCollector.AddLocalVariable( UniqueId, uvName + ".xy = " + coordInput + ".xy * " + propertyName + "_ST.xy + " + propertyName + "_ST.zw;" );
				}
				else
				{
					if( coordSize == 1 )
						uvName += coordSize;

					if( scaleOffset )
						dataCollector.AddLocalVariable( UniqueId, PrecisionType.Float, m_uvPort.DataType, uvName, coordInput + " * " + propertyName + "_ST.xy + " + propertyName + "_ST.zw" );
					else
						dataCollector.AddLocalVariable( UniqueId, PrecisionType.Float, m_uvPort.DataType, uvName, coordInput );
				}

				uvs = uvName;
			}

			if( isVertex )
			{
				string lodLevel = m_lodPort.GeneratePortInstructions( ref dataCollector );

				return UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, WirePortDataType.FLOAT4 ) + "( " + uvs + uvAppendix + lodLevel + ")";
			}
			else
			{
				if( ( m_mipMode == MipType.MipLevel || m_mipMode == MipType.MipBias ) && m_lodPort.IsConnected )
				{
					string lodLevel = m_lodPort.GeneratePortInstructions( ref dataCollector );

					return UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, WirePortDataType.FLOAT4 ) + "( " + uvs + uvAppendix + lodLevel + ")";
				}
				else if( m_mipMode == MipType.Derivative )
				{
					string ddx = m_ddxPort.GeneratePortInstructions( ref dataCollector );
					string ddy = m_ddyPort.GeneratePortInstructions( ref dataCollector );
					return uvs + ", " + ddx + ", " + ddy;
				}
				else
				{
					return uvs;
				}
			}
		}

		public override int VersionConvertInputPortId( int portId )
		{
			int newPort = portId;
			//change normal scale port to last
			if( UIUtils.CurrentShaderVersion() < 2407 )
			{
				if( portId == 1 )
					newPort = 4;
			}

			if( UIUtils.CurrentShaderVersion() < 2408 )
			{
				newPort = newPort + 1;
			}

			return newPort;
		}

		public override void Destroy()
		{
			base.Destroy();
			m_defaultValue = null;
			m_materialValue = null;
			m_referenceSampler = null;
			m_referenceStyle = null;
			m_referenceContent = null;
			m_texPort = null;
			m_uvPort = null;
			m_lodPort = null;
			m_ddxPort = null;
			m_ddyPort = null;
			m_normalPort = null;
			m_colorPort = null;

			if( m_referenceType == TexReferenceType.Object )
			{
				UIUtils.UnregisterSamplerNode( this );
				UIUtils.UnregisterPropertyNode( this );
			}
		}

		public override string GetPropertyValStr()
		{
			return m_materialMode ? ( m_materialValue != null ? m_materialValue.name : IOUtils.NO_TEXTURES ) : ( m_defaultValue != null ? m_defaultValue.name : IOUtils.NO_TEXTURES );
		}

		public TexturePropertyNode TextureProperty
		{
			get
			{
				if( m_referenceSampler != null )
				{
					m_textureProperty = m_referenceSampler as TexturePropertyNode;
				}
				else if( m_texPort.IsConnected )
				{
					m_textureProperty = m_texPort.GetOutputNode( 0 ) as TexturePropertyNode;
				}

				if( m_textureProperty == null )
					return this;

				return m_textureProperty;
			}
		}

		public override string GetPropertyValue()
		{
			if( SoftValidReference )
			{
				if( m_referenceSampler.TexPort.IsConnected )
				{
					return string.Empty;
				}
				else
				{
					return m_referenceSampler.TextureProperty.GetPropertyValue();
				}
			}
			else
			if( m_texPort.IsConnected && ( m_texPort.GetOutputNode( 0 ) as TexturePropertyNode ) != null )
			{
				return TextureProperty.GetPropertyValue();
			}

			switch( m_currentType )
			{
				case TextureType.Texture1D:
				{
					return PropertyAttributes + GetTexture1DPropertyValue();
				}
				case TextureType.ProceduralTexture:
				case TextureType.Texture2D:
				{
					return PropertyAttributes + GetTexture2DPropertyValue();
				}
				case TextureType.Texture3D:
				{
					return PropertyAttributes + GetTexture3DPropertyValue();
				}
				case TextureType.Cube:
				{
					return PropertyAttributes + GetCubePropertyValue();
				}
			}
			return string.Empty;
		}

		public override string GetUniformValue()
		{

			if( SoftValidReference )
			{
				if( m_referenceSampler.TexPort.IsConnected )
					return string.Empty;
				else
					return m_referenceSampler.TextureProperty.GetUniformValue();
			}
			else if( m_texPort.IsConnected && ( m_texPort.GetOutputNode( 0 ) as TexturePropertyNode ) != null )
			{
				return TextureProperty.GetUniformValue();
			}

			return base.GetUniformValue();
		}

		public override bool GetUniformData( out string dataType, out string dataName )
		{
			if( SoftValidReference )
			{
				if( m_referenceSampler.TexPort.IsConnected )
				{
					base.GetUniformData( out dataType, out dataName );
					return false;
				}
				else
					return m_referenceSampler.TextureProperty.GetUniformData( out dataType, out dataName );
			}
			else if( m_texPort.IsConnected && ( m_texPort.GetOutputNode( 0 ) as TexturePropertyNode ) != null )
			{
				return TextureProperty.GetUniformData( out dataType, out dataName );

			}

			return base.GetUniformData( out dataType, out dataName );
		}

		public string UVCoordsName { get { return Constants.InputVarStr + "." + IOUtils.GetUVChannelName( CurrentPropertyReference, m_textureCoordSet ); } }

		public override string CurrentPropertyReference
		{
			get
			{
				string propertyName = string.Empty;
				if( m_referenceType == TexReferenceType.Instance && m_referenceArrayId > -1 )
				{
					SamplerNode node = UIUtils.GetSamplerNode( m_referenceArrayId );
					propertyName = ( node != null ) ? node.TextureProperty.PropertyName : PropertyName;
				}
				else if( m_texPort.IsConnected && ( m_texPort.GetOutputNode( 0 ) as TexturePropertyNode ) != null )
				{
					propertyName = TextureProperty.PropertyName;
				}
				else
				{
					propertyName = PropertyName;
				}
				return propertyName;
			}
		}

		public bool SoftValidReference
		{
			get
			{
				if( m_referenceType == TexReferenceType.Instance && m_referenceArrayId > -1 )
				{
					m_referenceSampler = UIUtils.GetSamplerNode( m_referenceArrayId );

					m_texPort.Locked = true;

					if( m_referenceContent == null )
						m_referenceContent = new GUIContent();


					if( m_referenceSampler != null )
					{
						m_referenceContent.image = m_referenceSampler.Value;
						if( m_referenceWidth != m_referenceSampler.Position.width )
						{
							m_referenceWidth = m_referenceSampler.Position.width;
							m_sizeIsDirty = true;
						}
					}
					else
					{
						m_referenceArrayId = -1;
						m_referenceWidth = -1;
					}

					return m_referenceSampler != null;
				}
				m_texPort.Locked = false;
				return false;
			}
		}
		public override void ForceUpdateFromMaterial( Material material )
		{
			if( UIUtils.IsProperty( m_currentParameterType ) && material.HasProperty( PropertyName ) )
			{
				m_materialValue = material.GetTexture( PropertyName );
				CheckTextureImporter( true );
			}

		}
		public override void SetContainerGraph( ParentGraph newgraph )
		{
			base.SetContainerGraph( newgraph );
			m_textureProperty = m_texPort.GetOutputNode( 0 ) as TexturePropertyNode;
			if( m_textureProperty == null )
			{
				m_textureProperty = this;
			}
		}

		public bool AutoUnpackNormals
		{
			get { return m_autoUnpackNormals; }
			set
			{
				if( value != m_autoUnpackNormals )
				{
					m_autoUnpackNormals = value;
					if( !UIUtils.IsLoading )
					{
						m_defaultTextureValue = value ? TexturePropertyValues.bump : TexturePropertyValues.white;
					}
				}
			}
		}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			base.PropagateNodeData( nodeData, ref dataCollector );
			if( dataCollector.IsTemplate && !m_texPort.IsConnected)
			{
				dataCollector.TemplateDataCollectorInstance.SetUVUsage( m_textureCoordSet, m_uvPort.DataType );
			}
		}

		private InputPort TexPort { get { return m_texPort; } }
		public bool IsObject { get { return ( m_referenceType == TexReferenceType.Object ) || ( m_referenceSampler == null ); } }
	}
}
