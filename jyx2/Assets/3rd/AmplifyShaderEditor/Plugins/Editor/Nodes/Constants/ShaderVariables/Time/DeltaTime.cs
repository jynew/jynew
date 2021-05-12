// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Delta Time", "Time", "Delta time" )]
	public sealed class DeltaTime : ConstVecShaderVariable
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputName( 1, "dt" );
			ChangeOutputName( 2, "1/dt" );
			ChangeOutputName( 3, "smoothDt" );
			ChangeOutputName( 4, "1/smoothDt" );
			m_value = "unity_DeltaTime";
			m_previewShaderGUID = "9d69a693042c443498f96d6da60535eb";
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
