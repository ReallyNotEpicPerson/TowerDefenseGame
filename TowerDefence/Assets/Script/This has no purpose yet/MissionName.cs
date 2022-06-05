using UnityEngine;
using System.Collections;
using TMPro;


public class MissionName : MonoBehaviour
{
    public TMP_Text mission;


    //public int index=0;

    void Start()
    {
        StartCoroutine(StartPhrase());
    }

    string Phrase(int i)
    {
        string [] Serious = {"SURVIVE!!", "Good luck my friend.","CARNAGE!!","RUN!", "DANGER!!","INCOMING!!", "Get to Sector 42!", "They are coming...." };
        string[] Jokes = { "Good luck,lol", "Wait.Why tho?", "GG man.GG", "What?", "Not again.....", "Zombies ei?.shit.", "Where Is My phone?","Found it!" };
        return Jokes[i];
    }
    IEnumerator StartPhrase()
    {
        mission.enabled = true;
        mission.text = Phrase(Random.Range(0,8));
        yield return new WaitForSeconds(3);
        mission.enabled = false;
    }
}
