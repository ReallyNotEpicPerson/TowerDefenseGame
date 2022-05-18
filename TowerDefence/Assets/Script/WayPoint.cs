using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    public static Transform[] Points ;

    public static Transform[] Points2 ;
    public Transform secondPath;
    // Start is called before the first frame update
    void Awake()
    {
        Points = new Transform[transform.childCount];
        for (int i = 0; i < Points.Length; i++)
        {
            Points[i]= transform.GetChild(i);
        }
        if (secondPath != null)
        {
            Points2 = new Transform[secondPath.childCount];
            for (int i = 0; i < Points2.Length; i++)
            {
                Points2[i] = secondPath.GetChild(i);
            }
        }
    }
}
