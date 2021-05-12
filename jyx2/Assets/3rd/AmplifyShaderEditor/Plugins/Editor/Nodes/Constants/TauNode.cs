// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
//
// Custom Node TAU
// Donated by The Four Headed Cat - @fourheadedcat

using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Tau", "Constants And Properties", "Tau constant (2*PI): 6.28318530718", null, KeyCode.None, true, false, null,null, "The Four Headed Cat - @fourheadedcat" )]
	public sealed class TauNode : ParentNode
	{
		private readonly string Tau = ( 2.0 * Mathf.PI ).ToString();
		public TauNode() : base() { }
		public TauNode( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { }

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_previewShaderGUID = "701bc295c0d75d8429eabcf45e8e008d";
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			return dataCollector.IsSRP? "TWO_PI": Tau;
		}
	}
}
