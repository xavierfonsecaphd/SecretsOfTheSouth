using UnityEngine;

[System.Serializable]
public class OGAnimationLerpState
{
    public float percent;
    public Vector3 positionLerp;
    public float rotationLerp;
    public Vector3 scaleLerp;
    public float colorLerp;
    public float alphaLerp;

    public OGAnimationLerpState(float percent)
    {
        this.percent = percent;
        positionLerp = Vector3.one * percent;
        rotationLerp = percent;
        scaleLerp = Vector3.one * percent;
        colorLerp = percent;
        alphaLerp = percent;
    }

    public OGAnimationLerpState(float percent, float positionLerp, float rotationLerp, float scaleLerp, float colorLerp, float alphaLerp)
    {
        this.percent = percent;
        this.positionLerp = positionLerp * Vector3.one;
        this.rotationLerp = rotationLerp;
        this.scaleLerp = scaleLerp * Vector3.one;
        this.colorLerp = colorLerp;
        this.alphaLerp = alphaLerp;
    }

    public OGAnimationLerpState(float percent, Vector3 positionLerp, float rotationLerp, Vector3 scaleLerp, float colorLerp, float alphaLerp)
    {
        this.percent = percent;
        this.positionLerp = positionLerp;
        this.rotationLerp = rotationLerp;
        this.scaleLerp = scaleLerp;
        this.colorLerp = colorLerp;
        this.alphaLerp = alphaLerp;
    }
}
