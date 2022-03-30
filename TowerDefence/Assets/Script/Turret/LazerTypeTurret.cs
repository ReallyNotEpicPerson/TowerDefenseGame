using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class LazerTypeTurret : BaseTurretStat
{
    public CharacterStat critChance;//maybe
    public CharacterStat critDamage;//maybe
    public CharacterStat damageOverTime;//no
    public CharacterStat slowPtc;//no
    public Transform firePoint;//wait

    public LineRenderer lineRenderer;
    public ParticleSystem lazerFX;

    public string etag = "Enemy";
    private Enemy instance;
    private Enemy tar;

    public GameObject damageDisplayUI;
    private GameObject hiddenUI;
    private TMP_Text txt;
    private TMP_TextController txtManip; 

    public void OnValidate()
    {

        if (lineRenderer==null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        if (lazerFX == null)
        {
            lazerFX = transform.GetChild(0).GetComponentInChildren<ParticleSystem>();
        }

    }

    private void Start()
    {
        hiddenUI = Instantiate(damageDisplayUI, transform.position, Quaternion.identity);
        hiddenUI.name = "DamageDisplayer" + "of" + this.name;
        hiddenUI.SetActive(false);
        txt = hiddenUI.transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>();
        txtManip = txt.GetComponent<TMP_TextController>();
        //InvokeRepeating("UpdateTarget", 0f, 0.2f);
        InvokeRepeating(nameof(UpdateTarget), 0f, 0.2f);
    }
    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(etag);
        float shortestDis = Mathf.Infinity;
        GameObject nearestenemy = null;
        foreach (GameObject enemy in enemies)
        {
            float DisToenenmy = Vector3.Distance(transform.position, enemy.transform.position);
            if (DisToenenmy < shortestDis)
            {
                shortestDis = DisToenenmy;
                nearestenemy = enemy;
            }
        }
        if (nearestenemy != null && shortestDis <= range.baseValue)
        {
            target = nearestenemy.transform;
            tar = nearestenemy.GetComponent<Enemy>();
            if (instance != null)
            {
                instance.EffectColor(Color.white);
            }
        }
        else
        {
            target = null;
        }
    }
    void Update()
    {
        instance = tar;
        if (target == null)
        {
            if (lineRenderer.enabled)
            {
                lineRenderer.enabled = false;
                lazerFX.Stop();
            }
            hiddenUI.SetActive(false);
            return;
        }
        RotateToObject();
        hiddenUI.SetActive(true);
        LazerShoot();
    }
    float LazerDamageWithCrit()
    {
        return damageOverTime.baseValue + (damageOverTime.baseValue * critChance.baseValue) * Time.deltaTime;
    }
    float LazerDamageWithoutCrit()
    {
        return damageOverTime.baseValue * Time.deltaTime;
    }
    void LazerShoot()
    {
        hiddenUI.transform.position = tar.transform.position;
        if (Random.value <= critChance.baseValue)
        {
            tar.TakeDamage(LazerDamageWithCrit());
            txt.text = (System.Math.Round(LazerDamageWithCrit(), 2).ToString());
            txtManip.Blushing();
        }
        else
        {
            tar.TakeDamage(LazerDamageWithoutCrit());
            txt.text = (System.Math.Round(LazerDamageWithoutCrit(), 2).ToString());
            txtManip.NormalColor(new Color32(255, 255, 255, 255), new Color32(0, 0, 0, 255));
        }
        //tar.ConstantSlow(slowPtc.baseValue);
        tar.EffectColor(Color.blue);
        //Debug.Log(tar.name);
        if (!lineRenderer.enabled)
        {
            lineRenderer.enabled = true;
            lazerFX.Play();
        }
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target.position);
        Vector3 dir = firePoint.position - target.position;
        lazerFX.transform.position = target.position + dir.normalized;
        //lazerFX.transform.rotation = Quaternion.LookRotation(dir);
    }
    /*svoid RotateToObj2()
    {
        float angle = Mathf.Atan2(target.transform.position.y - transform.position.y, target.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed.baseValue * Time.deltaTime);
        Debug.DrawRay(transform.position, target.transform.position);
    }*/
    public void DestroyLeftoverUI()
    {
        Destroy(hiddenUI);
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.green;
        Handles.DrawWireDisc(transform.position, new Vector3(0, 0, 1), range.Value);
        /* Gizmos.color = Color.green;
         Gizmos.DrawWireSphere(transform.position, Range);*/
    }
#endif
}
