using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGTransition : OGSingletonBehaviour<OGTransition>
{
    protected override void OnSingletonInitialize() { }

    public static void In(Component comp, float speed)
    {
        In(comp.gameObject, "", speed);
    }

    public static void In(Component comp, string tag = "", float speed = 1f)
    {
        In(comp.gameObject, "", speed);
    }

    public static void In(GameObject go, float speed)
    {
        In(go, "", speed);
    }

    public static void In(GameObject go, string tag = "", float speed = 1f)
    {
        var results = go.GetComponentsInChildren<OGTransitionable>(true);
        for (int i = 0; i < results.Length; i++)
        {
            results[i].TransitionIn(tag, speed);
        }
    }

    public static void Out(Component comp, float speed)
    {
        Out(comp.gameObject, "", speed);
    }

    public static void Out(Component comp, string tag = "", float speed = 1f)
    {
        Out(comp.gameObject, tag, speed);
    }

    public static void Out(GameObject go, float speed)
    {
        Out(go, "", speed);
    }

    public static void Out(GameObject go, string tag = "", float speed = 1f)
    {
        var results = go.GetComponentsInChildren<OGTransitionable>(true);
        for (int i = 0; i < results.Length; i++)
        {
            results[i].TransitionOut(tag, speed);
        }
    }

    public static void PrepareIn(Component comp, string tag = "")
    {
        PrepareIn(comp.gameObject, tag);
    }

    public static void PrepareIn(GameObject go, string tag = "")
    {
        var results = go.GetComponentsInChildren<OGTransitionable>(true);
        for (int i = 0; i < results.Length; i++)
        {
            results[i].ForceRestart(tag);
        }
    }

    public static void End(Component comp, string tag = "")
    {
        End(comp, tag);
    }

    public static void End(GameObject go, string tag = "")
    {
        var results = go.GetComponentsInChildren<OGTransitionable>(true);
        for (int i = 0; i < results.Length; i++)
        {
            results[i].ForceEnd(tag);
        }
    }

    // currently has no effect besides normal deactivation
    public static void Disable(Component comp)
    {
        Disable(comp.gameObject);
    }

    // currently has no effect besides normal deactivation
    public static void Disable(GameObject go)
    {
        go.SetActive(false);
    }
}
