using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using ES3Internal;

namespace ES3Editor
{
	public static class ES3SettingsEditor
	{
		public static void Draw(ES3SerializableSettings settings)
		{
			var style = EditorStyle.Get;

			settings.location = (ES3.Location)EditorGUILayout.EnumPopup("Location", settings.location);
			// If the location is File, show the Directory.
			if(settings.location == ES3.Location.File)
				settings.directory = (ES3.Directory)EditorGUILayout.EnumPopup("Directory", settings.directory);

			settings.path = EditorGUILayout.TextField("Default File Path", settings.path);

			EditorGUILayout.Space();

			settings.encryptionType = (ES3.EncryptionType)EditorGUILayout.EnumPopup("Encryption", settings.encryptionType);
			settings.encryptionPassword = EditorGUILayout.TextField("Encryption Password", settings.encryptionPassword);

            EditorGUILayout.Space();

            settings.compressionType = (ES3.CompressionType)EditorGUILayout.EnumPopup("Compression", settings.compressionType);

            EditorGUILayout.Space();
			
			settings.saveChildren = EditorGUILayout.Toggle("Save GameObject Children", settings.saveChildren);
			
			EditorGUILayout.Space();

			if(settings.showAdvancedSettings = EditorGUILayout.Foldout(settings.showAdvancedSettings, "Advanced Settings"))
			{
				EditorGUILayout.BeginVertical(style.area);

				settings.format = (ES3.Format)EditorGUILayout.EnumPopup("Format", settings.format);
                if (settings.format == ES3.Format.JSON)
                    settings.prettyPrint = EditorGUILayout.Toggle(new GUIContent("Pretty print JSON"), settings.prettyPrint);
				settings.bufferSize = EditorGUILayout.IntField("Buffer Size", settings.bufferSize);
				settings.memberReferenceMode = (ES3.ReferenceMode)EditorGUILayout.EnumPopup("Serialise Unity Object fields", settings.memberReferenceMode);
                settings.serializationDepthLimit = EditorGUILayout.IntField("Serialisation Depth", settings.serializationDepthLimit);

                EditorGUILayout.Space();

				EditorGUILayout.EndVertical();
			}
		}
    }
}
