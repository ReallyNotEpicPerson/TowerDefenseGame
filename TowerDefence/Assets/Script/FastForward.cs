﻿using TMPro;
using UnityEngine;

public class FastForward : MonoBehaviour
{
    public int timeToFastForward;
    public TMP_Text ff;
    private int i = 1;

    public void Awake()
    {
        Time.timeScale = 1;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !PauseMenu.uiState)
        {
            Increase();
        }
    }
    public void Increase()
    {
        i++;
        ff.text = "Fast Forward " + "x " + i;
        if (i > timeToFastForward)
        {
            i = 1; ff.text = "Fast Forward";
        }
        Time.timeScale = i;
    }


}
