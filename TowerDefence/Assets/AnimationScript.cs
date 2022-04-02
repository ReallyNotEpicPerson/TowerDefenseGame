using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    Animator animator;
    public void Awake()
    {
        TryGetComponent(out animator);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
    public void CallThePool()
    {

    }
}
