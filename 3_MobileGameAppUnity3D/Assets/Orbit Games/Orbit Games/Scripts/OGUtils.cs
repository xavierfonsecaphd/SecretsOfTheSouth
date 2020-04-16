using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class OGUtils {
    
    public static void Swap<T>(ref T swap1, ref T swap2)
    {
        T temp = swap1;
        swap1 = swap2;
        swap2 = temp;
    }

    public static T FindObjectOfTypeInActiveScenes<T>()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            var rootObjects = scene.GetRootGameObjects();
            for (int g = 0; g < rootObjects.Length; g++)
            {
                var result = rootObjects[g].GetComponentInChildren<T>();
                if (result != null) return result;
            }
        }
        return default(T);
    }

    public static T FindObjectOfTypeInActiveScenes<T>(this GameObject component)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            var rootObjects = scene.GetRootGameObjects();
            for (int g = 0; g < rootObjects.Length; g++)
            {
                var result = rootObjects[g].GetComponentInChildren<T>();
                if (result != null) return result;
            }
        }
        return default(T);
    }

    public static T FindObjectOfTypeInActiveScenes<T>(this Component component)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            var rootObjects = scene.GetRootGameObjects();
            for (int g = 0; g < rootObjects.Length; g++)
            {
                var result = rootObjects[g].GetComponentInChildren<T>();
                if (result != null) return result;
            }
        }
        return default(T);
    }

    public static T[] FindObjectsOfTypeInActiveScenes<T>() where T : Component
    {
        var results = OGListPool<T>.Get();

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            //if (!scene.isLoaded)
            //{
            //    Debug.Log("Skipping non-loaded scene " + scene.name);
            //    continue;
            //}

            try
            {
                var rootObjects = scene.GetRootGameObjects();
                for (int g = 0; g < rootObjects.Length; g++)
                {
                    // Debug.Log("Checking object " + rootObjects[g]);
                    var result = rootObjects[g].GetComponentsInChildren<T>();
                    results.AddRange(result);
                }
            }
            catch { }
        }

        T[] array = results.ToArray();
        OGListPool<T>.Put(ref results);
        return array;
    }

    public static T[] FindObjectsOfTypeInActiveScenes<T>(this GameObject component) where T : Component
    {
        var results = OGListPool<T>.Get();

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            var rootObjects = scene.GetRootGameObjects();
            for (int g = 0; g < rootObjects.Length; g++)
            {
                var result = rootObjects[g].GetComponentsInChildren<T>();
                results.AddRange(result);
            }
        }

        T[] array = results.ToArray();
        OGListPool<T>.Put(ref results);
        return array;
    }

    public static T[] FindObjectsOfTypeInActiveScenes<T>(this Component component) where T : Component
    {
        var results = OGListPool<T>.Get();

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            var rootObjects = scene.GetRootGameObjects();
            for (int g = 0; g < rootObjects.Length; g++)
            {
                var result = rootObjects[g].GetComponentsInChildren<T>();
                results.AddRange(result);
            }
        }

        T[] array = results.ToArray();
        OGListPool<T>.Put(ref results);
        return array;
    }
}
