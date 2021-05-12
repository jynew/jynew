// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEditor;

namespace AmplifyShaderEditor
{
	public sealed class TemplatePostProcessor : AssetPostprocessor
	{
		static void OnPostprocessAllAssets( string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths )
		{
			if( UIUtils.CurrentWindow == null )
				return;

			TemplatesManager templatesManager = UIUtils.CurrentWindow.TemplatesManagerInstance;
			if( templatesManager == null )
				return;

			if( !templatesManager.Initialized )
			{
				templatesManager.Init();
			}

			bool refreshMenuItems = false;
			for( int i = 0; i < importedAssets.Length; i++ )
			{
				if( TemplateHelperFunctions.CheckIfTemplate( importedAssets[ i ] ) )
				{

					string guid = AssetDatabase.AssetPathToGUID( importedAssets[ i ] );
					TemplateDataParent templateData = templatesManager.GetTemplate( guid );
					if( templateData != null )
					{
						refreshMenuItems = templateData.Reload() || refreshMenuItems;
					}
					else
					{
						refreshMenuItems = true;
						string name = TemplatesManager.OfficialTemplates.ContainsKey( guid ) ? TemplatesManager.OfficialTemplates[ guid ] : string.Empty;
						TemplateMultiPass mp = TemplateMultiPass.CreateInstance<TemplateMultiPass>();
						mp.Init( name, guid, true );
						templatesManager.AddTemplate( mp );
					}
				}
			}

			if( deletedAssets.Length > 0 )
			{
				if( deletedAssets[ 0 ].IndexOf( Constants.InvalidPostProcessDatapath ) < 0 )
				{
					for( int i = 0; i < deletedAssets.Length; i++ )
					{
						string guid = AssetDatabase.AssetPathToGUID( deletedAssets[ i ] );
						TemplateDataParent templateData = templatesManager.GetTemplate( guid );
						if( templateData != null )
						{
							// Close any window using that template
							int windowCount = IOUtils.AllOpenedWindows.Count;
							for( int windowIdx = 0; windowIdx < windowCount; windowIdx++ )
							{
								TemplateMasterNode masterNode = IOUtils.AllOpenedWindows[ windowIdx ].CurrentGraph.CurrentMasterNode as TemplateMasterNode;
								if( masterNode != null && masterNode.CurrentTemplate.GUID.Equals( templateData.GUID ) )
								{
									IOUtils.AllOpenedWindows[ windowIdx ].Close();
								}
							}

							templatesManager.RemoveTemplate( templateData );
							refreshMenuItems = true;
						}
					}
				}
			}

			//for ( int i = 0; i < movedAssets.Length; i++ )
			//{
			//	if ( TemplateHelperFunctions.CheckIfTemplate( movedAssets[ i ] ) )
			//	{
			//		refreshMenuItems = true;
			//		break;
			//	}
			//}

			//for ( int i = 0; i < movedFromAssetPaths.Length; i++ )
			//{
			//	if ( TemplateHelperFunctions.CheckIfTemplate( movedFromAssetPaths[ i ] ) )
			//	{
			//		refreshMenuItems = true;
			//		break;
			//	}
			//}

			if( refreshMenuItems )
			{
				//UnityEngine.Debug.Log( "Refresh Menu Items" );
				refreshMenuItems = false;
				templatesManager.CreateTemplateMenuItems();

				int windowCount = IOUtils.AllOpenedWindows.Count;
				for( int windowIdx = 0; windowIdx < windowCount; windowIdx++ )
				{
					IOUtils.AllOpenedWindows[ windowIdx ].CurrentGraph.ForceCategoryRefresh();
				}
			}
		}
	}
}
