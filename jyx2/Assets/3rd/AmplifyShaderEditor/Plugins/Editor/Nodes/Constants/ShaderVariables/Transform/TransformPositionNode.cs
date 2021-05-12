// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Transform Position", "Object Transform", "Transforms a position value from one space to another" )]
	public sealed class TransformPositionNode : ParentNode
	{
		[SerializeField]
		private TransformSpace m_from = TransformSpace.Object;

		[SerializeField]
		private TransformSpace m_to = TransformSpace.World;

		[SerializeField]
		private bool m_perspectiveDivide = false;

		[SerializeField]
		private InverseTangentType m_inverseTangentType = InverseTangentType.Fast;

		private string InverseTBNStr = "Inverse TBN";

		private const string AseObjectToWorldPosVarName = "objToWorld";
		private const string AseObjectToWorldPosFormat = "mul( unity_ObjectToWorld, float4( {0}, 1 ) ).xyz";
		private const string AseHDObjectToWorldPosFormat = "mul( GetObjectToWorldMatrix(), float4( {0}, 1 ) ).xyz";

		private const string AseObjectToViewPosVarName = "objToView";
		private const string AseObjectToViewPosFormat = "mul( UNITY_MATRIX_MV, float4( {0}, 1 ) ).xyz";
		private const string AseHDObjectToViewPosFormat = "TransformWorldToView( TransformObjectToWorld({0}) )";

		private const string AseWorldToObjectPosVarName = "worldToObj";
		private const string AseWorldToObjectPosFormat = "mul( unity_WorldToObject, float4( {0}, 1 ) ).xyz";
		private const string AseHDWorldToObjectPosFormat = "mul( GetWorldToObjectMatrix(), float4( {0}, 1 ) ).xyz";


		private const string AseWorldToViewPosVarName = "worldToView";
		private const string AseWorldToViewPosFormat = "mul( UNITY_MATRIX_V, float4( {0}, 1 ) ).xyz";

		private const string AseViewToObjectPosVarName = "viewToObj";
		private const string AseViewToObjectPosFormat = "mul( unity_WorldToObject, mul( UNITY_MATRIX_I_V , float4( {0}, 1 ) ) ).xyz";
		private const string AseHDViewToObjectPosFormat = "mul( GetWorldToObjectMatrix(), mul( UNITY_MATRIX_I_V , float4( {0}, 1 ) ) ).xyz";

		private const string AseViewToWorldPosVarName = "viewToWorld";
		private const string AseViewToWorldPosFormat = "mul( UNITY_MATRIX_I_V, float4( {0}, 1 ) ).xyz";

		///////////////////////////////////////////////////////////
		private const string AseObjectToClipPosVarName = "objectToClip";
		private const string AseObjectToClipPosFormat = "UnityObjectToClipPos({0})";
		private const string AseSRPObjectToClipPosFormat = "TransformWorldToHClip(TransformObjectToWorld({0}))";

		private const string AseWorldToClipPosVarName = "worldToClip";
		private const string AseWorldToClipPosFormat = "mul(UNITY_MATRIX_VP, float4({0}, 1.0))";
		private const string AseSRPWorldToClipPosFormat = "TransformWorldToHClip({0})";

		private const string AseViewToClipPosVarName = "viewToClip";
		private const string AseViewToClipPosFormat = "mul(UNITY_MATRIX_P, float4({0}, 1.0))";
		private const string AseSRPViewToClipPosFormat = "TransformWViewToHClip({0})";
		//
		private const string AseClipToObjectPosVarName = "clipToObject";
		private const string AseClipToObjectPosFormat = "mul( UNITY_MATRIX_IT_MV, mul( unity_CameraInvProjection,float4({0},1)) ).xyz";
		private const string AseHDClipToObjectPosFormat = "mul( UNITY_MATRIX_I_M, mul( UNITY_MATRIX_I_VP,float4({0},1)) ).xyz";

		private const string AseClipToWorldPosVarName = "clipToWorld";
		private const string AseClipToWorldPosFormat = "mul( UNITY_MATRIX_I_V, mul( unity_CameraInvProjection,float4({0},1)) ).xyz";
		private const string AseHDClipToWorldPosFormat = "mul( UNITY_MATRIX_I_VP, float4({0},1) ).xyz";

		private const string AseClipToViewPosVarName = "clipToView";
		private const string AseClipToViewPosFormat = " mul( unity_CameraInvProjection,float4({0},1)).xyz";
		private const string AseHDClipToViewPosFormat = " mul( UNITY_MATRIX_I_P,float4({0},1)).xyz";
		private const string AseClipToNDC = "{0}.xyz/{0}.w";
		/////////////////////////////////////////////////////
		private const string AseObjectToTangentPosVarName = "objectToTangentPos";
		private const string AseWorldToTangentPosVarName = "worldToTangentPos";
		private const string AseViewToTangentPosVarName = "viewToTangentPos";
		private const string AseClipToTangentPosVarName = "clipToTangentPos";
		private const string ASEWorldToTangentFormat = "mul( ase_worldToTangent, {0})";


		private const string AseTangentToObjectPosVarName = "tangentTobjectPos";
		private const string AseTangentToWorldPosVarName = "tangentToWorldPos";
		private const string AseTangentToViewPosVarName = "tangentToViewPos";
		private const string AseTangentToClipPosVarName = "tangentToClipPos";
		private const string ASEMulOpFormat = "mul( {0}, {1} )";

		///////////////////////////////////////////////////////////
		private const string FromStr = "From";
		private const string ToStr = "To";
		private const string PerpectiveDivideStr = "Perpective Divide";
		private const string SubtitleFormat = "{0} to {1}";

		private readonly string[] m_spaceOptions =
		{
			"Object Space",
			"World Space",
			"View Space",
			"Clip",
			"Tangent"
		};

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, Constants.EmptyPortValue );
			AddOutputVectorPorts( WirePortDataType.FLOAT3, "XYZ" );
			m_useInternalPortData = true;
			m_autoWrapProperties = true;
			m_previewShaderGUID = "74e4d859fbdb2c0468de3612145f4929";
			m_textLabelWidth = 120;
			UpdateSubtitle();
		}

		private void UpdateSubtitle()
		{
			SetAdditonalTitleText( string.Format( SubtitleFormat, m_from, m_to ) );
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUI.BeginChangeCheck();
			m_from = (TransformSpace)EditorGUILayoutPopup( FromStr, (int)m_from, m_spaceOptions );
			m_to = (TransformSpace)EditorGUILayoutPopup( ToStr, (int)m_to, m_spaceOptions );
			if( m_from == TransformSpace.Tangent )
			{
				m_inverseTangentType = (InverseTangentType)EditorGUILayoutEnumPopup( InverseTBNStr, m_inverseTangentType );
			}
			if( EditorGUI.EndChangeCheck() )
			{
				UpdateSubtitle();
			}

			if( m_to == TransformSpace.Clip )
			{
				m_perspectiveDivide = EditorGUILayoutToggle( PerpectiveDivideStr, m_perspectiveDivide );
			}
		}

		void CalculateTransform( TransformSpace from, TransformSpace to, ref MasterNodeDataCollector dataCollector, ref string varName, ref string result )
		{
			switch( from )
			{
				case TransformSpace.Object:
				{
					switch( to )
					{
						default:
						case TransformSpace.Object: break;
						case TransformSpace.World:
						{
							if( dataCollector.IsTemplate && dataCollector.TemplateDataCollectorInstance.CurrentSRPType == TemplateSRPType.HD )
								result = string.Format( AseHDObjectToWorldPosFormat, result );
							else
								result = string.Format( AseObjectToWorldPosFormat, result );
							varName = AseObjectToWorldPosVarName + OutputId;
						}
						break;
						case TransformSpace.View:
						{
							if( dataCollector.IsTemplate && dataCollector.TemplateDataCollectorInstance.CurrentSRPType == TemplateSRPType.HD )
								result = string.Format( AseHDObjectToViewPosFormat, result );
							else
								result = string.Format( AseObjectToViewPosFormat, result );
							varName = AseObjectToViewPosVarName + OutputId;
						}
						break;
						case TransformSpace.Clip:
						{
							if( dataCollector.IsTemplate && dataCollector.TemplateDataCollectorInstance.CurrentSRPType != TemplateSRPType.BuiltIn )
							{
								result = string.Format( AseSRPObjectToClipPosFormat, result );
							}
							else
							{
								result = string.Format( AseObjectToClipPosFormat, result );
							}
							varName = AseObjectToClipPosVarName + OutputId;
						}
						break;
					}
				}
				break;
				case TransformSpace.World:
				{
					switch( to )
					{
						case TransformSpace.Object:
						{
							if( dataCollector.IsTemplate && dataCollector.TemplateDataCollectorInstance.CurrentSRPType == TemplateSRPType.HD )
								result = string.Format( AseHDWorldToObjectPosFormat, result );
							else
								result = string.Format( AseWorldToObjectPosFormat, result );
							varName = AseWorldToObjectPosVarName + OutputId;
						}
						break;
						default:
						case TransformSpace.World: break;
						case TransformSpace.View:
						{
							result = string.Format( AseWorldToViewPosFormat, result );
							varName = AseWorldToViewPosVarName + OutputId;
						}
						break;
						case TransformSpace.Clip:
						{
							if( dataCollector.IsTemplate && dataCollector.TemplateDataCollectorInstance.CurrentSRPType != TemplateSRPType.BuiltIn )
							{
								result = string.Format( AseSRPWorldToClipPosFormat, result );
							}
							else
							{
								result = string.Format( AseWorldToClipPosFormat, result );
							}
							varName = AseWorldToClipPosVarName + OutputId;
						}
						break;
					}
				}
				break;
				case TransformSpace.View:
				{
					switch( to )
					{
						case TransformSpace.Object:
						{
							if( dataCollector.IsTemplate && dataCollector.TemplateDataCollectorInstance.CurrentSRPType == TemplateSRPType.HD )
								result = string.Format( AseHDViewToObjectPosFormat, result );
							else
								result = string.Format( AseViewToObjectPosFormat, result );
							varName = AseViewToObjectPosVarName + OutputId;
						}
						break;
						case TransformSpace.World:
						{
							result = string.Format( AseViewToWorldPosFormat, result );
							varName = AseViewToWorldPosVarName + OutputId;
						}
						break;
						default:
						case TransformSpace.View: break;
						case TransformSpace.Clip:
						{
							if( dataCollector.IsTemplate && dataCollector.TemplateDataCollectorInstance.CurrentSRPType != TemplateSRPType.BuiltIn )
							{
								result = string.Format( AseSRPViewToClipPosFormat, result );
							}
							else
							{
								result = string.Format( AseViewToClipPosFormat, result );
							}
							varName = AseViewToClipPosVarName + OutputId;
						}
						break;
					}
				}
				break;
				case TransformSpace.Clip:
				{
					switch( to )
					{
						case TransformSpace.Object:
						{
							if( dataCollector.IsTemplate && dataCollector.TemplateDataCollectorInstance.CurrentSRPType == TemplateSRPType.HD )
							{
								result = string.Format( AseHDClipToObjectPosFormat, result );
							}
							else
							{
								result = string.Format( AseClipToObjectPosFormat, result );
							}
							varName = AseClipToObjectPosVarName + OutputId;
						}
						break;
						case TransformSpace.World:
						{
							if( dataCollector.IsTemplate && dataCollector.TemplateDataCollectorInstance.CurrentSRPType == TemplateSRPType.HD )
							{
								result = string.Format( AseHDClipToWorldPosFormat, result );
							}
							else
							{
								result = string.Format( AseClipToWorldPosFormat, result );
							}
							varName = AseClipToWorldPosVarName + OutputId;
						}
						break;
						case TransformSpace.View:
						{
							if( dataCollector.IsTemplate && dataCollector.TemplateDataCollectorInstance.CurrentSRPType == TemplateSRPType.HD )
							{
								result = string.Format( AseHDClipToViewPosFormat, result );
							}
							else
							{
								result = string.Format( AseClipToViewPosFormat, result );
							}
							varName = AseClipToViewPosVarName + OutputId;
						}
						break;
						case TransformSpace.Clip: break;
						default:
						break;
					}
				}
				break;
				default: break;
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );

			string result = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string varName = string.Empty;

			switch( m_from )
			{
				case TransformSpace.Object:
				{
					switch( m_to )
					{
						default:
						case TransformSpace.Object: break;
						case TransformSpace.World:
						{
							CalculateTransform( m_from, m_to, ref dataCollector, ref varName, ref result );
						}
						break;
						case TransformSpace.View:
						{
							CalculateTransform( m_from, m_to, ref dataCollector, ref varName, ref result );
						}
						break;
						case TransformSpace.Clip:
						{
							CalculateTransform( m_from, m_to, ref dataCollector, ref varName, ref result );
						}
						break;
						case TransformSpace.Tangent:
						{
							GeneratorUtils.GenerateWorldToTangentMatrix( ref dataCollector, UniqueId, m_currentPrecisionType );
							CalculateTransform( m_from, TransformSpace.World, ref dataCollector, ref varName, ref result );
							result = string.Format( ASEWorldToTangentFormat, result );
							varName = AseObjectToTangentPosVarName;
						}
						break;
					}
				}
				break;
				case TransformSpace.World:
				{
					switch( m_to )
					{
						case TransformSpace.Object:
						{
							CalculateTransform( m_from, m_to, ref dataCollector, ref varName, ref result );
						}
						break;
						default:
						case TransformSpace.World: break;
						case TransformSpace.View:
						{
							CalculateTransform( m_from, m_to, ref dataCollector, ref varName, ref result );
						}
						break;
						case TransformSpace.Clip:
						{
							CalculateTransform( m_from, m_to, ref dataCollector, ref varName, ref result );
						}
						break;
						case TransformSpace.Tangent:
						{
							GeneratorUtils.GenerateWorldToTangentMatrix( ref dataCollector, UniqueId, m_currentPrecisionType );
							result = string.Format( ASEWorldToTangentFormat, result );
							varName = AseWorldToTangentPosVarName;
						}
						break;
					}
				}
				break;
				case TransformSpace.View:
				{
					switch( m_to )
					{
						case TransformSpace.Object:
						{
							CalculateTransform( m_from, m_to, ref dataCollector, ref varName, ref result );
						}
						break;
						case TransformSpace.World:
						{
							CalculateTransform( m_from, m_to, ref dataCollector, ref varName, ref result ); ;
						}
						break;
						default:
						case TransformSpace.View: break;
						case TransformSpace.Clip:
						{
							CalculateTransform( m_from, m_to, ref dataCollector, ref varName, ref result );
						}
						break;
						case TransformSpace.Tangent:
						{
							GeneratorUtils.GenerateWorldToTangentMatrix( ref dataCollector, UniqueId, m_currentPrecisionType );
							CalculateTransform( m_from, TransformSpace.World, ref dataCollector, ref varName, ref result );
							result = string.Format( ASEWorldToTangentFormat, result );
							varName = AseViewToTangentPosVarName;
						}
						break;
					}
				}
				break;
				case TransformSpace.Clip:
				{
					switch( m_to )
					{
						case TransformSpace.Object:
						{
							CalculateTransform( m_from, m_to, ref dataCollector, ref varName, ref result );
						}
						break;
						case TransformSpace.World:
						{
							CalculateTransform( m_from, m_to, ref dataCollector, ref varName, ref result );
						}
						break;
						case TransformSpace.View:
						{
							CalculateTransform( m_from, m_to, ref dataCollector, ref varName, ref result );
						}
						break;
						case TransformSpace.Clip: break;
						case TransformSpace.Tangent:
						{
							GeneratorUtils.GenerateWorldToTangentMatrix( ref dataCollector, UniqueId, m_currentPrecisionType );
							CalculateTransform( m_from, TransformSpace.World, ref dataCollector, ref varName, ref result );
							result = string.Format( ASEWorldToTangentFormat, result );
							varName = AseClipToTangentPosVarName;
						}
						break;
						default:
						break;
					}
				}
				break;
				case TransformSpace.Tangent:
				{
					string matrixVal = string.Empty;
					if( m_inverseTangentType == InverseTangentType.Fast )
						matrixVal = GeneratorUtils.GenerateTangentToWorldMatrixFast( ref dataCollector, UniqueId, m_currentPrecisionType );
					else
						matrixVal = GeneratorUtils.GenerateTangentToWorldMatrixPrecise( ref dataCollector, UniqueId, m_currentPrecisionType );

					switch( m_to )
					{
						case TransformSpace.Object:
						{
							result = string.Format( ASEMulOpFormat, matrixVal, result );
							CalculateTransform( TransformSpace.World, m_to, ref dataCollector, ref varName, ref result );
							varName = AseTangentToObjectPosVarName;
						}
						break;
						case TransformSpace.World:
						{
							result = string.Format( ASEMulOpFormat, matrixVal, result );
							varName = AseTangentToWorldPosVarName;
						}
						break;
						case TransformSpace.View:
						{
							result = string.Format( ASEMulOpFormat, matrixVal, result );
							CalculateTransform( TransformSpace.World, m_to, ref dataCollector, ref varName, ref result );
							varName = AseTangentToViewPosVarName;
						}
						break;
						case TransformSpace.Clip:
						{
							result = string.Format( ASEMulOpFormat, matrixVal, result );
							CalculateTransform( TransformSpace.World, m_to, ref dataCollector, ref varName, ref result );
							varName = AseTangentToClipPosVarName;
						}
						break;
						case TransformSpace.Tangent:
						default:
						break;
					}
				}
				break;
				default: break;
			}

			if( m_to == TransformSpace.Clip )
			{
				if( m_perspectiveDivide )
				{
					dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT4, varName, result );
					result = string.Format( AseClipToNDC, varName );
					varName += "NDC";
				}
				else
				{
					result += ".xyz";
				}
			}

			RegisterLocalVariable( 0, result, ref dataCollector, varName );
			return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_from = (TransformSpace)Enum.Parse( typeof( TransformSpace ), GetCurrentParam( ref nodeParams ) );
			m_to = (TransformSpace)Enum.Parse( typeof( TransformSpace ), GetCurrentParam( ref nodeParams ) );
			if( UIUtils.CurrentShaderVersion() > 15701 )
			{
				m_perspectiveDivide = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			}
			if( UIUtils.CurrentShaderVersion() > 15800 )
			{
				m_inverseTangentType = (InverseTangentType)Enum.Parse( typeof( InverseTangentType ), GetCurrentParam( ref nodeParams ) );
			}
			UpdateSubtitle();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_from );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_to );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_perspectiveDivide );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_inverseTangentType );
		}
	}
}
