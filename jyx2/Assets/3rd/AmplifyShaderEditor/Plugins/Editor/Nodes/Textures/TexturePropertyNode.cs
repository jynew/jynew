// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	public enum TexturePropertyValues
	{
		white,
		black,
		gray,
		bump
	}

	public enum TextureType
	{
		Texture1D,
		Texture2D,
		Texture3D,
		Cube,
		Texture2DArray,
		ProceduralTexture
	}

	public enum AutoCastType
	{
		Auto = 0,
		LockedToTexture1D,
		LockedToTexture2D,
		LockedToTexture3D,
		LockedToCube,
		LockedToTexture2DArray
	}


	[Serializable]
	[NodeAttributes( "Texture Object", "Textures", "Represents a Texture Asset. Can be used in samplers <b>Tex</b> inputs or shader function inputs to reuse the same texture multiple times.", SortOrderPriority = 1 )]
	public class TexturePropertyNode : PropertyNode
	{
		private const string ObjectSelectorCmdStr = "ObjectSelectorClosed";

		protected readonly string[] AvailablePropertyTypeLabels = { PropertyType.Property.ToString(), PropertyType.Global.ToString() };
		protected readonly int[] AvailablePropertyTypeValues = { (int)PropertyType.Property, (int)PropertyType.Global };

		protected const int OriginalFontSizeUpper = 9;
		protected const int OriginalFontSizeLower = 9;

		protected const string DefaultTextureStr = "Default Texture";
		protected const string AutoCastModeStr = "Auto-Cast Mode";

		protected const string AutoUnpackNormalsStr = "Normal";

		[SerializeField]
		protected Texture m_defaultValue;

		[SerializeField]
		protected Texture m_materialValue;

		[SerializeField]
		protected TexturePropertyValues m_defaultTextureValue;

		[SerializeField]
		protected bool m_isNormalMap;

		[SerializeField]
		protected System.Type m_textureType = typeof( Texture2D );

		//[SerializeField]
		//protected bool m_isTextureFetched;

		//[SerializeField]
		//protected string m_textureFetchedValue;

		[SerializeField]
		protected TextureType m_currentType = TextureType.Texture2D;

		[SerializeField]
		protected AutoCastType m_autocastMode = AutoCastType.Auto;

		protected int PreviewSizeX = 128;
		protected int PreviewSizeY = 128;

		protected bool m_linearTexture;

		protected TexturePropertyNode m_textureProperty = null;

		protected bool m_drawPicker;

		protected bool m_drawAutocast = true;

		protected int m_cachedSamplerId = -1;
		protected int m_cachedSamplerIdArray = -1;
		protected int m_cachedSamplerIdCube = -1;
		protected int m_cachedSamplerId3D = -1;
		protected int m_defaultId = -1;
		protected int m_typeId = -1;

		private TextureType m_previousType = TextureType.Texture2D;
		private string m_labelText = "None (Texture2D)";

		protected bool m_isEditingPicker;

		public TexturePropertyNode() : base() { }
		public TexturePropertyNode( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { }
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			GlobalTypeWarningText = string.Format( GlobalTypeWarningText, "Texture" );
			m_defaultTextureValue = TexturePropertyValues.white;
			m_insideSize.Set( PreviewSizeX, PreviewSizeY + 5 );
			AddOutputPort( WirePortDataType.SAMPLER2D, "Tex" );
			m_outputPorts[ 0 ].CreatePortRestrictions( WirePortDataType.SAMPLER1D, WirePortDataType.SAMPLER2D, WirePortDataType.SAMPLER3D, WirePortDataType.SAMPLERCUBE, WirePortDataType.OBJECT );
			m_currentParameterType = PropertyType.Property;
			m_customPrefix = "Texture ";
			m_drawPrecisionUI = false;
			m_showVariableMode = true;
			m_freeType = false;
			m_drawPicker = true;
			m_hasLeftDropdown = true;
			m_textLabelWidth = 115;
			m_longNameSize = 225;
			m_availableAttribs.Add( new PropertyAttributes( "No Scale Offset", "[NoScaleOffset]" ) );
			m_availableAttribs.Add( new PropertyAttributes( "Normal", "[Normal]" ) );
			m_showPreview = true;
			m_drawPreviewExpander = false;
			m_drawPreview = false;
			m_drawPreviewMaskButtons = false;
			m_previewShaderGUID = "e53988745ec6e034694ee2640cd3d372";
		}

		public override void AfterCommonInit()
		{
			base.AfterCommonInit();
			m_hasLeftDropdown = true;
		}

		protected void SetPreviewTexture( Texture newValue )
		{
			if( newValue is Cubemap )
			{
				PreviewMaterial.SetInt( m_typeId, 3 );

				if( m_cachedSamplerIdCube == -1 )
					m_cachedSamplerIdCube = Shader.PropertyToID( "_Cube" );

				PreviewMaterial.SetTexture( m_cachedSamplerIdCube, newValue as Cubemap );
			}
			else if( newValue is Texture2DArray )
			{
				PreviewMaterial.SetInt( m_typeId, 4 );

				if( m_cachedSamplerIdArray == -1 )
					m_cachedSamplerIdArray = Shader.PropertyToID( "_Array" );

				PreviewMaterial.SetTexture( m_cachedSamplerIdArray, newValue as Texture2DArray );
			}
			else if( newValue is Texture3D )
			{
				PreviewMaterial.SetInt( m_typeId, 2 );

				if( m_cachedSamplerId3D == -1 )
					m_cachedSamplerId3D = Shader.PropertyToID( "_Sampler3D" );

				PreviewMaterial.SetTexture( m_cachedSamplerId3D, newValue as Texture3D );
			}
			else
			{
				PreviewMaterial.SetInt( m_typeId, 1 );

				if( m_cachedSamplerId == -1 )
					m_cachedSamplerId = Shader.PropertyToID( "_Sampler" );

				PreviewMaterial.SetTexture( m_cachedSamplerId, newValue );
			}
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if( Value == null )
			{
				if( m_defaultId == -1 )
					m_defaultId = Shader.PropertyToID( "_Default" );

				PreviewMaterial.SetInt( m_defaultId, ( (int)m_defaultTextureValue ) + 1 );
			}
			else
			{
				if( m_defaultId == -1 )
					m_defaultId = Shader.PropertyToID( "_Default" );

				PreviewMaterial.SetInt( m_defaultId, 0 );

				if( m_typeId == -1 )
					m_typeId = Shader.PropertyToID( "_Type" );

				SetPreviewTexture( Value );
				//if( Value is Cubemap )
				//{
				//	PreviewMaterial.SetInt( m_typeId, 3 );

				//	if( m_cachedSamplerIdCube == -1 )
				//		m_cachedSamplerIdCube = Shader.PropertyToID( "_Cube" );

				//	PreviewMaterial.SetTexture( m_cachedSamplerIdCube, Value as Cubemap );
				//}
				//else if( Value is Texture2DArray )
				//{
				//	PreviewMaterial.SetInt( m_typeId, 4 );

				//	if( m_cachedSamplerIdArray == -1 )
				//		m_cachedSamplerIdArray = Shader.PropertyToID( "_Array" );

				//	PreviewMaterial.SetTexture( m_cachedSamplerIdArray, Value as Texture2DArray );
				//}
				//else if( Value is Texture3D )
				//{
				//	PreviewMaterial.SetInt( m_typeId, 2 );

				//	if( m_cachedSamplerId3D == -1 )
				//		m_cachedSamplerId3D = Shader.PropertyToID( "_Sampler3D" );

				//	PreviewMaterial.SetTexture( m_cachedSamplerId3D, Value as Texture3D );
				//}
				//else
				//{
				//	PreviewMaterial.SetInt( m_typeId, 1 );

				//	if( m_cachedSamplerId == -1 )
				//		m_cachedSamplerId = Shader.PropertyToID( "_Sampler" );

				//	PreviewMaterial.SetTexture( m_cachedSamplerId, Value );
				//}
			}
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			m_textureProperty = this;
			UIUtils.RegisterPropertyNode( this );
			UIUtils.RegisterTexturePropertyNode( this );
		}

		protected void ConfigTextureData( TextureType type )
		{
			switch( m_autocastMode )
			{
				case AutoCastType.Auto:
				{
					m_currentType = type;
				}
				break;
				case AutoCastType.LockedToTexture1D:
				{
					m_currentType = TextureType.Texture1D;
				}
				break;
				case AutoCastType.LockedToTexture2DArray:
				{
					m_currentType = TextureType.Texture2DArray;
				}
				break;
				case AutoCastType.LockedToTexture2D:
				{
					m_currentType = TextureType.Texture2D;
				}
				break;
				case AutoCastType.LockedToTexture3D:
				{
					m_currentType = TextureType.Texture3D;
				}
				break;
				case AutoCastType.LockedToCube:
				{
					m_currentType = TextureType.Cube;
				}
				break;
			}

			ConfigTextureType();
		}

		protected void ConfigTextureType()
		{
			switch( m_currentType )
			{
				case TextureType.Texture1D:
				{
					m_textureType = typeof( Texture );
				}
				break;
				case TextureType.Texture2DArray:
				{
					m_textureType = typeof( Texture2DArray );
				}
				break;
				case TextureType.Texture2D:
				{
					m_textureType = typeof( Texture2D );
				}
				break;
				case TextureType.Texture3D:
				{
					m_textureType = typeof( Texture3D );
				}
				break;
				case TextureType.Cube:
				{
					m_textureType = typeof( Cubemap );
				}
				break;
#if !UNITY_2018_1_OR_NEWER
				// Disabling Substance Deprecated warning
#pragma warning disable 0618
				case TextureType.ProceduralTexture:
				{
					m_textureType = typeof( ProceduralTexture );
				}
				break;
#pragma warning restore 0618
#endif

			}
		}

		protected void DrawTexturePropertyType()
		{
			PropertyType parameterType = (PropertyType)EditorGUILayoutIntPopup( ParameterTypeStr, (int)m_currentParameterType, AvailablePropertyTypeLabels, AvailablePropertyTypeValues );
			if( parameterType != m_currentParameterType )
			{
				ChangeParameterType( parameterType );
			}
		}

		// Texture1D
		public string GetTexture1DPropertyValue()
		{
			return PropertyName + "(\"" + m_propertyInspectorName + "\", 2D) = \"" + m_defaultTextureValue + "\" {}";
		}

		public string GetTexture1DUniformValue()
		{
			return "uniform sampler1D " + PropertyName + ";";
		}

		// Texture2D
		public string GetTexture2DPropertyValue()
		{
			return PropertyName + "(\"" + m_propertyInspectorName + "\", 2D) = \"" + m_defaultTextureValue + "\" {}";
		}

		public string GetTexture2DUniformValue()
		{
			return "uniform sampler2D " + PropertyName + ";";
		}

		//Texture3D
		public string GetTexture3DPropertyValue()
		{
			return PropertyName + "(\"" + m_propertyInspectorName + "\", 3D) = \"" + m_defaultTextureValue + "\" {}";
		}

		public string GetTexture3DUniformValue()
		{
			return "uniform sampler3D " + PropertyName + ";";
		}

		// Cube
		public string GetCubePropertyValue()
		{
			return PropertyName + "(\"" + m_propertyInspectorName + "\", CUBE) = \"" + m_defaultTextureValue + "\" {}";
		}

		public string GetCubeUniformValue()
		{
			return "uniform samplerCUBE " + PropertyName + ";";
		}

		// Texture2DArray
		public string GetTexture2DArrayPropertyValue()
		{
			return PropertyName + "(\"" + m_propertyInspectorName + "\", 2DArray) = \"" + m_defaultTextureValue + "\" {}";
		}

		public string GetTexture2DArrayUniformValue()
		{
			return "uniform TEXTURE2D_ARRAY( " + PropertyName + " );" + "\nuniform SAMPLER( sampler" + PropertyName + " );";
		}

		public override void DrawMainPropertyBlock()
		{
			DrawTexturePropertyType();
			base.DrawMainPropertyBlock();
		}

		public override void DrawSubProperties()
		{
			ShowDefaults();
			EditorGUI.BeginChangeCheck();
			Type currType = ( m_autocastMode == AutoCastType.Auto ) ? typeof( Texture ) : m_textureType;
			m_defaultValue = EditorGUILayoutObjectField( Constants.DefaultValueLabel, m_defaultValue, currType, false ) as Texture;
			if( EditorGUI.EndChangeCheck() )
			{
				CheckTextureImporter( true );
				SetAdditonalTitleText( string.Format( Constants.PropertyValueLabel, GetPropertyValStr() ) );
			}
		}

		public override void DrawMaterialProperties()
		{
			ShowDefaults();

			EditorGUI.BeginChangeCheck();
			Type currType = ( m_autocastMode == AutoCastType.Auto ) ? typeof( Texture ) : m_textureType;
			m_materialValue = EditorGUILayoutObjectField( Constants.MaterialValueLabel, m_materialValue, currType, false ) as Texture;
			if( EditorGUI.EndChangeCheck() )
			{
				CheckTextureImporter( true );
				SetAdditonalTitleText( string.Format( Constants.PropertyValueLabel, GetPropertyValStr() ) );
			}
		}

		new void ShowDefaults()
		{
			m_defaultTextureValue = (TexturePropertyValues)EditorGUILayoutEnumPopup( DefaultTextureStr, m_defaultTextureValue );

			if( !m_drawAutocast )
				return;

			AutoCastType newAutoCast = (AutoCastType)EditorGUILayoutEnumPopup( AutoCastModeStr, m_autocastMode );
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

		private void ConfigurePortsFromReference()
		{
			m_sizeIsDirty = true;
		}

		public virtual void ConfigureOutputPorts()
		{
			switch( m_currentType )
			{
				case TextureType.Texture1D:
				m_outputPorts[ 0 ].ChangeType( WirePortDataType.SAMPLER1D, false );
				break;
				case TextureType.ProceduralTexture:
				case TextureType.Texture2D:
				m_outputPorts[ 0 ].ChangeType( WirePortDataType.SAMPLER2D, false );
				break;
				case TextureType.Texture3D:
				m_outputPorts[ 0 ].ChangeType( WirePortDataType.SAMPLER3D, false );
				break;
				case TextureType.Cube:
				m_outputPorts[ 0 ].ChangeType( WirePortDataType.SAMPLERCUBE, false );
				break;
				case TextureType.Texture2DArray:
				m_outputPorts[ 0 ].ChangeType( WirePortDataType.SAMPLER2D, false );
				break;
			}

			m_sizeIsDirty = true;
		}

		public virtual void ConfigureInputPorts()
		{
		}

		public virtual void AdditionalCheck()
		{
		}

		public virtual void CheckTextureImporter( bool additionalCheck, bool writeDefault = true )
		{
			m_requireMaterialUpdate = true;
			Texture texture = m_materialMode ? m_materialValue : m_defaultValue;
			TextureImporter importer = AssetImporter.GetAtPath( AssetDatabase.GetAssetPath( texture ) ) as TextureImporter;
			if( importer != null )
			{

#if UNITY_5_5_OR_NEWER
				m_isNormalMap = importer.textureType == TextureImporterType.NormalMap;
#else
				m_isNormalMap = importer.normalmap;
#endif
				if( writeDefault && !UIUtils.IsLoading )
				{
					if( m_defaultTextureValue == TexturePropertyValues.bump && !m_isNormalMap )
						m_defaultTextureValue = TexturePropertyValues.white;
					else if( m_isNormalMap )
						m_defaultTextureValue = TexturePropertyValues.bump;
				}

				if( additionalCheck )
					AdditionalCheck();
				m_linearTexture = !importer.sRGBTexture;
			}

			if( ( texture as Texture2DArray ) != null )
			{
				ConfigTextureData( TextureType.Texture2DArray );
			}
			else if( ( texture as Texture2D ) != null )
			{
				ConfigTextureData( TextureType.Texture2D );
			}
			else if( ( texture as Texture3D ) != null )
			{
				ConfigTextureData( TextureType.Texture3D );
			}
			else if( ( texture as Cubemap ) != null )
			{
				ConfigTextureData( TextureType.Cube );
			}
#if !UNITY_2018_1_OR_NEWER
			// Disabling Substance Deprecated warning
#pragma warning disable 0618
			else if( ( texture as ProceduralTexture ) != null )
			{
				ConfigTextureData( TextureType.ProceduralTexture );
			}
#pragma warning restore 0618
#endif

			ConfigureInputPorts();
			ConfigureOutputPorts();
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

		protected void ConfigFromObject( UnityEngine.Object obj, bool writeDefault = true, bool additionalCheck = true )
		{
			Texture texture = obj as Texture;
			if( texture )
			{
				m_materialValue = texture;
				m_defaultValue = texture;
				CheckTextureImporter( additionalCheck, writeDefault );
			}
		}



		public override void DrawGUIControls( DrawInfo drawInfo )
		{
			base.DrawGUIControls( drawInfo );

			if( !( drawInfo.CurrentEventType == EventType.MouseDown || drawInfo.CurrentEventType == EventType.MouseUp || drawInfo.CurrentEventType == EventType.ExecuteCommand || drawInfo.CurrentEventType == EventType.DragPerform ) )
				return;

			bool insideBox = m_previewRect.Contains( drawInfo.MousePosition );

			bool closePicker = false;
			if( insideBox )
			{
				m_isEditingPicker = true;
			}
			else if( m_isEditingPicker && !insideBox && drawInfo.CurrentEventType != EventType.ExecuteCommand )
			{
				closePicker = true;
			}

			if( m_isEditingPicker && drawInfo.CurrentEventType == EventType.ExecuteCommand &&
				Event.current.commandName.Equals( ObjectSelectorCmdStr ) )
			{
				closePicker = true;
			}

			if( closePicker )
			{
				GUI.FocusControl( null );
				m_isEditingPicker = false;
			}

		}

		public override void OnNodeLayout( DrawInfo drawInfo )
		{
			base.OnNodeLayout( drawInfo );
			ConfigTextureType();
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			if( m_dropdownEditing )
			{
				PropertyType parameterType = (PropertyType)EditorGUIIntPopup( m_dropdownRect,(int)m_currentParameterType, AvailablePropertyTypeLabels, AvailablePropertyTypeValues , UIUtils.PropertyPopUp );
				if( parameterType != m_currentParameterType )
				{
					ChangeParameterType( parameterType );
					m_dropdownEditing = false;
				}
			}

			if( m_isEditingPicker && m_drawPicker && m_currentParameterType != PropertyType.Global)
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
				Type currType = ( m_autocastMode == AutoCastType.Auto ) ? typeof( Texture ) : m_textureType;
				if( m_materialMode )
				{
					m_materialValue = EditorGUIObjectField( m_previewRect, m_materialValue, currType, false ) as Texture;
				}
				else
				{
					m_defaultValue = EditorGUIObjectField( m_previewRect, m_defaultValue, currType, false ) as Texture;
				}
				GUI.color = m_colorBuffer;

				if( EditorGUI.EndChangeCheck() )
				{
					CheckTextureImporter( true );
					SetTitleText( m_propertyInspectorName );
					SetAdditonalTitleText( string.Format( Constants.PropertyValueLabel, GetPropertyValStr() ) );
					ConfigureInputPorts();
					ConfigureOutputPorts();
					BeginDelayedDirtyProperty();
				}
				//else if( drawInfo.CurrentEventType == EventType.ExecuteCommand )
				//{
				//	GUI.FocusControl( null );
				//	m_isEditingPicker = false;
				//}

				if( restoreMouse )
				{
					Event.current.type = EventType.MouseDown;
				}

				if( ( drawInfo.CurrentEventType == EventType.MouseDown || drawInfo.CurrentEventType == EventType.MouseUp ) )
					DrawPreviewMaskButtonsLayout( drawInfo, m_previewRect );
			}

			if( !m_drawPicker )
				return;

			if( drawInfo.CurrentEventType == EventType.Repaint )
			{
				DrawTexturePicker( drawInfo );
			}
		}



		protected void DrawTexturePicker( DrawInfo drawInfo )
		{
			Rect newRect = m_previewRect;
			Texture currentValue = m_materialMode ? m_materialValue : m_defaultValue;

			//???
			//m_showPreview = true;
			bool showButtons = m_currentParameterType != PropertyType.Global;

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
					if( m_previousType != m_currentType )
					{
						m_previousType = m_currentType;
						m_labelText = "None (" + m_currentType.ToString() + ")";
					}

					GUI.Label( newRect, m_labelText, UIUtils.ObjectFieldThumbOverlay );
				}
				else if( showButtons )
				{
					DrawPreviewMaskButtonsRepaint( drawInfo, butRect );
				}

				if( showButtons )
					GUI.Label( smallButton, "Select", UIUtils.GetCustomStyle( CustomStyle.SamplerButton ) );
			}

			GUI.Label( newRect, string.Empty, UIUtils.GetCustomStyle( CustomStyle.SamplerFrame ) );
		}

		public string BaseGenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalVar );
			return PropertyName;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			return BaseGenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalVar );
		}

		public override void UpdateMaterial( Material mat )
		{
			base.UpdateMaterial( mat );
			if( UIUtils.IsProperty( m_currentParameterType ) && !InsideShaderFunction )
			{
				OnPropertyNameChanged();
				if( mat.HasProperty( PropertyName ) )
				{
					mat.SetTexture( PropertyName, m_materialValue );
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
					m_materialValue = mat.GetTexture( PropertyName );
					CheckTextureImporter( false, false );
				}
			}
		}

		public override void ForceUpdateFromMaterial( Material material )
		{
			if( UIUtils.IsProperty( m_currentParameterType ) && material.HasProperty( PropertyName ) )
			{
				m_materialValue = material.GetTexture( PropertyName );
				CheckTextureImporter( false, false );
			}
		}

		public override bool UpdateShaderDefaults( ref Shader shader, ref TextureDefaultsDataColector defaultCol/* ref string metaStr */)
		{
			if( m_defaultValue != null )
			{
				defaultCol.AddValue( PropertyName, m_defaultValue );
			}

			return true;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			ReadAdditionalData( ref nodeParams );
		}

		public virtual void ReadAdditionalData( ref string[] nodeParams )
		{
			string defaultTextureGUID = GetCurrentParam( ref nodeParams );
			//m_defaultValue = AssetDatabase.LoadAssetAtPath<Texture>( textureName );
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

			m_isNormalMap = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			m_defaultTextureValue = (TexturePropertyValues)Enum.Parse( typeof( TexturePropertyValues ), GetCurrentParam( ref nodeParams ) );
			m_autocastMode = (AutoCastType)Enum.Parse( typeof( AutoCastType ), GetCurrentParam( ref nodeParams ) );
			if( UIUtils.CurrentShaderVersion() > 15306 )
			{
				m_currentType = (TextureType)Enum.Parse( typeof( TextureType ), GetCurrentParam( ref nodeParams ) );
			}
			else
			{
				m_currentType = TextureType.Texture2D;
			}
			
			ConfigTextureData( m_currentType );

			//ConfigFromObject( m_defaultValue );
			if( m_materialValue == null )
			{
				ConfigFromObject( m_defaultValue );
			}
			else
			{
				CheckTextureImporter( true, true );
			}
			ConfigureInputPorts();
			ConfigureOutputPorts();
		}

		public override void ReadAdditionalClipboardData( ref string[] nodeParams )
		{
			base.ReadAdditionalClipboardData( ref nodeParams );
			string textureName = GetCurrentParam( ref nodeParams );
			m_materialValue = AssetDatabase.LoadAssetAtPath<Texture>( textureName );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			WriteAdditionalToString( ref nodeInfo, ref connectionsInfo );
		}

		public virtual void WriteAdditionalToString( ref string nodeInfo, ref string connectionsInfo )
		{
			//IOUtils.AddFieldValueToString( ref nodeInfo, ( m_defaultValue != null ) ? AssetDatabase.GetAssetPath( m_defaultValue ) : Constants.NoStringValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( m_defaultValue != null ) ? AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath( m_defaultValue ) ) : Constants.NoStringValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( m_materialValue != null ) ? AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath( m_materialValue ) ) : Constants.NoStringValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_isNormalMap.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_defaultTextureValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_autocastMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_currentType );
		}

		public override void WriteAdditionalClipboardData( ref string nodeInfo )
		{
			base.WriteAdditionalClipboardData( ref nodeInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( m_materialValue != null ) ? AssetDatabase.GetAssetPath( m_materialValue ) : Constants.NoStringValue );
		}

		public override void Destroy()
		{
			base.Destroy();
			m_defaultValue = null;
			m_materialValue = null;
			m_textureProperty = null;
			UIUtils.UnregisterPropertyNode( this );
			UIUtils.UnregisterTexturePropertyNode( this );
		}

		public override string GetPropertyValStr()
		{
			return m_materialMode ? ( m_materialValue != null ? m_materialValue.name : IOUtils.NO_TEXTURES ) : ( m_defaultValue != null ? m_defaultValue.name : IOUtils.NO_TEXTURES );
		}

		public override string GetPropertyValue()
		{
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
				case TextureType.Texture2DArray:
				{
					return PropertyAttributes + GetTexture2DArrayPropertyValue();
				}
			}
			return string.Empty;
		}

		public override string GetUniformValue()
		{
			switch( m_currentType )
			{
				case TextureType.Texture1D:
				{
					return GetTexture1DUniformValue();
				}
				case TextureType.ProceduralTexture:
				case TextureType.Texture2D:
				{
					return GetTexture2DUniformValue();
				}
				case TextureType.Texture3D:
				{
					return GetTexture3DUniformValue();
				}
				case TextureType.Cube:
				{
					return GetCubeUniformValue();
				}
				case TextureType.Texture2DArray:
				{
					return GetTexture2DArrayUniformValue();
				}
			}

			return string.Empty;
		}

		public override bool GetUniformData( out string dataType, out string dataName )
		{
			if( m_currentType == TextureType.Texture2DArray )
			{
				MasterNode masterNode = UIUtils.CurrentWindow.OutsideGraph.CurrentMasterNode;
				if( masterNode.CurrentDataCollector.IsTemplate && masterNode.CurrentDataCollector.IsSRP )
				{
					dataType = "TEXTURE2D_ARRAY( " + PropertyName + "";
					dataName = ");\nuniform SAMPLER( sampler" + PropertyName + " )";
					return true;
				}
				dataType = "UNITY_DECLARE_TEX2DARRAY(";
				dataName = m_propertyName + " )";
				return true;
			}

			dataType = UIUtils.TextureTypeToCgType( m_currentType );
			dataName = m_propertyName;
			return true;
		}

		public virtual string CurrentPropertyReference
		{
			get
			{
				string propertyName = string.Empty;
				propertyName = PropertyName;
				return propertyName;
			}
		}

		public Texture Value
		{
			get { return m_materialMode ? m_materialValue : m_defaultValue; }
			set
			{
				if( m_materialMode )
					m_materialValue = value;
				else
					m_defaultValue = value;
			}
		}

		public Texture MaterialValue
		{
			get { return m_materialValue; }
			set { m_materialValue = value; }
		}

		public Texture DefaultValue
		{
			get { return m_defaultValue; }
			set { m_defaultValue = value; }
		}

		public void SetInspectorName( string newName )
		{
			m_propertyInspectorName = newName;
		}

		public void SetPropertyName( string newName )
		{
			m_propertyName = newName;
		}

		public bool IsValid { get { return m_materialMode ? ( m_materialValue != null ) : ( m_defaultValue != null ); } }

		public virtual bool IsNormalMap { get { return m_isNormalMap; } }
		public bool IsLinearTexture { get { return m_linearTexture; } }

		public override void OnPropertyNameChanged()
		{
			base.OnPropertyNameChanged();
			UIUtils.UpdateTexturePropertyDataNode( UniqueId, PropertyInspectorName );
		}

		public override void SetGlobalValue() { Shader.SetGlobalTexture( m_propertyName, m_defaultValue ); }
		public override void FetchGlobalValue() { m_materialValue = Shader.GetGlobalTexture( m_propertyName ); }
		public override string DataToArray { get { return PropertyInspectorName; } }
		public TextureType CurrentType { get { return m_currentType; } }

		public bool DrawAutocast
		{
			get { return m_drawAutocast; }
			set { m_drawAutocast = value; }
		}

		public TexturePropertyValues DefaultTextureValue
		{
			get { return m_defaultTextureValue; }
			set { m_defaultTextureValue = value; }
		}

		public AutoCastType AutocastMode
		{
			get { return m_autocastMode; }
		}
	}
}
