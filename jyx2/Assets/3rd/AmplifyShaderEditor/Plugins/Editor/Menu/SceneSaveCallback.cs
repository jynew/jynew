// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	// Catch when scene is saved (Ctr+S) and also save ase shader
	public class SceneSaveCallback : UnityEditor.AssetModificationProcessor
	{
		private const string UnityStr = ".unity";

		static string[] OnWillSaveAssets( string[] paths )
		{
			bool canSave = false;

			if ( paths.Length == 0 )
			{
				canSave = true;
			}
			else
			{
				for ( int i = 0; i < paths.Length; i++ )
				{
					// Only save shader when saving scenes
					if ( paths[ i ].Contains( UnityStr ) )
					{
						canSave = true;
						break;
					}
				}
			}
			if ( canSave && UIUtils.CurrentWindow )
				UIUtils.CurrentWindow.SetCtrlSCallback( false );

			return paths;
		}
	}
}
