using System.Collections.Generic;
using UnityEngine;

public class SpecialFX : MonoBehaviour
{
    readonly Dictionary<string, GameObject> SpecialFXDict = new Dictionary<string, GameObject>();
    public Transform fxParent;
    private SpriteRenderer spriteRenderer;
    public void Awake()
    {
        TryGetComponent(out spriteRenderer);
    }
    public bool HaveThisFX(string ID)
    {
        return SpecialFXDict.ContainsKey(ID);
    }
    public void AddFX(TimedEffect fx)
    {
        GameObject go;
        switch (fx.Effect)
        {
            case SlowEffect _:
                if (fx.Effect.ID.Contains("SL"))
                {
                    //Debug.Log(spriteRenderer.sprite.rect.height/2);
                    go = Instantiate(fx.Effect.specialFX, fxParent.transform.position - new Vector3(0, 0.2f, 0), Quaternion.identity, fxParent);
                    SpecialFXDict.Add(fx.Effect.ID, go);
                }
                else
                {
                    //Debug.Log(spriteRenderer.sprite.rect.height / 2);
                    go = Instantiate(fx.Effect.specialFX, fxParent.transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity, fxParent);
                    SpecialFXDict.Add(fx.Effect.ID, go);
                }
                break;
            case DotsEffect _:
                if (fx.Effect.ID.Contains("BU"))
                {
                    go = Instantiate(fx.Effect.specialFX, fxParent.transform.position, Quaternion.identity, fxParent);
                    SpecialFXDict.Add(fx.Effect.ID, go);
                }
                else if (fx.Effect.ID.Contains("HEAL"))
                {
                    //go = Instantiate(fx.Effect.specialFX, fxParent.transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity, fxParent);
                    //SpecialFXDict.Add(fx.Effect.ID, go);
                }
                else if (fx.Effect.ID.Contains("POI"))
                {
                    go = Instantiate(fx.Effect.specialFX, fxParent.transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity, fxParent);
                    SpecialFXDict.Add(fx.Effect.ID, go);
                }
                else// if(fx.Effect.ID.Contains("POI"))
                {
                    go = Instantiate(fx.Effect.specialFX, fxParent.transform.position + new Vector3(0.5f, 0.5f, 0), Quaternion.identity, fxParent);
                    SpecialFXDict.Add(fx.Effect.ID, go);
                }
                break;
            case FearEffect _:
                go = Instantiate(fx.Effect.specialFX, fxParent.transform.position + new Vector3(0, 2, 0), Quaternion.identity, fxParent);
                SpecialFXDict.Add(fx.Effect.ID, go);
                break;
            case Weaken _:
                go = Instantiate(fx.Effect.specialFX, fxParent.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity, fxParent);
                SpecialFXDict.Add(fx.Effect.ID, go);
                break;
            case Revive _:
                Instantiate(fx.Effect.specialFX, fxParent.transform.position, Quaternion.identity, fxParent);
                break;
            case ArmorBreaking _:
                go = Instantiate(fx.Effect.specialFX, fxParent.transform.position + new Vector3(0, 1, 0), Quaternion.identity, fxParent);
                SpecialFXDict.Add(fx.Effect.ID, go);
                break;
            default:
                break;
        }
    }
    public void RemoveFX(string ID)
    {
        if (SpecialFXDict.ContainsKey(ID))
        {
            Destroy(SpecialFXDict[ID]);
        }
        SpecialFXDict.Remove(ID);
    }
    public void RemoveAllFX()
    {
        foreach (KeyValuePair<string, GameObject> fx in SpecialFXDict)
        {
            Destroy(SpecialFXDict[fx.Key]);
        }
        SpecialFXDict.Clear();
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
