using System.Collections.Generic;
using UnityEngine;

public class LoadExcel : MonoBehaviour
{
    private EnemyGroupStat blankGroupStat;
    public List<EnemyGroupStat> dataBase = new List<EnemyGroupStat>();

    public List<GameObject> Enemy;
    public string fileName;

    public void LoadData()
    {
        dataBase.Clear();
        List<Dictionary<string, object>> data = CSVReader.Read(fileName);
        for (var i = 0; i < data.Count; i++)
        {
            string name = data[i]["Name"].ToString();
            int count = int.Parse(data[i]["count"].ToString(), System.Globalization.NumberStyles.Integer);
            bool randomRateRange = bool.Parse(data[i]["randomRateRange"].ToString());
            float startRate = float.Parse(data[i]["startRate"].ToString());
            float endRate = float.Parse(data[i]["endRate"].ToString());
            int path = int.Parse(data[i]["path"].ToString(), System.Globalization.NumberStyles.Integer);
            bool useNavmesh= bool.Parse(data[i]["useNavmesh"].ToString());
            float delayBetweenGroup = float.Parse(data[i]["delayBetweenGroup"].ToString());
            //blankGroupStat.sprite = null;
            Add(name,count,randomRateRange,startRate,endRate,path,useNavmesh,delayBetweenGroup);
        }
    }
    void Add(string name, int count, bool randomRateRange, float startRate, float endRate, int path, bool useNavmesh, float delayBetweenGroup)
    {
        EnemyGroupStat temp = new EnemyGroupStat();

        temp.enemy = Enemy.Find(obj => obj.name == name);
        temp.sprite = temp.enemy.GetComponent<Enemy>().enemySprite;
        temp.count = count;
        temp.randomRateRange = randomRateRange;
        temp.startRate =startRate;
        temp.endRate= endRate;
        temp.path=path;
        temp.useNavmesh = useNavmesh;
        temp.delayBetweenGroup = delayBetweenGroup;

        dataBase.Add(temp);
    }
}
