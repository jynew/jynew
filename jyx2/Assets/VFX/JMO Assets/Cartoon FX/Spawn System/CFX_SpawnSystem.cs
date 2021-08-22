using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Spawn System:
// Preload GameObject to reuse them later, avoiding to Instantiate them.
// Very useful for mobile platforms.

public class CFX_SpawnSystem : MonoBehaviour
{
	/// <summary>
	/// Get the next available preloaded Object.
	/// </summary>
	/// <returns>
	/// The next available preloaded Object.
	/// </returns>
	/// <param name='sourceObj'>
	/// The source Object from which to get a preloaded copy.
	/// </param>
	/// <param name='activateObject'>
	/// Activates the object before returning it.
	/// </param>
	static public GameObject GetNextObject(GameObject sourceObj, bool activateObject = true)
	{
		int uniqueId = sourceObj.GetInstanceID();
		
		if(!instance.poolCursors.ContainsKey(uniqueId))
		{
			Debug.LogError("[CFX_SpawnSystem.GetNextPoolObject()] Object hasn't been preloaded: " + sourceObj.name + " (ID:" + uniqueId + ")");
			return null;
		}
		
		int cursor = instance.poolCursors[uniqueId];
		instance.poolCursors[uniqueId]++;
		if(instance.poolCursors[uniqueId] >= instance.instantiatedObjects[uniqueId].Count)
		{
			instance.poolCursors[uniqueId] = 0;
		}
		
		GameObject returnObj = instance.instantiatedObjects[uniqueId][cursor];
		if(activateObject)
			#if UNITY_3_5
					returnObj.SetActiveRecursively(true);
			#else
					returnObj.SetActive(true);
			#endif
		
		return returnObj;
	}
	
	/// <summary>
	/// Preloads an object a number of times in the pool.
	/// </summary>
	/// <param name='sourceObj'>
	/// The source Object.
	/// </param>
	/// <param name='poolSize'>
	/// The number of times it will be instantiated in the pool (i.e. the max number of same object that would appear simultaneously in your Scene).
	/// </param>
	static public void PreloadObject(GameObject sourceObj, int poolSize = 1)
	{
		instance.addObjectToPool(sourceObj, poolSize);
	}
	
	/// <summary>
	/// Unloads all the preloaded objects from a source Object.
	/// </summary>
	/// <param name='sourceObj'>
	/// Source object.
	/// </param>
	static public void UnloadObjects(GameObject sourceObj)
	{
		instance.removeObjectsFromPool(sourceObj);
	}
	
	/// <summary>
	/// Gets a value indicating whether all objects defined in the Editor are loaded or not.
	/// </summary>
	/// <value>
	/// <c>true</c> if all objects are loaded; otherwise, <c>false</c>.
	/// </value>
	static public bool AllObjectsLoaded
	{
		get
		{
			return instance.allObjectsLoaded;
		}
	}
	
	// INTERNAL SYSTEM ----------------------------------------------------------------------------------------------------------------------------------------
	
	static private CFX_SpawnSystem instance;
	
	public GameObject[] objectsToPreload = new GameObject[0];
	public int[] objectsToPreloadTimes = new int[0];
	public bool hideObjectsInHierarchy;
	
	private bool allObjectsLoaded;
	private Dictionary<int,List<GameObject>> instantiatedObjects = new Dictionary<int, List<GameObject>>();
	private Dictionary<int,int> poolCursors = new Dictionary<int, int>();
	
	private void addObjectToPool(GameObject sourceObject, int number)
	{
		int uniqueId = sourceObject.GetInstanceID();
				
		//Add new entry if it doesn't exist
		if(!instantiatedObjects.ContainsKey(uniqueId))
		{
			instantiatedObjects.Add(uniqueId, new List<GameObject>());
			poolCursors.Add(uniqueId, 0);
		}
		
		//Add the new objects
		GameObject newObj;
		for(int i = 0; i < number; i++)
		{
			newObj = (GameObject)Instantiate(sourceObject);
			#if UNITY_3_5
				newObj.SetActiveRecursively(false);
			#else
				newObj.SetActive(false);
			#endif
			
			//Set flag to not destruct object
			CFX_AutoDestructShuriken[] autoDestruct = newObj.GetComponentsInChildren<CFX_AutoDestructShuriken>(true);
			foreach(CFX_AutoDestructShuriken ad in autoDestruct)
			{
				ad.OnlyDeactivate = true;
			}
			//Set flag to not destruct light
			CFX_LightIntensityFade[] lightIntensity = newObj.GetComponentsInChildren<CFX_LightIntensityFade>(true);
			foreach(CFX_LightIntensityFade li in lightIntensity)
			{
				li.autodestruct = false;
			}
			
			instantiatedObjects[uniqueId].Add(newObj);
			
			if(hideObjectsInHierarchy)
				newObj.hideFlags = HideFlags.HideInHierarchy;
		}
	}
	
	private void removeObjectsFromPool(GameObject sourceObject)
	{
		int uniqueId = sourceObject.GetInstanceID();
		
		if(!instantiatedObjects.ContainsKey(uniqueId))
		{
			Debug.LogWarning("[CFX_SpawnSystem.removeObjectsFromPool()] There aren't any preloaded object for: " + sourceObject.name + " (ID:" + uniqueId + ")");
			return;
		}
		
		//Destroy all objects
		for(int i = instantiatedObjects[uniqueId].Count - 1; i >= 0; i--)
		{
			GameObject obj = instantiatedObjects[uniqueId][i];
			instantiatedObjects[uniqueId].RemoveAt(i);
			GameObject.Destroy(obj);
		}
		
		//Remove pool entry
		instantiatedObjects.Remove(uniqueId);
		poolCursors.Remove(uniqueId);
	}
	
	void Awake()
	{
		if(instance != null)
			Debug.LogWarning("CFX_SpawnSystem: There should only be one instance of CFX_SpawnSystem per Scene!");
		
		instance = this;
	}
	
	void Start()
	{
		allObjectsLoaded = false;
		
		for(int i = 0; i < objectsToPreload.Length; i++)
		{
			PreloadObject(objectsToPreload[i], objectsToPreloadTimes[i]);
		}
		
		allObjectsLoaded = true;
	}
}
