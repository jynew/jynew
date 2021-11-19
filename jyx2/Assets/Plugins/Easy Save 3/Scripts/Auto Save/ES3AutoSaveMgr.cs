using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ES3AutoSaveMgr : MonoBehaviour
{
	public static ES3AutoSaveMgr _current = null;
    public static ES3AutoSaveMgr Current
    {
        get
        {
            if (_current == null /*|| _current.gameObject.scene != SceneManager.GetActiveScene()*/)
            {
                var scene = SceneManager.GetActiveScene();
                var roots = scene.GetRootGameObjects();

                // First, look for Easy Save 3 Manager in the top-level.
                foreach (var root in roots)
                    if (root.name == "Easy Save 3 Manager")
                        return _current = root.GetComponent<ES3AutoSaveMgr>();

                // If the user has moved or renamed the Easy Save 3 Manager, we need to perform a deep search.
                foreach (var root in roots)
                    if ((_current = root.GetComponentInChildren<ES3AutoSaveMgr>()) != null)
                        return _current;
            }
            return _current;
        }
    }

    // Included for backwards compatibility.
    public static ES3AutoSaveMgr Instance
    {
        get { return Current; }
    }

    public enum LoadEvent { None, Awake, Start }
	public enum SaveEvent { None, OnApplicationQuit, OnApplicationPause }

	public string key = System.Guid.NewGuid().ToString();
	public SaveEvent saveEvent = SaveEvent.OnApplicationQuit;
	public LoadEvent loadEvent = LoadEvent.Awake;
	public ES3SerializableSettings settings = new ES3SerializableSettings("AutoSave.es3", ES3.Location.Cache);

	public HashSet<ES3AutoSave> autoSaves = new HashSet<ES3AutoSave>();

	public void Save()
	{
        if (autoSaves == null || autoSaves.Count == 0)
            return;

        // If we're using caching and we've not already cached this file, cache it.
        if (settings.location == ES3.Location.Cache && !ES3.FileExists(settings))
            ES3.CacheFile(settings);

        if (autoSaves == null || autoSaves.Count == 0)
        {
            ES3.DeleteKey(key, settings);
        }
        else
        {
            var gameObjects = new List<GameObject>();
            foreach (var autoSave in autoSaves)
                // If the ES3AutoSave component is disabled, don't save it.
                if (autoSave.enabled)
                    gameObjects.Add(autoSave.gameObject);
            ES3.Save<GameObject[]>(key, gameObjects.ToArray(), settings);
        }

        if(settings.location == ES3.Location.Cache && ES3.FileExists(settings))
            ES3.StoreCachedFile(settings);
	}

	public void Load()
	{
        try
        {
            // If we're using caching and we've not already cached this file, cache it.
            if (settings.location == ES3.Location.Cache && !ES3.FileExists(settings))
                ES3.CacheFile(settings);
        }
        catch { }

        ES3.Load<GameObject[]>(key, new GameObject[0], settings);
	}

	void Start()
	{
		if(loadEvent == LoadEvent.Start)
			Load();
	}

    public void Awake()
    {
        autoSaves = new HashSet<ES3AutoSave>();

        foreach (var go in this.gameObject.scene.GetRootGameObjects())
            autoSaves.UnionWith(go.GetComponentsInChildren<ES3AutoSave>(true));

        _current = this;

        if (loadEvent == LoadEvent.Awake)
            Load();
    }

    void OnApplicationQuit()
	{
		if(saveEvent == SaveEvent.OnApplicationQuit)
			Save();
	}

	void OnApplicationPause(bool paused)
	{
		if(	(saveEvent == SaveEvent.OnApplicationPause || 
			(Application.isMobilePlatform && saveEvent == SaveEvent.OnApplicationQuit)) && paused)
			Save();
	}

	/* Register an ES3AutoSave with the ES3AutoSaveMgr, if there is one */
	public static void AddAutoSave(ES3AutoSave autoSave)
	{
		if(ES3AutoSaveMgr.Current != null)
			ES3AutoSaveMgr.Current.autoSaves.Add(autoSave);
	}

	/* Remove an ES3AutoSave from the ES3AutoSaveMgr, for example if it's GameObject has been destroyed */
	public static void RemoveAutoSave(ES3AutoSave autoSave)
	{
		if(ES3AutoSaveMgr.Current != null)
			ES3AutoSaveMgr.Current.autoSaves.Remove(autoSave);
	}
}
