// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

using UnityEngine;
using UnityEditor;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Outline", "Miscellaneous", "Uses vertices to simulate an outline around the object" )]
	public sealed class OutlineNode : ParentNode
	{
		enum OutlineAlphaModes
		{
			None = 0,
			Masked,
			Transparent,
			AlphaPremultiplied
		};

		private const string CullModePortNameStr = "Cull Mode";
		private const string AlphaModePortNameStr = "Alpha";
		private const string MaskedModePortNamStr = "Opacity Mask";
		private const string OutlineAlphaModeStr = "Alpha Mode";
		private const string OpacityMaskClipValueStr = "Mask Clip Value";
		private const string ErrorMessage = "Outline node should only be connected to vertex ports.";

		[SerializeField]
		private bool m_noFog = true;

		[SerializeField]
		private string[] AvailableOutlineModes = { "Vertex Offset", "Vertex Scale", "Custom" };

		[SerializeField]
		private int[] AvailableOutlineValues = { 0, 1, 2 };

		[SerializeField]
		private int m_currentSelectedMode = 0;

		[SerializeField]
		private OutlineAlphaModes m_currentAlphaMode = OutlineAlphaModes.None;

		private UpperLeftWidgetHelper m_upperLeftWidget = new UpperLeftWidgetHelper();

		[NonSerialized]
		private StandardSurfaceOutputNode m_masterNode = null;

		[SerializeField]
		private int m_zTestMode = 0;

		[SerializeField]
		private int m_zWriteMode = 0;

		[SerializeField]
		private CullMode m_cullMode = CullMode.Front;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );

			AddOutputPort( WirePortDataType.FLOAT3, "Out" );

			AddInputPort( WirePortDataType.FLOAT3, false, "Color", -1, MasterNodePortCategory.Fragment, 0 );
			AddInputPort( WirePortDataType.FLOAT, false, "Alpha", -1, MasterNodePortCategory.Fragment, 2 );
			AddInputPort( WirePortDataType.FLOAT, false, "Width", -1, MasterNodePortCategory.Fragment, 1 );
			GetInputPortByUniqueId( 2 ).Visible = false;
			m_textLabelWidth = 115;
			m_hasLeftDropdown = true;
			SetAdditonalTitleText( string.Format( Constants.SubTitleTypeFormatStr, AvailableOutlineModes[ m_currentSelectedMode ] ) );
		}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			base.PropagateNodeData( nodeData, ref dataCollector );
			if( GetInputPortByUniqueId( 0 ).IsConnected )
				dataCollector.UsingCustomOutlineColor = true;

			if( GetInputPortByUniqueId( 1 ).IsConnected )
				dataCollector.UsingCustomOutlineWidth = true;

			if( GetInputPortByUniqueId( 2 ).IsConnected )
				dataCollector.UsingCustomOutlineAlpha = true;

			ContainerGraph.CurrentStandardSurface.OutlineHelper.ZWriteMode = m_zWriteMode;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.OffsetMode = m_currentSelectedMode;
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
			EditorGUI.BeginChangeCheck();
			m_currentSelectedMode = m_upperLeftWidget.DrawWidget( this, m_currentSelectedMode, AvailableOutlineModes );
			if( EditorGUI.EndChangeCheck() )
			{
				SetAdditonalTitleText( string.Format( Constants.SubTitleTypeFormatStr, AvailableOutlineModes[ m_currentSelectedMode ] ) );
				UpdatePorts();
			}
		}

		void CheckAlphaPortVisibility()
		{
			InputPort alphaPort = GetInputPortByUniqueId( 2 );
			if( m_currentAlphaMode != OutlineAlphaModes.None )
			{
				if( !alphaPort.Visible )
					alphaPort.Visible = true;

				if( m_currentAlphaMode == OutlineAlphaModes.Masked )
				{
					GetInputPortByUniqueId( 2 ).Name = MaskedModePortNamStr;
				}
				else
				{
					GetInputPortByUniqueId( 2 ).Name = AlphaModePortNameStr;
				}
				m_sizeIsDirty = true;
			}

			if( m_currentAlphaMode == OutlineAlphaModes.None && alphaPort.Visible )
			{
				alphaPort.Visible = false;
				m_sizeIsDirty = true;
			}
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			NodeUtils.DrawPropertyGroup( ref m_propertiesFoldout, Constants.ParameterLabelStr, () =>
			{
				EditorGUI.BeginChangeCheck();
				m_currentSelectedMode = EditorGUILayoutIntPopup( "Type", m_currentSelectedMode, AvailableOutlineModes, AvailableOutlineValues );
				if( EditorGUI.EndChangeCheck() )
				{
					SetAdditonalTitleText( string.Format( Constants.SubTitleTypeFormatStr, AvailableOutlineModes[ m_currentSelectedMode ] ) );
					UpdatePorts();
				}

				EditorGUI.BeginChangeCheck();
				m_currentAlphaMode = (OutlineAlphaModes)EditorGUILayoutEnumPopup( OutlineAlphaModeStr, m_currentAlphaMode );
				if( EditorGUI.EndChangeCheck() )
				{
					CheckAlphaPortVisibility();
				}

				if( m_currentAlphaMode == OutlineAlphaModes.Masked )
				{
					if( m_masterNode == null )
					{
						m_masterNode = m_containerGraph.CurrentMasterNode as StandardSurfaceOutputNode;
					}

					if( m_masterNode != null )
					{
						m_masterNode.ShowOpacityMaskValueUI();
					}
				}

				m_cullMode = (CullMode)EditorGUILayoutEnumPopup( CullModePortNameStr, m_cullMode ); 
				m_zWriteMode = EditorGUILayoutPopup( ZBufferOpHelper.ZWriteModeStr, m_zWriteMode, ZBufferOpHelper.ZWriteModeValues );
				m_zTestMode = EditorGUILayoutPopup( ZBufferOpHelper.ZTestModeStr, m_zTestMode, ZBufferOpHelper.ZTestModeLabels );
				m_noFog = EditorGUILayoutToggle( "No Fog", m_noFog );

			} );
		}

		void UpdatePorts()
		{
			if( m_currentSelectedMode == 2 ) //custom mode
			{
				GetInputPortByUniqueId( 1 ).ChangeProperties( "Offset", WirePortDataType.FLOAT3, false );
			}
			else
			{
				GetInputPortByUniqueId( 1 ).ChangeProperties( "Width", WirePortDataType.FLOAT, false );
			}

		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( dataCollector.IsTemplate )
				return "0";

			if( dataCollector.IsFragmentCategory )
			{
				UIUtils.ShowMessage( ErrorMessage );
				return "0";
			}
			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return "0";

			m_outputPorts[ 0 ].SetLocalValue( "0", dataCollector.PortCategory );

			StandardSurfaceOutputNode masterNode = m_containerGraph.CurrentMasterNode as StandardSurfaceOutputNode;

			MasterNodeDataCollector outlineDataCollector = new MasterNodeDataCollector();
			outlineDataCollector.IsOutlineDataCollector = true;
			outlineDataCollector.DirtyNormal = true;
			InputPort colorPort = GetInputPortByUniqueId( 0 );
			InputPort alphaPort = GetInputPortByUniqueId( 2 );
			InputPort vertexPort = GetInputPortByUniqueId( 1 );

			if( vertexPort.IsConnected )
			{
				outlineDataCollector.PortCategory = MasterNodePortCategory.Vertex;
				string outlineWidth = vertexPort.GenerateShaderForOutput( ref outlineDataCollector, vertexPort.DataType, true, true );
				outlineDataCollector.AddToVertexLocalVariables( UniqueId, PrecisionType.Float, vertexPort.DataType, "outlineVar", outlineWidth );

				outlineDataCollector.AddVertexInstruction( outlineDataCollector.SpecialLocalVariables, UniqueId, false );
				outlineDataCollector.ClearSpecialLocalVariables();

				outlineDataCollector.AddVertexInstruction( outlineDataCollector.VertexLocalVariables, UniqueId, false );
				outlineDataCollector.ClearVertexLocalVariables();

				// need to check whether this breaks other outputs or not
				ContainerGraph.ResetNodesLocalVariables();
			}

			outlineDataCollector.PortCategory = MasterNodePortCategory.Fragment;
			string outlineColor = colorPort.GeneratePortInstructions( ref outlineDataCollector );// "\to.Emission = " + colorPort.GeneratePortInstructions( ref outlineDataCollector ) + ";";
			string alphaValue = alphaPort.Visible ? alphaPort.GeneratePortInstructions( ref outlineDataCollector ) : string.Empty;

			bool addTabs = outlineDataCollector.DirtySpecialLocalVariables || alphaPort.Available;
			outlineDataCollector.AddInstructions( "\t" + outlineDataCollector.SpecialLocalVariables.TrimStart( '\t' ) );
			outlineDataCollector.ClearSpecialLocalVariables();
			outlineDataCollector.AddInstructions( ( addTabs ? "\t\t\t" : "" ) + "o.Emission = " + outlineColor + ";" );
			if( alphaPort.Visible )
			{
				if( m_currentAlphaMode == OutlineAlphaModes.Masked )
				{
					float maskClipValue = 0.5f;

					if( masterNode != null )
						maskClipValue = masterNode.OpacityMaskClipValue;

					if( masterNode.InlineOpacityMaskClipValue.IsValid )
					{
						RangedFloatNode fnode = UIUtils.GetNode( masterNode.InlineOpacityMaskClipValue.NodeId ) as RangedFloatNode;
						if( fnode != null )
						{
							outlineDataCollector.AddToProperties( fnode.UniqueId, fnode.GetPropertyValue(), fnode.OrderIndex );
							outlineDataCollector.AddToUniforms( fnode.UniqueId, fnode.GetUniformValue() );
						}
						else
						{
							IntNode inode = UIUtils.GetNode( masterNode.InlineOpacityMaskClipValue.NodeId ) as IntNode;
							outlineDataCollector.AddToProperties( inode.UniqueId, inode.GetPropertyValue(), inode.OrderIndex );
							outlineDataCollector.AddToUniforms( inode.UniqueId, inode.GetUniformValue() );
						}
					}
					else
					{
						outlineDataCollector.AddToProperties( -1, string.Format( IOUtils.MaskClipValueProperty, OpacityMaskClipValueStr, maskClipValue ), -1 );
						outlineDataCollector.AddToUniforms( -1, string.Format( IOUtils.MaskClipValueUniform, maskClipValue ) );
					}

					outlineDataCollector.AddInstructions( ( addTabs ? "\n\t\t\t" : "" ) + "clip( " + alphaValue + " - " + masterNode.InlineOpacityMaskClipValue.GetValueOrProperty( IOUtils.MaskClipValueName, false ) + " );" );
				}
				else
				{
					outlineDataCollector.AddInstructions( ( addTabs ? "\n\t\t\t" : "" ) + "o.Alpha = " + alphaValue + ";" );
				}
			}

			if( outlineDataCollector.UsingWorldNormal )
				outlineDataCollector.AddInstructions( ( addTabs ? "\n\t\t\t" : "" ) + "o.Normal = float3(0,0,-1);" );

			if( masterNode != null )
			{
				//masterNode.AdditionalIncludes.AddToDataCollector( ref outlineDataCollector );
				//masterNode.AdditionalPragmas.AddToDataCollector( ref outlineDataCollector );
				//masterNode.AdditionalDefines.AddToDataCollector( ref outlineDataCollector );
				masterNode.AdditionalDirectives.AddAllToDataCollector( ref outlineDataCollector );
			}

			ContainerGraph.CurrentStandardSurface.OutlineHelper.InputList = outlineDataCollector.InputList;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.Inputs = outlineDataCollector.Inputs;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.DirtyInput = outlineDataCollector.DirtyInputs;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.Includes = outlineDataCollector.Includes;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.Pragmas = outlineDataCollector.Pragmas;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.Defines = outlineDataCollector.Defines;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.Uniforms = outlineDataCollector.Uniforms;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.GrabPasses = outlineDataCollector.GrabPass;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.UniformList = outlineDataCollector.UniformsList;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.VertexData = outlineDataCollector.VertexData;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.Instructions = outlineDataCollector.Instructions;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.Functions = outlineDataCollector.Functions;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.LocalFunctions = outlineDataCollector.LocalFunctions;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.OutlineCullMode = m_cullMode;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.ZTestMode = m_zTestMode;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.ZWriteMode = m_zWriteMode;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.OffsetMode = m_currentSelectedMode;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.CustomNoFog = m_noFog;
			dataCollector.CustomOutlineSelectedAlpha = (int)m_currentAlphaMode;

			for( int i = 0; i < outlineDataCollector.PropertiesList.Count; i++ )
			{
				dataCollector.AddToProperties( UniqueId, outlineDataCollector.PropertiesList[ i ].PropertyName, outlineDataCollector.PropertiesList[ i ].OrderIndex );
			}

			ContainerGraph.ResetNodesLocalVariablesIfNot( MasterNodePortCategory.Vertex );
			return "0";
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_currentSelectedMode = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_noFog = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			if( UIUtils.CurrentShaderVersion() > 14202 )
				m_currentAlphaMode = (OutlineAlphaModes)Enum.Parse( typeof( OutlineAlphaModes ), GetCurrentParam( ref nodeParams ) );

			if( UIUtils.CurrentShaderVersion() > 14302 )
			{
				m_zWriteMode = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				m_zTestMode = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			}

			if( UIUtils.CurrentShaderVersion() > 15304 )
			{
				m_cullMode = (CullMode)Enum.Parse( typeof( CullMode ), GetCurrentParam( ref nodeParams ) );
			}

			SetAdditonalTitleText( string.Format( Constants.SubTitleTypeFormatStr, AvailableOutlineModes[ m_currentSelectedMode ] ) );
			UpdatePorts();
			CheckAlphaPortVisibility();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_currentSelectedMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_noFog );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_currentAlphaMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_zWriteMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_zTestMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_cullMode );
		}
	}
}
