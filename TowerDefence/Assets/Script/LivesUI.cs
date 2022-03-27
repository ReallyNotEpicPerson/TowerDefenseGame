using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LivesUI : MonoBehaviour
{
	public bool isTmp = true;
	public TMP_Text text;
	public Text livesText;
    private void OnValidate()
    {
        if (text != null)
        {
            isTmp = true;
        }
        else
        {
            isTmp = false;
        }
    }
    void Update()
	{
        if (isTmp)
        {		
            text.text = PlayerStat.Lives.ToString();
			return;
        }
        livesText.text = PlayerStat.Lives.ToString();

	}
}