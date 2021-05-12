// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class ConstantShaderVariable : ShaderVariablesNode
	{
		[SerializeField]
		protected string m_value;

		[SerializeField]
		protected string m_HDValue;

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			if( dataCollector.IsTemplate && dataCollector.TemplateDataCollectorInstance.CurrentSRPType == TemplateSRPType.HD && !string.IsNullOrEmpty( m_HDValue ) )
				return m_HDValue;

			return m_value;
		}
	}
}
