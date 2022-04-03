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
    public void Destroy(float timer)
    {
        Destroy(gameObject,timer);
    }
    public void Destroy2()
    {
        Destroy(gameObject);
    }
    public void CallThePool()
    {

    }
}
