// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public enum PropertyType
	{
		Constant = 0,
		Property,
		InstancedProperty,
		Global
	}

	public enum VariableMode
	{
		Create,
		Fetch
	}

	[Serializable]
	public class PropertyAttributes
	{
		public string Name;
		public string Attribute;
		public PropertyAttributes( string name, string attribute )
		{
			Name = name;
			Attribute = attribute;
		}
	}

	[Serializable]
	public class PropertyNode : ParentNode
	{
		private const string LongNameEnder = "... )";
		protected int m_longNameSize = 200;

		private string TooltipFormatter = "{0}\n\nName: {1}\nValue: {2}";
		protected string GlobalTypeWarningText = "Global variables must be set via a C# script using the Shader.SetGlobal{0}(...) method.\nPlease note that setting a global variable will affect all shaders which are using it.";
		private const string AutoRegisterStr = "Auto-Register";
		private const string IgnoreVarDeclarationStr = "Variable Mode";
		private const string IsPropertyStr = "Is Property";
		private const string PropertyNameStr = "Property Name";
		private const string PropertyInspectorStr = "Name";
		protected const string EnumsStr = "Enums";
		protected const string CustomAttrStr = "Custom Attributes";
		protected const string ParameterTypeStr = "Type";
		private const string PropertyTextfieldControlName = "PropertyName";
		private const string PropertyInspTextfieldControlName = "PropertyInspectorName";
		private const string OrderIndexStr = "Order Index";
		private const double MaxTimestamp = 2;
		private const double MaxPropertyTimestamp = 2;
		private const double MaxGlobalFetchTimestamp = 2;
		protected readonly string[] LabelToolbarTitle = { "Material", "Default" };
		protected readonly string[] EnumModesStr = { "Create Enums", "Use Engine Enum Class" };
		protected readonly int[] EnumModeIntValues = { 0, 1 };

		[SerializeField]
		protected PropertyType m_currentParameterType;

		[SerializeField]
		private PropertyType m_lastParameterType;

		[SerializeField]
		protected string m_propertyName;

		[SerializeField]
		protected string m_propertyInspectorName;

		[SerializeField]
		protected string m_precisionString;
		protected bool m_drawPrecisionUI = true;

		[SerializeField]
		private int m_orderIndex = -1;

		[SerializeField]
		protected VariableMode m_variableMode = VariableMode.Create;

		[SerializeField]
		private bool m_autoGlobalName = true;

		[SerializeField]
		protected bool m_autoRegister = false;

		[SerializeField]
		private List<string> m_enumNames = new List<string>();

		[SerializeField]
		private List<int> m_enumValues = new List<int>();

		[SerializeField]
		private int m_enumCount = 0;

		[SerializeField]
		private int m_enumModeInt = 0;

		[SerializeField]
		private int m_customAttrCount = 0;

		[SerializeField]
		private List<string> m_customAttr = new List<string>();

		[SerializeField]
		private string m_enumClassName = string.Empty;

		private bool m_hasEnum = false;

		protected bool m_showTitleWhenNotEditing = true;

		private int m_orderIndexOffset = 0;

		protected bool m_drawAttributes = true;

		protected bool m_underscoredGlobal = false;
		protected bool m_globalDefaultBehavior = true;

		protected bool m_freeName;
		protected bool m_freeType;
		protected bool m_showVariableMode = false;
		protected bool m_propertyNameIsDirty;

		protected bool m_showAutoRegisterUI = true;

		protected bool m_useVarSubtitle = false;

		protected bool m_propertyFromInspector;
		protected double m_propertyFromInspectorTimestamp;
		protected double m_globalFetchTimestamp;

		protected bool m_delayedDirtyProperty;
		protected double m_delayedDirtyPropertyTimestamp;

		protected string m_defaultPropertyName;
		protected string m_oldName = string.Empty;

		private bool m_reRegisterName = false;
		protected bool m_allowPropertyDuplicates = false;
		//protected bool m_useCustomPrefix = false;
		protected string m_customPrefix = null;

		protected int m_propertyTab = 0;

		[SerializeField]
		private string m_uniqueName;

		// Property Attributes
		private const float ButtonLayoutWidth = 15;

		protected bool m_visibleAttribsFoldout;
		protected bool m_visibleEnumsFoldout;
		protected bool m_visibleCustomAttrFoldout;
		protected List<PropertyAttributes> m_availableAttribs = new List<PropertyAttributes>();
		private string[] m_availableAttribsArr;

		[SerializeField]
		private bool[] m_selectedAttribsArr;

		[SerializeField]
		protected List<int> m_selectedAttribs = new List<int>();

		//Title editing 
		protected bool m_isEditing;
		protected bool m_stopEditing;
		protected bool m_startEditing;
		protected double m_clickTime;
		protected double m_doubleClickTime = 0.3;
		private Rect m_titleClickArea;

		public PropertyNode() : base() { }
		public PropertyNode( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { }


		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_textLabelWidth = 105;
			if( UIUtils.CurrentWindow != null && UIUtils.CurrentWindow.CurrentGraph != null )
				m_orderIndex = UIUtils.GetPropertyNodeAmount();
			m_currentParameterType = PropertyType.Constant;
			m_freeType = true;
			m_freeName = true;
			m_propertyNameIsDirty = true;
			m_availableAttribs.Add( new PropertyAttributes( "Hide in Inspector", "[HideInInspector]" ) );
			m_availableAttribs.Add( new PropertyAttributes( "HDR", "[HDR]" ) );
			m_availableAttribs.Add( new PropertyAttributes( "Gamma", "[Gamma]" ) );
			m_availableAttribs.Add( new PropertyAttributes( "Per Renderer Data", "[PerRendererData]" ) );
		}

		public override void AfterCommonInit()
		{
			base.AfterCommonInit();

			if( PaddingTitleLeft == 0 && m_freeType )
			{
				PaddingTitleLeft = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
				if( PaddingTitleRight == 0 )
					PaddingTitleRight = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
			}

			m_hasLeftDropdown = m_freeType;
		}

		protected void BeginDelayedDirtyProperty()
		{
			m_delayedDirtyProperty = true;
			m_delayedDirtyPropertyTimestamp = EditorApplication.timeSinceStartup;
		}

		public void CheckDelayedDirtyProperty()
		{
			if( m_delayedDirtyProperty )
			{
				if( ( EditorApplication.timeSinceStartup - m_delayedDirtyPropertyTimestamp ) > MaxPropertyTimestamp )
				{
					m_delayedDirtyProperty = false;
					m_propertyNameIsDirty = true;
					m_sizeIsDirty = true;
				}
			}
		}

		public void BeginPropertyFromInspectorCheck()
		{
			m_propertyFromInspector = true;
			m_propertyFromInspectorTimestamp = EditorApplication.timeSinceStartup;
		}

		public void CheckPropertyFromInspector( bool forceUpdate = false )
		{
			if( m_propertyFromInspector )
			{
				if( forceUpdate || ( EditorApplication.timeSinceStartup - m_propertyFromInspectorTimestamp ) > MaxTimestamp )
				{
					m_propertyFromInspector = false;
					RegisterPropertyName( true, m_propertyInspectorName, m_autoGlobalName, m_underscoredGlobal );
					m_propertyNameIsDirty = true;
				}
			}
		}

		protected override void OnUniqueIDAssigned()
		{
			RegisterFirstAvailablePropertyName( false );

			if( m_nodeAttribs != null )
				m_uniqueName = m_nodeAttribs.Name + UniqueId;

			UIUtils.RegisterRawPropertyNode( this );
		}

		public bool CheckLocalVariable( ref MasterNodeDataCollector dataCollector )
		{
			bool addToLocalValue = false;
			int count = 0;
			for( int i = 0; i < m_outputPorts.Count; i++ )
			{
				if( m_outputPorts[ i ].IsConnected )
				{
					if( m_outputPorts[ i ].ConnectionCount > 1 )
					{
						addToLocalValue = true;
						break;
					}
					count += 1;
					if( count > 1 )
					{
						addToLocalValue = true;
						break;
					}
				}
			}

			if( addToLocalValue )
			{
				ConfigureLocalVariable( ref dataCollector );
			}

			return addToLocalValue;
		}

		public virtual void ConfigureLocalVariable( ref MasterNodeDataCollector dataCollector ) { }
		public virtual void CopyDefaultsToMaterial() { }

		public override void SetupFromCastObject( UnityEngine.Object obj )
		{
			RegisterPropertyName( true, obj.name, true, m_underscoredGlobal );
		}

		public void ChangeParameterType( PropertyType parameterType )
		{
			Undo.RegisterCompleteObjectUndo( m_containerGraph.ParentWindow, Constants.UndoChangePropertyTypeNodesId );
			Undo.RegisterCompleteObjectUndo( m_containerGraph, Constants.UndoChangePropertyTypeNodesId );
			Undo.RecordObject( this, Constants.UndoChangePropertyTypeNodesId );

			if( m_currentParameterType == PropertyType.Constant || m_currentParameterType == PropertyType.Global )
			{
				CopyDefaultsToMaterial();
			}

			if( parameterType == PropertyType.InstancedProperty )
			{
				UIUtils.CurrentWindow.OutsideGraph.AddInstancePropertyCount();
			}
			else if( m_currentParameterType == PropertyType.InstancedProperty )
			{
				UIUtils.CurrentWindow.OutsideGraph.RemoveInstancePropertyCount();
			}

			if( ( parameterType == PropertyType.Property || parameterType == PropertyType.InstancedProperty )
				&& m_currentParameterType != PropertyType.Property && m_currentParameterType != PropertyType.InstancedProperty )
			{
				UIUtils.RegisterPropertyNode( this );
			}

			if( ( parameterType != PropertyType.Property && parameterType != PropertyType.InstancedProperty )
				&& ( m_currentParameterType == PropertyType.Property || m_currentParameterType == PropertyType.InstancedProperty ) )
			{
				UIUtils.UnregisterPropertyNode( this );
			}

			m_currentParameterType = parameterType;
		}

		void InitializeAttribsArray()
		{
			m_availableAttribsArr = new string[ m_availableAttribs.Count ];
			m_selectedAttribsArr = new bool[ m_availableAttribs.Count ];
			for( int i = 0; i < m_availableAttribsArr.Length; i++ )
			{
				m_availableAttribsArr[ i ] = m_availableAttribs[ i ].Name;
				m_selectedAttribsArr[ i ] = false;

				if( m_selectedAttribs.FindIndex( x => x == i ) > -1 )
				{
					m_selectedAttribsArr[ i ] = true;
					m_visibleAttribsFoldout = true;
				}
			}
		}

		protected virtual void OnAtrributesChanged() { CheckEnumAttribute(); }
		void DrawAttributesAddRemoveButtons()
		{
			if( m_availableAttribsArr == null )
			{
				InitializeAttribsArray();
			}

			int attribCount = m_selectedAttribs.Count;
			// Add new port
			if( GUILayout.Button( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
			{
				m_visibleAttribsFoldout = true;
				m_selectedAttribs.Add( 0 );
				OnAtrributesChanged();
			}

			//Remove port
			if( GUILayout.Button( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
			{
				if( attribCount > 0 )
				{
					m_selectedAttribs.RemoveAt( attribCount - 1 );
					OnAtrributesChanged();
				}
			}
		}

		void CheckEnumAttribute()
		{
			m_hasEnum = false;
			foreach( var item in m_selectedAttribs )
			{
				if( m_availableAttribsArr[ item ].Equals( "Enum" ) )
					m_hasEnum = true;
			}
		}
		void DrawEnumAddRemoveButtons()
		{
			// Add new port
			if( GUILayout.Button( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ButtonLayoutWidth ) ) && m_enumModeInt == 0 )
			{
				m_enumNames.Add( "Option" + ( m_enumValues.Count + 1 ) );
				m_enumValues.Add( m_enumValues.Count );
				m_enumCount++;
				m_visibleEnumsFoldout = true;
			}

			//Remove port
			if( GUILayout.Button( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ButtonLayoutWidth ) ) && m_enumModeInt == 0 )
			{
				if( m_enumNames.Count - 1 > -1 )
				{
					m_enumNames.RemoveAt( m_enumNames.Count - 1 );
					m_enumValues.RemoveAt( m_enumValues.Count - 1 );
					m_enumCount--;
				}
			}
		}

		protected void DrawEnums()
		{
			m_enumModeInt = EditorGUILayout.IntPopup( "Mode", m_enumModeInt, EnumModesStr, EnumModeIntValues );

			if( m_enumModeInt == 0 )
			{
				if( m_enumNames.Count == 0 )
					EditorGUILayout.HelpBox( "Your list is Empty!\nUse the plus button to add more.", MessageType.Info );

				float cacheLabelSize = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = 50;

				for( int i = 0; i < m_enumNames.Count; i++ )
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.BeginHorizontal();
					m_enumNames[ i ] = EditorGUILayoutTextField( "Name", m_enumNames[ i ] );
					m_enumValues[ i ] = EditorGUILayoutIntField( "Value", m_enumValues[ i ], GUILayout.Width( 100 ) );
					EditorGUILayout.EndHorizontal();
					if( EditorGUI.EndChangeCheck() )
					{
						m_enumNames[ i ] = UIUtils.RemoveInvalidEnumCharacters( m_enumNames[ i ] );
						if( string.IsNullOrEmpty( m_enumNames[ i ] ) )
						{
							m_enumNames[ i ] = "Option" + ( i + 1 );
						}
					}
				}

				EditorGUIUtility.labelWidth = cacheLabelSize;
				if( m_enumNames.Count > 0 )
				{
					EditorGUILayout.BeginHorizontal();
					GUILayout.Label( " " );
					DrawEnumAddRemoveButtons();
					EditorGUILayout.EndHorizontal();
				}
			}
			else
			{
				EditorGUILayout.BeginHorizontal();
				m_enumClassName = EditorGUILayoutTextField( "Class Name", m_enumClassName );

				if( GUILayout.Button( string.Empty, UIUtils.InspectorPopdropdownFallback, GUILayout.Width( 17 ), GUILayout.Height( 19 ) ) )
				{
					GenericMenu menu = new GenericMenu();
					AddMenuItem( menu, "UnityEngine.Rendering.CullMode" );
					AddMenuItem( menu, "UnityEngine.Rendering.ColorWriteMask" );
					AddMenuItem( menu, "UnityEngine.Rendering.CompareFunction" );
					AddMenuItem( menu, "UnityEngine.Rendering.StencilOp" );
					AddMenuItem( menu, "UnityEngine.Rendering.BlendMode" );
					AddMenuItem( menu, "UnityEngine.Rendering.BlendOp" );
					menu.ShowAsContext();
				}
				EditorGUILayout.EndHorizontal();
			}
		}

		private void AddMenuItem( GenericMenu menu, string newClass )
		{
			menu.AddItem( new GUIContent( newClass ), m_enumClassName.Equals( newClass ), OnSelection, newClass );
		}

		private void OnSelection( object newClass )
		{
			m_enumClassName = (string)newClass;
		}

		protected void DrawCustomAttrAddRemoveButtons()
		{
			// Add new port
			if( GUILayout.Button( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
			{
				m_customAttr.Add( "" );
				m_customAttrCount++;
				//m_enumCount++;
				m_visibleCustomAttrFoldout = true;
			}

			//Remove port
			if( GUILayout.Button( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
			{
				if( m_customAttr.Count - 1 > -1 )
				{
					m_customAttr.RemoveAt( m_customAttr.Count - 1 );
					m_customAttrCount--;
				}
			}
		}

		protected void DrawCustomAttributes()
		{
			for( int i = 0; i < m_customAttrCount; i++ )
			{
				EditorGUI.BeginChangeCheck();
				m_customAttr[ i ] = EditorGUILayoutTextField( "Attribute " + i, m_customAttr[ i ] );
				if( EditorGUI.EndChangeCheck() )
				{
					m_customAttr[ i ] = UIUtils.RemoveInvalidAttrCharacters( m_customAttr[ i ] );
				}
			}

			if( m_customAttrCount <= 0 )
			{
				EditorGUILayout.HelpBox( "Your list is Empty!\nUse the plus button to add more.", MessageType.Info );
				return;
			}

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( " " );
			DrawCustomAttrAddRemoveButtons();
			EditorGUILayout.EndHorizontal();
		}

		public virtual void DrawAttributes()
		{
			int attribCount = m_selectedAttribs.Count;
			EditorGUI.BeginChangeCheck();
			if( m_availableAttribsArr == null )
			{
				InitializeAttribsArray();
			}
			for( int i = 0; i < m_availableAttribsArr.Length; i++ )
			{
				m_selectedAttribsArr[ i ] = EditorGUILayoutToggleLeft( m_availableAttribsArr[ i ], m_selectedAttribsArr[ i ] );
			}
			if( EditorGUI.EndChangeCheck() )
			{
				m_selectedAttribs.Clear();
				for( int i = 0; i < m_selectedAttribsArr.Length; i++ )
				{
					if( m_selectedAttribsArr[ i ] )
						m_selectedAttribs.Add( i );
				}

				OnAtrributesChanged();
			}

			bool customAttr = EditorGUILayoutToggleLeft( "Custom", m_customAttrCount == 0 ? false : true );
			if( !customAttr )
			{
				m_customAttrCount = 0;
			}
			else if( customAttr && m_customAttrCount < 1 )
			{
				if( m_customAttr.Count == 0 )
					m_customAttr.Add( "" );

				m_customAttrCount = m_customAttr.Count;
			}
			//m_customAttrCount = EditorGUILayoutToggleLeft( "Custom Attribute", m_customAttrCount == 0 ? false : true ) == 0 ? false : true;

			//if( attribCount == 0 )
			//{
			//	EditorGUILayout.HelpBox( "Your list is Empty!\nUse the plus button to add more.", MessageType.Info );
			//}

			//bool actionAllowed = true;
			//int deleteItem = -1;

			//for ( int i = 0; i < attribCount; i++ )
			//{
			//	EditorGUI.BeginChangeCheck();
			//	{
			//		m_selectedAttribs[ i ] = EditorGUILayoutPopup( m_selectedAttribs[ i ], m_availableAttribsArr );
			//	}
			//	if ( EditorGUI.EndChangeCheck() )
			//	{
			//		OnAtrributesChanged();
			//	}

			//	EditorGUILayout.BeginHorizontal();
			//	GUILayout.Label( " " );
			//	// Add After
			//	if ( GUILayout.Button( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
			//	{
			//		if ( actionAllowed )
			//		{
			//			m_selectedAttribs.Insert( i, m_selectedAttribs[ i ] );
			//			actionAllowed = false;
			//			OnAtrributesChanged();
			//		}
			//	}

			//	// Remove Current
			//	if ( GUILayout.Button( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
			//	{
			//		if ( actionAllowed )
			//		{
			//			actionAllowed = false;
			//			deleteItem = i;
			//		}
			//	}
			//	EditorGUILayout.EndHorizontal();
			//}
			//if ( deleteItem > -1 )
			//{
			//	m_selectedAttribs.RemoveAt( deleteItem );
			//	OnAtrributesChanged();
			//}
		}
		public virtual void DrawMainPropertyBlock()
		{
			EditorGUILayout.BeginVertical();
			{
				if( m_freeType )
				{
					PropertyType parameterType = (PropertyType)EditorGUILayoutEnumPopup( ParameterTypeStr, m_currentParameterType );
					if( parameterType != m_currentParameterType )
					{
						ChangeParameterType( parameterType );
						BeginPropertyFromInspectorCheck();
					}
				}

				if( m_freeName )
				{
					switch( m_currentParameterType )
					{
						case PropertyType.Property:
						case PropertyType.InstancedProperty:
						{
							ShowPropertyInspectorNameGUI();
							ShowPropertyNameGUI( true );
							ShowVariableMode();
							ShowAutoRegister();
							ShowPrecision();
							ShowToolbar();
						}
						break;
						case PropertyType.Global:
						{
							ShowPropertyInspectorNameGUI();
							ShowPropertyNameGUI( false );
							ShowVariableMode();
							ShowAutoRegister();
							ShowPrecision();
							ShowDefaults();
						}
						break;
						case PropertyType.Constant:
						{
							ShowPropertyInspectorNameGUI();
							ShowPrecision();
							ShowDefaults();
						}
						break;
					}
				}
			}
			EditorGUILayout.EndVertical();
		}

		public void DrawMainPropertyBlockNoPrecision()
		{
			EditorGUILayout.BeginVertical();
			{
				if( m_freeType )
				{
					PropertyType parameterType = (PropertyType)EditorGUILayoutEnumPopup( ParameterTypeStr, m_currentParameterType );
					if( parameterType != m_currentParameterType )
					{
						ChangeParameterType( parameterType );
						BeginPropertyFromInspectorCheck();
					}
				}

				if( m_freeName )
				{
					switch( m_currentParameterType )
					{
						case PropertyType.Property:
						case PropertyType.InstancedProperty:
						{
							ShowPropertyInspectorNameGUI();
							ShowPropertyNameGUI( true );
							ShowToolbar();
						}
						break;
						case PropertyType.Global:
						{
							ShowPropertyInspectorNameGUI();
							ShowPropertyNameGUI( false );
							ShowDefaults();
						}
						break;
						case PropertyType.Constant:
						{
							ShowPropertyInspectorNameGUI();
							ShowDefaults();
						}
						break;
					}
				}
			}
			EditorGUILayout.EndVertical();
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			if( m_freeType || m_freeName )
			{
				NodeUtils.DrawPropertyGroup( ref m_propertiesFoldout, Constants.ParameterLabelStr, DrawMainPropertyBlock );
				if( m_drawAttributes )
					NodeUtils.DrawPropertyGroup( ref m_visibleAttribsFoldout, Constants.AttributesLaberStr, DrawAttributes );

				if( m_hasEnum )
				{
					if( m_enumModeInt == 0 )
						NodeUtils.DrawPropertyGroup( ref m_visibleEnumsFoldout, EnumsStr, DrawEnums, DrawEnumAddRemoveButtons );
					else
						NodeUtils.DrawPropertyGroup( ref m_visibleEnumsFoldout, EnumsStr, DrawEnums );
				}

				if( m_drawAttributes && m_customAttrCount > 0 )
					NodeUtils.DrawPropertyGroup( ref m_visibleCustomAttrFoldout, CustomAttrStr, DrawCustomAttributes, DrawCustomAttrAddRemoveButtons );

				CheckPropertyFromInspector();
			}
		}

		public void ShowPrecision()
		{
			if( m_drawPrecisionUI )
			{
				bool guiEnabled = GUI.enabled;
				GUI.enabled = m_currentParameterType == PropertyType.Constant || m_variableMode == VariableMode.Create;
				EditorGUI.BeginChangeCheck();
				DrawPrecisionProperty();
				if( EditorGUI.EndChangeCheck() )
					m_precisionString = UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType );

				GUI.enabled = guiEnabled;

			}
		}

		public void ShowToolbar()
		{
			//if ( !CanDrawMaterial )
			//{
			//	ShowDefaults();
			//	return;
			//}

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
					DrawMaterialProperties();
					if( EditorGUI.EndChangeCheck() )
					{
						BeginDelayedDirtyProperty();
					}
				}
				break;
				case 1:
				{
					ShowDefaults();
				}
				break;
			}
		}

		public void ShowDefaults()
		{
			EditorGUI.BeginChangeCheck();
			DrawSubProperties();
			if( EditorGUI.EndChangeCheck() )
			{
				BeginDelayedDirtyProperty();
			}
			if( m_currentParameterType == PropertyType.Global && m_globalDefaultBehavior )
			{
				if( DebugConsoleWindow.DeveloperMode )
				{
					ShowGlobalValueButton();
				}
				EditorGUILayout.HelpBox( GlobalTypeWarningText, MessageType.Warning );
			}
		}

		public void ShowPropertyInspectorNameGUI()
		{
			EditorGUI.BeginChangeCheck();
			m_propertyInspectorName = EditorGUILayoutTextField( PropertyInspectorStr, m_propertyInspectorName );
			if( EditorGUI.EndChangeCheck() )
			{
				if( m_propertyInspectorName.Length > 0 )
				{
					BeginPropertyFromInspectorCheck();
				}
			}
		}

		public void ShowPropertyNameGUI( bool isProperty )
		{
			bool guiEnabledBuffer = GUI.enabled;
			if( isProperty )
			{
				EditorGUILayout.BeginHorizontal();
				GUI.enabled = !m_autoGlobalName;
				EditorGUI.BeginChangeCheck();
				m_propertyName = EditorGUILayoutTextField( PropertyNameStr, m_propertyName );
				if( EditorGUI.EndChangeCheck() )
				{
					BeginPropertyFromInspectorCheck();
				}
				GUI.enabled = guiEnabledBuffer;
				EditorGUI.BeginChangeCheck();
				m_autoGlobalName = GUILayout.Toggle( m_autoGlobalName, ( m_autoGlobalName ? UIUtils.LockIconOpen : UIUtils.LockIconClosed ), "minibutton", GUILayout.Width( 22 ) );
				if( EditorGUI.EndChangeCheck() )
				{
					if( m_autoGlobalName )
						BeginPropertyFromInspectorCheck();
				}
				EditorGUILayout.EndHorizontal();
			}
			else
			{
				GUI.enabled = false;
				m_propertyName = EditorGUILayoutTextField( PropertyNameStr, m_propertyName );
				GUI.enabled = guiEnabledBuffer;
			}
		}

		public void ShowVariableMode()
		{
			if( m_showVariableMode || m_freeType )
				m_variableMode = (VariableMode)EditorGUILayoutEnumPopup( IgnoreVarDeclarationStr, m_variableMode );
		}

		public void ShowAutoRegister()
		{
			if( m_showAutoRegisterUI && CurrentParameterType != PropertyType.Constant )
			{
				m_autoRegister = EditorGUILayoutToggle( AutoRegisterStr, m_autoRegister );
			}
		}

		public virtual string GetPropertyValStr() { return string.Empty; }

		public override bool OnClick( Vector2 currentMousePos2D )
		{
			bool singleClick = base.OnClick( currentMousePos2D );
			m_propertyTab = m_materialMode ? 0 : 1;
			return singleClick;
		}

		public override void OnNodeDoubleClicked( Vector2 currentMousePos2D )
		{
			if( currentMousePos2D.y - m_globalPosition.y > ( Constants.NODE_HEADER_HEIGHT + Constants.NODE_HEADER_EXTRA_HEIGHT ) * ContainerGraph.ParentWindow.CameraDrawInfo.InvertedZoom )
			{
				ContainerGraph.ParentWindow.ParametersWindow.IsMaximized = !ContainerGraph.ParentWindow.ParametersWindow.IsMaximized;
			}
		}

		public override void DrawTitle( Rect titlePos )
		{
			//base.DrawTitle( titlePos );
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );

			// Custom Editable Title
			if( ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD3 )
			{
				if( !m_isEditing && ( ( !ContainerGraph.ParentWindow.MouseInteracted && drawInfo.CurrentEventType == EventType.MouseDown && m_titleClickArea.Contains( drawInfo.MousePosition ) ) ) )
				{
					if( ( EditorApplication.timeSinceStartup - m_clickTime ) < m_doubleClickTime )
						m_startEditing = true;
					else
						GUI.FocusControl( null );
					m_clickTime = EditorApplication.timeSinceStartup;
				}
				else if( m_isEditing && ( ( drawInfo.CurrentEventType == EventType.MouseDown && !m_titleClickArea.Contains( drawInfo.MousePosition ) ) || !EditorGUIUtility.editingTextField ) )
				{
					m_stopEditing = true;
				}

				if( m_isEditing || m_startEditing )
				{
					EditorGUI.BeginChangeCheck();
					GUI.SetNextControlName( m_uniqueName );
					m_propertyInspectorName = EditorGUITextField( m_titleClickArea, string.Empty, m_propertyInspectorName, UIUtils.GetCustomStyle( CustomStyle.NodeTitle ) );
					if( EditorGUI.EndChangeCheck() )
					{
						SetClippedTitle( m_propertyInspectorName, m_longNameSize );
						m_sizeIsDirty = true;
						m_isDirty = true;
						if( m_propertyInspectorName.Length > 0 )
						{
							BeginPropertyFromInspectorCheck();
						}
					}

					if( m_startEditing )
						EditorGUI.FocusTextInControl( m_uniqueName );
					//if( m_stopEditing )
					//	GUI.FocusControl( null );
				}

				if( drawInfo.CurrentEventType == EventType.Repaint )
				{
					if( m_startEditing )
					{
						m_startEditing = false;
						m_isEditing = true;
					}

					if( m_stopEditing )
					{
						m_stopEditing = false;
						m_isEditing = false;
						GUI.FocusControl( null );
					}
				}


				if( m_freeType )
				{
					if( m_dropdownEditing )
					{
						PropertyType parameterType = (PropertyType)EditorGUIEnumPopup( m_dropdownRect, m_currentParameterType, UIUtils.PropertyPopUp );
						if( parameterType != m_currentParameterType )
						{
							ChangeParameterType( parameterType );
							BeginPropertyFromInspectorCheck();
							m_dropdownEditing = false;
						}
					}
				}
			}

		}

		public override void OnNodeLayout( DrawInfo drawInfo )
		{
			//base.OnNodeLayout( drawInfo );
			if( m_reRegisterName )
			{
				m_reRegisterName = false;
				UIUtils.RegisterUniformName( UniqueId, m_propertyName );
			}

			CheckDelayedDirtyProperty();

			if( m_currentParameterType != m_lastParameterType || m_propertyNameIsDirty )
			{
				m_lastParameterType = m_currentParameterType;
				m_propertyNameIsDirty = false;
				OnDirtyProperty();
				if( m_currentParameterType != PropertyType.Constant )
				{
					SetClippedTitle( m_propertyInspectorName, m_longNameSize );
					//bool globalHandler = false;
					//if( globalHandler )
					//{
					string currValue = ( m_currentParameterType == PropertyType.Global && m_globalDefaultBehavior ) ? "<GLOBAL>" : GetPropertyValStr();
					SetClippedAdditionalTitle( string.Format( m_useVarSubtitle ? Constants.SubTitleVarNameFormatStr : Constants.SubTitleValueFormatStr, currValue ), m_longNameSize, LongNameEnder );
					//}
					//else
					//{
					//	if( m_currentParameterType == PropertyType.Global )
					//	{
					//		SetAdditonalTitleText( "Global" );
					//	}
					//	else
					//	{
					//		SetAdditonalTitleText( string.Format( m_useVarSubtitle ? Constants.SubTitleVarNameFormatStr : Constants.SubTitleValueFormatStr, GetPropertyValStr() ) );
					//	}
					//}
				}
				else
				{
					SetClippedTitle( m_propertyInspectorName, m_longNameSize );
					SetClippedAdditionalTitle( string.Format( Constants.SubTitleConstFormatStr, GetPropertyValStr() ), m_longNameSize, LongNameEnder );
				}
			}

			CheckPropertyFromInspector();

			// RUN LAYOUT CHANGES AFTER TITLES CHANGE
			base.OnNodeLayout( drawInfo );

			m_titleClickArea = m_titlePos;
			m_titleClickArea.height = Constants.NODE_HEADER_HEIGHT;
		}

		public override void OnNodeRepaint( DrawInfo drawInfo )
		{
			base.OnNodeRepaint( drawInfo );

			if( !m_isVisible )
				return;

			// Fixed Title ( only renders when not editing )
			if( m_showTitleWhenNotEditing && !m_isEditing && !m_startEditing && ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD3 )
			{
				GUI.Label( m_titleClickArea, m_content, UIUtils.GetCustomStyle( CustomStyle.NodeTitle ) );
			}
		}

		public void RegisterFirstAvailablePropertyName( bool releaseOldOne )
		{
			if( releaseOldOne )
				UIUtils.ReleaseUniformName( UniqueId, m_oldName );

			if( m_isNodeBeingCopied )
			{
				if( string.IsNullOrEmpty( m_propertyName ) )
					return;

				if( UIUtils.IsUniformNameAvailable( m_propertyName ) )
					UIUtils.RegisterUniformName( UniqueId, m_propertyName );
				else
					UIUtils.GetFirstAvailableName( UniqueId, m_outputPorts[ 0 ].DataType, out m_propertyName, out m_propertyInspectorName, !string.IsNullOrEmpty( m_customPrefix ), m_customPrefix );

			}
			else
			{
				UIUtils.GetFirstAvailableName( UniqueId, m_outputPorts[ 0 ].DataType, out m_propertyName, out m_propertyInspectorName, !string.IsNullOrEmpty( m_customPrefix ), m_customPrefix );
			}
			m_oldName = m_propertyName;
			m_propertyNameIsDirty = true;
			m_reRegisterName = false;
			OnPropertyNameChanged();
		}

		public void RegisterPropertyName( bool releaseOldOne, string newName, bool autoGlobal = true, bool forceUnderscore = false )
		{
			string propertyName = string.Empty;
			if( autoGlobal )
				propertyName = UIUtils.GeneratePropertyName( newName, m_currentParameterType, forceUnderscore );
			else
			{
				propertyName = UIUtils.GeneratePropertyName( m_propertyName, PropertyType.Global, forceUnderscore );
				if( UIUtils.IsNumericName( propertyName ) )
				{
					m_propertyName = m_oldName;
				}

			}
			if( m_propertyName.Equals( propertyName ) )
				return;

			if( UIUtils.IsUniformNameAvailable( propertyName ) || m_allowPropertyDuplicates )
			{
				if( releaseOldOne )
					UIUtils.ReleaseUniformName( UniqueId, m_oldName );

				m_oldName = propertyName;
				m_propertyName = propertyName;
				if( autoGlobal )
					m_propertyInspectorName = newName;
				m_propertyNameIsDirty = true;
				m_reRegisterName = false;
				UIUtils.RegisterUniformName( UniqueId, propertyName );
				OnPropertyNameChanged();
			}
			else
			{

				GUI.FocusControl( string.Empty );
				RegisterFirstAvailablePropertyName( releaseOldOne );
				UIUtils.ShowMessage( string.Format( "Duplicate name found on edited node.\nAssigning first valid one {0}", m_propertyInspectorName ) );
			}
		}

		protected string CreateLocalVarDec( string value )
		{
			return string.Format( Constants.PropertyLocalVarDec, UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType ), m_propertyName, value );
		}

		public virtual void CheckIfAutoRegister( ref MasterNodeDataCollector dataCollector )
		{
			if( CurrentParameterType != PropertyType.Constant && m_autoRegister && m_connStatus != NodeConnectionStatus.Connected )
			{
				RegisterProperty( ref dataCollector );
			}
		}

		virtual protected void RegisterProperty( ref MasterNodeDataCollector dataCollector )
		{
			CheckPropertyFromInspector( true );
			if( m_propertyName.Length == 0 )
			{
				RegisterFirstAvailablePropertyName( false );
			}

			switch( CurrentParameterType )
			{
				case PropertyType.Property:
				{
					//Debug.Log( this.GetInstanceID()+" "+ OrderIndex+" "+GetPropertyValue() );
					dataCollector.AddToProperties( UniqueId, GetPropertyValue(), OrderIndex );
					string dataType = string.Empty;
					string dataName = string.Empty;
					if( m_variableMode == VariableMode.Create && GetUniformData( out dataType, out dataName ) )
						dataCollector.AddToUniforms( UniqueId, dataType, dataName );
					//dataCollector.AddToUniforms( m_uniqueId, GetUniformValue() );
				}
				break;
				case PropertyType.InstancedProperty:
				{
					dataCollector.AddToPragmas( UniqueId, IOUtils.InstancedPropertiesHeader );
					dataCollector.AddToProperties( UniqueId, GetPropertyValue(), OrderIndex );
					dataCollector.AddToInstancedProperties( m_outputPorts[ 0 ].DataType, UniqueId, GetInstancedPropertyValue( dataCollector.IsTemplate ), OrderIndex );
				}
				break;
				case PropertyType.Global:
				{
					string dataType = string.Empty;
					string dataName = string.Empty;
					if( m_variableMode == VariableMode.Create && GetUniformData( out dataType, out dataName ) )
						dataCollector.AddToUniforms( UniqueId, dataType, dataName );
					//dataCollector.AddToUniforms( m_uniqueId, GetUniformValue() );
				}
				break;
				case PropertyType.Constant: break;
			}
			dataCollector.AddPropertyNode( this );
			if( m_currentParameterType == PropertyType.InstancedProperty && !m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
			{
				string instancedVar = dataCollector.IsSRP ?
					string.Format( IOUtils.LWSRPInstancedPropertiesData, dataCollector.InstanceBlockName, m_propertyName ) :
					string.Format( IOUtils.InstancedPropertiesData, m_propertyName );
				RegisterLocalVariable( 0, instancedVar, ref dataCollector, m_propertyName + "_Instance" );
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			RegisterProperty( ref dataCollector );
			return string.Empty;
		}

		public override void Destroy()
		{
			base.Destroy();
			UIUtils.UnregisterRawPropertyNode( this );
			if( !string.IsNullOrEmpty( m_propertyName ) )
				UIUtils.ReleaseUniformName( UniqueId, m_propertyName );
			if( m_currentParameterType == PropertyType.InstancedProperty )
			{
				UIUtils.CurrentWindow.OutsideGraph.RemoveInstancePropertyCount();
				UIUtils.UnregisterPropertyNode( this );
			}

			if( m_currentParameterType == PropertyType.Property )
			{
				UIUtils.UnregisterPropertyNode( this );
			}

			if( m_availableAttribs != null )
				m_availableAttribs.Clear();

			m_availableAttribs = null;
		}

		string BuildEnum()
		{
			string result = "[Enum(";
			if( m_enumModeInt == 0 )
			{
				for( int i = 0; i < m_enumNames.Count; i++ )
				{
					result += m_enumNames[ i ] + "," + m_enumValues[ i ];
					if( i + 1 < m_enumNames.Count )
						result += ",";
				}
			}
			else
			{
				result += m_enumClassName;
			}
			result += ")]";
			return result;
		}

		public string PropertyAttributes
		{
			get
			{
				int attribCount = m_selectedAttribs.Count;

				if( m_selectedAttribs.Count == 0 && m_customAttrCount == 0 )
					return string.Empty;

				string attribs = string.Empty;
				for( int i = 0; i < attribCount; i++ )
				{
					if( m_availableAttribs[ m_selectedAttribs[ i ] ].Name.Equals( "Enum" ) )
						attribs += BuildEnum();
					else
						attribs += m_availableAttribs[ m_selectedAttribs[ i ] ].Attribute;
				}

				for( int i = 0; i < m_customAttrCount; i++ )
				{
					if( !string.IsNullOrEmpty( m_customAttr[ i ] ) )
						attribs += "[" + m_customAttr[ i ] + "]";
				}
				return attribs;
			}
		}
		public virtual void OnDirtyProperty() { }
		public virtual void OnPropertyNameChanged() { UIUtils.UpdatePropertyDataNode( UniqueId, PropertyInspectorName ); }
		public virtual void DrawSubProperties() { }
		public virtual void DrawMaterialProperties() { }

		public virtual string GetPropertyValue() { return string.Empty; }

		public string GetInstancedPropertyValue( bool isTemplate )
		{
			if( isTemplate )
				return string.Format( IOUtils.InstancedPropertiesElement, UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType ), m_propertyName );
			else
				return string.Format( IOUtils.InstancedPropertiesElementTabs, UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType ), m_propertyName );
		}

		public virtual string GetUniformValue()
		{
			return string.Format( Constants.UniformDec, UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType ), m_propertyName );
		}

		public virtual bool GetUniformData( out string dataType, out string dataName )
		{
			dataType = UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType );
			dataName = m_propertyName;
			return true;
		}

		public PropertyType CurrentParameterType
		{
			get { return m_currentParameterType; }
			set { m_currentParameterType = value; }
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_currentParameterType );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_propertyName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_propertyInspectorName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_orderIndex );
			int attribCount = m_selectedAttribs.Count;
			IOUtils.AddFieldValueToString( ref nodeInfo, attribCount );
			if( attribCount > 0 )
			{
				for( int i = 0; i < attribCount; i++ )
				{
					IOUtils.AddFieldValueToString( ref nodeInfo, m_availableAttribs[ m_selectedAttribs[ i ] ].Attribute );
				}
			}
			IOUtils.AddFieldValueToString( ref nodeInfo, m_variableMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_autoGlobalName );


			IOUtils.AddFieldValueToString( ref nodeInfo, m_enumCount );
			for( int i = 0; i < m_enumCount; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_enumNames[ i ] );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_enumValues[ i ] );
			}
			IOUtils.AddFieldValueToString( ref nodeInfo, m_enumModeInt );
			if( m_enumModeInt == 1 )
				IOUtils.AddFieldValueToString( ref nodeInfo, m_enumClassName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_autoRegister );

			IOUtils.AddFieldValueToString( ref nodeInfo, m_customAttrCount );
			if( m_customAttrCount > 0 )
			{
				for( int i = 0; i < m_customAttrCount; i++ )
				{
					IOUtils.AddFieldValueToString( ref nodeInfo, m_customAttr[ i ] );
				}
			}
		}

		int IdForAttrib( string name )
		{
			int attribCount = m_availableAttribs.Count;
			for( int i = 0; i < attribCount; i++ )
			{
				if( m_availableAttribs[ i ].Attribute.Equals( name ) )
					return i;
			}
			return 0;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if( UIUtils.CurrentShaderVersion() < 2505 )
			{
				string property = GetCurrentParam( ref nodeParams );
				m_currentParameterType = property.Equals( "Uniform" ) ? PropertyType.Global : (PropertyType)Enum.Parse( typeof( PropertyType ), property );
			}
			else
			{
				m_currentParameterType = (PropertyType)Enum.Parse( typeof( PropertyType ), GetCurrentParam( ref nodeParams ) );
			}

			if( m_currentParameterType == PropertyType.InstancedProperty )
			{
				UIUtils.CurrentWindow.OutsideGraph.AddInstancePropertyCount();
				UIUtils.RegisterPropertyNode( this );
			}

			if( m_currentParameterType == PropertyType.Property )
			{
				UIUtils.RegisterPropertyNode( this );
			}

			m_propertyName = GetCurrentParam( ref nodeParams );
			m_propertyInspectorName = GetCurrentParam( ref nodeParams );

			if( UIUtils.CurrentShaderVersion() > 13 )
			{
				m_orderIndex = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			}

			if( UIUtils.CurrentShaderVersion() > 4102 )
			{
				int attribAmount = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				if( attribAmount > 0 )
				{
					for( int i = 0; i < attribAmount; i++ )
					{
						m_selectedAttribs.Add( IdForAttrib( GetCurrentParam( ref nodeParams ) ) );
					}

					m_visibleAttribsFoldout = true;
				}
				InitializeAttribsArray();
			}


			if( UIUtils.CurrentShaderVersion() > 14003 )
			{
				m_variableMode = (VariableMode)Enum.Parse( typeof( VariableMode ), GetCurrentParam( ref nodeParams ) );
			}

			if( UIUtils.CurrentShaderVersion() > 14201 )
			{
				m_autoGlobalName = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			}
			if( UIUtils.CurrentShaderVersion() > 14403 )
			{
				m_enumCount = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				for( int i = 0; i < m_enumCount; i++ )
				{
					m_enumNames.Add( GetCurrentParam( ref nodeParams ) );
					m_enumValues.Add( Convert.ToInt32( GetCurrentParam( ref nodeParams ) ) );
				}
			}

			if( UIUtils.CurrentShaderVersion() > 14501 )
			{
				m_enumModeInt = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				if( m_enumModeInt == 1 )
					m_enumClassName = GetCurrentParam( ref nodeParams );
				m_autoRegister = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );

				m_customAttrCount = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				for( int i = 0; i < m_customAttrCount; i++ )
				{
					m_customAttr.Add( GetCurrentParam( ref nodeParams ) );
				}
				if( m_customAttrCount > 0 )
				{
					m_visibleCustomAttrFoldout = true;
					m_visibleAttribsFoldout = true;
				}
			}

			CheckEnumAttribute();
			if( m_enumCount > 0 )
				m_visibleEnumsFoldout = true;

			m_propertyNameIsDirty = true;
			m_reRegisterName = false;

			if( !m_isNodeBeingCopied )
			{
				UIUtils.ReleaseUniformName( UniqueId, m_oldName );
				UIUtils.RegisterUniformName( UniqueId, m_propertyName );
			}
			m_oldName = m_propertyName;
		}

		void UpdateTooltip()
		{
			string currValue = string.Empty;
			if( m_currentParameterType != PropertyType.Constant )
			{
				currValue = ( m_currentParameterType == PropertyType.Global && m_globalDefaultBehavior ) ? "<GLOBAL>" : GetPropertyValStr();
			}
			else
			{
				currValue = GetPropertyValStr();
			}

			m_tooltipText = string.Format( TooltipFormatter, m_nodeAttribs.Description, m_propertyInspectorName, currValue );
		}

		public override void SetClippedTitle( string newText, int maxSize = 170, string endString = "..." )
		{
			base.SetClippedTitle( newText, maxSize, endString );
			UpdateTooltip();
		}

		public override void SetClippedAdditionalTitle( string newText, int maxSize = 170, string endString = "..." )
		{
			base.SetClippedAdditionalTitle( newText, maxSize, endString );
			UpdateTooltip();
		}

		public override void OnEnable()
		{
			base.OnEnable();
			m_reRegisterName = true;
		}

		public bool CanDrawMaterial { get { return m_materialMode && m_currentParameterType != PropertyType.Constant; } }
		public int RawOrderIndex
		{
			get { return m_orderIndex; }
		}

		public int OrderIndex
		{
			get { return m_orderIndex + m_orderIndexOffset; }
			set { m_orderIndex = value; }
		}

		public int OrderIndexOffset
		{
			get { return m_orderIndexOffset; }
			set { m_orderIndexOffset = value; }
		}
		public string PropertyData( MasterNodePortCategory portCategory )
		{
			return ( m_currentParameterType == PropertyType.InstancedProperty ) ? m_outputPorts[ 0 ].LocalValue( portCategory ) : m_propertyName;
		}

		public override void OnNodeLogicUpdate( DrawInfo drawInfo )
		{
			base.OnNodeLogicUpdate( drawInfo );
			if( m_currentParameterType == PropertyType.Global && m_globalDefaultBehavior && ( EditorApplication.timeSinceStartup - m_globalFetchTimestamp ) > MaxGlobalFetchTimestamp )
			{
				FetchGlobalValue();
				m_globalFetchTimestamp = EditorApplication.timeSinceStartup;
			}
		}

		public void ShowGlobalValueButton()
		{
			if( GUILayout.Button( "Set Global Value" ) )
			{
				SetGlobalValue();
			}
		}

		//This should only be used on template internal properties
		public void PropertyNameFromTemplate( TemplateShaderPropertyData data )
		{
			m_propertyName = data.PropertyName;
			m_propertyInspectorName = data.PropertyInspectorName;
		}

		public virtual void SetGlobalValue() { }
		public virtual void FetchGlobalValue() { }

		public virtual string PropertyName { get { return m_propertyName; } }
		public virtual string PropertyInspectorName { get { return m_propertyInspectorName; } }
		public bool FreeType { get { return m_freeType; } set { m_freeType = value; } }
		public bool ReRegisterName { get { return m_reRegisterName; } set { m_reRegisterName = value; } }
		public string CustomPrefix { get { return m_customPrefix; } set { m_customPrefix = value; } }
		public override void RefreshOnUndo()
		{
			base.RefreshOnUndo();
			BeginPropertyFromInspectorCheck();
		}
		public override string DataToArray { get { return PropertyInspectorName; } }
	}
}
