using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class StatToolTips : MonoBehaviour
{
    [SerializeField] Text StatNameText;
    [SerializeField] Text StatSlotLabelText;
    [SerializeField] Text StatModifierText;

    private StringBuilder sb = new StringBuilder();

    public void Show(CharacterStat characterStat, string statName)
    {
        StatNameText.text = GetStatTopText(characterStat, statName);
        StatModifierText.text = GetStatModifierText(characterStat);
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    private string GetStatTopText(CharacterStat stat, string statName)
    {
        sb.Length = 0;
        sb.Append(statName);
        sb.Append(" ");
        sb.Append(stat.Value);
        if(stat.Value != stat.baseValue)
        {
            sb.Append(" (");
            sb.Append(stat.baseValue);

            if (stat.Value > stat.baseValue)
            {
                sb.Append("+");
            }
            sb.Append(System.Math.Round(stat.Value - stat.baseValue,4));
            sb.Append(")");
        }
        return sb.ToString();
    }
    private string GetStatModifierText(CharacterStat stat)
    {
        sb.Length = 0;
        foreach (StatModifier mod in stat.StatModifiers)
        {
            if (sb.Length > 0)
            {
                sb.AppendLine();
            }
            if(mod.value > 0)
            {
                sb.Append("+");
            }
            if (mod.type == StatModType.Flat)
            {
                sb.Append(mod.value);
            }
            else
            {
                sb.Append(mod.value * 100);
                sb.Append("%");
            }
            EquippableItem item = mod.source as EquippableItem;

            if (item != null)
            {
                sb.Append(" ");
                sb.Append(item.itemName);
            }
            else
            {
                Debug.LogError("Modifier is not an Equippable Item");
            }
        }

        return sb.ToString();
    }
}
