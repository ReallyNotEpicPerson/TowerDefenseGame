using TMPro;
using UnityEngine;

[System.Flags]
public enum DamageDisplayerType
{
    Normal = 0,
    Critial = 1 << 0,
    ArmorHit = 1 << 1,
    Burned = 1 << 2,
    ArmorPenetration = 1 << 3,
    Poisoned = 1 << 4,
    Miss = 1 << 5,
    Insta_kill = 1 << 6,
    NO = 1 << 7,
    Non_Damage_Display = 1 << 8,
}

public class DamageDisplayer : MonoBehaviour
{
    private TextMeshPro textMesh;
    private Color textColor;
    private float disappearTimer;
    //private int limitCounter;
    [SerializeField] private DamageDisplayerType displayType;
    //private static int sortingOrder

    void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
        textMesh.color = Color.white;
    }
    private void Start()
    {
        textMesh.fontSize = 5;
    }
    public static DamageDisplayer Create(Vector3 pos, float amount, DamageDisplayerType type = DamageDisplayerType.Normal)
    {
        Transform DamagePopUp = Instantiate(GameAsset.I.damageDisplayer, pos, Quaternion.identity);
        DamageDisplayer displayer = DamagePopUp.GetComponent<DamageDisplayer>();
        displayer.SetUp(amount, type);
        //Debug.Log(critical);
        return displayer;
    }
    public static DamageDisplayer Create(Vector3 pos, string str = "MISS", DamageDisplayerType type = DamageDisplayerType.Miss)
    {
        Transform DamagePopUp = Instantiate(GameAsset.I.damageDisplayer, pos, Quaternion.identity);
        DamageDisplayer displayer = DamagePopUp.GetComponent<DamageDisplayer>();
        displayer.SetUp(str, type);
        return displayer;
    }
    public void SetUp(string str, DamageDisplayerType type = DamageDisplayerType.Miss)
    {
        EnableState(type);
        textMesh.SetText(str);
        switch (displayType)
        {
            case DamageDisplayerType.Miss:
                disappearTimer = 1f;
                textMesh.SetText(str);
                textColor = textMesh.color;
                break;
            case DamageDisplayerType.Insta_kill:
                disappearTimer = 2f;
                textMesh.SetText($"<color=#ff0000ff>{str}</color>");
                textColor = textMesh.color;
                break;
            case DamageDisplayerType.Non_Damage_Display:
                disappearTimer = 0.5f;
                textMesh.fontSize += 45;
                textMesh.color = Color.yellow;
                disappearTimer = 1f;
                break;
        }
        textColor = textMesh.color;
    }

    public void SetUp(float amount, DamageDisplayerType type = DamageDisplayerType.Normal)
    {
        EnableState(type);
        //Debug.Log(amount);
        textMesh.SetText(((float)System.Math.Round(amount, 2)).ToString());
        switch (displayType)
        {
            case DamageDisplayerType.Normal:
                textMesh.color = Color.white;
                disappearTimer = 0.5f;
                break;
            case DamageDisplayerType.Critial:
                textMesh.fontSize += 4;
                textMesh.color = Color.red;
                disappearTimer = 0.5f;
                //Debug.Log("Red");
                break;
            case DamageDisplayerType.ArmorHit:
                textMesh.fontSize += 1;
                textMesh.color = Color.blue;
                disappearTimer = 1f;
                break;
            case DamageDisplayerType.Burned:
                textMesh.fontSize -= 1;
                textMesh.color = Color.yellow;
                disappearTimer = 0.5f;
                break;
            case DamageDisplayerType.Poisoned:
                textMesh.fontSize -= 1;
                textMesh.color = Color.green;
                disappearTimer = 0.5f;
                break;
            case DamageDisplayerType.ArmorPenetration:
                textMesh.fontSize += 2;
                textMesh.color = Color.cyan;
                disappearTimer = 1f;
                break;           
            default:
                Debug.LogError("Fuckkkkk!!!");
                break;
        }
        /*if (type.HasFlag(DamageDisplayerType.Critial))
        {
            textMesh.fontSize += 2;
            textMesh.color = Color.red;
            //Debug.Log("Red");
        }*/

        textColor = textMesh.color;

        //sortingOrder++;
        //textMesh.sortingOrder = sortingOrder;
    }
    void SetTextTo(string str)
    {
        textMesh.SetText(str);
    }
    private void Update()
    {
        switch (displayType)
        {
            case DamageDisplayerType.Normal:
                TextGoUpAndDisappear(3f, 4f);
                break;
            case DamageDisplayerType.Miss:
                TextGoUpAndDisappear(2f, 5f);
                break;
            case DamageDisplayerType.Insta_kill:
                TextGoUpAndDisappear(2f, 5f);
                break;
            case DamageDisplayerType.Critial:
                TextGoUpAndDisappear(2f, 20f);
                break;
            case DamageDisplayerType.ArmorHit:
                TextGoUpAndDisappear(1.2f, 1f);
                break;
            case DamageDisplayerType.Burned:
                TextGoUpAndDisappear(1f, 20f);
                break;
            case DamageDisplayerType.Poisoned:
                TextGoUpAndDisappear(1f, 20f);
                break;
            case DamageDisplayerType.ArmorPenetration:
                TextGoUpAndDisappear(2f, 10f);
                break;
            case DamageDisplayerType.Non_Damage_Display:
                TextGoUpAndDisappear(0, 1f);
                break;
            default:
                break;
        }
        /*
        if (displayType.HasFlag(DamageDisplayerType.Normal) )
        {
            TextGoUpAndDisappear(3f, 4f);
        }
        if (displayType.HasFlag(DamageDisplayerType.Critial) )
        {
            TextGoUpAndDisappear(0.01f, 30f, 1);
        }
        if (displayType.HasFlag(DamageDisplayerType.ArmorHit))
        {
            TextGoUpAndDisappear(0.3f, 1f, 1);
        }
        if (displayType.HasFlag(DamageDisplayerType.Burned))
        {
            TextGoUpAndDisappear(0.2f, 10f, 5);
        }*/
    }

    public void TextGoUpAndDisappear(float YaxisSpeed, float DisappearSpeed)
    {
        transform.position += new Vector3(0, YaxisSpeed) * Time.deltaTime;
        disappearTimer -= Time.deltaTime;

        if (disappearTimer < 0)
        {
            textColor.a -= DisappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
                return;
            }
        }
    }
    public void DisableState(DamageDisplayerType es)
    {
        displayType &= ~es;
    }
    public void DisableState()
    {
        //displayType &= ~all;
    }
    public void EnableState(DamageDisplayerType es)
    {
        displayType |= es;
    }
}
