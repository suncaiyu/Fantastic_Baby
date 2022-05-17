using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField ] Text messageText;
    PartyMemberUI[] memberSlots;
    List<Pokemon> pokemons;
    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Pokemon> pokemons)
    {
        this.pokemons = pokemons;
        for (int i = 0; i < memberSlots.Length; ++i)
        {
            if (i < pokemons.Count)
            {
                memberSlots[i].SetData(pokemons[i]);
            } else
            {
                memberSlots[i].gameObject.SetActive(false);
            }

        }
        messageText.text = "Ñ¡Ò»¸ö";
    }

    public void UpdateMemberSelection(int index)
    {
        for(int i = 0; i < pokemons.Count; ++i)
        {
            if (i == index)
            {
                memberSlots[i].SetSelected(true);
            } else
            {
                memberSlots[i].SetSelected(false);
            }
        }
    }

    public void SetMessageText(string msg)
    {
        messageText.text = msg;
    }
}
