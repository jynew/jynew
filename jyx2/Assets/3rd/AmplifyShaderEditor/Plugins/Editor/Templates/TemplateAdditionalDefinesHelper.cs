// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class TemplateAdditionalDefinesHelper : TemplateAdditionalParentHelper
	{
		public TemplateAdditionalDefinesHelper() : base( "Additional Defines" )
		{
			m_helpBoxMessage = "Please add your defines without the #define keywords";
		}

		public override void AddToDataCollector( ref MasterNodeDataCollector dataCollector, TemplateIncludePragmaContainter nativesContainer )
		{
			for( int i = 0; i < m_additionalItems.Count; i++ )
			{
				if( !string.IsNullOrEmpty( m_additionalItems[ i ] ) && !nativesContainer.HasDefine( m_additionalItems[ i ] ) )
					dataCollector.AddToDefines( -1, m_additionalItems[ i ] );
			}

			for( int i = 0; i < m_outsideItems.Count; i++ )
			{
				if( !string.IsNullOrEmpty( m_outsideItems[ i ] ) && !nativesContainer.HasDefine( m_outsideItems[ i ] ) )
					dataCollector.AddToDefines( -1, m_outsideItems[ i ] );
			}
		}
	}
}
