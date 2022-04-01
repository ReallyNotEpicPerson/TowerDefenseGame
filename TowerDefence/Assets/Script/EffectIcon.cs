using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    readonly Dictionary<string, Image> dict = new Dictionary<string, Image>();
    readonly Dictionary<string, GameObject> gameObjectdict = new Dictionary<string, GameObject>();
    public Image image;
    public Transform parent;

    public bool HaveThis(string ID)
    {
        return dict.ContainsKey(ID);
    }

    public void AddIcon(TimedEffect fx)
    {
        Image img = Instantiate(image,parent);
        img.sprite = fx.Effect.Icon;
        dict.Add(fx.Effect.ID, img);
        //Debug.Log(img.sprite.name);
        if(fx.Effect.specialFX != null)
        {
            ApplySpecialEffect(fx.Effect.specialFX);
        }
    }
    
    public void ApplySpecialEffect(GameObject pfx)
    {
        Instantiate(pfx,transform.position,Quaternion.identity);
    }

    public void SetStack(TimedEffect fx)
    {
        TMP_Text text = dict[fx.Effect.ID].GetComponentInChildren<TMP_Text>();
        Debug.Log(fx.GetStack());
        if (fx.GetStack()>0)
        {
            text.text=fx.GetStack().ToString();
        }
    }

    public void DeleteIcon(string ID)
    {
        Destroy(dict[ID].gameObject);
        dict.Remove(ID);
    }
    public void DeleteAllICon()
    {
        foreach (KeyValuePair<string, Image> icon in dict)
        {
            Destroy(dict[icon.Key].gameObject);
        }
        dict.Clear();
    }
    /*
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
*/
}
