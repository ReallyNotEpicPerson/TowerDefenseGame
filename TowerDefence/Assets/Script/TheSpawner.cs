using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public enum SpawnType
{
    SinglePoint = 0,
    CircleZone = 1,
    RectangleZone = 2,
}
public class TheSpawner : MonoBehaviour
{
    public static int numOfEnemies;
    public Wave[] mainWaves;//<----(wave)

    public Transform[] SpawnPoint;
    public static Transform[] spawnPoint;
    public Transform[] EndPoint;
    public static Transform[] endPoint;
    [SerializeField] private SpawnType spawnType;
    [SerializeField] private Vector2[] sizes;
    public Game_Managers game_Managers;

    public float TimeBetweenWave = 5f;
    public float CountDown = 2f;
    [HideInInspector]
    public static int waveNum = 0;
    private GameObject SpawnFx;
    public TMP_Text leWaveTimer;
    public TMP_Text Counter;
    [SerializeField] ObjectPooler objectPooler;
    [SerializeField] StatusEffectDisplayer displayer;
    /*
    IEnumerator WaveCount()
    {
        Counter.enabled = true;
        Counter.text = "Wave " + (waveNum + 1);
        yield return new WaitForSeconds(5);
        Counter.enabled = false;
    }*/
    private void Awake()
    {
        spawnPoint = SpawnPoint;
        endPoint = EndPoint;
    }
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
    public void WaveCounter(string num)
    {

        Counter.text = "Wave " + num;
    }
    IEnumerator SpawnWave()
    {
        PlayerStat.rounds++;
        Wave wave = mainWaves[waveNum];// wave contain 
        for (int j = 0; j < wave.enemy.Length; j++)
        {
            numOfEnemies += wave.enemy[j].count;
            for (int i = 0; i < wave.enemy[j].count; i++)
            {
                EnemySpawner(wave.enemy[j].path, wave.enemy[j].enemy);
                yield return new WaitForSeconds(1f / wave.enemy[j].rate);
            }
            if (wave.enemy[j].delayBetweenGroup > 0)
            {
                yield return new WaitForSeconds(wave.enemy[j].delayBetweenGroup);
            }
        }
        waveNum++;
    }
    /*public Vector3 RandomPos(this TheSpawner theSpawner,float range)
    {
        return Random.insideUnitCircle * Random.Range(-range, range);
    }*/
    Vector3 RandomCoordinate(int i)
    {
        if (spawnType.HasFlag(SpawnType.CircleZone))
        {
            return Random.insideUnitCircle * Random.Range(-sizes[i].x, sizes[i].x);
        }
        if (spawnType.HasFlag(SpawnType.RectangleZone))
        {
            return new Vector3(Random.Range(-Mathf.Abs(sizes[i].x), Mathf.Abs(sizes[i].x)), Random.Range(-Mathf.Abs(sizes[i].y), Mathf.Abs(sizes[i].y)));
        }
        return Vector3.zero;
    }
    void EnemySpawner(int path, GameObject enemy)
    {
        //move all the enemy to a enemylist soon
        GameObject lol = Instantiate(enemy, SpawnPoint[path].position + RandomCoordinate(path), Quaternion.identity);//,"EnemyList")
        //spawn fx now
        Debug.Log(SpawnPoint[path].position + " " + lol.transform.position);
        if (lol.TryGetComponent(out NavMeshAI Navmesh))
        {        
            if (EndPoint.Length > 1)
            {
                Navmesh.SetDestination(SpawnPoint[path], EndPoint[path]);
            }
            else
            {
                Navmesh.SetDestination(SpawnPoint[path], EndPoint[0]);
            }
        }
        //displayer.SetObject(lol);
        //GameObject enemyScript = objectPooler.SpawnFromPool(enemy.name, SpawnPoint.position, SpawnPoint.rotation);
        //statusEffectManager.AddEnemy(enemyScript.GetComponent<Enemy>());
    }
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {   
        if (spawnType.HasFlag(SpawnType.CircleZone))
        {
            Handles.color = Color.blue;
            for (int i = 0; i < SpawnPoint.Length; i++)
            {
                Handles.DrawWireDisc(SpawnPoint[i].position, new Vector3(0, 0, 1), sizes[i].x);
            }
        }
        else if (spawnType.HasFlag(SpawnType.RectangleZone))
        {
            Handles.color = Color.green;
            for (int i = 0; i < SpawnPoint.Length; i++)
            {
                Handles.DrawWireCube(SpawnPoint[i].position, sizes[i]);
            }
        }
        //Gizmos.DrawWireSphere(transform.position,explosionRadius);    
    }
#endif
}
