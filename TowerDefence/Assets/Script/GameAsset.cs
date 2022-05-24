using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameAsset : MonoBehaviour
{
    public static Dictionary<string,GameObject> dict= new Dictionary<string, GameObject>();
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
    public CharacterFormation formation;

    public List<TurretBluePrint> turret;
    public List<GameObject> weaponSprite;
    public List<Image> charImg;
    public List<Sprite> turretSprite;

    public Transform damageDisplayer;
    public GameObject underling;
    [Header("SOUND")]
    public AudioClip PewPew_1;
    public AudioClip k;
    public AudioClip revive;
    public AudioClip whenEnemyPassThrough;
    public AudioClip notEnoughMoney;
    public AudioClip clickClack;

    public void PlaySound(AudioClip clip,float volume)
    {
        audioSource.PlayOneShot(clip,volume);
    }
    public static void Spawn(string name , Vector3 pos)
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
