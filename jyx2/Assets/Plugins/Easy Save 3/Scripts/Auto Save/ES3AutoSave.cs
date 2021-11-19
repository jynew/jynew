using UnityEngine;
using System.Collections.Generic;

public class ES3AutoSave : MonoBehaviour, ISerializationCallbackReceiver
{
    public bool saveLayer = true;
    public bool saveTag = true;
    public bool saveName = true;
    public bool saveHideFlags = true;
    public bool saveActive = true;
    public bool saveChildren = false;

    private bool isQuitting = false;

    //[HideInInspector]
    public List<Component> componentsToSave = new List<Component>();

    public void Reset()
    {
        // Initialise saveLayer (etc) to false for all new Components.
        saveLayer = false;
        saveTag = false;
        saveName = false;
        saveHideFlags = false;
        saveActive = false;
        saveChildren = false;
    }

    public void Awake()
    {
        if (ES3AutoSaveMgr.Current == null)
            ES3Internal.ES3Debug.LogWarning("<b>No GameObjects in this scene will be autosaved</b> because there is no Easy Save 3 Manager. To add a manager to this scene, exit playmode and go to Assets > Easy Save 3 > Add Manager to Scene.", this);
        else
            ES3AutoSaveMgr.AddAutoSave(this);
    }

    public void OnApplicationQuit()
    {
        isQuitting = true;
    }

    public void OnDestroy()
    {
        // If this is being destroyed, but not because the application is quitting,
        // remove the AutoSave from the manager.
        if (!isQuitting)
            ES3AutoSaveMgr.RemoveAutoSave(this);
    }
    public void OnBeforeSerialize() { }
    public void OnAfterDeserialize()
    {
        // Remove any null Components
        componentsToSave.RemoveAll(c => c == null || c.GetType() == typeof(Component));
    }
}