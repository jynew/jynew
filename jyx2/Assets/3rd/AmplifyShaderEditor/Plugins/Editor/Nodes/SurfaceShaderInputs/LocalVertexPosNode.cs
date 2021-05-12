// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "[Deprecated] Local Position", "Surface Data", "Interpolated Vertex Position in Local Space", null, KeyCode.None, true, true, "Vertex Position", typeof( PosVertexDataNode ) )]
	public sealed class LocalVertexPosNode : ParentNode
	{
		private const string VertexVarName = "localVertexPos";
		private readonly string VertexOnFrag = Constants.InputVarStr + "." + VertexVarName;
		private readonly string VertexOnVert = Constants.VertexShaderInputStr + ".vertex";


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
			AddOutputVectorPorts( WirePortDataType.FLOAT3, "XYZ" );
		}
		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				return GetOutputVectorItem( 0, outputId, VertexOnVert );
			}
			else
			{
				if ( m_addInstruction )
				{
					dataCollector.AddToInput( UniqueId, VertexVarName, WirePortDataType.FLOAT3 );
					dataCollector.AddVertexInstruction( Constants.VertexShaderOutputStr + "." + VertexVarName + " = " + Constants.VertexShaderInputStr + ".vertex.xyz ", UniqueId );
					m_addInstruction = false;
				}

				return GetOutputVectorItem( 0, outputId, VertexOnFrag );
			}
		}
	}
}
