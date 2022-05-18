using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PokemonMove", menuName = "Pokemon/Create pokemon move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] PokemonType type;
    [SerializeField] int power;
    [SerializeField] int pp;
    [SerializeField] int priority;
    [SerializeField] int accuracy;
    [SerializeField] bool alwaysHit;
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] List<SecondaryEffects> secondaries;
    [SerializeField] MoveTarget target;

    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public PokemonType Type { get => type; set => type = value; }
    public int Power { get => power; set => power = value; }
    public int Pp { get => pp; set => pp = value; }
    public int Accuracy { get => accuracy; set => accuracy = value; }
    public bool AlwaysHit { get => alwaysHit; set => alwaysHit = value; }

    public MoveCategory Category { get => category; }
    public MoveTarget Target { get => target; }
    public MoveEffects Effects { get => effects; }
    public List<SecondaryEffects> Secondaries { get => secondaries; set => secondaries = value; }
    public int Priority { get => priority; set => priority = value; }
}

[System.Serializable]
public class MoveEffects 
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionID status;
    [SerializeField] ConditionID volatileStatus;

    public List<StatBoost> Boosts
    {
        get => boosts;
    }
    public ConditionID Status { get => status; }
    public ConditionID VolatileStatus { get => volatileStatus; }
}

[System.Serializable]
public class SecondaryEffects : MoveEffects
{
    [SerializeField] int chance;
    [SerializeField] MoveTarget target;

    public int Chance { get => chance; set => chance = value; }
    public MoveTarget Target { get => target; set => target = value; }
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

public enum MoveCategory
{
    Physical, Special, Status
}

public enum MoveTarget
{
    Foe, Self
}