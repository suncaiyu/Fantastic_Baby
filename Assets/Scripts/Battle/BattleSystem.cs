using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  ս���еļ���״̬
public enum BattleState { Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartyScreen, BattleOver }
public enum BattleAction { Move, SwitchPokemon, UseItem, Run }
// ս���߼�ϵͳ
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

    public event Action<bool> OnBattleOver;
    BattleState state;
    BattleState? prevState;
    int currentAction = 0;
    int currentMove = 0;
    int currentMember;

    PokemonParty playerParty;
    Pokemon wildPokemon;
   
    /*
     * ��ʼս��
     * param : party player����Я����pokemon��
     * param : wild �ж�Ұ��������
     */
    public void StartBattle(PokemonParty party, Pokemon wild)
    {
        wildPokemon = wild;
        playerParty = party;
        StartCoroutine(SetUpBattle());
    }

    /*
     * brief ��ʼ������˫������Ϣ
     */
    private IEnumerator SetUpBattle()
    {
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(wildPokemon);

        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.pokemon.Moves);
        yield return dialogBox.TypeDialog($"һ��Ұ����{enemyUnit.pokemon.Base.Name}�����ˣ�");
        //StartCoroutine(dialogBox.TypeDialog($"һ��Ұ����{enemyUnit.pokemon._base.Name}�����ˣ�"));

        ActionSelection();
    }

    /*
     * brief : ����ж�ѡ��
     */
    private void ActionSelection()
    {
        state = BattleState.ActionSelection;
        StartCoroutine(dialogBox.TypeDialog("��Ҫ��ô��"));
        dialogBox.EnableActionSelector(true);
    }

    /*
     * brief : ��Ҽ���ѡ��
     */
    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableMoveSelector(true);
        dialogBox.EnableDialogText(false);
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        } else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        } else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
    }
    
    // ������pokemon����
    private void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ++currentMember;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --currentMember;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMember += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMember -= 2;
        }
        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Pokemons.Count - 1);
        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = playerParty.Pokemons[currentMember];
            if (selectedMember.HP <= 0) {
                partyScreen.SetMessageText("���Ѿ�GG�ˣ����ֵ�");
                return;
            }
            if (selectedMember == playerUnit.pokemon)
            {
                partyScreen.SetMessageText("���Ѿ����ڳ����ˣ��ֵ�");
                return;
            }
            
            partyScreen.gameObject.SetActive(false);
            if (prevState == BattleState.ActionSelection)
            {
                prevState = null;
                StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
            } 
            else
            {
                state = BattleState.Busy;
                StartCoroutine(SwitchPokemon(selectedMember));
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    /*
     * brief : ����pokemon 
     */
    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        if (playerUnit.pokemon.HP > 0)
        {
            yield return dialogBox.TypeDialog($"�쳷! {playerUnit.pokemon.Base.Name}");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }
        playerUnit.Setup(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"��ס�� {newPokemon.Base.Name}!");
        currentMove = 0;

        state = BattleState.RunningTurn;
    }

    // ������ѡ��
    private void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ++currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
                currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
                currentMove -= 2;
        }
        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.pokemon.Moves.Count - 1);
        dialogBox.UpdateMoveSelection(currentMove, playerUnit.pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var move = playerUnit.pokemon.Moves[currentMove];
            if (move.PP == 0) return;
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(RunTurns(BattleAction.Move));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            // ������һ���ж�ѡȡ
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableActionSelector(true);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    // չʾ���ܵ�������Ϣ
    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
        {
            yield return dialogBox.TypeDialog("����һ��!");
        }
        if(damageDetails.TypeEffectiveness > 1f)
        {
            yield return dialogBox.TypeDialog("Ч����Ⱥ��!");
        } else if (damageDetails.TypeEffectiveness < 1f)
        {
            yield return dialogBox.TypeDialog("��Ч��΢��!");
        }
    }

    // �����ж�ѡ��
    private void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ++currentAction;
        } else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentAction += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentAction -= 2;
        }
        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                // Fight
                MoveSelection();
            } else if (currentAction == 1)
            {
                // bag
            }else if (currentAction == 2)
            {
                // pokemon
                prevState = state;
                OpenPartyScreen();
            }else if (currentAction == 3)
            {
                // run
                OnBattleOver(true);
            }
        }
    }

    /*
     * brief : ��pokemon����
     */
    private void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);
    }

    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;

        if (playerAction == BattleAction.Move)
        {
            playerUnit.pokemon.CurrentMove = playerUnit.pokemon.Moves[currentMove];
            enemyUnit.pokemon.CurrentMove = enemyUnit.pokemon.GetRandomMove();

            int playerMovePriority = playerUnit.pokemon.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.pokemon.CurrentMove.Base.Priority;

            //Check Who Get First
            bool playerGoesFirst = true;
            if (enemyMovePriority > playerMovePriority)
            {
                playerGoesFirst = false;
            }
            else if (enemyMovePriority == playerMovePriority)
            {
                playerGoesFirst = playerUnit.pokemon.Speed >= enemyUnit.pokemon.Speed;
            }

            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            var secondPokemon = secondUnit.pokemon;

            // First Tuen
            yield return RunMove(firstUnit, secondUnit, firstUnit.pokemon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;

            if (secondPokemon.HP > 0)
            {
                // Second Ture
                yield return RunMove(secondUnit, firstUnit, secondUnit.pokemon.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }
        }
        else
        {
            if (playerAction == BattleAction.SwitchPokemon)
            {
                var selectedPokemon = playerParty.Pokemons[currentMember];
                state = BattleState.Busy;
                yield return SwitchPokemon(selectedPokemon);
            }

            // Enemy Turn
            var enemyMove = enemyUnit.pokemon.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver) yield break;
        }
        if (state != BattleState.BattleOver)
        {
            ActionSelection();
        }
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canMove = sourceUnit.pokemon.OnBeforeMove();
        if (!canMove)
        {
            yield return ShowStatusChanges(sourceUnit.pokemon);
            yield return sourceUnit.Hud.UpdateHP();

            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.pokemon);
        yield return dialogBox.TypeDialog($"{ sourceUnit.pokemon.Base.Name} ʹ����{move.Base.Name}");
        move.PP--;

        if (CheckIfMoveHit(move, sourceUnit.pokemon, targetUnit.pokemon))
        {
            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);
            targetUnit.PlayHitAnimation();

            if (move.Base.Category == MoveCategory.Status)
            {
                yield return RunMoveEffects(move.Base.Effects, sourceUnit.pokemon, targetUnit.pokemon, move.Base.Target);
            }
            else
            {
                var damageDetails = targetUnit.pokemon.TakeDamage(move, sourceUnit.pokemon);
                yield return (targetUnit.Hud.UpdateHP());
                yield return ShowDamageDetails(damageDetails);
            }

            if (move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targetUnit.pokemon.HP > 0)
            {
                foreach(var secondary in move.Base.Secondaries)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd < secondary.Chance)
                    {
                        yield return RunMoveEffects(secondary, sourceUnit.pokemon, targetUnit.pokemon, secondary.Target);
                    }
                }
            }

            if (targetUnit.pokemon.HP <= 0)
            {
                targetUnit.PlayFaintAnimation();
                yield return dialogBox.TypeDialog($"{targetUnit.pokemon.Base.Name}������!");
                yield return new WaitForSeconds(2f);

                CheckForBattleOver(targetUnit);
            }
        } else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.pokemon.Base.Name}�ļ���������!");
        }

        //// ����ȼ�գ��ж���״̬
        //sourceUnit.pokemon.OnAfterTurn();
        //yield return ShowStatusChanges(sourceUnit.pokemon);
        //yield return sourceUnit.Hud.UpdateHP();
        //if (sourceUnit.pokemon.HP <= 0)
        //{
        //    sourceUnit.PlayFaintAnimation();
        //    yield return dialogBox.TypeDialog($"{sourceUnit.pokemon.Base.Name}������!");
        //    yield return new WaitForSeconds(2f);

        //    CheckForBattleOver(sourceUnit);
        //}
    }

    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(()=>state == BattleState.RunningTurn);
        // ����ȼ�գ��ж���״̬
        sourceUnit.pokemon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.pokemon);
        yield return sourceUnit.Hud.UpdateHP();
        if (sourceUnit.pokemon.HP <= 0)
        {
            sourceUnit.PlayFaintAnimation();
            yield return dialogBox.TypeDialog($"{sourceUnit.pokemon.Base.Name}������!");
            yield return new WaitForSeconds(2f);

            CheckForBattleOver(sourceUnit);
        }
    }

    IEnumerator RunMoveEffects(MoveEffects effects, Pokemon source, Pokemon target, MoveTarget moveTarget)
    {
        // Stat Boosting ����buff
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
            {
                source.ApplyBoosts(effects.Boosts);
            }
            else
            {
                target.ApplyBoosts(effects.Boosts);
            }
        }
        // Stat Condition ����״̬
        if (effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }

        // ����״̬
        if (effects.VolatileStatus != ConditionID.none)
        {
            target.SetVolatileStatus(effects.VolatileStatus);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    bool CheckIfMoveHit(Move move, Pokemon source, Pokemon target)
    {
        if (move.Base.AlwaysHit)
        {
            return true;
        }
        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Evasion];

        var boostValue = new float[] {1f, 4f/3f, 5f/3f, 2f, 7f/3f, 8f/3f, 3f };
        if (accuracy > 0)
        {
            moveAccuracy *= boostValue[accuracy];
        } else
        {
            moveAccuracy /= boostValue[-accuracy];
        }

        if (evasion > 0)
        {
            moveAccuracy /= boostValue[evasion];
        }
        else
        {
            moveAccuracy *= boostValue[-evasion];
        }

        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }
    IEnumerator ShowStatusChanges(Pokemon pokemon)
    {
        while(pokemon.StatusChanges.Count > 0)
        {
            var message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
            {
                OpenPartyScreen();
            } else
            {
                BattleOver(false);
            }
        } else
        {
            BattleOver(true);
        }
    }

    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        playerParty.Pokemons.ForEach(p => p.OnBattleOver());
        OnBattleOver(won);
    }
}
