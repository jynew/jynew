// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace AmplifyShaderEditor
{

	public enum CustomExpressionMode
	{
		Create,
		Call
	}

	[Serializable]
	public class CustomExpressionInputItem
	{
		public PrecisionType Precision;
		public VariableQualifiers Qualifier;
		public WirePortDataType Type;
		public string CustomType;
		public bool FoldoutFlag;
		public string FoldoutLabel;

		public CustomExpressionInputItem( PrecisionType precision, VariableQualifiers qualifier, string customType, bool foldoutFlag, string foldoutLabel )
		{
			Precision = precision;
			Qualifier = qualifier;
			CustomType = customType;
			FoldoutFlag = foldoutFlag;
			FoldoutLabel = foldoutLabel;
		}
	}

	[Serializable]
	public class CustomExpressionDependency
	{
		public int DependencyArrayIdx;
		public int DependencyNodeId;
		public CustomExpressionDependency() { DependencyArrayIdx = DependencyNodeId = -1; }
		public CustomExpressionDependency( string id ) { DependencyNodeId = Convert.ToInt32( id ); DependencyArrayIdx = -1; }
		public void Reset()
		{
			DependencyArrayIdx = -1;
			DependencyNodeId = -1;
		}
	}

	[Serializable]
	[NodeAttributes( "Custom Expression", "Miscellaneous", "Creates a custom expression or function if <b>return</b> is detected in the written code." )]
	public sealed class CustomExpressionNode : ParentNode
	{
		private const float AddRemoveButtonLayoutWidth = 15;
		private const float LineAdjust = 1.15f;
		private const float IdentationAdjust = 5f;
		private const string CustomExpressionInfo = "Creates a custom expression or function according to how code is written on text area.\n\n" +
													" - If a return function is detected on Code text area then a function will be created.\n" +
													"Also in function mode a ; is expected on the end of each instruction line.\n\n" +
													"- If no return function is detected then an expression will be generated and used directly on the vertex/frag body.\n" +
													"On Expression mode a ; is not required on the end of an instruction line.";
		private const char LineFeedSeparator = '$';
		private const char SemiColonSeparator = '@';
		private const string ReturnHelper = "return";
		private const double MaxTimestamp = 1;
		private const string DefaultExpressionNameStr = "My Custom Expression";
		private const string DefaultInputNameStr = "In";
		private const string CodeTitleStr = "Code";
		private const string OutputTypeStr = "Output Type";
		private const string CustomTypeStr = " ";
		private const string InputsStr = "Inputs";
		private const string InputNameStr = "Name";
		private const string InputTypeStr = "Type";
		private const string InputValueStr = "Value";
		private const string InputQualifierStr = "Qualifier";
		private const string ExpressionNameLabelStr = "Name";
		private const string FunctionCallModeStr = "Mode";
		private const string GenerateUniqueNameStr = "Set Unique";
		private const string AutoRegisterStr = "Auto-Register";
		private const string DependenciesStr = "Dependencies";

		private readonly string[] AvailableWireTypesStr =
		{
		"int",
		"float",
		"float2",
		"float3",
		"float4",
		"float3x3",
		"float4x4",
		"sampler1D",
		"sampler2D",
		"sampler3D",
		"samplerCUBE",
		"custom"};

		private readonly string[] AvailableOutputWireTypesStr =
		{
		"int",
		"float",
		"float2",
		"float3",
		"float4",
		"float3x3",
		"float4x4",
		"void",
		};

		private readonly string[] QualifiersStr =
		{
			"In",
			"Out",
			"InOut"
		};

		private readonly WirePortDataType[] AvailableWireTypes =
		{
			WirePortDataType.INT,
			WirePortDataType.FLOAT,
			WirePortDataType.FLOAT2,
			WirePortDataType.FLOAT3,
			WirePortDataType.FLOAT4,
			WirePortDataType.FLOAT3x3,
			WirePortDataType.FLOAT4x4,
			WirePortDataType.SAMPLER1D,
			WirePortDataType.SAMPLER2D,
			WirePortDataType.SAMPLER3D,
			WirePortDataType.SAMPLERCUBE,
			WirePortDataType.OBJECT
		};

		private readonly WirePortDataType[] AvailableOutputWireTypes =
		{
			WirePortDataType.INT,
			WirePortDataType.FLOAT,
			WirePortDataType.FLOAT2,
			WirePortDataType.FLOAT3,
			WirePortDataType.FLOAT4,
			WirePortDataType.FLOAT3x3,
			WirePortDataType.FLOAT4x4,
			WirePortDataType.OBJECT,
		};


		private readonly Dictionary<WirePortDataType, int> WireToIdx = new Dictionary<WirePortDataType, int>
		{
			{ WirePortDataType.INT,         0},
			{ WirePortDataType.FLOAT,       1},
			{ WirePortDataType.FLOAT2,      2},
			{ WirePortDataType.FLOAT3,      3},
			{ WirePortDataType.FLOAT4,      4},
			{ WirePortDataType.FLOAT3x3,    5},
			{ WirePortDataType.FLOAT4x4,    6},
			{ WirePortDataType.SAMPLER1D,   7},
			{ WirePortDataType.SAMPLER2D,   8},
			{ WirePortDataType.SAMPLER3D,   9},
			{ WirePortDataType.SAMPLERCUBE, 10},
			{ WirePortDataType.OBJECT,      11}
		};

		[SerializeField]
		private string m_customExpressionName = DefaultExpressionNameStr;

		[SerializeField]
		private List<CustomExpressionInputItem> m_items = new List<CustomExpressionInputItem>();

		[SerializeField]
		private string m_code = " ";

		[SerializeField]
		private int m_outputTypeIdx = 1;

		[SerializeField]
		private bool m_visibleInputsFoldout = true;

		[SerializeField]
		private CustomExpressionMode m_mode = CustomExpressionMode.Create;

		[SerializeField]
		private bool m_voidMode = false;

		[SerializeField]
		private bool m_autoRegisterMode = false;

		[SerializeField]
		private bool m_functionMode = false;

		[SerializeField]
		private int m_firstAvailablePort = 0;

		[SerializeField]
		private string m_uniqueName;

		[SerializeField]
		private bool m_generateUniqueName = true;

		[SerializeField]
		private bool m_dependenciesFoldout = false;

		[SerializeField]
		private List<CustomExpressionDependency> m_dependencies = new List<CustomExpressionDependency>();

		private int m_markedToDelete = -1;
		private const float ButtonLayoutWidth = 15;

		private bool m_repopulateNameDictionary = true;
		private Dictionary<string, int> m_usedNames = new Dictionary<string, int>();

		private double m_lastTimeNameModified = 0;
		private bool m_nameModified = false;

		private double m_lastTimeCodeModified = 0;
		private bool m_codeModified = false;

		//Title editing 
		private bool m_isEditing;
		private bool m_stopEditing;
		private bool m_startEditing;
		private double m_clickTime;
		private double m_doubleClickTime = 0.3;
		private Rect m_titleClickArea;

		//Item Reordable List 
		private ReordableAction m_actionType = ReordableAction.None;
		private int m_actionIndex = 0;
		private int m_lastIndex = 0;

		private ReorderableList m_itemReordableList = null;
		private ReorderableList m_dependenciesReordableList = null;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, "In0" );
			m_items.Add( new CustomExpressionInputItem( PrecisionType.Float, VariableQualifiers.In, string.Empty, true, string.Empty/*"[0]"*/ ) );
			AddOutputPort( WirePortDataType.FLOAT, "Out" );
			m_textLabelWidth = 97;
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();

			if( m_mode == CustomExpressionMode.Create )
				m_containerGraph.CustomExpressionOnFunctionMode.AddNode( this );

			SetTitleText( m_customExpressionName );

			if( m_nodeAttribs != null )
				m_uniqueName = m_nodeAttribs.Name + OutputId;
			else
				m_uniqueName = "CustomExpression" + OutputId;
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			CheckPortConnection( portId );
		}

		public override void OnConnectedOutputNodeChanges( int portId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( portId, otherNodeId, otherPortId, name, type );
			CheckPortConnection( portId );
		}

		void CheckPortConnection( int portId )
		{
			if( portId == 0 && ( m_mode == CustomExpressionMode.Call || m_voidMode ) )
			{
				m_inputPorts[ 0 ].MatchPortToConnection();
				m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
			}
		}

		public override void OnNodeLogicUpdate( DrawInfo drawInfo )
		{
			base.OnNodeLogicUpdate( drawInfo );
			if( m_nameModified )
			{
				if( ( EditorApplication.timeSinceStartup - m_lastTimeNameModified ) > MaxTimestamp )
				{
					m_nameModified = false;
					m_repopulateNameDictionary = true;
				}
			}

			if( m_repopulateNameDictionary )
			{
				m_repopulateNameDictionary = false;
				m_usedNames.Clear();
				for( int i = 0; i < m_inputPorts.Count; i++ )
				{
					m_usedNames.Add( m_inputPorts[ i ].Name, i );
				}
			}

			if( m_codeModified )
			{
				if( ( EditorApplication.timeSinceStartup - m_lastTimeCodeModified ) > MaxTimestamp )
				{
					m_codeModified = false;
					bool functionMode = m_code.Contains( ReturnHelper );
					if( functionMode != m_functionMode )
					{
						m_functionMode = functionMode;
						CheckCallMode();
					}
				}
			}
		}

		bool CheckCallMode()
		{
			if( m_functionMode && m_mode == CustomExpressionMode.Call )
			{
				Mode = CustomExpressionMode.Create;
				m_outputTypeIdx = ( AvailableOutputWireTypesStr.Length - 1 );
				m_outputPorts[ 0 ].ChangeType( AvailableOutputWireTypes[ m_outputTypeIdx ], false );
				m_voidMode = true;
				return true;
			}
			return false;
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
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
					m_customExpressionName = EditorGUITextField( m_titleClickArea, string.Empty, m_customExpressionName, UIUtils.GetCustomStyle( CustomStyle.NodeTitle ) );
					if( EditorGUI.EndChangeCheck() )
					{
						SetTimedUpdate( 2 );
						SetTitleText( m_customExpressionName );
						m_sizeIsDirty = true;
						m_isDirty = true;
					}

					if( m_startEditing )
						EditorGUI.FocusTextInControl( m_uniqueName );
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
			}
		}

		public override void OnNodeLayout( DrawInfo drawInfo )
		{
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
			if( !m_isEditing && !m_startEditing && ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD3 )
			{
				GUI.Label( m_titleClickArea, m_content, UIUtils.GetCustomStyle( CustomStyle.NodeTitle ) );
			}
		}

		public string GetFirstAvailableName()
		{
			string name = string.Empty;
			for( int i = 0; i < m_inputPorts.Count + 1; i++ )
			{
				name = DefaultInputNameStr + i;
				if( !m_usedNames.ContainsKey( name ) )
				{
					return name;
				}
			}
			Debug.LogWarning( "Could not find valid name" );
			return string.Empty;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			NodeUtils.DrawPropertyGroup( ref m_propertiesFoldout, Constants.ParameterLabelStr, DrawBaseProperties );
			//NodeUtils.DrawPropertyGroup( ref m_visibleInputsFoldout, InputsStr, DrawInputs, DrawAddRemoveInputs );
			NodeUtils.DrawPropertyGroup( ref m_visibleInputsFoldout, InputsStr, DrawReordableInputs, DrawItemsAddRemoveInputs );

			EditorGUILayout.HelpBox( CustomExpressionInfo, MessageType.Info );
		}

		string WrapCodeInFunction( bool isTemplate, string functionName, bool expressionMode )
		{
			//Hack to be used util indent is properly used
			int currIndent = UIUtils.ShaderIndentLevel;
			UIUtils.ShaderIndentLevel = isTemplate ? 0 : 1;

			if( !isTemplate ) UIUtils.ShaderIndentLevel++;

			//string functionName = UIUtils.RemoveInvalidCharacters( m_customExpressionName );
			string returnType = ( m_mode == CustomExpressionMode.Call || m_voidMode ) ? "void" : UIUtils.PrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType );
			if( expressionMode )
				returnType = "inline " + returnType;

			string functionBody = UIUtils.ShaderIndentTabs + returnType + " " + functionName + "( ";
			int count = m_inputPorts.Count - m_firstAvailablePort;
			for( int i = 0; i < count; i++ )
			{
				int portIdx = i + m_firstAvailablePort;
				string qualifier = m_items[ i ].Qualifier == VariableQualifiers.In ? string.Empty : UIUtils.QualifierToCg( m_items[ i ].Qualifier ) + " ";
				PrecisionType precision = ( (int)m_items[ i ].Precision > (int)m_currentPrecisionType ) ? m_items[ i ].Precision : m_currentPrecisionType;
				string dataType = ( m_inputPorts[ portIdx ].DataType == WirePortDataType.OBJECT ) ? m_items[ i ].CustomType : UIUtils.PrecisionWirePortToCgType( precision, m_inputPorts[ portIdx ].DataType );
				functionBody += qualifier + dataType + " " + m_inputPorts[ portIdx ].Name;
				if( i < ( count - 1 ) )
				{
					functionBody += " , ";
				}
			}
			functionBody += " )\n" + UIUtils.ShaderIndentTabs + "{\n";
			UIUtils.ShaderIndentLevel++;
			{
				if( expressionMode )
					functionBody += UIUtils.ShaderIndentTabs + "return ";

				string[] codeLines = m_code.Split( IOUtils.LINE_TERMINATOR );
				for( int i = 0; i < codeLines.Length; i++ )
				{
					if( codeLines[ i ].Length > 0 )
					{
						functionBody += ( ( i == 0 && expressionMode ) ? string.Empty : UIUtils.ShaderIndentTabs ) + codeLines[ i ] + ( ( ( i == codeLines.Length - 1 ) && expressionMode ) ? string.Empty : "\n" );
					}
				}
				if( expressionMode )
					functionBody += ";\n";
			}
			UIUtils.ShaderIndentLevel--;

			functionBody += UIUtils.ShaderIndentTabs + "}\n";
			UIUtils.ShaderIndentLevel = currIndent;
			return functionBody;
		}

		void DrawBaseProperties()
		{
			EditorGUI.BeginChangeCheck();
			m_customExpressionName = EditorGUILayoutTextField( ExpressionNameLabelStr, m_customExpressionName );
			if( EditorGUI.EndChangeCheck() )
			{
				SetTimedUpdate( 2 );
				SetTitleText( m_customExpressionName );
			}

			EditorGUI.BeginChangeCheck();
			Mode = (CustomExpressionMode)EditorGUILayoutEnumPopup( FunctionCallModeStr, m_mode );
			if( EditorGUI.EndChangeCheck() )
			{
				if( CheckCallMode() )
					UIUtils.ShowMessage( "Call Mode cannot have return over is code.\nFalling back to Create Mode" );
				SetupCallMode();
				RecalculateInOutOutputPorts();
			}

			EditorGUILayout.LabelField( CodeTitleStr );
			EditorGUI.BeginChangeCheck();
			{
				m_code = EditorGUILayoutTextArea( m_code, UIUtils.MainSkin.textArea );
			}
			if( EditorGUI.EndChangeCheck() )
			{
				m_codeModified = true;
				m_lastTimeCodeModified = EditorApplication.timeSinceStartup;
			}

			if( m_mode == CustomExpressionMode.Create )
			{
				DrawPrecisionProperty();

				bool guiEnabled = GUI.enabled;

				GUI.enabled = !AutoRegisterMode;
				m_generateUniqueName = EditorGUILayoutToggle( GenerateUniqueNameStr, m_generateUniqueName ) && !AutoRegisterMode;

				GUI.enabled = !m_generateUniqueName;
				AutoRegisterMode = EditorGUILayoutToggle( AutoRegisterStr, AutoRegisterMode ) && !m_generateUniqueName;

				GUI.enabled = guiEnabled;

				EditorGUI.BeginChangeCheck();
				m_outputTypeIdx = EditorGUILayoutPopup( OutputTypeStr, m_outputTypeIdx, AvailableOutputWireTypesStr );
				if( EditorGUI.EndChangeCheck() )
				{
					bool oldVoidValue = m_voidMode;
					UpdateVoidMode();
					if( oldVoidValue != m_voidMode )
					{
						SetupCallMode();
						RecalculateInOutOutputPorts();
					}
					else
					{
						m_outputPorts[ 0 ].ChangeType( AvailableOutputWireTypes[ m_outputTypeIdx ], false );
					}
				}
			}
			NodeUtils.DrawNestedPropertyGroup( ref m_dependenciesFoldout, "Dependencies", DrawDependencies, DrawDependenciesAddRemoveInputs );
		}

		void UpdateVoidMode()
		{
			m_voidMode = ( m_outputTypeIdx == ( AvailableOutputWireTypesStr.Length - 1 ) );
		}

		void SetupCallMode()
		{
			if( m_mode == CustomExpressionMode.Call || m_voidMode )
			{
				if( m_firstAvailablePort != 1 )
				{
					m_firstAvailablePort = 1;
					AddInputPortAt( 0, WirePortDataType.FLOAT, false, DefaultInputNameStr );
					m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT, false );
				}
			}
			else
			{
				if( m_firstAvailablePort != 0 )
				{
					m_firstAvailablePort = 0;
					if( m_inputPorts[ 0 ].IsConnected )
					{
						m_containerGraph.DeleteConnection( true, UniqueId, m_inputPorts[ 0 ].PortId, false, true );
					}
					DeleteInputPortByArrayIdx( 0 );
					m_outputPorts[ 0 ].ChangeType( AvailableOutputWireTypes[ m_outputTypeIdx ], false );
				}
			}
		}

		void DrawItemsAddRemoveInputs()
		{
			if( m_inputPorts.Count == m_firstAvailablePort )
				m_visibleInputsFoldout = false;

			// Add new port
			if( GUILayoutButton( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
			{
				AddPortAt( m_inputPorts.Count );
				m_visibleInputsFoldout = true;
				EditorGUI.FocusTextInControl( null );
			}

			//Remove port
			if( GUILayoutButton( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
			{
				RemovePortAt( m_inputPorts.Count - 1 );
				EditorGUI.FocusTextInControl( null );
			}
		}

		void DrawDependenciesAddRemoveInputs()
		{
			// Add new port
			if( GUILayoutButton( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
			{
				m_dependencies.Add( new CustomExpressionDependency() );
				EditorGUI.FocusTextInControl( null );
			}

			//Remove port
			if( GUILayoutButton( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
			{
				m_dependencies.RemoveAt( m_dependencies.Count - 1 );
			}
		}

		void DrawDependencies()
		{
			if( m_dependenciesReordableList == null )
			{
				m_dependenciesReordableList = new ReorderableList( m_dependencies, typeof( CustomExpressionDependency ), true, false, false, false )
				{
					headerHeight = 0,
					footerHeight = 0,
					showDefaultBackground = false,
					drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) =>
					{
						if( m_dependencies[ index ] != null )
						{
							rect.xMin -= 1;

							Rect popupPos = new Rect( rect.x, rect.y, rect.width - 2 * Constants.PlusMinusButtonLayoutWidth, EditorGUIUtility.singleLineHeight );
							Rect buttonPlusPos = new Rect( rect.x + rect.width - 2 * Constants.PlusMinusButtonLayoutWidth, rect.y - 2, Constants.PlusMinusButtonLayoutWidth, Constants.PlusMinusButtonLayoutWidth );
							Rect buttonMinusPos = new Rect( rect.x + rect.width - Constants.PlusMinusButtonLayoutWidth, rect.y - 2, Constants.PlusMinusButtonLayoutWidth, Constants.PlusMinusButtonLayoutWidth );
							EditorGUI.BeginChangeCheck();
							m_dependencies[ index ].DependencyArrayIdx = EditorGUIPopup( popupPos, string.Empty, m_dependencies[ index ].DependencyArrayIdx, m_containerGraph.CustomExpressionOnFunctionMode.NodesArr );
							if( EditorGUI.EndChangeCheck() )
							{
								m_dependencies[ index ].DependencyNodeId = m_containerGraph.CustomExpressionOnFunctionMode.GetNode( m_dependencies[ index ].DependencyArrayIdx ).UniqueId;
								if( m_dependencies[ index ].DependencyNodeId == UniqueId )
								{
									m_dependencies[ index ].Reset();
								}
							}

							if( GUI.Button( buttonPlusPos, string.Empty, UIUtils.PlusStyle ) )
							{
								m_actionType = ReordableAction.Add;
								m_actionIndex = index;
							}

							if( GUI.Button( buttonMinusPos, string.Empty, UIUtils.MinusStyle ) )
							{
								m_actionType = ReordableAction.Remove;
								m_actionIndex = index;
							}
						}
					}
				};
			}

			if( m_dependenciesReordableList != null )
			{
				EditorGUILayout.Space();
				if( m_dependencies.Count == 0 )
				{
					EditorGUILayout.HelpBox( "Your list is Empty!\nUse the plus button to add one.", MessageType.Info );
				}
				else
				{
					m_dependenciesReordableList.DoLayoutList();
				}
				EditorGUILayout.Space();
			}

			if( m_actionType != ReordableAction.None )
			{
				switch( m_actionType )
				{
					case ReordableAction.Add:
					m_dependencies.Insert( m_actionIndex + 1, new CustomExpressionDependency() );
					break;
					case ReordableAction.Remove:
					m_dependencies.RemoveAt( m_actionIndex );
					break;
				}
				m_isDirty = true;
				m_actionType = ReordableAction.None;
				EditorGUI.FocusTextInControl( null );
			}
		}

		void DrawReordableInputs()
		{
			if( m_itemReordableList == null )
			{
				m_itemReordableList = new ReorderableList( m_items, typeof( CustomExpressionInputItem ), true, false, false, false )
				{
					headerHeight = 0,
					footerHeight = 0,
					showDefaultBackground = false,
					elementHeightCallback = ( int index ) =>
					{
						float lineHeight = EditorGUIUtility.singleLineHeight * LineAdjust;
						if( m_items[ index ].FoldoutFlag )
						{
							float size = 7 * lineHeight;

							if( m_inputPorts[ m_firstAvailablePort + index ].DataType == WirePortDataType.OBJECT )
								size += lineHeight;

							if( !m_inputPorts[ m_firstAvailablePort + index ].IsConnected )
							{
								switch( m_inputPorts[ m_firstAvailablePort + index ].DataType )
								{
									case WirePortDataType.INT:
									case WirePortDataType.FLOAT:
									size += 0;// lineHeight;
									break;
									case WirePortDataType.FLOAT2:
									case WirePortDataType.FLOAT3:
									case WirePortDataType.FLOAT4:
									size += lineHeight;//2 * lineHeight;
									break;
									case WirePortDataType.FLOAT3x3:
									size += 5 * lineHeight;//6 * lineHeight;
									break;
									case WirePortDataType.FLOAT4x4:
									size += 6 * lineHeight;//8 * lineHeight;
									break;

								}
							}

							return size;
						}
						else
						{
							return lineHeight;
						}
					},

					onReorderCallback = ( ReorderableList list ) =>
					{
						int realLastIndex = m_firstAvailablePort + m_lastIndex;
						int realCurrIndex = m_firstAvailablePort + list.index;
						SwapInputPorts( realLastIndex, realCurrIndex );
					},
					onSelectCallback = ( ReorderableList list ) =>
					{
						m_lastIndex = list.index;
					},
					drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) =>
					{
						if( m_items[ index ] != null )
						{
							float lineHeight = EditorGUIUtility.singleLineHeight;
							float lineSpacing = lineHeight * LineAdjust;

							rect.x -= IdentationAdjust;
							rect.height = lineHeight;
							int portIdx = index + m_firstAvailablePort;
							Rect foldoutRect = rect;
							if( !m_items[ index ].FoldoutFlag )
							{
								foldoutRect.width -= 2 * AddRemoveButtonLayoutWidth;
							}
							m_items[ index ].FoldoutFlag = EditorGUIFoldout( foldoutRect, m_items[ index ].FoldoutFlag, /*m_items[ index ].FoldoutLabel + " - " +*/ m_inputPorts[ portIdx ].Name );
							if( m_items[ index ].FoldoutFlag )
							{
								rect.x += IdentationAdjust;

								//Qualifier
								rect.y += lineSpacing;
								VariableQualifiers newQualifier = (VariableQualifiers)EditorGUIPopup( rect, InputQualifierStr, (int)m_items[ index ].Qualifier, QualifiersStr );
								if( newQualifier != m_items[ index ].Qualifier )
								{
									VariableQualifiers oldQualifier = m_items[ index ].Qualifier;
									m_items[ index ].Qualifier = newQualifier;
									if( newQualifier == VariableQualifiers.In )
									{
										RemoveOutputPort( CreateOutputId( m_inputPorts[ portIdx ].PortId ), false );
									}
									else if( oldQualifier == VariableQualifiers.In )
									{
										AddOutputPort( m_inputPorts[ portIdx ].DataType, m_inputPorts[ portIdx ].Name, CreateOutputId( m_inputPorts[ portIdx ].PortId ) );
									}
									m_inputPorts[ portIdx ].Visible = newQualifier != VariableQualifiers.Out;
									m_sizeIsDirty = true;
									RecalculateInOutOutputPorts();
								}
								
								// Precision
								rect.y += lineSpacing;
								m_items[ index ].Precision = (PrecisionType)EditorGUIPopup(rect, PrecisionContent.text, (int)m_items[ index ].Precision, PrecisionLabels );
								// Type
								rect.y += lineSpacing;
								int typeIdx = WireToIdx[ m_inputPorts[ portIdx ].DataType ];
								EditorGUI.BeginChangeCheck();
								{
									typeIdx = EditorGUIPopup( rect, InputTypeStr, typeIdx, AvailableWireTypesStr );
								}

								if( EditorGUI.EndChangeCheck() )
								{
									m_inputPorts[ portIdx ].ChangeType( AvailableWireTypes[ typeIdx ], false );
									if( m_items[ index ].Qualifier != VariableQualifiers.In )
									{
										OutputPort currOutPort = GetOutputPortByUniqueId( CreateOutputId( m_inputPorts[ portIdx ].PortId ) );
										currOutPort.ChangeType( AvailableWireTypes[ typeIdx ], false );
									}
								}

								if( AvailableWireTypes[ typeIdx ] == WirePortDataType.OBJECT )
								{
									rect.y += lineSpacing;
									m_items[ index ].CustomType = EditorGUITextField( rect, CustomTypeStr, m_items[ index ].CustomType );
								}

								//Name
								rect.y += lineSpacing;
								EditorGUI.BeginChangeCheck();
								{
									m_inputPorts[ portIdx ].Name = EditorGUITextField( rect, InputNameStr, m_inputPorts[ portIdx ].Name );
								}
								if( EditorGUI.EndChangeCheck() )
								{
									m_nameModified = true;
									m_lastTimeNameModified = EditorApplication.timeSinceStartup;
									m_inputPorts[ portIdx ].Name = UIUtils.RemoveInvalidCharacters( m_inputPorts[ portIdx ].Name );
									if( string.IsNullOrEmpty( m_inputPorts[ portIdx ].Name ) )
									{
										m_inputPorts[ portIdx ].Name = DefaultInputNameStr + index;
									}

									if( m_items[ index ].Qualifier != VariableQualifiers.In )
									{
										OutputPort currOutPort = GetOutputPortByUniqueId( CreateOutputId( m_inputPorts[ portIdx ].PortId ) );
										currOutPort.Name = m_inputPorts[ portIdx ].Name;
									}
								}

								// Port Data
								if( !m_inputPorts[ portIdx ].IsConnected )
								{
									rect.y += lineSpacing;
									m_inputPorts[ portIdx ].ShowInternalData( rect, this, true, InputValueStr );
								}

								//Buttons
								rect.x += rect.width - 2 * AddRemoveButtonLayoutWidth;
								rect.y += lineSpacing;
								if( !m_inputPorts[ m_firstAvailablePort + index ].IsConnected )
								{
									switch( m_inputPorts[ m_firstAvailablePort + index ].DataType )
									{
										case WirePortDataType.INT:
										case WirePortDataType.FLOAT:
										rect.y += 0;// lineSpacing;
										break;
										case WirePortDataType.FLOAT2:
										case WirePortDataType.FLOAT3:
										case WirePortDataType.FLOAT4:
										rect.y += lineSpacing;//2 * lineSpacing;
										break;
										case WirePortDataType.FLOAT3x3:
										rect.y += 5 * lineSpacing;//6 * lineSpacing;
										break;
										case WirePortDataType.FLOAT4x4:
										rect.y += 6 * lineSpacing;//8 * lineSpacing;
										break;

									}
								}
								rect.width = AddRemoveButtonLayoutWidth;
								if( GUI.Button( rect, string.Empty, UIUtils.PlusStyle ) )
								{
									m_actionType = ReordableAction.Add;
									m_actionIndex = index;
								}
								rect.x += AddRemoveButtonLayoutWidth;
								if( GUI.Button( rect, string.Empty, UIUtils.MinusStyle ) )
								{
									m_actionType = ReordableAction.Remove;
									m_actionIndex = index;
								}

							}
							else
							{
								//Buttons
								rect.x += IdentationAdjust + rect.width - 2 * AddRemoveButtonLayoutWidth;
								rect.width = AddRemoveButtonLayoutWidth;
								if( GUI.Button( rect, string.Empty, UIUtils.PlusStyle ) )
								{
									m_actionType = ReordableAction.Add;
									m_actionIndex = index;
								}
								rect.x += AddRemoveButtonLayoutWidth;
								if( GUI.Button( rect, string.Empty, UIUtils.MinusStyle ) )
								{
									m_actionType = ReordableAction.Remove;
									m_actionIndex = index;
								}
							}
						}
					}
				};
			}

			///////////////////////////////////
			if( m_itemReordableList != null )
			{
				EditorGUILayout.Space();
				if( m_items.Count == 0 )
				{
					EditorGUILayout.HelpBox( "Your list is Empty!\nUse the plus button to add one.", MessageType.Info );
				}
				else
				{
					m_itemReordableList.DoLayoutList();
				}
				EditorGUILayout.Space();
			}

			if( m_actionType != ReordableAction.None )
			{
				switch( m_actionType )
				{
					case ReordableAction.Add:
					AddPortAt( m_firstAvailablePort + m_actionIndex + 1 );
					break;
					case ReordableAction.Remove:
					RemovePortAt( m_firstAvailablePort + m_actionIndex );
					break;
				}
				m_isDirty = true;
				m_actionType = ReordableAction.None;
				EditorGUI.FocusTextInControl( null );
			}
		}


		void DrawInputs()
		{
			int count = m_inputPorts.Count - m_firstAvailablePort;
			for( int i = 0; i < count; i++ )
			{
				int portIdx = i + m_firstAvailablePort;
				m_items[ i ].FoldoutFlag = EditorGUILayoutFoldout( m_items[ i ].FoldoutFlag, /*m_items[ i ].FoldoutLabel + " - " +*/ m_inputPorts[ portIdx ].Name );

				if( m_items[ i ].FoldoutFlag )
				{
					EditorGUI.indentLevel += 1;

					//Qualifier
					VariableQualifiers newQualifier = (VariableQualifiers)EditorGUILayoutPopup( InputQualifierStr, (int)m_items[ i ].Qualifier, QualifiersStr );
					if( newQualifier != m_items[ i ].Qualifier )
					{
						VariableQualifiers oldQualifier = m_items[ i ].Qualifier;
						m_items[ i ].Qualifier = newQualifier;
						if( newQualifier == VariableQualifiers.In )
						{
							RemoveOutputPort( CreateOutputId( m_inputPorts[ portIdx ].PortId ), false );
						}
						else if( oldQualifier == VariableQualifiers.In )
						{
							AddOutputPort( m_inputPorts[ portIdx ].DataType, m_inputPorts[ portIdx ].Name, CreateOutputId( m_inputPorts[ portIdx ].PortId ) );
						}
						RecalculateInOutOutputPorts();
					}

					// Type
					int typeIdx = WireToIdx[ m_inputPorts[ portIdx ].DataType ];
					EditorGUI.BeginChangeCheck();
					{
						typeIdx = EditorGUILayoutPopup( InputTypeStr, typeIdx, AvailableWireTypesStr );
					}

					if( EditorGUI.EndChangeCheck() )
					{
						m_inputPorts[ portIdx ].ChangeType( AvailableWireTypes[ typeIdx ], false );
						if( m_items[ i ].Qualifier != VariableQualifiers.In )
						{
							OutputPort currOutPort = GetOutputPortByUniqueId( CreateOutputId( m_inputPorts[ portIdx ].PortId ) );
							currOutPort.ChangeType( AvailableWireTypes[ typeIdx ], false );
						}
					}

					if( AvailableWireTypes[ typeIdx ] == WirePortDataType.OBJECT )
					{
						EditorGUI.indentLevel += 1;
						m_items[ i ].CustomType = EditorGUILayoutTextField( CustomTypeStr, m_items[ i ].CustomType );
						EditorGUI.indentLevel -= 1;
					}

					//Name
					EditorGUI.BeginChangeCheck();
					{
						m_inputPorts[ portIdx ].Name = EditorGUILayoutTextField( InputNameStr, m_inputPorts[ portIdx ].Name );
					}
					if( EditorGUI.EndChangeCheck() )
					{
						m_nameModified = true;
						m_lastTimeNameModified = EditorApplication.timeSinceStartup;
						m_inputPorts[ portIdx ].Name = UIUtils.RemoveInvalidCharacters( m_inputPorts[ portIdx ].Name );
						if( string.IsNullOrEmpty( m_inputPorts[ portIdx ].Name ) )
						{
							m_inputPorts[ portIdx ].Name = DefaultInputNameStr + i;
						}
						if( m_items[ i ].Qualifier != VariableQualifiers.In )
						{
							OutputPort currOutPort = GetOutputPortByUniqueId( CreateOutputId( m_inputPorts[ portIdx ].PortId ) );
							currOutPort.Name = m_inputPorts[ portIdx ].Name;
						}
					}

					// Port Data
					if( !m_inputPorts[ portIdx ].IsConnected )
					{
						m_inputPorts[ portIdx ].ShowInternalData( this, true, InputValueStr );
					}

					EditorGUILayout.BeginHorizontal();
					{
						GUILayout.Label( " " );
						// Add new port
						if( GUILayoutButton( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
						{
							AddPortAt( portIdx );
							EditorGUI.FocusTextInControl( null );
						}

						//Remove port
						if( GUILayoutButton( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
						{
							m_markedToDelete = portIdx;
						}
					}
					EditorGUILayout.EndHorizontal();

					EditorGUI.indentLevel -= 1;
				}
			}

			if( m_markedToDelete > -1 )
			{
				RemovePortAt( m_markedToDelete );
				m_markedToDelete = -1;
				EditorGUI.FocusTextInControl( null );
			}
		}

		void RecalculateInOutOutputPorts()
		{
			m_outputPorts.Sort( ( x, y ) => x.PortId.CompareTo( y.PortId ) );

			m_outputPortsDict.Clear();
			int count = m_inputPorts.Count - m_firstAvailablePort;
			int outputId = 1;
			for( int i = 0; i < count; i++ )
			{
				int idx = i + m_firstAvailablePort;
				if( m_items[ i ].Qualifier != VariableQualifiers.In )
				{
					m_outputPorts[ outputId ].ChangeProperties( m_inputPorts[ idx ].Name, m_inputPorts[ idx ].DataType, false );
					m_outputPorts[ outputId ].ChangePortId( CreateOutputId( m_inputPorts[ idx ].PortId ) );
					outputId++;
				}
			}

			int outCount = m_outputPorts.Count;
			for( int i = 0; i < outCount; i++ )
			{
				m_outputPortsDict.Add( m_outputPorts[ i ].PortId, m_outputPorts[ i ] );
			}
		}

		void AddPortAt( int idx )
		{
			AddInputPortAt( idx, WirePortDataType.FLOAT, false, GetFirstAvailableName() );
			m_items.Insert( idx - m_firstAvailablePort, new CustomExpressionInputItem( PrecisionType.Float, VariableQualifiers.In, string.Empty, true, string.Empty/* "[" + idx + "]"*/ ) );
			m_repopulateNameDictionary = true;
		}

		void RemovePortAt( int idx )
		{
			if( m_inputPorts.Count > m_firstAvailablePort )
			{
				bool recalculateOutputs = false;
				int varIdx = idx - m_firstAvailablePort;
				if( m_items[ varIdx ].Qualifier != VariableQualifiers.In )
				{
					RemoveOutputPort( CreateOutputId( m_inputPorts[ idx ].PortId ), false );
					recalculateOutputs = true;
				}

				DeleteInputPortByArrayIdx( idx );
				m_items.RemoveAt( varIdx );

				m_repopulateNameDictionary = true;
				if( recalculateOutputs )
					RecalculateInOutOutputPorts();
			}
		}

		public override void OnAfterDeserialize()
		{
			base.OnAfterDeserialize();
			m_repopulateNameDictionary = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( string.IsNullOrEmpty( m_code ) )
			{
				UIUtils.ShowMessage( "Custom Expression need to have code associated", MessageSeverity.Warning );
				return "0";
			}

			m_code = m_code.Replace( "\r\n", "\n" );

			bool codeContainsReturn = m_code.Contains( ReturnHelper );
			if( !codeContainsReturn && outputId != 0 && m_mode == CustomExpressionMode.Create && !m_voidMode )
			{
				UIUtils.ShowMessage( "Attempting to get value from inexisting inout/out variable", MessageSeverity.Warning );
				return "0";
			}

			int dependenciesCount = m_dependencies.Count;
			Dictionary<int, CustomExpressionNode> examinedNodes = new Dictionary<int, CustomExpressionNode>();
			for( int i = 0; i < dependenciesCount; i++ )
			{
				CustomExpressionNode node = m_containerGraph.GetNode( m_dependencies[ i ].DependencyNodeId ) as CustomExpressionNode;
				if( node != null )
				{
					node.CheckDependencies( ref dataCollector, ref examinedNodes );
				}
			}
			examinedNodes.Clear();
			examinedNodes = null;


			OutputPort outputPort = GetOutputPortByUniqueId( outputId );
			if( outputPort.IsLocalValue( dataCollector.PortCategory ) )
				return outputPort.LocalValue( dataCollector.PortCategory );

			string expressionName = UIUtils.RemoveInvalidCharacters( m_customExpressionName );
			string localVarName = "local" + expressionName;

			if( m_generateUniqueName )
			{
				expressionName += OutputId;
			}
			localVarName += OutputId;

			int count = m_inputPorts.Count;
			if( count > 0 )
			{
				if( m_mode == CustomExpressionMode.Call || m_voidMode )
				{
					string mainData = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
					RegisterLocalVariable( 0, string.Format( Constants.CodeWrapper, mainData ), ref dataCollector, localVarName );
				}

				if( codeContainsReturn )
				{
					string function = WrapCodeInFunction( dataCollector.IsTemplate, expressionName, false );
					string functionCall = expressionName + "( ";
					for( int i = m_firstAvailablePort; i < count; i++ )
					{
						string inputPortLocalVar = m_inputPorts[ i ].Name + OutputId;
						string result = m_inputPorts[ i ].GeneratePortInstructions( ref dataCollector );
						dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, m_inputPorts[ i ].DataType, inputPortLocalVar, result );
						int idx = i - m_firstAvailablePort;
						if( m_items[ idx ].Qualifier != VariableQualifiers.In )
						{
							OutputPort currOutputPort = GetOutputPortByUniqueId( CreateOutputId( m_inputPorts[ i ].PortId ) );
							currOutputPort.SetLocalValue( inputPortLocalVar, dataCollector.PortCategory );
						}
						functionCall += inputPortLocalVar;
						if( i < ( count - 1 ) )
						{
							functionCall += " , ";
						}
					}
					functionCall += " )";

					if( m_mode == CustomExpressionMode.Call || m_voidMode )
					{
						dataCollector.AddLocalVariable( 0, functionCall + ";", true );
					}
					else
					{
						RegisterLocalVariable( 0, functionCall, ref dataCollector, localVarName );
					}

					dataCollector.AddFunction( expressionName, function );
				}
				else
				{
					string localCode = m_code;
					if( m_mode == CustomExpressionMode.Call || m_voidMode )
					{
						for( int i = m_firstAvailablePort; i < count; i++ )
						{
							string inputPortLocalVar = m_inputPorts[ i ].Name + OutputId;
							localCode = localCode.Replace( m_inputPorts[ i ].Name, inputPortLocalVar );

							if( m_inputPorts[ i ].IsConnected )
							{
								string result = m_inputPorts[ i ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ i ].DataType, true, true );
								dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, m_inputPorts[ i ].DataType, inputPortLocalVar, result );
							}
							else
							{
								dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, m_inputPorts[ i ].DataType, inputPortLocalVar, m_inputPorts[ i ].WrappedInternalData );
							}
							int idx = i - m_firstAvailablePort;
							if( m_items[ idx ].Qualifier != VariableQualifiers.In )
							{
								OutputPort currOutputPort = GetOutputPortByUniqueId( CreateOutputId( m_inputPorts[ i ].PortId ) );
								currOutputPort.SetLocalValue( inputPortLocalVar, dataCollector.PortCategory );
							}
						}
						string[] codeLines = localCode.Split( '\n' );
						for( int codeIdx = 0; codeIdx < codeLines.Length; codeIdx++ )
						{
							dataCollector.AddLocalVariable( 0, codeLines[ codeIdx ], true );
						}
					}
					else
					{
						string function = WrapCodeInFunction( dataCollector.IsTemplate, expressionName, true );

						string functionCall = expressionName + "( ";
						for( int i = m_firstAvailablePort; i < count; i++ )
						{
							string inputPortLocalVar = m_inputPorts[ i ].Name + OutputId;
							string result = m_inputPorts[ i ].GeneratePortInstructions( ref dataCollector );
							dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, m_inputPorts[ i ].DataType, inputPortLocalVar, result );
							int idx = i - m_firstAvailablePort;
							if( m_items[ idx ].Qualifier != VariableQualifiers.In )
							{
								OutputPort currOutputPort = GetOutputPortByUniqueId( CreateOutputId( m_inputPorts[ i ].PortId ) );
								currOutputPort.SetLocalValue( inputPortLocalVar, dataCollector.PortCategory );
							}
							functionCall += inputPortLocalVar;
							if( i < ( count - 1 ) )
							{
								functionCall += " , ";
							}
						}
						functionCall += " )";
						RegisterLocalVariable( 0, functionCall, ref dataCollector, localVarName );
						dataCollector.AddFunction( expressionName, function );
					}
				}

				return outputPort.LocalValue( dataCollector.PortCategory );
			}
			else
			{
				if( m_code.Contains( ReturnHelper ) )
				{
					string function = WrapCodeInFunction( dataCollector.IsTemplate, expressionName, false );
					dataCollector.AddFunction( expressionName, function );
					string functionCall = expressionName + "()";
					RegisterLocalVariable( 0, functionCall, ref dataCollector, localVarName );
				}
				else
				{
					RegisterLocalVariable( 0, string.Format( Constants.CodeWrapper, m_code ), ref dataCollector, localVarName );
				}

				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
			}
		}

		int CreateOutputId( int inputId )
		{
			return ( inputId + 1 );
		}

		int CreateInputId( int outputId )
		{
			return outputId - 1;
		}

		void UpdateOutputPorts()
		{
			int count = m_inputPorts.Count - m_firstAvailablePort;
			for( int i = 0; i < count; i++ )
			{
				if( m_items[ i ].Qualifier != VariableQualifiers.In )
				{
					int portIdx = i + m_firstAvailablePort;
					AddOutputPort( m_inputPorts[ portIdx ].DataType, m_inputPorts[ portIdx ].Name, CreateOutputId( m_inputPorts[ portIdx ].PortId ) );
				}
			}
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			// This node is, by default, created with one input port 
			base.ReadFromString( ref nodeParams );
			m_code = GetCurrentParam( ref nodeParams );
			m_code = m_code.Replace( LineFeedSeparator, '\n' );
			m_code = m_code.Replace( SemiColonSeparator, ';' );
			m_outputTypeIdx = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			if( m_outputTypeIdx >= AvailableWireTypes.Length )
			{
				UIUtils.ShowMessage( "Sampler types were removed as a valid output custom expression type" );
				m_outputTypeIdx = 1;
			}
			UpdateVoidMode();
			m_outputPorts[ 0 ].ChangeType( AvailableWireTypes[ m_outputTypeIdx ], false );

			if( UIUtils.CurrentShaderVersion() > 12001 )
			{
				bool mode = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
				m_mode = mode ? CustomExpressionMode.Call : CustomExpressionMode.Create;
				if( m_mode == CustomExpressionMode.Call || m_voidMode )
				{
					m_firstAvailablePort = 1;
					AddInputPortAt( 0, WirePortDataType.FLOAT, false, DefaultInputNameStr );
				}
			}

			if( m_mode == CustomExpressionMode.Call )
				m_containerGraph.CustomExpressionOnFunctionMode.RemoveNode( this );

			int count = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			if( count == 0 )
			{
				DeleteInputPortByArrayIdx( 0 );
				m_items.Clear();
			}
			else
			{
				for( int i = 0; i < count; i++ )
				{
					bool foldoutValue = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
					string name = GetCurrentParam( ref nodeParams );
					WirePortDataType type = (WirePortDataType)Enum.Parse( typeof( WirePortDataType ), GetCurrentParam( ref nodeParams ) );
					string internalData = GetCurrentParam( ref nodeParams );
					VariableQualifiers qualifier = VariableQualifiers.In;
					if( UIUtils.CurrentShaderVersion() > 12001 )
					{
						qualifier = (VariableQualifiers)Enum.Parse( typeof( VariableQualifiers ), GetCurrentParam( ref nodeParams ) );
					}
					string customType = string.Empty;
					if( UIUtils.CurrentShaderVersion() > 15311 )
					{
						customType = GetCurrentParam( ref nodeParams );
					}
					PrecisionType precision = PrecisionType.Float;
					if( UIUtils.CurrentShaderVersion() > 15607 )
					{
						precision = (PrecisionType)Enum.Parse( typeof( PrecisionType ), GetCurrentParam( ref nodeParams ));
					}

					int portIdx = i + m_firstAvailablePort;
					if( i == 0 )
					{
						m_inputPorts[ portIdx ].ChangeProperties( name, type, false );
						m_inputPorts[ portIdx ].Visible = qualifier != VariableQualifiers.Out;
						m_items[ 0 ].Qualifier = qualifier;
						m_items[ 0 ].FoldoutFlag = foldoutValue;
						m_items[ 0 ].CustomType = customType;
						m_items[ 0 ].Precision = precision;
					}
					else
					{
						m_items.Add( new CustomExpressionInputItem( precision, qualifier, customType, foldoutValue, string.Empty/*"[" + i + "]"*/ ) );
						AddInputPort( type, false, name );
						m_inputPorts[ m_inputPorts.Count -1  ].Visible = qualifier != VariableQualifiers.Out;
					}
					m_inputPorts[ i ].InternalData = internalData;
				}
			}

			if( UIUtils.CurrentShaderVersion() > 7205 )
			{
				m_customExpressionName = GetCurrentParam( ref nodeParams );
				SetTitleText( m_customExpressionName );
			}

			if( UIUtils.CurrentShaderVersion() > 14401 )
			{
				m_generateUniqueName = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			}

			if( UIUtils.CurrentShaderVersion() > 15102 )
			{
				m_autoRegisterMode = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			}

			if( UIUtils.CurrentShaderVersion() > 15403 )
			{
				int dependencyCount = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				for( int i = 0; i < dependencyCount; i++ )
				{
					m_dependencies.Add( new CustomExpressionDependency( GetCurrentParam( ref nodeParams ) ) );
				}
			}

			if( m_mode == CustomExpressionMode.Create )
			{
				m_containerGraph.CustomExpressionOnFunctionMode.AddNode( this );
			}
			UpdateOutputPorts();

			m_repopulateNameDictionary = true;
			m_functionMode = m_code.Contains( ReturnHelper );
			CheckCallMode();

		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );

			m_code = m_code.Replace( "\r\n", "\n" );

			string parsedCode = m_code.Replace( '\n', LineFeedSeparator );
			parsedCode = parsedCode.Replace( ';', SemiColonSeparator );

			IOUtils.AddFieldValueToString( ref nodeInfo, parsedCode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_outputTypeIdx );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_mode == CustomExpressionMode.Call );

			int count = m_inputPorts.Count - m_firstAvailablePort;
			IOUtils.AddFieldValueToString( ref nodeInfo, count );
			for( int i = 0; i < count; i++ )
			{
				int portIdx = m_firstAvailablePort + i;
				IOUtils.AddFieldValueToString( ref nodeInfo, m_items[ i ].FoldoutFlag );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_inputPorts[ portIdx ].Name );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_inputPorts[ portIdx ].DataType );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_inputPorts[ portIdx ].InternalData );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_items[ i ].Qualifier );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_items[ i ].CustomType );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_items[ i ].Precision );
			}
			IOUtils.AddFieldValueToString( ref nodeInfo, m_customExpressionName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_generateUniqueName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_autoRegisterMode );
			count = m_dependencies.Count;
			IOUtils.AddFieldValueToString( ref nodeInfo, count );
			for( int i = 0; i < count; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_dependencies[ i ].DependencyNodeId );
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			if( m_mode == CustomExpressionMode.Create )
			{
				m_containerGraph.CustomExpressionOnFunctionMode.RemoveNode( this );
			}
			m_items.Clear();
			m_items = null;
			m_dependencies.Clear();
			m_dependencies = null;
			m_itemReordableList = null;
		}

		public void CheckDependencies( ref MasterNodeDataCollector dataCollector, ref Dictionary<int, CustomExpressionNode> examinedNodes )
		{
			if( !examinedNodes.ContainsKey( UniqueId ) && m_mode == CustomExpressionMode.Create && !m_generateUniqueName )
			{
				int dependencyCount = m_dependencies.Count;
				for( int d = 0; d < dependencyCount; d++ )
				{
					if( !examinedNodes.ContainsKey( m_dependencies[ d ].DependencyNodeId ) )
					{
						CustomExpressionNode dNode = m_containerGraph.GetNode( m_dependencies[ d ].DependencyNodeId ) as CustomExpressionNode;
						if( dNode != null )
						{
							dNode.CheckDependencies( ref dataCollector, ref examinedNodes );
						}
					}
				}
				dataCollector.AddFunction( ExpressionName, EncapsulatedCode( dataCollector.IsTemplate ) );
				examinedNodes.Add( UniqueId, this );
			}
		}

		public string EncapsulatedCode( bool isTemplate )
		{
			string functionName = UIUtils.RemoveInvalidCharacters( m_customExpressionName );
			if( m_generateUniqueName )
			{
				functionName += OutputId;
			}
			return WrapCodeInFunction( isTemplate, functionName, false );
		}

		public CustomExpressionMode Mode
		{
			get { return m_mode; }
			set
			{
				if( m_mode != value )
				{
					m_mode = value;
					if( m_mode == CustomExpressionMode.Call )
					{
						AutoRegisterMode = false;
						m_generateUniqueName = false;
						m_containerGraph.CustomExpressionOnFunctionMode.RemoveNode( this );
					}
					else
					{
						m_containerGraph.CustomExpressionOnFunctionMode.AddNode( this );
					}
				}
			}
		}

		public string ExpressionName
		{
			get
			{
				string expressionName = UIUtils.RemoveInvalidCharacters( m_customExpressionName );

				if( m_generateUniqueName )
				{
					expressionName += OutputId;
				}
				return expressionName;
			}
		}
		public override string DataToArray { get { return m_customExpressionName; } }
		public bool AutoRegisterMode
		{
			get { return m_autoRegisterMode; }
			set
			{
				if( value != m_autoRegisterMode )
				{
					m_autoRegisterMode = value;
				}
			}
		}

		public override void RefreshExternalReferences()
		{
			base.RefreshExternalReferences();
			int portCount = m_inputPorts.Count;
			for( int i = 0; i < portCount; i++ )
			{
				if( m_inputPorts[ i ].DataType == WirePortDataType.COLOR )
				{
					m_inputPorts[ i ].ChangeType( WirePortDataType.FLOAT4, false ); ;
				}
			}

			int dependencyCount = m_dependencies.Count;
			for( int i = 0; i < dependencyCount; i++ )
			{
				m_dependencies[ i ].DependencyArrayIdx = m_containerGraph.CustomExpressionOnFunctionMode.GetNodeRegisterIdx( m_dependencies[ i ].DependencyNodeId );
			}
		}

		public override void FireTimedUpdate()
		{
			m_containerGraph.CustomExpressionOnFunctionMode.UpdateDataOnNode( UniqueId, m_customExpressionName );
		}

		public List<CustomExpressionDependency> Dependencies { get { return m_dependencies; } }
	}
}
