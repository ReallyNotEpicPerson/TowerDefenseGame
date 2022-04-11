using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
    public string name;
    public GameObject prefab;
    public int size;
}
public class ObjectPooler : MonoBehaviour
{
    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDict;
    void Awake()
    {
        poolDict = new Dictionary<string,Queue<GameObject>>();
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectpool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab,transform);
                obj.name = pool.prefab.name;
                obj.SetActive(false);
                objectpool.Enqueue(obj);
            }
            poolDict.Add(pool.name, objectpool);
        }
    }
    public GameObject SpawnFromPool(string name , Vector3 position ,Quaternion rotation)
    {
        if(!poolDict.ContainsKey(name))
        {
            Debug.Log("What are you trying to access buddy? "+name+" Doesn`t exist");
            return null;
        }
        GameObject objToSpawn = poolDict[name].Dequeue();
        objToSpawn.SetActive(true);
        objToSpawn.transform.SetPositionAndRotation(position, rotation);
        poolDict[name].Enqueue(objToSpawn);
        return objToSpawn;
    }
    public void idkyet()
    {

    }
}
