// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Custom Add Node", "Debug", "Custom Node Debug ( Only for debug purposes)", null, UnityEngine.KeyCode.None, false )]
	public sealed class CustomAddNode : CustomNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputsFromString( "customOut0", "#IP2*(#IP0 + #IP1 / #IP2)" );
			AddOutputsFromString( "customOut1", "#IP3 + #IP0*#IP2 + #IP1 / #IP2" );
		}
	}
}
