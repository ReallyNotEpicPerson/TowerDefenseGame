using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ListDisplayUI : MonoBehaviour
{
    public GameObject turrets;
    public GameObject enemies;
    public Transform turretList;
    public Transform enemyList;
    private List<Image> upgradeImageList = new List<Image>();
    public Transform turretStatInfo;
    public Transform enemyStatInfo;
    //private List<BaseTurretStat> turretList;
    public List<Enemy> enemy;
    private TurretBluePrint clickedTurret;
    private BaseTurretStat extractedTurret;

    private TMP_Text enemyName;
    private Transform enemyStats;
    private Transform enemyIcons;
    private List<TMP_Text> enemyStat = new List<TMP_Text>();
    private TMP_Text description;

    private TMP_Text turretName;
    private Transform turretStats;
    private Transform turretIcons;
    private List<TMP_Text> turretStat = new List<TMP_Text>();
    private TMP_Text statusEffect;
    private void OnValidate()
    {
        for (int i = 0; i < enemyList.childCount; i++)
        {
            enemyList.GetChild(i).GetChild(0).GetComponent<Image>().sprite = enemy[i].enemySprite;
        }
    }
    private void Awake()
    {
        for (int i = 0; i < turretList.childCount; i++)
        {
            upgradeImageList.Add(turretList.GetChild(i).GetChild(0).GetComponent<Image>());
        }

        turretStatInfo.Find("Name").TryGetComponent(out turretName);
        turretStats = turretStatInfo.Find("Stats");
        turretStatInfo.Find("StatusEffect").TryGetComponent(out statusEffect);
        for (int i = 0; i < turretStats.childCount; i++)
        {
            turretStat.Add(turretStats.GetChild(i).GetComponent<TMP_Text>());
        }
        turretIcons = turretStatInfo.Find("Icon");

        enemyStatInfo.Find("Name").TryGetComponent(out enemyName);
        enemyStats = enemyStatInfo.Find("Stats");
        enemyStatInfo.Find("Description").TryGetComponent(out description);
        for (int i = 0; i < enemyStats.childCount; i++)
        {
            enemyStat.Add(enemyStats.GetChild(i).GetComponent<TMP_Text>());
        }
        enemyIcons = enemyStatInfo.Find("Icon");
    }
    void Start()
    {
        GetBaseTurret(0);
    }
    public void GetBaseTurret(int index)
    {
        clickedTurret = GameAsset.I.turret[index];
        for (int i = 0; i < upgradeImageList.Count; i++)
        {
            if (i == 0)//lv1
            {
                upgradeImageList[i].sprite = GameAsset.I.turretSprite[index];
                upgradeImageList[i].transform.parent.gameObject.SetActive(true);
            }
            else if (i == 1 && GameAsset.I.upgradeTurret_2[index] != null)//lv2
            {
                upgradeImageList[i].sprite = GameAsset.I.upgradeTurret_2[index];
                upgradeImageList[i].transform.parent.gameObject.SetActive(true);
            }
            else if (i == 2 && GameAsset.I.upgradeTurret_3[index] != null)//lv3
            {
                upgradeImageList[i].sprite = GameAsset.I.upgradeTurret_3[index];
                upgradeImageList[i].transform.parent.gameObject.SetActive(true);
            }
            else if (i == 3 && GameAsset.I.upgradeTurret_Tree_0[index] != null)//tree 0
            {
                upgradeImageList[i].sprite = GameAsset.I.upgradeTurret_Tree_0[index];
                upgradeImageList[i].transform.parent.gameObject.SetActive(true);
            }
            else if (i == 4 && GameAsset.I.upgradeTurret_Tree_1[index] != null)//tree 1
            {
                upgradeImageList[i].sprite = GameAsset.I.upgradeTurret_Tree_1[index];
                upgradeImageList[i].transform.parent.gameObject.SetActive(true);
            }
            else
            {
                upgradeImageList[i].transform.parent.gameObject.SetActive(false);
            }
            /*
            else if (i == 5)//tree 0_lv2
            {
                upgradeImageList[i].sprite = GameAsset.I.turretSprite[index];
            }
            else if (i == 6)//tree 0_lv2
            {
                upgradeImageList[i].sprite = GameAsset.I.turretSprite[index];
            }
            else if (i == 7)//tree 0_lv3
            {
                upgradeImageList[i].sprite = GameAsset.I.turretSprite[index];
            }
            else if (i == 8)//tree 0_lv3
            {
                upgradeImageList[i].sprite = GameAsset.I.turretSprite[index];
            }*/
        }
        //all image of that turret then find by index supa simple,set them accordingly to the child turretImage......is hard af 
        //check condition to set if != null
        //
    }
    public void GetFullEnemyStat(int i)
    {
        enemyName.text = enemy[i].name;

        enemyStat[0].text = enemy[i].GetHealth() + " hp" + "\n";
        enemyStat[1].text = enemy[i].GetSpeed() + "\n";
        enemyStat[2].text = enemy[i].GetChaceToInvade() + "\n";
        enemyStat[3].text = enemy[i].worth + "\n";
        if (enemy[i].enemyState.HasFlag(EnemyState.Armored))
        {
            enemyStat[4].text = enemy[i].GetArmorHealth() + " ap" + "\n";
            enemyStat[5].text = enemy[i].GetArmorDamageReduction() + "\n";
            enemyStat[6].text = enemy[i].GetArmorLayer() + "\n";
            enemyIcons.GetChild(4).gameObject.SetActive(true);
            enemyIcons.GetChild(5).gameObject.SetActive(true);
        }
        else
        {
            enemyStat[4].text = "";
            enemyStat[5].text = "";
            enemyStat[6].text = "";
            enemyIcons.GetChild(4).gameObject.SetActive(false);
            enemyIcons.GetChild(5).gameObject.SetActive(false);
        }
        description.text = $"<color=#ffffffff>{"description:"}</color>" + 
            GameAsset.I.description[i] + "\n" + enemy[i].Ability() + "\n";
    }
    public void RemoveAllEnemyStat()
    {
        enemyName.text = "";
        enemyStat[0].text = "";
        enemyStat[1].text = "";
        enemyStat[2].text = "";
        enemyStat[3].text = "";
        enemyStat[4].text = "";
        enemyStat[5].text = "";
        enemyStat[6].text = "";
        description.text = "";
    }
    public void GetFullTurretStat(int i)
    {
        switch (i)
        {
            case 0:
                extractedTurret = clickedTurret.prefab.GetComponentInChildren<BaseTurretStat>();
                break;
            case 1:
                extractedTurret = clickedTurret.upgradePrefabs[0].GetComponentInChildren<BaseTurretStat>();
                break;
            case 2:
                extractedTurret = clickedTurret.upgradePrefabs[1].GetComponentInChildren<BaseTurretStat>();
                break;
            case 3:// tree_0_lv1
                extractedTurret = clickedTurret.ultraUpgrades[0].ultraUpgradePrefab[0].GetComponentInChildren<BaseTurretStat>();
                break;
            case 4:// tree_1_lv1
                extractedTurret = clickedTurret.ultraUpgrades[1].ultraUpgradePrefab[0].GetComponentInChildren<BaseTurretStat>();
                break;
            default:
                Debug.LogError("Something went wrong!");
                break;
        }
        turretName.text = extractedTurret.gameObject.name;
        switch (extractedTurret)
        {
            case BulletTypeTurret bulletTurret:
                turretStat[0].text = bulletTurret.GetDamage() + "\n";
                turretStat[1].text = bulletTurret.GetROF() + "s" + "\n";
                turretStat[2].text = bulletTurret.GetRange() + "\n";
                turretStat[3].text = bulletTurret.GetCritChance() + "\n";
                turretStat[4].text = bulletTurret.GetCritDamage() + "\n";
                //if(bulletTurret.)
                turretStat[5].text = bulletTurret.GetBulletSpeed() + "\n";
                turretStat[5].gameObject.SetActive(true);
                turretIcons.GetChild(5).gameObject.SetActive(true);
                statusEffect.text = bulletTurret.GetTargetingType() + "\n"+ bulletTurret.GetStatusEffect();
                break;
            case LazerTypeTurret lazerTurret:
                turretStat[0].text = lazerTurret.GetDamage() + "\n";
                turretStat[1].text = lazerTurret.GetROF() + "s" + "\n";
                turretStat[2].text = lazerTurret.GetRange() + "\n";
                turretStat[3].text = lazerTurret.GetCritChance() + "\n";
                turretStat[4].text = lazerTurret.GetCritDamage() + "\n";
                turretStat[5].gameObject.SetActive(false);
                turretIcons.GetChild(5).gameObject.SetActive(false);
                statusEffect.text = lazerTurret.GetTargetingType() + "\n"+ lazerTurret.GetStatusEffect();
                break;
        }
    }
    public void RemoveAllTurretStat()
    {
        turretName.text = "";
        turretStat[0].text = "";
        turretStat[1].text = "";
        turretStat[2].text = "";
        turretStat[3].text = "";
        turretStat[4].text = "";
        turretStat[5].text = "";
        statusEffect.text = "";
    }

    public void Debugshiz()
    {
        Debug.Log("Can click clack");
    }
    public void ShowEnemiesUI()
    {
        enemies.SetActive(true);
    }
    public void HideEnemiesUI()
    {
        enemies.SetActive(false);
    }
    public void HideTurretsUI()
    {
        turrets.SetActive(false);
    }
    public void ShowTurretsUI()
    {
        turrets.SetActive(true);
    }

    public void ShowTurretStatInfo()
    {
        turretStatInfo.gameObject.SetActive(true);
    }
    public void HideTurretStatInfo()
    {
        turretStatInfo.gameObject.SetActive(false);
    }
    public void ShowEnemyStatInfo()
    {
        enemyStatInfo.gameObject.SetActive(true);
    }
    public void HideEnemyStatInfo()
    {
        enemyStatInfo.gameObject.SetActive(false);
    }
}
