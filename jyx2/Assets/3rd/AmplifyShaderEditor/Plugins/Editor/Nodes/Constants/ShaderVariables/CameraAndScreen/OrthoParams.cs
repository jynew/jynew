// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Ortho Params", "Camera And Screen", "Orthographic Parameters" )]
	public sealed class OrthoParams : ConstVecShaderVariable
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputName( 1, "Ortho Cam Width" );
			ChangeOutputName( 2, "Ortho Cam Height" );
			ChangeOutputName( 3, "Unused" );
			ChangeOutputName( 4, "Projection Mode" );
			m_value = "unity_OrthoParams";
		}

		public override void RefreshExternalReferences()
		{
			base.RefreshExternalReferences();
			if( !m_outputPorts[ 0 ].IsConnected )
			{
				m_outputPorts[ 0 ].Visible = false;
				m_sizeIsDirty = true;
			}

			if( !m_outputPorts[ 3 ].IsConnected )
			{
				m_outputPorts[ 3 ].Visible = false;
				m_sizeIsDirty = true;
			}
		}
	}
}
