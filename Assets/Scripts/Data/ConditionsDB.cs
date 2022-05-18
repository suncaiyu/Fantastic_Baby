using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static void Init()
    {
        foreach(var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId;
        }
    }
    static void PoisonEffect(Pokemon pokemon)
    {

    }
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>() {
        {
            ConditionID.psn,
            new Condition()
            {
                Name = "毒",
                StartMessage = "已经中毒了",
                //OnAfterTurn = PoisonEffect
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHp / 8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}因为中毒受到了伤害");
                }
            }
        },
        {
            ConditionID.brn,
            new Condition()
            {
                Name = "烧伤",
                StartMessage = "已经烧伤了",
                //OnAfterTurn = PoisonEffect
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHp / 16);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}因为灼烧受到了伤害");
                }
            }
        },
        {
            ConditionID.par,
            new Condition()
            {
                Name = "麻痹",
                StartMessage = "已经麻痹了",
                //OnAfterTurn = PoisonEffect
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (Random.Range(1, 5) == 1)
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}因为麻痹无法行动");
                        return false;
                    }
                    return true;
                }
            }
        },
        {
            ConditionID.frz,
            new Condition()
            {
                Name = "冰冻",
                StartMessage = "已经被冻住了",
                //OnAfterTurn = PoisonEffect
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (Random.Range(1, 5) == 1)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}从冰冻恢复了");
                        return true;
                    }
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}因为冰冻无法行动");
                    return false;
                }
            }
        },
        {
            ConditionID.slp,
            new Condition()
            {
                Name = "睡眠",
                StartMessage = "已经被睡眠了",
                OnStart = (Pokemon pokemon) =>
                {
                    // 睡 1 - 3 
                    pokemon.StatusTime = Random.Range(1, 4);
                    Debug.Log($"will be asleep for {pokemon.StatusTime} moves");
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}睡醒了");
                        return true;
                    }
                    pokemon.StatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}正在呼呼大睡");
                    return false;
                }
            }
        },

        // Volatile status condition
        {
            ConditionID.confusion,
            new Condition()
            {
                Name = "混乱",
                StartMessage = "变得混乱了",
                OnStart = (Pokemon pokemon) =>
                {
                    // 混乱1-4
                    pokemon.VolatileStatusTime = Random.Range(1, 5);
                    Debug.Log($"will be confused for {pokemon.VolatileStatusTime} moves");
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.VolatileStatusTime <= 0)
                    {
                        pokemon.CureVolitileStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}解除混乱了");
                        return true;
                    }
                    pokemon.VolatileStatusTime--;
                    // 50% chance to do a move
                    if (Random.Range(1, 3) == 1)
                    {
                        return true;
                    }
                    // hurt by confusion
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} 混乱了");
                    pokemon.UpdateHP(pokemon.MaxHp / 8);
                    pokemon.StatusChanges.Enqueue($"把自己淦住了");
                    return false;
                }
            }
        }
    };
}

public enum ConditionID
{
    // 无 中毒， 燃烧 睡眠 麻痹 冰冻, 混乱
    none, psn, brn,slp,par,frz, confusion
}
