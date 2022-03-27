public enum StatModType
{
    None=0,
    Flat = 100,
    PercentAdd = 200,
    PercentMult = 300,
    PercentDebuffBest = 400,
}
[System.Serializable]
public class StatValueType
{
    public StatModType modType;
    public CharacterStat statValue;
}

public class StatModifier
{
    public readonly float value;
    public readonly StatModType type;
    public readonly int order;
    public readonly object source; // Added this variable

    // "Main" constructor. Requires all variables.
    public StatModifier(float Value, StatModType Type, int Order, object Source) // Added "source" input parameter
    {
        value = Value;
        type = Type;
        order = Order;
        source = Source; // Assign Source to our new input parameter
    }

    // Requires Value and Type. Calls the "Main" constructor and sets Order and Source to their default values: (int)type and null, respectively.
    public StatModifier(float value, StatModType type) : this(value, type, (int)type, null) { }

    // Requires Value, Type and Order. Sets Source to its default value: null
    public StatModifier(float value, StatModType type, int order) : this(value, type, order, null) { }

    // Requires Value, Type and Source. Sets Order to its default value: (int)Type
    public StatModifier(float value, StatModType type, object source) : this(value, type, (int)type, source) { }
}
