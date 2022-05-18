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
                Name = "��",
                StartMessage = "�Ѿ��ж���",
                //OnAfterTurn = PoisonEffect
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHp / 8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}��Ϊ�ж��ܵ����˺�");
                }
            }
        },
        {
            ConditionID.brn,
            new Condition()
            {
                Name = "����",
                StartMessage = "�Ѿ�������",
                //OnAfterTurn = PoisonEffect
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHp / 16);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}��Ϊ�����ܵ����˺�");
                }
            }
        },
        {
            ConditionID.par,
            new Condition()
            {
                Name = "���",
                StartMessage = "�Ѿ������",
                //OnAfterTurn = PoisonEffect
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (Random.Range(1, 5) == 1)
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}��Ϊ����޷��ж�");
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
                Name = "����",
                StartMessage = "�Ѿ�����ס��",
                //OnAfterTurn = PoisonEffect
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (Random.Range(1, 5) == 1)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}�ӱ����ָ���");
                        return true;
                    }
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}��Ϊ�����޷��ж�");
                    return false;
                }
            }
        },
        {
            ConditionID.slp,
            new Condition()
            {
                Name = "˯��",
                StartMessage = "�Ѿ���˯����",
                OnStart = (Pokemon pokemon) =>
                {
                    // ˯ 1 - 3 
                    pokemon.StatusTime = Random.Range(1, 4);
                    Debug.Log($"will be asleep for {pokemon.StatusTime} moves");
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}˯����");
                        return true;
                    }
                    pokemon.StatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}���ں�����˯");
                    return false;
                }
            }
        },

        // Volatile status condition
        {
            ConditionID.confusion,
            new Condition()
            {
                Name = "����",
                StartMessage = "��û�����",
                OnStart = (Pokemon pokemon) =>
                {
                    // ����1-4
                    pokemon.VolatileStatusTime = Random.Range(1, 5);
                    Debug.Log($"will be confused for {pokemon.VolatileStatusTime} moves");
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.VolatileStatusTime <= 0)
                    {
                        pokemon.CureVolitileStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}���������");
                        return true;
                    }
                    pokemon.VolatileStatusTime--;
                    // 50% chance to do a move
                    if (Random.Range(1, 3) == 1)
                    {
                        return true;
                    }
                    // hurt by confusion
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} ������");
                    pokemon.UpdateHP(pokemon.MaxHp / 8);
                    pokemon.StatusChanges.Enqueue($"���Լ���ס��");
                    return false;
                }
            }
        }
    };
}

public enum ConditionID
{
    // �� �ж��� ȼ�� ˯�� ��� ����, ����
    none, psn, brn,slp,par,frz, confusion
}
