using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Fenderrio.ImageWarp;

[ExecuteInEditMode]
public class OGConnectingWarpedLine : MonoBehaviour
{
    [Header("References")]
    public RectTransform rectTransform;
    public RawImageWarp rawImageWarp;
    public Transform from;
    public Transform to;

    [Header("Settings")]
    public float imageOffset;
    public float beginWarp;
    public float endWarp;
    public bool ignoreZ = false;

    public float startWidth = 1f;
    public float endWidth = 1f;

    [Header("Animation")]
    public bool animate = false;
    public float offsetSpeed = 0.1f;

    private void Reset()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        if (rawImageWarp == null)
        {
            rawImageWarp = GetComponent<RawImageWarp>();
        }
    }

    public void LateUpdate()
    {
        if (from == null || to == null || rectTransform == null) return;

        if (animate)
        {
            imageOffset += Time.deltaTime * offsetSpeed;
            imageOffset = imageOffset % 1f;
        }

        Vector3 fromPos = from.position;
        Vector3 toPos = to.position;

        //if (previousFrom == fromPos && previousTo == toPos) return;
        
        Vector3 diff = ignoreZ ? (Vector3)((Vector2)toPos - (Vector2)fromPos) : toPos - fromPos;

        rectTransform.position = fromPos + diff * 0.5f;
        rectTransform.rotation = Quaternion.LookRotation(Vector3.forward, diff);

        var targetHeight = diff.magnitude / transform.parent.lossyScale.y;
        var myHeight = rectTransform.rect.height;
        var myWidth = rectTransform.rect.width;
        var resizeFactor = targetHeight / myHeight;
        var cropSize = (resizeFactor - 1f) * -0.5f;

        var myQuarterEndWidth = myWidth * 0.25f;
        var myQuarterStartWidth = myWidth * 0.25f;
        var targetQuarterHeight = targetHeight * 0.25f;
        var myStartWidthOffset = myWidth * (startWidth - 1f) * 0.5f;
        var myEndWidthOffset = myWidth * (endWidth - 1f) * 0.5f;

        rawImageWarp.cropTop = cropSize - imageOffset;
        rawImageWarp.cropBottom = cropSize + imageOffset;

        rawImageWarp.cornerOffsetBL = new Vector3(-myStartWidthOffset, -myHeight * imageOffset, 0f);
        rawImageWarp.cornerOffsetBR = new Vector3(myStartWidthOffset, -myHeight * imageOffset, 0f);
        rawImageWarp.cornerOffsetTL = new Vector3(-myEndWidthOffset, -myHeight * imageOffset, 0f);
        rawImageWarp.cornerOffsetTR = new Vector3(myEndWidthOffset, -myHeight * imageOffset, 0f);

        rawImageWarp.bezierEdges = true;

        rawImageWarp.leftBezierHandleA = new Vector3(myQuarterStartWidth * beginWarp * resizeFactor, targetQuarterHeight, 0f);
        rawImageWarp.leftBezierHandleB = new Vector3(myQuarterEndWidth * endWarp * resizeFactor, -targetQuarterHeight, 0f);

        rawImageWarp.rightBezierHandleA = new Vector3(myQuarterEndWidth * endWarp * resizeFactor, -targetQuarterHeight, 0f);
        rawImageWarp.rightBezierHandleB = new Vector3(myQuarterStartWidth * beginWarp * resizeFactor, targetQuarterHeight, 0f);

        rawImageWarp.bottomBezierHandleA = new Vector3(-myQuarterStartWidth, 0f, 0f);
        rawImageWarp.bottomBezierHandleB = new Vector3(myQuarterStartWidth, 0f, 0f);

        rawImageWarp.topBezierHandleA = new Vector3(myQuarterEndWidth, 0f, 0f);
        rawImageWarp.topBezierHandleB = new Vector3(-myQuarterEndWidth, 0f, 0f);
    }
}

