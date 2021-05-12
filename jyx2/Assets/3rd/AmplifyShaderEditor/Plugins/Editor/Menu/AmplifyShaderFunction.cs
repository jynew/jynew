// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using AmplifyShaderEditor;

[Serializable]
public class AmplifyShaderFunction : ScriptableObject
{
	[SerializeField]
	private string m_functionInfo = string.Empty;
	public string FunctionInfo
	{
		get { return m_functionInfo; }
		set { m_functionInfo = value; }
	}

	[SerializeField]
	private string m_functionName = string.Empty;
	public string FunctionName
	{
		get { if( m_functionName.Length == 0 ) return name; else return m_functionName; }
		set { m_functionName = value; }
	}

	[SerializeField]
	[TextArea( 5, 15 )]
	private string m_description = string.Empty;
	public string Description
	{
		get { return m_description; }
		set { m_description = value; }
	}

	[SerializeField]
	private AdditionalIncludesHelper m_additionalIncludes = new AdditionalIncludesHelper();
	//public AdditionalIncludesHelper AdditionalIncludes
	//{
	//	get { return m_additionalIncludes; }
	//	set { m_additionalIncludes = value; }
	//}

	[SerializeField]
	private AdditionalPragmasHelper m_additionalPragmas = new AdditionalPragmasHelper();
	//public AdditionalPragmasHelper AdditionalPragmas
	//{
	//	get { return m_additionalPragmas; }
	//	set { m_additionalPragmas = value; }
	//}

	[SerializeField]
	private TemplateAdditionalDirectivesHelper m_additionalDirectives = new TemplateAdditionalDirectivesHelper( " Additional Directives" );
	public TemplateAdditionalDirectivesHelper AdditionalDirectives
	{
		get { return m_additionalDirectives; }
		set { m_additionalDirectives = value; }
	}
	
	[SerializeField]
	private FunctionNodeCategories m_nodeCategory = FunctionNodeCategories.Functions;
	public FunctionNodeCategories NodeCategory
	{
		get { return m_nodeCategory; }
		set { m_nodeCategory = value; }
	}

	[SerializeField]
	private string m_customNodeCategory = string.Empty;
	public string CustomNodeCategory
	{
		get
		{
			if( m_nodeCategory == FunctionNodeCategories.Custom )
			{
				if( string.IsNullOrEmpty( m_customNodeCategory ) )
					return "Functions";
				else
					return m_customNodeCategory;
			}
			else
			{
				return UIUtils.CategoryPresets[ (int)m_nodeCategory ];
				//return new SerializedObject( this ).FindProperty( "m_nodeCategory" ).enumDisplayNames[ (int)m_nodeCategory ];
			}
		}
	}
	
	[SerializeField]
	private PreviewLocation m_previewPosition = PreviewLocation.Auto;
	public PreviewLocation PreviewPosition
	{
		get { return m_previewPosition; }
		set { m_previewPosition = value; }
	}

	public void UpdateDirectivesList()
	{
		m_additionalDirectives.CleanNullDirectives();
		m_additionalDirectives.UpdateDirectivesFromSaveItems();

		if( m_additionalIncludes.IncludeList.Count > 0 )
		{
			m_additionalDirectives.AddItems( AdditionalLineType.Include, m_additionalIncludes.IncludeList );
			m_additionalIncludes.IncludeList.Clear();
		}

		if( m_additionalPragmas.PragmaList.Count > 0 )
		{
			m_additionalDirectives.AddItems( AdditionalLineType.Pragma, m_additionalPragmas.PragmaList );
			m_additionalPragmas.PragmaList.Clear();
		}

	}
}

public class ShaderFunctionDetector : AssetPostprocessor
{
	static void OnPostprocessAllAssets( string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths )
	{
		if( UIUtils.CurrentWindow == null )
			return;

		bool markForRefresh = false;
		AmplifyShaderFunction function = null;
		for( int i = 0; i < importedAssets.Length; i++ )
		{
			function = AssetDatabase.LoadAssetAtPath<AmplifyShaderFunction>( importedAssets[ i ] );
			if( function != null )
			{
				markForRefresh = true;
				break;
			}
		}

		if( deletedAssets.Length > 0 )
			markForRefresh = true;

		for( int i = 0; i < movedAssets.Length; i++ )
		{
			function = AssetDatabase.LoadAssetAtPath<AmplifyShaderFunction>( movedAssets[ i ] );
			if( function != null )
			{
				markForRefresh = true;
				break;
			}
		}

		for( int i = 0; i < movedFromAssetPaths.Length; i++ )
		{
			function = AssetDatabase.LoadAssetAtPath<AmplifyShaderFunction>( movedFromAssetPaths[ i ] );
			if( function != null )
			{
				markForRefresh = true;
				break;
			}
		}

		if( markForRefresh )
		{
			markForRefresh = false;
			if( function != null )
			{
				IOUtils.UpdateSFandRefreshWindows( function );
			}
			UIUtils.CurrentWindow.LateRefreshAvailableNodes();
		}
	}
}
