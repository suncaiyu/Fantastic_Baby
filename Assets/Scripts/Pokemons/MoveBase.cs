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
    [SerializeField] int accuracy;
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] MoveTarget target;

    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public PokemonType Type { get => type; set => type = value; }
    public int Power { get => power; set => power = value; }
    public int Pp { get => pp; set => pp = value; }
    public int Accuracy { get => accuracy; set => accuracy = value; }

    public MoveCategory Category { get => category; }
    public MoveTarget Target { get => target; }
    public MoveEffects Effects { get => effects; }
}

[System.Serializable]
public class MoveEffects 
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionID status;

    public List<StatBoost> Boosts
    {
        get => boosts;
    }
    public ConditionID Status { get => status; }
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