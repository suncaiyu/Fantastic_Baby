using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ս����Ϣ��ʾ������ѡ�������
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
    * brief : ��ʾ��Ϣ
    */
    public void SetDialog(string text)
    {
        dialogText.text = text;
    }

    /*
    * brief : ƽ����ʾ��Ϣ
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
    * brief : ʹ����Ϣչʾ���
    */
    public void EnableDialogText(bool enable)
    {
        dialogText.enabled = enable;
    }

    /*
    * brief : ʹ���ж�ѡ�����
    */
    public void EnableActionSelector(bool enable)
    {
        actionSelector.SetActive(enable);
    }

    /*
     * brief : ʹ�ܼ���ѡ�����
     */
    public void EnableMoveSelector(bool enable)
    {
        moveSelector.SetActive(enable);
        moveDetails.SetActive(enable);
    }

    /*
    * brief : ����ѡ���ж���ui��ʾ
    * param : selectedAction ѡ�ĵڼ����ж�����
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
     * brief : ����ѡ���ܵ�ui��ʾ
     * param : selectedMove ѡ�ĵڼ������ܸ���
     * param : move ѡ�е��Ǹ�����
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
     * brief : ������г�ʼ��������
     * param : �������ļ����б�
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
