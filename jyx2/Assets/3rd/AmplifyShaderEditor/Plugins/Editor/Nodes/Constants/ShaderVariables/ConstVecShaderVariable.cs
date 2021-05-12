// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	public class ConstVecShaderVariable : ShaderVariablesNode
	{
		[SerializeField]
		protected string m_value;
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputProperties( 0, " ", WirePortDataType.FLOAT4 );
			AddOutputPort( WirePortDataType.FLOAT, "0" );
			AddOutputPort( WirePortDataType.FLOAT, "1" );
			AddOutputPort( WirePortDataType.FLOAT, "2" );
			AddOutputPort( WirePortDataType.FLOAT, "3" );
		}
		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			switch ( outputId )
			{
				case 0: return m_value;
				case 1: return ( m_value + ".x" );
				case 2: return ( m_value + ".y" );
				case 3: return ( m_value + ".z" );
				case 4: return ( m_value + ".w" );
			}

			UIUtils.ShowMessage( "ConstVecShaderVariable generating empty code", MessageSeverity.Warning );
			return string.Empty;
		}

	}
}
