// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public enum ShaderPropertyScope
	{
		Shader,
		SubShader,
		Pass
	}

	[Serializable]
	[NodeAttributes( "Template Parameter", "Constants And Properties", "Select and use one of the pre-existing properties given by the template" )]
	public sealed class TemplateShaderPropertyNode : TemplateNodeParent
	{
		private const string CurrentScopeStr = "Scope";
		private const string WarningStr = "Preview doesn't work with global variables";
		private const string PropertyLabelStr = "Parameter";
		private const string TypeLabelStr = "Type: ";
		private const string PropertyNameStr = "Property Name: ";

		private int IntPropertyId;
		private int FloatPropertyId;
		private int VectorPropertyId;
		private int Sampler2DPropertyId;
		private int Sampler3DPropertyId;
		private int SamplerCubePropertyId;

		[SerializeField]
		private int m_currentPropertyIdx = -1;

		[SerializeField]
		private string m_propertyName = string.Empty;

		[SerializeField]
		private int m_propertyNameId = 0;

		[SerializeField]
		private string m_typeName = string.Empty;

		[SerializeField]
		private string m_propertyNameLabel = string.Empty;

		private bool m_fetchPropertyId = false;
		private bool m_fetchScopeFromProperty = false;

		private List<TemplateShaderPropertyData> m_shaderProperties = null;
		private string[] m_propertyLabels = null;

		private UpperLeftWidgetHelper m_upperLeftWidgetHelper = new UpperLeftWidgetHelper();

		//Multi-Pass only properties
		[SerializeField]
		private ShaderPropertyScope m_currentScope = ShaderPropertyScope.Shader;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_previewShaderGUID = "4feb2016be0ece148b8bf234508f6aa4";
		}

		void FetchScope()
		{
			int shaderScopeCount = m_templateMPData.AvailableShaderProperties.Count;
			for( int i = 0; i < shaderScopeCount; i++ )
			{
				if( m_templateMPData.AvailableShaderProperties[ i ].PropertyName.Equals( m_propertyName ) )
				{
					m_currentScope = ShaderPropertyScope.Shader;
				}
			}

			int subShaderScopeCount = m_templateMPData.SubShaders[ SubShaderIdx ].AvailableShaderGlobals.Count;
			for( int i = 0; i < subShaderScopeCount; i++ )
			{
				if( m_templateMPData.SubShaders[ SubShaderIdx ].AvailableShaderGlobals[ i ].PropertyName.Equals( m_propertyName ) )
				{
					m_currentScope = ShaderPropertyScope.SubShader;
				}
			}

			int passScopeCount = m_templateMPData.SubShaders[ SubShaderIdx ].Passes[ PassIdx ].AvailableShaderGlobals.Count;
			for( int i = 0; i < passScopeCount; i++ )
			{
				if( m_templateMPData.SubShaders[ SubShaderIdx ].Passes[ PassIdx ].AvailableShaderGlobals[ i ].PropertyName.Equals( m_propertyName ) )
				{
					m_currentScope = ShaderPropertyScope.Pass;
				}
			}
		}

		void FetchShaderProperties()
		{
			if( m_templateMPData == null )
				m_templateMPData = ( m_containerGraph.CurrentMasterNode as TemplateMultiPassMasterNode ).CurrentTemplate;

			if( m_templateMPData != null )
			{
				switch( m_currentScope )
				{
					case ShaderPropertyScope.Shader:
					m_shaderProperties = m_templateMPData.AvailableShaderProperties;
					break;
					case ShaderPropertyScope.SubShader:
					m_shaderProperties = m_templateMPData.SubShaders[ SubShaderIdx ].AvailableShaderGlobals;
					break;
					case ShaderPropertyScope.Pass:
					m_shaderProperties = m_templateMPData.SubShaders[ SubShaderIdx ].Passes[ PassIdx ].AvailableShaderGlobals;
					break;
				}
			}
		}

		public override void OnEnable()
		{
			base.OnEnable();
			IntPropertyId = Shader.PropertyToID( "_IntData" );
			FloatPropertyId = Shader.PropertyToID( "_FloatData" );
			VectorPropertyId = Shader.PropertyToID( "_VectorData" );
			Sampler2DPropertyId = Shader.PropertyToID( "_Sampler2DData" );
			Sampler3DPropertyId = Shader.PropertyToID( "_Sampler3DData" );
			SamplerCubePropertyId = Shader.PropertyToID( "_SamplerCubeData" );
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			if( m_multiPassMode )
			{
				DrawMultipassProperties();
			}

			if( m_currentPropertyIdx > -1 )
			{

				bool hasProperties = ( m_shaderProperties != null && m_shaderProperties.Count > 0 );
				if( hasProperties )
				{
					EditorGUI.BeginChangeCheck();
					m_currentPropertyIdx = EditorGUILayoutPopup( PropertyLabelStr, m_currentPropertyIdx, m_propertyLabels );
					if( EditorGUI.EndChangeCheck() )
					{
						UpdateFromId();
					}
					EditorGUILayout.LabelField( m_typeName );
					if( m_shaderProperties[ m_currentPropertyIdx ].PropertyType != PropertyType.Global )
					{
						EditorGUILayout.LabelField( m_propertyNameLabel );
					}
				}
			}
		}

		void DrawMultipassProperties()
		{
			EditorGUI.BeginChangeCheck();
			m_currentScope = (ShaderPropertyScope)EditorGUILayoutEnumPopup( CurrentScopeStr, m_currentScope );
			if( EditorGUI.EndChangeCheck() )
			{
				FetchShaderProperties();
				FetchPropertyId();
			}

			bool showSubShader = false;
			bool showPass = false;
			switch( m_currentScope )
			{
				case ShaderPropertyScope.SubShader:
				{
					showSubShader = true;
				}
				break;
				case ShaderPropertyScope.Pass:
				{
					showSubShader = true;
					showPass = true;
				}
				break;
			}

			if( showSubShader )
			{
				DrawSubShaderUI();
			}

			if( showPass )
			{
				DrawPassUI();
			}
		}

		protected override void OnSubShaderChange()
		{
			FetchShaderProperties();
			FetchPropertyId();
		}

		protected override void OnPassChange()
		{
			FetchShaderProperties();
			FetchPropertyId();
		}

		override protected void CheckWarningState()
		{
			if( m_containerGraph.CurrentCanvasMode != NodeAvailability.TemplateShader )
			{
				ShowTab( NodeMessageType.Error, ErrorMessageStr );
			}
			else
			{
				if( m_shaderProperties != null &&
					m_shaderProperties.Count > 0 && 
					m_shaderProperties.Count > m_currentPropertyIdx &&
					m_shaderProperties[ m_currentPropertyIdx ].PropertyType == PropertyType.Global &&
					m_showPreview )
				{
					ShowTab( NodeMessageType.Info, WarningStr );
				}
				else
				{
					m_showErrorMessage = false;
				}
			}
		}

		public override void SetPreviewInputs()
		{
			if( m_containerGraph.CurrentCanvasMode != NodeAvailability.TemplateShader )
				return;

			if( m_shaderProperties == null || m_currentPropertyIdx >= m_shaderProperties.Count )
				return;

			if( m_shaderProperties[ m_currentPropertyIdx ].PropertyType == PropertyType.Global )
			{
				m_additionalContent.text = string.Empty;
				PreviewMaterial.SetInt( IntPropertyId, 0 );
				return;
			}

			Material currMat = m_containerGraph.CurrentMaterial;
			if( currMat != null && currMat.HasProperty( m_propertyNameId ) )
			{
				switch( m_shaderProperties[ m_currentPropertyIdx ].PropertyDataType )
				{
					case WirePortDataType.INT:
					{
						int value = currMat.GetInt( m_propertyNameId );
						SetAdditonalTitleText( string.Format( Constants.SubTitleValueFormatStr, GenerateTitle( value ) ) );
						PreviewMaterial.SetInt( IntPropertyId, value );
					}
					break;
					case WirePortDataType.FLOAT:
					{
						float value = currMat.GetFloat( m_propertyNameId );
						SetAdditonalTitleText( string.Format( Constants.SubTitleValueFormatStr, GenerateTitle( value ) ) );
						PreviewMaterial.SetFloat( FloatPropertyId, value );
					}
					break;
					case WirePortDataType.FLOAT4:
					{
						Vector4 value = currMat.GetVector( m_propertyNameId );
						SetAdditonalTitleText( string.Format( Constants.SubTitleValueFormatStr, GenerateTitle( value.x, value.y, value.z, value.w ) ) );
						PreviewMaterial.SetVector( VectorPropertyId, value );
					}
					break;
					case WirePortDataType.COLOR:
					{
						Color value = currMat.GetColor( m_propertyNameId );
						SetAdditonalTitleText( string.Format( Constants.SubTitleValueFormatStr, GenerateTitle( value.r, value.g, value.b, value.a ) ) );
						PreviewMaterial.SetColor( VectorPropertyId, value );
					}
					break;
					case WirePortDataType.SAMPLER2D:
					{
						Texture value = currMat.GetTexture( m_propertyNameId );
						if( value )
							SetAdditonalTitleText( string.Format( Constants.SubTitleValueFormatStr, value.name ) );
						else
							SetAdditonalTitleText( string.Empty );
						PreviewMaterial.SetTexture( Sampler2DPropertyId, value );
					}
					break;
					case WirePortDataType.SAMPLER3D:
					{
						Texture value = currMat.GetTexture( m_propertyNameId );
						if( value )
							SetAdditonalTitleText( string.Format( Constants.SubTitleValueFormatStr, value.name ) );
						else
							SetAdditonalTitleText( string.Empty );
						PreviewMaterial.SetTexture( Sampler3DPropertyId, value );
					}
					break;
					case WirePortDataType.SAMPLERCUBE:
					{
						Texture value = currMat.GetTexture( m_propertyNameId );
						if( value )
							SetAdditonalTitleText( string.Format( Constants.SubTitleValueFormatStr, value.name ) );
						else
							SetAdditonalTitleText( string.Empty );
						PreviewMaterial.SetTexture( SamplerCubePropertyId, value );
					}
					break;
				}
			}
			else
			{
				SetAdditonalTitleText( string.Empty );
			}
		}

		public override void Draw( DrawInfo drawInfo )
		{
			if( m_containerGraph.CurrentCanvasMode != NodeAvailability.TemplateShader )
			{
				if( !m_showErrorMessage || m_errorMessageTypeIsError == NodeMessageType.Info )
				{
					ShowTab( NodeMessageType.Error, ErrorMessageStr );
				}
			}
			else if( m_showErrorMessage )
			{
				if( m_errorMessageTypeIsError == NodeMessageType.Error )
					HideTab();
			}

			base.Draw( drawInfo );
			if( m_containerGraph.CurrentCanvasMode != NodeAvailability.TemplateShader )
				return;

			if( m_shaderProperties == null )
			{
				MasterNode masterNode = m_containerGraph.CurrentMasterNode;
				if( masterNode.CurrentMasterNodeCategory == AvailableShaderTypes.Template )
				{
					if( SetTemplate( masterNode ) )
					{
						m_fetchPropertyId = true;
					}
				}
			}

			if( m_fetchScopeFromProperty )
			{
				m_fetchScopeFromProperty = false;
				FetchScope();
				FetchShaderProperties();
			}

			if( m_fetchPropertyId )
			{
				m_fetchPropertyId = false;
				FetchPropertyId();
			}

			if( m_currentPropertyIdx > -1 )
			{
				EditorGUI.BeginChangeCheck();
				m_currentPropertyIdx = m_upperLeftWidgetHelper.DrawWidget( this, m_currentPropertyIdx, m_propertyLabels );
				if( EditorGUI.EndChangeCheck() )
				{
					UpdateFromId();
				}
			}
		}

		void FetchPropertyId()
		{
			if( m_shaderProperties != null )
			{
				m_currentPropertyIdx = 0;
				m_propertyLabels = new string[ m_shaderProperties.Count ];
				for( int i = 0; i < m_shaderProperties.Count; i++ )
				{
					if( m_shaderProperties[ i ].PropertyName.Equals( m_propertyName ) )
					{
						m_currentPropertyIdx = i;
					}
					m_propertyLabels[ i ] = m_shaderProperties[ i ].PropertyInspectorName;
				}
				UpdateFromId();
			}
			else
			{
				m_currentPropertyIdx = -1;
			}
		}

		void UpdateFromId()
		{

			if( m_shaderProperties != null )
			{
				if( m_shaderProperties.Count == 0 )
				{
					for( int i = 0; i < 4; i++ )
						m_containerGraph.DeleteConnection( false, UniqueId, i, false, true );

					m_headerColor = UIUtils.GetColorFromCategory( "Default" );
					m_content.text = "None";
					m_additionalContent.text = string.Empty;
					m_previewMaterialPassId = 1;
					PreviewMaterial.SetFloat( FloatPropertyId, 0 );
					m_showPreview = false;
					m_drawPreviewExpander = false;
					m_outputPorts[ 0 ].ChangeProperties( "None", WirePortDataType.FLOAT, false );
					ConfigurePorts();
					return;
				}

				m_drawPreviewExpander = true;
				bool areCompatible = TemplateHelperFunctions.CheckIfCompatibles( m_outputPorts[ 0 ].DataType, m_shaderProperties[ m_currentPropertyIdx ].PropertyDataType );
				switch( m_shaderProperties[ m_currentPropertyIdx ].PropertyDataType )
				{
					case WirePortDataType.SAMPLER1D:
					case WirePortDataType.SAMPLER2D:
					case WirePortDataType.SAMPLER3D:
					case WirePortDataType.SAMPLERCUBE:
					m_outputPorts[ 0 ].ChangeProperties( "Tex", m_shaderProperties[ m_currentPropertyIdx ].PropertyDataType, false );
					m_headerColor = UIUtils.GetColorFromCategory( "Textures" );
					break;
					case WirePortDataType.INT:
					case WirePortDataType.FLOAT:
					m_outputPorts[ 0 ].ChangeProperties( Constants.EmptyPortValue, m_shaderProperties[ m_currentPropertyIdx ].PropertyDataType, false );
					m_headerColor = UIUtils.GetColorFromCategory( "Constants And Properties" );
					break;
					case WirePortDataType.FLOAT4:
					m_outputPorts[ 0 ].ChangeProperties( "XYZW", m_shaderProperties[ m_currentPropertyIdx ].PropertyDataType, false );
					m_headerColor = UIUtils.GetColorFromCategory( "Constants And Properties" );
					break;
					case WirePortDataType.COLOR:
					m_outputPorts[ 0 ].ChangeProperties( "RGBA", m_shaderProperties[ m_currentPropertyIdx ].PropertyDataType, false );
					m_headerColor = UIUtils.GetColorFromCategory( "Constants And Properties" );
					break;
					default:
					case WirePortDataType.OBJECT:
					case WirePortDataType.FLOAT3x3:
					case WirePortDataType.FLOAT4x4:
					m_outputPorts[ 0 ].ChangeProperties( "Out", m_shaderProperties[ m_currentPropertyIdx ].PropertyDataType, false );
					m_headerColor = UIUtils.GetColorFromCategory( "Constants And Properties" );
					break;
				}

				if( !areCompatible )
				{
					for( int i = 0; i < 4; i++ )
						m_containerGraph.DeleteConnection( false, UniqueId, i, false, true );
				}

				ConfigurePorts();

				m_propertyName = m_shaderProperties[ m_currentPropertyIdx ].PropertyName;
				m_content.text = m_shaderProperties[ m_currentPropertyIdx ].PropertyInspectorName;
				m_propertyNameId = Shader.PropertyToID( m_propertyName );
				m_typeName = TypeLabelStr + m_shaderProperties[ m_currentPropertyIdx ].PropertyType.ToString();
				if( m_shaderProperties[ m_currentPropertyIdx ].PropertyType != PropertyType.Global )
				{
					m_propertyNameLabel = PropertyNameStr + m_shaderProperties[ m_currentPropertyIdx ].PropertyName;
				}

				m_sizeIsDirty = true;
				Material currMat = m_containerGraph.CurrentMaterial;
				if( currMat != null )
				{
					if( m_shaderProperties[ m_currentPropertyIdx ].PropertyType == PropertyType.Global )
					{
						m_previewMaterialPassId = 0;
						if( !m_showErrorMessage && m_showPreview )
						{
							ShowTab( NodeMessageType.Info, WarningStr );
						}
					}
					else
					{
						if( m_showErrorMessage && m_errorMessageTypeIsError != NodeMessageType.Error )
						{
							HideTab();
						}
						switch( m_shaderProperties[ m_currentPropertyIdx ].PropertyDataType )
						{
							case WirePortDataType.INT: m_previewMaterialPassId = 0; break;
							case WirePortDataType.FLOAT: m_previewMaterialPassId = 1; break;
							case WirePortDataType.FLOAT4:
							case WirePortDataType.COLOR: m_previewMaterialPassId = 2; break;
							case WirePortDataType.SAMPLER2D: m_previewMaterialPassId = 3; break;
							case WirePortDataType.SAMPLER3D: m_previewMaterialPassId = 4; break;
							case WirePortDataType.SAMPLERCUBE: m_previewMaterialPassId = 5; break;
							default: PreviewMaterial.SetPass( 0 ); break;
						}
					}
				}

				CheckWarningState();
			}
		}

		string GenerateTitle( params float[] values )
		{
			//string finalResult = "( ";
			string finalResult = string.Empty;
			if( values.Length == 1 )
			{
				finalResult += values[ 0 ].ToString( Mathf.Abs( values[ 0 ] ) > 1000 ? Constants.PropertyBigFloatFormatLabel : Constants.PropertyFloatFormatLabel );
			}
			else
			{
				for( int i = 0; i < values.Length; i++ )
				{
					finalResult += values[ i ].ToString( Mathf.Abs( values[ i ] ) > 1000 ? Constants.PropertyBigVectorFormatLabel : Constants.PropertyVectorFormatLabel );
					if( i < ( values.Length - 1 ) )
						finalResult += ",";
				}
			}
			//finalResult += " )";
			return finalResult;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( dataCollector.MasterNodeCategory != AvailableShaderTypes.Template )
			{
				UIUtils.ShowMessage( "Template Parameter node is only intended for templates use only" );
				return m_outputPorts[ outputId ].ErrorValue;
			}

			if( m_shaderProperties == null || m_shaderProperties.Count ==0  )
			{
				UIUtils.ShowMessage( "Attempting to fetch inexistant parameter on " + m_nodeAttribs.Name +" node");
				return m_outputPorts[ outputId ].ErrorValue;
			}

			if( m_multiPassMode )
			{
				switch( m_currentScope )
				{
					case ShaderPropertyScope.SubShader:
					{
						if( dataCollector.TemplateDataCollectorInstance.MultipassSubshaderIdx != SubShaderIdx )
						{
							UIUtils.ShowMessage( string.Format( "{0} is only intended for subshader {1}", m_propertyLabels[ m_currentPropertyIdx ], SubShaderIdx ) );
							return m_outputPorts[ outputId ].ErrorValue;
						}
					}
					break;
					case ShaderPropertyScope.Pass:
					{
						if( dataCollector.TemplateDataCollectorInstance.MultipassSubshaderIdx != SubShaderIdx ||
							dataCollector.TemplateDataCollectorInstance.MultipassPassIdx != PassIdx
							)
						{
							UIUtils.ShowMessage( string.Format( "{0} is only intended for subshader {1} and pass {2}", m_propertyLabels[ m_currentPropertyIdx ], SubShaderIdx, PassIdx ) );
							return m_outputPorts[ outputId ].ErrorValue;
						}
					}
					break;
				}
			}

			return GetOutputVectorItem( 0, outputId, m_propertyName );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_propertyName = GetCurrentParam( ref nodeParams );
			m_propertyNameId = Shader.PropertyToID( m_propertyName );
			if( UIUtils.CurrentShaderVersion() > TemplatesManager.MPShaderVersion )
			{
				m_currentScope = (ShaderPropertyScope)Enum.Parse( typeof( ShaderPropertyScope ), GetCurrentParam( ref nodeParams ) );
			}
			else
			{
				m_fetchScopeFromProperty = true;
			}
			m_fetchPropertyId = true;
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_propertyName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_currentScope );
		}

		public override void OnMasterNodeReplaced( MasterNode newMasterNode )
		{
			base.OnMasterNodeReplaced( newMasterNode );
			if( newMasterNode.CurrentMasterNodeCategory == AvailableShaderTypes.Template )
			{
				SetTemplate( newMasterNode );
				if( m_fetchScopeFromProperty )
				{
					m_fetchScopeFromProperty = false;
					FetchScope();
				}
				FetchShaderProperties();
				FetchPropertyId();
				//m_containerGraph.DeleteConnection( false, UniqueId, 0, false, true );
			}
		}

		bool SetTemplate( MasterNode newMasterNode )
		{
			if( m_containerGraph.MultiPassMasterNodes.NodesList.Count > 0 )
			{
				m_multiPassMode = true;
				TemplateMultiPassMasterNode templateMasterNode = ( newMasterNode as TemplateMultiPassMasterNode );
				if( templateMasterNode != null )
				{
					m_templateMPData = templateMasterNode.CurrentTemplate;
					UpdateSubShaderAmount();
					FetchShaderProperties();
					return true;
				}
			}
			else
			{
				m_multiPassMode = false;
				TemplateMasterNode templateMasterNode = ( newMasterNode as TemplateMasterNode );
				if( templateMasterNode != null )
				{
					m_shaderProperties = templateMasterNode.CurrentTemplate.AvailableShaderProperties;
					return true;
				}
			}
			return false;
		}

		public override void RefreshExternalReferences()
		{
			base.RefreshExternalReferences();
			CheckWarningState();
		}

		public override void Destroy()
		{
			base.Destroy();
			m_propertyLabels = null;
			m_shaderProperties = null;
			m_upperLeftWidgetHelper = null;
		}
	}
}
