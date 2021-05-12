// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class TemplateAdditionalPragmasHelper : TemplateAdditionalParentHelper
	{
		public TemplateAdditionalPragmasHelper() : base( "Additional Pragmas" )
		{
			m_helpBoxMessage = "Please add your pragmas without the #pragma keywords";
		}

		public override void AddToDataCollector( ref MasterNodeDataCollector dataCollector, TemplateIncludePragmaContainter nativesContainer )
		{
			for( int i = 0; i < m_additionalItems.Count; i++ )
			{
				if( !string.IsNullOrEmpty( m_additionalItems[ i ] ) && !nativesContainer.HasPragma( m_additionalItems[ i ] ))
					dataCollector.AddToPragmas( -1, m_additionalItems[ i ] );
			}

			for( int i = 0; i < m_outsideItems.Count; i++ )
			{
				if( !string.IsNullOrEmpty( m_outsideItems[ i ] ) && !nativesContainer.HasPragma( m_outsideItems[ i ] ) )
					dataCollector.AddToPragmas( -1, m_outsideItems[ i ] );
			}
		}
	}
}
