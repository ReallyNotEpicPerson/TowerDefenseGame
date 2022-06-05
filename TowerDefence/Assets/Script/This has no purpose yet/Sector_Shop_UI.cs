using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector_Shop_UI : MonoBehaviour
{
    [SerializeField] private GameObject Shop;
    [SerializeField] private Transform Template;

    //void Create bullshit

    public void TurnOff()
    {
        Shop.SetActive(false);
    }
    
}
