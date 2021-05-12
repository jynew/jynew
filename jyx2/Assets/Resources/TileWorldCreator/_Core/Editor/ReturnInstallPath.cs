using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

namespace TileWorld
{
    public class ReturnInstallPath : MonoBehaviour
    {

        //return install path
        public static string GetInstallPath(string _currentFolder, ScriptableObject _object)
        {
            string _scriptFilePath = "";
            string _scriptFolder = "";

            MonoScript ms = MonoScript.FromScriptableObject(_object);
            _scriptFilePath = AssetDatabase.GetAssetPath(ms);

            FileInfo fi = new FileInfo(_scriptFilePath);

            _scriptFolder = fi.DirectoryName;
            _scriptFolder = _scriptFolder.Replace(Path.DirectorySeparatorChar, '/'); //Path.DirectorySeparatorChar

            string _subString = Application.dataPath;
            _subString = _subString.Substring(0, _subString.Length - 6);
            _scriptFolder = _scriptFolder.Replace(_subString, "");

            //replace inspectors folder
            _scriptFolder = _scriptFolder.Replace(_currentFolder, "");

            return _scriptFolder;
        }
    }
}
