﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatUI_InGame : MonoBehaviour
{
    public GameObject statUI;

    public Image _img;
    public TMP_Text _charName;
    public TMP_Text _Charstat;

    GameObject main;
    private BulletTypeTurret spinningTurret;
    private Bullet bullet;
    private Enemy enemy;

    private void OnValidate()
    {
        
    }
    public void TransferCharacter(GameObject Character)
    {
        statUI.SetActive(true);
        
        main = Character;
        if (Character.name.Equals("LazerTurret") || Character.name.Equals("LazerTurret_Lv2"))
        {
            StoreTurretComponet();
            ShowLazerTypeStat();
        }
        else if (Character.tag.Equals("Enemy"))
        {
            StoreEnemyComponent();
            ShowEnemyStat();
        }
        else
        {
            StoreTurretComponet();
            StoreBulletComponent();
            ShowNonLazerTypeStat();
        }
    }
    void StoreTurretComponet()
    {
        spinningTurret = main.GetComponent<BulletTypeTurret>();
    }
    void StoreBulletComponent()
    {
        bullet = spinningTurret.BulletPrefab.GetComponent<Bullet>();
    }
    void StoreEnemyComponent()
    {
        enemy = main.GetComponent<Enemy>();
    }
    void ShowEnemyStat()
    {
        _charName.text = main.name;
        _Charstat.text = "Health" + enemy.startHealth + "\n" +
            "Speed:" + enemy.startSpeed + "\n" +
            "Worth:" + enemy.worth + "\n"+
            "Description:"+ EnemyDesscription(main.name);
    }
    void ShowNonLazerTypeStat()
    {
        /*
        //_img.GetComponent<Image>().overrideSprite = Instantiate( spinningTurret.GetComponent<Image>().sprite);
        _charName.text = main.name;
        _Charstat.text = "Damage:" + bullet.bulletDamage + "\n" +
            "Range:" + spinningTurret.range + "\n" +
            "Speed:" + bullet.speed + "\n" +
            //"Explosion Radius:" + bullet + "\n" +
            "Fire Rate:" + spinningTurret.firerate + "\n" +
            //"CritChance:" + spinningTurret.critChance*100+"%" + "\n" +
            //"CritDamage:" + spinningTurret.critDamage*100+"%" + "\n"+
            "Description:"+TurretDescription(main.name);*/
    }
    void ShowLazerTypeStat()
    {
       // _img.GetComponent<Image>().overrideSprite = Instantiate(spinningTurret.GetComponent<Image>().sprite); 
        _charName.text = main.name;
        //_Charstat.text = "Damage/s:" + spinningTurret.damageOverTime + "\n" +
            //"Range:" + spinningTurret.range + "\n" +
            //"Slow:" + spinningTurret.slowPtc + "\n" +
            //"CritChance:" + spinningTurret.critChance * 100 + "%" + "\n" +
            //"CritDamage:" + spinningTurret.critDamage * 100 + "%" + "\n" +
        //"Rotation Speed:" + spinningTurret.RotationSpeed/100 + "\n" +
        //"Speed:" + "INF" + "\n" +
        //"Explosion Radius:" + bullet.explosionRadius + "\n" +
        //"Fire Rate:" + spinningTurret.firerate + "\n" +
       // "Description:" + TurretDescription(main.name);
    }
    
    string TurretDescription(string n)
    { 
        string des="";
        switch (n)
        {
            case "LazerTurret":
                des= "A Lazer";
                break;
            case "BasicTurret":
                des= "A Turd";
                break;
            case "MissleTurret":
                des= "A Missle";
                break;
            case "LazerTurret_Lv2":
                des= "A lv2 Lazer";
                break;
            case "BasicTurret_Lv2":
                des = "A Lv2 Turd";
                break;
            case "MissleTurret_Lv2":
                des = "A lv2 Missle";
                break;
            case "manaxe":
                des = "An axe man";
                break;
            default:
                des = "Nothing,you type wrong buddy";
                break;
        }
        return des;
    }
    string EnemyDesscription(string n)
    {
        string des = "";
        switch (n)
        {
            case "Test enemy":
                des = "A basic Bad guy";
                break;
            case "Tank man":
                des = "A Strong Bad Guy";
                break;
            case "Leg man":
                des = "A Fast bad guy";
                break;
            case "Boss":
                des = "BIG boss";
                break;
            default:
                des = "nothing,fix code u dummy!";
                break;
        }

        return des;
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1))//||Input.GetKey(KeyCode.Mouse0))
        {
            statUI.SetActive(false);
        }
    }
}