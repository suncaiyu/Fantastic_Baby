using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create pokemon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite fontSprite;
    [SerializeField] Sprite baskSprite;

    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    // »ù±¾×´Ì¬
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [SerializeField] List<LearnableMove> learnableMoves;

    public string Description { get => description; set => description = value; }
    public Sprite FontSprite { get => fontSprite; set => fontSprite = value; }
    public Sprite BaskSprite { get => baskSprite; set => baskSprite = value; }
    public PokemonType Type1 { get => type1; set => type1 = value; }
    public PokemonType Type2 { get => type2; set => type2 = value; }
    public int MaxHp { get => maxHp; set => maxHp = value; }
    public int Attack { get => attack; set => attack = value; }
    public int Defense { get => defense; set => defense = value; }
    public int SpAttack { get => spAttack; set => spAttack = value; }
    public int SpDefense { get => spDefense; set => spDefense = value; }
    public int Speed { get => speed; set => speed = value; }
    public string Name { get => name; set => name = value; }
    public List<LearnableMove> LearnableMoves { get => learnableMoves; set => learnableMoves = value; }
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public int Level { get => level; set => level = value; }
    public MoveBase MoveBase { get => moveBase; set => moveBase = value; }
}
public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Posion,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy
}

public enum Stat
{
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed,

    // these 2 are not actual stats, they're used to boost the move accuracy
    Accuracy,
    Evasion
}
public class TypeChart
{
    static float[][] chart =
    {                      /*      NOR   FIR       WAT      ELE       GRA       ICE       FIG       POI        gro     fly       psy        bug       roc         gho       dra        dar         ste        fai*/
        /*normal*/   new float[] { 1f  , 1f  ,      1f,     1f       ,1f,       1f,       1f,        1f,        1f,    1f,       1f,        1f,       0.5f,        0f,       1f,       0.5f,       1f,        1f},
        /*fire*/     new float[] { 1f  , 0.5f,    0.5f,    1f  ,      2f,       2f,       1f,        1f ,       1f,    1f,       1f,        2f,       0.5f,      1f,        0.5f,      1f,         2f,        1f},
        /*water*/    new float[] { 1f  , 2f  ,    0.5f,    2f,        0.5f,     2f,       1f,        1f,        2f,    1f,       1f,        1f,       2f,         1f,       0.5f,      1f,         1f,        1f},
        /*electric*/ new float[] { 1f  , 1f  ,    1f  ,    0.5f,      0.5f,    2f,        1f,         1f,        0f  ,  2f,      1f,       1f,        1f,         1f,       0.5f,      1f,         1f,        1f},
        /*grass*/    new float[] { 1f  , 0.5f,    2f  ,    2f,        0.5f,    1f,        1f,         0.5f     ,2f,    0.5f,     1f,       0.5f,      2f,        1f,       0.5f,      1f,         0.5f,      1f},
        /*ice*/      new float[] {1f,    0.5f,    0.5f,    1f,        2f,       0.5f,     1f,         1f,       2f,     2f,       1f,       1f,       1f,         1f,       2f,        1f,         1f,        1f},
        /*fight*/    new float[] {2f,     1f,     1f,      1f,        1f,       2f,       1f,       0.5f,       1f,     0.5f,     0.5f,     0.5f,     2f,         0f,      2f,        1f,         2f,         0.5f},
        /*poison*/   new float[] { 1f   ,1f  ,    1f  ,    1f,         2f,    1f,         1f,        1f ,       0.5f,    1f,     1f,        1f,       0.5f,      0.5f,     1f,        1f,        0f,         2f},
        /*ground*/   new float[] { 1f,   2f,     1f,       2f,         0.5f,     1f,      1f,       2f,         1f,     0f,      1f,        0.5f,     2f,       1f,        1f,         1f,       2f,         1f},
        /*flying*/   new float[]{ 1f,    1f,     1f,        0.5f,      2f,      1f,        2f,      1f,      1f,        1f,      1f,       2f,         0.5f,      1f,      1f,         1f,       0.5f,       1f},              
        /*psychic*/  new float[]{ 1f,    1f,      1f,       1f,       1f,       1f,       2f,       2f,       1f,       1f,       0.5f,    1f,       1f,          1f,       1f,        0f,        0.5f,      1f},              
        /*bug*/      new float[]{ 1f,    0.5f,    1f,       1f,       2f,       1f,       0.5f,      0.5f,     1f,     0.5f,      2f,      1f,       1f,          0.5f,      1f,      2f,        0.5f,       0.5f},              
        /*rock*/     new float[]{ 1f,    2f,      1f,       1f,       1f,       2f,       0.5f,      1f,        0.5f,    2f,     1f,       2f,       1f,         1f,         1f,      1f,        0.5f,       1f},              
        /*ghost*/    new float[]{ 0f,    1f,      1f,      1f,        1f,       1f,       1f,       1f,        1f,       1f,     2f,       1f,       1f,         2f,        1f,       0.5f,        1f,        1f},              
        /*dragon*/   new float[]{ 1f,     1f,      1f,      1f,       1f,       1f,       1f,        1f,       1f,       1f,      1f,      1f,       1f,         1f,        2f,        1f,        0.5f,       0f},              
        /*dark*/     new float[]{ 1f,     1f,      1f,      1f,      1f,        1f,      0.5f,     1f,        1f,       1f,       2f,      1f,       1f,        2f,        1f,        0.5f,        1f,       0.5f },              
        /*steel*/    new float[]{ 1f,     0.5f,    0.5f,    0.5f,     1f,       2f,      1f,       1f,        1f,       1f,       1f,       1f,      2f,         1f,         1f,        1f,      0.5f,         2f},              
        /*fairy*/    new float[]{ 1f,     0.5f,    1f,      1f,       1f,       1f,      2f,       0.5f,      1f,        1f,      1f,       1f,      1f,         1f,        2f,        2f,         0.5f,      1f}               
    };

    public static float GetEffectiveness(PokemonType attakType, PokemonType defenseType)
    {
        if (attakType == PokemonType.None || defenseType == PokemonType.None)
        {
            return 1;
        }
        int row = (int)attakType - 1;
        Debug.Log("Attack" + row);
        int col = (int)defenseType - 1;
        Debug.Log("Defense" + row);
        return chart[row][col];
    }
}