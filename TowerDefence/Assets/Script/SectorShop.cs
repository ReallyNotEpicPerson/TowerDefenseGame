using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectorShop : MonoBehaviour
{
    [SerializeField] private GameObject Shop;
    
    public enum Weapons 
    {
        Pistol,//Hand gun
        SniperRifle,//Rifle 
        ShotGun,//Shot gun
        Rifle,//Sniper
        Rock,//The troll

    }
    public void Buy()
    {
        
    }
    public void Sell()
    {

    }
    public float getCost(Weapons weapon)
    {
        switch (weapon)
        {
            case Weapons.Pistol:
                return 12f;
            case Weapons.Rifle:
                return 50f;
            case Weapons.ShotGun:
                return 50f;
            case Weapons.SniperRifle:
                return 80f;
            case Weapons.Rock:
                return 1f;
            default:
                return 0;
                
        }
    }
    /*GameObject getTurret(Weapons weapon)
    {
        switch (weapon)
        {
            case Weapons.Pistol:
                return GameAsset.i.turret[0];
            case Weapons.Rifle:
                return 50f;
            case Weapons.ShotGun:
                return 50f;
            case Weapons.SniperRifle:
                return 80f;
            case Weapons.Rock:
                return 1f;
            default:
                return 0;

        }
    }*/
    private void OnMouseDown()
    {
        Shop.SetActive(true);
    }

}