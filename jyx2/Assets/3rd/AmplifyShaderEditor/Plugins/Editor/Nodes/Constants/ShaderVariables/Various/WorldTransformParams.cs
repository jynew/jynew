// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "World Transform Params", "Object Transform", "World Transform Params contains information about the transform, W is usually 1.0, or -1.0 for odd-negative scale transforms" )]
	public sealed class WorldTransformParams : ConstVecShaderVariable
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputName( 1, "X" );
			ChangeOutputName( 2, "Y" );
			ChangeOutputName( 3, "Z" );
			ChangeOutputName( 4, "W" );
			m_value = "unity_WorldTransformParams";
			m_previewShaderGUID = "5a2642605f085da458d6e03ade47b87a";
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
	}
}
