
using UnityEngine;

[System.Serializable]
public struct OGAnimationMask
{
    [HideInInspector]
    public bool active;
    public Vector3Mask position;
    public Vector3Mask rotation;
    public Vector3Mask scale;
    public bool color;
    public bool alpha;
    
    public OGAnimationMask(bool active, Vector3Mask position, Vector3Mask rotation, Vector3Mask scale, bool color, bool alpha)
    {
        this.active = false;// active;
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
        this.color = color;
        this.alpha = alpha;
    }

    public OGAnimationMask(Preset preset)
    {
        active = false;
        position = Vector3Mask.False;
        rotation = Vector3Mask.False;
        scale = Vector3Mask.False;
        color = false;
        alpha = false;

        switch (preset)
        {
            case Preset.All:
                active = false;//true;
                position = Vector3Mask.True;
                rotation = Vector3Mask.True;
                scale = Vector3Mask.True;
                color = true;
                alpha = true;
                break;
            case Preset.Transform:
                position = Vector3Mask.True;
                rotation = Vector3Mask.True;
                scale = Vector3Mask.True;
                break;
            case Preset.Position:
                position = Vector3Mask.True;
                break;
            case Preset.Position_Rotation:
                position = Vector3Mask.True;
                rotation = Vector3Mask.True;
                break;
            case Preset.Position_Scale:
                position = Vector3Mask.True;
                scale = Vector3Mask.True;
                break;
            case Preset.Object_Transform:
                position = Vector3Mask.True;
                rotation = Vector3Mask.True;
                scale = Vector3Mask.True;
                break;
            case Preset.Object_Position:
                active = false;//true;
                position = Vector3Mask.True;
                break;
            case Preset.Object_Position_Rotation:
                active = false;//true;
                position = Vector3Mask.True;
                rotation = Vector3Mask.True;
                break;
            case Preset.Object_Position_Scale:
                active = false;//true;
                position = Vector3Mask.True;
                scale = Vector3Mask.True;
                break;
            default:
                break;
        }
    }

    public enum Preset
    {
        All,
        Transform,
        Position,
        Position_Rotation,
        Position_Scale,
        Object_Transform,
        Object_Position,
        Object_Position_Rotation,
        Object_Position_Scale,
    }
}
