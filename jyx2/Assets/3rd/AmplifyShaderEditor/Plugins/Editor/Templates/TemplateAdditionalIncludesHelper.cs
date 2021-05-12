// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class TemplateAdditionalIncludesHelper : TemplateAdditionalParentHelper
	{
		public TemplateAdditionalIncludesHelper() : base( "Additional Includes" )
		{
			m_helpBoxMessage = "Please add your includes without the #include \"\" keywords";
		}	

		public override void AddToDataCollector( ref MasterNodeDataCollector dataCollector , TemplateIncludePragmaContainter nativesContainer )
		{
			for( int i = 0; i < m_additionalItems.Count; i++ )
			{
				if( !string.IsNullOrEmpty( m_additionalItems[ i ] ) && !nativesContainer.HasInclude( m_additionalItems[ i ] ) )
					dataCollector.AddToIncludes( -1, m_additionalItems[ i ] );
			}

			for( int i = 0; i < m_outsideItems.Count; i++ )
			{
				if( !string.IsNullOrEmpty( m_outsideItems[ i ] ) && !nativesContainer.HasInclude( m_outsideItems[ i ] ) )
					dataCollector.AddToIncludes( -1, m_outsideItems[ i ] );
			}
		}
	}
}
