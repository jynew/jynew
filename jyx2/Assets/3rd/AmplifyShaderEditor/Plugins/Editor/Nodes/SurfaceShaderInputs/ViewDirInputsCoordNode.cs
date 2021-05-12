// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public enum ViewSpace
	{
		Tangent,
		World
	}

	[Serializable]
	[NodeAttributes( "View Dir", "Camera And Screen", "View direction vector, you can select between <b>World</b> space or <b>Tangent</b> space" )]
	public sealed class ViewDirInputsCoordNode : SurfaceShaderINParentNode
	{
		private const string SpaceStr = "Space";
		private const string WorldDirVarStr = "worldViewDir";
		private const string NormalizeOptionStr = "Safe Normalize";

		[SerializeField]
		private bool m_safeNormalize = false;

		[SerializeField]
		private ViewSpace m_viewDirSpace = ViewSpace.World;

		private UpperLeftWidgetHelper m_upperLeftWidget = new UpperLeftWidgetHelper();

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentInput = SurfaceInputs.VIEW_DIR;
			InitialSetup();
			m_textLabelWidth = 120;
			m_autoWrapProperties = true;
			m_drawPreviewAsSphere = true;
			m_hasLeftDropdown = true;
			UpdateTitle();
			m_previewShaderGUID = "07b57d9823df4bd4d8fe6dcb29fca36a";
		}

		private void UpdateTitle()
		{
			m_additionalContent.text = string.Format( Constants.SubTitleSpaceFormatStr, m_viewDirSpace.ToString() );
			m_sizeIsDirty = true;
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			m_upperLeftWidget.DrawWidget<ViewSpace>( ref m_viewDirSpace, this, OnWidgetUpdate );
		}

		private readonly Action<ParentNode> OnWidgetUpdate = ( x ) =>
		{
			( x as ViewDirInputsCoordNode ).UpdateTitle();
		};

		public override void DrawProperties()
		{
			//base.DrawProperties();
			EditorGUI.BeginChangeCheck();
			m_viewDirSpace = (ViewSpace)EditorGUILayoutEnumPopup( SpaceStr, m_viewDirSpace );
			if( EditorGUI.EndChangeCheck() )
			{
				UpdateTitle();
			}
			m_safeNormalize = EditorGUILayoutToggle( NormalizeOptionStr, m_safeNormalize );
			EditorGUILayout.HelpBox( "Having safe normalize ON makes sure your view vector is not zero even if you are using your shader with no cameras.", MessageType.None );
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if( m_viewDirSpace == ViewSpace.World )
				m_previewMaterialPassId = 0;
			else if( m_viewDirSpace == ViewSpace.Tangent )
				m_previewMaterialPassId = 1;
		}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			base.PropagateNodeData( nodeData, ref dataCollector );
			if( m_viewDirSpace == ViewSpace.Tangent )
				dataCollector.DirtyNormal = true;

			if( m_safeNormalize )
				dataCollector.SafeNormalizeViewDir = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if( dataCollector.IsTemplate )
			{
				string varName = ( m_viewDirSpace == ViewSpace.World ) ? dataCollector.TemplateDataCollectorInstance.GetViewDir(true,MasterNodePortCategory.Fragment, m_safeNormalize?NormalizeType.Safe:NormalizeType.Regular) :
																		dataCollector.TemplateDataCollectorInstance.GetTangentViewDir( m_currentPrecisionType ,true,MasterNodePortCategory.Fragment, m_safeNormalize ? NormalizeType.Safe : NormalizeType.Regular );
				return GetOutputVectorItem( 0, outputId, varName );
			}


			if( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				string result = GeneratorUtils.GenerateViewDirection( ref dataCollector, UniqueId, m_viewDirSpace );
				return GetOutputVectorItem( 0, outputId, result );
			}
			else
			{
				if( m_viewDirSpace == ViewSpace.World )
				{
					if( dataCollector.DirtyNormal || m_safeNormalize )
					{
						dataCollector.AddToInput( UniqueId, SurfaceInputs.WORLD_POS );
						string result = GeneratorUtils.GenerateViewDirection( ref dataCollector, UniqueId );
						return GetOutputVectorItem( 0, outputId, result );
					}
					else
					{
						dataCollector.AddToInput( UniqueId, SurfaceInputs.VIEW_DIR, PrecisionType.Float );
						return GetOutputVectorItem( 0, outputId, m_currentInputValueStr );
						//return base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalVar );
					}
				}
				else
				{
					if( m_safeNormalize )
					{
						dataCollector.AddToInput( UniqueId, SurfaceInputs.WORLD_NORMAL, m_currentPrecisionType );
						dataCollector.AddToInput( UniqueId, SurfaceInputs.INTERNALDATA, addSemiColon: false );
						dataCollector.ForceNormal = true;
						dataCollector.AddToInput( UniqueId, SurfaceInputs.WORLD_POS );
						string result = GeneratorUtils.GenerateViewDirection( ref dataCollector, UniqueId, ViewSpace.Tangent );
						return GetOutputVectorItem( 0, outputId, result );
					}
					else
					{
						dataCollector.ForceNormal = true;
						return base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalVar );
					}
				}
			}
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if( UIUtils.CurrentShaderVersion() > 2402 )
				m_viewDirSpace = (ViewSpace)Enum.Parse( typeof( ViewSpace ), GetCurrentParam( ref nodeParams ) );

			if( UIUtils.CurrentShaderVersion() > 15201 )
			{
				m_safeNormalize = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			}

			UpdateTitle();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_viewDirSpace );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_safeNormalize );
		}
	}
}
