// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Simplified Fmod", "Math Operators", "Floating point remainder of x/y" )]
	public sealed class SimplifiedFModOpNode : DynamicTypeNode
	{
		private const string FmodCustomOp = "frac({0}/{1})*{1}";

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_previewShaderGUID = "2688236fb4f37ce47b81cc818c53321d";
		}

		public override string BuildResults( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
			{
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
			}

			base.BuildResults( outputId, ref dataCollector, ignoreLocalvar );
			RegisterLocalVariable( 0, string.Format( FmodCustomOp, m_inputA, m_inputB ), ref dataCollector, ( "fmodResult" + OutputId ) );
			return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
		}
	}
}
