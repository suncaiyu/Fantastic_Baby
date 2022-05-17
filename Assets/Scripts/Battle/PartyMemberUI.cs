using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;
    [SerializeField] Color hightlightColor;

    Pokemon _pokemon;

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        nameText.text = pokemon.Base.Name;
        levelText.text = "Lvl" + pokemon.Level;
        Debug.Log(pokemon.HP / pokemon.MaxHp);
        hpBar.SetHP((float)pokemon.HP / pokemon.MaxHp);
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            nameText.color = hightlightColor;
        } else
        {
            nameText.color = Color.black;
        }
    }
}
