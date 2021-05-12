// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class ShaderVariablesNode : ParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputPort( WirePortDataType.OBJECT, "Out" );
		}
		public override string GetIncludes()
		{
			return Constants.UnityShaderVariables;
		}
		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( !( dataCollector.IsTemplate && dataCollector.TemplateDataCollectorInstance.IsSRP ) )
				dataCollector.AddToIncludes( UniqueId, Constants.UnityShaderVariables );
			return string.Empty;
		}
	}
}
