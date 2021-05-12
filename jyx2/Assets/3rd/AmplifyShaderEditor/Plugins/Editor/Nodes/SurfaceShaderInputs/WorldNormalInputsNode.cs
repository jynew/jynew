// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "[Deprecated] World Normal", "Surface Data", "Vertex Normal World", null, KeyCode.None, true, true, "World Normal", typeof( WorldNormalVector ) )]
	public sealed class WorldNormalInputsNode : SurfaceShaderINParentNode
	{
		private const string PerPixelLabelStr = "Per Pixel";

		[SerializeField]
		private bool m_perPixel = true;

		[SerializeField]
		private string m_precisionString;

		[SerializeField]
		private bool m_addInstruction = false;

		public override void Reset()
		{
			base.Reset();
			m_addInstruction = true;
		}

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentInput = SurfaceInputs.WORLD_NORMAL;
			InitialSetup();
			//UIUtils.AddNormalDependentCount();
			m_precisionString = UIUtils.PrecisionWirePortToCgType( m_currentPrecisionType, WirePortDataType.FLOAT3 );
		}

		//public override void Destroy()
		//{
		//	ContainerGraph.RemoveNormalDependentCount();
		//	base.Destroy();
		//}

		public override void DrawProperties()
		{
			base.DrawProperties();
			m_perPixel = EditorGUILayoutToggleLeft( PerPixelLabelStr, m_perPixel );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				if ( m_addInstruction )
				{
					string precision = UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, WirePortDataType.FLOAT3 );
					dataCollector.AddVertexInstruction( precision + " worldNormal = UnityObjectToWorldNormal(" + Constants.VertexShaderInputStr + ".normal)", UniqueId );
					m_addInstruction = false;
				}

				return GetOutputVectorItem( 0, outputId, "worldNormal" );
			}
			else
			{
				dataCollector.AddToInput( UniqueId, SurfaceInputs.WORLD_NORMAL, m_currentPrecisionType );
				dataCollector.AddToInput( UniqueId, SurfaceInputs.INTERNALDATA, addSemiColon: false );
				if ( dataCollector.PortCategory != MasterNodePortCategory.Debug && m_perPixel && dataCollector.DirtyNormal )
				{
					//string result = "WorldNormalVector( " + Constants.InputVarStr + " , float3( 0,0,1 ))";
					m_precisionString = UIUtils.PrecisionWirePortToCgType( m_currentPrecisionType, WirePortDataType.FLOAT3 );
					string result = string.Format( Constants.WorldNormalLocalDecStr, m_precisionString );
					int count = 0;
					for ( int i = 0; i < m_outputPorts.Count; i++ )
					{
						if ( m_outputPorts[ i ].IsConnected )
						{
							if ( m_outputPorts[ i ].ConnectionCount > 2 )
							{
								count = 2;
								break;
							}
							count += 1;
							if ( count > 1 )
								break;
						}
					}
					if ( count > 1 )
					{
						string localVarName = "WorldNormal" + OutputId;
						dataCollector.AddToLocalVariables( UniqueId, m_currentPrecisionType, m_outputPorts[ 0 ].DataType, localVarName, result );
						return GetOutputVectorItem( 0, outputId, localVarName );
					}
					else
					{
						return GetOutputVectorItem( 0, outputId, result );
					}
				}
				else
				{
					return base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalVar );
				}
			}
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if ( UIUtils.CurrentShaderVersion() > 2504 )
				m_perPixel = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_perPixel );
		}
	}
}
