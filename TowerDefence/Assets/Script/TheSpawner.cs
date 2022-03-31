using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TheSpawner : MonoBehaviour
{
    public static int numOfEnemies;

    public Wave[] mainWaves;//<----(wave)
    //[]
    public Transform SpawnPoint;
    public Game_Managers game_Managers;

    public float TimeBetweenWave = 5f;
    public float CountDown = 2f;
    [HideInInspector]
    public static int waveNum = 0;
    
    public TMP_Text leWaveTimer;
    public TMP_Text Counter;
    [SerializeField] ObjectPooler objectPooler;
   // StatusEffectManager statusEffectManager;

    [SerializeField] StatusEffectDisplayer displayer;
    /*void Awake()
    {
        //statusEffectManager = GetComponent<StatusEffectManager>();
    }*/
    public void WaveCounter(string num)
    {
        Counter.text = "Wave " + num;
    }/*
    IEnumerator WaveCount()
    {
        Counter.enabled = true;
        Counter.text = "Wave " + (waveNum + 1);
        yield return new WaitForSeconds(5);
        Counter.enabled = false;
    }*/
    //
    void Start()
    {
        numOfEnemies = 0;
        //objectPooler = ObjectPooler.instance;
    }
    void Update()
    {
        if (numOfEnemies > 0)
        {
            return;
        }
        if (waveNum == mainWaves.Length)
        {
            game_Managers.WinLevel();
            enabled = false;
        }
        if (Input.GetKey(KeyCode.Tab) || Input.GetKey(KeyCode.X))
        {
            CountDown = 0f;
            leWaveTimer.text = String.Format("{0:00.00}", CountDown);
            //StartCoroutine(WaveCount());
            WaveCounter(mainWaves[waveNum].waveName);
            return;
        }
        if (CountDown <= 0)
        {
            StartCoroutine(SpawnWave());
            CountDown = TimeBetweenWave;
            //StartCoroutine(WaveCount());
            WaveCounter(mainWaves[waveNum].waveName);
            return;
        }
        CountDown -= Time.deltaTime;

        CountDown = Mathf.Clamp(CountDown, 0f, Mathf.Infinity);

        leWaveTimer.text = String.Format("{0:00.00}", CountDown);

    }
    IEnumerator SpawnWave()
    {
        PlayerStat.rounds++;

        Wave wave = mainWaves[waveNum];
        for (int j = 0; j < wave.enemy.Length; j++)
        {      
            numOfEnemies += wave.enemy[j].count;

            for (int i = 0; i < wave.enemy[j].count; i++)
            {
                EnemySpawner(wave.enemy[j].enemy);
                yield return new WaitForSeconds(1f / wave.enemy[j].rate);
            }
        }
        waveNum++;
    }
    void EnemySpawner(GameObject enemy)
    {
        //move all the enemy to a enemylist soon
        GameObject lol= Instantiate(enemy, SpawnPoint.position, SpawnPoint.rotation);//,"EnemyList")
        lol.name = enemy.name;
        //displayer.SetObject(lol);
        //GameObject enemyScript = objectPooler.SpawnFromPool(enemy.name, SpawnPoint.position, SpawnPoint.rotation);
        //statusEffectManager.AddEnemy(enemyScript.GetComponent<Enemy>());
    }
}
