using TMPro;
using UnityEngine;

public class CloseCombatTurret : BaseTurretStat
{
    public Animator animator;
    [SerializeField] CharacterStat damage;
    [SerializeField] float coolDown;
    private bool delay = true;
    public string etag = "Enemy";
    private float coolDownDelay;
    public GameObject damageDisplayUI;
    private Quaternion tempRotation;
    private float modifier ;

    public void OnValidate()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (damageDisplayUI == null)
        {
            damageDisplayUI = (GameObject)Resources.Load("Displayer");
        }
    }
    void Start()
    {
        coolDownDelay = coolDown;
        tempRotation = transform.rotation;
        InvokeRepeating("UpdateTarget", 0f, 0.2f);
    }
    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(etag);
        float shortestDis = Mathf.Infinity;
        GameObject nearestenemy = null;
        foreach (GameObject enemy in enemies)
        {
            float DisToenenmy = Vector3.Distance(transform.position, enemy.transform.position);//use Distancesquared??
            if (DisToenenmy < shortestDis)
            {
                shortestDis = DisToenenmy;
                nearestenemy = enemy;
            }
        }
        if (nearestenemy != null && shortestDis <= range.baseValue)
        {
            target = nearestenemy.transform;
        }
        else
        {
            target = null;
        }
    }
    void Update()
    {
        if (target == null)
        {
            return;
        }
        if (coolDownDelay <= 0f)
        {
            delay = false;
            coolDownDelay = coolDown;
        }
        coolDownDelay -= Time.deltaTime;
        RotateToObject();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Shit");
        delay = true;
        EnemyType enemyType;
        Enemy ene = collision.gameObject.GetComponent<Enemy>();
        ene.EnemyType(out enemyType);
        modifier = BaseTurretStat.CheckType(turretType, enemyType);
        
        ene.TakeDamage(damage.baseValue* modifier);
        
        GameObject damdis = Instantiate(damageDisplayUI, collision.transform.position, target.rotation);
        damdis.transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text = System.Math.Round(damage.baseValue*modifier, 4).ToString();
        Destroy(damdis, 0.5f);
    }
    public override void RotateToObject()
    {
        float angle = Mathf.Atan2(target.transform.position.y - transform.position.y, target.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        tempRotation = Quaternion.RotateTowards(tempRotation, targetRotation, rotationSpeed.baseValue * Time.deltaTime);
        //transform.rotation= Quaternion.RotateTowards(tempRotation, targetRotation, rotationSpeed.baseValue * Time.deltaTime);
        //Debug.Log(tempRotation);
        if (delay == true)
        {
            ChangeAnimationState(true, false, false, false);
            return;
        }
        if (tempRotation.eulerAngles.z >= 45 && tempRotation.eulerAngles.z <= 135)
        {
            ChangeAnimationState(false, true, false, false);
            //Debug.Log(1);
        }
        else if (tempRotation.eulerAngles.z >= 135 && tempRotation.eulerAngles.z <= 225)
        {
            ChangeAnimationState(false, false, true, false);
            transform.localRotation = Quaternion.Euler(0, 180, 0);
            //Debug.Log(2);
        }
        else if (tempRotation.eulerAngles.z >= 225 && tempRotation.eulerAngles.z <= 315)
        {
            ChangeAnimationState(false, false, false, true);
            //Debug.Log(3);
        }
        else if ((tempRotation.eulerAngles.z % 360 >= 0 && tempRotation.eulerAngles.z % 360 < 45) || (tempRotation.eulerAngles.z >= 315 && tempRotation.eulerAngles.z % 360 <= 360))
        {
            ChangeAnimationState(false, false, true, false);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            //Debug.Log(4);
        }
    }
    void ChangeAnimationState(bool a, bool b, bool c, bool d)
    {
        animator.SetBool("Idle", a);
        animator.SetBool("Turn Back", b);
        animator.SetBool("Turn Side", c);
        animator.SetBool("Turn Front", d);
    }
    bool AnimatorIsPlaying()
    {
        return animator.GetCurrentAnimatorStateInfo(0).length >
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
    bool AnimatorIsPlaying(string stateName)
    {
        return AnimatorIsPlaying() && animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
    public void BackToIdle()//Event
    {
        ChangeAnimationState(true, false, false, false);
    }
}
