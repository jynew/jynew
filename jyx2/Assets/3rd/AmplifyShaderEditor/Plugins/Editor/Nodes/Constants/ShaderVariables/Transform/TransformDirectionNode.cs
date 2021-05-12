// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public enum InverseTangentType
	{
		Fast,
		Precise
	}

	[Serializable]
	[NodeAttributes( "Transform Direction", "Vector Operators", "Transforms a direction vector from one space to another" )]
	public sealed class TransformDirectionNode : ParentNode
	{

		[SerializeField]
		private TransformSpace m_from = TransformSpace.Object;

		[SerializeField]
		private TransformSpace m_to = TransformSpace.World;

		[SerializeField]
		private bool m_normalize = false;

		[SerializeField]
		private InverseTangentType m_inverseTangentType = InverseTangentType.Fast;

		private string InverseTBNStr = "Inverse TBN";

		private const string NormalizeOptionStr = "Normalize";
		private const string NormalizeFunc = "normalize( {0} )";

		private const string AseObjectToWorldDirVarName = "objToWorldDir";
		private const string AseObjectToWorldDirFormat = "mul( unity_ObjectToWorld, float4( {0}, 0 ) ).xyz";
		private const string AseHDObjectToWorldDirFormat = "mul( GetObjectToWorldMatrix(), float4( {0}, 0 ) ).xyz";

		private const string AseObjectToViewDirVarName = "objToViewDir";
		private const string AseObjectToViewDirFormat = "mul( UNITY_MATRIX_IT_MV, float4( {0}, 0 ) ).xyz";
		private const string AseHDObjectToViewDirFormat = "TransformWorldToViewDir( TransformObjectToWorldDir( {0} ))";

		private const string AseWorldToObjectDirVarName = "worldToObjDir";
		private const string AseWorldToObjectDirFormat = "mul( unity_WorldToObject, float4( {0}, 0 ) ).xyz";
		private const string AseHDWorldToObjectDirFormat = "mul( GetWorldToObjectMatrix(), float4( {0}, 0 ) ).xyz";


		private const string AseWorldToViewDirVarName = "worldToViewDir";
		private const string AseWorldToViewDirFormat = "mul( UNITY_MATRIX_V, float4( {0}, 0 ) ).xyz";

		private const string AseViewToObjectDirVarName = "viewToObjDir";
		private const string AseViewToObjectDirFormat = "mul( UNITY_MATRIX_T_MV, float4( {0}, 0 ) ).xyz";

		private const string AseViewToWorldDirVarName = "viewToWorldDir";
		private const string AseViewToWorldDirFormat = "mul( UNITY_MATRIX_I_V, float4( {0}, 0 ) ).xyz";

		///////////////////////////////////////////////////////////
		private const string AseObjectToClipDirVarName = "objectToClipDir";
		private const string AseObjectToClipDirFormat = "mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, float4({0}, 0.0)))";
		private const string AseSRPObjectToClipDirFormat = "TransformWorldToHClipDir(TransformObjectToWorldDir({0}))";

		private const string AseWorldToClipDirVarName = "worldToClipDir";
		private const string AseWorldToClipDirFormat = "mul(UNITY_MATRIX_VP, float4({0}, 0.0))";
		private const string AseSRPWorldToClipDirFormat = "TransformWorldToHClipDir({0})";

		private const string AseViewToClipDirVarName = "viewToClipDir";
		private const string AseViewToClipDirFormat = "mul(UNITY_MATRIX_P, float4({0}, 0.0))";
		private const string AseSRPViewToClipDirFormat = "mul(GetViewToHClipMatrix(), float4({0}, 1.0))";
		//
		private const string AseClipToObjectDirVarName = "clipToObjectDir";
		private const string AseClipToObjectDirFormat = "mul( UNITY_MATRIX_IT_MV, mul( unity_CameraInvProjection,float4({0},0)) ).xyz";
		private const string AseHDClipToObjectDirFormat = "mul( UNITY_MATRIX_I_M, mul( UNITY_MATRIX_I_VP,float4({0},0)) ).xyz";

		private const string AseClipToWorldDirVarName = "clipToWorldDir";
		private const string AseClipToWorldDirFormat = "mul( UNITY_MATRIX_I_V, mul( unity_CameraInvProjection,float4({0},0)) ).xyz";
		private const string AseHDClipToWorldDirFormat = "mul( UNITY_MATRIX_I_VP, float4({0},0) ).xyz";

		private const string AseClipToViewDirVarName = "clipToViewDir";
		private const string AseClipToViewDirFormat = " mul( unity_CameraInvProjection,float4({0},0)).xyz";
		private const string AseHDClipToViewDirFormat = " mul( UNITY_MATRIX_I_P,float4({0},0)).xyz";
		private const string AseClipToNDC = "{0}.xyz/{0}.w";
		
		/////////////////////////////////////////////////////
		private const string AseObjectToTangentDirVarName = "objectToTangentDir";
		private const string AseWorldToTangentDirVarName = "worldToTangentDir";
		private const string AseViewToTangentDirVarName = "viewToTangentDir";
		private const string AseClipToTangentDirVarName = "clipToTangentDir";
		private const string ASEWorldToTangentFormat = "mul( ase_worldToTangent, {0})";


		private const string AseTangentToObjectDirVarName = "tangentTobjectDir";
		private const string AseTangentToWorldDirVarName = "tangentToWorldDir";
		private const string AseTangentToViewDirVarName = "tangentToViewDir";
		private const string AseTangentToClipDirVarName = "tangentToClipDir";
		private const string ASEMulOpFormat = "mul( {0}, {1} )";
		


		///////////////////////////////////////////////////////////
		private const string FromStr = "From";
		private const string ToStr = "To";
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
			m_textLabelWidth = 100;
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

			m_normalize = EditorGUILayoutToggle( NormalizeOptionStr, m_normalize );
			if( EditorGUI.EndChangeCheck() )
			{
				UpdateSubtitle();
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
						default: case TransformSpace.Object: break;
						case TransformSpace.World:
						{
							if( dataCollector.IsTemplate && dataCollector.TemplateDataCollectorInstance.CurrentSRPType == TemplateSRPType.HD )
								result = string.Format( AseHDObjectToWorldDirFormat, result );
							else
								result = string.Format( AseObjectToWorldDirFormat, result );
							varName = AseObjectToWorldDirVarName + OutputId;
						}
						break;
						case TransformSpace.View:
						{
							if( dataCollector.IsTemplate && dataCollector.TemplateDataCollectorInstance.CurrentSRPType == TemplateSRPType.HD )
								result = string.Format( AseHDObjectToViewDirFormat, result );
							else
								result = string.Format( AseObjectToViewDirFormat, result );
							varName = AseObjectToViewDirVarName + OutputId;
						}
						break;
						case TransformSpace.Clip:
						{
							if( dataCollector.IsTemplate && dataCollector.TemplateDataCollectorInstance.CurrentSRPType != TemplateSRPType.BuiltIn )
							{
								result = string.Format( AseSRPObjectToClipDirFormat, result );
							}
							else
							{
								result = string.Format( AseObjectToClipDirFormat, result );
							}
							varName = AseObjectToClipDirVarName + OutputId;
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
								result = string.Format( AseHDWorldToObjectDirFormat, result );
							else
								result = string.Format( AseWorldToObjectDirFormat, result );
							varName = AseWorldToObjectDirVarName + OutputId;
						}
						break;
						default:
						case TransformSpace.World: break;
						case TransformSpace.View:
						{
							result = string.Format( AseWorldToViewDirFormat, result );
							varName = AseWorldToViewDirVarName + OutputId;
						}
						break;
						case TransformSpace.Clip:
						{
							if( dataCollector.IsTemplate && dataCollector.TemplateDataCollectorInstance.CurrentSRPType != TemplateSRPType.BuiltIn )
							{
								result = string.Format( AseSRPWorldToClipDirFormat, result );
							}
							else
							{
								result = string.Format( AseWorldToClipDirFormat, result );
							}
							varName = AseWorldToClipDirVarName + OutputId;
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
							result = string.Format( AseViewToObjectDirFormat, result );
							varName = AseViewToObjectDirVarName + OutputId;
						}
						break;
						case TransformSpace.World:
						{
							result = string.Format( AseViewToWorldDirFormat, result );
							varName = AseViewToWorldDirVarName + OutputId;
						}
						break;
						default: case TransformSpace.View: break;
						case TransformSpace.Clip:
						{
							if( dataCollector.IsTemplate && dataCollector.TemplateDataCollectorInstance.CurrentSRPType != TemplateSRPType.BuiltIn )
							{
								result = string.Format( AseSRPViewToClipDirFormat, result );
							}
							else
							{
								result = string.Format( AseViewToClipDirFormat, result );
							}
							varName = AseViewToClipDirVarName + OutputId;
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
								result = string.Format( AseHDClipToObjectDirFormat, result );
							}
							else
							{
								result = string.Format( AseClipToObjectDirFormat, result );
							}
							varName = AseClipToObjectDirVarName + OutputId;
						}
						break;
						case TransformSpace.World:
						{
							if( dataCollector.IsTemplate && dataCollector.TemplateDataCollectorInstance.CurrentSRPType == TemplateSRPType.HD )
							{
								result = string.Format( AseHDClipToWorldDirFormat, result );
							}
							else
							{
								result = string.Format( AseClipToWorldDirFormat, result );
							}
							varName = AseClipToWorldDirVarName + OutputId;
						}
						break;
						case TransformSpace.View:
						{
							if( dataCollector.IsTemplate && dataCollector.TemplateDataCollectorInstance.CurrentSRPType == TemplateSRPType.HD )
							{
								result = string.Format( AseHDClipToViewDirFormat, result );
							}
							else
							{
								result = string.Format( AseClipToViewDirFormat, result );
							}
							varName = AseClipToViewDirVarName + OutputId;
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
						default: case TransformSpace.Object: break;
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
							GeneratorUtils.GenerateWorldToTangentMatrix(ref dataCollector, UniqueId,m_currentPrecisionType );
							CalculateTransform( m_from, TransformSpace.World, ref dataCollector, ref varName, ref result );
							result = string.Format( ASEWorldToTangentFormat, result );
							varName = AseObjectToTangentDirVarName;
						}
						break;
					}
				}break;
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
						case TransformSpace.World:break;
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
							varName = AseWorldToTangentDirVarName;
						}
						break;
					}
				}break;
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
							CalculateTransform( m_from, m_to, ref dataCollector, ref varName, ref result );
						}
						break;
						default:case TransformSpace.View:break;
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
							varName = AseViewToTangentDirVarName;
						}
						break;
					}
				}break;
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
							varName = AseClipToTangentDirVarName;
						}
						break;
						default:
						break;
					}
				}break;
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
							varName = AseTangentToObjectDirVarName;
						}
						break;
						case TransformSpace.World:
						{
							result = string.Format( ASEMulOpFormat, matrixVal, result );
							varName = AseTangentToWorldDirVarName;
						}
						break;
						case TransformSpace.View:
						{
							result = string.Format( ASEMulOpFormat, matrixVal, result );
							CalculateTransform( TransformSpace.World, m_to, ref dataCollector, ref varName, ref result );
							varName = AseTangentToViewDirVarName;
						}
						break;
						case TransformSpace.Clip:
						{
							result = string.Format( ASEMulOpFormat, matrixVal, result );
							CalculateTransform( TransformSpace.World, m_to, ref dataCollector, ref varName, ref result );
							varName = AseTangentToClipDirVarName;
						}
						break;
						case TransformSpace.Tangent:
						default:
						break;
					}
				}
				break;
				default:break;
			}

			if( m_normalize )
				result = string.Format( NormalizeFunc, result );

			RegisterLocalVariable( 0, result, ref dataCollector, varName );
			return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_from = (TransformSpace)Enum.Parse( typeof( TransformSpace ), GetCurrentParam( ref nodeParams ) );
			m_to = (TransformSpace)Enum.Parse( typeof( TransformSpace ), GetCurrentParam( ref nodeParams ) );
			m_normalize = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
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
			IOUtils.AddFieldValueToString( ref nodeInfo, m_normalize );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_inverseTangentType );
		}
	}
}
