// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
// http://kylehalladay.com/blog/tutorial/2014/02/18/Fresnel-Shaders-From-The-Ground-Up.html
// http://http.developer.nvidia.com/CgTutorial/cg_tutorial_chapter07.html

using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Fresnel", "Surface Data", "Simple Fresnel effect" )]
	public sealed class FresnelNode : ParentNode
	{
		private const string FresnedFinalVar = "fresnelNode";

		[SerializeField]
		private ViewSpace m_normalSpace = ViewSpace.Tangent;

		enum FresnelType
		{
			Standard = 0,
			Schlick,
			SchlickIOR,
		}

		enum NormalType
		{
			WorldNormal = 0,
			TangentNormal,
			HalfVector,
		}

		enum ViewType
		{
			ViewDir = 0,
			LightDir,
		}

		[SerializeField]
		private FresnelType m_fresnelType = FresnelType.Standard;

		[SerializeField]
		private NormalType m_normalType = NormalType.WorldNormal;

		[SerializeField]
		private ViewType m_viewType = ViewType.ViewDir;

		[SerializeField]
		private bool m_normalizeVectors = false;

		private InputPort m_normalVecPort;
		private InputPort m_viewVecPort;
		private InputPort m_biasPort;
		private InputPort m_scalePort;
		private InputPort m_powerPort;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "World Normal", -1, MasterNodePortCategory.Fragment, 0 );
			AddInputPort( WirePortDataType.FLOAT3, false, "View Dir", -1, MasterNodePortCategory.Fragment, 4 );
			AddInputPort( WirePortDataType.FLOAT, false, "Bias", -1, MasterNodePortCategory.Fragment, 1 );
			AddInputPort( WirePortDataType.FLOAT, false, "Scale", -1, MasterNodePortCategory.Fragment, 2 );
			AddInputPort( WirePortDataType.FLOAT, false, "Power", -1, MasterNodePortCategory.Fragment, 3 );
			AddOutputPort( WirePortDataType.FLOAT, "Out" );

			m_normalVecPort = m_inputPorts[ 0 ];
			m_viewVecPort = m_inputPorts[ 1 ];
			m_biasPort = m_inputPorts[ 2 ];
			m_scalePort = m_inputPorts[ 3 ];
			m_powerPort = m_inputPorts[ 4 ];

			m_biasPort.AutoDrawInternalData = true;
			m_scalePort.AutoDrawInternalData = true;
			m_powerPort.AutoDrawInternalData = true;
			m_autoWrapProperties = true;
			m_drawPreviewAsSphere = true;
			m_normalVecPort.Vector3InternalData = Vector3.forward;
			m_scalePort.FloatInternalData = 1;
			m_powerPort.FloatInternalData = 5;
			m_previewShaderGUID = "240145eb70cf79f428015012559f4e7d";
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			//m_mate
			PreviewMaterial.SetInt( "_FresnelType", (int)m_fresnelType );

			if( m_normalType == NormalType.TangentNormal && m_normalVecPort.IsConnected )
				m_previewMaterialPassId = 2;
			else if( (m_normalType == NormalType.WorldNormal || m_normalType == NormalType.HalfVector ) && m_normalVecPort.IsConnected && !m_viewVecPort.IsConnected )
				m_previewMaterialPassId = 1;
			else if( m_normalType == NormalType.HalfVector && !m_normalVecPort.IsConnected && !m_viewVecPort.IsConnected )
				m_previewMaterialPassId = 3;
			else if( m_normalVecPort.IsConnected && m_viewVecPort.IsConnected )
				m_previewMaterialPassId = 4;
			else if( !m_normalVecPort.IsConnected && !m_viewVecPort.IsConnected && m_viewType == ViewType.LightDir )
				m_previewMaterialPassId = 5;
			else if( !m_normalVecPort.IsConnected && m_viewVecPort.IsConnected && m_normalType == NormalType.HalfVector )
				m_previewMaterialPassId = 7;
			else if( !m_normalVecPort.IsConnected && m_viewVecPort.IsConnected )
				m_previewMaterialPassId = 6;
			else
				m_previewMaterialPassId = 0;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();

			EditorGUI.BeginChangeCheck();
			m_fresnelType = (FresnelType)EditorGUILayoutEnumPopup( "Type", m_fresnelType );
			m_normalType = (NormalType)EditorGUILayoutEnumPopup( "Normal Vector", m_normalType );
			m_viewType = (ViewType)EditorGUILayoutEnumPopup( "View Vector", m_viewType );
			if( EditorGUI.EndChangeCheck() )
			{
				UpdatePort();
			}

			if( !m_biasPort.IsConnected && m_biasPort.Visible )
				m_biasPort.FloatInternalData = EditorGUILayoutFloatField( m_biasPort.Name, m_biasPort.FloatInternalData );
			if( !m_scalePort.IsConnected && m_scalePort.Visible )
				m_scalePort.FloatInternalData = EditorGUILayoutFloatField( m_scalePort.Name, m_scalePort.FloatInternalData );
			if( !m_powerPort.IsConnected && m_powerPort.Visible )
				m_powerPort.FloatInternalData = EditorGUILayoutFloatField( m_powerPort.Name, m_powerPort.FloatInternalData );

			m_normalizeVectors = EditorGUILayoutToggle( "Normalize Vectors", m_normalizeVectors );
		}

		private void UpdatePort()
		{
			switch( m_normalType )
			{
				default:
				case NormalType.WorldNormal:
				m_normalVecPort.Name = "World Normal";
				break;
				case NormalType.TangentNormal:
				m_normalVecPort.Name = "Normal";
				break;
				case NormalType.HalfVector:
				m_normalVecPort.Name = "Half Vector";
				break;
			}

			switch( m_viewType )
			{
				default:
				case ViewType.ViewDir:
				m_viewVecPort.Name = "View Dir";
				break;
				case ViewType.LightDir:
				m_viewVecPort.Name = "Light Dir";
				break;
			}

			switch( m_fresnelType )
			{
				default:
				case FresnelType.Standard:
				m_biasPort.Visible = true;
				m_biasPort.Name = "Bias";
				m_scalePort.Name = "Scale";
				m_scalePort.Visible = true;
				m_powerPort.Visible = true;
				break;
				case FresnelType.Schlick:
				m_biasPort.Visible = true;
				m_biasPort.Name = "F0";
				m_scalePort.Visible = false;
				m_powerPort.Visible = false;
				break;
				case FresnelType.SchlickIOR:
				m_biasPort.Visible = false;
				m_scalePort.Name = "IOR";
				m_scalePort.Visible = true;
				m_powerPort.Visible = false;
				break;
			}

			m_sizeIsDirty = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			if( dataCollector.IsFragmentCategory )
				dataCollector.AddToInput( UniqueId, SurfaceInputs.WORLD_POS );

			string viewdir = string.Empty;
			if( m_viewType == ViewType.ViewDir )
			{
				if( m_viewVecPort.IsConnected )
					viewdir = m_viewVecPort.GeneratePortInstructions( ref dataCollector );
				else
					viewdir = GeneratorUtils.GenerateViewDirection( ref dataCollector, UniqueId, ViewSpace.World );
			}
			else
			{
				if( m_viewVecPort.IsConnected )
					viewdir = m_viewVecPort.GeneratePortInstructions( ref dataCollector );
				else
					viewdir = GeneratorUtils.GenerateWorldLightDirection( ref dataCollector, UniqueId, m_currentPrecisionType );
			}

			string normal = string.Empty;
			if( m_normalType == NormalType.WorldNormal || m_normalType == NormalType.TangentNormal )
			{
				if( m_normalVecPort.IsConnected )
				{
					normal = m_normalVecPort.GeneratePortInstructions( ref dataCollector );

					if( dataCollector.IsFragmentCategory )
					{
						dataCollector.AddToInput( UniqueId, SurfaceInputs.INTERNALDATA, addSemiColon: false );

						if( m_normalType == NormalType.TangentNormal )
						{
							if( dataCollector.IsTemplate )
							{
								normal = dataCollector.TemplateDataCollectorInstance.GetWorldNormal( UniqueId, m_currentPrecisionType, normal, OutputId );
							}
							else
							{
								normal = GeneratorUtils.GenerateWorldNormal( ref dataCollector, UniqueId, m_currentPrecisionType, normal, OutputId );
								dataCollector.AddToInput( UniqueId, SurfaceInputs.WORLD_NORMAL, m_currentPrecisionType );
								dataCollector.ForceNormal = true;
							}
						}
						else
						{
							if( m_normalizeVectors )
								normal = string.Format( "normalize( {0} )", normal );
						}
					}
					else
					{
						if( m_normalType == NormalType.TangentNormal )
						{
							string wtMatrix = GeneratorUtils.GenerateWorldToTangentMatrix( ref dataCollector, UniqueId, m_currentPrecisionType );
							normal = "mul( " + normal + "," + wtMatrix + " )";
						}
					}
				}
				else
				{
					if( dataCollector.IsFragmentCategory )
					{
						dataCollector.AddToInput( UniqueId, SurfaceInputs.WORLD_NORMAL, m_currentPrecisionType );
						if( dataCollector.DirtyNormal )
							dataCollector.AddToInput( UniqueId, SurfaceInputs.INTERNALDATA, addSemiColon: false );
					}

					if( dataCollector.IsTemplate )
						normal = dataCollector.TemplateDataCollectorInstance.GetWorldNormal( m_currentPrecisionType, normalize: ( dataCollector.DirtyNormal && m_normalizeVectors ) );
					else
						normal = GeneratorUtils.GenerateWorldNormal( ref dataCollector, UniqueId, ( dataCollector.DirtyNormal && m_normalizeVectors ) );

					if( dataCollector.DirtyNormal )
					{
						dataCollector.ForceNormal = true;
					}
				}
			}
			else
			{
				// generate HV
				if( !m_normalVecPort.IsConnected )
				{
					string halfView = GeneratorUtils.GenerateViewDirection( ref dataCollector, UniqueId, ViewSpace.World );
					string halfLight = GeneratorUtils.GenerateWorldLightDirection( ref dataCollector, UniqueId, m_currentPrecisionType );
					normal = "halfVector" + OutputId;
					dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT3, normal, "normalize( " + halfView + " + " + halfLight + " )" );
				}
				else
				{
					normal = m_normalVecPort.GeneratePortInstructions( ref dataCollector );
					if( m_normalizeVectors )
						normal = string.Format( "normalize( {0} )", normal );
				}
			}

			string bias = m_biasPort.GeneratePortInstructions( ref dataCollector );
			string scale = m_scalePort.GeneratePortInstructions( ref dataCollector );
			string power = m_powerPort.GeneratePortInstructions( ref dataCollector );

			string fresnelNDotVLocalValue = "dot( " + normal + ", " + viewdir + " )";
			string fresnelNDotVLocalVar = "fresnelNdotV" + OutputId;
			dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT, fresnelNDotVLocalVar, fresnelNDotVLocalValue );

			string fresnelFinalVar = FresnedFinalVar + OutputId;

			string result = string.Empty;
			switch( m_fresnelType )
			{
				default:
				case FresnelType.Standard:
				{
					result = string.Format( "( {0} + {1} * pow( 1.0 - {2}, {3} ) )", bias, scale, fresnelNDotVLocalVar, power );
				}
				break;
				case FresnelType.Schlick:
				{
					string f0VarName = "f0" + OutputId;
					dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT, f0VarName, bias );
					result = string.Format( "( {0} + ( 1.0 - {0} ) * pow( 1.0 - {1}, 5 ) )", f0VarName, fresnelNDotVLocalVar );
				}
				break;
				case FresnelType.SchlickIOR:
				{
					string iorVarName = "ior" + OutputId;
					dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT, iorVarName, scale );
					dataCollector.AddLocalVariable( UniqueId, iorVarName +" = pow( ( 1-"+ iorVarName +" )/( 1+"+iorVarName+" ), 2 );");
					result = string.Format( "( {0} + ( 1.0 - {0} ) * pow( 1.0 - {1}, 5 ) )", iorVarName, fresnelNDotVLocalVar );
				}
				break;
			}

			RegisterLocalVariable( 0, result, ref dataCollector, fresnelFinalVar );
			return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
		}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			base.PropagateNodeData( nodeData, ref dataCollector );
			if( m_normalType == NormalType.TangentNormal && m_normalVecPort.IsConnected )
				dataCollector.DirtyNormal = true;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );

			if( UIUtils.CurrentShaderVersion() > 15305 )
			{
				m_fresnelType = (FresnelType)Enum.Parse( typeof( FresnelType ), GetCurrentParam( ref nodeParams ) );
				m_normalType = (NormalType)Enum.Parse( typeof( NormalType ), GetCurrentParam( ref nodeParams ) );
				m_viewType = (ViewType)Enum.Parse( typeof( ViewType ), GetCurrentParam( ref nodeParams ) );
				m_normalizeVectors = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			}
			else
			{
				if( UIUtils.CurrentShaderVersion() >= 13202 )
				{
					m_normalSpace = (ViewSpace)Enum.Parse( typeof( ViewSpace ), GetCurrentParam( ref nodeParams ) );
				}
				else
				{
					m_normalSpace = ViewSpace.World;
				}

				if( m_normalSpace == ViewSpace.World )
					m_normalType = NormalType.WorldNormal;
				else
					m_normalType = NormalType.TangentNormal;
			}
			UpdatePort();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );

			IOUtils.AddFieldValueToString( ref nodeInfo, m_fresnelType );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_normalType );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_viewType );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_normalizeVectors );
		}
	}
}
