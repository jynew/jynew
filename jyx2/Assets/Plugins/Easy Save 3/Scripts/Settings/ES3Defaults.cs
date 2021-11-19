using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES3Defaults : ScriptableObject
{
    [SerializeField]
    public ES3SerializableSettings settings = new ES3SerializableSettings();

    public bool addMgrToSceneAutomatically = false;
    public bool autoUpdateReferences = true;
    public bool addAllPrefabsToManager = true;

    public bool logDebugInfo = false;
    public bool logWarnings = true;
    public bool logErrors = true;
}