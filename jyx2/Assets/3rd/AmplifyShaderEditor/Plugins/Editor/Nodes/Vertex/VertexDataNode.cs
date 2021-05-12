// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	//public enum VertexData
	//{
	//	vertex = 0,
	//	tangent,
	//	normal,
	//	texcoord,
	//	texcoord1,
	//	color
	//}

	[Serializable]
	public class VertexDataNode : ParentNode
	{
		[SerializeField]
		protected string m_currentVertexData;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentVertexData = "vertex";

//			Type type = typeof( StandardSurfaceOutputNode );
			//m_restictions.AddPortRestriction( type, 0 );
			//m_restictions.AddPortRestriction( type, 2 );
			//m_restictions.AddPortRestriction( type, 3 );
			//m_restictions.AddPortRestriction( type, 4 );
			//m_restictions.AddPortRestriction( type, 5 );
			//m_restictions.AddPortRestriction( type, 6 );
			//m_restictions.AddPortRestriction( type, 7 );
			//m_restictions.AddPortRestriction( type, 8 );
			//m_restictions.AddPortRestriction( type, 9 );
			//m_restictions.AddPortRestriction( type, 10 );
			AddOutputVectorPorts( WirePortDataType.FLOAT4, "Out" );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalVar );
			return GetOutputVectorItem( 0, outputId, Constants.VertexShaderInputStr + "." + m_currentVertexData );
		}
	}
}
