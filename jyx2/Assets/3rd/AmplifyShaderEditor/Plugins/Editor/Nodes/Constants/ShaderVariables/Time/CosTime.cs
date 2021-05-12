// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Cos Time", "Time", "Cosine of time" )]
	public sealed class CosTime : ConstVecShaderVariable
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputName( 1, "t/8" );
			ChangeOutputName( 2, "t/4" );
			ChangeOutputName( 3, "t/2" );
			ChangeOutputName( 4, "t" );
			m_value = "_CosTime";
			m_previewShaderGUID = "3093999b42c3c0940a71799511d7781c";
		}

		//public override void AfterPreviewRefresh()
		//{
		//	base.AfterPreviewRefresh();
		//	MarkForPreviewUpdate();
		//}

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
