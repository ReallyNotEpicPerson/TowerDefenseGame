using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class AbstractManager<T> : MonoBehaviour
{
    [SerializeField] protected T target;
    [SerializeField] List<T> effects;

}
