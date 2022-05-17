using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 战斗信息显示，技能选择面板类
public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] Text dialogText;
    [SerializeField] int letterPerSecond;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;

    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> moveTexts;

    [SerializeField] Text ppText;
    [SerializeField] Text typeText;
    [SerializeField] Color highlightColor;

    /*
    * brief : 显示信息
    */
    public void SetDialog(string text)
    {
        dialogText.text = text;
    }

    /*
    * brief : 平缓显示信息
    */
    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / letterPerSecond);
        }
        yield return new WaitForSeconds(1f);
    }


    /*
    * brief : 使能信息展示面板
    */
    public void EnableDialogText(bool enable)
    {
        dialogText.enabled = enable;
    }

    /*
    * brief : 使能行动选择面板
    */
    public void EnableActionSelector(bool enable)
    {
        actionSelector.SetActive(enable);
    }

    /*
     * brief : 使能技能选择面板
     */
    public void EnableMoveSelector(bool enable)
    {
        moveSelector.SetActive(enable);
        moveDetails.SetActive(enable);
    }

    /*
    * brief : 更新选择行动的ui显示
    * param : selectedAction 选的第几个行动高亮
    */
    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; ++i)
        {
            if (i == selectedAction)
            {
                actionTexts[i].color = highlightColor;
            } else
            {
                actionTexts[i].color = Color.black;
            }
        }
    }

    /*
     * brief : 更新选择技能的ui显示
     * param : selectedMove 选的第几个技能高亮
     * param : move 选中的那个技能
     */
    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < moveTexts.Count; ++i)
        {
            if (i == selectedMove)
            {
                moveTexts[i].color = highlightColor;
            }
            else
            {
                moveTexts[i].color = Color.black;
            }
        }
        ppText.text = $"{move.PP} / {move.Base.Pp}";
        typeText.text = move.Base.Type.ToString();
    }

    /*
     * brief : 在面板中初始化技能名
     * param : 传过来的技能列表
     */
    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < moveTexts.Count; ++i)
        {
            if (i < moves.Count)
            {
                moveTexts[i].text = moves[i].Base.Name;
            } else
            {
                moveTexts[i].text = "-";
            }
        }
    }
}
