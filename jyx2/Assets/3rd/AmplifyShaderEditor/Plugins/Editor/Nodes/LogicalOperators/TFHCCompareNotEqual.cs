// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
//
// Custom Node Compare (A != B)
// Donated by The Four Headed Cat - @fourheadedcat

using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
    [Serializable]
	[NodeAttributes("Compare (A \u2260 B)", "Logical Operators", "Check if A is not equal to B. If true return value of True else return value of False", null, KeyCode.None, true, false, null, null, "The Four Headed Cat - @fourheadedcat" )]
    public sealed class TFHCCompareNotEqual : TFHCStub
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_inputPorts[ 0 ].Name = "A";
			m_inputPorts[ 1 ].Name = "B";
			AddInputPort( WirePortDataType.FLOAT, false, "True" );
			AddInputPort( WirePortDataType.FLOAT, false, "False" );
			m_textLabelWidth = 100;
			m_useInternalPortData = true;
			m_previewShaderGUID = "75f433376eef1ad4a881d99124e08008";
		}
		
		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			GetInputData( ref dataCollector, ignoreLocalvar );
			string strout = "(( " + m_inputDataPort0 + " != " + m_inputDataPort1 + " ) ? " + m_inputDataPort2 + " :  " + m_inputDataPort3  + " )";
			return CreateOutputLocalVariable( 0, strout, ref dataCollector );
		}
	}
}
