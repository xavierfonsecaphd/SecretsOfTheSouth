using Fenderrio.ImageWarp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawImageWarpSprite : MonoBehaviour {

    public Sprite sprite;
    public bool autoSetOnGizmo;
    private RawImageWarp imageWarp;
    [HideInInspector]
    public Rect oldSpriteRect;
    [HideInInspector]
    public Sprite oldSprite;

    public void SetSprite(Sprite sprite)
    {
        if (oldSprite == sprite) return;

        this.sprite = sprite;

        if (!imageWarp) imageWarp = GetComponent<RawImageWarp>();

        if (imageWarp && sprite && sprite.texture)
        {
            var rectTransform = transform as RectTransform;
            var spriteSize = rectTransform.sizeDelta;
            var newRect = sprite.textureRect;

            var uLeftOld = oldSpriteRect.xMin / sprite.texture.width;
            var uLeftNew = newRect.xMin / sprite.texture.width;
            var xLeftDiff = (uLeftNew - uLeftOld) * spriteSize.x;

            var vTopOld = oldSpriteRect.yMax / sprite.texture.height;
            var vTopNew = newRect.yMax / sprite.texture.height;
            var yTopDiff = (vTopNew - vTopOld) * spriteSize.y;

            var uRightOld = oldSpriteRect.xMax / sprite.texture.width;
            var uRightNew = newRect.xMax / sprite.texture.width;
            var xRightDiff = (uRightNew - uRightOld) * spriteSize.x;

            var vBottomOld = oldSpriteRect.yMax / sprite.texture.height;
            var vBottomNew = newRect.yMax / sprite.texture.height;
            var yBottomDiff = (vBottomNew - vBottomOld) * spriteSize.y;

            imageWarp.cornerOffsetTL -= new Vector3(xLeftDiff, yTopDiff, 0);
            imageWarp.cornerOffsetBL -= new Vector3(xLeftDiff, yBottomDiff, 0);
            imageWarp.cornerOffsetTR -= new Vector3(xRightDiff, yTopDiff, 0);
            imageWarp.cornerOffsetBR -= new Vector3(xRightDiff, yBottomDiff, 0);

            imageWarp.texture = sprite.texture;
            imageWarp.cropTop = (sprite.texture.height - newRect.yMax) / sprite.texture.height;
            imageWarp.cropBottom = newRect.yMin / sprite.texture.height;
            imageWarp.cropLeft = newRect.xMin / sprite.texture.width;
            imageWarp.cropRight = (sprite.texture.width - newRect.xMax) / sprite.texture.width;

            oldSpriteRect = newRect;
            oldSprite = sprite;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (autoSetOnGizmo)
        {
            SetSprite(sprite);
        }
    }
}
