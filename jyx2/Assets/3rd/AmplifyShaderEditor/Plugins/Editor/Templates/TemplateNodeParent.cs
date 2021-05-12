// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class TemplateNodeParent : ParentNode
	{
		

		protected const string ErrorMessageStr = "This node can only be used inside a Template category!";
		protected const string DataLabelStr = "Data";
		protected const string SubShaderStr = "SubShader";
		protected const string PassStr = "Pass";

		[SerializeField]
		private int m_subShaderIdx = 0;

		[SerializeField]
		private int m_passIdx = 0;

		[SerializeField]
		private int m_passLocalArrayIdx = 0;

		[SerializeField]
		protected bool m_multiPassMode = false;

		[SerializeField]
		protected string[] m_availableSubshaders;

		[SerializeField]
		protected string[] m_availablePassesLabels;

		[SerializeField]
		protected int[] m_availablePassesValues;

		[NonSerialized]
		protected TemplateMultiPass m_templateMPData = null;
		protected bool m_createAllOutputs = true;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputPort( WirePortDataType.FLOAT, "Out" );
			if( m_createAllOutputs )
			{
				AddOutputPort( WirePortDataType.FLOAT, "X" );
				AddOutputPort( WirePortDataType.FLOAT, "Y" );
				AddOutputPort( WirePortDataType.FLOAT, "Z" );
				AddOutputPort( WirePortDataType.FLOAT, "W" );
			}
			m_textLabelWidth = 67;
			m_hasLeftDropdown = true;
		}

		public override void AfterCommonInit()
		{
			base.AfterCommonInit();

			if( PaddingTitleLeft == 0 )
			{
				PaddingTitleLeft = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
				if( PaddingTitleRight == 0 )
					PaddingTitleRight = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
			}
		}

		protected void ConfigurePorts()
		{
			switch( m_outputPorts[ 0 ].DataType )
			{
				default:
				{
					for( int i = 1; i < 5; i++ )
					{
						m_outputPorts[ i ].Visible = false;
					}
				}
				break;
				case WirePortDataType.FLOAT2:
				{
					for( int i = 1; i < 5; i++ )
					{
						m_outputPorts[ i ].Visible = ( i < 3 );
						if( m_outputPorts[ i ].Visible )
						{
							m_outputPorts[ i ].Name = Constants.ChannelNamesVector[ i - 1 ];
						}
					}
				}
				break;
				case WirePortDataType.FLOAT3:
				{
					for( int i = 1; i < 5; i++ )
					{
						m_outputPorts[ i ].Visible = ( i < 4 );
						if( m_outputPorts[ i ].Visible )
						{
							m_outputPorts[ i ].Name = Constants.ChannelNamesVector[ i - 1 ];
						}
					}
				}
				break;
				case WirePortDataType.FLOAT4:
				{
					for( int i = 1; i < 5; i++ )
					{
						m_outputPorts[ i ].Visible = true;
						m_outputPorts[ i ].Name = Constants.ChannelNamesVector[ i - 1 ];
					}
				}
				break;
				case WirePortDataType.COLOR:
				{
					for( int i = 1; i < 5; i++ )
					{
						m_outputPorts[ i ].Visible = true;
						m_outputPorts[ i ].Name = Constants.ChannelNamesColor[ i - 1 ];
					}
				}
				break;
			}
			m_sizeIsDirty = true;
		}

		protected virtual void OnSubShaderChange() { }
		protected virtual void OnPassChange() { }

		protected void DrawSubShaderUI()
		{
			EditorGUI.BeginChangeCheck();
			m_subShaderIdx = EditorGUILayoutPopup( SubShaderStr, m_subShaderIdx, m_availableSubshaders );
			if( EditorGUI.EndChangeCheck() )
			{
				//UpdateSubShaderAmount();
				UpdatePassAmount();
				OnSubShaderChange();
			}
		}

		protected void DrawPassUI()
		{
			EditorGUI.BeginChangeCheck();
			m_passLocalArrayIdx = EditorGUILayoutPopup( PassStr, m_passLocalArrayIdx, m_availablePassesLabels );
			if( EditorGUI.EndChangeCheck() )
			{
				m_passIdx = m_availablePassesValues[ m_passLocalArrayIdx ];
				//UpdatePassAmount();
				OnPassChange();
			}
		}
		
		virtual protected void CheckWarningState()
		{
			if( m_containerGraph.CurrentCanvasMode != NodeAvailability.TemplateShader )
			{
				ShowTab( NodeMessageType.Error, ErrorMessageStr );
			}
			else
			{
				m_showErrorMessage = false;
			}
		}

		protected void FetchMultiPassTemplate( MasterNode masterNode = null )
		{
			m_multiPassMode = m_containerGraph.MultiPassMasterNodes.NodesList.Count > 0;
			if( m_multiPassMode )
			{
				m_templateMPData = ( ( ( masterNode == null ) ? m_containerGraph.CurrentMasterNode : masterNode ) as TemplateMultiPassMasterNode ).CurrentTemplate;
				if( m_templateMPData != null )
				{
					UpdateSubShaderAmount();
				}
			}
		}

		protected void UpdateSubShaderAmount()
		{
			if( m_templateMPData == null )
				m_templateMPData = ( m_containerGraph.CurrentMasterNode as TemplateMultiPassMasterNode ).CurrentTemplate;

			if( m_templateMPData != null )
			{
				int subShaderCount = m_templateMPData.SubShaders.Count;
				if( m_availableSubshaders == null || subShaderCount != m_availableSubshaders.Length )
				{
					m_availableSubshaders = new string[ subShaderCount ];
					for( int i = 0; i < subShaderCount; i++ )
					{
						m_availableSubshaders[ i ] = i.ToString();
					}
				}
				m_subShaderIdx = Mathf.Min( m_subShaderIdx, subShaderCount - 1 );
				UpdatePassAmount();
			}
		}
		protected virtual bool ValidatePass( int passIdx ) { return true; }
		protected void UpdatePassAmount()
		{
			if( !m_multiPassMode )
				return;

			if( m_templateMPData == null )
				m_templateMPData = ( m_containerGraph.CurrentMasterNode as TemplateMultiPassMasterNode ).CurrentTemplate;

			List<string> passLabels = new List<string>();
			List<int> passValues = new List<int>();
			int minPassIdx = int.MaxValue;
			int passCount = m_templateMPData.SubShaders[ m_subShaderIdx ].Passes.Count;
			bool resetPassIdx = true;
			for( int i = 0; i < passCount; i++ )
			{
				if( ValidatePass( i ) )
				{
					passLabels.Add( i.ToString() );
					passValues.Add( i );
					minPassIdx = Mathf.Min( minPassIdx, i );
					if( m_passIdx == i )
						resetPassIdx = false;
				}
			}
			m_availablePassesLabels = passLabels.ToArray();
			m_availablePassesValues = passValues.ToArray();
			if( resetPassIdx )
				m_passIdx = minPassIdx;

			RefreshPassLocalArrayIdx();
		}

		void RefreshPassLocalArrayIdx( )
		{
			for( int i = 0; i < m_availablePassesValues.Length; i++ )
			{
				if( m_availablePassesValues[ i ] == m_passIdx )
				{
					m_passLocalArrayIdx = i;
				}
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			m_templateMPData = null;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if( UIUtils.CurrentShaderVersion() > TemplatesManager.MPShaderVersion )
			{
				m_subShaderIdx = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				m_passIdx = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_subShaderIdx );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_passIdx );
		}
		public int SubShaderIdx { get { return m_subShaderIdx; } }
		public int PassIdx { get { return m_passIdx; } }
	}
}
