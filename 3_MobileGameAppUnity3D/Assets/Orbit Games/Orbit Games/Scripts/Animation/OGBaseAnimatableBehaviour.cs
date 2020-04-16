using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OGBaseAnimatableBehaviour : MonoBehaviour {
    
    [QuickButtons]
    public ButtonsContainer quickButtons;
    
    [Header("Control")]
    [DynamicButtons("GetTestButtons")]
    public ButtonsContainer testButtons;
    [Buttons(true, "► Play 5 second demo", "PlayDemo")]
    public ButtonsContainer demoButtons;
    [Buttons("● Set origin", "WriteActiveState", "◀ Return", "ReadActiveState")]
    public ButtonsContainer recorder;
    public EditorHelpBox errorMessage = EditorHelpBox.None();

    protected int demoID = 0;
    private void StopDemo()
    {
        demoID++; // incrementing will cause any active demo to not trigger on the delayed call 
    }

    public List<EditorButton> GetTestButtons()
    {
        List<EditorButton> buttons = GetPlayTestButtons();
        buttons.Add(new EditorButton("■ Stop", "StopActiveAnimations"));
        return buttons;
    }

    protected abstract List<EditorButton> GetPlayTestButtons();
    public abstract void PlayDemo();
    protected abstract void OnEditorUpdate();

    [Header("Settings")]
    public bool unscaledTime;
    public bool stopAnimationsOnDisable = true;
    public bool startDisabled;
    public bool endDisabled;
    public bool endInPool;
    public OGAnimationTargets animationTargets;
    public OGAnimationMask animatedProperties;
    public OGAnimationState origin;
    [HideInInspector]
    public bool originDefined = false;

    [Header("Tag")]
    public string activeTag;

    private HashSet<int> activeAnimationIDs = new HashSet<int>();

    private void OnDisable()
    {
        if (stopAnimationsOnDisable) 
            StopActiveAnimations();
    }

    public int GetAnimationPlayCount
    {
        get
        {
            return activeAnimationIDs.Count;
        }
    }

    public bool IsPlaying
    {
        get
        {
            return activeAnimationIDs.Count > 0;
        }
    }

    public void WriteActiveState()
    {
        origin = new OGAnimationState(animationTargets);
        originDefined = true;
    }

    public void DetermineAnimatableTargets()
    {
        animationTargets = new OGAnimationTargets(gameObject);
    }

    public void DetectAnimatableProperties(bool detectColorsOnly = false)
    {
        if (!detectColorsOnly)
        {
            animatedProperties.active = false;// true;
            animatedProperties.position = Vector3Mask.W;
            animatedProperties.rotation = Vector3Mask.W;
            animatedProperties.scale = Vector3Mask.W;
        }

        animatedProperties.color = animationTargets.image != null;
        animatedProperties.alpha = animationTargets.canvasGroup != null;
    }

    public void ReadActiveState()
    {
        origin.ApplyTo(animationTargets, animatedProperties);
    }

    protected OGAnimationTask CreateAnimationTask(OGStateAnimation animationSetup, float speed, OGLoopType loop, bool randomStartTime, float fadeIn)
    {
        var animationTask = new OGAnimationTask
        {
            target = animationTargets,
            origin = origin,
            unscaledTime = unscaledTime,
            mask = animatedProperties,
            randomStartTime = randomStartTime,
            speed = speed,
            loop = loop,
            fadeIn = fadeIn,
            delay = animationSetup.baseDelay + animationSetup.childIndexDelay * transform.GetSiblingIndex(),
            animation = animationSetup
        };

        return animationTask;
    }

    public void StopActiveAnimations()
    {
        StopDemo();

        List<int> keys = OGListPool<int>.Get(activeAnimationIDs);
        for (int i = 0; i < keys.Count; i++)
        {
            OGAnimator.StopCoroutine(animationTargets.gameObject, keys[i]);
        }
        OGListPool<int>.Put(ref keys);

        ReadActiveState();
    }

    public OGAnimationTask ProcessAndPlayAnimationSetup(OGStateAnimation animationSetup, float speed, OGLoopType loop, bool randomStartTime, float fadeIn, OGAnimationTask.OnAnimationEndedListener onAnimationEnded = null)
    {
        if (animationSetup == null) return null;
        if (animationSetup.curves == null)
        {
            Debug.LogError("Could not play animation with tag '" + animationSetup.tag + "' on object '" + gameObject.name + "', because no curves were defined");
            return null;
        }

        if (animationSetup.autoOriginOnAnimTrigger)
        {
            WriteActiveState();
        }

        var animationTask = CreateAnimationTask(animationSetup, speed, loop, randomStartTime, fadeIn);

        if (onAnimationEnded != null)
        {
            animationTask.onAnimationEnded += onAnimationEnded;
        }

        var guessedAnimationID = OGAnimator.GetNextAnimationID(animationTargets.gameObject);
        activeAnimationIDs.Add(guessedAnimationID);
        animationTask.onAnimationEnded += (reason) => 
        {
            activeAnimationIDs.Remove(guessedAnimationID);
        };

        var returnedAnimationID = OGAnimator.Animate(animationTask);
        if (guessedAnimationID != returnedAnimationID)
        {
            Debug.LogError("Guessed animation ID did not match the actual received ID");
        }
        return animationTask;
    }
    
    [HideInInspector]
    public GameObject originalGameObject;
    public void EditorHelpBoxUpdate() { }
    private void OnDrawGizmosSelected()
    {
        errorMessage = EditorHelpBox.None();

        if (originalGameObject == null)
        {
            DetermineAnimatableTargets();
            DetectAnimatableProperties();
            WriteActiveState();

            originalGameObject = gameObject;
        }

        if (gameObject != originalGameObject)
        {
            Debug.LogWarning("Remembered parent GameObject of this component did not match the actual one, the targets have been reset to avoid animating an unwanted other GameObject!");
            DetermineAnimatableTargets();
            DetectAnimatableProperties(detectColorsOnly: true);
            originalGameObject = gameObject;
        }

        if (!originDefined)
            WriteActiveState();
        
        if (origin.IsEmpty())
        {
            errorMessage = EditorHelpBox.Error("There seems to have been no origin defined yet");
            return;
        }

        // scale all gizmos based upon scale of this object in world
        if (transform.parent != null)
        {
            Gizmos.matrix = transform.parent.localToWorldMatrix;
        }

        OnEditorUpdate();
    }
}
