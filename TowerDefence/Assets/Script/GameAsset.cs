using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameAsset : MonoBehaviour
{
    private static GameAsset _i;
    public static GameAsset I
    {
        get
        {
            if (_i == null) _i = Instantiate(Resources.Load<GameAsset>("GameAsset"));
            return _i;
        }
    }
    public List<TurretBluePrint> turret;
    public List<GameObject> weaponSprite;
    public Image[] charImg;

    public Transform damageDisplayer;

}
