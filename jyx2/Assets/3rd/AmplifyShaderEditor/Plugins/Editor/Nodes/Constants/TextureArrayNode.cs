// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Texture Array", "Textures", "Texture Array fetches a texture from a texture2DArray asset file given a index value", KeyCode.None, true, 0, int.MaxValue, typeof( Texture2DArray ) )]
	public class TextureArrayNode : PropertyNode
	{
		[SerializeField]
		private Texture2DArray m_defaultTextureArray;

		[SerializeField]
		private Texture2DArray m_materialTextureArray;

		[SerializeField]
		private TexReferenceType m_referenceType = TexReferenceType.Object;

		[SerializeField]
		private int m_uvSet = 0;

		[SerializeField]
		private MipType m_mipMode = MipType.Auto;

		private readonly string[] m_mipOptions = { "Auto", "Mip Level", "Derivative" };

		private TextureArrayNode m_referenceSampler = null;

		[SerializeField]
		private int m_referenceArrayId = -1;

		[SerializeField]
		private int m_referenceNodeId = -1;

		[SerializeField]
		private bool m_autoUnpackNormals = false;

		private InputPort m_texPort;
		private InputPort m_uvPort;
		private InputPort m_indexPort;
		private InputPort m_lodPort;
		private InputPort m_normalPort;
		private InputPort m_ddxPort;
		private InputPort m_ddyPort;

		private OutputPort m_colorPort;

		private const string AutoUnpackNormalsStr = "Normal";
		private const string NormalScaleStr = "Scale";

		private string m_labelText = "None (Texture2DArray)";

		private readonly Color ReferenceHeaderColor = new Color( 2.66f, 1.02f, 0.6f, 1.0f );

		private int m_cachedUvsId = -1;
		private int m_cachedSamplerId = -1;
		private int m_texConnectedId = -1;
		private int m_cachedUnpackId = -1;
		private int m_cachedLodId = -1;

		private Rect m_iconPos;
		private bool m_isEditingPicker;

		private bool m_linearTexture;
		protected bool m_drawPicker;

		private ReferenceState m_state = ReferenceState.Self;
		private ParentNode m_previewTextProp = null;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputColorPorts( "RGBA" );
			m_colorPort = m_outputPorts[ 0 ];
			AddInputPort( WirePortDataType.SAMPLER2D, false, "Tex", -1, MasterNodePortCategory.Fragment, 6 );
			AddInputPort( WirePortDataType.FLOAT2, false, "UV", -1, MasterNodePortCategory.Fragment, 0 );
			AddInputPort( WirePortDataType.FLOAT, false, "Index", -1, MasterNodePortCategory.Fragment, 1 );
			AddInputPort( WirePortDataType.FLOAT, false, "Level", -1, MasterNodePortCategory.Fragment, 2 );
			AddInputPort( WirePortDataType.FLOAT, false, NormalScaleStr, -1, MasterNodePortCategory.Fragment, 3 );
			AddInputPort( WirePortDataType.FLOAT2, false, "DDX", -1, MasterNodePortCategory.Fragment, 4 );
			AddInputPort( WirePortDataType.FLOAT2, false, "DDY", -1, MasterNodePortCategory.Fragment, 5 );
			m_inputPorts[ 2 ].AutoDrawInternalData = true;
			m_texPort = m_inputPorts[ 0 ];
			m_uvPort = m_inputPorts[ 1 ];
			m_indexPort = m_inputPorts[ 2 ];
			m_lodPort = m_inputPorts[ 3 ];
			m_lodPort.Visible = false;
			m_normalPort = m_inputPorts[ 4 ];
			m_normalPort.Visible = m_autoUnpackNormals;
			m_normalPort.FloatInternalData = 1.0f;
			m_ddxPort = m_inputPorts[ 5 ];
			m_ddxPort.Visible = false;
			m_ddyPort = m_inputPorts[ 6 ];
			m_ddyPort.Visible = false;
			m_insideSize.Set( 128, 128 + 5 );
			m_drawPrecisionUI = false;
			m_currentParameterType = PropertyType.Property;
			m_freeType = false;
			m_showPreview = true;
			m_drawPreviewExpander = false;
			m_drawPreview = false;
			m_drawPicker = true;
			m_customPrefix = "Texture Array ";
			m_selectedLocation = PreviewLocation.TopCenter;
			m_precisionString = UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType );
			m_previewShaderGUID = "2e6d093df2d289f47b827b36efb31a81";
			m_showAutoRegisterUI = false;
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if( m_cachedUvsId == -1 )
				m_cachedUvsId = Shader.PropertyToID( "_CustomUVs" );

			if( m_cachedSamplerId == -1 )
				m_cachedSamplerId = Shader.PropertyToID( "_Sampler" );

			if( m_texConnectedId == -1 )
				m_texConnectedId = Shader.PropertyToID( "_TexConnected" );

			if( m_cachedUnpackId == -1 )
				m_cachedUnpackId = Shader.PropertyToID( "_Unpack" );

			if( m_cachedLodId == -1 )
				m_cachedLodId = Shader.PropertyToID( "_LodType" );

			PreviewMaterial.SetFloat( m_cachedLodId, ( m_mipMode == MipType.MipLevel ? 1 : 0 ) );
			PreviewMaterial.SetFloat( m_cachedUnpackId, m_autoUnpackNormals ? 1 : 0 );
			if( m_referenceType == TexReferenceType.Instance && m_referenceSampler != null )
			{
				if( (ParentNode)m_referenceSampler != m_referenceSampler.PreviewTextProp )
				{
					PreviewMaterial.SetInt( m_texConnectedId, 1 );
					PreviewMaterial.SetTexture( "_G", m_referenceSampler.PreviewTextProp.PreviewTexture );
				}
				else
				{
					PreviewMaterial.SetInt( m_texConnectedId, 0 );
					PreviewMaterial.SetTexture( m_cachedSamplerId, m_referenceSampler.TextureArray );
				}
			}
			else if( m_texPort.IsConnected )
			{
				PreviewMaterial.SetInt( m_texConnectedId, 1 );
			}
			else
			{
				PreviewMaterial.SetInt( m_texConnectedId, 0 );
				PreviewMaterial.SetTexture( m_cachedSamplerId, TextureArray );
			}
			PreviewMaterial.SetFloat( m_cachedUvsId, ( m_uvPort.IsConnected ? 1 : 0 ) );
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			if( m_referenceType == TexReferenceType.Object )
			{
				UIUtils.RegisterTextureArrayNode( this );
				UIUtils.RegisterPropertyNode( this );
			}
		}

		new void ShowDefaults()
		{
			m_uvSet = EditorGUILayoutIntPopup( Constants.AvailableUVSetsLabel, m_uvSet, Constants.AvailableUVSetsStr, Constants.AvailableUVSets );

			MipType newMipMode = (MipType)EditorGUILayoutPopup( "Mip Mode", (int)m_mipMode, m_mipOptions );
			if( newMipMode != m_mipMode )
			{
				m_mipMode = newMipMode;
			}

			switch( m_mipMode )
			{
				case MipType.Auto:
				m_lodPort.Visible = false;
				m_ddxPort.Visible = false;
				m_ddyPort.Visible = false;
				break;
				case MipType.MipLevel:
				m_lodPort.Visible = true;
				m_ddxPort.Visible = false;
				m_ddyPort.Visible = false;
				break;
				case MipType.MipBias:
				case MipType.Derivative:
				m_ddxPort.Visible = true;
				m_ddyPort.Visible = true;
				m_lodPort.Visible = false;
				break;
			}

			if( m_ddxPort.Visible )
			{
				EditorGUILayout.HelpBox( "Warning: Derivative Mip Mode only works on some platforms (D3D11 XBOXONE GLES3 GLCORE)", MessageType.Warning );
			}

			if( !m_lodPort.IsConnected && m_lodPort.Visible )
			{
				m_lodPort.FloatInternalData = EditorGUILayoutFloatField( "Mip Level", m_lodPort.FloatInternalData );
			}

			if( !m_indexPort.IsConnected )
			{
				m_indexPort.FloatInternalData = EditorGUILayoutFloatField( "Index", m_indexPort.FloatInternalData );
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
					UIUtils.RegisterTextureArrayNode( this );
					UIUtils.RegisterPropertyNode( this );

					SetTitleText( m_propertyInspectorName );
					SetAdditonalTitleText( string.Format( Constants.PropertyValueLabel, GetPropertyValStr() ) );
					m_referenceArrayId = -1;
					m_referenceNodeId = -1;
					m_referenceSampler = null;
				}
				else
				{
					UIUtils.UnregisterTextureArrayNode( this );
					UIUtils.UnregisterPropertyNode( this );
				}
				UpdateHeaderColor();
			}

			if( m_referenceType == TexReferenceType.Object )
			{
				EditorGUI.BeginChangeCheck();
				base.DrawMainPropertyBlock();
				if( EditorGUI.EndChangeCheck() )
				{
					OnPropertyNameChanged();
				}
			}
			else
			{
				string[] arr = UIUtils.TextureArrayNodeArr();
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

				ShowDefaults();

				DrawSamplerOptions();
			}
		}

		public override void OnPropertyNameChanged()
		{
			base.OnPropertyNameChanged();
			UIUtils.UpdateTextureArrayDataNode( UniqueId, PropertyInspectorName );
		}

		public override void DrawSubProperties()
		{
			ShowDefaults();

			DrawSamplerOptions();

			EditorGUI.BeginChangeCheck();
			m_defaultTextureArray = EditorGUILayoutObjectField( Constants.DefaultValueLabel, m_defaultTextureArray, typeof( Texture2DArray ), false ) as Texture2DArray;
			if( EditorGUI.EndChangeCheck() )
			{
				CheckTextureImporter( true );
				SetAdditonalTitleText( string.Format( Constants.PropertyValueLabel, GetPropertyValStr() ) );
			}
		}

		public override void DrawMaterialProperties()
		{
			ShowDefaults();

			DrawSamplerOptions();

			EditorGUI.BeginChangeCheck();
			m_materialTextureArray = EditorGUILayoutObjectField( Constants.MaterialValueLabel, m_materialTextureArray, typeof( Texture2DArray ), false ) as Texture2DArray;
			if( EditorGUI.EndChangeCheck() )
			{
				CheckTextureImporter( true );
				SetAdditonalTitleText( string.Format( Constants.PropertyValueLabel, GetPropertyValStr() ) );
				m_requireMaterialUpdate = true;
			}
		}

		public void DrawSamplerOptions()
		{
			EditorGUI.BeginChangeCheck();
			bool autoUnpackNormals = EditorGUILayoutToggle( "Normal Map", m_autoUnpackNormals );
			if( EditorGUI.EndChangeCheck() )
			{
				if( m_autoUnpackNormals != autoUnpackNormals )
				{
					AutoUnpackNormals = autoUnpackNormals;

					ConfigureInputPorts();
					ConfigureOutputPorts();
				}
			}

			if( m_autoUnpackNormals && !m_normalPort.IsConnected )
			{
				m_normalPort.FloatInternalData = EditorGUILayoutFloatField( NormalScaleStr, m_normalPort.FloatInternalData );
			}
		}

		public void ConfigureInputPorts()
		{
			m_normalPort.Visible = AutoUnpackNormals;

			m_sizeIsDirty = true;
		}

		public void ConfigureOutputPorts()
		{
			m_outputPorts[ m_colorPort.PortId + 4 ].Visible = !AutoUnpackNormals;

			if( !AutoUnpackNormals )
			{
				m_colorPort.ChangeProperties( "RGBA", WirePortDataType.FLOAT4, false );
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

		public virtual void CheckTextureImporter( bool additionalCheck )
		{
			m_requireMaterialUpdate = true;
			Texture2DArray texture = m_materialMode ? m_materialTextureArray : m_defaultTextureArray;

			UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath( AssetDatabase.GetAssetPath( texture ), typeof( UnityEngine.Object ) );

			if( obj != null )
			{
				SerializedObject serializedObject = new UnityEditor.SerializedObject( obj );

				if( serializedObject != null )
				{
					SerializedProperty colorSpace = serializedObject.FindProperty( "m_ColorSpace" );
					m_linearTexture = ( colorSpace.intValue == 0 );
				}
			}
		}

		void UpdateHeaderColor()
		{
			m_headerColorModifier = ( m_referenceType == TexReferenceType.Object ) ? Color.white : ReferenceHeaderColor;
		}

		public override void DrawGUIControls( DrawInfo drawInfo )
		{
			base.DrawGUIControls( drawInfo );

			if( !( drawInfo.CurrentEventType == EventType.MouseDown || drawInfo.CurrentEventType == EventType.MouseUp || drawInfo.CurrentEventType == EventType.ExecuteCommand || drawInfo.CurrentEventType == EventType.DragPerform ) )
				return;

			bool insideBox = m_previewRect.Contains( drawInfo.MousePosition );

			if( insideBox )
			{
				m_isEditingPicker = true;
			}
			else if( m_isEditingPicker && !insideBox && drawInfo.CurrentEventType != EventType.ExecuteCommand )
			{
				GUI.FocusControl( null );
				m_isEditingPicker = false;
			}

			if( m_state != ReferenceState.Self && drawInfo.CurrentEventType == EventType.MouseDown && m_previewRect.Contains( drawInfo.MousePosition ) )
			{
				UIUtils.FocusOnNode( m_previewTextProp, 1, true );
				Event.current.Use();
			}
		}

		public override void OnNodeLayout( DrawInfo drawInfo )
		{
			base.OnNodeLayout( drawInfo );

			if( m_drawPreview )
			{
				m_iconPos = m_globalPosition;
				m_iconPos.width = 19 * drawInfo.InvertedZoom;
				m_iconPos.height = 19 * drawInfo.InvertedZoom;

				m_iconPos.y += 10 * drawInfo.InvertedZoom;
				m_iconPos.x += m_globalPosition.width - m_iconPos.width - 5 * drawInfo.InvertedZoom;
			}

			bool instanced = CheckReference();
			if( instanced )
			{
				m_state = ReferenceState.Instance;
				m_previewTextProp = m_referenceSampler;
			}
			else if( m_texPort.IsConnected )
			{
				m_state = ReferenceState.Connected;
				m_previewTextProp = m_texPort.GetOutputNode( 0 ) as ParentNode;
			}
			else
			{
				m_state = ReferenceState.Self;
				m_previewTextProp = this;
			}

			if( m_previewTextProp == null )
				m_previewTextProp = this;

		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );

			if( m_isEditingPicker && m_drawPicker )
			{
				Rect hitRect = m_previewRect;
				hitRect.height = 14 * drawInfo.InvertedZoom;
				hitRect.y = m_previewRect.yMax - hitRect.height;
				hitRect.width = 4 * 14 * drawInfo.InvertedZoom;

				bool restoreMouse = false;
				if( Event.current.type == EventType.MouseDown && hitRect.Contains( drawInfo.MousePosition ) )
				{
					restoreMouse = true;
					Event.current.type = EventType.Ignore;
				}

				EditorGUI.BeginChangeCheck();
				m_colorBuffer = GUI.color;
				GUI.color = Color.clear;
				if( m_materialMode )
					m_materialTextureArray = EditorGUIObjectField( m_previewRect, m_materialTextureArray, typeof( Texture2DArray ), false ) as Texture2DArray;
				else
					m_defaultTextureArray = EditorGUIObjectField( m_previewRect, m_defaultTextureArray, typeof( Texture2DArray ), false ) as Texture2DArray;
				GUI.color = m_colorBuffer;

				if( EditorGUI.EndChangeCheck() )
				{
					CheckTextureImporter( true );
					SetTitleText( PropertyInspectorName );
					SetAdditonalTitleText( string.Format( Constants.PropertyValueLabel, GetPropertyValStr() ) );
					ConfigureInputPorts();
					ConfigureOutputPorts();
					BeginDelayedDirtyProperty();
					m_requireMaterialUpdate = true;
				}

				if( restoreMouse )
				{
					Event.current.type = EventType.MouseDown;
				}

				if( ( drawInfo.CurrentEventType == EventType.MouseDown || drawInfo.CurrentEventType == EventType.MouseUp ) )
					DrawPreviewMaskButtonsLayout( drawInfo, m_previewRect );
			}

			if( drawInfo.CurrentEventType != EventType.Repaint )
				return;

			switch( m_state )
			{
				default:
				case ReferenceState.Self:
				if( drawInfo.CurrentEventType == EventType.Repaint )
				{
					m_drawPreview = false;
					m_drawPicker = true;

					DrawTexturePicker( drawInfo );
				}
				break;
				case ReferenceState.Connected:
				if( drawInfo.CurrentEventType == EventType.Repaint )
				{
					m_drawPreview = true;
					m_drawPicker = false;

					if( m_previewTextProp != null )
					{
						SetTitleTextOnCallback( m_previewTextProp.TitleContent.text, ( instance, newTitle ) => instance.TitleContent.text = newTitle + " (Input)" );
						SetAdditonalTitleText( m_previewTextProp.AdditonalTitleContent.text );
					}

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

					if( m_referenceSampler != null )
					{
						SetTitleTextOnCallback( m_referenceSampler.PreviewTextProp.TitleContent.text, ( instance, newTitle ) => instance.TitleContent.text = newTitle + Constants.InstancePostfixStr );
						SetAdditonalTitleText( m_referenceSampler.PreviewTextProp.AdditonalTitleContent.text );
					}

					// Draw chain lock
					GUI.Label( m_iconPos, string.Empty, UIUtils.GetCustomStyle( CustomStyle.SamplerTextureIcon ) );

					// Draw frame around preview
					GUI.Label( m_previewRect, string.Empty, UIUtils.GetCustomStyle( CustomStyle.SamplerFrame ) );
				}
				break;
			}
		}

		protected void DrawTexturePicker( DrawInfo drawInfo )
		{
			Rect newRect = m_previewRect;
			Texture2DArray currentValue = m_materialMode ? m_materialTextureArray : m_defaultTextureArray;

			if( currentValue == null )
				GUI.Label( newRect, string.Empty, UIUtils.ObjectFieldThumb );
			else
				DrawPreview( drawInfo, m_previewRect );

			if( ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD2 )
			{
				Rect butRect = m_previewRect;
				butRect.y -= 1;
				butRect.x += 1;

				Rect smallButton = newRect;
				smallButton.height = 14 * drawInfo.InvertedZoom;
				smallButton.y = newRect.yMax - smallButton.height - 2;
				smallButton.width = 40 * drawInfo.InvertedZoom;
				smallButton.x = newRect.xMax - smallButton.width - 2;
				if( currentValue == null )
				{
					GUI.Label( newRect, m_labelText, UIUtils.ObjectFieldThumbOverlay );
				}
				else
				{
					DrawPreviewMaskButtonsRepaint( drawInfo, butRect );
				}
				GUI.Label( smallButton, "Select", UIUtils.GetCustomStyle( CustomStyle.SamplerButton ) );
			}

			GUI.Label( newRect, string.Empty, UIUtils.GetCustomStyle( CustomStyle.SamplerFrame ) );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );


			OnPropertyNameChanged();

			CheckReference();

			bool isVertex = ( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation );

			bool instanced = false;
			if( m_referenceType == TexReferenceType.Instance && m_referenceSampler != null )
				instanced = true;

			if( instanced )
			{
				if( !m_referenceSampler.TexPort.IsConnected )
					base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			}
			else if( !m_texPort.IsConnected )
				base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );

			string level = string.Empty;
			if( m_lodPort.Visible )
			{
				level = m_lodPort.GeneratePortInstructions( ref dataCollector );
			}

			if( isVertex && !m_lodPort.Visible )
				level = "0";

			string propertyName = string.Empty;
			if( instanced )
			{
				if( m_referenceSampler.TexPort.IsConnected )
					propertyName = m_referenceSampler.TexPort.GeneratePortInstructions( ref dataCollector );
				else
					propertyName = m_referenceSampler.PropertyName;
			}
			else if( m_texPort.IsConnected )
				propertyName = m_texPort.GeneratePortInstructions( ref dataCollector );
			else
				propertyName = PropertyName;

			string uvs = string.Empty;
			if( m_uvPort.IsConnected )
			{
				uvs = m_uvPort.GeneratePortInstructions( ref dataCollector );
			}
			else
			{
				if( dataCollector.IsTemplate )
				{
					uvs = dataCollector.TemplateDataCollectorInstance.GetTextureCoord( m_uvSet, ( instanced ? m_referenceSampler.PropertyName : PropertyName ), UniqueId, m_currentPrecisionType );
				}
				else
				{
					if( isVertex )
						uvs = TexCoordVertexDataNode.GenerateVertexUVs( ref dataCollector, UniqueId, m_uvSet, propertyName );
					else
						uvs = TexCoordVertexDataNode.GenerateFragUVs( ref dataCollector, UniqueId, m_uvSet, propertyName );
				}
			}
			string index = m_indexPort.GeneratePortInstructions( ref dataCollector );

			string m_normalMapUnpackMode = "";
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

				string scaleValue = isScaledNormal?m_normalPort.GeneratePortInstructions( ref dataCollector ):"1.0";
				m_normalMapUnpackMode = TemplateHelperFunctions.CreateUnpackNormalStr( dataCollector, isScaledNormal, scaleValue );
				if(  isScaledNormal && (! dataCollector.IsTemplate || !dataCollector.IsSRP ))
				{
					dataCollector.AddToIncludes( UniqueId, Constants.UnityStandardUtilsLibFuncs );
				}
				
			}

			string result = string.Empty;

			if( dataCollector.IsTemplate && dataCollector.IsSRP )
			{
				//CAREFUL mipbias here means derivative (this needs index changes)
				//TODO: unity now supports bias as well
				if( m_mipMode == MipType.MipBias )
				{
					dataCollector.UsingArrayDerivatives = true;
					result = propertyName + ".SampleGrad(sampler" + propertyName + ", float3(" + uvs + ", " + index + "), " + m_ddxPort.GeneratePortInstructions( ref dataCollector ) + ", " + m_ddyPort.GeneratePortInstructions( ref dataCollector ) + ");";
				}
				else if( m_lodPort.Visible || isVertex )
				{
					result = "SAMPLE_TEXTURE2D_ARRAY_LOD(" + propertyName + ", sampler" + propertyName + ", " + uvs + ", " + index + ", " + level + " )";
				}
				else
				{
					result = "SAMPLE_TEXTURE2D_ARRAY(" + propertyName + ", sampler" + propertyName + ", " + uvs + ", " + index + " )";
				}
			}
			else
			{
				//CAREFUL mipbias here means derivative (this needs index changes)
				if( m_mipMode == MipType.MipBias )
				{
					dataCollector.UsingArrayDerivatives = true;
					result = "ASE_SAMPLE_TEX2DARRAY_GRAD(" + propertyName + ", float3(" + uvs + ", " + index + "), " + m_ddxPort.GeneratePortInstructions( ref dataCollector ) + ", " + m_ddyPort.GeneratePortInstructions( ref dataCollector ) + " )";
				}
				else if( m_lodPort.Visible || isVertex )
				{
					result = "UNITY_SAMPLE_TEX2DARRAY_LOD(" + propertyName + ", float3(" + uvs + ", " + index + "), " + level + " )";
				}
				else
				{
					result = "UNITY_SAMPLE_TEX2DARRAY" + ( m_lodPort.Visible || isVertex ? "_LOD" : "" ) + "(" + propertyName + ", float3(" + uvs + ", " + index + ") " + ( m_lodPort.Visible || isVertex ? ", " + level : "" ) + " )";
				}
			}

			if( m_autoUnpackNormals )
				result = string.Format( m_normalMapUnpackMode, result );

			RegisterLocalVariable( 0, result, ref dataCollector, "texArray" + OutputId );
			return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );
		}

		public override string PropertyName
		{
			get
			{
				if( m_referenceType == TexReferenceType.Instance && m_referenceSampler != null )
					return m_referenceSampler.PropertyName;
				else
					return base.PropertyName;
			}
		}

		public override string PropertyInspectorName
		{
			get
			{
				if( m_referenceType == TexReferenceType.Instance && m_referenceSampler != null )
					return m_referenceSampler.PropertyInspectorName;
				else
					return base.PropertyInspectorName;
			}
		}

		public override string GetPropertyValue()
		{
			return PropertyAttributes + PropertyName + "(\"" + PropertyInspectorName + "\", 2DArray ) = \"\" {}";
		}

		public override bool GetUniformData( out string dataType, out string dataName )
		{
			MasterNode currMasterNode = ( m_containerGraph.CurrentMasterNode != null ) ? m_containerGraph.CurrentMasterNode : m_containerGraph.ParentWindow.OutsideGraph.CurrentMasterNode;
			if( currMasterNode != null && currMasterNode.CurrentDataCollector.IsTemplate && currMasterNode.CurrentDataCollector.IsSRP )
			{
				dataType = "TEXTURE2D_ARRAY( " + PropertyName + "";
				dataName = ");\nuniform SAMPLER( sampler" + PropertyName + " )";
				return true;
			}
			dataType = "UNITY_DECLARE_TEX2DARRAY(";
			dataName = PropertyName + " )";
			return true;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			string textureName = GetCurrentParam( ref nodeParams );
			m_defaultTextureArray = AssetDatabase.LoadAssetAtPath<Texture2DArray>( textureName );
			m_uvSet = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_referenceType = (TexReferenceType)Enum.Parse( typeof( TexReferenceType ), GetCurrentParam( ref nodeParams ) );
			m_referenceNodeId = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			if( UIUtils.CurrentShaderVersion() > 3202 )
				m_mipMode = (MipType)Enum.Parse( typeof( MipType ), GetCurrentParam( ref nodeParams ) );
			if( UIUtils.CurrentShaderVersion() > 5105 )
				m_autoUnpackNormals = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );

			if( m_referenceType == TexReferenceType.Instance )
			{
				UIUtils.UnregisterTextureArrayNode( this );
				UIUtils.UnregisterPropertyNode( this );
			}

			ConfigureInputPorts();
			ConfigureOutputPorts();

			m_lodPort.Visible = ( m_mipMode == MipType.MipLevel );
			m_ddxPort.Visible = ( m_mipMode == MipType.MipBias ); //not really bias, it's derivative
			m_ddyPort.Visible = ( m_mipMode == MipType.MipBias ); //not really bias, it's derivative

			UpdateHeaderColor();

			if( m_defaultTextureArray )
			{
				m_materialTextureArray = m_defaultTextureArray;
			}
		}

		public override void RefreshExternalReferences()
		{
			base.RefreshExternalReferences();

			m_referenceSampler = UIUtils.GetNode( m_referenceNodeId ) as TextureArrayNode;
			m_referenceArrayId = UIUtils.GetTextureArrayNodeRegisterId( m_referenceNodeId );
			OnPropertyNameChanged();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( m_defaultTextureArray != null ) ? AssetDatabase.GetAssetPath( m_defaultTextureArray ) : Constants.NoStringValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_uvSet.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_referenceType );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( ( m_referenceSampler != null ) ? m_referenceSampler.UniqueId : -1 ) );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_mipMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_autoUnpackNormals );
		}

		public override void ReadAdditionalClipboardData( ref string[] nodeParams )
		{
			base.ReadAdditionalClipboardData( ref nodeParams );
			string textureName = GetCurrentParam( ref nodeParams );
			m_materialTextureArray = AssetDatabase.LoadAssetAtPath<Texture2DArray>( textureName );
		}

		public override void WriteAdditionalClipboardData( ref string nodeInfo )
		{
			base.WriteAdditionalClipboardData( ref nodeInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( m_materialTextureArray != null ) ? AssetDatabase.GetAssetPath( m_materialTextureArray ) : Constants.NoStringValue );
		}


		public override void UpdateMaterial( Material mat )
		{
			base.UpdateMaterial( mat );
			if( UIUtils.IsProperty( m_currentParameterType ) && !InsideShaderFunction && m_referenceType == TexReferenceType.Object )
			{
				OnPropertyNameChanged();
				if( mat.HasProperty( PropertyName ) )
				{
					mat.SetTexture( PropertyName, m_materialTextureArray );
				}
			}
		}

		public override void SetMaterialMode( Material mat, bool fetchMaterialValues )
		{
			base.SetMaterialMode( mat, fetchMaterialValues );
			if( fetchMaterialValues && m_materialMode && UIUtils.IsProperty( m_currentParameterType ) )
			{
				if( mat.HasProperty( PropertyName ) )
				{
					m_materialTextureArray = (Texture2DArray)mat.GetTexture( PropertyName );
					if( m_materialTextureArray == null )
						m_materialTextureArray = m_defaultTextureArray;
				}
			}
		}

		public override void ForceUpdateFromMaterial( Material material )
		{
			if( UIUtils.IsProperty( m_currentParameterType ) && material.HasProperty( PropertyName ) )
			{
				m_materialTextureArray = (Texture2DArray)material.GetTexture( PropertyName );
				if( m_materialTextureArray == null )
					m_materialTextureArray = m_defaultTextureArray;
			}
		}

		public override bool UpdateShaderDefaults( ref Shader shader, ref TextureDefaultsDataColector defaultCol )
		{
			if( m_defaultTextureArray != null )
			{
				defaultCol.AddValue( PropertyName, m_defaultTextureArray );
			}

			return true;
		}

		public override string GetPropertyValStr()
		{
			return m_materialMode ? ( m_materialTextureArray != null ? m_materialTextureArray.name : IOUtils.NO_TEXTURES ) : ( m_defaultTextureArray != null ? m_defaultTextureArray.name : IOUtils.NO_TEXTURES );
		}

		public bool CheckReference()
		{
			if( m_referenceType == TexReferenceType.Instance && m_referenceArrayId > -1 )
			{
				m_referenceSampler = UIUtils.GetTextureArrayNode( m_referenceArrayId );

				if( m_referenceSampler == null )
				{
					m_texPort.Locked = false;
					m_referenceArrayId = -1;
				}
				else
					m_texPort.Locked = true;
			}
			else
			{
				m_texPort.Locked = false;
			}

			return m_referenceSampler != null;
		}

		public override void SetupFromCastObject( UnityEngine.Object obj )
		{
			base.SetupFromCastObject( obj );
			SetupFromObject( obj );
		}

		public override void OnObjectDropped( UnityEngine.Object obj )
		{
			SetupFromObject( obj );
		}

		private void SetupFromObject( UnityEngine.Object obj )
		{
			if( m_materialMode )
				m_materialTextureArray = obj as Texture2DArray;
			else
				m_defaultTextureArray = obj as Texture2DArray;
		}

		public Texture2DArray TextureArray { get { return ( m_materialMode ? m_materialTextureArray : m_defaultTextureArray ); } }

		public bool IsLinearTexture { get { return m_linearTexture; } }

		public bool AutoUnpackNormals
		{
			get { return m_autoUnpackNormals; }
			set { m_autoUnpackNormals = value; }
		}

		public override string DataToArray { get { return PropertyInspectorName; } }

		public override void Destroy()
		{
			base.Destroy();
			m_defaultTextureArray = null;
			m_materialTextureArray = null;

			m_texPort = null;
			m_uvPort = null;
			m_indexPort = null;
			m_lodPort = null;
			m_normalPort = null;
			m_ddxPort = null;
			m_ddyPort = null;

			if( m_referenceType == TexReferenceType.Object )
			{
				UIUtils.UnregisterTextureArrayNode( this );
				UIUtils.UnregisterPropertyNode( this );
			}
		}

		public ParentNode PreviewTextProp { get { return m_previewTextProp; } }
		public InputPort TexPort { get { return m_texPort; } }
	}
}
