using System;
using System.Collections;
using System.Collections.Generic;
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
    public Wave[] mainWaves;//<------------------------------------(wave)

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

    public GameObject waveDisplayer;
    private bool DoneWithSpawning = true;
    [SerializeField] private List<StartWaveEnemyDisplay> startWaveEnemyDisplays;
    [SerializeField] private List<Sprite> imageList = new List<Sprite>();
    private Dictionary<Sprite, int> numOfEnemy = new Dictionary<Sprite, int>();
    //private List<int> numOfEnemy = new List<int>();

    private void OnValidate()
    {
        for (int i = 0; i < mainWaves.Length; i++)
        {
            for (int j = 0; j < mainWaves[i].enemy.Length; j++)
            {
                if (mainWaves[i].enemy[j].enemy != null)
                {
                    mainWaves[i].enemy[j].sprite = mainWaves[i].enemy[j].enemy.GetComponent<Enemy>().GetSprite();
                }
                if (mainWaves[i].enemy[j].path >= EndPoint.Length)
                {
                    mainWaves[i].enemy[j].path = EndPoint.Length - 1;
                }
                if (mainWaves[i].enemy[j].path >= SpawnPoint.Length)
                {
                    mainWaves[i].enemy[j].path = SpawnPoint.Length - 1;
                }
            }
        }
        startWaveEnemyDisplays.Clear();
        for (int i = 0; i < waveDisplayer.transform.childCount; i++)
        {
            startWaveEnemyDisplays.Add(waveDisplayer.transform.GetChild(i).GetComponent<StartWaveEnemyDisplay>());
        }
    }
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
        waveNum = 0;
    }
    void Start()
    {
        numOfEnemies = 0;
        //objectPooler = ObjectPooler.instance;
    }
    void Update()
    {
        if (numOfEnemies > 0 || !DoneWithSpawning)
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
            DoneWithSpawning = false;
            StartCoroutine(SpawnWave());
            waveDisplayer.SetActive(false);
            imageList.Clear();
            numOfEnemy.Clear();
            CountDown = TimeBetweenWave;
            //StartCoroutine(WaveCount());
            WaveCounter(mainWaves[waveNum].waveName);
            return;
        }
        if (!waveDisplayer.activeSelf && waveNum != mainWaves.Length)
        {
            waveDisplayer.SetActive(true);
            EnemyWaveDisplay();
        }
        CountDown -= Time.deltaTime;
        CountDown = Mathf.Clamp(CountDown, 0f, Mathf.Infinity);

        leWaveTimer.text = String.Format("{0:00.00}", CountDown);
    }
    public void EnemyWaveDisplay()
    {
        for (int j = 0; j < mainWaves[waveNum].enemy.Length; j++)
        {
            if (!imageList.Contains(mainWaves[waveNum].enemy[j].sprite))
            {
                imageList.Add(mainWaves[waveNum].enemy[j].sprite);
            }
            if (!numOfEnemy.ContainsKey(mainWaves[waveNum].enemy[j].sprite))
            {
                numOfEnemy.Add(mainWaves[waveNum].enemy[j].sprite, mainWaves[waveNum].enemy[j].count);
            }
            else
            {
                numOfEnemy[mainWaves[waveNum].enemy[j].sprite] += mainWaves[waveNum].enemy[j].count;
            }
            //img.transform.GetChild(0);
        }
        //waveDisplayer.transform.GetChild().GetComponent<Image>();
        for (int i = 0; i < startWaveEnemyDisplays.Count; i++)
        {
            //Debug.Log(i + " " + imageList.Count);
            if (i >= imageList.Count)
            {
                startWaveEnemyDisplays[i].gameObject.SetActive(false);
                continue;
            }
            startWaveEnemyDisplays[i].SetSprite(imageList[i]);
            startWaveEnemyDisplays[i].SetText(numOfEnemy[imageList[i]].ToString());
            startWaveEnemyDisplays[i].gameObject.SetActive(true);
        }
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
                yield return new WaitForSeconds(wave.enemy[j].startRate);
            }
            if (wave.enemy[j].delayBetweenGroup > 0)
            {
                yield return new WaitForSeconds(wave.enemy[j].delayBetweenGroup);
            }
        }
        waveNum++;        
        DoneWithSpawning = true;
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
