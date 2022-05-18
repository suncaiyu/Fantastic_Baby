using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

// 战斗的pokemon对象类
public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHud hud;

    public BattleHud Hud { get { return hud; } }
    public bool IsPlayerUnit{get;set;}
    public Pokemon pokemon { get; set; }
    Image image;
    Vector3 originalPos;
    Color originColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        originColor = image.color;
    }

    /*
     * brief : 初始化pokemon外形
     */
    public void Setup(Pokemon po)
    {
        pokemon =  po;
        if (isPlayerUnit)
        {
            image.sprite = pokemon.Base.BaskSprite;
        } else
        {
            image.sprite = pokemon.Base.FontSprite;
        }
        hud.SetData(pokemon);
        image.color = originColor;
        PlayEnterAnimation();
    }

    // 进场动画
    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
        {
            image.transform.localPosition = new Vector3(-500f, originalPos.y);
        }
        else
        {
            image.transform.localPosition = new Vector3(500f, originalPos.y);
        }
        image.transform.DOLocalMoveX(originalPos.x, 1f);
    }
    
    // 攻击动画
    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
        {
           sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
        }
        else
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));
        }
        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));
    }

    // 受击动画
    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originColor, 0.1f));
    }

    // 离场动画
    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }
}
