#if PLAYMAKER_1_8_OR_NEWER

using UnityEngine;
using UnityEditor;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMakerEditor;
using System.Text.RegularExpressions;

namespace ES3PlayMaker
{
    #region Base Actions

    public abstract class BaseEditor : CustomActionEditor
    {
        bool showErrorHandling = false;

        public abstract void DrawGUI();

        public override bool OnGUI()
        {
            DrawGUI();

            EditorGUILayout.Separator();

            showErrorHandling = EditorGUILayout.Foldout(showErrorHandling, "Error Handling");
            if (showErrorHandling)
            {
                EditorGUI.indentLevel++;
                EditField("errorEvent");
                EditField("errorMessage");
                EditorGUI.indentLevel--;
            }

            return GUI.changed;
        }

        // Displays the FsmVar field without the unnecessary Type field.
        protected void FsmVarField(string fieldName)
        {
            if (target == null || target.State == null)
                return;

            var fsmVar = (FsmVar)ES3Internal.ES3Reflection.GetField(target.GetType(), fieldName).GetValue(target);

            if (fsmVar == null)
            {
                fsmVar = new FsmVar();
                ES3Internal.ES3Reflection.GetField(target.GetType(), fieldName).SetValue(target, fsmVar);
            }

            EditorGUILayout.BeginHorizontal();
            var label = Regex.Replace(fieldName, @"\p{Lu}", m => " " + m.Value.ToLowerInvariant());
            EditorGUILayout.PrefixLabel(char.ToUpperInvariant(label[0]) + label.Substring(1));

            var localVariables = target.Fsm.Variables.GetAllNamedVariablesSorted();
            var globalVariables = FsmVariables.GlobalVariables.GetAllNamedVariablesSorted();

            var variableNames = new string[localVariables.Length + globalVariables.Length];
            int selected = -1;

            for(int i=0; i<variableNames.Length; i++)
            {
                var variable = i >= localVariables.Length ? globalVariables[i - localVariables.Length] : localVariables[i];
                variableNames[i] = i >= localVariables.Length ? "Globals/"+variable.Name : variable.Name;
                if (fsmVar.NamedVar == variable)
                    selected = i;
            }

            var newSelected = EditorGUILayout.Popup(selected, variableNames);


            EditorGUILayout.EndHorizontal();

            if (newSelected == -1)
                return;

            if (selected != newSelected)
            {
                if (newSelected >= localVariables.Length)
                    fsmVar.NamedVar = globalVariables[newSelected - localVariables.Length];
                else
                    fsmVar.NamedVar = localVariables[newSelected];
            }
        }
    }

	public abstract class SettingsEditor : BaseEditor
	{
		public override bool OnGUI()
		{
			base.OnGUI();

			var action = target as ES3PlayMaker.SettingsAction;
			if(action == null)
				return false;
			action.overrideDefaultSettings.Value = EditorGUILayout.ToggleLeft("Override Default Settings", action.overrideDefaultSettings.Value);

			if(action.overrideDefaultSettings.Value)
			{
				EditorGUI.indentLevel++;

                EditField("path");
                EditField("location");
				EditField("encryptionType");
				EditField("encryptionPassword");
                EditField("compressionType");
				EditField("directory");
				EditField("format");
				EditField("bufferSize");

				EditorGUI.indentLevel--;
				EditorGUILayout.Space();
			}
			return GUI.changed;
		}
	}

	public abstract class KeyValueSettingsEditor : SettingsEditor
	{
		public override bool OnGUI()
		{
			EditField("key");
			EditField("value");

			base.OnGUI();

			return GUI.changed;
		}

		public override void DrawGUI(){}
	}

    public abstract class ES3FileActionEditor : BaseEditor
    {
        public override bool OnGUI()
        {
            EditField("fsmES3File");

            base.OnGUI();

            var action = target as ES3PlayMaker.ES3FileAction;
            if (action == null)
                return false;

            return GUI.changed;
        }
    }
#endregion

#region Save Actions

    [CustomActionEditor(typeof(ES3PlayMaker.Save))]
	public class SaveEditor : KeyValueSettingsEditor{}

    [CustomActionEditor(typeof(ES3PlayMaker.SaveAll))]
    public class SaveAllEditor : SettingsEditor
    {
        public override void DrawGUI()
        {
            EditField("key");
            EditField("saveFsmVariables");
            EditField("saveGlobalVariables");
        }
    }

    [CustomActionEditor(typeof(ES3PlayMaker.SaveRaw))]
	public class SaveRawEditor : SettingsEditor
	{
		public override void DrawGUI()
		{
			EditField("str");
			EditField("useBase64Encoding");
			EditField("appendNewline");
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.AppendRaw))]
	public class AppendRawEditor : SettingsEditor
	{
		public override void DrawGUI()
		{
			EditField("str");
			EditField("useBase64Encoding");
			EditField("appendNewline");
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.SaveImage))]
	public class SaveImageEditor : SettingsEditor
	{
		public override void DrawGUI()
		{
			EditField("imagePath");
			EditField("texture2D");
            EditField("quality");
		}
	}

#endregion

#region Load Actions

	[CustomActionEditor(typeof(ES3PlayMaker.Load))]
	public class LoadEditor : KeyValueSettingsEditor
	{
		public override void DrawGUI()
		{
			EditorGUILayout.Space();
			EditField("defaultValue");
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.LoadInto))]
	public class LoadIntoEditor : KeyValueSettingsEditor{}

    [CustomActionEditor(typeof(ES3PlayMaker.LoadAll))]
    public class LoadAllEditor : SettingsEditor
    {
        public override void DrawGUI()
        {
            EditField("key");
            EditField("loadFsmVariables");
            EditField("loadGlobalVariables");
        }
    }

    [CustomActionEditor(typeof(ES3PlayMaker.LoadAudio))]
	public class LoadAudioEditor : SettingsEditor
	{
		public override void DrawGUI()
		{
			EditField("audioFilePath");
			EditField("audioClip");
#if UNITY_2018_3_OR_NEWER
            EditField("audioType");
#endif
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.LoadImage))]
	public class LoadImageEditor : SettingsEditor
	{
		public override void DrawGUI()
		{
			EditField("imagePath");
			EditField("texture2D");
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.LoadRawString))]
	public class LoadRawStringEditor : SettingsEditor
	{
		public override void DrawGUI()
		{
			EditField("str");
			EditField("useBase64Encoding");
		}
	}

#endregion

#region Exists Actions

	[CustomActionEditor(typeof(ES3PlayMaker.KeyExists))]
	public class KeyExistsEditor : SettingsEditor
	{
		public override void DrawGUI()
		{
			EditField("key");
			EditField("exists");
			EditorGUILayout.Separator();
			EditField("existsEvent");
			EditField("doesNotExistEvent");
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.FileExists))]
	public class FileExistsEditor : SettingsEditor
	{
		public override void DrawGUI()
		{
			EditField("filePath");
			EditField("exists");
			EditorGUILayout.Separator();
			EditField("existsEvent");
			EditField("doesNotExistEvent");
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.DirectoryExists))]
	public class DirectoryExistsEditor : SettingsEditor
	{
		public override void DrawGUI()
		{
			EditField("directoryPath");
			EditField("exists");
			EditorGUILayout.Separator();
			EditField("existsEvent");
			EditField("doesNotExistEvent");
		}
	}

#endregion

#region Delete Actions

	[CustomActionEditor(typeof(ES3PlayMaker.DeleteKey))]
	public class DeleteKeyEditor : SettingsEditor
	{
		public override void DrawGUI()
		{
			EditField("key");
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.DeleteFile))]
	public class DeleteFileEditor : SettingsEditor
	{
		public override void DrawGUI()
		{
			EditField("filePath");
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.DeleteDirectory))]
	public class DeleteDirectoryEditor : SettingsEditor
	{
		public override void DrawGUI()
		{
			EditField("directoryPath");
		}
	}

#endregion

#region Backup Actions

	[CustomActionEditor(typeof(ES3PlayMaker.CreateBackup))]
	public class CreateBackupEditor : SettingsEditor
	{
		public override void DrawGUI()
		{
			EditField("filePath");
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.RestoreBackup))]
	public class RestoreBackupEditor : SettingsEditor
	{
		public override void DrawGUI()
		{
			EditField("filePath");
			EditField("backupWasRestored");
		}
	}

#endregion

#region Key, File and Directory methods

	[CustomActionEditor(typeof(ES3PlayMaker.RenameFile))]
	public class RenameFileEditor : SettingsEditor
	{
		public override void DrawGUI()
		{
			EditField("oldFilePath");
			EditField("newFilePath");
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.CopyFile))]
	public class CopyFileEditor : SettingsEditor
	{
		public override void DrawGUI()
		{
			EditField("oldFilePath");
			EditField("newFilePath");
		}
	}
	
	[CustomActionEditor(typeof(ES3PlayMaker.CopyDirectory))]
	public class CopyDirectoryEditor : SettingsEditor
	{
		public override void DrawGUI()
		{
			EditField("oldDirectoryPath");
			EditField("newDirectoryPath");
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.GetKeys))]
	public class GetKeysEditor : SettingsEditor
	{
		public override void DrawGUI()
		{
			EditField("filePath");
			EditField("keys");
		}
	}

    [CustomActionEditor(typeof(ES3PlayMaker.GetKeyCount))]
    public class GetKeyCountEditor : SettingsEditor
    {
        public override void DrawGUI()
        {
            EditField("filePath");
            EditField("keyCount");
        }
    }

    [CustomActionEditor(typeof(ES3PlayMaker.GetFiles))]
	public class GetFilesEditor : SettingsEditor
	{
		public override void DrawGUI()
		{
			EditField("directoryPath");
			EditField("files");
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.GetDirectories))]
	public class GetDirectoriesEditor : SettingsEditor
	{
		public override void DrawGUI()
		{
			EditField("directoryPath");
			EditField("directories");
		}
	}

#endregion

#region ES3File Actions

	[CustomActionEditor(typeof(ES3PlayMaker.ES3FileCreate))]
	public class ES3FileCreateEditor : SettingsEditor
	{
		public override void DrawGUI()
		{
			EditField("fsmES3File");
			EditField("filePath");
			EditField("syncWithFile");
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.ES3FileSync))]
	public class ES3FileSyncEditor : SettingsEditor
	{
		public override void DrawGUI()
		{
			EditField("fsmES3File");
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.ES3FileSave))]
	public class ES3FileSaveEditor : SaveEditor
	{
		public override void DrawGUI()
		{
			EditField("fsmES3File");
			base.DrawGUI();
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.ES3FileLoad))]
	public class ES3FileLoadEditor : LoadEditor
	{
		public override void DrawGUI()
		{
			EditField("fsmES3File");
			base.DrawGUI();
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.ES3FileLoadInto))]
	public class ES3FileLoadIntoEditor : LoadIntoEditor
	{
		public override void DrawGUI()
		{
			base.DrawGUI();
			EditField("fsmES3File");
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.ES3FileDeleteKey))]
	public class ES3FileDeleteKeyEditor : DeleteKeyEditor
	{
		public override void DrawGUI()
		{
			base.DrawGUI();
			EditField("fsmES3File");
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.ES3FileKeyExists))]
	public class ES3FileKeyExistsEditor : KeyExistsEditor
	{
		public override void DrawGUI()
		{
			EditField("fsmES3File");
			base.DrawGUI();
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.ES3FileGetKeys))]
	public class ES3FileGetKeysEditor : ES3FileActionEditor
	{
		public override void DrawGUI()
		{
			EditField("keys");
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.ES3FileClear))]
	public class ES3FileClearEditor : BaseEditor
	{
		public override void DrawGUI()
		{
			EditField("fsmES3File");
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.ES3FileSize))]
	public class ES3FileSizeEditor : BaseEditor
	{
		public override void DrawGUI()
		{
			EditField("size");
			EditField("fsmES3File");
		}
	}


#endregion

#region ES3Cloud Actions
#if !DISABLE_WEB
	
	public abstract class ES3CloudEditor : SettingsEditor
	{
		protected abstract void DrawChildGUI();

		public override void DrawGUI()
		{
			EditField("url");
			EditField("apiKey");
			EditorGUILayout.Space();
			DrawChildGUI();
			EditorGUILayout.Space();
			EditField("errorCode");
		}
	}

	public abstract class ES3CloudUserEditor : ES3CloudEditor
	{
		public bool showUser = false;

		protected override void DrawChildGUI()
		{
			if((showUser = EditorGUILayout.Foldout(showUser, "User (optional)")))
			{
				EditorGUI.indentLevel++;
				EditField("user");
				EditField("password");
				EditorGUI.indentLevel--;
			}
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.ES3CloudSync))]
	public class ES3CloudSyncEditor : ES3CloudUserEditor
	{
		protected override void DrawChildGUI()
		{
			EditField("path");
			base.DrawChildGUI();
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.ES3CloudDownloadFile))]
	public class ES3CloudDownloadFileEditor : ES3CloudUserEditor
	{
		protected override void DrawChildGUI()
		{
			EditField("path");
			base.DrawChildGUI();
		}
	}

    [CustomActionEditor(typeof(ES3PlayMaker.ES3CloudDownloadES3File))]
    public class ES3CloudDownloadES3FileEditor : BaseEditor
    {
        public bool showUser = false;
        public override void DrawGUI()
        {
            EditField("fsmES3File");
            EditField("url");
            EditField("apiKey");
            EditField("errorCode");
            if ((showUser = EditorGUILayout.Foldout(showUser, "User (optional)")))
            {
                EditorGUI.indentLevel++;
                EditField("user");
                EditField("password");
                EditorGUI.indentLevel--;
            }
        }
    }


    [CustomActionEditor(typeof(ES3PlayMaker.ES3CloudUploadFile))]
	public class ES3CloudUploadFileEditor : ES3CloudUserEditor
	{
		protected override void DrawChildGUI()
		{
			EditField("path");
			base.DrawChildGUI();
		}
	}

    [CustomActionEditor(typeof(ES3PlayMaker.ES3CloudUploadES3File))]
    public class ES3CloudUploadES3FileEditor : BaseEditor
    {
        public bool showUser = false;
        public override void DrawGUI()
        {
            EditField("fsmES3File");
            EditField("url");
            EditField("apiKey");
            EditField("errorCode");
            if((showUser = EditorGUILayout.Foldout(showUser, "User (optional)")))
			{
				EditorGUI.indentLevel++;
				EditField("user");
				EditField("password");
				EditorGUI.indentLevel--;
			}
        }
    }



    [CustomActionEditor(typeof(ES3PlayMaker.ES3CloudDeleteFile))]
	public class ES3CloudDeleteFileEditor : ES3CloudUserEditor
	{
		protected override void DrawChildGUI()
		{
			EditField("path");
			base.DrawChildGUI();
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.ES3CloudRenameFile))]
	public class ES3CloudRenameFileEditor : ES3CloudUserEditor
	{
		protected override void DrawChildGUI()
		{
			EditField("path");
			EditField("newFilename");
			base.DrawChildGUI();
		}
	}

    [CustomActionEditor(typeof(ES3PlayMaker.ES3CloudDownloadFilenames))]
	public class ES3CloudDownloadFilenamesEditor : ES3CloudUserEditor
	{
		protected override void DrawChildGUI()
		{
			EditField("filenames");
            EditField("searchPattern");
            base.DrawChildGUI();
		}
	}

    [CustomActionEditor(typeof(ES3PlayMaker.ES3CloudSearchFilenames))]
    public class ES3CloudSearchFilenamesEditor : ES3CloudUserEditor
    {
        protected override void DrawChildGUI()
        {
            EditField("filenames");
            EditField("searchPattern");
            base.DrawChildGUI();
        }
    }

    [CustomActionEditor(typeof(ES3PlayMaker.ES3CloudDownloadTimestamp))]
	public class ES3CloudDownloadTimestampEditor : ES3CloudUserEditor
	{
		protected override void DrawChildGUI()
		{
			EditField("timestamp");
			base.DrawChildGUI();
		}
	}

#endif

#endregion

#region ES3SpreadsheetActions


	[CustomActionEditor(typeof(ES3PlayMaker.ES3SpreadsheetCreate))]
	public class ES3SpreadsheetCreateEditor : BaseEditor
	{
		public override void DrawGUI()
		{
			EditField("fsmES3Spreadsheet");
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.ES3SpreadsheetSetCell))]
	public class ES3SpreadsheetSetCellEditor : BaseEditor
	{
		public override void DrawGUI()
		{
			EditField("fsmES3Spreadsheet");
			EditField("col");
			EditField("row");
            EditField("value");
        }
	}

	[CustomActionEditor(typeof(ES3PlayMaker.ES3SpreadsheetGetCell))]
	public class ES3SpreadsheetGetCellEditor : BaseEditor
	{
		public override void DrawGUI()
		{
			EditField("fsmES3Spreadsheet");
			EditField("col");
			EditField("row");
			EditField("value");
		}
	}

	[CustomActionEditor(typeof(ES3PlayMaker.ES3SpreadsheetLoad))]
	public class ES3SpreadsheetLoadEditor : SettingsEditor
	{
		public override void DrawGUI()
		{
            EditField("fsmES3Spreadsheet");
            EditField("filePath");
		}
	}

    [CustomActionEditor(typeof(ES3PlayMaker.ES3SpreadsheetSave))]
    public class ES3SpreadsheetSaveEditor : SettingsEditor
    {
        public override void DrawGUI()
        {
            EditField("fsmES3Spreadsheet");
            EditField("filePath");
            EditField("append");
        }
    }

    #endregion

    #region Caching

    [CustomActionEditor(typeof(ES3PlayMaker.CacheFile))]
    public class CacheFileEditor : SettingsEditor
    {
        public override void DrawGUI()
        {
            EditField("filePath");
        }
    }

    [CustomActionEditor(typeof(ES3PlayMaker.StoreCachedFile))]
    public class StoreCachedFileEditor : SettingsEditor
    {
        public override void DrawGUI()
        {
            EditField("filePath");
        }
    }

    #endregion
}
#endif
