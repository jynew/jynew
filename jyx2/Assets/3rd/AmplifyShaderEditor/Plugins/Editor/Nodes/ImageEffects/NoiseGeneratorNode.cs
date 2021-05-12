// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
// Based on the work by https://github.com/keijiro/NoiseShader

using System;
using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public enum NoiseGeneratorType
	{
		Simplex2D,
		Simplex3D
	};

	[Serializable]
	[NodeAttributes( "Noise Generator", "Miscellaneous", "Collection of procedural noise generators" )]
	public sealed class NoiseGeneratorNode : ParentNode
	{
		private const string TypeLabelStr = "Type";

		// Simplex 2D
		private const string Simplex2DFloat3Mod289Func = "float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }";
		private const string Simplex2DFloat2Mod289Func = "float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }";
		private const string Simplex2DPermuteFunc = "float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }";

		private const string SimplexNoise2DHeader = "float snoise( float2 v )";
		private const string SimplexNoise2DFunc = "snoise( {0} )";
		private readonly string[] SimplexNoise2DBody = {"float snoise( float2 v )\n",
														"{\n",
														"\tconst float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );\n",
														"\tfloat2 i = floor( v + dot( v, C.yy ) );\n",
														"\tfloat2 x0 = v - i + dot( i, C.xx );\n",
														"\tfloat2 i1;\n",
														"\ti1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );\n",
														"\tfloat4 x12 = x0.xyxy + C.xxzz;\n",
														"\tx12.xy -= i1;\n",
														"\ti = mod2D289( i );\n",
														"\tfloat3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );\n",
														"\tfloat3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );\n",
														"\tm = m * m;\n",
														"\tm = m * m;\n",
														"\tfloat3 x = 2.0 * frac( p * C.www ) - 1.0;\n",
														"\tfloat3 h = abs( x ) - 0.5;\n",
														"\tfloat3 ox = floor( x + 0.5 );\n",
														"\tfloat3 a0 = x - ox;\n",
														"\tm *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );\n",
														"\tfloat3 g;\n",
														"\tg.x = a0.x * x0.x + h.x * x0.y;\n",
														"\tg.yz = a0.yz * x12.xz + h.yz * x12.yw;\n",
														"\treturn 130.0 * dot( m, g );\n",
														"}\n"};
		// Simplex 3D



		private const string Simplex3DFloat3Mod289 = "float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }";
		private const string Simplex3DFloat4Mod289 = "float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }";
		private const string Simplex3DFloat4Permute = "float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }";
		private const string TaylorInvSqrtFunc = "float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }";

		private const string SimplexNoise3DHeader = "float snoise( float3 v )";
		private const string SimplexNoise3DFunc = "snoise( {0} )";
		private readonly string[] SimplexNoise3DBody = {"float snoise( float3 v )\n",
														"{\n",
														"\tconst float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );\n",
														"\tfloat3 i = floor( v + dot( v, C.yyy ) );\n",
														"\tfloat3 x0 = v - i + dot( i, C.xxx );\n",
														"\tfloat3 g = step( x0.yzx, x0.xyz );\n",
														"\tfloat3 l = 1.0 - g;\n",
														"\tfloat3 i1 = min( g.xyz, l.zxy );\n",
														"\tfloat3 i2 = max( g.xyz, l.zxy );\n",
														"\tfloat3 x1 = x0 - i1 + C.xxx;\n",
														"\tfloat3 x2 = x0 - i2 + C.yyy;\n",
														"\tfloat3 x3 = x0 - 0.5;\n",
														"\ti = mod3D289( i);\n",
														"\tfloat4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );\n",
														"\tfloat4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)\n",
														"\tfloat4 x_ = floor( j / 7.0 );\n",
														"\tfloat4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)\n",
														"\tfloat4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;\n",
														"\tfloat4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;\n",
														"\tfloat4 h = 1.0 - abs( x ) - abs( y );\n",
														"\tfloat4 b0 = float4( x.xy, y.xy );\n",
														"\tfloat4 b1 = float4( x.zw, y.zw );\n",
														"\tfloat4 s0 = floor( b0 ) * 2.0 + 1.0;\n",
														"\tfloat4 s1 = floor( b1 ) * 2.0 + 1.0;\n",
														"\tfloat4 sh = -step( h, 0.0 );\n",
														"\tfloat4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;\n",
														"\tfloat4 a1 = b1.xzyw + s1.xzyw * sh.zzww;\n",
														"\tfloat3 g0 = float3( a0.xy, h.x );\n",
														"\tfloat3 g1 = float3( a0.zw, h.y );\n",
														"\tfloat3 g2 = float3( a1.xy, h.z );\n",
														"\tfloat3 g3 = float3( a1.zw, h.w );\n",
														"\tfloat4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );\n",
														"\tg0 *= norm.x;\n",
														"\tg1 *= norm.y;\n",
														"\tg2 *= norm.z;\n",
														"\tg3 *= norm.w;\n",
														"\tfloat4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );\n",
														"\tm = m* m;\n",
														"\tm = m* m;\n",
														"\tfloat4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );\n",
														"\treturn 42.0 * dot( m, px);\n",
														"}\n"};


		[SerializeField]
		private NoiseGeneratorType m_type = NoiseGeneratorType.Simplex2D;

		private UpperLeftWidgetHelper m_upperLeftWidget = new UpperLeftWidgetHelper();

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT2, false, "Size" );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_useInternalPortData = true;
			m_autoWrapProperties = true;
			m_hasLeftDropdown = true;
			SetAdditonalTitleText( string.Format( Constants.SubTitleTypeFormatStr, m_type ) );
			m_previewShaderGUID = "cd2d37ef5da190b42a91a5a690ba2a7d";
		}

		public override void AfterCommonInit()
		{
			base.AfterCommonInit();
			if( PaddingTitleLeft == 0 )
			{
				PaddingTitleLeft = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
				if( PaddingTitleRight == 0 )
					PaddingTitleRight = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			m_upperLeftWidget = null;
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			m_upperLeftWidget.DrawWidget<NoiseGeneratorType>( ref m_type, this, OnWidgetUpdate );
		}

		private readonly Action<ParentNode> OnWidgetUpdate = ( x ) =>
		{
			( x as NoiseGeneratorNode ).ConfigurePorts();
		};

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUI.BeginChangeCheck();
			m_type = (NoiseGeneratorType)EditorGUILayoutEnumPopup( TypeLabelStr, m_type );
			if( EditorGUI.EndChangeCheck() )
			{
				ConfigurePorts();
			}
			//EditorGUILayout.HelpBox( "Node still under construction. Use with caution", MessageType.Info );
		}

		private void ConfigurePorts()
		{
			SetAdditonalTitleText( string.Format( Constants.SubTitleTypeFormatStr, m_type ) );

			switch( m_type )
			{
				case NoiseGeneratorType.Simplex2D:
				{
					m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT2, false );
					m_previewMaterialPassId = 0;
				}
				break;

				case NoiseGeneratorType.Simplex3D:
				{
					m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT3, false );
					m_previewMaterialPassId = 1;
				}
				break;
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
			{
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
			}
			switch( m_type )
			{
				case NoiseGeneratorType.Simplex2D:
				{

					string float3Mod289Func = ( dataCollector.IsTemplate ) ? Simplex2DFloat3Mod289Func : "\t\t" + Simplex2DFloat3Mod289Func;
					dataCollector.AddFunction( Simplex2DFloat3Mod289Func, float3Mod289Func );

					string float2Mod289Func = ( dataCollector.IsTemplate ) ? Simplex2DFloat2Mod289Func : "\t\t" + Simplex2DFloat2Mod289Func;
					dataCollector.AddFunction( Simplex2DFloat2Mod289Func, float2Mod289Func );

					string permuteFunc = ( dataCollector.IsTemplate ) ? Simplex2DPermuteFunc : "\t\t" + Simplex2DPermuteFunc;
					dataCollector.AddFunction( Simplex2DPermuteFunc, permuteFunc );

					dataCollector.AddFunction( SimplexNoise2DHeader, SimplexNoise2DBody, false );


					string size = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
					RegisterLocalVariable( 0, string.Format( SimplexNoise2DFunc, size ), ref dataCollector, ( "simplePerlin2D" + OutputId ) );
				}
				break;
				case NoiseGeneratorType.Simplex3D:
				{

					string float3Mod289Func = ( dataCollector.IsTemplate ) ? Simplex3DFloat3Mod289 : "\t\t" + Simplex3DFloat3Mod289;
					dataCollector.AddFunction( Simplex3DFloat3Mod289, float3Mod289Func );

					string float4Mod289Func = ( dataCollector.IsTemplate ) ? Simplex3DFloat4Mod289 : "\t\t" + Simplex3DFloat4Mod289;
					dataCollector.AddFunction( Simplex3DFloat4Mod289, float4Mod289Func );

					string permuteFunc = ( dataCollector.IsTemplate ) ? Simplex3DFloat4Permute : "\t\t" + Simplex3DFloat4Permute;
					dataCollector.AddFunction( Simplex3DFloat4Permute, permuteFunc );

					string taylorInvSqrtFunc = ( dataCollector.IsTemplate ) ? TaylorInvSqrtFunc : "\t\t" + TaylorInvSqrtFunc;
					dataCollector.AddFunction( TaylorInvSqrtFunc, taylorInvSqrtFunc );

					dataCollector.AddFunction( SimplexNoise3DHeader, SimplexNoise3DBody, false );

					string size = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );

					RegisterLocalVariable( 0, string.Format( SimplexNoise3DFunc, size ), ref dataCollector, ( "simplePerlin3D" + OutputId ) );
				}
				break;
			}
			return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_type = (NoiseGeneratorType)Enum.Parse( typeof( NoiseGeneratorType ), GetCurrentParam( ref nodeParams ) );
			ConfigurePorts();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_type );
		}
	}
}
