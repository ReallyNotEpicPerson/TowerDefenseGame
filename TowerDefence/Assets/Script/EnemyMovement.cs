using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour
{
    private Transform Target;
    private int wpIndex = 0;
    //private Enemy enemy;
    private bool turnBack = false;

    public CharacterStat startSpeed;
    [HideInInspector]
    public float speed;

    private void Awake()
    {
        //enemy = GetComponent<Enemy>();
        Target = WayPoint.Points[0];
    }
    private void Start()
    {
        speed = startSpeed.value;
    }
    void Update()
    {
        //Target.position.z *= 0;
        Vector3 dir = Target.position - transform.position;
        transform.Translate(speed * Time.deltaTime * dir.normalized, Space.World);
        //if (turnBack == false) { 
            if (Vector3.Distance(transform.position, Target.position) <= 0.1f)
            {
                NextWayPoint();
            }
        //}     
        /*else
        {
            if (Vector3.Distance(transform.position, Target.position) <= 0.1f)
            {
                PreviousWayPoint();
            }
        }*/
        //enemy.speed = enemy.startSpeed;
    }
    public void AdjustSpeed(float slowPtc)
    {
        speed = startSpeed.value * (1 - slowPtc);
    }
    public void Turn()
    {
        turnBack = !turnBack;
    }
    private void NextWayPoint()
    {
        if (wpIndex >= WayPoint.Points.Length - 1)
        {
            EndPath();
            return;
        }
        wpIndex++;
        Target = WayPoint.Points[wpIndex];
    }
    private void PreviousWayPoint()
    {
        if (wpIndex == 0)
        {
            //EndPath();
            return;
        }
        wpIndex--;
        Target = WayPoint.Points[wpIndex];
    }
    void EndPath()
    {
        PlayerStat.Lives--;
        Destroy(gameObject);
        TheSpawner.numOfEnemies--;
    }
}
