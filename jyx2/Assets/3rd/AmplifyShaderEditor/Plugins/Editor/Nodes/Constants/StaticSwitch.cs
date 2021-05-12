// Amplify Shader Editor - Visual Shader vEditing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Static Switch", "Logical Operators", "Creates a shader keyword toggle", Available = true )]
	public sealed class StaticSwitch : PropertyNode
	{
		[SerializeField]
		private int m_defaultValue = 0;

		[SerializeField]
		private int m_materialValue = 0;

		[SerializeField]
		private int m_multiCompile = 0;

		[SerializeField]
		private int m_currentKeywordId = 0;

		[SerializeField]
		private string m_currentKeyword = string.Empty;

		[SerializeField]
		private bool m_createToggle = true;

		private GUIContent m_checkContent;
		private GUIContent m_popContent;

		private int m_conditionId = -1;

		private const int MinComboSize = 50;
		private const int MaxComboSize = 105;

		private Rect m_varRect;
		private Rect m_imgRect;
		private bool m_editing;

		enum KeywordModeType
		{
			Toggle = 0,
			ToggleOff,
			KeywordEnum,
		}

		[SerializeField]
		private KeywordModeType m_keywordModeType = KeywordModeType.Toggle;

		private const string StaticSwitchStr = "Static Switch";
		private const string MaterialToggleStr = "Material Toggle";

		private const string ToggleMaterialValueStr = "Material Value";
		private const string ToggleDefaultValueStr = "Default Value";

		private const string AmountStr = "Amount";
		private const string KeywordStr = "Keyword";
		private const string CustomStr = "Custom";
		private const string ToggleTypeStr = "Toggle Type";
		private const string TypeStr = "Type";
		private const string ModeStr = "Mode";
		private const string KeywordTypeStr = "Keyword Type";

		private const string KeywordNameStr = "Keyword Name";
		public readonly static string[] KeywordTypeList = { "Shader Feature", "Multi Compile"/*, "Define Symbol"*/ };
		public readonly static int[] KeywordTypeInt = { 0, 1/*, 2*/ };

		[SerializeField]
		private string[] m_defaultKeywordNames = { "Key0", "Key1", "Key2", "Key3", "Key4", "Key5", "Key6", "Key7", "Key8" };

		[SerializeField]
		private string[] m_keywordEnumList = { "Key0", "Key1" };

		int m_keywordEnumAmount = 2;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			AddInputPort( WirePortDataType.FLOAT, false, "False", -1, MasterNodePortCategory.Fragment, 1 );
			AddInputPort( WirePortDataType.FLOAT, false, "True", -1, MasterNodePortCategory.Fragment, 0 );
			for( int i = 2; i < 9; i++ )
			{
				AddInputPort( WirePortDataType.FLOAT, false, m_defaultKeywordNames[ i ] );
				m_inputPorts[ i ].Visible = false;
			}
			m_headerColor = new Color( 0.0f, 0.55f, 0.45f, 1f );
			m_customPrefix = KeywordStr+" ";
			m_autoWrapProperties = false;
			m_freeType = false;
			m_useVarSubtitle = true;
			m_allowPropertyDuplicates = true;
			m_showTitleWhenNotEditing = false;
			m_currentParameterType = PropertyType.Property;

			m_checkContent = new GUIContent();
			m_checkContent.image = UIUtils.CheckmarkIcon;

			m_popContent = new GUIContent();
			m_popContent.image = UIUtils.PopupIcon;

			m_previewShaderGUID = "0b708c11c68e6a9478ac97fe3643eab1";
			m_showAutoRegisterUI = true;
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if( m_conditionId == -1 )
				m_conditionId = Shader.PropertyToID( "_Condition" );

			if( m_createToggle )
				PreviewMaterial.SetInt( m_conditionId, m_materialValue );
			else
				PreviewMaterial.SetInt( m_conditionId, m_defaultValue );
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			if( m_createToggle )
				UIUtils.RegisterPropertyNode( this );
			else
				UIUtils.UnregisterPropertyNode( this );
		}

		public override void Destroy()
		{
			base.Destroy();
			UIUtils.UnregisterPropertyNode( this );
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			UpdateConnections();
		}

		public override void OnConnectedOutputNodeChanges( int inputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( inputPortId, otherNodeId, otherPortId, name, type );
			UpdateConnections();
		}

		public override void OnInputPortDisconnected( int portId )
		{
			base.OnInputPortDisconnected( portId );
			UpdateConnections();
		}

		private void UpdateConnections()
		{
			WirePortDataType mainType = WirePortDataType.FLOAT;

			int highest = UIUtils.GetPriority( mainType );
			for( int i = 0; i < m_inputPorts.Count; i++ )
			{
				if( m_inputPorts[ i ].IsConnected )
				{
					WirePortDataType portType = m_inputPorts[ i ].GetOutputConnection().DataType;
					if( UIUtils.GetPriority( portType ) > highest )
					{
						mainType = portType;
						highest = UIUtils.GetPriority( portType );
					}
				}
			}

			for( int i = 0; i < m_inputPorts.Count; i++ )
			{
				m_inputPorts[ i ].ChangeType( mainType, false );
			}

			m_outputPorts[ 0 ].ChangeType( mainType, false );
		}

		public override string GetPropertyValue()
		{
			if( m_createToggle )
				if( m_keywordModeType == KeywordModeType.KeywordEnum && m_keywordEnumAmount > 0 )
					return PropertyAttributes + "[" + m_keywordModeType.ToString() + "(" + GetKeywordEnumPropertyList() + ")] " + m_propertyName + "(\"" + m_propertyInspectorName + "\", Float) = " + m_defaultValue;
				else
					return PropertyAttributes + "[" + m_keywordModeType.ToString() + "(" + GetPropertyValStr() + ")] " + m_propertyName + "(\"" + m_propertyInspectorName + "\", Float) = " + m_defaultValue;
			else
				return string.Empty;
		}
		public string KeywordEnumList(int index)
		{
			if( m_variableMode == VariableMode.Fetch )
				return m_keywordEnumList[index];
			else
				return m_keywordEnumList[index].ToUpper();
			
		}
		public override string PropertyName
		{
			get
			{
				if( m_variableMode == VariableMode.Fetch )
					return m_currentKeyword;
				else
					return base.PropertyName.ToUpper();
			}
		}

		public override string GetPropertyValStr()
		{
			if( m_keywordModeType == KeywordModeType.KeywordEnum )
				return PropertyName;
			else if( m_variableMode == VariableMode.Fetch )
				return m_currentKeyword;
			else
				return PropertyName + ( m_createToggle ? OnOffStr : "_ON" );
		}

		private string GetKeywordEnumPropertyList()
		{
			string result = string.Empty;
			for( int i = 0; i < m_keywordEnumList.Length; i++ )
			{
				if( i == 0 )
					result = m_keywordEnumList[ i ];
				else
					result += "," + m_keywordEnumList[ i ];
			}
			return result;
		}

		private string GetKeywordEnumPragmaList()
		{
			string result = string.Empty;
			for( int i = 0; i < m_keywordEnumList.Length; i++ )
			{
				if( i == 0 )
					result = PropertyName + "_" + KeywordEnumList(i);
				else
					result += " " + PropertyName + "_" + KeywordEnumList( i );
			}
			return result;
		}

		public override string GetUniformValue()
		{
			return string.Empty;
		}

		public override bool GetUniformData( out string dataType, out string dataName )
		{
			dataType = string.Empty;
			dataName = string.Empty;
			return false;
		}

		public override void DrawProperties()
		{
			//base.DrawProperties();
			NodeUtils.DrawPropertyGroup( ref m_propertiesFoldout, Constants.ParameterLabelStr, PropertyGroup );
			NodeUtils.DrawPropertyGroup( ref m_visibleCustomAttrFoldout, CustomAttrStr, DrawCustomAttributes, DrawCustomAttrAddRemoveButtons );
			CheckPropertyFromInspector();
		}

		void DrawEnumList()
		{
			EditorGUI.BeginChangeCheck();
			m_keywordEnumAmount = EditorGUILayoutIntSlider( AmountStr, m_keywordEnumAmount, 2, 9 );
			if( EditorGUI.EndChangeCheck() )
			{
				CurrentSelectedInput = Mathf.Clamp( CurrentSelectedInput, 0, m_keywordEnumAmount - 1 );
				UpdateLabels();
			}
			EditorGUI.indentLevel++;
			for( int i = 0; i < m_keywordEnumList.Length; i++ )
			{
				EditorGUI.BeginChangeCheck();
				m_keywordEnumList[ i ] = EditorGUILayoutTextField( "Item " + i, m_keywordEnumList[ i ] );
				if( EditorGUI.EndChangeCheck() )
				{
					m_keywordEnumList[ i ] = UIUtils.RemoveInvalidEnumCharacters( m_keywordEnumList[ i ] );
					m_keywordEnumList[ i ] = m_keywordEnumList[ i ].Replace( " ", "" ); // sad face :( does not support spaces
					m_inputPorts[ i ].Name = m_keywordEnumList[ i ];
					m_defaultKeywordNames[ i ] = m_inputPorts[ i ].Name;
				}
			}
			EditorGUI.indentLevel--;
		}

		public void UpdateLabels()
		{
			int maxinputs = m_keywordModeType == KeywordModeType.KeywordEnum ? m_keywordEnumAmount : 2;
			m_keywordEnumAmount = Mathf.Clamp( m_keywordEnumAmount, 0, maxinputs );
			m_keywordEnumList = new string[ maxinputs ];

			for( int i = 0; i < maxinputs; i++ )
			{
				m_keywordEnumList[ i ] = m_defaultKeywordNames[ i ];
				m_inputPorts[ i ].Name = m_keywordEnumList[ i ];
			}

			if( m_keywordModeType != KeywordModeType.KeywordEnum )
			{
				m_inputPorts[ 0 ].Name = "False";
				m_inputPorts[ 1 ].Name = "True";
			}

			for( int i = 0; i < m_inputPorts.Count; i++ )
			{
				m_inputPorts[ i ].Visible = ( i < maxinputs );
			}
			m_sizeIsDirty = true;
		}

		void PropertyGroup()
		{
			EditorGUI.BeginChangeCheck();
			m_keywordModeType = (KeywordModeType)EditorGUILayoutEnumPopup( TypeStr, m_keywordModeType );
			if( EditorGUI.EndChangeCheck() )
			{
				UpdateLabels();
			}

			m_variableMode = (VariableMode)EditorGUILayoutEnumPopup( ModeStr, m_variableMode );

			if( m_variableMode == VariableMode.Create )
			{
				EditorGUI.BeginChangeCheck();
				m_multiCompile = EditorGUILayoutIntPopup( KeywordTypeStr, m_multiCompile, KeywordTypeList, KeywordTypeInt );
				if( EditorGUI.EndChangeCheck() )
				{
					BeginPropertyFromInspectorCheck();
				}
			}

			if( m_keywordModeType != KeywordModeType.KeywordEnum )
			{
				if( m_variableMode == VariableMode.Create )
				{
					ShowPropertyInspectorNameGUI();
					ShowPropertyNameGUI( true );
					bool guiEnabledBuffer = GUI.enabled;
					GUI.enabled = false;
					EditorGUILayout.TextField( KeywordNameStr, GetPropertyValStr() );
					GUI.enabled = guiEnabledBuffer;
				}
				else
				{
					ShowPropertyInspectorNameGUI();
					EditorGUI.BeginChangeCheck();
					m_currentKeywordId = EditorGUILayoutPopup( KeywordStr, m_currentKeywordId, UIUtils.AvailableKeywords );
					if( EditorGUI.EndChangeCheck() )
					{
						if( m_currentKeywordId != 0 )
						{
							m_currentKeyword = UIUtils.AvailableKeywords[ m_currentKeywordId ];
						}
					}

					if( m_currentKeywordId == 0 )
					{
						EditorGUI.BeginChangeCheck();
						m_currentKeyword = EditorGUILayoutTextField( CustomStr, m_currentKeyword );
						if( EditorGUI.EndChangeCheck() )
						{
							m_currentKeyword = UIUtils.RemoveInvalidCharacters( m_currentKeyword );
						}
					}
				}
			}
			else
			{
				ShowPropertyInspectorNameGUI();
				ShowPropertyNameGUI( true );
				DrawEnumList();
			}

			ShowAutoRegister();
			EditorGUI.BeginChangeCheck();
			m_createToggle = EditorGUILayoutToggle( MaterialToggleStr, m_createToggle );
			if( EditorGUI.EndChangeCheck() )
			{
				if( m_createToggle )
					UIUtils.RegisterPropertyNode( this );
				else
					UIUtils.UnregisterPropertyNode( this );
			}

			if( m_createToggle )
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space( 20 );
				m_propertyTab = GUILayout.Toolbar( m_propertyTab, LabelToolbarTitle );
				EditorGUILayout.EndHorizontal();
				switch( m_propertyTab )
				{
					default:
					case 0:
					{
						EditorGUI.BeginChangeCheck();
						if( m_keywordModeType != KeywordModeType.KeywordEnum )
							m_materialValue = EditorGUILayoutToggle( ToggleMaterialValueStr, m_materialValue == 1 ) ? 1 : 0;
						else
							m_materialValue = EditorGUILayoutPopup( ToggleMaterialValueStr, m_materialValue, m_keywordEnumList );
						if( EditorGUI.EndChangeCheck() )
							m_requireMaterialUpdate = true;
					}
					break;
					case 1:
					{
						if( m_keywordModeType != KeywordModeType.KeywordEnum )
							m_defaultValue = EditorGUILayoutToggle( ToggleDefaultValueStr, m_defaultValue == 1 ) ? 1 : 0;
						else
							m_defaultValue = EditorGUILayoutPopup( ToggleDefaultValueStr, m_materialValue, m_keywordEnumList );
					}
					break;
				}
			}

			//EditorGUILayout.HelpBox( "Keyword Type:\n" +
			//	"The difference is that unused variants of \"Shader Feature\" shaders will not be included into game build while \"Multi Compile\" variants are included regardless of their usage.\n\n" +
			//	"So \"Shader Feature\" makes most sense for keywords that will be set on the materials, while \"Multi Compile\" for keywords that will be set from code globally.\n\n" +
			//	"You can set keywords using the material property using the \"Property Name\" or you can set the keyword directly using the \"Keyword Name\".", MessageType.None );
		}


		public override void OnNodeLayout( DrawInfo drawInfo )
		{
			float finalSize = 0;
			if( m_keywordModeType == KeywordModeType.KeywordEnum )
			{
				GUIContent dropdown = new GUIContent( m_inputPorts[ CurrentSelectedInput ].Name );
				int cacheSize = UIUtils.GraphDropDown.fontSize;
				UIUtils.GraphDropDown.fontSize = 10;
				Vector2 calcSize = UIUtils.GraphDropDown.CalcSize( dropdown );
				UIUtils.GraphDropDown.fontSize = cacheSize;
				finalSize = Mathf.Clamp( calcSize.x, MinComboSize, MaxComboSize );
				if( m_insideSize.x != finalSize )
				{
					m_insideSize.Set( finalSize, 25 );
					m_sizeIsDirty = true;
				}
			}

			base.OnNodeLayout( drawInfo );

			if( m_keywordModeType != KeywordModeType.KeywordEnum )
			{
				m_varRect = m_remainingBox;
				m_varRect.size = Vector2.one * 22 * drawInfo.InvertedZoom;
				m_varRect.center = m_remainingBox.center;
				if( m_showPreview )
					m_varRect.y = m_remainingBox.y;
			}
			else
			{
				m_varRect = m_remainingBox;
				m_varRect.width = finalSize * drawInfo.InvertedZoom;
				m_varRect.height = 16 * drawInfo.InvertedZoom;
				m_varRect.x = m_remainingBox.xMax - m_varRect.width;
				m_varRect.y += 1 * drawInfo.InvertedZoom;

				m_imgRect = m_varRect;
				m_imgRect.x = m_varRect.xMax - 16 * drawInfo.InvertedZoom;
				m_imgRect.width = 16 * drawInfo.InvertedZoom;
				m_imgRect.height = m_imgRect.width;
			}
		}

		public override void DrawGUIControls( DrawInfo drawInfo )
		{
			base.DrawGUIControls( drawInfo );

			if( drawInfo.CurrentEventType != EventType.MouseDown || !m_createToggle )
				return;

			if( m_varRect.Contains( drawInfo.MousePosition ) )
			{
				m_editing = true;
			}
			else if( m_editing )
			{
				m_editing = false;
			}
		}

		private int CurrentSelectedInput
		{
			get
			{
				return m_materialMode ? m_materialValue : m_defaultValue;
			}
			set
			{
				if( m_materialMode )
					m_materialValue = value;
				else
					m_defaultValue = value;
			}
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );

			if( m_editing )
			{
				if( m_keywordModeType != KeywordModeType.KeywordEnum )
				{
					if( GUI.Button( m_varRect, GUIContent.none, UIUtils.GraphButton ) )
					{
						CurrentSelectedInput = CurrentSelectedInput == 1 ? 0 : 1;
						m_editing = false;
						if( m_materialMode )
							m_requireMaterialUpdate = true;
					}

					if( CurrentSelectedInput == 1 )
					{
						GUI.Label( m_varRect, m_checkContent, UIUtils.GraphButtonIcon );
					}
				}
				else
				{
					EditorGUI.BeginChangeCheck();
					CurrentSelectedInput = EditorGUIPopup( m_varRect, CurrentSelectedInput, m_keywordEnumList, UIUtils.GraphDropDown );
					if( EditorGUI.EndChangeCheck() )
					{
						m_editing = false;
						if( m_materialMode )
							m_requireMaterialUpdate = true;
					}
				}
			}
		}

		public override void OnNodeRepaint( DrawInfo drawInfo )
		{
			base.OnNodeRepaint( drawInfo );

			if( !m_isVisible )
				return;

			if( m_createToggle && ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD2 )
			{
				if( !m_editing )
				{
					if( m_keywordModeType != KeywordModeType.KeywordEnum )
					{
						GUI.Label( m_varRect, GUIContent.none, UIUtils.GraphButton );

						if( CurrentSelectedInput == 1 )
							GUI.Label( m_varRect, m_checkContent, UIUtils.GraphButtonIcon );
					}
					else
					{
						GUI.Label( m_varRect, m_keywordEnumList[ CurrentSelectedInput ], UIUtils.GraphDropDown );
						GUI.Label( m_imgRect, m_popContent, UIUtils.GraphButtonIcon );
					}
				}
			}
		}

		private string OnOffStr
		{
			get
			{
				switch( m_keywordModeType )
				{
					default:
					case KeywordModeType.Toggle:
					return "_ON";
					case KeywordModeType.ToggleOff:
					return "_OFF";
				}
			}
		}

		void RegisterPragmas( ref MasterNodeDataCollector dataCollector )
		{
			if( m_variableMode == VariableMode.Create )
			{
				if( m_keywordModeType == KeywordModeType.KeywordEnum )
				{
					if( m_multiCompile == 1 )
						dataCollector.AddToPragmas( UniqueId, "multi_compile " + GetKeywordEnumPragmaList() );
					else if( m_multiCompile == 0 )
						dataCollector.AddToPragmas( UniqueId, "shader_feature " + GetKeywordEnumPragmaList() );
				}
				else
				{
					if( m_multiCompile == 1 )
						dataCollector.AddToPragmas( UniqueId, "multi_compile __ " + PropertyName + OnOffStr );
					else if( m_multiCompile == 0 )
						dataCollector.AddToPragmas( UniqueId, "shader_feature " + PropertyName + OnOffStr );
				}
			}
		}

		protected override void RegisterProperty( ref MasterNodeDataCollector dataCollector )
		{
			base.RegisterProperty( ref dataCollector );
			RegisterPragmas( ref dataCollector );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );

			//if( m_keywordModeType == KeywordModeType.KeywordEnum )

			RegisterPragmas( ref dataCollector );

			string outType = UIUtils.PrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType );

			if( m_keywordModeType == KeywordModeType.KeywordEnum )
			{
				string defaultKey = "\t" + outType + " staticSwitch" + OutputId + " = " + m_inputPorts[ m_defaultValue ].GeneratePortInstructions( ref dataCollector ) + ";";

				string[] allOutputs = new string[ m_keywordEnumAmount ];
				for( int i = 0; i < m_keywordEnumAmount; i++ )
					allOutputs[ i ] = m_inputPorts[ i ].GeneratePortInstructions( ref dataCollector );

				for( int i = 0; i < m_keywordEnumAmount; i++ )
				{
					string keyword = PropertyName + "_" + KeywordEnumList( i );
					if( i == 0 )
						dataCollector.AddLocalVariable( UniqueId, "#if defined(" + keyword + ")", true );
					else
						dataCollector.AddLocalVariable( UniqueId, "#elif defined(" + keyword + ")", true );

					if( m_defaultValue == i )
						dataCollector.AddLocalVariable( UniqueId, defaultKey, true );
					else
						dataCollector.AddLocalVariable( UniqueId, "\t" + outType + " staticSwitch" + OutputId + " = " + allOutputs[ i ] + ";", true );
				}
				dataCollector.AddLocalVariable( UniqueId, "#else", true );
				dataCollector.AddLocalVariable( UniqueId, defaultKey, true );
				dataCollector.AddLocalVariable( UniqueId, "#endif", true );
			}
			else
			{
				string falseCode = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
				string trueCode = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );

				if( m_variableMode == VariableMode.Fetch )
					dataCollector.AddLocalVariable( UniqueId, "#ifdef " + m_currentKeyword, true );
				else
					dataCollector.AddLocalVariable( UniqueId, "#ifdef " + PropertyName + OnOffStr, true );
				dataCollector.AddLocalVariable( UniqueId, "\t" + outType + " staticSwitch" + OutputId + " = " + trueCode + ";", true );
				dataCollector.AddLocalVariable( UniqueId, "#else", true );
				dataCollector.AddLocalVariable( UniqueId, "\t" + outType + " staticSwitch" + OutputId + " = " + falseCode + ";", true );
				dataCollector.AddLocalVariable( UniqueId, "#endif", true );
			}

			m_outputPorts[ 0 ].SetLocalValue( "staticSwitch" + OutputId , dataCollector.PortCategory );
			return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
		}

		public override void DrawTitle( Rect titlePos )
		{
			SetAdditonalTitleTextOnCallback( GetPropertyValStr(), ( instance, newSubTitle ) => instance.AdditonalTitleContent.text = string.Format( Constants.SubTitleVarNameFormatStr, newSubTitle ) );

			if( !m_isEditing && ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD3 )
			{
				GUI.Label( titlePos, StaticSwitchStr, UIUtils.GetCustomStyle( CustomStyle.NodeTitle ) );
			}
		}

		public override void UpdateMaterial( Material mat )
		{
			base.UpdateMaterial( mat );
			if( UIUtils.IsProperty( m_currentParameterType ) && !InsideShaderFunction )
			{
				if( m_keywordModeType == KeywordModeType.KeywordEnum )
				{
					for( int i = 0; i < m_keywordEnumAmount; i++ )
					{
						string key = PropertyName + "_" + KeywordEnumList( i );
						mat.DisableKeyword( key );
					}
					mat.EnableKeyword( PropertyName + "_" + KeywordEnumList( m_materialValue ));
				}
				else
				{
					int final = m_materialValue;
					if( m_keywordModeType == KeywordModeType.ToggleOff )
						final = final == 1 ? 0 : 1;
					mat.SetFloat( m_propertyName, m_materialValue );
					if( final == 1 )
						mat.EnableKeyword( GetPropertyValStr() );
					else
						mat.DisableKeyword( GetPropertyValStr() );
				}
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
			m_multiCompile = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			if( UIUtils.CurrentShaderVersion() > 14403 )
			{
				m_defaultValue = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				if( UIUtils.CurrentShaderVersion() > 14101 )
				{
					m_materialValue = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				}
			}
			else
			{
				m_defaultValue = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) ) ? 1 : 0;
				if( UIUtils.CurrentShaderVersion() > 14101 )
				{
					m_materialValue = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) ) ? 1 : 0;
				}
			}

			if( UIUtils.CurrentShaderVersion() > 13104 )
			{
				m_createToggle = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
				m_currentKeyword = GetCurrentParam( ref nodeParams );
				m_currentKeywordId = UIUtils.GetKeywordId( m_currentKeyword );
			}
			if( UIUtils.CurrentShaderVersion() > 14001 )
			{
				m_keywordModeType = (KeywordModeType)Enum.Parse( typeof( KeywordModeType ), GetCurrentParam( ref nodeParams ) );
			}

			if( UIUtils.CurrentShaderVersion() > 14403 )
			{
				m_keywordEnumAmount = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				for( int i = 0; i < m_keywordEnumAmount; i++ )
				{
					m_defaultKeywordNames[ i ] = GetCurrentParam( ref nodeParams );
				}

				UpdateLabels();
			}

			if( m_createToggle )
				UIUtils.RegisterPropertyNode( this );
			else
				UIUtils.UnregisterPropertyNode( this );
		}

		public override void ReadFromDeprecated( ref string[] nodeParams, Type oldType = null )
		{
			base.ReadFromDeprecated( ref nodeParams, oldType );
			{
				m_currentKeyword = GetCurrentParam( ref nodeParams );
				m_currentKeywordId = UIUtils.GetKeywordId( m_currentKeyword );
				m_createToggle = false;
				m_keywordModeType = KeywordModeType.Toggle;
				m_variableMode = VariableMode.Fetch;
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_multiCompile );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_defaultValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_materialValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_createToggle );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_currentKeyword );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_keywordModeType );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_keywordEnumAmount );
			for( int i = 0; i < m_keywordEnumAmount; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_keywordEnumList[ i ] );
			}
		}
	}
}
