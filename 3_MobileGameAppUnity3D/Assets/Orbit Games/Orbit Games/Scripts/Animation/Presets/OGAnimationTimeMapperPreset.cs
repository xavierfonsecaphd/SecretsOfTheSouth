using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Time Mapper", menuName = "Animation/Time Mapper", order = 1)]
public class OGAnimationTimeMapperPreset : ScriptableObject {
    [Height(100f)]
    public AnimationCurve curve;
}
