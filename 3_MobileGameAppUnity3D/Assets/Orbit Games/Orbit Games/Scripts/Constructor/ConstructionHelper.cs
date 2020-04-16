using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ConstructionHelper : MonoBehaviour {

#if UNITY_EDITOR
    [Header("Sprites to replace with")]
    public List<Sprite> replacables;

    [Header("Duplication of PLUG")]
    public OGPlug theObject;
    public Image theImage1;
    public Image theImage2;

    [Buttons("DuplicatePlug")]
    public ButtonsContainer button;
    public void DuplicatePlug()
    {
        int i = 0;
        foreach (var sprite in replacables)
        {
            var name = sprite.name;
            var newDef = Instantiate(theObject.definition);
            newDef.name = name;
            newDef.ForceOverrideUniqueID(name.ToLower());
            newDef.ToTitle();

            var fileName = newDef.GetNameFromID();
            var path = AssetDatabase.GetAssetPath(theObject.definition);
            path = path.Remove(path.LastIndexOf('/'));

            AssetDatabase.CreateAsset(newDef, path + "/" + fileName + ".asset");

            if (theImage1) theImage1.sprite = sprite;
            if (theImage2) theImage2.sprite = sprite;

            var newCopy = Instantiate(theObject.gameObject);
            newCopy.name = fileName;
            newCopy.GetComponent<OGPlug>().definition = newDef;

            var prefabObject = PrefabUtility.CreatePrefab(path + "/" + fileName + ".prefab", newCopy);
            DestroyImmediate(newCopy);

            var prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefabObject);
            prefabInstance.transform.SetParent(theObject.transform.parent);
            prefabInstance.transform.position = theObject.transform.position + Vector3.right * (i++) * 105f;
        }
    }

    [Header("Duplication of FEATURE")]
    public OGFeatureDesign templateFeature;
    
    [Buttons("DuplicateFeature")]
    public ButtonsContainer duplicateFeature;
    public void DuplicateFeature()
    {
        foreach (var sprite in replacables)
        {
            var name = sprite.name;
            var newDesign = Instantiate(templateFeature);
            newDesign.name = name;
            newDesign.ForceOverrideUniqueID(name.ToLower());
            newDesign.ToTitle();

            var fileName = newDesign.GetNameFromID();
            var path = AssetDatabase.GetAssetPath(templateFeature);
            path = path.Remove(path.LastIndexOf('/'));

            AssetDatabase.CreateAsset(newDesign, path + "/" + fileName + ".asset");

            newDesign.details[0].sprite = sprite;
            OGEditorExtensions.SaveAsset(newDesign);
        }
    }
#endif
}
