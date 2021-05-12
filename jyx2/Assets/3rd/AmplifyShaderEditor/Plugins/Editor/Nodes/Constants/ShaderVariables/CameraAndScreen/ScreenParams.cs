// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Screen Params", "Camera And Screen", "Camera's Render Target size parameters" )]
	public sealed class ScreenParams : ConstVecShaderVariable
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputName( 1, "RT Width" );
			ChangeOutputName( 2, "RT Height" );
			ChangeOutputName( 3, "1+1/Width" );
			ChangeOutputName( 4, "1+1/Height" );
			m_value = "_ScreenParams";
		}

		//public override void RefreshExternalReferences()
		//{
		//	base.RefreshExternalReferences();
		//	if( !m_outputPorts[ 0 ].IsConnected )
		//	{
		//		m_outputPorts[ 0 ].Visible = false;
		//		m_sizeIsDirty = true;
		//	}
		//}
	}
}
