// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	public enum BuiltInShaderTransformTypes
	{
		UNITY_MATRIX_MVP = 0,
		UNITY_MATRIX_MV,
		UNITY_MATRIX_V,
		UNITY_MATRIX_P,
		UNITY_MATRIX_VP,
		UNITY_MATRIX_T_MV,
		UNITY_MATRIX_IT_MV,
		//UNITY_MATRIX_TEXTURE0,
		//UNITY_MATRIX_TEXTURE1,
		//UNITY_MATRIX_TEXTURE2,
		//UNITY_MATRIX_TEXTURE3,
		_Object2World,
		_World2Object//,
		//unity_Scale
	}

	[Serializable]
	[NodeAttributes( "Common Transform Matrices", "Matrix Transform", "All Transformation types" )]
	public sealed class TransformVariables : ShaderVariablesNode
	{
		[SerializeField]
		private BuiltInShaderTransformTypes m_selectedType = BuiltInShaderTransformTypes.UNITY_MATRIX_MVP;
		
		private const string MatrixLabelStr = "Matrix";
		private readonly string[] ValuesStr =  
		{
			"Model View Projection",
			"Model View",
			"View",
			"Projection",
			"View Projection",
			"Transpose Model View",
			"Inverse Transpose Model View",
			"Object to World",
			"Word to Object"
		};

		private UpperLeftWidgetHelper m_upperLeftWidget = new UpperLeftWidgetHelper();
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputProperties( 0, ValuesStr[ ( int ) m_selectedType ], WirePortDataType.FLOAT4x4 );
			m_textLabelWidth = 60;
			m_hasLeftDropdown = true;
			m_autoWrapProperties = true;
			m_drawPreview = false;
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
			m_selectedType = (BuiltInShaderTransformTypes)m_upperLeftWidget.DrawWidget( this, (int)m_selectedType, ValuesStr );
			if( EditorGUI.EndChangeCheck() )
			{
				ChangeOutputName( 0, ValuesStr[ (int)m_selectedType ] );
				m_sizeIsDirty = true;
			}
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUI.BeginChangeCheck();
			m_selectedType = ( BuiltInShaderTransformTypes ) EditorGUILayoutPopup( MatrixLabelStr, ( int ) m_selectedType, ValuesStr );
			if ( EditorGUI.EndChangeCheck() )
			{
				ChangeOutputName( 0, ValuesStr[ ( int ) m_selectedType ] );
				m_sizeIsDirty = true;
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
			string selectedTypeStr = GetCurrentParam( ref nodeParams );
			try
			{
				BuiltInShaderTransformTypes selectedType = (BuiltInShaderTransformTypes)Enum.Parse( typeof( BuiltInShaderTransformTypes ), selectedTypeStr );
				m_selectedType = selectedType;
			}
			catch( Exception e )
			{
				switch( selectedTypeStr )
				{
					default: Debug.LogException( e );break;
					case "UNITY_MATRIX_TEXTURE0":UIUtils.ShowMessage("Texture 0 matrix is no longer supported",MessageSeverity.Warning);break;
					case "UNITY_MATRIX_TEXTURE1":UIUtils.ShowMessage("Texture 1 matrix is no longer supported",MessageSeverity.Warning);break;
					case "UNITY_MATRIX_TEXTURE2":UIUtils.ShowMessage("Texture 2 matrix is no longer supported",MessageSeverity.Warning);break;
					case "UNITY_MATRIX_TEXTURE3":UIUtils.ShowMessage("Texture 3 matrix is no longer supported",MessageSeverity.Warning); break;
					case "unity_Scale": UIUtils.ShowMessage( "Scale matrix is no longer supported", MessageSeverity.Warning ); break;
				}
			}

			ChangeOutputName( 0, ValuesStr[ ( int ) m_selectedType ] );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_selectedType );
		}
	}
}
