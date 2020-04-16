using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOGAnimatable
{
    void Animate(int animationLayer = 0, string tag = "", float speed = 1f);
    void Animate(string tag = "", float speed = 1f);
    void Animate(float speed = 1f);
}
