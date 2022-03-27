using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCharacterFormation : MonoBehaviour
{
    // Start is called before the first frame update
    public static List<TurretBluePrint> characterLineUp;
    [SerializeField] private int moneyEachMatch;

    private void Start()
    {
        characterLineUp = new List<TurretBluePrint>();
    }
    public void SetRewardMoney(int m)
    {
        moneyEachMatch = m;
    }
    public int GetMoney()
    {
        return moneyEachMatch;
    }
    public void AddCharacter(TurretBluePrint character)
    {
        characterLineUp.Add(character);
    }
    public void RemoveCharacter(TurretBluePrint character)
    {
        characterLineUp.Remove(character);
    }

}
