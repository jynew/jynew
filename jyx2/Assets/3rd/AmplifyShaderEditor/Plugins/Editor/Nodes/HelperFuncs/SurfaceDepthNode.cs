using UnityEngine;
using UnityEditor;

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Surface Depth", "Surface Data", "Returns the surface view depth" )]
	public sealed class SurfaceDepthNode : ParentNode
	{
		[SerializeField]
		private int m_viewSpaceInt = 0;

		private readonly string[] m_viewSpaceStr = { "Eye Space", "0-1 Space" };
		private readonly string[] m_vertexNameStr = { "eyeDepth", "clampDepth" };

		private UpperLeftWidgetHelper m_upperLeftWidget = new UpperLeftWidgetHelper();

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "Vertex Position" );
			AddOutputPort( WirePortDataType.FLOAT, "Depth" );
			m_autoWrapProperties = true;
			m_hasLeftDropdown = true;
			SetAdditonalTitleText( string.Format( Constants.SubTitleSpaceFormatStr, m_viewSpaceStr[ m_viewSpaceInt ] ) );
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
			m_viewSpaceInt = m_upperLeftWidget.DrawWidget( this, m_viewSpaceInt, m_viewSpaceStr );
			if( EditorGUI.EndChangeCheck() )
			{
				SetAdditonalTitleText( string.Format( Constants.SubTitleSpaceFormatStr, m_viewSpaceStr[ m_viewSpaceInt ] ) );
			}
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUI.BeginChangeCheck();
			m_viewSpaceInt = EditorGUILayoutPopup( "View Space", m_viewSpaceInt, m_viewSpaceStr );
			if( EditorGUI.EndChangeCheck() )
			{
				SetAdditonalTitleText( string.Format( Constants.SubTitleSpaceFormatStr, m_viewSpaceStr[ m_viewSpaceInt ] ) );
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( dataCollector.IsTemplate )
			{
				if( m_inputPorts[ 0 ].IsConnected )
				{
					string space = string.Empty;
					if( m_viewSpaceInt == 1 )
						space = " * _ProjectionParams.w";

					string varName = "customSurfaceDepth" + OutputId;
					GenerateInputInVertex( ref dataCollector, 0, varName, false );
					string instruction = "-UnityObjectToViewPos( " + varName + " ).z" + space;
					if( dataCollector.IsSRP )
						instruction = "-TransformWorldToView(TransformObjectToWorld( " + varName + " )).z" + space;
					string eyeVarName = "customEye" + OutputId;
					dataCollector.TemplateDataCollectorInstance.RegisterCustomInterpolatedData( eyeVarName, WirePortDataType.FLOAT, m_currentPrecisionType, instruction );
					return eyeVarName;
				}
				else
				{
					return dataCollector.TemplateDataCollectorInstance.GetEyeDepth( m_currentPrecisionType, true, MasterNodePortCategory.Fragment, m_viewSpaceInt );
				}
			}

			if( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				string vertexVarName = string.Empty;
				if( m_inputPorts[ 0 ].IsConnected )
				{
					vertexVarName = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
				}
				else
				{
					vertexVarName = Constants.VertexShaderInputStr + ".vertex.xyz";
				}

				string vertexSpace = m_viewSpaceInt == 1 ? " * _ProjectionParams.w" : "";
				string vertexInstruction = "-UnityObjectToViewPos( " + vertexVarName + " ).z" + vertexSpace;
				dataCollector.AddVertexInstruction( "float " + m_vertexNameStr[ m_viewSpaceInt ] + " = " + vertexInstruction, UniqueId );

				return m_vertexNameStr[ m_viewSpaceInt ];
			}

			dataCollector.AddToIncludes( UniqueId, Constants.UnityShaderVariables );


			if( dataCollector.TesselationActive )
			{
				if( m_inputPorts[ 0 ].IsConnected )
				{
					string space = string.Empty;
					if( m_viewSpaceInt == 1 )
						space = " * _ProjectionParams.w";

					if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
						return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

					string value = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
					RegisterLocalVariable( 0, string.Format( "-UnityObjectToViewPos( {0} ).z", value ) + space, ref dataCollector, "customSurfaceDepth" + OutputId );
					return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
				}
				else
				{
					string eyeDepth = GeneratorUtils.GenerateScreenDepthOnFrag( ref dataCollector, UniqueId, m_currentPrecisionType );
					if( m_viewSpaceInt == 1 )
					{
						dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT, m_vertexNameStr[ 1 ], eyeDepth + " * _ProjectionParams.w" );
						return m_vertexNameStr[ 1 ];
					}
					else
					{
						return eyeDepth;
					}
				}
			}
			else
			{

				string space = string.Empty;
				if( m_viewSpaceInt == 1 )
					space = " * _ProjectionParams.w";

				if( m_inputPorts[ 0 ].IsConnected )
				{
					string varName = "customSurfaceDepth" + OutputId;
					GenerateInputInVertex( ref dataCollector, 0, varName, false );
					dataCollector.AddToInput( UniqueId, varName, WirePortDataType.FLOAT );
					string instruction = "-UnityObjectToViewPos( " + varName + " ).z" + space;
					dataCollector.AddToVertexLocalVariables( UniqueId , Constants.VertexShaderOutputStr + "." + varName + " = " + instruction+";" );
					return Constants.InputVarStr + "." + varName;
				}
				else
				{
					dataCollector.AddToInput( UniqueId, m_vertexNameStr[ m_viewSpaceInt ], WirePortDataType.FLOAT );
					string instruction = "-UnityObjectToViewPos( " + Constants.VertexShaderInputStr + ".vertex.xyz ).z" + space;
					dataCollector.AddToVertexLocalVariables( UniqueId , Constants.VertexShaderOutputStr + "." + m_vertexNameStr[ m_viewSpaceInt ] + " = " + instruction+";" );
					return Constants.InputVarStr + "." + m_vertexNameStr[ m_viewSpaceInt ];
				}
			}
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_viewSpaceInt = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			SetAdditonalTitleText( string.Format( Constants.SubTitleSpaceFormatStr, m_viewSpaceStr[ m_viewSpaceInt ] ) );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_viewSpaceInt );
		}
	}

}
