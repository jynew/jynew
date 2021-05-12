// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Template Local Var Data", "Surface Data", "Select and use available local variable data from the template" )]
	public sealed class TemplateLocalVarsNode : TemplateNodeParent
	{
		private List<TemplateLocalVarData> m_localVarsData = null;

		[SerializeField]
		private int m_currentDataIdx = -1;

		[SerializeField]
		private string m_dataName = string.Empty;

		private string[] m_dataLabels = null;

		private bool m_fetchDataId = false;
		private UpperLeftWidgetHelper m_upperLeftWidgetHelper = new UpperLeftWidgetHelper();

		void FetchDataId()
		{
			if( m_localVarsData != null && m_localVarsData.Count > 0 )
			{
				m_currentDataIdx = 0;
				int count = m_localVarsData.Count;
				m_dataLabels = new string[ count ];
				for( int i = 0; i < count; i++ )
				{
					m_dataLabels[ i ] = m_localVarsData[ i ].LocalVarName;
					if( m_localVarsData[ i ].LocalVarName.Equals( m_dataName ) )
					{
						m_currentDataIdx = i;
					}
				}
				UpdateFromId();
			}
			else
			{
				m_currentDataIdx = -1;
			}
		}

		void UpdateFromId()
		{
			if( m_localVarsData != null )
			{
				if( m_localVarsData.Count == 0 )
				{
					for( int i = 0; i < 4; i++ )
						m_containerGraph.DeleteConnection( false, UniqueId, i, false, true );

					m_headerColor = UIUtils.GetColorFromCategory( "Default" );
					m_content.text = "None";
					m_additionalContent.text = string.Empty;
					m_outputPorts[ 0 ].ChangeProperties( "None", WirePortDataType.OBJECT, false );
					ConfigurePorts();
					return;
				}

				bool areCompatible = TemplateHelperFunctions.CheckIfCompatibles( m_outputPorts[ 0 ].DataType, m_localVarsData[ m_currentDataIdx ].DataType );
				string category = m_localVarsData[ m_currentDataIdx ].Category == MasterNodePortCategory.Fragment ? "Surface Data" : "Vertex Data";
				m_headerColor = UIUtils.GetColorFromCategory( category );
				switch( m_localVarsData[ m_currentDataIdx ].DataType )
				{
					default:
					case WirePortDataType.INT:
					case WirePortDataType.FLOAT:
					m_outputPorts[ 0 ].ChangeProperties( Constants.EmptyPortValue, m_localVarsData[ m_currentDataIdx ].DataType, false );
					break;
					case WirePortDataType.FLOAT2:
					m_outputPorts[ 0 ].ChangeProperties( "XY", m_localVarsData[ m_currentDataIdx ].DataType, false );
					break;
					case WirePortDataType.FLOAT3:
					m_outputPorts[ 0 ].ChangeProperties( "XYZ", m_localVarsData[ m_currentDataIdx ].DataType, false );
					break;
					case WirePortDataType.FLOAT4:
					m_outputPorts[ 0 ].ChangeProperties( "XYZW", m_localVarsData[ m_currentDataIdx ].DataType, false );
					break;
					case WirePortDataType.COLOR:
					m_outputPorts[ 0 ].ChangeProperties( "RGBA", m_localVarsData[ m_currentDataIdx ].DataType, false );
					break;
				}

				ConfigurePorts();

				if( !areCompatible )
				{
					m_containerGraph.DeleteConnection( false, UniqueId, 0, false, true );
				}

				m_dataName = m_localVarsData[ m_currentDataIdx ].LocalVarName;
				m_content.text = m_dataName;
				m_sizeIsDirty = true;
				CheckWarningState();
			}
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			if( m_multiPassMode )
			{
				DrawMultipassProperties();
			}

			if( m_currentDataIdx > -1 )
			{
				EditorGUI.BeginChangeCheck();
				m_currentDataIdx = EditorGUILayoutPopup( DataLabelStr, m_currentDataIdx, m_dataLabels );
				if( EditorGUI.EndChangeCheck() )
				{
					UpdateFromId();
				}
			}
		}
		protected override void OnSubShaderChange()
		{
			FetchLocalVarData();
			FetchDataId();
		}

		protected override void OnPassChange()
		{
			base.OnPassChange();
			FetchLocalVarData();
			FetchDataId();
		}

		void DrawMultipassProperties()
		{
			DrawSubShaderUI();
			DrawPassUI();
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			if( m_containerGraph.CurrentCanvasMode != NodeAvailability.TemplateShader )
				return;

			if( m_localVarsData == null || m_localVarsData.Count == 0 )
			{
				MasterNode masterNode = m_containerGraph.CurrentMasterNode;
				if( masterNode.CurrentMasterNodeCategory == AvailableShaderTypes.Template )
				{
					FetchLocalVarData( masterNode );
				}
			}

			if( m_fetchDataId )
			{
				m_fetchDataId = false;
				FetchDataId();
			}

			if( m_currentDataIdx > -1 )
			{
				EditorGUI.BeginChangeCheck();
				m_currentDataIdx = m_upperLeftWidgetHelper.DrawWidget( this, m_currentDataIdx, m_dataLabels );
				if( EditorGUI.EndChangeCheck() )
				{
					UpdateFromId();
				}
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( m_localVarsData[ m_currentDataIdx ].Category != dataCollector.PortCategory )
			{
				UIUtils.ShowMessage( string.Format( "Local Var {0} can only work on ports of type {1}", m_localVarsData[ m_currentDataIdx ].LocalVarName, m_localVarsData[ m_currentDataIdx ].Category ) );
				return m_outputPorts[ 0 ].ErrorValue;
			}

			if( m_multiPassMode )
			{
				if( dataCollector.TemplateDataCollectorInstance.MultipassSubshaderIdx != SubShaderIdx ||
					dataCollector.TemplateDataCollectorInstance.MultipassPassIdx != PassIdx
					)
				{
					UIUtils.ShowMessage( string.Format( "{0} is only intended for subshader {1} and pass {2}", m_dataLabels[ m_currentDataIdx ], SubShaderIdx, PassIdx ) );
					return m_outputPorts[ outputId ].ErrorValue;
				}
			}

			return GetOutputVectorItem( 0, outputId, m_dataName );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_dataName = GetCurrentParam( ref nodeParams );
			m_fetchDataId = true;
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_dataName );
		}

		public override void OnMasterNodeReplaced( MasterNode newMasterNode )
		{
			base.OnMasterNodeReplaced( newMasterNode );
			if( newMasterNode.CurrentMasterNodeCategory == AvailableShaderTypes.Template )
			{
				FetchLocalVarData( newMasterNode );
			}
			else
			{
				m_localVarsData = null;
				m_currentDataIdx = -1;
			}
		}

		void FetchLocalVarData( MasterNode masterNode = null )
		{
			FetchMultiPassTemplate( masterNode );
			if( m_multiPassMode )
			{
				if( m_templateMPData != null )
				{
					m_localVarsData = m_templateMPData.SubShaders[ SubShaderIdx ].Passes[ PassIdx ].LocalVarsList;
					m_fetchDataId = true;
				}
			}
			else
			{
				TemplateData currentTemplate = ( masterNode as TemplateMasterNode ).CurrentTemplate;
				if( currentTemplate != null )
				{
					m_localVarsData = currentTemplate.LocalVarsList;
					m_fetchDataId = true;
				}
				else
				{
					m_localVarsData = null;
					m_currentDataIdx = -1;
				}
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			m_dataLabels = null;
			m_localVarsData = null;
			m_upperLeftWidgetHelper = null;
		}
	}
}
