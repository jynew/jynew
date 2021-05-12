// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Z-Buffer Params", "Camera And Screen", "Linearized Z buffer values" )]
	public sealed class ZBufferParams : ConstVecShaderVariable
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputName( 1, "1-far/near" );
			ChangeOutputName( 2, "far/near" );
			ChangeOutputName( 3, "[0]/far" );
			ChangeOutputName( 4, "[1]/far" );
			m_value = "_ZBufferParams";
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
