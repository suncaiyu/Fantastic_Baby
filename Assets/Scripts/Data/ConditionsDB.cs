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
                Name = "��",
                StartMessage = "�Ѿ��ж���",
                //OnAfterTurn = PoisonEffect
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHp / 8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}��Ϊ�ж��ܵ����˺�");
                }
            }
        }
    };
}

public enum ConditionID
{
    // �� �ж��� ȼ�� ˯�� ��� ����
    none, psn, brn,slp,par,frz
}
