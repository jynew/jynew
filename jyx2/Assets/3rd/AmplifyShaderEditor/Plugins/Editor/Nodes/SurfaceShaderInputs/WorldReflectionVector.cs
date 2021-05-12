// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "World Reflection", "Surface Data", "Per pixel world reflection vector, accepts a <b>Normal</b> vector in tangent space (ie: normalmap)" )]
	public sealed class WorldReflectionVector : ParentNode
	{
		private const string ReflectionVecValStr = "newWorldReflection";
		private const string ReflectionVecDecStr = "float3 {0} = {1};";

		private const string NormalizeOptionStr = "Normalize";
		private const string NormalizeFunc = "normalize( {0} )";

		[SerializeField]
		private bool m_normalize = false;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "Normal" );
			AddOutputVectorPorts( WirePortDataType.FLOAT3, "XYZ" );
			m_drawPreviewAsSphere = true;
			m_previewShaderGUID = "8e267e9aa545eeb418585a730f50273e";
			m_autoWrapProperties = true;
			m_textLabelWidth = 80;
			//UIUtils.AddNormalDependentCount();
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if( m_inputPorts[ 0 ].IsConnected )
				m_previewMaterialPassId = 1;
			else
				m_previewMaterialPassId = 0;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			m_normalize = EditorGUILayoutToggle( NormalizeOptionStr, m_normalize );
		}

		//public override void Destroy()
		//{
		//	ContainerGraph.RemoveNormalDependentCount();
		//	base.Destroy();
		//}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			base.PropagateNodeData( nodeData, ref dataCollector );
			if( m_inputPorts[ 0 ].IsConnected )
				dataCollector.DirtyNormal = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if( dataCollector.IsTemplate )
			{
				if( m_inputPorts[ 0 ].IsConnected )
				{
					if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
						return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );


					string value = dataCollector.TemplateDataCollectorInstance.GetWorldReflection( m_currentPrecisionType, m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector ) );
					if( m_normalize )
					{
						value = string.Format( NormalizeFunc, value );
					}
					RegisterLocalVariable( 0, value, ref dataCollector, "worldRefl" + OutputId );
					return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );
				}
				else
				{
					string name;
					string value = dataCollector.TemplateDataCollectorInstance.GetWorldReflection( m_currentPrecisionType );
					if( m_normalize )
					{
						name = "normalizedWorldRefl";
						value = string.Format( NormalizeFunc, value );
						RegisterLocalVariable( 0, value, ref dataCollector, name );
					}
					else
					{
						name = value;
					}
					return GetOutputVectorItem( 0, outputId, name );
				}
			}

			bool isVertex = ( dataCollector.PortCategory == MasterNodePortCategory.Tessellation || dataCollector.PortCategory == MasterNodePortCategory.Vertex );
			if( isVertex )
			{
				if( m_inputPorts[ 0 ].IsConnected )
				{
					if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
						return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );

					string normal = GeneratorUtils.GenerateWorldNormal( ref dataCollector, UniqueId );
					string tangent = GeneratorUtils.GenerateWorldTangent( ref dataCollector, UniqueId );
					dataCollector.AddToVertexLocalVariables( UniqueId, "float3x3 tangentToWorld = CreateTangentToWorldPerVertex( " + normal + ", "+ tangent + ", "+ Constants.VertexShaderInputStr + ".tangent.w );" );
					string inputTangent = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
					dataCollector.AddToVertexLocalVariables( UniqueId, "float3 tangentNormal" + OutputId + " = " + inputTangent + ";" );

					string viewDir = GeneratorUtils.GenerateViewDirection( ref dataCollector, UniqueId );
					dataCollector.AddToVertexLocalVariables( UniqueId, "float3 modWorldNormal" + OutputId + " = ( tangentToWorld[0] * tangentNormal" + OutputId + ".x + tangentToWorld[1] * tangentNormal" + OutputId + ".y + tangentToWorld[2] * tangentNormal" + OutputId + ".z);" );

					string value = "reflect( -" + viewDir + ", modWorldNormal" + OutputId + " )";
					if( m_normalize )
					{
						value = string.Format( NormalizeFunc, value );
					}

					RegisterLocalVariable( 0, value, ref dataCollector, "modReflection" + OutputId );
					return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );
				}
				else
				{
					if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
						return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );

					string worldNormal = GeneratorUtils.GenerateWorldNormal( ref dataCollector, UniqueId );
					string viewDir = GeneratorUtils.GenerateViewDirection( ref dataCollector, UniqueId );

					string value = "reflect( -" + viewDir + ", " + worldNormal + " )";
					if( m_normalize )
					{
						value = string.Format( NormalizeFunc, value );
					}
					RegisterLocalVariable( 0, value, ref dataCollector, ReflectionVecValStr + OutputId );
					return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );
				}
			}
			else
			{
				if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
					return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );
				
				dataCollector.AddToInput( UniqueId, SurfaceInputs.WORLD_REFL, m_currentPrecisionType );

				string result = string.Empty;
				if( m_inputPorts[ 0 ].IsConnected )
				{
					dataCollector.AddToInput( UniqueId, SurfaceInputs.INTERNALDATA, addSemiColon: false );
					dataCollector.ForceNormal = true;

					result = "WorldReflectionVector( " + Constants.InputVarStr + " , " + m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector ) + " )";
					if( m_normalize )
					{
						result = String.Format( NormalizeFunc, result );
					}
					int connCount = 0;
					for( int i = 0; i < m_outputPorts.Count; i++ )
					{
						connCount += m_outputPorts[ i ].ConnectionCount;
					}

					if( connCount > 1 )
					{
						dataCollector.AddToLocalVariables( UniqueId, string.Format( ReflectionVecDecStr, ReflectionVecValStr + OutputId, result ) );
						RegisterLocalVariable( 0, result, ref dataCollector, ReflectionVecValStr + OutputId );
						return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );
					}
				}
				else
				{
					dataCollector.AddToInput( UniqueId, SurfaceInputs.INTERNALDATA, addSemiColon: false );
					result = GeneratorUtils.GenerateWorldReflection( ref dataCollector, UniqueId , m_normalize );
					if( dataCollector.DirtyNormal )
						dataCollector.ForceNormal = true;
				}

				return GetOutputVectorItem( 0, outputId, result );
				//RegisterLocalVariable( 0, result, ref dataCollector, "worldrefVec" + OutputId );
				//return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );
			}
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if( UIUtils.CurrentShaderVersion() > 14202 )
			{
				m_normalize = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_normalize );
		}

		public override void RefreshExternalReferences()
		{
			base.RefreshExternalReferences();
			if( UIUtils.CurrentShaderVersion() <= 14202 )
			{
				if( !m_inputPorts[ 0 ].IsConnected )
				{
					m_normalize = true;
				}
			}
		}
	}
}
