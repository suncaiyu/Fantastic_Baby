using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  战斗中的几个状态
public enum BattleState
{
    Start,
    ActionSelection,
    MoveSelection,
    PerformMove,
    Busy,
    PartyScreen,
    BattleOver
}

// 战斗逻辑系统
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

    public event Action<bool> OnBattleOver;
    BattleState state;
    int currentAction = 0;
    int currentMove = 0;
    int currentMember;

    PokemonParty playerParty;
    Pokemon wildPokemon;
   
    /*
     * 开始战斗
     * param : party player身上携带的pokemon包
     * param : wild 敌对野生宝可梦
     */
    public void StartBattle(PokemonParty party, Pokemon wild)
    {
        wildPokemon = wild;
        playerParty = party;
        StartCoroutine(SetUpBattle());
    }

    void ChooseFirstTurn()
    {
        if (playerUnit.pokemon.Speed > enemyUnit.pokemon.Speed)
        {
            ActionSelection();
        } else
        {
            StartCoroutine(EnemyMove());
        }
    }

    /*
     * brief 初始化敌我双方的信息
     */
    private IEnumerator SetUpBattle()
    {
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(wildPokemon);

        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.pokemon.Moves);
        yield return dialogBox.TypeDialog($"一个野生的{enemyUnit.pokemon.Base.Name}出现了！");
        //StartCoroutine(dialogBox.TypeDialog($"一个野生的{enemyUnit.pokemon._base.Name}出现了！"));

        ChooseFirstTurn();
    }

    /*
     * brief : 玩家行动选择
     */
    private void ActionSelection()
    {
        state = BattleState.ActionSelection;
        StartCoroutine(dialogBox.TypeDialog("你要怎么做"));
        dialogBox.EnableActionSelector(true);
    }

    /*
     * brief : 玩家技能选择
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
    
    // 处理交换pokemon操作
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
                partyScreen.SetMessageText("它已经GG了！！兄弟");
                return;
            }
            if (selectedMember == playerUnit.pokemon)
            {
                partyScreen.SetMessageText("它已经顶在场上了！兄弟");
                return;
            }

            partyScreen.gameObject.SetActive(false);
            state = BattleState.Busy;
            StartCoroutine(SwitchPokemon(selectedMember));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    /*
     * brief : 交换pokemon 
     */
    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        bool currentPokemonFainted = true;
        if (playerUnit.pokemon.HP > 0)
        {
            currentPokemonFainted = false;
            yield return dialogBox.TypeDialog($"快撤! {playerUnit.pokemon.Base.Name}");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }
        playerUnit.Setup(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"顶住啊 {newPokemon.Base.Name}!");

        if (currentPokemonFainted)
        {
            ChooseFirstTurn();
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    // 处理技能选择
    private void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerUnit.pokemon.Moves.Count - 1)
            {
                ++currentMove;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
            {
                --currentMove;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < playerUnit.pokemon.Moves.Count - 2)
            {
                currentMove += 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1)
            {
                currentMove -= 2;
            }
        }
        dialogBox.UpdateMoveSelection(currentMove, playerUnit.pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PlayerMove());
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            // 返回上一步行动选取
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableActionSelector(true);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    // 展示技能的特殊信息
    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
        {
            yield return dialogBox.TypeDialog("会心一击!");
        }
        if(damageDetails.TypeEffectiveness > 1f)
        {
            yield return dialogBox.TypeDialog("效果拔群啊!");
        } else if (damageDetails.TypeEffectiveness < 1f)
        {
            yield return dialogBox.TypeDialog("收效甚微哎!");
        }
    }

    // 处理行动选择
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
                OpenPartyScreen();
            }else if (currentAction == 3)
            {
                // run
                OnBattleOver(true);
            }
        }
    }

    /*
     * brief : 打开pokemon包裹
     */
    private void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);
    }

    // 技能的处理，动画及造成伤害
    IEnumerator PlayerMove()
    {
        state = BattleState.PerformMove;
        var move = playerUnit.pokemon.Moves[currentMove];
        yield return RunMove(playerUnit, enemyUnit, move);

        if (state == BattleState.PerformMove)
        {
            StartCoroutine(EnemyMove());
        }
    }

    // 敌人的技能动画及造成伤害
    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;

        var move = enemyUnit.pokemon.GetRandomMove();
        yield return RunMove(enemyUnit, playerUnit, move);

        ActionSelection();
    }
    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        yield return dialogBox.TypeDialog($"{ sourceUnit.pokemon.Base.Name} 使用了{move.Base.Name}");
        move.PP--;
        sourceUnit.PlayerAttackAnimation();
        yield return new WaitForSeconds(1f);
        targetUnit.PlayHitAnimation();

        if(move.Base.Category == MoveCategory.Status)
        {
            yield return RunMoveEffects(move, sourceUnit.pokemon, targetUnit.pokemon);
        }
        else
        {
            var damageDetails = targetUnit.pokemon.TakeDamage(move, sourceUnit.pokemon);
            yield return (targetUnit.Hud.UpdateHP());
            yield return ShowDamageDetails(damageDetails);
        }


        if (targetUnit.pokemon.HP <= 0)
        {
            targetUnit.PlayFaintAnimation();
            yield return dialogBox.TypeDialog($"{targetUnit.pokemon.Base.Name}倒下了!");
            yield return new WaitForSeconds(2f);

            CheckForBattleOver(targetUnit);
        }
        sourceUnit.pokemon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.pokemon);
        yield return sourceUnit.Hud.UpdateHP();
        if (sourceUnit.pokemon.HP <= 0)
        {
            sourceUnit.PlayFaintAnimation();
            yield return dialogBox.TypeDialog($"{sourceUnit.pokemon.Base.Name}倒下了!");
            yield return new WaitForSeconds(2f);

            CheckForBattleOver(sourceUnit);
        }
    }

    IEnumerator RunMoveEffects(Move move, Pokemon source, Pokemon target)
    {
        var effects = move.Base.Effects;
        // Stat Boosting 各种buff
        if (move.Base.Effects.Boosts != null)
        {
            if (move.Base.Target == MoveTarget.Self)
            {
                source.ApplyBoosts(effects.Boosts);
            }
            else
            {
                target.ApplyBoosts(effects.Boosts);
            }
        }
        // Stat Condition 各种状态
        if (effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
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
