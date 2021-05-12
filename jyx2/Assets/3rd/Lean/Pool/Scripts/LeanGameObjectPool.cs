
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Lean.Common;

namespace Lean.Pool
{
	/// <summary>This component allows you to pool GameObjects, giving you a very fast alternative to Instantiate and Destroy.
	/// Pools also have settings to preload, recycle, and set the spawn capacity, giving you lots of control over your spawning.</summary>
	[ExecuteInEditMode]
	[HelpURL(LeanPool.HelpUrlPrefix + "LeanGameObjectPool")]
	[AddComponentMenu(LeanPool.ComponentPathPrefix + "GameObject Pool")]
	public class LeanGameObjectPool : MonoBehaviour, ISerializationCallbackReceiver
	{
		[System.Serializable]
		public class Delay
		{
			public GameObject Clone;
			public float      Life;
		}

		public enum NotificationType
		{
			None,
			SendMessage,
			BroadcastMessage,
			IPoolable,
			BroadcastIPoolable
		}

		public enum StrategyType
		{
			ActivateAndDeactivate,
			DeactivateViaHierarchy
		}

		/// <summary>All active and enabled pools in the scene.</summary>
		public static LinkedList<LeanGameObjectPool> Instances = new LinkedList<LeanGameObjectPool>();

		/// <summary>The prefab this pool controls.</summary>
		public GameObject Prefab
		{
			set
			{
				if (value != prefab)
				{
					UnregisterPrefab();

					prefab = value;

					RegisterPrefab();
				}
			}

			get
			{
				return prefab;
			}
		}

		[SerializeField] [UnityEngine.Serialization.FormerlySerializedAs("Prefab")] private GameObject prefab;

		/// <summary>If you need to peform a special action when a prefab is spawned or despawned, then this allows you to control how that action is performed.
		/// <tip>None</tip>If you use this then you must rely on the OnEnable and OnDisable messages.
		/// <tip>SendMessage</tip>The prefab clone is sent the OnSpawn and OnDespawn messages.
		/// <tip>BroadcastMessage</tip>The prefab clone and all its children are sent the OnSpawn and OnDespawn messages.
		/// <tip>IPoolable</tip>The prefab clone's components implementing IPoolable are called.
		/// <tip>Broadcast IPoolable</tip>The prefab clone and all its child components implementing IPoolable are called.</summary>
		public NotificationType Notification = NotificationType.IPoolable;

		/// <summary>This allows you to control how spawned/despawned GameObjects will be handled. The <b>DeactivateViaHierarchy</b> mode should be used if you need to maintain your prefab's de/activation state.
		/// ActivateAndDeactivate = Despawned clones will be deactivated and placed under this GameObject.
		/// DeactivateViaHierarchy = Despawned clones will be placed under a deactivated GameObject and left alone.</summary>
		public StrategyType Strategy = StrategyType.ActivateAndDeactivate;

		/// <summary>Should this pool preload some clones?</summary>
		public int Preload;

		/// <summary>Should this pool have a maximum amount of spawnable clones?</summary>
		public int Capacity;

		/// <summary>If the pool reaches capacity, should new spawns force older ones to despawn?</summary>
		public bool Recycle;

		/// <summary>Should this pool be marked as DontDestroyOnLoad?</summary>
		public bool Persist;

		/// <summary>Should the spawned clones have their clone index appended to their name?</summary>
		public bool Stamp;

		/// <summary>Should detected issues be output to the console?</summary>
		public bool Warnings = true;

		/// <summary>This stores all spawned clones in a list. This is used when Recycle is enabled, because knowing the spawn order must be known. This list is also used during serialization.</summary>
		[SerializeField]
		private List<GameObject> spawnedClonesList = new List<GameObject>();

		/// <summary>This stores all spawned clones in a hash set. This is used when Recycle is disabled, because their storage order isn't important. This allows us to quickly find the Clone associated with the specified GameObject.</summary>
		private HashSet<GameObject> spawnedClonesHashSet = new HashSet<GameObject>();

		/// <summary>All the currently despawned prefab instances.</summary>
		[SerializeField]
		private List<GameObject> despawnedClones = new List<GameObject>();

		/// <summary>All the delayed destruction objects.</summary>
		[SerializeField]
		private List<Delay> delays = new List<Delay>();
		
		[SerializeField]
		private Transform deactivatedChild;

		/// <summary>Node within Instances.</summary>
		private LinkedListNode<LeanGameObjectPool> node;

		private static Dictionary<GameObject, LeanGameObjectPool> prefabMap = new Dictionary<GameObject, LeanGameObjectPool>();

		private static List<IPoolable> tempPoolables = new List<IPoolable>();

		/// <summary>If you're using the <b>Strategy = DeactivateViaHierarchy</b> mode, then all despawned clones will be placed under this.</summary>
		public Transform DeactivatedChild
		{
			get
			{
				if (deactivatedChild == null)
				{
					var child = new GameObject("Depawned Clones");

					child.SetActive(false);

					deactivatedChild = child.transform;

					deactivatedChild.SetParent(transform, false);
				}

				return deactivatedChild;
			}
		}

#if UNITY_EDITOR
		/// <summary>This will return false if you have pre-loaded prefabs do not match the <b>Prefab</b>.
		/// NOTE: This is only availible in the editor.</summary>
		public bool DespawnedClonesMatch
		{
			get
			{
				for (var i = despawnedClones.Count - 1; i >= 0; i--)
				{
					var clone = despawnedClones[i];

					if (clone != null && UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(clone) != prefab)
					{
						return false;
					}
				}

				return true;
			}
		}
#endif

		/// <summary>Find the pool responsible for handling the specified prefab.</summary>
		public static bool TryFindPoolByPrefab(GameObject prefab, ref LeanGameObjectPool foundPool)
		{
			return prefabMap.TryGetValue(prefab, out foundPool);
		}

		/// <summary>Find the pool responsible for handling the specified prefab clone.
		/// NOTE: This can be an expensive operation if you have many large pools.</summary>
		public static bool TryFindPoolByClone(GameObject clone, ref LeanGameObjectPool pool)
		{
			foreach (var instance in Instances)
			{
				// Search hash set
				if (instance.spawnedClonesHashSet.Contains(clone) == true)
				{
					pool = instance;

					return true;
				}

				// Search list
				for (var j = instance.spawnedClonesList.Count - 1; j >= 0; j--)
				{
					if (instance.spawnedClonesList[j] == clone)
					{
						pool = instance;

						return true;
					}
				}
			}

			return false;
		}

		/// <summary>Returns the amount of spawned clones.</summary>
		public int Spawned
		{
			get
			{
				return spawnedClonesList.Count + spawnedClonesHashSet.Count;
			}
		}

		/// <summary>Returns the amount of despawned clones.</summary>
		public int Despawned
		{
			get
			{
				return despawnedClones.Count;
			}
		}

		/// <summary>Returns the total amount of spawned and despawned clones.</summary>
		public int Total
		{
			get
			{
				return Spawned + Despawned;
			}
		}

		/// <summary>This will either spawn a previously despanwed/preloaded clone, recycle one, create a new one, or return null.
		/// NOTE: This method is designed to work with Unity's event system, so it has no return value.</summary>
		public void Spawn()
		{
			var clone = default(GameObject); TrySpawn(ref clone);
		}

		/// <summary>This will either spawn a previously despanwed/preloaded clone, recycle one, create a new one, or return null.
		/// NOTE: This method is designed to work with Unity's event system, so it has no return value.</summary>
		public void Spawn(Vector3 position)
		{
			var clone = default(GameObject); TrySpawn(ref clone, position, transform.localRotation);
		}

		/// <summary>This will either spawn a previously despanwed/preloaded clone, recycle one, create a new one, or return null.</summary>
		public GameObject Spawn(Transform parent, bool worldPositionStays = false)
		{
			var clone = default(GameObject); TrySpawn(ref clone, parent, worldPositionStays); return clone;
		}

		/// <summary>This will either spawn a previously despanwed/preloaded clone, recycle one, create a new one, or return null.</summary>
		public GameObject Spawn(Vector3 position, Quaternion rotation, Transform parent = null)
		{
			var clone = default(GameObject); TrySpawn(ref clone, position, rotation, parent); return clone;
		}

		/// <summary>This will either spawn a previously despanwed/preloaded clone, recycle one, create a new one, or return null.</summary>
		public bool TrySpawn(ref GameObject clone, Transform parent, bool worldPositionStays = false)
		{
			if (prefab == null) { if (Warnings == true) Debug.LogWarning("You're attempting to spawn from a pool with a null prefab", this); return false; }
			if (parent != null && worldPositionStays == true)
			{
				return TrySpawn(ref clone, prefab.transform.position, Quaternion.identity, Vector3.one, parent, worldPositionStays);
			}
			return TrySpawn(ref clone, transform.localPosition, transform.localRotation, transform.localScale, parent, worldPositionStays);
		}

		/// <summary>This will either spawn a previously despanwed/preloaded clone, recycle one, create a new one, or return null.</summary>
		public bool TrySpawn(ref GameObject clone, Vector3 position, Quaternion rotation, Transform parent = null)
		{
			if (prefab == null) { if (Warnings == true) Debug.LogWarning("You're attempting to spawn from a pool with a null prefab", this); return false; }
			if (parent != null)
			{
				position = parent.InverseTransformPoint(position);
				rotation = Quaternion.Inverse(parent.rotation) * rotation;
			}
			return TrySpawn(ref clone, position, rotation, prefab.transform.localScale, parent, false);
		}

		/// <summary>This will either spawn a previously despanwed/preloaded clone, recycle one, create a new one, or return null.</summary>
		public bool TrySpawn(ref GameObject clone)
		{
			if (prefab == null) { if (Warnings == true) Debug.LogWarning("You're attempting to spawn from a pool with a null prefab", this); return false; }
			var transform = prefab.transform;
			return TrySpawn(ref clone, transform.localPosition, transform.localRotation, transform.localScale, null, false);
		}

		/// <summary>This will either spawn a previously despanwed/preloaded clone, recycle one, create a new one, or return null.</summary>
		public bool TrySpawn(ref GameObject clone, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, Transform parent, bool worldPositionStays)
		{
			if (prefab != null)
			{
				// Spawn a previously despanwed/preloaded clone?
				for (var i = despawnedClones.Count - 1; i >= 0; i--)
				{
					clone = despawnedClones[i];

					despawnedClones.RemoveAt(i);

					if (clone != null)
					{
						SpawnClone(clone, localPosition, localRotation, localScale, parent, worldPositionStays);

						return true;
					}

					if (Warnings == true) Debug.LogWarning("This pool contained a null despawned clone, did you accidentally destroy it?", this);
				}

				// Make a new clone?
				if (Capacity <= 0 || Total < Capacity)
				{
					clone = CreateClone(localPosition, localRotation, localScale, parent, worldPositionStays);

					// Add clone to spawned list
					if (Recycle == true)
					{
						spawnedClonesList.Add(clone);
					}
					else
					{
						spawnedClonesHashSet.Add(clone);
					}

					// Activate?
					if (Strategy == StrategyType.ActivateAndDeactivate)
					{
						clone.SetActive(true);
					}

					// Notifications
					InvokeOnSpawn(clone);

					return true;
				}

				// Recycle?
				if (Recycle == true && TryDespawnOldest(ref clone, false) == true)
				{
					SpawnClone(clone, localPosition, localRotation, localScale, parent, worldPositionStays);

					return true;
				}
			}
			else
			{
				if (Warnings == true) Debug.LogWarning("You're attempting to spawn from a pool with a null prefab", this);
			}

			return false;
		}

		/// <summary>This will despawn the oldest prefab clone that is still spawned.</summary>
		[ContextMenu("Despawn Oldest")]
		public void DespawnOldest()
		{
			var clone = default(GameObject);

			TryDespawnOldest(ref clone, true);
		}

		private bool TryDespawnOldest(ref GameObject clone, bool registerDespawned)
		{
			MergeSpawnedClonesToList();

			// Loop through all spawnedClones from the front (oldest) until one is found
			while (spawnedClonesList.Count > 0)
			{
				clone = spawnedClonesList[0];

				spawnedClonesList.RemoveAt(0);

				if (clone != null)
				{
					DespawnNow(clone, registerDespawned);

					return true;
				}

				if (Warnings == true) Debug.LogWarning("This pool contained a null spawned clone, did you accidentally destroy it?", this);
			}

			return false;
		}

		/// <summary>This method will despawn all currently spawned prefabs managed by this pool.</summary>
		[ContextMenu("Despawn All")]
		public void DespawnAll()
		{
			// Merge
			MergeSpawnedClonesToList();

			// Despawn
			for (var i = spawnedClonesList.Count - 1; i >= 0; i--)
			{
				var clone = spawnedClonesList[i];

				if (clone != null)
				{
					DespawnNow(clone);
				}
			}

			spawnedClonesList.Clear();

			// Clear all delays
			for (var i = delays.Count - 1; i >= 0; i--)
			{
				LeanClassPool<Delay>.Despawn(delays[i]);
			}

			delays.Clear();
		}

		/// <summary>This will either instantly despawn the specified gameObject, or delay despawn it after t seconds.</summary>
		public void Despawn(GameObject clone, float t = 0.0f)
		{
			if (clone != null)
			{
				// Delay the despawn?
				if (t > 0.0f)
				{
					DespawnWithDelay(clone, t);
				}
				// Despawn now?
				else
				{
					TryDespawn(clone);

					// If this clone was marked for delayed despawn, remove it
					for (var i = delays.Count - 1; i >= 0; i--)
					{
						var delay = delays[i];

						if (delay.Clone == clone)
						{
							delays.RemoveAt(i);
						}
					}
				}
			}
			else
			{
				if (Warnings == true) Debug.LogWarning("You're attempting to despawn a null gameObject", this);
			}
		}

		/// <summary>This allows you to remove all references to the specified clone from this pool.
		/// A detached clone will act as a normal GameObject, requiring you to manually destroy or otherwise manage it.
		/// NOTE: If this clone has been despawned then it will still be parented to the pool.</summary>
		public void Detach(GameObject clone)
		{
			if (clone != null)
			{
				if (spawnedClonesHashSet.Remove(clone) == true || spawnedClonesList.Remove(clone) == true || despawnedClones.Remove(clone) == true)
				{
					// If this clone was marked for delayed despawn, remove it
					for (var i = delays.Count - 1; i >= 0; i--)
					{
						var delay = delays[i];

						if (delay.Clone == clone)
						{
							delays.RemoveAt(i);
						}
					}
				}
				else
				{
					if (Warnings == true) Debug.LogWarning("You're attempting to detach a GameObject that wasn't spawned from this pool.", clone);
				}
			}
			else
			{
				if (Warnings == true) Debug.LogWarning("You're attempting to detach a null GameObject", this);
			}
		}

		/// <summary>This method will create an additional prefab clone and add it to the despawned list.</summary>
		[ContextMenu("Preload One More")]
		public void PreloadOneMore()
		{
			if (prefab != null)
			{
				// Create clone
				var clone = CreateClone(Vector3.zero, Quaternion.identity, Vector3.one, null, false);

				// Add clone to despawned list
				despawnedClones.Add(clone);

				// Deactivate it
				if (Strategy == StrategyType.ActivateAndDeactivate)
				{
					clone.SetActive(false);

					clone.transform.SetParent(transform, false);
				}
				else
				{
					clone.transform.SetParent(DeactivatedChild, false);
				}

				if (Warnings == true && Capacity > 0 && Total > Capacity) Debug.LogWarning("You've preloaded more than the pool capacity, please verify you're preloading the intended amount.", this);
			}
			else
			{
				if (Warnings == true) Debug.LogWarning("Attempting to preload a null prefab.", this);
			}
		}

		/// <summary>This will preload the pool based on the Preload setting.</summary>
		[ContextMenu("Preload All")]
		public void PreloadAll()
		{
			if (Preload > 0)
			{
				if (prefab != null)
				{
					for (var i = Total; i < Preload; i++)
					{
						PreloadOneMore();
					}
				}
				else if (Warnings == true)
				{
					if (Warnings == true) Debug.LogWarning("Attempting to preload a null prefab", this);
				}
			}
		}

		/// <summary>This will destroy all preloaded or despawned clones. This is useful if your prefab has been modified, and the old ones are no longer useful.</summary>
		[ContextMenu("Clean")]
		public void Clean()
		{
			for (var i = despawnedClones.Count - 1; i >= 0; i--)
			{
				DestroyImmediate(despawnedClones[i]);
			}

			despawnedClones.Clear();
		}

		protected virtual void Awake()
		{
			if (Application.isPlaying == true)
			{
				PreloadAll();

				if (Persist == true)
				{
					DontDestroyOnLoad(this);
				}
			}
		}

		protected virtual void OnEnable()
		{
			node = Instances.AddLast(this);

			RegisterPrefab();
		}

		protected virtual void OnDisable()
		{
			UnregisterPrefab();

			Instances.Remove(node);

			node = null;
		}

		protected virtual void Update()
		{
			// Decay the life of all delayed destruction calls
			for (var i = delays.Count - 1; i >= 0; i--)
			{
				var delay = delays[i];

				delay.Life -= Time.deltaTime;

				// Skip to next one?
				if (delay.Life > 0.0f)
				{
					continue;
				}

				// Remove and pool delay
				delays.RemoveAt(i); LeanClassPool<Delay>.Despawn(delay);

				// Finally despawn it after delay
				if (delay.Clone != null)
				{
					Despawn(delay.Clone);
				}
				else
				{
					if (Warnings == true) Debug.LogWarning("Attempting to update the delayed destruction of a prefab clone that no longer exists, did you accidentally delete it?", this);
				}
			}
		}

		private void RegisterPrefab()
		{
			if (prefab != null)
			{
				var existingPool = default(LeanGameObjectPool);

				if (prefabMap.TryGetValue(prefab, out existingPool) == true)
				{
					Debug.LogWarning("You have multiple pools managing the same prefab (" + prefab.name + ").", existingPool);
				}
				else
				{
					prefabMap.Add(prefab, this);
				}
			}
		}

		private void UnregisterPrefab()
		{
			// Skip actually null prefabs, but allow destroyed prefabs
			if (Equals(prefab, null) == true)
			{
				return;
			}

			var existingPool = default(LeanGameObjectPool);

			if (prefabMap.TryGetValue(prefab, out existingPool) == true && existingPool == this)
			{
				prefabMap.Remove(prefab);
			}
		}

		private void DespawnWithDelay(GameObject clone, float t)
		{
			// If this object is already marked for delayed despawn, update the time and return
			for (var i = delays.Count - 1; i >= 0; i--)
			{
				var delay = delays[i];

				if (delay.Clone == clone)
				{
					if (t < delay.Life)
					{
						delay.Life = t;
					}

					return;
				}
			}

			// Create delay
			var newDelay = LeanClassPool<Delay>.Spawn() ?? new Delay();

			newDelay.Clone = clone;
			newDelay.Life  = t;

			delays.Add(newDelay);
		}

		private void TryDespawn(GameObject clone)
		{
			if (spawnedClonesHashSet.Remove(clone) == true || spawnedClonesList.Remove(clone) == true)
			{
				DespawnNow(clone);
			}
			else
			{
				if (Warnings == true) Debug.LogWarning("You're attempting to despawn a GameObject that wasn't spawned from this pool, make sure your Spawn and Despawn calls match.", clone);
			}
		}

		private void DespawnNow(GameObject clone, bool register = true)
		{
			// Add clone to despawned list
			if (register == true)
			{
				despawnedClones.Add(clone);
			}

			// Messages?
			InvokeOnDespawn(clone);

			// Deactivate it
			if (Strategy == StrategyType.ActivateAndDeactivate)
			{
				clone.SetActive(false);

				clone.transform.SetParent(transform, false);
			}
			else
			{
				clone.transform.SetParent(DeactivatedChild, false);
			}
		}

		private GameObject CreateClone(Vector3 localPosition, Quaternion localRotation, Vector3 localScale, Transform parent, bool worldPositionStays)
		{
			var clone = DoInstantiate(prefab, localPosition, localRotation, localScale, parent, worldPositionStays);

			if (Stamp == true)
			{
				clone.name = prefab.name + " " + Total;
			}
			else
			{
				clone.name = prefab.name;
			}

			return clone;
		}

		private GameObject DoInstantiate(GameObject prefab, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, Transform parent, bool worldPositionStays)
		{
#if UNITY_EDITOR
			if (Application.isPlaying == false && UnityEditor.PrefabUtility.IsPartOfRegularPrefab(prefab) == true)
			{
				if (worldPositionStays == true)
				{
					return (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, parent);
				}
				else
				{
					var clone = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, parent);

					clone.transform.localPosition = localPosition;
					clone.transform.localRotation = localRotation;
					clone.transform.localScale    = localScale;

					return clone;
				}
			}
#endif

			if (worldPositionStays == true)
			{
				return Instantiate(prefab, parent, true);
			}
			else
			{
				var clone = Instantiate(prefab, localPosition, localRotation, parent);

				clone.transform.localPosition = localPosition;
				clone.transform.localRotation = localRotation;
				clone.transform.localScale    = localScale;

				return clone;
			}
		}

		private void SpawnClone(GameObject clone, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, Transform parent, bool worldPositionStays)
		{
			// Register
			if (Recycle == true)
			{
				spawnedClonesList.Add(clone);
			}
			else
			{
				spawnedClonesHashSet.Add(clone);
			}

			// Update transform
			var cloneTransform = clone.transform;

			cloneTransform.SetParent(null, false);

			cloneTransform.localPosition = localPosition;
			cloneTransform.localRotation = localRotation;
			cloneTransform.localScale    = localScale;

			cloneTransform.SetParent(parent, worldPositionStays);

			// Make sure it's in the current scene
			if (parent == null)
			{
				SceneManager.MoveGameObjectToScene(clone, SceneManager.GetActiveScene());
			}

			// Activate
			if (Strategy == StrategyType.ActivateAndDeactivate)
			{
				clone.SetActive(true);
			}

			// Notifications
			InvokeOnSpawn(clone);
		}

		private void InvokeOnSpawn(GameObject clone)
		{
			switch (Notification)
			{
				case NotificationType.SendMessage: clone.SendMessage("OnSpawn", SendMessageOptions.DontRequireReceiver); break;
				case NotificationType.BroadcastMessage: clone.BroadcastMessage("OnSpawn", SendMessageOptions.DontRequireReceiver); break;
				case NotificationType.IPoolable: clone.GetComponents(tempPoolables); for (var i = tempPoolables.Count - 1; i >= 0; i--) tempPoolables[i].OnSpawn(); break;
				case NotificationType.BroadcastIPoolable: clone.GetComponentsInChildren(tempPoolables); for (var i = tempPoolables.Count - 1; i >= 0; i--) tempPoolables[i].OnSpawn(); break;
			}
		}

		private void InvokeOnDespawn(GameObject clone)
		{
			switch (Notification)
			{
				case NotificationType.SendMessage: clone.SendMessage("OnDespawn", SendMessageOptions.DontRequireReceiver); break;
				case NotificationType.BroadcastMessage: clone.BroadcastMessage("OnDespawn", SendMessageOptions.DontRequireReceiver); break;
				case NotificationType.IPoolable: clone.GetComponents(tempPoolables); for (var i = tempPoolables.Count - 1; i >= 0; i--) tempPoolables[i].OnDespawn(); break;
				case NotificationType.BroadcastIPoolable: clone.GetComponentsInChildren(tempPoolables); for (var i = tempPoolables.Count - 1; i >= 0; i--) tempPoolables[i].OnDespawn(); break;
			}
		}

		private void MergeSpawnedClonesToList()
		{
			if (spawnedClonesHashSet.Count > 0)
			{
				spawnedClonesList.AddRange(spawnedClonesHashSet);

				spawnedClonesHashSet.Clear();
			}
		}

		public void OnBeforeSerialize()
		{
			MergeSpawnedClonesToList();
		}

		public void OnAfterDeserialize()
		{
			if (Recycle == false)
			{
				for (var i = spawnedClonesList.Count - 1; i >= 0; i--)
				{
					var clone = spawnedClonesList[i];

					spawnedClonesHashSet.Add(clone);
				}

				spawnedClonesList.Clear();
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Pool
{
	using UnityEditor;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(LeanGameObjectPool))]
	public class LeanGameObjectPool_Inspector : LeanInspector<LeanGameObjectPool>
	{
		protected override void DrawInspector()
		{
			BeginError(Any(t => t.Prefab == null));
				if (Draw("prefab", "The prefab this pool controls.") == true)
				{
					Each(t => { t.Prefab = (GameObject)serializedObject.FindProperty("prefab").objectReferenceValue; }, true);
				}
			EndError();
			Draw("Notification", "If you need to peform a special action when a prefab is spawned or despawned, then this allows you to control how that action is performed. None = If you use this then you must rely on the OnEnable and OnDisable messages. SendMessage = The prefab clone is sent the OnSpawn and OnDespawn messages. BroadcastMessage = The prefab clone and all its children are sent the OnSpawn and OnDespawn messages. IPoolable = The prefab clone's components implementing IPoolable are called. Broadcast IPoolable = The prefab clone and all its child components implementing IPoolable are called.");
			Draw("Strategy", "This allows you to control how spawned/despawned GameObjects will be handled. The DeactivateViaHierarchy mode should be used if you need to maintain your prefab's de/activation state.\n\nActivateAndDeactivate = Despawned clones will be deactivated and placed under this GameObject.\n\nDeactivateViaHierarchy = Despawned clones will be placed under a deactivated GameObject and left alone.");
			Draw("Preload", "Should this pool preload some clones?");
			Draw("Capacity", "Should this pool have a maximum amount of spawnable clones?");
			Draw("Recycle", "If the pool reaches capacity, should new spawns force older ones to despawn?");
			Draw("Persist", "Should this pool be marked as DontDestroyOnLoad?");
			Draw("Stamp", "Should the spawned clones have their clone index appended to their name?");
			Draw("Warnings", "Should detected issues be output to the console?");

			EditorGUILayout.Separator();

			EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.IntField("Spawned", tgt.Spawned);
				EditorGUILayout.IntField("Despawned", tgt.Despawned);
				EditorGUILayout.IntField("Total", tgt.Total);
			EditorGUI.EndDisabledGroup();

			if (Application.isPlaying == false)
			{
				if (Any(t => t.DespawnedClonesMatch == false))
				{
					EditorGUILayout.HelpBox("Your preloaded clones no longer match the Prefab.", MessageType.Warning);
				}
			}
		}

		[MenuItem("GameObject/Lean/Pool", false, 1)]
		private static void CreateLocalization()
		{
			var gameObject = new GameObject(typeof(LeanGameObjectPool).Name);

			Undo.RegisterCreatedObjectUndo(gameObject, "Create LeanGameObjectPool");

			gameObject.AddComponent<LeanGameObjectPool>();

			Selection.activeGameObject = gameObject;
		}
	}
}
#endif