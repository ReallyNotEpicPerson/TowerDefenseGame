using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pewpew : MonoBehaviour
{
    [SerializeField] GameObject thePewPew;
    [SerializeField] GameObject LeTurret;
    // Start is called before the first frame update
    int timer;
    void Start()
    {
        thePewPew.transform.position=LeTurret.transform.position;
        timer = 200;
    }

    // Update is called once per frame
    void Update()
    {
        timer--;
        if (timer == 0)
        {
            thePewPew.transform.position += new Vector3(0.01f, 0.01f, 0);
            timer = 200;
        }
    }

}
