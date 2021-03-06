using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Pokemon
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;
    public PokemonBase Base { get { return _base; } }
    public int Level { get { return level; } }

    public int HP { get; set; }

    public List<Move> Moves { get; set; }
    public Move CurrentMove { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    // 自身的一些状态buff以及等级
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    public Condition Status { get; private set; }
    public Condition VolatileStatus { get; private set; }
    public int VolatileStatusTime {get;set;}

    public int StatusTime { get; set; }

    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();

    public bool HpChanged { get; set; }

    public event System.Action OnStatusChanged;
    public void Init()
    {
        // 初始化技能
        Moves = new List<Move>();
        foreach(var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
            {
                Moves.Add(new Move(move.MoveBase));
            }
            if (Moves.Count >= 4)
            {
                break;
            }
        }
        CalculateStats();
        HP = MaxHp;

        ResetStatBoost();
        CureStatus();
        CureVolitileStatus();
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5);

        MaxHp = Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10 + level;
    }

    void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            { Stat.Attack, 0},
            { Stat.Defense, 0},
            {Stat.SpAttack, 0 },
            { Stat.SpDefense, 0},
            {Stat.Speed, 0 },
            { Stat.Accuracy,0},
            { Stat.Evasion, 0},
        };
    }

    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];
        // Apply stat boost
        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f};
        if (boost > 0)
        {
            statVal = Mathf.FloorToInt( statVal * boostValues[boost]);
        } else
        {
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);
        }
        return statVal;
    }
    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statboost in statBoosts)
        {
            var stat = statboost.stat;
            var boost = statboost.boost;

            StatBoosts[stat] = Mathf.Clamp((StatBoosts[stat] + boost), -6, 6);
            if (boost > 0)
            {
                StatusChanges.Enqueue($"{Base.Name}的{stat}提升了");
            } else
            {
                StatusChanges.Enqueue($"{Base.Name}的{stat}降低了");
            }

            Debug.Log($"{stat} 已经被叠加到 {StatBoosts[stat]}");
        }
    }

    public int Attack
    {
        get => GetStat(Stat.Attack);
    }

    public int Defense
    {
        get => GetStat(Stat.Defense);
    }

    public int SpAttack
    {
        get => GetStat(Stat.SpAttack);
    }

    public int SpDefense
    {
        get => GetStat(Stat.SpDefense);
    }

    public int Speed
    {
        get => GetStat(Stat.Speed);
    }

    public int MaxHp
    {
        get;
        private set;
    }

    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {
        // 暴击
        float critical = 1f;
        if (Random.value * 100f < 6.25f)
        {
            critical = 2f;
        }
        // 属性关系
        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);
        Debug.Log(attacker.Base.Name + type);
        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };

        float attack = (move.Base.Category == MoveCategory.Special) ? attacker.SpAttack : attacker.Attack;
        float defense = (move.Base.Category == MoveCategory.Special) ? SpDefense : Defense;

        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * (attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        UpdateHP(damage);
        return damageDetails;
    }

    public void SetStatus(ConditionID conditionID)
    {
        if (Status != null) {
            return;
        }
        Status =  ConditionsDB.Conditions[conditionID];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name}{Status.StartMessage}");
        OnStatusChanged?.Invoke();
    }
    public Move GetRandomMove()
    {
        var moveWidthPP = Moves.Where(x => x.PP > 0).ToList();
        int r = Random.Range(0, moveWidthPP.Count);
        return moveWidthPP[r];
    }

    public void OnAfterTurn()
    {
        //if (Status!= null)
        //{
        //    Status.OnAfterTurn(this);
        //}
        //Status?.OnAfterTurn(this);
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    public void SetVolatileStatus(ConditionID conditionID)
    {
        if (VolatileStatus != null)
        {
            return;
        }
        VolatileStatus = ConditionsDB.Conditions[conditionID];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name}{VolatileStatus.StartMessage}");
    }
    public void CureVolitileStatus()
    {
        VolatileStatus = null;
    }

    public bool OnBeforeMove()
    {
        bool canPerformMove = true;
        if (Status?.OnBeforeMove != null)
        {
            if (!Status.OnBeforeMove(this))
            {
                canPerformMove = false;
            }
        }

        if (VolatileStatus?.OnBeforeMove != null)
        {
            if (!VolatileStatus.OnBeforeMove(this))
            {
                canPerformMove = false;
            }
        }

        return canPerformMove;
    }

    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoost();
    }

    public void UpdateHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHp);
        HpChanged = true;
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}