// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Time Parameters", "Time", "Time since level load" )]
	public sealed class TimeNode : ConstVecShaderVariable
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputName( 1, "t/20" );
			ChangeOutputName( 2, "t" );
			ChangeOutputName( 3, "t*2" );
			ChangeOutputName( 4, "t*3" );
			m_value = "_Time";
			m_previewShaderGUID = "73abc10c8d1399444827a7eeb9c24c2a";
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
		//public override void AfterPreviewRefresh()
		//{
		//	base.AfterPreviewRefresh();
		//	MarkForPreviewUpdate();
		//}
	}
}
