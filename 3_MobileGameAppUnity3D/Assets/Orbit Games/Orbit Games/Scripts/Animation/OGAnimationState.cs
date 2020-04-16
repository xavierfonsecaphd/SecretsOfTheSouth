using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct OGAnimationState
{
    public bool isAbsolute;
    public bool isCanvasState;
    [Range(-1f, 1f)]
    public float active;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public Vector4 color;
    [Range(-1f, 1f)]
    public float alpha;

    public bool IsEmpty()
    {
        return !isAbsolute && !isCanvasState
            && active == 0f
            && position == Vector3.zero
            && rotation.x == 0f
            && rotation.y == 0f
            && rotation.z == 0f
            && rotation.w == 0f
            && scale == Vector3.zero
            && color == Vector4.zero
            && alpha == 0f;
    }

    public bool IsNoChange()
    {
        return active == 0f
            && position == Vector3.zero
            && rotation == Quaternion.identity
            && scale == Vector3.one
            && color == Vector4.zero
            && alpha == 0f;
    }

    public OGAnimationState Duplicate()
    {
        var duplicant = new OGAnimationState();
        duplicant.isAbsolute = isAbsolute;
        duplicant.isCanvasState = isCanvasState;

        duplicant.active = active;
        duplicant.position = position;
        duplicant.rotation = rotation;
        duplicant.scale = scale;
        duplicant.color = color;
        duplicant.alpha = alpha;
        return duplicant;
    }

    public OGAnimationState(bool startsActive)
    {
        isAbsolute = false;
        isCanvasState = false;
        active = startsActive ? 1f : 0f;
        position = Vector3.zero;
        rotation = Quaternion.identity;
        scale = Vector3.one;
        color = Vector4.zero;
        alpha = 1f;
    }

    public OGAnimationState(GameObject gameObject) : this(true)
    {
        var transform = gameObject.transform;
        if (transform is RectTransform)
        {
            From(transform as RectTransform, gameObject.GetComponent<Image>(), gameObject.GetComponent<CanvasGroup>());
        }
        else
        {
            From(transform);
        }
    }

    public OGAnimationState(OGAnimationTargets targets) : this(true)
    {
        if (targets.transform is RectTransform)
        {
            From(targets.transform as RectTransform, targets.image, targets.canvasGroup);
        }
        else
        {
            From(targets.transform);
        }
    }

    public OGAnimationState(RectTransform rectTransform, Image image = null, CanvasGroup canvasGroup = null) : this(true)
    {
        From(rectTransform, image, canvasGroup);
    }

    public OGAnimationState(Transform transform) : this(true)
    {
        From(transform);
    }

    public void From(RectTransform rectTransform, Image image = null, CanvasGroup canvasGroup = null)
    {
        isAbsolute = true;
        isCanvasState = true;
        active = rectTransform.gameObject.activeSelf ? 1f : 0f;
        if (rectTransform != null) CopyFromRectTransform(rectTransform);
        if (image != null) CopyColorFrom(image);
        if (canvasGroup != null) CopyAlphaFrom(canvasGroup);
    }

    public void From(Transform transform)
    {
        isAbsolute = true;
        isCanvasState = false;
        active = transform.gameObject.activeSelf ? 1f : 0f;
        if (transform != null) CopyFromTransform(transform);
    }

    public override bool Equals(object obj)
    {
        if (obj is OGAnimationState)
        {
            return this == (OGAnimationState)obj;
        }
        else
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(OGAnimationState lhs, OGAnimationState rhs)
    {
        return lhs.isAbsolute == rhs.isAbsolute 
            && lhs.isCanvasState == rhs.isCanvasState
            && lhs.active == rhs.active
            && lhs.position == rhs.position
            && lhs.rotation == rhs.rotation
            && lhs.scale == rhs.scale
            && lhs.color == rhs.color
            && lhs.alpha == rhs.alpha;
    }

    public static bool operator !=(OGAnimationState lhs, OGAnimationState rhs)
    {
        return lhs.isAbsolute != rhs.isAbsolute
            || lhs.isCanvasState != rhs.isCanvasState
            || lhs.active != rhs.active
            || lhs.position != rhs.position
            || lhs.rotation != rhs.rotation
            || lhs.scale != rhs.scale
            || lhs.color != rhs.color
            || lhs.alpha != rhs.alpha;
    }

    public static OGAnimationState operator -(OGAnimationState lhs, OGAnimationState rhs)
    {
        return Subtract(lhs, rhs);
    }

    public static OGAnimationState operator -(OGAnimationState state)
    {
        return Negate(state);
    }

    public static OGAnimationState operator +(OGAnimationState lhs, OGAnimationState rhs)
    {
        return Add(lhs, rhs);
    }

    public static OGAnimationState Negate(OGAnimationState state)
    {
        var duplicant = state.Duplicate();
        duplicant.isAbsolute = false;
        duplicant.isCanvasState = state.isCanvasState;
        duplicant.active = -state.active;
        duplicant.position = -state.position;
        duplicant.rotation = Quaternion.Inverse(state.rotation);
        duplicant.scale = new Vector3(1f / state.scale.x, 1f / state.scale.y, 1f / state.scale.z);
        duplicant.color = -state.color;
        duplicant.alpha = -state.alpha;
        return duplicant;
    }

    public static OGAnimationState Subtract(OGAnimationState lhs, OGAnimationState rhs)
    {
        if (rhs.isCanvasState != lhs.isCanvasState)
            Debug.LogWarning("Animation states incompatible: one is a UI state, while another is not");

        var duplicant = rhs.Duplicate();
        duplicant.isAbsolute = false;
        duplicant.isCanvasState = rhs.isCanvasState && lhs.isCanvasState;
        duplicant.active = Mathf.Clamp(lhs.active - rhs.active, -1f, 1f);
        duplicant.position = lhs.position - rhs.position;
        duplicant.rotation = lhs.rotation * Quaternion.Inverse(rhs.rotation);
        duplicant.scale = new Vector3(
            rhs.scale.x == 0 ? 0 : lhs.scale.x / rhs.scale.x,
            rhs.scale.y == 0 ? 0 : lhs.scale.y / rhs.scale.y,
            rhs.scale.z == 0 ? 0 : lhs.scale.z / rhs.scale.z);
        duplicant.color = lhs.color - rhs.color;
        duplicant.alpha = lhs.alpha - rhs.alpha;
        return duplicant;
    }

    public static OGAnimationState Add(OGAnimationState lhs, OGAnimationState rhs)
    {
        if (lhs.isCanvasState != rhs.isCanvasState)
            Debug.LogWarning("Animation states incompatible: one is a UI state, while another is not");

        var duplicant = lhs.Duplicate();
        duplicant.isAbsolute = lhs.isAbsolute || rhs.isAbsolute;
        duplicant.isCanvasState = lhs.isCanvasState && rhs.isCanvasState;
        duplicant.active = Mathf.Clamp(lhs.active + rhs.active, -1f, 1f); // only the end state of stateAdd matters

        duplicant.position = lhs.position + rhs.position;
        duplicant.rotation = lhs.rotation * rhs.rotation;
        duplicant.scale = new Vector3(
            lhs.scale.x * rhs.scale.x,
            lhs.scale.y * rhs.scale.y,
            lhs.scale.z * rhs.scale.z);
        duplicant.color = lhs.color + rhs.color;
        duplicant.alpha = lhs.alpha + rhs.alpha;
        return duplicant;
    }

    public static OGAnimationState Lerp(OGAnimationState stateFrom, OGAnimationState stateTo, float percent)
    {
        return Lerp(stateFrom, stateTo, null, percent);
    }

    public static OGAnimationState Lerp(OGAnimationState stateFrom, OGAnimationState stateTo, OGAnimationCurvesPreset curves, float percent)
    {
        if (stateFrom.isCanvasState != stateTo.isCanvasState)
            Debug.LogWarning("Animation states incompatible: one is a UI state, while another is not");

        var result = stateFrom.Duplicate();
        result.isAbsolute = stateFrom.isAbsolute && stateTo.isAbsolute;
        result.isCanvasState = stateFrom.isCanvasState && stateTo.isCanvasState;

        var lerp = curves != null ? curves.Evaluate(percent) : new OGAnimationLerpState(percent);

        result.active = Lerp(stateFrom.active, stateTo.active, lerp.percent);

        result.position = new Vector3(
            Lerp(stateFrom.position.x, stateTo.position.x, lerp.positionLerp.x),
            Lerp(stateFrom.position.y, stateTo.position.y, lerp.positionLerp.y),
            Lerp(stateFrom.position.z, stateTo.position.z, lerp.positionLerp.z));

        result.scale = new Vector3(
            Lerp(stateFrom.scale.x, stateTo.scale.x, lerp.scaleLerp.x),
            Lerp(stateFrom.scale.y, stateTo.scale.y, lerp.scaleLerp.y),
            Lerp(stateFrom.scale.z, stateTo.scale.z, lerp.scaleLerp.z));

        result.rotation = Quaternion.LerpUnclamped(stateFrom.rotation, stateTo.rotation, lerp.rotationLerp);
        result.color = stateFrom.color * (1f - lerp.colorLerp) + stateTo.color * lerp.colorLerp;
        result.alpha = stateFrom.alpha * (1f - lerp.alphaLerp) + stateTo.alpha * lerp.alphaLerp;
        return result;
    }

    private static float Lerp(float x, float y, float t)
    {
        return x + (y - x) * t;
    }

    public OGAnimationState SetActive(bool active)
    {
        this.active = active ? 1f : 0f;
        return this;
    }

    public OGAnimationState Translate(Vector3 translate)
    {
        position += translate;
        return this;
    }

    public OGAnimationState Rotate(Quaternion quaternion)
    {
        rotation = quaternion * rotation;
        return this;
    }

    public OGAnimationState Rotate(float x, float y, float z)
    {
        return Rotate(Quaternion.Euler(x, y, z));
    }

    public OGAnimationState Rotate(Vector3 eulerAngles)
    {
        return Rotate(eulerAngles.x, eulerAngles.y, eulerAngles.z);
    }

    public OGAnimationState Rotate(float z)
    {
        return Rotate(0, 0, z);
    }

    public OGAnimationState Scale(float x, float y, float z)
    {
        scale = new Vector3(scale.x * x, scale.y * y, scale.z * z);
        return this;
    }

    public OGAnimationState Scale(Vector3 scale)
    {
        return Scale(scale.x, scale.y, scale.z);
    }

    public OGAnimationState Scale(float scale)
    {
        return Scale(0, 0, scale);
    }

    public OGAnimationState SetColor(Vector4 color)
    {
        this.color = color;
        return this;
    }

    public OGAnimationState SetAlpha(float alpha)
    {
        this.alpha = alpha;
        return this;
    }

    public void CopyActiveFrom(Component component)
    {
        active = component.gameObject.activeSelf ? 1f : 0f;
    }

    public void CopyActiveFrom(GameObject gameObject)
    {
        active = gameObject.activeSelf ? 1f : 0f;
    }

    public void CopyFromTransform(Transform transform)
    {
        if (transform is RectTransform)
        {
            CopyFromRectTransform(transform as RectTransform);
            return;
        }

        isCanvasState = false;
        position = transform.localPosition;
        rotation = transform.localRotation;
        scale = transform.localScale;
    }

    public void CopyFromRectTransform(RectTransform transform)
    {
        isCanvasState = true;
        position = transform.anchoredPosition3D;
        rotation = transform.localRotation;
        scale = transform.localScale;
    }

    public void CopyAlphaFrom(CanvasGroup group)
    {
        alpha = group.alpha;
    }

    public void CopyAlphaFrom(Image image)
    {
        alpha = image.color.a;
    }

    public void CopyColorFrom(Image image)
    {
        color = image.color;
    }

    public void ApplyActiveTo(Component component)
    {
        ApplyActiveTo(component.gameObject);
    }

    public void ApplyActiveTo(GameObject gameObject)
    {
        if (active >= Mathf.Epsilon)
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
        }
        else
        {
            if (gameObject.activeSelf)
                gameObject.SetActive(false);
        }
    }

    public void ApplyToTransform(Transform transform, Vector3Mask applyPosition = default(Vector3Mask), Vector3Mask applyRotation = default(Vector3Mask), Vector3Mask applyScale = default(Vector3Mask))
    {
        if (isCanvasState)
        {
            if (transform is RectTransform)
            {
                ApplyToRectTransform(transform as RectTransform, applyPosition, applyRotation, applyScale);
                return;
            }
            else
            {
                Debug.LogWarning("Animation state is meant for canvas animation, but not applied as such.");
            }
        }

        transform.localPosition = applyPosition.CopyValues(position, transform.localPosition);
        transform.localEulerAngles = applyRotation.CopyValues(rotation.eulerAngles, transform.localEulerAngles);
        transform.localScale = applyScale.CopyValues(scale, transform.localScale);
    }

    public void ApplyToRectTransform(RectTransform transform, Vector3Mask applyPosition = default(Vector3Mask), Vector3Mask applyRotation = default(Vector3Mask), Vector3Mask applyScale = default(Vector3Mask))
    {
        transform.anchoredPosition3D = applyPosition.CopyValues(position, transform.anchoredPosition3D);
        transform.localEulerAngles = applyRotation.CopyValues(rotation.eulerAngles, transform.localEulerAngles);
        transform.localScale = applyScale.CopyValues(scale, transform.localScale);
    }

    public void ApplyAlphaTo(CanvasGroup group)
    {
        group.alpha = alpha;
    }

    public void ApplyAlphaTo(Image image)
    {
        var temp = image.color;
        temp.a = alpha;
        image.color = temp;
    }

    public void ApplyColorTo(Image image)
    {
        image.color = color;
    }

    public void ApplyTo(OGAnimationTargets targets, OGAnimationMask mask, float lerp = 1f, OGAnimationState lerpWith = default(OGAnimationState))
    {
        var state = this;

        // lerp if needed
        if (lerp < 1f)
        {
            // var current = new OGAnimationState(targets);
            state = Lerp(lerpWith, this, lerp);
        }
        
        if (mask.active && targets.gameObject != null)
            state.ApplyActiveTo(targets.gameObject);

        if (targets.transform != null)
            state.ApplyToTransform(targets.transform, mask.position, mask.rotation, mask.scale);

        if (mask.color && targets.image != null)
            state.ApplyColorTo(targets.image);

        if (mask.alpha)
        {
            if (targets.canvasGroup != null) state.ApplyAlphaTo(targets.canvasGroup);
            else if (targets.image != null) state.ApplyAlphaTo(targets.image);
        }
    }
}
