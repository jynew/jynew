// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Function Input", "Functions", "Function Input adds an input port to the shader function", NodeAvailabilityFlags = (int)NodeAvailability.ShaderFunction )]
	public sealed class FunctionInput : ParentNode
	{
		private const string InputTypeStr = "Input Type";
		private readonly string[] m_inputValueTypes ={  "Int",
														"Float",
														"Vector2",
														"Vector3",
														"Vector4",
														"Color",
														"Matrix 3x3",
														"Matrix 4x4",
														"Sampler 1D",
														"Sampler 2D",
														"Sampler 3D",
														"Sampler Cube"};

		[SerializeField]
		private int m_selectedInputTypeInt = 1;

		private WirePortDataType m_selectedInputType = WirePortDataType.FLOAT;

		[SerializeField]
		private string m_inputName = "Input";

		[SerializeField]
		private bool m_autoCast = false;

		[SerializeField]
		private int m_orderIndex = -1;

		private int m_typeId = -1;

		public bool m_ignoreConnection = false;

		public delegate string PortGeneration( ref MasterNodeDataCollector dataCollector, int index, ParentGraph graph );
		public PortGeneration OnPortGeneration = null;

		//Title editing 
		[SerializeField]
		private string m_uniqueName;

		private bool m_isEditing;
		private bool m_stopEditing;
		private bool m_startEditing;
		private double m_clickTime;
		private double m_doubleClickTime = 0.3;
		private Rect m_titleClickArea;
		private bool m_showTitleWhenNotEditing = true;


		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, Constants.EmptyPortValue );
			m_inputPorts[ 0 ].AutoDrawInternalData = true;
			//m_inputPorts[ 0 ].Visible = false;
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_autoWrapProperties = true;
			m_textLabelWidth = 100;
			SetTitleText( m_inputName );
			UpdatePorts();
			SetAdditonalTitleText( "( " + m_inputValueTypes[ m_selectedInputTypeInt ] + " )" );
			m_previewShaderGUID = "04bc8e7b317dccb4d8da601680dd8140";
		}

		public override void SetPreviewInputs()
		{
			if( !m_ignoreConnection )
				base.SetPreviewInputs();

			if( m_typeId == -1 )
				m_typeId = Shader.PropertyToID( "_Type" );

			if( m_inputPorts[ 0 ].DataType == WirePortDataType.FLOAT || m_inputPorts[ 0 ].DataType == WirePortDataType.INT )
				PreviewMaterial.SetInt( m_typeId, 1 );
			else if( m_inputPorts[ 0 ].DataType == WirePortDataType.FLOAT2 )
				PreviewMaterial.SetInt( m_typeId, 2 );
			else if( m_inputPorts[ 0 ].DataType == WirePortDataType.FLOAT3 )
				PreviewMaterial.SetInt( m_typeId, 3 );
			else
				PreviewMaterial.SetInt( m_typeId, 0 );

		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			UIUtils.RegisterFunctionInputNode( this );
			if( m_nodeAttribs != null )
				m_uniqueName = m_nodeAttribs.Name + UniqueId;
		}
		
		public override void Destroy()
		{
			base.Destroy();
			OnPortGeneration = null;
			UIUtils.UnregisterFunctionInputNode( this );
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			if( AutoCast )
			{
				m_inputPorts[ 0 ].MatchPortToConnection();
				SetIntTypeFromPort();
				UpdatePorts();
				SetAdditonalTitleText( "( " + m_inputValueTypes[ m_selectedInputTypeInt ] + " )" );
			}
		}

		public override void OnConnectedOutputNodeChanges( int portId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( portId, otherNodeId, otherPortId, name, type );
			if( AutoCast )
			{
				m_inputPorts[ 0 ].MatchPortToConnection();
				SetIntTypeFromPort();
				UpdatePorts();
				SetAdditonalTitleText( "( " + m_inputValueTypes[ m_selectedInputTypeInt ] + " )" );
			}
		}

		public void SetIntTypeFromPort()
		{
			switch( m_inputPorts[ 0 ].DataType )
			{
				case WirePortDataType.INT: m_selectedInputTypeInt = 0; break;
				default:
				case WirePortDataType.FLOAT: m_selectedInputTypeInt = 1; break;
				case WirePortDataType.FLOAT2: m_selectedInputTypeInt = 2; break;
				case WirePortDataType.FLOAT3: m_selectedInputTypeInt = 3; break;
				case WirePortDataType.FLOAT4: m_selectedInputTypeInt = 4; break;
				case WirePortDataType.COLOR: m_selectedInputTypeInt = 5; break;
				case WirePortDataType.FLOAT3x3: m_selectedInputTypeInt = 6; break;
				case WirePortDataType.FLOAT4x4: m_selectedInputTypeInt = 7; break;
				case WirePortDataType.SAMPLER1D: m_selectedInputTypeInt = 8; break;
				case WirePortDataType.SAMPLER2D: m_selectedInputTypeInt = 9; break;
				case WirePortDataType.SAMPLER3D: m_selectedInputTypeInt = 10; break;
				case WirePortDataType.SAMPLERCUBE: m_selectedInputTypeInt = 11; break;
			}
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
					m_inputName = EditorGUITextField( m_titleClickArea, string.Empty, m_inputName, UIUtils.GetCustomStyle( CustomStyle.NodeTitle ) );
					if( EditorGUI.EndChangeCheck() )
					{
						SetTitleText( m_inputName );
						UIUtils.UpdateFunctionInputData( UniqueId, m_inputName );
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

		public override void OnNodeDoubleClicked( Vector2 currentMousePos2D )
		{
			if( currentMousePos2D.y - m_globalPosition.y > ( Constants.NODE_HEADER_HEIGHT + Constants.NODE_HEADER_EXTRA_HEIGHT ) * ContainerGraph.ParentWindow.CameraDrawInfo.InvertedZoom )
			{
				ContainerGraph.ParentWindow.ParametersWindow.IsMaximized = !ContainerGraph.ParentWindow.ParametersWindow.IsMaximized;
			}
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUILayout.BeginVertical();
			EditorGUI.BeginChangeCheck();
			m_inputName = EditorGUILayoutTextField( "Name", m_inputName );
			if( EditorGUI.EndChangeCheck() )
			{
				SetTitleText( m_inputName );
				UIUtils.UpdateFunctionInputData( UniqueId, m_inputName );
			}
			EditorGUI.BeginChangeCheck();
			m_selectedInputTypeInt = EditorGUILayoutPopup( InputTypeStr, m_selectedInputTypeInt, m_inputValueTypes );
			if( EditorGUI.EndChangeCheck() )
			{
				UpdatePorts();
				SetAdditonalTitleText( "( " + m_inputValueTypes[ m_selectedInputTypeInt ] + " )" );
			}

			m_autoCast = EditorGUILayoutToggle( "Auto Cast", m_autoCast );

			EditorGUILayout.Separator();
			if( !m_inputPorts[ 0 ].IsConnected && m_inputPorts[ 0 ].ValidInternalData )
			{
				m_inputPorts[ 0 ].ShowInternalData( this, true, "Default Value" );
			}


			EditorGUILayout.EndVertical();
		}

		void UpdatePorts()
		{
			//switch( m_inputPorts[ 0 ].DataType )
			//{
			//	case WirePortDataType.INT: m_selectedInputTypeInt = 0; break;
			//	default:
			//	case WirePortDataType.FLOAT: m_selectedInputTypeInt = 1; break;
			//	case WirePortDataType.FLOAT2: m_selectedInputTypeInt = 2; break;
			//	case WirePortDataType.FLOAT3: m_selectedInputTypeInt = 3; break;

			//	//case 2: m_selectedInputType = WirePortDataType.FLOAT2; break;
			//	//case 3: m_selectedInputType = WirePortDataType.FLOAT3; break;
			//	//case 4: m_selectedInputType = WirePortDataType.FLOAT4; break;
			//	//case 5: m_selectedInputType = WirePortDataType.COLOR; break;
			//	//case 6: m_selectedInputType = WirePortDataType.FLOAT3x3; break;
			//	//case 7: m_selectedInputType = WirePortDataType.FLOAT4x4; break;
			//	//case 8: m_selectedInputType = WirePortDataType.SAMPLER1D; break;
			//	//case 9: m_selectedInputType = WirePortDataType.SAMPLER2D; break;
			//	//case 10: m_selectedInputType = WirePortDataType.SAMPLER3D; break;
			//	//case 11: m_selectedInputType = WirePortDataType.SAMPLERCUBE; break;
			//}

			switch( m_selectedInputTypeInt )
			{
				case 0: m_selectedInputType = WirePortDataType.INT; break;
				default:
				case 1: m_selectedInputType = WirePortDataType.FLOAT; break;
				case 2: m_selectedInputType = WirePortDataType.FLOAT2; break;
				case 3: m_selectedInputType = WirePortDataType.FLOAT3; break;
				case 4: m_selectedInputType = WirePortDataType.FLOAT4; break;
				case 5: m_selectedInputType = WirePortDataType.COLOR; break;
				case 6: m_selectedInputType = WirePortDataType.FLOAT3x3; break;
				case 7: m_selectedInputType = WirePortDataType.FLOAT4x4; break;
				case 8: m_selectedInputType = WirePortDataType.SAMPLER1D; break;
				case 9: m_selectedInputType = WirePortDataType.SAMPLER2D; break;
				case 10: m_selectedInputType = WirePortDataType.SAMPLER3D; break;
				case 11: m_selectedInputType = WirePortDataType.SAMPLERCUBE; break;
			}

			ChangeInputType( m_selectedInputType, false );

			//This node doesn't have any restrictions but changing types should be restricted to prevent invalid connections
			m_outputPorts[ 0 ].ChangeTypeWithRestrictions( m_selectedInputType, PortCreateRestriction( m_selectedInputType ) );
			m_sizeIsDirty = true;
		}

		public int PortCreateRestriction( WirePortDataType dataType )
		{
			int restrictions = 0;
			WirePortDataType[] types = null;
			switch( dataType )
			{
				case WirePortDataType.OBJECT:
				break;
				case WirePortDataType.FLOAT:
				case WirePortDataType.FLOAT2:
				case WirePortDataType.FLOAT3:
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				case WirePortDataType.INT:
				{
					types = new WirePortDataType[] { WirePortDataType.FLOAT, WirePortDataType.FLOAT2, WirePortDataType.FLOAT3, WirePortDataType.FLOAT4, WirePortDataType.COLOR, WirePortDataType.INT, WirePortDataType.OBJECT };
				}
				break;
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{
					types = new WirePortDataType[] { WirePortDataType.FLOAT3x3, WirePortDataType.FLOAT4x4, WirePortDataType.OBJECT };
				}
				break;
				case WirePortDataType.SAMPLER1D:
				case WirePortDataType.SAMPLER2D:
				case WirePortDataType.SAMPLER3D:
				case WirePortDataType.SAMPLERCUBE:
				{
					types = new WirePortDataType[] { WirePortDataType.SAMPLER1D, WirePortDataType.SAMPLER2D, WirePortDataType.SAMPLER3D, WirePortDataType.SAMPLERCUBE, WirePortDataType.OBJECT };
				}
				break;
				default:
				break;
			}

			if( types != null )
			{
				for( int i = 0; i < types.Length; i++ )
				{
					restrictions = restrictions | (int)types[ i ];
				}
			}

			return restrictions;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( m_outputPorts[ outputId ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ outputId ].LocalValue( dataCollector.PortCategory );

			string result = string.Empty;
			if( OnPortGeneration != null )
				result = OnPortGeneration( ref dataCollector, m_orderIndex, ContainerGraph.ParentWindow.CustomGraph );
			else
				result = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );

			if( m_outputPorts[ outputId ].ConnectionCount > 1 )
				RegisterLocalVariable( outputId, result, ref dataCollector );
			else
				m_outputPorts[ outputId ].SetLocalValue( result, dataCollector.PortCategory );

			return m_outputPorts[ outputId ].LocalValue( dataCollector.PortCategory );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_inputName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_selectedInputTypeInt );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_orderIndex );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_autoCast );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_inputName = GetCurrentParam( ref nodeParams );
			m_selectedInputTypeInt = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_orderIndex = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_autoCast = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );

			SetTitleText( m_inputName );
			UpdatePorts();
			SetAdditonalTitleText( "( " + m_inputValueTypes[ m_selectedInputTypeInt ] + " )" );
		}

		public WirePortDataType SelectedInputType
		{
			get { return m_selectedInputType; }
		}

		public string InputName
		{
			get { return m_inputName; }
		}

		public int OrderIndex
		{
			get { return m_orderIndex; }
			set { m_orderIndex = value; }
		}

		public bool AutoCast
		{
			get { return m_autoCast; }
			set { m_autoCast = value; }
		}
	}
}
