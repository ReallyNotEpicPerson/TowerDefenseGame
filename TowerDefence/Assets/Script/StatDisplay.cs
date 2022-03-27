using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class StatDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private CharacterStat _stat;
    public CharacterStat Stat
    {
        get { return _stat; }
        set
        {
            _stat = value;
            UpdateStat();
        }
    }
    private string _name;
    public string Name
    {
        get { return _name; }
        set
        {
            _name = value;
            nameText.text = _name.ToLower();
        }
    }
    [SerializeField] Text nameText;
    [SerializeField] Text valueText;
    [SerializeField] StatToolTips statToolTips;
    private void OnValidate()
    {
        Text[] txt = new Text[transform.childCount];
        for (int i = 0; i < txt.Length; i++)
        {
            txt[i] = transform.GetChild(i).GetComponent<Text>();
        }
        nameText = txt[0];
        valueText = txt[1];
        if (statToolTips == null)
        {
            statToolTips = FindObjectOfType<StatToolTips>();
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        statToolTips.Show(Stat, Name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        statToolTips.Hide();
    }
    public void UpdateStat()
    {
        valueText.text = _stat.Value.ToString();
    }
}
