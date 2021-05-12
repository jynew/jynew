// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Time", "Time", "Time in seconds with a scale multiplier" )]
	public sealed class SimpleTimeNode : ShaderVariablesNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputProperties( 0, "Out", WirePortDataType.FLOAT );
			AddInputPort( WirePortDataType.FLOAT, false, "Scale" );
			m_inputPorts[ 0 ].FloatInternalData = 1;
			m_useInternalPortData = true;
			m_previewShaderGUID = "45b7107d5d11f124fad92bcb1fa53661";
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			string multiplier = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			if ( multiplier == "1.0" )
				return "_Time.y";

			dataCollector.AddLocalVariable( UniqueId, "float mulTime" + OutputId + " = _Time.y * " + multiplier + ";" );
			return "mulTime" + OutputId;
		}
	}
}
