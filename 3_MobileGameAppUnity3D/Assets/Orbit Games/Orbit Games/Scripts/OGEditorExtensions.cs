using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class OGEditorExtensions {

    public static void SaveAsset(Object asset)
    {
#if UNITY_EDITOR
        //if (!Application.isPlaying)
        {
            if (asset is Component && IsInScene((Component)asset))
                return;
            if (asset is GameObject && IsInScene((GameObject)asset))
                return;
            if (asset is Component && IsPrefabInstance(((Component)asset).gameObject))
                return;
            if (asset is GameObject && IsPrefabInstance((GameObject)asset))
                return;

            Debug.Log("Saving asset: " + asset);
            UnityEditor.EditorUtility.SetDirty(asset);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }
#else
#endif
    }

    public static T SaveAsPreset<T, W>(W originalAsset, string windowTitle, string assetName, bool focusOnCreatedFile)
        where T : OGPresetScriptableObject<W> where W : IOGInitializable, new()
    {
#if UNITY_EDITOR

        var absolutePath = EditorUtility.SaveFilePanel(windowTitle, "", assetName + ".asset", "asset");
        if (absolutePath.Length != 0)
        {
            var savedAsset = ScriptableObject.CreateInstance<T>();
            var copiedValues = JsonUtility.ToJson(originalAsset);
            JsonUtility.FromJsonOverwrite(copiedValues, savedAsset.preset);

            var relativepath = "Assets" + absolutePath.Substring(Application.dataPath.Length);

            AssetDatabase.CreateAsset(savedAsset, relativepath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            if (focusOnCreatedFile)
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = savedAsset;
            }
            return savedAsset;
        }
#endif
        return null;
    }

    public static void RecordUndo(Object asset, string action)
    {
#if UNITY_EDITOR
        Undo.RecordObject(asset, action);
#endif
    }

    private static string copiedValues;

    public static void CopyValues(object asset)
    {
        copiedValues = JsonUtility.ToJson(asset);
    }

    public static void CopyPasteValues(object fromAsset, object toAsset)
    {
        JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(fromAsset), toAsset);
    }

    public static void PasteValues(object asset)
    {
        JsonUtility.FromJsonOverwrite(copiedValues, asset);
    }

    // source: http://answers.unity3d.com/questions/425012/get-the-instance-the-serializedproperty-belongs-to.html

    public static MethodInfo GetMethod(object obj, string methodName)
    {
        MethodInfo[] methods = obj.GetType().GetMethods();
        foreach (var method in methods)
        {
            if (method.Name.Equals(methodName))
            {
                return method;
            }
        }
        return null;
    }

#if UNITY_EDITOR
    public static object GetParentObject(SerializedProperty prop)
    {
        var path = prop.propertyPath.Replace(".Array.data[", "[");
        object obj = prop.serializedObject.targetObject;
        var elements = path.Split('.');
        foreach (var element in elements.Take(elements.Length - 1))
        {
            if (element.Contains("["))
            {
                var elementName = element.Substring(0, element.IndexOf("["));
                var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetFieldValue(obj, elementName, index);
            }
            else
            {
                obj = GetFieldValue(obj, element);
            }
        }
        return obj;
    }
#endif

    private static object GetFieldValue(object source, string name)
    {
        if (source == null)
            return null;
        var type = source.GetType();
        var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        if (f == null)
        {
            var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p == null)
                return null;
            return p.GetValue(source, null);
        }
        return f.GetValue(source);
    }

    private static object GetFieldValue(object source, string name, int index)
    {
        var enumerable = GetFieldValue(source, name) as IEnumerable;
        var enm = enumerable.GetEnumerator();
        if (!enm.MoveNext()) return null;
        else index--;
        while (index-- >= 0)
            enm.MoveNext();
        return enm.Current;
    }

    public static bool IsInScene(GameObject gameObject)
    {
        return !gameObject.scene.name.IsNullOrEmpty();
    }

    public static bool IsInScene(Component obj)
    {
        return IsInScene(obj.gameObject);
    }

    public static bool IsPrefabInstance(GameObject gameObject)
    {
#if UNITY_EDITOR
        return UnityEditor.PrefabUtility.GetPrefabParent(gameObject) != null && UnityEditor.PrefabUtility.GetPrefabObject(gameObject.transform) != null;
#else
        return false;
#endif
    }

    public static bool IsDisconnectedPrefabInstance(GameObject gameObject)
    {
#if UNITY_EDITOR
        return UnityEditor.PrefabUtility.GetPrefabParent(gameObject) != null && UnityEditor.PrefabUtility.GetPrefabObject(gameObject.transform) == null;
#else
        return false;
#endif
    }

    public static bool IsPrefabOriginal(GameObject gameObject)
    {
#if UNITY_EDITOR
        return UnityEditor.PrefabUtility.GetPrefabParent(gameObject) == null && UnityEditor.PrefabUtility.GetPrefabObject(gameObject.transform) != null;
#else
        return false;
#endif
    }

    public static Object GetPrefabOriginal(GameObject gameObject)
    {
#if UNITY_EDITOR
        return UnityEditor.PrefabUtility.GetPrefabParent(gameObject);
#else
        return null;
#endif
    }
}
