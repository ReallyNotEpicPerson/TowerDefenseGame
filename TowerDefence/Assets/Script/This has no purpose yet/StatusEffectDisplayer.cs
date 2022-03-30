using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectDisplayer : MonoBehaviour
{
    //public List<Component> componentList;
    private EntityEffectHandler effectHandler;
    private List<EntityEffectHandler> effectHandlers;
    public GameObject anObject;
    //private Dictionary<BaseEffect, int> effect;
    public Text debugWindow;
    StringBuilder sb= new StringBuilder();

    /*private void OnValidate()
    {
        extractComponent();
    }*/
    /*public void SetObject(GameObject gObj)//will use soon,like very soon
    {   
        effectHandlers.Add(ExtractComponent(gObj));
    }*/
    public void SetObject(GameObject gObj)
    {
        anObject = gObj;
        ExtractComponent();
    }
    public EntityEffectHandler ExtractComponent(GameObject gObj)
    {
        foreach (var component in gObj.GetComponents<Component>())
        {
            //componentList.Add(component);
            if (component is EntityEffectHandler handler)
            {
                //TurretEffectManager FxManager = (TurretEffectManager)component;
                return handler;
            }
        }
        return null;
    }
    public void ExtractComponent()
    {
        foreach (var component in anObject.GetComponents<Component>())
        {
            //componentList.Add(component);
            if (component is EntityEffectHandler handler)
            {
                effectHandler = handler;
                //TurretEffectManager FxManager = (TurretEffectManager)component;
                return;
            }
        }
    }
    public void CheckStatusEffectDisplay(TimedEffect fx)
    {
        sb.Clear();
        if (fx is TimedBurnEffect tbe)
        {
            sb.Append(tbe.Display());
        }
        else if (fx is TimedSlowEffect tse)
        {
            sb.Append(tse.Display());
        }
        Template(sb);
        /*
        else if (fx is TimedFearEffect)
        {
            Template(fx.effect.ID, "BurnEffect", fx.GetDuration(), fx.effect.description);
        }*/
    }
    public void Template(StringBuilder data)
    {
        debugWindow.text = data.ToString(); 
    }
    string DescriptionTemplate(int temp)//might use this???
    {
        switch (temp)
        {
            case 1:
                return "";
            default:
                break;
        }
        return "";
    }

    public void Update()
    {
        if (anObject == null)
        {
            return;
        }
        if (effectHandler._effectList.Count == 0)
        {
            return;
        }
        foreach (var effect in effectHandler._effectList.Values.ToList())
        {
            CheckStatusEffectDisplay(effect);
        }
    }
}
