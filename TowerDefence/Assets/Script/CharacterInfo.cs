using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{

    BulletTypeTurret sp;
    Bullet bullet;
    private GameObject Main; 

    public void ShowStat(GameObject character)
    {
        Main = character;
    }
}
