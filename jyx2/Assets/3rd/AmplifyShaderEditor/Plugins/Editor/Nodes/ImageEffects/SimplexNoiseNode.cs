// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

//https://www.shadertoy.com/view/XsX3zB
using System;
using UnityEngine;
using UnityEditor;


namespace AmplifyShaderEditor
{
	public enum NoiseType
	{
		Simplex3D,
		Simplex3DFractal
	}

	[Serializable]
	[NodeAttributes( "[Deprecated] Simplex Noise", "Image Effects", "Noise generated via the Simplex algorithm",null,KeyCode.None,false,true)]
	public sealed class SimplexNoiseNode : ParentNode
	{
		private string m_randomFuncBody;
		private string m_simplex3dFuncBody;
		private string m_simplex3dFractalFuncBody;

		private const string RandomfunctionHeader = "Random3({0})";
		private const string Simplex3dfunctionHeader = "Simplex3d({0})";
		private const string Simplex3dFractalfunctionHeader = "Simplex3dFractal( {0})";

		private const string NoiseTypeStr = "Type";

		[SerializeField]
		private NoiseType m_type = NoiseType.Simplex3D;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );

			IOUtils.AddFunctionHeader( ref m_randomFuncBody, "float3 Random3 ( float3 c )" );
			IOUtils.AddFunctionLine( ref m_randomFuncBody, "float fracMul = 512.0;float j = 4096.0*sin ( dot ( c, float3 ( 17.0, 59.4, 15.0 ) ) );float3 r;r.z = frac ( fracMul*j );j *= .125;r.x = frac ( fracMul*j );j *= .125;r.y = frac ( fracMul*j );return r - 0.5;" );
			IOUtils.CloseFunctionBody( ref m_randomFuncBody );

			IOUtils.AddFunctionHeader( ref m_simplex3dFuncBody, "float3 Simplex3d ( float3 p )" );
			IOUtils.AddFunctionLine( ref m_simplex3dFuncBody, "float F3 = 0.3333333;float G3 = 0.1666667;float3 s = floor ( p + dot ( p, F3.xxx ) );float3 x = p - s + dot ( s,  G3.xxx );float3 e = step ( ( 0.0 ).xxx, x - x.yzx );float3 i1 = e*( 1.0 - e.zxy );float3 i2 = 1.0 - e.zxy*( 1.0 - e );float3 x1 = x - i1 + G3;float3 x2 = x - i2 + 2.0*G3;float3 x3 = x - 1.0 + 3.0*G3;float4 w, d;w.x = dot ( x, x );w.y = dot ( x1, x1 );w.z = dot ( x2, x2 );w.w = dot ( x3, x3 );w = max ( 0.6 - w, 0.0 );d.x = dot ( Random3 ( s ), x );d.y = dot ( Random3 ( s + i1 ), x1 );d.z = dot ( Random3 ( s + i2 ), x2 );d.w = dot ( Random3 ( s + 1.0 ), x3 );w *= w;w *= w;d *= w;return dot ( d, ( 52.0 ).xxx ).xxx;" );
			IOUtils.CloseFunctionBody( ref m_simplex3dFuncBody );

			IOUtils.AddFunctionHeader( ref m_simplex3dFractalFuncBody, "float3 Simplex3dFractal ( float3 m )" );
			IOUtils.AddFunctionLine( ref m_simplex3dFractalFuncBody, "return (0.5333333*Simplex3d ( m ) + 0.2666667*Simplex3d ( 2.0*m ) + 0.1333333*Simplex3d ( 4.0*m ) + 0.0666667*Simplex3d ( 8.0*m )).xxx;" );
			IOUtils.CloseFunctionBody( ref m_simplex3dFractalFuncBody );

			AddInputPort( WirePortDataType.FLOAT3, false, "Position" );
			AddInputPort( WirePortDataType.FLOAT, false, "Width" );
			AddOutputPort( WirePortDataType.FLOAT3, Constants.EmptyPortValue );
			m_textLabelWidth = 50;
			m_useInternalPortData = true;
			m_autoWrapProperties = true;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			m_type = ( NoiseType ) EditorGUILayoutEnumPopup( NoiseTypeStr, m_type );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			

			string posValue = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string widthValue = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );
			dataCollector.AddFunctions( RandomfunctionHeader, m_randomFuncBody, "0" );
			string result = string.Empty;
			switch ( m_type )
			{
				case NoiseType.Simplex3D:
				{
					string finalValue = dataCollector.AddFunctions( Simplex3dfunctionHeader, m_simplex3dFuncBody, posValue + "*" + widthValue );
					result = finalValue + "* 0.5 + 0.5";
				}break;

				case NoiseType.Simplex3DFractal:
				{
					dataCollector.AddFunctions( Simplex3dfunctionHeader, m_simplex3dFuncBody, posValue + "*" + widthValue );
					string finalValue = dataCollector.AddFunctions( Simplex3dFractalfunctionHeader, m_simplex3dFractalFuncBody, posValue + "*" + widthValue + "+" + widthValue );
					result = finalValue + "* 0.5 + 0.5";
				}break;
			}

			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_type = ( NoiseType ) Enum.Parse( typeof( NoiseType ), GetCurrentParam( ref nodeParams ) );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_type );
		}
	}
}
