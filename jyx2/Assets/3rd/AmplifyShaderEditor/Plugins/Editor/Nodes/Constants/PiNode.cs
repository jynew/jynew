// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "PI", "Constants And Properties", "PI constant : 3.14159265359" )]
	public sealed class PiNode : ParentNode
	{
		public PiNode() : base() { }
		public PiNode( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { }
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, true, "Multiplier" );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_textLabelWidth = 70;
			InputPorts[ 0 ].FloatInternalData = 1;
			m_useInternalPortData = true;
			m_previewShaderGUID = "bf4a65726dab3d445a69fb1d0945c33e";
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			string finalValue = string.Empty;
			string piString = dataCollector.IsSRP ? "PI" : "UNITY_PI";
			if( !InputPorts[ 0 ].IsConnected && InputPorts[ 0 ].FloatInternalData == 1 )
			{
				finalValue = piString;
			} else
			{
				string multiplier = InputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
				finalValue = "( " + multiplier + " * " + piString + " )";
			}


			if ( finalValue.Equals( string.Empty ) )
			{
				UIUtils.ShowMessage( "PINode generating empty code", MessageSeverity.Warning );
			}
			return finalValue;
		}

		//public override void ReadFromString( ref string[] nodeParams )
		//{
		//	base.ReadFromString( ref nodeParams );

			// Removed on version 5004
			//m_value = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
		//}

		//public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		//{
		//	base.WriteToString( ref nodeInfo, ref connectionsInfo );
		//}

	}
}
