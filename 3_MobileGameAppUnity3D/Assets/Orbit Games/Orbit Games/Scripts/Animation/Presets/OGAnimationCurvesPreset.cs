using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Curve", menuName = "Animation/Curves Set preset", order = 1)]
public class OGAnimationCurvesPreset : ScriptableObject {
    
    public OGAnimationCurvesPreset()
    {
        Reset();
    }

    //[Header("Main curve")]
    //[Buttons("Reset")]
    //public ButtonsContainer reset;

    [Header("Level of control")]
    public PropertyPreciseControl propertyControl;
    public TimeMappingsEnabled timeMappingControl;
    [ConditionalField("propertyControl", PropertyPreciseControl.ControlEachPropertySeparately)]
    public VectorPreciseControl vectorControl;

    [Header("Animation Curves Setup")]
    [ConditionalField("propertyControl", PropertyPreciseControl.ControlEntireAnimationWithOneCurve, "timeMappingControl", TimeMappingsEnabled.NoTimeWarpingControl)]
    public StaticTimeCurvePreset animateAllProperties;
    [ConditionalField("propertyControl", PropertyPreciseControl.ControlEntireAnimationWithOneCurve, "timeMappingControl", TimeMappingsEnabled.ShowTimeWarpingControls)]
    public TimeWarpableCurvePreset animateAllPropertiesWarpable;

    [ConditionalField("propertyControl", PropertyPreciseControl.ControlEachPropertySeparately, "timeMappingControl", TimeMappingsEnabled.NoTimeWarpingControl, "vectorControl", VectorPreciseControl.ControlEntireVectorWithOneCurve)]
    public StaticTimeCurvePreset animatePosition;
    [ConditionalField("propertyControl", PropertyPreciseControl.ControlEachPropertySeparately, "timeMappingControl", TimeMappingsEnabled.ShowTimeWarpingControls, "vectorControl", VectorPreciseControl.ControlEntireVectorWithOneCurve)]
    public TimeWarpableCurvePreset animatePositionWarpable;
    [ConditionalField("propertyControl", PropertyPreciseControl.ControlEachPropertySeparately, "timeMappingControl", TimeMappingsEnabled.NoTimeWarpingControl, "vectorControl", VectorPreciseControl.ControlEachComponentSeparately)]
    public StaticTimeVector3CurvePreset animatePositionComponents;
    [ConditionalField("propertyControl", PropertyPreciseControl.ControlEachPropertySeparately, "timeMappingControl", TimeMappingsEnabled.ShowTimeWarpingControls, "vectorControl", VectorPreciseControl.ControlEachComponentSeparately)]
    public TimeWarpableVector3CurvePreset animatePositionComponentsWarpable;

    [ConditionalField("propertyControl", PropertyPreciseControl.ControlEachPropertySeparately, "timeMappingControl", TimeMappingsEnabled.NoTimeWarpingControl)]
    public StaticTimeCurvePreset animateRotation;
    [ConditionalField("propertyControl", PropertyPreciseControl.ControlEachPropertySeparately, "timeMappingControl", TimeMappingsEnabled.ShowTimeWarpingControls)]
    public TimeWarpableCurvePreset animateRotationWarpable;

    [ConditionalField("propertyControl", PropertyPreciseControl.ControlEachPropertySeparately, "timeMappingControl", TimeMappingsEnabled.NoTimeWarpingControl, "vectorControl", VectorPreciseControl.ControlEntireVectorWithOneCurve)]
    public StaticTimeCurvePreset animateScale;
    [ConditionalField("propertyControl", PropertyPreciseControl.ControlEachPropertySeparately, "timeMappingControl", TimeMappingsEnabled.ShowTimeWarpingControls, "vectorControl", VectorPreciseControl.ControlEntireVectorWithOneCurve)]
    public TimeWarpableCurvePreset animateScaleWarpable;
    [ConditionalField("propertyControl", PropertyPreciseControl.ControlEachPropertySeparately, "timeMappingControl", TimeMappingsEnabled.NoTimeWarpingControl, "vectorControl", VectorPreciseControl.ControlEachComponentSeparately)]
    public StaticTimeVector3CurvePreset animateScaleComponents;
    [ConditionalField("propertyControl", PropertyPreciseControl.ControlEachPropertySeparately, "timeMappingControl", TimeMappingsEnabled.ShowTimeWarpingControls, "vectorControl", VectorPreciseControl.ControlEachComponentSeparately)]
    public TimeWarpableVector3CurvePreset animateScaleComponentsWarpable;

    [ConditionalField("propertyControl", PropertyPreciseControl.ControlEachPropertySeparately, "timeMappingControl", TimeMappingsEnabled.NoTimeWarpingControl)]
    public StaticTimeCurvePreset animateColor;
    [ConditionalField("propertyControl", PropertyPreciseControl.ControlEachPropertySeparately, "timeMappingControl", TimeMappingsEnabled.ShowTimeWarpingControls)]
    public TimeWarpableCurvePreset animateColorWarpable;

    [ConditionalField("propertyControl", PropertyPreciseControl.ControlEachPropertySeparately, "timeMappingControl", TimeMappingsEnabled.NoTimeWarpingControl)]
    public StaticTimeCurvePreset animateAlpha;
    [ConditionalField("propertyControl", PropertyPreciseControl.ControlEachPropertySeparately, "timeMappingControl", TimeMappingsEnabled.ShowTimeWarpingControls)]
    public TimeWarpableCurvePreset animateAlphaWarpable;

    public void Reset()
    {
        animateAllProperties = new StaticTimeCurvePreset();
        animateAllPropertiesWarpable = new TimeWarpableCurvePreset();
        animatePosition = new StaticTimeCurvePreset();
        animatePositionWarpable = new TimeWarpableCurvePreset();
        animatePositionComponents = new StaticTimeVector3CurvePreset();
        animatePositionComponentsWarpable = new TimeWarpableVector3CurvePreset();
        animateRotation = new StaticTimeCurvePreset();
        animateRotationWarpable = new TimeWarpableCurvePreset();
        animateScale = new StaticTimeCurvePreset();
        animateScaleWarpable = new TimeWarpableCurvePreset();
        animateScaleComponents = new StaticTimeVector3CurvePreset();
        animateScaleComponentsWarpable = new TimeWarpableVector3CurvePreset();
        animateColor = new StaticTimeCurvePreset();
        animateColorWarpable = new TimeWarpableCurvePreset();
        animateAlpha = new StaticTimeCurvePreset();
        animateAlphaWarpable = new TimeWarpableCurvePreset();
    }

    public float GetMaxWarpedDuration(float duration)
    {
        switch (propertyControl)
        {
            case PropertyPreciseControl.ControlEntireAnimationWithOneCurve:

                switch (timeMappingControl)
                {
                    case TimeMappingsEnabled.ShowTimeWarpingControls:
                        return animateAllPropertiesWarpable.durationModifier * duration;
                }

                break;

            case PropertyPreciseControl.ControlEachPropertySeparately:

                switch (timeMappingControl)
                {
                    case TimeMappingsEnabled.ShowTimeWarpingControls:

                        switch (vectorControl)
                        {
                            case VectorPreciseControl.ControlEntireVectorWithOneCurve:
                                return Mathf.Max(
                                    animatePositionWarpable.durationModifier,
                                    animateRotationWarpable.durationModifier,
                                    animateScaleWarpable.durationModifier,
                                    animateColorWarpable.durationModifier,
                                    animateAlphaWarpable.durationModifier) * duration;

                            case VectorPreciseControl.ControlEachComponentSeparately:
                                return Mathf.Max(
                                    animatePositionComponentsWarpable.GetMaxDurationModifier(),
                                    animateRotationWarpable.durationModifier,
                                    animateScaleComponentsWarpable.GetMaxDurationModifier(),
                                    animateColorWarpable.durationModifier,
                                    animateAlphaWarpable.durationModifier) * duration;
                        }

                        break;
                }
                break;
        }

        return duration;
    }

    public OGAnimationLerpState Evaluate(float percent)
    {
        switch (propertyControl)
        {
            case PropertyPreciseControl.ControlEntireAnimationWithOneCurve:

                switch (timeMappingControl)
                {
                    case TimeMappingsEnabled.NoTimeWarpingControl:
                        return new OGAnimationLerpState(animateAllProperties.Evaluate(percent));

                    case TimeMappingsEnabled.ShowTimeWarpingControls:
                        return new OGAnimationLerpState(animateAllPropertiesWarpable.Evaluate(percent));
                }

                break;

            case PropertyPreciseControl.ControlEachPropertySeparately:

                switch (timeMappingControl)
                {
                    case TimeMappingsEnabled.NoTimeWarpingControl:

                        switch (vectorControl)
                        {
                            case VectorPreciseControl.ControlEntireVectorWithOneCurve:
                                return new OGAnimationLerpState(percent,
                                        animatePosition.Evaluate(percent),
                                        animateRotation.Evaluate(percent),
                                        animateScale.Evaluate(percent),
                                        animateColor.Evaluate(percent),
                                        animateAlpha.Evaluate(percent));

                            case VectorPreciseControl.ControlEachComponentSeparately:
                                return new OGAnimationLerpState(percent,
                                        animatePositionComponents.Evaluate(percent),
                                        animateRotation.Evaluate(percent),
                                        animateScaleComponents.Evaluate(percent),
                                        animateColor.Evaluate(percent),
                                        animateAlpha.Evaluate(percent));
                        }

                        break;

                    case TimeMappingsEnabled.ShowTimeWarpingControls:

                        switch (vectorControl)
                        {
                            case VectorPreciseControl.ControlEntireVectorWithOneCurve:
                                return new OGAnimationLerpState(percent,
                                        animatePositionWarpable.Evaluate(percent),
                                        animateRotationWarpable.Evaluate(percent),
                                        animateScaleWarpable.Evaluate(percent),
                                        animateColorWarpable.Evaluate(percent),
                                        animateAlphaWarpable.Evaluate(percent));

                            case VectorPreciseControl.ControlEachComponentSeparately:
                                return new OGAnimationLerpState(percent,
                                        animatePositionComponentsWarpable.Evaluate(percent),
                                        animateRotationWarpable.Evaluate(percent),
                                        animateScaleComponentsWarpable.Evaluate(percent),
                                        animateColorWarpable.Evaluate(percent),
                                        animateAlphaWarpable.Evaluate(percent));
                        }

                        break;
                }
                break;
        }

        return null;
    }

    public enum PropertyPreciseControl
    {
        ControlEntireAnimationWithOneCurve, ControlEachPropertySeparately
    }

    public enum VectorPreciseControl
    {
        ControlEntireVectorWithOneCurve, ControlEachComponentSeparately
    }

    public enum TimeMappingsEnabled
    {
        NoTimeWarpingControl, ShowTimeWarpingControls
    }

    [System.Serializable]
    public class StaticTimeCurvePreset
    {
        public OGAnimationCurvePreset curvePreset;

        public float Evaluate(float percent)
        {
            return curvePreset != null ? curvePreset.curve.Evaluate(percent) : percent;
        }
    }

    [System.Serializable]
    public class StaticTimeVector3CurvePreset
    {
        public OGAnimationCurvePreset xCurvePreset;
        public OGAnimationCurvePreset yCurvePreset;
        public OGAnimationCurvePreset zCurvePreset;

        public Vector3 Evaluate(float percent)
        {
            return new Vector3(
                xCurvePreset != null ? xCurvePreset.curve.Evaluate(percent) : percent,
                yCurvePreset != null ? yCurvePreset.curve.Evaluate(percent) : percent,
                zCurvePreset != null ? zCurvePreset.curve.Evaluate(percent) : percent);
        }
    }

    [System.Serializable]
    public class TimeWarpableCurvePreset
    {
        [Unit("times")]
        public float durationModifier;
        public OGAnimationTimeMapperPreset timeWarp1;
        public OGAnimationTimeMapperPreset timeWarp2;
        public OGAnimationCurvePreset curvePreset;

        public float Evaluate(float percent)
        {
            percent *= durationModifier;
            if (timeWarp1 != null) percent = timeWarp1.curve.Evaluate(percent);
            if (timeWarp2 != null) percent = timeWarp2.curve.Evaluate(percent);

            return curvePreset != null ? curvePreset.curve.Evaluate(percent) : percent;
        }

        public TimeWarpableCurvePreset()
        {
            durationModifier = 1f;
        }
    }

    [System.Serializable]
    public class TimeWarpableVector3CurvePreset
    {
        [Header("X")]
        [Unit("times")]
        public float xDurationModifier;
        public OGAnimationTimeMapperPreset xTimeWarp1;
        public OGAnimationTimeMapperPreset xTimeWarp2;
        public OGAnimationCurvePreset xCurvePreset;

        [Header("Y")]
        [Unit("times")]
        public float yDurationModifier;
        public OGAnimationTimeMapperPreset yTimeWarp1;
        public OGAnimationTimeMapperPreset yTimeWarp2;
        public OGAnimationCurvePreset yCurvePreset;

        [Header("Z")]
        [Unit("times")]
        public float zDurationModifier;
        public OGAnimationTimeMapperPreset zTimeWarp1;
        public OGAnimationTimeMapperPreset zTimeWarp2;
        public OGAnimationCurvePreset zCurvePreset;

        public float GetMaxDurationModifier()
        {
            return Mathf.Max(xDurationModifier, yDurationModifier, zDurationModifier);
        }

        public Vector3 Evaluate(float percent)
        {
            var xPercent = percent * xDurationModifier;
            if (xTimeWarp1 != null) xPercent = xTimeWarp1.curve.Evaluate(xPercent);
            if (xTimeWarp2 != null) xPercent = xTimeWarp2.curve.Evaluate(xPercent);

            var yPercent = percent * yDurationModifier;
            if (yTimeWarp1 != null) yPercent = yTimeWarp1.curve.Evaluate(yPercent);
            if (yTimeWarp2 != null) yPercent = yTimeWarp2.curve.Evaluate(yPercent);
            
            var zPercent = percent * zDurationModifier;
            if (zTimeWarp1 != null) zPercent = zTimeWarp1.curve.Evaluate(zPercent);
            if (zTimeWarp2 != null) zPercent = zTimeWarp2.curve.Evaluate(zPercent);
            
            return new Vector3(
                xCurvePreset != null ? xCurvePreset.curve.Evaluate(xPercent) : xPercent,
                yCurvePreset != null ? yCurvePreset.curve.Evaluate(yPercent) : yPercent,
                zCurvePreset != null ? zCurvePreset.curve.Evaluate(zPercent) : zPercent);
        }

        public TimeWarpableVector3CurvePreset()
        {
            xDurationModifier = 1f;
            yDurationModifier = 1f;
            zDurationModifier = 1f;
        }
    }
}
