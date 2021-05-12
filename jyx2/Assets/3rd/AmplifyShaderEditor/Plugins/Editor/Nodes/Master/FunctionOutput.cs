// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Function Output", "Functions", "Function Output adds an output port to the shader function, it's port type is determined automatically.", NodeAvailabilityFlags = (int)NodeAvailability.ShaderFunction )]
	public sealed class FunctionOutput : OutputNode
	{
		public FunctionOutput() : base() { CommonInit(); }
		public FunctionOutput( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { CommonInit(); }

		[SerializeField]
		private bool m_previewNode = false;

		[SerializeField]
		private string m_outputName = "Output";

		[SerializeField]
		private int m_orderIndex = -1;
		
		[SerializeField]
		private AmplifyShaderFunction m_function;

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

		[SerializeField]
		private string m_subTitle = string.Empty;


		void CommonInit()
		{
			m_isMainOutputNode = false;
			m_connStatus = NodeConnectionStatus.Connected;
			m_activeType = GetType();
			m_currentPrecisionType = PrecisionType.Float;
			m_textLabelWidth = 100;
			m_autoWrapProperties = true;
			AddInputPort( WirePortDataType.FLOAT, false, "  " );
			AddOutputPort( WirePortDataType.FLOAT, "  " );
			m_outputPorts[ 0 ].Visible = false;
			SetTitleText( m_outputName );
			m_previewShaderGUID = "e6d5f64114b18e24f99dc65290c0fe98";
		}

		public override void SetupNodeCategories()
		{
			//base.SetupNodeCategories();
			ContainerGraph.ResetNodesData();
			MasterNode masterNode = ContainerGraph.ParentWindow.CurrentGraph.CurrentMasterNode;
			if( masterNode != null )
			{
				int count = m_inputPorts.Count;
				for( int i = 0; i < count; i++ )
				{
					if( m_inputPorts[ i ].IsConnected )
					{
						NodeData nodeData = new NodeData( m_inputPorts[ i ].Category );
						ParentNode node = m_inputPorts[ i ].GetOutputNode();
						MasterNodeDataCollector temp = masterNode.CurrentDataCollector;
						node.PropagateNodeData( nodeData, ref temp );
						temp = null;
					}
				}
			}
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			UIUtils.RegisterFunctionOutputNode( this );
			if( m_nodeAttribs != null )
				m_uniqueName = m_nodeAttribs.Name + UniqueId;
		}


		public override void Destroy()
		{
			base.Destroy();
			UIUtils.UnregisterFunctionOutputNode( this );
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			m_inputPorts[ 0 ].MatchPortToConnection();
			m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
		}

		public override void OnConnectedOutputNodeChanges( int outputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( outputPortId, otherNodeId, otherPortId, name, type );
			m_inputPorts[ 0 ].MatchPortToConnection();
			m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			return m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUI.BeginChangeCheck();
			m_outputName = EditorGUILayoutTextField( "Name", m_outputName );

			if( EditorGUI.EndChangeCheck() )
			{
				SetTitleText( m_outputName );
				UIUtils.UpdateFunctionOutputData( UniqueId, m_outputName );
			}

			EditorGUI.BeginDisabledGroup( m_previewNode );
			if( GUILayout.Button( "Set as Preview" ) )
			{
				List<FunctionOutput> allOutputs = UIUtils.FunctionOutputList();

				foreach( FunctionOutput item in allOutputs )
					item.PreviewNode = false;

				m_previewNode = true;
			}
			EditorGUI.EndDisabledGroup();
		}
		[SerializeField]
		private string m_currentTitle = string.Empty;

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			if( m_previewNode )
				m_currentTitle = "Preview";
			else
				m_currentTitle = string.Empty;

			SetAdditonalTitleTextOnCallback( m_currentTitle, ( instance, newSubTitle ) => instance.AdditonalTitleContent.text = newSubTitle );

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
					m_outputName = EditorGUITextField( m_titleClickArea, string.Empty, m_outputName, UIUtils.GetCustomStyle( CustomStyle.NodeTitle ) );
					if( EditorGUI.EndChangeCheck() )
					{
						SetTitleText( m_outputName );
						UIUtils.UpdateFunctionInputData( UniqueId, m_outputName );
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
		
		public WirePortDataType AutoOutputType
		{
			get { return m_inputPorts[ 0 ].DataType; }
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_outputName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_orderIndex );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_previewNode );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_outputName = GetCurrentParam( ref nodeParams );
			m_orderIndex = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );

			if( UIUtils.CurrentShaderVersion() > 13706 )
				m_previewNode = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );

			if( IsNodeBeingCopied )
				PreviewNode = false;

			if( m_function == null )
				m_function = UIUtils.CurrentWindow.OpenedShaderFunction;

			if( m_isMainOutputNode && m_function != null )
			{
				m_function.UpdateDirectivesList();
			}

			SetTitleText( m_outputName );
			UIUtils.UpdateFunctionOutputData( UniqueId, m_outputName );
		}
		
		public AmplifyShaderFunction Function
		{
			get { return m_function; }
			set
			{
				m_function = value;
				if( m_isMainOutputNode && m_function != null )
				{
					m_function.UpdateDirectivesList();
				}
			}
		}

		public string OutputName
		{
			get { return m_outputName; }
		}

		public int OrderIndex
		{
			get { return m_orderIndex; }
			set { m_orderIndex = value; }
		}

		public string SubTitle
		{
			get { return m_subTitle; }
			set { m_subTitle = value; }
		}

		public bool PreviewNode
		{
			get { return m_previewNode; }
			set
			{
				m_previewNode = value;
				m_sizeIsDirty = true;
				if( m_previewNode )
				{
					m_currentTitle = "Preview";
				}
				else
				{
					m_currentTitle = "";
				}
			}
		}
	}
}
