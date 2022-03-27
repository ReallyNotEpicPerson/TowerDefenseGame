[System.Flags]
public enum DamageType
{
    Normal,
    Rate,
    //Stackable,
    //Stackable_After_Time_Full,
    //Stackable_After_Time_Rate,
    AfterSecond,
    AfterSecondRate,
    Percentage,
    Additional,

}
public interface IDamage <T1, T2, T3>
{
    int stackNum { get; set; }
    public void Damage(T1 amount);
    public void Damage(T1 amount, T2 rate);//Decrement after time
    public void Damage_After_Second(T1 amount, T2 second);//full hit after second
    public void Damage_After_Second(T1 amount, T2 second, T3 rate);//slowly take effect by rate after second
    public void Damage_Percent(T1 Percent);//OP af
    //public void Damage_Stack_Times(T1 amount, int time);//add damage nth + 1 times 
    public void Damage_Additional(T1 amount, T2 additionalAmount);
}