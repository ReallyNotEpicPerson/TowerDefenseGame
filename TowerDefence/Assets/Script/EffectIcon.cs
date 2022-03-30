using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Flags]
public enum EffectIconState
{
    None = 0,
    Burn = 1 << 1,
    Weaken = 1 << 2,
    Slowed = 1 << 3,
    Revive = 1 << 4,
    Fear = 1 << 5,
    Cleanse = 1 << 6,
    Heal = 1 << 7,
}
public class EffectIcon : MonoBehaviour
{
    private Dictionary<Sprite, Image> dict;
    public Camera cam;
    public Image Image;
    public Transform parent;
    EffectIconState state;


    #region state flip
    public void DisableState(EffectIconState ies)
    {
        state &= ~ies;
    }
    public void EnableState(EffectIconState ies)
    {
        state |= ies;
    }
    #endregion
}
