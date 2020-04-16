using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class OGAbstractSingleton : MonoBehaviour {
    public abstract void _Initialize();
}

public abstract class OGSingletonBehaviour<T> : OGAbstractSingleton where T : OGSingletonBehaviour<T>
{
    private static bool quiting = false;
    private static List<T> instances = new List<T>();
    [NonSerialized]
    private bool _initialized = false;
    private void Awake()
    {
        _Initialize();
    }
    private void OnDrawGizmos()
    {
        _Initialize();
    }

    /// <summary>
    /// Shortcut for GetInstance()
    /// </summary>
    public static T I
    {
        get
        {
            return GetInstance();
        }
    }

    private void OnApplicationQuit()
    {
        quiting = true;
    }

    /// <summary>
    /// Gets the singleton instance of this class. Checks if the originally known object is destroyed, and will find a new one
    /// If multiple exist, the first that is active in the list is returned. This way you could swap singletons by enabling/disabling them
    /// </summary>
    /// <returns></returns>
    public static T GetInstance()
    {
        if (instances.Count == 0) {
            // then try to find by searching scenes
            T[] objects = OGUtils.FindObjectsOfTypeInActiveScenes<T>();
            
            foreach (var obj in objects)
            {
                // skip prefabs from the assets folder, only in-scene objects pls
                if (obj.gameObject.scene.name == null || obj.gameObject.scene.rootCount == 0)
                    continue; 

                Debug.LogWarning("Instance of " + typeof(T).ToString() + " was not yet set. Found it, though!");
                obj._Initialize();
            }
        }
        if (instances.Count == 1)
        {
            // get the only instance known to us
            return ReturnUndestroyed(instances[0]);
        }
        if (instances.Count > 1) 
        {
            // get the one that is currently active
            // this singleton script is assuming there is always only 1 ever active
            for (int i = 0; i < instances.Count; i++)
            {
                if (instances[i] == null)
                {
                    // an object was destroyed, let's handle it and try getting a new one
                    return ReturnUndestroyed(instances[i]);
                }
                if (instances[i].gameObject.activeInHierarchy)
                {
                    return instances[i];
                }
            }
            // nothing was active, so let's just get the first
            return ReturnUndestroyed(instances[0]);
        }

        if (!quiting)
        {
            Debug.LogWarning("Instance of " + typeof(T).ToString() + " does not exist! Did you forget to insert the ORBIT GAMES CORE prefab into your scene?");
        }
        else
        {
            Debug.LogWarning("Instance of " + typeof(T).ToString() + " could not be found.");
        }
        return null;
    }

    private static T ReturnUndestroyed(T instanceToCheck)
    {
        if (instanceToCheck != null) return instanceToCheck;
        instances.RemoveAll(instance => instance == null);

        if (!quiting)
        {
            Debug.LogWarning("Instance of " + typeof(T).ToString() + " was destroyed! Trying to find a new one");
            return GetInstance();
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Helper method to initialize only once
    /// </summary>
    public override void _Initialize()
    {
        if (!_initialized)
        {
            _initialized = true;
            instances.AddUnique(this as T);

            if (Application.isPlaying)
                OnSingletonInitialize();
        }
    }

    /// <summary>
    /// Called only once, whenever this behaviour is awoken or created.
    /// </summary>
    abstract protected void OnSingletonInitialize();
}
