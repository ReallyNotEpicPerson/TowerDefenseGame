using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ItemToolTips : MonoBehaviour
{
    [SerializeField] Text NameText;
    [SerializeField] Text SlotText;
    [SerializeField] Text StatText;

    private StringBuilder sb = new StringBuilder();

    public void Show(EquippableItem item)
    {
        NameText.text = item.name;
        SlotText.text = item.equipmentType.ToString();

        sb.Length = 0;
        AddStat(item.StrengthBonus, "Strength");
        AddStat(item.IDKBonus, "Strength");
        AddStat(item.BullshitBonus, "Strength");
        AddStat(item.Like4realBonus, "Strength");

        AddStat(item.StrengthBonusPercentage, "Strength",isPercent: true);
        StatText.text = sb.ToString();

        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    private void AddStat(float value, string statName, bool isPercent = false)
    {
        if (value != 0)
        {
            if (sb.Length > 0)
            {
                sb.AppendLine();
            }
            if (value > 0)
            {
                sb.Append("+");
            }
            if (isPercent)
            {
                sb.Append(value * 100);
                sb.Append("% ");
            }
            else
            {
                sb.Append(value);
                sb.Append(" ");
            }

            sb.Append(statName);
        }
    }
}
