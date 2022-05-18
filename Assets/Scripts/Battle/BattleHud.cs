using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 战斗pokemon信息展示面板类
public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    public Text statusText;
    [SerializeField] HPBar hpBar;
    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color slpColor;
    [SerializeField] Color parColor;
    [SerializeField] Color frzColor;

    Pokemon _pokemon;

    Dictionary<ConditionID, Color> statusColors;

    /*
     * brief : 初始化展示的pokemon信息(name lv hp)
     */
    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        nameText.text = pokemon.Base.Name;
        levelText.text = "Lvl" + pokemon.Level;
        hpBar.SetHP((float)pokemon.HP / pokemon.MaxHp);

        statusColors = new Dictionary<ConditionID, Color> {
            { ConditionID.psn, psnColor},
            { ConditionID.slp, slpColor},
            { ConditionID.brn, brnColor},
            { ConditionID.frz, frzColor},
            { ConditionID.par, parColor}
        };

        SetStatusText();
        _pokemon.OnStatusChanged += SetStatusText;
    }

    void SetStatusText()
    {
        if (_pokemon.Status == null)
        {
            statusText.text = "";
        } else
        {
            statusText.text = _pokemon.Status.Name;
            statusText.color = statusColors[_pokemon.Status.Id];
        }
    }

    /*
     * brief : 更新血量
     */
    public IEnumerator UpdateHP()
    {
        if (_pokemon.HpChanged)
        {
            yield return hpBar.SetHPSmooth((float)_pokemon.HP / _pokemon.MaxHp);
            _pokemon.HpChanged = false;
        }
    }
}
