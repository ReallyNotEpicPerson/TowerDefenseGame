using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LivesUI : MonoBehaviour
{
	public bool isTmp = true;
	public TMP_Text text;
	public Text livesText;

    void Update()
	{
        if (!isTmp)
        {
			livesText.text = PlayerStat.Lives.ToString()+ " Lives";
			return;
        }
		text.text = PlayerStat.Lives.ToString()+" Lives";
	}
}