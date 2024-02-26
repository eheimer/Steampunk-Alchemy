using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallAndFadeHandler : MonoBehaviour
{
    public void AnimationFinished()
    {
        Destroy(gameObject);
    }
}
