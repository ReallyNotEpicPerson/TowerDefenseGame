using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierPathFollow : MonoBehaviour
{
    [SerializeField] private float speed=0.5f;
    [SerializeField] private Transform[] PathPoint;

    private int PathToFollow;

    private float tPara;

    private bool allowCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        PathToFollow = 0;
        tPara = 0f;
        allowCoroutine = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (allowCoroutine)
        {
            StartCoroutine(FollowPath(PathToFollow));
        }
    }

    private IEnumerator FollowPath(int pathNum)
    {
        allowCoroutine = false;

        Vector2 p0 = PathPoint[pathNum].GetChild(0).position;
        Vector2 p1 = PathPoint[pathNum].GetChild(1).position;
        Vector2 p2 = PathPoint[pathNum].GetChild(2).position;
        Vector2 p3 = PathPoint[pathNum].GetChild(3).position;

        Vector2 catPos; 
        while (tPara < 1)
        {
            tPara += Time.deltaTime * speed;

            catPos= Mathf.Pow(1 - tPara, 3) * p0
            + 3 * Mathf.Pow(1 - tPara, 2) * tPara * p1 + 3 * (1 - tPara) * Mathf.Pow(tPara, 2)
            * p2 + Mathf.Pow(tPara, 3) * p3;

            transform.position = catPos;
            yield return new WaitForEndOfFrame();
        }
        tPara = 0;

        PathToFollow++;
        if (PathToFollow > PathPoint.Length - 1)
            PathToFollow = 0;

        allowCoroutine = true;
    }
    
}
