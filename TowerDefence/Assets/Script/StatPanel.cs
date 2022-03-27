using UnityEngine;

public class StatPanel : MonoBehaviour
{
    [SerializeField] StatDisplay[] statDisplay;
    [SerializeField] string[] statNames;
    private CharacterStat[] stats;
    private void OnValidate()
    {
        statDisplay = new StatDisplay[transform.childCount-1];
        for (int i = 0; i < statDisplay.Length; i++)
        {
            statDisplay[i] = transform.GetChild(i+1).GetComponent<StatDisplay>();
        }
        UpdateStatName();
    }
    private void Awake()
    {
        UpdateStatName();
    }
    public void SetStat(params CharacterStat[] charStat)
    {
        stats = charStat;
        if (stats.Length > statDisplay.Length)
        {
            Debug.LogError("OOPSIE WHOOPSIE NO Roooommmm!");
            return;
        }
        for (int i = 0; i < stats.Length; i++)
        {
            statDisplay[i].gameObject.SetActive(i < statDisplay.Length);
            if (i<stats.Length)
            {
                statDisplay[i].Stat = stats[i];
            }
        }
    }
    public void UpdateStatValue()
    {
        for (int i = 0; i < stats.Length; i++)
        {
            statDisplay[i].UpdateStat();
        }
    }
    public void UpdateStatName()
    {
        for (int i = 0; i < statNames.Length; i++)
        {
            statDisplay[i].Name = statNames[i];
        }
    }
}
