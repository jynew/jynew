// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
//
// Custom Node HeightMap Texture Masking
// Donated by Rea

using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "HeightMap Texture Blend", "Textures", "Advanced Texture Blending by using heightMap and splatMask, usefull for texture layering ", null, KeyCode.None, true, false, null, null, "Rea" )]
	public sealed class HeightMapBlendNode : ParentNode
	{

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, "HeightMap" );
			AddInputPort( WirePortDataType.FLOAT, false, "SplatMask" );
			AddInputPort( WirePortDataType.FLOAT, false, "BlendStrength" );
			AddOutputVectorPorts( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_textLabelWidth = 120;
			m_useInternalPortData = true;
			m_inputPorts[ 2 ].FloatInternalData = 1;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			string HeightMap = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string SplatMask = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector);
			string Blend = m_inputPorts[ 2 ].GeneratePortInstructions( ref dataCollector );

			string HeightMask =  "saturate(pow(((" + HeightMap + "*" + SplatMask + ")*4)+(" + SplatMask + "*2)," + Blend + "))";
			string varName = "HeightMask" + OutputId;

			RegisterLocalVariable( 0, HeightMask, ref dataCollector , varName );
			return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
		}
		/*
         A = (heightMap * SplatMask)*4
         B = SplatMask*2
         C = pow(A+B,Blend)
         saturate(C)
         saturate(pow(((heightMap * SplatMask)*4)+(SplatMask*2),Blend));
         */
	}
}
