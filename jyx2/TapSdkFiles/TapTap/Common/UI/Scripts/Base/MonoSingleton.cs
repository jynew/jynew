using System;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                CreateInstance();
            }
            return _instance;
        }
    }

    private static void CreateInstance()
    {
        Type theType = typeof(T);

        _instance = (T)FindObjectOfType(theType);

        if (_instance == null)
        {
            var go = new GameObject(typeof(T).Name);
            _instance = go.AddComponent<T>();

            GameObject rootObj = GameObject.Find("TapSDKSingletons");
            if (rootObj == null)
            {
                rootObj = new GameObject("TapSDKSingletons");
                DontDestroyOnLoad(rootObj);
            }
            go.transform.SetParent(rootObj.transform);
        }
    }

    public static void DestroyInstance()
    {
        if (_instance != null)
        {
            Destroy(_instance.gameObject);
        }
    }

    public static bool HasInstance()
    {
        return _instance != null;
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance.gameObject != gameObject)
        {
            if (Application.isPlaying)
            {
                Destroy(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject); // UNITY_EDITOR
            }
            return;
        }
        else if (_instance == null)
        {
            _instance = GetComponent<T>();
        }

        DontDestroyOnLoad(gameObject);

        Init();
    }

    protected virtual void OnDestroy()
    {
        Uninit();

        if (_instance != null && _instance.gameObject == gameObject)
        {
            _instance = null;
        }
    }

    protected virtual void Init() {}

    public virtual void Uninit() {}

}
