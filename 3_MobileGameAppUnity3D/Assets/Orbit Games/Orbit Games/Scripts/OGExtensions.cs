using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class OGExtensions {

    private static CultureInfo cult;
    private static CultureInfo GetCulture()
    {
        if (cult == null) 
        {
            try {

                cult = new CultureInfo("en-EN", false);
            }
            catch { cult = null; }
            if (cult == null)
            {
                cult = CultureInfo.InvariantCulture;
            }
        }
        return cult;
    }

    public static bool IsNullOrEmpty(this string item)
    {
        return item == null || item.Length == 0;
    }

    public static string ToTitleCase(this string item)
    {
        return GetCulture().TextInfo.ToTitleCase(item);
    }

    public static string ToFirstUpper(this string str)
    {
        if (str == null)
            return null;

        if (str.Length > 1)
            return char.ToUpper(str[0]) + str.Substring(1);

        return str.ToUpper();
    }

    public static bool IsWhiteSpace(this string input)
    {
        if (input.IsNullOrEmpty()) return false;
        return System.Text.RegularExpressions.Regex.IsMatch(input, @"^\s+$");
    }

    //public static bool IsValidJSON(this string strInput)
    //{
    //        strInput = strInput.Trim();
    //        if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
    //            (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
    //        {
    //            try
    //            {
    //                var obj = JToken.Parse(strInput);
    //                return true;
    //            }
    //            catch
    //            {
    //                return false;
    //            }
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //}

    public static string azAZ09_(this string item, string replaceWith = "")
    {
        return System.Text.RegularExpressions.Regex.Replace(item, @"[^\w]", replaceWith);
    }

    public static string azAZ09(this string item, string replaceWith = "")
    {
        return System.Text.RegularExpressions.Regex.Replace(item, @"[^a-zA-Z0-9]", replaceWith);
    }

    public static string azAZ(this string item, string replaceWith = "")
    {
        return System.Text.RegularExpressions.Regex.Replace(item, @"[^a-zA-Z]", replaceWith);
    }
    
    public static bool IsNullOrEmpty<TValue>(this ICollection<TValue> collection)
    {
        return collection == null || collection.Count == 0;
    }

    public static string ToDebugString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, string separator = ", ", params TKey[] skipKeys)
    {
        if (dictionary == null) return "null";
        if (dictionary.Count == 0) return "empty";
        return "{" + string.Join(separator, dictionary.Where(kv => !skipKeys.Contains(kv.Key)).Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}";
    }

    public static TValue CreateOrAddToList<TKey, TValue>(this IDictionary<TKey, List<TValue>> dictionary, TKey key, TValue value)
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key].Add(value);
        }
        else
        {
            var list = new List<TValue>();
            list.Add(value);
            dictionary.Add(key, list);
        }
        return value;
    }

    public static TValue CreateSetOrAddToDictionary<TKey, TSubKey, TValue>(this IDictionary<TKey, Dictionary<TSubKey, TValue>> dictionary, TKey key, TSubKey subkey, TValue subValue)
    {
        if (!dictionary.ContainsKey(key))
        {
            dictionary.Add(key, new Dictionary<TSubKey, TValue>());
        }
        dictionary[key].SetOrAdd(subkey, subValue);
        return subValue;
    }

    public static TValue GetOrDefault<TKey, TSubKey, TValue>(this IDictionary<TKey, Dictionary<TSubKey, TValue>> dictionary, TKey key, TSubKey subkey)
    {
        if (!dictionary.ContainsKey(key))
        {
            dictionary.Add(key, new Dictionary<TSubKey, TValue>());
        }
        return dictionary[key].GetOrDefault(subkey);
    }

    public static bool Contains<TKey, TSubKey, TValue>(this IDictionary<TKey, Dictionary<TSubKey, TValue>> dictionary, TKey key, TSubKey subkey)
    {
        if (!dictionary.ContainsKey(key))
        {
            dictionary.Add(key, new Dictionary<TSubKey, TValue>());
        }
        return dictionary.GetOrDefault(key, subkey) != null;
    }

    public static TValue SetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key] = value;
        }
        else
        {
            dictionary.Add(key, value);
        }
        return value;
    }

    public static void RemoveIfContainsKey<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
    {
        if (dict.ContainsKey(key))
        {
            dict.Remove(key);
        }
    }

    public static void RemoveIfContains<TValue>(this ICollection<TValue> list, TValue value)
    {
        if (list.Contains(value))
        {
            list.Remove(value);
        }
    }

    public static TValue AddUnique<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if (!dictionary.ContainsKey(key))
        {
            dictionary[key] = value;
        }
        return value;
    }

    public static TValue AddUnique<TValue>(this ICollection<TValue> list, TValue value)
    {
        if (!list.Contains(value))
        {
            list.Add(value);
        }
        return value;
    }

    public static void AddRangeUnique<TValue>(this ICollection<TValue> list, IEnumerable<TValue> values)
    {
        foreach (var value in values)
        {
            list.AddUnique(value);
        }
    }

    public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
    {
        if (key == null) return defaultValue;
        if (dictionary.ContainsKey(key))
        {
            return dictionary[key];
        }
        else
        {
            return defaultValue;
        }
    }

    public static TValue GetOrDefault<TValue>(this IList<TValue> list, int index, TValue defaultValue = default(TValue))
    {
        if (0 <= index && index < list.Count)
        {
            return list[index];
        }
        else
        {
            return defaultValue;
        }
    }

    public static TValue GetClamped<TValue>(this IList<TValue> list, int index)
    {
        if (list.Count == 0) return default(TValue);
        return list[Mathf.Clamp(index, 0, list.Count - 1)];
    }

    public static TValue GetLooped<TValue>(this IList<TValue> list, int index)
    {
        if (list.Count == 0) return default(TValue);
        // note that dividing integers results in floored int values, thus (x / y) * y is NOT the same as x
        if (index < 0) index -= (index / list.Count) * list.Count - list.Count; // funky code to get index above 0 with multiples of list.Count
        return list[index % list.Count];
    }

    public static TValue GetRandom<TValue>(this IList<TValue> list)
    {
        if (list.Count == 0) return default(TValue);
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public static int GetRandomIndex<TValue>(this IList<TValue> list)
    {
        if (list.Count == 0) return -1;
        return UnityEngine.Random.Range(0, list.Count);
    }

    public static string toNumberString(this float number, int decimals = 0)
    {
        if (decimals == 0) return (Mathf.RoundToInt(number)).ToString("d", GetCulture());
        return number.ToString("n" + decimals, GetCulture());
    }

    public static string toRoundTripString(this float number)
    {
        return number.ToString("G9", CultureInfo.InvariantCulture);
    }

    public static string GetPath(this GameObject gameObject)
    {
        List<string> path = new List<string>();
        while (gameObject != null)
        {
            path.Add(gameObject.name);
            gameObject = gameObject.transform.parent == null ? null : gameObject.transform.parent.gameObject;
        }
        path.Reverse();
        return string.Join(" > ", path.ToArray());
    }

    public static void ResetTransform(this Component component)
    {
        component.transform.localPosition = Vector3.zero;
        component.transform.localScale = Vector3.one;
        component.transform.localRotation = Quaternion.identity;
    }

    public static void ResetTransform(this GameObject component)
    {
        component.transform.ResetTransform();
    }

    public static Vector3 LocalPositionWithin(this GameObject thisObject, Component otherObject)
    {
        return otherObject.transform.InverseTransformPoint(thisObject.transform.position);
    }

    public static Vector3 LocalPositionWithin(this Component thisObject, Component otherObject)
    {
        return otherObject.transform.InverseTransformPoint(thisObject.transform.position);
    }

    public static Vector3 LocalPositionWithin(this GameObject thisObject, GameObject otherObject)
    {
        return otherObject.transform.InverseTransformPoint(thisObject.transform.position);
    }

    public static Vector3 LocalPositionWithin(this Component thisObject, GameObject otherObject)
    {
        return otherObject.transform.InverseTransformPoint(thisObject.transform.position);
    }

    public static Vector3 LocalPointWithin(this Component source, Vector3 point, Component destination)
    {
        return destination.transform.InverseTransformPoint(source.transform.TransformPoint(point));
    }

    public static Vector3 LocalPointWithin(this Component source, Vector3 point, GameObject destination)
    {
        return destination.transform.InverseTransformPoint(source.transform.TransformPoint(point));
    }

    public static Vector3 LocalPointWithin(this GameObject source, Vector3 point, GameObject destination)
    {
        return destination.transform.InverseTransformPoint(source.transform.TransformPoint(point));
    }

    public static Vector3 LocalPointWithin(this GameObject source, Vector3 point, Component destination)
    {
        return destination.transform.InverseTransformPoint(source.transform.TransformPoint(point));
    }

    /// <summary>
    /// Tries to match the transform of an object with that of another in the scene. Note that this fails when objects have complex
    /// transformations, because Unity does not support proper shearing
    /// </summary>
    /// <param name="matcher"></param>
    /// <param name="with"></param>
    public static void Match(this Transform matcher, Transform with)
    {

        // hacky way to grab same transform as another object
        // but fails because unity doesnt support shearing
        //var rememberParent = transform.parent;
        //transform.SetParent(toMatch, false);
        //transform.localPosition = Vector3.zero;
        //transform.localEulerAngles = Vector3.zero;
        //transform.localScale = Vector3.one;
        //transform.SetParent(rememberParent, true);

        // correct way to grab same transform like another object
        // but also fails because unity doesnt support shearing
        matcher.position = with.transform.position;
        matcher.rotation = with.transform.rotation;
        matcher.localScale = matcher.parent.InverseTransformVector(with.lossyScale);
    }
}
