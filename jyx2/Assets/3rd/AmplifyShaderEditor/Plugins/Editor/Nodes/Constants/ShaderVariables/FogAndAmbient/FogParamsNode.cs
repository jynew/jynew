// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Fog Params", "Light", "Parameters for fog calculation" )]
	public sealed class FogParamsNode : ConstVecShaderVariable
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputName( 1, "Density/Sqrt(Ln(2))" );
			ChangeOutputName( 2, "Density/Ln(2)" );
			ChangeOutputName( 3, "-1/(End-Start)" );
			ChangeOutputName( 4, "End/(End-Start))" );
			m_value = "unity_FogParams";
		}

		public override void RefreshExternalReferences()
		{
			base.RefreshExternalReferences();
			if( !m_outputPorts[ 0 ].IsConnected )
			{
				m_outputPorts[ 0 ].Visible = false;
				m_sizeIsDirty = true;
			}
		}
	}
}
