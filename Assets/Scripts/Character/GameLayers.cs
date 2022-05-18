using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    // 从外部获取碰撞检测层
    [SerializeField] LayerMask solidObjectLayer;
    // 交互npc
    [SerializeField] LayerMask interactableLayer;
    // 遇怪长草层
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
