using UnityEngine;

[CreateAssetMenu(fileName = "Install_Kill", menuName = "Insta_kill")]
public class Insta_Kill : BaseEffect
{
    public bool CanKYS()
    {
        if (chance > Random.value)
        {
            return true;
        }
        return false;
    }
    public override TimedEffect init(GameObject obj)
    {
        throw new System.NotImplementedException();
    }
}
