using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public static int moneyInGame;
    public int startMoneyInGame=200;

    public static int Lives;
    public int startlives;

    public static int rounds;
    void Start()
    {
        moneyInGame = startMoneyInGame;
        Lives = startlives;

        rounds = 0;
    }
}
