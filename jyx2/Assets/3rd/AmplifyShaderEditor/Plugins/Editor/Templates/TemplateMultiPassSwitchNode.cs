// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{

	[Serializable]
	public class InputSwitchMPHelper
	{
		public int SubShaderIdx;
		public int PassIdx;
		public InputSwitchMPHelper( int subShaderIdx, int passIdx )
		{
			SubShaderIdx = subShaderIdx;
			PassIdx = passIdx;
		}
	}

	[Serializable]
	[NodeAttributes( "Template Multi-Pass Switch", "Logical Operators", "Relays, in compile time, the correct input port according to current analyzed sub-shader/pass." )]
	public sealed class TemplateMultiPassSwitchNode : TemplateNodeParent
	{
		private const string InputLabelStr = "SubShader {0} Pass {1}";

		[SerializeField]
		private List<InputSwitchMPHelper> m_inputHelper = new List<InputSwitchMPHelper>();

		[SerializeField]
		private int m_inputCountHelper = -1;

		protected override void CommonInit( int uniqueId )
		{
			m_createAllOutputs = false;
			base.CommonInit( uniqueId );
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

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );

			if( m_templateMPData == null )
			{
				FetchMultiPassTemplate();
				if( m_inputPorts.Count != m_inputCountHelper )
				{
					CreateInputPorts();
				}
				else
				{
					RefreshInputPorts();
				}
			}
		}


		public void RefreshInputPorts()
		{
			if( m_multiPassMode )
			{
				m_inputHelper.Clear();
				if( m_templateMPData != null )
				{
					int index = 0;
					int subShaderCount = m_templateMPData.SubShaders.Count;
					for( int subShaderIdx = 0; subShaderIdx < subShaderCount; subShaderIdx++ )
					{
						int passCount = m_templateMPData.SubShaders[ subShaderIdx ].Passes.Count;
						for( int passIdx = 0; passIdx < passCount; passIdx++ )
						{
							if( m_templateMPData.SubShaders[ subShaderIdx ].Passes[ passIdx ].HasValidFunctionBody )
							{
								m_inputPorts[ index ].Name = string.Format( InputLabelStr, subShaderIdx, passIdx );
								m_inputHelper.Add( new InputSwitchMPHelper( subShaderIdx, passIdx ) );
								index += 1;
							}
						}
					}
				}
			}
			else
			{
				m_inputPorts[0].Name = "In";
			}
		}

		public int RefreshInputCountHelper()
		{
			int inputCountHelper = 0;
			if( m_multiPassMode )
			{
				if( m_templateMPData != null )
				{
					int subShaderCount = m_templateMPData.SubShaders.Count;
					for( int subShaderIdx = 0; subShaderIdx < subShaderCount; subShaderIdx++ )
					{
						int passCount = m_templateMPData.SubShaders[ subShaderIdx ].Passes.Count;
						for( int passIdx = 0; passIdx < passCount; passIdx++ )
						{
							if( m_templateMPData.SubShaders[ subShaderIdx ].Passes[passIdx].HasValidFunctionBody )
								inputCountHelper += 1;
						}
					}
				}
			}
			else
			{
				inputCountHelper += 1;
			}
			return inputCountHelper;
		}

		public void CreateInputPorts()
		{
			m_inputCountHelper = 0;
			DeleteAllInputConnections( true );
			if( m_multiPassMode )
			{
				m_inputHelper.Clear();
				if( m_templateMPData != null )
				{
					int subShaderCount = m_templateMPData.SubShaders.Count;
					for( int subShaderIdx = 0; subShaderIdx < subShaderCount; subShaderIdx++ )
					{
						int passCount = m_templateMPData.SubShaders[ subShaderIdx ].Passes.Count;
						for( int passIdx = 0; passIdx < passCount; passIdx++ )
						{
							if( m_templateMPData.SubShaders[ subShaderIdx ].Passes[ passIdx ].HasValidFunctionBody )
							{
								AddInputPort( WirePortDataType.FLOAT, false, string.Format( InputLabelStr, subShaderIdx, passIdx ) );
								m_inputHelper.Add( new InputSwitchMPHelper( subShaderIdx, passIdx ) );
								m_inputCountHelper += 1;
							}
						}
					}
				}
			}
			else
			{
				AddInputPort( WirePortDataType.FLOAT, false, "In" );
				m_inputCountHelper += 1;
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( dataCollector.MasterNodeCategory != AvailableShaderTypes.Template )
			{
				UIUtils.ShowMessage( "Template Multi-Pass Switch Data node is only intended for templates use only" );
				return m_outputPorts[ 0 ].ErrorValue;
			}
			
			int currSubShaderIdx = dataCollector.TemplateDataCollectorInstance.MultipassSubshaderIdx;
			int currPassIdx = dataCollector.TemplateDataCollectorInstance.MultipassPassIdx;

			int inputHelperCount = m_inputHelper.Count;
			for( int i = 0; i< inputHelperCount; i++ )
			{
				if(m_inputHelper[i].SubShaderIdx == currSubShaderIdx && m_inputHelper[ i ].PassIdx == currPassIdx )
					return m_inputPorts[ i ].GeneratePortInstructions( ref dataCollector );
			}
			
			UIUtils.ShowMessage( "Invalid subshader or pass on Template Multi-Pass Switch Data" );
			return m_outputPorts[ 0 ].ErrorValue;
		}

		public override void OnMasterNodeReplaced( MasterNode newMasterNode )
		{
			base.OnMasterNodeReplaced( newMasterNode );
			if( newMasterNode.CurrentMasterNodeCategory == AvailableShaderTypes.Template )
			{
				FetchMultiPassTemplate( newMasterNode );
				m_inputCountHelper = RefreshInputCountHelper();
				if( m_inputPorts.Count != m_inputCountHelper )
				{
					CreateInputPorts();
				}
				else
				{
					RefreshInputPorts();
				}
			}
			else
			{
				DeleteAllInputConnections( true );
			}
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_inputCountHelper = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			// Need to add ports here so read internal data is correct
			for( int i = 0; i < m_inputCountHelper; i++ )
			{
				AddInputPort( WirePortDataType.FLOAT, false, Constants.EmptyPortValue );
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_inputCountHelper );
		}

		public override void Destroy()
		{
			base.Destroy();
			m_inputHelper.Clear();
			m_inputHelper = null;
		}

		public override void RefreshExternalReferences()
		{
			base.RefreshExternalReferences();
			FetchMultiPassTemplate();

			bool create = false;
			if( m_inputCountHelper == -1 )
			{
				create = true;
			}
			else
			{
				int newInputCount = RefreshInputCountHelper();
				if( newInputCount != m_inputCountHelper )
				{
					create = true;
				}
			}

			
			if( m_multiPassMode )
			{
				if( m_templateMPData != null )
				{
					if( create )
					{
						CreateInputPorts();
					}
					else
					{
						m_inputHelper.Clear();
						int index = 0;
						int subShaderCount = m_templateMPData.SubShaders.Count;
						for( int subShaderIdx = 0; subShaderIdx < subShaderCount; subShaderIdx++ )
						{
							int passCount = m_templateMPData.SubShaders[ subShaderIdx ].Passes.Count;
							for( int passIdx = 0; passIdx < passCount; passIdx++ )
							{
								if( m_templateMPData.SubShaders[ subShaderIdx ].Passes[ passIdx ].HasValidFunctionBody )
								{
									m_inputPorts[ index ].Name = string.Format( InputLabelStr, subShaderIdx, passIdx );
									m_inputHelper.Add( new InputSwitchMPHelper( subShaderIdx, passIdx ));
									index += 1;
								}
							}
						}

						if( index != m_inputCountHelper )
						{
							Debug.LogWarning( "Something wrong occured in reading MultiPass Switch node" );
						}
					}
				}
			}
			else
			{
				if( create )
				{
					AddInputPort( WirePortDataType.FLOAT, false, "In" );
				}
				else
				{
					m_inputPorts[ 0 ].Name = "In";
				}	 
			}
		}
	}
}
