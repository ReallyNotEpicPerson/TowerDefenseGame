using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    public static Transform[] Points ;
    public static Transform[] Points2 ;
    public static Transform[] Points3 ;
    public Transform secondPath;
    public Transform thirdPath;
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
        if (thirdPath != null)
        {
            Points3 = new Transform[thirdPath.childCount];
            for (int i = 0; i < Points3.Length; i++)
            {
                Points3[i] = thirdPath.GetChild(i);
            }
        }
    }
}
