using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ArmorType
{
    None,
    Single,
    Multiple,
}
public class Armor : MonoBehaviour
{
    public ArmorType armorType;
    [SerializeField] private float startArmor = 100f;
    [SerializeField] private float damangeReductionPtc = 0;
    public int MaxLayer = 1;
    [SerializeField] private Image armorBar;
    public TMP_Text layerNum;
    private float armor;
    private int layer;

    private void OnValidate()
    {
        if (armorType.HasFlag(ArmorType.Multiple))
        {
            armor = startArmor * MaxLayer;
            if (MaxLayer - 1 > 0)
            {
                layerNum.text = (MaxLayer - 1).ToString();
            }
            ArmorBarAdjust(true);
        }
        else if (armorType.HasFlag(ArmorType.Single))
        {
            layerNum.text = "";
            armor = startArmor;
            MaxLayer = 1;
            ArmorBarAdjust(true);
        }
        else if (armorType.HasFlag(ArmorType.None))
        {
            layerNum.text = "";
            MaxLayer = 0;
            ArmorBarAdjust(false);
        }
    }
    private void Awake()
    {
        SetArmorType();
    }
    void SetArmorType()
    {
        if (armorType.HasFlag(ArmorType.Multiple))
        {
            armor = startArmor * MaxLayer;
            if (MaxLayer - 1 > 0)
            {
                layerNum.text = (MaxLayer - 1).ToString();
            }
            MaxLayer = 1;
            ArmorBarAdjust(true);
        }
        else if (armorType.HasFlag(ArmorType.Single))
        {
            layerNum.text = "";
            armor = startArmor;
            MaxLayer = 1;
            ArmorBarAdjust(true);
        }
        else if (armorType.HasFlag(ArmorType.None))
        {
            layerNum.text = "";
            MaxLayer = 0;
            ArmorBarAdjust(false);
        }
        armorBar.fillAmount = armor / startArmor;
    }
    public float Refresh()
    {
        if (armorType.HasFlag(ArmorType.Single))
        {
            return armor;
        }
        else
        {
            layer = (int)(armor / startArmor);
            return armor - (int)(armor / startArmor) * startArmor;
        }  
    }
    public bool DamageArmor(float amount)
    {
        DamageDisplayer.Create(transform.position,(amount * (1f - damangeReductionPtc)),DamageDisplayerType.ArmorHit);
        armor -= amount * (1f - damangeReductionPtc);
        FillBar(Refresh());
        if (armor <= 0)
            return false;
        return true;
    }
    public void restoreArmor()
    {
        SetArmorType();
    }
    public void AddArmor(float amount)
    {
        armor += amount;
        ArmorBarAdjust(true);
    }
    public void FillBar(float amount)
    {
        armorBar.fillAmount = amount / startArmor;
        if (layer == 0|| armorType.HasFlag(ArmorType.Single))
            layerNum.text = "";
        else
        {
            layerNum.text = layer.ToString();
        }
    }
    public void ArmorBarAdjust(bool state)
    {
        armorBar.transform.parent.gameObject.SetActive(state);
        //Debug.Log(armorBar.transform.parent.name);
    }


    public void DisableState(ArmorType es)
    {
        armorType &= ~es;
    }
    public void EnableState(ArmorType es)
    {
        armorType |= es;
    }
}
