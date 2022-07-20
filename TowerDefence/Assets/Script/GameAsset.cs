using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameAsset : MonoBehaviour
{
    public static Dictionary<string, GameObject> dict = new Dictionary<string, GameObject>();
    public AudioSource audioSource;

    private static GameAsset _i;
    public static GameAsset I
    {
        get
        {
            if (_i == null) _i = Instantiate(Resources.Load<GameAsset>("GameAsset"));
            return _i;
        }
    }
    public Setting formation;
    public List<GameObject> weaponSprite;
    public List<Image> charImg;
    public Transform damageDisplayer;
    public GameObject underling;

    [TextArea(1, 6)]
    public string[] description;
    [TextArea(1, 4)]
    public string[] turretDescription;
    [Header("Enemy")]
    public List<Sprite> enemySprite;
    public Sprite questionMarkSprite;
    [Header("Turret")]
    public List<TurretBluePrint> turret;
    public List<Sprite> turretSprite;
    public List<Sprite> upgradeTurret_2;
    public List<Sprite> upgradeTurret_3;
    public List<Sprite> upgradeTurret_Tree_0;
    public List<Sprite> upgradeTurret_Tree_1;
    [Header("MAP")]
    public Sprite[] Map;
    public List<List2> enemyThatRound;

    [Header("SOUND")]
    public AudioClip PewPew_1;
    public AudioClip k;
    public AudioClip revive;
    public AudioClip whenEnemyPassThrough;
    public AudioClip notEnoughMoney;
    public AudioClip clickClack;
    public AudioClip PopingSound;
    public AudioClip ClickSound;
    public AudioClip BuildSound;
    public AudioClip SellingSound;


    public void PlayClickSound()
    {
        audioSource.PlayOneShot(ClickSound);
    }
    public void PlayPopingSound()
    {
        audioSource.PlayOneShot(PopingSound);
    }
    public void PlayMoneySound()
    {
        audioSource.PlayOneShot(SellingSound);
    }
    public static void Spawn(string name, Vector3 pos)
    {
        Instantiate(dict[name], pos, Quaternion.identity);
    }
    public static void SetCharacterFormation()
    {
        dict.Clear();
    }
    public void ReadData()
    {
        return;
    }
}
[System.Serializable]
public class List2
{
    public List<int> list;
}