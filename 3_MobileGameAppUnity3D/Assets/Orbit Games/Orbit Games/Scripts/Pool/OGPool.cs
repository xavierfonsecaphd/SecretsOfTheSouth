using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGPool : OGSingletonBehaviour<OGPool>
{
    /// <summary>
    /// For each prefab reference 
    /// </summary>
    private static HashSet<GameObject> inPool = new HashSet<GameObject>();
    private static Dictionary<GameObject, Stack<GameObject>> poolStacks = new Dictionary<GameObject, Stack<GameObject>>();
    private static Dictionary<GameObject, GameObject> objectToOriginal = new Dictionary<GameObject, GameObject>();
    private static Dictionary<GameObject, IOGPoolEventsListener[]> objectToListeners = new Dictionary<GameObject, IOGPoolEventsListener[]>();
    /// <summary>
    /// The location where all objects are placed when stored and waiting to be used in the game
    /// </summary>
    private static Transform objectPool;

    public bool enablePooling = true;

    protected override void OnSingletonInitialize()
    {
        enablePooling = Debug.isDebugBuild ? enablePooling : true;

        var objectPool = new GameObject("OBJECT POOL", typeof(RectTransform));
        objectPool.transform.localPosition = Vector3.up * 1000000f;
        objectPool.transform.localRotation = Quaternion.identity;
        objectPool.transform.localScale = Vector3.one;
        objectPool.SetActive(false);
        objectPool.hideFlags = HideFlags.DontSave | HideFlags.NotEditable | HideFlags.HideInInspector;
        
        objectPool.transform.SetParent(transform);

        OGPool.objectPool = objectPool.transform;
    }

    private static GameObject createPoolableCopy(GameObject original)
    {
        GameObject go = Instantiate(original);
        objectToOriginal.Add(go, original);
        objectToListeners.Add(go, go.GetComponentsInChildren<IOGPoolEventsListener>(true));
        inPool.Add(go);
        NotifyCreated(go);
        return go;
    }

    public static void preparePool<T>(T original, int count) where T : Component
    {
        preparePool(original.gameObject, count);
    }

    public static void preparePool(GameObject original, int count)
    {
        if (!Application.isPlaying) return;
        if (!poolStacks.ContainsKey(original))
        {
            poolStacks.Add(original, new Stack<GameObject>());
        }

        var poolStack = poolStacks[original];
        while (poolStack.Count < count)
        {
            GameObject go = createPoolableCopy(original);
            poolStack.Push(go);
            go.transform.SetParent(objectPool.transform);
            go.transform.localPosition = Vector3.zero;
        }
    }

    public static GameObject GetOriginal(GameObject copy)
    {
        return objectToOriginal.GetOrDefault(copy);
    }

    public static T placeCopy<T, W>(T original, W newParent) where T : Component where W : Component
    {
        return placeCopy(original.gameObject, newParent.transform).GetComponent<T>();
    }

    public static T placeCopy<T>(T original, GameObject newParent) where T : Component
    {
        return placeCopy(original.gameObject, newParent.transform).GetComponent<T>();
    }

    public static GameObject placeCopy(GameObject original, GameObject newParent)
    {
        return placeCopy(original, newParent.transform);
    }

    public static GameObject placeCopy<T>(GameObject original, T newParent) where T : Component
    {
        GameObject retreivedCopy = null;

        if (Application.isPlaying && I.enablePooling)
        {
            if (!poolStacks.ContainsKey(original))
            {
                poolStacks.Add(original, new Stack<GameObject>());
            }

            while (poolStacks[original].Count > 0 && retreivedCopy == null)
            {
                retreivedCopy = poolStacks[original].Pop();
            }
        }

        // if no object retreived from stack, we need to make one
        if (retreivedCopy == null)
        {
            retreivedCopy = createPoolableCopy(original);
        }

        if (newParent != null)
        {
            retreivedCopy.transform.SetParent(newParent.transform);
            retreivedCopy.transform.localPosition = Vector3.zero;
            retreivedCopy.transform.localRotation = Quaternion.identity;
            retreivedCopy.transform.localScale = Vector3.one;
        }
        else
        {
            throw new System.Exception("Can't place a copy from pool to a transform that is null");
        }

        retreivedCopy.SetActive(true);
        inPool.Remove(retreivedCopy);

        NotifyPlaced(retreivedCopy);
        return retreivedCopy;
    }

    public static void removeCopy(GameObject thisPoolable)
    {
        if (thisPoolable == null || I == null)
        {
            return;
        }

        if (inPool.Contains(thisPoolable))
        {
            return;
        }

        NotifyRemoved(thisPoolable);

        if (!Application.isPlaying)
        {
            DestroyImmediate(thisPoolable);
            return;
        }

        if (!objectToOriginal.ContainsKey(thisPoolable))
        {
            // object was not found in pool register, but hey, who cares! Let's just use itself as the parent
            objectToOriginal.Add(thisPoolable, thisPoolable);
        }

        var original = objectToOriginal[thisPoolable];
        thisPoolable.transform.SetParent(objectPool);
        thisPoolable.transform.localPosition = Vector3.zero;
        
        if (!I.enablePooling)
        {
            Destroy(thisPoolable);
            return;
        }

        poolStacks[original].Push(thisPoolable);
        inPool.Add(thisPoolable);
    }

    public static void removeCopy(Component thisPoolable)
    {
        removeCopy(thisPoolable.gameObject);
    }

    public static void removeAllPoolableChildren(GameObject ofObject)
    {
        var poolables = ofObject.GetComponentsInChildren<PoolableObject>(true);
        foreach (var p in poolables)
        {
            removeCopy(p);
        }
    }

    public static void removeAllPoolableChildren(Component ofObject)
    {
        removeAllPoolableChildren(ofObject.gameObject);
    }

    public static void removeAllPoolableChildren(GameObject ofOriginal, GameObject ofObject)
    {
        var poolables = ofObject.GetComponentsInChildren<PoolableObject>(true);
        foreach (var p in poolables)
        {
            if (objectToOriginal[p.gameObject] == ofOriginal)
            {
                removeCopy(p);
            }
        }
    }

    public static void removeAllPoolableChildren(Component ofOriginal, GameObject ofObject)
    {
        removeAllPoolableChildren(ofOriginal.gameObject, ofObject);
    }

    public static void removeAllPoolableChildren(GameObject ofOriginal, Component ofObject)
    {
        removeAllPoolableChildren(ofOriginal, ofObject.gameObject);
    }

    public static void removeAllPoolableChildren(Component ofOriginal, Component ofObject)
    {
        removeAllPoolableChildren(ofOriginal.gameObject, ofObject.gameObject);
    }

    private static void NotifyCreated(GameObject go)
    {
        if (!objectToListeners.ContainsKey(go))
        {
            objectToListeners.Add(go, go.GetComponentsInChildren<IOGPoolEventsListener>(true));
        }
        var listeners = objectToListeners[go];
        for (int i = 0; i < listeners.Length; i++)
        {
            listeners[i].OnCreatedForPool();
        }
    }

    private static void NotifyPlaced(GameObject go)
    {
        if (!objectToListeners.ContainsKey(go))
        {
            objectToListeners.Add(go, go.GetComponentsInChildren<IOGPoolEventsListener>(true));
        }
        var listeners = objectToListeners[go];
        for (int i = 0; i < listeners.Length; i++)
        {
            listeners[i].OnPlacedFromPool();
        }
    }

    private static void NotifyRemoved(GameObject go)
    {
        if (!objectToListeners.ContainsKey(go))
        {
            objectToListeners.Add(go, go.GetComponentsInChildren<IOGPoolEventsListener>(true));
        }
        var listeners = objectToListeners[go];
        for (int i = 0; i < listeners.Length; i++)
        {
            listeners[i].OnRemovedToPool();
        }
    }

    public static void notifyDestroying(GameObject copy)
    {
        if (objectToOriginal.ContainsKey(copy))
        {
            objectToOriginal.Remove(copy);
        }
    }
}

public class PoolableObject : MonoBehaviour
{
    private void OnDestroy()
    {
        OGPool.notifyDestroying(gameObject);
    }
}

public interface IOGPoolEventsListener
{
    void OnRemovedToPool();
    void OnPlacedFromPool();
    void OnCreatedForPool();
}