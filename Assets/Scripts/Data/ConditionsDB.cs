using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
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
        }
    };
}

public enum ConditionID
{
    // 无 中毒， 燃烧 睡眠 麻痹 冰冻
    none, psn, brn,slp,par,frz
}
