// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Debug Switch", "Logical Operators", "Hard Switch between any of its input ports" )]
	public class SwitchNode : ParentNode
	{
		private const string Info = "This is a Debug node which only generates the source for the selected port. This means that no properties are generated for other ports and information might be lost.";
		private const string InputPortName = "In ";

		private const string CurrSelectedStr = "Current";
		private const string MaxAmountStr = "Max Amount";
		private const int MaxAllowedAmount = 8;

		[SerializeField]
		private string[] m_availableInputsLabels = { "In 0", "In 1" };

		[SerializeField]
		private int[] m_availableInputsValues = { 0, 1 };

		[SerializeField]
		private int m_currentSelectedInput = 0;

		[SerializeField]
		private int m_maxAmountInputs = 2;

		private int m_cachedPropertyId = -1;

		private GUIContent m_popContent;

		private Rect m_varRect;
		private Rect m_imgRect;
		private bool m_editing;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			for( int i = 0; i < MaxAllowedAmount; i++ )
			{
				AddInputPort( WirePortDataType.FLOAT, false, InputPortName + i );
				m_inputPorts[ i ].Visible = ( i < 2 );
			}
			AddOutputPort( WirePortDataType.FLOAT, " " );

			m_popContent = new GUIContent();
			m_popContent.image = UIUtils.PopupIcon;

			m_insideSize.Set( 50, 25 );
			m_textLabelWidth = 100;
			m_autoWrapProperties = false;
			m_previewShaderGUID = "a58e46feaa5e3d14383bfeac24d008bc";
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if( m_cachedPropertyId == -1 )
				m_cachedPropertyId = Shader.PropertyToID( "_Current" );

			PreviewMaterial.SetInt( m_cachedPropertyId, m_currentSelectedInput );
		}

		public override void OnConnectedOutputNodeChanges( int inputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			m_inputPorts[ inputPortId ].MatchPortToConnection();
			if( inputPortId == m_currentSelectedInput )
				m_outputPorts[ 0 ].ChangeType( m_inputPorts[ inputPortId ].DataType, false );
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			m_inputPorts[ portId ].MatchPortToConnection();
			if( portId == m_currentSelectedInput )
				m_outputPorts[ 0 ].ChangeType( m_inputPorts[ portId ].DataType, false );
		}

		public void UpdateLabels()
		{
			m_availableInputsLabels = new string[ m_maxAmountInputs ];
			m_availableInputsValues = new int[ m_maxAmountInputs ];

			for( int i = 0; i < m_maxAmountInputs; i++ )
			{
				m_availableInputsLabels[ i ] = InputPortName + i;
				m_availableInputsValues[ i ] = i;
			}
		}

		//void UpdateOutput()
		//{
		//	m_outputPorts[ 0 ].ChangeProperties( m_inputPorts[ m_currentSelectedInput ].Name, m_inputPorts[ m_currentSelectedInput ].DataType, false );
		//	m_sizeIsDirty = true;
		//}

		public override void OnNodeLayout( DrawInfo drawInfo )
		{
			base.OnNodeLayout( drawInfo );

			m_varRect = m_remainingBox;
			m_varRect.width = 50 * drawInfo.InvertedZoom;
			m_varRect.height = 16 * drawInfo.InvertedZoom;
			m_varRect.x = m_remainingBox.xMax - m_varRect.width;
			//m_varRect.x += m_remainingBox.width * 0.5f - m_varRect.width * 0.5f;
			m_varRect.y += 1 * drawInfo.InvertedZoom;

			m_imgRect = m_varRect;
			m_imgRect.x = m_varRect.xMax - 16 * drawInfo.InvertedZoom;
			m_imgRect.width = 16 * drawInfo.InvertedZoom;
			m_imgRect.height = m_imgRect.width;
		}

		public override void DrawGUIControls( DrawInfo drawInfo )
		{
			base.DrawGUIControls( drawInfo );

			if( drawInfo.CurrentEventType != EventType.MouseDown )
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

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );

			if( m_editing )
			{
				EditorGUI.BeginChangeCheck();
				m_currentSelectedInput = EditorGUIIntPopup( m_varRect, m_currentSelectedInput, m_availableInputsLabels, m_availableInputsValues, UIUtils.GraphDropDown );
				if( EditorGUI.EndChangeCheck() )
				{
					m_editing = false;
				}
			}
		}

		public override void OnNodeRepaint( DrawInfo drawInfo )
		{
			base.OnNodeRepaint( drawInfo );

			if( !m_isVisible )
				return;

			if( !m_editing && ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD4 )
			{
				GUI.Label( m_varRect, m_availableInputsLabels[ m_currentSelectedInput ], UIUtils.GraphDropDown );
				GUI.Label( m_imgRect, m_popContent, UIUtils.GraphButtonIcon );
			}
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			NodeUtils.DrawPropertyGroup( ref m_propertiesFoldout, Constants.ParameterLabelStr, DrawDebugOptions );

			EditorGUILayout.HelpBox( Info, MessageType.Warning );
		}

		void DrawDebugOptions()
		{
			EditorGUI.BeginChangeCheck();
			m_maxAmountInputs = EditorGUILayoutIntSlider( MaxAmountStr, m_maxAmountInputs, 2, MaxAllowedAmount );
			if( EditorGUI.EndChangeCheck() )
			{
				for( int i = 0; i < MaxAllowedAmount; i++ )
				{
					m_inputPorts[ i ].Visible = ( i < m_maxAmountInputs );
				}

				if( m_currentSelectedInput >= m_maxAmountInputs )
				{
					m_currentSelectedInput = m_maxAmountInputs - 1;
				}

				UpdateLabels();
				m_sizeIsDirty = true;
			}

			m_currentSelectedInput = EditorGUILayoutIntPopup( CurrSelectedStr, m_currentSelectedInput, m_availableInputsLabels, m_availableInputsValues );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			return m_inputPorts[ m_currentSelectedInput ].GeneratePortInstructions( ref dataCollector );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_currentSelectedInput = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_maxAmountInputs = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );

			for( int i = 0; i < MaxAllowedAmount; i++ )
			{
				m_inputPorts[ i ].Visible = ( i < m_maxAmountInputs );
			}

			if( m_currentSelectedInput >= m_maxAmountInputs )
			{
				m_currentSelectedInput = m_maxAmountInputs - 1;
			}

			UpdateLabels();
			m_sizeIsDirty = true;
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_currentSelectedInput );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_maxAmountInputs );
		}
	}
}
