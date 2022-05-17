using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * brief 管理所有在某个区域内可能遇到的种类
 */
public class MapArea : MonoBehaviour
{
    [SerializeField] List<Pokemon> wildPokemons;

    public Pokemon GetRandomWildPokemon()
    {
        var wildpokemon =  wildPokemons[Random.Range(0, wildPokemons.Count)];
        wildpokemon.Init();
        return wildpokemon;
    }
}
