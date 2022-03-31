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
    Dictionary<string, Image> dict = new Dictionary<string, Image>();
    public Image image;
    public Transform parent;

    public bool HaveThis(string ID)
    {
        return dict.ContainsKey(ID);
    }

    public void AddIcon(BaseEffect bfx)
    {
        Image img = Instantiate(image,parent);
        img.sprite = bfx.sprite;
        /*if (bfx.effectType.HasFlag(EffectType.StackingEffect))
        {
            img.GetComponentInChildren<TMP_Text>().text = ;
        }*/
        //Debug.Log(img.sprite.name);

        dict.Add(bfx.ID, img);
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
