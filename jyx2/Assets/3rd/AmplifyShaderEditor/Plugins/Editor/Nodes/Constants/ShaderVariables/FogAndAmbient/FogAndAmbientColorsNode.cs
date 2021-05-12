// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	public enum BuiltInFogAndAmbientColors
	{
		UNITY_LIGHTMODEL_AMBIENT = 0,
		unity_AmbientSky,
		unity_AmbientEquator,
		unity_AmbientGround,
		unity_FogColor
	}

	[Serializable]
	[NodeAttributes( "Fog And Ambient Colors", "Light", "Fog and Ambient colors" )]
	public sealed class FogAndAmbientColorsNode : ShaderVariablesNode
	{
		private const string ColorLabelStr = "Color";
		private readonly string[] ColorValuesStr = {
														"Ambient light ( Legacy )",
														"Sky ambient light",
														"Equator ambient light",
														"Ground ambient light",
														"Fog"
													};

		[SerializeField]
		private BuiltInFogAndAmbientColors m_selectedType = BuiltInFogAndAmbientColors.UNITY_LIGHTMODEL_AMBIENT;
		
		private UpperLeftWidgetHelper m_upperLeftWidget = new UpperLeftWidgetHelper();

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputProperties( 0, ColorValuesStr[ ( int ) m_selectedType ], WirePortDataType.COLOR );
			m_textLabelWidth = 50;
			m_autoWrapProperties = true;
			m_hasLeftDropdown = true;
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
			m_selectedType = (BuiltInFogAndAmbientColors)m_upperLeftWidget.DrawWidget( this, (int)m_selectedType, ColorValuesStr );
			if( EditorGUI.EndChangeCheck() )
			{
				ChangeOutputName( 0, ColorValuesStr[ (int)m_selectedType ] );
			}
		}
		
		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUI.BeginChangeCheck();
			m_selectedType = ( BuiltInFogAndAmbientColors ) EditorGUILayoutPopup( ColorLabelStr, ( int ) m_selectedType, ColorValuesStr );

			if ( EditorGUI.EndChangeCheck() )
			{
				ChangeOutputName( 0, ColorValuesStr[ ( int ) m_selectedType ] );
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			return m_selectedType.ToString();
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_selectedType = ( BuiltInFogAndAmbientColors ) Enum.Parse( typeof( BuiltInFogAndAmbientColors ), GetCurrentParam( ref nodeParams ) );
			ChangeOutputName( 0, ColorValuesStr[ ( int ) m_selectedType ] );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_selectedType );
		}
	}
}
