using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class OGPlugPin : OGConnectable, IOGPoolEventsListener
{
    [Header("Definition")]
    public OGSocketHoleDefinition forSocketHole;

    [Header("Connection")]
    public OGPlug parentPlug;
    public OGSocketHole currentSocketHole;
    [Buttons("Update parent", "UpdateParent")]
    public ButtonsContainer updateButton;
    
    public void Reset()
    {
        if (currentSocketHole != null)
        {
            currentSocketHole = null;
            SetParent(parentPlug.transform);
        }
    }
    
    public void ConnectToSocketHole(OGSocketHole hole)
    {
        if (hole == null)
        {
            gameObject.SetActive(false);
            Reset();
            return;
        }
        else
        {
            if (hole.definition != forSocketHole)
            {
                throw new System.Exception("Can't connect to different socket hole " + hole + " than designed for. " +
                    "Was expecting " + forSocketHole);
            }
            else
            {
                gameObject.SetActive(true);
                currentSocketHole = hole;
                SetParent(currentSocketHole.transform);
            }
        }
    }

    private void SetParent(Transform newParent)
    {
        transform.SetParent(newParent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public void OnCreatedForPool() { Reset(); }
    public void OnPlacedFromPool() { Reset(); }
    public void OnRemovedToPool() { Reset(); }

    [Header("Debug")]
    public OGSocketHole hole;
    [Buttons("Connect", "Connect", "Reset", "Reset")]
    public ButtonsContainer debug;

    public void Connect()
    {
        ConnectToSocketHole(hole);
    }
}
