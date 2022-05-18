using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    // ���ⲿ��ȡ��ײ����
    [SerializeField] LayerMask solidObjectLayer;
    // ����npc
    [SerializeField] LayerMask interactableLayer;
    // ���ֳ��ݲ�
    [SerializeField] LayerMask longGrassLayer;
    [SerializeField] LayerMask playerLayer;


    public static GameLayers i { get; set; }
    private void Awake()
    {
        i = this;
    }
    public LayerMask SolidObjectLayer { get => solidObjectLayer; }
    public LayerMask InteractableLayer { get => interactableLayer; }
    public LayerMask LongGrassLayer { get => longGrassLayer; }
    public LayerMask PlayerLayer { get => playerLayer; }
}
