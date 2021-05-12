// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Dither", "Camera And Screen", "Generates a dithering pattern" )]
	public sealed class DitheringNode : ParentNode
	{
		private const string InputTypeStr = "Pattern";
		private const string CustomScreenPosStr = "screenPosition";

		private string m_functionHeader = "Dither4x4Bayer( {0}, {1} )";
		private string m_functionBody = string.Empty;

		[SerializeField]
		private int m_selectedPatternInt = 0;

		[SerializeField]
		private bool m_customScreenPos = false;

		private readonly string[] PatternsFuncStr = { "4x4Bayer", "8x8Bayer", "NoiseTex" };
		private readonly string[] PatternsStr = { "4x4 Bayer", "8x8 Bayer", "Noise Texture" };

		private UpperLeftWidgetHelper m_upperLeftWidget = new UpperLeftWidgetHelper();

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, Constants.EmptyPortValue );
			AddInputPort( WirePortDataType.SAMPLER2D, false, "Pattern");
			AddInputPort( WirePortDataType.FLOAT4, false, "Screen Position" );

			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_textLabelWidth = 110;
			m_autoWrapProperties = true;
			m_hasLeftDropdown = true;
			SetAdditonalTitleText( string.Format( Constants.SubTitleTypeFormatStr, PatternsStr[ m_selectedPatternInt ] ) );
			UpdatePorts();
			GeneratePattern();
		}

		public override void Destroy()
		{
			base.Destroy();
			m_upperLeftWidget = null;
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

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			EditorGUI.BeginChangeCheck();
			m_selectedPatternInt = m_upperLeftWidget.DrawWidget( this, m_selectedPatternInt, PatternsStr );
			if( EditorGUI.EndChangeCheck() )
			{
				UpdatePorts();
				GeneratePattern();
			}
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUI.BeginChangeCheck();
			m_selectedPatternInt = EditorGUILayoutPopup( "Pattern", m_selectedPatternInt, PatternsStr );
			if ( EditorGUI.EndChangeCheck() )
			{
				UpdatePorts();
				GeneratePattern();
			}
			EditorGUI.BeginChangeCheck();
			m_customScreenPos = EditorGUILayoutToggle( "Screen Position", m_customScreenPos );
			if( EditorGUI.EndChangeCheck() )
			{
				UpdatePorts();
			}
		}

		private void UpdatePorts()
		{
			m_inputPorts[ 1 ].Visible = ( m_selectedPatternInt == 2 );
			m_inputPorts[ 2 ].Visible = m_customScreenPos;
			m_sizeIsDirty = true;
		}

		private void GeneratePattern()
		{
			SetAdditonalTitleText( string.Format( Constants.SubTitleTypeFormatStr, PatternsStr[ m_selectedPatternInt ] ) );
			switch ( m_selectedPatternInt )
			{
				default:
				case 0:
				{
					m_functionBody = string.Empty;
					m_functionHeader = "Dither" + PatternsFuncStr[ m_selectedPatternInt ] + "( {0}, {1} )";
					IOUtils.AddFunctionHeader( ref m_functionBody, "inline float Dither" + PatternsFuncStr[ m_selectedPatternInt ] + "( int x, int y )" );
					IOUtils.AddFunctionLine( ref m_functionBody, "const float dither[ 16 ] = {" );
					IOUtils.AddFunctionLine( ref m_functionBody, "	 1,  9,  3, 11," );
					IOUtils.AddFunctionLine( ref m_functionBody, "	13,  5, 15,  7," );
					IOUtils.AddFunctionLine( ref m_functionBody, "	 4, 12,  2, 10," );
					IOUtils.AddFunctionLine( ref m_functionBody, "	16,  8, 14,  6 };" );
					IOUtils.AddFunctionLine( ref m_functionBody, "int r = y * 4 + x;" );
					IOUtils.AddFunctionLine( ref m_functionBody, "return dither[r] / 16; // same # of instructions as pre-dividing due to compiler magic" );
					IOUtils.CloseFunctionBody( ref m_functionBody );
				}
				break;
				case 1:
				{
					m_functionBody = string.Empty;
					m_functionHeader = "Dither" + PatternsFuncStr[ m_selectedPatternInt ] + "( {0}, {1} )";
					IOUtils.AddFunctionHeader( ref m_functionBody, "inline float Dither" + PatternsFuncStr[ m_selectedPatternInt ] + "( int x, int y )" );
					IOUtils.AddFunctionLine( ref m_functionBody, "const float dither[ 64 ] = {" );
					IOUtils.AddFunctionLine( ref m_functionBody, "	 1, 49, 13, 61,  4, 52, 16, 64," );
					IOUtils.AddFunctionLine( ref m_functionBody, "	33, 17, 45, 29, 36, 20, 48, 32," );
					IOUtils.AddFunctionLine( ref m_functionBody, "	 9, 57,  5, 53, 12, 60,  8, 56," );
					IOUtils.AddFunctionLine( ref m_functionBody, "	41, 25, 37, 21, 44, 28, 40, 24," );
					IOUtils.AddFunctionLine( ref m_functionBody, "	 3, 51, 15, 63,  2, 50, 14, 62," );
					IOUtils.AddFunctionLine( ref m_functionBody, "	35, 19, 47, 31, 34, 18, 46, 30," );
					IOUtils.AddFunctionLine( ref m_functionBody, "	11, 59,  7, 55, 10, 58,  6, 54," );
					IOUtils.AddFunctionLine( ref m_functionBody, "	43, 27, 39, 23, 42, 26, 38, 22};" );
					IOUtils.AddFunctionLine( ref m_functionBody, "int r = y * 8 + x;" );
					IOUtils.AddFunctionLine( ref m_functionBody, "return dither[r] / 64; // same # of instructions as pre-dividing due to compiler magic" );
					IOUtils.CloseFunctionBody( ref m_functionBody );
				}
				break;
				case 2:
				{
					m_functionBody = string.Empty;
					m_functionHeader = "Dither" + PatternsFuncStr[ m_selectedPatternInt ] + "( {0}, {1}, {2})";
					IOUtils.AddFunctionHeader( ref m_functionBody, "inline float Dither" + PatternsFuncStr[ m_selectedPatternInt ] + "( float4 screenPos, sampler2D noiseTexture, float4 noiseTexelSize )" );
					IOUtils.AddFunctionLine( ref m_functionBody, "float dither = tex2Dlod( noiseTexture, float4( screenPos.xy * _ScreenParams.xy * noiseTexelSize.xy, 0, 0 ) ).g;" );
					IOUtils.AddFunctionLine( ref m_functionBody, "float ditherRate = noiseTexelSize.x * noiseTexelSize.y;" );
					IOUtils.AddFunctionLine( ref m_functionBody, "dither = ( 1 - ditherRate ) * dither + ditherRate;" );
					IOUtils.AddFunctionLine( ref m_functionBody, "return dither;" );
					IOUtils.CloseFunctionBody( ref m_functionBody );
				}
				break;
			}
		}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			base.PropagateNodeData( nodeData, ref dataCollector );
			dataCollector.UsingCustomScreenPos = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			GeneratePattern();

			if( !( dataCollector.IsTemplate && dataCollector.IsSRP ) )
				dataCollector.AddToIncludes( UniqueId, Constants.UnityShaderVariables );
			string varName = string.Empty;
			bool isFragment = dataCollector.IsFragmentCategory;
			if( m_customScreenPos && m_inputPorts[ 2 ].IsConnected )
			{
				varName = "ditherCustomScreenPos" + OutputId;
				string customScreenPosVal = m_inputPorts[ 2 ].GeneratePortInstructions( ref dataCollector );
				dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT4, varName, customScreenPosVal );
			}
			else
			{
				if( dataCollector.TesselationActive && isFragment )
				{
					varName = GeneratorUtils.GenerateClipPositionOnFrag( ref dataCollector, UniqueId, m_currentPrecisionType );
				}
				else
				{
					if( dataCollector.IsTemplate )
					{
						varName = dataCollector.TemplateDataCollectorInstance.GetScreenPosNormalized();
					}
					else
					{
						varName = GeneratorUtils.GenerateScreenPositionNormalized( ref dataCollector, UniqueId, m_currentPrecisionType, !dataCollector.UsingCustomScreenPos );
					}
				}
			}
			string surfInstruction = varName + ".xy * _ScreenParams.xy";
			m_showErrorMessage = false;
			string functionResult = "";
			switch ( m_selectedPatternInt )
			{
				default:
				case 0:
				dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT2, "clipScreen" + OutputId, surfInstruction );
				functionResult = dataCollector.AddFunctions( m_functionHeader, m_functionBody, "fmod(" + "clipScreen" + OutputId + ".x, 4)", "fmod(" + "clipScreen" + OutputId + ".y, 4)" );
				break;
				case 1:
				dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT2, "clipScreen" + OutputId, surfInstruction );
				functionResult = dataCollector.AddFunctions( m_functionHeader, m_functionBody, "fmod(" + "clipScreen" + OutputId + ".x, 8)", "fmod(" + "clipScreen" + OutputId + ".y, 8)" );
				break;
				case 2:
				{
					if( !m_inputPorts[ 1 ].IsConnected )
					{
						m_showErrorMessage = true;
						m_errorMessageTypeIsError = NodeMessageType.Warning;
						m_errorMessageTooltip = "Please connect a texture object to the Pattern input port to generate a proper dithered pattern";
						return "0";
					} else
					{
						string noiseTex = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );
						dataCollector.AddToUniforms( UniqueId, "uniform float4 " + noiseTex + "_TexelSize;" );
						functionResult = dataCollector.AddFunctions( m_functionHeader, m_functionBody, varName, noiseTex, noiseTex+"_TexelSize" );
					}
				}
				break;
			}

			dataCollector.AddLocalVariable( UniqueId, "float dither" + OutputId + " = "+ functionResult+";" );

			if( m_inputPorts[ 0 ].IsConnected )
			{
				string driver = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
				dataCollector.AddLocalVariable( UniqueId, "dither" + OutputId+" = step( dither"+ OutputId + ", "+ driver + " );" );
			}

			//RegisterLocalVariable( 0, functionResult, ref dataCollector, "dither" + OutputId );
			m_outputPorts[ 0 ].SetLocalValue( "dither" + OutputId, dataCollector.PortCategory );

			return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_selectedPatternInt = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			if( UIUtils.CurrentShaderVersion() > 15404 )
			{
				m_customScreenPos = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			}
			UpdatePorts();
			GeneratePattern();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_selectedPatternInt );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_customScreenPos );
		}
	}
}
